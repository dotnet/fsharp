
E_TwoEqualTypeVariables02rec.fsx(60,41,60,42): typecheck error FS0041: A unique overload for method 'M' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known types of arguments: 'a * 'a

Candidates:
 - static member C12.M: x: 'a * y: 'a -> One
 - static member C12.M: x: 'a * y: 'b -> Two

E_TwoEqualTypeVariables02rec.fsx(61,41,61,42): typecheck error FS0041: A unique overload for method 'M' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known types of arguments: 'a * 'a

Candidates:
 - static member C14.M: x: 'a * y: 'a -> One
 - static member C14.M: x: 'a * y: C -> Four

E_TwoEqualTypeVariables02rec.fsx(62,41,62,42): typecheck error FS0041: A unique overload for method 'M' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known types of arguments: 'a * 'a

Candidates:
 - static member C24.M: x: 'a * y: 'b -> Two
 - static member C24.M: x: 'a * y: C -> Four

E_TwoEqualTypeVariables02rec.fsx(63,42,63,43): typecheck error FS0041: A unique overload for method 'M' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known types of arguments: 'a * 'a

Candidates:
 - static member C123.M: x: 'a * y: 'a -> One
 - static member C123.M: x: 'a * y: 'b -> Two
 - static member C123.M: x: 'a * y: int -> Three

E_TwoEqualTypeVariables02rec.fsx(64,43,64,44): typecheck error FS0041: A unique overload for method 'M' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known types of arguments: 'a * 'a

Candidates:
 - static member C1234.M: x: 'a * y: 'a -> One
 - static member C1234.M: x: 'a * y: 'b -> Two
 - static member C1234.M: x: 'a * y: C -> Four
 - static member C1234.M: x: 'a * y: int -> Three

E_TwoEqualTypeVariables02rec.fsx(65,41,65,42): typecheck error FS0041: A unique overload for method 'M' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known types of arguments: 'a * 'a

Candidates:
 - static member C14.M: x: 'a * y: 'a -> One
 - static member C14.M: x: 'a * y: C -> Four
