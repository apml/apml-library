using System;
using System.CodeDom;
using System.ComponentModel;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace APML.AutoWrapper.Strategies {
  /// <summary>
  /// Strategy used to handle retrieving attibute values.
  /// </summary>
  public class PrimitiveElementStrategy : IPropertyStrategy {
    #region IStrategy Members
    public StrategyPriority Priority {
      get { return StrategyPriority.BaseCode; }
    }
    #endregion

    #region IPropertyStrategy Members
    /// <summary>
    /// Checks whether the given strategy applies to a property.
    /// </summary>
    /// <param name="pProp">the property to check</param>
    /// <returns>true - strategies apply</returns>
    public bool AppliesToProperty(PropertyInfo pProp) {
      XmlElementAttribute elementTag = AttributeHelper.GetAttribute<XmlElementAttribute>(pProp);
      return (elementTag != null && !pProp.PropertyType.IsInterface && !pProp.PropertyType.IsArray);
    }

    /// <summary>
    /// Applies the primitive element strategy to the given generated property.
    /// </summary>
    /// <param name="pContext">the context of the generation</param>
    /// <param name="pProp">the declared property</param>
    /// <param name="pGeneratedProp">the property being generated</param>
    /// <param name="pClass">the class being declared</param>
    public void Apply(GenerationContext pContext, PropertyInfo pProp, CodeMemberProperty pGeneratedProp, CodeTypeDeclaration pClass) {
      // Generate the cache field
      pClass.Members.Add(MethodHelper.GenerateCacheField(pProp));

      // Generate the properties
      if (pProp.CanRead) {
        pGeneratedProp.GetStatements.AddRange(GetPrimitiveElementStrategy(pProp));
      }
      if (pProp.CanWrite) {
        pGeneratedProp.SetStatements.AddRange(SetPrimitiveElementStrategy(pProp));
      }

      // Add our various default support methods
      MethodHelper.GenerateClearMethod(pProp, pClass);
      MethodHelper.GenerateBaseInitMethod(pProp, pClass);
    }

    #endregion

    private static CodeStatement[] GetPrimitiveElementStrategy(PropertyInfo prop) {
      DefaultValueAttribute defaultValue = AttributeHelper.GetAttribute<DefaultValueAttribute>(prop);
      CodeExpression cacheRef = MethodHelper.GenerateCacheExpression(prop);
      CodeStatement cacheTest = MethodHelper.GenerateCheckCacheAndReturnValue(prop, cacheRef);

      // Build the get method
      CodeMethodInvokeExpression getInvoke = new CodeMethodInvokeExpression(
        new CodeThisReferenceExpression(), "GetElementOrDefault",
        new CodePrimitiveExpression(AttributeHelper.SelectXmlElementName(prop)),
        new CodePrimitiveExpression(defaultValue != null ? defaultValue.Value : null));
      getInvoke.Method.TypeArguments.Add(new CodeTypeReference(prop.PropertyType));
      CodeStatement cacheStoreStmt = MethodHelper.GenerateCacheStoreStatement(prop, getInvoke);
      CodeMethodReturnStatement getStmt = MethodHelper.GenerateCacheReturnStatement(prop);

      return new CodeStatement[] { cacheTest, cacheStoreStmt, getStmt };
    }

    private static CodeStatement[] SetPrimitiveElementStrategy(PropertyInfo prop) {
      CodeMethodInvokeExpression setExpr = new CodeMethodInvokeExpression(
          new CodeThisReferenceExpression(), "SetElement",
          new CodePrimitiveExpression(AttributeHelper.SelectXmlElementName(prop)),
          new CodePropertySetValueReferenceExpression()
          );
      setExpr.Method.TypeArguments.Add(new CodeTypeReference(prop.PropertyType));
      CodeExpressionStatement setStmt = new CodeExpressionStatement(setExpr);

      return new CodeStatement[] { setStmt, MethodHelper.GenerateCacheStoreStatement(prop, new CodePropertySetValueReferenceExpression()) };
    }
  }
}