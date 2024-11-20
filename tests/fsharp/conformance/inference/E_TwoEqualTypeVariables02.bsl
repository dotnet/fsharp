
E_TwoEqualTypeVariables02.fsx(61,37,61,38): typecheck error FS0041: A unique overload for method 'M' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known types of arguments: 'a * 'a

Candidates:
 - static member C12.M: x: 'a * y: 'a -> One
 - static member C12.M: x: 'a * y: 'b -> Two

E_TwoEqualTypeVariables02.fsx(62,37,62,38): typecheck error FS0041: A unique overload for method 'M' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known types of arguments: 'a * 'a

Candidates:
 - static member C14.M: x: 'a * y: 'a -> One
 - static member C14.M: x: 'a * y: C -> Four

E_TwoEqualTypeVariables02.fsx(63,37,63,38): typecheck error FS0041: A unique overload for method 'M' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known types of arguments: 'a * 'a

Candidates:
 - static member C24.M: x: 'a * y: 'b -> Two
 - static member C24.M: x: 'a * y: C -> Four

E_TwoEqualTypeVariables02.fsx(64,38,64,39): typecheck error FS0041: A unique overload for method 'M' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known types of arguments: 'a * 'a

Candidates:
 - static member C123.M: x: 'a * y: 'a -> One
 - static member C123.M: x: 'a * y: 'b -> Two
 - static member C123.M: x: 'a * y: int -> Three

E_TwoEqualTypeVariables02.fsx(65,39,65,40): typecheck error FS0041: A unique overload for method 'M' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known types of arguments: 'a * 'a

Candidates:
 - static member C1234.M: x: 'a * y: 'a -> One
 - static member C1234.M: x: 'a * y: 'b -> Two
 - static member C1234.M: x: 'a * y: C -> Four
 - static member C1234.M: x: 'a * y: int -> Three

E_TwoEqualTypeVariables02.fsx(66,37,66,38): typecheck error FS0041: A unique overload for method 'M' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known types of arguments: 'a * 'a

Candidates:
 - static member C14.M: x: 'a * y: 'a -> One
 - static member C14.M: x: 'a * y: C -> Four
