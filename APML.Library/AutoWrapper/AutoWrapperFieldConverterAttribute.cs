using System;
using System.Collections.Generic;
using System.Text;

namespace APML.AutoWrapper {
  /// <summary>
  /// Attribute indicating that there is a custom field converter for the given field.
  /// </summary>
  public class AutoWrapperFieldConverterAttribute : Attribute {
    private readonly Type mConverterType;

    /// <summary>
    /// Marks a field as using a custom converter.
    /// </summary>
    /// <param name="pConverterType">the type of the converter</param>
    public AutoWrapperFieldConverterAttribute(Type pConverterType) {
      mConverterType = pConverterType;
    }

    /// <summary>
    /// The type for the converter.
    /// </summary>
    public Type ConverterType {
      get { return mConverterType; }
    }
  }
}
