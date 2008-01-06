using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using APML.AutoWrapper.Strategies;
using Microsoft.CSharp;

namespace APML.AutoWrapper {
  /// <summary>
  /// Utility class for generating XML wrappers that provide strongly typed access to underlying XML.
  /// </summary>
  public class AutoWrapperGenerator {
    /// <summary>
    /// Namespace that all autowrapper classes will be generated into.
    /// </summary>
    internal const string GENERATED_NAMESPACE = "APML.AutoWrapper.Generated";

    /// <summary>
    /// The property strategies being used.
    /// </summary>
    private readonly List<IPropertyStrategy> mPropStrategies;

    /// <summary>
    /// The method strategies being used.
    /// </summary>
    private readonly List<IMethodStrategy> mMethodStrategies;

    /// <summary>
    /// The map of already generated types.
    /// </summary>
    private readonly IDictionary<Type, Type> mGeneratedTypes;

    /// <summary>
    /// Creates a new AutoWrapperGenerator.
    /// </summary>
    public AutoWrapperGenerator() {
      mGeneratedTypes = new Dictionary<Type, Type>();

      mPropStrategies = new List<IPropertyStrategy>();
      mPropStrategies.Add(new AttributeStrategy());
      mPropStrategies.Add(new PrimitiveElementStrategy());
      mPropStrategies.Add(new ComplexElementStrategy());
      mPropStrategies.Add(new SequenceArrayPropertyStrategy());
      mPropStrategies.Add(new SequenceListPropertyStrategy());
      mPropStrategies.Add(new SequenceDictionaryPropertyStrategy());
      mPropStrategies.Add(new SequenceDictionaryOfListsPropertyStrategy());

      mMethodStrategies = new List<IMethodStrategy>();
      mMethodStrategies.Add(new InitMethodStrategy());
      mMethodStrategies.Add(new AddMethodStrategy());
    }

    /// <summary>
    /// Generates a wrapper instance for the given node.
    /// </summary>
    /// <typeparam name="T">the type of the node</typeparam>
    /// <param name="pNode">the xml node being wrapped</param>
    /// <returns>the node wrapper instance</returns>
    public T GenerateWrapper<T>(XmlNode pNode) where T : class {
      if (!mGeneratedTypes.ContainsKey(typeof(T))) {
        Generate(typeof (T)); 
      }

      return Instantiate<T>(mGeneratedTypes[typeof(T)], pNode);
    }

    /// <summary>
    /// Instantiates the given type, providing it with the given node.
    /// </summary>
    /// <param name="pAutoWrapperType">the wrapper node to instantiate</param>
    /// <param name="pNode">the node to provide it</param>
    /// <returns>an instance of the given object</returns>
    private T Instantiate<T>(Type pAutoWrapperType, XmlNode pNode) {
      ConstructorInfo c = pAutoWrapperType.GetConstructor(new Type[] { typeof(XmlElement), typeof(AutoWrapperGenerator) });

      return (T) c.Invoke(new object[] { pNode, this });
    }

    /// <summary>
    /// Generates a wrapper for the given type.
    /// </summary>
    /// <param name="pType">the interface that the wrapper should support</param>
    private void Generate(Type pType) {
      // Create the namespace and the compile unit
      CodeNamespace ns = new CodeNamespace(GENERATED_NAMESPACE);
      CodeCompileUnit unit = new CodeCompileUnit();
      unit.Namespaces.Add(ns);

      // Create a queue of types to generate, and seed it with the initial item
      GenerationContext context = new GenerationContext(pType);
      while (context.GenerationRequired) {
        Type next = context.DequeueNextForGeneration();
        string nextName = context.LookupRequiredTypeName(next);

        Generate(context, unit, ns, nextName, next);
      }
      
      // Compile the assembly
      CSharpCodeProvider csp = new CSharpCodeProvider();
      CompilerParameters compileParams = new CompilerParameters();
      CompilerResults result = csp.CompileAssemblyFromDom(compileParams, unit);
      if (result.Errors.HasErrors) {
        foreach (CompilerError error in result.Errors) {
          Debug.WriteLine(error);
        }

        throw new Exception("Failed to generate XML AutoWrapper");
      }

      // Add each of the generated types into the generated type map
      foreach (KeyValuePair<Type, string> generatedType in context.GeneratedTypes) {
        mGeneratedTypes[generatedType.Key] = result.CompiledAssembly.GetType(ns.Name + "." + generatedType.Value);
      }
    }

    /// <summary>
    /// Generates a wrapper for the given type.
    /// </summary>
    /// <param name="pContext">the context of this generation operation</param>
    /// <param name="pUnit">the code compile unit to add the wrapper to</param>
    /// <param name="pNs">the namespace to add the unit to</param>
    /// <param name="pName">the name to give the generated object</param>
    /// <param name="pTypes">the interfaces that the wrapper should support</param>
    /// <returns>the generated wrapper</returns>
    private void Generate(GenerationContext pContext, CodeCompileUnit pUnit, CodeNamespace pNs, string pName, params Type[] pTypes) {
      // Create the type, connect it to the interface and put it in the namespace
      CodeTypeDeclaration clsDecl = new CodeTypeDeclaration(pName);
      clsDecl.IsClass = true;
      clsDecl.BaseTypes.Add(typeof(AutoWrapperBase));
      pNs.Types.Add(clsDecl);
      EnsureAssembly(pUnit, typeof(AutoWrapperBase));
      EnsureAssembly(pUnit, typeof(XmlElement));

      // Add a constructor that delegates to the parent class
      CodeParameterDeclarationExpression dParam = new CodeParameterDeclarationExpression(typeof(XmlElement), "element");
      CodeParameterDeclarationExpression gParam = new CodeParameterDeclarationExpression(typeof(AutoWrapperGenerator), "generator");
      CodeConstructor clsConstructor = new CodeConstructor();
      clsConstructor.Attributes = MemberAttributes.Public;
      clsConstructor.Parameters.Add(dParam);
      clsConstructor.Parameters.Add(gParam);
      clsConstructor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("element"));
      clsConstructor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("generator"));
      clsDecl.Members.Add(clsConstructor);

      foreach (Type type in pTypes) {
        clsDecl.BaseTypes.Add(type);
        EnsureAssembly(pUnit, type);

        // Generate all of the properties
        GenerateProperties(pContext, clsDecl, pUnit, type);

        // Generate all of the methods
        GenerateMethods(pContext, clsDecl, pUnit, type);
      }

#if DEBUG
      CSharpCodeProvider csp = new CSharpCodeProvider();

      // Write out the generated code
      StreamWriter sw = new StreamWriter(string.Format("{0}.{1}.cs", pNs.Name, pName));
      csp.GenerateCodeFromCompileUnit(pUnit, sw, new CodeGeneratorOptions());
      sw.Close();
#endif
    }

    /// <summary>
    /// Generates the implementations for the properties in the given class.
    /// </summary>
    /// <param name="pContext">the context of this generation operation</param>
    /// <param name="pClass">the class being generated</param>
    /// <param name="pUnit">the codecompile unit the class belongs to</param>
    /// <param name="pType">the type being wrapped</param>
    private void GenerateProperties(GenerationContext pContext, CodeTypeDeclaration pClass, CodeCompileUnit pUnit, Type pType) {
      PropertyInfo[] props = pType.GetProperties();
      foreach (PropertyInfo prop in props) {
        // Determine the strategy to use
        IPropertyStrategy[] strategies = SelectPropertyStrategies(prop);
        
        // Build the property
        CodeMemberProperty cProp = new CodeMemberProperty();
        cProp.Name = prop.Name;
        cProp.Type = new CodeTypeReference(prop.PropertyType);
        cProp.Attributes = MemberAttributes.Public;

        // Run each of the strategies
        foreach (IPropertyStrategy strategy in strategies) {
          strategy.Apply(pContext, prop, cProp, pClass);
        }

        // Add the property, and ensure the type's assembly is listed
        pClass.Members.Add(cProp);
        EnsureAssembly(pUnit, prop.PropertyType);
      }
    }

    /// <summary>
    /// Generates the implementations for the methods in the given class.
    /// </summary>
    /// <param name="pContext">the context of this generation operation</param>
    /// <param name="pClass">the class being generated</param>
    /// <param name="pUnit">the codecompile unit the class belongs to</param>
    /// <param name="pType">the type being wrapped</param>
    private void GenerateMethods(GenerationContext pContext, CodeTypeDeclaration pClass, CodeCompileUnit pUnit, Type pType) {
      MethodInfo[] methods = pType.GetMethods();
      foreach (MethodInfo method in methods) {
        // Work out if we actually want to complete this method
        if (method.IsAbstract && !IsMethodGenerated(pClass, method)) {
          // Determine the strategy to use
          IMethodStrategy[] strategies = SelectMethodStrategies(method);

          // Build the method
          CodeMemberMethod cMethod = new CodeMemberMethod();
          cMethod.Name = method.Name;
          cMethod.ReturnType = new CodeTypeReference(method.ReturnType);
          foreach (ParameterInfo paramInfo in method.GetParameters()) {
            cMethod.Parameters.Add(new CodeParameterDeclarationExpression(paramInfo.ParameterType, paramInfo.Name));
            EnsureAssembly(pUnit, paramInfo.ParameterType);
          }
          cMethod.Attributes = MemberAttributes.Public;

          // Run each of the strategies
          foreach (IMethodStrategy strategy in strategies) {
            strategy.Apply(pContext, method, cMethod, pClass);
          }

          // Add the property, and ensure the type's assembly is listed
          pClass.Members.Add(cMethod);
          EnsureAssembly(pUnit, method.ReturnType);
        }
      }
    }

    /// <summary>
    /// Ensures that the type of the given assembly is in the code compile unit.
    /// </summary>
    /// <param name="pUnit">the code compile unit being gathered</param>
    /// <param name="pType">the type to check the assembly of</param>
    private static void EnsureAssembly(CodeCompileUnit pUnit, Type pType) {
      if (!pUnit.ReferencedAssemblies.Contains(pType.Assembly.Location)) {
        pUnit.ReferencedAssemblies.Add(pType.Assembly.Location);
      }
    }

    /// <summary>
    /// Determines whether the given method has already been generated.
    /// </summary>
    /// <param name="pClass">the generated class</param>
    /// <param name="pMethod">the method to be checked for</param>
    /// <returns>true - a definition already exists</returns>
    private static bool IsMethodGenerated(CodeTypeDeclaration pClass, MethodInfo pMethod) {
      foreach (CodeTypeMember member in pClass.Members) {
        if (member is CodeMemberProperty) {
          if (pMethod.Name.Equals("get_" + member.Name) && pMethod.GetParameters().Length == 0) {
            return true;
          } else if (pMethod.Name.Equals("set_" + member.Name) && pMethod.GetParameters().Length == 1) {
            return true;
          }
        } else if (member is CodeMemberMethod) {
          CodeMemberMethod cMethod = (CodeMemberMethod) member;
          if (IsMatchingMethod(pMethod, cMethod)) {
            return true;
          }
        }
      }

      return false;
    }

    /// <summary>
    /// Determines whether the given method matches the generated method.
    /// </summary>
    /// <param name="pMethod">the interface method</param>
    /// <param name="pGeneratedMethod">the generated method</param>
    /// <returns>true - the generated method matches</returns>
    private static bool IsMatchingMethod(MethodInfo pMethod, CodeMemberMethod pGeneratedMethod) {
      if (!pMethod.Name.Equals(pGeneratedMethod.Name)) {
        return false;
      }
      ParameterInfo[] parameters = pMethod.GetParameters();
      if (parameters.Length != pGeneratedMethod.Parameters.Count) {
        return false;
      }
      for (int i = 0; i < parameters.Length; ++i) {
        if (parameters[i].ParameterType.FullName != pGeneratedMethod.Parameters[i].Type.BaseType) {
          return false;
        }
      }

      return true;
      
    }

    /// <summary>
    /// Selects the strategies to be used for generating the code used for properties.
    /// </summary>
    /// <param name="pProp">the property being generated</param>
    private IPropertyStrategy[] SelectPropertyStrategies(PropertyInfo pProp) {
      List<IPropertyStrategy> result = new List<IPropertyStrategy>();

      foreach (IPropertyStrategy propStategy in mPropStrategies) {
        if (propStategy.AppliesToProperty(pProp)) {
          result.Add(propStategy);
        }
      }
      if (result.Count == 0) {
        throw new ArgumentException("Property " + pProp.Name + " does not have any recognised attributes");
      }

      // Sort the strategies
      result.Sort(StrategySorter);

      return result.ToArray();
    }

    /// <summary>
    /// Selects the strategies to be used for generating the code used for methods.
    /// </summary>
    /// <param name="pMethod">the method being generated</param>
    private IMethodStrategy[] SelectMethodStrategies(MethodInfo pMethod) {
      List<IMethodStrategy> result = new List<IMethodStrategy>();

      foreach (IMethodStrategy methodStrategy in mMethodStrategies) {
        if (methodStrategy.AppliesToMethod(pMethod)) {
          result.Add(methodStrategy);
        }
      }
      if (result.Count == 0) {
        throw new ArgumentException("Method " + pMethod.Name + " does not have any recognised attributes");
      }

      // Sort the strategies
      result.Sort(StrategySorter);

      return result.ToArray();
    }

    /// <summary>
    /// Sorts strategies based on their priorities.
    /// </summary>
    /// <param name="pFirst">the first strategy</param>
    /// <param name="pSecond">the second strategy</param>
    /// <returns>the sort order</returns>
    private static int StrategySorter(IStrategy pFirst, IStrategy pSecond) {
      return pFirst.Priority.CompareTo(pSecond.Priority);
    }
  }
}
