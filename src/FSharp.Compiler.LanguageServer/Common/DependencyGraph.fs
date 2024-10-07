module FSharp.Compiler.LanguageServer.Common.DependencyGraph

open System.Collections.Generic


type DependencyNode<'Identifier, 'Value> =
    { 
        Id: 'Identifier 
        Value: 'Value option

        // TODO: optional if it's root node
        Compute: 'Value seq -> 'Value
    }


let insert key value (dict: Dictionary<_, _>)=
    match dict.TryGetValue key with
    | true, _ -> dict[key] <- value
    | false, _ -> dict.Add(key, value)


// TODO: thread safety
type DependencyGraph<'Id, 'Val when 'Id :equality >() =
    let nodes = Dictionary<'Id, DependencyNode<'Id, 'Val>>()
    let dependencies = Dictionary<'Id, HashSet<'Id>>()
    let dependents = Dictionary<'Id, HashSet<'Id>>()

    let rec invalidateDependents (id: 'Id) =
        match dependents.TryGetValue id with
        | true, set ->
            for dependent in set do
                nodes.[dependent] <- { nodes.[dependent] with Value = None }
                invalidateDependents dependent
        | false, _ -> ()

    let addNode node =
        nodes |> insert node.Id node
        invalidateDependents node.Id

    member _.Debug = {|
        Nodes = nodes
        Dependencies = dependencies
        Dependents = dependents
        |}

    member this.AddOrUpdateNode(id: 'Id, value: 'Val) =
        addNode { Id = id; Value = Some value; Compute = (fun _ -> value) }
        GraphBuilder(this, [id])

    member this.AddList(nodes: ('Id * 'Val) seq) =
        nodes |> Seq.iter (fun (id, value) -> addNode { Id = id; Value = Some value; Compute = (fun _ -> value) })
        GraphBuilder(this, nodes |> Seq.map fst)

    member this.AddOrUpdateNode(id: 'Id, dependsOn: 'Id seq, value: 'Val seq -> 'Val  ) =
        addNode { Id = id; Value = None; Compute = value }
        
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

        GraphBuilder(this, [id])

    member this.GetValue(id: 'Id) =
        let node = nodes.[id]
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


and GraphBuilder<'Id, 'Val when 'Id :equality> internal (graph: DependencyGraph<'Id, 'Val>, ids: 'Id seq) =
    
    member private _.Ids = ids

    member _.AddDependentNode(id, compute) =
        graph.AddOrUpdateNode(id, ids, compute)
            
    member _.And(graphBuilder: GraphBuilder<'Id, 'Val>) =
        GraphBuilder(graph, seq { yield! ids; yield! graphBuilder.Ids })