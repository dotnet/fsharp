// #Conformance #Constants 
#if TESTS_AS_APP
module Core_int32
#endif

#light

let failures = ref []

let report_failure (s : string) = 
    stderr.Write" NO: "
    stderr.WriteLine s
    failures := !failures @ [s]

let test (s : string) b = 
    stderr.Write(s)
    if b then stderr.WriteLine " OK"
    else report_failure (s)


(* TEST SUITE FOR Int32 *)



do stdout.WriteLine "checking unchecked conversions"; 
#if MONO // https://github.com/fsharp/fsharp/issues/186
#else
do test "testb3" (try nativeint 0.0 = 0n with _ -> false)
#endif
do test "testnr6" (try int64 0.0 = 0L with _ -> false)
do test "testn46" (try int32 0.0 = 0 with _ -> false)
do test "testqb3" (try int16 0.0 = 0s with _ -> false)
do test "testn4" (try sbyte 0.0 = 0y with _ -> false)
do test "testn4" (try int8 0.0 = 0y with _ -> false)

do test "test4b" (try unativeint 0.0 = 0un with _ -> false)
do test "test75j" (try uint64 0.0 = 0UL with _ -> false)
do test "test4n6" (try uint32 0.0 = 0u with _ -> false)
do test "testc24q" (try uint16 0.0 = 0us with _ -> false)
do test "testv43" (try byte 0.0 = 0uy with _ -> false)
do test "testv43" (try uint8 0.0 = 0uy with _ -> false)

do test "testv3w" (try unativeint 10.0E100 |> ignore ; true  with _ -> false)
do test "testv3q" (try uint64 10.0E100 |> ignore ; true  with _ -> false)
do test "testv3" (try uint32 10.0E100 |> ignore ; true  with _ -> false)
do test "tesvtv3" (try uint16 10.0E100 |> ignore ; true  with _ -> false)
do test "testvq34" (try byte 10.0E100 |> ignore; true with _ -> false)
do test "testvq34" (try uint8 10.0E100 |> ignore; true with _ -> false)

do test "testb4wy" (try nativeint 10.0E100 |> ignore ; true  with _ -> false)
do test "testb4w" (try int64 10.0E100 |> ignore ; true  with _ -> false)
do test "testnr" (try int32 10.0E100 |> ignore ; true  with _ -> false)
do test "testjy" (try int16 10.0E100 |> ignore ; true  with _ -> false)
do test "testny" (try sbyte 10.0E100 |> ignore; true with _ -> false)
do test "testny" (try int8 10.0E100 |> ignore; true with _ -> false)


do test "test3fwe" (try Checked.(+) 0.0 1.0 = 1.0 with _ -> false)
do test "testc4" (try Checked.(-) 1.0 0.0 = 1.0 with _ -> false)
do test "testc34" (try Checked.( * ) 0.0 0.0 = 0.0 with _ -> false)

do test "testv3" (try Checked.(+) 0.0f 1.0f = 1.0f with _ -> false)
do test "testv5" (try Checked.(-) 1.0f 0.0f = 1.0f with _ -> false)
do test "test4y5b" (try Checked.( * ) 0.0f 0.0f = 0.0f with _ -> false)

do stdout.WriteLine "checking checked conversions"; 
do test "test" (try Checked.nativeint 0.0 = 0n with _ -> false)
do test "test" (try Checked.int64 0.0 = 0L with _ -> false)
do test "test" (try Checked.int32 0.0 = 0 with _ -> false)
do test "test" (try Checked.int16 0.0 = 0s with _ -> false)
do test "test" (try Checked.sbyte 0.0 = 0y with _ -> false)

do test "test" (try Checked.unativeint 0.0 = 0un with _ -> false)
do test "test" (try Checked.uint64 0.0 = 0UL with _ -> false)
do test "test" (try Checked.uint32 0.0 = 0u with _ -> false)
do test "test" (try Checked.uint16 0.0 = 0us with _ -> false)
do test "test" (try Checked.byte 0.0 = 0uy with _ -> false)
do test "test" (try Checked.uint8 0.0 = 0uy with _ -> false)

do test "test" (try Checked.nativeint 0.0f = 0n with _ -> false)
do test "test" (try Checked.int64 0.0f = 0L with _ -> false)
do test "test" (try Checked.int32 0.0f = 0 with _ -> false)
do test "test" (try Checked.int16 0.0f = 0s with _ -> false)
do test "test" (try Checked.sbyte 0.0f = 0y with _ -> false)
do test "test" (try Checked.int8 0.0f = 0y with _ -> false)

do test "test" (try Checked.unativeint 0.0f = 0un with _ -> false)
do test "test" (try Checked.uint64 0.0f = 0UL with _ -> false)
do test "test" (try Checked.uint32 0.0f = 0u with _ -> false)
do test "test" (try Checked.uint16 0.0f = 0us with _ -> false)
do test "test" (try Checked.byte 0.0f = 0uy with _ -> false)
do test "test" (try Checked.uint8 0.0f = 0uy with _ -> false)

do test "test" (try Checked.nativeint 0L = 0n with _ -> false)
do test "test" (try Checked.int64 0L = 0L with _ -> false)
do test "test" (try Checked.int32 0L = 0 with _ -> false)
do test "test" (try Checked.int16 0L = 0s with _ -> false)
do test "test" (try Checked.sbyte 0L = 0y with _ -> false)
do test "test" (try Checked.int8 0L = 0y with _ -> false)

do test "test" (try Checked.unativeint 0L = 0un with _ -> false)
do test "test" (try Checked.uint64 0L = 0UL with _ -> false)
do test "test" (try Checked.uint32 0L = 0u with _ -> false)
do test "test" (try Checked.uint16 0L = 0us with _ -> false)
do test "test" (try Checked.byte 0L = 0uy with _ -> false)
do test "test" (try Checked.uint8 0L = 0uy with _ -> false)

do test "test" (try Checked.nativeint 0UL = 0n with _ -> false)
do test "test" (try Checked.int64 0UL = 0L with _ -> false)
do test "test" (try Checked.int32 0UL = 0 with _ -> false)
do test "test" (try Checked.int16 0UL = 0s with _ -> false)
do test "test" (try Checked.sbyte 0UL = 0y with _ -> false)
do test "test" (try Checked.int8 0UL = 0y with _ -> false)

do test "test" (try Checked.unativeint 0UL = 0un with _ -> false)
do test "test" (try Checked.uint64 0UL = 0UL with _ -> false)
do test "test" (try Checked.uint32 0UL = 0u with _ -> false)
do test "test" (try Checked.uint16 0UL = 0us with _ -> false)
do test "test" (try Checked.byte 0UL = 0uy with _ -> false)
do test "test" (try Checked.uint8 0UL = 0uy with _ -> false)

do test "test" (try Checked.nativeint 0 = 0n with _ -> false)
do test "test" (try Checked.int64 0 = 0L with _ -> false)
do test "test" (try Checked.int32 0 = 0 with _ -> false)
do test "test" (try Checked.int16 0 = 0s with _ -> false)
do test "test" (try Checked.sbyte 0 = 0y with _ -> false)
do test "test" (try Checked.int8 0 = 0y with _ -> false)

do test "test" (try Checked.unativeint 0 = 0un with _ -> false)
do test "test" (try Checked.uint64 0 = 0UL with _ -> false)
do test "test" (try Checked.uint32 0 = 0u with _ -> false)
do test "test" (try Checked.uint16 0 = 0us with _ -> false)
do test "test" (try Checked.byte 0 = 0uy with _ -> false)
do test "test" (try Checked.uint8 0 = 0uy with _ -> false)

do test "test" (try Checked.nativeint 0u = 0n with _ -> false)
do test "test" (try Checked.int64 0u = 0L with _ -> false)
do test "test" (try Checked.int32 0u = 0 with _ -> false)
do test "test" (try Checked.int16 0u = 0s with _ -> false)
do test "test" (try Checked.sbyte 0u = 0y with _ -> false)
do test "test" (try Checked.int8 0u = 0y with _ -> false)

do test "test" (try Checked.unativeint 0u = 0un with _ -> false)
do test "test" (try Checked.uint64 0u = 0UL with _ -> false)
do test "test" (try Checked.uint32 0u = 0u with _ -> false)
do test "test" (try Checked.uint16 0u = 0us with _ -> false)
do test "test" (try Checked.byte 0u = 0uy with _ -> false)
do test "test" (try Checked.uint8 0u = 0uy with _ -> false)

do test "test" (try Checked.nativeint 0s = 0n with _ -> false)
do test "test" (try Checked.int64 0s = 0L with _ -> false)
do test "test" (try Checked.int32 0s = 0 with _ -> false)
do test "test" (try Checked.int16 0s = 0s with _ -> false)
do test "test" (try Checked.sbyte 0s = 0y with _ -> false)
do test "test" (try Checked.int8 0s = 0y with _ -> false)

do test "test" (try Checked.unativeint 0s = 0un with _ -> false)
do test "test" (try Checked.uint64 0s = 0UL with _ -> false)
do test "test" (try Checked.uint32 0s = 0u with _ -> false)
do test "test" (try Checked.uint16 0s = 0us with _ -> false)
do test "test" (try Checked.byte 0s = 0uy with _ -> false)
do test "test" (try Checked.uint8 0s = 0uy with _ -> false)



do test "test" (try Checked.nativeint 0us = 0n with _ -> false)
do test "test" (try Checked.int64 0us = 0L with _ -> false)
do test "test" (try Checked.int32 0us = 0 with _ -> false)
do test "test" (try Checked.int16 0us = 0s with _ -> false)
do test "test" (try Checked.sbyte 0us = 0y with _ -> false)
do test "test" (try Checked.int8 0us = 0y with _ -> false)

do test "test" (try Checked.unativeint 0us = 0un with _ -> false)
do test "test" (try Checked.uint64 0us = 0UL with _ -> false)
do test "test" (try Checked.uint32 0us = 0u with _ -> false)
do test "test" (try Checked.uint16 0us = 0us with _ -> false)
do test "test" (try Checked.byte 0us = 0uy with _ -> false)
do test "test" (try Checked.uint8 0us = 0uy with _ -> false)

do test "test" (try Checked.nativeint 0uy = 0n with _ -> false)
do test "test" (try Checked.int64 0uy = 0L with _ -> false)
do test "test" (try Checked.int32 0uy = 0 with _ -> false)
do test "test" (try Checked.int16 0uy = 0s with _ -> false)
do test "test" (try Checked.sbyte 0uy = 0y with _ -> false)
do test "test" (try Checked.int8 0uy = 0y with _ -> false)

do test "test" (try Checked.unativeint 0uy = 0un with _ -> false)
do test "test" (try Checked.uint64 0uy = 0UL with _ -> false)
do test "test" (try Checked.uint32 0uy = 0u with _ -> false)
do test "test" (try Checked.uint16 0uy = 0us with _ -> false)
do test "test" (try Checked.byte 0uy = 0uy with _ -> false)
do test "test" (try Checked.uint8 0uy = 0uy with _ -> false)

do test "test" (try Checked.nativeint 0y = 0n with _ -> false)
do test "test" (try Checked.int64 0y = 0L with _ -> false)
do test "test" (try Checked.int32 0y = 0 with _ -> false)
do test "test" (try Checked.int16 0y = 0s with _ -> false)
do test "test" (try Checked.sbyte 0y = 0y with _ -> false)
do test "test" (try Checked.int8 0y = 0y with _ -> false)

do test "test" (try Checked.unativeint 0y = 0un with _ -> false)
do test "test" (try Checked.uint64 0y = 0UL with _ -> false)
do test "test" (try Checked.uint32 0y = 0u with _ -> false)
do test "test" (try Checked.uint16 0y = 0us with _ -> false)
do test "test" (try Checked.byte 0y = 0uy with _ -> false)
do test "test" (try Checked.uint8 0y = 0uy with _ -> false)


do test "test" (try Checked.nativeint 0n = 0n with _ -> false)
do test "test" (try Checked.int64 0n = 0L with _ -> false)
do test "test" (try Checked.int32 0n = 0 with _ -> false)
do test "test" (try Checked.int16 0n = 0s with _ -> false)
do test "test" (try Checked.sbyte 0n = 0y with _ -> false)
do test "test" (try Checked.int8 0n = 0y with _ -> false)

do test "test" (try Checked.unativeint 0n = 0un with _ -> false)
do test "test" (try Checked.uint64 0n = 0UL with _ -> false)
do test "test" (try Checked.uint32 0n = 0u with _ -> false)
do test "test" (try Checked.uint16 0n = 0us with _ -> false)
do test "test" (try Checked.byte 0n = 0uy with _ -> false)
do test "test" (try Checked.uint8 0n = 0uy with _ -> false)

do test "test" (try Checked.nativeint 0un = 0n with _ -> false)
do test "test" (try Checked.int64 0un = 0L with _ -> false)
do test "test" (try Checked.int32 0un = 0 with _ -> false)
do test "test" (try Checked.int16 0un = 0s with _ -> false)
do test "test" (try Checked.sbyte 0un = 0y with _ -> false)
do test "test" (try Checked.int8 0un = 0y with _ -> false)

do test "test" (try Checked.unativeint 0un = 0un with _ -> false)
do test "test" (try Checked.uint64 0un = 0UL with _ -> false)
do test "test" (try Checked.uint32 0un = 0u with _ -> false)
do test "test" (try Checked.uint16 0un = 0us with _ -> false)
do test "test" (try Checked.byte 0un = 0uy with _ -> false)
do test "test" (try Checked.uint8 0un = 0uy with _ -> false)



do test "test" (try Checked.unativeint 10.0E100 |> ignore ; false with _ -> true)
do test "test" (try Checked.uint64 10.0E100 |> ignore ; false with _ -> true)
do test "test" (try Checked.uint32 10.0E100 |> ignore ; false with _ -> true)
do test "test" (try Checked.uint16 10.0E100 |> ignore ; false with _ -> true)
do test "test" (try Checked.byte 10.0E100 |> ignore; false with _ -> true)
do test "test" (try Checked.uint8 10.0E100 |> ignore; false with _ -> true)

do test "test" (try Checked.nativeint 10.0E100 |> ignore ; false with _ -> true)
do test "test" (try Checked.int64 10.0E100 |> ignore ; false with _ -> true)
do test "test" (try Checked.int32 10.0E100 |> ignore ; false with _ -> true)
do test "test" (try Checked.int16 10.0E100 |> ignore ; false with _ -> true)
do test "test" (try Checked.sbyte 10.0E100 |> ignore; false with _ -> true)
do test "test" (try Checked.int8 10.0E100 |> ignore; false with _ -> true)

let fails f x = try ignore (f x); false with _ -> true
let succeeds f x = not (fails f x)

do stdout.WriteLine "further checks on checked conversions"; 
do test "test" (fails Checked.byte 0xFFy)
do test "test" (fails Checked.uint16 0xFFy)
do test "test" (fails Checked.uint16 0xFFFFs)
do test "test" (fails Checked.uint32 0xFFy)
do test "test" (fails Checked.uint32 0xFFFFs)
do test "test" (fails Checked.uint32 0xFFFFFFFF)
do test "test" (fails Checked.uint64 0xFFy)
do test "test" (fails Checked.uint64 0xFFFFs)
do test "test" (fails Checked.uint64 0xFFFFFFFF)
do test "test" (fails Checked.uint64 0xFFFFFFFFFFFFFFFFL)

do test "test" (fails Checked.sbyte 0xFFuy)
do test "test" (succeeds Checked.int16 0xFFuy)
do test "test" (fails Checked.int16 0xFFFFus)
do test "test" (succeeds Checked.int32 0xFFuy)
do test "test" (succeeds Checked.int32 0xFFFFus)
do test "test" (fails Checked.int32 0xFFFFFFFFu)
do test "test" (succeeds Checked.int64 0xFFuy)
do test "test" (succeeds Checked.int64 0xFFFFus)
do test "test" (succeeds Checked.int64 0xFFFFFFFFu)
do test "test" (fails Checked.int64 0xFFFFFFFFFFFFFFFFUL)
do test "test" (fails Checked.int64 0x8000000000000000UL)
do test "test" (succeeds Checked.int64 0x7FFFFFFFFFFFFFFFUL)

do stdout.WriteLine "still further checks on checked conversions"; 
do test "test" (succeeds Checked.sbyte (-0x80s))
do test "test" (fails Checked.sbyte (-0x81s))
do test "test" (succeeds Checked.sbyte (-0x80))
do test "test" (fails Checked.sbyte (-0x81))
do test "test" (succeeds Checked.sbyte (-0x80L))
do test "test" (fails Checked.sbyte (-0x81L))
do test "test" (succeeds Checked.sbyte (-0x80n))
do test "test" (fails Checked.sbyte (-0x81n))

do test "test" (succeeds Checked.int16 (-0x8000))
do test "test" (fails Checked.int16 (-0x8001))
do test "test" (succeeds Checked.int16 (-0x8000L))
do test "test" (fails Checked.int16 (-0x8001L))
do test "test" (succeeds Checked.int16 (-0x8000n))
do test "test" (fails Checked.int16 (-0x8001n))

do test "test" (succeeds Checked.int32 (-0x80000000L))
do test "test" (fails Checked.int32 (-0x80000001L))


do stdout.WriteLine "done checking checked ops"

do test "test" (int32 0xFFy = -1)
do test "test" (uint32 0xFFy = 4294967295u)
do test "test" (int32 0xFFuy = 255)
do test "test" (uint32 0xFFuy = 255u)

module MinMaxAbs32 = begin
        do test "ceijoe9cewz1" (min 0 -1 = -1)
        do test "ceijoe9cewz2" (min -1 0 = -1)
        do test "ceijoe9cewz3" (min 1 0 = 0)
        do test "ceijoe9cewz4" (min 0 1 = 0)

        do test "ceijoe9cewz5" (max 0 -1 = 0)
        do test "ceijoe9cewz6" (max -1 0 = 0)
        do test "ceijoe9cewz7" (max 1 0 = 1)
        do test "ceijoe9cewz8" (max 0 1 = 1)
        do test "ceijoe9cewz9" (max 1 1 = 1)
        do test "ceijoe9cewz0" (max 1 1 = 1)

        do test "ceijoe9cewzA" (abs 0 = 0)
        do test "ceijoe9cewzB" (abs -1 = 1)
        do test "ceijoe9cewzC" (abs 1 = 1)
        do test "ceijoe9cewzD" (abs System.Int32.MaxValue = System.Int32.MaxValue)
        do test "ceijoe9cewzE" (abs (System.Int32.MinValue + 1) = System.Int32.MaxValue)
        do test "ceijoe9cewzF" (try abs System.Int32.MinValue |> ignore; false with :? System.OverflowException -> true)
end

module MinMaxAbs64 = begin
        do test "ceijoe9cewz1" (min 0L -1L = -1L)
        do test "ceijoe9cewz2" (min -1L 0L = -1L)
        do test "ceijoe9cewz3" (min 1L 0L = 0L)
        do test "ceijoe9cewz4" (min 0L 1L = 0L)

        do test "ceijoe9cewz5" (max 0L -1L = 0L)
        do test "ceijoe9cewz6" (max -1L 0L = 0L)
        do test "ceijoe9cewz7" (max 1L 0L = 1L)
        do test "ceijoe9cewz8" (max 0L 1L = 1L)
        do test "ceijoe9cewz9" (max 1L 1L = 1L)
        do test "ceijoe9cewz0" (max 1L 1L = 1L)

        do test "ceijoe9cewzA" (abs 0L = 0L)
        do test "ceijoe9cewzB" (abs -1L = 1L)
        do test "ceijoe9cewzC" (abs 1L = 1L)
        do test "ceijoe9cewzD" (abs System.Int64.MaxValue = System.Int64.MaxValue)
        do test "ceijoe9cewzE" (abs (System.Int64.MinValue + 1L) = System.Int64.MaxValue)
        do test "ceijoe9cewzF" (try abs System.Int64.MinValue |> ignore; false with :? System.OverflowException -> true)
end


module MinMaxAbs16 = begin
        do test "ceijoe9cewz1" (min 0s -1s = -1s)
        do test "ceijoe9cewz2" (min -1s 0s = -1s)
        do test "ceijoe9cewz3" (min 1s 0s = 0s)
        do test "ceijoe9cewz4" (min 0s 1s = 0s)

        do test "ceijoe9cewz5" (max 0s -1s = 0s)
        do test "ceijoe9cewz6" (max -1s 0s = 0s)
        do test "ceijoe9cewz7" (max 1s 0s = 1s)
        do test "ceijoe9cewz8" (max 0s 1s = 1s)
        do test "ceijoe9cewz9" (max 1s 1s = 1s)
        do test "ceijoe9cewz0" (max 1s 1s = 1s)

        do test "ceijoe9cewzA" (abs 0s = 0s)
        do test "ceijoe9cewzB" (abs -1s = 1s)
        do test "ceijoe9cewzC" (abs 1s = 1s)
        do test "ceijoe9cewzD" (abs System.Int16.MaxValue = System.Int16.MaxValue)
        do test "ceijoe9cewzE" (abs (System.Int16.MinValue + 1s) = System.Int16.MaxValue)
        do test "ceijoe9cewzF" (try abs System.Int16.MinValue |> ignore; false with :? System.OverflowException -> true)
end

module MinMaxAbs8 = begin
        do test "ceijoe9cewz1" (min 0y -1y = -1y)
        do test "ceijoe9cewz2" (min -1y 0y = -1y)
        do test "ceijoe9cewz3" (min 1y 0y = 0y)
        do test "ceijoe9cewz4" (min 0y 1y = 0y)

        do test "ceijoe9cewz5" (max 0y -1y = 0y)
        do test "ceijoe9cewz6" (max -1y 0y = 0y)
        do test "ceijoe9cewz7" (max 1y 0y = 1y)
        do test "ceijoe9cewz8" (max 0y 1y = 1y)
        do test "ceijoe9cewz9" (max 1y 1y = 1y)
        do test "ceijoe9cewz0" (max 1y 1y = 1y)

        do test "ceijoe9cewzA" (abs 0y = 0y)
        do test "ceijoe9cewzB" (abs -1y = 1y)
        do test "ceijoe9cewzC" (abs 1y = 1y)
        do test "ceijoe9cewzD" (abs System.SByte.MaxValue = System.SByte.MaxValue)
        do test "ceijoe9cewzE" (abs (System.SByte.MinValue + 1y) = System.SByte.MaxValue)
        do test "ceijoe9cewzF" (try abs System.SByte.MinValue |> ignore; false with :? System.OverflowException -> true)
end

module MinMaxAbsNative = begin
        do test "ceijoe9cewz1" (min 0n -1n = -1n)
        do test "ceijoe9cewz2" (min -1n 0n = -1n)
        do test "ceijoe9cewz3" (min 1n 0n = 0n)
        do test "ceijoe9cewz4" (min 0n 1n = 0n)

        do test "ceijoe9cewz5" (max 0n -1n = 0n)
        do test "ceijoe9cewz6" (max -1n 0n = 0n)
        do test "ceijoe9cewz7" (max 1n 0n = 1n)
        do test "ceijoe9cewz8" (max 0n 1n = 1n)
        do test "ceijoe9cewz9" (max 1n 1n = 1n)
        do test "ceijoe9cewz0" (max 1n 1n = 1n)

        do test "ceijoe9cewzA" (abs 0n = 0n)
        do test "ceijoe9cewzB" (abs -1n = 1n)
        do test "ceijoe9cewzC" (abs 1n = 1n)
        do if sizeof<nativeint> = 4 then 
              test "ceijoe9cewzD1" (abs (nativeint System.Int32.MaxValue) = (nativeint System.Int32.MaxValue));
              test "ceijoe9cewzF2" (abs (nativeint System.Int32.MinValue + 1n) = nativeint System.Int32.MaxValue);
              test "ceijoe9cewzG3" (try abs (nativeint System.Int32.MinValue) |> ignore; false with :? System.OverflowException -> true)
           elif sizeof<nativeint> = 8 then 
              test "ceijoe9cewzD4" (abs (nativeint System.Int64.MaxValue) = (nativeint System.Int64.MaxValue));
              test "ceijoe9cewzF5" (abs (nativeint System.Int64.MinValue + 1n) = nativeint System.Int64.MaxValue);
              test "ceijoe9cewzG6" (try abs (nativeint System.Int64.MinValue) |> ignore; false with :? System.OverflowException -> true)
           
end

#if TESTS_AS_APP
let RUN() = !failures
#else
let aa =
  match !failures with 
  | [] -> 
      stdout.WriteLine "Test Passed"
      printf "TEST PASSED OK" ;
      exit 0
  | _ -> 
      stdout.WriteLine "Test Failed"
      exit 1
#endif

