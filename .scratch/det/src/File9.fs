module File9

let processTuple9 (a: int, b: string) =
    let inner x = x + a
    let nested () = inner 42
    (nested (), b.Length)

let anon9 () =
    {| Name = "f9"; Index = 9; Children = [| 1; 2; 3 |] |}

let anon9b () =
    {| Tag = "T9"; Value = 9 * 7; Extras = "x" |}

let useAnon9 () =
    let r = anon9 ()
    let r2 = anon9b ()
    let composed (k: int) =
        let h x = x + r.Index + r2.Value + k
        h 100
    composed 0 + r.Name.Length

[<Struct>]
type Rec9 = { X: int; Y: string }

let mkRec9 () = { X = 9; Y = "rec9" }

let mainCall9 () =
    let _ = processTuple9 (1, "a")
    let _ = useAnon9 ()
    let _ = mkRec9 ()
    ()
