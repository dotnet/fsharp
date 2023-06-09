module Test

type MyDu = 
    | A 
    | Other

    static let ofString s = match s with | "A" -> A | _ -> Other
    static member OfString = ofString
    static let toString x = match x with | A -> "A" | Other -> "..."
    static member PrintToString = toString

let myVal = MyDu.OfString "brmbrm"

printfn "%s" (myVal |> MyDu.PrintToString)