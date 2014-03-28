// #Regression #TypeProvider #MethodsAndProperties
// This is regression test for DevDiv:217538 ("[TypeProviders] Post-hoc property setters on erased types result in invalid IL")
//<Expects status="success"></Expects>

let t = N.T( IntProp = 42 )

let _ = t.GetHashCode()

exit 0



