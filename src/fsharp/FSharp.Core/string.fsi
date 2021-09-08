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

        /// <summary>Builds a new string whose characters are the results of applying the function <c>mapping</c>
        /// to each of the characters of the input string and concatenating the resulting
        /// strings.</summary>
        ///
        /// <param name="mapping">The function to produce a string from each character of the input string.</param>
        /// <param name="str">The input string.</param>
        ///
        /// <returns>The concatenated string.</returns>
        /// 
        /// <example> The following samples shows how to interspace spaces in a text
        /// <code lang="fsharp">
        /// String.collect (sprintf "%c ") "Stefan says: Hi!"  // evaluates "S t e f a n   s a y s :   H i ! "
        /// </code>
        /// </example>
        /// 
        /// <example>How to show the ASCII representation of a very secret text:
        /// <code lang="fsharp">
        /// String.collect (fun chr -> int chr |> sprintf "%d ") "Secret"  // evaluates "83 101 99 114 101 116 "
        /// </code>
        /// </example>
        [<CompiledName("Collect")>]
        val collect: mapping:(char -> string) -> str:string -> string
        
        /// <summary>Returns a new string made by concatenating the given strings
        /// with separator <c>sep</c>, that is <c>a1 + sep + ... + sep + aN</c>.</summary>
        /// <param name="sep">The separator string to be inserted between the strings
        /// of the input sequence.</param>
        /// <param name="strings">The sequence of strings to be concatenated.</param>
        ///
        /// <returns>A new string consisting of the concatenated strings separated by
        /// the separation string.</returns>
        /// <exception cref="T:System.ArgumentNullException">Thrown when <c>strings</c> is null.</exception>
        /// 
        /// <example>
        /// <code lang="fsharp">
        /// String.concat " " ["Stefan"; "says:"; "Hello"; "there!"]  // evaluates "Stefan says: Hello there!"
        /// [0..9] |> List.map string |> String.concat ""             // evaluates "0123456789"
        /// String.concat "!" ["No exclamation point here"]           // evaluates "No exclamation point here"
        /// </code>
        /// </example>
        [<CompiledName("Concat")>]
        val concat: sep:string -> strings: seq<string> -> string

        /// <summary>Tests if any character of the string satisfies the given predicate.</summary>
        ///
        /// <param name="predicate">The function to test each character of the string.</param>
        /// <param name="str">The input string.</param>
        ///
        /// <returns>True if any character returns true for the predicate and false otherwise.</returns>
        /// 
        /// <example> Looking for uppercase characters
        /// <code lang="fsharp">
        /// String.exists System.Char.IsUpper "Yoda"  // evaluates true
        /// String.exists System.Char.IsUpper "nope"  // evaluates false
        /// </code>
        /// </example>
        [<CompiledName("Exists")>]
        val exists: predicate:(char -> bool) -> str:string -> bool

        /// <summary>Builds a new string containing only the characters of the input string
        /// for which the given predicate returns "true".</summary>
        ///
        /// <remarks>Returns an empty string if the input string is null</remarks>
        ///
        /// <param name="predicate">A function to test whether each character in the input sequence should be included in the output string.</param>
        /// <param name="str">The input string.</param>
        ///
        /// <returns>The resulting string.</returns>
        /// 
        /// <example> Filtering out just alphanumeric characters or just digits
        /// <code lang="fsharp">
        /// String.filter System.Uri.IsHexDigit "0 1 2 3 4 5 6 7 8 9 a A m M"  // evaluates "123456789aA"
        /// String.filter System.Char.IsDigit "hello"                          // evaluates ""
        /// </code>
        /// </example>
        [<CompiledName("Filter")>]
        val filter: predicate:(char -> bool) -> str:string -> string

        /// <summary>Tests if all characters in the string satisfy the given predicate.</summary>
        ///
        /// <param name="predicate">The function to test each character of the string.</param>
        /// <param name="str">The input string.</param>
        ///
        /// <returns>True if all characters return true for the predicate and false otherwise.</returns>
        ///         
        /// <example> Looking for lowercase characters
        /// <code lang="fsharp">
        /// String.forall System.Char.IsLower "all are lower"  // evaluates false
        /// String.forall System.Char.IsLower "allarelower"    // evaluates true
        /// </code>
        /// </example>
        [<CompiledName("ForAll")>]
        val forall: predicate:(char -> bool) -> str:string -> bool

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
        /// 
        /// <example> Enumerate digits ASCII codes
        /// <code lang="fsharp">
        /// String.init 10 (fun i -> int '0' + i |> sprintf "%d ")  // evaluates "48 49 50 51 52 53 54 55 56 57 "
        /// </code>
        /// </example>
        [<CompiledName("Initialize")>]
        val init: count:int -> initializer:(int -> string) -> string

        /// <summary>Applies the function <c>action</c> to each character in the string.</summary>
        ///
        /// <param name="action">The function to be applied to each character of the string.</param>
        /// <param name="str">The input string.</param>
        /// 
        /// <example>
        /// <code lang="fsharp">
        /// String.iter (fun c -> printfn "%c %d" c (int c)) "Hello"  
        /// // evaluates unit
        /// // prints:
        /// H 72
        /// e 101
        /// l 108
        /// l 108
        /// o 111
        /// </code>
        /// </example>
        [<CompiledName("Iterate")>]
        val iter: action:(char -> unit) -> str:string -> unit

        /// <summary>Applies the function <c>action</c> to the index of each character in the string and the
        /// character itself.</summary>
        ///
        /// <param name="action">The function to apply to each character and index of the string.</param>
        /// <param name="str">The input string.</param>
        /// 
        /// <example>
        /// <code lang="fsharp">
        /// String.length null   // evaluates 0
        /// String.length ""     // evaluates 0
        /// String.length "123"  // evaluates 3
        /// </code>
        /// </example>
        /// 
        /// <example>
        /// <code lang="fsharp">
        /// String.iteri (fun i c -> printfn "%d. %c %d" (i + 1) c (int c)) "Hello"
        /// // evaluates unit
        /// // prints:
        /// 1. H 72
        /// 2. e 101
        /// 3. l 108
        /// 4. l 108
        /// 5. o 111
        /// </code>
        /// </example>
        [<CompiledName("IterateIndexed")>]
        val iteri: action:(int -> char -> unit) -> str:string -> unit

        /// <summary>Returns the length of the string.</summary>
        ///
        /// <param name="str">The input string.</param>
        ///
        /// <returns>The number of characters in the string.</returns>
        /// 
        /// <example>
        /// <code lang="fsharp">
        /// String.length null   // evaluates 0
        /// String.length ""     // evaluates 0
        /// String.length "123"  // evaluates 3
        /// </code>
        /// </example>
        [<CompiledName("Length")>]
        val length: str:string -> int

        /// <summary>Builds a new string whose characters are the results of applying the function <c>mapping</c>
        /// to each of the characters of the input string.</summary>
        ///
        /// <param name="mapping">The function to apply to the characters of the string.</param>
        /// <param name="str">The input string.</param>
        ///
        /// <returns>The resulting string.</returns>
        /// 
        /// <example> Changing case to upper for all characters in the input string
        /// <code lang="fsharp">
        /// String.map System.Char.ToUpper "Hello there!"   // evaluates "HELLO THERE!"
        /// </code>
        /// </example>
        [<CompiledName("Map")>]
        val map: mapping:(char -> char) -> str:string -> string

        /// <summary>Builds a new string whose characters are the results of applying the function <c>mapping</c>
        /// to each character and index of the input string.</summary>
        ///
        /// <param name="mapping">The function to apply to each character and index of the string.</param>
        /// <param name="str">The input string.</param>
        ///
        /// <returns>The resulting string.</returns>
        /// 
        /// <example> Alternating case for all characters in the input string
        /// <code lang="fsharp">
        /// let alternateCase indx chr =
        ///     if 0 = indx % 2 
        ///         then System.Char.ToUpper chr 
        ///         else System.Char.ToLower chr
        /// String.mapi alternateCase "Hello there!"   // evaluates "HeLlO ThErE!"
        /// </code>
        /// </example>
        [<CompiledName("MapIndexed")>]
        val mapi: mapping:(int -> char -> char) -> str:string -> string

        /// <summary>Returns a string by concatenating <c>count</c> instances of <c>str</c>.</summary>
        ///
        /// <param name="count">The number of copies of the input string will be copied.</param>
        /// <param name="str">The input string.</param>
        ///
        /// <returns>The concatenated string.</returns>
        /// <exception cref="T:System.ArgumentException">Thrown when <c>count</c> is negative.</exception>
        /// 
        /// <example>
        /// <code lang="fsharp">
        /// String.replicate 3 "Do it! "  // evaluates "Do it! Do it! Do it! "
        /// </code>
        /// </example>
        [<CompiledName("Replicate")>]
        val replicate: count:int -> str: string -> string
        

