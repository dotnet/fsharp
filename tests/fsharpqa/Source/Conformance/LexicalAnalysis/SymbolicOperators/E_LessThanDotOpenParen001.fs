// #Regression #Conformance #LexicalAnalysis #Operators 
// Regression test for FSHARP1.0:4805
// We are not really after the actual error messages here (some of them have been omitted), rather we
// want to verify we do not crash!
//<Expects status="warning" id="FS0064">This construct causes code to be less generic than indicated by the type annotations\. The type variable 'S has been constrained to be type 'int'</Expects>
//<Expects status="error" id="FS0670">This code is not sufficiently generic\. The type variable  \^T when  \^T : \(static member \( \+ \) :  \^T \*  \^T ->  \^a\) could not be generalized because it would escape its scope</Expects>

type public TestType<'T,'S>() =
    
    member public s.Value with get() = Unchecked.defaultof<'T>
    static member public (+++) (a : TestType<'T,'S>, b : TestType<'T,'S>) = a.Value
    static member public (+++) (a : TestType<'T,'S>, b : 'T) = b
    static member public (+++) (a : 'T, b : TestType<'T,'S>) = a
    static member public (+++) (a : TestType<'T,'S>, b : 'T -> 'S) = a.Value
    static member public (+++) (a : 'S -> 'T, b : TestType<'T,'S>) = (a 17) + b.Value

let inline (+++) (a : ^a) (b : ^b) = ((^a or ^b): (static member (+++): ^a * ^b -> ^c) (a,b) )

let tt0 = TestType<int, string>()
let tt1 = TestType<int, string>()

let f (x : string) = 18

let a0 = tt0 +++ tt1
let a1 = tt0 +++ 11
let a2 =  12 +++ tt1
let a3 = tt0 +++ (fun x -> "18")
let a4 = f +++ tt0

let a5 = TestType<int, string>.(+++)(f, tt0)
let a6 = TestType<int, string>.(+++)((fun (x : string) -> 18), tt0)
