// #Conformance #ConstraintCall
//<Expects id="FS0001" status="error">The type 'int' does not support the operator 'M'</Expects>

let inline h x = (^a: (static member M : ^a -> ^b) (x))    
h 1
