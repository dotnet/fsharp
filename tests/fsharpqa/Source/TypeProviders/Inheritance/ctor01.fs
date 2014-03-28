// #Regression #TypeProvider #Inheritance #Generated
// This is regression test for DevDiv:392751
// [Type Providers] Constructors in derived classes from generated type are created incorrectly
// We used to emit BAD code (i.e. failing peverify-cation)
//<Expects status="success"></Expects>

type T = GeneratedTypes.Root
type DT() = inherit T("default")

let M (a : T) = 
    let _ = a.Get()
    ()

M(new T("default"))
M(new DT())

