module File5

let processTuple5 (a: int, b: string) =
    let inner x = x + a
    let nested () = inner 42
    (nested (), b.Length)

let anon5 () =
    {| Name = "f5"; Index = 5; Children = [| 1; 2; 3 |] |}

let anon5b () =
    {| Tag = "T5"; Value = 5 * 7; Extras = "x" |}

let useAnon5 () =
    let r = anon5 ()
    let r2 = anon5b ()
    let composed (k: int) =
        let h x = x + r.Index + r2.Value + k
        h 100
    composed 0 + r.Name.Length

[<Struct>]
type Rec5 = { X: int; Y: string }

let mkRec5 () = { X = 5; Y = "rec5" }

let mainCall5 () =
    let _ = processTuple5 (1, "a")
    let _ = useAnon5 ()
    let _ = mkRec5 ()
    ()
