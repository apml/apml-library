using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using NUnit.Framework;

namespace APML.Test {
  [TestFixture]
  public class GenerateByHandTest {
    [Test]
    public void TestGenerateByHand() {
      File.Delete("TestGenerate.apml");

      APMLDocumentFactory.DefaultApplicationId = "APMLUnitTests";
      IAPMLDocument doc = APMLDocumentFactory.LoadDocument("TestGenerate.apml", false);
      doc.Generator = "APML Test Cases";
      doc.Title = "Test APML File";
      doc.UserEmail = "user2@example.com";
      doc.DateCreated = new DateTime(2007, 03, 25, 2, 22, 00).ToLocalTime();
      doc.Save();

      doc.SetActiveProfile(doc.DefaultProfile);
      doc.ActiveProfile.ExplicitData.AddExplicitConcept("MyConcept", 1.0);
      doc.ActiveProfile.ImplicitData.AddImplicitConcept("MyImplConcept", 1.0, "APMLUnitTests");
      doc.ActiveProfile.ExplicitData.AddExplicitSource("http://www.apml.org/sample", 0.5, "APMLSample", "application/rss+xml");
      doc.ActiveProfile.ExplicitData.ExplicitSources["http://www.apml.org/sample"].AddAuthor("Author1", 1.0);
      doc.ActiveProfile.ImplicitData.AddImplicitSource("http://www.apml.org/sample2", 0.5, "APMLSample2", "application/rss+xml", "APMLUnitTests");
      doc.ActiveProfile.ImplicitData.ImplicitSources["http://www.apml.org/sample2"][0].AddAuthor("Author1", 1.0, "APMLUnitTests");

      TestUtils.FixUpdatedTimes(doc);

      doc.Save();

      string expected = File.ReadAllText(TestUtils.FindFile("APML.Library.Test\\Docs\\Expected-TestGenerate.apml"));
      string actual = File.ReadAllText("TestGenerate.apml");

      Assert.AreEqual(expected, actual, "Output should match expected");
    }

    public void TestInternationalGenerated() {
      CultureInfo iCulture = new CultureInfo("fr-FR");
      CultureInfo defCulture = Thread.CurrentThread.CurrentCulture;

      try {
        Thread.CurrentThread.CurrentCulture = iCulture;

        File.Delete("TestGenerateI.apml");

        APMLDocumentFactory.DefaultApplicationId = "APMLUnitTests";
        IAPMLDocument doc = APMLDocumentFactory.LoadDocument("TestGenerateI.apml", false);
        doc.Generator = "APML Test Cases";
        doc.Title = "Test APML File";
        doc.UserEmail = "user2@example.com";
        doc.DateCreated = new DateTime(2007, 03, 25, 2, 22, 00).ToLocalTime();
        doc.Save();

        doc.SetActiveProfile(doc.DefaultProfile);
        doc.ActiveProfile.ExplicitData.AddExplicitConcept("MyConcept", 1.0);
        doc.ActiveProfile.ImplicitData.AddImplicitConcept("MyImplConcept", 1.0, "APMLUnitTests");
        doc.ActiveProfile.ExplicitData.AddExplicitSource("http://www.apml.org/sample", 0.5, "APMLSample", "application/rss+xml");
        doc.ActiveProfile.ExplicitData.ExplicitSources["http://www.apml.org/sample"].AddAuthor("Author1", 1.0);
        doc.ActiveProfile.ImplicitData.AddImplicitSource("http://www.apml.org/sample2", 0.5, "APMLSample2", "application/rss+xml", "APMLUnitTests");
        doc.ActiveProfile.ImplicitData.ImplicitSources["http://www.apml.org/sample2"][0].AddAuthor("Author1", 1.0, "APMLUnitTests");

        TestUtils.FixUpdatedTimes(doc);

        doc.Save();

        string expected = File.ReadAllText(TestUtils.FindFile("APML.Library.Test\\Docs\\Expected-TestGenerate.apml"));
        string actual = File.ReadAllText("TestGenerateI.apml");

        Assert.AreEqual(expected, actual, "Output should match expected");
      } finally {
        Thread.CurrentThread.CurrentCulture = defCulture;
      }
    }
  }
}
