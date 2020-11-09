// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.CompilerOptions.fsc

open Xunit
open FSharp.Test.Utilities.Compiler
open FSharp.Test.Utilities.Xunit.Attributes

module dumpAllCommandLineOptions =

    // This test was automatically generated (moved from FSharpQA suite - CompilerOptions/fsc/dumpAllCommandLineOptions)
    //<Expects status="notin">section='- ADVANCED -             ' ! option=readline                       kind=OptionSwitch</Expects>
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/CompilerOptions/fsc/dumpAllCommandLineOptions", Includes=[|"dummy.fs"|])>]
    let ``dumpAllCommandLineOptions - dummy.fs - --dumpAllCommandLineOptions`` compilation =
        compilation
        |> withOptions ["--dumpAllCommandLineOptions"]
        |> typecheck
        |> withDiagnosticMessageMatches "section='- ADVANCED -             ' ! option=readline                       kind=OptionSwitch"

