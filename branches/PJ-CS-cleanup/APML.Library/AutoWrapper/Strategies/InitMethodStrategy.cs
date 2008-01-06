using System.CodeDom;
using System.Reflection;
using System.Xml;

namespace APML.AutoWrapper.Strategies {
  /// <summary>
  /// Strategy for generating init methods.
  /// </summary>
  public class InitMethodStrategy : BaseStrategy, IMethodStrategy {
    #region IStrategy Members
    /// <summary>
    /// Defines the priority of the strategy.
    /// </summary>
    public override StrategyPriority Priority {
      get { return StrategyPriority.BaseCode; }
    }
    #endregion    

    #region IMethodStrategy Members
    /// <summary>
    /// Checks whether the given strategy applies to method by checking whether the name of the method starts with
    /// "Init" and has at least one parameter.
    /// </summary>
    /// <param name="pMethod">the method to check</param>
    /// <returns>true - strategies apply</returns>
    public bool AppliesToMethod(MethodInfo pMethod) {
      return pMethod.Name.StartsWith("Init") && pMethod.GetParameters().Length > 0;
    }

    /// <summary>
    /// Applies the given strategy to the given generated method by creating an "Init" method which accepts the given
    /// parameters.
    /// </summary>
    /// <param name="pContext">the context of the generation</param>
    /// <param name="pMethod">the declared method</param>
    /// <param name="pGeneratedMethod">the method being generated</param>
    /// <param name="pClass">the class being declared</param>
    public void Apply(GenerationContext pContext, MethodInfo pMethod, CodeMemberMethod pGeneratedMethod,
                      CodeTypeDeclaration pClass) {
      // Generate the base init
      CodeExpression baseGenerate = new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), pGeneratedMethod.Name);
      pGeneratedMethod.Statements.Add(baseGenerate);

      // Build the property applications
      PropertyInfo prop = MethodHelper.FindRelevantProperty(pMethod, "Init");
      CodeExpression propertyRef = new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), prop.Name);
      pGeneratedMethod.Statements.AddRange(MethodHelper.GeneratePropertyApplications(pMethod, prop.PropertyType, propertyRef));

      // If the return type is not void, then return the property
      if (pMethod.ReturnType != typeof(void)) {
        pGeneratedMethod.Statements.Add(new CodeMethodReturnStatement(propertyRef));
      }
    }
    #endregion

    
  }
}
