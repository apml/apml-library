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
      return typeof (DictionaryOfLists<,>).MakeGenericType(typeof (string), GetElementType(pProp));
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
          typeof (ReadOnlyDictionary<,>).MakeGenericType(typeof (string), typeof(IList<>).MakeGenericType(GetElementType(pProp))),
          pResult));
    }

    protected override CodeStatement[] GenerateEnumerateForWalk(GenerationContext pContext, PropertyInfo pProp, GenerateHandleItemDelegate pHandleItemDelegate) {
      CodeExpression cacheRef = MethodHelper.GenerateCacheExpression(pProp);

      // Take a snapshot of all the lists
      CodeExpression keyColExpr = new CodePropertyReferenceExpression(cacheRef, "Keys");
      Type keyArrayType = typeof(string[]);
      CodeVariableDeclarationStatement keyArrDecl = new CodeVariableDeclarationStatement(keyArrayType, "keys",
        new CodeArrayCreateExpression(keyArrayType, new CodePropertyReferenceExpression(cacheRef, "Count")));
      CodeVariableReferenceExpression keyArrRefExpr = new CodeVariableReferenceExpression("keys");
      CodeMethodInvokeExpression copyToArrayExpr =
        new CodeMethodInvokeExpression(keyColExpr, "CopyTo", keyArrRefExpr, new CodePrimitiveExpression(0));

      CodeVariableReferenceExpression listIndexerExpr = new CodeVariableReferenceExpression("i");
      CodeIterationStatement iterate = new CodeIterationStatement(
        new CodeVariableDeclarationStatement(typeof(int), "i", new CodePrimitiveExpression(0)),
        new CodeBinaryOperatorExpression(listIndexerExpr, CodeBinaryOperatorType.LessThanOrEqual, new CodePropertyReferenceExpression(keyArrRefExpr, "Length")),
        new CodeAssignStatement(listIndexerExpr, new CodeBinaryOperatorExpression(listIndexerExpr, CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))));
      CodeExpression keyExpr = new CodeIndexerExpression(keyArrRefExpr, listIndexerExpr);
      CodeVariableDeclarationStatement listDecl =
        new CodeVariableDeclarationStatement(typeof(IList<>).MakeGenericType(GetElementType(pProp)), "list", 
          new CodeIndexerExpression(cacheRef, keyExpr));
      CodeExpression listValueRef = new CodeVariableReferenceExpression("list");
      AutoWrapperKeyAttribute keyAttr = AttributeHelper.GetAttribute<AutoWrapperKeyAttribute>(pProp);

      CodeVariableReferenceExpression itemIndexerExpr = new CodeVariableReferenceExpression("j");
      CodeIterationStatement childIterate = new CodeIterationStatement(
        new CodeVariableDeclarationStatement(typeof(int), "j", new CodePrimitiveExpression(0)),
        new CodeBinaryOperatorExpression(itemIndexerExpr, CodeBinaryOperatorType.LessThanOrEqual, new CodePropertyReferenceExpression(listValueRef, "Count")),
        new CodeAssignStatement(itemIndexerExpr, new CodeBinaryOperatorExpression(itemIndexerExpr, CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))));
      CodeExpression itemValueRef = new CodeIndexerExpression(listValueRef, itemIndexerExpr);
      iterate.Statements.Add(listDecl);
      iterate.Statements.Add(childIterate);

      // Remove empty lists
      CodeConditionStatement emptyCheck = new CodeConditionStatement(
        new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(listValueRef, "Count"),
                                         CodeBinaryOperatorType.LessThanOrEqual, new CodePrimitiveExpression(0)));
      emptyCheck.TrueStatements.Add(
        new CodeMethodInvokeExpression(cacheRef, "Remove", keyExpr));
      iterate.Statements.Add(emptyCheck);

      childIterate.Statements.AddRange(pHandleItemDelegate(
        itemValueRef,
        new CodeStatement[] {
          // Remove from the list, then go back a value in the iteration variable
          new CodeExpressionStatement(new CodeMethodInvokeExpression(cacheRef, "Remove", new CodePropertyReferenceExpression(itemValueRef, keyAttr.KeyAttribute))),
          new CodeAssignStatement(itemIndexerExpr, new CodeBinaryOperatorExpression(itemIndexerExpr, CodeBinaryOperatorType.Subtract, new CodePrimitiveExpression(1))),
        }));

      return new CodeStatement[] { keyArrDecl, new CodeExpressionStatement(copyToArrayExpr), iterate };
    }

    /*protected override CodeStatement[] GenerateEnumerateForWalk(GenerationContext pContext, PropertyInfo pProp, GenerateHandleItemDelegate pHandleItemDelegate) {
      CodeExpression cacheRef = MethodHelper.GenerateCacheExpression(pProp);

      // Take a snapshot of all the lists
      CodeExpression valueListColExpr = new CodePropertyReferenceExpression(cacheRef, "Values");
      Type listArrayType = typeof (IList<>).MakeGenericType(GetElementType(pProp)).MakeArrayType();
      CodeVariableDeclarationStatement valueListArrDecl = new CodeVariableDeclarationStatement(listArrayType, "valueLists",
        new CodeArrayCreateExpression(listArrayType, new CodePropertyReferenceExpression(cacheRef, "Count")));
      CodeVariableReferenceExpression valueListArrRefExpr = new CodeVariableReferenceExpression("valueLists");
      CodeMethodInvokeExpression copyToArrayExpr =
        new CodeMethodInvokeExpression(valueListColExpr, "CopyTo", valueListArrRefExpr, new CodePrimitiveExpression(0));

      CodeVariableReferenceExpression listIndexerExpr = new CodeVariableReferenceExpression("i");
      CodeIterationStatement iterate = new CodeIterationStatement(
        new CodeVariableDeclarationStatement(typeof(int), "i", new CodePrimitiveExpression(0)),
        new CodeBinaryOperatorExpression(listIndexerExpr, CodeBinaryOperatorType.LessThanOrEqual, new CodePropertyReferenceExpression(valueListArrRefExpr, "Length")),
        new CodeAssignStatement(listIndexerExpr, new CodeBinaryOperatorExpression(listIndexerExpr, CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))));
      CodeExpression listValueRef = new CodeIndexerExpression(valueListArrRefExpr, listIndexerExpr);
      AutoWrapperKeyAttribute keyAttr = AttributeHelper.GetAttribute<AutoWrapperKeyAttribute>(pProp);

      CodeVariableReferenceExpression itemIndexerExpr = new CodeVariableReferenceExpression("j");
      CodeIterationStatement childIterate = new CodeIterationStatement(
        new CodeVariableDeclarationStatement(typeof(int), "j", new CodePrimitiveExpression(0)),
        new CodeBinaryOperatorExpression(itemIndexerExpr, CodeBinaryOperatorType.LessThanOrEqual, new CodePropertyReferenceExpression(listValueRef, "Count")),
        new CodeAssignStatement(itemIndexerExpr, new CodeBinaryOperatorExpression(itemIndexerExpr, CodeBinaryOperatorType.Add, new CodePrimitiveExpression(1))));
      CodeExpression itemValueRef = new CodeIndexerExpression(listValueRef, itemIndexerExpr);
      iterate.Statements.Add(childIterate);
      
      // TODO: Remove empty lists - this will require knowing the key, which this code currently doesn't let us know
      CodeConditionStatement emptyCheck = new CodeConditionStatement(
        new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(listValueRef, "Count"),
                                         CodeBinaryOperatorType.LessThanOrEqual, new CodePrimitiveExpression(0)));


      childIterate.Statements.AddRange(pHandleItemDelegate(
        itemValueRef,
        new CodeStatement[] {
          // Remove from the list, then go back a value in the iteration variable
          new CodeExpressionStatement(new CodeMethodInvokeExpression(cacheRef, "Remove", new CodePropertyReferenceExpression(itemValueRef, keyAttr.KeyAttribute))),
          new CodeAssignStatement(itemIndexerExpr, new CodeBinaryOperatorExpression(itemIndexerExpr, CodeBinaryOperatorType.Subtract, new CodePrimitiveExpression(1))),
        }));

      return new CodeStatement[] { valueListArrDecl, new CodeExpressionStatement(copyToArrayExpr), iterate };
    }*/
    #endregion
  }
}