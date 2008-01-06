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
  public class XmlExplicitSourceNode : XmlAttentionNodeBase<IExplicitSource>, IExplicitSource {
    private XmlSourceNodeHelper<IExplicitSource> mHelper;
    private Dictionary<string, IExplicitAuthor> mAuthors;

    private object mAuthorLock = new object();

    public XmlExplicitSourceNode(APMLFileBase pFile, XmlNode pNode) : base(pFile, pNode) {
      mHelper = new XmlSourceNodeHelper<IExplicitSource>(pFile, pNode, this);
    }

    #region ISource Members
    public string Type {
      get { return mHelper.Type; }
      set { mHelper.Type = value; }
    }

    public string Name {
      get { return mHelper.Name; }
      set { mHelper.Name = value; }
    }

    public IExplicitAuthor AddAuthor(string pKey, double pValue) {
      using (OpenWriteSession()) {
        EnsureAuthorCacheExists();

        if (mAuthors.ContainsKey(pKey)) {
          throw new ArgumentException(pKey + " is already an explicit author");
        }

        XmlNode author = AddChildNode(
          null, "Author",
          new XAttribute("key", pKey), new XAttribute("value", pValue.ToString("f2", CultureInfo.InvariantCulture)));

        XmlExplicitAuthorNode authorNode = new XmlExplicitAuthorNode(File, author);
        mAuthors.Add(pKey, authorNode);
        authorNode.KeyChanged += new KeyChangedEventHandler<IExplicitAuthor>(Authors_KeyChanged);
        authorNode.Removed += new APMLComponentRemovedHandler(Authors_AuthorRemoved);

        return authorNode;
      }
    }

    public IReadOnlyDictionary<string, IExplicitAuthor> Authors {
      get {
        EnsureAuthorCacheExists();

        return new ReadOnlyDictionary<string, IExplicitAuthor>(mAuthors);
      }
    }

    public event SourceTypeChangedEventHandler TypeChanged {
      add { mHelper.TypeChanged += value; }
      remove { mHelper.TypeChanged -= value; }
    }
    public event SourceNameChangedEventHandler NameChanged {
      add { mHelper.NameChanged += value; }
      remove { mHelper.NameChanged -= value; }
    }
    #endregion

    #region Cache Management
    private void EnsureAuthorCacheExists() {
      using (OpenReadSession()) {
        // If the cache has already been created, we're done
        if (mAuthors != null) {
          return;
        }

        lock (mAuthorLock) {
          if (mAuthors != null) {
            // Prevent a race condition
            return;
          }

          // Allocate the cache
          mAuthors = new Dictionary<string, IExplicitAuthor>();

          // Work through each device
          XmlNodeList conceptNodes = Node.SelectNodes("Author");
          foreach (XmlNode conceptNode in conceptNodes) {
            XmlExplicitAuthorNode author = new XmlExplicitAuthorNode(File, conceptNode);

            if (!mAuthors.ContainsKey(author.Key)) {
              mAuthors.Add(author.Key, author);
              author.KeyChanged += new KeyChangedEventHandler<IExplicitAuthor>(Authors_KeyChanged);
              author.Removed += new APMLComponentRemovedHandler(Authors_AuthorRemoved);
            } else {
              Debug.WriteLine("Warning: Duplicate Explicit Concept: " + author.Key);
            }
          }
        }
      }
    }
    #endregion

    #region Event Handlers
    private void Authors_KeyChanged(IExplicitAuthor pAuthor, string pOldName, string pNewName) {
      using (OpenWriteSession()) {
        if (mAuthors != null) {
          mAuthors.Remove(pOldName);
          mAuthors.Add(pNewName, pAuthor);
        }
      }
    }

    private void Authors_AuthorRemoved(IAPMLComponent pComponent) {
      using (OpenWriteSession()) {
        IExplicitAuthor author = (IExplicitAuthor)pComponent;

        author.KeyChanged -= new KeyChangedEventHandler<IExplicitAuthor>(Authors_KeyChanged);
        author.Removed -= new APMLComponentRemovedHandler(Authors_AuthorRemoved);

        if (mAuthors != null) {
          mAuthors.Remove(author.Key);
        }
      }
    }
    #endregion
  }
}
