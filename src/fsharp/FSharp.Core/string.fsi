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
        /// <example id="collect-example-1"> The following samples shows how to interspace spaces in a text
        /// <code lang="fsharp">
        /// let input = "Stefan says: Hi!"
        /// 
        /// input |> String.collect (sprintf "%c ")
        /// </code>
        /// The sample evaluates to <c>"S t e f a n   s a y s :   H i ! "</c>
        /// </example>
        /// 
        /// <example id="collect-example-2"> How to show the ASCII representation of a very secret text
        /// <code lang="fsharp">
        /// "Secret" |> String.collect (fun chr -> int chr |> sprintf "%d ")
        /// </code>
        /// The sample evaluates to <c>"83 101 99 114 101 116 "</c>
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
        /// <example id="concat-example-1">
        /// <code lang="fsharp">
        /// let input1 = ["Stefan"; "says:"; "Hello"; "there!"]
        /// 
        /// input1 |> String.concat " "  // evaluates "Stefan says: Hello there!"
        /// 
        /// let input2 = [0..9] |> List.map string 
        /// 
        /// input2 |> String.concat ""    // evaluates "0123456789"
        /// input2 |> String.concat ", "  // evaluates "0, 1, 2, 3, 4, 5, 6, 7, 8, 9"
        /// 
        /// let input3 = ["No comma"]
        /// 
        /// input3 |> String.concat ","   // evaluates "No comma"
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
        /// <example id="exists-example-1"> Looking for uppercase characters
        /// <code lang="fsharp">
        /// open System
        /// 
        /// "Yoda" |> String.exists Char.IsUpper  // evaluates true
        /// 
        /// "nope" |> String.exists Char.IsUpper  // evaluates false
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
        /// <example id="filter-example-1"> Filtering out just alphanumeric characters
        /// <code lang="fsharp">
        /// open System
        /// 
        /// let input = "0 1 2 3 4 5 6 7 8 9 a A m M"
        /// 
        /// input |> String.filter Uri.IsHexDigit  // evaluates "123456789aA"
        /// </code>
        /// </example>
        /// <example id="filter-example-2"> Filtering out just digits
        /// <code lang="fsharp">
        /// open System
        /// 
        /// "hello" |> String.filter Char.IsDigit  // evaluates ""
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
        /// <example id="forall-example-1"> Looking for lowercase characters
        /// <code lang="fsharp">
        /// open System
        /// 
        /// "all are lower" |> String.forall Char.IsLower  // evaluates false
        /// 
        /// "allarelower" |> String.forall Char.IsLower    // evaluates true
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
        ///
        /// <exception cref="T:System.ArgumentException">Thrown when <c>count</c> is negative.</exception>
        /// 
        /// <example id="init-example-1"> Enumerate digits ASCII codes
        /// <code lang="fsharp">
        /// String.init 10 (fun i -> int '0' + i |> sprintf "%d ")
        /// </code>
        /// The sample evaluates to: <c>"48 49 50 51 52 53 54 55 56 57 "</c>
        /// </example>
        [<CompiledName("Initialize")>]
        val init: count:int -> initializer:(int -> string) -> string

        /// <summary>Applies the function <c>action</c> to each character in the string.</summary>
        ///
        /// <param name="action">The function to be applied to each character of the string.</param>
        /// <param name="str">The input string.</param>
        /// 
        /// <example id="iter-example-1"> Printing the ASCII code for each characater in the string
        /// <code lang="fsharp">
        /// let input = "Hello"
        /// input |> String.iter (fun c -> printfn "%c %d" c (int c))
        /// </code>
        /// The sample evaluates as <c>unit</c>, but prints:
        /// <code>
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
        /// <example id="iteri-example-1"> Numbering the characters and printing the associated ASCII code 
        /// for each characater in the input string
        /// <code lang="fsharp">
        /// let input = "Hello"
        /// input |> String.iteri (fun i c -> printfn "%d. %c %d" (i + 1) c (int c))
        /// </code>
        /// The sample evaluates as <c>unit</c>, but prints:
        /// <code>
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
        /// <example id="length-example-1"> Getting the length of different strings
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
        /// <example id="map-example-1"> Changing case to upper for all characters in the input string
        /// <code lang="fsharp">
        /// open System
        /// 
        /// let input = "Hello there!"
        /// 
        /// input |> String.map Char.ToUpper  // evaluates "HELLO THERE!"
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
        /// <example id="mapi-example-1"> Alternating case for all characters in the input string
        /// <code lang="fsharp">
        /// open System
        /// 
        /// let alternateCase indx chr =
        ///     if 0 = indx % 2 then
        ///         Char.ToUpper chr 
        ///     else
        ///         Char.ToLower chr
        /// 
        /// let input = "Hello there!"
        /// 
        /// input |> String.mapi alternateCase  // evaluates "HeLlO ThErE!"
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
        /// <example id="replicate-example-1">
        /// <code lang="fsharp">
        /// "Do it!" |> String.replicate 3   // evaluates "Do it!Do it!Do it!"
        /// </code>
        /// </example>
        [<CompiledName("Replicate")>]
        val replicate: count:int -> str: string -> string
        

