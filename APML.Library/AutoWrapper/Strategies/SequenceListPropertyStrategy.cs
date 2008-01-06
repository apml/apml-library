using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace APML.AutoWrapper.Strategies {
  ///<summary>
  /// Strategy for visualising a sequence as a list.
  ///</summary>
  public class SequenceListPropertyStrategy : SequencePropertyStrategy {
    /// <summary>
    /// Checks whether the given strategy applies to a property.
    /// </summary>
    /// <param name="pProp">the property to check</param>
    /// <returns>true - strategies apply</returns>
    public override bool AppliesToProperty(PropertyInfo pProp) {
      return base.AppliesToProperty(pProp) && pProp.PropertyType.IsGenericType && typeof(IList<>).IsAssignableFrom(pProp.PropertyType.GetGenericTypeDefinition());
    }

    #region SequencePropertyStrategy Members

    /// <summary>
    /// Provides the ability for subclasses to control the initialisation method for the result class.
    /// </summary>
    /// <param name="pContext">the context of the generation</param>
    /// <param name="pProp">the property being generated</param>
    /// <returns>the initialisation expression</returns>
    protected override CodeExpression GetInitResultExpression(GenerationContext pContext, PropertyInfo pProp) {
      return new CodeObjectCreateExpression(typeof(List<>).MakeGenericType(GetElementType(pProp)));
    }

    /// <summary>
    /// Retrieves the first generic type argument of the lst.
    /// </summary>
    /// <param name="pProp">the property being generated</param>
    /// <returns>the element type</returns>
    protected override Type GetElementType(PropertyInfo pProp) {
      return pProp.PropertyType.GetGenericArguments()[0];
    }

    /// <summary>
    /// Retrieves a statement that adds the given converted type to the result.
    /// </summary>
    /// <param name="pContext">the generation context</param>
    /// <param name="pResult">the result object</param>
    /// <param name="pPosition">the expression to retrieve the position</param>
    /// <param name="pConverted">the expression containing the converted value</param>
    /// <returns>the statement to perform the storage of the converted result</returns>
    protected override CodeStatement GetConvertedResultStoreStatement(GenerationContext pContext, CodeExpression pResult, CodeExpression pPosition, CodeExpression pConverted) {
      return new CodeExpressionStatement(new CodeMethodInvokeExpression(pResult, "Add", pConverted));
    }
    #endregion
  }
}
