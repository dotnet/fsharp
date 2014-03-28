// #Conformance #PatternMatching 
#light

// Pattern match short unicode literals in strings

[<Literal>]
let UnicodeString1 = "\u0000\uFFFF"

[<Literal>]
let UnicodeString2 = "\u1111\u2222"

let testStr x =
    match x with
    | UnicodeString1 -> 1
    | UnicodeString2 -> 2
    | _ -> 0

if testStr "foo" <> 0 then exit 1

if testStr UnicodeString1         <> 1 then exit 1
if testStr "\u0000\uFFFF" <> 1 then exit 1

if testStr UnicodeString2         <> 2 then exit 1
if testStr "\u1111\u2222" <> 2 then exit 1

exit 0
