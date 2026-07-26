// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace EmittedIL

open Xunit
open FSharp.Test
open FSharp.Test.Compiler
open FSharp.Test.Utilities

module CodeGenRegressions_Crashes =

    let private getActualIL (result: CompilationResult) =
        match result with
        | CompilationResult.Success s ->
            match s.OutputPath with
            | Some p ->
                let (_, _, actualIL) = ILChecker.verifyILAndReturnActual [] p [ "// dummy" ]
                actualIL
            | None -> failwith "No output path"
        | _ -> failwith "Compilation failed"

    // https://github.com/dotnet/fsharp/issues/18956
    [<Fact>]
    let ``Issue_18956_DecimalConstantInvalidProgram`` () =
        let source = """
module A =
    [<Literal>]
    let B = 42m

[<EntryPoint>]
let main args =
    printfn "%M" A.B
    0
"""
        FSharp source
        |> asExe
        |> withDebug
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/18319
    [<Fact>]
    let ``Issue_18319_LiteralUpcastMissingBox`` () =
        let source = """
module Test

[<Literal>]
let badobj: System.ValueType = 1

[<EntryPoint>]
let main _ =
    System.Console.WriteLine(badobj)
    0
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/18319
    // Second case from issue: literal upcast used as default parameter value
    [<Fact>]
    let ``Issue_18319_LiteralUpcastDefaultParam`` () =
        let source = """
module Test

open System.Runtime.InteropServices

[<Literal>]
let lit: System.ValueType = 1

type C() =
    static member M([<Optional; DefaultParameterValue(lit)>] param: System.ValueType) =
        System.Console.WriteLine(param)

[<EntryPoint>]
let main _ =
    C.M()
    C.M(42)
    0
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/14508
    [<Fact>]
    let ``Issue_14508_NativeptrInInterfaces_CompileOnly`` () =
        let source = """
module Test

open Microsoft.FSharp.NativeInterop

type IFoo<'T when 'T : unmanaged> =
    abstract member Pointer : nativeptr<'T>

type Broken() =
    member x.Pointer : nativeptr<int> = Unchecked.defaultof<_>
    interface IFoo<int> with
        member x.Pointer = x.Pointer

type Working<'T when 'T : unmanaged>() =
    member x.Pointer : nativeptr<'T> = Unchecked.defaultof<_>
    interface IFoo<'T> with
        member x.Pointer = x.Pointer
"""
        FSharp source
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/14508
    [<Fact>]
    let ``Issue_14508_NativeptrInInterfaces_RuntimeWorking`` () =
        let source = """
open Microsoft.FSharp.NativeInterop

type IFoo<'T when 'T : unmanaged> =
    abstract member Pointer : nativeptr<'T>

type Working<'T when 'T : unmanaged>() =
    member x.Pointer : nativeptr<'T> = Unchecked.defaultof<_>
    interface IFoo<'T> with
        member x.Pointer = x.Pointer

printfn "Working type loaded successfully"
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/14508
    [<Fact>]
    let ``Issue_14508_NativeptrInInterfaces_RuntimeBroken`` () =
        let source = """
open Microsoft.FSharp.NativeInterop

type IFoo<'T when 'T : unmanaged> =
    abstract member Pointer : nativeptr<'T>

type Broken() =
    member x.Pointer : nativeptr<int> = Unchecked.defaultof<_>
    interface IFoo<int> with
        member x.Pointer = x.Pointer

let b = Broken()
let p = (b :> IFoo<int>).Pointer
printfn "Broken type loaded and Pointer accessed: %A" p
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/14508
    [<Fact>]
    let ``Issue_14508_NativeptrInInterfaceParams`` () =
        let source = """
open Microsoft.FSharp.NativeInterop

type IBar<'T when 'T : unmanaged> =
    abstract member DoSomething : nativeptr<'T> -> unit

type BarImpl() =
    interface IBar<int> with
        member _.DoSomething(_p: nativeptr<int>) = ()

let b = BarImpl()
(b :> IBar<int>).DoSomething(Unchecked.defaultof<_>)
printfn "BarImpl loaded and DoSomething called"
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/14508
    [<Fact>]
    let ``Issue_14508_NativeptrInInterfaceParamsAndReturn`` () =
        let source = """
open Microsoft.FSharp.NativeInterop

type ITransform<'T when 'T : unmanaged> =
    abstract member Transform : nativeptr<'T> -> nativeptr<'T>

type TransformImpl() =
    interface ITransform<int> with
        member _.Transform(p: nativeptr<int>) : nativeptr<int> = p

let t = TransformImpl()
let result = (t :> ITransform<int>).Transform(Unchecked.defaultof<_>)
printfn "TransformImpl loaded, result: %A" result
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/14508
    [<Fact>]
    let ``Issue_14508_NativeptrInInterfaceParamWithOtherParams`` () =
        let source = """
open Microsoft.FSharp.NativeInterop

type IProcess<'T when 'T : unmanaged> =
    abstract member Process : nativeptr<'T> * int -> unit

type ProcessImpl() =
    interface IProcess<int> with
        member _.Process((_p: nativeptr<int>), (_len: int)) = ()

let p = ProcessImpl()
(p :> IProcess<int>).Process(Unchecked.defaultof<_>, 42)
printfn "ProcessImpl loaded and Process called"
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/14508
    [<Fact>]
    let ``Issue_14508_NativeptrMultipleParams`` () =
        let source = """
open Microsoft.FSharp.NativeInterop

type ICopy<'T when 'T : unmanaged> =
    abstract member Copy : nativeptr<'T> * nativeptr<'T> * int -> unit

type CopyImpl() =
    interface ICopy<int> with
        member _.Copy((_src: nativeptr<int>), (_dst: nativeptr<int>), (_len: int)) = ()

let c = CopyImpl()
(c :> ICopy<int>).Copy(Unchecked.defaultof<_>, Unchecked.defaultof<_>, 10)
printfn "CopyImpl loaded and Copy called"
"""
        FSharp source
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/14492
    // Same-assembly variant: constrained generics inlined into closures trigger Specialize
    [<Fact>]
    let ``Issue_14492_SameAssemblyInline`` () =
        let source = """
module Test

#nowarn "3370"

let inline refEquals<'a when 'a : not struct> (a : 'a) (b : 'a) = obj.ReferenceEquals (a, b)

let inline tee f x = 
    f x
    x

let memoizeLatestRef (f: 'a -> 'b) =
    let cell = ref None
    let f' (x: 'a) =
        match !cell with
        | Some (x', value) when refEquals x' x -> value
        | _ -> f x |> tee (fun y -> cell := Some (x, y))
    f'

module BugInReleaseConfig = 
    let test f x = 
        printfn "%s" (f x)

    let f: string -> string = memoizeLatestRef id

    let run () = test f "ok"

[<EntryPoint>]
let main _ =
    BugInReleaseConfig.run ()
    0
"""
        FSharp source
        |> asExe
        |> withOptimize
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/13447
    [<FactForNETCOREAPP>]
    let ``Issue_13447_TailInstructionCorruption`` () =
        let source = """
module Test
open System
open Microsoft.FSharp.NativeInterop

#nowarn "9" // Uses of this construct may result in the generation of unverifiable .NET IL code

[<Struct>]
type MyResult<'T, 'E> = 
    | Ok of value: 'T 
    | Error of error: 'E

let useStackAlloc () : MyResult<int, string> =
    let ptr = NativePtr.stackalloc<byte> 100
    let span = Span<byte>(NativePtr.toVoidPtr ptr, 100)
    span.[0] <- 42uy
    Ok (int span.[0])

let test () =
    match useStackAlloc () with
    | Ok v -> v
    | Error _ -> -1
"""
        let actualIL =
            FSharp source
            |> asLibrary
            |> withOptimize
            |> compile
            |> shouldSucceed
            |> getActualIL

        let useStackAllocIdx = actualIL.IndexOf("useStackAlloc")
        Assert.True(useStackAllocIdx >= 0, "useStackAlloc method not found in IL")
        let methodEnd = actualIL.IndexOf("\n    } ", useStackAllocIdx)
        let methodIL = if methodEnd > 0 then actualIL.Substring(useStackAllocIdx, methodEnd - useStackAllocIdx) else actualIL.Substring(useStackAllocIdx)
        Assert.DoesNotContain("tail.", methodIL)

    // https://github.com/dotnet/fsharp/issues/11132
    [<FactForNETCOREAPP>]
    let ``Issue_11132_VoidptrDelegate`` () =
        let source = """
module Test
#nowarn "9"

open System

type MyDelegate = delegate of voidptr -> unit

let method (ptr: voidptr) = ()

let getDelegate (m: voidptr -> unit) : MyDelegate = MyDelegate(m)

let test() =
    let d = getDelegate method
    d.Invoke(IntPtr.Zero.ToPointer())

do test()
"""
        FSharp source
        |> asExe
        |> withOptimize
        |> compileAndRun
        |> shouldSucceed
        |> ignore

    // https://github.com/dotnet/fsharp/issues/14492
    // Variant: cross-assembly inlining with constrained generics triggers Specialize closure
    [<Fact>]
    let ``Issue_14492_CrossAssemblyInline`` () =
        let lib = FSharp """
module Lib

let inline refEquals<'a when 'a : not struct> (a : 'a) (b : 'a) = obj.ReferenceEquals (a, b)

let inline tee f x = 
    f x
    x
"""                     |> withOptimize
                        |> asLibrary

        let app = FSharp """
module App

open Lib

let memoizeLatestRef (f: 'a -> 'b) =
    let cell = ref None
    let f' (x: 'a) =
        match cell.Value with
        | Some (x', value) when refEquals x' x -> value
        | _ -> f x |> tee (fun y -> cell.Value <- Some (x, y))
    f'

module Test = 
    let f: string -> string = memoizeLatestRef id
    let run () = printfn "%s" (f "ok")

[<EntryPoint>]
let main _ = Test.run(); 0
"""
        app
        |> withOptimize
        |> asExe
        |> withReferences [lib]
        |> compileAndRun
        |> shouldSucceed
        |> ignore

