module File3

let processTuple3 (a: int, b: string) =
    let inner x = x + a
    let nested () = inner 42
    (nested (), b.Length)

let anon3 () =
    {| Name = "f3"; Index = 3; Children = [| 1; 2; 3 |] |}

let anon3b () =
    {| Tag = "T3"; Value = 3 * 7; Extras = "x" |}

let useAnon3 () =
    let r = anon3 ()
    let r2 = anon3b ()
    let composed (k: int) =
        let h x = x + r.Index + r2.Value + k
        h 100
    composed 0 + r.Name.Length

[<Struct>]
type Rec3 = { X: int; Y: string }

let mkRec3 () = { X = 3; Y = "rec3" }

let mainCall3 () =
    let _ = processTuple3 (1, "a")
    let _ = useAnon3 ()
    let _ = mkRec3 ()
    ()
