using System.CodeDom;
using System.Reflection;

namespace APML.AutoWrapper.Strategies {
  /// <summary>
  /// A property strategy provides code generation features for working with properties.
  /// </summary>
  public interface IPropertyStrategy : IStrategy {
    /// <summary>
    /// Checks whether the given strategy applies to a property.
    /// </summary>
    /// <param name="pProp">the property to check</param>
    /// <returns>true - strategies apply</returns>
    bool AppliesToProperty(PropertyInfo pProp);

    /// <summary>
    /// Applies the given strategy to the given generated property.
    /// </summary>
    /// <param name="pContext">the context of the generation</param>
    /// <param name="pProp">the declared property</param>
    /// <param name="pGeneratedProp">the property being generated</param>
    /// <param name="pClass">the class being declared</param>
    void Apply(GenerationContext pContext, PropertyInfo pProp, CodeMemberProperty pGeneratedProp, CodeTypeDeclaration pClass);
  }
}
