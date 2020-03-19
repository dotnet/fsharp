
neg120.fs(95,18,95,21): typecheck error FS0071: Type constraint mismatch when applying the default type 'obj' for a type inference variable. No overloads match for method 'op_GreaterGreaterEquals'.

Known return type: obj

Known type parameters: < Id<int> , (int -> obj) >

Available overloads:
 - static member Bind.( >>= )<'T,'U> : source:'T option * f:('T -> 'U option) -> 'U option // Argument 'source' doesn't match
 - static member Bind.( >>= )<'T,'U> : source:Id<'T> * f:('T -> Id<'U>) -> Id<'U> // Argument 'f' doesn't match
 - static member Bind.( >>= )<'T,'U> : source:Lazy<'T> * f:('T -> Lazy<'U>) -> Lazy<'U> // Argument 'source' doesn't match
 - static member Bind.( >>= )<'T,'U> : source:Task<'T> * f:('T -> Task<'U>) -> Task<'U> // Argument 'source' doesn't match
 - static member Bind.( >>= )<'T,'a1> : source:Async<'T> * f:('T -> Async<'a1>) -> Async<'a1> // Argument 'source' doesn't match Consider adding further type constraints
