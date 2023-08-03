
neg_anon_1.fs(5,50,5,61): typecheck error FS0001: This anonymous record is missing field 'b'.

neg_anon_1.fs(7,41,7,52): typecheck error FS0001: This anonymous record should have field 'b' but here has field 'a'.

neg_anon_1.fs(10,27,10,55): typecheck error FS0059: The type '{| a: int |}' does not have any proper subtypes and need not be used as the target of a static coercion

neg_anon_1.fs(10,27,10,55): typecheck error FS0193: Type constraint mismatch. The type 
    '{| b: int |}'    
is not compatible with type
    '{| a: int |}'    


neg_anon_1.fs(13,27,13,62): typecheck error FS0059: The type '{| a: int |}' does not have any proper subtypes and need not be used as the target of a static coercion

neg_anon_1.fs(13,27,13,62): typecheck error FS0193: Type constraint mismatch. The type 
    '{| a: int; b: int |}'    
is not compatible with type
    '{| a: int |}'    


neg_anon_1.fs(18,34,18,36): typecheck error FS0001: The type '('a -> 'a)' does not support the 'comparison' constraint. For example, it does not support the 'System.IComparable' interface

neg_anon_1.fs(20,42,20,44): typecheck error FS0001: The type '('a -> 'a)' does not support the 'comparison' constraint. For example, it does not support the 'System.IComparable' interface
