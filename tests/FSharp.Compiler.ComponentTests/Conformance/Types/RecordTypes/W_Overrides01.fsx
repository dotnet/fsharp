// #Regression #Conformance #TypesAndModules #Records 
// Trying to override GetHashCode(), Equals() and ToString() yields warnings
// See also FSHARP1.0:5446 

[<Measure>] type Kg

type I = { A : float<Kg> ; B : decimal<Kg> }
         with
            member x.GetHashCode(o) = 1
            member x.Equals(o) = false
            member x.ToString() = "Hello"
         end
let p = { A = 10.0<Kg>; B = 11.0M<Kg> }
