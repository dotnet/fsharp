// #Warnings
//<Expects status="Error" id="FS0039">The field, constructor or member 'AntherCase' is not defined.</Expects>
//<Expects>Maybe you want one of the following:\s+AnotherCase</Expects>

type MyUnion = 
| ASimpleCase
| AnotherCase of int

let u = MyUnion.AntherCase


exit 0