// #Regression #Conformance #DeclarationElements #Modules 
// This used to be a positive test. Now it is not.







// Test 1 --------------------------
module IO = System.IO

let sepChar = IO.Path.DirectorySeparatorChar

// Test 2 --------------------------
module rx = System.Text.RegularExpressions

let myregex = new rx.Regex("x(a)*y")

let mutable success = 0
if myregex.IsMatch("xaaaaay") then
    success <- success + 1
