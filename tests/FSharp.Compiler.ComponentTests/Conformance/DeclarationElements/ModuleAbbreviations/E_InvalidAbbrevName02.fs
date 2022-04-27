// #Regression #Conformance #DeclarationElements #Modules 
// Verify error when using a dot in the name
//<Expects id="FS0534" status="error" span="(5,1)">A module abbreviation must be a simple name, not a path</Expects>

module MS.FS.Co.L = Microsoft.FSharp.Collections.List
