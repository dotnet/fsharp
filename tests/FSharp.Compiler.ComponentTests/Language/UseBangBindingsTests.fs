// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

open FSharp.Test
open Xunit
open FSharp.Test.Compiler

module UseBangBindingsVersion9Tests =
    [<Fact>]
    let ``Error when using discard pattern in use! binding`` () =
        FSharp """
module Program
open System
type Counter(value: int) =
    member _.Value = value
    interface IDisposable with
        member _.Dispose() = ()

let getCounterAsync initial = async {
    return new Counter(initial)
}

let exampleAsync() = async {
    use! counter = getCounterAsync 5 
    use! __ = getCounterAsync 4
    use! _ = getCounterAsync 3
    return ()
}

[<EntryPoint>]
let main _ =
    exampleAsync() |> Async.RunSynchronously
    0
        """
        |> withLangVersion90 
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 1228, Line 16, Col 10, Line 16, Col 11, "'use!' bindings must be of the form 'use! <var> = <expr>'")
        ]
        
    [<FactForNETCOREAPP>]
    let ``Named patterns are allowed in use! binding`` () =
        FSharp """
module Program 
let doSomething () =
    async {
        use _ = { new System.IDisposable with member _.Dispose() = printfn "disposed 1" }
        use! __ = Async.OnCancel (fun () -> printfn "disposed 2")
        use! res2 = Async.OnCancel (fun () -> printfn "disposed 3")
        do! Async.Sleep(100)
        return ()
    }

[<EntryPoint>]
let main _ =
    let cts = new System.Threading.CancellationTokenSource()
    Async.Start(doSomething(), cts.Token)
    System.Threading.Thread.Sleep(100)
    cts.Cancel()
    0
        """
        |> withLangVersion90
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> withStdOutContains "disposed 1"
        |> withStdOutContains "disposed 2"
        |> withStdOutContains "disposed 3"
        
    [<Fact>]
    let ``Both named and discard patterns in custom computation expression use! binding`` () =
        FSharp """
module Program 
open System
type Counter(value: int) =
    member _.Value = value
    
    interface IDisposable with
        member _.Dispose() = 
            printfn $"Counter with value %d{value} disposed"

type CounterBuilder() =
    member _.Using(resource: #IDisposable, f) =
        async {
            use res = resource
            return! f res
        }
        
    member _.Bind(counter: Counter, f) =
        async {
            let c = counter
            return! f c
        }
    
    member _.Return(x) = async.Return x
    
    member _.ReturnFrom(x) = x
    
    member _.Bind(task, f) = async.Bind(task, f)

[<EntryPoint>]
let main _ =
    let counterBuilder = CounterBuilder()

    let example() =
        counterBuilder {
            use! res = new Counter(5)
            use! __ = new Counter(4)
            do! Async.Sleep 1000
            return ()
        }

    example()
    |> Async.RunSynchronously
    0
        """
        |> withLangVersion90
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> withStdOutContains "Counter with value 4 disposed"
        |> withStdOutContains "Counter with value 5 disposed"
        
module UseBangBindingsPreviewTests =
    [<Fact>]
    let ``Discard pattern allowed in async use! binding`` () =
        FSharp """
module Program

open System

type Counter(value: int) =
    member _.Value = value
    
    interface IDisposable with
        member _.Dispose() = 
            printfn $"Counter with value %d{value} disposed"

let getCounterAsync initial = async {
    do! Async.Sleep 100 // Simulate async work
    printfn $"Created counter with value %d{initial}"
    return new Counter(initial)
}

let exampleAsync() = async {
    printfn "Starting async workflow"
    use! counter = getCounterAsync 5 
    use! __ = getCounterAsync 4
    use! _ = getCounterAsync 3
    printfn "Done using the resource"
    return ()
}
        
[<EntryPoint>]
let main _ =
    exampleAsync() |> Async.RunSynchronously
    0
        """
        |> withLangVersionPreview 
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> withStdOutContains "Starting async workflow"
        |> withStdOutContains "Created counter with value 5"
        |> withStdOutContains "Created counter with value 4"
        |> withStdOutContains "Created counter with value 3"
        |> withStdOutContains "Done using the resource"
        |> withStdOutContains "Counter with value 3 disposed"
        |> withStdOutContains "Counter with value 4 disposed"
        |> withStdOutContains "Counter with value 5 disposed"

    [<FactForNETCOREAPP>]
    let ``Discard pattern allowed in async use! binding 2`` () =
        FSharp """
module Program 
let doSomething () =
    async {
        use _ = { new System.IDisposable with member _.Dispose() = printfn "disposed 1" }
        use! __ = Async.OnCancel (fun () -> printfn "disposed 2")
        use! res2 = Async.OnCancel (fun () -> printfn "disposed 3")
        use! _ = Async.OnCancel (fun () -> printfn "disposed 4")
        do! Async.Sleep(100)
        return ()
    }

[<EntryPoint>]
let main _ =
    let cts = new System.Threading.CancellationTokenSource()
    Async.Start(doSomething(), cts.Token)
    System.Threading.Thread.Sleep(100)
    cts.Cancel()
    0
        """
        |> withLangVersionPreview
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> withStdOutContains "disposed 1"
        |> withStdOutContains "disposed 2"
        |> withStdOutContains "disposed 3"
        |> withStdOutContains "disposed 4"
        
    [<Fact>]
    let ``Both named and discard patterns in custom computation expression use! binding`` () =
        FSharp """
module Program 
open System
type Counter(value: int) =
    member _.Value = value
    
    interface IDisposable with
        member _.Dispose() = 
            printfn $"Counter with value %d{value} disposed"

type CounterBuilder() =
    member _.Using(resource: #IDisposable, f) =
        async {
            use res = resource
            return! f res
        }
        
    member _.Bind(counter: Counter, f) =
        async {
            let c = counter
            return! f c
        }
    
    member _.Return(x) = async.Return x
    
    member _.ReturnFrom(x) = x
    
    member _.Bind(task, f) = async.Bind(task, f)

[<EntryPoint>]
let main _ =
    let counterBuilder = CounterBuilder()

    let example() =
        counterBuilder {
            use! res = new Counter(5)
            use! __ = new Counter(4)
            use! _ = new Counter(3)
            do! Async.Sleep 1000
            return ()
        }

    example()
    |> Async.RunSynchronously
    0
        """
        |> withLangVersionPreview
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> withStdOutContains "Counter with value 3 disposed"
        |> withStdOutContains "Counter with value 4 disposed"
        |> withStdOutContains "Counter with value 5 disposed"
        
    [<Fact>]
    let ``Discard patterns are allowed in use and use! binding`` () =
        FSharp """
module Program 
open System
type Counter(value: int) =
    member _.Value = value
    
    interface IDisposable with
        member _.Dispose() = 
            printfn $"Counter with value %d{value} disposed"

type CounterBuilder() =
    member _.Using(resource: #IDisposable, f) =
        async {
            use res = resource
            return! f res
        }
        
    member _.Bind(counter: Counter, f) =
        async {
            let c = counter
            return! f c
        }
    
    member _.Return(x) = async.Return x
    
    member _.ReturnFrom(x) = x
    
    member _.Bind(task, f) = async.Bind(task, f)

[<EntryPoint>]
let main _ =
    let counterBuilder = CounterBuilder()

    let example() =
        counterBuilder {
            use _ = { new System.IDisposable with member _.Dispose() = printfn "disposed 1" }
            use! _ = new Counter(2)
            do! Async.Sleep 1000
            return ()
        }

    example()
    |> Async.RunSynchronously
    0
        """
        |> withLangVersionPreview
        |> asExe
        |> compile
        |> shouldSucceed
        |> run
        |> shouldSucceed
        |> withStdOutContains "disposed 1"
        |> withStdOutContains "Counter with value 2 disposed"