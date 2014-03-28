// #Regression #Conformance #ObjectOrientedTypes #Structs 
#light

// FSB 1768, Allow the definition of immutable structs using the implicit construction syntax

[<Struct>]
type MyInt(sign:int, v : int) =
    member this.xSig = sign
    member this.xV = v

let someStruct = MyInt(-1, 42)
if someStruct.xSig <> -1 or someStruct.xV <> 42 then exit 1

exit 0
