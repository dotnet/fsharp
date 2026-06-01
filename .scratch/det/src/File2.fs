module File2

let processTuple2 (a: int, b: string) =
    let inner x = x + a
    let nested () = inner 42
    (nested (), b.Length)

let anon2 () =
    {| Name = "f2"; Index = 2; Children = [| 1; 2; 3 |] |}

let anon2b () =
    {| Tag = "T2"; Value = 2 * 7; Extras = "x" |}

let useAnon2 () =
    let r = anon2 ()
    let r2 = anon2b ()
    let composed (k: int) =
        let h x = x + r.Index + r2.Value + k
        h 100
    composed 0 + r.Name.Length

[<Struct>]
type Rec2 = { X: int; Y: string }

let mkRec2 () = { X = 2; Y = "rec2" }

let mainCall2 () =
    let _ = processTuple2 (1, "a")
    let _ = useAnon2 ()
    let _ = mkRec2 ()
    ()
