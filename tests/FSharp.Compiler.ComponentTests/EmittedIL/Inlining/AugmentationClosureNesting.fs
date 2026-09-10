module Sample
type C() =
    static let mutable backing = 0
    static member Set v = backing <- v
    [<NoCompilerInlining>]
    static member private Secret() = backing + 1
type C with
    member _.Run() =
        let rec h n = if n = 0 then C.Secret() else h (n - 1)
        h 5
[<EntryPoint>]
let main _ =
    C.Set 41
    if C().Run() = 42 then 0 else 1
