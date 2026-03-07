// #Regression #Conformance #TypeInference #Attributes 
// Verify error when not fully qualifying a record field when it
// has the RequireQualifiedAccess attribute.

//<Expects id="FS0039" status="error">The record label 'Field1' is not defined\.</Expects>

[<RequireQualifiedAccess>]
type R = { Field1 : int; Field2 : string }

// Error 1
let test1 = { Field1 = 1; R.Field2 = null }

// Error 2
do match test1 with
   | { Field1 = x; Field2 = y } -> ()
   | _ -> ()

exit 1
