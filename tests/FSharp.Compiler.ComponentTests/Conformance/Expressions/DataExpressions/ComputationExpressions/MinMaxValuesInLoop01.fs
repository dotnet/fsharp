// #Regression #Conformance #DataExpressions #ComputationExpressions 
// Regression test for FSHARP1.0:4370
// for loops involving System.Int32.MaxValue as the upper limit does not work correctly

let mutable acc = 0

// add 2 to accumulator
for i = (System.Int32.MaxValue-1) to (System.Int32.MaxValue) do acc <- acc + 1
// now extract 2 from accumulator
for i = (System.Int32.MinValue) to (System.Int32.MinValue+1) do acc <- acc - 1


// now trying with while loops
let mutable i = System.Int32.MaxValue - 2
while i < System.Int32.MaxValue do
    acc <- acc + 1
    i <- i + 1

i <- System.Int32.MinValue + 2
while i > System.Int32.MinValue do
    acc <- acc - 1
    i <- i - 1
    
if acc <> 0 then exit 1

exit 0
