// #Conformance #TypeInference 
#light

// Verify the difference between 'unit' and 'no args'

type Foo<'a>() =
    // Taking no arguments
    member this.DoStuff ()       = true
    // Taking argument of type 'a
    member this.DoStuff (x : 'a) = 1

let test = new Foo<unit>()

// To pass 'unit' as a value, you must warp it in parentheses
let unitCall  = test.DoStuff(())
let noArgCall = test.DoStuff()

if unitCall <> 1 then exit 1
if noArgCall <> true then exit 1

exit 0
