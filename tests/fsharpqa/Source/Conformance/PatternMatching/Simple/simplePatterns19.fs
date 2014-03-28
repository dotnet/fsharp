// #Conformance #PatternMatching 
#light

// Pattern match long unicode literals

[<Literal>]
let UnicodeString1 = "\U00000000\UFFFFFFFF"

[<Literal>]
let UnicodeString2 = "\U11111111\U22222222"

let testStr x =
    match x with
    | UnicodeString1 -> 1
    | UnicodeString2 -> 2
    | _ -> 0

if testStr "foo" <> 0 then exit 1

if testStr UnicodeString1         <> 1 then exit 1
if testStr "\U00000000\UFFFFFFFF" <> 1 then exit 1

if testStr UnicodeString2         <> 2 then exit 1
if testStr "\U11111111\U22222222" <> 2 then exit 1

exit 0
