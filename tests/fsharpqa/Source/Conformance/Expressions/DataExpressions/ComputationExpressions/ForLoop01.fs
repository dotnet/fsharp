// #Conformance #DataExpressions #ComputationExpressions 
#light

// Verify use of 'for i = ... in Computation Expressions

let f = seq { for i = 1 to 10 do
                yield i }
if Seq.toList f <> [ 1; 2; 3; 4; 5; 6; 7; 8; 9; 10 ] then exit 1

exit 0
