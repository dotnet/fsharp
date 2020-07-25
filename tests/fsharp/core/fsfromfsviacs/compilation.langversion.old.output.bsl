
test.fsx(99,70): error FS0193: Type constraint mismatch. The type 
    'int option'    
is not compatible with type
    'int'    


test.fsx(100,70): error FS0193: Type constraint mismatch. The type 
    'string option'    
is not compatible with type
    'string'    


test.fsx(101,70): error FS0193: Type constraint mismatch. The type 
    'float option'    
is not compatible with type
    'float'    


test.fsx(103,70): error FS0193: Type constraint mismatch. The type 
    ''a option'    
is not compatible with type
    'int'    


test.fsx(104,70): error FS0193: Type constraint mismatch. The type 
    ''a option'    
is not compatible with type
    'string'    


test.fsx(105,70): error FS0193: Type constraint mismatch. The type 
    ''a option'    
is not compatible with type
    'float'    


test.fsx(111,89): error FS0001: This expression was expected to have type
    'Nullable<int>'    
but here has type
    'int'    

test.fsx(117,90): error FS0001: This expression was expected to have type
    'Nullable<int>'    
but here has type
    'int'    

test.fsx(118,90): error FS0001: This expression was expected to have type
    'Nullable<float>'    
but here has type
    'float'    

test.fsx(121,91): error FS0193: Type constraint mismatch. The type 
    'int option'    
is not compatible with type
    'Nullable<int>'    


test.fsx(122,91): error FS0193: Type constraint mismatch. The type 
    'float option'    
is not compatible with type
    'Nullable<float>'    


test.fsx(124,91): error FS0193: Type constraint mismatch. The type 
    ''a option'    
is not compatible with type
    'Nullable<int>'    


test.fsx(125,91): error FS0193: Type constraint mismatch. The type 
    ''a option'    
is not compatible with type
    'Nullable<float>'    


test.fsx(131,77): error FS0001: This expression was expected to have type
    'Nullable<int>'    
but here has type
    'int'    

test.fsx(133,77): error FS0001: This expression was expected to have type
    'Nullable<float>'    
but here has type
    'float'    

test.fsx(138,78): error FS0001: This expression was expected to have type
    'Nullable<int>'    
but here has type
    'int'    

test.fsx(139,78): error FS0001: This expression was expected to have type
    'Nullable<float>'    
but here has type
    'float'    

test.fsx(142,79): error FS0193: Type constraint mismatch. The type 
    'int option'    
is not compatible with type
    'Nullable<int>'    


test.fsx(143,79): error FS0193: Type constraint mismatch. The type 
    'float option'    
is not compatible with type
    'Nullable<float>'    


test.fsx(145,79): error FS0193: Type constraint mismatch. The type 
    ''a option'    
is not compatible with type
    'Nullable<int>'    


test.fsx(146,79): error FS0193: Type constraint mismatch. The type 
    ''a option'    
is not compatible with type
    'Nullable<float>'    


test.fsx(267,15): warning FS0025: Incomplete pattern matches on this expression. For example, the value 'U2 (_, U1 (_, "a"))' may indicate a case not covered by the pattern(s).

test.fsx(284,15): warning FS0025: Incomplete pattern matches on this expression. For example, the value 'U2 (_, U1 (_, "a"))' may indicate a case not covered by the pattern(s).

test.fsx(418,29): error FS0041: A unique overload for method 'SimpleOverload' could not be determined based on type information prior to this program point. A type annotation may be needed.
Candidates:
 - SomeClass.SimpleOverload(?x: Nullable<int>) : int
 - SomeClass.SimpleOverload(?x: int) : int
