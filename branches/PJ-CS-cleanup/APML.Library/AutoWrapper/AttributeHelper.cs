using System;
using System.Reflection;
using System.Xml.Serialization;

namespace APML.AutoWrapper {
  /// <summary>
  /// Helper for working with custom class metadata.
  /// </summary>
  public static class AttributeHelper {
    /// <summary>
    /// Retrieves the value of an attribute that exists 0 to 1 times on a given attribute provider.
    /// </summary>
    /// <typeparam name="T">the type of the attribute required</typeparam>
    /// <param name="pProp">the provider (PropertyInfo, MethodInfo, etc) to check</param>
    /// <returns>the property instance, or null if it wasn't found</returns>
    public static T GetAttribute<T>(ICustomAttributeProvider pProp) where T : Attribute {
      object[] attrs = pProp.GetCustomAttributes(typeof(T), true);
      if (attrs.Length == 0) {
        return null;
      }

      return (T)attrs[0];
    }

    /// <summary>
    /// Works out the correct name for the XmlElement.
    /// </summary>
    /// <param name="pProp">the property to inspect</param>
    /// <returns>the element name</returns>
    public static string SelectXmlElementName(MemberInfo pProp) {
      XmlElementAttribute elTag = GetAttribute<XmlElementAttribute>(pProp);
      if (elTag.ElementName != null && elTag.ElementName != string.Empty) {
        return elTag.ElementName;
      }

      return pProp.Name;
    }

    /// <summary>
    /// Works out the correct namespace for the XmlElement.
    /// </summary>
    /// <param name="pProp">the property to inspect</param>
    /// <returns>the element namespace</returns>
    public static string SelectXmlElementNamespace(MemberInfo pProp) {
      XmlElementAttribute elTag = GetAttribute<XmlElementAttribute>(pProp);
      return elTag.Namespace;
    }
  }
}
