// #Regression #Conformance #DataExpressions 
// Verify that nameof doesn't work on const string
//<Expects id="FS3250" span="(6,16)" status="error">Expression does not have a name.</Expects>
//<Expects id="FS0033" span="(9,19)" status="error">expects 1 type argument(s) but is given 3</Expects>
//<Expects id="FS0033" span="(11,22)" status="error">The type 'GetCarter' is not defined</Expects>
let x = nameof(1+2)
type C<'T>() = class end

let name = nameof C<int, string, GetCarter>

let name2 = nameof C<GetCarter>

exit 0
