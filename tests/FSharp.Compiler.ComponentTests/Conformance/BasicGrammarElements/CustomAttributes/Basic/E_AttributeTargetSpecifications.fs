// #Regression #Conformance #DeclarationElements #Attributes 
// Regression test for FSharp1.0:1089 - Support "module" and other attribute target specifications  in parser



#light

open System
[<someTotallyBogusAttributeTarget : System.ObsoleteAttribute("asdf")>]
let x = 5
