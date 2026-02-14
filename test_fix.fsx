printfn "Assembly Location: %s" (typeof<set<int>>.Assembly.Location)
#r "./FSharp.Core.Test.dll"

open System

type Element(id: int, version: string) =
    member _.Id = id
    member _.Version = version
    interface IComparable with
        member _.CompareTo(obj) = 
            match obj with
            | :? Element as other -> compare id other.Id
            | _ -> -1
    override _.Equals(obj) = 
        match obj with
        | :? Element as other -> id = other.Id
        | _ -> false
    override _.GetHashCode() = hash id

printfn "--- Testing Identity ---"
let a = Set.ofList [ Element(1, "Old"); Element(2, "Old") ]
let b = Set.ofList [ Element(1, "New") ]

let res1 = Set.intersect a b 
let aSmall = Set.ofList [ Element(1, "Old") ]
let bBig = Set.ofList [ Element(1, "New"); Element(2, "New") ]
let res2 = Set.intersect aSmall bBig

printfn "Identity Test (Must be 'Old' in both cases):"
res1 |> Set.iter (fun e -> printfn "Case 1 (a is Big): %s" e.Version)
res2 |> Set.iter (fun e -> printfn "Case 2 (a is Small): %s" e.Version)

printfn "\n--- Testing Performance ---"
let longSet = Set.ofList [ 1 .. 1000000 ]
let shortSet = Set.ofList [ 1 .. 10 ]

let sw = Diagnostics.Stopwatch.StartNew()
ignore (Set.intersect longSet shortSet)
sw.Stop()
printfn "Long (a) with Short (b): %f ms" sw.Elapsed.TotalMilliseconds

sw.Restart()
ignore (Set.intersect shortSet longSet)
sw.Stop()
printfn "Short (a) with Long (b): %f ms" sw.Elapsed.TotalMilliseconds