// #Regression #Conformance #TypesAndModules #Exceptions 
// Exception definition define new discriminated union cases
// Verify that we can use misc types (notice that the "sig-spec" cannot be used [covered in another testcase]
// This is reqgression test for FSHARP1.0:3725
//<Expects status="success"></Expects>

[<Measure>] type Kg

[<NoComparison>]
[<CustomEquality>]
exception E1 of decimal<Kg>
    with
        override o.Equals(o2) = true
        override o.Message = "aa"
        override o.GetHashCode() = 0
    end

let e = E1(1.0M<Kg>)
(if e.Equals(null) then 0 else 1) |> exit
