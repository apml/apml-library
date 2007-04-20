using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using APML.XmlWrappers.Common;

namespace APML.XmlWrappers.v0_6 {
  public class XmlSourceNodeHelper<T> : XmlWrapperBase 
      where T : ISource {
    private T mOwner;

    public XmlSourceNodeHelper(APMLFileBase pFile, XmlNode pNode, T pOwner) : base(pFile, pNode) {
      mOwner = pOwner;
    }

    #region ISource Members
    public string Type {
      get { return GetAttribute("type"); }
      set { FireTypeChanged(SetAttribute("type", value), value); }
    }

    public string Name {
      get { return GetAttribute("name"); }
      set { FireNameChanged(SetAttribute("name", value), value); }
    }

    public event SourceTypeChangedEventHandler TypeChanged;
    public event SourceNameChangedEventHandler NameChanged;
    #endregion

    protected void FireNameChanged(string pOld, string pNew) {
      if (pOld == null) {
        return;
      }

      SourceNameChangedEventHandler handler = NameChanged;

      if (handler != null) {
        handler(mOwner, pOld, pNew);
      }
    }

    protected void FireTypeChanged(string pOld, string pNew) {
      if (pOld == null) {
        return;
      }

      SourceTypeChangedEventHandler handler = TypeChanged;

      if (handler != null) {
        handler(mOwner, pOld, pNew);
      }
    }
  }
}
