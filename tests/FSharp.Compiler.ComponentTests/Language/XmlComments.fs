// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.XmlComments

open Xunit
open FSharp.Test.Utilities.Compiler

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
         |> ignoreWarnings // means "don't treat warnings as errors"
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
    let ``invalid parameter reference is reported`` () =
        Fsx"""
    /// <summary> Return <paramref name="b" /> </summary>
    /// <param name="a"> the parameter </param>
    let f a = a
        """ 
         |> withXmlCommentChecking
         |> ignoreWarnings // means "don't treat warnings as errors"
         |> compile
         |> shouldSucceed
         |> withDiagnostics
                [ (Warning 3390, Line 2, Col 5, Line 3, Col 48,
                   "This XML comment is invalid: invalid parameter reference 'b'");
                ]
