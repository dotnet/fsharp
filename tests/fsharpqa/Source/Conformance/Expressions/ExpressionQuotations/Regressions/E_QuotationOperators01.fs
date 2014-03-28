// #Regression #Conformance #Quotations 
// Regression for 6073 (quotation operators)
//<Expects status="error" id="FS0010" span="(10,10-10,13)">Unexpected infix operator in binding\. Expected '\)' or other token\.</Expects>
//<Expects status="error" id="FS0583" span="(10,5-10,6)">Unmatched '\('</Expects>
module M

let (<@++@>) x y = x + y
1 <@++@> 1 |> ignore

let (<@+ +@>) x y = x + y

exit 0
