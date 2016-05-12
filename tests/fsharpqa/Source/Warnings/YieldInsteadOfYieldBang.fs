// #Warnings
//<Expects status="Error" span="(7,30)" id="FS0001">.Type mismatch. Expecting a.+''a'.+but given a.+''a list'.+The types ''a' and ''a list' cannot be unified. Consider using 'yield!' instead of 'yield'.*</Expects>

type Foo() =
  member this.Yield(x) = [x]
  
let rec f () = Foo() { yield f ()}
    
exit 0