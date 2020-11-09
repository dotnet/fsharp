// #Conformance #TypeConstraints 
#light

// Verify the ability to state type parameters in modules

let tupify (x : 'a) = (x, x)

let emptyList : 'a list = []

// Note that x and y are constrained to have the same type
let twoPartTripple x y z : ('a * 'a * 'b) = (x, y, z)

exit 0
