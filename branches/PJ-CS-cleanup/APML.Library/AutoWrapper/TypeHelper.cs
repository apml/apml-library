using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace APML.AutoWrapper {
  /// <summary>
  /// Helper class for dealing with types.
  /// </summary>
  public static class TypeHelper {
    /// <summary>
    /// Checks whether the given type is Nullable. This needs to be done because classes
    /// wrapped in Nullable<> will still report being a struct, and it is sometimes necessary
    /// to know if it is already nullable before wrapping it.
    /// </summary>
    /// <param name="pType">the type to check</param>
    /// <returns>true - the type is nullable</returns>
    public static bool IsNullable(Type pType) {
      return pType.IsGenericType && pType.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    /// <summary>
    /// Checks whether the given type needs a Nullable wrapper to meaningfully be checked for null.
    /// </summary>
    /// <param name="pType">the type to check</param>
    /// <returns>true - a nullable wrapper would be required to meaningfully check for null</returns>
    public static bool RequiresNullableWrapperForNullCheck(Type pType) {
      return pType.IsValueType && !IsNullable(pType);
    }

    /// <summary>
    /// Retrieves all properties for a type, including from super-classes.
    /// </summary>
    /// <param name="pType"></param>
    /// <returns></returns>
    public static IEnumerable<PropertyInfo> GetAllProperties(Type pType) {
      // Do our direct properties
      foreach (PropertyInfo prop in pType.GetProperties()) {
        yield return prop;
      }

      // Do our inherited properties
      Type[] interfaces = pType.GetInterfaces();
      foreach (Type parent in interfaces) {
        foreach (PropertyInfo prop in GetAllProperties(parent)) {
          yield return prop;
        }
      }
    }

    /// <summary>
    /// Retrieves the element type for the given type.
    /// </summary>
    /// <param name="pType">the type to find the element type for</param>
    /// <returns>the element type</returns>
    public static Type GetElementType(Type pType) {
      if (!pType.IsInterface) {
        return pType;
      }
      if (!typeof(IEnumerable).IsAssignableFrom(pType)) {
        return pType;
      }
      if (pType.IsGenericType) {
        if (typeof(IList<>).IsAssignableFrom(pType.GetGenericTypeDefinition())) {
          return pType.GetGenericArguments()[0];
        }
        if (typeof(IReadOnlyDictionary<,>).IsAssignableFrom(pType.GetGenericTypeDefinition())) {
          if (!typeof(IEnumerable).IsAssignableFrom(pType.GetGenericArguments()[1])) {
            return pType.GetGenericArguments()[1];
          }
          if (pType.GetGenericArguments()[1].IsGenericType &&
              typeof(IList<>).IsAssignableFrom(pType.GetGenericArguments()[1].GetGenericTypeDefinition())) {
            return pType.GetGenericArguments()[1].GetGenericArguments()[0];
          }
        }
      }

      throw new ArgumentException("Cannot get Element type for " + pType.FullName);
    }
  }
}
