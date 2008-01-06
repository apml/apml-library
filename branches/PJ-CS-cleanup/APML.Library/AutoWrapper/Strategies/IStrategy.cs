using System;
using System.Collections.Generic;
using System.Text;

namespace APML.AutoWrapper.Strategies {
  ///<summary>
  /// Base interface supported by all strategies.
  ///</summary>
  public interface IStrategy {
    /// <summary>
    /// Defines the priority of the strategy.
    /// </summary>
    StrategyPriority Priority { get; }
  }

  /// <summary>
  /// Defines the various priorities of strategies.
  /// </summary>
  public enum StrategyPriority {
    /// <summary>
    /// Provides the base code for the implementation
    /// </summary>
    BaseCode,

    /// <summary>
    /// Defines code that helps with issues such as caching.
    /// </summary>
    CachingCode,

    /// <summary>
    /// Defines code that may provide guarding functions (eg locks)
    /// </summary>
    GuardCode
  }
}
