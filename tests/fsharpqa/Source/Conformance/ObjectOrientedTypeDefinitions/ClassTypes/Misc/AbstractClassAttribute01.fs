// #Conformance #ObjectOrientedTypes #Classes 
#light

// Verify that the AbstractClass attribute works as expected.

[<AbstractClass>]
type AbstractFoo() = 
    abstract Square : int -> int
    member this.Cube x = x * x * x


type Foo() =
    inherit AbstractFoo()

    override this.Square x = x * x


let t = new Foo()   

if t.Square 4 <> 16 then exit 1
if t.Cube   3 <> 27 then exit 1

exit 0
