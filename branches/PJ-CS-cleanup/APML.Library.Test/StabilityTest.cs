using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using NUnit.Framework;

namespace APML.Test {
  [TestFixture]
  public class StabilityTest {
    [SetUp]
    public void Init() {
    }

    public void TestCreateAPML() {
      File.Delete("TestOutput.apml");

      IAPMLDocument doc = APMLDocumentFactory.LoadDocument("TestOutput.apml", false);
      doc.Generator = "Test Cases";
      doc.Title = "Test APML File";
      doc.UserEmail = "user@example.com";
      doc.DateCreated = new DateTime(2007, 03, 25, 2, 22, 00).ToLocalTime();
      doc.Save();

      string expected = File.ReadAllText(TestUtils.FindFile("APML.Library.Test\\Docs\\Expected-TestOutput.apml"));
      string actual = File.ReadAllText("TestOutput.apml");

      Assert.AreEqual(expected, actual, "Output should match expected");
    }

    public void TestLoadOfInternationalisedDocument() {
      CultureInfo iCulture = new CultureInfo("fr-FR");
      CultureInfo defCulture = Thread.CurrentThread.CurrentCulture;

      try {
        Thread.CurrentThread.CurrentCulture = iCulture;

        string copiedDoc = TestUtils.DuplicateDoc(TestUtils.FindFile("APML.Library.Test\\Docs\\APML-with-I-Chars.apml"));

        APMLDocumentFactory.DefaultApplicationId = "APMLUnitTests";
        IAPMLDocument doc = APMLDocumentFactory.LoadDocument(copiedDoc, false);
        TestUtils.CrawlAPML0_6(doc);
      } finally {
        Thread.CurrentThread.CurrentCulture = defCulture;
      }
    }

    [Test]
    public void TestClearImplicitConcepts() {
      File.Delete("TestOutput-ClearImplicitConcepts.apml");

      IAPMLDocument doc = APMLDocumentFactory.LoadDocument("TestOutput-ClearImplicitConcepts.apml", false);
      doc.Generator = "Test Cases";
      doc.Title = "Test APML File";
      doc.UserEmail = "user@example.com";
      doc.DateCreated = new DateTime(2007, 03, 25, 2, 22, 00).ToLocalTime();
      doc.Save();

      doc.DefaultProfile.ImplicitData.AddImplicitConcept("blah", 0.5, "app1");
      doc.DefaultProfile.ImplicitData.AddImplicitConcept("blah", 0.6, "app2");
      doc.DefaultProfile.ImplicitData.AddImplicitConcept("blah2", 0.5, "app1");

      doc.DefaultProfile.ImplicitData.ClearImplicitConcepts("app1");
      Assert.IsFalse(doc.DefaultProfile.ImplicitData.ImplicitConcepts.ContainsKey("blah2"));
      Assert.IsTrue(doc.DefaultProfile.ImplicitData.ImplicitConcepts.ContainsKey("blah"));
      Assert.AreEqual(1, doc.DefaultProfile.ImplicitData.ImplicitConcepts["blah"].Count);
    }
  }
}
