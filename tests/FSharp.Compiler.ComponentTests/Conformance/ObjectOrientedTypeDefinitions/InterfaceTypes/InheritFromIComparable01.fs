// #Regression #Conformance #ObjectOrientedTypes #InterfacesAndImplementations 
// Regression test for FSHARP1.0:4852
// Verify there is no bad code generation (i.e. pass peverify test)
// when defining a type (interface) that inherits from System.IComparable
//<Expects status="success"></Expects>
module TestModule
type I = inherit System.IComparable
         abstract P : int with get, set
