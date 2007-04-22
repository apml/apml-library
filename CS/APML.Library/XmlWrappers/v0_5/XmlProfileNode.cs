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
  public class XmlProfileNode : XmlAPMLComponentBase, IProfile {
    private Dictionary<string, IExplicitConcept> mExplicitConcepts;
    private Dictionary<string, IList<IImplicitConcept>> mImplicitConcepts;
    private Dictionary<string, IExplicitSource> mSources;

    public XmlProfileNode(APMLFileBase pFile, XmlNode pNode) : base(pFile, pNode) {
    }

    #region IProfile Members
    public string Name {
      get { return GetAttribute("Name"); }
      set { FireNameChanged(SetAttribute("Name", value), value); }
    }

    public string Type {
      get { return GetAttribute("Type"); }
      set { SetAttribute("Type", value); }
    }

    public IExplicitConcept AddExplicitConcept(string pName, double pValue) {
      using (OpenWriteSession()) {
        EnsureExplicitConceptCacheExists();

        XmlNode concept = AddChildNode(
          "ExplicitConcepts", "Concept",
          new XAttribute("Phrase", pName), new XAttribute("Rank", pValue.ToString("f2")));

        
        XmlExplicitConceptNode conceptNode = new XmlExplicitConceptNode(File, concept);

        mExplicitConcepts.Add(pName, conceptNode);
        conceptNode.KeyChanged += new KeyChangedEventHandler<IExplicitConcept>(ExplicitConcepts_KeyChanged);
        conceptNode.Removed += new APMLComponentRemovedHandler(ExplicitConcepts_ConceptRemoved);

        return conceptNode;
      }
    }

    public IImplicitConcept AddImplicitConcept(string pName, double pValue) {
      using (OpenWriteSession()) {
        EnsureImplicitConceptCacheExists();

        XmlNode concept = AddChildNode(
          "ImplicitConcepts", "Concept",
          new XAttribute("Phrase", pName), new XAttribute("Rank", pValue.ToString("f2")));

        XmlImplicitConceptNode conceptNode = new XmlImplicitConceptNode(File, concept);
        if (!mImplicitConcepts.ContainsKey(pName)) {
          mImplicitConcepts.Add(pName, new List<IImplicitConcept>());
        }

        mImplicitConcepts[pName].Add(conceptNode);
        conceptNode.KeyChanged += new KeyChangedEventHandler<IImplicitConcept>(ImplicitConcepts_KeyChanged);
        conceptNode.Removed += new APMLComponentRemovedHandler(ImplicitConcepts_ConceptRemoved);

        return conceptNode;
      }
    }

    public IExplicitSource AddExplicitSource(string pKey, double pValue, string pName, string pType) {
      using (OpenWriteSession()) {
        EnsureSourceCacheExists();

        XmlNode source = AddChildNode(
          "ExplicitSources", "Source",
          new XAttribute("Url", pKey), new XAttribute("Rank", pValue.ToString("f2")),
          new XAttribute("Value", pName));

        
        XmlSourceNode sourceNode = new XmlSourceNode(File, source);

        mSources.Add(pName, sourceNode);
        sourceNode.KeyChanged += new KeyChangedEventHandler<IExplicitSource>(Sources_KeyChanged);
        sourceNode.Removed += new APMLComponentRemovedHandler(Sources_SourceRemoved);

        return sourceNode;
      }
    }

    public IImplicitSource AddImplicitSource(string pKey, double pValue, string pName, string pType) {
      // Ignored
      return null;
    }

    public void ClearImplicitConcepts() {
      using (OpenWriteSession()) {
        ClearChildContainer("ImplicitConcepts");

        if (mImplicitConcepts != null) {
          foreach (IImplicitConcept concept in mImplicitConcepts.Values) {
            concept.KeyChanged -= new KeyChangedEventHandler<IImplicitConcept>(ImplicitConcepts_KeyChanged);
            concept.Removed -= new APMLComponentRemovedHandler(ImplicitConcepts_ConceptRemoved);
          }

          mImplicitConcepts.Clear();
        } 
      }
    }

    public IReadOnlyDictionary<string, IExplicitConcept> ExplicitConcepts {
      get {
        EnsureExplicitConceptCacheExists();

        return new ReadOnlyDictionary<string, IExplicitConcept>(mExplicitConcepts);
      }
    }

    public IReadOnlyDictionary<string, IList<IImplicitConcept>> ImplicitConcepts {
      get {
        EnsureImplicitConceptCacheExists();

        return new ReadOnlyDictionary<string, IList<IImplicitConcept>>(mImplicitConcepts);
      }
    }

    public IReadOnlyDictionary<string, IExplicitSource> ExplicitSources {
      get {
        EnsureSourceCacheExists();

        return new ReadOnlyDictionary<string, IExplicitSource>(mSources);
      }
    }

    public IReadOnlyDictionary<string, IList<IImplicitSource>> ImplicitSources {
      get {
        return new ReadOnlyDictionary<string, IList<IImplicitSource>>(new Dictionary<string, IList<IImplicitSource>>());
      }
    }

    public event ProfileNameChangedEventHandler NameChanged;
    #endregion

    protected void FireNameChanged(string pOld, string pNew) {
      if (pOld == null) {
        return;
      }

      ProfileNameChangedEventHandler handler = NameChanged;

      if (handler != null) {
        handler(this, pOld, pNew);
      }
    }

    private void EnsureImplicitConceptCacheExists() {
      using (OpenReadSession()) {
        // If the cache has already been created, we're done
        if (mImplicitConcepts != null) {
          return;
        }
      }

      using (OpenWriteSession()) {
        if (mImplicitConcepts != null) { // Prevent a race condition
          return;
        }

        // Allocate the cache
        mImplicitConcepts = new Dictionary<string, IList<IImplicitConcept>>();

        // Work through each device
        XmlNodeList conceptNodes = Node.SelectNodes("ImplicitConcepts/Concept");
        foreach (XmlNode conceptNode in conceptNodes) {
          XmlImplicitConceptNode concept = new XmlImplicitConceptNode(File, conceptNode);

          string key = GetNodeAttribute(conceptNode, "Phrase");

          if (!mImplicitConcepts.ContainsKey(key)) {
            mImplicitConcepts.Add(key, new List<IImplicitConcept>());
          }

          mImplicitConcepts[key].Add(concept);
          concept.KeyChanged += new KeyChangedEventHandler<IImplicitConcept>(ImplicitConcepts_KeyChanged);
          concept.Removed += new APMLComponentRemovedHandler(ImplicitConcepts_ConceptRemoved);
        }
      }
    }

    private void EnsureExplicitConceptCacheExists() {
      using (OpenReadSession()) {
        // If the cache has already been created, we're done
        if (mExplicitConcepts != null) {
          return;
        }
      }

      using (OpenWriteSession()) {
        if (mExplicitConcepts != null) { // Prevent a race condition
          return;
        }

        // Allocate the cache
        mExplicitConcepts = new Dictionary<string, IExplicitConcept>();

        // Work through each device
        XmlNodeList conceptNodes = Node.SelectNodes("ExplicitConcepts/Concept");
        foreach (XmlNode conceptNode in conceptNodes) {
          XmlExplicitConceptNode concept = new XmlExplicitConceptNode(File, conceptNode);

          mExplicitConcepts.Add(GetNodeAttribute(conceptNode, "Phrase"), concept);
          concept.KeyChanged += new KeyChangedEventHandler<IExplicitConcept>(ExplicitConcepts_KeyChanged);
          concept.Removed += new APMLComponentRemovedHandler(ExplicitConcepts_ConceptRemoved);
        }
      }
    }

    private void EnsureSourceCacheExists() {
      using (OpenReadSession()) {
        // If the cache has already been created, we're done
        if (mSources != null) {
          return;
        }
      }

      using (OpenWriteSession()) {
        if (mSources != null) { // Prevent a race condition
          return;
        }

        // Allocate the cache
        mSources = new Dictionary<string, IExplicitSource>();

        // Work through each device
        XmlNodeList sourcesNode = Node.SelectNodes("ExplicitSources/Source");
        foreach (XmlNode sourceNode in sourcesNode) {
          XmlSourceNode source = new XmlSourceNode(File, sourceNode);

          mSources.Add(GetNodeAttribute(sourceNode, "Value"), source);
          source.KeyChanged += new KeyChangedEventHandler<IExplicitSource>(Sources_KeyChanged);
          source.Removed += new APMLComponentRemovedHandler(Sources_SourceRemoved);
        }
      }
    }

    #region Event Handlers
    private void ImplicitConcepts_KeyChanged(IImplicitConcept pConcept, string pOldName, string pNewName) {
      using (OpenWriteSession()) {
        if (mImplicitConcepts != null) {
          mImplicitConcepts[pOldName].Remove(pConcept);
          if (mImplicitConcepts[pOldName].Count == 0) {
            mImplicitConcepts.Remove(pOldName);
          }

          if (!mImplicitConcepts.ContainsKey(pNewName)) {
            mImplicitConcepts.Add(pNewName, new List<IImplicitConcept>());
          }
          mImplicitConcepts[pNewName].Add(pConcept);
        }
      }
    }

    private void ImplicitConcepts_ConceptRemoved(IAPMLComponent pComponent) {
      using (OpenWriteSession()) {
        IImplicitConcept concept = (IImplicitConcept)pComponent;

        concept.KeyChanged -= new KeyChangedEventHandler<IImplicitConcept>(ImplicitConcepts_KeyChanged);
        concept.Removed -= new APMLComponentRemovedHandler(ImplicitConcepts_ConceptRemoved);

        if (mImplicitConcepts != null) {
          mImplicitConcepts[concept.Key].Remove(concept);
          if (mImplicitConcepts[concept.Key].Count == 0) {
            mImplicitConcepts.Remove(concept.Key);
          }
        }
      }
    }

    private void ExplicitConcepts_KeyChanged(IExplicitConcept pConcept, string pOldName, string pNewName) {
      using (OpenWriteSession()) {
        if (mExplicitConcepts != null) {
          mExplicitConcepts.Remove(pOldName);
          mExplicitConcepts.Add(pNewName, pConcept);
        }
      }
    }

    private void ExplicitConcepts_ConceptRemoved(IAPMLComponent pComponent) {
      using (OpenWriteSession()) {
        IExplicitConcept concept = (IExplicitConcept) pComponent;

        concept.KeyChanged -= new KeyChangedEventHandler<IExplicitConcept>(ExplicitConcepts_KeyChanged);
        concept.Removed -= new APMLComponentRemovedHandler(ExplicitConcepts_ConceptRemoved);

        if (mExplicitConcepts != null) {
          mExplicitConcepts.Remove(concept.Key);
        }
      }
    }

    private void Sources_KeyChanged(IExplicitSource pSource, string pOldName, string pNewName) {
      using (OpenWriteSession()) {
        if (mSources != null) {
          mSources.Remove(pOldName);
          mSources.Add(pNewName, pSource);
        }
      }
    }

    private void Sources_SourceRemoved(IAPMLComponent pComponent) {
      using (OpenWriteSession()) {
        IExplicitSource source = (IExplicitSource) pComponent;

        source.KeyChanged -= new KeyChangedEventHandler<IExplicitSource>(Sources_KeyChanged);
        source.Removed -= new APMLComponentRemovedHandler(Sources_SourceRemoved);

        if (mSources != null) {
          mSources.Remove(source.Key);
        }
      }
    }
    #endregion
  }
}
