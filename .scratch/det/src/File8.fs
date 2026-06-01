module File8

let processTuple8 (a: int, b: string) =
    let inner x = x + a
    let nested () = inner 42
    (nested (), b.Length)

let anon8 () =
    {| Name = "f8"; Index = 8; Children = [| 1; 2; 3 |] |}

let anon8b () =
    {| Tag = "T8"; Value = 8 * 7; Extras = "x" |}

let useAnon8 () =
    let r = anon8 ()
    let r2 = anon8b ()
    let composed (k: int) =
        let h x = x + r.Index + r2.Value + k
        h 100
    composed 0 + r.Name.Length

[<Struct>]
type Rec8 = { X: int; Y: string }

let mkRec8 () = { X = 8; Y = "rec8" }

let mainCall8 () =
    let _ = processTuple8 (1, "a")
    let _ = useAnon8 ()
    let _ = mkRec8 ()
    ()
