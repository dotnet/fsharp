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

type RangeScopedCollection<'T when 'T :> IRangeOwner>() as this =
    let mutable currentScope = this

    member val Items = ResizeArray<'T>()
    member val NestedScopes = ResizeArray<range * RangeScopedCollection<'T>>()

    member _.AddItem(item: 'T) =
        currentScope.Items.Add(item)

    member _.CreateScope(m: range) =
        let newScope = RangeScopedCollection()
        currentScope.NestedScopes.Add(m, newScope)

        let parentScope = currentScope
        currentScope <- newScope

        { new IDisposable with
            member x.Dispose() =
                // todo: remove scopes without items 
                currentScope <- parentScope }


[<RequireQualifiedAccess>]
module RangeScopedCollection =
    let itemsByEndPos (endPos: pos) (rsc: RangeScopedCollection<_>): IList<_> =
        let rec loop (result: ResizeArray<_>) endPos (rsc: RangeScopedCollection<_>) =
            for item in rsc.Items do
                if rangeEndsAtPos endPos item.Range then
                    result.Add(item)

            for range, scope in rsc.NestedScopes do
                if rangeContainsPos range endPos then
                    loop result endPos scope 

        let result = ResizeArray()
        loop result endPos rsc

        result :> _
