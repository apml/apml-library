using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using APML.AutoWrapper;
using NUnit.Framework;

namespace APML.Test {
  [TestFixture]
  public class APMLAutoWrapperTest {
    private readonly AutoWrapperGenerator mGenerator = new AutoWrapperGenerator();

    [Test]
    public void IProfileShouldSupportAutowrapper() {
      Assert.IsNotNull(mGenerator.GenerateWrapper<IProfile>(new XmlDocument().CreateElement("Profile")));
    }

    [Test]
    public void IApplicationShouldSupportAutowrapper() {
      Assert.IsNotNull(mGenerator.GenerateWrapper<IApplication>(new XmlDocument().CreateElement("Application")));
    }

    [Test]
    public void IAPMLRootShouldSupportAutowrapper() {
      Assert.IsNotNull(mGenerator.GenerateWrapper<IAPMLRoot>(new XmlDocument().CreateElement("APML")));
    }
  }
}
