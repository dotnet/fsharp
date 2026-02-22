// #Conformance #TypeConstraints 
//<Expects status="error" span="(4,5-4,52)" id="FS0071">Type constraint mismatch when applying the default type 'obj' for a type inference variable\. The type 'obj' does not support the operator 'someFunc' Consider adding further type constraints$</Expects>
let testFunc (a : ^x) =
    (^x : (static member someFunc : unit -> ^x) ())

exit 1