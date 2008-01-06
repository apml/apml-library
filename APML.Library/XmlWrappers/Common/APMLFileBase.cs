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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using APML.Utilities;
using APML.XmlWrappers;

namespace APML.XmlWrappers.Common {

  /// <summary>
  /// Internal class to store and modify the APML XML Document
  /// </summary>
  /// <author>Michael McNeill</author>
  /// <author>Paul Jones</author>
  /// <copyright>Faraday Media 2006</copyright>
  public abstract class APMLFileBase : IAPMLDocument {
    #region Members
    /// <summary>
    /// The number of changes made since the last save.
    /// </summary>
    private int mCount;
    
    /// <summary>
    /// The name of the current file.
    /// </summary>
    private string mFilename;

    /// <summary>
    /// The current XML document.
    /// </summary>
    private XmlDocument mXML;

    /// <summary>
    /// The name of the active profile.
    /// </summary>
    private string mActiveProfileName;

    /// <summary>
    /// The id of the accessing application.
    /// </summary>
    private string mApplicationId;

    /// <summary>
    /// The cache of generated profiles.
    /// </summary>
    private IDictionary<string, IProfile> mProfiles;

    /// <summary>
    /// The wrapper for the profiles list that makes it accessible as a read-only colleciton.
    /// </summary>
    private IReadOnlyDictionary<string, IProfile> mReadOnlyProfiles;

    /// <summary>
    /// The cache for the current application.
    /// </summary>
    private IApplication mApplication;

    /// <summary>
    /// Helper for managing locking of the APML file
    /// </summary>
    private APMLLockHelper mLockHelper;

    /// <summary>
    /// Whether the file was created by this object (ie, it didn't exist before).
    /// </summary>
    private bool mWasCreated;

    /// <summary>
    /// Cache of the default profile value.
    /// </summary>
    private string mDefaultProfile;

    /// <summary>
    /// Lock used for serializing access to the on-disk file. This is done outside of the main lock since
    /// we don't need exclusive access to the APML structure - just the disk file.
    /// </summary>
    private object mFileAccessLock = new object();

    /// <summary>
    /// Whether we background save.
    /// </summary>
    private bool mEnableBackgroundSave = false;
    #endregion

    #region Enums

    public enum FormatDirection {
      In,
      Out
    }

    #endregion

    #region Constructors
    /// <summary>
    /// Initialize a new APML file
    /// </summary>
    /// <param name="pFilename">The full path and filename of the APML file to work with</param>
    public APMLFileBase(string pFilename) : this(pFilename, null) {
    }

    /// <summary>
    /// Initialize a new APML file, with the given (pre-loaded) APML document.
    /// </summary>
    /// <param name="pFilename">the fullpath and filename of the APML file to work with</param>
    /// <param name="pPreLoaded">a loaded XML document</param>
    public APMLFileBase(string pFilename, XmlDocument pPreLoaded) {
      // Allocate the lock
      mLockHelper = new APMLLockHelper();
      mLockHelper.WriteCompleted += new WriteCompletedEventHandler(LockHelper_WriteCompleted);

      // Store the file reference
      mFilename = pFilename;

      // Prepare the document
      if (pPreLoaded == null) {
        mXML = new XmlDocument();
        mXML.PreserveWhitespace = false;

        // Load the file
        LoadOrCreateFile();
      } else {
        mXML = pPreLoaded;
        mXML.PreserveWhitespace = false;

        mWasCreated = false;
      }
    }
    #endregion

    #region IAPMLDocument Members
    public void Reload() {
      // Clear all the caches
      mProfiles = null;
      mApplication = null;

      // Load the file in again
      LoadOrCreateFile();
    }

    public IProfile ActiveProfile {
      get { return Profiles[ActiveProfileID]; }
    }

    public string ActiveProfileID {
      get { return mActiveProfileName; }
    }

    public IApplication Application {
      get {
        if (ApplicationID == null) {
          throw new ArgumentException("ApplicationID has not been set");
        }

        EnsureApplicationCacheExists();

        return mApplication;
      }
    }

    public string ApplicationID {
      get { return mApplicationId; }
      set {
        using (OpenWriteSession()) {
          if (mApplicationId == value) {
            return;
          }

          mApplicationId = value;
          mApplication = null;
        }
      }
    }

    public IProfile DefaultProfile {
      get {
        using (OpenReadSession()) {
          if (mDefaultProfile == null) {
            mDefaultProfile = DefaultProfileAttribute;
          }

          return Profiles[mDefaultProfile];
        }
      }
    }

    public string Filename {
      get { return mFilename; }
    }

    public string Generator {
      get { return GetGenerator(); }
      set { UpdateHeaderItem("Generator", value); }
    }

    public string Title {
      get { return GetTitle(); }
      set { UpdateHeaderItem("Title", value); }
    }

    public string UserEmail {
      get { return GetUserEmail(); }
      set { UpdateHeaderItem("UserEmail", value); }
    }

    /// <summary>
    /// TODO: Proper date formatting!
    /// </summary>
    public DateTime DateCreated {
      get { return ParseDate(GetDateCreated()).GetValueOrDefault(DateTime.Now); }
      set { UpdateHeaderItem("DateCreated", DateToString(value)); }
    }

    public IReadOnlyDictionary<string, IProfile> Profiles {
      get {
        EnsureProfileCacheExists();

        return mReadOnlyProfiles;
      }
    }

    public void Save() {
      if (mEnableBackgroundSave) {
        Thread saveThread = new Thread(BackgroundSave);
        saveThread.IsBackground = false;
        saveThread.Start();
      } else {
        SaveInternal();
      }
    }

    public void SetActiveProfile(string pName) {
      mActiveProfileName = pName;
    }

    public void SetActiveProfile(IProfile pProfile) {
      mActiveProfileName = pProfile.Name;
    }

    
    public void SetDefaultProfile(IProfile pProfile) {
      SetDefaultProfile(pProfile.Name);
    }

    public void SetDefaultProfile(string pProfileName) {
      using (OpenWriteSession()) {
        if (Profiles.ContainsKey(pProfileName)) {
          DefaultProfileAttribute = pProfileName;
          mDefaultProfile = pProfileName;
        } else {
          throw new ArgumentException("Profile " + pProfileName + " doesn't exist");
        }
      }
    }


    public bool WasCreated {
      get { return mWasCreated; }
    }


    public bool EnableBackgroundSave {
      get { return mEnableBackgroundSave; }
      set { mEnableBackgroundSave = value; }
    }

    public event BackgroundSaveFailureHandler BackgroundSaveFailed;
    #endregion

    #region Public Methods
    public DateTime? GetAttributeAsDateTime(XmlNode pNode, string pAttr) {
      string val = GetValue(pNode, pAttr);

      return ParseDate(val);
    }

    public DateTime? SetAttributeAsDateTime(XmlNode pNode, string pAttr, DateTime? pValue) {
      // TODO: Implement proper parsing
      string previous = AddXmlAttribute(pNode, pAttr, DateToString(pValue));

      return ParseDate(previous);
    }
    #endregion

    #region Abstract Methods
    public abstract void AddProfile(string pName);

    protected abstract void EnsureProfileCacheExists();

    protected abstract void EnsureApplicationCacheExists();

    protected abstract string DefaultProfileAttribute { get; set; }

    public abstract DateTime? ParseDate(string pDateStr);

    public abstract string DateToString(DateTime? pDate);

    protected abstract void SetVersion();

    protected abstract void CreateFile();
    #endregion

    #region IAPMLLockable Members
    public IAPMLReadSession OpenReadSession() {
      return mLockHelper.OpenReadSession();
    }
    
    public IAPMLWriteSession OpenWriteSession() {
      return mLockHelper.OpenWriteSession();
    }
    #endregion

    #region Private Methods

    /// <summary>
    /// Call to Check Save with no Force Save
    /// </summary>
    private void CheckSave() {
      CheckSave(false);
    }

    /// <summary>
    /// Check if the file needs Saving (or if forced, Save)
    /// </summary>
    /// <param name="pForceSave">Whether the file should be forced to Save</param>
    private void CheckSave(bool pForceSave) {
      if (Count >= 5 | pForceSave) {
        Save();
        
        Count = 0;
      }
    }

    /// <summary>
    /// Retrieves the Date Created data from the APML Header
    /// </summary>
    /// <returns>The value of the Date Created node if it exists, otherwise ""</returns>
    private string GetDateCreated() {
      return GetHeadAttributeOrEmpty("DateCreated");
    }

    /// <summary>
    /// Retrieves the Generator data from the APML Header
    /// </summary>
    /// <returns>The value of the Generator node if it exists, otherwise ""</returns>
    private string GetGenerator() {
      return GetHeadAttributeOrEmpty("Generator");
    }

    /// <summary>
    /// Retrieves the Title data from the APML Header
    /// </summary>
    /// <returns>The value of the Title node if it exists, otherwise ""</returns>
    private string GetTitle() {
      return GetHeadAttributeOrEmpty("Title");
    }

    /// <summary>
    /// Retrives the User Email data from the APML Header
    /// </summary>
    /// <returns>The value of the User Email node if it exists, otherwise ""</returns>
    private string GetUserEmail() {
      return GetHeadAttributeOrEmpty("UserEmail");
    }

    private void UpdateHeaderItem(string pHeaderItem, string pValue) {
      using (OpenWriteSession()) {
        XmlNode headNode = mXML.SelectSingleNode("/APML/Head");

        if (headNode == null) {
          headNode = AddXmlElement(mXML.SelectSingleNode("/APML"), "Head", null);
        }

        XmlNode itemNode = headNode.SelectSingleNode(pHeaderItem);
        if (itemNode == null) {
          AddXmlElement(headNode, pHeaderItem, pValue);
        } else {
          itemNode.InnerText = pValue;
        }
      }
    }
    
    /// <summary>
    /// Add the Applications node
    /// </summary>
    private XmlNode AddApplications() {
      return AddXmlElement(mXML.SelectSingleNode("/APML"), "Applications", null);
    }

    /// <summary>
    /// Add the Head node
    /// </summary>
    protected void AddHead() {
      AddXmlElement(mXML.SelectSingleNode("/APML"), "Head", null);
    }

    /// <summary>
    /// Add the Body node
    /// </summary>
    protected void AddBody() {
      AddXmlElement(mXML.SelectSingleNode("/APML"), "Body", null);
    }

    /// <summary>
    /// Adds an XML Attribute to the node
    /// </summary>
    /// <param name="xml_node">XmlNode to add the Attribute to</param>
    /// <param name="attribute_name">Name of the Attribute</param>
    /// <param name="attribute_value">Value of the Attribute</param>
    public static string AddXmlAttribute(XmlNode xml_node, string attribute_name, string attribute_value) {
      return AddXmlAttribute(xml_node, attribute_name, attribute_value, true);
    }

    /// <summary>
    /// Adds an XML Attribute to the node
    /// </summary>
    /// <param name="pXmlNode">XmlNode to add the Attribute to</param>
    /// <param name="pAttributeName">Name of the Attribute</param>
    /// <param name="pAttributeValue">Value of the Attribute</param>
    /// <param name="pDoFormat">Specify if pAttributeValue is to be formatted</param>
    public static string AddXmlAttribute(XmlNode pXmlNode, string pAttributeName, string pAttributeValue, bool pDoFormat) {
      XmlAttribute attr = (XmlAttribute) pXmlNode.Attributes.GetNamedItem(pAttributeName);
      string oldValue = null;

      if (attr == null) {
        attr = pXmlNode.OwnerDocument.CreateAttribute(pAttributeName);
        pXmlNode.Attributes.Append(attr);
      } else {
        oldValue = attr.Value;
      }

      if (pDoFormat) {
        attr.Value = Format(pAttributeValue, FormatDirection.In);
      } else {
        attr.Value = pAttributeValue;
      }

      return oldValue;
    }

    /// <summary>
    /// Removes an XML attribute from the node.
    /// </summary>
    /// <param name="pXmlNode">XmlNode to add the Attribute to</param>
    /// <param name="pAttributeName">Name of the Attribute</param>
    public static void DeleteXmlAttribute(XmlNode pXmlNode, string pAttributeName) {
      pXmlNode.Attributes.RemoveNamedItem(pAttributeName);
    }

    /// <summary>
    /// Adds an XML Declaration to the XML document
    /// </summary>
    /// <param name="version">XML Version</param>
    /// <param name="encoding">XML Encoding</param>
    /// <param name="standalone">XML Standalone</param>
    protected void AddXmlDeclaration(string version, string encoding, string standalone) {
      if (version == null) version = "1.0";
      mXML.PrependChild(mXML.CreateXmlDeclaration(version, encoding, standalone));
    }

    /// <summary>
    /// Adds an XML Element with an optional inner value
    /// </summary>
    /// <param name="parent_node">XMLNode to add the Element to</param>
    /// <param name="element_name">Name of the Element</param>
    /// <param name="element_value">(Optional) Inner Value</param>
    protected XmlNode AddXmlElement(XmlNode parent_node, string element_name, string element_value) {
      if (element_value == null) element_value = "";
      XmlElement xml_element;
      xml_element = mXML.CreateElement(element_name);
      if (element_value != "") {
        xml_element.InnerText = element_value;
      }
      parent_node.AppendChild(xml_element);

      return xml_element;
    }

    /// <summary>
    /// Adds an XML Element with a key attribute
    /// </summary>
    /// <param name="parent">Node XPath to add the Element to</param>
    /// <param name="element_name">Name of the Element</param>
    /// <param name="key_attribute_name">Name of the Key Attribute</param>
    /// <param name="key_attribute_value">Value of the Key Attribute</param>
    protected XmlNode AddXmlElement(XmlNode parent, string element_name, string key_attribute_name, string key_attribute_value) {
      XmlElement xml_element;
      XmlAttribute xml_attribute;

      XmlNode current = parent.SelectSingleNode(element_name + "[translate(@" + key_attribute_name +
                              ",'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='" +
                              Format(key_attribute_value.ToLower(), FormatDirection.In) + "']");
      if (current != null) {
        return current;
      }

      xml_element = mXML.CreateElement(element_name);
      xml_attribute = mXML.CreateAttribute(key_attribute_name);
      xml_attribute.Value = Format(key_attribute_value, FormatDirection.In);
      xml_element.Attributes.Append(xml_attribute);
      parent.AppendChild(xml_element);

      return xml_element;
    }

    /// <summary>
    /// Formats the specified text in the direction
    /// </summary>
    /// <param name="value">Text to format</param>
    /// <param name="direction">Direction of formatting (FormatDirection.In = to XML, FormatDirection.Out = to human-readable)</param>
    /// <returns>If direction is In, the XML-safe text; if direction is Out, the human-readable text</returns>
    public static string Format(string value, FormatDirection direction) {
      switch (direction) {
        case FormatDirection.In:
          return XmlHelper.ConvertToXmlString(value);
        case FormatDirection.Out:
          return XmlHelper.ConvertToNormalString(value);
        default:
          return value;
      }
    }

    /// <summary>
    /// Retrieves the given attribute from the given node and formats
    /// </summary>
    /// <param name="node">The XmlNode to retrieve the attribute from</param>
    /// <param name="attribute_name">The attribute name in the given node</param>
    /// <returns>If the node exists, the human-readable text.  If not, returns ""</returns>
    public static string GetValue(XmlNode node, string attribute_name) {
      if ((node.Attributes.GetNamedItem(attribute_name) == null)) {
        return "";
      } else {
        string result = node.Attributes.GetNamedItem(attribute_name).Value;
        /*if (CheckFormat(ref result, FormatDirection.Out)) {
          AddXmlAttribute(node, attribute_name, result);
        }
        if (CheckFormat(ref result, FormatDirection.In)) {
          AddXmlAttribute(node, attribute_name, result, false);
        }*/
        return Format(result, FormatDirection.Out);
      }
    }

    /// <summary>
    /// Increase the Change Count and check if the files needs Saving
    /// </summary>
    private void IncreaseCount() {
      Count += 1;
      CheckSave();
    }

    /// <summary>
    /// Checks if the File exists, otherwise create the file, and check if the Touchstone values exist in the file
    /// </summary>
    private void LoadOrCreateFile() {
      if (File.Exists(mFilename) && new FileInfo(mFilename).Length > 0) {
        using (OpenWriteSession()) {
          mXML.Load(mFilename);
        }

        mWasCreated = true;
      } else {
        CreateFile();

        mWasCreated = true;
      }
    }

    private string GetHeadAttributeOrEmpty(string pAttributeName) {
      using (OpenReadSession()) {
        XmlNode headAttr = mXML.SelectSingleNode("/APML/Head/" + pAttributeName);

        if (headAttr == null) {
          return "";
        } else {
          return headAttr.InnerText;
        }
      }
    }

    private void BackgroundSave() {
      try {
        SaveInternal();
      } catch (Exception ex) {
        BackgroundSaveFailureHandler handler = BackgroundSaveFailed;
        if (handler != null) {
          handler(this, ex);
        }
      }
    }

    private void SaveInternal() {
      using (OpenReadSession()) {
        // Open an exclusive lock for the file
        lock (mFileAccessLock) {
          // Save to a temp file first
          string mTempFilename = mFilename + ".tmp";
          mXML.Save(mTempFilename);

          if (!File.Exists(mFilename)) {
            // Make the temp file the real file
            File.Move(mTempFilename, mFilename);
          } else {
            string backupName = mFilename + "~";

            File.Replace(mTempFilename, mFilename, backupName);
          }
        }
      }
    }
    #endregion

    #region Protected Properties
    protected IDictionary<string, IProfile> ProfileCache {
      get { return mProfiles; }
      set {
        mProfiles = value;
        mReadOnlyProfiles = new ReadOnlyDictionary<string, IProfile>(mProfiles);
      }
    }

    protected XmlDocument Xml {
      get { return mXML; }
    }

    protected IApplication LoadedApplication {
      get { return mApplication; }
      set { mApplication = value; }
    }
    #endregion

    #region Node Access Methods
    protected XmlNode GetBodyNode() {
      return mXML.SelectSingleNode("/APML/Body");
    }

    protected XmlNode GetApplicationsNode() {
      return mXML.SelectSingleNode("/APML/Body/Applications");
    }

    protected XmlNode GetApplicationNode(string pAppName) {
      return mXML.SelectSingleNode(BuildApplicationPath(pAppName));
    }
    #endregion

    #region String Building Methods
    private static string BuildProfilePath(string pProfileName) {
      return
        "/APML/Body/Profile[translate(@Name,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz')='" +
        Format(pProfileName.ToLower(), FormatDirection.In) + "']";
    }

    private static string BuildApplicationPath(string pApplicationName) {
      return "/APML/Applications/Application[translate(@Name,'ABCDEFGHIJKLMNOPQRSTUVWXYZ','abcdefghijklmnopqrstuvwxyz') = '" + 
        Format(pApplicationName.ToLower(), FormatDirection.In) + "']";
    }
    #endregion

    #region Event Handlers
    private void LockHelper_WriteCompleted(APMLLockHelper pHelper) {
      // Count every time a write lock is released
      IncreaseCount();
    }

    protected void Profiles_NameChanged(IProfile pSource, string pOldName, string pNewName) {
      using (OpenWriteSession()) {
        if (mProfiles != null) {
          mProfiles.Remove(pOldName);
          mProfiles.Add(pNewName, pSource);
        }
      }
    }

    protected void Profiles_ProfileRemoved(IAPMLComponent pComponent) {
      using (OpenWriteSession()) {
        IProfile profile = (IProfile) pComponent;

        profile.NameChanged -= new ProfileNameChangedEventHandler(Profiles_NameChanged);
        profile.Removed -= new APMLComponentRemovedHandler(Profiles_ProfileRemoved);

        if (mProfiles != null) {
          mProfiles.Remove(profile.Name);
        }
      }
    }
    #endregion

    #region Properties

    /// <summary>
    /// Change Count
    /// </summary>
    private int Count {
      get { return mCount; }
      set { mCount = value; }
    }

    #endregion

  }

}
