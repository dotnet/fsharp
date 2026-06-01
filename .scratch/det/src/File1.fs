module File1

let processTuple1 (a: int, b: string) =
    let inner x = x + a
    let nested () = inner 42
    (nested (), b.Length)

let anon1 () =
    {| Name = "f1"; Index = 1; Children = [| 1; 2; 3 |] |}

let anon1b () =
    {| Tag = "T1"; Value = 1 * 7; Extras = "x" |}

let useAnon1 () =
    let r = anon1 ()
    let r2 = anon1b ()
    let composed (k: int) =
        let h x = x + r.Index + r2.Value + k
        h 100
    composed 0 + r.Name.Length

[<Struct>]
type Rec1 = { X: int; Y: string }

let mkRec1 () = { X = 1; Y = "rec1" }

let mainCall1 () =
    let _ = processTuple1 (1, "a")
    let _ = useAnon1 ()
    let _ = mkRec1 ()
    ()
