// #Regression #Diagnostics 
// Regression test for FSHARP1.0:1181
//<Expects id="FS0010" status="error" span="(8,1)">Incomplete structured construct at or before this point in member definition\. Expected 'with', '=' or other token\.</Expects>


type INumericNorm<'T,'Measure>  = interface INumeric<'T>
                                   member Norm : 'T -> 'Measure
