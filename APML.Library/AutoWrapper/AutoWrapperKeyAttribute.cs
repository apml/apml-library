using System;
using System.Collections.Generic;
using System.Text;

namespace APML.AutoWrapper {
  public class AutoWrapperKeyAttribute : Attribute {
    private string mKeyAttribute;

    public AutoWrapperKeyAttribute(string pKeyAttribute) {
      mKeyAttribute = pKeyAttribute;
    }

    public string KeyAttribute {
      get { return mKeyAttribute; }
    }
  }
}
