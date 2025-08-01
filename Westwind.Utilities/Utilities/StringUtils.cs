#region License
/*
 **************************************************************
 *  Author: Rick Strahl 
 *          (c) West Wind Technologies, 2008 - 2024
 *          http://www.west-wind.com/
 * 
 * Created: 09/08/2008
 *
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 **************************************************************  
*/
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using Westwind.Utilities.Properties;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;

namespace Westwind.Utilities
{
    /// <summary>
    /// String utility class that provides a host of string related operations
    /// </summary>
    public static class StringUtils
    {
        #region Basic String Tasks

        /// <summary>
        /// Trims the beginning of a string by a matching string.
        /// 
        /// Overrides string behavior which only works with char.
        /// </summary>
        /// <param name="text">Text to trim</param>
        /// <param name="textToTrim">Text to trim with</param>
        /// <param name="caseInsensitive">If true ignore case</param>
        /// <returns>Trimmed string if match is found</returns>        
        public static string TrimStart(this string text, string textToTrim, bool caseInsensitive = false)
        {
            if (string.IsNullOrEmpty(text) ||
                string.IsNullOrEmpty(textToTrim) ||
                text.Length < textToTrim.Length)
                return text;

            StringComparison comparison = caseInsensitive
                ? StringComparison.OrdinalIgnoreCase
                : StringComparison.Ordinal;

            // account for multiple instances of the text to trim
            while (text.Length >= textToTrim.Length &&
                   text.Substring(0, textToTrim.Length).Equals(textToTrim, comparison))
            {
                text = text.Substring(textToTrim.Length);
            }

            return text;
        }

        /// <summary>
        /// Trims the end of a string  with a matching string
        /// </summary>
        /// <param name="text">Text to trim</param>
        /// <param name="textToTrim">Text to trim with</param>
        /// <param name="caseInsensitive">If true ignore case</param>
        /// <returns>Trimmed string if match is found</returns>   
        public static string TrimEnd(this string text, string textToTrim, bool caseInsensitive = false)
        {
            if (string.IsNullOrEmpty(text) || 
                string.IsNullOrEmpty(textToTrim) ||
                text.Length < textToTrim.Length)
                return text;

            while (true)
            {
                var idx = text.LastIndexOf(textToTrim);
                if (idx == -1)
                    return text;

                string match = text.Substring(text.Length - textToTrim.Length, textToTrim.Length);

                if (match == textToTrim ||
                    (!caseInsensitive && match.Equals(textToTrim, StringComparison.OrdinalIgnoreCase)))
                {
                    if (text.Length <= match.Length)
                        text = "";
                    else
                        text = text.Substring(0, idx);
                }
                else
                    break;
            }
            return text;
        }


        /// <summary>
        /// Trims a string to a specific number of max characters
        /// </summary>
        /// <param name="value"></param>
        /// <param name="charCount"></param>
        /// <returns></returns>
        [Obsolete("Please use the StringUtils.Truncate() method instead.")]
        public static string TrimTo(string value, int charCount)
        {
            if (value == null)
                return value;

            if (value.Length > charCount)
                return value.Substring(0, charCount);

            return value;
        }

        /// <summary>
        /// Replicates an input string n number of times
        /// </summary>
        /// <param name="input"></param>
        /// <param name="charCount"></param>
        /// <returns></returns>
        public static string Replicate(string input, int charCount)
        {
            StringBuilder sb = new StringBuilder(input.Length * charCount);
            for (int i = 0; i < charCount; i++)
                sb.Append(input);

            return sb.ToString();
        }

        /// <summary>
        /// Replicates a character n number of times and returns a string
        /// You can use `new string(char, count)` directly though.
        /// </summary>
        /// <param name="charCount"></param>
        /// <param name="character"></param>
        /// <returns></returns>
        public static string Replicate(char character, int charCount)
        {
            return new string(character, charCount);
        }

        /// <summary>
        /// Finds the nth index of string in a string
        /// </summary>
        /// <param name="source"></param>
        /// <param name="matchString"></param>
        /// <param name="stringInstance"></param>
        /// <returns></returns>
        public static int IndexOfNth(this string source, string matchString, int stringInstance, StringComparison stringComparison = StringComparison.CurrentCulture)
        {
            if (string.IsNullOrEmpty(source))
                return -1;

            int lastPos = 0;
            int count = 0;

            while (count < stringInstance)
            {
                var len = source.Length - lastPos;
                lastPos = source.IndexOf(matchString, lastPos, len, stringComparison);
                if (lastPos == -1)
                    break;

                count++;
                if (count == stringInstance)
                    return lastPos;

                lastPos += matchString.Length;
            }
            return -1;
        }

        /// <summary>
        /// Returns the nth Index of a character in a string
        /// </summary>
        /// <param name="source"></param>
        /// <param name="matchChar"></param>
        /// <param name="charInstance"></param>
        /// <returns></returns>
        public static int IndexOfNth(this string source, char matchChar, int charInstance)
        {
            if (string.IsNullOrEmpty(source))
                return -1;

            if (charInstance < 1)
                return -1;

            int count = 0;
            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] == matchChar)
                {
                    count++;
                    if (count == charInstance)
                        return i;
                }
            }
            return -1;
        }



        /// <summary>
        /// Finds the nth index of strting in a string
        /// </summary>
        /// <param name="source"></param>
        /// <param name="matchString"></param>
        /// <param name="charInstance"></param>
        /// <returns></returns>
        public static int LastIndexOfNth(this string source, string matchString, int charInstance, StringComparison stringComparison = StringComparison.CurrentCulture)
        {
            if (string.IsNullOrEmpty(source))
                return -1;

            int lastPos = source.Length;
            int count = 0;

            while (count < charInstance)
            {
                lastPos = source.LastIndexOf(matchString, lastPos, lastPos, stringComparison);
                if (lastPos == -1)
                    break;

                count++;
                if (count == charInstance)
                    return lastPos;
            }
            return -1;
        }

        /// <summary>
        /// Finds the nth index of in a string from the end.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="matchChar"></param>
        /// <param name="charInstance"></param>
        /// <returns></returns>
        public static int LastIndexOfNth(this string source, char matchChar, int charInstance)
        {
            if (string.IsNullOrEmpty(source))
                return -1;

            int count = 0;
            for (int i = source.Length - 1; i > -1; i--)
            {
                if (source[i] == matchChar)
                {
                    count++;
                    if (count == charInstance)
                        return i;
                }
            }
            return -1;
        }
        #endregion

        #region String Casing


        /// <summary>
        /// Compares to strings for equality ignoring case.
        /// Uses OrdinalIgnoreCase
        /// </summary>
        /// <param name="text"></param>
        /// <param name="compareTo"></param>
        /// <returns></returns>
        public static bool EqualsNoCase(this string text, string compareTo)
        {
            if (text == null && compareTo == null)
                return true;
            if (text == null || compareTo == null)
                return false;

            return text.Equals(compareTo, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Return a string in proper Case format
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static string ProperCase(string Input)
        {
            if (Input == null)
                return null;
            return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(Input);
        }

        /// <summary>
        /// Takes a phrase and turns it into CamelCase text.
        /// White Space, punctuation and separators are stripped
        /// </summary>
        /// <param name="phrase">Text to convert to CamelCase</param>
        public static string ToCamelCase(string phrase)
        {
            if (phrase == null)
                return string.Empty;

            StringBuilder sb = new StringBuilder(phrase.Length);

            // First letter is always upper case
            bool nextUpper = true;

            foreach (char ch in phrase)
            {
                if (char.IsWhiteSpace(ch) || char.IsPunctuation(ch) || char.IsSeparator(ch) || ch > 32 && ch < 48)
                {
                    nextUpper = true;
                    continue;
                }
                if (char.IsDigit(ch))
                {
                    sb.Append(ch);
                    nextUpper = true;
                    continue;
                }

                if (nextUpper)
                    sb.Append(char.ToUpper(ch));
                else
                    sb.Append(char.ToLower(ch));

                nextUpper = false;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Tries to create a phrase string from CamelCase text
        /// into Proper Case text.  Will place spaces before capitalized
        /// letters.
        /// 
        /// Note that this method may not work for round tripping 
        /// ToCamelCase calls, since ToCamelCase strips more characters
        /// than just spaces.
        /// </summary>
        /// <param name="camelCase">Camel Case Text: firstName -> First Name</param>
        /// <returns></returns>
        public static string FromCamelCase(string camelCase)
        {
            if (string.IsNullOrEmpty(camelCase))
                return camelCase;

            StringBuilder sb = new StringBuilder(camelCase.Length + 10);
            bool first = true;
            char lastChar = '\0';

            foreach (char ch in camelCase)
            {
                if (!first &&
                    lastChar != ' ' && !char.IsSymbol(lastChar) && !char.IsPunctuation(lastChar) &&
                    ((char.IsUpper(ch) && !char.IsUpper(lastChar)) ||
                     char.IsDigit(ch) && !char.IsDigit(lastChar)))
                    sb.Append(' ');

                sb.Append(ch);
                first = false;
                lastChar = ch;
            }

            return sb.ToString(); ;
        }


        /// <summary>
        /// Attempts to convert a string that is encoded in camel or snake case or
        /// and convert it into a proper case string. This is useful for converting
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string BreakIntoWords(string text)
        {
            // if the text contains spaces it's already real text     
            if (string.IsNullOrEmpty(text) || text.Contains(" ") || text.Contains("\t"))
                return text;

            if (text.Contains("-"))
                text = text.Replace("-", " ").Trim();

            if (text.Contains("_"))
                text = text.Replace("_", " ").Trim();

            char c = text[0];

            // assume file name was valid as a 'title'     
            if (char.IsUpper(c))
            {
                // Split before uppercase letters, but not if preceded by another uppercase letter
                // and followed by a lowercase letter (to handle acronyms properly)
                string[] words = Regex.Split(text, @"(?<!^)(?<![A-Z])(?=[A-Z])|(?<=[A-Z])(?=[A-Z][a-z])");
                return string.Join(" ", words);
            }

            // lower case and no spaces - assume camel case     
            if (char.IsLower(c) & !text.Contains(" "))
            {
                return StringUtils.FromCamelCase(text);
            }

            // just return as proper case     
            return Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(text);
        }



        #endregion

        #region String Manipulation

        /// <summary>
        /// Extracts a string from between a pair of delimiters. Only the first 
        /// instance is found.
        /// </summary>
        /// <param name="source">Input String to work on</param>
        /// <param name="beginDelim">Beginning delimiter</param>
        /// <param name="endDelim">ending delimiter</param>
        /// <param name="caseSensitive">Determines whether the search for delimiters is case sensitive</param>        
        /// <param name="allowMissingEndDelimiter"></param>
        /// <param name="returnDelimiters"></param>
        /// <returns>Extracted string or string.Empty on no match</returns>
        public static string ExtractString(this string source,
            string beginDelim,
            string endDelim,
            bool caseSensitive = false,
            bool allowMissingEndDelimiter = false,
            bool returnDelimiters = false)
        {
            int at1, at2;

            if (string.IsNullOrEmpty(source))
                return string.Empty;

            if (caseSensitive)
            {
                at1 = source.IndexOf(beginDelim, StringComparison.CurrentCulture);
                if (at1 == -1)
                    return string.Empty;

                at2 = source.IndexOf(endDelim, at1 + beginDelim.Length, StringComparison.CurrentCulture);
            }
            else
            {
                //string Lower = source.ToLower();
                at1 = source.IndexOf(beginDelim, 0, source.Length, StringComparison.OrdinalIgnoreCase);
                if (at1 == -1)
                    return string.Empty;

                at2 = source.IndexOf(endDelim, at1 + beginDelim.Length, StringComparison.OrdinalIgnoreCase);
            }

            if (allowMissingEndDelimiter && at2 < 0)
            {
                if (!returnDelimiters)
                    return source.Substring(at1 + beginDelim.Length);

                return source.Substring(at1);
            }

            if (at1 > -1 && at2 > 1)
            {
                if (!returnDelimiters)
                    return source.Substring(at1 + beginDelim.Length, at2 - at1 - beginDelim.Length);

                return source.Substring(at1, at2 - at1 + endDelim.Length);
            }

            return string.Empty;
        }

        /// <summary>
        /// Strips characters of a string that follow the specified delimiter
        /// </summary>
        /// <param name="value">String to work with</param>
        /// <param name="delimiter">String to search for from end of string</param>
        /// <param name="caseSensitive">by default ignores case, set to true to care</param>
        /// <returns>stripped string, or original string if delimiter was not found</returns>
        public static string StripAfter(this string value, string delimiter, bool caseSensitive = false)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            var pos = caseSensitive ?
                         value.LastIndexOf(delimiter) : 
                         value.LastIndexOf(delimiter, StringComparison.OrdinalIgnoreCase);
            if (pos < 0)
                return value;

            return value.Substring(0, pos);
        }

        /// <summary>
        /// String replace function that supports replacing a specific instance with 
        /// case insensitivity
        /// </summary>
        /// <param name="origString">Original input string</param>
        /// <param name="findString">The string that is to be replaced</param>
        /// <param name="replaceWith">The replacement string</param>
        /// <param name="instance">Instance of the FindString that is to be found. 1 based. If Instance = -1 all are replaced</param>
        /// <param name="caseInsensitive">Case insensitivity flag</param>
        /// <returns>updated string or original string if no matches</returns>
        public static string ReplaceStringInstance(string origString, string findString,
            string replaceWith, int instance, bool caseInsensitive)
        {
            if (string.IsNullOrEmpty(origString) || string.IsNullOrEmpty(findString))
                return origString; // nothing to do

            if (instance == -1) // all instances
#if NET6_0_OR_GREATER
                // use native if possible - can only replace all instances
                return origString.Replace(findString, replaceWith, StringComparison.OrdinalIgnoreCase);
#else
                return ReplaceString(origString, findString, replaceWith, caseInsensitive);
#endif
            int at1 = 0;
            for (int x = 0; x < instance; x++)
            {
                if (caseInsensitive)
                    at1 = origString.IndexOf(findString, at1, origString.Length - at1, StringComparison.OrdinalIgnoreCase);
                else
                    at1 = origString.IndexOf(findString, at1);

                if (at1 == -1)
                    return origString;

                if (x < instance - 1)
                    at1 += findString.Length;
            }

            return origString.Substring(0, at1) + replaceWith + origString.Substring(at1 + findString.Length);
        }


        /// <summary>
        /// Replaces a substring within a string with another substring with optional case sensitivity turned off.
        /// </summary>
        /// <param name="origString">String to do replacements on</param>
        /// <param name="findString">The string to find</param>
        /// <param name="replaceString">The string to replace found string wiht</param>
        /// <param name="caseInsensitive">If true case insensitive search is performed</param>
        /// <returns>updated string or original string if no matches</returns>
#if NET6_0_OR_GREATER
        [Obsolete("You can use native `string.Replace()` with StringComparison in .NET Core")]
#endif
        public static string ReplaceString(string origString, string findString, string replaceString, bool caseInsensitive)
        {
            if (string.IsNullOrEmpty(origString) || string.IsNullOrEmpty(findString))
                return origString; // nothing to do

            int at1 = 0;
            while (true)
            {
                if (caseInsensitive)
                    at1 = origString.IndexOf(findString, at1, origString.Length - at1, StringComparison.OrdinalIgnoreCase);
                else
                    at1 = origString.IndexOf(findString, at1);

                if (at1 == -1)
                    break;

                origString = origString.Substring(0, at1) + replaceString + origString.Substring(at1 + findString.Length);

                at1 += replaceString.Length;
            }

            return origString;
        }

        /// <summary>
        /// Replaces the last nth occurrence of a string within a string with another string
        /// </summary>
        /// <param name="source">Souce string</param>
        /// <param name="oldValue">Value to replace</param>
        /// <param name="newValue">Value to replace with</param>
        /// <param name="instanceFromEnd">The instance from the end to replace</param>
        /// <param name="compare">String comparison mode</param>
        /// <returns>replaced string or original string if replacement is not found</returns>
        public static string ReplaceLastNthInstance(string source, string oldValue, string newValue, int instanceFromEnd = 1, StringComparison compare = StringComparison.CurrentCulture)
        {
            if (instanceFromEnd <= 0 || source == null || oldValue == null)  return source; // Invalid n value

            int lastIndex = source.Length;
            
            // Traverse the string backwards
            while (instanceFromEnd > 0)
            {
                lastIndex = source.LastIndexOf(oldValue, lastIndex - 1, compare);
                if (lastIndex == -1) return source; // If not found, return the original string
                instanceFromEnd--;
            }

            // Replace the found occurrence
            return source.Substring(0, lastIndex) + newValue + source.Substring(lastIndex + oldValue.Length);
        }

        /// <summary>
        /// Truncate a string to maximum length.
        /// </summary>
        /// <param name="text">Text to truncate</param>
        /// <param name="maxLength">Maximum length</param>
        /// <returns>Trimmed string</returns>
        public static string Truncate(this string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
                return text;

            return text.Substring(0, maxLength);
        }

        /// <summary>
        /// Returns an abstract of the provided text by returning up to Length characters
        /// of a text string. If the text is truncated a ... is appended.
        ///
        /// Note: Linebreaks are converted into spaces.
        /// </summary>
        /// <param name="text">Text to abstract</param>
        /// <param name="length">Number of characters to abstract to</param>
        /// <returns>string</returns>
        public static string TextAbstract(string text, int length)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            if (text.Length > length)
            {
                text = text.Substring(0, length);
                var idx = text.LastIndexOf(' ');
                if (idx > -1)
                    text = text.Substring(0, idx) + "…";              
            }

            if (!text.Contains("\n"))
                return text;

            // linebreaks to spaces
            StringBuilder sb = new StringBuilder(text.Length);
            foreach (var s in GetLines(text))
                sb.Append(s.Trim() + " ");
            return sb.ToString().Trim();
        }

        /// <summary>
        /// Terminates a string with the given end string/character, but only if the
        /// text specified doesn't already exist and the string is not empty.
        /// </summary>
        /// <param name="value">String to terminate</param>
        /// <param name="terminatorString">String to terminate the text string with</param>
        /// <returns></returns>
        public static string TerminateString(string value, string terminatorString)
        {
            if (string.IsNullOrEmpty(value))
                return terminatorString;

            if (value.EndsWith(terminatorString))
                return value;

            return value + terminatorString;
        }



        /// <summary>
        /// Returns the number or right characters specified
        /// </summary>
        /// <param name="full">full string to work with</param>
        /// <param name="rightCharCount">number of right characters to return</param>
        /// <returns></returns>
        public static string Right(string full, int rightCharCount)
        {
            if (string.IsNullOrEmpty(full) || full.Length < rightCharCount || full.Length - rightCharCount < 0)
                return full;

            return full.Substring(full.Length - rightCharCount);
        }
        #endregion

        #region String 'Any' and 'Many' Operations

        /// <summary>
        /// Checks many a string for multiple string values to start with
        /// </summary>
        /// <param name="str"></param>
        /// <param name="matchValues"></param>
        /// <returns></returns>
        public static bool StartsWithAny(this string str, params string[] matchValues)
        {
            foreach (var value in matchValues)
            {
                if (str.StartsWith(value))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks many a string for multiple string values to start with
        /// </summary>
        /// <param name="str"></param>
        /// <param name="matchValues"></param>
        /// <returns></returns>
        public static bool StartsWithAny(this string str, StringComparison compare, params string[] matchValues)
        {
            foreach (var value in matchValues)
            {
                if (str.StartsWith(value, compare))
                    return true;
            }

            return false;
        }


        /// <summary>
        /// Checks a string form multiple contained string values
        /// </summary>
        /// <param name="str">String to match</param>
        /// <param name="matchValues">Matches to find in the string</param>
        /// <returns></returns>
        public static bool ContainsAny(this string str, params string[] matchValues)
        {
            foreach (var value in matchValues)
            {
                if (str.Contains(value))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks a string form multiple contained string values
        /// </summary>
        /// <param name="str">String to match</param>
        /// <param name="compare">Type of comparison</param>
        /// <param name="matchValues">Matches to find in the string</param>
        /// <returns></returns>
        public static bool ContainsAny(this string str, StringComparison compare, params string[] matchValues)
        {
            foreach (var value in matchValues)
            {
                if (str.Contains(value, compare))
                    return true;
            }

            return false;
        }


        /// <summary>
        /// Checks a string form multiple contained string values
        /// </summary>
        /// <param name="str">String to match</param>
        /// <param name="matchValues">Matches to find in the string</param>
        /// <returns></returns>
        public static bool ContainsAny(this string str, params char[] matchValues)
        {
            foreach (var value in matchValues)
            {
                if (str.Contains(value))
                    return true;
            }

            return false;
        }

#if NET6_0_OR_GREATER
        /// <summary>
        /// Checks a string form multiple contained string values
        /// </summary>
        /// <param name="str">String to match</param>
        /// <param name="compare">Type of comparison</param>
        /// <param name="matchValues">Matches to find in the string</param>
        /// <returns></returns>
        public static bool ContainsAny(this string str, StringComparison compare, params char[] matchValues)
        {
            foreach (var value in matchValues)
            {
                if (str.Contains(value, compare))
                    return true;
            }

            return false;
        }
#endif

        /// <summary>
        /// Checks to see if a string contains any of a set of values
        /// </summary>
        /// <param name="str">String to check</param>
        /// <param name="compare">Comparison mode</param>
        /// <param name="matchValues">Strings to check for</param>
        /// <returns></returns>
        public static bool EqualsAny(this string str, StringComparison compare, params string[] matchValues)
        {
            foreach (var value in matchValues)
            {
                if (str.Equals(value, compare))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks to see if a string contains any of a set of values.
        /// </summary>
        /// <param name="str">String to compare</param>        
        /// <param name="matchValues">String values to compare to</param>
        /// <returns></returns>
        public static bool EqualsAny(this string str, params string[] matchValues)
        {
            foreach (var value in matchValues)
            {
                if (str.Equals(value))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Replaces multiple matches with a single new value
        ///         
        /// This version takes an array of strings as input
        /// </summary>
        /// <param name="str">String to work on</param>
        /// <param name="matchValues">String values to match</param>
        /// <param name="replaceWith">String to replace with</param>
        /// <returns></returns>
        public static string ReplaceMany(this string str, string[] matchValues, string replaceWith)
        {
            foreach (var value in matchValues)
            {
                str = str.Replace(value, replaceWith);
            }

            return str;
        }

        /// <summary>
        /// Replaces multiple matches with a single new value. 
        /// 
        /// This version takes a comma delimited list of strings
        /// </summary>
        /// <param name="str">String to work on</param>
        /// <param name="valuesToMatch">Comma delimited list of values. Values are start and end trimmed</param>
        /// <param name="replaceWith">String to replace with</param>
        /// <returns></returns>
        public static string ReplaceMany(this string str, string valuesToMatch, string replaceWith)
        {
            if (string.IsNullOrEmpty(valuesToMatch))
                return str;

            var matchValues = valuesToMatch.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(v => v.Trim())
                .ToArray();

            return ReplaceMany(str, matchValues, replaceWith);
        }
        #endregion


        #region String Parsing
        /// <summary>
        /// Determines if a string is contained in a list of other strings
        /// </summary>
        /// <param name="s"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool Inlist(string s, params string[] list)
        {
            return list.Contains(s);
        }


        /// <summary>
        /// Checks to see if value is part of a delimited list of values.
        /// Example: IsStringInList("value1,value2,value3","value3");
        /// </summary>
        /// <param name="stringList">A list of delimited strings (ie. value1, value2, value3) with or without spaces (values are trimmed)</param>
        /// <param name="valueToFind">value to match against the list</param>
        /// <param name="separator">Character that separates the list values</param>
        /// <param name="ignoreCase">If true ignores case for the list value matches</param>
        public static bool IsStringInList(string stringList, string valueToFind, char separator = ',', bool ignoreCase = false)
        {
            var tokens = stringList.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0)
                return false;

            var comparer = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
            foreach (var tok in tokens)
            {
                if (tok.Trim().Equals(valueToFind, comparer))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// String.Contains() extension method that allows to specify case
        /// </summary>
        /// <param name="text">Input text</param>
        /// <param name="searchFor">text to search for</param>
        /// <param name="stringComparison">Case sensitivity options</param>
        /// <returns></returns>
        public static bool Contains(this string text, string searchFor, StringComparison stringComparison)
        {
            return text.IndexOf(searchFor, stringComparison) > -1;
        }


        /// <summary>
        /// Parses a string into an array of lines broken
        /// by \r\n or \n
        /// </summary>
        /// <param name="s">String to check for lines</param>
        /// <param name="maxLines">Optional - max number of lines to return</param>
        /// <returns>array of strings, or null if the string passed was a null</returns>
        public static string[] GetLines(this string s, int maxLines = 0)
        {
            if (s == null)
                return new string[] { };

            s = s.Replace("\r\n", "\n").Replace("\r", "\n");

            if (maxLines < 1)
                return s.Split(new char[] { '\n' });

            return s.Split(new char[] { '\n' }).Take(maxLines).ToArray();
        }

        /// <summary>
        /// Returns a line count for a string
        /// </summary>
        /// <param name="s">string to count lines for</param>
        /// <returns></returns>
        public static int CountLines(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return 0;

            return s.Split('\n').Length;
        }


        /// <summary>
        /// Counts the number of times a character occurs
        /// in a given string
        /// </summary>
        /// <param name="source">input string</param>
        /// <param name="match">character to match</param>
        /// <returns></returns>
        public static int Occurs(string source, char match)
        {
            if (string.IsNullOrEmpty(source)) return 0;

            int count = 0;
            foreach (char c in source)
                if (c == match)
                    count++;

            return count;
        }

        /// <summary>
        /// Counts the number of times a sub string occurs
        /// in a given string
        /// </summary>
        /// <param name="source">input string</param>
        /// <param name="match">string to match</param>
        /// <returns></returns>
        public static int Occurs(string source, string match)
        {
            if (string.IsNullOrEmpty(source)) return 0;
            return source.Split(new[] { match }, StringSplitOptions.None).Length - 1;
        }

        /// <summary>
        /// Returns a string that has the max amount of characters of the source string.
        /// If the string is shorter than the max length the entire string is returned.
        /// If the string is longer it's truncated.
        /// If empty the original value is returned (null or string.Empty)
        /// If the startPosition is greater than the length of the string null is returned
        /// </summary>
        /// <param name="s">source string to work on</param>
        /// <param name="maxCharacters">Maximum number of characters to return</param>
        /// <param name="startPosition">Optional start position. If not specified uses entire string (0)</param>
        /// <returns></returns>
        public static string GetMaxCharacters(this string s, int maxCharacters, int startPosition = 0)
        {
            if (string.IsNullOrEmpty(s) || startPosition == 0 && maxCharacters > s.Length)
                return s;

            if (startPosition > s.Length - 1)
                return null;

            var available = s.Length - startPosition;

            return s.Substring(startPosition, Math.Min(available, maxCharacters));
        }

        /// <summary>
        /// Retrieves the last n characters from the end of a string up to the
        /// number of characters specified. If there are fewer characters
        /// the original string is returned otherwise the last n characters
        /// are returned.
        /// </summary>
        /// <param name="s">input string</param>
        /// <param name="characterCount">number of characters to retrieve from end of string</param>
        /// <returns>Up to the last n characters of the string. Empty string on empty or null</returns>
        public static string GetLastCharacters(this string s, int characterCount)
        {
            if (string.IsNullOrEmpty(s) || s.Length < characterCount)
                return s ?? string.Empty;

            return s.Substring(s.Length - characterCount);
        }

        /// <summary>
        /// Strips all non digit values from a string and only
        /// returns the numeric string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string StripNonNumber(string input)
        {
            var chars = input.ToCharArray();
            StringBuilder sb = new StringBuilder();
            foreach (var chr in chars)
            {
                if (char.IsNumber(chr) || char.IsSeparator(chr))
                    sb.Append(chr);
            }

            return sb.ToString();
        }

        static Regex tokenizeRegex = new Regex("{{.*?}}");

        /// <summary>
        /// Tokenizes a string based on a start and end string. Replaces the values with a token
        /// text (#@#1#@# for example).
        /// 
        /// You can use Detokenize to get the original values back using DetokenizeString
        /// using the same token replacement text.
        /// </summary>
        /// <param name="text">Text to search</param>
        /// <param name="startMatch">starting match string</param>
        /// <param name="endMatch">ending match string</param>
        /// <param name="replaceDelimiter">token replacement text - make sure this string is a value that is unique and **doesn't occur in the document**</param>
        /// <returns>A list of extracted string tokens that have been replaced in `ref text` with the replace delimiter</returns>
        public static List<string> TokenizeString(ref string text, string startMatch, string endMatch, string replaceDelimiter = "#@#")
        {
            var strings = new List<string>();
            var matches = tokenizeRegex.Matches(text);

            int i = 0;
            foreach (Match match in matches)
            {
                tokenizeRegex = new Regex(Regex.Escape(match.Value));
                text = tokenizeRegex.Replace(text, $"{replaceDelimiter}{i}{replaceDelimiter}", 1);
                strings.Add(match.Value);
                i++;
            }

            return strings;
        }


        /// <summary>
        /// Detokenizes a string tokenized with TokenizeString. Requires the collection created
        /// by detokenization
        /// </summary>
        /// <param name="text">Text to work with</param>
        /// <param name="tokens">list of previously extracted tokens</param>
        /// <param name="replaceDelimiter">the token replacement string that replaced the captured tokens</param>
        /// <returns></returns>
        public static string DetokenizeString(string text, IEnumerable<string> tokens, string replaceDelimiter = "#@#")
        {
            int i = 0;
            foreach (string token in tokens)
            {
                text = text.Replace($"{replaceDelimiter}{i}{replaceDelimiter}", token);
                i++;
            }
            return text;
        }

        /// <summary>
        /// Parses an string into an integer. If the text can't be parsed
        /// a default text is returned instead
        /// </summary>
        /// <param name="input">Input numeric string to be parsed</param>
        /// <param name="defaultValue">Optional default text if parsing fails</param>
        /// <param name="formatProvider">Optional NumberFormat provider. Defaults to current culture's number format</param>
        /// <returns></returns>
        public static int ParseInt(string input, int defaultValue = 0, IFormatProvider numberFormat = null)
        {

            if (numberFormat == null)
                numberFormat = CultureInfo.CurrentCulture.NumberFormat;

            int val = defaultValue;

            if (input == null)
                return defaultValue;

            if (!int.TryParse(input, NumberStyles.Any, numberFormat, out val))
                return defaultValue;
            return val;
        }



        /// <summary>
        /// Parses an string into an decimal. If the text can't be parsed
        /// a default text is returned instead
        /// </summary>
        /// <param name="input"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static decimal ParseDecimal(string input, decimal defaultValue = 0M, IFormatProvider numberFormat = null)
        {
            numberFormat = numberFormat ?? CultureInfo.CurrentCulture.NumberFormat;
            decimal val = defaultValue;

            if (input == null)
                return defaultValue;

            if (!decimal.TryParse(input, NumberStyles.Any, numberFormat, out val))
                return defaultValue;
            return val;
        }

        #endregion

        #region String Ids
        /// <summary>
        /// Creates short string id based on a GUID hashcode.
        /// Not guaranteed to be unique across machines, but unlikely
        /// to duplicate in medium volume situations.
        /// </summary>
        /// <returns></returns>
        public static string NewStringId()
        {
            return Guid.NewGuid().ToString().GetHashCode().ToString("x");
        }

        /// <summary>
        /// Creates a new random string of upper, lower case letters and digits.
        /// Very useful for generating random data for storage in test data.
        /// </summary>
        /// <param name="size">The number of characters of the string to generate</param>
        /// <returns>randomized string</returns>
        public static string RandomString(int size, bool includeNumbers = false)
        {
            StringBuilder builder = new StringBuilder(size);
            char ch;
            int num;

            for (int i = 0; i < size; i++)
            {
                if (includeNumbers)
                    num = Convert.ToInt32(Math.Floor(62 * random.NextDouble()));
                else
                    num = Convert.ToInt32(Math.Floor(52 * random.NextDouble()));

                if (num < 26)
                    ch = Convert.ToChar(num + 65);
                // lower case
                else if (num > 25 && num < 52)
                    ch = Convert.ToChar(num - 26 + 97);
                // numbers
                else
                    ch = Convert.ToChar(num - 52 + 48);

                builder.Append(ch);
            }

            return builder.ToString();
        }
        private static Random random = new Random((int)DateTime.Now.Ticks);

        #endregion

        #region Encodings

        /// <summary>
        /// UrlEncodes a string without the requirement for System.Web
        /// </summary>
        /// <param name="String"></param>
        /// <returns></returns>
        // [Obsolete("Use System.Uri.EscapeDataString instead")]
        public static string UrlEncode(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            return Uri.EscapeDataString(text);
        }

        /// <summary>
        /// Encodes a few additional characters for use in paths
        /// Encodes: . #
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string UrlEncodePathSafe(string text)
        {
            string escaped = UrlEncode(text);
            return escaped.Replace(".", "%2E").Replace("#", "%23");
        }

        /// <summary>
        /// UrlDecodes a string without requiring System.Web
        /// </summary>
        /// <param name="text">String to decode.</param>
        /// <returns>decoded string</returns>
        public static string UrlDecode(string text)
        {
            // pre-process for + sign space formatting since System.Uri doesn't handle it
            // plus literals are encoded as %2b normally so this should be safe
            text = text.Replace("+", " ");
            string decoded = Uri.UnescapeDataString(text);
            return decoded;
        }

        /// <summary>
        /// Retrieves a text by key from a UrlEncoded string.
        /// </summary>
        /// <param name="urlEncoded">UrlEncoded String</param>
        /// <param name="key">Key to retrieve text for</param>
        /// <returns>returns the text or "" if the key is not found or the text is blank</returns>
        public static string GetUrlEncodedKey(string urlEncoded, string key)
        {
            urlEncoded = "&" + urlEncoded + "&";

            int Index = urlEncoded.IndexOf("&" + key + "=", StringComparison.OrdinalIgnoreCase);
            if (Index < 0)
                return string.Empty;

            int lnStart = Index + 2 + key.Length;

            int Index2 = urlEncoded.IndexOf("&", lnStart);
            if (Index2 < 0)
                return string.Empty;

            return UrlDecode(urlEncoded.Substring(lnStart, Index2 - lnStart));
        }

        /// <summary>
        /// Allows setting of a text in a UrlEncoded string. If the key doesn't exist
        /// a new one is set, if it exists it's replaced with the new text.
        /// </summary>
        /// <param name="urlEncoded">A UrlEncoded string of key text pairs</param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SetUrlEncodedKey(string urlEncoded, string key, string value)
        {
            if (!urlEncoded.EndsWith("?") && !urlEncoded.EndsWith("&"))
                urlEncoded += "&";

            Match match = Regex.Match(urlEncoded, "[?|&]" + key + "=.*?&");

            if (match == null || string.IsNullOrEmpty(match.Value))
                urlEncoded = urlEncoded + key + "=" + UrlEncode(value) + "&";
            else
                urlEncoded = urlEncoded.Replace(match.Value, match.Value.Substring(0, 1) + key + "=" + UrlEncode(value) + "&");

            return urlEncoded.TrimEnd('&');
        }
        #endregion

        #region Binary Encoding

        /// <summary>
        /// Turns a BinHex string that contains raw byte values
        /// into a byte array
        /// </summary>
        /// <param name="hex">BinHex string (just two byte hex digits strung together)</param>
        /// <returns></returns>
        public static byte[] BinHexToBinary(string hex)
        {
            int offset = hex.StartsWith("0x") ? 2 : 0;
            if ((hex.Length % 2) != 0)
                throw new ArgumentException(string.Format(Resources.InvalidHexStringLength, hex.Length));

            byte[] ret = new byte[(hex.Length - offset) / 2];

            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = (byte)((ParseHexChar(hex[offset]) << 4)
                                | ParseHexChar(hex[offset + 1]));
                offset += 2;
            }
            return ret;
        }

        /// <summary>
        /// Converts a byte array into a BinHex string.
        /// BinHex is two digit hex byte values squished together
        /// into a string.
        /// </summary>
        /// <param name="data">Raw data to send</param>
        /// <returns>BinHex string or null if input is null</returns>
        public static string BinaryToBinHex(byte[] data)
        {
            if (data == null)
                return null;

            char[] c = new char[data.Length * 2];
            int b;
            for (int i = 0; i < data.Length; i++)
            {
                b = data[i] >> 4;
                c[i * 2] = (char)(55 + b + (((b - 10) >> 31) & -7));
                b = data[i] & 0xF;
                c[i * 2 + 1] = (char)(55 + b + (((b - 10) >> 31) & -7));
            }
            return new string(c).ToLower();
        }

        /// <summary>
        /// Converts a string into bytes for storage in any byte[] types
        /// buffer or stream format (like MemoryStream).
        /// </summary>
        /// <param name="text"></param>
        /// <param name="encoding">The character encoding to use. Defaults to Unicode</param>
        /// <returns></returns>
        public static byte[] StringToBytes(string text, Encoding encoding = null)
        {
            if (text == null)
                return null;

            if (encoding == null)
                encoding = Encoding.Unicode;

            return encoding.GetBytes(text);
        }

        /// <summary>
        /// Converts a byte array to a stringUtils
        /// </summary>
        /// <param name="buffer">raw string byte data</param>
        /// <param name="encoding">Character encoding to use. Defaults to Unicode</param>
        /// <returns></returns>
        public static string BytesToString(byte[] buffer, Encoding encoding = null)
        {
            if (buffer == null)
                return null;

            if (encoding == null)
                encoding = Encoding.Unicode;

            return encoding.GetString(buffer);
        }

        /// <summary>
        /// Converts a string to a Base64 string
        /// </summary>
        /// <param name="text">A string to convert to base64</param>
        /// <param name="encoding">Optional encoding - if not passed assumed to be Unicode</param>
        /// <returns>Base 64 or null</returns>
        public static string ToBase64String(string text, Encoding encoding = null)
        {
            var bytes = StringToBytes(text, encoding);
            if (bytes == null)
                return null;

            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Converts a base64 string back to a string
        /// </summary>
        /// <param name="base64">A base 64 string</param>
        /// <param name="encoding">Optional encoding - if not passed assumed to be Unicode</param>
        /// <returns></returns>
        public static string FromBase64String(string base64, Encoding encoding = null)
        {
            var bytes = Convert.FromBase64String(base64);
            if (bytes == null)
                return null;

            return BytesToString(bytes, encoding);
        }

        static int ParseHexChar(char c)
        {
            if (c >= '0' && c <= '9')
                return c - '0';
            if (c >= 'A' && c <= 'F')
                return c - 'A' + 10;
            if (c >= 'a' && c <= 'f')
                return c - 'a' + 10;

            throw new ArgumentException(Resources.InvalidHexDigit + c);
        }

        static char[] base36CharArray = "0123456789abcdefghijklmnopqrstuvwxyz".ToCharArray();
        static string base36Chars = "0123456789abcdefghijklmnopqrstuvwxyz";

        /// <summary>
        /// Encodes an integer into a string by mapping to alpha and digits (36 chars)
        /// chars are embedded as lower case
        /// 
        /// Example: 4zx12ss
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Base36Encode(long value)
        {
            string returnValue = "";
            bool isNegative = value < 0;
            if (isNegative)
                value = value * -1;

            do
            {
                returnValue = base36CharArray[value % base36CharArray.Length] + returnValue;
                value /= 36;
            } while (value != 0);

            return isNegative ? returnValue + "-" : returnValue;
        }

        /// <summary>
        /// Decodes a base36 encoded string to an integer
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static long Base36Decode(string input)
        {
            bool isNegative = false;
            if (input.EndsWith("-"))
            {
                isNegative = true;
                input = input.Substring(0, input.Length - 1);
            }

            char[] arrInput = input.ToCharArray();
            Array.Reverse(arrInput);
            long returnValue = 0;
            for (long i = 0; i < arrInput.Length; i++)
            {
                long valueindex = base36Chars.IndexOf(arrInput[i]);
                returnValue += Convert.ToInt64(valueindex * Math.Pow(36, i));
            }
            return isNegative ? returnValue * -1 : returnValue;
        }
        #endregion

        #region Miscellaneous

        /// <summary>
        /// Normalizes linefeeds to the appropriate 
        /// </summary>
        /// <param name="text">The text to fix up</param>
        /// <param name="type">Type of linefeed to fix up to</param>
        /// <returns></returns>
        public static string NormalizeLineFeeds(string text, LineFeedTypes type = LineFeedTypes.Auto)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            if (type == LineFeedTypes.Auto)
            {
                if (Environment.NewLine.Contains('\r'))
                    type = LineFeedTypes.CrLf;
                else
                    type = LineFeedTypes.Lf;
            }

            if (type == LineFeedTypes.Lf)
                return text.Replace("\r\n", "\n");

            return text.Replace("\r\n", "*@\r@*").Replace("\n", "\r\n").Replace("*@\r@*", "\r\n");
        }

        /// <summary>
        /// Strips any common white space from all lines of text that have the same
        /// common white space text. Effectively removes common code indentation from
        /// code blocks for example so you can get a left aligned code snippet.
        /// </summary>
        /// <param name="code">Text to normalize</param>
        /// <returns></returns>
        public static string NormalizeIndentation(string code)
        {
            if (string.IsNullOrEmpty(code))
                return string.Empty;

            // normalize tabs to 3 spaces
            string text = code.Replace("\t", "   ");

            string[] lines = text.Split(new string[3] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            // keep track of the smallest indent
            int minPadding = 1000;

            foreach (var line in lines)
            {
                if (line.Length == 0)  // ignore blank lines
                    continue;

                int count = 0;
                foreach (char chr in line)
                {
                    if (chr == ' ' && count < minPadding)
                        count++;
                    else
                        break;
                }
                if (count == 0)
                    return code;

                minPadding = count;
            }

            string strip = new String(' ', minPadding);

            StringBuilder sb = new StringBuilder();
            foreach (var line in lines)
            {
                sb.AppendLine(StringUtils.ReplaceStringInstance(line, strip, "", 1, false));
            }

            return sb.ToString();
        }



        /// <summary>
        /// Simple Logging method that allows quickly writing a string to a file
        /// </summary>
        /// <param name="output"></param>
        /// <param name="filename"></param>
        /// <param name="encoding">if not specified used UTF-8</param>
        public static void LogString(string output, string filename, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            lock (_logLock)
            {
                var writer = new StreamWriter(filename, true, encoding);
                writer.WriteLine(DateTime.Now + " - " + output);
                writer.Close();
            }
        }
        private static object _logLock = new object();

        /// <summary>
        /// Creates a Stream from a string. Internally creates
        /// a memory stream and returns that.
        ///
        /// Note: stream returned should be disposed!
        /// </summary>
        /// <param name="text"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static Stream StringToStream(string text, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.Default;

            var ms = new MemoryStream(text.Length * 2);
            byte[] data = encoding.GetBytes(text);
            ms.Write(data, 0, data.Length);
            ms.Position = 0;
            return ms;
        }

        /// <summary>
        /// Creates a string from a text based stream
        /// </summary>
        /// <param name="stream">input stream (not closed by operation)</param>
        /// <param name="encoding">Optional encoding - if not specified assumes 'Encoding.Default'</param>
        /// <returns></returns>
        /// <exception cref="InvalidOleVariantTypeException"></exception>
        public static string StreamToString(Stream stream, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.Default;

            if (!stream.CanRead)
                throw new InvalidOleVariantTypeException("Stream cannot be read.");

            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Retrieves a string value from an XML-like string collection that was stored via SetProperty()
        /// </summary>
        /// <param name="propertyString">String of XML like values (not proper XML)</param>
        /// <param name="key">The key of the property to return or empty string</param>
        /// <returns></returns>
        public static string GetProperty(string propertyString, string key)
        {
            var value = StringUtils.ExtractString(propertyString, "<" + key + ">", "</" + key + ">");
            return value;
        }


        /// <summary>
        /// Sets a property value in an XML-like structure that can be used to store properties
        /// in a string.
        /// </summary>
        /// <param name="propertyString">String of XML like values (not proper XML)</param>
        /// <param name="key">a key in that string</param>
        /// <param name="value">the string value to store</param>
        /// <returns></returns>
        public static string SetProperty(string propertyString, string key, string value)
        {
            string extract = StringUtils.ExtractString(propertyString, "<" + key + ">", "</" + key + ">");

            if (string.IsNullOrEmpty(value) && extract != string.Empty)
            {
                return propertyString.Replace(extract, "");
            }

            // NOTE: Value is not XML encoded - we only retrieve based on named nodes so no conflict
            string xmlLine = "<" + key + ">" + value + "</" + key + ">";

            // replace existing
            if (extract != string.Empty)
                return propertyString.Replace(extract, xmlLine);

            // add new
            return propertyString + xmlLine + "\r\n";
        }


        /// <summary>
        /// A helper to generate a JSON string from a string value
        /// 
        /// Use this to avoid bringing in a full JSON Serializer for
        /// scenarios of string serialization.
        /// </summary>
        /// <param name="text"></param>
        /// <returns>JSON encoded string ("text"), empty ("") or "null".</returns>
        public static string ToJsonString(string text)
        {
            if (text is null)
                return "null";

            var sb = new StringBuilder(text.Length);
            sb.Append("\"");
            var ct = text.Length;

            for (int x = 0; x < ct; x++)
            {
                var c = text[x];

                switch (c)
                {
                    case '\"':
                        sb.Append("\\\"");
                        break;
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    default:
                        uint i = c;
                        if (i < 32)  // || i > 255
                            sb.Append($"\\u{i:x4}");
                        else
                            sb.Append(c);
                        break;
                }
            }
            sb.Append("\"");

            return sb.ToString();
        }
        #endregion
    }

    public enum LineFeedTypes
    {
        // Linefeed \n only
        Lf,
        // Carriage Return and Linefeed \r\n
        CrLf,
        // Platform default Environment.NewLine
        Auto
    }
}