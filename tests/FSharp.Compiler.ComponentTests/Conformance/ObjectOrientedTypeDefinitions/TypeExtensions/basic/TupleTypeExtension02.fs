// #Conformance #ObjectOrientedTypes #TypeExtensions

// Verify ability to define and use SRTP extensions on struct tuple types using syntactic tuple syntax

open System

type Int32 with
    static member (++) (x1: int, x2: int) = x1 + x2

type struct ('T1 * 'T2) with
    static member inline (<*>) (struct (a, f), struct (b, x)) = struct (a ++ b, f x)

let x1 = struct (1, string) <*> struct (2, 3)
if x1 <> struct (3, "3") then exit 1

// Also test with non-inline
type struct ('T1 * 'T2) with
    static member PairFirst (struct (a, _b)) = a

let x2 = System.ValueTuple<int, string>.PairFirst(struct (42, "hello"))
if x2 <> 42 then exit 1

exit 0
