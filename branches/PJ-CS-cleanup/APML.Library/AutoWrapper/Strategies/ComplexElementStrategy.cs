using System;
using System.CodeDom;
using System.Collections;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace APML.AutoWrapper.Strategies {
  /// <summary>
  /// Strategy used to handle retrieving attibute values.
  /// </summary>
  public class ComplexElementStrategy : BaseStrategy, IPropertyStrategy {
    #region IStrategy Members
    /// <summary>
    /// Defines the priority of the strategy as being base code.
    /// </summary>
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
      XmlElementAttribute elementTag = AttributeHelper.GetAttribute<XmlElementAttribute>(pProp);
      return (elementTag != null && pProp.PropertyType.IsInterface && !typeof(IEnumerable).IsAssignableFrom(pProp.PropertyType));
    }

    /// <summary>
    /// Applies the complex element strategy to the given generated property.
    /// </summary>
    /// <param name="pContext">the context of the generation</param>
    /// <param name="pProp">the declared property</param>
    /// <param name="pGeneratedProp">the property being generated</param>
    /// <param name="pClass">the class being declared</param>
    public void Apply(GenerationContext pContext, PropertyInfo pProp, CodeMemberProperty pGeneratedProp, CodeTypeDeclaration pClass) {
      // Generate the caching property
      pClass.Members.Add(MethodHelper.GenerateCacheField(pProp));

      if (pProp.CanRead) {
        pGeneratedProp.GetStatements.AddRange(GetComplexElementStrategy(pContext, pProp));
      }
      if (pProp.CanWrite) {
        throw new ArgumentException("Setters are not supported on complex elements (found on " + pClass.Name + "." + pGeneratedProp.Name + ")");
      }

      // Add our various default support methods
      MethodHelper.GenerateBaseInitMethod(pProp, pClass, true, 
        delegate (CodeExpression storeExpr) {
          return new CodeStatement[] {
                                       GetCacheAssignStatement(pContext, pProp, storeExpr)
                                     };
        });
      MethodHelper.GenerateClearMethod(pProp, pClass);
    }
    #endregion

    private CodeStatement[] GetComplexElementStrategy(GenerationContext pContext, PropertyInfo pProp) {
      CodeExpression cacheRef = MethodHelper.GenerateCacheExpression(pProp);
      CodeStatement cacheTest = MethodHelper.GenerateCheckCacheAndReturnValue(pProp, cacheRef);

      CodeMethodInvokeExpression findElementInvoke = new CodeMethodInvokeExpression(
        new CodeThisReferenceExpression(), "FindElement",
        new CodePrimitiveExpression(AttributeHelper.SelectXmlElementName(pProp)),
        new CodePrimitiveExpression(AttributeHelper.SelectXmlElementNamespace(pProp)),
        new CodePrimitiveExpression(false));
      CodeVariableDeclarationStatement elementDecl = new CodeVariableDeclarationStatement(
        typeof(XmlElement), "element", findElementInvoke);
      CodeStatement cacheAssignStmt = GetCacheAssignStatement(pContext, pProp, new CodeVariableReferenceExpression("element"));
      CodeMethodReturnStatement returnNullStmt = new CodeMethodReturnStatement(new CodePrimitiveExpression(null));
      
      CodeMethodReturnStatement returnWrapperStmt = new CodeMethodReturnStatement(cacheRef);
      CodeConditionStatement checkElementStmt = new CodeConditionStatement(
        new CodeBinaryOperatorExpression(
          new CodeVariableReferenceExpression("element"), 
          CodeBinaryOperatorType.IdentityInequality, 
          new CodePrimitiveExpression(null)),
        new CodeStatement[]{ cacheAssignStmt, returnWrapperStmt },
        new CodeStatement[] { returnNullStmt });

      return new CodeStatement[] { cacheTest, elementDecl, checkElementStmt };
    }

    private CodeStatement GetCacheAssignStatement(GenerationContext pContext, PropertyInfo pProp, CodeExpression pValExpr) {
      CodeExpression cacheRef = MethodHelper.GenerateCacheExpression(pProp);
      CodeExpression createWrapper =
        CreateCreateObjectExpression(pContext, pProp.PropertyType, pValExpr);

      return new CodeAssignStatement(cacheRef, createWrapper);
    }
  }
}