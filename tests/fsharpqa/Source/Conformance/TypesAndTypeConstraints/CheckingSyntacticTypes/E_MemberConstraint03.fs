// #Conformance #TypeConstraints #Diagnostics
// Spec 5.2.3: The member-sig may not be generic, i.e. may not include explicit type parameter definitions.
//<Expects status="error" span="(5,5-5,50)" id="FS0735">Expected 1 expressions, got 0$</Expects>
let inline testFunc (a : ^x) =
    (^x : (static member someFunc : 'a -> ^x) ())

type Foo(x) =
    member this.Test = x
    member this.someFunc() = Foo(this.Test + 1)

exit 1