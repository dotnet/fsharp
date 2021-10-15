
E_TwoEqualTypeVariables02.fsx(61,33,61,43): typecheck error FS0041: A unique overload for method 'M' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known types of arguments: 'a * 'a

Candidates:
 - static member C12.M: x: 'a * y: 'a -> One
 - static member C12.M: x: 'a * y: 'b -> Two

E_TwoEqualTypeVariables02.fsx(62,33,62,43): typecheck error FS0041: A unique overload for method 'M' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known types of arguments: 'a * 'a

Candidates:
 - static member C14.M: x: 'a * y: 'a -> One
 - static member C14.M: x: 'a * y: C -> Four

E_TwoEqualTypeVariables02.fsx(63,33,63,43): typecheck error FS0041: A unique overload for method 'M' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known types of arguments: 'a * 'a

Candidates:
 - static member C24.M: x: 'a * y: 'b -> Two
 - static member C24.M: x: 'a * y: C -> Four

E_TwoEqualTypeVariables02.fsx(64,33,64,44): typecheck error FS0041: A unique overload for method 'M' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known types of arguments: 'a * 'a

Candidates:
 - static member C123.M: x: 'a * y: 'a -> One
 - static member C123.M: x: 'a * y: 'b -> Two
 - static member C123.M: x: 'a * y: int -> Three

E_TwoEqualTypeVariables02.fsx(65,33,65,45): typecheck error FS0041: A unique overload for method 'M' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known types of arguments: 'a * 'a

Candidates:
 - static member C1234.M: x: 'a * y: 'a -> One
 - static member C1234.M: x: 'a * y: 'b -> Two
 - static member C1234.M: x: 'a * y: C -> Four
 - static member C1234.M: x: 'a * y: int -> Three

E_TwoEqualTypeVariables02.fsx(66,33,66,46): typecheck error FS0041: A unique overload for method 'M' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known types of arguments: 'a * 'a

Candidates:
 - static member C14.M: x: 'a * y: 'a -> One
 - static member C14.M: x: 'a * y: C -> Four
