// #Conformance #DeclarationElements #MemberDefinitions #OptionalDefaultParameterValueArguments

//<Expects id="FS3211" status="warning">The default value does not have the same type as the argument. </Expects>
//<Expects>The DefaultParameterValue attribute and any Optional attribute will be ignored.</Expects>
//<Expects>Note: 'null' needs to be annotated with the correct type, e.g. 'DefaultParameterValue(null:obj)'.</Expects>

open System.Runtime.InteropServices

type Class() =
    static member WrongType([<Optional;DefaultParameterValue(1)>]b:string) = b

let r = Class.WrongType("123")

do ()