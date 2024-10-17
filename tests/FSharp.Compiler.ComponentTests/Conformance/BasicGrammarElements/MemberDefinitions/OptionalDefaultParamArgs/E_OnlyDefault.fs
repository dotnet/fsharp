// #Conformance #DeclarationElements #MemberDefinitions #OptionalDefaultParameterValueArguments


//<Expects>    'int'    </Expects>
//<Expects>but here has type</Expects>
//<Expects>    'unit'    </Expects>

open System.Runtime.InteropServices

type Class() =
    static member OnlyDefault([<DefaultParameterValueAttribute(1)>]a: int) = ()

Class.OnlyDefault()