// #Regression #Conformance #TypesAndModules #Modules 
// Regression test for FSHARP1.0:2644 (a module may start with an expression)
// Module abbreviation: Trying to abbreviate a namespace is deprecated
// See also FSHARP1.0:2848
//<Expects id="FS0965" span="(8,1-9,26)" status="error">The path 'Microsoft\.FSharp\.Core' is a namespace\. A module abbreviation may not abbreviate a namespace\.</Expects>

// Module abbreviations
module M2 = 
    Microsoft.FSharp.Core

exit 0
