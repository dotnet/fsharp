// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.Service.Tests.ConstraintSolverTests

open System

open FSharp.Test.Assert

open Internal.Utilities.Library.Extras

open FSharp.Compiler.ConstraintSolver
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.Xml

open Xunit

let private setState trace (state: int ref) value =
    let previous = state.Value
    (WithTrace trace).Exec (fun () -> state.Value <- value) (fun () -> state.Value <- previous)

[<Theory>]
[<InlineData(true, true)>]
[<InlineData(true, false)>]
[<InlineData(false, false)>]
let ``Operation success commits even when its payload is false`` successful payload =
    let state = ref 0
    let warning = InvalidOperationException("warning")
    let result = if successful then OkResult([ warning ], payload) else ErrorResult([ warning ], InvalidOperationException("failure"))
    let actual =
        NoTrace.CollectThenUndoOrCommit
            (fun (res: OperationResult<_>) -> res.IsOkResult)
            (fun trace ->
                setState trace state 10
                result)
    Assert.Same(result, actual)
    state.Value |> shouldEqual (if successful then 10 else 0)

[<Theory>]
[<InlineData(false, false)>]
[<InlineData(false, true)>]
[<InlineData(true, false)>]
[<InlineData(true, true)>]
let ``Exceptions from solving or accepting a probe restore state`` fromPredicate canceled =
    let state = ref 0
    let failure: exn = if canceled then OperationCanceledException() else InvalidOperationException()
    let thrown =
        Assert.ThrowsAny<Exception>(fun () ->
            NoTrace.CollectThenUndoOrCommit
                (fun _ -> if fromPredicate then raise failure else true)
                (fun trace ->
                    setState trace state 10
                    if not fromPredicate then raise failure)
            |> ignore)
    Assert.Same(failure, thrown)
    state.Value |> shouldEqual 0

[<Theory>]
[<InlineData(false, false)>]
[<InlineData(false, true)>]
[<InlineData(true, false)>]
[<InlineData(true, true)>]
let ``Committed inner changes belong to the outer transaction`` outerCommit innerCommit =
    let state = ref 0
    NoTrace.CollectThenUndoOrCommit
        (fun _ -> outerCommit)
        (fun outer ->
            setState outer state 10
            (WithTrace outer).CollectThenUndoOrCommit
                (fun _ -> innerCommit)
                (fun inner -> setState inner state 20))
    state.Value |> shouldEqual (if not outerCommit then 0 elif innerCommit then 20 else 10)

[<Fact>]
let ``An outer exception undoes a committed inner transaction`` () =
    let state = ref 0
    let failure = InvalidOperationException()
    Assert.Throws<InvalidOperationException>(fun () ->
        NoTrace.CollectThenUndoOrCommit
            (fun _ -> true)
            (fun outer ->
                setState outer state 10
                (WithTrace outer).CollectThenUndoOrCommit (fun _ -> true) (fun inner -> setState inner state 20)
                raise failure)
        |> ignore)
    |> ignore
    state.Value |> shouldEqual 0

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``Candidate replay is enlisted in its accepting transaction`` commit =
    let state = ref 0
    let candidates = FilterEachThenUndo (fun trace value -> setState trace state value; OkResult([], false)) [ 10 ]
    state.Value |> shouldEqual 0
    let _, _, trace, payload = Assert.Single candidates
    payload |> shouldEqual false
    NoTrace.CollectThenUndoOrCommit (fun _ -> commit) (fun owner -> (WithTrace owner).AddFromReplay trace)
    state.Value |> shouldEqual (if commit then 10 else 0)

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``Probe-only helpers undo when a candidate throws`` filtering =
    let state = ref 0
    let failure = OperationCanceledException()
    Assert.Throws<OperationCanceledException>(fun () ->
        if filtering then
            FilterEachThenUndo (fun trace _ -> setState trace state 10; raise failure) [ () ] |> ignore
        else
            CollectThenUndo (fun trace -> setState trace state 10; raise failure))
    |> ignore
    state.Value |> shouldEqual 0

[<Theory>]
[<InlineData(false)>]
[<InlineData(true)>]
let ``Extension scope remapping preserves composition and rejects collisions`` leaveOneUnchanged =
    let makeReferences () =
        [| for name in [ "A"; "B"; "C" ] ->
            Construct.NewModuleOrNamespace
                None
                taccessPublic
                (ident (name, Range.range0))
                XmlDoc.Empty
                []
                (MaybeLazy.Strict(Construct.NewEmptyModuleOrNamespaceType(Namespace true)))
            |> mkLocalTyconRef |]
    let original = makeReferences ()
    let first = makeReferences ()
    let second = makeReferences ()
    if leaveOneUnchanged then
        first[2] <- original[2]
        second[2] <- original[2]
    let renaming (source: TyconRef array) (target: TyconRef array) =
        let stamps = Array.map2 (fun (source: TyconRef) (target: TyconRef) -> source.Stamp, target.Stamp) source target |> Map.ofArray
        fun stamp -> stamps |> Map.tryFind stamp |> Option.defaultValue stamp
    let renameFirst = renaming original first
    let renameSecond = renaming first second
    let scope = TyconRefMultiMap.OfList [ original[0], 1; original[0], 2; original[1], 3; original[2], 4 ]
    let identity = scope.Remap(id, id)
    let sequential = scope.Remap(renameFirst, ((+) 10)).Remap(renameSecond, ((*) 2))
    let composed = scope.Remap(renameFirst >> renameSecond, ((+) 10) >> ((*) 2))
    for i in 0..2 do
        identity.Find original[i] |> shouldEqual (scope.Find original[i])
        sequential.Find second[i] |> shouldEqual (composed.Find second[i])
        composed.Find second[i] |> shouldEqual (scope.Find original[i] |> List.map (((+) 10) >> ((*) 2)))

    let collapsingRemap stamp =
        if stamp = original[0].Stamp then renameFirst original[2].Stamp else renameFirst stamp
    Assert.Throws<InvalidOperationException>(fun () -> scope.Remap(collapsingRemap, id) |> ignore) |> ignore
