using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;
using APML.AutoWrapper;
using NUnit.Framework;

namespace APML.Test.AutoWrapper {
  [TestFixture]
  public class AutoWrapperGenerator_MethodTest {
    private AutoWrapperGenerator mGenerator = new AutoWrapperGenerator();

    [Test]
    public void ShouldHandleInitMethodForSingleProperty() {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(@"<Simple></Simple>");

      ISingleElementWithInitMethod init = mGenerator.GenerateWrapper<ISingleElementWithInitMethod>(doc.DocumentElement);
      init.InitSingle("a");
      Assert.AreEqual("a", init.Single.Key);
    }

    [Test]
    public void ShouldHandleClearMethodForSingleProperty() {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(@"<Simple><Single Key=""a"" /></Simple>");

      ISingleElementWithClearMethod clear = mGenerator.GenerateWrapper<ISingleElementWithClearMethod>(doc.DocumentElement);
      clear.ClearSingle();
      Assert.IsNull(clear.Single);
    }

    [Test]
    public void ShouldHandleAddMethodForArrayProperty() {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(@"<Simple></Simple>");

      IArrayElementWithAddMethod add = mGenerator.GenerateWrapper<IArrayElementWithAddMethod>(doc.DocumentElement);
      add.AddSingle("a");
      Assert.AreEqual("a", add.Single[0].Key);
    }

    [Test]
    public void ShouldHandleAddMethodForListProperty() {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(@"<Simple></Simple>");

      IListElementWithAddMethod add = mGenerator.GenerateWrapper<IListElementWithAddMethod>(doc.DocumentElement);
      add.AddSingle("a");
      Assert.AreEqual("a", add.Single[0].Key);
    }

    [Test]
    public void ShouldHandleAddMethodForDictionaryProperty() {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(@"<Simple></Simple>");

      IDictionaryElementWithAddMethod add = mGenerator.GenerateWrapper<IDictionaryElementWithAddMethod>(doc.DocumentElement);
      add.AddDouble("a", 0.8);
      Assert.AreEqual(0.8, add.Double["a"].Value);
    }

    [Test]
    public void ShouldHandleAddToArrayAfterPropertyAlreadyAccessed() {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(@"<Simple></Simple>");

      IArrayElementWithAddMethod add = mGenerator.GenerateWrapper<IArrayElementWithAddMethod>(doc.DocumentElement);
      Assert.AreEqual(0, add.Single.Length);
      add.AddSingle("a");
      Assert.AreEqual("a", add.Single[0].Key);
    }

    [Test]
    public void ShouldHandleAddToListAfterPropertyAlreadyAccessed() {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(@"<Simple></Simple>");

      IListElementWithAddMethod add = mGenerator.GenerateWrapper<IListElementWithAddMethod>(doc.DocumentElement);
      Assert.AreEqual(0, add.Single.Count);
      add.AddSingle("a");
      Assert.AreEqual("a", add.Single[0].Key);
    }

    [Test]
    public void ShouldHandleAddToDictionaryAfterPropertyAlreadyAccessed() {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(@"<Simple></Simple>");

      IDictionaryElementWithAddMethod add = mGenerator.GenerateWrapper<IDictionaryElementWithAddMethod>(doc.DocumentElement);
      Assert.AreEqual(0, add.Double.Count);
      add.AddDouble("a", 0.8);
      Assert.AreEqual(0.8, add.Double["a"].Value);
    }

    [Test]
    public void ShouldHandleAddMethodWithExtraInheritedPropertiesForListProperty() {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(@"<Simple></Simple>");

      IDictionaryElementWithAddMethodAndInherited add = mGenerator.GenerateWrapper<IDictionaryElementWithAddMethodAndInherited>(doc.DocumentElement);
      add.AddDouble("a", 0.6);
      Assert.AreEqual("a", add.Double["a"].Key);
    }
  }

  public interface ISingleElementWithInitMethod {
    [XmlElement]
    ISingleProperty Single { get; }

    void InitSingle(string key);
  }

  public interface ISingleElementWithClearMethod {
    [XmlElement]
    ISingleProperty Single { get; }

    void ClearSingle();
  }

  public interface IArrayElementWithAddMethod {
    [XmlElement]
    ISingleProperty[] Single { get; }

    ISingleProperty AddSingle(string key);
  }

  public interface IListElementWithAddMethod {
    [XmlElement]
    IList<ISingleProperty> Single { get; }

    ISingleProperty AddSingle(string key);
  }

  public interface IDictionaryElementWithAddMethod {
    [XmlElement]
    [AutoWrapperKey("Key")]
    IReadOnlyDictionary<string, IDoubleProperty> Double { get; }

    IDoubleProperty AddDouble(string key, double value);
  }

  public interface IDoubleProperty {
    [XmlAttribute]
    string Key { get; set; }

    [XmlAttribute]
    [DefaultValue(1.0)]
    double Value { get; set; }
  }

  public interface IInheritsDoubleProperty : IDoubleProperty {
    
  }

  public interface IDictionaryElementWithAddMethodAndInherited {
    [XmlElement]
    [AutoWrapperKey("Key")]
    IReadOnlyDictionary<string, IInheritsDoubleProperty> Double { get; }

    IInheritsDoubleProperty AddDouble(string key, double value);
  }
}
