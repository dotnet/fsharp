namespace FSharp.Compiler.ComponentTests.EmittedIL

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Inlining =

    let verifyCompilation compilation =
        compilation
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> asExe
        |> withOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> withCulture "en-US"
        |> verifyILBaseline

    // SOURCE=Match01.fs SCFLAGS="-a --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Match01.dll"	# Match01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Match01.fs"|])>]
    let ``Match01_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=Match02.fs SCFLAGS="-a --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd Match02.dll"	# Match02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Match02.fs"|])>]
    let ``Match02_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=StructUnion01.fs SCFLAGS="-a --optimize+" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd StructUnion01.dll"	# StructUnion01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"StructUnion01.fs"|])>]
    let ``StructUnion01_fs`` compilation =
        compilation
        |> verifyCompilation
