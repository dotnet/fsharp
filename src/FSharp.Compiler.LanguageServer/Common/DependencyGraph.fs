module FSharp.Compiler.LanguageServer.Common.DependencyGraph

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

    abstract member AddOrUpdateNode: id: 'Id * value: 'Val -> IGraphBuilder<'Id, 'Val>
    abstract member AddList: nodes: ('Id * 'Val) seq -> IGraphBuilder<'Id, 'Val>
    abstract member AddOrUpdateNode: id: 'Id * dependsOn: 'Id seq * compute: ('Val seq -> 'Val) -> IGraphBuilder<'Id, 'Val>
    abstract member GetValue: id: 'Id -> 'Val
    abstract member GetDependentsOf: id: 'Id -> 'Id seq
    abstract member AddDependency: node: 'Id * dependsOn: 'Id -> unit
    abstract member RemoveDependency: node: 'Id * noLongerDependsOn: 'Id -> unit
    abstract member UpdateNode: id: 'Id * update: ('Val -> 'Val) -> unit
    abstract member Debug_GetNodes: ('Id -> bool) -> DependencyNode<'Id, 'Val> seq

and IThreadSafeDependencyGraph<'Id, 'Val when 'Id: equality> =
    inherit IDependencyGraph<'Id, 'Val>

    abstract member Transact<'a>: (IDependencyGraph<'Id, 'Val> -> 'a) -> 'a

and IGraphBuilder<'Id, 'Val when 'Id: equality> =

    abstract member Ids: 'Id seq
    abstract member AddDependentNode: 'Id * ('Val seq -> 'Val) -> IGraphBuilder<'Id, 'Val>
    abstract member And: IGraphBuilder<'Id, 'Val> -> IGraphBuilder<'Id, 'Val>

module Internal =

    type DependencyGraph<'Id, 'Val when 'Id: equality>(?graphBuilder: 'Id seq -> IGraphBuilder<_, _>) as self =
        let nodes = Dictionary<'Id, DependencyNode<'Id, 'Val>>()
        let dependencies = Dictionary<'Id, HashSet<'Id>>()
        let dependents = Dictionary<'Id, HashSet<'Id>>()

        let graphBuilder = defaultArg graphBuilder (fun x -> (GraphBuilder(self, x)))

        let rec invalidateDependents (id: 'Id) =
            match dependents.TryGetValue id with
            | true, set ->
                for dependent in set do
                    nodes.[dependent] <- { nodes.[dependent] with Value = None }
                    invalidateDependents dependent
            | false, _ -> ()

        let invalidateNodeAndDependents id =
            nodes[id] <- { nodes[id] with Value = None }
            invalidateDependents id

        let addNode node =
            nodes |> insert node.Id node
            invalidateDependents node.Id

        member _.Debug =
            {|
                Nodes = nodes
                Dependencies = dependencies
                Dependents = dependents
            |}

        member this.AddOrUpdateNode(id: 'Id, value: 'Val) =
            addNode
                {
                    Id = id
                    Value = Some value
                    Compute = (fun _ -> value)
                }

            graphBuilder [ id ]

        member this.AddList(nodes: ('Id * 'Val) seq) =
            nodes
            |> Seq.iter (fun (id, value) ->
                addNode
                    {
                        Id = id
                        Value = Some value
                        Compute = (fun _ -> value)
                    })

            graphBuilder (nodes |> Seq.map fst)

        member this.AddOrUpdateNode(id: 'Id, dependsOn: 'Id seq, value: 'Val seq -> 'Val) =
            addNode
                {
                    Id = id
                    Value = None
                    Compute = value
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

            graphBuilder [ id ]

        member this.GetValue(id: 'Id) =
            let node = nodes[id]

            match node.Value with
            | Some value -> value
            | None ->
                let dependencies = dependencies.[id]
                let values = dependencies |> Seq.map (fun id -> this.GetValue id)
                let value = node.Compute values
                nodes.[id] <- { node with Value = Some value }
                value

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

        member this.Debug_GetNodes(predicate: 'Id -> bool): DependencyNode<'Id,'Val> seq =
            nodes.Values |> Seq.filter (fun node -> predicate node.Id)


        interface IDependencyGraph<'Id, 'Val> with

            member this.Debug_GetNodes(predicate)= self.Debug_GetNodes(predicate)

            member _.AddOrUpdateNode(id, value) = self.AddOrUpdateNode(id, value)
            member _.AddList(nodes) = self.AddList(nodes)

            member _.AddOrUpdateNode(id, dependsOn, compute) =
                self.AddOrUpdateNode(id, dependsOn, compute)

            member _.GetValue(id) = self.GetValue(id)
            member _.GetDependentsOf(id) = self.GetDependentsOf(id)
            member _.AddDependency(node, dependsOn) = self.AddDependency(node, dependsOn)

            member _.RemoveDependency(node, noLongerDependsOn) =
                self.RemoveDependency(node, noLongerDependsOn)

            member _.UpdateNode(id, update) = self.UpdateNode(id, update)

    and GraphBuilder<'Id, 'Val when 'Id: equality> internal (graph: IDependencyGraph<'Id, 'Val>, ids: 'Id seq) =

        interface IGraphBuilder<'Id, 'Val> with

            member _.Ids = ids

            member _.AddDependentNode(id, compute) = graph.AddOrUpdateNode(id, ids, compute)

            member _.And(graphBuilder: IGraphBuilder<'Id, 'Val>) =
                GraphBuilder(
                    graph,
                    seq {
                        yield! ids
                        yield! graphBuilder.Ids
                    }
                )

open Internal
open System.Runtime.CompilerServices

type LockOperatedDependencyGraph<'Id, 'Val when 'Id: equality>() as self =

    let lockObj = System.Object()
    let graph = DependencyGraph<_, _>(fun x -> GraphBuilder(self, x))

    interface IThreadSafeDependencyGraph<'Id, 'Val> with

        member _.AddDependency(node, dependsOn) =
            lock lockObj (fun () -> graph.AddDependency(node, dependsOn))

        member _.AddList(nodes)=
            lock lockObj (fun () -> graph.AddList(nodes))

        member _.AddOrUpdateNode(id, value)=
            lock lockObj (fun () -> graph.AddOrUpdateNode(id, value))

        member _.AddOrUpdateNode(id, dependsOn, compute)=
            lock lockObj (fun () -> graph.AddOrUpdateNode(id, dependsOn, compute))

        member _.GetDependentsOf(id) =
            lock lockObj (fun () -> graph.GetDependentsOf(id))

        member _.GetValue(id) =
            lock lockObj (fun () -> graph.GetValue(id))

        member _.UpdateNode(id, update) =
            lock lockObj (fun () -> graph.UpdateNode(id, update))

        member _.RemoveDependency(node, noLongerDependsOn) =
            lock lockObj (fun () -> graph.RemoveDependency(node, noLongerDependsOn))

        member _.Transact(f) = lock lockObj (fun () -> f graph)

        member _.Debug_GetNodes(predicate) =
            lock lockObj (fun () -> graph.Debug_GetNodes(predicate))



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
        let results =
            dependencies
            |> Seq.choose unpacker

        if dependencies |> Seq.length <> (results |> Seq.length) then
            failwith $"Expected all dependencies to match {unpacker} but got: %A{dependencies |> Seq.toArray}"

        results

    [<Extension>]
    static member UnpackOneMany(dependencies: 'NodeValue seq, headUnpacker, tailUnpacker) =
        let mutable oneResult = None
        let manyResult = new ResizeArray<_>()
        let extras = new ResizeArray<_>()

        for item in dependencies do
            match headUnpacker item, tailUnpacker item with
            | Some head, _ -> oneResult <- Some head
            | None, Some item -> manyResult.Add item |> ignore
            | None, None -> extras.Add item |> ignore

        match oneResult with
        | None -> failwith $"Expected exactly one dependency matching {headUnpacker} but didn't find any"
        | Some head ->
            if extras.Count > 0 then
                failwith $"Found extra dependencies: %A{extras.ToArray()}"

            head, seq manyResult

