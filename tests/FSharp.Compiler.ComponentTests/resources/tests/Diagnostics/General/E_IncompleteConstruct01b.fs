// #Regression #Diagnostics 
// Regression test for FSHARP1.0:1181
//<Expects status="notin">syntax error</Expects>
//<Expects id="FS3567" status="error"></Expects>



type INumericNorm<'T,'Measure>  = interface INumeric<'T>
                                   member Norm : 'T -> 'Measure
