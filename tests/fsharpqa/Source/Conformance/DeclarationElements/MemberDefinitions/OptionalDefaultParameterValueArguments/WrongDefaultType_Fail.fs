// #Conformance #DeclarationElements #MemberDefinitions #OptionalDefaultParameterValueArguments

//this currently fails the PEVerify step - need to fix by creating appropriate compile error.

open System.Runtime.InteropServices

type Class() =
    static member WrongType([<Optional;DefaultParameterValue(1)>]b:string) = b

let r = Class.WrongType()

do ()