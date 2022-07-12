
neg132.fs(15,9,15,55): typecheck error FS3510: Using methods with 'NoEagerConstraintApplicationAttribute' requires /langversion:6.0 or later

neg132.fs(15,9,15,55): typecheck error FS0041: A unique overload for method 'SomeMethod' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known types of arguments: 'a * ('b -> int)

Candidates:
 - static member OverloadsWithSrtp.SomeMethod: x:  ^T * f: ( ^T -> int) -> int when  ^T: (member get_Length:  ^T -> int)
 - static member OverloadsWithSrtp.SomeMethod: x: 'T list * f: ('T list -> int) -> int