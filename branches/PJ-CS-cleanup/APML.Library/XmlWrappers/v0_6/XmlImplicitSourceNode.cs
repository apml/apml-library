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
using APML.XmlWrappers.v0_6;

namespace APML.XmlWrappers.v0_6 {
  public class XmlImplicitSourceNode : XmlAttentionNodeBase<IImplicitSource>, IImplicitSource {
    private XmlSourceNodeHelper<IImplicitSource> mSourceHelper;
    private XmlImplicitNodeHelper<IImplicitSource> mImplicitHelper;
    private Dictionary<string, IList<IImplicitAuthor>> mAuthors;

    private object mAuthorLock = new object();

    public XmlImplicitSourceNode(APMLFileBase pFile, XmlNode pNode) : base(pFile, pNode) {
      mSourceHelper = new XmlSourceNodeHelper<IImplicitSource>(pFile, pNode, this);
      mImplicitHelper = new XmlImplicitNodeHelper<IImplicitSource>(pFile, pNode, this);
    }

    #region ISource Members
    public string Type {
      get { return mSourceHelper.Type; }
      set { mSourceHelper.Type = value; }
    }

    public string Name {
      get { return mSourceHelper.Name; }
      set { mSourceHelper.Name = value; }
    }

    public IImplicitAuthor AddAuthor(string pKey, double pValue) {
      using (OpenWriteSession()) {
        EnsureAuthorCacheExists();

        XmlNode author = AddChildNode(
          null, "Author",
          new XAttribute("key", pKey), new XAttribute("value", pValue.ToString("f2", CultureInfo.InvariantCulture)));
        AddImplicitAttributes(author);

        XmlImplicitAuthorNode authorNode = new XmlImplicitAuthorNode(File, author);

        if (!mAuthors.ContainsKey(pKey)) {
          mAuthors.Add(pKey, new List<IImplicitAuthor>());
        }
        mAuthors[pKey].Add(authorNode);
        authorNode.KeyChanged += new KeyChangedEventHandler<IImplicitAuthor>(Authors_KeyChanged);
        authorNode.Removed += new APMLComponentRemovedHandler(Authors_AuthorRemoved);

        return authorNode;
      }
    }

    public IReadOnlyDictionary<string, IList<IImplicitAuthor>> Authors {
      get {
        EnsureAuthorCacheExists();

        return new ReadOnlyDictionary<string, IList<IImplicitAuthor>>(mAuthors);
      }
    }

    public event SourceTypeChangedEventHandler TypeChanged {
      add { mSourceHelper.TypeChanged += value; }
      remove { mSourceHelper.TypeChanged -= value; }
    }
    public event SourceNameChangedEventHandler NameChanged {
      add { mSourceHelper.NameChanged += value; }
      remove { mSourceHelper.NameChanged -= value; }
    }
    #endregion

    #region IImplicitAttention Members
    public string From {
      get { return mImplicitHelper.From; }
      set { mImplicitHelper.From = value; }
    }
    
    public DateTime? Updated {
      get { return mImplicitHelper.Updated; }
      set { mImplicitHelper.Updated = value; }
    }

    #endregion

    #region Cache Methods
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
          mAuthors = new Dictionary<string, IList<IImplicitAuthor>>();

          // Work through each device
          XmlNodeList authorsNode = Node.SelectNodes("Author");
          foreach (XmlNode authorNode in authorsNode) {
            XmlImplicitAuthorNode author = new XmlImplicitAuthorNode(File, authorNode);

            if (!mAuthors.ContainsKey(author.Key)) {
              mAuthors.Add(author.Key, new List<IImplicitAuthor>());
            }

            mAuthors[author.Key].Add(author);
            author.KeyChanged += new KeyChangedEventHandler<IImplicitAuthor>(Authors_KeyChanged);
            author.Removed += new APMLComponentRemovedHandler(Authors_AuthorRemoved);
          }
        }
      }
    }
    #endregion

    #region Event Handlers
    private void Authors_KeyChanged(IImplicitAuthor pAuthor, string pOldName, string pNewName) {
      using (OpenWriteSession()) {
        if (mAuthors != null) {
          mAuthors[pOldName].Remove(pAuthor);
          if (mAuthors[pOldName].Count == 0) {
            mAuthors.Remove(pOldName);
          }

          if (!mAuthors.ContainsKey(pNewName)) {
            mAuthors.Add(pNewName, new List<IImplicitAuthor>());
          }
          mAuthors[pNewName].Add(pAuthor);
        }
      }
    }

    private void Authors_AuthorRemoved(IAPMLComponent pComponent) {
      using (OpenWriteSession()) {
        IImplicitAuthor author = (IImplicitAuthor)pComponent;

        author.KeyChanged -= new KeyChangedEventHandler<IImplicitAuthor>(Authors_KeyChanged);
        author.Removed -= new APMLComponentRemovedHandler(Authors_AuthorRemoved);

        if (mAuthors != null) {
          mAuthors[author.Key].Remove(author);
          if (mAuthors[author.Key].Count == 0) {
            mAuthors.Remove(author.Key);
          }
        }
      }
    }
    #endregion
  }
}
