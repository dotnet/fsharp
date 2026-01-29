// #Regression #Conformance #ControlFlow 
// Regression for Dev10:850869
// Previously, using an uppercase identifier in the loop variable caused an error

for A = 0 to 100 do
  A = A - 1 |> ignore

for ``A.X`` = 0 to 100 do
  ``A.X`` = ``A.X`` - 1 |> ignore

for A in 0 .. 100 do
  A = A - 1 |> ignore

for A in 0 .. 2 .. 100 do
  A = A - 1 |> ignore

for ``A.X`` in 0 .. 100 do
  ``A.X`` = ``A.X`` - 1 |> ignore

let X = 1

for X in 0..5 do
    ()

let s = seq { for X in 1..100 -> X }

exit 0
