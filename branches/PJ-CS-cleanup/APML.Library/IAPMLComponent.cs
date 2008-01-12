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
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace APML {
  /// <summary>
  /// Delegate indicating that an APML component has been removed.
  /// </summary>
  /// <param name="pComponent">the component that was removed.</param>
  public delegate void APMLComponentRemovedHandler(IAPMLComponent pComponent);

  public interface IAPMLComponent {
    /// <summary>
    /// Removes the named APML item.
    /// </summary>
//    void Remove();

    /// <summary>
    /// Fired when the component is removed from the document.
    /// </summary>
//    event APMLComponentRemovedHandler Removed;

    /// <summary>
    /// Sets an attribute on the component with the given value.
    /// </summary>
    /// <param name="pAttrName">the name of the attribute</param>
    /// <param name="pAttrValue">the value of the attribute</param>
//    void SetExtraAttribute(string pAttrName, string pAttrValue);

    /// <summary>
    /// Removes the attribute with the given name.
    /// </summary>
    /// <param name="pAttrName">the name of the attribute</param>
//    void ClearExtraAttribute(string pAttrName);

    /// <summary>
    /// Retrieves the attribute with the given name.
    /// </summary>
    /// <param name="pAttrName">the name of the attribute</param>
    /// <returns>the value of the attribute, or null if the attribute isn't set</returns>
//    string GetExtraAttribute(string pAttrName);

    /// <summary>
    /// Provides access to the underlying XmlNode.
    /// </summary>
    XmlNode Node { get; }
  }
}
