// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.XmlComments

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
        /// <param name="inner"> The innver value to unwrap</param>
        let doer ((Inner s) as inner) = ignore s; ignore inner
        """
         |> withXmlCommentChecking
         |> ignoreWarnings
         |> compile
         |> shouldSucceed
         |> withDiagnostics [ ]