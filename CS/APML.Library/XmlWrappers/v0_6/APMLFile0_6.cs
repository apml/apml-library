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
using System.Globalization;
using System.Text;
using System.Xml;
using APML.XmlWrappers.Common;

namespace APML.XmlWrappers.v0_6 {
  public class APMLFile0_6 : APMLFileBase {
    #region Constants
    /// <summary>
    /// The format used for APML dates
    /// </summary>
    private const string APML_DATE_FORMAT = @"yyyy\-MM\-dd\THH:mm:ss\Z";
    #endregion

    #region Members
    private object mProfileLock = new object();
    private object mApplicationLock = new object();
    #endregion

    #region Constructors
    public APMLFile0_6(string pFilename)
      : base(pFilename) {
    }

    public APMLFile0_6(string pFilename, XmlDocument pPreLoaded) : base(pFilename, pPreLoaded) {
    }
    #endregion

    #region APMLFileBase Implementation
    public override void AddProfile(string pName) {
      using (OpenWriteSession()) {
        XmlNode bodyNode = GetBodyNode();
        XmlElement profileNode = Xml.CreateElement("Profile");
        AddXmlAttribute(profileNode, "name", pName);

        bodyNode.InsertBefore(profileNode, GetApplicationsNode());

        // Add the standard child nodes
        XmlNode implicitData = AddXmlElement(profileNode, "ImplicitData", null);
        XmlNode explicitData = AddXmlElement(profileNode, "ExplicitData", null);
        AddXmlElement(implicitData, "Concepts", null);
        AddXmlElement(implicitData, "Sources", null);
        AddXmlElement(explicitData, "Concepts", null);
        AddXmlElement(explicitData, "Sources", null);

        // Cache it if our cache is in use
        if (ProfileCache != null) {
          XmlProfileNode profile = new XmlProfileNode(this, profileNode);

          ProfileCache.Add(pName, profile);
          profile.NameChanged += new ProfileNameChangedEventHandler(Profiles_NameChanged);
          profile.Removed += new APMLComponentRemovedHandler(Profiles_ProfileRemoved);
        }
      }
    }

    protected override void EnsureProfileCacheExists() {
      using (OpenReadSession()) {
        // If the profiles have already been created, we're done
        if (ProfileCache != null) {
          return;
        }

        lock (mProfileLock) {
          // Allocate the profiles structure
          ProfileCache = new Dictionary<string, IProfile>();

          // Work through each profile
          XmlNodeList profileNodes = Xml.SelectNodes("/APML/Body/Profile");
          foreach (XmlNode profileNode in profileNodes) {
            XmlProfileNode profile = new XmlProfileNode(this, profileNode);

            ProfileCache.Add(profile.Name, profile);
            profile.NameChanged += new ProfileNameChangedEventHandler(Profiles_NameChanged);
            profile.Removed += new APMLComponentRemovedHandler(Profiles_ProfileRemoved);
          }
        }
      }
    }

    protected override void EnsureApplicationCacheExists() {
      using (OpenReadSession()) {
        // If the application has already been created, we're done
        if (LoadedApplication != null) {
          return;
        }

        lock (mApplicationLock) {
          if (LoadedApplication != null) {
            // Prevent a race condition
            return;
          }

          // Abort if we have no id
          if (ApplicationID == null) return;

          // Check the applications node exists.
          XmlNode applicationsNode = GetBodyNode().SelectSingleNode("Applications");
          if (applicationsNode == null) {
            XmlNode bodyNode = GetBodyNode();

            applicationsNode = Xml.CreateElement("Applications");
            bodyNode.AppendChild(applicationsNode);
          }

          // Find the application (if it exists). Otherwise, create it.
          XmlNode applicationNode = GetApplicationNode(ApplicationID);
          if (applicationNode == null) {
            // We need to create the application node
            applicationNode = AddXmlElement(applicationsNode, "Application", "Name", ApplicationID);
          }

          LoadedApplication = new XmlApplicationNode(this, applicationNode);
        }
      }
    }

    public override DateTime? ParseDate(string pDateStr) {
      if (pDateStr == null || pDateStr == string.Empty) {
        return null;
      }

      // First-up, try parsing with the proper APML style
      DateTime result;
      if (DateTime.TryParseExact(pDateStr, APML_DATE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out result)) {
        return result;
      }

      // Fall-back is to try using a generic parse
      return DateTime.Parse(pDateStr);
    }

    public override string DateToString(DateTime? pDate) {
      if (pDate != null) {
        return pDate.Value.ToUniversalTime().ToString(APML_DATE_FORMAT, DateTimeFormatInfo.CurrentInfo);
      }

      return null;
    }

    protected override string DefaultProfileAttribute {
      get { return GetValue(GetBodyNode(), "defaultprofile"); }
      set { AddXmlAttribute(GetBodyNode(), "defaultprofile", value); }
    }

    protected override void SetVersion() {
      AddXmlAttribute(Xml.DocumentElement, "version", "0.6");
    }

    /// <summary>
    /// Create the file, adding the initial elements to the XML
    /// </summary>
    protected override void CreateFile() {
      using (OpenWriteSession()) {
        AddXmlDeclaration(null, null, null);
        AddXmlElement(Xml, "APML", null);
        SetVersion();
        //Xml.Schemas.Add("", "http://www.apml.org/apml-0.6");

        //Touchstone Only
        //SaveHeader("Touchstone APML File", "Touchstone", DateTime.Now.ToLongDateString(), Base.CurrentUser.EmailAddress);

        AddHead();
        AddBody();

        AddProfile("Home");
        SetDefaultProfile("Home");
        DefaultProfile.AddExplicitSource("http://feeds.reuters.com/reuters/topNews/", 4, "Reuters: Top News", "application/rss+xml");
      }
      Save();
    }
    #endregion
  }
}
