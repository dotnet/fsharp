// #Regression #Conformance #DeclarationElements #Attributes 
// Attribute 
//<Expects status="error" span="(8,5-8,20)" id="FS0824">Attributes are not permitted on 'let' bindings in expressions$</Expects>

[<Class>]
type A() = inherit System.Attribute()

let foo ( [<A>] x ) = 1 in foo 2 + foo 3

let foo2 ( [<A>] x ) ( [<A>] y ) = 1 in foo2 2 3 + foo2 4 5    

