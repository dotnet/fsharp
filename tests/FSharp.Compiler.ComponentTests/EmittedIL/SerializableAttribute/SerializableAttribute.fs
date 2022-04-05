namespace FSharp.Compiler.ComponentTests.EmittedIL

open Xunit
open System.IO
open FSharp.Test
open FSharp.Test.Compiler

module SerializableAttribute =

    let verifyCompilation compilation =
        compilation
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> asExe
        |> withNoOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> verifyILBaseline

    // SOURCE=ToplevelModule.fs    SCFLAGS="-a -g --out:TopLevelModule.dll --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ToplevelModule.dll"		# ToplevelModule.fs - Desktop
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ToplevelModule.fs"|])>]
    let ``ToplevelModule_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=ToplevelNamespace.fs SCFLAGS="-a -g --out:ToplevelNamespace.dll --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ToplevelNamespace.dll"		# ToplevelNamespace.fs - Desktop
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ToplevelNamespace.fs"|])>]
    let ``ToplevelNamespace_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=ToplevelModule.fs    SCFLAGS="-a -g --langversion:preview --out:TopLevelModule-preview.dll --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ToplevelModule-preview.dll"		# ToplevelModule.fs - Desktop preview
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ToplevelModule.fs"|])>]
    let ``ToplevelModule_LangVersion6.0_fs`` compilation =
        compilation
        |> withLangVersion60
        |> verifyCompilation

    // SOURCE=ToplevelNamespace.fs SCFLAGS="-a -g --langversion:preview --out:ToplevelNamespace-preview.dll --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ToplevelNamespace-preview.dll"		# ToplevelNamespace.fs - Desktop preview
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ToplevelNamespace.fs"|])>]
    let ``ToplevelNamespace_LangVersion6.0_fs`` compilation =
        compilation
        |> withLangVersion60
        |> verifyCompilation
