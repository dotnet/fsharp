// #Warnings
//<Expects status="Error" span="(8,32)" id="FS0001">Type mismatch. Expecting a</Expects>
//<Expects>''a'</Expects>
//<Expects>but given a</Expects>
//<Expects>'Async<'a>'</Expects>
//<Expects>The types ''a' and 'Async<'a>' cannot be unified. Consider using 'return!' instead of 'return'.</Expects>

let rec foo() = async { return foo() }
    
exit 0