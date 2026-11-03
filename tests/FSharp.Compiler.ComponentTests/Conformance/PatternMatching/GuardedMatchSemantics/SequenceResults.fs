module Test

let choose (xs: int array) =
    {{OPEN}}
        for n in xs do
            {{PREFIX}}
                match n % 2, n with
                | (0 | 1), value when value >= 2 -> {{SUCCESS}}
                | _ -> {{EMPTY}}
    {{CLOSE}}

[<EntryPoint>]
let main _ =
    let values = [|-1; 0; 1; 2; 3; 4|]
    if (choose values |> Seq.toArray) <> [|2; 3; 4|] then failwith "sequence result"
    if {{ALLOCATION}} then
        let input = [|0..999|]
        let consume () =
            for _ in 1..100 do
                for _ in choose input do ()
        consume ()
        let before = System.GC.GetAllocatedBytesForCurrentThread()
        consume ()
        if System.GC.GetAllocatedBytesForCurrentThread() - before >= 5_000_000L then failwith "per-yield allocation"
    0
