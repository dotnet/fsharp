// #Regression #Conformance #DeclarationElements #MemberDefinitions #Overloading 
// Regression for FSHARP1.0:5803
// Compiler was tripping up on this before

module M

type Vector() =
    static member (+)(v1:Vector,v2) = 0

let foo (v1:Vector) v2 : int = v1 + v2
