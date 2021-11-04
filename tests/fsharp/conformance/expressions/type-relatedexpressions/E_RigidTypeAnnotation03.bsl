
E_RigidTypeAnnotation03.fsx(17,13,17,16): typecheck error FS0001: This expression was expected to have type
    'sbyte'    
but here has type
    'byte'    

E_RigidTypeAnnotation03.fsx(17,9,17,25): typecheck error FS0041: No overloads match for method 'M'.

Known type of argument: sbyte

Available overloads:
 - static member T.M: a: byte -> int // Argument 'a' doesn't match
 - static member T.M: a: decimal<Kg> -> int // Argument 'a' doesn't match
 - static member T.M: a: float -> int // Argument 'a' doesn't match
 - static member T.M: a: float32<Kg> -> int // Argument 'a' doesn't match
 - static member T.M: a: string -> int // Argument 'a' doesn't match

E_RigidTypeAnnotation03.fsx(18,13,18,19): typecheck error FS0001: This expression was expected to have type
    'float32'    
but here has type
    'float<'u>'    

E_RigidTypeAnnotation03.fsx(18,9,18,30): typecheck error FS0041: No overloads match for method 'M'.

Known type of argument: float32

Available overloads:
 - static member T.M: a: byte -> int // Argument 'a' doesn't match
 - static member T.M: a: decimal<Kg> -> int // Argument 'a' doesn't match
 - static member T.M: a: float -> int // Argument 'a' doesn't match
 - static member T.M: a: float32<Kg> -> int // Argument 'a' doesn't match
 - static member T.M: a: string -> int // Argument 'a' doesn't match

E_RigidTypeAnnotation03.fsx(19,13,19,20): typecheck error FS0001: This expression was expected to have type
    'float32<'u>'    
but here has type
    'decimal<s>'    

E_RigidTypeAnnotation03.fsx(20,13,20,21): typecheck error FS0001: Type mismatch. Expecting a
    'decimal<N s ^ 2>'    
but given a
    'decimal<Kg>'    
The unit of measure 'N s ^ 2' does not match the unit of measure 'Kg'

E_RigidTypeAnnotation03.fsx(20,9,20,39): typecheck error FS0041: No overloads match for method 'M'.

Known type of argument: decimal<N s ^ 2>

Available overloads:
 - static member T.M: a: byte -> int // Argument 'a' doesn't match
 - static member T.M: a: decimal<Kg> -> int // Argument 'a' doesn't match
 - static member T.M: a: float -> int // Argument 'a' doesn't match
 - static member T.M: a: float32<Kg> -> int // Argument 'a' doesn't match
 - static member T.M: a: string -> int // Argument 'a' doesn't match

E_RigidTypeAnnotation03.fsx(21,14,21,18): typecheck error FS0001: This expression was expected to have type
    'char'    
but here has type
    'string'    

E_RigidTypeAnnotation03.fsx(21,9,21,27): typecheck error FS0041: No overloads match for method 'M'.

Known type of argument: char

Available overloads:
 - static member T.M: a: byte -> int // Argument 'a' doesn't match
 - static member T.M: a: decimal<Kg> -> int // Argument 'a' doesn't match
 - static member T.M: a: float -> int // Argument 'a' doesn't match
 - static member T.M: a: float32<Kg> -> int // Argument 'a' doesn't match
 - static member T.M: a: string -> int // Argument 'a' doesn't match
