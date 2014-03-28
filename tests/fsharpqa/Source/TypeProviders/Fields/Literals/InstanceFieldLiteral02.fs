// #Regression #TypeProvider #Fields #Literals
// This is regression test for DevDiv:233896
//<Expects status="success"></Expects>

type G = N.T
let t = new G()

t.Instance_Field_bool |> ignore

exit 0
