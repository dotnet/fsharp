// #Conformance #LexicalAnalysis #Constants  
// Verify ability to write custom big num libraries

type DerivedFrom = FromInt32 | FromInt64 | FromString

module NumericLiteralG =
    let IAmImplemented = true
    
    let FromInt32(i : int)     = (i, FromInt32)
    let FromInt64(i : int64)   = (i, FromInt64)
    let FromString(s : string) = (s, FromString)
    
let int32Range      = -2147483647G
if int32Range <> (-2147483647, FromInt32) then exit 1

let int64Range      = 9223372036854775807G
if int64Range <> (9223372036854775807L, FromInt64) then exit 1

let fromStringRange = 12345678901234567890G
if fromStringRange <> ("12345678901234567890", FromString) then exit 1

exit 0
