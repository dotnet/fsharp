
E_Slices01.fsx(15,9,15,19): typecheck error FS0041: A unique overload for method 'GetSlice' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known types of arguments: int * int option * 'a option

Candidates:
 - member Foo.GetSlice: x: int * y1: int option * y2: float option -> unit
 - member Foo.GetSlice: x: int * y1: int option * y2: int option -> unit

E_Slices01.fsx(16,9,16,17): typecheck error FS0041: A unique overload for method 'GetSlice' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known types of arguments: int * 'a option * 'b option

Candidates:
 - member Foo.GetSlice: x: int * y1: int option * y2: float option -> unit
 - member Foo.GetSlice: x: int * y1: int option * y2: int option -> unit

E_Slices01.fsx(17,9,17,19): typecheck error FS0041: A unique overload for method 'GetSlice' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known types of arguments: 'a option * int option * int

Candidates:
 - member Foo.GetSlice: x1: float option * x2: int option * y: int -> unit
 - member Foo.GetSlice: x1: int option * x2: int option * y: int -> unit

E_Slices01.fsx(18,9,18,17): typecheck error FS0041: A unique overload for method 'GetSlice' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known types of arguments: 'a option * 'b option * int

Candidates:
 - member Foo.GetSlice: x1: float option * x2: int option * y: int -> unit
 - member Foo.GetSlice: x1: int option * x2: int option * y: int -> unit

E_Slices01.fsx(19,9,19,17): typecheck error FS0039: The type 'Foo<_>' does not define the field, constructor or member 'Item'.

E_Slices01.fsx(20,9,20,26): typecheck error FS0503: A member or object constructor 'GetSlice' taking 4 arguments is not accessible from this code location. All accessible versions of method 'GetSlice' take 3 arguments.

E_Slices01.fsx(21,9,21,20): typecheck error FS0503: A member or object constructor 'GetSlice' taking 5 arguments is not accessible from this code location. All accessible versions of method 'GetSlice' take 3 arguments.
