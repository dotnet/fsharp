module File12

let processTuple12 (a: int, b: string) =
    let inner x = x + a
    let nested () = inner 42
    (nested (), b.Length)

let anon12 () =
    {| Name = "f12"; Index = 12; Children = [| 1; 2; 3 |] |}

let anon12b () =
    {| Tag = "T12"; Value = 12 * 7; Extras = "x" |}

let useAnon12 () =
    let r = anon12 ()
    let r2 = anon12b ()
    let composed (k: int) =
        let h x = x + r.Index + r2.Value + k
        h 100
    composed 0 + r.Name.Length

[<Struct>]
type Rec12 = { X: int; Y: string }

let mkRec12 () = { X = 12; Y = "rec12" }

let mainCall12 () =
    let _ = processTuple12 (1, "a")
    let _ = useAnon12 ()
    let _ = mkRec12 ()
    ()
