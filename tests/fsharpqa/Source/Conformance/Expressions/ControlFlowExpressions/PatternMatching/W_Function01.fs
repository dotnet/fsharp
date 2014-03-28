// #Regression #Conformance #ControlFlow 
// Regression test for FSharp1.0:1713
// FS0025: Incomplete pattern matches on this expression... - wording could be improved a bit
//<Expects id="FS0025" span="(8,14-8,24)" status="warning">'0.0'</Expects>

#light

let x = fun (4.1 | 1.26) -> 33    (* warning FS0025: Incomplete pattern matches on this expression. For example, the value '0.0' will not be matched *)

let r = if (x(4.1) = 33) then 0 else 1

exit r
