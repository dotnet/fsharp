// #Regression #Conformance #DeclarationElements #Import 
// Verify you can open a module or namespace twice and not get a warning/error
//<Expects status="error" span="(11,6-11,39)" id="FS0892">This declaration opens the module 'Microsoft\.FSharp\.Collections\.List', which is marked as 'RequireQualifiedAccess'\. Adjust your code to use qualified references to the elements of the module instead, e\.g\. 'List\.map' instead of 'map'\. This change will ensure that your code is robust as new constructs are added to libraries\.$</Expects>
//<Expects status="error" span="(12,6-12,39)" id="FS0892">This declaration opens the module 'Microsoft\.FSharp\.Collections\.List', which is marked as 'RequireQualifiedAccess'\. Adjust your code to use qualified references to the elements of the module instead, e\.g\. 'List\.map' instead of 'map'\. This change will ensure that your code is robust as new constructs are added to libraries\.$</Expects>

// Namespace  (currently we don't give an error/warning - see FSHARP1.0:2931)
open System.Collections
open System.Collections

// Module
open Microsoft.FSharp.Collections.List
open Microsoft.FSharp.Collections.List
