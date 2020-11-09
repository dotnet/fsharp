// #Regression #Conformance #ObjectOrientedTypes #Structs 
//<Expects id="FS0947" status="error" span="(5,6-5,7)">Struct types cannot contain abstract members$</Expects>
// See FSHARP1.0:5493
[<Struct>]
type S( i: int) =
    abstract M : int -> int
