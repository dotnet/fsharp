// #Regression #Conformance #DeclarationElements #Import 
// Verify warning when opening a non-fully qualified namespace
//<Expects id="FS0893" status="error" span="(7,6)">This declaration opens the namespace or module 'System\.Collections\.Generic' through a partially qualified path\. Adjust this code to use the full path of the namespace\. This change will make your code more robust as new constructs are added to the F# and CLI libraries\.$</Expects>
//<Expects id="FS0893" status="error" span="(7,6)">This declaration opens the namespace or module 'System\.Collections\.Generic' through a partially qualified path\. Adjust this code to use the full path of the namespace\. This change will make your code more robust as new constructs are added to the F# and CLI libraries\.$</Expects>

open System.Collections
open Generic

let t = new List<int>()
