// #Conformance #BasicGrammarElements #Operators 
// Test issue FSHARP1.0:902

open System
open Operators.Checked

let testNoOverflow op overflowArg = 
    try 
        let r = op overflowArg 
        ()
    with
        | :? OverflowException -> failwith "Failed: 1"

type T(x : float) =
    member this.Data = x
    static member op_Explicit (x : T) = byte x.Data
    static member op_Explicit (x : T) = char x.Data
    static member op_Explicit (x : T) = int16 x.Data
    static member op_Explicit (x : T) = int32 x.Data
    static member op_Explicit (x : T) = int64 x.Data
    static member op_Explicit (x : T) = nativeint x.Data
    static member op_Explicit (x : T) = sbyte x.Data
    static member op_Explicit (x : T) = uint16 x.Data
    static member op_Explicit (x : T) = uint32 x.Data
    static member op_Explicit (x : T) = uint64 x.Data
    static member op_Explicit (x : T) = unativeint x.Data


testNoOverflow byte 255
testNoOverflow char 123
testNoOverflow int 2147483647L
testNoOverflow int16 32767
testNoOverflow int32 2147483647L
testNoOverflow int64 8446744073709551615.0f
testNoOverflow nativeint 2147483647L
testNoOverflow sbyte 127
testNoOverflow uint16 65535
testNoOverflow uint32 4294967295L
testNoOverflow uint64 8446744073709551615.0f
testNoOverflow unativeint 4294967295L
    
testNoOverflow int -2147483647L
testNoOverflow int16 -32767
testNoOverflow int32 -2147483647L
testNoOverflow int64 -8446744073709551615.0f
testNoOverflow nativeint -2147483647L
testNoOverflow sbyte -127

testNoOverflow int "-123"
testNoOverflow int16 "-32767"
testNoOverflow int32 "-2147483647"
testNoOverflow int64 "-8446744073709551615"
testNoOverflow sbyte "-127"

testNoOverflow int (T(-2147483647.0))
testNoOverflow int16 (T(-32767.0))
testNoOverflow int32 (T(-2147483647.0))
testNoOverflow int64 (T(-8446744073709551615.0))
testNoOverflow nativeint (T(-2147483647.0))
testNoOverflow sbyte (T(-127.0))
