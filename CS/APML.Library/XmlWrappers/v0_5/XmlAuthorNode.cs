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
using System.Globalization;
using System.Text;
using System.Xml;
using APML.XmlWrappers.Common;

namespace APML.XmlWrappers.v0_5 {
  public class XmlAuthorNode : XmlAttentionNodeBase<IExplicitAuthor>, IExplicitAuthor {
    public XmlAuthorNode(APMLFileBase pFile, XmlNode pNode) : base(pFile, pNode) {
    }

    #region IAttention Members
    public override string Key {
      get { return GetAttribute("Value"); }
      set { FireKeyChanged(SetAttribute("Value", value), value); }
    }

    public override double Value {
      get { return double.Parse(GetAttribute("Rank"))/5.0; }
      set { FireValueChanged(SetAttribute("Rank", (value * 5.0).ToString("f2", CultureInfo.InvariantCulture)), value * 5.0); }
    }
    #endregion
  }
}
