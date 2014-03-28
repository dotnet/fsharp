// #Conformance #TypeConstraints 
//<Expects status="error" span="(5,5-5,52)" id="FS0332">Could not resolve the ambiguity inherent in the use of the operator 'someFunc' at or near this program point\. Consider using type annotations to resolve the ambiguity\.$</Expects>
//<Expects status="error" span="(5,5-5,52)" id="FS0071">Type constraint mismatch when applying the default type 'obj' for a type inference variable\. The type 'obj' does not support the operator 'someFunc' Consider adding further type constraints$</Expects>
let testFunc (a : ^x) =
    (^x : (static member someFunc : unit -> ^x) ())

exit 1