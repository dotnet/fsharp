// #Regression #Diagnostics 
// Regression test for FSHARP1.0:2391, FSHARP1.0:1479
//<Expects id="FS0020" span="(5,1-5,24)" status="warning">This expression has a value of type 'seq.Quotations\.Var.' that is implicitly ignored</Expects>
#light "off"
<@@ 1 @@>.GetFreeVars()
