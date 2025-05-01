// #Conformance #PatternMatching 
#light

// Pattern match long unicode literals

[<Literal>]
let UnicodeString1 = "\U00000000\U0002FFFF"

[<Literal>]
let UnicodeString2 = "\U00101111\U000F2222"

let testStr x =
    match x with
    | UnicodeString1 -> 1
    | UnicodeString2 -> 2
    | _ -> 0

if testStr "foo" <> 0 then exit 1

if testStr UnicodeString1         <> 1 then exit 1
if testStr "\U00000000\U0002FFFF" <> 1 then exit 1

if testStr UnicodeString2         <> 2 then exit 1
if testStr "\U00101111\U000F2222" <> 2 then exit 1

exit 0
