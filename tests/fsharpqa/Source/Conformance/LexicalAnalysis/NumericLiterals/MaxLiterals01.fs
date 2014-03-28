// #Conformance #LexicalAnalysis #Constants 
#light

// Verify ability to specify min and max for literals

let minUByte =   0uy
let maxUByte = 255uy

let minByte = -128y
let midByte =    0y
let maxByte =  127y

let minUInt16 =     0us
let maxUInt16 = 65535us

let minInt16 = -32768s
let midInt16 =      0s
let maxInt16 =  32767s

let minUInt32 =          0u
let maxUInt32 = 4294967295u

let minInt32 = -2147483648
let midInt32 =           0
let maxInt32 =  2147483647

let minUInt64 =                    0UL
let maxUInt64 = 18446744073709551615UL

let minInt64 = -9223372036854775808L
let midInt64 =           0
let maxInt64 =  9223372036854775807L

// Decimals only store 28 digits, but we will truncate this
// to prevent throwing an exception at runtime.
let longDecimalLit = 0.00000000000000000000000000000000000000000M
if longDecimalLit <> 0.0M then exit 1

// If this compiles OK, then we are good to go.
exit 0
