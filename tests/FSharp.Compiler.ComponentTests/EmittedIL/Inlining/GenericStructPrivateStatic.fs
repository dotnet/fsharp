module Sample
[<Struct>]
type Ticker<'T> =
    val Stage: int
    [<NoCompilerInlining>]
    static member private Advance (v: int) = v + 1
    member this.Run() =
        let rec walk (n: int) (acc: int) =
            if n <= 0 then acc
            else walk (n - 1) (Ticker<'T>.Advance acc)
        walk 1000 this.Stage
[<EntryPoint>]
let main _ =
    let t = Ticker<int>()
    if t.Run() = 1000 then 0 else 1
