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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace APML {
  /// <summary>
  /// Interface for a dictionary that only support read operations.
  /// </summary>
  /// <typeparam name="TKey">key type</typeparam>
  /// <typeparam name="TValue">value type</typeparam>
  public interface IReadOnlyDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable {
    bool ContainsKey(TKey key);

    bool TryGetValue(TKey key, out TValue value);

    TValue this[TKey key] { get; }

    ICollection<TKey> Keys { get; }

    ICollection<TValue> Values { get; }

    bool Contains(KeyValuePair<TKey, TValue> item);

    void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex);

    bool Remove(KeyValuePair<TKey, TValue> item);

    int Count { get; }

    bool IsReadOnly { get; }

    /// <summary>
    /// Retrieve a copy of this dictionary as a IDictionary. The provided object will
    /// provide exceptions on non-supported calls.
    /// </summary>
    /// <returns>an IDictionary instance</returns>
    IDictionary<TKey, TValue> AsIDictionary();
  }

  /// <summary>
  /// Wrapper for making a dictionary read only.
  /// </summary>
  /// <typeparam name="TKey">key type</typeparam>
  /// <typeparam name="TValue">value type</typeparam>
  public class ReadOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>, IDictionary<TKey, TValue>, IEnumerable<KeyValuePair<TKey, TValue>>, IEnumerable {
    private IDictionary<TKey, TValue> mUnderlying;


    public ReadOnlyDictionary(IDictionary<TKey, TValue> pUnderlying) {
      mUnderlying = pUnderlying;
    }

    public IDictionary<TKey, TValue> AsIDictionary() {
      return this;
    }

    #region IDictionary<TKey,TValue> Members

    public bool ContainsKey(TKey key) {
      return mUnderlying.ContainsKey(key);
    }

    public void Add(TKey key, TValue value) {
      throw new NotImplementedException();
    }

    public bool Remove(TKey key) {
      throw new NotImplementedException();
    }

    public bool TryGetValue(TKey key, out TValue value) {
      return mUnderlying.TryGetValue(key, out value);
    }

    public TValue this[TKey key] {
      get { return mUnderlying[key]; }
      set { throw new NotImplementedException(); }
    }

    public ICollection<TKey> Keys {
      get { return new ReadOnlyCollection<TKey>(new List<TKey>(mUnderlying.Keys)); }
    }

    public ICollection<TValue> Values {
      get { return new ReadOnlyCollection<TValue>(new List<TValue>(mUnderlying.Values)); }
    }

    #endregion

    #region ICollection<KeyValuePair<TKey,TValue>> Members

    public void Add(KeyValuePair<TKey, TValue> item) {
      throw new NotImplementedException();
    }

    public void Clear() {
      throw new NotImplementedException();
    }

    public bool Contains(KeyValuePair<TKey, TValue> item) {
      return mUnderlying.Contains(item);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
      mUnderlying.CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<TKey, TValue> item) {
      throw new NotImplementedException();
    }

    public int Count {
      get { return mUnderlying.Count; }
    }

    public bool IsReadOnly {
      get { return true; }
    }

    #endregion

    #region IEnumerable<KeyValuePair<TKey,TValue>> Members

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() {
      return mUnderlying.GetEnumerator();
    }

    #endregion

    #region IEnumerable Members

    public IEnumerator GetEnumerator() {
      return mUnderlying.GetEnumerator();
    }

    #endregion
  }
}
