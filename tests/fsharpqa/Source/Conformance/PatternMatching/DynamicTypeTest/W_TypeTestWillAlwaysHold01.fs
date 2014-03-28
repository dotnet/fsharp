// #Regression #Conformance #PatternMatching #TypeTests 
#light

// Verify warning for when dynamic type test will always hold.
//<Expects id="FS0067" status="warning">This type test or downcast will always hold</Expects>


type Foo() =
    member this.Value = 42

let test (x : Foo) =
    match x with
    | :? Foo as xAsInt -> false
    | _ -> false


exit 0
