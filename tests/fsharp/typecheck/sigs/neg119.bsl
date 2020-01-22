
neg119.fs(40,20,40,22): typecheck error FS0071: Type constraint mismatch when applying the default type 'obj' for a type inference variable. No overloads match for method 'Return'. The available overloads are shown below. Consider adding further type constraints
neg119.fs(40,20,40,22): typecheck error FS0071: Possible overload: 'static member Applicatives.Ap.Return : ('r -> 'a) * Ap:Applicatives.Ap -> (('a -> 'r -> 'a2) -> 'a3 -> 'a -> 'r -> 'a2)'. Type constraint mismatch. The type 
    'obj'    
is not compatible with type
    ''a -> 'b'    
.
neg119.fs(40,20,40,22): typecheck error FS0071: Possible overload: 'static member Applicatives.Ap.Return : System.Tuple<'a> * Ap:Applicatives.Ap -> ('a -> System.Tuple<'a>)'. Type constraint mismatch. The type 
    'obj'    
is not compatible with type
    'System.Tuple<'a>'    
.
neg119.fs(40,20,40,22): typecheck error FS0071: Possible overload: 'static member Applicatives.Ap.Return : seq<'a> * Ap:Applicatives.Ap -> ('a -> seq<'a>)'. Type constraint mismatch. The type 
    'obj'    
is not compatible with type
    'seq<'a>'    
.
neg119.fs(40,20,40,22): typecheck error FS0071: Possible overload: 'static member Applicatives.Ap.Return : r: ^R * obj -> ('a1 ->  ^R) when  ^R : (static member Return : 'a1 ->  ^R)'. Type constraint mismatch. The type 
    'obj'    
is not compatible with type
    ''a'    
.
