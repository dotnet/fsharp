
E_OneTypeVariable03rec.fsx(60,38,60,48): typecheck error FS0041: A unique overload for method 'M' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known types of arguments: 'a * int

Candidates:
 - static member C23.M: x: 'a * y: 'b -> Two
 - static member C23.M: x: 'a * y: int -> Three

E_OneTypeVariable03rec.fsx(61,38,61,49): typecheck error FS0041: A unique overload for method 'M' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known types of arguments: 'a * int

Candidates:
 - static member C123.M: x: 'a * y: 'a -> One
 - static member C123.M: x: 'a * y: 'b -> Two
 - static member C123.M: x: 'a * y: int -> Three
