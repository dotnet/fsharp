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
        |> withRealInternalSignature realSig
        |> withOptimize
        |> compileAndRun
        |> shouldSucceed

    //[<InlineData(true, true)>]          // RealSig Optimize
    [<InlineData(true, false)>]         // RealSig NoOptimize
    [<InlineData(false, true)>]         // Regular Optimize
    [<InlineData(false, false)>]        // Regular NoOptimize
    [<Theory>]
    let ``Generic class with closure with constraints`` (realSig, optimize) =
        let withOptimization compilation =
            if optimize then compilation |> withOptimize
            else compilation |> withNoOptimize

        FSharp """
namespace Test
open System

module RuntimeHelpers =
    [<Sealed>]
    type MyType<'A,'B when 'B :> seq<'A>>(sources: seq<'B>) =

        member x.MoveNext() =
            let rec takeInner c =
                let rec takeOuter b =
                    if b.ToString () = "1" then failwith "Oops"
                    if sources |> Seq.length > 10 then
                        sources |> Seq.skip 10
                    else
                        sources
                if c.ToString() = "1" then failwith "Oops"
                if sources |> Seq.length < 5 then
                    sources
                else
                    takeOuter 7
            takeInner 3

open RuntimeHelpers
module doIt =
    let x = seq {  ArraySegment([|1uy; 2uy; 3uy|]); ArraySegment([|1uy; 2uy; 3uy|]); ArraySegment([|1uy; 2uy; 3uy|]); }
    let enumerator = x |> MyType<_,_>
    for i in enumerator.MoveNext() do
        printfn "%A" i
    """
        |> asExe
        |> withRealInternalSignature realSig
        |> withOptimization
        |> compileAndRun
        |> shouldSucceed

    //[<InlineData(true, true)>]          // RealSig Optimize
    [<InlineData(true, false)>]         // RealSig NoOptimize
    [<InlineData(false, true)>]         // Regular Optimize
    [<InlineData(false, false)>]        // Regular NoOptimize
    [<Theory>]
    let ``AgedLookup `` (realSig, optimize) =
        let withOptimization compilation =
            if optimize then compilation |> withOptimize
            else compilation |> withNoOptimize

        FSharp """
namespace Internal.Utilities.Collections

open System

[<StructuralEquality; NoComparison>]
type internal ValueStrength<'T when 'T: not struct> =
    | Strong of 'T
    | Weak of WeakReference<'T>

type internal AgedLookup<'Token, 'Key, 'Value when 'Value: not struct>(keepStrongly: int, areSimilar, ?requiredToKeep, ?keepMax: int) =
    /// The list of items stored. Youngest is at the end of the list.
    /// The choice of order is somewhat arbitrary. If the other way then adding
    /// items would be O(1) and removing O(N).
    let mutable refs: ('Key * ValueStrength<'Value>) list = []

    let mutable keepStrongly = keepStrongly

    // The 75 here determines how long the list should be passed the end of strongly held
    // references. Some operations are O(N) and we don't want to let things get out of
    // hand.
    let keepMax = defaultArg keepMax 75
    let mutable keepMax = max keepStrongly keepMax
    let requiredToKeep = defaultArg requiredToKeep (fun _ -> false)

    /// Look up the given key, return <c>None</c> if not found.
    let TryPeekKeyValueImpl (data, key) =
        let rec Lookup key =
            function
            // Treat a list of key-value pairs as a lookup collection.
            // This function returns true if two keys are the same according to the predicate
            // function passed in.
            | [] -> None
            | (similarKey, value) :: t ->
                if areSimilar (key, similarKey) then
                    Some(similarKey, value)
                else
                    Lookup key t

        Lookup key data

    /// Determines whether a particular key exists.
    let Exists (data, key) = TryPeekKeyValueImpl(data, key).IsSome

    /// Set a particular key's value.
    let Add (data, key, value) = data @ [ key, value ]

    /// Promote a particular key value.
    let Promote (data, key, value) =
        (data |> List.filter (fun (similarKey, _) -> not (areSimilar (key, similarKey))))
        @ [ (key, value) ]

    /// Remove a particular key value.
    let RemoveImpl (data, key) =
        let keep =
            data |> List.filter (fun (similarKey, _) -> not (areSimilar (key, similarKey)))

        keep

    let TryGetKeyValueImpl (data, key) =
        match TryPeekKeyValueImpl(data, key) with
        | Some(similarKey, value) as result ->
            // If the result existed, move it to the end of the list (more likely to keep it)
            result, Promote(data, similarKey, value)
        | None -> None, data

    /// Remove weak entries from the list that have been collected.
    let FilterAndHold (tok: 'Token) =
        ignore tok // reading 'refs' requires a token

        [
            for key, value in refs do
                match value with
                | Strong(value) -> yield (key, value)
                | Weak(weakReference) ->
                    match weakReference.TryGetTarget() with
                    | false, _ -> ()
                    | true, value -> yield key, value
        ]

    let AssignWithStrength (tok, newData) =
        let actualLength = List.length newData
        let tossThreshold = max 0 (actualLength - keepMax) // Delete everything less than this threshold
        let weakThreshold = max 0 (actualLength - keepStrongly) // Weaken everything less than this threshold

        let newData = newData |> List.mapi (fun n kv -> n, kv) // Place the index.

        let newData =
            newData
            |> List.filter (fun (n: int, v) -> n >= tossThreshold || requiredToKeep (snd v))

        let newData =
            newData
            |> List.map (fun (n: int, (k, v)) ->
                let handle =
                    if n < weakThreshold && not (requiredToKeep v) then
                        Weak(WeakReference<_>(v))
                    else
                        Strong(v)

                k, handle)

        ignore tok // Updating refs requires tok
        refs <- newData

    member al.TryPeekKeyValue(tok, key) =
        // Returns the original key value as well since it may be different depending on equality test.
        let data = FilterAndHold(tok)
        TryPeekKeyValueImpl(data, key)

    member al.TryGetKeyValue(tok, key) =
        let data = FilterAndHold(tok)
        let result, newData = TryGetKeyValueImpl(data, key)
        AssignWithStrength(tok, newData)
        result

    member al.TryGet(tok, key) =
        let data = FilterAndHold(tok)
        let result, newData = TryGetKeyValueImpl(data, key)
        AssignWithStrength(tok, newData)

        match result with
        | Some(_, value) -> Some(value)
        | None -> None

    member al.Put(tok, key, value) =
        let data = FilterAndHold(tok)

        let data = if Exists(data, key) then RemoveImpl(data, key) else data

        let data = Add(data, key, value)
        AssignWithStrength(tok, data) // This will remove extras

    member al.Remove(tok, key) =
        let data = FilterAndHold(tok)
        let newData = RemoveImpl(data, key)
        AssignWithStrength(tok, newData)

    member al.Clear(tok) =
        let _discards = FilterAndHold(tok)
        AssignWithStrength(tok, [])

    member al.Resize(tok, newKeepStrongly, ?newKeepMax) =
        let newKeepMax = defaultArg newKeepMax 75
        keepStrongly <- newKeepStrongly
        keepMax <- max newKeepStrongly newKeepMax
        let keep = FilterAndHold(tok)
        AssignWithStrength(tok, keep)
    """
        |> asLibrary
        |> withRealInternalSignature realSig
        |> withOptimization
        |> compile
        |> shouldSucceed

    [<InlineData(true, true)>]          // RealSig Optimize
    [<InlineData(true, false)>]         // RealSig NoOptimize
    [<InlineData(false, true)>]         // Regular Optimize
    [<InlineData(false, false)>]        // Regular NoOptimize
    [<Theory>]
    let ``BigTuples`` (realSig, optimize) =
        let withOptimization compilation =
            if optimize then compilation |> withOptimize
            else compilation |> withNoOptimize

        FSharp """
namespace Equality

type SmallNonGenericTuple = SmallNonGenericTuple of int * string

type SmallGenericTuple<'a> = SmallGenericTuple of int * 'a

type BigNonGenericTuple = BigNonGenericTuple of int * string * byte * int * string * byte

type BigGenericTuple<'a> = BigGenericTuple of int * 'a * byte * int * 'a * byte

[<Struct>]
type SmallNonGenericTupleStruct = SmallNonGenericTupleStruct of int * string

[<Struct>]
type SmallGenericTupleStruct<'a> = SmallGenericTupleStruct of int * 'a

[<Struct>]
type BigNonGenericTupleStruct = BigNonGenericTupleStruct of int * string * byte * int * string * byte

[<Struct>]
type BigGenericTupleStruct<'a> = BigGenericTupleStruct of int * 'a * byte * int * 'a * byte

type ReferenceTuples() =

    let numbers = Array.init 1000 id

    member _.SmallNonGenericTuple() =
        numbers
        |> Array.countBy (fun n -> SmallNonGenericTuple(n, string n))

    member _.SmallGenericTuple() =
        numbers
        |> Array.countBy (fun n -> SmallGenericTuple(n, string n))

    member _.BigNonGenericTuple() =
        numbers
        |> Array.countBy (fun n -> BigNonGenericTuple(n, string n, byte n, n, string n, byte n))

    member _.BigGenericTuple() =
        numbers
        |> Array.countBy (fun n -> BigGenericTuple(n, string n, byte n, n, string n, byte n))

    member _.SmallNonGenericTupleStruct() =
        numbers
        |> Array.countBy (fun n -> SmallNonGenericTupleStruct(n, string n))

    member _.SmallGenericTupleStruct() =
        numbers
        |> Array.countBy (fun n -> SmallGenericTupleStruct(n, string n))

    member _.BigNonGenericTupleStruct() =
        numbers
        |> Array.countBy (fun n -> BigNonGenericTupleStruct(n, string n, byte n, n, string n, byte n))

    member _.BigGenericTupleStruct() =
        numbers
        |> Array.countBy (fun n -> BigGenericTupleStruct(n, string n, byte n, n, string n, byte n))
    """
        |> asLibrary
        |> withRealInternalSignature realSig
        |> withOptimization
        |> compile
        |> shouldSucceed
