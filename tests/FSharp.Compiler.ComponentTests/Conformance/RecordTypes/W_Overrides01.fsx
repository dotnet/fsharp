// #Regression #Conformance #TypesAndModules #Records 
// Trying to override GetHashCode(), Equals() and ToString() yields warnings
// See also FSHARP1.0:5446 
//<Expects id="FS0864" span="(11,22-11,30)" status="warning">This new member hides the abstract member 'System\.Object\.ToString\(\) : string'\. Rename the member or use 'override' instead\.$</Expects>
[<Measure>] type Kg

type I = { A : float<Kg> ; B : decimal<Kg> }
         with
            member x.GetHashCode(o) = 1
            member x.Equals(o) = false
            member x.ToString() = "Hello"
         end
let p = { A = 10.0<Kg>; B = 11.0M<Kg> }
