using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace APML.AutoWrapper.Strategies {
  /// <summary>
  /// Strategy for handling properties that already exist in the base class.
  /// </summary>
  public class BaseClassHandledStrategy : IPropertyStrategy {
    #region IStrategy Members
    public StrategyPriority Priority {
      get { return StrategyPriority.BaseCode; }
    }
    #endregion

    #region IPropertyStrategy Members
    public bool AppliesToProperty(PropertyInfo pProp) {
      return typeof (AutoWrapperBase).GetProperty(pProp.Name) != null;
    }

    public void Apply(GenerationContext pContext, PropertyInfo pProp, CodeMemberProperty pGeneratedProp,
                      CodeTypeDeclaration pClass) {
      pGeneratedProp.Attributes |= MemberAttributes.Override;

      pGeneratedProp.GetStatements.Add(
        new CodeMethodReturnStatement(new CodePropertyReferenceExpression(new CodeBaseReferenceExpression(), pProp.Name)));
    }

    #endregion


  }
}
