// #Printing 
//<Expects status="error" span="(5,9-5,15)" id="FS0741">error FS0741: Unable to parse format string ''A' format does not support '0' flag'</Expects>
//<Expects status="error" span="(6,9-6,15)" id="FS0741">error FS0741: Unable to parse format string ''A' format does not support precision'</Expects>

sprintf "%03A" "Foo"
sprintf "%.3A" "Bar"

exit 1
