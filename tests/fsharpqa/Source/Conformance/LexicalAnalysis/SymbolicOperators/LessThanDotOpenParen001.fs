// #Regression #Conformance #LexicalAnalysis #Operators 
// Regression test for FSHARP1.0:4805
//<Expects status="success"></Expects>
 
type public TestType<'T,'S>() =
    
    member public s.Value with get() = Unchecked.defaultof<'T>
    static member public (+++) (a : TestType<'T,'S>, b : TestType<'T,'S>) = a.Value
    static member public (+++) (a : TestType<'T,'S>, b : 'T) = b
    static member public (+++) (a : 'T, b : TestType<'T,'S>) = a
    static member public (+++) (a : TestType<'T,'S>, b : 'T -> 'S) = a.Value
    //static member public (+++) (a : 'S -> 'T, b : TestType<'T,'S>) = (a 17) + b.Value

let inline (+++) (a : ^a) (b : ^b) = ((^a or ^b): (static member (+++): ^a * ^b -> ^c) (a,b) )

let tt0 = TestType<int, string>()
let tt1 = TestType<int, string>()

let f (x : string) = 18

let a0 = tt0 +++ tt1
let a1 = tt0 +++ 11
let a2 =  12 +++ tt1
let a3 = tt0 +++ (fun x -> "18")
//let a4 = f +++ tt0

//let a5 = TestType<int, string>.(+++)(f, tt0)
//let a6 = TestType<int, string>.(+++)((fun (x : string) -> 18), tt0)
