// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Tests for camelCase functions in module Async

namespace FSharp.Core.UnitTests.Control

open Xunit

module AsyncModuleFunctionsTests =

    [<Fact>]
    let ``Async.result wraps value`` () =
        let actual = Async.result 42 |> Async.RunSynchronously
        Assert.Equal(42, actual)

    [<Fact>]
    let ``Async.map transforms value`` () =
        let actual = Async.result 21 |> Async.map (fun x -> x * 2) |> Async.RunSynchronously
        Assert.Equal(42, actual)

    [<Fact>]
    let ``Async.map preserves exception`` () =
        let comp = async { return failwith "boom" : int } |> Async.map (fun x -> x * 2)
        let e = Assert.Throws<exn>(fun () -> comp |> Async.RunSynchronously |> ignore)
        Assert.Equal("boom", e.Message)

    [<Fact>]
    let ``Async.empty returns unit`` () =
        let actual = Async.empty |> Async.RunSynchronously
        Assert.Equal((), actual)

    [<Fact>]
    let ``Async.bind threads value`` () =
        let actual =
            Async.result 21
            |> Async.bind (fun x -> Async.result (x * 2))
            |> Async.RunSynchronously
        Assert.Equal(42, actual)

    [<Fact>]
    let ``Async.bind preserves exception`` () =
        let comp = async { return failwith "boom" : int } |> Async.bind Async.result
        let e = Assert.Throws<exn>(fun () -> comp |> Async.RunSynchronously |> ignore)
        Assert.Equal("boom", e.Message)

    [<Fact>]
    let ``Async.ignore discards result`` () =
        let actual = Async.result 42 |> Async.ignore<int> |> Async.RunSynchronously
        Assert.Equal((), actual)

    [<Fact>]
    let ``Async.catchWith recovers from exception`` () =
        let actual =
            async { return failwith "boom" : int }
            |> Async.catchWith (fun e -> Assert.Equal("boom", e.Message); -1)
            |> Async.RunSynchronously
        Assert.Equal(-1, actual)

    [<Fact>]
    let ``Async.catchWith passes through success`` () =
        let actual =
            Async.result 42
            |> Async.catchWith (fun _ -> -1)
            |> Async.RunSynchronously
        Assert.Equal(42, actual)

    [<Fact>]
    let ``Async.catch returns Ok on success`` () =
        let actual = Async.result 42 |> Async.catch |> Async.RunSynchronously
        Assert.Equal(Ok 42, actual)

    [<Fact>]
    let ``Async.catch returns Error on exception`` () =
        let comp = async { return failwith "boom" : int } |> Async.catch
        match comp |> Async.RunSynchronously with
        | Error ex -> Assert.Equal("boom", ex.Message)
        | Ok _ -> failwith "expected Error"

    [<Fact>]
    let ``Async.ignore runs the computation`` () : System.Threading.Tasks.Task =
        let comp = async { return failwith "boom" : int } |> Async.ignore<int>
        task {
            let! e = Assert.ThrowsAsync<exn>(fun () -> Async.StartAsTask comp)
            Assert.Equal("boom", e.Message)
        }
