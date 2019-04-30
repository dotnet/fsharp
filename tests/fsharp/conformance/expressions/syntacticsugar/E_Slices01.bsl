
E_Slices01.fsx(22,9,22,19): typecheck error FS0041: A unique overload for method 'GetSlice' could not be determined based on type information prior to this program point. A type annotation may be needed.



Arguments given:

 - int

 - int option

 - 'a option







Candidates:

 - member Foo.GetSlice : x:int * y1:int option * y2:float option -> unit

 - member Foo.GetSlice : x:int * y1:int option * y2:int option -> unit



E_Slices01.fsx(23,9,23,17): typecheck error FS0041: A unique overload for method 'GetSlice' could not be determined based on type information prior to this program point. A type annotation may be needed.



Arguments given:

 - int

 - 'a option

 - 'a option







Candidates:

 - member Foo.GetSlice : x:int * y1:int option * y2:float option -> unit

 - member Foo.GetSlice : x:int * y1:int option * y2:int option -> unit



E_Slices01.fsx(24,9,24,19): typecheck error FS0041: A unique overload for method 'GetSlice' could not be determined based on type information prior to this program point. A type annotation may be needed.



Arguments given:

 - 'a option

 - int option

 - int







Candidates:

 - member Foo.GetSlice : x1:float option * x2:int option * y:int -> unit

 - member Foo.GetSlice : x1:int option * x2:int option * y:int -> unit



E_Slices01.fsx(25,9,25,17): typecheck error FS0041: A unique overload for method 'GetSlice' could not be determined based on type information prior to this program point. A type annotation may be needed.



Arguments given:

 - 'a option

 - 'a option

 - int







Candidates:

 - member Foo.GetSlice : x1:float option * x2:int option * y:int -> unit

 - member Foo.GetSlice : x1:int option * x2:int option * y:int -> unit



E_Slices01.fsx(26,9,26,17): typecheck error FS0039: The type 'Foo<a>' does not define the field, constructor or member 'Item'.

E_Slices01.fsx(27,9,27,26): typecheck error FS0503: A member or object constructor 'GetSlice' taking 4 arguments is not accessible from this code location. All accessible versions of method 'GetSlice' take 3 arguments.

E_Slices01.fsx(28,9,28,20): typecheck error FS0503: A member or object constructor 'GetSlice' taking 5 arguments is not accessible from this code location. All accessible versions of method 'GetSlice' take 3 arguments.
