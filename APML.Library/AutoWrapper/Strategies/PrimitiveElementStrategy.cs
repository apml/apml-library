using System;
using System.CodeDom;
using System.Reflection;
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
        pGeneratedProp.GetStatements.AddRange(MethodHelper.GenerateSimpleGetMethod(
          pProp, delegate(Type pType, object pDefaultValue) {
                   CodeMethodInvokeExpression getInvoke = new CodeMethodInvokeExpression(
                     new CodeThisReferenceExpression(), "GetElementOrDefault",
                     new CodePrimitiveExpression(AttributeHelper.SelectXmlElementName(pProp)),
                     new CodePrimitiveExpression(AttributeHelper.SelectXmlElementNamespace(pProp)),
                     new CodePrimitiveExpression(pDefaultValue));
                   getInvoke.Method.TypeArguments.Add(new CodeTypeReference(pType));

                   return getInvoke;
                 }));
      }
      if (pProp.CanWrite) {
        pGeneratedProp.SetStatements.AddRange(MethodHelper.GenerateSimpleSetMethod(
          pProp, delegate(Type pType, CodeExpression pValueExpr) {
                   CodeMethodInvokeExpression setExpr = new CodeMethodInvokeExpression(
                     new CodeThisReferenceExpression(), "SetElement",
                     new CodePrimitiveExpression(AttributeHelper.SelectXmlElementName(pProp)),
                     new CodePrimitiveExpression(AttributeHelper.SelectXmlElementNamespace(pProp)),
                     pValueExpr
                     );
                   setExpr.Method.TypeArguments.Add(new CodeTypeReference(pType));
                   return setExpr;
                 }));
      }

      // Add our various default support methods
      MethodHelper.GenerateClearMethod(pProp, pClass);
      MethodHelper.GenerateBaseInitMethod(pProp, pClass);
    }
    #endregion
  }
}