namespace ClassLibrary

open System
open System.Threading.Tasks

/// A delegate reaches the lambda through `Invoke` on a generated type, so the frame for the body is
/// a closure rather than anything named in source.
type Callback = delegate of int -> int

/// Call shapes for exercising "Show Call Stack on Code Map".
/// Every scenario funnels into `Demo.sink`, so a single breakpoint there is hit once per scenario
/// with a differently shaped stack each time.
module Demo =

    /// PUT THE BREAKPOINT HERE.
    let sink (scenario: string) =
        printfn "scenario: %s" scenario
        scenario.Length

    // ------------------------------------------------------------ plain module functions
    let private moduleLevelThree (a: int, b: string) = sink "module functions"
    let private moduleLevelTwo (a: int) (b: string) (c: float) = moduleLevelThree (a, b)
    let moduleFunctions () = moduleLevelTwo 1 "x" 2.0

    // ------------------------------------------------------------ lambdas in a pipeline
    let pipelineLambdas () =
        [ 1 ]
        |> List.map (fun x -> x + 1)
        |> List.collect (fun x -> [ x ])
        |> List.map (fun x -> sink "pipeline lambdas")
        |> List.sum

    // ------------------------------------------------------------ nested closures
    let nestedClosures () =
        let outer factor =
            let middle scale =
                let inner () = sink "nested closures"
                inner () * scale

            middle 2 * factor

        outer 3

    // ------------------------------------------------------------ local functions
    let localFunctions (n: int) =
        let helperTwo k = sink "local functions"
        let helperOne k = helperTwo (k + 1)
        helperOne n

    // ------------------------------------------------------------ generics, operators, renames
    let genericFunction<'T> (value: 'T) = sink "generic function"

    let (>=>) a b =
        sink "custom operator" |> ignore
        a + b

    let customOperator () = 1 >=> 2

    [<CompiledName("RenamedInMetadata")>]
    let originalSourceName () = sink "CompiledName rename"

    // ------------------------------------------------------------ active patterns
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

    // ------------------------------------------------------------ recursion
    let rec private countdown n =
        if n <= 0 then sink "recursion" else countdown (n - 1)

    let recursion () = countdown 5

    let rec private isEven n =
        if n = 0 then sink "mutual recursion" else isOdd (n - 1)

    and private isOdd n =
        if n = 0 then sink "mutual recursion" else isEven (n - 1)

    let mutualRecursion () = isEven 4

    // ------------------------------------------------------------ higher-order functions
    let private applyTwice (f: int -> int) x = f (f x)
    let higherOrder () = applyTwice (fun x -> sink "higher order") 1

    // ------------------------------------------------------------ computation expressions
    let asyncBody () =
        async {
            do! Async.Sleep 1
            return sink "async body"
        }
        |> Async.RunSynchronously

    /// Two levels on purpose. The debugger builds its logical async stack by walking continuations,
    /// so the inner body is reached from the outer one through `let!` - a continuation it can follow,
    /// and the pipeline draws that hop as an indirect link. The last hop out, `GetResult`, is a
    /// blocking wait rather than an await: whoever is parked on it sits on another thread, and no
    /// continuation leads back there, which is why the body never joins `Main`.
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

    // ------------------------------------------------------------ nested modules
    module Outer =
        module Middle =
            module Inner =
                let deeplyNested () = sink "nested modules"

    let nestedModules () = Outer.Middle.Inner.deeplyNested ()

    // ------------------------------------------------------------ a C# callback reached from F#
    /// Gives a stack that alternates languages: C# -> F# -> closure -> C# -> F#.
    let throughCallback (callback: Func<int, int>) =
        [ 1 ]
        |> List.map (fun x -> callback.Invoke x)
        |> List.sum

    // ------------------------------------------------------------ delegates
    /// The lambda is reached through `Invoke` on the delegate type, so its frame is a closure.
    let throughDelegate (callback: Callback) = callback.Invoke 1

/// Members, properties, constructors and an interface implementation.
type Worker(seed: int) =
    let mutable state = seed
    do state <- state + 1

    member this.Start() = this.CurriedStep 1 2
    member _.CurriedStep (a: int) (b: int) = Demo.sink "class members"

    member _.Computed =
        state <- state + 1
        Demo.sink "property getter"

    member _.Tuned
        with get () = state
        and set value =
            Demo.sink "property setter" |> ignore
            state <- value

    static member StaticEntry() = Demo.sink "static member"

/// An event adds a second indirection - the handler runs through `MulticastDelegate.Invoke` - and the
/// `[<CLIEvent>]` accessors are the only `add_`/`remove_` frames F# produces.
type Publisher() =
    let fired = Event<int>()

    /// An event is never a frame a debugger stops in. F# reads `p.Fired` by building an `IEvent`
    /// over `add_Fired`/`remove_Fired` rather than by calling the getter, C# subscribes through the
    /// same accessors, and those hold no user code. What reaches the stack is the handler.
    [<CLIEvent>]
    member _.Fired = fired.Publish

    member _.Fire() = fired.Trigger 1

type IRunner =
    abstract Run: string -> int

type Runner() =
    interface IRunner with
        member _.Run name = Demo.sink "interface implementation"

/// Generic type member.
type Box<'T>(value: 'T) =
    member _.Unwrap() = Demo.sink "generic type member"

/// Constructor and static-constructor frames (`..ctor` / `..cctor` in metadata).
/// The first `new Initialized(...)` hits the breakpoint twice: static init, then instance init.
type Initialized(seed: int) =
    static let staticState = Demo.sink "static constructor"
    let state = Demo.sink "constructor" + seed

    member _.State = state
    static member StaticState = staticState

/// Members on a discriminated union and on a record.
type Shape =
    | Circle of radius: float
    | Rect of width: float * height: float

    member this.Area() =
        Demo.sink "union member" |> ignore

        match this with
        | Circle r -> Math.PI * r * r
        | Rect(w, h) -> w * h

type Point =
    { X: float
      Y: float }

    member this.Norm() =
        Demo.sink "record member" |> ignore
        sqrt (this.X * this.X + this.Y * this.Y)

/// Module-level initialization: the stack frame comes from the compiler-synthesized
/// `<StartupCode$...>` class, not from any user-visible method.
module Startup =
    let initialized = Demo.sink "module initialization"

/// The longest mixed stack: member, local function and lambda in one chain.
module MixedChain =

    let run () =
        let stage (values: int list) =
            values
            |> List.map (fun v ->
                let finish () = Demo.sink "long mixed chain"
                finish () + v)
            |> List.sum

        Worker(1).Start() |> ignore
        stage [ 1; 2 ]

/// Zero-argument entry points, so the C# driver can pass a method group instead of a lambda.
/// A C# lambda reaches the map as an `AnonymousMethod__N` node from the built-in provider and
/// buries the F# frames the map is meant to show.
module Scenarios =

    let localFunctions () = Demo.localFunctions 1
    let genericFunction () = Demo.genericFunction 42
    let unionMember () = int ((Shape.Circle 1.0).Area())
    let recordMember () = int ({ X = 3.0; Y = 4.0 }.Norm())
    let genericTypeMember () = Box<string>("s").Unwrap()
    let interfaceImplementation () = (Runner() :> IRunner).Run "x"
    let constructors () = Initialized(1).State
    let moduleInitialization () = Startup.initialized

    /// Owned here rather than by the C# driver: passing an instance in would force a lambda at the
    /// call site, and that lambda becomes an `AnonymousMethod__N` node on the map.
    let private worker = Worker 10

    let propertyGetter () = worker.Computed

    let propertySetter () =
        worker.Tuned <- 5
        worker.Tuned

    let instanceMember () = worker.Start()

    let delegateCall () =
        Demo.throughDelegate (Callback(fun _ -> Demo.sink "delegate call"))

    let eventHandler () =
        let publisher = Publisher()
        let mutable result = 0
        publisher.Fired.Add(fun _ -> result <- Demo.sink "event handler")
        publisher.Fire()
        result

    let callbackIntoCSharp (callback: Func<int, int>) = Demo.throughCallback callback
