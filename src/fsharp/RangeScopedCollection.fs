namespace FSharp.Compiler

open System
open System.Collections.Generic
open FSharp.Compiler.Range

type IRangeOwner =
    abstract Range: range

type Disposable() =
    static member val Empty: IDisposable = new Disposable() :> _

    interface IDisposable with
        member _.Dispose() = ()


type ScopeData<'T when 'T :> IRangeOwner>() =
    member val Items = ResizeArray<'T>()
    member val NestedScopes = ResizeArray<RangedScope<'T>>()

and RangedScope<'T when 'T :> IRangeOwner> =
    private
        | Global of ScopeData<'T>
        | Nested of range * ScopeData<'T>

    member private scope.Data =
        match scope with
        | Global data
        | Nested (_, data) -> data

    member scope.NestedScopes =
        scope.Data.NestedScopes

    member scope.Items =
        scope.Data.Items

    member scope.Range =
        match scope with
        | Global _ -> None
        | Nested (scopeRange, _) -> Some scopeRange

    member scope.Contains(m: range) =
        match scope with
        | Global _ -> true
        | Nested (scopeRange, _) -> rangeContainsRange scopeRange m

    member scope.Contains(p: pos) =
        match scope with
        | Global _ -> true
        | Nested (scopeRange, _) -> rangeContainsPos scopeRange p

    member x.CreateNestedScope(m: range) =
        match x with
        | Global _ -> ()
        | Nested (currentScopeRange, _) ->
            assert rangeContainsRange currentScopeRange m

        let scope = Nested(m, ScopeData())
        x.NestedScopes.Add(scope)

        scope

    static member CreateGlobalScope<'T>() =
        RangedScope.Global(ScopeData<'T>())

type RangeScopedCollection<'T when 'T :> IRangeOwner>() =
    let globalScope = RangedScope.CreateGlobalScope()
    let mutable currentScope = globalScope

    member _.AddItem(item: 'T) =
        currentScope.Items.Add(item)

    member _.CreateScope(m: range) =
        let newScope = currentScope.CreateNestedScope(m)

        let parentScope = currentScope
        currentScope <- newScope

        { new IDisposable with
            member x.Dispose() =
                // todo: remove scopes without items 
                currentScope <- parentScope }

    member x.GlobalScope = globalScope

[<RequireQualifiedAccess>]
module RangeScopedCollection =
    let itemsByEndPos (endPos: pos) (rsc: RangeScopedCollection<_>): IList<_> =
        let rec loop (result: ResizeArray<_>) endPos (scope: RangedScope<_>) =
            for item in scope.Items do
                if rangeEndsAtPos endPos item.Range then
                    result.Add(item)

            for scope in scope.NestedScopes do
                if scope.Contains(endPos) then
                    loop result endPos scope

        let result = ResizeArray()
        loop result endPos rsc.GlobalScope

        result :> _
