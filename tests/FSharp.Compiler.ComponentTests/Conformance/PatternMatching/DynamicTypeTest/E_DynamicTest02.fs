let stuff(x: obj) =
    match x with
    | :? option<int> -> 0x200
    | null -> 0x100
    | _ -> 0x500