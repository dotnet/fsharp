// #Regression #Conformance #TypeConstraints 
// Regression test for FSharp1.0:3407 - Internal error on code with member constraint and invalid method signature
//<Expects id="FS0001" span="(6,5-6,60)" status="error">This expression was expected to have type.    ''a -> unit'    .but here has type.    'unit'</Expects>

let inline callfoo (a: ^a) : ^a -> unit when  ^a : (member foo :  ^a -> unit) = 
    (^a : (member foo: unit (* <- incorrect *)-> unit) (a))

