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

    protected override CodeStatement[] GenerateEnumerateForWalk(GenerationContext pContext, PropertyInfo pProp, GenerateHandleItemDelegate pHandleItemDelegate) {
      CodeExpression cacheRef = MethodHelper.GenerateCacheExpression(pProp);

      CodeVariableDeclarationStatement valueColDecl =
        new CodeVariableDeclarationStatement(typeof (ICollection<>).MakeGenericType(GetElementType(pProp)), "valuesCol",
                                             new CodePropertyReferenceExpression(cacheRef, "Values"));
      CodeVariableReferenceExpression valuesColRef = new CodeVariableReferenceExpression("valuesCol");
      CodeVariableDeclarationStatement valueArrDecl = new CodeVariableDeclarationStatement(GetElementType(pProp).MakeArrayType(), "values",
        new CodeArrayCreateExpression(GetElementType(pProp).MakeArrayType(), new CodePropertyReferenceExpression(valuesColRef, "Count")));
      CodeVariableReferenceExpression valueArrRefExpr = new CodeVariableReferenceExpression("values");
      CodeMethodInvokeExpression copyToArrayExpr =
        new CodeMethodInvokeExpression(valuesColRef, "CopyTo", valueArrRefExpr, new CodePrimitiveExpression(0));

      CodeVariableReferenceExpression indexerExpr = new CodeVariableReferenceExpression("i");
      
      CodeIterationStatement iterate = new CodeIterationStatement(
        new CodeVariableDeclarationStatement(typeof(int), "i", new CodePrimitiveExpression(0)),
        new CodeBinaryOperatorExpression(indexerExpr, CodeBinaryOperatorType.LessThan, new CodePropertyReferenceExpression(valueArrRefExpr, "Length")),
        new CodeAssignStatement(indexerExpr, new CodeBinaryOperatorExpression(indexerExpr, CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))));
      CodeExpression valueRef = new CodeIndexerExpression(valueArrRefExpr, indexerExpr);

      AutoWrapperKeyAttribute keyAttr = AttributeHelper.GetAttribute<AutoWrapperKeyAttribute>(pProp);

      iterate.Statements.AddRange(pHandleItemDelegate(
        valueRef,
        new CodeStatement[] {
          // No need to change indexer, since we took a static snapshot

          new CodeExpressionStatement(new CodeMethodInvokeExpression(cacheRef, "Remove", new CodePropertyReferenceExpression(valueRef, keyAttr.KeyAttribute))),
        }));

      return new CodeStatement[] { valueColDecl, valueArrDecl, new CodeExpressionStatement(copyToArrayExpr), iterate };
    }
    #endregion
  }
}
