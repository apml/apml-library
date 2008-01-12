using System;
using System.CodeDom;

namespace APML.AutoWrapper.Strategies {
  /// <summary>
  /// Base class implemented by all strategies.
  /// </summary>
  public abstract class BaseStrategy : IStrategy {
    #region IStrategy Members
    public abstract StrategyPriority Priority { get; }
    #endregion

    /// <summary>
    /// Creates the CreateObjectExpression that can be used for assigning the element.
    /// </summary>
    /// <param name="pContext">the context of the generation</param>
    /// <param name="pReturnType">the interface type being generated to</param>
    /// <param name="pElementExpression">the expression used to fetch the element</param>
    /// <returns>an expression for creating the wrapper object</returns>
    protected CodeExpression CreateCreateObjectExpression(GenerationContext pContext, Type pReturnType, CodeExpression pElementExpression) {
      return new CodeObjectCreateExpression(
        pContext.LookupRequiredTypeName(pReturnType), 
        pElementExpression,
        new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "WrapperGenerator"));
    }
  }
}
