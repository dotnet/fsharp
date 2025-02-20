// #Conformance #DeclarationElements #MemberDefinitions #OptionalDefaultParameterValueArguments


//<Expects>The DefaultParameterValue attribute and any Optional attribute will be ignored.</Expects>
//<Expects>Note: 'null' needs to be annotated with the correct type, e.g. 'DefaultParameterValue(null:obj)'.</Expects>

open System.Runtime.InteropServices

type Class() =
    static member WrongType([<Optional;DefaultParameterValue("xxx")>]b:obj) = printfn "%A" b

do Class.WrongType()
do Class.WrongType("")