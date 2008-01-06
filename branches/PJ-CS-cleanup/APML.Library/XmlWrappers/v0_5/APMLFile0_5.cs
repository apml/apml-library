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
using System.Text;
using System.Xml;
using APML.XmlWrappers.Common;

namespace APML.XmlWrappers.v0_5 {
  public class APMLFile0_5 : APMLFileBase {
    /// <summary>
    /// The cache of generated devices.
    /// </summary>
    private Dictionary<string, IDevice> mDevices;

    public APMLFile0_5(string pFilename) : base(pFilename) {
    }

    public APMLFile0_5(string pFilename, XmlDocument pPreLoaded) : base(pFilename, pPreLoaded) {
    }

    #region APMLFileBase Implementation
    public override void AddProfile(string pName) {
      using (OpenWriteSession()) {
        XmlNode profileNode = AddXmlElement(GetBodyNode(), "Profile", "Name", pName);

        // Add the standard child nodes
        AddXmlElement(profileNode, "Usage", null);
        AddXmlElement(profileNode, "ImplicitConcepts", null);
        AddXmlElement(profileNode, "ExplicitConcepts", null);
        AddXmlElement(profileNode, "ExplicitSources", null);
        AddXmlElement(profileNode, "Authors", null);

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
      }

      using (OpenWriteSession()) {
        if (ProfileCache != null) { // Prevent a race condition
          return;
        }

        // Allocate the profiles structure
        ProfileCache = new Dictionary<string, IProfile>();

        // Work through each profile
        XmlNodeList profileNodes = Xml.SelectNodes("/APML/Body/Profile");
        foreach (XmlNode profileNode in profileNodes) {
          XmlProfileNode profile = new XmlProfileNode(this, profileNode);

          ProfileCache.Add(GetValue(profileNode, "Name"), profile);
          profile.NameChanged += new ProfileNameChangedEventHandler(Profiles_NameChanged);
        }
      }
    }

    protected override void EnsureApplicationCacheExists() {
      using (OpenReadSession()) {
        // If the application has already been created, we're done
        if (LoadedApplication != null) {
          return;
        }
      }

      using (OpenWriteSession()) {
        if (LoadedApplication != null) { // Prevent a race condition
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

    public override DateTime? ParseDate(string pDateStr) {
      return DateTime.Parse(pDateStr);
    }

    public override string DateToString(DateTime? pDate) {
      return pDate.Value.ToString();
    }

    protected override string DefaultProfileAttribute {
      get { return GetValue(GetBodyNode(), "DefaultProfile"); }
      set { AddXmlAttribute(GetBodyNode(), "DefaultProfile", value); }
    }

    protected override void SetVersion() {
      AddXmlAttribute(Xml.DocumentElement, "Version", "0.5");
    }

    /// <summary>
    /// Create the file, adding the initial elements to the XML
    /// </summary>
    protected override void CreateFile() {
      using (OpenWriteSession()) {
        AddXmlDeclaration(null, null, null);
        AddXmlElement(Xml, "APML", null);
        SetVersion();
        AddXmlAttribute(Xml.SelectSingleNode("/APML"), "Information", "http://www.apml.org");

        //Touchstone Only
        //SaveHeader("Touchstone APML File", "Touchstone", DateTime.Now.ToLongDateString(), Base.CurrentUser.EmailAddress);

        AddHead();
        AddDevices();
        AddBody();

        AddProfile("Home");
        SetDefaultProfile("Home");
      }
      Save();
    }
    #endregion

    public IReadOnlyDictionary<string, IDevice> Devices {
      get {
        EnsureDeviceCacheExists();

        return new ReadOnlyDictionary<string, IDevice>(mDevices);
      }
    }

    private void EnsureDeviceCacheExists() {
      using (OpenReadSession()) {
        // If the devices have already been created, we're done
        if (mDevices != null) {
          return;
        }
      }

      using (OpenWriteSession()) {
        if (mDevices != null) { // Prevent a race condition
          return;
        }

        // Allocate the devices structure
        mDevices = new Dictionary<string, IDevice>();

        // Work through each device
        XmlNodeList deviceNodes = Xml.SelectNodes("/APML/Devices/Device");
        foreach (XmlNode deviceNode in deviceNodes) {
          mDevices.Add(GetValue(deviceNode, "ID"), new XmlDeviceNode(this, deviceNode));
        }
      }
    }

    /// <summary>
    /// Add the Devices node
    /// </summary>
    private void AddDevices() {
      AddXmlElement(Xml.SelectSingleNode("/APML"), "Devices", null);
    }
  }
}
