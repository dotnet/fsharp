// #Regression #NoMT #FSI 
// Regression test for FSHARP1.0:5208
// Field lookup across multiple compilations (see the ;; right after the let)
// - with a class

type G =
    val mutable x1 : int
    new (x1) = {x1=x1}
let g1 = G(1);;

g1.x1;;   // BUG OBSERVED HERE

exit 0;;
