// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace EmittedIL.RealInternalSignature

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ClassTypeInitialization =

    let withRealInternalSignature realSig compilation =
        compilation
        |> withOptions [if realSig then "--realsig+" else "--realsig-" ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Simple types in namespace`` (realSig) =

        FSharp """
namespace MyLibrary
type MyFirstType =
    static let x1 = 1100 + System.Random().Next(0)
    static let _ = printfn "Hello, World from MyLibrary.MyFirstType"

type MySecondType =
    static let x2 = 2100 + System.Random().Next(0)
    static let _ = printfn "Hello, World from MyLibrary.MySecondType"

module MyModule =
    [<EntryPoint>]
    let main args =
        printfn "Hello from main method"
        0
        """
        |> withLangVersionPreview
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Hello, World from MyLibrary.MyFirstType"
            "Hello, World from MyLibrary.MySecondType"
            "Hello from main method"
        ]


    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Simple types in implicit main`` (realSig) =

        FSharp """
type MyFirstType =
    static let x1 = 1100 + System.Random().Next(0)
    static let _ = printfn "Hello, World from MyProgram.MyFirstType"

type MySecondType =
    static let x2 = 2100 + System.Random().Next(0)
    static let _ = printfn "Hello, World from MyProgram.MySecondType"

printfn "Hello from implicit main method"
        """
        |> withLangVersionPreview
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Hello, World from MyProgram.MyFirstType"
            "Hello, World from MyProgram.MySecondType"
            "Hello from implicit main method"
        ]


    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Simple TypeOne and TypeTwo in namespace`` (realSig) =

        FSharp """
namespace MyLibrary
type MyFirstType =
    static let x1 = 1100 + System.Random().Next(0)
    static let _ = printfn "Hello, World from MyLibrary.MyFirstType"

and MySecondType =
    static let x2 = 2100 + System.Random().Next(0)
    static let _ = printfn "Hello, World from MyLibrary.MySecondType"

module MyModule =
    [<EntryPoint>]
    let main args =
        printfn "Hello from main method"
        0
        """
        |> withLangVersionPreview
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> withStdOutContainsAllInOrder [
            "Hello, World from MyLibrary.MyFirstType"
            "Hello, World from MyLibrary.MySecondType"
            "Hello from main method"
        ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Public type - public ctor`` (realSig) =

        FSharp """
namespace FSharp.Compiler.CodeAnalysis

open System
open System.IO


[<AbstractClass>]
type FSharpSource () =
    abstract FilePath: string

type FSharpSourceFromFile public (filePath: string) =
    inherit FSharpSource()

    override _.FilePath = filePath

type FSharpSource with

    static member CreateFromFile (filePath: string) =
        ()  //FSharpSourceFromFile(filePath) :> FSharpSource

module doit =
    FSharpSource.CreateFromFile("Hello") |> ignore
    printfn "Main program"
        """
        |> withLangVersionPreview
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Main program"
        ]


    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Public type - internal ctor`` (realSig) =

        FSharp """
namespace FSharp.Compiler.CodeAnalysis

open System
open System.IO


[<AbstractClass>]
type FSharpSource () =
    abstract FilePath: string

type FSharpSourceFromFile internal (filePath: string) =
    inherit FSharpSource()

    override _.FilePath = filePath

type FSharpSource with

    static member CreateFromFile(filePath: string) =
        FSharpSourceFromFile(filePath) :> FSharpSource

module doit =
    FSharpSource.CreateFromFile("Hello") |> ignore
    printfn "Main program"
        """
        |> withLangVersionPreview
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Main program"
        ]


    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Public type - private ctor`` (realSig) =

        FSharp """
namespace FSharp.Compiler.CodeAnalysis

open System
open System.IO


[<AbstractClass>]
type FSharpSource () =
    abstract FilePath: string

type FSharpSourceFromFile private (filePath: string) =
    inherit FSharpSource()

    override _.FilePath = filePath

type FSharpSource with

    static member CreateFromFile (filePath: string) =
        ()  //FSharpSourceFromFile(filePath) :> FSharpSource

module doit =
    FSharpSource.CreateFromFile("Hello") |> ignore
    printfn "Main program"
        """
        |> withLangVersionPreview
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Main program"
        ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Public type - unspecified ctor`` (realSig) =

        FSharp """
namespace FSharp.Compiler.CodeAnalysis

open System
open System.IO


[<AbstractClass>]
type FSharpSource () =
    abstract FilePath: string

type FSharpSourceFromFile (filePath: string) =
    inherit FSharpSource()

    override _.FilePath = filePath

type FSharpSource with

    static member CreateFromFile (filePath: string) =
        ()  //FSharpSourceFromFile(filePath) :> FSharpSource

module doit =
    FSharpSource.CreateFromFile("Hello") |> ignore
    printfn "Main program"
        """
        |> withLangVersionPreview
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Main program"
        ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Private type - public ctor`` (realSig) =

        FSharp """
namespace FSharp.Compiler.CodeAnalysis

open System
open System.IO


[<AbstractClass>]
type FSharpSource () =
    abstract FilePath: string

type private FSharpSourceFromFile public (filePath: string) =
    inherit FSharpSource()

    override _.FilePath = filePath

type FSharpSource with

    static member CreateFromFile (filePath: string) =
        ()  //FSharpSourceFromFile(filePath) :> FSharpSource

module doit =
    FSharpSource.CreateFromFile("Hello") |> ignore
    printfn "Main program"
        """
        |> withLangVersionPreview
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Main program"
        ]


    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Private type - internal ctor`` (realSig) =

        FSharp """
namespace FSharp.Compiler.CodeAnalysis

open System
open System.IO


[<AbstractClass>]
type FSharpSource () =
    abstract FilePath: string

type private FSharpSourceFromFile internal (filePath: string) =
    inherit FSharpSource()

    override _.FilePath = filePath

type FSharpSource with

    static member CreateFromFile(filePath: string) =
        FSharpSourceFromFile(filePath) :> FSharpSource

module doit =
    FSharpSource.CreateFromFile("Hello") |> ignore
    printfn "Main program"
        """
        |> withLangVersionPreview
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> withStdOutContainsAllInOrder [
            "Main program"
        ]
        |> shouldSucceed

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Private type - private ctor`` (realSig) =

        FSharp """
namespace FSharp.Compiler.CodeAnalysis

open System
open System.IO


[<AbstractClass>]
type public FSharpSourcePublic () =
    abstract PublicFilePath: string

[<AbstractClass>]
type internal FSharpSource () =
    inherit FSharpSourcePublic()
    abstract InternalFilePath: string

type private FSharpSourceFromFile private (filePath: string) =
    inherit FSharpSource()

    override _.PublicFilePath = filePath
    override _.InternalFilePath = filePath
    member   _.PrivateFilePath = filePath

type FSharpSource with

    static member public PublicCreateFromFile (filePath: string) = ()
    static member internal InternalCreateFromFile (filePath: string) = ()
    static member private PrivateCreateFromFile (filePath: string) = ()

module doit =
    printfn "Main program"
        """
        |> withLangVersionPreview
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Main program"
        ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Private type - unspecified ctor`` (realSig) =

        FSharp """
namespace FSharp.Compiler.CodeAnalysis

open System
open System.IO


[<AbstractClass>]
type FSharpSource () =
    abstract FilePath: string

type private FSharpSourceFromFile (filePath: string) =
    inherit FSharpSource()

    override _.FilePath = filePath

type FSharpSource with

    static member CreateFromFile (filePath: string) =
        FSharpSourceFromFile(filePath) :> FSharpSource

module doit =
    FSharpSource.CreateFromFile("Hello") |> ignore
    printfn "Main program"
        """
        |> withLangVersionPreview
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Main program"
        ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Static Initialization - don't inline when method includes a private field`` (realSig) =

        FSharp """
module FSharp.Compiler.CodeAnalysis

open System
open System.IO

[<AbstractClass>]
type FSharpSource () =
    abstract FilePath: string

type public FSharpSourceFromFile private (filePath: string) =
    inherit FSharpSource()

    static let mutable someValue = "not set yet"

    override _.FilePath = filePath

    static member private SetSomeValue (value: string): string =
        someValue <- value
        someValue

    static member public SetIt (value: string) : string =
        FSharpSourceFromFile.SetSomeValue (value)

let message = FSharpSourceFromFile.SetIt ("Here is something")

printfn $"{message}"
        """
        |> withLangVersionPreview
        |> withRealInternalSignature realSig
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Here is something"
        ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Computation expression - access private binding`` (realSig) =

        FSharp """
module MyMaybeStuff =
    type MaybeBuilder() =
        member this.Bind(x, f) =
            match x with
            | Some v -> f v
            | None -> None
        member this.Return(x) = Some x

    let maybe = new MaybeBuilder()

open MyMaybeStuff
type MyClass =
    static member private privateThing(someValue) = someValue

    static member result () =
        maybe {
            let mutable someValue = 0
            let! u = someValue <- 1; Some (MyClass.privateThing(someValue))
            let! v = someValue <- 2; Some (MyClass.privateThing(someValue))
            return u + v
        }

printfn "%A" (MyClass.result())
"""
        |> withLangVersionPreview
        |> withRealInternalSignature realSig
        |> withNoOptimize
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContainsAllInOrder [
            "Some 3"
        ]


    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``nested generic closure`` (realSig) =

        FSharp """
// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace MyCollections

#nowarn "52" // The value has been copied to ensure the original is not mutated by this operation

open System.Diagnostics
open System.Collections
open System.Collections.Generic
open Microsoft.FSharp.Core

module internal IEnumerator =

    let noReset() = raise (new System.NotSupportedException("SR.GetString(SR.resetNotSupported)"))
    let notStarted() = raise (new System.InvalidOperationException("SR.GetString(SR.enumerationNotStarted)"))
    let alreadyFinished() = raise (new System.InvalidOperationException("SR.GetString(SR.enumerationAlreadyFinished)"))
    let check started = if not started then notStarted()
    let dispose (r : System.IDisposable) = r.Dispose()

    let cast (e : IEnumerator) : IEnumerator<'T> =
        { new IEnumerator<'T> with
              member _.Current = unbox<'T> e.Current

          interface IEnumerator with
              member _.Current = unbox<'T> e.Current :> obj

              [<DebuggerStepThrough>]
              member _.MoveNext() = e.MoveNext()

              member _.Reset() = noReset()

          interface System.IDisposable with
              member _.Dispose() =
                  match e with
                  | :? System.IDisposable as e -> e.Dispose()
                  | _ -> ()   }

    /// A concrete implementation of an enumerator that returns no values
    [<Sealed>]
    type EmptyEnumerator<'T>() =
        let mutable started = false
        interface IEnumerator<'T> with
            member _.Current =
                check started
                (alreadyFinished() : 'T)

        interface System.Collections.IEnumerator with
            member _.Current =
                check started
                (alreadyFinished() : obj)

            [<DebuggerStepThrough>]
            member _.MoveNext() =
                if not started then started <- true
                false

            member _.Reset() = noReset()

        interface System.IDisposable with
             member _.Dispose() = ()
            
    let Empty<'T> () = (new EmptyEnumerator<'T>() :> IEnumerator<'T>)

    [<NoEquality; NoComparison>]
    type EmptyEnumerable<'T> =

        | EmptyEnumerable

        interface IEnumerable<'T> with
            member _.GetEnumerator() = Empty<'T>()

        interface IEnumerable with
            member _.GetEnumerator() = (Empty<'T>() :> IEnumerator)

    type GeneratedEnumerable<'T, 'State>(openf: unit -> 'State, compute: 'State -> 'T option, closef: 'State -> unit) =
        let mutable started = false
        let mutable curr = None
        let state = ref (Some (openf ()))
        let getCurr() : 'T =
            check started
            match curr with
            | None -> alreadyFinished()
            | Some x -> x

        let readAndClear () =
            lock state (fun () ->
                match state.Value with
                | None -> None
                | Some _ as res ->
                    state.Value <- None
                    res)

        let start() =
            if not started then
                started <- true

        let dispose() =
            readAndClear() |> Option.iter closef

        let finish() =
            try dispose()
            finally curr <- None

        interface IEnumerator<'T> with
            member _.Current = getCurr()

        interface IEnumerator with
            member _.Current = box (getCurr())

            [<DebuggerStepThrough>]
            member _.MoveNext() =
                start()
                match state.Value with
                | None -> false // we started, then reached the end, then got another MoveNext
                | Some s ->
                    match (try compute s with e -> finish(); reraise()) with
                    | None -> finish(); false
                    | Some _ as x ->
                        curr <- x
                        true

            member _.Reset() = noReset()

        interface System.IDisposable with
             member _.Dispose() = dispose()

    [<Sealed>]
    type Singleton<'T>(v:'T) =
        let mutable started = false

        interface IEnumerator<'T> with
             member _.Current = v

        interface IEnumerator with
            member _.Current = box v

            [<DebuggerStepThrough>]
            member _.MoveNext() = if started then false else (started <- true; true)

            member _.Reset() = noReset()

        interface System.IDisposable with
            member _.Dispose() = ()

    let Singleton x = (new Singleton<'T>(x) :> IEnumerator<'T>)

    let EnumerateThenFinally f (e : IEnumerator<'T>) =
        { new IEnumerator<'T> with
             member _.Current = e.Current

          interface IEnumerator with
              member _.Current = (e :> IEnumerator).Current

              [<DebuggerStepThrough>]
              member _.MoveNext() = e.MoveNext()

              member _.Reset() = noReset()

          interface System.IDisposable with
              member _.Dispose() =
                  try
                      e.Dispose()
                  finally
                      f()
        }

    let inline checkNonNull argName arg =
        if isNull arg then
            nullArg argName

    let mkSeq f =
        { new IEnumerable<'U> with
            member _.GetEnumerator() = f()

          interface IEnumerable with
            member _.GetEnumerator() = (f() :> IEnumerator) }

namespace Microsoft.FSharp.Core.CompilerServices

    open System.Diagnostics
    open Microsoft.FSharp.Core
    open MyCollections
    open MyCollections.IEnumerator
    open System.Collections
    open System.Collections.Generic

    module RuntimeHelpers =

        [<Struct; NoComparison; NoEquality>]
        type internal StructBox<'T when 'T:equality>(value:'T) =
            member x.Value = value

            static member Comparer =
                let gcomparer = HashIdentity.Structural<'T>
                { new IEqualityComparer<StructBox<'T>> with
                       member _.GetHashCode(v) = gcomparer.GetHashCode(v.Value)
                       member _.Equals(v1,v2) = gcomparer.Equals(v1.Value,v2.Value) }

        let Generate openf compute closef =
            mkSeq (fun () -> new IEnumerator.GeneratedEnumerable<_,_>(openf, compute, closef) :> IEnumerator<'T>)

        let EnumerateFromFunctions create moveNext current =
            Generate
                create
                (fun x -> if moveNext x then Some(current x) else None)
                (fun x -> match box(x) with :? System.IDisposable as id -> id.Dispose() | _ -> ())

        // A family of enumerators that can have additional 'finally' actions added to the enumerator through
        // the use of mutation. This is used to 'push' the disposal action for a 'use' into the next enumerator.
        // For example,
        //    seq { use x = ...
        //          while ... }
        // results in the 'while' loop giving an adjustable enumerator. This is then adjusted by adding the disposal action
        // from the 'use' into the enumerator. This means that we avoid constructing a two-deep enumerator chain in this
        // common case.
        type IFinallyEnumerator =
            abstract AppendFinallyAction : (unit -> unit) -> unit

        /// A concrete implementation of IEnumerable that adds the given compensation to the "Dispose" chain of any
        /// enumerators returned by the enumerable.
        [<Sealed>]
        type FinallyEnumerable<'T>(compensation: unit -> unit, restf: unit -> seq<'T>) =
            interface IEnumerable<'T> with
                member _.GetEnumerator() =
                    try
                        let ie = restf().GetEnumerator()
                        match ie with
                        | :? IFinallyEnumerator as a ->
                            a.AppendFinallyAction(compensation)
                            ie
                        | _ ->
                            IEnumerator.EnumerateThenFinally compensation ie
                    with e ->
                        compensation()
                        reraise()
            interface IEnumerable with
                member x.GetEnumerator() = ((x :> IEnumerable<'T>).GetEnumerator() :> IEnumerator)

        /// An optimized object for concatenating a sequence of enumerables
        [<Sealed>]
        type ConcatEnumerator<'T,'U when 'U :> seq<'T>>(sources: seq<'U>) =
            let mutable outerEnum = sources.GetEnumerator()
            let mutable currInnerEnum = IEnumerator.Empty()

            let mutable started = false
            let mutable finished = false
            let mutable compensations = []

            [<DefaultValue(false)>] // false = unchecked
            val mutable private currElement : 'T

            member _.Finish() =
                finished <- true
                try
                    match currInnerEnum with
                    | null -> ()
                    | _ ->
                        try
                            currInnerEnum.Dispose()
                        finally
                            currInnerEnum <- null
                finally
                    try
                        match outerEnum with
                        | null -> ()
                        | _ ->
                            try
                                outerEnum.Dispose()
                            finally
                                outerEnum <- null
                    finally
                        let rec iter comps =
                            match comps with
                            |   [] -> ()
                            |   h :: t ->
                                    try h() finally iter t
                        try
                            compensations |> List.rev |> iter
                        finally
                            compensations <- []

            member x.GetCurrent() =
                IEnumerator.check started
                if finished then IEnumerator.alreadyFinished() else x.currElement

            interface IFinallyEnumerator with
                member _.AppendFinallyAction(f) =
                    compensations <- f :: compensations

            interface IEnumerator<'T> with
                member x.Current = x.GetCurrent()

            interface IEnumerator with
                member x.Current = box (x.GetCurrent())

                [<DebuggerStepThrough>]
                member x.MoveNext() =
                   if not started then started <- true
                   if finished then false
                   else
                      let rec takeInner () =
                        // check the inner list
                        if currInnerEnum.MoveNext() then
                            x.currElement <- currInnerEnum.Current
                            true
                        else
                            // check the outer list
                            let rec takeOuter() =
                                if outerEnum.MoveNext() then
                                    let ie = outerEnum.Current
                                    // Optimization to detect the statically-allocated empty IEnumerables
                                    match box ie with
                                    | :? EmptyEnumerable<'T> ->
                                         // This one is empty, just skip, don't call GetEnumerator, try again
                                         takeOuter()
                                    | _ ->
                                         // OK, this one may not be empty.
                                         // Don't forget to dispose of the enumerator for the inner list now we're done with it
                                         currInnerEnum.Dispose()
                                         currInnerEnum <- ie.GetEnumerator()
                                         takeInner ()
                                else
                                    // We're done
                                    x.Finish()
                                    false
                            takeOuter()
                      takeInner ()

                member _.Reset() = IEnumerator.noReset()

            interface System.IDisposable with

                [<DebuggerStepThrough>]
                member x.Dispose() =
                    if not finished then
                        x.Finish()

    module doIt =
        open System
        open Microsoft.FSharp.Core.CompilerServices
        open Microsoft.FSharp.Collections

        let x = seq { ArraySegment([|1uy; 2uy; 3uy|]); ArraySegment([|1uy; 2uy; 3uy|]); ArraySegment([|1uy; 2uy; 3uy|]); }
        let enumerator = new RuntimeHelpers.ConcatEnumerator<_,_>(x) :> IEnumerator
        let rec loop () =
            if enumerator.MoveNext() then
                printfn $"{enumerator.Current}"
                loop ()
        loop ()
"""
        |> asExe
        |> withLangVersionPreview
        |> withRealInternalSignature realSig
        |> withOptimize
        |> compileAndRun
        |> shouldSucceed
