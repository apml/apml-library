using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Xml;

namespace APML.AutoWrapper {
  /// <summary>
  /// Helper class with various methods for generating support methods.
  /// </summary>
  public static class MethodHelper {
    /// <summary>
    /// Generates a method for clearing an element.
    /// </summary>
    /// <param name="pProp">the property this clear method is related to</param>
    /// <param name="pClass">the class being generated</param>
    public static void GenerateClearMethod(MemberInfo pProp, CodeTypeDeclaration pClass) {
      CodeMemberMethod method = new CodeMemberMethod();
      method.Name = "Clear" + pProp.Name;
      method.Attributes = MemberAttributes.Public;
      method.Statements.Add(
        new CodeMethodInvokeExpression(
          new CodeBaseReferenceExpression(), 
          "ClearElement", 
          new CodePrimitiveExpression(AttributeHelper.SelectXmlElementName(pProp)),
          new CodePrimitiveExpression(AttributeHelper.SelectXmlElementNamespace(pProp))));
      pClass.Members.Add(method);
    }

    ///<summary>
    /// Generate a method for initialising an element.
    ///</summary>
    ///<param name="pProp">the property this init method is related to</param>
    ///<param name="pClass">the class being generated</param>
    public static void GenerateBaseInitMethod(PropertyInfo pProp, CodeTypeDeclaration pClass) {
      GenerateBaseInitMethod(pProp, pClass, false, null);
    }

    ///<summary>
    /// Generate a method for initialising an element.
    ///</summary>
    ///<param name="pProp">the property this init method is related to</param>
    ///<param name="pClass">the class being generated</param>
    ///<param name="pShouldInitCache">whether the method should init the cache</param>
    ///<param name="pStoreDelegate">delegate for storing into the cache</param>
    public static void GenerateBaseInitMethod(PropertyInfo pProp, CodeTypeDeclaration pClass, bool pShouldInitCache, GenerateCacheStoreDelegate pStoreDelegate) {
      CodeMemberMethod method = new CodeMemberMethod();
      method.Name = "Init" + pProp.Name;
      method.Attributes = MemberAttributes.Family;

      CodeMethodInvokeExpression initInvoke = new CodeMethodInvokeExpression(
        new CodeBaseReferenceExpression(),
        "InitElement",
        new CodePrimitiveExpression(AttributeHelper.SelectXmlElementName(pProp)),
        new CodePrimitiveExpression(AttributeHelper.SelectXmlElementNamespace(pProp)));
      if (pShouldInitCache) {
        method.Statements.AddRange(pStoreDelegate(initInvoke));
      } else {
        method.Statements.Add(initInvoke);
      }
      pClass.Members.Add(method);
    }

    /// <summary>
    /// Finds the property that a given method applies to by removing the given name prefix and finding a
    /// property with the name.
    /// </summary>
    /// <param name="pMethod">the method to find the property for</param>
    /// <param name="pMethodNamePrefix">the prefix on the method</param>
    /// <returns>the relevant property, or null if the property wasn't found</returns>
    public static PropertyInfo FindRelevantProperty(MethodInfo pMethod, string pMethodNamePrefix) {
      string propName = pMethod.Name.Substring(pMethodNamePrefix.Length);
      
      // Check for the direct version
      PropertyInfo prop = pMethod.DeclaringType.GetProperty(propName);
      if (prop != null) {
        return prop;
      }

      // See if we have a plural version
      string pluralPropName = propName + "s";
      PropertyInfo pluralProp = pMethod.DeclaringType.GetProperty(pluralPropName);
      if (pluralProp != null) {
        return pluralProp;
      }

      return null;
    }

    /// <summary>
    /// Generates a series of assignment statements that assign the parameters provided in a method to
    /// properties on the result being generated.
    /// </summary>
    /// <param name="pMethod">the method that an implementation is being generated for</param>
    /// <param name="pPropertyType">the type of the relevant property</param>
    /// <returns>the statements to use</returns>
    /// <param name="pTarget">the target object for the assignments</param>
    public static CodeStatement[] GeneratePropertyApplications(MethodInfo pMethod, Type pPropertyType, CodeExpression pTarget) {
      ParameterInfo[] methodParams = pMethod.GetParameters();
      CodeStatement[] result = new CodeStatement[methodParams.Length];
      PropertyInfo[] paramProps = MatchPropertiesForParams(pMethod, pPropertyType);

      // For each parameter, find a property that matches
      for (int i = 0; i < result.Length; ++i) {
        // We've found the property that we want
        result[i] = new CodeAssignStatement(
          new CodePropertyReferenceExpression(
            pTarget,
            paramProps[i].Name),
          new CodeVariableReferenceExpression(methodParams[i].Name));
      }

      return result;
    }

    /// <summary>
    /// Finds the relevant properties for the parameters in the given method.
    /// </summary>
    /// <param name="pMethod">the method the parameters should be matched for</param>
    /// <param name="pPropertyType">the type of the property the method is working with</param>
    /// <returns>the relevant properties</returns>
    public static PropertyInfo[] MatchPropertiesForParams(MethodInfo pMethod, Type pPropertyType) {
      ParameterInfo[] methodParams = pMethod.GetParameters();
      PropertyInfo[] result = new PropertyInfo[methodParams.Length];

      // For each parameter, find a property that matches
      for (int i = 0; i < methodParams.Length; ++i) {
        foreach (PropertyInfo childProp in TypeHelper.GetAllProperties(pPropertyType)) {
          if (childProp.Name.ToLower() == methodParams[i].Name.ToLower()) {
            result[i] = childProp;
            break;
          }
        }

        if (result[i] == null) {
          throw new ArgumentException(methodParams[i].Name + " in " + pMethod.DeclaringType.FullName + "." +
                                      pMethod.Name + " could not be matched with a property in " +
                                      pPropertyType.FullName);
        }
      }

      return result;
    }

    ///<summary>
    /// Generates the expression used to access a cached version.
    ///</summary>
    ///<param name="pProp">the property that is cached</param>
    ///<returns>the expression to access the cache</returns>
    public static CodeExpression GenerateCacheExpression(PropertyInfo pProp) {
      return new CodeVariableReferenceExpression("m" + pProp.Name);
    }

    /// <summary>
    /// Finds the type that is used for elements for the property that the given method is related to. This is
    /// done by searching the generated class for a method with the same name as the current method that is public,
    /// and returns a type.
    /// </summary>
    /// <param name="pMethod">the method being generated and that an element type is required for</param>
    /// <param name="pGeneratedClass">the class being generated</param>
    /// <returns>the element type</returns>
    /*public static Type FindElementType(MethodInfo pMethod, CodeTypeDeclaration pGeneratedClass) {
      foreach (CodeTypeMember member in pGeneratedClass.Members) {
        if (member is CodeMemberMethod && member.Name == pMethod.Name) {
          // We've found a potential candidate. Check if it returns a type.
          CodeMemberMethod genMethod = (CodeMemberMethod) member;
          if (genMethod.ReturnType != null && genMethod.ReturnType.BaseType != typeof(void).FullName) {
            string typeName = genMethod.ReturnType.BaseType;

            return Type.GetType(typeName, true);
          }
        }
      }

      throw new ArgumentException("No matching property adder found for " + pMethod.Name);
    }*/

    public static CodeStatement GenerateCheckCacheAndReturnValue(PropertyInfo pProp, CodeExpression pCacheRef) {
      return GenerateCheckCacheAndReturnValue(pProp, pCacheRef, GenerateCacheReturnStatement(pProp));
    }

    public static CodeStatement GenerateCheckCacheAndReturnValue(PropertyInfo pProp, CodeExpression pCacheRef, CodeExpression pReturnValue) {
      return GenerateCheckCacheAndReturnValue(pProp, pCacheRef, new CodeMethodReturnStatement(pReturnValue));
    }

    public static CodeStatement GenerateCheckCacheAndReturnValue(PropertyInfo pProp, CodeExpression pCacheRef, CodeStatement pReturnValue) {
      return GenerateCacheGuardedStatements(pProp, pCacheRef, pReturnValue);
    }

    public static CodeStatement GenerateCacheGuardedStatements(PropertyInfo pProp, CodeExpression pCacheRef, params CodeStatement[] pCacheValidStatements) {
      return new CodeConditionStatement(
        new CodeBinaryOperatorExpression(pCacheRef, CodeBinaryOperatorType.IdentityInequality, new CodePrimitiveExpression(null)),
        pCacheValidStatements);
    }

    ///<summary>
    /// Generates the field for storing a cached item.
    ///</summary>
    ///<param name="pProp">the property being cached</param>
    ///<returns>a member definition for the cache</returns>
    public static CodeTypeMember GenerateCacheField(PropertyInfo pProp) {
      if (TypeHelper.RequiresNullableWrapperForNullCheck(pProp.PropertyType)) {
        return GenerateCacheField(pProp, typeof (Nullable<>).MakeGenericType(pProp.PropertyType));
      } else {
        return GenerateCacheField(pProp, pProp.PropertyType);
      }
    }

    ///<summary>
    /// Generates the field for storing a cached item.
    ///</summary>
    ///<param name="pProp">the property being cached</param>
    /// <param param name="pCacheType">the type that the cache should use</param>
    ///<returns>a member definition for the cache</returns>
    public static CodeTypeMember GenerateCacheField(PropertyInfo pProp, Type pCacheType) {
      return new CodeMemberField(pCacheType, "m" + pProp.Name);
    }

    ///<summary>
    /// Generates a statement that stores the given value into the cache.
    ///</summary>
    ///<param name="pProp">the property being cached</param>
    ///<param name="pValueExpr">an expression containing the new value</param>
    ///<returns>the statement performing the assignment</returns>
    public static CodeStatement GenerateCacheStoreStatement(PropertyInfo pProp, CodeExpression pValueExpr) {
      return new CodeAssignStatement(GenerateCacheExpression(pProp), pValueExpr);
    }

    ///<summary>
    /// Generates a statement that returns the cached valued.
    ///</summary>
    ///<param name="pProp">the property being generated</param>
    ///<returns>a statement for returning the cached value</returns>
    public static CodeMethodReturnStatement GenerateCacheReturnStatement(PropertyInfo pProp) {
      CodeExpression cacheRef = GenerateCacheExpression(pProp);
      if (TypeHelper.RequiresNullableWrapperForNullCheck(pProp.PropertyType)) {
        return new CodeMethodReturnStatement(new CodePropertyReferenceExpression(cacheRef, "Value"));
      } else {
        return new CodeMethodReturnStatement(cacheRef);
      }
    }

    

    /// <summary>
    /// Generates a method for retrieving the value of a simple parameter.
    /// </summary>
    /// <param name="pProp">the property being generated</param>
    /// <param name="pGetRetrieveDelegate">delegate for generating retrieve method</param>
    /// <returns>the statements necessary to implement the get method</returns>
    public static CodeStatement[] GenerateSimpleGetMethod(PropertyInfo pProp, GenerateValueRetrieveDelegate pGetRetrieveDelegate) {
      CodeExpression cacheRef = GenerateCacheExpression(pProp);
      CodeStatement cacheTest = GenerateCheckCacheAndReturnValue(pProp, cacheRef);
      DefaultValueAttribute defaultValue = AttributeHelper.GetAttribute<DefaultValueAttribute>(pProp);
      CodeExpression getInvoke;

      // See if we have a custom converter, and build the correct getter method based on it
      AutoWrapperFieldConverterAttribute converterAttr = AttributeHelper.GetAttribute<AutoWrapperFieldConverterAttribute>(pProp);
      if (converterAttr != null) {
        if (defaultValue != null) {
          throw new ArgumentException(
            "DefaultValue is not supported in conjunction with AutoWrapperFieldConverter. Found on " +
            pProp.DeclaringType.FullName + "." + pProp.Name);
        }
        Type converterType = converterAttr.ConverterType;
        Type requiredConverterType = typeof (IFieldConverter<>).MakeGenericType(pProp.PropertyType);
        if (!requiredConverterType.IsAssignableFrom(converterType)) {
          throw new ArgumentException("Converter for " + pProp.DeclaringType.FullName + "." + pProp.Name +
                                      " does not inherit from IFieldConverter<" +
                                      pProp.PropertyType + ">");
        }

        CodeExpression getStringInvoke = pGetRetrieveDelegate(typeof (string), null);
        getInvoke = new CodeMethodInvokeExpression(
          new CodeCastExpression(requiredConverterType, new CodeObjectCreateExpression(converterAttr.ConverterType)),
                                 "FromString",
                                 getStringInvoke);
      } else {
        getInvoke = pGetRetrieveDelegate(pProp.PropertyType, defaultValue != null ? defaultValue.Value : null);  
      }

      CodeStatement cacheStoreStmt = GenerateCacheStoreStatement(pProp, getInvoke);
      CodeMethodReturnStatement getStmt = GenerateCacheReturnStatement(pProp);

      return new CodeStatement[] { cacheTest, cacheStoreStmt, getStmt };
    }

    /// <summary>
    /// Generates a method for setting the value of a simple parameter.
    /// </summary>
    /// <param name="pProp">the property being generated</param>
    /// <param name="pSetRetrieveDelegate">delegate for generating store method</param>
    /// <returns>the statements necessary to implement the set method</returns>
    public static CodeStatement[] GenerateSimpleSetMethod(PropertyInfo pProp, GenerateValueStoreDelegate pSetRetrieveDelegate) {
      CodeExpression setInvoke;

      // See if we have a custom converter, and build the correct getter method based on it
      AutoWrapperFieldConverterAttribute converterAttr = AttributeHelper.GetAttribute<AutoWrapperFieldConverterAttribute>(pProp);
      if (converterAttr != null) {
        Type converterType = converterAttr.ConverterType;
        Type requiredConverterType = typeof(IFieldConverter<>).MakeGenericType(pProp.PropertyType);
        if (!requiredConverterType.IsAssignableFrom(converterType)) {
          throw new ArgumentException("Converter for " + pProp.DeclaringType.FullName + "." + pProp.Name +
                                      " does not inherit from IFieldConverter<" +
                                      pProp.PropertyType + ">");
        }

        CodeExpression convertToString = new CodeMethodInvokeExpression(
          new CodeCastExpression(requiredConverterType, new CodeObjectCreateExpression(converterAttr.ConverterType)),
                                 "ToString",
                                 new CodePropertySetValueReferenceExpression());
        setInvoke = pSetRetrieveDelegate(typeof(string), convertToString);
      } else {
        setInvoke = pSetRetrieveDelegate(pProp.PropertyType, new CodePropertySetValueReferenceExpression());
      }

      CodeExpressionStatement setStmt = new CodeExpressionStatement(setInvoke);

      return new CodeStatement[] { setStmt, GenerateCacheStoreStatement(pProp, new CodePropertySetValueReferenceExpression()) };
    }
  }

  /// <summary>
  /// Delegate provided to the getter method that will generate an expression that fetches a value of the given type.
  /// </summary>
  /// <param name="pRetrieveType">the type that is be being retrieved</param>
  /// <returns>the expression that will perform the retrieval</returns>
  /// <param name="pDefaultValue">the default value that should be used</param>
  public delegate CodeExpression GenerateValueRetrieveDelegate(Type pRetrieveType, object pDefaultValue);

  /// <summary>
  /// Delegate provided to the setter method that will generate an expression that stores a value for the given type.
  /// </summary>
  /// <remarks>
  /// This method accepts its parameter as an expression. However, it should be noted that only expression that reference variables (fields, params, locals)
  /// should be used, as if the expression passed had side-effects, bad things could happen.
  /// </remarks>
  /// <param name="pStoreType">the type that is being stored</param>
  /// <param name="pStoreVariable">a variable containing the variable to be stored</param>
  /// <returns></returns>
  public delegate CodeExpression GenerateValueStoreDelegate(Type pStoreType, CodeExpression pStoreVariable);

  /// <summary>
  /// Delegate that provides the code to store a given expression into the cache.
  /// </summary>
  /// <param name="pStoreExpr">the expression that generates the value to be stored</param>
  /// <returns>the statements performing the store</returns>
  public delegate CodeStatement[] GenerateCacheStoreDelegate(CodeExpression pStoreExpr);
}
