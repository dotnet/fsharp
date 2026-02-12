// #Regression #Conformance #Quotations 
// Verify quotations cannot contain object expressions
//<Expects id="FS0449" status="error" span="(5,12-6,46)">Quotations cannot contain object expressions$</Expects>
    
let _ = <@ { new System.IDisposable with
                 member this.Dispose() = () } @>
