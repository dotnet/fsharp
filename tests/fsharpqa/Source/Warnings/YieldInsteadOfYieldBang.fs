// #Warnings
//<Expects status="error" span="(11,30)" id="FS0001">Type mismatch. Expecting a</Expects>
//<Expects status="error">''a'</Expects>
//<Expects status="error">but given a</Expects>
//<Expects status="error">''a list'</Expects>
//<Expects status="error">The types ''a' and ''a list' cannot be unified. Consider using 'yield!' instead of 'yield'.</Expects>

type Foo() =
  member this.Yield(x) = [x]
  
let rec f () = Foo() { yield f ()}
    
exit 0