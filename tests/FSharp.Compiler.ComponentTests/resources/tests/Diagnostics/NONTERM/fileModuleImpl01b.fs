// #Regression #Diagnostics 
// Regression test for FSharp1.0:2219
//<Expects status="notin">NONTERM</Expects>
//<Expects span="(7,1)" status="error" id="FS0010">Unexpected keyword 'val' in definition</Expects>

[<RequiresExplicitTypeArguments>] 
val inline sizeof<'a> : int
