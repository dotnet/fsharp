// #Regression #Conformance #DeclarationElements #Attributes 


open System

// FSharp1.0#1475: validate that an incorrect attribute application reports an error.


type TestType() =
    [<Obsolete>] 
    member s.ObsoleteMethod =
        10

[<ParamArray>]
type TestType1() =
    member s.ThisWontWork =
        10
