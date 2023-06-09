// #Regression #NoMT #EntryPoint 
#light

// Verify error when signature for the [<EntryPoint>] doesn't
// match expected.

//<Expects id="FS0001" span="(15,4-15,6)" status="error"></Expects>

[<EntryPoint>]
let main (args : string[]) =

   printfn "Hello, World"

   // Error, must return an int!
   ()
