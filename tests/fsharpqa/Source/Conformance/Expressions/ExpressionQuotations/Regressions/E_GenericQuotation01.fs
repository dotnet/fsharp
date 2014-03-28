// #Regression #Conformance #Quotations 
#light

// Verify error for generalized quotations
//<Expects id="FS1230" status="error">Inner generic functions are not permitted in quoted expressions\. Consider adding some type constraints until this function is no longer generic</Expects>

let _ = <@ let x = []
           x           @>     

exit 1
