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
        |> compile
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

    // =====================================================================================
    // Task 14: IL baseline tests for RuntimeAsync feature
    // Verify that [<MethodImpl(MethodImplOptions.Async)>] emits 'cil managed async' in IL
    // =====================================================================================

    // Verify that a simple async method with MethodImplOptions.Async emits the async IL flag.
    // The body returns Task<int> directly (function bindings use the declared return type).
    [<FactForNETCOREAPP>]
    let ``RuntimeAsync - method with Async attribute emits cil managed async in IL``() =
        FSharp """
module TestModule

open System.Runtime.CompilerServices
open System.Threading.Tasks

[<MethodImplAttribute(MethodImplOptions.Async)>]
let asyncMethod () : Task<int> = Task.FromResult(42)
"""
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
        |> verifyIL [
            // The method must carry the 'async' flag in its IL method header
            """cil managed async"""
        ]

    // Verify that the async flag is present on a method returning Task (non-generic).
    [<FactForNETCOREAPP>]
    let ``RuntimeAsync - Task-returning method emits cil managed async in IL``() =
        FSharp """
module TestModule

open System.Runtime.CompilerServices
open System.Threading.Tasks

[<MethodImplAttribute(MethodImplOptions.Async)>]
let asyncVoidMethod () : Task = Task.CompletedTask
"""
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
        |> verifyIL [
            """cil managed async"""
        ]

    // =====================================================================================
    // Task 15: Validation error tests for RuntimeAsync feature
    // =====================================================================================

    // Error 3885: async method cannot also be synchronized (0x2020 = 0x2000 | 0x20)
    [<Fact>]
    let ``RuntimeAsync - error when async method is also synchronized``() =
        FSharp """
module TestModule

open System.Runtime.CompilerServices
open System.Threading.Tasks

// Note: 0x2020 = Async (0x2000) + Synchronized (0x20)
[<MethodImplAttribute(enum<MethodImplOptions>(0x2020))>]
let invalidMethod () : Task<int> = Task.FromResult(42)
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withDiagnosticMessageMatches "cannot also use"

    // Error 3884: async method must return Task, Task<T>, ValueTask, or ValueTask<T>
    [<Fact>]
    let ``RuntimeAsync - error when async method does not return a Task type``() =
        FSharp """
module TestModule

open System.Runtime.CompilerServices

[<MethodImplAttribute(enum<MethodImplOptions>(0x2000))>]
let invalidMethod () : int = 42
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withDiagnosticMessageMatches "must return Task"

    // Feature gate: using MethodImplOptions.Async without --langversion:preview emits a preview feature error
    [<Fact>]
    let ``RuntimeAsync - error when Async attribute used without preview langversion``() =
        FSharp """
module TestModule

open System.Runtime.CompilerServices
open System.Threading.Tasks

[<MethodImplAttribute(enum<MethodImplOptions>(0x2000))>]
let asyncMethod () : Task<int> = Task.FromResult(42)
"""
        |> withLangVersion90
        |> typecheck
        |> shouldFail
        |> withDiagnosticMessageMatches "runtime async"