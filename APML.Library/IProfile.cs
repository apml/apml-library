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
using System.Xml.Serialization;
using APML.AutoWrapper;

namespace APML {
  public delegate void ProfileNameChangedEventHandler(IProfile pSender, string pOldName, string pNewName);

  public delegate void PropertyChangeHandler<T, U>(T pSender, U pOldValue, U pNewValue);

  /// <summary>
  /// Defines the object model for an APML profile.
  /// </summary>
  public interface IProfile : IAPMLComponent {
    /// <summary>
    /// The name of the profile.
    /// </summary>
    [XmlAttribute("name", Namespace = APMLConstants.NAMESPACE_0_6)]
    string Name { get; set; }

    /// <summary>
    /// The implicit data for the profile.
    /// </summary>
    [XmlElement(Namespace = APMLConstants.NAMESPACE_0_6)]
    [AutoWrapperAutoInit]
    IImplicitData ImplicitData { get; }

    /// <summary>
    /// The explicit data for the profile.
    /// </summary>
    [XmlElement(Namespace = APMLConstants.NAMESPACE_0_6)]
    [AutoWrapperAutoInit]
    IExplicitData ExplicitData { get; }

    /// <summary>
    /// Event issued when the name of the profile changes.
    /// </summary>
//    event PropertyChangeHandler<IProfile, string> NameChanged;
  }
}