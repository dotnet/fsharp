module DelegatePartialApplication

open System

// Section D, bullet 2: cases 15/16 (in DelegateKnownFunction.fs / DelegateStaticMethod.fs) capture a
// constant first argument, so their closure can stay static (the constant is re-materialised in Invoke
// with no instance field). Capturing a runtime VALUE instead forces the closure to carry an instance
// field, which exercises a distinct emit path. None of these can ever become a direct delegate (the
// target has more parameters than the delegate's Invoke), so a closure must remain regardless of
// langversion; the only proposed change here is friendlier elided argument names.

let handler3 (x: int) (y: int) (z: int) : unit = ()

type C =
    static member Add3 (x: int) (y: int) (z: int) : unit = ()

type I(k: int) =
    member _.Add3 (x: int) (y: int) (z: int) : unit = ignore k

// variable-capture partial application of a known function (instance-field capture of n)
let papKnownVar (n: int) = Action<int, int>(handler3 n)

// variable-capture partial application of a static method
let papStaticVar (n: int) = Action<int, int>(C.Add3 n)

// variable-capture partial application of an instance method (captures both the receiver and n)
let papInstanceVar (o: I) (n: int) = Action<int, int>(o.Add3 n)
