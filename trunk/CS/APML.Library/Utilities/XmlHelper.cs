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
using System.Text;
using System.Text.RegularExpressions;

namespace APML.Utilities {

  /// <summary>
  /// Utility class aimed at assisting with writing to and from XML
  /// replacing characters and text blocks that can break XML
  /// with XML-safe text, and converting back to human readable text
  /// </summary>
  public sealed class XmlHelper {
    /// <summary>
    /// Converts value to text that can safely be written to XML
    /// </summary>
    /// <param name="value">The string to be converted</param>
    /// <returns>The XML-safe text</returns>
    public static string ConvertToXmlString(string value) {
      if (value == null) {
        return null;
      }

      value = Regex.Replace(value, "(& )", "&amp; ", RegexOptions.None);
      value = Regex.Replace(value, "(\')", "&apos;", RegexOptions.None);
      value = Regex.Replace(value, "(\")", "&quot;", RegexOptions.None);
      value = Regex.Replace(value, "(<)", "&lt;", RegexOptions.None);
      value = Regex.Replace(value, "(>)", "&gt;", RegexOptions.None);

      return value;
    }

    /// <summary>
    /// Converts an XML-safe string back to human readable form
    /// </summary>
    /// <param name="value">The string to be converted</param>
    /// <returns>The human-readable text</returns>
    public static string ConvertToNormalString(string value) {
//      string tempValue = "";
//      do {
//        tempValue = value;
        value = Regex.Replace(value, "(&apos;)", "'", RegexOptions.IgnoreCase);
        //value = Regex.Replace(value, "(#39;)", "'", RegexOptions.IgnoreCase);
        value = Regex.Replace(value, "(&quot;)", "\"", RegexOptions.IgnoreCase);
        value = Regex.Replace(value, "(&lt;)", "<", RegexOptions.IgnoreCase);
        value = Regex.Replace(value, "(&gt;)", ">", RegexOptions.IgnoreCase);
        value = Regex.Replace(value, "(&amp;)", "&", RegexOptions.IgnoreCase);
//      } while (tempValue != value);
      return value;

    }

  }
}
