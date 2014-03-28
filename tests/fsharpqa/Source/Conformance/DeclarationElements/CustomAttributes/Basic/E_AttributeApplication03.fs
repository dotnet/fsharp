// #Regression #Conformance #DeclarationElements #Attributes 


open System

// FSharp1.0#1475: validate that an incorrect attribute application reports an error.
//<Expects id="FS0842" span="(15,7-15,17)" status="error">This attribute is not valid for use on this language element</Expects>

type TestType() =
    [<Obsolete>] 
    member s.ObsoleteMethod =
        10

type TestType1() =
    [<ParamArray>]
    member s.ThisWontWork =
        10
