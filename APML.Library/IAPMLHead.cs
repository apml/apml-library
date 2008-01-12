using System;
using System.Xml.Serialization;
using APML.AutoWrapper;

namespace APML {
  /// <summary>
  /// The header of an APML document.
  /// </summary>
  public interface IAPMLHead {
    /// <summary>
    /// The title of the document.
    /// </summary>
    [XmlElement(Namespace = APMLConstants.NAMESPACE_0_6)]
    [AutoWrapperAutoInit]
    string Title { get; set; }

    /// <summary>
    /// The application that generated the APML.
    /// </summary>
    [XmlElement(Namespace = APMLConstants.NAMESPACE_0_6)]
    [AutoWrapperAutoInit]
    string Generator { get; set; }

    /// <summary>
    /// The email address of the user owning this document.
    /// </summary>
    [XmlElement(Namespace = APMLConstants.NAMESPACE_0_6)]
    [AutoWrapperAutoInit]
    string UserEmail { get; set; }

    /// <summary>
    /// The date the document was generated.
    /// </summary>
    [XmlElement(Namespace = APMLConstants.NAMESPACE_0_6)]
    [AutoWrapperAutoInit]
    [AutoWrapperFieldConverter(typeof(APMLDateConverter))]
    DateTime DateCreated { get; set; }
  }
}
