// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// This Dependency Graph provides a way to maintain an up-to-date but lazy set of dependent values.
/// When changes are applied to the graph (either vertices change value or edges change), no computation is performed.
/// Only when a value is requested it is lazily computed and thereafter stored until invalidated by further changes.
module internal Internal.Utilities.DependencyGraph

open System.Collections.Generic

type DependencyNode<'Identifier, 'Value> =
    {
        Id: 'Identifier // TODO: probably not needed
        Value: 'Value option

        // TODO: optional if it's root node
        Compute: 'Value seq -> 'Value
    }

let insert key value (dict: Dictionary<_, _>) =
    match dict.TryGetValue key with
    | true, _ -> dict[key] <- value
    | false, _ -> dict.Add(key, value)

type IDependencyGraph<'Id, 'Val when 'Id: equality> =

    abstract member AddOrUpdateNode: id: 'Id * value: 'Val -> unit
    abstract member AddList: nodes: ('Id * 'Val) seq -> 'Id seq
    abstract member AddOrUpdateNode: id: 'Id * dependsOn: 'Id seq * compute: ('Val seq -> 'Val) -> unit
    abstract member GetValue: id: 'Id -> 'Val
    abstract member GetDependenciesOf: id: 'Id -> 'Id seq
    abstract member GetDependentsOf: id: 'Id -> 'Id seq
    abstract member AddDependency: node: 'Id * dependsOn: 'Id -> unit
    abstract member RemoveDependency: node: 'Id * noLongerDependsOn: 'Id -> unit
    abstract member UpdateNode: id: 'Id * update: ('Val -> 'Val) -> unit
    abstract member RemoveNode: id: 'Id -> unit
    abstract member Debug_RenderMermaid: ?mapping: ('Id -> 'Id) -> string
    abstract member OnWarning: (string -> unit) -> unit

and IThreadSafeDependencyGraph<'Id, 'Val when 'Id: equality> =
    inherit IDependencyGraph<'Id, 'Val>

    abstract member Transact<'a> : (IDependencyGraph<'Id, 'Val> -> 'a) -> 'a

module Internal =

    type DependencyGraph<'Id, 'Val when 'Id: equality and 'Id: not null>() as self =
        let nodes = Dictionary<'Id, DependencyNode<'Id, 'Val>>()
        let dependencies = Dictionary<'Id, HashSet<'Id>>()
        let dependents = Dictionary<'Id, HashSet<'Id>>()
        let warningSubscribers = ResizeArray()

        let rec invalidateDependents (id: 'Id) =
            match dependents.TryGetValue id with
            | true, set ->
                for dependent in set do
                    nodes[dependent] <- { nodes[dependent] with Value = None }
                    invalidateDependents dependent
            | false, _ -> ()

        let invalidateNodeAndDependents id =
            nodes[id] <- { nodes[id] with Value = None }
            invalidateDependents id

        let addNode node =
            nodes |> insert node.Id node
            invalidateDependents node.Id

        member _.Debug_Nodes = nodes

        member _.AddOrUpdateNode(id: 'Id, value: 'Val) =
            addNode
                {
                    Id = id
                    Value = Some value
                    Compute = (fun _ -> value)
                }

        member _.AddList(nodes: ('Id * 'Val) seq) =
            nodes
            |> Seq.map (fun (id, value) ->
                addNode
                    {
                        Id = id
                        Value = Some value
                        Compute = (fun _ -> value)
                    }

                id)
            |> Seq.toList

        member _.AddOrUpdateNode(id: 'Id, dependsOn: 'Id seq, compute: 'Val seq -> 'Val) =
            addNode
                {
                    Id = id
                    Value = None
                    Compute = compute
                }

            match dependencies.TryGetValue id with
            | true, oldDependencies ->
                for dep in oldDependencies do
                    match dependents.TryGetValue dep with
                    | true, set -> set.Remove id |> ignore
                    | _ -> ()
            | _ -> ()

            dependencies |> insert id (HashSet dependsOn)

            for dep in dependsOn do
                match dependents.TryGetValue dep with
                | true, set -> set.Add id |> ignore
                | false, _ -> dependents.Add(dep, HashSet([| id |]))

        member this.GetValue(id: 'Id) =
            let node = nodes[id]

            match node.Value with
            | Some value -> value
            | None ->
                let dependencies = dependencies[id]
                let values = dependencies |> Seq.map (fun id -> this.GetValue id)
                let value = node.Compute values
                nodes[id] <- { node with Value = Some value }
                value

        member this.GetDependenciesOf(identifier: 'Id) =
            match dependencies.TryGetValue identifier with
            | true, set -> set |> Seq.map id
            | false, _ -> Seq.empty

        member this.GetDependentsOf(identifier: 'Id) =
            match dependents.TryGetValue identifier with
            | true, set -> set |> Seq.map id
            | false, _ -> Seq.empty

        member this.AddDependency(node: 'Id, dependsOn: 'Id) =
            match dependencies.TryGetValue node with
            | true, deps -> deps.Add dependsOn |> ignore
            | false, _ -> dependencies.Add(node, HashSet([| dependsOn |]))

            match dependents.TryGetValue dependsOn with
            | true, deps -> deps.Add node |> ignore
            | false, _ -> dependents.Add(dependsOn, HashSet([| node |]))

            invalidateDependents dependsOn

        member this.RemoveDependency(node: 'Id, noLongerDependsOn: 'Id) =
            match dependencies.TryGetValue node with
            | true, deps -> deps.Remove noLongerDependsOn |> ignore
            | false, _ -> ()

            match dependents.TryGetValue noLongerDependsOn with
            | true, deps -> deps.Remove node |> ignore
            | false, _ -> ()

            invalidateNodeAndDependents node

        member this.UpdateNode(id: 'Id, update: 'Val -> 'Val) =
            this.GetValue id
            |> update
            |> fun value -> this.AddOrUpdateNode(id, value) |> ignore

        member this.RemoveNode(id: 'Id) =

            match nodes.TryGetValue id with
            | true, _ ->
                // Invalidate dependents of the removed node
                invalidateDependents id

                // Remove the node from the nodes dictionary
                nodes.Remove id |> ignore

                // Remove the node from dependencies and update dependents
                match dependencies.TryGetValue id with
                | true, deps ->
                    for dep in deps do
                        match dependents.TryGetValue dep with
                        | true, set -> set.Remove id |> ignore
                        | false, _ -> ()

                    dependencies.Remove id |> ignore
                | false, _ -> ()

                // Remove the node from dependents and update dependencies
                match dependents.TryGetValue id with
                | true, deps ->
                    for dep in deps do
                        match dependencies.TryGetValue dep with
                        | true, set -> set.Remove id |> ignore
                        | false, _ -> ()

                    dependents.Remove id |> ignore
                | false, _ -> ()
            | false, _ -> ()

        member _.Debug_RenderMermaid(?mapping) =

            let mapping = defaultArg mapping id

            // We need to give each node a number so the graph is easy to render
            let nodeNumbersById = Dictionary()

            nodes.Keys
            |> Seq.map mapping
            |> Seq.distinct
            |> Seq.indexed
            |> Seq.iter (fun (x, y) -> nodeNumbersById.Add(y, x))

            let content =
                dependencies
                |> Seq.collect (fun kv ->
                    let node = kv.Key
                    let nodeNumber = nodeNumbersById[mapping node]

                    kv.Value
                    |> Seq.map (fun dep -> nodeNumbersById[mapping dep], mapping dep)
                    |> Seq.map (fun (depNumber, dep) -> $"{nodeNumber}[{node}] --> {depNumber}[{dep}]")
                    |> Seq.distinct)
                |> String.concat "\n"

            $"```mermaid\n\ngraph LR\n\n{content}\n\n```"

        member _.OnWarning(f) = warningSubscribers.Add f |> ignore

        interface IDependencyGraph<'Id, 'Val> with

            member _.AddOrUpdateNode(id, value) = self.AddOrUpdateNode(id, value)
            member _.AddList(nodes) = self.AddList(nodes)

            member _.AddOrUpdateNode(id, dependsOn, compute) =
                self.AddOrUpdateNode(id, dependsOn, compute)

            member _.GetValue(id) = self.GetValue(id)
            member _.GetDependenciesOf(id) = self.GetDependenciesOf(id)
            member _.GetDependentsOf(id) = self.GetDependentsOf(id)
            member _.AddDependency(node, dependsOn) = self.AddDependency(node, dependsOn)

            member _.RemoveDependency(node, noLongerDependsOn) =
                self.RemoveDependency(node, noLongerDependsOn)

            member _.UpdateNode(id, update) = self.UpdateNode(id, update)
            member _.RemoveNode(id) = self.RemoveNode(id)

            member _.OnWarning f = self.OnWarning f

            member _.Debug_RenderMermaid(x) = self.Debug_RenderMermaid(?mapping = x)

/// This type can be used to chain together a series of dependent nodes when there is some kind of type hierarchy in the graph.
/// That is when 'T represents some subset of 'Val (e.g. a sub type or a case in DU).
/// It can also carry some state that is passed along the chain.
type GraphBuilder<'Id, 'Val, 'T, 'State when 'Id: equality>
    (graph: IDependencyGraph<'Id, 'Val>, ids: 'Id seq, unwrap: 'Val seq -> 'T, state: 'State) =

    member _.Ids = ids

    member _.State = state

    member _.Graph = graph

    member _.AddDependentNode(id, compute, unwrapNext) =
        graph.AddOrUpdateNode(id, ids, unwrap >> compute)
        GraphBuilder(graph, Seq.singleton id, unwrapNext, state)

    member _.AddDependentNode(id, compute, unwrapNext, nextState) =
        graph.AddOrUpdateNode(id, ids, unwrap >> compute)
        GraphBuilder(graph, Seq.singleton id, unwrapNext, nextState)

open Internal
open System.Runtime.CompilerServices

type LockOperatedDependencyGraph<'Id, 'Val when 'Id: equality and 'Id: not null>() =

    let lockObj = System.Object()
    let graph = DependencyGraph<_, _>()

    interface IThreadSafeDependencyGraph<'Id, 'Val> with

        member _.AddDependency(node, dependsOn) =
            lock lockObj (fun () -> graph.AddDependency(node, dependsOn))

        member _.AddList(nodes) =
            lock lockObj (fun () -> graph.AddList(nodes))

        member _.AddOrUpdateNode(id, value) =
            lock lockObj (fun () -> graph.AddOrUpdateNode(id, value))

        member _.AddOrUpdateNode(id, dependsOn, compute) =
            lock lockObj (fun () -> graph.AddOrUpdateNode(id, dependsOn, compute))

        member _.GetDependenciesOf(id) =
            lock lockObj (fun () -> graph.GetDependenciesOf(id))

        member _.GetDependentsOf(id) =
            lock lockObj (fun () -> graph.GetDependentsOf(id))

        member _.GetValue(id) =
            lock lockObj (fun () -> graph.GetValue(id))

        member _.UpdateNode(id, update) =
            lock lockObj (fun () -> graph.UpdateNode(id, update))

        member _.RemoveNode(id) =
            lock lockObj (fun () -> graph.RemoveNode(id))

        member _.RemoveDependency(node, noLongerDependsOn) =
            lock lockObj (fun () -> graph.RemoveDependency(node, noLongerDependsOn))

        member _.Transact(f) = lock lockObj (fun () -> f graph)

        member _.OnWarning(f) =
            lock lockObj (fun () -> graph.OnWarning f)

        member _.Debug_RenderMermaid(m) =
            lock lockObj (fun () -> graph.Debug_RenderMermaid(?mapping = m))

[<Extension>]
type GraphExtensions =

    [<Extension>]
    static member Unpack(node: 'NodeValue, unpacker) =
        match unpacker node with
        | Some value -> value
        | None -> failwith $"Expected {unpacker} but got: {node}"

    [<Extension>]
    static member UnpackOne(dependencies: 'NodeValue seq, unpacker: 'NodeValue -> 'UnpackedDependency option) =
        dependencies
        |> Seq.tryExactlyOne
        |> Option.bind unpacker
        |> Option.defaultWith (fun () ->
            failwith $"Expected exactly one dependency matching {unpacker} but got: %A{dependencies |> Seq.toArray}")

    [<Extension>]
    static member UnpackMany(dependencies: 'NodeValue seq, unpacker) =
        let results = dependencies |> Seq.choose unpacker

        if dependencies |> Seq.length <> (results |> Seq.length) then
            failwith $"Expected all dependencies to match {unpacker} but got: %A{dependencies |> Seq.toArray}"

        results

    [<Extension>]
    static member UnpackOneMany(dependencies: 'NodeValue seq, oneUnpacker, manyUnpacker) =
        let mutable oneResult = None
        let manyResult = new ResizeArray<_>()
        let extras = new ResizeArray<_>()

        for dependency in dependencies do
            match oneUnpacker dependency, manyUnpacker dependency with
            | Some item, _ -> oneResult <- Some item
            | None, Some item -> manyResult.Add item |> ignore
            | None, None -> extras.Add dependency |> ignore

        match oneResult with
        | None -> failwith $"Expected exactly one dependency matching {oneUnpacker} but didn't find any"
        | Some head ->
            if extras.Count > 0 then
                failwith $"Found extra dependencies: %A{extras.ToArray()}"

            head, seq manyResult
