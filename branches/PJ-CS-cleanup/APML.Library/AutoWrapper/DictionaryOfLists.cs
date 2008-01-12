using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace APML.AutoWrapper {
  public class DictionaryOfLists<TKey, TValue> : IDictionary<TKey, IList<TValue>>  {
    private IDictionary<TKey, IList<TValue>> mUnderlying = new Dictionary<TKey, IList<TValue>>();

    public DictionaryOfLists() {
    }

    public IReadOnlyDictionary<TKey, IList<TValue>> ReadOnlyDictionary {
      get {
        return new ReadOnlyDictionary<TKey, IList<TValue>>(this);
      }
    }

    #region IDictionary<TKey,TValue> Members

    public bool ContainsKey(TKey key) {
      return mUnderlying.ContainsKey(key);
    }

    public void Add(TKey key, IList<TValue> value) {
      mUnderlying.Add(key, value);
    }

    public void Add(TKey key, TValue value) {
      if (!mUnderlying.ContainsKey(key)) {
        mUnderlying.Add(key, new List<TValue>());
      }
      mUnderlying[key].Add(value);
    }

    public bool Remove(TKey key) {
      return mUnderlying.Remove(key);
    }

    public bool TryGetValue(TKey key, out IList<TValue> value) {
      return mUnderlying.TryGetValue(key, out value);
    }

    public IList<TValue> this[TKey key] {
      get { return mUnderlying[key]; }
      set { mUnderlying[key] = value; }
    }

    public ICollection<TKey> Keys {
      get { return new ReadOnlyCollection<TKey>(new List<TKey>(mUnderlying.Keys)); }
    }

    public ICollection<IList<TValue>> Values {
      get { return new ReadOnlyCollection<IList<TValue>>(new List<IList<TValue>>(mUnderlying.Values)); }
    }

    #endregion

    #region ICollection<KeyValuePair<TKey,TValue>> Members

    public void Add(KeyValuePair<TKey, IList<TValue>> item) {
      mUnderlying.Add(item);
    }

    public void Clear() {
      mUnderlying.Clear();
    }

    public bool Contains(KeyValuePair<TKey, IList<TValue>> item) {
      return mUnderlying.Contains(item);
    }

    public void CopyTo(KeyValuePair<TKey, IList<TValue>>[] array, int arrayIndex) {
      mUnderlying.CopyTo(array, arrayIndex);
    }

    public bool Remove(KeyValuePair<TKey, IList<TValue>> item) {
      return mUnderlying.Remove(item);
    }

    public int Count {
      get { return mUnderlying.Count; }
    }

    public bool IsReadOnly {
      get { return true; }
    }

    #endregion

    #region IEnumerable<KeyValuePair<TKey,TValue>> Members

    IEnumerator<KeyValuePair<TKey, IList<TValue>>> IEnumerable<KeyValuePair<TKey, IList<TValue>>>.GetEnumerator() {
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
