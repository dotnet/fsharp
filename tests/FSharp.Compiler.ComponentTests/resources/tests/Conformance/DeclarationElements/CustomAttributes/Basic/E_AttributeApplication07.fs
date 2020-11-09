// #Regression #Conformance #DeclarationElements #Attributes 
// Regression test for FSharp1.0:4740

//<Expects status="error" span="(10,3)" id="FS0850">This attribute cannot be used in this version of F#</Expects>
//<Expects status="error" span="(13,3)" id="FS0850">This attribute cannot be used in this version of F#</Expects>
//<Expects status="error" span="(16,13)" id="FS0850">This attribute cannot be used in this version of F#</Expects>

open System.Runtime.CompilerServices

[<Microsoft.FSharp.Core.CompilationArgumentCountsAttribute>]
let f1 x y = x + y

[<Microsoft.FSharp.Core.CompilationMappingAttribute(Microsoft.FSharp.Core.SourceConstructFlags.None)>]
let f2 x y = x + y

[<assembly: TypeForwardedToAttribute(typeof<int>)>]
do ()
