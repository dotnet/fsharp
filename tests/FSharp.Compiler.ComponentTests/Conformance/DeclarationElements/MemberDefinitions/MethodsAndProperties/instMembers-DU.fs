// #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
#light

// Testing instance methods on discriminated unions
type VanillaDU =
    | A of int
    | B of string
    | C
    member this.GetValue param1 param2 =
        match this with
        | A x   -> x.ToString()
        | B x   -> x.ToString()
        | C     -> "C"

let vdu1 = B "C"
let vdu2 = C
if vdu1.GetValue [] () <> vdu2.GetValue [] () then failwith "Failed: 1"

