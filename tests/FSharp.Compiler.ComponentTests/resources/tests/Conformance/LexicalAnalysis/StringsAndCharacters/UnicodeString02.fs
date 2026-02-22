// Test string literals with long Unicode Literals

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
    
test "\U00002660\U00002663\U00002665\U00002666" [| 0x2660; 0x2663; 0x2665; 0x2666 |] (Some("♠♣♥♦"))
test "\U0000D800 \U0000DBFF \U0000DC00 \U0000DFFF" [| 0xD800; 32; 0xDBFF; 32; 0xDC00; 32; 0xDFFF |] None
test "\U00000000\U00000000\U0000FFFE\U0000FFFD\U0000FFFC" [| 0; 0; 0xFFFE; 0xFFFD; 0xFFFC |] None
test "\U0000D900\U0000D901\U0000D902" [| 0xD900; 0xD901; 0xD902 |] None
test "\U00010437" [| 0xD801; 0xDC37;|] (Some("𐐷"))
test "\U00024B62" [| 0xD852 ; 0xDF62 |] (Some("𤭢"))

exit (if failure then 1 else 0)
