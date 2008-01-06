using System;
using System.CodeDom;
using System.Collections.Generic;
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
          new CodePrimitiveExpression(AttributeHelper.SelectXmlElementName(pProp))));
      pClass.Members.Add(method);
    }

    ///<summary>
    /// Generate a method for initialising an element.
    ///</summary>
    ///<param name="pProp">the property this init method is related to</param>
    ///<param name="pClass">the class being generated</param>
    public static void GenerateBaseInitMethod(PropertyInfo pProp, CodeTypeDeclaration pClass) {
      CodeMemberMethod method = new CodeMemberMethod();
      method.Name = "Init" + pProp.Name;
      method.Attributes = MemberAttributes.Family;
      method.ReturnType = new CodeTypeReference(typeof (XmlElement));
      method.Statements.Add(
        new CodeMethodReturnStatement(
          new CodeMethodInvokeExpression(
            new CodeBaseReferenceExpression(),
            "InitElement",
            new CodePrimitiveExpression(AttributeHelper.SelectXmlElementName(pProp)))));
      pClass.Members.Add(method);
    }

    /// <summary>
    /// Finds the property that a given method applies to by removing the given name prefix and finding a
    /// property with the name.
    /// </summary>
    /// <param name="pMethod">the method to find the property for</param>
    /// <param name="pMethodNamePrefix">the prefix on the method</param>
    /// <returns>the relevant property</returns>
    public static PropertyInfo FindRelevantProperty(MethodInfo pMethod, string pMethodNamePrefix) {
      return pMethod.DeclaringType.GetProperty(pMethod.Name.Substring(pMethodNamePrefix.Length));
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
      List<CodeStatement> result = new List<CodeStatement>();

      // For each parameter, find a property that matches
      foreach (ParameterInfo param in pMethod.GetParameters()) {
        foreach (PropertyInfo childProp in pPropertyType.GetProperties()) {
          if (childProp.Name.ToLower() == param.Name.ToLower()) {
            // We've found the property that we want
            CodeAssignStatement assign = new CodeAssignStatement(
              new CodePropertyReferenceExpression(
                pTarget,
                childProp.Name),
              new CodeVariableReferenceExpression(param.Name));
            result.Add(assign);
          }
        }
      }

      return result.ToArray();
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
      if (pProp.PropertyType.IsPrimitive) {
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
      if (pProp.PropertyType.IsPrimitive) {
        return new CodeMethodReturnStatement(new CodePropertyReferenceExpression(cacheRef, "Value"));
      } else {
        return new CodeMethodReturnStatement(cacheRef);
      }
    }
  }
}
