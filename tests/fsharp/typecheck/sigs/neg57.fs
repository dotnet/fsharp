module M

[<Struct>]
type Foo<'T> =
   val offset : int
   new (x:'T) = { offset = 1 } 
