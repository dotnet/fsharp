// #Conformance #TypesAndModules #Unions 
// DU may include overrides
//<Expects status="success"></Expects>

[<CustomEquality>]
[<NoComparison>]
type T1 = | C of int * int
          | D of (int * int)
          override x.ToString() = "Hello"
          override x.Equals(o) = o = null
          override x.GetHashCode() = 1

let d = D(1,2)

if d.ToString()="Hello" && d.Equals(null) && d.GetHashCode()=1 then 0 else failwith "Failed: 1"
