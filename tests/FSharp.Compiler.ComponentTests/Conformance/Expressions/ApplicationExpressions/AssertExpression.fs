// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Expressions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module AssertExpression =

    let test (code: string) =
        let msgShouldContains = code.[7..].Replace(@"\", @"\\").Replace("\"", "\\\"")
        FSharp $"try
%s{code}
with ex -> if not(ex.Message.Contains \"%s{msgShouldContains}\") then reraise()"
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
