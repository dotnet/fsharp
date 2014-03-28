// #Regression #Conformance #DeclarationElements #LetBindings #TypeTests 
// Verify error asssociated with putting type functions inside types
//<Expects id="FS0671" status="error" span="(6,12)">A property cannot have explicit type parameters\. Consider using a method instead\.$</Expects>

type Foo() =
    member this.TypeFunc<'a> = typeof<'a>.Name
