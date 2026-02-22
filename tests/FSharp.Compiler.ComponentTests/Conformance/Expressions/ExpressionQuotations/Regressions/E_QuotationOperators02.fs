// #Regression #Conformance #Quotations 
// Regression for 6073 (quotation operators)
//<Expects status="error" id="FS0010" span="(6,9-6,13)">Unexpected infix operator in binding</Expects>
module M

let x = <@-- 1 --@>

exit 0
