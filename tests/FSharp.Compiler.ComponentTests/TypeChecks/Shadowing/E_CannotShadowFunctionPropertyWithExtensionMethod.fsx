open System.Runtime.CompilerServices

type Foo() = 
    member val X : int -> int = (+) 1 with get,set

[<Extension>]
type FooExt =
    [<Extension>]
    static member X (f: Foo, i: int -> int) = f.X <- i; f

let f = Foo()
f.X 1
if f.X 0 <> 1 then
    System.Environment.Exit 1
f.X <- (-) 1

if f.X 2 <> -1 then
    System.Environment.Exit 2

f.X((-) 1) // This expression was expected to have type 'int' but here has type ''a -> 'c'