// #Conformance #Constants 
// Constant expressions
// This is the example quoted from the specs
// Verification:
// - all the different types work
// - the type we get is what we meant it to be
//<Expects status="success"></Expects>

open System.Numerics                    // <- we need this since the deprecated 'bigint' has been moved here

let v1 = 99999999I                      // bigint          (System.Numerics.BigInteger)

let check(x:BigInteger) = true

exit (if check(v1) then 0 else 1)
