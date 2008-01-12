using APML.AutoWrapper;

namespace APML {
  /// <summary>
  /// Converter used for dealing with APML value fields - ensures that numbers are printed
  /// with two decimal places.
  /// </summary>
  public class APMLNumberConverter : IFieldConverter<double>, IFieldConverter<double?> {
    #region IFieldConverter<double> Members
    double IFieldConverter<double>.FromString(string pValue) {
      if (pValue == null) {
        return 0;
      }

      return ((IFieldConverter<double?>)this).FromString(pValue).Value;
    }

    string IFieldConverter<double>.ToString(double pValue) {
      return ((IFieldConverter<double?>)this).ToString(pValue);
    }
    #endregion

    #region IFieldConverter<double?> Members
    double? IFieldConverter<double?>.FromString(string pNumStr) {
      if (pNumStr == null || pNumStr == string.Empty) {
        return null;
      }

      // Use a generic parse
      return double.Parse(pNumStr);
    }

    public string ToString(double? pNum) {
      if (pNum != null) {
        return pNum.Value.ToString("f2");
      }

      return null;
    }
    #endregion
  }
}
