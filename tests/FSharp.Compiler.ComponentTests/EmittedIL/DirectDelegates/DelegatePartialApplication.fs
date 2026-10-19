module DelegatePartialApplication

open System

// Cases 37/38 (in DelegateKnownFunction.fs / DelegateStaticMethod.fs) capture a
// constant first argument, so their closure can stay static (the constant is re-materialised in Invoke
// with no instance field). Capturing a runtime VALUE instead forces the closure to carry an instance
// field, which exercises a distinct emit path. None of the cases below can become a direct delegate:
//  - The CLR's closed delegate binds exactly ONE leading value as the Target. papInstanceVar fixes two
//    leading values (the receiver 'o' and the argument 'n'), so there is no closed form.
//  - papKnownVar / papStaticVar fix a single leading value, but it is an 'int'. The closed-delegate thunk
//    passes the Target (an 'object') straight into the method's first parameter with NO unboxing, so a
//    value-type first parameter has no closed form at all (the same reason a value-type receiver is
//    excluded). A reference-type fixed argument, by contrast, IS emitted directly (see the execution test
//    `Reference-type single-argument partial application is direct`).

let handler3 (x: int) (y: int) (z: int) : unit = ()

type C =
    static member Add3 (x: int) (y: int) (z: int) : unit = ()

type I(k: int) =
    member _.Add3 (x: int) (y: int) (z: int) : unit = ignore k

// 39. partial application of module function (captured var: instance-field capture of n)
let papKnownVar (n: int) = Action<int, int>(handler3 n)

// 40. partial application of static method (captured var)
let papStaticVar (n: int) = Action<int, int>(C.Add3 n)

// 41. partial application of instance method (captures both the receiver and n)
let papInstanceVar (o: I) (n: int) = Action<int, int>(o.Add3 n)
