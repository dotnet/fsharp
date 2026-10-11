// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Every call shape an F# stack can take, compiled into the test assembly and embedded in it as text:
/// a test runs a scenario and resolves the frames the runtime reports against this very source.
///
/// Nothing a scenario runs through is `private`: a private binding never reaches the assembly
/// signature the resolver reads.
module FSharp.Editor.Tests.CodeMap.CallStackSample

open System
open System.Diagnostics
open System.IO
open System.Reflection
open System.Threading
open System.Threading.Tasks

let sourceText () =
    let assembly = Assembly.GetExecutingAssembly()

    let name =
        assembly.GetManifestResourceNames()
        |> Array.find (fun name -> name.EndsWith(__SOURCE_FILE__, StringComparison.Ordinal))

    use stream = assembly.GetManifestResourceStream name
    use reader = new StreamReader(stream)
    reader.ReadToEnd()

/// 1-based, as the line of a stack frame and of an FCS range is.
let lineIn (text: string) (snippet: string) =
    1
    + (text.Split '\n'
       |> Array.findIndex (fun line -> line.IndexOf(snippet, StringComparison.Ordinal) >= 0))

let private withoutArity (name: string) =
    match name.IndexOf '`' with
    | -1 -> name
    | i -> name.Substring(0, i)

let private inAngles (arguments: Type seq) =
    match
        arguments
        |> Seq.map (fun t -> if t.IsGenericParameter then t.Name else t.FullName)
        |> String.concat ", "
    with
    | "" -> ""
    | arguments -> $"<{arguments}>"

/// The name as reflection spells it, with `+` between nested types.
let reflectedName (method: MethodBase) =
    $"{method.DeclaringType.FullName}.{method.Name}"

/// The name as a debugger's call stack spells it, which is the spelling a Code Map receives and the
/// one no test can observe: instantiations in angle brackets, a constructor named after its type,
/// an accessor as a trailing `.get` or `.set`.
let debuggerName (method: MethodBase) =
    let rec typeName (t: Type) =
        let inherited =
            if t.IsNested then
                t.DeclaringType.GetGenericArguments().Length
            else
                0

        let name =
            withoutArity t.Name + inAngles (t.GetGenericArguments() |> Seq.skip inherited)

        match t.Namespace with
        | _ when t.IsNested -> $"{typeName t.DeclaringType}.{name}"
        | null -> name
        | ns -> $"{ns}.{name}"

    let declaringType = method.DeclaringType

    let accessor =
        declaringType.GetProperties(
            BindingFlags.Instance
            ||| BindingFlags.Static
            ||| BindingFlags.Public
            ||| BindingFlags.NonPublic
        )
        |> Seq.tryPick (fun property ->
            if method.Equals property.GetMethod then
                Some $"{property.Name}.get"
            elif method.Equals property.SetMethod then
                Some $"{property.Name}.set"
            else
                None)

    let memberName =
        match accessor, method with
        | Some accessor, _ -> accessor
        | None, :? ConstructorInfo -> withoutArity declaringType.Name
        | None, _ when method.IsGenericMethod -> method.Name + inAngles (method.GetGenericArguments())
        | None, _ -> method.Name

    $"{typeName declaringType}.{memberName}"

type CapturedFrame =
    {
        Method: MethodBase
        Line: int
        /// What a stack trace prints for the frame, from the type name to the closing parenthesis.
        Printed: string
    }

    member frame.Spellings =
        [ frame.Printed; reflectedName frame.Method; debuggerName frame.Method ]

type CapturedStack =
    {
        /// The frames a scenario put on the stack from this file, innermost first.
        Own: CapturedFrame array
        /// The FSharp.Core frames a pipeline, a computation expression or an event runs through.
        Plumbing: CapturedFrame array
    }

let private sample = typeof<CapturedStack>.DeclaringType
let private nothing = { Own = [||]; Plumbing = [||] }
let mutable private lastStack = nothing
let private gate = obj ()

/// A scenario is any parameterless function of this module that returns `int`.
let scenarios () =
    sample.GetMethods(BindingFlags.Public ||| BindingFlags.Static)
    |> Array.filter (fun method ->
        method.ReturnType = typeof<int>
        && method.GetParameters().Length = 0
        && not method.IsSpecialName)
    |> Array.map _.Name

/// Runs a scenario and hands back the stack it built - one at a time, because the capture is shared.
let run (scenario: string) =
    Monitor.Enter gate

    try
        lastStack <- nothing
        sample.GetMethod(scenario).Invoke(null, [||]) |> ignore
        lastStack
    finally
        Monitor.Exit gate

let rec private outermost (t: Type) =
    if t.IsNested then outermost t.DeclaringType else t

/// Every scenario funnels here, so one call captures exactly the stack that scenario built.
let sink (scenario: string) =
    let frames =
        StackTrace(1, true).GetFrames()
        |> Array.choose (fun frame ->
            match frame.GetMethod() with
            | null -> None
            | method ->
                match method.DeclaringType with
                | null -> None
                | declaringType ->
                    let text = StackTrace(frame).ToString()

                    let start =
                        text.IndexOf(declaringType.FullName.Replace('+', '.'), StringComparison.Ordinal)

                    Some
                        {
                            Method = method
                            Line = frame.GetFileLineNumber()
                            Printed = text.Substring(start, text.IndexOf(')', start) + 1 - start)
                        })

    lastStack <-
        {
            Own =
                frames
                |> Array.filter (fun frame -> outermost frame.Method.DeclaringType = sample && frame.Method.Name <> nameof run)
            Plumbing =
                frames
                |> Array.filter (fun frame -> frame.Method.DeclaringType.Assembly = typeof<unit>.Assembly)
        }

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

let localFunctions () =
    let helperTwo _k = sink "local functions"
    let helperOne k = helperTwo (k + 1)
    helperOne 1

let genericFunction<'T> (_value: 'T) = sink "generic function"
let genericFunctionCall () = genericFunction 42

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

/// The outer task is resumed after `let!` by the inner one completing, so its body is on the stack
/// as a continuation; `taskBody` itself, parked on `GetResult`, never is.
let taskBody () =
    let inner () =
        task {
            do! Task.Delay 1
            return sink "task body"
        }

    let outer =
        task {
            let! value = inner ()
            return value + sink "task continuation"
        }

    outer.GetAwaiter().GetResult()

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

type Callback = delegate of int -> int

let throughDelegate (callback: Callback) = callback.Invoke 1

let delegateCall () =
    throughDelegate (Callback(fun _ -> sink "delegate call"))

type Publisher() =
    let fired = Event<int>()

    [<CLIEvent>]
    member _.Fired = fired.Publish

    member _.Fire() = fired.Trigger 1

let eventHandler () =
    let publisher = Publisher()
    let mutable result = 0
    publisher.Fired.Add(fun _ -> result <- sink "event handler")
    publisher.Fire()
    result

type IRunner =
    abstract Run: string -> int

type Runner() =
    interface IRunner with
        member _.Run _name = sink "interface implementation"

/// Declared immediately before `Initialized`: a line in one of the two must be answered with the
/// type it is in, not with the nearest declaration above it.
type Box<'T>(_value: 'T) =
    member _.Unwrap() =
        // Nothing is declared on the line below, so a frame there can only be placed by its type.
        sink "generic type member"

type Initialized(seed: int) =
    /// Private, and so absent from the assembly signature.
    static let staticState = 7

    let state = sink "constructor" + seed
    member _.State = state + staticState

/// A module and a type of the same name make the compiler add the `Module` suffix.
type Collision = { Value: int }

module Collision =
    let helper () = sink "module and type collision"

module Startup =
    let initialized = 3

    /// A module-level binding runs in the method the compiler writes to initialize the file, which
    /// is the one method a debugger reports for every initializer in it.
    let initializer = MethodBase.GetCurrentMethod()

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
let moduleAndTypeCollision () = Collision.helper ()
let unionMember () = int ((Shape.Circle 1.0).Area())
let recordMember () = int ({ X = 3.0; Y = 4.0 }.Norm())
