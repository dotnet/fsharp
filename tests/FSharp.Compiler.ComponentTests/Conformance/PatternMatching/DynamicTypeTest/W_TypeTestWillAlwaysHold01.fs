// #Regression #Conformance #PatternMatching #TypeTests 
#light

// Verify warning for when dynamic type test will always hold.



type Foo() =
    member this.Value = 42

let test (x : Foo) =
    match x with
    | :? Foo as xAsInt -> false
    | _ -> false


exit 0
