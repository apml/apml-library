using System;
using System.Collections.Generic;
using System.Text;

namespace APML {
  /// <summary>
  /// Container for all explicit data in the APML.
  /// </summary>
  public interface IExplicitData {
    /// <summary>
    /// Adds a new explicit concept to the user's profile.
    /// </summary>
    /// <param name="pKey">the key of the concept</param>
    /// <param name="pValue">the value of the concept</param>
    /// <returns>the generated explicit concept</returns>
    IExplicitConcept AddExplicitConcept(string pKey, double pValue);

    /// <summary>
    /// Adds a new explicit source to the user's profile.
    /// </summary>
    /// <param name="pKey">the key for the source</param>
    /// <param name="pValue">the value for this source</param>
    /// <param name="pName">the friendly name for the source</param>
    /// <param name="pType">the type of the source, expressed as a mime-type</param>
    /// <returns>the generated explicit source</returns>
    IExplicitSource AddExplicitSource(string pKey, double pValue, string pName, string pType);
  }
}
