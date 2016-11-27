// #Conformance #DeclarationElements #MemberDefinitions #OptionalDefaultParameterValueArguments

//<Expects id="FS3211" status="error"> The default value does not have the same type as the argument.</Expects>

open System.Runtime.InteropServices

type Class() =
    static member WrongType([<Optional;DefaultParameterValue(1)>]b:string) = b

let r = Class.WrongType()

do ()