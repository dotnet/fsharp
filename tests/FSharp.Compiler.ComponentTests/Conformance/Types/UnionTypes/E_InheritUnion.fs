// #Regression #Conformance #TypesAndModules #Unions 
// Verify error when inherit from union types
//<Expects id="FS0961" status="error">This 'inherit' declaration specifies the inherited type but no arguments.</Expects>
//<Expects id="FS0945" status="error">Cannot inherit a sealed type</Expects>

type DiscUnion = A of int | B of string


type Foo() =
    inherit DiscUnion

    member this.Stuff = 1


exit 1
