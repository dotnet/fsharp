// #Regression #Conformance #DeclarationElements #Attributes 
// Regression test for FSharp1.0:1089 - Support "module" and other attribute target specifications  in parser
//<Expects id="FS0840" status="error">Unrecognized attribute target\. Valid attribute targets are 'assembly', 'module', 'type', 'method', 'property', 'return', 'param', 'field', 'event', 'constructor'</Expects>


#light

open System
[<someTotallyBogusAttributeTarget : System.ObsoleteAttribute("asdf")>]
let x = 5
