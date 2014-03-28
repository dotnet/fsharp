// #Conformance #ControlFlow #Sequences 
#light

// Test Sequence iteration expressions (for loops).

// Somewhat complex expression in the 'in' clause.

let mutable sum = (0, 0)
for x, y in [1..10] |> List.map (fun i -> (i, 1)) do
    let a, b = sum
    sum <- a + x, b + y

let result1, result2 = sum

if result1 <> 55 then exit 1
if result2 <> 10 then exit 1

exit 0
