// #Regression #Diagnostics 
// Regression test for FSHARP1.0:2391, FSHARP1.0:1479
//<Expects id="FS0020" span="(5,1-5,24)" status="warning">The result of this expression has type 'Quotations.Var seq' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'.</Expects>
#light "off"
<@@ 1 @@>.GetFreeVars()
