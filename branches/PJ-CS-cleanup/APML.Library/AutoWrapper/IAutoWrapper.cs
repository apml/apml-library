using System;
using System.Collections.Generic;
using System.Text;

namespace APML.AutoWrapper {
  ///<summary>
  /// Interface that all auto-wrappers can implement in order to provide additional features.
  ///</summary>
  public interface IAutoWrapper {
    /// <summary>
    /// Requests that an implementation of the provided interface is returned for the current node.
    /// This provides the ability to access the node with custom properties, or to apply additional
    /// access methods.
    /// </summary>
    /// <typeparam name="T">the type of the interface that should be implemented</typeparam>
    /// <returns>the interface implemented as an autowrapper</returns>
    T As<T>() where T : class;
  }
}
