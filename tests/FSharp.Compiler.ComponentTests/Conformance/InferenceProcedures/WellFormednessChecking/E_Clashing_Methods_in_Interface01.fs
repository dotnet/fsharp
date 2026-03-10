// #Regression #Conformance #TypeInference 
// Regression tests for FSHARP1.0:1348
//<Expects id="FS0438" span="(7,14-7,21)" status="error">Duplicate method\. The method 'DoStuff' has the same name and signature as another method in type 'IFoo'</Expects>

type IFoo =
    abstract DoStuff : int -> (int -> int)
    abstract DoStuff : int -> (int -> int -> int)
    abstract DoStuff : int -> (int -> int -> int)

