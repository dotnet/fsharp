namespace FSharp.Core.UnitTests.FSharp_Core.Microsoft_FSharp_Collections

open NUnit.Framework

[<TestFixture>]
module SeqMultipleIteration =
    let makeNewSeq () =
        let haveCalled = false |> ref
        seq {
            if !haveCalled then failwith "Should not have iterated this sequence before"
            haveCalled := true
            yield 3
        }, haveCalled

    [<Test>]
    let ``Seq.distinct only evaluates the seq once`` () =
        let s, haveCalled = makeNewSeq ()
        let distincts = Seq.distinct s
        Assert.IsFalse !haveCalled
        CollectionAssert.AreEqual (distincts |> Seq.toList, [3])
        Assert.IsTrue !haveCalled

    [<Test>]
    let ``Seq.distinctBy only evaluates the seq once`` () =
        let s, haveCalled = makeNewSeq ()
        let distincts = Seq.distinctBy id s
        Assert.IsFalse !haveCalled
        CollectionAssert.AreEqual (distincts |> Seq.toList, [3])
        Assert.IsTrue !haveCalled

    [<Test>]
    let ``Seq.groupBy only evaluates the seq once`` () =
        let s, haveCalled = makeNewSeq ()
        let distincts : seq<int * seq<int>> = Seq.groupBy id s
        Assert.IsFalse !haveCalled
        let distincts : list<int * seq<int>> = Seq.toList distincts
        Assert.IsFalse !haveCalled
        let distincts : list<int * list<int>> = distincts |> List.map (fun (i, s) -> (i, Seq.toList s))
        CollectionAssert.AreEqual (distincts |> Seq.toList, [3, [3]])
        Assert.IsTrue !haveCalled

    [<Test>]
    let ``Seq.countBy only evaluates the seq once`` () =
        let s, haveCalled = makeNewSeq ()
        let distincts : seq<int * int> = Seq.countBy id s
        Assert.IsFalse !haveCalled
        let distincts : list<int * int> = Seq.toList distincts
        Assert.IsTrue !haveCalled
        CollectionAssert.AreEqual (distincts |> Seq.toList, [(1, 3)])
