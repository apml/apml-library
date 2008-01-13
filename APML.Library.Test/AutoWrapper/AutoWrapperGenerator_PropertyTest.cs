using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;
using APML.AutoWrapper;
using NUnit.Framework;

namespace APML.Test.AutoWrapper {
  [TestFixture]
  public class AutoWrapperGenerator_PropertyTest {
    private readonly AutoWrapperGenerator mGenerator = new AutoWrapperGenerator();

    [SetUp]
    public void SetUp() {
      // Moved to happen once per entire test suite
//      mGenerator = new AutoWrapperGenerator();
    }

    [Test]
    public void ShouldCreateWrapperForEmptyInterface() {
      Assert.IsNotNull(mGenerator.GenerateWrapper<IEmpty>(null));
    }

    [Test]
    public void ShouldCreateWrapperForInterfaceWithPropertyOnly() {
      Assert.IsNotNull(mGenerator.GenerateWrapper<ISingleProperty>(null));
    }

    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void ShouldRejectPropertyWithoutTag() {
      mGenerator.GenerateWrapper<IPropertyWithoutTag>(null);
    }

    [Test]
    public void ShouldHandleFetchingProperty() {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(@"<Single Key=""Blah""></Single>");

      Assert.AreEqual("Blah", mGenerator.GenerateWrapper<ISingleProperty>(doc.DocumentElement).Key);
    }

    [Test]
    public void ShouldHandleUpdatingProperty() {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(@"<Single Key=""Blah""></Single>");

      ISingleProperty wrapper = mGenerator.GenerateWrapper<ISingleProperty>(doc.DocumentElement);
      wrapper.Key = "Hello";
      Assert.AreEqual("Hello", wrapper.Key);
    }

    [Test]
    public void ShouldHandleFetchingPropertyWithAlternateName() {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(@"<Single Key2=""Blah""></Single>");

      Assert.AreEqual("Blah", mGenerator.GenerateWrapper<IPropertyWithAlternateName>(doc.DocumentElement).Key);
    }

    [Test]
    public void ShouldHandleUpdatingPropertyWithAlternateName() {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(@"<Single Key2=""Blah""></Single>");

      IPropertyWithAlternateName wrapper = mGenerator.GenerateWrapper<IPropertyWithAlternateName>(doc.DocumentElement);
      wrapper.Key = "Hello";
      Assert.AreEqual("Hello", wrapper.Key);
    }

    [Test]
    public void ShouldHandleApplyingDefault() {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(@"<Single></Single>");

      ISinglePropertyWithDefault wrapper = mGenerator.GenerateWrapper<ISinglePropertyWithDefault>(doc.DocumentElement);
      Assert.AreEqual(12, wrapper.Key);
    }

    [Test]
    public void ShouldHandleSimpleTypeAsElement() {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(@"<Single><Content>Blah</Content></Single>");

      Assert.AreEqual("Blah", mGenerator.GenerateWrapper<IElements>(doc.DocumentElement).Content);
    }

    [Test]
    public void ShouldHandleUpdatingElement() {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(@"<Single><Content>Blah</Content></Single>");

      IElements els = mGenerator.GenerateWrapper<IElements>(doc.DocumentElement);
      els.Content = "hello";
      Assert.AreEqual("hello", els.Content);
    }

    [Test]
    public void ShouldHandleFetchingElementWithAlternateName() {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(@"<Single><ContentC2>Blah</ContentC2></Single>");

      Assert.AreEqual("Blah", mGenerator.GenerateWrapper<IElements>(doc.DocumentElement).ContentC);
    }

    [Test]
    public void ShouldHandleUpdatingElementWithAlternateName() {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(@"<Single></Single>");

      IElements wrapper = mGenerator.GenerateWrapper<IElements>(doc.DocumentElement);
      wrapper.ContentC = "Hello";
      Assert.AreEqual("Hello", wrapper.ContentC);
    }

    [Test]
    public void ShouldHandleSimpleTypeAsElementWithDefault() {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(@"<Single></Single>");

      Assert.AreEqual("my-default", mGenerator.GenerateWrapper<IElements>(doc.DocumentElement).ContentB);
    }

    [Test]
    public void ShouldHandleTypeWithComplexChild() {
      Assert.IsNotNull(mGenerator.GenerateWrapper<IWithComplexElement>(null));
    }

    [Test]
    public void ShouldAllowAccessToChildValue() {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(@"<Single><Child Key=""child-key""/></Single>");

      Assert.AreEqual("child-key", mGenerator.GenerateWrapper<IWithComplexElement>(doc.DocumentElement).Child.Key);
    }

    [Test]
    public void ShouldHandleMissingComplexChild() {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(@"<Single></Single>");

      Assert.IsNull(mGenerator.GenerateWrapper<IWithComplexElement>(doc.DocumentElement).Child);
    }

    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void ShouldThrowExceptionOnComplexChildElementWithSetter() {
      mGenerator.GenerateWrapper<IWithSettableComplexElement>(null);
    }

    [Test]
    public void ShouldHandleListElementAsArray() {
      Assert.IsNotNull(mGenerator.GenerateWrapper<IWithElementSequenceAsArray>(null));
    }

    [Test]
    public void ShouldGetCorrectCountForArray() {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(@"<Single><Child Key=""a"" /><Child Key=""b"" /></Single>");

      Assert.AreEqual(2, mGenerator.GenerateWrapper<IWithElementSequenceAsArray>(doc.DocumentElement).Child.Length);
    }

    [Test]
    public void ShouldHandleListElementAsList() {
      Assert.IsNotNull(mGenerator.GenerateWrapper<IWithElementSequenceAsList>(null));
    }

    [Test]
    public void ShouldGetCorrectCountForList() {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(@"<Single><Child Key=""a"" /><Child Key=""b"" /></Single>");

      Assert.AreEqual(2, mGenerator.GenerateWrapper<IWithElementSequenceAsList>(doc.DocumentElement).Child.Count);
    }

    [Test]
    public void ShouldHandleDictionaryElement() {
      Assert.IsNotNull(mGenerator.GenerateWrapper<IWithElementSequenceAsDictionary>(null));
    }

    [Test]
    public void ShouldCorrectCountForDictionaryList() {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(@"<Single><Child Key=""a"" /><Child Key=""b"" /></Single>");

      Assert.AreEqual(2, mGenerator.GenerateWrapper<IWithElementSequenceAsDictionary>(doc.DocumentElement).Child.Count);
    }

    [Test]
    public void ShouldIndexElementsCorrectForDictionary() {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(@"<Single><Child Key=""a"" /><Child Key=""b"" /></Single>");

      Assert.AreEqual("a", mGenerator.GenerateWrapper<IWithElementSequenceAsDictionary>(doc.DocumentElement).Child["a"].Key);
    }

    [Test]
    public void ShouldHandleContainedListAsArray() {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(@"<Single><Children><Child Key=""a"" /><Child Key=""b"" /></Children></Single>");

      Assert.AreEqual(2, mGenerator.GenerateWrapper<IWithElementContainerAsArray>(doc.DocumentElement).Child.Length);
    }

    [Test]
    public void ShouldHandleContainedListAsList() {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(@"<Single><Children><Child Key=""a"" /><Child Key=""b"" /></Children></Single>");

      Assert.AreEqual(2, mGenerator.GenerateWrapper<IWithElementContainerAsList>(doc.DocumentElement).Child.Count);
    }

    [Test]
    public void ShouldHandleContainedListAsDictionary() {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(@"<Single><Children><Child Key=""a"" /><Child Key=""b"" /></Children></Single>");

      Assert.AreEqual(2, mGenerator.GenerateWrapper<IWithElementContainerAsDictionary>(doc.DocumentElement).Child.Count);
    }

    [Test]
    public void ShouldHandleContainedListAsDictionaryOfLists() {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(@"<Single><Children><Child Key=""a"" /><Child Key=""b"" /></Children></Single>");

      Assert.AreEqual(2, mGenerator.GenerateWrapper<IWithElementContainerAsDictionaryOfLists>(doc.DocumentElement).Child.Count);
    }

    [Test]
    public void ShouldHandleMultipleKeyInstancesInDictionaryOfLists() {
      XmlDocument doc = new XmlDocument();
      doc.LoadXml(@"<Single><Children><Child Key=""a""/><Child Key=""a""/></Children></Single>");

      Assert.AreEqual(2, mGenerator.GenerateWrapper<IWithElementContainerAsDictionaryOfLists>(doc.DocumentElement).Child["a"].Count);
    }

    [Test]
    public void ShouldHandleInheritedInterfaces() {
      Assert.IsNotNull(mGenerator.GenerateWrapper<IInheritedInterface>(null));
    }
  }

  public interface IEmpty {
  }

  public interface IPropertyWithoutTag {
    string Key { get; set; }
  }

  public interface ISingleProperty {
    [XmlAttribute]
    string Key { get; set; }
  }

  public interface ISinglePropertyWithDefault {
    [XmlAttribute]
    [DefaultValue(12)]
    int Key { get; set; }
  }

  public interface IPropertyWithAlternateName {
    [XmlAttribute("Key2")]
    string Key { get; set; }
  }

  public interface IElements {
    [XmlElement]
    string Content { get; set; }

    [XmlElement]
    [DefaultValue("my-default")]
    string ContentB { get; set; }

    [XmlElement("ContentC2")]
    string ContentC { get; set; }
  }

  public interface ISingleElementWithDefault {
    [XmlElement]
    string Content { get; set; }
  }

  public interface IWithComplexElement {
    [XmlElement] 
    ISingleProperty Child { get; }
  }

  public interface IWithSettableComplexElement {
    [XmlElement]
    ISingleProperty Child { get; set; }
  }

  public interface IWithElementSequenceAsArray {
    [XmlElement] 
    ISingleProperty[] Child { get; }
  }

  public interface IWithElementSequenceAsList {
    [XmlElement]
    IList<ISingleProperty> Child { get; }
  }

  public interface IWithElementSequenceAsDictionary {
    [XmlElement]
    [AutoWrapperKey("Key")]
    IReadOnlyDictionary<string, ISingleProperty> Child { get; }
  }

  public interface IWithElementContainerAsArray {
    [XmlArray("Children")]
    ISingleProperty[] Child { get; }
  }

  public interface IWithElementContainerAsList {
    [XmlArray("Children")]
    IList<ISingleProperty> Child { get; }
  }

  public interface IWithElementContainerAsDictionary {
    [XmlArray("Children")]
    [AutoWrapperKey("Key")]
    IReadOnlyDictionary<string, ISingleProperty> Child { get; }
  }

  public interface IWithElementContainerAsDictionaryOfLists {
    [XmlArray("Children")]
    [AutoWrapperKey("Key")]
    IReadOnlyDictionary<string, IList<ISingleProperty>> Child { get; }
  }

  public interface IInheritedInterface : IWithComplexElement {
  }
}
