// #Conformance #DeclarationElements #MemberDefinitions #OptionalArguments  
// Should be no errors here
module M

open TestLib

let a = new T()
let x = a.NonNullOptArg()
let y = a.ValueTypeOptArg()
