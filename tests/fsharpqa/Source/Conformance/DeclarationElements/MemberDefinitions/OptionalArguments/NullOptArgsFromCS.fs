// #Regression #Conformance #DeclarationElements #MemberDefinitions #OptionalArguments #NETFX40Only 
// Dev10: 811195. Internal parse error when calling C#/VB methods with optional args whose default value is null
// Should be no errors here
module M

open TestLib

let a = new T()
let x = a.NullOptArg()
let y = a.NullOptArg("hi")
let z = a.NullableOptArg()

exit 0
