// #Regression #NoMT #EntryPoint 
#light

// Verify error when signature for the [<EntryPoint>] doesn't
// match expected.



[<EntryPoint>]
let main (args : string[]) =

   printfn "Hello, World"

   // Error, must return an int!
   ()
