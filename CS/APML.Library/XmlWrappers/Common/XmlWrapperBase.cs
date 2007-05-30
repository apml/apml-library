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

namespace APML.XmlWrappers.Common {
  public class XAttribute {
    public readonly string Key;
    public readonly string Value;

    public XAttribute(string pKey, string pVal) {
      Key = pKey;
      Value = pVal;
    }
  }

  public class XmlWrapperBase : IAPMLLockable {
    private APMLFileBase mFile;
    private XmlNode mNode;
    private IDictionary<string, object> mAttrCache;

    public XmlWrapperBase(APMLFileBase pFile, XmlNode pNode) {
      mFile = pFile;
      mNode = pNode;

      mAttrCache = new Dictionary<string, object>();
    }

    #region IAPMLLockable Members
    public IAPMLReadSession OpenReadSession() {
      return mFile.OpenReadSession();
    }

    public IAPMLWriteSession OpenWriteSession() {
      return mFile.OpenWriteSession();
    }
    #endregion

    #region Properties
    public XmlNode Node {
      get { return mNode; }
    }

    protected APMLFileBase File {
      get { return mFile; }
    }
    #endregion

    protected string GetAttribute(string pAttrName) {
      using (OpenReadSession()) {
        lock (mAttrCache) {
          if (mAttrCache.ContainsKey(pAttrName) && mAttrCache[pAttrName] is string) {
            return (string) mAttrCache[pAttrName];
          }

          string result = APMLFileBase.GetValue(mNode, pAttrName);
          mAttrCache[pAttrName] = result;

          return result;
        }
      }
    }

    protected DateTime? GetAttributeAsDateTime(string pAttrName) {
      using (OpenReadSession()) {
        lock (mAttrCache) {
          if (mAttrCache.ContainsKey(pAttrName) && mAttrCache[pAttrName] is DateTime?) {
            return (DateTime?) mAttrCache[pAttrName];
          }

          DateTime? result = mFile.GetAttributeAsDateTime(mNode, pAttrName);
          mAttrCache[pAttrName] = result;

          return result;
        }
      }
    }

    protected double GetAttributeAsDouble(string pAttrName) {
      using (OpenReadSession()) {
        lock (mAttrCache) {
          if (mAttrCache.ContainsKey(pAttrName) && mAttrCache[pAttrName] is double) {
            return (double) mAttrCache[pAttrName];
          }

          string strValue = APMLFileBase.GetValue(mNode, pAttrName);
          double result = 0;

          if (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator == "," && strValue.Contains(",")) {
            // We need to parse in the local culture, cause otherwise a 1,00 would be interpreted as 100
            result = double.Parse(strValue);
          } else {
            result = double.Parse(strValue, CultureInfo.InvariantCulture);
          }

          
          mAttrCache[pAttrName] = result;

          return result;
        }
      }
    }

    protected string SetAttribute(string pAttrName, string pAttrValue) {
      using (OpenWriteSession()) {
        // No need to lock mAttrCache since we've already locked the entire APML tree
        
        mAttrCache[pAttrName] = pAttrValue;
        return APMLFileBase.AddXmlAttribute(mNode, pAttrName, pAttrValue);
      }
    }

    protected DateTime? SetAttributeAsDateTime(string pAttrName, DateTime? pAttrValue) {
      using (OpenWriteSession()) {
        mAttrCache[pAttrName] = pAttrValue;
        return mFile.SetAttributeAsDateTime(mNode, pAttrName, pAttrValue);
      }
    }

    protected double SetAttributeAsDouble(string pAttrName, double pAttrValue) {
      using (OpenWriteSession()) {
        mAttrCache[pAttrName] = pAttrValue;
        return double.Parse(APMLFileBase.AddXmlAttribute(mNode, pAttrName, pAttrValue.ToString("f2", CultureInfo.InvariantCulture)), CultureInfo.InvariantCulture);
      }
    }

    protected void ClearAttribute(string pAttrName) {
      using (OpenWriteSession()) {
        mAttrCache.Remove(pAttrName);

        APMLFileBase.DeleteXmlAttribute(mNode, pAttrName);
      }
    }

    protected static string GetNodeAttribute(XmlNode pNode, string pAttrName) {
      return APMLFileBase.GetValue(pNode, pAttrName);
    }

    protected XmlNode AddChildNode(string pContainer, string pName, params XAttribute[] pAttrs) {
      using (mFile.OpenWriteSession()) {
        XmlNode parent = pContainer != null ? Node.SelectSingleNode(pContainer) : mNode;

        XmlNode newNode = parent.OwnerDocument.CreateElement(pName);
        foreach (XAttribute attr in pAttrs) {
          XmlAttribute xAttr = newNode.OwnerDocument.CreateAttribute(attr.Key);
          xAttr.Value = attr.Value;

          newNode.Attributes.Append(xAttr);
        }

        parent.AppendChild(newNode);

        return newNode;
      }
    }

    /*protected void RemoveChildNode(string pContainer, string pElName, string pAttrKey, string pAttrVal) {
      using (mFile.OpenWriteSession()) {
        XmlNode node = Node.SelectSingleNode(
                            "/" + pContainer + "/" + pElName + 
                            "[translate(@" + pAttrKey + ",'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz') = '" +
                            APMLFile.Format(pAttrVal.ToLower(), APMLFile.FormatDirection.In) + "']");

        
        if (node != null) {
          node.ParentNode.RemoveChild(node);
        }
      }
    }*/

    protected void ClearChildContainer(string pContainer) {
      using (mFile.OpenWriteSession()) {
        XmlNode container = Node.SelectSingleNode(pContainer);

        while (container.FirstChild != null) {
          container.RemoveChild(container.FirstChild);
        }
      }
    }
  }
}
