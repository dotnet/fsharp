// #Regression #Conformance #ObjectOrientedTypes #Classes #LetBindings 
// Scoping:
// identifier introduced by let is local
//<Expects status="error" span="(12,14-12,15)" id="FS0039">The namespace or module 'm' is not defined$</Expects>
//<Expects status="error" span="(12,23-12,24)" id="FS0039">The namespace or module 'n' is not defined$</Expects>
//<Expects status="notin" span="(12,21-12,22)" id="FS0332">Could not resolve the ambiguity inherent in the use of the operator '\( \+ \)' at or near this program point\. Consider using type annotations to resolve the ambiguity\.$</Expects>

type C() = class
             static let mutable m = [1;2;3]
             static let n = [1;2]
           end
let verify = m.Head + n.Head        // Should give an error
