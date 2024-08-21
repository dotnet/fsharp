// #Regression #NoMT #EntryPoint 
// Regression test for FSHARP1.0:1304
// Explicit program entry point: [<EntryPoint>]
// Verify that static initializers are invoked before entering the 'main' function
//<Expects status="success"></Expects>

#light

let static_initializer = 10
 
[<EntryPoint>]
let main (argsz:string []) = 
   exit(if(static_initializer=10) then 0 else 1)
   
