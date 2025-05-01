// #Regression #Conformance #PatternMatching 
#light

// Verify error when trying to match type against null if it doesn't support that


type Foo() =
    member this.Value = 42



let isNull2 (x : Foo) =
    match x with
    | null -> true
    | _ -> false


exit 1
