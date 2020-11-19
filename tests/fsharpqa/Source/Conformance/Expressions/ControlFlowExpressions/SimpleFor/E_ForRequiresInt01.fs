// #Regression #Conformance #ControlFlow 
// Verify for loops require integer values for bounds
//<Expects id="FS0001" span="(9,9-9,23)" status="error">This expression was expected to have type.    'int'    .but here has type.    'float'</Expects>
//<Expects id="FS0001" span="(9,27-9,41)" status="error">This expression was expected to have type.    'int'    .but here has type.    'float'</Expects>

let returnFloat1() =  0.0
let returnFloat2() = 10.0

for i = returnFloat1() to returnFloat2() do
    ()
