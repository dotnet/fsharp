module File6

let processTuple6 (a: int, b: string) =
    let inner x = x + a
    let nested () = inner 42
    (nested (), b.Length)

let anon6 () =
    {| Name = "f6"; Index = 6; Children = [| 1; 2; 3 |] |}

let anon6b () =
    {| Tag = "T6"; Value = 6 * 7; Extras = "x" |}

let useAnon6 () =
    let r = anon6 ()
    let r2 = anon6b ()
    let composed (k: int) =
        let h x = x + r.Index + r2.Value + k
        h 100
    composed 0 + r.Name.Length

[<Struct>]
type Rec6 = { X: int; Y: string }

let mkRec6 () = { X = 6; Y = "rec6" }

let mainCall6 () =
    let _ = processTuple6 (1, "a")
    let _ = useAnon6 ()
    let _ = mkRec6 ()
    ()
