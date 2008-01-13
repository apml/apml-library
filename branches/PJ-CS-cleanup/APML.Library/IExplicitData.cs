using System.Xml.Serialization;
using APML.AutoWrapper;

namespace APML {
  /// <summary>
  /// Container for all explicit data in the APML.
  /// </summary>
  public interface IExplicitData {
    /// <summary>
    /// The explicit concepts attached to the profile.
    /// </summary>
    [XmlArray("Concepts", Namespace = APMLConstants.NAMESPACE_0_6)]
    [XmlArrayItem("Concept", Namespace = APMLConstants.NAMESPACE_0_6)]
    [AutoWrapperKey("Key")]
    [AutoWrapperAutoInit]
    IReadOnlyDictionary<string, IExplicitConcept> ExplicitConcepts { get; }

    /// <summary>
    /// The explicit source attached to the profile.
    /// </summary>
    [XmlArray("Sources", Namespace = APMLConstants.NAMESPACE_0_6)]
    [XmlArrayItem("Source", Namespace = APMLConstants.NAMESPACE_0_6)]
    [AutoWrapperKey("Key")]
    [AutoWrapperAutoInit]
    IReadOnlyDictionary<string, IExplicitSource> ExplicitSources { get; }

    /// <summary>
    /// Adds a new explicit concept to the user's profile.
    /// </summary>
    /// <param name="key">the key of the concept</param>
    /// <param name="value">the value of the concept</param>
    /// <returns>the generated explicit concept</returns>
    IExplicitConcept AddExplicitConcept(string key, double value);

    /// <summary>
    /// Adds a new explicit source to the user's profile.
    /// </summary>
    /// <param name="key">the key for the source</param>
    /// <param name="value">the value for this source</param>
    /// <param name="name">the friendly name for the source</param>
    /// <param name="type">the type of the source, expressed as a mime-type</param>
    /// <returns>the generated explicit source</returns>
    IExplicitSource AddExplicitSource(string key, double value, string name, string type);
  }
}
