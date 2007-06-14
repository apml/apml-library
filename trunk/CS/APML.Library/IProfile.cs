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
  public delegate void ProfileNameChangedEventHandler(IProfile pSender, string pOldName, string pNewName);

  /// <summary>
  /// Defines the object model for an APML profile.
  /// </summary>
  public interface IProfile : IAPMLComponent {
    /// <summary>
    /// The name of the profile.
    /// </summary>
    string Name { get; set; }
    
    /// <summary>
    /// Adds a new explicit concept to the user's profile.
    /// </summary>
    /// <param name="pKey">the key of the concept</param>
    /// <param name="pValue">the value of the concept</param>
    /// <returns>the generated explicit concept</returns>
    IExplicitConcept AddExplicitConcept(string pKey, double pValue);

    /// <summary>
    /// Adds a new implicit concept to the user's profile. Note that the
    /// Updated and From fields are dynamically populated.
    /// </summary>
    /// <param name="pKey">the key of the concept</param>
    /// <param name="pValue">the value of the concept</param>
    /// <returns>the generated implicit concept</returns>
    IImplicitConcept AddImplicitConcept(string pKey, double pValue);

    /// <summary>
    /// Adds a new explicit source to the user's profile.
    /// </summary>
    /// <param name="pKey">the key for the source</param>
    /// <param name="pValue">the value for this source</param>
    /// <param name="pName">the friendly name for the source</param>
    /// <param name="pType">the type of the source, expressed as a mime-type</param>
    /// <returns>the generated explicit source</returns>
    IExplicitSource AddExplicitSource(string pKey, double pValue, string pName, string pType);

    /// <summary>
    /// Adds a new implicit source to the user's profile. Note that the
    /// Updated and 
    /// </summary>
    /// <param name="pKey">the key for the source</param>
    /// <param name="pValue">the value for this source</param>
    /// <param name="pName">the friendly name for the source</param>
    /// <param name="pType">the type of the source, expressed as a mime-type</param>
    /// <returns>the generated implicit source</returns>
    IImplicitSource AddImplicitSource(string pKey, double pValue, string pName, string pType);

    /// <summary>
    /// Clears the user's set of implicit concepts.
    /// </summary>
    void ClearImplicitConcepts();

    /// <summary>
    /// Clears the user's set of implicit concepts, where the provided from tag is matched.
    /// <param name="pFrom">the application to clear the concepts from</param>
    /// </summary>
    void ClearImplicitConcepts(string pFrom);

    /// <summary>
    /// Retrieves all of the user's explicit concepts.
    /// </summary>
    IReadOnlyDictionary<string, IExplicitConcept> ExplicitConcepts { get; }

    /// <summary>
    /// Retrieves all of the implicit concepts managed by this application.
    /// </summary>
    IReadOnlyDictionary<string, IList<IImplicitConcept>> ImplicitConcepts { get; }

    /// <summary>
    /// Retrieves all of the user's explicit sources.
    /// </summary>
    IReadOnlyDictionary<string, IExplicitSource> ExplicitSources { get; }

    /// <summary>
    /// Retrieves all of the user's implicit sources.
    /// </summary>
    IReadOnlyDictionary<string, IList<IImplicitSource>> ImplicitSources { get; }

    /// <summary>
    /// Event issued when the name of the profile changes.
    /// </summary>
    event ProfileNameChangedEventHandler NameChanged;
  }
}