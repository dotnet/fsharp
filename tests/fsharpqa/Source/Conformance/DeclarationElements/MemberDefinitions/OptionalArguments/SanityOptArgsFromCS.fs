// #Conformance #DeclarationElements #MemberDefinitions #OptionalArguments #NETFX40Only 
// Should be no errors here
module M

open TestLib

let a = new T()
let x = a.NonNullOptArg()
let y = a.ValueTypeOptArg()

exit 0
