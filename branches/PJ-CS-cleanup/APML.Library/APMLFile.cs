using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using APML.AutoWrapper;

namespace APML {
  public class APMLFile : IAPMLDocument {
    private string mFileName;
    private IAPMLRoot mRoot;
    private XmlDocument mDoc;
    private readonly bool mWasCreated;
    private readonly AutoWrapperGenerator mGenerator;
    private string mApplicationId;
    private IProfile mActiveProfile;

    public APMLFile(string pFileName, XmlDocument pDoc, bool pWasCreated, AutoWrapperGenerator pGenerator) {
      mFileName = pFileName;
      mDoc = pDoc;
      mWasCreated = pWasCreated;

      mGenerator = pGenerator;
      mRoot = mGenerator.GenerateWrapper<IAPMLRoot>(mDoc.DocumentElement);

      mActiveProfile = DefaultProfile;
    }

    #region IAPMLDocument Members
    public string Title {
      get { return mRoot.Head.Title; }
      set { mRoot.Head.Title = value; }
    }

    public string UserEmail {
      get { return mRoot.Head.UserEmail; }
      set { mRoot.Head.UserEmail = value; }
    }

    public IReadOnlyDictionary<string, IProfile> Profiles {
      get { return mRoot.Body.Profiles; }
    }

    public void Save() {
      if (mFileName == null) {
        throw new ArgumentNullException("Filename", "Must set Filename before saving");
      }

      mDoc.Save(mFileName);
    }

    public bool EnableBackgroundSave {
      get { throw new NotImplementedException(); }
      set { throw new NotImplementedException(); }
    }

    public void SetActiveProfile(string pName) {
      mActiveProfile = mRoot.Body.Profiles[pName];
    }

    public void SetActiveProfile(IProfile pProfile) {
      mActiveProfile = pProfile;
    }

    public void SetDefaultProfile(string pName) {
      mRoot.Body.DefaultProfile = pName;
    }

    public void SetDefaultProfile(IProfile pProfile) {
      mRoot.Body.DefaultProfile = pProfile.Name;
    }

    public void AddProfile(string pName) {
      mRoot.Body.AddProfile(pName);
    }

    public bool WasCreated {
      get { return mWasCreated; }
    }

    public event BackgroundSaveFailureHandler BackgroundSaveFailed;


    public void Reload() {
      mDoc.Load(mFileName);

      throw new NotImplementedException("Root is not being reset - data may be inconsistent");
      // TODO: Inform the root to reset!
    }

    public string ToXml() {
      XmlWriterSettings settings = new XmlWriterSettings();
      settings.Indent = true;

      StringWriter result = new StringWriter();
      XmlWriter xw = XmlWriter.Create(result, settings);

      mDoc.WriteTo(xw);
      xw.Close();

      return result.ToString();
    }

    public IProfile ActiveProfile {
      get { return mActiveProfile; }
    }

    public string ActiveProfileID {
      get { return mActiveProfile.Name; }
    }

    public IApplication Application {
      get { throw new NotImplementedException(); }
    }

    public string ApplicationID {
      get { return mApplicationId; }
      set { mApplicationId = value; }
    }

    public DateTime DateCreated {
      get { return mRoot.Head.DateCreated; }
      set { mRoot.Head.DateCreated = value; }
    }

    public IProfile DefaultProfile {
      get {
        if (mRoot.Body.DefaultProfile == null) {
          mRoot.Body.DefaultProfile = APMLConstants.STANDARD_DEFAULT_PROFILE;
        }

        if (!mRoot.Body.Profiles.ContainsKey(mRoot.Body.DefaultProfile)) {
          return mRoot.Body.AddProfile(mRoot.Body.DefaultProfile);
        } else {
          return mRoot.Body.Profiles[mRoot.Body.DefaultProfile];
        }
      }
    }

    public string Filename {
      get { return mFileName; }
      set { mFileName = value; }
    }

    public string Generator {
      get { return mRoot.Head.Generator; }
      set { mRoot.Head.Generator = value; }
    }

    #endregion

    #region IAPMLLockable Members
    public IAPMLReadSession OpenReadSession() {
      throw new NotImplementedException();
    }

    public IAPMLWriteSession OpenWriteSession() {
      throw new NotImplementedException();
    }

    #endregion
  }
}
