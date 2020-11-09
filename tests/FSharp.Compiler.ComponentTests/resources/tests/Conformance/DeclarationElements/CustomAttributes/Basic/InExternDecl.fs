// #Regression #Conformance #DeclarationElements #Attributes 
// Regression test for FSHARP1.0:4192
// We should not give any warning when attribute is used on extern declaration

module M

type myAttrib() = inherit System.Attribute()
extern int A([<myAttrib>] int a)
