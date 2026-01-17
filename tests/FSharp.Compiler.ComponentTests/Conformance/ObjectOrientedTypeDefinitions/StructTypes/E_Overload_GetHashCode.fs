// #Regression #Conformance #ObjectOrientedTypes #Structs #ReqNOMT 
// Regression test for FSHARP1.0:5223
// Overloading of GetHashCode()
//<Expects status="error" span="(8,17-8,28)" id="FS0438">Duplicate method\. The method 'GetHashCode' has the same name and signature as another method in type 'S2'\.$</Expects>

[<Struct>]
type S2 =
    member this.GetHashCode() = 1.
