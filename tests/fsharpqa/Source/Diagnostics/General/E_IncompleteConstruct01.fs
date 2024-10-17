// #Regression #Diagnostics 
// Regression test for FSHARP1.0:1181
//<Expects id="FS3567" status="error" span="(8,1)">Expecting member body</Expects>


type INumericNorm<'T,'Measure>  = interface INumeric<'T>
                                   member Norm : 'T -> 'Measure
