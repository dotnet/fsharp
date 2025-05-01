// #Conformance #DeclarationElements #MemberDefinitions #OptionalDefaultParameterValueArguments


//<Expects>The DefaultParameterValue attribute and any Optional attribute will be ignored.</Expects>
//<Expects>Note: 'null' needs to be annotated with the correct type, e.g. 'DefaultParameterValue(null:obj)'.</Expects>

open System.Runtime.InteropServices

type Class() =
    static member WrongType([<Optional;DefaultParameterValue(1)>]b:string) = b

let r = Class.WrongType("123")
let r2 = Class.WrongType()

do ()