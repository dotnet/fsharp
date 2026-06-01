module File7

let processTuple7 (a: int, b: string) =
    let inner x = x + a
    let nested () = inner 42
    (nested (), b.Length)

let anon7 () =
    {| Name = "f7"; Index = 7; Children = [| 1; 2; 3 |] |}

let anon7b () =
    {| Tag = "T7"; Value = 7 * 7; Extras = "x" |}

let useAnon7 () =
    let r = anon7 ()
    let r2 = anon7b ()
    let composed (k: int) =
        let h x = x + r.Index + r2.Value + k
        h 100
    composed 0 + r.Name.Length

[<Struct>]
type Rec7 = { X: int; Y: string }

let mkRec7 () = { X = 7; Y = "rec7" }

let mainCall7 () =
    let _ = processTuple7 (1, "a")
    let _ = useAnon7 ()
    let _ = mkRec7 ()
    ()
