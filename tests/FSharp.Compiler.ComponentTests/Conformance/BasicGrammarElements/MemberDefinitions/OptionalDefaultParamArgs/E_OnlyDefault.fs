// #Conformance #DeclarationElements #MemberDefinitions #OptionalDefaultParameterValueArguments

//<Expects id="FS0001" status="error">This expression was expected to have type </Expects>
//<Expects>    'int'    </Expects>
//<Expects>but here has type</Expects>
//<Expects>    'unit'    </Expects>

open System.Runtime.InteropServices

type Class() =
    static member OnlyDefault([<DefaultParameterValueAttribute(1)>]a: int) = ()

Class.OnlyDefault()