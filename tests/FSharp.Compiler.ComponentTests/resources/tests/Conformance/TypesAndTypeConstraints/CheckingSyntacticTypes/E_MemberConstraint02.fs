// #Conformance #TypeConstraints #Diagnostics
//<Expects status="error" span="(10,19-10,25)" id="FS0001">The type 'Foo' has a method 'someFunc' \(full name 'someFunc'\), but the method is not static$</Expects>
let inline testFunc (a : ^x) =
    (^x : (static member someFunc : unit -> ^x) ())

type Foo(x) =
    member this.Test = x
    member this.someFunc() = Foo(this.Test + 1)

let f = testFunc (Foo(1))
let r = f.Test

exit 1