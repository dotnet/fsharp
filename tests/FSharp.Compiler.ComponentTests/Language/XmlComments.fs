// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace XmlComments

open Xunit
open FSharp.Test.Compiler

module XmlCommentChecking =

    [<Fact>]
    let ``invalid XML is reported`` () =
        Fsx"""
/// <summary>
let x = 1

///
/// <summary>Yo</summary>
/// <remark>Yo</rem>
module M =
    /// <summary> <
    let y = 1

        """
         |> withXmlCommentChecking
         |> ignoreWarnings
         |> compile
         |> shouldSucceed
         |> withDiagnostics
                [ (Warning 3390, Line 2, Col 1, Line 2, Col 14,
                   "This XML comment is invalid: 'The 'summary' start tag on line 2 position 3 does not match the end tag of 'doc'. Line 3, position 3.'");
                  (Warning 3390, Line 5, Col 1, Line 7, Col 21,
                   """This XML comment is invalid: 'The 'remark' start tag on line 3 position 3 does not match the end tag of 'rem'. Line 3, position 14.'""");
                  (Warning 3390, Line 9, Col 5, Line 9, Col 20,
                   "This XML comment is invalid: 'Name cannot begin with the '\n' character, hexadecimal value 0x0A. Line 2, position 13.'")
                ]
    [<Fact>]
    let ``unknown parameter is reported`` () =
        Fsx"""
    /// <summary> Return <paramref name="b" /> </summary>
    /// <param name="a"> the parameter </param>
    let f a = a
        """
         |> withXmlCommentChecking
         |> ignoreWarnings
         |> compile
         |> shouldSucceed
         |> withDiagnostics
                [ (Warning 3390, Line 2, Col 5, Line 3, Col 48,
                   "This XML comment is invalid: unknown parameter 'b'");
                ]

    [<Fact>]
    let ``diagnostic is not reported when disabled`` () =
        Fsx"""
    /// <summary> F </summary>
    /// <param name="x"> the parameter </param>
    let f a = a
        """
         |> compile
         |> shouldSucceed
         |> withDiagnostics []

    [<Fact>]
    let ``invalid parameter name is reported`` () =
        Fsx"""
    /// <summary> Return <paramref name="b" /> </summary>
    /// <param name="b"> the parameter </param>
    let f a = a
        """
         |> withXmlCommentChecking
         |> ignoreWarnings
         |> compile
         |> shouldSucceed
         |> withDiagnostics
                [ (Warning 3390, Line 2, Col 5, Line 3, Col 48,
                   "This XML comment is invalid: unknown parameter 'b'");
                  (Warning 3390, Line 2, Col 5, Line 3, Col 48,
                   "This XML comment is incomplete: no documentation for parameter 'a'");
                ]

    [<Fact>]
    let ``duplicate parameter docs are reported`` () =
        Fsx"""
    /// <summary> Return <paramref name="a" /> </summary>
    /// <param name="a"> the parameter </param>
    /// <param name="a"> the parameter </param>
    let f a = a
        """
         |> withXmlCommentChecking
         |> ignoreWarnings
         |> compile
         |> shouldSucceed
         |> withDiagnostics
                [ (Warning 3390, Line 2, Col 5, Line 4, Col 48,
                   "This XML comment is invalid: multiple documentation entries for parameter 'a'");
                ]

    [<Fact>]
    let ``missing parameter name is reported`` () =
        Fsx"""
    /// <summary> Return <paramref/> </summary>
    /// <param> the parameter </param>
    let f a = a
        """
         |> withXmlCommentChecking
         |> ignoreWarnings
         |> compile
         |> shouldSucceed
         |> withDiagnostics
                [ (Warning 3390, Line 2, Col 5, Line 3, Col 39,
                   "This XML comment is invalid: missing 'name' attribute for parameter or parameter reference");
                ]

    [<Fact>]
    let ``valid parameter names are not reported`` () =
        Fsx"""
    /// <summary> Return <paramref name="a" /> </summary>
    /// <param name="a"> the parameter </param>
    let f a = a

    /// <summary> The type </summary>
    type C(x1: string, x2: string) =
         let _unused = (x1, x2)
        /// <summary> The instance method</summary>
        /// <param name="p1"> the parameter </param>
        /// <param name="p2"> the other parameter </param>
         member x.M(p1: string, p2: string) = (p1, p2)

        /// <summary> The instance method</summary>
        /// <param name="p2"> the other parameter </param>
         member x.OtherM((a,b): (string * string), p2: string) = ((a,b), p2)
        """
         |> withXmlCommentChecking
         |> ignoreWarnings
         |> compile
         |> shouldSucceed
         |> withDiagnostics [ ]

    [<Fact>]
    let ``valid parameter names are not reported for documented implicit constructor`` () =
        Fsx"""
    /// <summary> The type with an implicit constructor</summary>
    type C
        /// <summary> The constructor</summary>
        /// <param name="x1"> the parameter </param>
        /// <param name="x2"> the other parameter </param>
         (x1: string, x2: string) =
         let _unused = (x1, x2)

         member x.M(p1: string, p2: string) = (p1, p2)
        """
         |> withXmlCommentChecking
         |> ignoreWarnings
         |> compile
         |> shouldSucceed
         |> withDiagnostics [ ]

    [<Fact>]
    let ``valid parameter names are not reported for documented implicit constructor with visibility`` () =
        Fsx"""
    /// <summary> The type with an implicit constructor with visibility</summary>
    type C
        /// <summary> The constructor</summary>
        /// <param name="x1"> the parameter </param>
        /// <param name="x2"> the other parameter </param>
         public (x1: string, x2: string) =
         let _unused = (x1, x2)

         member x.M(p1: string, p2: string) = (p1, p2)
        """
         |> withXmlCommentChecking
         |> ignoreWarnings
         |> compile
         |> shouldSucceed
         |> withDiagnostics [ ]

    [<Fact>]
    let ``valid parameter names are reported for documented implicit constructor`` () =
        Fsx"""
    /// <summary> The type with an implicit constructor with visibility</summary>
    /// <param name="x1"> the first parameter </param>
    /// <param name="x2"> the second parameter </param>
    type C (x1: string, x2: string) =
         let _unused = (x1, x2)

         member x.M(p1: string, p2: string) = (p1, p2)
        """
         |> withXmlCommentChecking
         |> ignoreWarnings
         |> compile
         |> shouldSucceed
         |> withDiagnostics [ ]

    [<Fact>]
    let ``delegates can have param docs`` () =
        Fsx"""
    /// <summary> The type with an implicit constructor with visibility</summary>
    /// <param name="sender"> The sender</param>
    /// <param name="args"> The args</param>
    type C = delegate of sender: obj * args: int -> C
        """
         |> withXmlCommentChecking
         |> ignoreWarnings
         |> compile
         |> shouldSucceed
         |> withDiagnostics [ ]

    [<Fact>]
    let ``function parameters use names from as patterns`` () =
        Fsx"""
        type Thing = Inner of s: string
        /// <summary> A function with an extracted inner value</summary>
        /// <param name="inner"> The inner value to unwrap</param>
        let doer ((Inner s) as inner) = ignore s; ignore inner
        """
         |> withXmlCommentChecking
         |> ignoreWarnings
         |> compile
         |> shouldSucceed
         |> withDiagnostics [ ]

    [<Fact>]
    let ``Union field - unnamed 01`` () =
        Fsx"""
        type A =
            /// <summary>A</summary>
            /// <param name="Item">Item</param>
            | A of int
        """
         |> withXmlCommentChecking
         |> compile
         |> withDiagnostics [ Warning 3390, Line 3, Col 13, Line 4, Col 48, "This XML comment is invalid: unknown parameter 'Item'" ]

    [<Fact>]
    let ``Union field - unnamed 02`` () =
        Fsx"""
        type A =
            /// <summary>A</summary>
            /// <param name="a">a</param>
            | A of int * a: int
        """
         |> withXmlCommentChecking
         |> compile
         |> withDiagnostics [ ]

    // regression test for #18433
    [<Fact>]
    let OverrideXmlCommentsWithSameRange () =
        Fs"""
        module A
        # 1
        /// A is int
        type A = {a: int}
        # 1
        /// B is int
        type B = {b: int}
        """
        |> withXmlCommentChecking
        |> compile
        |> shouldSucceed

module XmlCommentCheckingGetSetProperty =

    [<Fact>]
    let ``Fully documented get-set property produces no xmldoc warning`` () =
        Fsx """
type MyClass() =
    /// <summary>A property</summary>
    /// <param name="index">The index</param>
    /// <param name="value">The value</param>
    member _.Item
        with get(index: int) = index
        and set (index: int) (value: int) = ()
        """
         |> withXmlCommentChecking
         |> ignoreWarnings
         |> compile
         |> shouldSucceed
         |> withDiagnostics []

    [<Fact>]
    let ``Simple get-set property with xmldoc no false warning`` () =
        Fsx """
type MyClass() =
    /// <summary>Gets or sets the value</summary>
    /// <param name="v">The value</param>
    member _.Prop
        with get() = 0
        and set (v: int) = ()
        """
         |> withXmlCommentChecking
         |> ignoreWarnings
         |> compile
         |> shouldSucceed
         |> withDiagnostics []

    [<Fact>]
    let ``Get-only property xmldoc check unaffected`` () =
        Fsx """
type MyClass() =
    /// <summary>A property</summary>
    /// <param name="index">The index</param>
    member _.Item with get(index: int) = index
        """
         |> withXmlCommentChecking
         |> ignoreWarnings
         |> compile
         |> shouldSucceed
         |> withDiagnostics []

    [<Fact>]
    let ``Two get-set properties each missing a param both warn`` () =
        Fsx """
type MyClass() =
    /// <summary>P1</summary>
    /// <param name="a">a</param>
    member _.P1 with get(a: int) = a and set (a: int) (b: int) = ()
    /// <summary>P2</summary>
    /// <param name="x">x</param>
    member _.P2 with get(x: int) = x and set (x: int) (y: int) = ()
        """
         |> withXmlCommentChecking
         |> ignoreWarnings
         |> compile
         |> shouldSucceed
         |> withDiagnostics
                [ (Warning 3390, Line 3, Col 5, Line 4, Col 34,
                   "This XML comment is incomplete: no documentation for parameter 'b'")
                  (Warning 3390, Line 6, Col 5, Line 7, Col 34,
                   "This XML comment is incomplete: no documentation for parameter 'y'") ]

    [<Fact>]
    let ``Actually missing param doc still warns for get-set`` () =
        Fsx """
type MyClass() =
    /// <summary>A property</summary>
    /// <param name="index">The index</param>
    member _.Item
        with get(index: int) = index
        and set (index: int) (value: int) = ()
        """
         |> withXmlCommentChecking
         |> ignoreWarnings
         |> compile
         |> shouldSucceed
         |> withDiagnostics
                [ (Warning 3390, Line 3, Col 5, Line 4, Col 46,
                   "This XML comment is incomplete: no documentation for parameter 'value'") ]

    [<Fact>]
    let ``Documented param that exists in neither accessor warns`` () =
        Fsx """
type MyClass() =
    /// <summary>A property</summary>
    /// <param name="index">The index</param>
    /// <param name="value">The value</param>
    /// <param name="ghost">Does not exist</param>
    member _.Item
        with get(index: int) = index
        and set (index: int) (value: int) = ()
        """
         |> withXmlCommentChecking
         |> ignoreWarnings
         |> compile
         |> shouldSucceed
         |> withDiagnostics
                [ (Warning 3390, Line 3, Col 5, Line 6, Col 51,
                   "This XML comment is invalid: unknown parameter 'ghost'") ]

    [<Theory>]
    [<InlineData("int", "int")>]
    [<InlineData("float", "float")>]
    let ``Get-set with different types fully documented is clean`` (getType: string) (setType: string) =
        let source =
            "\ntype MyClass() =\n"
            + "    /// <summary>Prop</summary>\n"
            + "    /// <param name=\"v\">The value</param>\n"
            + "    member _.Prop\n"
            + "        with get() : " + getType + " = Unchecked.defaultof<_>\n"
            + "        and set (v: " + setType + ") = ()\n        "
        Fsx source
         |> withXmlCommentChecking
         |> ignoreWarnings
         |> compile
         |> shouldSucceed
         |> withDiagnostics []

    [<Fact>]
    let ``Issue 13684 repro indexed property fully documented is clean`` () =
        Fsx """
type A =
    /// <summary></summary>
    /// <param name="j"></param>
    /// <param name="k"></param>
    /// <param name="l"></param>
    member x.A with get (j: int) : int = 3
               and set (k: int) (l: int) = ()
        """
         |> withXmlCommentChecking
         |> ignoreWarnings
         |> compile
         |> shouldSucceed
         |> withDiagnostics []

module XmlCommentCheckingGetSetPropertyRawFsc =

    open System
    open System.IO
    open FSharp.Test
    open FSharp.Test.Utilities
    open TestFramework

    // #13684: in-process diagnostics dedupe by range+message, so the raw fsc subprocess is the only backstop.
    let private paramLine name =
        sprintf "    /// <param name=\"%s\">_</param>\n" name

    let private sourceFor (paramNames: string) =
        let paramDocLines =
            paramNames.Split(' ') |> Array.map paramLine |> String.concat ""

        "module Repro\n\n"
        + "type MyClass() =\n"
        + "    /// <summary>A property</summary>\n"
        + paramDocLines
        + "    member _.Item\n"
        + "        with get(index: int) = index\n"
        + "        and set (index: int) (value: int) = ()\n"

    let private countWarnings (needle: string) (combined: string) =
        combined.Split([| '\n'; '\r' |], StringSplitOptions.RemoveEmptyEntries)
        |> Array.filter (fun line -> line.Contains "FS3390" && line.Contains needle)
        |> Array.length

    let private compileWithWarnon3390 (source: string) =
        let dir = createTemporaryDirectory().FullName
        let src = Path.Combine(dir, "Repro.fs")
        File.WriteAllText(src, source)
        let outDll = Path.Combine(dir, "Out.dll")
        let defaultOpts = CompilerAssert.DefaultProjectOptions(TargetFramework.Current).OtherOptions

        runFscProcess [
            "--target:library"
            "--warnon:3390"
            yield! defaultOpts |> Array.map Commands.quotepath
            $"-o:{Commands.quotepath outDll}"
            Commands.quotepath src
        ]

    [<TheoryForNETCOREAPP>]
    [<InlineData("index", "'value'")>]
    [<InlineData("index value ghost", "'ghost'")>]
    [<InlineData("value", "'index'")>]
    [<InlineData("index index value", "multiple documentation entries for parameter 'index'")>]
    let ``Property get-set XmlDoc warnings are not duplicated in raw fsc output``
        (paramNames: string, needle: string)
        =
        let result = compileWithWarnon3390 (sourceFor paramNames)
        let occurrences = countWarnings needle (result.StdOut + "\n" + result.StdErr)

        if occurrences <> 1 then
            failwithf
                "Expected exactly one FS3390 warning matching %s in raw fsc output, got %d.\nstdout:\n%s\nstderr:\n%s"
                needle occurrences result.StdOut result.StdErr