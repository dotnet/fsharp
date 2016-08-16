// #Regression #Conformance #PatternMatching #ActivePatterns 
// Verify error if Active Patterns do not start with an upper case letter
//<Expects id="FS0623" status="error" span="(12,7)">Active pattern case identifiers must begin with an uppercase letter</Expects>
//<Expects id="FS0623" status="error" span="(12,16)">Active pattern case identifiers must begin with an uppercase letter</Expects>
//<Expects id="FS0623" status="error" span="(13,7)">Active pattern case identifiers must begin with an uppercase letter</Expects>
//<Expects id="FS0623" status="error" span="(14,10)">Active pattern case identifiers must begin with an uppercase letter</Expects>
//<Expects id="FS0623" status="error" span="(15,7)">Active pattern case identifiers must begin with an uppercase letter</Expects>
//<Expects id="FS0624" status="error" span="(16,7)">The '\|' character is not permitted in active pattern case identifiers</Expects>
//<Expects id="FS0624" status="error" span="(17,9)">The '\|' character is not permitted in active pattern case identifiers</Expects>
//<Expects id="FS3210" status="error" span="(19,15)">A is an active pattern and cannot be treated as a discriminated union case with named fields.</Expects>

let (|positive|negative|) n = if n < 0 then positive else negative
let (|`` A``|) (x:int) = x
let (|B1|``+B2``|) (x:int) = if x = 0 then OneA else ``+B2``
let (|`` C``|_|) (x:int) = if x = 0 then Some(x) else None
let (|``D|E``|F|) (x:int) = if x = 0 then D elif x = 1 then E else F
let (|G|``H||I``|) (x:int) = if x = 0 then G elif x = 1 then H else ``|I``
let (|A|B|) n = if n%2 = 0 then A n else B n
match 10 with A(hoho=n) -> n | _ -> 0

exit 1
