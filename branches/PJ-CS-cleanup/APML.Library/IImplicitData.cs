using System.Collections.Generic;
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
    /// <param name="key">the key of the concept</param>
    /// <param name="value">the value of the concept</param>
    /// <param name="from">the application the concept originated from</param>
    /// <returns>the generated implicit concept</returns>
    IImplicitConcept AddImplicitConcept(string key, double value, string from);

    /// <summary>
    /// Adds a new implicit source to the user's profile. Note that the
    /// Updated and 
    /// </summary>
    /// <param name="key">the key for the source</param>
    /// <param name="value">the value for this source</param>
    /// <param name="name">the friendly name for the source</param>
    /// <param name="type">the type of the source, expressed as a mime-type</param>
    /// <param name="from">the application the concept originated from</param>
    /// <returns>the generated implicit source</returns>
    IImplicitSource AddImplicitSource(string key, double value, string name, string type, string from);

    /// <summary>
    /// Clears the user's set of implicit concepts.
    /// </summary>
    void ClearImplicitConcepts();

    /// <summary>
    /// Clears the user's set of implicit concepts, where the provided from tag is matched.
    /// <param name="from">the application to clear the concepts from</param>
    /// </summary>
    void ClearImplicitConcepts(string from);

    /// <summary>
    /// Retrieves all of the implicit concepts managed by this application.
    /// </summary>
    [XmlArray("Concepts", Namespace = APMLConstants.NAMESPACE_0_6)]
    [XmlArrayItem("Concept", Namespace = APMLConstants.NAMESPACE_0_6)]
    [AutoWrapperKey("Key")]
    [AutoWrapperAutoInit]
    IReadOnlyDictionary<string, IList<IImplicitConcept>> ImplicitConcepts { get; }

    /// <summary>
    /// Retrieves all of the user's implicit sources.
    /// </summary>
    [XmlArray("Sources", Namespace = APMLConstants.NAMESPACE_0_6)]
    [XmlArrayItem("Source", Namespace = APMLConstants.NAMESPACE_0_6)]
    [AutoWrapperKey("Key")]
    [AutoWrapperAutoInit]
    IReadOnlyDictionary<string, IList<IImplicitSource>> ImplicitSources { get; }
  }
}
