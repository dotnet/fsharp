// #Regression #Conformance #DeclarationElements #Attributes 
// Attribute 
//<Expects status="error" span="(8,7-8,8)" id="FS0842">This attribute is not valid for use on this language element</Expects>

[<Class>]
type A() = inherit System.Attribute()

let [<A>] x : int = 1 in x + x 
