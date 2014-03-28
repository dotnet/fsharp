// #Conformance #TypeInference #TypeConstraints 
#light

// Verify ability to have unknown open variables if
// they are internal.

let test1() =
    let _ = ref [] // Normally a Value restriction, unknown what type 'ref []' is 'a list
    ()


type Test() =
    let m_x = ref []

if test1() <> () then exit 1

let test2 = new Test()

exit 0
