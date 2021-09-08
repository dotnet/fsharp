
E_OneTypeVariable03.fsx(60,34,60,44): typecheck error FS0041: A unique overload for method 'M' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known types of arguments: 'a * int

Candidates:
 - static member C23.M: x: 'a * y: 'b -> Two
 - static member C23.M: x: 'a * y: int -> Three

E_OneTypeVariable03.fsx(61,34,61,45): typecheck error FS0041: A unique overload for method 'M' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known types of arguments: 'a * int

Candidates:
 - static member C123.M: x: 'a * y: 'a -> One
 - static member C123.M: x: 'a * y: 'b -> Two
 - static member C123.M: x: 'a * y: int -> Three
