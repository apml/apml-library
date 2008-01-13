using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace APML.Test {
  class TestUtils {
    public static string FindFile(string pSubFile) {
      DirectoryInfo curDir = new DirectoryInfo(Environment.CurrentDirectory);

      while (curDir != null) {
        string testPath = Path.Combine(curDir.FullName, pSubFile);
        if (File.Exists(testPath)) {
          return testPath ;
        }

        curDir = curDir.Parent;
      }

      throw new ArgumentException("File " + pSubFile + " not found.");
    }

    public static void CrawlAPML0_5(IAPMLDocument pDoc) {
      foreach (IProfile profile in pDoc.Profiles.Values) {
        bool exConceptFound = false, imConceptFound = false, exSourceFound = false, imSourceFound = false;

        foreach (IExplicitConcept concept in profile.ExplicitData.ExplicitConcepts.Values) {
          exConceptFound = true;

          //Debug.WriteLine(concept.Name + " = " + concept.Value);

          concept.Value = 5.0;
        }

        foreach (IList<IImplicitConcept> conceptList in profile.ImplicitData.ImplicitConcepts.Values) {
          foreach (IImplicitConcept concept in conceptList) {
            imConceptFound = true;

            //Debug.WriteLine(concept.Name + " = " + concept.Value);

            concept.Value = 5.0;
          }
        }

        foreach (IExplicitSource source in profile.ExplicitData.ExplicitSources.Values) {
          exSourceFound = true;

          //Debug.WriteLine(source.Name + " = " + source.Value + " @ " + source.Url);

          source.Value = 5.0;
        }

        foreach (IList<IImplicitSource> sourceList in profile.ImplicitData.ImplicitSources.Values) {
          foreach (IImplicitSource source in sourceList) {
            imSourceFound = true;

            //Debug.WriteLine(source.Name + " = " + source.Value + " @ " + source.Url);

            source.Value = 5.0;
          }
        }
        
        Assert.IsTrue(imConceptFound, "Should have seen at least one implicit concept");
        Assert.IsTrue(exConceptFound, "Should have seen at least one explicit concept");
        Assert.IsFalse(imSourceFound, "Should have seen at no implicit sources");
        Assert.IsTrue(exSourceFound, "Should have seen at least one explicit source");
      }
    }

    public static void CrawlAPML0_6(IAPMLDocument pDoc) {
      foreach (IProfile profile in pDoc.Profiles.Values) {
        bool exConceptFound = false, imConceptFound = false, exSourceFound = false, imSourceFound = false;

        foreach (IExplicitConcept concept in profile.ExplicitData.ExplicitConcepts.Values) {
          exConceptFound = true;

          //Debug.WriteLine(concept.Name + " = " + concept.Value);

          concept.Value -= 0.1;
        }

        foreach (List<IImplicitConcept> conceptList in profile.ImplicitData.ImplicitConcepts.Values) {
          foreach (IImplicitConcept concept in conceptList) {
            imConceptFound = true;

            //Debug.WriteLine(concept.Name + " = " + concept.Value);

            concept.Value -= 0.1;
          }
        }

        foreach (IExplicitSource source in profile.ExplicitData.ExplicitSources.Values) {
          exSourceFound = true;

          //Debug.WriteLine(source.Name + " = " + source.Value + " @ " + source.Url);

          source.Value -= 0.1;
        }

        foreach (List<IImplicitSource> sourceList in profile.ImplicitData.ImplicitSources.Values) {
          foreach (IImplicitSource source in sourceList) {
            imSourceFound = true;

            //Debug.WriteLine(source.Name + " = " + source.Value + " @ " + source.Url);

            source.Value -= 0.1;
          }
        }

        Assert.IsTrue(imConceptFound, "Should have seen at least one implicit concept");
        Assert.IsTrue(exConceptFound, "Should have seen at least one explicit concept");
        Assert.IsTrue(imSourceFound, "Should have seen at least one implicit sources");
        Assert.IsTrue(exSourceFound, "Should have seen at least one explicit source");
      }
    }

    public static string DuplicateDoc(string pSourceName) {
      string tempFile = Path.GetTempFileName();
      File.Copy(pSourceName, tempFile, true);

      return tempFile;
    }

    public static void FixUpdatedTimes(IAPMLDocument pDoc) {
      DateTime expectedTime = DateTime.ParseExact("2007-04-11T23:35:14Z", @"yyyy\-MM\-dd\THH:mm:ss\Z", 
        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

      foreach (IProfile profile in pDoc.Profiles.Values) {
        foreach (IList<IImplicitConcept> conceptList in profile.ImplicitData.ImplicitConcepts.Values) {
          foreach (IImplicitConcept concept in conceptList) {
            concept.Updated = expectedTime;
          }
        }

        foreach (IList<IImplicitSource> sourceList in profile.ImplicitData.ImplicitSources.Values) {
          foreach (IImplicitSource source in sourceList) {
            source.Updated = expectedTime;

            foreach (IList<IImplicitAuthor> authorList in source.Authors.Values) {
              foreach (IImplicitAuthor author in authorList) {
                author.Updated = expectedTime;
              }
            }
          }
        }
      }
    }
  }
}
