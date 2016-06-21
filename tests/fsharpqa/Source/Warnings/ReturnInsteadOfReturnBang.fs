// #Warnings
//<Expects status="Error" span="(8,32)" id="FS0001">Type mismatch. Expecting a</Expects>
//<Expects status="error">''a'</Expects>
//<Expects status="error">but given a</Expects>
//<Expects status="error">'Async<'a>'</Expects>
//<Expects status="error">The types ''a' and 'Async<'a>' cannot be unified. Consider using 'return!' instead of 'return'.</Expects>

let rec foo() = async { return foo() }
    
exit 0