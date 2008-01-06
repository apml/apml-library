using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace APML.AutoWrapper.Strategies {
  /// <summary>
  /// Strategy for handling a sequence that is visualised as an array.
  /// </summary>
  public class SequenceArrayPropertyStrategy : SequencePropertyStrategy {
    #region IPropertyStrategy Members
    /// <summary>
    /// Checks whether the given strategy applies to a property.
    /// </summary>
    /// <param name="pProp">the property to check</param>
    /// <returns>true - strategies apply</returns>
    public override bool AppliesToProperty(PropertyInfo pProp) {
      return base.AppliesToProperty(pProp) && pProp.PropertyType.IsArray;
    }
    #endregion

    #region SequencePropertyStrategy Members
    protected override CodeExpression GetInitResultExpression(GenerationContext pContext, PropertyInfo pProp) {
      return new CodeArrayCreateExpression(
        pProp.PropertyType.GetElementType(),
        new CodePropertyReferenceExpression(new CodeVariableReferenceExpression("elements"), "Length"));
    }

    /// <summary>
    /// Retrieves the type of the elements in the given property.
    /// </summary>
    /// <param name="pProp">the property being generated</param>
    /// <returns>the element type</returns>
    protected override Type GetElementType(PropertyInfo pProp) {
      return pProp.PropertyType.GetElementType();
    }

    /// <summary>
    /// Generates an array index store statement.
    /// </summary>
    /// <param name="pContext">the context of the generation</param>
    /// <param name="pResult">the result object</param>
    /// <param name="pPosition">the expression that retrieves the current position in the element array</param>
    /// <param name="pConverted">the expression that represents the converted value</param>
    /// <returns>the generated statement</returns>
    protected override CodeStatement GetConvertedResultStoreStatement(GenerationContext pContext, CodeExpression pResult, CodeExpression pPosition, CodeExpression pConverted) {
      return new CodeAssignStatement(
        new CodeArrayIndexerExpression(pResult, pPosition), 
        pConverted);
    }

    /// <summary>
    /// The array sequence type does not support an updatable cache - it needs to be invalidated upon updates.
    /// </summary>
    protected override bool HasUpdateableCache {
      get { return false; }
    }
    #endregion
  }
}
