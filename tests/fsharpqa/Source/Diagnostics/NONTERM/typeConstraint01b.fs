// #Regression #Diagnostics 
// Regression test for FSHARP1.0:2648
//<Expects status="notin">NONTERM</Expects>
//<Expects id="FS0010" status="error"></Expects>
#light

type MonadMaker<'m, 
                ^a when ^a : (new : unit -> ^a) and
                        ^a : (static member Zero : unit -> 'm<'b>)



