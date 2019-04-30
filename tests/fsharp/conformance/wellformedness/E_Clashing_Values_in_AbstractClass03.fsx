// #Regression #Conformance #TypeInference 
// Regression test for FSHARP1.0:5939
// With generics
//<Expects status="error" span="(18,18-18,25)" id="FS0041">A unique overload for method 'M' could not be determined based on type information prior to this program point\. A type annotation may be needed\. Candidates: abstract member D\.M : 'T -> int, abstract member D\.M : 'U -> string$</Expects>
//<Expects status="error" span="(19,18-19,25)" id="FS0041">A unique overload for method 'M' could not be determined based on type information prior to this program point\. A type annotation may be needed\. Candidates: abstract member D\.M : 'T -> int, abstract member D\.M : 'U -> string$</Expects>
[<AbstractClass>]
type D<'T,'U>() = 
    abstract M : 'T  -> int
    abstract M : 'U  -> string

// not an abstract class
type D2<'T,'U>() = 
    inherit D<'T,'U>()
    override this.M(x:'T) = 2
    override this.M(x:'U) = "text"

let d2 = D2<int,int>()
let x : int    = d2.M(2)    // should not be ok
let x : string = d2.M(2)    // should not be
