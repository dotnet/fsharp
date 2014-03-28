// #Conformance #TypeConstraints #Regression
// Regression for 95481, previously this gave an error on the function definition that a type parameter was missing a constraint

let inline sincos< ^t when ^t : (static member Sin : ^t -> ^t)
                       and ^t : (static member Cos : ^t -> ^t)> (a: ^t) =
   let y = sin a
   let x = cos a
   y, x

let r = sincos 3.0

exit 0

