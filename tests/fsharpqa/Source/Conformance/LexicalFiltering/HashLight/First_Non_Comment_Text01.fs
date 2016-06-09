// #Regression #Conformance #LexFilter 
// Regression test for FSHARP1.0:1078
// The #light is now the default. See also FSHARP1.0:2319
//<Expects status="notin">#light</Expects>
//<Expects status="warning" span="(6,1)" id="FS0988">Main module of program is empty: nothing will happen when it is run</Expects>
