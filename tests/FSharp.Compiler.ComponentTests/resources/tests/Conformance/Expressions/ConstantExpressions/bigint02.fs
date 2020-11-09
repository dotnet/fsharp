// #Conformance #Constants 
// Constant expressions
// This is the example quoted from the specs
// Verification:
// - all the different types work
// - the type we get is what we meant it to be

//<Expects status="success"></Expects>

let v1 = 99999999I      // bigint          (Microsoft.FSharp.Core.bigint)

let check(x:bigint) = true

if check(v1) = false then exit 1

exit 0
