using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace APML.AutoWrapper.Strategies {
  public class ClearMethodStrategy : BaseStrategy, IMethodStrategy {
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
    /// Checks whether the given strategy applies to a property by checking if the method name start with "Add".
    /// </summary>
    /// <param name="pMethod">the method to check</param>
    /// <returns>true - strategies apply</returns>
    public bool AppliesToMethod(MethodInfo pMethod) {
      return pMethod.Name.StartsWith("Clear");
    }

    /// <summary>
    /// Applies the given strategy to the given generated method by creating an "Add" method which accepts the given
    /// parameters.
    /// </summary>
    /// <param name="pContext">the context of the generation</param>
    /// <param name="pMethod">the declared method</param>
    /// <param name="pGeneratedMethod">the method being generated</param>
    /// <param name="pClass">the class being declared</param>
    public void Apply(GenerationContext pContext, MethodInfo pMethod, CodeMemberMethod pGeneratedMethod,
                      CodeTypeDeclaration pClass) {
      if (pMethod.ReturnType != typeof(void)) {
        throw new ArgumentException(pMethod.Name + " must return void");
      }

      // Find the property, and see if it has a key value
      PropertyInfo prop = MethodHelper.FindRelevantProperty(pMethod, "Clear");
      if (prop == null) {
        throw new ArgumentException("No matching property found for method " + pClass.Name + "." + pMethod.Name);
      }
      AutoWrapperKeyAttribute keyAttr = AttributeHelper.GetAttribute<AutoWrapperKeyAttribute>(prop);

      // Generate the call parameters
      /*CodeExpression[] baseParameters = new CodeExpression[0];
      if (keyAttr != null) {
        // Find the parameter that matches the base parameter name
        foreach (CodeParameterDeclarationExpression param in pGeneratedMethod.Parameters) {
          if (param.Name.ToLower() == keyAttr.KeyAttribute.ToLower()) {
            baseParameters = new CodeExpression[] { new CodeVariableReferenceExpression(param.Name) };
            break;
          }
        }
      }

      // Generate the base variable
      CodeVariableDeclarationStatement baseGenerate =
        new CodeVariableDeclarationStatement(
          pMethod.ReturnType,
          "result",
          new CodeMethodInvokeExpression(
            new CodeThisReferenceExpression(),
            pGeneratedMethod.Name,
            baseParameters));
      pGeneratedMethod.Statements.Add(baseGenerate);

      // Build the property applications
      pGeneratedMethod.Statements.AddRange(MethodHelper.GeneratePropertyApplications(pMethod, pMethod.ReturnType, new CodeVariableReferenceExpression("result")));

      // Return the result if necessary
      pGeneratedMethod.Statements.Add(new CodeMethodReturnStatement(new CodeVariableReferenceExpression("result")));*/
      // TODO: Implement
    }

    #endregion
  }
}
