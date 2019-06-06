// #Conformance #ObjectOrientedTypes #Delegates 
// Delegate returning a function value
// Declaration is in the form: typ -> (typ -> ... -> typ -> typ)
//<Expects status="success"></Expects>

// Non-generic
type DelegateReturningAFunctionValue = delegate of int -> (string -> int)
let p = new DelegateReturningAFunctionValue(fun x -> fun y -> 1)
p.Invoke(1) |> ignore

// Generic
type DelegateReturningAFunctionValue<'T> = delegate of int -> (string -> 'T)
let q = new DelegateReturningAFunctionValue<_>(fun x -> fun y -> -1.1M)
q.Invoke(1) |> ignore

exit 0

