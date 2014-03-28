// #Conformance #ObjectOrientedTypes #Structs 
// Verify the struct attribute works as expected

[<Struct>]
type Foo =
    new(_x, _y) = { x = _x; y = _y }
    val x : int
    val y : int

    member this.X = this.x
    member this.Y = this.y


let structArray = Array.zeroCreate 1 : Foo[]

// If Foo were a reference type this would throw a
// null reference exception.
let test = structArray.[0]
if test.X <> 0 || test.Y <> 0 then exit 1

exit 0
