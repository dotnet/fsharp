// #Regression #Conformance #DeclarationElements #MemberDefinitions #MethodsAndProperties 
// FSB 1007, internal error tripped by property member without this
//<Expects id="FS0673" status="error">This instance member needs a parameter to represent the object being invoked</Expects>

type DUType =
    | A
    | B of int
    | C of DUType * DUType option
    override ToString() = "No 'this' provided."

