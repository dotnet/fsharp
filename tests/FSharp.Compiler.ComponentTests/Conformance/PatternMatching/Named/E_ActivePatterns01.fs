// #Regression #Conformance #PatternMatching #ActivePatterns 
// Verify error if Active Patterns do not start with an upper case letter









let (|positive|negative|) n = if n < 0 then positive else negative
let (|`` A``|) (x:int) = x
let (|B1|``+B2``|) (x:int) = if x = 0 then OneA else ``+B2``
let (|`` C``|_|) (x:int) = if x = 0 then Some(x) else None
let (|``D|E``|F|) (x:int) = if x = 0 then D elif x = 1 then E else F
let (|G|``H||I``|) (x:int) = if x = 0 then G elif x = 1 then H else ``|I``
let (|_J|) (x:int) = _J

exit 1
