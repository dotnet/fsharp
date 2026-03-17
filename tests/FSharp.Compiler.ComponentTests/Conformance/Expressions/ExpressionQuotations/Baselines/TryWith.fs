// #Regression #Conformance #Quotations 
// Regression test for FSHARP1.0:5644

open System
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open QuoteUtils

exception E of int

let q = <@ try let x = 1 in raise (E 3) with | :? ArgumentException as e when e.InnerException <> null -> -1 | E x -> x @>
let q' = Expr.TryWith(
            Expr.Value(0), 
            Var("f", typeof<string>), Expr.Value(1), 
            Var("c", typeof<Exception>), Expr.Call(typeof<System.Environment>.GetMethod("Exit"), [Expr.Value(1)]))
            
let r1 = verify q (|TryWith|_|) "TryWith (Let (x, Value (1),
              Call (None, Raise, [Coerce (NewObject (E, Value (3)), Exception)])),
         matchValue,
         IfThenElse (TypeTest (ArgumentException, matchValue),
                     IfThenElse (Let (e, Call (None, UnboxFast, [matchValue]),
                                      Call (None, op_Inequality,
                                            [PropertyGet (Some (e),
                                                          InnerException, []),
                                             Value (<null>)])),
                                 Let (e, Call (None, UnboxFast, [matchValue]),
                                      Value (1)), Value (0)),
                     IfThenElse (TypeTest (E, matchValue),
                                 Let (x,
                                      PropertyGet (Some (Coerce (matchValue, E)),
                                                   Data0, []), Value (1)),
                                 Value (0))), matchValue,
         IfThenElse (TypeTest (ArgumentException, matchValue),
                     IfThenElse (Let (e, Call (None, UnboxFast, [matchValue]),
                                      Call (None, op_Inequality,
                                            [PropertyGet (Some (e),
                                                          InnerException, []),
                                             Value (<null>)])),
                                 Let (e, Call (None, UnboxFast, [matchValue]),
                                      Value (-1)), Call (None, Reraise, [])),
                     IfThenElse (TypeTest (E, matchValue),
                                 Let (x,
                                      PropertyGet (Some (Coerce (matchValue, E)),
                                                   Data0, []), x),
                                 Call (None, Reraise, []))))"
let r2 = verify q' (|TryWith|_|) "TryWith (Value (0), f, Value (1), c, Call (None, Exit, [Value (1)]))"
exit <| if r1 = 0 && r2 = 0 then 0 else 1
