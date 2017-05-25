// #Regression #Conformance #TypeInference 
// Regression tests for FSHARP1.0:1348
// Method have different signatures, but the proper OverloadID attribute avoid the clash
//<Expects status="error" span="(7,14-7,21)" id="FS0438">Duplicate method\. The method 'DoStuff' has the same name and signature as another method in type 'IFoo'\.$</Expects>

type IFoo =
    abstract DoStuff : int -> (int -> int)
    abstract DoStuff : int -> (int -> int -> int)
