// #Regression #Conformance #TypeConstraints #Diagnostics
// Spec 5.2.3: A type variable may not be involved in the support set of more than one member constraint with the same name, staticness, argument arity and support set – if it is, the argument and return types in the two member constraints are themselves constrained to be equal.
// I think this is correct, a is involved in 2 different type constraints meeting the parameters above, so the ^x and ^y return types are asserted to be the same type
//<Expects status="error" span="(18,28-18,39)" id="FS0001">This expression was expected to have type.    'Foo'    .but here has type.    'OtherFoo'</Expects>
//<Expects status="error" span="(19,20-19,31)" id="FS0001">This expression was expected to have type.    'OtherFoo'    .but here has type.    'Foo'</Expects>
//<Expects status="error" span="(19,34-19,40)" id="FS0001">This expression was expected to have type.    'OtherFoo'    .but here has type.    'Foo'</Expects>

let inline testFunc (a : ^x) (b : ^y) =
    let r1 = (^x : (static member someFunc : unit -> ^x) ())
    let r2 = (^x : (static member someFunc : unit -> ^y) ())
    ()
 
type Foo(x) = 
    static member someFunc() = Foo(10)
type OtherFoo(x) = 
    static member someFunc() = Foo(20)
 
let r = testFunc (Foo(1)) (OtherFoo(2))
let r2 = testFunc (OtherFoo(2)) (Foo(1))

exit 1