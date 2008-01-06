using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using APML.AutoWrapper;

namespace APML {
  /// <summary>
  /// Container for Implicit data within a profile.
  /// </summary>
  public interface IImplicitData {
    /// <summary>
    /// Adds a new implicit concept to the user's profile. Note that the
    /// Updated and From fields are dynamically populated.
    /// </summary>
    /// <param name="pKey">the key of the concept</param>
    /// <param name="pValue">the value of the concept</param>
    /// <returns>the generated implicit concept</returns>
    IImplicitConcept AddImplicitConcept(string pKey, double pValue);

    /// <summary>
    /// Adds a new implicit source to the user's profile. Note that the
    /// Updated and 
    /// </summary>
    /// <param name="pKey">the key for the source</param>
    /// <param name="pValue">the value for this source</param>
    /// <param name="pName">the friendly name for the source</param>
    /// <param name="pType">the type of the source, expressed as a mime-type</param>
    /// <returns>the generated implicit source</returns>
    IImplicitSource AddImplicitSource(string pKey, double pValue, string pName, string pType);

    /// <summary>
    /// Retrieves all of the implicit concepts managed by this application.
    /// </summary>
    [XmlArray("Concepts")]
    [XmlArrayItem("Concept")]
    [AutoWrapperKey("Key")]
    IReadOnlyDictionary<string, IList<IImplicitConcept>> ImplicitConcepts { get; }

    /// <summary>
    /// Retrieves all of the user's implicit sources.
    /// </summary>
    [XmlArray("Sources")]
    [XmlArrayItem("Source")]
    [AutoWrapperKey("Key")]
    IReadOnlyDictionary<string, IList<IImplicitSource>> ImplicitSources { get; }
  }
}
