
let (|Int32Const|_|) (a: SynConst) = match a with SynConst.Int32 _ -> Some a | _ -> None
