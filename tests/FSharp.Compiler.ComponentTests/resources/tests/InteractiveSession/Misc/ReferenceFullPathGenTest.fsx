// #NoMT #FSI 
let fwkdir = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory()
let path = System.IO.Path.Combine(fwkdir, "System.Security.dll")

printfn "//<Expects status=\"success\">.+System\\.Security\\.dll</Expects>"
printfn "//<Expects status=\"success\">val f: unit -> System\\.Security\\.Cryptography\\.Xml\\.Signature</Expects>"
printfn "//<Expects status=\"success\">val it: System\\.Security\\.Cryptography\\.Xml\\.Signature =</Expects>"
//printfn "//<Expects status=\"success\">System\\.Security\\.Cryptography\\.Xml\\.Signature {Id = null;</Expects>"
printfn "//<Expects status=\"success\">KeyInfo = seq \\[\\];</Expects>"
printfn "//<Expects status=\"success\">ObjectList = seq \\[\\];</Expects>"
printfn "//<Expects status=\"success\">SignatureValue = null;</Expects>"
printfn "//<Expects status=\"success\">SignedInfo = null;}</Expects>"

printfn "#r @\"%s\";;" path
printfn "let f() = System.Security.Cryptography.Xml.Signature();;"
printfn "f();;"
printfn "exit 0;;"
