// #Warnings
//<Expects status="Error" span="(4,32)" id="FS0001">Type mismatch. Expecting a.+''a'.+but given a.+'Async<'a>'.+The types ''a' and 'Async<'a>' cannot be unified. Consider using 'return!' instead of 'return'.*</Expects>

let rec foo() = async { return foo() }
    
exit 0