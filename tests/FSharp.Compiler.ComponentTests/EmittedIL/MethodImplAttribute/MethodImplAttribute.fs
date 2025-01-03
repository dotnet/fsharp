namespace EmittedIL

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
    [<Theory; FileInlineData("MethodImplAttribute.ForwardRef.fs")>]
    let ``ForwardRef_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=MethodImplAttribute.InternalCall.fs      SCFLAGS="-a -g --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplAttribute.InternalCall.dll"	# MethodImplAttribute.InternalCall.fs
    [<Theory; FileInlineData("MethodImplAttribute.InternalCall.fs")>]
    let ``InternalCall_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=MethodImplAttribute.NoInlining.fs        SCFLAGS="-a -g --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplAttribute.NoInlining.dll"	# MethodImplAttribute.NoInlining.fs
    [<Theory; FileInlineData("MethodImplAttribute.NoInlining.fs")>]
    let ``NoInlining_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation
     
    [<Theory; FileInlineData("MethodImplAttribute.NoInlining_InlineKeyword.fs")>]
    let ``NoInlining_fs with inline keyword => should warn in preview version`` compilation =
        compilation
        |> getCompilation
        |> withLangVersion80
        |> typecheck
        |> withSingleDiagnostic (Warning 3151, Line 3, Col 12, Line 3, Col 19, "This member, function or value declaration may not be declared 'inline'")
    
    [<Theory; FileInlineData("MethodImplAttribute.NoInlining_InlineKeyword.fs")>]
    let ``NoInlining_fs with inline keyword => should not warn in F# 7 or older`` compilation =
        compilation
        |> getCompilation
        |> withLangVersion70
        |> typecheck
        |> withDiagnostics  []       

    // SOURCE=MethodImplAttribute.AggressiveInlining.fs SCFLAGS="-a -g --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplAttribute.AggressiveInlining.dll"	# MethodImplAttribute.AggressiveInlining.fs
    [<Theory; FileInlineData("MethodImplAttribute.AggressiveInlining.fs")>]
    let ``AggressiveInlining_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=MethodImplAttribute.NoOptimization.fs    SCFLAGS="-a -g --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplAttribute.NoOptimization.dll"	# MethodImplAttribute.NoOptimization.fs
    [<Theory; FileInlineData("MethodImplAttribute.NoOptimization.fs")>]
    let ``NoOptimization_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=MethodImplAttribute.PreserveSig.fs       SCFLAGS="-a -g --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplAttribute.PreserveSig.dll"	# MethodImplAttribute.PreserveSig.fs
    [<Theory; FileInlineData("MethodImplAttribute.PreserveSig.fs")>]
    let ``PreserveSig_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=MethodImplAttribute.Synchronized.fs      SCFLAGS="-a -g --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplAttribute.Synchronized.dll"	# MethodImplAttribute.Synchronized.fs
    [<Theory; FileInlineData("MethodImplAttribute.Synchronized.fs")>]
    let ``Synchronized_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation

    // SOURCE=MethodImplAttribute.Unmanaged.fs         SCFLAGS="-a -g --optimize-" COMPILE_ONLY=1 POSTCMD="..\\CompareIL.cmd MethodImplAttribute.Unmanaged.dll"	# MethodImplAttribute.Unmanaged.fs
    [<Theory; FileInlineData("MethodImplAttribute.Unmanaged.fs")>]
    let ``Unmanaged_fs`` compilation =
        compilation
        |> getCompilation
        |> verifyCompilation
