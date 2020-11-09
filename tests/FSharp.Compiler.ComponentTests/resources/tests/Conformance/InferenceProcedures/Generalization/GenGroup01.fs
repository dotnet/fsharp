// #Conformance #TypeInference 
#light

// Verify you generalization applies to groups of methods at once
// (Let bindings)

let rec f x = (g x, g x)
and     g x = [x]

if f 5 <> ([5], [5]) then exit 1

exit 0
