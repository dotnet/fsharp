// #Regression #Conformance #PatternMatching #ActivePatterns 
// Regression test for FSHARP1.0:5590
// Note that the real test is E_ActivePatternUnconstrained01.fs
// This ones shows just one way to make the code compile again

let (|A1|A2|A3|) (inp:int) : Choice<unit, unit, unit> = 
    printfn "hello"
    printfn "hello"
    A1

let f2 x = match x with A1 -> 1 | A2 -> 2 | A3 -> 3 

f2 3
