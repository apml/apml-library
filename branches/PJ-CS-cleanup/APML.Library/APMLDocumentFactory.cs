
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
using System.IO;
using System.Text;
using System.Xml;
using APML.AutoWrapper;

namespace APML {
  public class APMLDocumentFactory {
    private static string mDefaultApplicationId;

    /// <summary>
    /// The AutoWrapperGenerator used for creating APML wrappers.
    /// </summary>
    private static readonly AutoWrapperGenerator sGenerator = new AutoWrapperGenerator();

    /// <summary>
    /// Lock used to ensure that generator isn't accessed from multiple classes at once.
    /// </summary>
    private static object sGeneratorLock = new object();

    /// <summary>
    /// The default application id set on documents.
    /// </summary>
    public static string DefaultApplicationId {
      get { return mDefaultApplicationId; }
      set { mDefaultApplicationId = value; }
    }

    /// <summary>
    /// Returns an IAPMLDocument instance for the given file.
    /// </summary>
    /// <param name="pFile">the file to open</param>
    /// <param name="pShouldUpgrade">
    ///   if the APML document is in an old format, a value of true indicates that it should be upgraded. A value
    ///   of false indicates that the document should be worked with in a compatability mode.
    /// </param>
    /// <returns>a document instance</returns>
    public static IAPMLDocument LoadDocument(string pFile, bool pShouldUpgrade) {
      XmlDocument doc;
      bool created;

      // If the file doesn't exist, just create a new file of the latest version
      if (!File.Exists(pFile)) {
        doc = new XmlDocument();
        doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));
        doc.AppendChild(doc.CreateElement("APML", APMLConstants.NAMESPACE_0_6));
        created = true;

//        IAPMLDocument newDoc = new APMLFile0_6(pFile);
//        newDoc.ApplicationID = DefaultApplicationId;

//        return newDoc;
      } else {
        doc = new XmlDocument();
        doc.Load(pFile);
        created = false;

        // Check if we need to fix the namespace
        if (doc.DocumentElement.Name == "APML" && doc.DocumentElement.NamespaceURI == string.Empty) {
          // We have a non-namespaced doc. Apply the namespace and reload it.
          doc.DocumentElement.SetAttribute("xmlns", APMLConstants.NAMESPACE_0_6);
          doc.LoadXml(doc.OuterXml);
        }
      }

      
      /*XmlAttributeCollection docElAttr = doc.DocumentElement.Attributes;
      if (docElAttr["Version"] != null && docElAttr["Version"].Value == "0.5") {
        if (pShouldUpgrade) {
          // Backup the old version
          File.Copy(pFile, BuildBackupName(pFile, "0.5", true));

          return UpgradeDocument(new APMLFile0_5(pFile, doc), UpgradeOption.SPLIT_EXPLICIT_AUTHORS);
        }

        IAPMLDocument aDoc = new APMLFile0_5(pFile, doc);
        aDoc.ApplicationID = DefaultApplicationId;
        return aDoc;
      }
      if (docElAttr["version"] != null && docElAttr["version"].Value == "0.6") {
        IAPMLDocument aDoc = new APMLFile0_6(pFile, doc);
        aDoc.ApplicationID = DefaultApplicationId;
        return aDoc;
      }*/

      // For the moment, just return an APMLFile instance
      lock (sGeneratorLock) {
        IAPMLDocument result = new APMLFile(pFile, doc, created, sGenerator);
        result.ApplicationID = DefaultApplicationId;
        return result;
      }
    }

    /*private static IAPMLDocument UpgradeDocument(IAPMLDocument pOld, params UpgradeOption[] pOptions) {
      // Generate the temporary file name, and make sure it doesn't exist
      string tempFile = pOld.Filename + ".apmlupgrade";
      File.Delete(tempFile);

      APMLFile0_6 newDoc = new APMLFile0_6(tempFile);
      newDoc.ApplicationID = DefaultApplicationId;

      // Copy all the attributes in
      newDoc.Title = pOld.Title;
      newDoc.DateCreated = pOld.DateCreated;
      newDoc.Generator = pOld.Generator;
      newDoc.UserEmail = pOld.UserEmail;

      // Remove all the profiles from the new document
      IProfile[] oldProfiles = new IProfile[newDoc.Profiles.Count];
      newDoc.Profiles.Values.CopyTo(oldProfiles, 0);
      foreach (IProfile oldProfile in oldProfiles) {
        oldProfile.Remove();
      }

      // Copy all of the applications and profiles
      foreach (IProfile oldProfile in pOld.Profiles.Values) {
        newDoc.AddProfile(oldProfile.Name);
        IProfile newProfile = newDoc.Profiles[oldProfile.Name];

        // Add all of the sources to the new profile
        if (HasOption(pOptions, UpgradeOption.SPLIT_EXPLICIT_AUTHORS)) {
          foreach (IExplicitSource source in oldProfile.ExplicitSources.Values) {
            if (source.Value != 0) {
              if (!newProfile.ExplicitSources.ContainsKey(source.Key)) {
                newProfile.AddExplicitSource(source.Key, source.Value, source.Name, source.Type);
              }
            } else {
              newProfile.AddImplicitSource(source.Key, source.Value, source.Name, source.Type);
            }
          }
        } else {
          foreach (IExplicitSource source in oldProfile.ExplicitSources.Values) {
            if (!newProfile.ExplicitSources.ContainsKey(source.Key)) {
              newProfile.AddExplicitSource(source.Key, source.Value, source.Name, source.Type);
            }
          }
          foreach (IList<IImplicitSource> sourceList in oldProfile.ImplicitSources.Values) {
            foreach (IImplicitSource source in sourceList) {
              newProfile.AddImplicitSource(source.Key, source.Value, source.Name, source.Type);
            }
          }
        }

        // Add all of the concepts to the new profile
        foreach (IExplicitConcept concept in oldProfile.ExplicitConcepts.Values) {
          if (!newProfile.ExplicitConcepts.ContainsKey(concept.Key)) {
            newProfile.AddExplicitConcept(concept.Key, concept.Value);
          }
        }
        foreach (IList<IImplicitConcept> conceptList in oldProfile.ImplicitConcepts.Values) {
          foreach (IImplicitConcept concept in conceptList) {
            newProfile.AddImplicitConcept(concept.Key, concept.Value);
          }
        }
      }

      // Copy the default profile
      newDoc.SetDefaultProfile(pOld.DefaultProfile.Name);

      // Switch the new document into place
      newDoc.Save();
      File.Replace(tempFile, pOld.Filename, null);

      // Load a new document off the updated file
      return new APMLFile0_6(pOld.Filename);
    }*/

    /// <summary>
    /// Tests whether the given list of options contains the desired option.
    /// </summary>
    /// <param name="pOptions">the list of options to test</param>
    /// <param name="pTest">the option to test for</param>
    /// <returns>true - the list contains the option</returns>
    /*private static bool HasOption(UpgradeOption[] pOptions, UpgradeOption pTest) {
      foreach (UpgradeOption opt in pOptions) {
        if (opt == pTest) {
          return true;
        }
      }

      return false;
    }*/

    /// <summary>
    /// Builds a name for a backup file. For instance C:\apml.apml will become C:\apml-0.5.apml when
    /// backing up a version 0.5 file. Note that if pMustNotExist is true, and C:\apml-0.5.apml exists,
    /// then C:\apml-0.5-1.apml will be generated. If this exists, then C:\apml-0.5-2.apml
    /// </summary>
    /// <param name="pFileName">the name of the file that should be backed up</param>
    /// <param name="pCurrentVersion">the version of the apml file</param>
    /// <param name="pMustNotExist">whether the backup file should be a unique name</param>
    /// <returns>the backup name</returns>
    /*private static string BuildBackupName(string pFileName, string pCurrentVersion, bool pMustNotExist) {
      FileInfo fileInfo = new FileInfo(pFileName);

      // Get the name without its extension
      string nameWithoutExtension = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
      
      // Work out the name base
      string baseApmlName = Path.Combine(fileInfo.DirectoryName, nameWithoutExtension + "-" + pCurrentVersion);

      // If no uniqueness check is required, then just return the name
      if (!pMustNotExist) {
        return baseApmlName + fileInfo.Extension;
      }

      // Find a name that isn't used
      int curCounter = 0;
      string curBase = baseApmlName;

      while (File.Exists(curBase + fileInfo.Extension)) {
        curBase = baseApmlName + "-" + ++curCounter;
      }

      // Build the new name
      return curBase + fileInfo.Extension;
    }*/
  }

  internal enum UpgradeOption {
    /// <summary>
    /// Indicates that the upgrade should split explicit authors into explicit and
    /// implicit based on whether they have a non-zero score.
    /// </summary>
    SPLIT_EXPLICIT_AUTHORS
  }
}
