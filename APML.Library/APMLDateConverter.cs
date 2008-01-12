using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using APML.AutoWrapper;

namespace APML {
  public class APMLDateConverter : IFieldConverter<DateTime>, IFieldConverter<DateTime?> {
    /// <summary>
    /// The format used for APML dates
    /// </summary>
    private const string APML_DATE_FORMAT = @"yyyy\-MM\-dd\THH:mm:ss\Z";

    #region IFieldConverter<DateTime> Members
    DateTime IFieldConverter<DateTime>.FromString(string pValue) {
      if (pValue == null) {
        return DateTime.Now;
      }

      return ((IFieldConverter<DateTime?>) this).FromString(pValue).Value;
    }

    string IFieldConverter<DateTime>.ToString(DateTime pValue) {
      return ((IFieldConverter<DateTime?>)this).ToString(pValue);
    }
    #endregion

    #region IFieldConverter<DateTime?> Members
    DateTime? IFieldConverter<DateTime?>.FromString(string pDateStr) {
      if (pDateStr == null || pDateStr == string.Empty) {
        return null;
      }

      // First-up, try parsing with the proper APML style
      DateTime result;
      if (DateTime.TryParseExact(pDateStr, APML_DATE_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out result)) {
        return result;
      }

      // Fall-back is to try using a generic parse
      return DateTime.Parse(pDateStr);
    }

    public string ToString(DateTime? pDate) {
      if (pDate != null) {
        return pDate.Value.ToUniversalTime().ToString(APML_DATE_FORMAT, DateTimeFormatInfo.CurrentInfo);
      }

      return null;
    }
    #endregion
  }
}
