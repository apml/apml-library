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
using System.Xml.Serialization;
using APML.AutoWrapper;

namespace APML {
  /// <summary>
  /// Base set of properties that are present in all nodes that handle implicit attention.
  /// </summary>
  public interface IImplicitAttention {
    /// <summary>
    /// The time this implicit record was last updated.
    /// </summary>
    [XmlAttribute("updated", Namespace = APMLConstants.NAMESPACE_0_6)]
    [AutoWrapperFieldConverter(typeof(APMLDateConverter))]
    [AutoWrapperAutoInit]
    DateTime? Updated { get; set; }

    /// <summary>
    /// The tool/source that provided this item.
    /// </summary>
    [XmlAttribute("from", Namespace = APMLConstants.NAMESPACE_0_6)]
    [AutoWrapperAutoInit]
    string From { get; set; }
  }
}
