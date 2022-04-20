// #Regression #Conformance #TypeInference #ByRef 
// Regression for FSHARP1.0:5680
// Statically resolved types and tupled byref parameters generating InvalidProgramException
//<Expects status="error" id="FS0043" span="(11,11-11,12)">The member or object constructor 'TryParse' does not take 1 argument\(s\)\. An overload was found taking 2 arguments\.</Expects>
// Error span is affected by bug 6080

module M

let inline try_parse x = (^a: (static member TryParse: string -> bool * ^a) x)
let _, x = try_parse "4" 
let z = x + 1
