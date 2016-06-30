// #Regression #Conformance #PatternMatching #ActivePatterns 
// Regression test for FSHARP1.0:5590
// This code used to compile, but fail peverification
// Now, it just does not compile anymore telling the user to annotated it a bit.
//<Expects status="warning" span="(14,1-14,5)" id="FS0020">The result of this expression is implicitly ignored\. Consider using 'ignore' to discard this value explicitly, e\.g\. 'expr \|> ignore', or 'let' to bind the result to a name, e\.g\. 'let result = expr'.$</Expects>
//<Expects status="error" span="(7,6-7,16)" id="FS1210">Active pattern '\|A1\|A2\|A3\|' has a result type containing type variables that are not determined by the input\. The common cause is a when a result case is not mentioned, e\.g\. 'let \(\|A\|B\|\) \(x:int\) = A x'\. This can be fixed with a type constraint, e\.g\. 'let \(\|A\|B\|\) \(x:int\) : Choice<int,unit> = A x'$</Expects>
let (|A1|A2|A3|) (inp:int) : Choice<unit,'T,'U> = 
    printfn "hello"
    printfn "hello"
    A1

let f2 x = match x with A1 -> 1 | A2 -> 2 | A3 -> 3 

f2 3
