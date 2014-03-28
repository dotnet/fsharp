// #Regression #TypeProvider #Fields #Literals
// This is regression test for DevDiv:233896
//<Expects status="success">val t : G = null/Expects>
//<Expects status="success">val it : bool = true</Expects>

type G = N.T;;
let t = new G();;
t.Instance_Field_bool;;

#q;;
