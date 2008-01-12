using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;

namespace APML.AutoWrapper {
  /// <summary>
  /// Base class used when generating AutoWrapper classes.
  /// </summary>
  public class AutoWrapperBase : IAutoWrapper {
    private readonly XmlElement mElement;
    private readonly AutoWrapperGenerator mWrapperGenerator;

    /// <summary>
    /// Creates a new AutoWrapper base that wraps the given node.
    /// </summary>
    /// <param name="pElement">the element</param>
    /// <param name="pGenerator">
    ///   the generator used to create this wrapper (to help support As calls), 
    ///   or null if generator is not available
    /// </param>
    public AutoWrapperBase(XmlElement pElement, AutoWrapperGenerator pGenerator) {
      mElement = pElement;
      mWrapperGenerator = pGenerator;
    }

    #region IAutoWrapper Members
    /// <summary>
    /// Requests that an implementation of the provided interface is returned for the current node.
    /// This provides the ability to access the node with custom properties, or to apply additional
    /// access methods.
    /// </summary>
    /// <typeparam name="T">the type of the interface that should be implemented</typeparam>
    /// <returns>the interface implemented as an autowrapper</returns>
    public T As<T>() where T : class {
      if (mWrapperGenerator == null) {
        throw new System.NotSupportedException("No generator is available");
      }

      return mWrapperGenerator.GenerateWrapper<T>(mElement);
    }
    #endregion

    /// <summary>
    /// The underlying XML node.
    /// </summary>
    public virtual XmlNode Node {
      get { return mElement; }
    }

    /// <summary>
    /// Retrieves access to the WrapperGenerator in the wrapper.
    /// </summary>
    protected AutoWrapperGenerator WrapperGenerator {
      get { return mWrapperGenerator; }
    }

    /// <summary>
    /// Retrieves the given attribute, or its default value if it doesn't exist.
    /// </summary>
    /// <param name="pAttrName"></param>
    /// <param name="pDefaultValue"></param>
    /// <returns></returns>
    protected T GetAttributeOrDefault<T>(string pAttrName, T pDefaultValue) {
      XmlAttribute attribute = mElement.Attributes[pAttrName];
      if (attribute == null) {
        return pDefaultValue;
      }

      return (T) TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(attribute.InnerText);
    }

    /// <summary>
    /// Sets the given attribute with the given value.
    /// </summary>
    /// <typeparam name="T">the type of the attribute</typeparam>
    /// <param name="pAttrName">the name of the attribute</param>
    /// <param name="pValue">the new value</param>
    protected void SetAttribute<T>(string pAttrName, object pValue) {
      XmlAttribute attribute = mElement.Attributes[pAttrName];
      if (attribute == null) {
        attribute = mElement.OwnerDocument.CreateAttribute(pAttrName);
        mElement.Attributes.Append(attribute);
      }

      attribute.InnerText = TypeDescriptor.GetConverter(typeof(T)).ConvertToString(pValue);
    }

    /// <summary>
    /// Retrives the content of the given element.
    /// </summary>
    /// <typeparam name="T">the type of the element</typeparam>
    /// <param name="pElName">the name of the element</param>
    /// <param name="pElNamespace">the element namespace</param>
    /// <param name="pDefaultValue">the default value</param>
    /// <returns>the value, or the default</returns>
    protected T GetElementOrDefault<T>(string pElName, string pElNamespace, T pDefaultValue) {
      XmlElement element = FindElement(pElName, pElNamespace, false);
      if (element == null) {
        return pDefaultValue;
      }

      return (T) TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(element.InnerText);
    }

    /// <summary>
    /// Sets the value of a child element.
    /// </summary>
    /// <param name="pElName">the name of the element</param>
    /// <param name="pElNamespace">the element namespace</param>
    /// <param name="pValue">the new value of the element</param>
    protected void SetElement<T>(string pElName, string pElNamespace, T pValue) {
      XmlElement element = FindElement(pElName, pElNamespace, true);
      element.InnerText = TypeDescriptor.GetConverter(typeof(T)).ConvertToString(pValue);
    }

    /// <summary>
    /// Retrieves all element with the given element name.
    /// </summary>
    /// <param name="pContainerName">
    ///   the name of the container for these elements, or null if they 
    ///   should be found directly under the current node
    /// </param>
    /// <param name="pContainerNamespace">the namespace of the container</param>
    /// <param name="pElName">the element name</param>
    /// <param name="pElNamespace">expected namespace for the element</param>
    /// <returns>the list of elements</returns>
    protected XmlElement[] GetAllElements(string pContainerName, string pContainerNamespace, string pElName, string pElNamespace) {
      XmlNode searchNode = mElement;
      if (pContainerName != null) {
        searchNode = FindElement(pContainerName, pContainerNamespace, false);
      }

      List<XmlElement> result = new List<XmlElement>();
      foreach (XmlNode node in searchNode.ChildNodes) {
        if (node.NodeType == XmlNodeType.Element && node.Name == pElName && CompareNamespace(node.NamespaceURI, pElNamespace)) {
          result.Add((XmlElement) node);
        }
      }

      return result.ToArray();
    }

    /// <summary>
    /// Attempts to find a given child element.
    /// </summary>
    /// <param name="pElName">the name of the element</param>
    /// <param name="pElNamespace">the namespace of the element</param>
    /// <param name="pCanCreate">whether the element can be created</param>
    /// <returns>the found (or created) element, or null if it was found and couldn't be created</returns>
    protected XmlElement FindElement(string pElName, string pElNamespace, bool pCanCreate) {
      foreach (XmlNode node in mElement.ChildNodes) {
        if (node.NodeType == XmlNodeType.Element && node.Name == pElName && CompareNamespace(node.NamespaceURI, pElNamespace)) {
          return (XmlElement) node;
        }
      }

      if (pCanCreate) {
        XmlElement el = mElement.OwnerDocument.CreateElement(pElName, pElNamespace);
        mElement.AppendChild(el);

        return el;
      }

      return null;
    }

    /// <summary>
    /// Removes the given element.
    /// </summary>
    /// <param name="pElName">the name of the lement</param>
    /// <param name="pElNamespace">the namespace of the element</param>
    protected void ClearElement(string pElName, string pElNamespace) {
      XmlElement element = FindElement(pElName, pElNamespace, false);
      element.ParentNode.RemoveChild(element);
    }

    /// <summary>
    /// Initialises the given element, adding it if necessary.
    /// </summary>
    /// <param name="pElName">the name of the element</param>
    /// <param name="pElNamespace">the namespace of the element</param>
    /// <returns>the element</returns>
    protected XmlElement InitElement(string pElName, string pElNamespace) {
      return FindElement(pElName, pElNamespace, true);
    }

    /// <summary>
    /// Adds a new element with the given name either to this node (for a null container), or the given
    /// container element.
    /// </summary>
    /// <param name="pContainerName">the name of the container element, or null if the element belongs directly under this element</param>
    /// <param name="pContainerNamespace">the namespace of the container element</param>
    /// <param name="pElName">the element name</param>
    /// <param name="pElNamespace">the namespace of the element</param>
    /// <returns>the created element</returns>
    protected XmlElement AddElement(string pContainerName, string pContainerNamespace, string pElName, string pElNamespace) {
      XmlElement container = pContainerName != null ? FindElement(pContainerName, pContainerNamespace, true) : mElement;
      XmlElement result = container.OwnerDocument.CreateElement(pElName, pElNamespace);
      container.AppendChild(result);

      return result;
    }

    /// <summary>
    /// Compares a node namespace and an expected namespace.
    /// </summary>
    /// <param name="pNodeNamespace">the namespace of the declared node</param>
    /// <param name="pExpectedNamespace">the expected namespace</param>
    /// <returns>true - the namespaces are identitical</returns>
    private bool CompareNamespace(string pNodeNamespace, string pExpectedNamespace) {
      if (pNodeNamespace == pExpectedNamespace) {
        return true;
      }
      if (pNodeNamespace == string.Empty && pExpectedNamespace == null) {
        return true;
      }

      return false;
    }
  }
}
