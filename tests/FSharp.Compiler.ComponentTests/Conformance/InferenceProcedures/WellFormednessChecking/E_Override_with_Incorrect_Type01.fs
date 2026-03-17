// #Regression #Conformance #TypeInference 
// Regression tests for FSHARP1.0:1348, 4253
//<Expects id="FS0442" span="(12,23-12,30)" status="error">Duplicate method\. The abstract method 'DoStuff' has the same name and signature as an abstract method in an inherited type</Expects>

type A() = class
             abstract DoStuff : int -> int
             override x.DoStuff x' = 42
           end
           
type B() = class
             inherit A()
             abstract DoStuff : int -> int       // error: duplicate method!
             default x.DoStuff x' = 50           // commenting out as not essential: see FSHARP1.0:6013
           end
