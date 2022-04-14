open System
open Operators.Checked

let testOverflow op overflowArg = 
    try 
        let r = op overflowArg 
        printfn "%A" overflowArg
        failwith "Failed: 1"
    with
        | :? OverflowException -> ()

// Because there's no way to overflow nativeint/unativeint on 64bit since they won't accept bigint
let testOverflow2 op overflowArg = 
    if System.IntPtr.Size = 8 then         // we cannot use System.Environment.Is64BitProcess because this could be targeting 2.0
        () 
    else
        try 
            let r = op overflowArg 
            printfn "%A" overflowArg
            failwith "Failed: 2"
        with
            | :? OverflowException -> ()

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

testOverflow byte 256
testOverflow char 2147483648L
testOverflow int 2147483648L
testOverflow int16 32768
testOverflow int32 2147483648L
testOverflow int64 18446744073709551616.0f
testOverflow2 nativeint 2147483648L
testOverflow sbyte 128
testOverflow uint16 65536
testOverflow uint32 4294967296L
testOverflow uint64 18446744073709551616.0f
testOverflow2 unativeint 4294967296L

testOverflow byte -256
testOverflow char -2147483648L
testOverflow int -2147483649L
testOverflow int16 -32769
testOverflow int32 -2147483649L
testOverflow int64 -18446744073709551616.0f
testOverflow2 nativeint -2147483649L
testOverflow sbyte -129
testOverflow uint16 -65536
testOverflow uint32 -4294967296L
testOverflow uint64 -18446744073709551616.0f
testOverflow2 unativeint -4294967296L

// nativeint/unativeint don't support strings, char will throw FormatException if str.length > 1 so no overflow 

testOverflow byte "256"
testOverflow int "2147483649"
testOverflow int16 "32768"
testOverflow int32 "2147483648"
testOverflow int64 "18446744073709551616"
testOverflow sbyte "128"
testOverflow uint16 "65536"
testOverflow uint32 "4294967296"
testOverflow uint64 "18446744073709551616"

testOverflow byte "-256"
testOverflow int "-2147483649"
testOverflow int16 "-32769"
testOverflow int32 "-2147483649"
testOverflow int64 "-18446744073709551616"
testOverflow sbyte "-129"
testOverflow uint16 "-65536"
testOverflow uint32 "-4294967296"
testOverflow uint64 "-18446744073709551616"

testOverflow byte (T(256.0))
testOverflow char (T(2147483648.0))
testOverflow int (T(2147483649.0))
testOverflow int16 (T(32768.0))
testOverflow int32 (T(2147483648.0))
testOverflow int64 (T(18446744073709551616.0))
testOverflow2 nativeint (T(2147483648.0))
testOverflow sbyte (T(128.0))
testOverflow uint16 (T(65536.0))
testOverflow uint32 (T(4294967296.0))
testOverflow uint64 (T(18446744073709551616.0))
testOverflow2 unativeint (T(4294967296.0))

testOverflow byte (T(-256.0))
testOverflow char (T(-2147483648.0))
testOverflow int (T(-2147483649.0))
testOverflow int16 (T(-32769.0))
testOverflow int32 (T(-2147483649.0))
testOverflow int64 (T(-18446744073709551616.0))
testOverflow2 nativeint (T(-2147483649.0))
testOverflow sbyte (T(-129.0))
testOverflow uint16 (T(-65536.0))
testOverflow uint32 (T(-4294967296.0))
testOverflow uint64 (T(-18446744073709551616.0))
testOverflow2 unativeint (T(-4294967296.0))
