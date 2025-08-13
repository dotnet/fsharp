// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Expressions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module AssertExpression =

    [<Fact>]
    let ``assert (1 = 2)`` () =
        FSharp """assert (1 = 2)"""
        |> withOptions ["--define:DEBUG"]
        |> compileAndRun
        |> shouldFail
        |> withStdErrContains """(1 = 2)"""
    
    [<Fact>]
    let ``assert (1 = 2) in fsi`` () =
        Fsx """assert (1 = 2)"""
        |> withOptions ["--define:DEBUG"]
        |> runFsi
        |> shouldFail
        |> withStdErrContains """(1 = 2)"""

    [<Fact>]
    let ``assert ("\n" = "\n\n")`` () =
        FSharp """assert ("\n" = "\n\n")"""
        |> withOptions ["--define:DEBUG"]
        |> compileAndRun
        |> shouldFail
        |> withStdErrContains """("\n" = "\n\n")"""

    [<Fact>]
    let ``assert tripleQuoteString`` () =
        FSharp "
assert
    \"\"\"
    abcdef
    \"\"\" 
        = 
            \"\"\"
    ghi
    \"\"\"
    "
        |> withOptions ["--define:DEBUG"]
        |> compileAndRun
        |> shouldFail
        |> withStdErrContains "\"\"\"
    abcdef
    \"\"\" 
        = 
            \"\"\"
    ghi
    \"\"\""
