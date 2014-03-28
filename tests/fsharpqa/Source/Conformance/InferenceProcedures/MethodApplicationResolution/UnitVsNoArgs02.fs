// #Conformance #TypeInference 
// Verify we allow units to be passed as values to generic functions

type 'a GenericType() =

     member this.TakesUnit (x : unit) = true

     member this.Takes'a   (x : 'a)   = true



// Explicit
let explicitTests = new GenericType<unit> ()
if explicitTests.TakesUnit ( () ) <> true then exit 1
if explicitTests.Takes'a (())     <> true then exit 1

// From value
let x = ()

let fromValue = new GenericType<unit> ()
if fromValue.TakesUnit x <> true then exit 1
if fromValue.Takes'a   x <> true then exit 1

exit 0
