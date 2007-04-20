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
using APML.XmlWrappers.Common;

namespace APML.XmlWrappers.v0_5 {
  public abstract class XmlAttentionNodeBase<T> 
      : XmlAPMLComponentBase, IAttention<T>
      where T : class, IAttention<T> {
    public XmlAttentionNodeBase(APMLFileBase pFile, XmlNode pNode) : base(pFile, pNode) {
    }

    #region IAttention Members
    public abstract string Key { get; set; }
    public abstract double Value { get; set; }

    public event KeyChangedEventHandler<T> KeyChanged;
    public event ValueChangedEventHandler<T> ValueChanged;
    #endregion

    protected void FireKeyChanged(string pOld, string pNew) {
      if (pOld == null) {
        return;
      }

      KeyChangedEventHandler<T> handler = KeyChanged;

      if (handler != null) {
        handler(this as T, pOld, pNew);
      }
    }

    protected void FireValueChanged(string pOld, double pNew) {
      if (pOld == null) {
        return;
      }

      ValueChangedEventHandler<T> handler = ValueChanged;

      if (handler != null) {
        handler(this as T, double.Parse(pOld), pNew);
      }
    } 
  }
}
