// #Regression #Conformance #Quotations 
#light

// Verify restrictions for what can be written in a quotation
//<Expects id="FS0462" status="error" span="(8,16-8,17)">Quotations cannot contain this kind of type</Expects>

let _ = <@ let mutable x = 0
           let y = &x
           y @>     
