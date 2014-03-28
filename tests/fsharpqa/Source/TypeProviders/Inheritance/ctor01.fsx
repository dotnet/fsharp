// #Regression #TypeProvider #Inheritance #Generated
// This is regression test for DevDiv:392751
// [Type Providers] Constructors in derived classes from generated type are created incorrectly
// We used to emit BAD code (i.e. failing peverify-cation)

//<Expects status="success">type T =</Expects>
//<Expects status="success">  class</Expects>
//<Expects status="success">    new : s: string -> T</Expects>
//<Expects status="success">    member Get : unit -> string</Expects>
//<Expects status="success">  end</Expects>
//<Expects status="success">type DT =</Expects>
//<Expects status="success">  class</Expects>
//<Expects status="success">    inherit T</Expects>
//<Expects status="success">    new : unit -> DT</Expects>
//<Expects status="success">  end</Expects>
//<Expects status="success">val M : a:T -> unit</Expects>

type T = GeneratedTypes.Root
type DT() = inherit T("default")

let M (a : T) = 
    let _ = a.Get()
    ()

M(new T("default"))
M(new DT())

#q;;