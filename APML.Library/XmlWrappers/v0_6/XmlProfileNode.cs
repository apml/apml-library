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
using System.Globalization;
using System.Text;
using System.Xml;
using APML.XmlWrappers.Common;

namespace APML.XmlWrappers.v0_6 {
  public class XmlProfileNode : XmlAPMLComponentBase, IProfile {
    private Dictionary<string, IExplicitConcept> mExplicitConcepts;
    private Dictionary<string, IList<IImplicitConcept>> mImplicitConcepts;
    private Dictionary<string, IExplicitSource> mExplicitSources;
    private Dictionary<string, IList<IImplicitSource>> mImplicitSources;

    private object mExplicitConceptLock = new object();
    private object mImplicitConceptLock = new object();
    private object mExplicitSourceLock = new object();
    private object mImplicitSourceLock = new object();

    private KeyChangedEventHandler<IExplicitConcept> mExplicitConceptKeyChanged;
    private APMLComponentRemovedHandler mExplicitConceptRemoved;
    private KeyChangedEventHandler<IImplicitConcept> mImplicitConceptKeyChanged;
    private APMLComponentRemovedHandler mImplicitConceptRemoved;
    private KeyChangedEventHandler<IExplicitSource> mExplicitSourceKeyChanged;
    private APMLComponentRemovedHandler mExplicitSourceRemoved;
    private KeyChangedEventHandler<IImplicitSource> mImplicitSourceKeyChanged;
    private APMLComponentRemovedHandler mImplicitSourceRemoved;

    public XmlProfileNode(APMLFileBase pFile, XmlNode pNode) : base(pFile, pNode) {
      mExplicitConceptKeyChanged = new KeyChangedEventHandler<IExplicitConcept>(ExplicitConcepts_KeyChanged);
      mExplicitConceptRemoved = new APMLComponentRemovedHandler(ExplicitConcepts_ConceptRemoved);
      mImplicitConceptKeyChanged = new KeyChangedEventHandler<IImplicitConcept>(ImplicitConcepts_KeyChanged);
      mImplicitConceptRemoved = new APMLComponentRemovedHandler(ImplicitConcepts_ConceptRemoved);
      mExplicitSourceKeyChanged = new KeyChangedEventHandler<IExplicitSource>(ExplicitSources_KeyChanged);
      mExplicitSourceRemoved = new APMLComponentRemovedHandler(ExplicitSources_SourceRemoved);
      mImplicitSourceKeyChanged = new KeyChangedEventHandler<IImplicitSource>(ImplicitSources_KeyChanged);
      mImplicitSourceRemoved = new APMLComponentRemovedHandler(ImplicitSources_SourceRemoved);
    }

    #region IProfile Members
    public string Name {
      get { return GetAttribute("name"); }
      set { FireNameChanged(SetAttribute("name", value), value); }
    }

    public IExplicitConcept AddExplicitConcept(string pKey, double pValue) {
      using (OpenWriteSession()) {
        EnsureExplicitConceptCacheExists();

        if (mExplicitConcepts.ContainsKey(pKey)) {
          throw new ArgumentException(pKey + " is already an explicit concept");
        }

        XmlNode concept = AddChildNode(
          "ExplicitData/Concepts", "Concept",
          new XAttribute("key", pKey), new XAttribute("value", pValue.ToString("f2", CultureInfo.InvariantCulture)));

        XmlExplicitConceptNode conceptNode = new XmlExplicitConceptNode(File, concept);
        mExplicitConcepts.Add(pKey, conceptNode);
        conceptNode.KeyChanged += mExplicitConceptKeyChanged;
        conceptNode.Removed += mExplicitConceptRemoved;

        return conceptNode;
      }
    }

    public IImplicitConcept AddImplicitConcept(string pKey, double pValue) {
      using (OpenWriteSession()) {
        EnsureImplicitConceptCacheExists();

        XmlNode concept = AddChildNode(
          "ImplicitData/Concepts", "Concept",
          new XAttribute("key", pKey), new XAttribute("value", pValue.ToString("f2", CultureInfo.InvariantCulture)));
        AddImplicitAttributes(concept);

        XmlImplicitConceptNode conceptNode = new XmlImplicitConceptNode(File, concept);
        if (!mImplicitConcepts.ContainsKey(pKey)) {
          mImplicitConcepts.Add(pKey, new List<IImplicitConcept>());
        }

        mImplicitConcepts[pKey].Add(conceptNode);
        conceptNode.KeyChanged += mImplicitConceptKeyChanged;
        conceptNode.Removed += mImplicitConceptRemoved;

        return conceptNode;
      }
    }

    public IExplicitSource AddExplicitSource(string pKey, double pValue, string pName, string pType) {
      using (OpenWriteSession()) {
        EnsureExplicitSourceCacheExists();

        if (mExplicitSources.ContainsKey(pKey)) {
          throw new ArgumentException(pKey + " is already an explicit source");
        }

        XmlNode source = AddChildNode(
          "ExplicitData/Sources", "Source",
          new XAttribute("key", pKey), new XAttribute("value", pValue.ToString("f2", CultureInfo.InvariantCulture)),
          new XAttribute("name", pName), new XAttribute("type", pType));

        XmlExplicitSourceNode sourceNode = new XmlExplicitSourceNode(File, source);
        mExplicitSources.Add(pKey, sourceNode);
        sourceNode.KeyChanged += mExplicitSourceKeyChanged;
        sourceNode.Removed += mExplicitSourceRemoved;

        return sourceNode;
      }
    }

    public IImplicitSource AddImplicitSource(string pKey, double pValue, string pName, string pType) {
      using (OpenWriteSession()) {
        EnsureImplicitSourceCacheExists();

        XmlNode source = AddChildNode(
          "ImplicitData/Sources", "Source",
          new XAttribute("key", pKey), new XAttribute("value", pValue.ToString("f2", CultureInfo.InvariantCulture)),
          new XAttribute("name", pName), new XAttribute("type", pType));
        AddImplicitAttributes(source);

        XmlImplicitSourceNode sourceNode = new XmlImplicitSourceNode(File, source);

        if (!mImplicitSources.ContainsKey(pKey)) {
          mImplicitSources.Add(pKey, new List<IImplicitSource>());
        }
        mImplicitSources[pKey].Add(sourceNode);
        sourceNode.KeyChanged += mImplicitSourceKeyChanged;
        sourceNode.Removed += mImplicitSourceRemoved;

        return sourceNode;
      }
    }

    public void ClearImplicitConcepts() {
      using (OpenWriteSession()) {
        ClearChildContainer("ImplicitData/Concepts");

        if (mImplicitConcepts != null) {
          foreach (IList<IImplicitConcept> conceptList in mImplicitConcepts.Values) {
            foreach (IImplicitConcept concept in conceptList) {
              concept.KeyChanged -= mImplicitConceptKeyChanged;
              concept.Removed -= mImplicitConceptRemoved;
            }
          }

          mImplicitConcepts.Clear();
        } 
      }
    }

    public void ClearImplicitConcepts(string pFrom) {
      using (OpenWriteSession()) {
        ClearChildContainer("ImplicitData/Concepts");

        if (mImplicitConcepts != null) {
          List<string> lRemove = new List<string>();

          foreach (string conceptName in mImplicitConcepts.Keys) {
            IList<IImplicitConcept> conceptList = mImplicitConcepts[conceptName];

            List<IImplicitConcept> cRemove = new List<IImplicitConcept>();

            foreach (IImplicitConcept concept in conceptList) {
              if (concept.From == pFrom) {
                concept.KeyChanged -= mImplicitConceptKeyChanged;
                concept.Removed -= mImplicitConceptRemoved;

                cRemove.Add(concept);
              }
            }

            foreach (IImplicitConcept r in cRemove) {
              conceptList.Remove(r);
            }

            if (conceptList.Count == 0) {
              lRemove.Add(conceptName);
            }
          }

          foreach (string r in lRemove) {
            mImplicitConcepts.Remove(r);
          }
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
        EnsureExplicitSourceCacheExists();

        return new ReadOnlyDictionary<string, IExplicitSource>(mExplicitSources);
      }
    }

    public IReadOnlyDictionary<string, IList<IImplicitSource>> ImplicitSources {
      get {
        EnsureImplicitSourceCacheExists();

        return new ReadOnlyDictionary<string, IList<IImplicitSource>>(mImplicitSources);
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
        
        lock (mImplicitConceptLock) {
          if (mImplicitConcepts != null) {
            // Prevent a race condition
            return;
          }

          // Allocate the cache
          mImplicitConcepts = new Dictionary<string, IList<IImplicitConcept>>();

          // Work through each device
          XmlNodeList conceptNodes = Node.SelectNodes("ImplicitData/Concepts/Concept");
          foreach (XmlNode conceptNode in conceptNodes) {
            XmlImplicitConceptNode concept = new XmlImplicitConceptNode(File, conceptNode);

            if (!mImplicitConcepts.ContainsKey(concept.Key)) {
              mImplicitConcepts.Add(concept.Key, new List<IImplicitConcept>());
            }

            mImplicitConcepts[concept.Key].Add(concept);
            concept.KeyChanged += mImplicitConceptKeyChanged;
            concept.Removed += mImplicitConceptRemoved;
          }
        }
      }
    }

    private void EnsureExplicitConceptCacheExists() {
      using (OpenReadSession()) {
        // If the cache has already been created, we're done
        if (mExplicitConcepts != null) {
          return;
        }

        lock (mExplicitConceptLock) {
          if (mExplicitConcepts != null) {
            // Prevent a race condition
            return;
          }

          // Allocate the cache
          mExplicitConcepts = new Dictionary<string, IExplicitConcept>();

          // Work through each device
          XmlNodeList conceptNodes = Node.SelectNodes("ExplicitData/Concepts/Concept");
          foreach (XmlNode conceptNode in conceptNodes) {
            XmlExplicitConceptNode concept = new XmlExplicitConceptNode(File, conceptNode);

            if (!mExplicitConcepts.ContainsKey(concept.Key)) {
              mExplicitConcepts.Add(concept.Key, concept);
              concept.KeyChanged += mExplicitConceptKeyChanged;
              concept.Removed += mExplicitConceptRemoved;
            } else {
              Debug.WriteLine("Warning: Duplicate Explicit Concept: " + concept.Key);
            }
          }
        }
      }
    }

    private void EnsureExplicitSourceCacheExists() {
      using (OpenReadSession()) {
        // If the cache has already been created, we're done
        if (mExplicitSources != null) {
          return;
        }

        lock (mExplicitSourceLock) {
          if (mExplicitSources != null) {
            // Prevent a race condition
            return;
          }

          // Allocate the cacheDe
          mExplicitSources = new Dictionary<string, IExplicitSource>();

          // Work through each device
          XmlNodeList sourcesNode = Node.SelectNodes("ExplicitData/Sources/Source");
          foreach (XmlNode sourceNode in sourcesNode) {
            XmlExplicitSourceNode source = new XmlExplicitSourceNode(File, sourceNode);

            if (!mExplicitSources.ContainsKey(source.Key)) {
              mExplicitSources.Add(source.Key, source);
              source.KeyChanged += mExplicitSourceKeyChanged;
              source.Removed += mExplicitSourceRemoved;
            } else {
              Debug.WriteLine("Warning: Duplicate Explicit Source: " + source.Key);
            }
          }
        }
      }
    }

    private void EnsureImplicitSourceCacheExists() {
      using (OpenReadSession()) {
        // If the cache has already been created, we're done
        if (mImplicitSources != null) {
          return;
        }

        lock (mImplicitSourceLock) {
          if (mImplicitSources != null) {
            // Prevent a race condition
            return;
          }

          // Allocate the cache
          mImplicitSources = new Dictionary<string, IList<IImplicitSource>>();

          // Work through each device
          XmlNodeList sourcesNode = Node.SelectNodes("ImplicitData/Sources/Source");
          foreach (XmlNode sourceNode in sourcesNode) {
            XmlImplicitSourceNode source = new XmlImplicitSourceNode(File, sourceNode);

            if (!mImplicitSources.ContainsKey(source.Key)) {
              mImplicitSources.Add(source.Key, new List<IImplicitSource>());
            }

            mImplicitSources[source.Key].Add(source);
            source.KeyChanged += mImplicitSourceKeyChanged;
            source.Removed += mImplicitSourceRemoved;
          }
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
        IImplicitConcept concept = (IImplicitConcept) pComponent;

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

    private void ExplicitSources_KeyChanged(IExplicitSource pSource, string pOldName, string pNewName) {
      using (OpenWriteSession()) {
        if (mExplicitSources != null) {
          mExplicitSources.Remove(pOldName);
          mExplicitSources.Add(pNewName, pSource);
        }
      }
    }

    private void ExplicitSources_SourceRemoved(IAPMLComponent pComponent) {
      using (OpenWriteSession()) {
        IExplicitSource source = (IExplicitSource) pComponent;

        source.KeyChanged -= new KeyChangedEventHandler<IExplicitSource>(ExplicitSources_KeyChanged);
        source.Removed -= new APMLComponentRemovedHandler(ExplicitSources_SourceRemoved);

        if (mExplicitSources != null) {
          mExplicitSources.Remove(source.Key);
        }
      }
    }

    private void ImplicitSources_KeyChanged(IImplicitSource pSource, string pOldName, string pNewName) {
      using (OpenWriteSession()) {
        if (mImplicitSources != null) {
          mImplicitSources[pOldName].Remove(pSource);
          if (mImplicitSources[pOldName].Count == 0) {
            mImplicitSources.Remove(pOldName);
          }

          if (!mImplicitSources.ContainsKey(pNewName)) {
            mImplicitSources.Add(pNewName, new List<IImplicitSource>());
          }
          mImplicitSources[pNewName].Add(pSource);
        }
      }
    }

    private void ImplicitSources_SourceRemoved(IAPMLComponent pComponent) {
      using (OpenWriteSession()) {
        IImplicitSource source = (IImplicitSource) pComponent;

        source.KeyChanged -= new KeyChangedEventHandler<IImplicitSource>(ImplicitSources_KeyChanged);
        source.Removed -= new APMLComponentRemovedHandler(ExplicitSources_SourceRemoved);

        if (mImplicitSources != null) {
          mImplicitSources[source.Key].Remove(source);
          if (mImplicitSources[source.Key].Count == 0) {
            mImplicitSources.Remove(source.Key);
          }
        }
      }
    }
    #endregion
  }
}
