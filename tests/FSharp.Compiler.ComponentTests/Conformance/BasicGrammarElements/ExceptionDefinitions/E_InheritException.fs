// #Regression #Conformance #TypesAndModules #Exceptions 
// Verify error when trying to inherit from F# exception types
//<Expects id="FS0945" span="(9,5-9,24)" status="error">Cannot inherit a sealed type</Expects>
//<Expects id="FS1133" span="(9,5-9,24)" status="error">No constructors are available for the type 'FSharpExn'</Expects>

exception FSharpExn of int * string

type Foo() =
    inherit FSharpExn()
    member this.Value = 1



