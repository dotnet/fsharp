// #Conformance #ObjectOrientedTypes #TypeExtensions

// Verify SRTP resolution finds extension methods on tuple types via System.Tuple lookup

open System

type System.Tuple<'T1, 'T2> with
    static member inline Swap ((a, b)) = (b, a)

let x1 = System.Tuple<int, string>.Swap((1, "hello"))
if x1 <> ("hello", 1) then exit 1

// SRTP should find this through tuple type extension lookup
type ('T1 * 'T2) with
    static member inline (<**>) ((a, f), (b, x)) = (f b, a x)

let x2 = (string, int) <**> (42, "7")
if x2 <> ("42", 7) then exit 1

exit 0
