// #Conformance #TypeConstraints
//<Expects id="FS0001" status="error">The type 'int' does not support the operator 'get_M'</Expects>

let inline f< ^t when ^t : (static member M : string)>(x : ^t) = 0
let _ = f 5