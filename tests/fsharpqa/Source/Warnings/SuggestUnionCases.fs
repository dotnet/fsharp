// #Warnings
//<Expects status="Error" id="FS0039">The field, constructor or member 'AntherCase' is not defined. Maybe you want one of the following: AnotherCase</Expects>

type MyUnion = 
| ASimpleCase
| AnotherCase of int

let u = MyUnion.AntherCase


exit 0