// #Regression #Conformance #Quotations 
// Regression for 6073 (quotation operators)
//<Expects status="error" id="FS0010" span="(7,24-7,28)">Unexpected infix operator in binding\. Expected '\)' or other token\.</Expects>
//<Expects status="error" id="FS0583" span="(7,18-7,19)">Unmatched '\('</Expects>
module M

let inline f x = (<@-- --@>) x

exit 0
