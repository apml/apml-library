/// Copyright 2007 Faraday Media
/// 
/// Licensed under the Apache License, Version 2.0 (the "License"); 
/// you may not use this file except in compliance with the License. 
/// You may obtain a copy of the License at 
/// 
///   http://www.apache.org/licenses/LICENSE-2.0 
/// 
/// Unless required by applicable law or agreed to in writing, software 
/// distributed under the License is distributed on an "AS IS" BASIS, 
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
/// See the License for the specific language governing permissions and 
/// limitations under the License.
using System.Xml;
using System.Xml.Serialization;

namespace APML {
  public interface IApplication {
    /// <summary>
    /// Name of the Application
    /// </summary>
    [XmlAttribute]
    string Name { get; }

    /// <summary>
    /// Returns whether the given item exists.
    /// </summary>
    /// <param name="pElName">the name of the element</param>
    /// <param name="pMatchKey">the key to match</param>
    /// <returns></returns>
    bool HasItem(string pElName, string pMatchKey);

    /// <summary>
    /// Adds an item with the given element name.
    /// </summary>
    /// <param name="pElName">the element name</param>
    /// <param name="pKeyName">the key name for the element</param>
    /// <returns>the created item</returns>
    IGenericItem AddItem(string pElName, string pKeyName);

    /// <summary>
    /// Retrieves the list of available items.
    /// </summary>
    IReadOnlyDictionary<string, IGenericItem> Items { get; }

    /// <summary>
    /// The xml document owning this application.
    /// </summary>
    XmlDocument OwnerDocument { get; }

    /// <summary>
    /// The child nodes of this application.
    /// </summary>
    XmlNodeList ChildNodes { get; }

    /// <summary>
    /// Adds the given child node.
    /// </summary>
    /// <param name="pChild">the child</param>
    void AppendChild(XmlNode pChild);

    /// <summary>
    /// Removes the given child node.
    /// </summary>
    /// <param name="pChild"></param>
    void RemoveChild(XmlNode pChild);
  }
}