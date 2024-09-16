// #Regression #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
// FSB 1007, internal error tripped by property member without this


type DUType =
    | A
    | B of int
    | C of DUType * DUType option
    override ToString() = "No 'this' provided."

