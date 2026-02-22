// #Regression #Conformance #ObjectOrientedTypes #Structs #ReqNOMT 
// Regression test for FSHARP1.0:5223
// Overloading of Equals()
//<Expects status="error" span="(8,17-8,23)" id="FS0438">Duplicate method\. The method 'Equals' has the same name and signature as another method in type 'S3'\.$</Expects>

[<Struct>]
type S3 =
    member this.Equals(s:S3) = 1.
