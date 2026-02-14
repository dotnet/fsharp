open System
open Microsoft.FSharp.Collections

[<CustomEquality; CustomComparison>]
type Element = 
    { Id: int; Version: string }
    interface IComparable with
        member x.CompareTo(obj) = 
            match obj with
            | :? Element as other -> compare x.Id other.Id
            | _ -> -1
    override x.Equals(obj) = 
        match obj with
        | :? Element as other -> x.Id = other.Id
        | _ -> false
    override x.GetHashCode() = hash x.Id

let oldIntersect (a: Set<'T>) (b: Set<'T>) =
    a |> Set.filter (fun item -> b.Contains item)

let cleanGC () = 
    GC.Collect()
    GC.WaitForPendingFinalizers()
    GC.Collect()

let measure f iterations =
    let sw = Diagnostics.Stopwatch.StartNew()
    for _ in 1 .. iterations do ignore (f())
    sw.Stop()
    float sw.Elapsed.TotalMilliseconds / float iterations

let run name (a: Set<Element>) (b: Set<Element>) =
    printf "Benchmarking %-25s... " name
    cleanGC()
    let oldTime = measure (fun () -> oldIntersect a b) 5
    cleanGC()
    let newTime = measure (fun () -> Set.intersect a b) 5
    let result = Set.intersect a b
    let idOk = if not result.IsEmpty then (result.MinimumElement).Version = "FromA" else true
    printfn "Done"
    (name, oldTime, newTime, (oldTime/newTime), idOk)

printfn "Generating Data..."
let hA = Set.ofSeq (seq { for i in 1 .. 1_000_000 -> { Id = i; Version = "FromA" } })
let hB = Set.ofSeq (seq { for i in 1 .. 1_000_000 -> { Id = i; Version = "FromB" } })
let tB = Set.ofSeq (seq { for i in 1 .. 10 -> { Id = i; Version = "FromB" } })
let tA = Set.ofSeq (seq { for i in 1 .. 10 -> { Id = i; Version = "FromA" } })

let res = [
    run "Huge(a) ∩ Tiny(b)" hA tB
    run "Tiny(a) ∩ Huge(b)" tA hB
    run "Huge(a) ∩ Huge(b)" hA hB
]

printfn "\n%-25s | %12s | %12s | %8s | %s" "Scenario" "Old (Actual)" "New (Actual)" "Speedup" "Identity OK"
printfn "%s" (String.replicate 85 "-")
for (n, o, nw, s, i) in res do
    printfn "%-25s | %10.4f ms | %10.4f ms | %7.2fx | %11b" n o nw s i

printfn "\nAssembly: %s" (typeof<Set<int>>.Assembly.Location)