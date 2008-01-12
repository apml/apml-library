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
using APML.AutoWrapper;

namespace APML {
  /// <summary>
  /// An explicit source.
  /// </summary>
  public interface IExplicitSource : IAttention<IExplicitSource>, ISource {
    /// <summary>
    /// Creates a new author for this source.
    /// </summary>
    /// <param name="key">the key for the author</param>
    /// <param name="value">the value for the author</param>
    IExplicitAuthor AddAuthor(string key, double value);

    /// <summary>
    /// The authors attached to this source.
    /// </summary>
    [XmlElement("Author", Namespace = APMLConstants.NAMESPACE_0_6)]
    [XmlArray("Authors", Namespace = APMLConstants.NAMESPACE_0_6)]
    [AutoWrapperKey("Key")]
    IReadOnlyDictionary<string, IExplicitAuthor> Authors { get; }
  }
}
