// #Regression #Conformance #Quotations 
// Verify restrictions for what can be written in a quotation
//<Expects id="FS0010" status="error">Unexpected keyword 'type' in quotation literal</Expects>

let _ = <@ type Foo(x : int) =
              member this.Length = x @>     

exit 1
