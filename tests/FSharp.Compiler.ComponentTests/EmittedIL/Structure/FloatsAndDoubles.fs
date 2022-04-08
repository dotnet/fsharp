// ===========================================================================================================================
//  Test case for GenericEqualityERFast with floats/doubles
//  Ensures that matrix of values evaluate to the same values as the shipping compiler
// ===========================================================================================================================
module floatsanddoubles

open System
open System

type Float =
    struct
        val F : float
        new (f:float) = { F = f }
    end

type Double =
    struct
        val D : double
        new (d:double) = { D = d }
    end

let floats  = [| Float(Double.Epsilon); Float(Double.MinValue); Float(Double.MaxValue);Float(Double.NegativeInfinity);Float(Double.PositiveInfinity);Float(Double.NaN); Float(7.0)|]
let doubles = [| Double(Double.Epsilon); Double(Double.MinValue); Double(Double.MaxValue);Double(Double.NegativeInfinity);Double(Double.PositiveInfinity);Double(Double.NaN); Double(7.0)|]
let names = [| "Epsilon"; "MinValue"; "MaxValue";"NegativeInfinity";"PositiveInfinity";"NaN";"Number" |]

[<EntryPoint>]
let main argv =

    for i in 0 .. doubles.Length - 1  do
        for j in 0 .. doubles.Length - 1 do
            printfn "Doubles:   %-17s = %-17s  is:  %-5b   Values %f = %f" (names.[i]) (names.[j]) (doubles.[i].Equals(doubles.[j])) (doubles.[i].D) (doubles.[j].D)
        printfn ""

    for i in 0 .. floats.Length - 1  do
        for j in 0 .. floats.Length - 1 do
            printfn "Floats:    %-17s = %-17s  is:  %-5b   Values %f = %f" (names.[i]) (names.[j]) (floats.[i].Equals(floats.[j])) (floats.[i].F) (floats.[j].F)
        printfn ""

    0 // return an integer exit code
