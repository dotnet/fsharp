// #Regression #Conformance #ObjectOrientedTypes #Classes 
// Regression for FSHARP:6039
// Abstract classes cannot have multiple constructors

module T

[<AbstractClass>]
type MyType(x:int, s:string) =
    public new() = MyType(42,"forty-two")     // This is ok. We are defining a ctor - we are NOT instantiating a MyType object
    member this.X = x
    member this.S = s
    abstract member Foo : int -> int

type A(x:int, s:string) =
    inherit MyType(x,s) with
        override this.Foo x = x + 1
        
let t = A(1, "a")
let r = if t.X = 1 && t.S = "a" then 0 else 1

exit r
