// #Regression #Conformance #DeclarationElements #Attributes 


open System

// FSharp1.0#1475: validate that an incorrect attribute application reports an error.
//<Expects id="FS0842" span="(14,3-14,13)" status="error">This attribute is not valid for use on this language element</Expects>

type TestType() =
    [<Obsolete>] 
    member s.ObsoleteMethod =
        10

[<ParamArray>]
type TestType1() =
    member s.ThisWontWork =
        10
