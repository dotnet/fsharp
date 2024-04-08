// #Regression #Conformance #TypesAndModules #GeneratedEqualityAndHashing 
// Regression test for FSHARP1.0:5345
// exception types do not allow structural comparison
//<Expects span="(9,9-9,15)" status="error" id="FS0001">The type 'exn' does not support the 'comparison' constraint\. For example, it does not support the 'System\.IComparable' interface</Expects>
//<Expects span="(10,9-10,15)" status="error" id="FS0001">The type 'exn' does not support the 'comparison' constraint\. For example, it does not support the 'System\.IComparable' interface</Expects>

exception E of int * int

let _ = E(1,2) < E(1,3)
let _ = E(1,2) > E(2,1)
