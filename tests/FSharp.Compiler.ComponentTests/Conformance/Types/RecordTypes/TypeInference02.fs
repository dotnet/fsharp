// #Conformance #TypesAndModules #Records 
#light

// Verify no errors for type inference with ambiguous types
// and type annotations.

type Red   = { A : char }
type Blue  = { A : char; B : int }
type Green = { A : char; B : int; C : string }

let test1 () =
    let t : Red = { A = 'a' }
    t.A

let test2 () =
    let funcExpectsBlue (x : Blue) = x.B
    let temp : Blue = { A = ' '; B = 42 }
    (funcExpectsBlue temp)

let test3 () = 
    // This shouldn't be ambig, since only one record has a C field
    let g = { A = '1'; B = 0; C = "abc" }
    g.C

if test1() <> 'a'   then failwith "Failed"
if test2() <> 42    then failwith "Failed"
if test3() <> "abc" then failwith "Failed"
