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
using System.ComponentModel;
using System.Xml.Serialization;
using APML.AutoWrapper;

namespace APML {
  /// <summary>
  /// Delegate method for the NameChanged event
  /// </summary>
  /// <param name="pSender">source of this event</param>
  /// <param name="pOldName">Old Name of the object</param>
  /// <param name="pNewName">New Name of the object</param>
  public delegate void KeyChangedEventHandler<T>(T pSender, string pOldName, string pNewName);

  /// <summary>
  /// Delegate method for the ValueChanged event
  /// </summary>
  /// <param name="pSender">source of this event</param>
  /// <param name="pOldValue">old value of the object</param>
  /// <param name="pNewValue">new value of the object</param>
  public delegate void ValueChangedEventHandler<T>(T pSender, double pOldValue, double pNewValue);

  /// <summary>
  /// Defines the base object model for any attention management element
  /// within the APML file.
  /// </summary>
  public interface IAttention<T> : IAPMLComponent {
    /// <summary>
    /// The key for this attention item.
    /// </summary>
    [XmlAttribute("key", Namespace = APMLConstants.NAMESPACE_0_6)]
    string Key { get; set; }

    /// <summary>
    /// The value for this attention item.
    /// </summary>
    [XmlAttribute("value", Namespace = APMLConstants.NAMESPACE_0_6)]
    [AutoWrapperFieldConverter(typeof(APMLNumberConverter))]
    double Value { get; set; }

    /// <summary>
    /// Fires when the Key of the object changes
    /// </summary>
//    event KeyChangedEventHandler<T> KeyChanged;    

    /// <summary>
    /// Fires when the Value of the object changes
    /// </summary>
//    event ValueChangedEventHandler<T> ValueChanged;
  }
}