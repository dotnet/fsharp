// #Regression #Diagnostics 
// Regression test for FSharp1.0:2219
//<Expects id="FS0010" span="(8,1-8,4)" status="error">'val'</Expects>

#light

[<RequiresExplicitTypeArguments>] 
val inline sizeof<'a> : int
