using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace APML.XmlWrappers.v0_6 {
  public class XmlSourceNode : XmlAttentionNodeBase<ISource>, ISource {
    public XmlSourceNode(APMLFile pFile, XmlNode pNode) : base(pFile, pNode) {
    }

    #region IAttention Members
    public override string Name {
      get { return GetAttribute("Value"); }
      set { FireNameChanged(SetAttribute("Value", value), value); }
    }

    public override double Value {
      get { return double.Parse(GetAttribute("Rank")); }
      set { FireValueChanged(SetAttribute("Rank", value.ToString("f2")), value); }
    }
    #endregion

    #region ISource Members
    public event NameChangedEventHandler UrlChanged;

    public string Url {
      get { return GetAttribute("Url"); }
      set { FireUrlChanged(SetAttribute("Url", value), value); }
    }
    #endregion


    protected void FireUrlChanged(string pOld, string pNew) {
      if (pOld == null) {
        return;
      }

      NameChangedEventHandler handler = UrlChanged;

      if (handler != null) {
        handler(this, pOld, pNew);
      }
    }
  }
}
