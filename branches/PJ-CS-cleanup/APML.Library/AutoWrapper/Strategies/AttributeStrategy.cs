using System;
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
        pGeneratedProp.GetStatements.AddRange(MethodHelper.GenerateSimpleGetMethod(
          pProp, delegate(Type pType, object pDefaultValue) {
                   CodeMethodInvokeExpression getInvoke = new CodeMethodInvokeExpression(
                     new CodeThisReferenceExpression(), "GetAttributeOrDefault",
                     new CodePrimitiveExpression(SelectAttributeName(pProp)),
                     new CodePrimitiveExpression(pDefaultValue));

                   getInvoke.Method.TypeArguments.Add(new CodeTypeReference(pType));
                   return getInvoke;
                 }));
      }
      if (pProp.CanWrite) {
        pGeneratedProp.SetStatements.AddRange(MethodHelper.GenerateSimpleSetMethod(
          pProp, delegate(Type pType, CodeExpression pValueExpr) {
                   CodeMethodInvokeExpression setExpr =
                     new CodeMethodInvokeExpression(
                       new CodeThisReferenceExpression(), "SetAttribute",
                       new CodePrimitiveExpression(SelectAttributeName(pProp)),
                       pValueExpr
                       );
                   setExpr.Method.TypeArguments.Add(new CodeTypeReference(pType));
                   return setExpr;
                 }));
      }
    }

    #endregion

    private static string SelectAttributeName(MemberInfo pProp) {
      XmlAttributeAttribute attrTag = AttributeHelper.GetAttribute<XmlAttributeAttribute>(pProp);
      if (attrTag.AttributeName != null && attrTag.AttributeName != string.Empty) {
        return attrTag.AttributeName;
      }

      return pProp.Name;
    }
  }
}
