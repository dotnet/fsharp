// #Regression #Conformance #Quotations 
// Regression for Dev10:844084
// Previously the second quote would leave the op_Splice operator in the quoted tree

let f, arg = <@ sin @>, <@ 1.0 @>
let pq = <@ (%arg) |> (%f) @>
let q = <@ (%f) (%arg) @>

let x = sprintf "%A" pq
let y = sprintf "%A" q

let x1 = "Call (None, op_PipeRight,
      [Value (1.0), Lambda (value, Call (None, Sin, [value]))])"

let y1 = "Application (Lambda (value, Call (None, Sin, [value])), Value (1.0))"

exit <| if (x.Replace("\r\n", "\n") = x1.Replace("\r\n", "\n")) && (y.Replace("\r\n", "\n") = y1.Replace("\r\n", "\n")) then 
                                      0 
                                else
                                      if(x.Replace("\r\n", "\n") <> x1.Replace("\r\n", "\n")) then
                                          printfn "Expected:"
                                          printfn "========="
                                          printfn "%s" x1
                                          printfn "Actual:"
                                          printfn "========="
                                          printfn "%s" x
                                      if(y.Replace("\r\n", "\n") <> y1.Replace("\r\n", "\n")) then    
                                          printfn "Expected:"
                                          printfn "========="
                                          printfn "%s" y1
                                          printfn "Actual:"
                                          printfn "========="
                                          printfn "%s" y
                                      1

exit 0
