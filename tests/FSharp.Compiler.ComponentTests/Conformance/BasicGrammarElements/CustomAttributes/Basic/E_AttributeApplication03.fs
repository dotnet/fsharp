// #Regression #Conformance #DeclarationElements #Attributes 


open System

// FSharp1.0#1475: validate that an incorrect attribute application reports an error.


type TestType() =
    [<Obsolete>] 
    member s.ObsoleteMethod =
        10

type TestType1() =
    [<ParamArray>]
    member s.ThisWontWork =
        10
