// #Conformance #BasicGrammarElements #Operators 
#light

// Verify you can negate any numeric value
let negByte  (x : sbyte) = -x
let negInt16 (x : int16) = -x
let negInt32 (x : int32) = -x
let negInt64 (x : int64) = -x

let negFloat   (x : float)   = -x
let negFloat32 (x : float32) = -x
let negDecimal (x : decimal) = -x

// Tests
if negByte  1y <> -1y then failwith "Failed: 1"
if negInt16 1s <> -1s then failwith "Failed: 2"
if negInt32 1  <> -1  then failwith "Failed: 3"
if negInt64 1L <> -1L then failwith "Failed: 4"

if negFloat   1.0  <> -1.0  then failwith "Failed: 5"
if negFloat32 1.0f <> -1.0f then failwith "Failed: 6"
if negDecimal 1.0M <> -1.0M then failwith "Failed: 7"
