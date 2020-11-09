// #Regression #Conformance #TypeConstraints 
// Verify error when treating F#-defined classes as null
//<Expects id="FS0043" status="error">The type 'Foo' does not have 'null' as a proper value</Expects>

type Foo() =
    member this.Value = 1

let printFoo (x : Foo) =
    match x with
    | null -> printfn "Foo is null!"
    | f    -> printfn "Foo has value %d" f.Value

exit 1
