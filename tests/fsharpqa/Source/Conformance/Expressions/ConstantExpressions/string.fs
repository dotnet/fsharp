// #Conformance #Constants 
// Constant expressions
// This is the example quoted from the specs
// Verification:
// - all the different types work
// - the type we get is what we meant it to be
#light 


let v1 = "3"            // string          (String)
let v2 = "c:\\home"     // string          (System.String)
let v3 = @"c:\home"     // string          (Verbatim Unicode, System.String)
let v4 = @"@"           // string

let check(x:string) = true

exit (if (check(v1) && check(v2) && check(v3) && check(v4)) then 0 else 1)
