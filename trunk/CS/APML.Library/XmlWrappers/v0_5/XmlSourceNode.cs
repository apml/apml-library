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
  public class XmlSourceNode : XmlAttentionNodeBase<IExplicitSource>, IExplicitSource {
    public XmlSourceNode(APMLFileBase pFile, XmlNode pNode) : base(pFile, pNode) {
    }

    #region IAttention Members
    public override string Key {
      get { return GetAttribute("Url"); }
      set { FireKeyChanged(SetAttribute("Url", value), value); }
    }

    public override double Value {
      get { return double.Parse(GetAttribute("Rank")) / 5.0; }
      set { FireValueChanged(SetAttribute("Rank", (value*5.0).ToString("f2")), value*5.0); }
    }
    #endregion

    #region ISource Members

    public string Type {
      get { return ""; }
      set { 
        //Ignored 
      }
    }

    public string Name {
      get { return GetAttribute("Value"); }
      set { FireNameChanged(SetAttribute("Name", value), value); }
    }

    public IExplicitAuthor AddAuthor(string pKey, double pValue) {
      // Ignored
      return null;
    }

    public IReadOnlyDictionary<string, IExplicitAuthor> Authors {
      get { return new ReadOnlyDictionary<string, IExplicitAuthor>(new Dictionary<string, IExplicitAuthor>()); }
    }

    public event SourceTypeChangedEventHandler TypeChanged;
    public event SourceNameChangedEventHandler NameChanged;

    public string Url {
      get { return GetAttribute("Url"); }
      set { FireNameChanged(SetAttribute("Url", value), value); }
    }
    #endregion


    protected void FireNameChanged(string pOld, string pNew) {
      if (pOld == null) {
        return;
      }

      SourceNameChangedEventHandler handler = NameChanged;

      if (handler != null) {
        handler(this, pOld, pNew);
      }
    }
  }
}
