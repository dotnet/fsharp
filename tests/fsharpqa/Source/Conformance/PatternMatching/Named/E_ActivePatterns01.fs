// #Regression #Conformance #PatternMatching #ActivePatterns 
// Verify error if Active Patterns do not start with an upper case letter
//<Expects id="FS0623" status="error" span="(8,7)">Active pattern case identifiers must begin with an uppercase letter</Expects>
//<Expects id="FS0623" status="error" span="(8,16)">Active pattern case identifiers must begin with an uppercase letter</Expects>
//<Expects id="FS0624" status="error" span="(9,7)">The '\|' character is not permitted in active pattern case identifiers</Expects>
//<Expects id="FS0624" status="error" span="(10,9)">The '\|' character is not permitted in active pattern case identifiers</Expects>

let (|positive|negative|) n = if n < 0 then positive else negative
let (|``D|E``|F|) (x:int) = if x = 0 then D elif x = 1 then E else F
let (|G|``H||I``|) (x:int) = if x = 0 then G elif x = 1 then H else ``|I``

exit 1
