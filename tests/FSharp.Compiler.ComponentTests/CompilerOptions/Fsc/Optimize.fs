// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test.Compiler

/// Tests for --optimize compiler option
module Optimize =

    // Sanity check - simply check that the option is valid
    [<InlineData("--optimize")>]
    [<InlineData("--optimize+")>]
    [<InlineData("-O")>]
    [<InlineData("--optimize-")>]
    [<Theory>]
    let ``optimize01 - valid optimize options`` (option: string) =
        Fs """
module optimize01
exit 0
"""
        |> asExe
        |> withOptions [option]
        |> compile
        |> shouldSucceed
        |> ignore

    // -O+ is not a valid option
    [<Fact>]
    let ``E_optimizeOPlus - invalid -O+ option`` () =
        Fs """exit 0"""
        |> asExe
        |> withOptions ["-O+"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 243, Line 0, Col 1, Line 0, Col 1, "Unrecognized option: '-O+'. Use '--help' to learn about recognized command line options.")
        ]

    // -O- is not a valid option
    [<Fact>]
    let ``E_optimizeOMinus - invalid -O- option`` () =
        Fs """exit 0"""
        |> asExe
        |> withOptions ["-O-"]
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Error 243, Line 0, Col 1, Line 0, Col 1, "Unrecognized option: '-O-'. Use '--help' to learn about recognized command line options.")
        ]

    // Regression for internal compiler error (ICE), FSB 4674
    // Compile with '--debug --optimize-'
    [<Fact>]
    let ``Regressions01 - debug with optimize minus`` () =
        Fs """
open System.IO

let PrependOrReplaceByToString s = id

type StorageDirectory() =

  let rec ReplaceOrInsert (e:StorageDirectory) = 
      let newFiles =  PrependOrReplaceByToString 3 []
      e.Copy(newFiles)

  member self.Copy(files) = 1

exit 0
"""
        |> asExe
        |> withOptions ["--debug"; "--optimize-"]
        |> compile
        |> shouldSucceed
        |> ignore
