// #Regression #Conformance #ObjectOrientedTypes #Structs #ReqNOMT 
// Regression test for FSHARP1.0:5223
// Overloading of ToString()

[<Struct>]
type S1 =
    member this.ToString(s:char) = true
    member this.ToString() =       true
    member this.ToString(?s:char) = true
