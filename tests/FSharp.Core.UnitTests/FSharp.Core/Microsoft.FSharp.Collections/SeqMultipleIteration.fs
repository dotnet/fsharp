namespace FSharp.Core.UnitTests.Collections

open Xunit


module SeqMultipleIteration =
    let makeNewSeq () =
        let mutable haveCalled = false
        seq {
            if haveCalled then failwith "Should not have iterated this sequence before"
            haveCalled <- true
            yield 3
        }, (fun () -> haveCalled)

    [<Fact>]
    let ``Seq.distinct only evaluates the seq once`` () =
        let s, haveCalled = makeNewSeq ()
        let distincts = Seq.distinct s
        Assert.False (haveCalled())
        CollectionAssert.AreEqual (distincts |> Seq.toList, [3])
        Assert.True (haveCalled())

    [<Fact>]
    let ``Seq.distinctBy only evaluates the seq once`` () =
        let s, haveCalled = makeNewSeq ()
        let distincts = Seq.distinctBy id s
        Assert.False (haveCalled())
        CollectionAssert.AreEqual (distincts |> Seq.toList, [3])
        Assert.True (haveCalled())

    [<Fact>]
    let ``Seq.groupBy only evaluates the seq once`` () =
        let s, haveCalled = makeNewSeq ()
        let groups : seq<int * seq<int>> = Seq.groupBy id s
        Assert.False (haveCalled())
        let groups : list<int * seq<int>> = Seq.toList groups
        // Seq.groupBy iterates the entire sequence as soon as it begins iteration.
        Assert.True (haveCalled())

    [<Fact>]
    let ``Seq.countBy only evaluates the seq once`` () =
        let s, haveCalled = makeNewSeq ()
        let counts : seq<int * int> = Seq.countBy id s
        Assert.False (haveCalled())
        let counts : list<int * int> = Seq.toList counts
        Assert.True (haveCalled())
        CollectionAssert.AreEqual (counts |> Seq.toList, [(3, 1)])
