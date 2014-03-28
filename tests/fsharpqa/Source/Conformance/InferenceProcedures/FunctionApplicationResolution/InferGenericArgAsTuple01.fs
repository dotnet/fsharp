// #Conformance #TypeInference #ApplicationExpressions 
#light

// Verify we can infer generic type args to be a tuple without adding
// additional parens.

open System.Collections.Generic

// The generic typearg of l is unknown...
let l = new List<_>()

// Method 'Add' doesn't take multiple args, just a single T
// So the typegarg of l is inferred to be a tuple.
l.Add(1, '2', "3", 4.0M)

// Return the element T, which is a four-element tuple
let w, x, y, z = l.[0]

if w <> 1    then exit 1
if x <> '2'  then exit 1
if y <> "3"  then exit 1
if z <> 4.0M then exit 1

exit 0
