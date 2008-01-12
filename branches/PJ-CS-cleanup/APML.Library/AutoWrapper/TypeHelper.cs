using System;
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
      return pType.IsGenericType && pType.GetGenericTypeDefinition() == typeof (Nullable<>);
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
  }
}
