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
  /// Delegate for the ValueChanged event
  /// </summary>
  /// <param name="Device">The device where the Value is changed</param>
  public delegate void DeviceValueChangedEventHandler(IDevice Device);

  public interface IDevice : IAPMLComponent {
    /// <summary>
    /// Fires when a Value in the object is changed
    /// </summary>
    event DeviceValueChangedEventHandler ValueChanged;

    /// <summary>
    /// ID of the Device
    /// </summary>
    string ID { get; set; }

    /// <summary>
    /// Name of the Device
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Type of the Device
    /// </summary>
    string Type { get; set; }
  }
}