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
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"ToplevelModule.fs"|])>]
    let ``ToplevelModule_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=ToplevelModule.fs    SCFLAGS="-a -g --out:TopLevelModule.dll --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ToplevelModule.dll"		# ToplevelModule.fs - Desktop
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"ToplevelModule.fs"|])>]
    let ``ToplevelModule_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=ToplevelNamespace.fs SCFLAGS="-a -g --out:ToplevelNamespace.dll --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ToplevelNamespace.dll"		# ToplevelNamespace.fs - Desktop
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"ToplevelNamespace.fs"|])>]
    let ``ToplevelNamespace_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> verifyCompilation

    // SOURCE=ToplevelNamespace.fs SCFLAGS="-a -g --out:ToplevelNamespace.dll --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ToplevelNamespace.dll"		# ToplevelNamespace.fs - Desktop
    [<Theory; Directory(__SOURCE_DIRECTORY__,BaselineSuffix=".RealInternalSignatureOff",  Includes=[|"ToplevelNamespace.fs"|])>]
    let ``ToplevelNamespacec_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> verifyCompilation

    // SOURCE=ToplevelModule.fs    SCFLAGS="-a -g --langversion:6.0 --out:TopLevelModule-preview.dll --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ToplevelModule-preview.dll"		# ToplevelModule.fs - Desktop preview
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"ToplevelModule60.fs"|])>]
    let ``ToplevelModule_LangVersion60_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> withLangVersion60
        |> verifyCompilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"ToplevelModule60.fs"|])>]
    let ``ToplevelModule_LangVersion60_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> withLangVersion60
        |> verifyCompilation

    // SOURCE=ToplevelNamespace.fs SCFLAGS="-a -g --langversion:6.0 --out:ToplevelNamespace-preview.dll --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ToplevelNamespace-preview.dll"		# ToplevelNamespace.fs - Desktop preview
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOn", Includes=[|"ToplevelNamespace60.fs"|])>]
    let ``ToplevelNamespace_LangVersion60_RealInternalSignatureOn_fs`` compilation =
        compilation
        |> withRealInternalSignatureOn
        |> withLangVersion60
        |> verifyCompilation

    // SOURCE=ToplevelNamespace.fs SCFLAGS="-a -g --langversion:6.0 --out:ToplevelNamespace-preview.dll --test:EmitFeeFeeAs100001 --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd ToplevelNamespace-preview.dll"		# ToplevelNamespace.fs - Desktop preview
    [<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".RealInternalSignatureOff", Includes=[|"ToplevelNamespace60.fs"|])>]
    let ``ToplevelNamespace_LangVersion60_RealInternalSignatureOff_fs`` compilation =
        compilation
        |> withRealInternalSignatureOff
        |> withLangVersion60
        |> verifyCompilation
