// #Regression #Conformance #TypeRelatedExpressions #TypeAnnotations 
// Regression test for FSharp1.0:3644 - Type annotation ignored for lookup of union case
//<Expects id="FS0001" span="(6,29-6,32)" status="error">This expression was expected to have type.    'int'    .but here has type.    'string'</Expects>

type Data<'a> = ConsData of 'a * Data<'a> | EmptyData
let d = (Data<int>.ConsData("A", Data.EmptyData)) 

