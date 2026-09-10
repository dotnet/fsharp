namespace FSharp.Compiler.ComponentTests.Language

open Xunit
open FSharp.Test.Compiler

module UseBindingDisposalTests =

    [<Fact>]
    let ``use a as b = d disposes once`` () =
        FSharp """
module M
let mutable count = 0
let d = { new System.IDisposable with
            member _.Dispose() = count <- count + 1 }
do
    use a as b = d
    ()
printfn "%d" count
"""
        |> asExe
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "1"

    [<Fact>]
    let ``use rebinding of use-bound value disposes once`` () =
        FSharp """
module M
type Counter() =
    let mutable count = 0
    member _.DisposeCount = count
    interface System.IDisposable with
        member this.Dispose() = count <- count + 1
let test() =
    let c = new Counter()
    (
        use a = c :> System.IDisposable
        use b = a
        ()
    )
    c.DisposeCount
printfn "%d" (test())
"""
        |> asExe
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "1"

    [<Fact>]
    let ``Normal use disposes exactly once`` () =
        FSharp """
module M
type Counter() =
    let mutable count = 0
    member _.DisposeCount = count
    interface System.IDisposable with
        member this.Dispose() = count <- count + 1
let c = new Counter()
let test() =
    (
        use x = (c :> System.IDisposable)
        ()
    )
    c.DisposeCount
printfn "%d" (test())
"""
        |> asExe
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "1"

    [<Fact>]
    let ``Discarded use still disposes`` () =
        FSharp """
module M
let mutable count = 0
let d = { new System.IDisposable with
            member _.Dispose() = count <- count + 1 }
do
    use _ = d
    ()
printfn "%d" count
"""
        |> asExe
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "1"

    [<Fact>]
    let ``Two independent use bindings each dispose`` () =
        FSharp """
module M
let mutable count = 0
let mk () = { new System.IDisposable with member _.Dispose() = count <- count + 1 }
let test() =
    (
        use a = mk()
        use b = mk()
        ()
    )
    count
printfn "%d" (test())
"""
        |> asExe
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "2"

    [<Fact>]
    let ``use rebinding with upcast of use-bound value disposes once`` () =
        FSharp """
module M
type Counter() =
    let mutable count = 0
    member _.DisposeCount = count
    interface System.IDisposable with
        member this.Dispose() = count <- count + 1
let test() =
    let c = new Counter()
    (
        use a = c
        use b = (a :> System.IDisposable)
        ()
    )
    c.DisposeCount
printfn "%d" (test())
"""
        |> asExe
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "1"

    [<Fact>]
    let ``Triple alias via as-pattern disposes once`` () =
        FSharp """
module M
let mutable count = 0
let d = { new System.IDisposable with member _.Dispose() = count <- count + 1 }
do
    use a as b as c = d
    ignore (a, b, c)
printfn "%d" count
"""
        |> asExe
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "1"
