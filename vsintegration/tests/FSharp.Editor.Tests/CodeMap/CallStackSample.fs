// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Every call shape a debugger stack can take in F#, in one compiled file. The tests run a scenario,
/// take the frame names the runtime reports for it, and put those through the frame parser and the
/// call stack resolver - so the corpus is observed rather than written by hand.
///
/// Nothing here is `private`: a private binding never reaches the assembly signature the resolver
/// reads, and every frame in this file is meant to resolve.
module FSharp.Editor.Tests.CodeMap.CallStackSample

open System
open System.Diagnostics
open System.IO
open System.Reflection
open System.Threading.Tasks

/// This file's own text, embedded beside the compiled copy, so a test can hand a workspace exactly
/// the source the captured frames were compiled from.
let sourceText () =
    let assembly = Assembly.GetExecutingAssembly()

    let name =
        assembly.GetManifestResourceNames()
        |> Array.find (fun name -> name.EndsWith("CallStackSample.fs", StringComparison.Ordinal))

    use stream = assembly.GetManifestResourceStream name
    use reader = new StreamReader(stream)
    reader.ReadToEnd()

/// This module's own compiled type. Everything the scenarios build - closures, state machines, the
/// types declared below - is nested inside it, which is what tells its frames apart from the test
/// runner's.
[<Literal>]
let private SampleType = "FSharp.Editor.Tests.CodeMap.CallStackSample"

let mutable private lastFrames: struct (string * int) array = [||]

/// The frames of the scenario that ran last, innermost first: the name the runtime reports and the
/// line it sits on, which is what a debug engine hands the provider.
let frames () = lastFrames

/// Every scenario funnels here, so one call captures exactly the stack that scenario built. Frames
/// from outside this file - the test runner, FSharp.Core's own plumbing - are dropped, leaving the
/// shapes the provider has to resolve.
let sink (scenario: string) =
    lastFrames <-
        StackTrace(true).GetFrames()
        |> Array.choose (fun frame ->
            match frame.GetMethod() with
            | null -> None
            | method ->
                match method.DeclaringType with
                | null -> None
                | declaringType ->
                    match declaringType.FullName with
                    | null -> None
                    | fullName when
                        fullName = SampleType
                        || fullName.StartsWith(SampleType + "+", StringComparison.Ordinal)
                        ->
                        Some(struct ($"{fullName}.{method.Name}", frame.GetFileLineNumber()))
                    | _ -> None)

    scenario.Length

let moduleLevelThree (_a: int, _b: string) = sink "module functions"
let moduleLevelTwo (a: int) (b: string) (_c: float) = moduleLevelThree (a, b)
let moduleFunctions () = moduleLevelTwo 1 "x" 2.0

let pipelineLambdas () =
    [ 1 ]
    |> List.map (fun x -> x + 1)
    |> List.collect (fun x -> [ x ])
    |> List.map (fun _x -> sink "pipeline lambdas")
    |> List.sum

let nestedClosures () =
    let outer factor =
        let middle scale =
            let inner () = sink "nested closures"
            inner () * scale

        middle 2 * factor

    outer 3

let localFunctions (n: int) =
    let helperTwo _k = sink "local functions"
    let helperOne k = helperTwo (k + 1)
    helperOne n

let genericFunction<'T> (_value: 'T) = sink "generic function"

let (>=>) a b =
    sink "custom operator" |> ignore
    a + b

let customOperator () = 1 >=> 2

[<CompiledName("RenamedInMetadata")>]
let originalSourceName () = sink "CompiledName rename"

let (|Even|Odd|) n =
    sink "active pattern" |> ignore
    if n % 2 = 0 then Even else Odd

let activePattern () =
    match 4 with
    | Even -> 0
    | Odd -> 1

let (|Positive|_|) n =
    sink "partial active pattern" |> ignore
    if n > 0 then Some n else None

let partialActivePattern () =
    match 3 with
    | Positive _ -> 0
    | _ -> 1

let rec countdown n =
    if n <= 0 then sink "recursion" else countdown (n - 1)

let recursion () = countdown 5

let rec isEven n =
    if n = 0 then sink "mutual recursion" else isOdd (n - 1)

and isOdd n =
    if n = 0 then sink "mutual recursion" else isEven (n - 1)

let mutualRecursion () = isEven 4

let applyTwice (f: int -> int) x = f (f x)

let higherOrder () =
    applyTwice (fun _x -> sink "higher order") 1

let asyncBody () =
    async {
        do! Async.Sleep 1
        return sink "async body"
    }
    |> Async.RunSynchronously

let taskBody () =
    let work =
        task {
            do! Task.Delay 1
            return sink "task body"
        }

    work.GetAwaiter().GetResult()

let seqBody () =
    seq {
        yield sink "seq body"
        yield 0
    }
    |> Seq.head

module Outer =
    module Middle =
        module Inner =
            let deeplyNested () = sink "nested modules"

let nestedModules () = Outer.Middle.Inner.deeplyNested ()

type Worker(seed: int) =
    let mutable state = seed

    member this.Start() = this.CurriedStep 1 2
    member _.CurriedStep (_a: int) (_b: int) = sink "class members"

    member _.Computed =
        state <- state + 1
        sink "property getter"

    member _.Tuned
        with get () = state
        and set value =
            sink "property setter" |> ignore
            state <- value

    static member StaticEntry() = sink "static member"

type IRunner =
    abstract Run: string -> int

type Runner() =
    interface IRunner with
        member _.Run _name = sink "interface implementation"

type Box<'T>(_value: 'T) =
    member _.Unwrap() = sink "generic type member"

type Initialized(seed: int) =
    /// Private, and so absent from the assembly signature: a frame landing on this line can only be
    /// named after the type whose static constructor runs it.
    static let staticState = 7

    let state = sink "constructor" + seed
    member _.State = state + staticState

/// A module and a type of the same name, which is what makes the compiler add the `Module` suffix
/// the resolver has to undo.
type Collision = { Value: int }

module Collision =
    let helper () = sink "module and type collision"

module Startup =
    let initialized = 3

type Shape =
    | Circle of radius: float
    | Rect of width: float * height: float

    member this.Area() =
        sink "union member" |> ignore

        match this with
        | Circle r -> Math.PI * r * r
        | Rect(w, h) -> w * h

type Point =
    {
        X: float
        Y: float
    }

    member this.Norm() =
        sink "record member" |> ignore
        sqrt (this.X * this.X + this.Y * this.Y)

/// The zero-argument entry points the tests call, so a scenario is one function to invoke.
let worker = Worker 10

let instanceMember () = worker.Start()
let propertyGetter () = worker.Computed

let propertySetter () =
    worker.Tuned <- 5
    worker.Tuned

let staticMember () = Worker.StaticEntry()
let interfaceImplementation () = (Runner() :> IRunner).Run "x"
let genericTypeMember () = Box<string>("s").Unwrap()
let constructors () = Initialized(1).State
let unionMember () = int ((Shape.Circle 1.0).Area())
let recordMember () = int ({ X = 3.0; Y = 4.0 }.Norm())
let genericFunctionScenario () = genericFunction 42
