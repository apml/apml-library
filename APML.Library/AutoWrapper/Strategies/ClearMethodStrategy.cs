using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace APML.AutoWrapper.Strategies {
  public class ClearMethodStrategy : BaseStrategy, IMethodStrategy {
    #region IStrategy Members
    /// <summary>
    /// Defines the priority of the strategy.
    /// </summary>
    public override StrategyPriority Priority {
      get { return StrategyPriority.BaseCode; }
    }
    #endregion

    #region IMethodStrategy Members
    /// <summary>
    /// Checks whether the given strategy applies to a property by checking if the method name start with "Add".
    /// </summary>
    /// <param name="pMethod">the method to check</param>
    /// <returns>true - strategies apply</returns>
    public bool AppliesToMethod(MethodInfo pMethod) {
      return pMethod.Name.StartsWith("Clear");
    }

    /// <summary>
    /// Applies the given strategy to the given generated method by creating an "Add" method which accepts the given
    /// parameters.
    /// </summary>
    /// <param name="pContext">the context of the generation</param>
    /// <param name="pMethod">the declared method</param>
    /// <param name="pGeneratedMethod">the method being generated</param>
    /// <param name="pClass">the class being declared</param>
    public void Apply(GenerationContext pContext, MethodInfo pMethod, CodeMemberMethod pGeneratedMethod,
                      CodeTypeDeclaration pClass) {
      if (pMethod.ReturnType != typeof(void)) {
        throw new ArgumentException(pMethod.Name + " must return void");
      }

      // Find the property, and see if it has a key value
      PropertyInfo prop = MethodHelper.FindRelevantProperty(pMethod, "Clear");
      if (prop == null) {
        throw new ArgumentException("No matching property found for method " + pClass.Name + "." + pMethod.Name);
      }
//      AutoWrapperKeyAttribute keyAttr = AttributeHelper.GetAttribute<AutoWrapperKeyAttribute>(prop);
      ParameterInfo[] clearParams = pMethod.GetParameters();

      // Ensure that the cache is built
      pGeneratedMethod.Statements.Add(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "EnsureCacheFor" + prop.Name));
       
      // Generate our support method
      Type elementType = TypeHelper.GetElementType(prop.PropertyType);
      CodeMemberMethod checkMethod = new CodeMemberMethod();
      checkMethod.Name = GenerateMethodName(pMethod);
      checkMethod.Attributes = MemberAttributes.Public;
      checkMethod.Parameters.Add(new CodeParameterDeclarationExpression(elementType, "item"));
      checkMethod.ReturnType = new CodeTypeReference(typeof (bool));

      // Build a statement that returns true if all property/param combinations match
      CodeExpression allTestOps = null;
      PropertyInfo[] clearProperties = MethodHelper.MatchPropertiesForParams(pMethod, elementType);
      for (int i = 0; i < clearParams.Length; ++i) {
        CodeBinaryOperatorExpression testOp = new CodeBinaryOperatorExpression(
          new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("item"), clearProperties[i].Name),
          CodeBinaryOperatorType.IdentityEquality,
          new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), clearParams[i].Name));

        if (allTestOps == null) {
          allTestOps = testOp;
        } else {
          allTestOps = new CodeBinaryOperatorExpression(allTestOps, CodeBinaryOperatorType.BitwiseAnd, testOp);
        }
      }
      if (allTestOps == null) {
        allTestOps = new CodePrimitiveExpression(true);
      }
      checkMethod.Statements.Add(new CodeMethodReturnStatement(allTestOps));

      CodeDelegateCreateExpression testDelegateRef;
      if (clearParams.Length > 0) {
        // Generate our support check type
        CodeTypeDeclaration innerCheckType = new CodeTypeDeclaration(GenerateInnerTypeName(pMethod));
        innerCheckType.Attributes = MemberAttributes.Private;
        
        // Generate the constructor, and the call to it
        CodeConstructor constructor = new CodeConstructor();
        constructor.Attributes = MemberAttributes.Public;
        CodeObjectCreateExpression innerTypeCreate = new CodeObjectCreateExpression(innerCheckType.Name);

        // Add fields for each parameter, then the method for the check
        foreach (ParameterInfo clearParam in clearParams) {
          innerCheckType.Members.Add(new CodeMemberField(clearParam.ParameterType, clearParam.Name));
          constructor.Parameters.Add(new CodeParameterDeclarationExpression(clearParam.ParameterType, clearParam.Name));
          constructor.Statements.Add(new CodeAssignStatement(new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), clearParam.Name),
            new CodeVariableReferenceExpression(clearParam.Name)));
          innerTypeCreate.Parameters.Add(new CodeVariableReferenceExpression(clearParam.Name));
        }
        innerCheckType.Members.Add(constructor);
        innerCheckType.Members.Add(checkMethod);

        // Add the check type to the class
        pClass.Members.Add(innerCheckType);
        
        // Add the declaration for the delegate container
        pGeneratedMethod.Statements.Add(new CodeVariableDeclarationStatement(innerCheckType.Name, "delegateContainer", innerTypeCreate));

        testDelegateRef = new CodeDelegateCreateExpression(
          new CodeTypeReference(prop.Name + "WalkCallbackDelegate"), 
          new CodeVariableReferenceExpression("delegateContainer"), checkMethod.Name);
      } else {
        // Just put the delegate directly in the class, since we don't need to pass it any parameters
        pClass.Members.Add(checkMethod);

        testDelegateRef = new CodeDelegateCreateExpression(
          new CodeTypeReference(prop.Name + "WalkCallbackDelegate"), new CodeThisReferenceExpression(), checkMethod.Name);
      }
      
      // Invoke the walk method for it, and pass our removal statement
      CodeExpression walkInvoke = new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), "Walk" + prop.Name, testDelegateRef);
      pGeneratedMethod.Statements.Add(walkInvoke);
    }

    #endregion

    private string GenerateMethodName(MethodInfo pMethod) {
      return pMethod.Name + "_Check";
    }

    private string GenerateInnerTypeName(MethodInfo pMethod) {
      return pMethod.Name + "_CheckContainer";
    }
  }

  
}
