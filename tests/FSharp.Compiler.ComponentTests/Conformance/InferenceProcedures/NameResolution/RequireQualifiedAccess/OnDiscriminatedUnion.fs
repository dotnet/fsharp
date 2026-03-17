// #Conformance #TypeInference #Attributes 
// Verify the RequireQualifiedAccess attribute works on discriminated unions

[<RequireQualifiedAccess>]
type DiscUnion =
    | A 
    | B of string
    | C of DiscUnion * float


let x = DiscUnion.C(DiscUnion.A, 1.0)

do  match x with
    | DiscUnion.C(a, 1.0) ->
         match a with 
         | DiscUnion.A -> exit 0
         | _ -> exit 1
    | _ -> exit 1
