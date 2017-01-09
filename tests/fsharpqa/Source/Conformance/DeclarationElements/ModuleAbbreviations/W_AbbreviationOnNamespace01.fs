// #Regression #Conformance #DeclarationElements #Modules 
// This used to be a positive test. Now it is not.
//<Expects id="FS0965" span="(11,1-11,22)" status="error">The path 'System\.IO' is a namespace. A module abbreviation may not abbreviate a namespace\.</Expects>
//<Expects id="FS0039" span="(13,15-13,17)" status="error">The value, namespace, type or module 'IO' is not defined.</Expects>
//<Expects id="FS0965" span="(16,1-16,43)" status="error">The path 'System\.Text\.RegularExpressions' is a namespace. A module abbreviation may not abbreviate a namespace\.</Expects>
//<Expects id="FS0039" span="(18,19-18,21)" status="error">The namespace or module 'rx' is not defined</Expects>
//<Expects id="FS0072" span="(21,4-21,19)" status="error">Lookup on object of indeterminate type based on information prior to this program point\..+</Expects>


// Test 1 --------------------------
module IO = System.IO

let sepChar = IO.Path.DirectorySeparatorChar

// Test 2 --------------------------
module rx = System.Text.RegularExpressions

let myregex = new rx.Regex("x(a)*y")

let mutable success = 0
if myregex.IsMatch("xaaaaay") then
    success <- success + 1
