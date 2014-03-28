// #Regression #Conformance #DataExpressions #ComputationExpressions 
// Regression test for FSHARP1.0:4370
// for loops involving System.Int32.MinValue

let mutable acc = 0

// add 2 to accumulator
for i = (System.Int32.MinValue+1) downto (System.Int32.MinValue) do acc <- acc + 1
// now extract 2 from accumulator
for i = (System.Int32.MaxValue) downto (System.Int32.MaxValue-1) do acc <- acc - 1

if acc <> 0 then exit 1

exit 0
