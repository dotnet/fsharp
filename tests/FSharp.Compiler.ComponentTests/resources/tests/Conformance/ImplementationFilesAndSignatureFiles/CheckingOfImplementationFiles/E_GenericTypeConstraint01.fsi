// #Regression #Conformance #SignatureFiles 
module M

// Regression test for FSharp1.0:2089 - Generic parameter type constraints in .fsi files are ignored
//<Expects status="error" span="(10,47)" id="FS0341">The signature and implementation are not compatible because the type parameter 'enumType' has a constraint of the form 'enumType :> Enum but the implementation does not\. Either remove this constraint from the signature or add it to the implementation\.$</Expects>

open System 

module Enum = 
    val fromString : string -> 'enumType when 'enumType :> Enum
