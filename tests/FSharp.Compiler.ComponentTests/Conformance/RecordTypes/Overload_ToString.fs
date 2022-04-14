// #Regression #Conformance #TypesAndModules #Records #ReqNOMT 
// Regression test for FSHARP1.0:5223
// Overloading of ToString()

type R =
    { x: int }
    member this.ToString(s:char) = true
    member this.ToString() =       true
    member this.ToString(?s:char) = true
