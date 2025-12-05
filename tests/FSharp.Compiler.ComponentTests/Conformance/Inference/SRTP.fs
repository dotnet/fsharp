namespace FSharp.Compiler.ComponentTests.Conformance.Inference

open FSharp.Test
open Xunit

module SRTP =

    [<Fact>]
    let ``SRTP resolution with curryN and Tuple`` () =
        let source = """
module SRTP_Repro

open System

type Curry =
    static member inline Invoke f =
        let inline call_2 (a: ^a, b: ^b) = ((^a or ^b) : (static member Curry: _*_ -> _) b, a)
        call_2 (Unchecked.defaultof<Curry>, Unchecked.defaultof<'t>) (f: 't -> 'r) : 'args

    static member Curry (_: Tuple<'t1>        , _: Curry) = fun f t1                   -> f (Tuple<_> t1)
    static member Curry (_: Tuple<'t1, 't2>   , _: Curry) = fun f t1 t2                -> f (Tuple<_,_>(t1, t2))

let inline curryN (f: (^``T1 * ^T2 * ... * ^Tn``) -> 'Result) : 'T1 -> '``T2 -> ... -> 'Tn -> 'Result`` = fun t -> Curry.Invoke f t

let f1 (x: Tuple<_>) = [x.Item1]
let f2 (x: Tuple<_,_>) = [x.Item1; x.Item2]

let test () =
    let _x1 = curryN f1 100
    let _x2 = curryN f2 10 20
    ()
"""
        CompilerAssert.TypeCheckWithErrors(source)
