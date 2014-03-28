// #Regression #Conformance #Quotations 
#light
// Regression test for FSHARP1.0:2546 
//<Expects status=success></Expects>

let quote = 
    <@ 
        let mutable x = 0
        while x < 10 do 
            x <- x + 1             // we used to emit an error here - no more!
        x
        
        
    @>
 
let quote2 = 
    <@ 
        let mutable x = 0
        for i = 1 to 10 do 
            x <- x + 1
        x
    @>

let x = sprintf "%A" quote
let y = sprintf "%A" quote2

let x1 = "Let (x, Value (0),
     Sequential (WhileLoop (Call (None, op_LessThan, [x, Value (10)]),
                            VarSet (x, Call (None, op_Addition, [x, Value (1)]))),
                 x))"
let y1 = "Let (x, Value (0),
     Sequential (ForIntegerRangeLoop (i, Value (1), Value (10),
                                      VarSet (x,
                                              Call (None, op_Addition,
                                                    [x, Value (1)]))), x))"

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
