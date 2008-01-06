using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;

namespace APML.AutoWrapper.Strategies {
  /// <summary>
  /// Strategy for handling transforming a list of elements into a dictionary of lists - 
  /// hence allowing duplicate keys.
  /// </summary>
  public class SequenceDictionaryOfListsPropertyStrategy : SequenceDictionaryPropertyStrategy {
    #region IPropertyStrategy Members
    public override bool AppliesToProperty(PropertyInfo pProp) {
      XmlElementAttribute elAttr = AttributeHelper.GetAttribute<XmlElementAttribute>(pProp);
      XmlArrayAttribute arrAttr = AttributeHelper.GetAttribute<XmlArrayAttribute>(pProp);

      return (elAttr != null || arrAttr != null) && pProp.PropertyType.IsGenericType &&
             typeof(IReadOnlyDictionary<,>).IsAssignableFrom(pProp.PropertyType.GetGenericTypeDefinition()) &&
             pProp.PropertyType.GetGenericArguments()[1].IsGenericType &&
             typeof(IList<>).IsAssignableFrom(pProp.PropertyType.GetGenericArguments()[1].GetGenericTypeDefinition());
    }
    #endregion

    #region SequencePropertyStrategy Members
    protected override Type GetResultType(PropertyInfo pProp) {
      return typeof (Dictionary<,>).MakeGenericType(typeof (string), typeof(List<>).MakeGenericType(GetElementType(pProp)));
    }

    protected override Type GetElementType(PropertyInfo pProp) {
      return pProp.PropertyType.GetGenericArguments()[1].GetGenericArguments()[0];
    }

    /*protected override CodeStatement[] GetConvertedResultStoreStatements(GenerationContext pContext, PropertyInfo pProp,
                                                                         CodeExpression pResult, CodeExpression pPosition, CodeExpression pConverted) {
      AutoWrapperKeyAttribute keyAttr = AttributeHelper.GetAttribute<AutoWrapperKeyAttribute>(pProp);

      return new CodeStatement[] {
                                   new CodeVariableDeclarationStatement(GetElementType(pProp), "resultObj", pConverted),
                                   new CodeExpressionStatement(
                                     new CodeMethodInvokeExpression(
                                       pResult, "Add", 
                                       new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("resultObj"), keyAttr.KeyAttribute),
                                       new CodeVariableReferenceExpression("resultObj")))
                                 };
    }*/

    protected override CodeMethodReturnStatement GetReturnStatement(GenerationContext pContext, PropertyInfo pProp, CodeExpression pResult) {
      return new CodeMethodReturnStatement(
        new CodeObjectCreateExpression(
          typeof (ReadOnlyDictionary<,>).MakeGenericType(typeof (string), typeof(List<>).MakeGenericType(GetElementType(pProp))),
          pResult));
    }
    #endregion
  }
}