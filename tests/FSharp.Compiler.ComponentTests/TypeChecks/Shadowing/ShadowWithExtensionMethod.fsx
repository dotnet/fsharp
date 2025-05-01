open System.Runtime.CompilerServices

type Foo() = 
    member val X : int = 0 with get,set

[<Extension>]
type FooExt =
    [<Extension>]
    static member X (f: Foo, i: int) = f.X <- i; f


let f = Foo()
f.X(1)
if f.X <> 1 then
    System.Environment.Exit 1

f.X(1).X(2).X(3)

if f.X <> 3 then
    System.Environment.Exit 2
