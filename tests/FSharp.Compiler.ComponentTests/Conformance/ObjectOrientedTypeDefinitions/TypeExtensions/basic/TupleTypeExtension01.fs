// #Conformance #ObjectOrientedTypes #TypeExtensions

// Verify ability to define and use SRTP extensions on tuple types using syntactic tuple syntax

open System

type Int32 with
    static member (++) (x1: int, x2: int) = x1 + x2

type ('T1 * 'T2) with
    static member inline (<*>) ((a, f), (b, x)) = (a ++ b, f x)

let x1 = (1, string) <*> (2, 3)
if x1 <> (3, "3") then exit 1

// Also test with non-inline
type ('T1 * 'T2) with
    static member PairFirst ((a, _b)) = a

let x2 = System.Tuple<int, string>.PairFirst((42, "hello"))
if x2 <> 42 then exit 1

exit 0
