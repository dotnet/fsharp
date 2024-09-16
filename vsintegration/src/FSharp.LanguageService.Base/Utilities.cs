// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

using System;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Shell;
using System.Diagnostics;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Microsoft.VisualStudio.FSharp.LanguageService
{

    internal class NativeHelpers
    {
        private NativeHelpers() { }

        internal static void RaiseComError(int hr)
        {
            throw new COMException("", (int)hr);
        }
        internal static void RaiseComError(int hr, string message)
        {
            throw new COMException(message, (int)hr);
        }
    }

    internal sealed class TextSpanHelper
    {

        private TextSpanHelper() { }

        /// <devdoc>Returns true if the first span starts after the start of the second span.</devdoc>
        internal static bool StartsAfterStartOf(TextSpan span1, TextSpan span2)
        {
            return (span1.iStartLine > span2.iStartLine || (span1.iStartLine == span2.iStartLine && span1.iStartIndex >= span2.iStartIndex));
        }
        /// <devdoc>Returns true if the first span starts after the end of the second span.</devdoc>
        internal static bool StartsAfterEndOf(TextSpan span1, TextSpan span2)
        {
            return (span1.iStartLine > span2.iEndLine || (span1.iStartLine == span2.iEndLine && span1.iStartIndex >= span2.iEndIndex));
        }
        /// <devdoc>Returns true if the first span starts before the start of the second span.</devdoc>
        internal static bool StartsBeforeStartOf(TextSpan span1, TextSpan span2)
        {
            return !StartsAfterStartOf(span1, span2);
        }
        /// <devdoc>Returns true if the first span starts before the end of the second span.</devdoc>
        internal static bool StartsBeforeEndOf(TextSpan span1, TextSpan span2)
        {
            return (span1.iStartLine < span2.iEndLine ||
                (span1.iStartLine == span2.iEndLine && span1.iStartIndex < span2.iEndIndex));
        }

        /// <devdoc>Returns true if the first span ends before the start of the second span.</devdoc>
        internal static bool EndsBeforeStartOf(TextSpan span1, TextSpan span2)
        {
            return (span1.iEndLine < span2.iStartLine || (span1.iEndLine == span2.iStartLine && span1.iEndIndex <= span2.iStartIndex));
        }
        /// <devdoc>Returns true if the first span starts before the end of the second span.</devdoc>
        internal static bool EndsBeforeEndOf(TextSpan span1, TextSpan span2)
        {
            return (span1.iEndLine < span2.iEndLine || (span1.iEndLine == span2.iEndLine && span1.iEndIndex <= span2.iEndIndex));
        }
        /// <devdoc>Returns true if the first span ends after the start of the second span.</devdoc>
        internal static bool EndsAfterStartOf(TextSpan span1, TextSpan span2)
        {
            return (span1.iEndLine > span2.iStartLine ||
                (span1.iEndLine == span2.iStartLine && span1.iEndIndex > span2.iStartIndex));
        }
        /// <devdoc>Returns true if the first span starts after the end of the second span.</devdoc>
        internal static bool EndsAfterEndOf(TextSpan span1, TextSpan span2)
        {
            return !EndsBeforeEndOf(span1, span2);
        }

        internal static TextSpan Merge(TextSpan span1, TextSpan span2)
        {
            TextSpan span = new TextSpan();

            if (StartsAfterStartOf(span1, span2))
            {
                span.iStartLine = span2.iStartLine;
                span.iStartIndex = span2.iStartIndex;
            }
            else
            {
                span.iStartLine = span1.iStartLine;
                span.iStartIndex = span1.iStartIndex;
            }

            if (EndsBeforeEndOf(span1, span2))
            {
                span.iEndLine = span2.iEndLine;
                span.iEndIndex = span2.iEndIndex;
            }
            else
            {
                span.iEndLine = span1.iEndLine;
                span.iEndIndex = span1.iEndIndex;
            }

            return span;
        }
        internal static bool IsPositive(TextSpan span)
        {
            return (span.iStartLine < span.iEndLine || (span.iStartLine == span.iEndLine && span.iStartIndex <= span.iEndIndex));
        }
        internal static void Clear(ref TextSpan span)
        {
            span.iStartLine = span.iEndLine = 0;
            span.iStartIndex = span.iEndIndex = 0;
        }
        internal static bool IsEmpty(TextSpan span)
        {
            return (span.iStartLine == span.iEndLine) && (span.iStartIndex == span.iEndIndex);
        }
        internal static void MakePositive(ref TextSpan span)
        {
            if (!IsPositive(span))
            {
                int line;
                int idx;

                line = span.iStartLine;
                idx = span.iStartIndex;
                span.iStartLine = span.iEndLine;
                span.iStartIndex = span.iEndIndex;
                span.iEndLine = line;
                span.iEndIndex = idx;
            }

            return;
        }
        /// <devdoc>Pins the text span to valid line bounds returned from IVsTextLines.</devdoc>
        internal static void Normalize(ref  TextSpan span, IVsTextLines textLines)
        {
            MakePositive(ref span);
            if (textLines == null) return;
            //adjust max. lines
            int lineCount;
            if (NativeMethods.Failed(textLines.GetLineCount(out lineCount)))
                return;
            span.iEndLine = Math.Min(span.iEndLine, lineCount - 1);
            //make sure the start is still before the end
            if (!IsPositive(span))
            {
                span.iStartLine = span.iEndLine;
                span.iStartIndex = span.iEndIndex;
            }
            //adjust for line length
            int lineLength;
            if (NativeMethods.Failed(textLines.GetLengthOfLine(span.iStartLine, out lineLength)))
                return;
            span.iStartIndex = Math.Min(span.iStartIndex, lineLength);
            if (NativeMethods.Failed(textLines.GetLengthOfLine(span.iEndLine, out lineLength)))
                return;
            span.iEndIndex = Math.Min(span.iEndIndex, lineLength);
        }

        internal static bool IsSameSpan(TextSpan span1, TextSpan span2)
        {
            return span1.iStartLine == span2.iStartLine && span1.iStartIndex == span2.iStartIndex && span1.iEndLine == span2.iEndLine && span1.iEndIndex == span2.iEndIndex;
        }

        // Returns true if the given position is to left of textspan.
        internal static bool IsBeforeStartOf(TextSpan span, int line, int col)
        {
            if (line < span.iStartLine || (line == span.iStartLine && col < span.iStartIndex))
            {
                return true;
            }
            return false;
        }

        // Returns true if the given position is to right of textspan.
        internal static bool IsAfterEndOf(TextSpan span, int line, int col)
        {
            if (line > span.iEndLine || (line == span.iEndLine && col > span.iEndIndex))
            {
                return true;
            }
            return false;
        }

        // Returns true if the given position is at the edge or inside the span.
        internal static bool ContainsInclusive(TextSpan span, int line, int col)
        {
            if (line > span.iStartLine && line < span.iEndLine)
                return true;

            if (line == span.iStartLine)
            {
                return (col >= span.iStartIndex && (line < span.iEndLine ||
                    (line == span.iEndLine && col <= span.iEndIndex)));
            }
            if (line == span.iEndLine)
            {
                return col <= span.iEndIndex;
            }
            return false;
        }

        // Returns true if the given position is purely inside the span.
        internal static bool ContainsExclusive(TextSpan span, int line, int col)
        {
            if (line > span.iStartLine && line < span.iEndLine)
                return true;

            if (line == span.iStartLine)
            {
                return (col > span.iStartIndex && (line < span.iEndLine ||
                    (line == span.iEndLine && col < span.iEndIndex)));
            }
            if (line == span.iEndLine)
            {
                return col < span.iEndIndex;
            }
            return false;
        }

        //returns true is span1 is Embedded in span2
        internal static bool IsEmbedded(TextSpan span1, TextSpan span2)
        {
            return (!TextSpanHelper.IsSameSpan(span1, span2) &&
                TextSpanHelper.StartsAfterStartOf(span1, span2) &&
                    TextSpanHelper.EndsBeforeEndOf(span1, span2));
        }

        internal static bool Intersects(TextSpan span1, TextSpan span2)
        {
            return TextSpanHelper.StartsBeforeEndOf(span1, span2) &&
                TextSpanHelper.EndsAfterStartOf(span1, span2);
        }

        // This method simulates what VS does in debug mode so that we can catch the
        // errors in managed code before they go to the native debug assert.
        internal static bool ValidSpan(ISource src, TextSpan span)
        {
            if (!ValidCoord(src, span.iStartLine, span.iStartIndex))
                return false;

            if (!ValidCoord(src, span.iEndLine, span.iEndIndex))
                return false;

            // end must be >= start
            if (!TextSpanHelper.IsPositive(span))
                return false;

            return true;
        }

        internal static bool ValidCoord(ISource src, int line, int pos)
        {
            // validate line
            if (line < 0)
            {
                Debug.Assert(false, "line < 0");
                return false;
            }

            // validate index
            if (pos < 0)
            {
                Debug.Assert(false, "pos < 0");
                return false;
            }

            if (src != null)
            {
                int lineCount = src.GetLineCount();
                if (line >= lineCount)
                {
                    Debug.Assert(false, "line > linecount");
                    return false;
                }

                int lineLength = src.GetLineLength(line);
                if (pos > lineLength)
                {
                    Debug.Assert(false, "pos > linelength");
                    return false;
                }

            }
            return true;
        }
    }

    internal struct Variant
    {

        internal enum VariantType
        {
            VT_EMPTY = 0,
            VT_NULL = 1,
            VT_I2 = 2,
            VT_I4 = 3,
            VT_R4 = 4,
            VT_R8 = 5,
            VT_CY = 6,
            VT_DATE = 7,
            VT_BSTR = 8,
            VT_DISPATCH = 9,
            VT_ERROR = 10,
            VT_BOOL = 11,
            VT_VARIANT = 12,
            VT_UNKNOWN = 13,
            VT_DECIMAL = 14,
            VT_I1 = 16,
            VT_UI1 = 17,
            VT_UI2 = 18,
            VT_UI4 = 19,
            VT_I8 = 20,
            VT_UI8 = 21,
            VT_INT = 22,
            VT_UINT = 23,
            VT_VOID = 24,
            VT_HRESULT = 25,
            VT_PTR = 26,
            VT_SAFEARRAY = 27,
            VT_CARRAY = 28,
            VT_USERDEFINED = 29,
            VT_LPSTR = 30,
            VT_LPWSTR = 31,
            VT_FILETIME = 64,
            VT_BLOB = 65,
            VT_STREAM = 66,
            VT_STORAGE = 67,
            VT_STREAMED_OBJECT = 68,
            VT_STORED_OBJECT = 69,
            VT_BLOB_OBJECT = 70,
            VT_CF = 71,
            VT_CLSID = 72,
            VT_VECTOR = 0x1000,
            VT_ARRAY = 0x2000,
            VT_BYREF = 0x4000,
            VT_RESERVED = 0x8000,
            VT_ILLEGAL = 0xffff,
            VT_ILLEGALMASKED = 0xfff,
            VT_TYPEMASK = 0xfff
        };

        private ushort vt;

        internal VariantType Vt
        {
            get
            {
                return (VariantType)vt;
            }
            set
            {
                vt = (ushort)value;
            }
        }

        private long value;

        internal long Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }

        internal static Variant ToVariant(IntPtr ptr)
        {
            // Marshal.GetObjectForNativeVariant is doing way too much work.
            // it is safer and more efficient to map only those things we 
            // care about.

            try
            {
                Variant v = (Variant)Marshal.PtrToStructure(ptr, typeof(Variant));
                return v;
#if DEBUG
            }
            catch (ArgumentException e)
            {
                Debug.Assert(false, e.Message);
#else
                } catch (ArgumentException) {
#endif
            }
            return new Variant();
        }

        internal char ToChar()
        {
            if (this.Vt == VariantType.VT_UI2)
            {
                ushort cv = (ushort)(this.value & 0xffff);
                return Convert.ToChar(cv);
            }
            return '\0';
        }

    }

    internal sealed class FilePathUtilities
    {
        /// <summary>
        /// Get path for text buffer.
        /// </summary>
        /// <param name="textLines">The text buffer.</param>
        /// <returns>The path of the text buffer.</returns>
        internal static string GetFilePath(IVsTextLines textLines)
        {
            if (textLines == null)
            {
                throw new ArgumentNullException("textLines");
            }

            return GetFilePathInternal(textLines);
        }

        /// <summary>
        /// Get file path for an object that is implementing IVsUserData.
        /// </summary>
        /// <param name="unknown">Reference to an IUnknown interface.</param>
        /// <returns>The file path</returns>
        internal static string GetFilePath(IntPtr unknown)
        {
            if (unknown == IntPtr.Zero)
            {
                throw new ArgumentNullException("unknown");
            }
            object obj = Marshal.GetObjectForIUnknown(unknown);
            return GetFilePathInternal(obj);
        }

        internal static string GetFilePathInternal(object obj)
        {
            string fname = null;
            int hr = 0;
            IVsUserData ud = obj as IVsUserData;
            if (ud != null)
            {
                object oname;
                Guid GUID_VsBufferMoniker = typeof(IVsUserData).GUID;
                hr = ud.GetData(ref GUID_VsBufferMoniker, out oname);
                if ( Microsoft.VisualStudio.ErrorHandler.Succeeded(hr) && oname != null)
                    fname = oname.ToString();
            }
            if (string.IsNullOrEmpty(fname))
            {
                IPersistFileFormat fileFormat = obj as IPersistFileFormat;
                if (fileFormat != null)
                {
                    uint format;
                    hr = fileFormat.GetCurFile(out fname, out format);
                }
            }
            if (!string.IsNullOrEmpty(fname))
            {
                Microsoft.VisualStudio.Shell.Url url = new Microsoft.VisualStudio.Shell.Url(fname);
                if (!url.Uri.IsAbsoluteUri)
                {
                    // make the file name absolute using app startup path...
                    Microsoft.VisualStudio.Shell.Url baseUrl = new Microsoft.VisualStudio.Shell.Url(Application.StartupPath + Path.DirectorySeparatorChar);
                    url = new Microsoft.VisualStudio.Shell.Url (baseUrl, fname);
                    fname = url.AbsoluteUrl;
                }
            }
            return fname;
        }

        /// <summary>This method returns the file extension in lower case, including the "."
        /// and trims any blanks or null characters from the string.  Null's can creep in via
        /// interop if we get a badly formed BSTR</summary>
        internal static string GetFileExtension(string moniker)
        {
            string ext = Path.GetExtension(moniker) ?? string.Empty;
            ext = ext.Trim();
            int i = 0;
            for (i = ext.Length - 1; i >= 0; i--)
            {
                if (ext[i] != '\0') break;
            }
            i++;
            if (i >= 0 && i < ext.Length) ext = ext.Substring(0, i);
            return ext;
        }
    }



    ////////////////////////////////////////////////////////////////////////////
    // below here is completion-filtering-matching algorithm, stolen from Roslyn
    // stuff that allows e.g.
    //     System.Console.WrL
    // to find WriteLine from WrL
    ////////////////////////////////////////////////////////////////////////////

    internal enum MatchResultType
    {
        Exact,
        Prefix,
        CamelCase
    }

    internal class MatchResult
    {
        public MatchResult(MatchResultType type, bool caseSensitive, int? camelCaseWeight)
        {
            this.ResultType = type;
            this.CaseSensitive = caseSensitive;
            this.CamelCaseWeight = camelCaseWeight;
        }
        public int? CamelCaseWeight { get; private set; }
        public bool CaseSensitive { get; private set; }
        public MatchResultType ResultType { get; private set; }
    }

    internal class AbstractPatternMatcher
    {
        static internal AbstractPatternMatcher Singleton = new AbstractPatternMatcher(false);
        /// <summary>
        /// Whether case-sensitive matches are preferred. When set to true, this means that
        /// case-sensitive matches may be prefered over a case-insensitive one. For example, if the
        /// candidate matches the pattern exactly but case insensitively, it will still lose to a
        /// case-sensitive camel-case match.
        /// </summary>
        private readonly bool caseSensitive;
        private readonly Dictionary<string, List<string>> stringToWordParts = new Dictionary<string, List<string>>();
        private readonly Dictionary<string, List<string>> stringToCharacterParts = new Dictionary<string, List<string>>();
        private readonly Func<string, List<string>> breakIntoWordParts = BreakIntoWordParts;
        private readonly Func<string, List<string>> breakIntoCharacterParts = BreakIntoCharacterParts;

        public AbstractPatternMatcher(bool caseSensitive)
        {
            this.caseSensitive = caseSensitive;
        }

        private List<string> GetOrAdd(Dictionary<string, List<string>> d, string candidate, Func<string, List<string>> f)
        {
            List<string> items;
            if (!d.TryGetValue(candidate, out items))
            {
                items = f(candidate);
                d.Add(candidate, items);
            }
            return items;
        }

        /// <summary>
        /// Determines if a candidate string should matched given the user's pattern. 
        /// </summary>
        /// <param name="candidate">The string to test.</param>
        /// <param name="pattern">The pattern to match against, which may use things like
        /// Camel-Cased patterns.</param>
        public MatchResult MatchSingleWordPattern(string candidate, string pattern)
        {
            lock (stringToWordParts)
            {
                return MatchSingleWordPattern(candidate, pattern, caseSensitive);
            }
        }

        private MatchResult MatchSingleWordPattern(string candidate, string pattern, bool preferCaseSensitive)
        {
            // We never match whitespace only
            if (string.IsNullOrWhiteSpace(pattern) || string.IsNullOrWhiteSpace(candidate))
            {
                return null;
            }

            var isCaseSensitiveExact = string.Equals(candidate, pattern, StringComparison.CurrentCulture);
            var isCaseSensitivePrefix = candidate.StartsWith(pattern, StringComparison.CurrentCulture);

            if (preferCaseSensitive && isCaseSensitiveExact)
            {
                return new MatchResult(MatchResultType.Exact, caseSensitive: true, camelCaseWeight: null);
            }

            if (preferCaseSensitive && isCaseSensitivePrefix)
            {
                return new MatchResult(MatchResultType.Prefix, caseSensitive: true, camelCaseWeight: null);
            }

            var candidateParts = GetOrAdd(stringToWordParts, candidate, breakIntoWordParts);
            var patternParts = GetOrAdd(stringToCharacterParts, pattern, breakIntoCharacterParts);
            // Periodically clear these out if they grow very large, to prevent large leaks over long periods of time.
            // These dictionaries are just a cache that uses some space to save time repeatedly parsing identifiers that
            // may appear again and again in completion lists.
            if (stringToWordParts.Count > 10000)
            {
                stringToWordParts.Clear();
            }
            if (stringToCharacterParts.Count > 10000)
            {
                stringToCharacterParts.Clear();
            }

            var caseSensitiveCamelCaseMatch = TryCamelCaseMatch(candidateParts, patternParts, StringComparison.CurrentCulture);

            if (preferCaseSensitive && caseSensitiveCamelCaseMatch.HasValue)
            {
                return new MatchResult(MatchResultType.CamelCase, true, caseSensitiveCamelCaseMatch);
            }

            // Now try case-insensitive matches
            if (string.Equals(candidate, pattern, StringComparison.CurrentCultureIgnoreCase))
            {
                // Even though we didn't prefer case-sensitive, this might have actually been it if
                // we did. Report it as being the original case sensitivity.
                return new MatchResult(MatchResultType.Exact, caseSensitive: isCaseSensitiveExact, camelCaseWeight: null);
            }

            if (candidate.StartsWith(pattern, StringComparison.CurrentCultureIgnoreCase))
            {
                return new MatchResult(MatchResultType.Prefix, caseSensitive: isCaseSensitivePrefix, camelCaseWeight: null);
            }

            var caseInsensitiveCamelCaseMatch = TryCamelCaseMatch(candidateParts, patternParts, StringComparison.CurrentCultureIgnoreCase);
            if (caseInsensitiveCamelCaseMatch.HasValue)
            {
                return new MatchResult(MatchResultType.CamelCase, caseSensitive: caseSensitiveCamelCaseMatch.HasValue, camelCaseWeight: caseInsensitiveCamelCaseMatch);
            }

            // Absolutely no match
            return null;
        }

        /// <summary>
        /// Breaks an identifier string into constituent parts. Do not call. Internal only for
        /// testing purposes.
        /// </summary>
        private static List<string> BreakIntoCharacterParts(string identifier)
        {
            return BreakIntoParts(identifier, word: false);
        }

        /// <summary>
        /// Breaks an identifier string into constituent parts. Do not call. Internal only for
        /// testing purposes.
        /// </summary>
        private static List<string> BreakIntoWordParts(string identifier)
        {
            return BreakIntoParts(identifier, word: true);
        }

        private static List<string> BreakIntoParts(string identifier, bool word)
        {
            var result = new List<string>();

            int wordStart = 0;
            for (int i = 1; i < identifier.Length; i++)
            {
                var lastIsDigit = char.IsDigit(identifier[i - 1]);
                var currentIsDigit = char.IsDigit(identifier[i]);

                var case1 = TransitionFromLowerToUpper(identifier, word, i);
                var case2 = TransitionFromUpperToLower(identifier, word, i, wordStart);

                if (identifier[i - 1] == '_' ||
                    identifier[i] == '_' ||
                    identifier[i - 1] == ' ' ||
                    identifier[i] == ' ' ||
                    lastIsDigit != currentIsDigit ||
                    case1 ||
                    case2)
                {
                    result.Add(identifier.Substring(wordStart, i - wordStart));
                    wordStart = i;
                }
            }

            result.Add(identifier.Substring(wordStart));
            return result;
        }

        private static bool TransitionFromUpperToLower(string identifier, bool word, int index, int wordStart)
        {
            if (word)
            {
                // Cases this supports:
                // 1) IDisposable -> I, Disposable
                // 2) UIElement -> UI, Element
                // 3) HTMLDocument -> HTML, Document
                //
                // etc.
                if (index != wordStart &&
                    index + 1 < identifier.Length)
                {
                    var currentIsUpper = char.IsUpper(identifier[index]);
                    var nextIsLower = char.IsLower(identifier[index + 1]);
                    if (currentIsUpper && nextIsLower)
                    {
                        // We have a transition from an upper to a lower letter here.  But we only
                        // want to break if all the letters that preceded are uppercase.  i.e. if we
                        // have "Foo" we don't want to break that into "F, oo".  But if we have
                        // "IFoo" or "UIFoo", then we want to break that into "I, Foo" and "UI,
                        // Foo".  i.e. the last uppercase letter belongs to the lowercase letters
                        // that follows.  Note: this will make the following not split properly:
                        // "HELLOthere".  However, these sorts of names do not show up in .Net
                        // programs.
                        for (int i = wordStart; i < index; i++)
                        {
                            if (!char.IsUpper(identifier[i]))
                            {
                                return false;
                            }
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        private static bool TransitionFromLowerToUpper(string identifier, bool word, int index)
        {
            var lastIsUpper = char.IsUpper(identifier[index - 1]);
            var currentIsUpper = char.IsUpper(identifier[index]);

            // See if the casing indicates we're starting a new word. Note: if we're breaking on
            // words, then just seeing an upper case character isn't enough.  Instead, it has to
            // be uppercase and the previous character can't be uppercase. 
            //
            // For example, breaking "AddMetadata" on words would make: Add Metadata
            //
            // on characters would be: A dd M etadata
            //
            // Break "AM" on words would be: AM
            //
            // on characters would be: A M
            //
            // We break the search string on characters.  But we break the symbol name on words.
            var transition = word
                ? (currentIsUpper && !lastIsUpper)
                : currentIsUpper;
            return transition;
        }

        protected static int? TryCamelCaseMatch(IList<string> candidateParts, IList<string> patternParts, StringComparison stringComparison)
        {
            // We must have at least as many 
            if (candidateParts.Count < patternParts.Count)
            {
                return null;
            }

            int candidateCurrent = 0;
            int patternCurrent = 0;
            int? firstMatch = null;
            int? lastMatch = null;

            while (true)
            {
                // Let's consider our termination cases
                if (patternCurrent == patternParts.Count)
                {
                    Debug.Assert(firstMatch.HasValue);
                    Debug.Assert(lastMatch.HasValue);

                    // We did match! We shall assign a weight to this
                    int weight = 0;

                    // Was this contiguous?
                    if (lastMatch.Value - firstMatch.Value + 1 == patternParts.Count)
                    {
                        weight += 1;
                    }

                    // Did we start at the beginning of the candidate?
                    if (firstMatch.Value == 0)
                    {
                        weight += 2;
                    }

                    return weight;
                }
                else if (candidateCurrent == candidateParts.Count)
                {
                    // No match, since we still have more of the pattern to hit
                    return null;
                }

                // Do we have a match?
                if (candidateParts[candidateCurrent].StartsWith(patternParts[patternCurrent], stringComparison))
                {
                    firstMatch = firstMatch.GetValueOrDefault(candidateCurrent);
                    lastMatch = candidateCurrent;
                    candidateCurrent++;
                    patternCurrent++;
                }
                else
                {
                    // We'll try to match the same word in our pattern to the next candidate word
                    candidateCurrent++;
                }
            }
        }
    }
}
