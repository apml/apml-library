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
using System.Xml;
using APML.XmlWrappers.Common;

namespace APML.XmlWrappers.v0_6 {
  public class XmlImplicitNodeHelper<T> : XmlWrapperBase
      where T : IImplicitAttention {
    private T mOwner;

    public XmlImplicitNodeHelper(APMLFileBase pFile, XmlNode pNode, T pOwner)
      : base(pFile, pNode) {
      mOwner = pOwner;
    }

    #region IImplicitAttention Members
    public string From {
      get { return GetAttribute("from"); }
      set { FireFromChanged(SetAttribute("from", value), value); }
    }

    public DateTime? Updated {
      get { return GetAttributeAsDateTime("updated"); }
      set { FireUpdatedChanged(SetAttributeAsDateTime("updated", value), value); }
    }
    #endregion

    protected void FireFromChanged(string pOld, string pNew) {
      // TODO: If needed
    }

    protected void FireUpdatedChanged(DateTime? pOld, DateTime? pNew) {
      // TODO: If needed
    }
  }
}