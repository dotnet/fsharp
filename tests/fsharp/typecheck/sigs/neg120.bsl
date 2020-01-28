
neg120.fs(95,18,95,21): typecheck error FS0071: Type constraint mismatch when applying the default type 'obj' for a type inference variable. No overloads match for method 'op_GreaterGreaterEquals'. The available overloads are shown below. Consider adding further type constraints
neg120.fs(95,18,95,21): typecheck error FS0071: Possible overload: 'static member Bind.( >>= ) : source:Id<'T> * f:('T -> Id<'U>) -> Id<'U>'. Type constraint mismatch. The type 
    'int -> obj'    
is not compatible with type
    ''a -> Id<'b>'    
.
neg120.fs(95,18,95,21): typecheck error FS0071: Possible overload: 'static member Bind.( >>= ) : source:Async<'T> * f:('T -> Async<'a1>) -> Async<'a1>'. Type constraint mismatch. The type 
    'Id<int>'    
is not compatible with type
    'Async<'a>'    
.
neg120.fs(95,18,95,21): typecheck error FS0071: Possible overload: 'static member Bind.( >>= ) : source:'T option * f:('T -> 'U option) -> 'U option'. Type constraint mismatch. The type 
    'Id<int>'    
is not compatible with type
    ''a option'    
.
neg120.fs(95,18,95,21): typecheck error FS0071: Possible overload: 'static member Bind.( >>= ) : source:Task<'T> * f:('T -> Task<'U>) -> Task<'U>'. Type constraint mismatch. The type 
    'Id<int>'    
is not compatible with type
    'Task<'a>'    
.
neg120.fs(95,18,95,21): typecheck error FS0071: Possible overload: 'static member Bind.( >>= ) : source:Lazy<'T> * f:('T -> Lazy<'U>) -> Lazy<'U>'. Type constraint mismatch. The type 
    'Id<int>'    
is not compatible with type
    'Lazy<'a>'    
.
