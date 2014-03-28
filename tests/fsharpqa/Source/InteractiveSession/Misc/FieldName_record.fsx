// #Regression #NoMT #FSI 
// Regression test for FSHARP1.0:5208
// Field lookup across multiple compilations (see the ;; right after the let)
// - with a record

type R = { x1 : int }
let g1 = { x1 = 1 };;

g1.x1;;   // BUG OBSERVED HERE

exit 0;;
