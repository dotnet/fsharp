// #Conformance #TypeInference #Attributes 
// Verify the RequireQualifiedAccess attribute works on records

[<RequireQualifiedAccess>]
type R = { Field1 : int; Field2 : string }

let test1 = { R.Field1 = 1; R.Field2 = null }

// Can omit param for 'Field1' since R is inferred
let test2 : R = { Field1 = 1; R.Field2 = null }

do match test1 with
   | { R.Field1 = x; R.Field2 = y } -> exit 0
   | _ -> exit 1
