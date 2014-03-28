// #Regression #Conformance #Quotations 
// Regression test for FSHARP1.0:5339
// Make sure nested quotations are printing correctly. Nested quotation here used to print x in + expression as UnitVar0
#light

open Microsoft.FSharp.Quotations

let q = <@ fun () -> <@ fun x -> x + 1 @> @>
let x = sprintf "%A" q

let expected = "Lambda (unitVar0, Quote (Lambda (x, Call (None, op_Addition, [x, Value (1)]))))"
                             
exit <| if (x.Replace("\r\n", "\n") = expected.Replace("\r\n", "\n")) 
                                      then 0 
                                      else
                                          printfn "Expected:"
                                          printfn "========="
                                          printfn "%s" expected
                                          printfn "Actual:"
                                          printfn "========="
                                          printfn "%s" x
                                          1
