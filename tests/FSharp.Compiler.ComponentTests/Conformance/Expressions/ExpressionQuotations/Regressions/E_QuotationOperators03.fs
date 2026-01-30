// #Regression #Conformance #Quotations 
// Regression for 6073 (quotation operators)
//<Expects status="error" id="FS0010" span="(6,18-6,22)">Unexpected infix operator in binding</Expects>
module M

let inline f x = <@-- x --@>

exit 0
