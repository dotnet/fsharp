// #Regression #Conformance #DataExpressions #ComputationExpressions 
// Regression test for FSHARP1.0:1234
// Combine results in "computation expressions" when not using the #light syntax
// comp-expr := 
//    | comp-expr ; comp-expr	-- combine results
//<Expects status="success"></Expects>
#indent "off"

module M

let two_list = [ yield 1; yield 2 ]
let two_seq  = seq { yield 1; yield 2 }
let two = [  yield "start";
            for x in ["a";"b";"c"] do yield x;
           done;
          yield "end"; ]

