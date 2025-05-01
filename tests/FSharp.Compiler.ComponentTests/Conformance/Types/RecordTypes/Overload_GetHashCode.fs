// #Regression #Conformance #TypesAndModules #Records #ReqNOMT 
// Regression test for FSHARP1.0:5223
// Overloading of GetHashCode()
type R =
    { x: int }
    member this.GetHashCode(s:char) =  1
    //member this.GetHashCode() = 1.
    member this.GetHashCode(?s:char) = 1 
