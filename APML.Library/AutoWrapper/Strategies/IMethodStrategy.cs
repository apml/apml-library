using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace APML.AutoWrapper.Strategies {
  ///<summary>
  /// Defines a strategy for building a method.
  ///</summary>
  public interface IMethodStrategy : IStrategy {
    /// <summary>
    /// Checks whether the given strategy applies to a property.
    /// </summary>
    /// <param name="pMethod">the method to check</param>
    /// <returns>true - strategies apply</returns>
    bool AppliesToMethod(MethodInfo pMethod);

    /// <summary>
    /// Applies the given strategy to the given generated method.
    /// </summary>
    /// <param name="pContext">the context of the generation</param>
    /// <param name="pMethod">the declared method</param>
    /// <param name="pGeneratedMethod">the method being generated</param>
    /// <param name="pClass">the class being declared</param>
    void Apply(GenerationContext pContext, MethodInfo pMethod, CodeMemberMethod pGeneratedMethod, CodeTypeDeclaration pClass);
  }
}
