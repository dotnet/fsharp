module File4

let processTuple4 (a: int, b: string) =
    let inner x = x + a
    let nested () = inner 42
    (nested (), b.Length)

let anon4 () =
    {| Name = "f4"; Index = 4; Children = [| 1; 2; 3 |] |}

let anon4b () =
    {| Tag = "T4"; Value = 4 * 7; Extras = "x" |}

let useAnon4 () =
    let r = anon4 ()
    let r2 = anon4b ()
    let composed (k: int) =
        let h x = x + r.Index + r2.Value + k
        h 100
    composed 0 + r.Name.Length

[<Struct>]
type Rec4 = { X: int; Y: string }

let mkRec4 () = { X = 4; Y = "rec4" }

let mainCall4 () =
    let _ = processTuple4 (1, "a")
    let _ = useAnon4 ()
    let _ = mkRec4 ()
    ()
