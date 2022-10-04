// #Conformance #DeclarationElements #MemberDefinitions #Overloading 
// inference failed for Linq .ToDictionary(fun,fun) - #3170

open System
open System.Linq

type GuidWrapper = GuidWrapper of Guid
    with member x.Id =
            let (GuidWrapper id) = x
            x

let id = Guid.NewGuid()
let map = [GuidWrapper id, GuidWrapper id] |> Map

let y = map.ToDictionary((fun m -> m.Key.Id), (fun m -> m.Value.Id))

printfn "%A" y
