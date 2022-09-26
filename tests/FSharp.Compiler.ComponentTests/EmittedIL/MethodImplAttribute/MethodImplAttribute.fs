namespace FSharp.Compiler.ComponentTests.EmittedIL

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module MethodImplAttribute =

    let verifyCompilation compilation =
        compilation
        |> withOptions [ "--test:EmitFeeFeeAs100001" ]
        |> asExe
        |> withNoOptimize
        |> withEmbeddedPdb
        |> withEmbedAllSource
        |> ignoreWarnings
        |> verifyILBaseline

    // SOURCE=MethodImplAttribute.ForwardRef.fs        SCFLAGS="-a -g --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplAttribute.ForwardRef.dll"	# MethodImplAttribute.ForwardRef.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"MethodImplAttribute.ForwardRef.fs"|])>]
    let ``ForwardRef_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=MethodImplAttribute.InternalCall.fs      SCFLAGS="-a -g --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplAttribute.InternalCall.dll"	# MethodImplAttribute.InternalCall.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"MethodImplAttribute.InternalCall.fs"|])>]
    let ``InternalCall_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=MethodImplAttribute.NoInlining.fs        SCFLAGS="-a -g --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplAttribute.NoInlining.dll"	# MethodImplAttribute.NoInlining.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"MethodImplAttribute.NoInlining.fs"|])>]
    let ``NoInlining_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=MethodImplAttribute.AggressiveInlining.fs SCFLAGS="-a -g --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplAttribute.AggressiveInlining.dll"	# MethodImplAttribute.AggressiveInlining.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"MethodImplAttribute.AggressiveInlining.fs"|])>]
    let ``AggressiveInlining_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=MethodImplAttribute.NoOptimization.fs    SCFLAGS="-a -g --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplAttribute.NoOptimization.dll"	# MethodImplAttribute.NoOptimization.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"MethodImplAttribute.NoOptimization.fs"|])>]
    let ``NoOptimization_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=MethodImplAttribute.PreserveSig.fs       SCFLAGS="-a -g --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplAttribute.PreserveSig.dll"	# MethodImplAttribute.PreserveSig.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"MethodImplAttribute.PreserveSig.fs"|])>]
    let ``PreserveSig_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=MethodImplAttribute.Synchronized.fs      SCFLAGS="-a -g --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplAttribute.Synchronized.dll"	# MethodImplAttribute.Synchronized.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"MethodImplAttribute.Synchronized.fs"|])>]
    let ``Synchronized_fs`` compilation =
        compilation
        |> verifyCompilation

    // SOURCE=MethodImplAttribute.Unmanaged.fs         SCFLAGS="-a -g --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplAttribute.Unmanaged.dll"	# MethodImplAttribute.Unmanaged.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"MethodImplAttribute.Unmanaged.fs"|])>]
    let ``Unmanaged_fs`` compilation =
        compilation
        |> verifyCompilation
