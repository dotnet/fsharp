// #Regression #Conformance #ObjectOrientedTypes #Classes #LetBindings 
// Scoping:
// identifier introduced by let is local
//<Expects status="error" span="(11,14-11,15)" id="FS0039">The value, namespace, type or module 'm' is not defined</Expects>
//<Expects status="error" span="(11,23-11,24)" id="FS0039">The value, namespace, type or module 'n' is not defined</Expects>

type C() = class
             static let mutable m = [1;2;3]
             static let n = [1;2]
           end
let verify = m.Head + n.Head        // Should give an error
