// Test string literals with short Unicode Literals

let mutable failure = false

let checkStr (inputStr:string) expectedChars expectedStr =
    let charCodes = inputStr.ToCharArray() |> Array.map int
    if charCodes <> expectedChars then
        printfn "Character encodings don't match"
        printfn "  Expected %A" expectedChars
        printfn "  Actual   %A" charCodes
        false
    else
    match expectedStr with
    | Some(exp) when exp <> inputStr ->
        printfn "String representation doesn't match"
        printfn "  Expected %s" exp
        printfn "  Actual   %s" inputStr
        false
    | _ -> true

let test (inputStr:string) expectedChars expectedStr =
    failure <- (checkStr inputStr expectedChars expectedStr) && failure
    
test "\u2660\u2663\u2665\u2666" [| 0x2660; 0x2663; 0x2665; 0x2666 |] (Some("♠♣♥♦"))
test "\uD800 \uDBFF \uDC00 \uDFFF" [| 0xD800; 32; 0xDBFF; 32; 0xDC00; 32; 0xDFFF |] None
test "\u0000\u0000\uFFFE\uFFFD\uFFFC" [| 0; 0; 0xFFFE; 0xFFFD; 0xFFFC |] None
test "\uD900\uD901\uD902" [| 0xD900; 0xD901; 0xD902 |] None

exit (if failure then 1 else 0)
