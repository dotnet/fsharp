module File11

let processTuple11 (a: int, b: string) =
    let inner x = x + a
    let nested () = inner 42
    (nested (), b.Length)

let anon11 () =
    {| Name = "f11"; Index = 11; Children = [| 1; 2; 3 |] |}

let anon11b () =
    {| Tag = "T11"; Value = 11 * 7; Extras = "x" |}

let useAnon11 () =
    let r = anon11 ()
    let r2 = anon11b ()
    let composed (k: int) =
        let h x = x + r.Index + r2.Value + k
        h 100
    composed 0 + r.Name.Length

[<Struct>]
type Rec11 = { X: int; Y: string }

let mkRec11 () = { X = 11; Y = "rec11" }

let mainCall11 () =
    let _ = processTuple11 (1, "a")
    let _ = useAnon11 ()
    let _ = mkRec11 ()
    ()
