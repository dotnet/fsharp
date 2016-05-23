// #Warnings
//<Expects status="Error" span="(11,30)" id="FS0001">Type mismatch. Expecting a</Expects>
//<Expects>''a'</Expects>
//<Expects>but given a</Expects>
//<Expects>''a list'</Expects>
//<Expects>The types ''a' and ''a list' cannot be unified. Consider using 'yield!' instead of 'yield'.</Expects>

type Foo() =
  member this.Yield(x) = [x]
  
let rec f () = Foo() { yield f ()}
    
exit 0