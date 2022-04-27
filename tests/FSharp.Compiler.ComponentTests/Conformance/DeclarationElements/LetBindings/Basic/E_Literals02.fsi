// #Regression #Conformance #DeclarationElements #LetBindings 
//<Expects status="error" span="(12,1)" id="FS0876">A declaration may only be the \[<Literal>\] attribute if a constant value is also given, e\.g\. 'val x: int = 1'$</Expects>
//<Expects status="error" span="(15,1)" id="FS0876">A declaration may only be the \[<Literal>\] attribute if a constant value is also given, e\.g\. 'val x: int = 1'$</Expects>


// FSB 1981, Signature must contain the literal value

// The literal value must be given in the signature as well: 
// we must be able to type check against a signature alone, 
// and literal values are relevant to things like redundant pattern match detection (you can use literals in patterns)
module M
[<Literal>]
val test1: string

[<Literal>]
val test2: int
