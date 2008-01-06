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

namespace APML.XmlWrappers.Common {
  public class XmlAPMLComponentBase : XmlWrapperBase, IAPMLComponent {
    public XmlAPMLComponentBase(APMLFileBase pFile, XmlNode pNode) : base(pFile, pNode) {
    }

    #region IAPMLComponent Members
    public void Remove() {
      using (OpenWriteSession()) {
        Node.ParentNode.RemoveChild(Node);
      }

      APMLComponentRemovedHandler removedHandler = Removed;
      if (removedHandler != null) {
        removedHandler(this);
      } 
    }

    public event APMLComponentRemovedHandler Removed;

    public void SetExtraAttribute(string pAttrName, string pAttrValue) {
      APMLFileBase.AddXmlAttribute(Node, pAttrName, pAttrValue);
    }

    public void ClearExtraAttribute(string pAttrName) {
      APMLFileBase.DeleteXmlAttribute(Node, pAttrName);
    }

    public string GetExtraAttribute(string pAttrName) {
      return APMLFileBase.GetValue(Node, pAttrName);
    }

    #endregion


    protected void AddImplicitAttributes(XmlNode pNode) {
      File.SetAttributeAsDateTime(pNode, "updated", DateTime.Now);
      APMLFileBase.AddXmlAttribute(pNode, "from", File.ApplicationID);
    }
  }
}
