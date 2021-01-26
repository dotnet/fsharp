
test.fsx(8,20,8,21): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(11,20,11,41): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(14,20,14,44): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(17,21,17,24): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    ''a * 'b'    

test.fsx(25,31,25,32): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'string'    

test.fsx(30,27,30,28): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(30,29,30,30): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(33,56,33,57): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(33,58,33,59): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(36,43,36,44): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(36,45,36,46): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(41,41,41,42): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(42,48,42,49): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(42,50,42,51): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(45,30,45,31): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(45,32,45,33): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(51,22,51,24): typecheck error FS0001: This expression was expected to have type
    'int64'    
but here has type
    'int'    

test.fsx(52,22,52,23): typecheck error FS0001: This expression was expected to have type
    'int64'    
but here has type
    'int'    

test.fsx(53,28,53,29): typecheck error FS0001: This expression was expected to have type
    'int64'    
but here has type
    'int'    

test.fsx(53,30,53,31): typecheck error FS0001: All elements of a list must be of the same type as the first element, which here is 'int64'. This element has type 'int'.

test.fsx(53,32,53,33): typecheck error FS0001: All elements of a list must be of the same type as the first element, which here is 'int64'. This element has type 'int'.

test.fsx(53,34,53,35): typecheck error FS0001: All elements of a list must be of the same type as the first element, which here is 'int64'. This element has type 'int'.

test.fsx(54,28,54,29): typecheck error FS0001: This expression was expected to have type
    'float'    
but here has type
    'int'    

test.fsx(54,30,54,31): typecheck error FS0001: All elements of a list must be of the same type as the first element, which here is 'float'. This element has type 'int'.

test.fsx(54,32,54,33): typecheck error FS0001: All elements of a list must be of the same type as the first element, which here is 'float'. This element has type 'int'.

test.fsx(54,34,54,35): typecheck error FS0001: All elements of a list must be of the same type as the first element, which here is 'float'. This element has type 'int'.

test.fsx(55,30,55,31): typecheck error FS0001: This expression was expected to have type
    'float32'    
but here has type
    'int'    

test.fsx(55,32,55,33): typecheck error FS0001: All elements of a list must be of the same type as the first element, which here is 'float32'. This element has type 'int'.

test.fsx(55,34,55,35): typecheck error FS0001: All elements of a list must be of the same type as the first element, which here is 'float32'. This element has type 'int'.

test.fsx(55,36,55,37): typecheck error FS0001: All elements of a list must be of the same type as the first element, which here is 'float32'. This element has type 'int'.

test.fsx(56,22,56,43): typecheck error FS0001: This expression was expected to have type
    'int64'    
but here has type
    'int'    

test.fsx(57,28,57,49): typecheck error FS0001: This expression was expected to have type
    'int64'    
but here has type
    'int'    

test.fsx(57,50,57,71): typecheck error FS0001: All elements of a list must be of the same type as the first element, which here is 'int64'. This element has type 'int'.

test.fsx(60,20,60,23): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(60,20,60,23): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(61,22,61,25): typecheck error FS0001: This expression was expected to have type
    'int64'    
but here has type
    'int'    

test.fsx(61,22,61,25): typecheck error FS0001: This expression was expected to have type
    'int64'    
but here has type
    'int'    

test.fsx(62,26,62,29): typecheck error FS0001: This expression was expected to have type
    'int64'    
but here has type
    'int'    

test.fsx(62,26,62,29): typecheck error FS0001: This expression was expected to have type
    'int64'    
but here has type
    'int'    

test.fsx(86,13,86,20): typecheck error FS0041: No overloads match for method 'M1'.

Known type of argument: int

Available overloads:
 - static member C.M1 : x:int64 -> unit // Argument 'x' doesn't match
 - static member C.M1 : x:string -> 'a0 // Argument 'x' doesn't match

test.fsx(92,13,92,22): typecheck error FS0041: No overloads match for method 'M1'.

Known type of argument: x:int

Available overloads:
 - static member C.M1 : ?x:int64 -> unit // Argument 'x' doesn't match
 - static member C.M1 : x:string -> 'a0 // Argument 'x' doesn't match

test.fsx(109,20,109,21): typecheck error FS0001: This expression was expected to have type
    'int64'    
but here has type
    'int'    

test.fsx(114,14,114,21): typecheck error FS0041: No overloads match for method 'M1'.

Known type of argument: int

Available overloads:
 - static member C.M1 : [<ParamArray>] x:int64 [] -> int64 // Argument 'x' doesn't match
 - static member C.M1 : [<ParamArray>] x:int64 [] -> int64 // Argument at index 1 doesn't match

test.fsx(115,19,115,20): typecheck error FS0001: This expression was expected to have type
    'int64'    
but here has type
    'int'    

test.fsx(115,22,115,23): typecheck error FS0001: This expression was expected to have type
    'int64'    
but here has type
    'int'    

test.fsx(120,14,120,21): typecheck error FS0041: No overloads match for method 'M1'.

Known type of argument: int

Available overloads:
 - static member C.M1 : [<ParamArray>] x:double [] -> double // Argument 'x' doesn't match
 - static member C.M1 : [<ParamArray>] x:double [] -> double // Argument at index 1 doesn't match

test.fsx(121,19,121,20): typecheck error FS0001: This expression was expected to have type
    'double'    
but here has type
    'int'    

test.fsx(121,22,121,23): typecheck error FS0001: This expression was expected to have type
    'double'    
but here has type
    'int'    

test.fsx(128,18,128,19): typecheck error FS0001: This expression was expected to have type
    'C'    
but here has type
    'int'    

test.fsx(133,18,133,19): typecheck error FS0001: This expression was expected to have type
    'int64'    
but here has type
    'int'    

test.fsx(138,18,138,19): typecheck error FS0001: This expression was expected to have type
    'decimal'    
but here has type
    'int'    

test.fsx(140,18,140,19): typecheck error FS0001: This expression was expected to have type
    'decimal'    
but here has type
    'int'    

test.fsx(142,39,142,41): typecheck error FS0001: This expression was expected to have type
    'Xml.Linq.XNamespace'    
but here has type
    'string'    

test.fsx(147,18,147,20): typecheck error FS0001: This expression was expected to have type
    'Xml.Linq.XNamespace'    
but here has type
    'string'    

test.fsx(152,18,152,21): typecheck error FS0001: This expression was expected to have type
    'Xml.Linq.XName'    
but here has type
    'string'    

test.fsx(158,18,158,19): typecheck error FS0001: The type 'int' is not compatible with the type 'C'

test.fsx(158,17,158,20): typecheck error FS0193: Type constraint mismatch. The type 
    'int'    
is not compatible with type
    'C'    


test.fsx(165,18,165,21): typecheck error FS0001: The type 'Y' is not compatible with the type 'X'

test.fsx(165,17,165,22): typecheck error FS0193: Type constraint mismatch. The type 
    'Y'    
is not compatible with type
    'X'    


test.fsx(171,20,171,21): typecheck error FS0001: This expression was expected to have type
    'C<int>'    
but here has type
    'int'    

test.fsx(173,15,173,16): typecheck error FS0001: The type 'int' is not compatible with the type 'C<int>'

test.fsx(179,27,179,28): typecheck error FS0001: This expression was expected to have type
    'Nullable<int>'    
but here has type
    'int'    

test.fsx(181,15,181,16): typecheck error FS0001: This expression was expected to have type
    'Nullable<int>'    
but here has type
    'int'    

test.fsx(184,20,184,23): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    ''a * 'b'    

test.fsx(185,26,185,27): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(185,28,185,29): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(188,55,188,56): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(188,57,188,58): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(191,48,191,49): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(191,50,191,53): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'float'    

test.fsx(194,32,194,33): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(194,34,194,37): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'float'    

test.fsx(197,42,197,43): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(197,44,197,45): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(203,13,203,14): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(203,16,203,19): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'byte'    

test.fsx(203,30,203,33): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'float'    

test.fsx(203,35,203,36): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(209,40,209,41): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(212,43,212,44): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(212,52,212,55): typecheck error FS0001: All branches of a pattern match expression must return values of the same type as the first branch, which here is 'obj'. This branch returns a value of type 'float'.

test.fsx(212,63,212,66): typecheck error FS0001: All branches of a pattern match expression must return values of the same type as the first branch, which here is 'obj'. This branch returns a value of type 'byte'.

test.fsx(214,47,214,48): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(214,49,214,50): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(219,34,219,35): typecheck error FS0001: This expression was expected to have type
    'IComparable'    
but here has type
    'int'    

test.fsx(222,28,222,37): typecheck error FS0001: This expression was expected to have type
    'Array'    
but here has type
    'uint16 []'    

test.fsx(224,28,224,36): typecheck error FS0001: This expression was expected to have type
    'Array'    
but here has type
    ''a []'    

test.fsx(226,34,226,36): typecheck error FS0001: This expression was expected to have type
    'IComparable'    
but here has type
    'Numerics.BigInteger'    

test.fsx(229,42,229,61): typecheck error FS0001: This expression was expected to have type
    'IComparable<string>'    
but here has type
    'string'    

test.fsx(232,42,232,66): typecheck error FS0001: This expression was expected to have type
    'IComparable<string>'    
but here has type
    'string'    

test.fsx(234,19,234,43): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'string'    

test.fsx(236,34,236,58): typecheck error FS0001: This expression was expected to have type
    'IComparable'    
but here has type
    'string'    

test.fsx(241,33,241,34): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(241,35,241,36): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(242,33,242,38): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'string'    

test.fsx(242,39,242,40): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(243,42,243,43): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(244,42,244,43): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(244,53,244,56): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'float'    

test.fsx(245,66,245,69): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'float'    

test.fsx(246,80,246,83): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'float'    

test.fsx(248,19,248,20): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(249,20,249,27): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(250,19,250,21): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'string'    

test.fsx(251,20,251,31): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'string'    

test.fsx(252,20,252,43): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'IComparable'    

test.fsx(253,20,253,22): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'string'    

test.fsx(254,20,254,22): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'string'    

test.fsx(255,19,255,74): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'ICloneable'    

test.fsx(256,19,256,21): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'string'    

test.fsx(257,19,257,28): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'string'    

test.fsx(257,19,257,28): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'string'    

test.fsx(258,22,258,24): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'string'    

test.fsx(261,32,261,33): typecheck error FS0001: The 'if' expression needs to have type 'obj' to satisfy context type requirements. It currently has type 'int'.

test.fsx(261,39,261,42): typecheck error FS0001: All branches of an 'if' expression must return values of the same type as the first branch, which here is 'obj'. This branch returns a value of type 'float'.

test.fsx(262,33,262,34): typecheck error FS0001: The 'if' expression needs to have type 'obj' to satisfy context type requirements. It currently has type 'int'.

test.fsx(262,40,262,43): typecheck error FS0001: All branches of an 'if' expression must return values of the same type as the first branch, which here is 'obj'. This branch returns a value of type 'float'.

test.fsx(263,33,263,34): typecheck error FS0001: The 'if' expression needs to have type 'obj' to satisfy context type requirements. It currently has type 'int'.

test.fsx(263,50,263,53): typecheck error FS0001: All branches of an 'if' expression must return values of the same type as the first branch, which here is 'obj'. This branch returns a value of type 'byte'.

test.fsx(263,59,263,62): typecheck error FS0001: All branches of an 'if' expression must return values of the same type as the first branch, which here is 'obj'. This branch returns a value of type 'float'.

test.fsx(266,23,266,24): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(266,35,266,38): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'float'    

test.fsx(269,23,269,24): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(272,43,272,44): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(272,52,272,55): typecheck error FS0001: All branches of a pattern match expression must return values of the same type as the first branch, which here is 'obj'. This branch returns a value of type 'float'.

test.fsx(275,19,275,20): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'int'    

test.fsx(276,32,276,33): typecheck error FS0001: The 'if' expression needs to have type 'obj' to satisfy context type requirements. It currently has type 'int'.

test.fsx(276,39,276,42): typecheck error FS0001: All branches of an 'if' expression must return values of the same type as the first branch, which here is 'obj'. This branch returns a value of type 'float'.

test.fsx(280,9,280,11): typecheck error FS0001: This expression was expected to have type
    'obj'    
but here has type
    'string'    

test.fsx(281,7,281,22): typecheck error FS0001: This expression was expected to have type
    'unit'    
but here has type
    'obj'    

test.fsx(283,32,283,35): typecheck error FS0001: All branches of an 'if' expression must return values of the same type as the first branch, which here is 'int'. This branch returns a value of type 'float'.
