using System.CodeDom;
using System.ComponentModel;
using System.Reflection;
using System.Xml.Serialization;

namespace APML.AutoWrapper.Strategies {
  /// <summary>
  /// Strategy used to handle retrieving attibute values.
  /// </summary>
  public class AttributeStrategy : BaseStrategy, IPropertyStrategy {
    #region IStrategy Members
    public override StrategyPriority Priority {
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
      XmlAttributeAttribute attrTag = AttributeHelper.GetAttribute<XmlAttributeAttribute>(pProp);
      return (attrTag != null);
    }

    public void Apply(GenerationContext pContext, PropertyInfo pProp, CodeMemberProperty pGeneratedProp, CodeTypeDeclaration pClass) {
      pClass.Members.Add(MethodHelper.GenerateCacheField(pProp));

      if (pProp.CanRead) {
        pGeneratedProp.GetStatements.AddRange(GetAttributeStrategy(pProp));
      }
      if (pProp.CanWrite) {
        pGeneratedProp.SetStatements.AddRange(SetAttributeStrategy(pProp));
      }
    }

    #endregion

    private static CodeStatement[] GetAttributeStrategy(PropertyInfo prop) {
      DefaultValueAttribute defaultValue = AttributeHelper.GetAttribute<DefaultValueAttribute>(prop);

      CodeExpression cacheRef = MethodHelper.GenerateCacheExpression(prop);
      CodeStatement cacheTest = MethodHelper.GenerateCheckCacheAndReturnValue(prop, cacheRef);

      // Build the get method
      CodeMethodInvokeExpression getInvoke = new CodeMethodInvokeExpression(
        new CodeThisReferenceExpression(), "GetAttributeOrDefault",
        new CodePrimitiveExpression(SelectAttributeName(prop)),
        new CodePrimitiveExpression(defaultValue != null ? defaultValue.Value : null));
      getInvoke.Method.TypeArguments.Add(new CodeTypeReference(prop.PropertyType));
      CodeStatement cacheStoreStmt = MethodHelper.GenerateCacheStoreStatement(prop, getInvoke);
      CodeMethodReturnStatement getStmt = MethodHelper.GenerateCacheReturnStatement(prop);

      return new CodeStatement[] { cacheTest, cacheStoreStmt, getStmt };
    }

    private static CodeStatement[] SetAttributeStrategy(PropertyInfo prop) {
      CodeMethodInvokeExpression setExpr = new CodeMethodInvokeExpression(
          new CodeThisReferenceExpression(), "SetAttribute",
          new CodePrimitiveExpression(SelectAttributeName(prop)),
          new CodeArgumentReferenceExpression("value")
          );
      setExpr.Method.TypeArguments.Add(new CodeTypeReference(prop.PropertyType));
      CodeExpressionStatement setStmt = new CodeExpressionStatement(setExpr);

      return new CodeStatement[] { setStmt, MethodHelper.GenerateCacheStoreStatement(prop, new CodePropertySetValueReferenceExpression()) };
    }

    private static string SelectAttributeName(MemberInfo pProp) {
      XmlAttributeAttribute attrTag = AttributeHelper.GetAttribute<XmlAttributeAttribute>(pProp);
      if (attrTag.AttributeName != null && attrTag.AttributeName != string.Empty) {
        return attrTag.AttributeName;
      }

      return pProp.Name;
    }
  }
}
