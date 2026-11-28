module Sample
type Safe<'T>(mark: 'T) =
    static let mutable stage = 0
    static member Set v = stage <- v
    member _.Mark = mark
    [<NoCompilerInlining>]
    static member private Secret() = stage + 1
    member _.Run<'U>(u: 'U) =
        let rec go (n: int) (acc: int) (p: 'U) =
            if n <= 0 then acc
            elif box p = null then acc - 1
            else skip (n - 1) (acc + Safe<'T>.Secret()) p
        and skip (n: int) (acc: int) (p: 'U) =
            if n % 2 = 0 then skip (n - 1) acc p
            else go (n - 1) acc p
        go 1000 0 u
[<EntryPoint>]
let main _ =
    let s = Safe<int>(0)
    Safe<int>.Set 41
    if s.Run<string>("x") = 21000 then 0 else 1
