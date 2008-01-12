using System.ComponentModel;
using System.Xml.Serialization;
using APML.AutoWrapper;

namespace APML {
  /// <summary>
  /// The root of an APML document.
  /// </summary>
  public interface IAPMLRoot {
    /// <summary>
    /// The document header.
    /// </summary>
    [XmlElement(Namespace = APMLConstants.NAMESPACE_0_6)]
    [AutoWrapperAutoInit]
    IAPMLHead Head { get; }

    /// <summary>
    /// The document body.
    /// </summary>
    [XmlElement(Namespace = APMLConstants.NAMESPACE_0_6)]
    [AutoWrapperAutoInit]
    IAPMLBody Body { get; }

    /// <summary>
    /// The version of this APML document.
    /// </summary>
    [XmlAttribute("version", Namespace = APMLConstants.NAMESPACE_0_6)]
    [AutoWrapperAutoInit]
    [DefaultValue("0.6")]
    string Version { get; set; }
  }
}
