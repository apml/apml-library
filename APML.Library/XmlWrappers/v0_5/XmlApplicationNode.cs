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
  public class XmlApplicationNode : XmlWrapperBase, IApplication {
    private IDictionary<string, IGenericItem> mItems = null;

    public XmlApplicationNode(APMLFileBase pFile, XmlNode pNode)
      : base(pFile, pNode) {
    }

    #region IApplication Members
    public string Name {
      get { return GetAttribute("Name"); }
    }


    public bool HasItem(string pElName, string pMatchKey) {
      using (OpenReadSession()) {
        XmlNode itemNode = Node.SelectSingleNode("/Data/" + pElName +
                                "[translate(@Key,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='" +
                                APMLFileBase.Format(pMatchKey.ToLower(), APMLFileBase.FormatDirection.In) + "']");

        return itemNode != null;
      }
    }

    public IGenericItem AddItem(string pElName, string pKeyName) {
      using (OpenWriteSession()) {
        XmlNode itemNode = Node.OwnerDocument.CreateElement(pElName);
        Node.AppendChild(itemNode);

        XmlAttribute attr = Node.OwnerDocument.CreateAttribute("Key");
        attr.Value = pKeyName;
        itemNode.Attributes.Append(attr);

        IGenericItem item = new XmlGenericItemNode(File, itemNode);
        mItems.Add(pKeyName, item);

        return item;
      }
    }

    public IReadOnlyDictionary<string, IGenericItem> Items {
      get {
        EnsureItemCacheCreated();

        return new ReadOnlyDictionary<string, IGenericItem>(mItems);
      }
    }

    public XmlDocument OwnerDocument {
      get { return Node.OwnerDocument; }
    }

    public XmlNodeList ChildNodes {
      get { return Node.ChildNodes; }
    }

    public void AppendChild(XmlNode pChild) {
      using (OpenWriteSession()) {
        Node.AppendChild(pChild);
      }
    }

    public void RemoveChild(XmlNode pChild) {
      using (OpenWriteSession()) {
        Node.RemoveChild(pChild);
      }
    }

    #endregion

    private void EnsureItemCacheCreated() {
      using (OpenReadSession()) {
        // If the cache has already been created, we're done
        if (mItems != null) {
          return;
        }
      }

      using (OpenWriteSession()) {
        if (mItems != null) { // Prevent a race condition
          return;
        }

        // Allocate the cache
        mItems = new Dictionary<string, IGenericItem>();

        // Work through each device
        XmlNodeList itemNodes = Node.SelectSingleNode("Data").ChildNodes;
        foreach (XmlNode itemNode in itemNodes) {
          if (itemNode.NodeType == XmlNodeType.Element) {
            XmlGenericItemNode item = new XmlGenericItemNode(File, itemNode);

            item.Removed += new APMLComponentRemovedHandler(Items_ItemRemoved);

            mItems.Add(item.Name, item);
          }
        }
      }
    }

    private void Items_ItemRemoved(IAPMLComponent pComponent) {
      using (OpenWriteSession()) {
        IGenericItem item = (IGenericItem)pComponent;

        item.Removed -= new APMLComponentRemovedHandler(Items_ItemRemoved);

        if (mItems != null) {
          mItems.Remove(item.Name);
        }
      }
    }
  }
}
