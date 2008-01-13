using System;
using System.CodeDom;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace APML.AutoWrapper.Strategies {
  /// <summary>
  /// Base class for strategies that apply to sequences.
  /// </summary>
  public abstract class SequencePropertyStrategy : BaseStrategy, IPropertyStrategy {
    /// <summary>
    /// Defines the priority of the strategy.
    /// </summary>
    public override StrategyPriority Priority {
      get { return StrategyPriority.BaseCode; }
    }

    /// <summary>
    /// Checks whether the given strategy applies to a property.
    /// </summary>
    /// <param name="pProp">the property to check</param>
    /// <returns>true - strategies apply</returns>
    public virtual bool AppliesToProperty(PropertyInfo pProp) {
      XmlElementAttribute elAttr = AttributeHelper.GetAttribute<XmlElementAttribute>(pProp);
      XmlArrayAttribute arrAttr = AttributeHelper.GetAttribute<XmlArrayAttribute>(pProp);
      
      return elAttr != null || arrAttr != null;
    }

    /// <summary>
    /// Applies the given strategy to the given generated property.
    /// </summary>
    /// <param name="pContext">the context of the generation</param>
    /// <param name="pProp">the declared property</param>
    /// <param name="pGeneratedProp">the property being generated</param>
    /// <param name="pClass">the class being declared</param>
    public void Apply(GenerationContext pContext, PropertyInfo pProp, CodeMemberProperty pGeneratedProp, CodeTypeDeclaration pClass) {
      // Generate the field
      pClass.Members.Add(MethodHelper.GenerateCacheField(pProp, GetResultType(pProp)));

      if (pProp.CanRead) {
        pGeneratedProp.GetStatements.AddRange(GetSequenceListStrategy(pContext, pProp));
      }
      if (pProp.CanWrite) {
        throw new ArgumentException("Setters are not supported on sequence list elements");
      }

      // Generate the default support methods
      GenerateAddMethod(pContext, pProp, pClass);
      GenerateInitMethod(pContext, pProp, pClass);
      GenerateRemoveMethod();
    }

    /// <summary>
    /// Creates the getter method content.
    /// </summary>
    /// <param name="pContext">context of the generation</param>
    /// <param name="pProp">the property being generated</param>
    /// <returns>the result</returns>
    protected CodeStatement[] GetSequenceListStrategy(GenerationContext pContext, PropertyInfo pProp) {
      // Retrieve the necessary attributes
      XmlElementAttribute elAttr = AttributeHelper.GetAttribute<XmlElementAttribute>(pProp);
      XmlArrayAttribute arrAttr = AttributeHelper.GetAttribute<XmlArrayAttribute>(pProp);
      XmlArrayItemAttribute arrItemAttr = AttributeHelper.GetAttribute<XmlArrayItemAttribute>(pProp);

      // Check for the cache
      CodeExpression cacheRef = MethodHelper.GenerateCacheExpression(pProp);
      CodeStatement cacheCheck =
        MethodHelper.GenerateCheckCacheAndReturnValue(pProp, cacheRef, GetReturnStatement(pContext, pProp, cacheRef));

      // Build the get method
      CodeMethodInvokeExpression getElementsInvoke;
      if (elAttr != null) {
        getElementsInvoke = new CodeMethodInvokeExpression(
          new CodeBaseReferenceExpression(), "GetAllElements",
          new CodePrimitiveExpression(null),
          new CodePrimitiveExpression(null),
          new CodePrimitiveExpression(AttributeHelper.SelectXmlElementName(pProp)),
          new CodePrimitiveExpression(AttributeHelper.SelectXmlElementNamespace(pProp)));
      } else {
        getElementsInvoke = new CodeMethodInvokeExpression(
          new CodeThisReferenceExpression(), "GetAllElements",
          new CodePrimitiveExpression(arrAttr.ElementName ?? null),
          new CodePrimitiveExpression(arrAttr.Namespace),
          new CodePrimitiveExpression((arrItemAttr != null && arrItemAttr.ElementName != null) ? arrItemAttr.ElementName : pProp.Name),
          new CodePrimitiveExpression((arrItemAttr != null && arrItemAttr.ElementName != null) ? arrItemAttr.Namespace : arrAttr.Namespace));
      }
      CodeVariableDeclarationStatement elementsDecl = new CodeVariableDeclarationStatement(
        typeof(XmlElement[]), "elements", getElementsInvoke);
//      CodeVariableDeclarationStatement resultDecl = new CodeVariableDeclarationStatement(
//        GetResultType(pProp), "result", GetInitResultExpression(pContext, pProp));
      CodeAssignStatement resultDecl = new CodeAssignStatement(cacheRef, GetInitResultExpression(pContext, pProp));

      // Perform the conversion
      CodeIterationStatement conversionIt = new CodeIterationStatement(
        new CodeVariableDeclarationStatement(typeof (int), "i", new CodePrimitiveExpression(0)),
        new CodeBinaryOperatorExpression(
          new CodeVariableReferenceExpression("i"),
          CodeBinaryOperatorType.LessThan,
          new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("elements"), "Length")),
        new CodeAssignStatement(
          new CodeVariableReferenceExpression("i"),
          new CodeBinaryOperatorExpression(new CodeVariableReferenceExpression("i"), CodeBinaryOperatorType.Add,
                                           new CodePrimitiveExpression(1))));
      conversionIt.Statements.AddRange(
        GetConvertedResultStoreStatements(pContext, pProp, cacheRef,
          new CodeVariableReferenceExpression("i"),
          CreateCreateObjectExpression(
            pContext, 
            GetElementType(pProp), 
            new CodeArrayIndexerExpression(new CodeVariableReferenceExpression("elements"), new CodeVariableReferenceExpression("i")))));
      
      // Return the result
      CodeMethodReturnStatement returnResultStmt =
        GetReturnStatement(pContext, pProp, cacheRef);
      
      return new CodeStatement[] { cacheCheck, elementsDecl, resultDecl, conversionIt, returnResultStmt };
    }

    /// <summary>
    /// Provides the ability for subclasses to control the initialisation method for the result class.
    /// </summary>
    /// <param name="pContext">the context of the generation</param>
    /// <param name="pProp">the property being generated</param>
    /// <returns>the initialisation expression</returns>
    protected abstract CodeExpression GetInitResultExpression(GenerationContext pContext, PropertyInfo pProp);

    /// <summary>
    /// Returns the type that the internal result storage should use.
    /// </summary>
    /// <param name="pProp">the property being generated</param>
    /// <returns>the type to use</returns>
    protected virtual Type GetResultType(PropertyInfo pProp) {
      return pProp.PropertyType;
    }

    /// <summary>
    /// Retrieves the type of the elements in the given property.
    /// </summary>
    /// <param name="pProp">the property being generated</param>
    /// <returns>the element type</returns>
    protected abstract Type GetElementType(PropertyInfo pProp);

    /// <summary>
    /// Provides the ability for subclasses to control the storage of the converted type.
    /// </summary>
    /// <param name="pContext">the context of the generation</param>
    /// <param name="pProp">the property being generated</param>
    /// <param name="pResult">the result object</param>
    /// <param name="pPosition">the expression that retrieves the current position in the element array</param>
    /// <param name="pConverted">the expression that represents the converted value</param>
    /// <returns>the generated statement</returns>
    protected virtual CodeStatement[] GetConvertedResultStoreStatements(GenerationContext pContext, PropertyInfo pProp, CodeExpression pResult, 
                                                                        CodeExpression pPosition, CodeExpression pConverted) {
      return new CodeStatement[] {GetConvertedResultStoreStatement(pContext, pResult, pPosition, pConverted)};
    }

    /// <summary>
    /// Provides the ability for subclasses to control the storage of the converted type.
    /// </summary>
    /// <param name="pContext">the context of the generation</param>
    /// <param name="pResult">the result object</param>
    /// <param name="pPosition">the expression that retrieves the current position in the element array</param>
    /// <param name="pConverted">the expression that represents the converted value</param>
    /// <returns>the generated statement</returns>
    protected virtual CodeStatement GetConvertedResultStoreStatement(GenerationContext pContext, CodeExpression pResult, CodeExpression pPosition,
                                                                     CodeExpression pConverted) {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Provides the ability for subclasses to control the return type generated.
    /// </summary>
    /// <param name="pContext">the context of the generation</param>
    /// <param name="pProp">the property being generated</param>
    /// <param name="pResult">the result object</param>
    /// <returns>the statement for the return value</returns>
    protected virtual CodeMethodReturnStatement GetReturnStatement(GenerationContext pContext, PropertyInfo pProp, CodeExpression pResult) {
      return new CodeMethodReturnStatement(pResult);
    }

    /// <summary>
    /// Generates the method to add a new element.
    /// </summary>
    /// <param name="pContext">the context of the generation</param>
    /// <param name="pClass">the class being generated</param>
    /// <param name="pProp">the property being generated</param>
    protected virtual void GenerateAddMethod(GenerationContext pContext, PropertyInfo pProp, CodeTypeDeclaration pClass) {
      // Retrieve the necessary attributes
      XmlElementAttribute elAttr = AttributeHelper.GetAttribute<XmlElementAttribute>(pProp);
      XmlArrayAttribute arrAttr = AttributeHelper.GetAttribute<XmlArrayAttribute>(pProp);
      XmlArrayItemAttribute arrItemAttr = AttributeHelper.GetAttribute<XmlArrayItemAttribute>(pProp);

      // Start building the method
      CodeMemberMethod addMethod = new CodeMemberMethod();
      addMethod.Attributes = MemberAttributes.Public;
      addMethod.Name = "Add" + pProp.Name;
      addMethod.ReturnType = new CodeTypeReference(GetElementType(pProp));
      addMethod.Parameters.AddRange(ProvideMandatoryAddParameters(pContext, pProp));

      CodeMethodInvokeExpression addElementInvoke;
      if (elAttr != null) {
        addElementInvoke = new CodeMethodInvokeExpression(
          new CodeThisReferenceExpression(), "AddElement",
          new CodePrimitiveExpression(null),
          new CodePrimitiveExpression(null),
          new CodePrimitiveExpression(AttributeHelper.SelectXmlElementName(pProp)),
          new CodePrimitiveExpression(AttributeHelper.SelectXmlElementNamespace(pProp)));
      } else {
        addElementInvoke = new CodeMethodInvokeExpression(
          new CodeBaseReferenceExpression(), "AddElement",
          new CodePrimitiveExpression(arrAttr.ElementName ?? null),
          new CodePrimitiveExpression(arrAttr.Namespace),
          new CodePrimitiveExpression((arrItemAttr != null && arrItemAttr.ElementName != null) ? arrItemAttr.ElementName : pProp.Name),
          new CodePrimitiveExpression((arrItemAttr != null && arrItemAttr.ElementName != null) ? arrItemAttr.Namespace : arrAttr.Namespace));
      }
      CodeVariableDeclarationStatement resultElDecl = new CodeVariableDeclarationStatement(typeof(XmlElement), "resultEl", addElementInvoke);
      addMethod.Statements.Add(resultElDecl);

      CodeExpression createResultExpr = CreateCreateObjectExpression(pContext, GetElementType(pProp), new CodeVariableReferenceExpression("resultEl"));
      CodeVariableDeclarationStatement resultDecl = new CodeVariableDeclarationStatement(addMethod.ReturnType.BaseType, "result", createResultExpr);
      addMethod.Statements.Add(resultDecl);

      CodeVariableReferenceExpression resultRef = new CodeVariableReferenceExpression("result");
      addMethod.Statements.AddRange(ApplyMandatoryAddParameters(pContext, pProp, resultRef));

      // TODO: Add to cache?
      // Fix the cache
      CodeExpression cacheRef = MethodHelper.GenerateCacheExpression(pProp);
      if (!HasUpdateableCache) {
        // We need to clear the cache
        CodeAssignStatement clearCache = new CodeAssignStatement(cacheRef, new CodePrimitiveExpression(null));
        addMethod.Statements.Add(clearCache);
      } else {
        // We need to add the item to the cache if the cache exists
        addMethod.Statements.Add(
          MethodHelper.GenerateCacheGuardedStatements(
            pProp, 
            cacheRef,
            GetConvertedResultStoreStatements(pContext, pProp, cacheRef, null, resultRef)));
      }

      CodeMethodReturnStatement returnStmt = new CodeMethodReturnStatement(resultRef);
      addMethod.Statements.Add(returnStmt);

      pClass.Members.Add(addMethod);
    }

    /// <summary>
    /// Generates an Init method for the given class. For container elements, this will generate the container. For non-container elements,
    /// the method is not generated.
    /// </summary>
    /// <param name="pContext">the context of the generation</param>
    /// <param name="pProp">the property being generated</param>
    /// <param name="pClass">the class being generated</param>
    protected virtual void GenerateInitMethod(GenerationContext pContext, PropertyInfo pProp, CodeTypeDeclaration pClass) {
      // Retrieve the necessary attributes
      XmlArrayAttribute arrAttr = AttributeHelper.GetAttribute<XmlArrayAttribute>(pProp);
      
      // If this doesn't have a container attribute, then we aren't interested
      if (arrAttr == null) {
        return;
      }

      // Start building the method
      CodeMemberMethod initMethod = new CodeMemberMethod();
      initMethod.Attributes = MemberAttributes.Public;
      initMethod.Name = "Init" + pProp.Name;
      initMethod.ReturnType = new CodeTypeReference(typeof(void));
      
      CodeMethodInvokeExpression addElementInvoke;
      addElementInvoke = new CodeMethodInvokeExpression(
        new CodeBaseReferenceExpression(), "AddElement",
        new CodePrimitiveExpression(null),
        new CodePrimitiveExpression(null),
        new CodePrimitiveExpression(arrAttr.ElementName),
        new CodePrimitiveExpression(arrAttr.Namespace));
      initMethod.Statements.Add(addElementInvoke);

      pClass.Members.Add(initMethod);
    }

    /// <summary>
    /// Allows subclasses to provide mandatory parameters that the add method must use.
    /// </summary>
    /// <param name="pContext">the generation context</param>
    /// <param name="pProp">the property being generated</param>
    /// <returns>the list of parameters that need to be declared</returns>
    protected virtual CodeParameterDeclarationExpression[] ProvideMandatoryAddParameters(GenerationContext pContext, PropertyInfo pProp) {
      return new CodeParameterDeclarationExpression[0];
    }

    /// <summary>
    /// Allows subclasses to apply the mandatory add parameters to the result class.
    /// </summary>
    /// <param name="pContext">the generation context</param>
    /// <param name="pProp">the property being generated</param>
    /// <param name="pResultExpr">the expression that can be used to access the result variable</param>
    /// <returns>the code statements to apply the mandatory parameters</returns>
    protected virtual CodeStatement[] ApplyMandatoryAddParameters(GenerationContext pContext, PropertyInfo pProp, CodeExpression pResultExpr) {
      return new CodeStatement[0];
    }

    /// <summary>
    /// Retrieves whether the cache for this type can be updated, or if the cache should simply be invalidated
    /// upon item addition/removal.
    /// </summary>
    protected virtual bool HasUpdateableCache {
      get { return true; }
    }

    /// <summary>
    /// Generates the method to remove elements.
    /// </summary>
    protected virtual void GenerateRemoveMethod() {
      // TODO:
    }
  }
}