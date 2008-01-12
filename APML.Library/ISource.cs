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
using System.Xml.Serialization;

namespace APML {
  /// <summary>
  /// Delegate for indicating that a source type has changed.
  /// </summary>
  /// <param name="pSource">the source</param>
  /// <param name="pOldType">the old type</param>
  /// <param name="pNewType">the new type</param>
  public delegate void SourceTypeChangedEventHandler(ISource pSource, string pOldType, string pNewType);

  /// <summary>
  /// Delegate for indicating that a source name has changed.
  /// </summary>
  /// <param name="pSource">the source</param>
  /// <param name="pOldType">the old name</param>
  /// <param name="pNewType">the new name</param>
  public delegate void SourceNameChangedEventHandler(ISource pSource, string pOldType, string pNewType);

  /// <summary>
  /// Identifies an APML source, such as a website.
  /// </summary>
  public interface ISource : IAPMLComponent {
    /// <summary>
    /// The type of this source. Expressed as a mime-type (such as application/rss+xml for example).
    /// </summary>
    [XmlAttribute("type")]
    string Type { get; set; }

    /// <summary>
    /// The friendly-name of this source.
    /// </summary>
    [XmlAttribute("name")]
    string Name { get; set; }

    /// <summary>
    /// Raised when the type changes.
    /// </summary>
//    event SourceTypeChangedEventHandler TypeChanged;

    /// <summary>
    /// Raised when the source name changes.
    /// </summary>
//    event SourceNameChangedEventHandler NameChanged;
  }
}
