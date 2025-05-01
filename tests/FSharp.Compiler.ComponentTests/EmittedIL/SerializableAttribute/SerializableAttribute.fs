namespace EmittedIL.RealInternalSignature

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
    [<Theory; FileInlineData("ToplevelModule.fs", Realsig=BooleanOptions.Both)>]
    let ``ToplevelModule_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=ToplevelNamespace.fs SCFLAGS="-a -g --out:ToplevelNamespace.dll --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ToplevelNamespace.dll"		# ToplevelNamespace.fs - Desktop
    [<Theory; FileInlineData("ToplevelNamespace.fs", Realsig=BooleanOptions.Both)>]
    let ``ToplevelNamespace_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=ToplevelModule.fs    SCFLAGS="-a -g --langversion:6.0 --out:TopLevelModule-preview.dll --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ToplevelModule-preview.dll"		# ToplevelModule.fs - Desktop preview
    [<Theory; FileInlineData("ToplevelModule60.fs", Realsig=BooleanOptions.Both)>]
    let ``ToplevelModule_LangVersion60_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersion60
        |> verifyCompilation

    // SOURCE=ToplevelNamespace.fs SCFLAGS="-a -g --langversion:6.0 --out:ToplevelNamespace-preview.dll --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ToplevelNamespace-preview.dll"		# ToplevelNamespace.fs - Desktop preview
    [<Theory; FileInlineData("ToplevelNamespace60.fs", Realsig=BooleanOptions.Both)>]
    let ``ToplevelNamespace_LangVersion60_fs`` compilation =
        compilation
        |> getCompilation
        |> withLangVersion60
        |> verifyCompilation
