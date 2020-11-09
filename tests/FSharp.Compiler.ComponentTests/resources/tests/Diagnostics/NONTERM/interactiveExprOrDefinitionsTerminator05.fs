// #Regression #Diagnostics 
// Regression test for FSHARP1.0:2124
//<Expects id="FS0010" span="(5,16-5,17)" status="error">':'</Expects>

failwith "foo" : int;;
