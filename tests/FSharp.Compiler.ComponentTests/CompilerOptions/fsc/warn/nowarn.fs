// #Regression #NoMT #CompilerOptions 
// Regression test for FSHARP1.0:3789
// Unfixable warning 45
//<Expects status="success"></Expects>
#nowarn "0988"
type I =
   abstract M : unit -> unit
type private X() =
   interface I with
      member x.M() = ()
