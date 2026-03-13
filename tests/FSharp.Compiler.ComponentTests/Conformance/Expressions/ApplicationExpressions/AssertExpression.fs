// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Expressions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module AssertExpression =

    let test (code: string) =
        let msgShouldContains = code.[7..].Replace(@"\", @"\\").Replace("\"", "\\\"")
        FSharp $"
namespace System.Diagnostics
type Debug =
    static member Assert(condition: bool) = if not condition then Some \"Assertion failed\" else None
    static member Assert(condition: bool, message: string) =
        if not condition then Some message else None

namespace global
module Test =
    match %s{code} with
    | Some msg when msg = \"%s{msgShouldContains}\" -> ()
    | None -> ()
    | Some msg -> failwith msg"
        |> withOptions ["--define:DEBUG"]
        |> withLangVersionPreview
        |> asExe
        |> compileAndRun
        |> shouldSucceed
    
    [<Fact>]
    let ``assert (1 = 2)`` () =
        test "assert (1 = 2)"

    [<Fact>]
    let ``assert ("\n" = "\n\n")`` () =
        test "assert (\"\\n\" = \"\\n\\n\")"
