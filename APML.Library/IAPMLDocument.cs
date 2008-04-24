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

namespace APML {
  /// <summary>
  /// Defines the document model for an APML document.
  /// </summary>
  public interface IAPMLDocument : IAPMLLockable {
    /// <summary>
    /// Requests that the APMLDocument reload from its bound file.
    /// </summary>
    void Reload();

    /// <summary>
    /// Returns the document as XML.
    /// </summary>
    /// <returns></returns>
    string ToXml();

    /// <summary>
    /// Retrieves the active profile.
    /// </summary>
    IProfile ActiveProfile { get; }

    /// <summary>
    /// Retrieves the ID of the active profile.
    /// </summary>
    string ActiveProfileID { get; }

    /// <summary>
    /// Retrieves the current application object instance, that provides a repository
    /// for application specific data storage.
    /// </summary>
    IApplication Application { get; }

    /// <summary>
    /// Manages the Application Id.
    /// </summary>
    string ApplicationID { get; set; }

    /// <summary>
    /// Manages the creation date of the APML file.
    /// </summary>
    DateTime DateCreated { get; set; }

    /// <summary>
    /// Retrieves the default profile.
    /// </summary>
    IProfile DefaultProfile { get; }

    /// <summary>
    /// Retrieves the APML filename.
    /// </summary>
    string Filename { get; set; }

    /// <summary>
    /// Manages the generator of this document.
    /// </summary>
    string Generator { get; set; }

    /// <summary>
    /// Manages the title of the document.
    /// </summary>
    string Title { get; set; }

    /// <summary>
    /// Manages the email address of the owning user.
    /// </summary>
    string UserEmail { get; set; }

    /// <summary>
    /// Retrieves the profiles available within the document.
    /// </summary>
    IReadOnlyDictionary<string, IProfile> Profiles { get; }

    /// <summary>
    /// Saves the APML document.
    /// </summary>
    void Save();

    /// <summary>
    /// Configures whether the document should perform background saves.
    /// </summary>
    bool EnableBackgroundSave { get; set; }

    /// <summary>
    /// Sets the active profile.
    /// </summary>
    /// <param name="pName">the profile that should be active.</param>
    void SetActiveProfile(string pName);

    /// <summary>
    /// Sets the active profile.
    /// </summary>
    /// <param name="pProfile">reference to the profile that should be active.</param>
    void SetActiveProfile(IProfile pProfile);

    /// <summary>
    /// Sets the default profile.
    /// </summary>
    /// <param name="pName">name of the profile that should be active.</param>
    void SetDefaultProfile(string pName);

    /// <summary>
    /// Sets the default profile.
    /// </summary>
    /// <param name="pProfile">reference to the profile that should be default</param>
    void SetDefaultProfile(IProfile pProfile);

    /// <summary>
    /// Creates a new profile with the given name.
    /// </summary>
    /// <param name="pName">the name of the profile</param>
    void AddProfile(string pName);

    /// <summary>
    /// Whether this object just created the APML document.
    /// </summary>
    bool WasCreated { get; }

    /// <summary>
    /// Event indicating that a background save operation has failed.
    /// </summary>
    event BackgroundSaveFailureHandler BackgroundSaveFailed;
  }

  /// <summary>
  /// Delegate used to indicate that a background save has failed.
  /// </summary>
  /// <param name="pDocument">the document that the error originated in</param>
  /// <param name="pException">the exception that occurred</param>
  public delegate void BackgroundSaveFailureHandler(IAPMLDocument pDocument, Exception pException);
}
