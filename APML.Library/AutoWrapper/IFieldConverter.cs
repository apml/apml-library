using System;
using System.Collections.Generic;
using System.Text;

namespace APML.AutoWrapper {
  /// <summary>
  /// Defines a converter used for mapping fields between strings and their native representations.
  /// </summary>
  /// <typeparam name="T">the type of the field</typeparam>
  public interface IFieldConverter<T> {
    /// <summary>
    /// Converts to the given type from a string.
    /// </summary>
    /// <param name="pValue">the string value</param>
    /// <returns>the converted value</returns>
    T FromString(string pValue);

    /// <summary>
    /// Converts the given type to a string.
    /// </summary>
    /// <param name="pValue">the value in its native format</param>
    /// <returns>the string version</returns>
    string ToString(T pValue);
  }
}
