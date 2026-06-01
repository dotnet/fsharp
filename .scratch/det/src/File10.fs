module File10

let processTuple10 (a: int, b: string) =
    let inner x = x + a
    let nested () = inner 42
    (nested (), b.Length)

let anon10 () =
    {| Name = "f10"; Index = 10; Children = [| 1; 2; 3 |] |}

let anon10b () =
    {| Tag = "T10"; Value = 10 * 7; Extras = "x" |}

let useAnon10 () =
    let r = anon10 ()
    let r2 = anon10b ()
    let composed (k: int) =
        let h x = x + r.Index + r2.Value + k
        h 100
    composed 0 + r.Name.Length

[<Struct>]
type Rec10 = { X: int; Y: string }

let mkRec10 () = { X = 10; Y = "rec10" }

let mainCall10 () =
    let _ = processTuple10 (1, "a")
    let _ = useAnon10 ()
    let _ = mkRec10 ()
    ()
