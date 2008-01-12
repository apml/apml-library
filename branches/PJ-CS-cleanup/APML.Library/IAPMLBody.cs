using System.Collections.Generic;
using System.Xml.Serialization;
using APML.AutoWrapper;

namespace APML {
  /// <summary>
  /// The body of the APML document.
  /// </summary>
  public interface IAPMLBody {
    /// <summary>
    /// The default profile to use for the user.
    /// </summary>
    [XmlAttribute("defaultprofile", Namespace =  APMLConstants.NAMESPACE_0_6)]
    string DefaultProfile { get; set; }

    /// <summary>
    /// The profiles in the document.
    /// </summary>
    [XmlElement("Profile", Namespace = APMLConstants.NAMESPACE_0_6)]
    [XmlArray(Namespace = APMLConstants.NAMESPACE_0_6)]
    [AutoWrapperKey("Name")]
    IReadOnlyDictionary<string, IProfile> Profiles { get; }

    /// <summary>
    /// Adds a new profile with the given name.
    /// </summary>
    /// <param name="name">the name of the profile</param>
    /// <returns>the created profile</returns>
    IProfile AddProfile(string name);

    /// <summary>
    /// The applications registered in the document.
    /// </summary>
//    [XmlElement]
//    [XmlArray("Applications")]
//    IList<IApplication> Applications { get; }
  }
}
