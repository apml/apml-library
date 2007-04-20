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

namespace APML.XmlWrappers.v0_6 {
  public class XmlImplicitConceptNode : XmlAttentionNodeBase<IImplicitConcept>, IImplicitConcept {
    private XmlImplicitNodeHelper<IImplicitConcept> mImplicitHelper;

    public XmlImplicitConceptNode(APMLFileBase pFile, XmlNode pNode) : base(pFile, pNode) {
      mImplicitHelper = new XmlImplicitNodeHelper<IImplicitConcept>(pFile, pNode, this);
    }

    #region IImplicitAttention Members
    public string From {
      get { return mImplicitHelper.From; }
      set { mImplicitHelper.From = value; }
    }

    public DateTime? Updated {
      get { return mImplicitHelper.Updated; }
      set { mImplicitHelper.Updated = value; }
    }
    #endregion
  }
}
