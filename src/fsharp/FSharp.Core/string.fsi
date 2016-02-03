// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Core

    open System
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.Operators
    open Microsoft.FSharp.Collections

    /// <summary>Functional programming operators for string processing.  Further string operations
    /// are available via the member functions on strings and other functionality in
    ///  <a href="http://msdn2.microsoft.com/en-us/library/system.string.aspx">System.String</a> 
    /// and <a href="http://msdn2.microsoft.com/library/system.text.regularexpressions.aspx">System.Text.RegularExpressions</a> types.</summary>
    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    [<RequireQualifiedAccess>]
    module String = 

        /// <summary>Returns a new string made by concatenating the given strings
        /// with separator <c>sep</c>, that is <c>a1 + sep + ... + sep + aN</c>.</summary>
        /// <param name="sep">The separator string to be inserted between the strings
        /// of the input sequence.</param>
        /// <param name="strings">The sequence of strings to be concatenated.</param>
        /// <returns>A new string consisting of the concatenated strings separated by
        /// the separation string.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown when <c>strings</c> is null.</exception>
        [<CompiledName("Concat")>]
        val concat: sep:string -> strings: seq<string> -> string

        /// <summary>Applies the function <c>action</c> to each character in the string.</summary>
        /// <param name="action">The function to be applied to each character of the string.</param>
        /// <param name="str">The input string.</param>
        [<CompiledName("Iterate")>]
        val iter: action:(char -> unit) -> str:string -> unit

        /// <summary>Applies the function <c>action</c> to the index of each character in the string and the
        /// character itself.</summary>
        /// <param name="action">The function to apply to each character and index of the string.</param>
        /// <param name="str">The input string.</param>
        [<CompiledName("IterateIndexed")>]
        val iteri: action:(int -> char -> unit) -> str:string -> unit

        /// <summary>Builds a new string whose characters are the results of applying the function <c>mapping</c>
        /// to each of the characters of the input string.</summary>
        /// <param name="mapping">The function to apply to the characters of the string.</param>
        /// <param name="str">The input string.</param>
        /// <returns>The resulting string.</returns>
        [<CompiledName("Map")>]
        val map: mapping:(char -> char) -> str:string -> string

        /// <summary>Builds a new string whose characters are the results of applying the function <c>mapping</c>
        /// to each character and index of the input string.</summary>
        /// <param name="mapping">The function to apply to each character and index of the string.</param>
        /// <param name="str">The input string.</param>
        /// <returns>The resulting string.</returns>
        [<CompiledName("MapIndexed")>]
        val mapi: mapping:(int -> char -> char) -> str:string -> string

        /// <summary>Builds a new string whose characters are the results of applying the function <c>mapping</c>
        /// to each of the characters of the input string and concatenating the resulting
        /// strings.</summary>
        /// <param name="mapping">The function to produce a string from each character of the input string.</param>
        /// <param name="str">The input string.</param>
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
        /// <returns>The resulting string.</returns>
        [<CompiledName("Filter")>]
        val filter: predicate:(char -> bool) -> str:string -> string

        /// <summary>Builds a new string whose characters are the results of applying the function <c>mapping</c>
        /// to each index from <c>0</c> to <c>count-1</c> and concatenating the resulting
        /// strings.</summary>
        /// <param name="count">The number of strings to initialize.</param>
        /// <param name="initializer">The function to take an index and produce a string to
        /// be concatenated with the others.</param>
        /// <returns>The constructed string.</returns>
        /// <exception cref="System.ArgumentException">Thrown when <c>count</c> is negative.</exception>
        [<CompiledName("Initialize")>]
        val init: count:int -> initializer:(int -> string) -> string

        /// <summary>Tests if all characters in the string satisfy the given predicate.</summary>
        /// <param name="predicate">The function to test each character of the string.</param>
        /// <param name="str">The input string.</param>
        /// <returns>True if all characters return true for the predicate and false otherwise.</returns>
        [<CompiledName("ForAll")>]
        val forall: predicate:(char -> bool) -> str:string -> bool

        /// <summary>Tests if any character of the string satisfies the given predicate.</summary>
        /// <param name="predicate">The function to test each character of the string.</param>
        /// <param name="str">The input string.</param>
        /// <returns>True if any character returns true for the predicate and false otherwise.</returns>
        [<CompiledName("Exists")>]
        val exists: predicate:(char -> bool) -> str:string -> bool

        /// <summary>Returns a string by concatenating <c>count</c> instances of <c>str</c>.</summary>
        /// <param name="count">The number of copies of the input string will be copied.</param>
        /// <param name="str">The input string.</param>
        /// <returns>The concatenated string.</returns>
        /// <exception cref="System.ArgumentException">Thrown when <c>count</c> is negative.</exception>
        [<CompiledName("Replicate")>]
        val replicate: count:int -> str: string -> string

        /// <summary>Returns the length of the string.</summary>
        /// <param name="str">The input string.</param>
        /// <returns>The number of characters in the string.</returns>
        [<CompiledName("Length")>]
        val length: str:string -> int

