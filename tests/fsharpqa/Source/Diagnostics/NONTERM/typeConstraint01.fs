// #Regression #Diagnostics 
// Regression test for FSHARP1.0:2648
//<Expects id="FS0010" span="(9,1-9,1)" status="error">Unexpected end of input in type name\. Expected '>' or other token</Expects>
#light

type MonadMaker<'m, 
                ^a when ^a : (new : unit -> ^a) and
                        ^a : (static member Zero : unit -> 'm<'b>)
