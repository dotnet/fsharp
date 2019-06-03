// #Regression #Diagnostics 
// Regression test for FSHARP1.0:2391, FSHARP1.0:1479
//<Expects status="notin">NONTERM</Expects>
//<Expects id="FS0020" status="warning"></Expects>

#light "off"
<@@ 1 @@>.GetFreeVars()
