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
namespace APML {
  /// <summary>
  /// Delegate called when a value on a generic item changes.
  /// </summary>
  /// <param name="pItem"></param>
  /// <param name="pKey"></param>
  /// <param name="pValue"></param>
  /// <returns></returns>
  public delegate void GenericItemValueChangedEventHandler(IGenericItem pItem, string pKey, string pValue);

  public interface IGenericItem : IAPMLComponent {
    /// <summary>
    /// Fires when a Value is changed
    /// </summary>
    event GenericItemValueChangedEventHandler ValueChanged;

    /// <summary>
    /// Name of the Item
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Retrieves the value for the given key.
    /// </summary>
    /// <param name="pKey">the key</param>
    /// <returns>the value</returns>
    string GetValue(string pKey);

    /// <summary>
    /// Sets the value for the given key.
    /// </summary>
    /// <param name="pKey">the key</param>
    /// <param name="pValue">the value</param>
    void SetValue(string pKey, string pValue);

    /// <summary>
    /// Removes the given value.
    /// </summary>
    /// <param name="pKey"></param>
    void RemoveValue(string pKey);
  }
}