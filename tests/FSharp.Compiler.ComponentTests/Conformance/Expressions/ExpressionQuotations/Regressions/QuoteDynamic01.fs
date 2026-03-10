// #Regression #Conformance #Quotations 
//Regression for FSHARP1.0:6030
//Quoting a class member ? results in error about 'trait members' but quoting it as op_Dynamic is ok
// Confirm it works fine for let bound operators

// Let bound functions handle this ok
let (?) o s =
    printfn "%s" s

let x = 1
let q1 = <@ x ? hello @>
let q2 = <@ op_Dynamic x "hello" @>

exit 0
