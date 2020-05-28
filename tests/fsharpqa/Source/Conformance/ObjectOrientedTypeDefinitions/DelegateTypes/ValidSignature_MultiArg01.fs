// #Conformance #ObjectOrientedTypes #Delegates 
// Delegate taking multiple arguments
// Declaration is in the form: typ * ... * typ -> typ
//<Expects status="success"></Expects>

// Non-generic
type MultiArgDelegate = delegate of int * string * byte -> unit
let p = new MultiArgDelegate(fun x y z ->())
p.Invoke(1,"",0uy)

// Generic
type MultiArgDelegate<'T> = delegate of int * string * 'T -> unit
let q = new MultiArgDelegate<_>(fun x y z ->())
q.Invoke(1,"",1.1)

exit 0

