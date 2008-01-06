using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace APML.AutoWrapper.Strategies {
  /// <summary>
  /// Strategy for handling transforming a list of elements into a dictionary.
  /// </summary>
  public class SequenceDictionaryPropertyStrategy : SequencePropertyStrategy {
    #region IPropertyStrategy Members
    public override bool AppliesToProperty(PropertyInfo pProp) {
      return base.AppliesToProperty(pProp) && pProp.PropertyType.IsGenericType && 
        typeof(IReadOnlyDictionary<,>).IsAssignableFrom(pProp.PropertyType.GetGenericTypeDefinition()) &&
        !typeof(IEnumerable).IsAssignableFrom(pProp.PropertyType.GetGenericArguments()[1]);
    }
    #endregion

    #region SequencePropertyStrategy Members
    protected override CodeExpression GetInitResultExpression(GenerationContext pContext, PropertyInfo pProp) {
      return new CodeObjectCreateExpression(GetResultType(pProp));
    }

    protected override Type GetResultType(PropertyInfo pProp) {
      return typeof (Dictionary<,>).MakeGenericType(typeof (string), GetElementType(pProp));
    }

    protected override Type GetElementType(PropertyInfo pProp) {
      return pProp.PropertyType.GetGenericArguments()[1];
    }

    protected override CodeStatement[] GetConvertedResultStoreStatements(GenerationContext pContext, PropertyInfo pProp,
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
    }

    protected override CodeMethodReturnStatement GetReturnStatement(GenerationContext pContext, PropertyInfo pProp, CodeExpression pResult) {
      return new CodeMethodReturnStatement(
        new CodeObjectCreateExpression(
          typeof (ReadOnlyDictionary<,>).MakeGenericType(typeof (string), GetElementType(pProp)),
          pResult));
    }

    protected override CodeParameterDeclarationExpression[] ProvideMandatoryAddParameters(GenerationContext pContext, PropertyInfo pProp) {
      AutoWrapperKeyAttribute keyAttr = AttributeHelper.GetAttribute<AutoWrapperKeyAttribute>(pProp);

      return new CodeParameterDeclarationExpression[] {
        new CodeParameterDeclarationExpression(typeof(string), "p" + keyAttr.KeyAttribute)
                                                      };
    }

    protected override CodeStatement[] ApplyMandatoryAddParameters(GenerationContext pContext, PropertyInfo pProp, CodeExpression pResultExpr) {
      AutoWrapperKeyAttribute keyAttr = AttributeHelper.GetAttribute<AutoWrapperKeyAttribute>(pProp);

      return new CodeStatement[] {
        new CodeAssignStatement(new CodePropertyReferenceExpression(pResultExpr, keyAttr.KeyAttribute), new CodeVariableReferenceExpression("p" + keyAttr.KeyAttribute))
                                 };
    }
    #endregion
  }
}
