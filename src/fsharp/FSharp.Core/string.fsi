// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Core

    open System
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.Operators
    open Microsoft.FSharp.Collections

    /// <summary>Functional programming operators for string processing.  Further string operations
    /// are available via the member functions on strings and other functionality in
    ///  <a href="http://msdn2.microsoft.com/en-us/library/system.string.aspx">System.String</a> 
    /// and <a href="http://msdn2.microsoft.com/library/system.text.regularexpressions.aspx">System.Text.RegularExpressions</a> types.
    /// </summary>
    ///
    /// <category>Strings and Text</category>
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    [<RequireQualifiedAccess>]
    module String = 

        /// <summary>Returns a new string made by concatenating the given strings
        /// with separator <c>sep</c>, that is <c>a1 + sep + ... + sep + aN</c>.</summary>
        /// <param name="sep">The separator string to be inserted between the strings
        /// of the input sequence.</param>
        /// <param name="strings">The sequence of strings to be concatenated.</param>
        ///
        /// <returns>A new string consisting of the concatenated strings separated by
        /// the separation string.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <c>strings</c> is null.</exception>
        [<CompiledName("Concat")>]
        val concat: sep:string -> strings: seq<string> -> string

        /// <summary>Applies the function <c>action</c> to each character in the string.</summary>
        ///
        /// <param name="action">The function to be applied to each character of the string.</param>
        /// <param name="str">The input string.</param>
        [<CompiledName("Iterate")>]
        val iter: action:(char -> unit) -> str:string -> unit

        /// <summary>Applies the function <c>action</c> to the index of each character in the string and the
        /// character itself.</summary>
        ///
        /// <param name="action">The function to apply to each character and index of the string.</param>
        /// <param name="str">The input string.</param>
        [<CompiledName("IterateIndexed")>]
        val iteri: action:(int -> char -> unit) -> str:string -> unit

        /// <summary>Builds a new string whose characters are the results of applying the function <c>mapping</c>
        /// to each of the characters of the input string.</summary>
        ///
        /// <param name="mapping">The function to apply to the characters of the string.</param>
        /// <param name="str">The input string.</param>
        ///
        /// <returns>The resulting string.</returns>
        [<CompiledName("Map")>]
        val map: mapping:(char -> char) -> str:string -> string

        /// <summary>Builds a new string whose characters are the results of applying the function <c>mapping</c>
        /// to each character and index of the input string.</summary>
        ///
        /// <param name="mapping">The function to apply to each character and index of the string.</param>
        /// <param name="str">The input string.</param>
        ///
        /// <returns>The resulting string.</returns>
        [<CompiledName("MapIndexed")>]
        val mapi: mapping:(int -> char -> char) -> str:string -> string

        /// <summary>Builds a new string whose characters are the results of applying the function <c>mapping</c>
        /// to each of the characters of the input string and concatenating the resulting
        /// strings.</summary>
        ///
        /// <param name="mapping">The function to produce a string from each character of the input string.</param>
        /// <param name="str">The input string.</param>
        ///
        /// <returns>The concatenated string.</returns>
        [<CompiledName("Collect")>]
        val collect: mapping:(char -> string) -> str:string -> string

        /// <summary>Builds a new string containing only the characters of the input string
        /// for which the given predicate returns "true".</summary>
        ///
        /// <remarks>Returns an empty string if the input string is null</remarks>
        ///
        /// <param name="predicate">A function to test whether each character in the input sequence should be included in the output string.</param>
        /// <param name="str">The input string.</param>
        ///
        /// <returns>The resulting string.</returns>
        [<CompiledName("Filter")>]
        val filter: predicate:(char -> bool) -> str:string -> string

        /// <summary>Builds a new string whose characters are the results of applying the function <c>mapping</c>
        /// to each index from <c>0</c> to <c>count-1</c> and concatenating the resulting
        /// strings.</summary>
        ///
        /// <param name="count">The number of strings to initialize.</param>
        /// <param name="initializer">The function to take an index and produce a string to
        /// be concatenated with the others.</param>
        ///
        /// <returns>The constructed string.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <c>count</c> is negative.</exception>
        [<CompiledName("Initialize")>]
        val init: count:int -> initializer:(int -> string) -> string

        /// <summary>Tests if all characters in the string satisfy the given predicate.</summary>
        ///
        /// <param name="predicate">The function to test each character of the string.</param>
        /// <param name="str">The input string.</param>
        ///
        /// <returns>True if all characters return true for the predicate and false otherwise.</returns>
        [<CompiledName("ForAll")>]
        val forall: predicate:(char -> bool) -> str:string -> bool

        /// <summary>Tests if any character of the string satisfies the given predicate.</summary>
        ///
        /// <param name="predicate">The function to test each character of the string.</param>
        /// <param name="str">The input string.</param>
        ///
        /// <returns>True if any character returns true for the predicate and false otherwise.</returns>
        [<CompiledName("Exists")>]
        val exists: predicate:(char -> bool) -> str:string -> bool

        /// <summary>Returns a string by concatenating <c>count</c> instances of <c>str</c>.</summary>
        ///
        /// <param name="count">The number of copies of the input string will be copied.</param>
        /// <param name="str">The input string.</param>
        ///
        /// <returns>The concatenated string.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <c>count</c> is negative.</exception>
        [<CompiledName("Replicate")>]
        val replicate: count:int -> str: string -> string

        /// <summary>Returns the length of the string.</summary>
        ///
        /// <param name="str">The input string.</param>
        ///
        /// <returns>The number of characters in the string.</returns>
        [<CompiledName("Length")>]
        val length: str:string -> int

        /// <summary>Returns whether or not <c>str</c> contains another string, <c>substring</c>.</summary>
        ///
        /// <param name="substring">The substring to check for.</param>
        /// <param name="str">The input string.</param>
        ///
        /// <returns>True if <c>substring</c> is a substring and false otherwise.</returns> 
        /// <exception cref="T:System.ArgumentNullException">Thrown when <c>str</c> is null.</exception>
        [<CompiledName("Contains")>]
        val contains: substring:string -> str:string -> bool

        /// <summary>
        ///     Returns a new string in which all occurrences of a specified string in an input string are replaced
        ///     with another string.
        /// </summary>
        ///
        /// <param name="oldValue">The string to be replaced.</param>
        /// <param name="newValue">The string to replace all occurences of <c>oldValue</c>.</param>
        /// <param name="str">The input string.</param>
        /// 
        /// <returns>
        ///     A string that is equivalent to this instance except that all occurences of <c>oldValue</c> are replaced
        ///     with <c>newValue</c>. If <c>oldValue</c> is not found in <c>str</c>, the function returns <c>str</c>
        ///     unchanged.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <c>str</c> is null.</exception>
        [<CompiledName("Replace")>]
        val replace: oldValue:string -> newValue:string -> str:string -> string

        /// <summary>
        ///     Returns a new string in which all occurrences of a specified Unicode character in an input string are
        /// replaced with another Unicode character.
        /// </summary>
        ///
        /// <param name="oldChar">The Unicode character to be replaced.</param>
        /// <param name="newChar">The Unicode character to replace all occurences of <c>oldChar</c>.</param>
        /// <param name="str">The input string.</param>
        /// 
        /// <returns>
        ///     A string that is equivalent to this instance except that all occurences of <c>oldChar</c> are replaced
        ///     with <c>newChar</c>. If <c>oldChar</c> is not found in <c>str</c>, the function returns <c>str</c>
        ///     unchanged.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <c>str</c> is null.</exception>
        [<CompiledName("ReplaceChar")>]
        val replaceChar: oldChar:char -> newChar:char -> str:string -> string
        
        /// <summary>
        ///     Returns a new string in which all leading and trailing white-space characters in an input string are
        ///     removed. White-space characters are defined by any character for which <c>System.Char.IsWhiteSpace</c>
        ///     returns <c>true</c>.
        /// </summary>
        ///
        /// <param name="str">The input string.</param>
        /// 
        /// <returns>The resulting string.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <c>str</c> is null.</exception>
        [<CompiledName("Trim")>]
        val trim: str:string -> string
        
        /// <summary>
        ///     Returns a new string in which all leading and trailing occurences of a set of specified characters 
        ///     in an input string are removed.
        /// </summary>
        ///
        /// <param name="chars">The set of strings to remove.</param>
        /// <param name="str">The input string.</param>
        /// 
        /// <returns>The resulting string.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <c>str</c> is null.</exception>
        [<CompiledName("TrimChars")>]
        val trimChars: chars:seq<char> -> str:string -> string
        
        /// <summary>
        ///     Returns a new string in which all leading occurences of a set of specified characters in an input
        ///     string are removed.
        /// </summary>
        ///
        /// <param name="chars">The set of strings to remove.</param>
        /// <param name="str">The input string.</param>
        /// 
        /// <returns>The resulting string.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <c>str</c> is null.</exception>
        [<CompiledName("TrimStartChars")>]
        val trimStartChars: chars:seq<char> -> str:string -> string
        
        /// <summary>
        ///     Returns a new string in which all trailing occurences of a set of specified characters in an input
        ///     string are removed.
        /// </summary>
        ///
        /// <param name="chars">The set of strings to remove.</param>
        /// <param name="str">The input string.</param>
        /// 
        /// <returns>The resulting string.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <c>str</c> is null.</exception>
        [<CompiledName("TrimEndChars")>]
        val trimEndChars: chars:seq<char> -> str:string -> string

        /// <summary>Splits a string into substrings based on specified delimiting characters.</summary>
        ///
        /// <param name="separators">The set of delimiting characters.</param>
        /// <param name="str">The input string.</param>
        /// 
        /// <returns>
        ///     An array whose elements contain the substrings from <c>str</c> that are delimited by one or more
        ///     characters in <c>separators</c>
        ///</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <c>str</c> is null.</exception>
        [<CompiledName("Split")>]
        val split: separators:seq<char> -> str:string -> string[]
        
        /// <summary>Splits a string into substrings based on specified delimiting strings.</summary>
        ///
        /// <param name="separators">The set of delimiting strings.</param>
        /// <param name="str">The input string.</param>
        /// 
        /// <returns>
        ///     An array whose elements contain the substrings from <c>str</c> that are delimited by one or more
        ///     strings in <c>separators</c>
        ///</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <c>str</c> is null.</exception>
        [<CompiledName("SplitStrings")>]
        val splitStrings: separators:seq<string> -> str:string -> string[]

        [<CompiledName("StartsWith")>]
        val startsWith: value:string -> str:string -> bool

        [<CompiledName("StartsWithComparison")>]
        val startsWithComparison : comparisonType:StringComparison -> value:string -> str:string -> bool
