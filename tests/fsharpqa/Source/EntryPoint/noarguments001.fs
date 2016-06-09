// #Regression #NoMT #EntryPoint 
// Regression test for FSHARP1.0:1304
// Explicit program entry point: [<ExtryPoint>]
// 'main' function invoked with no arguments
//<Expects status="success"></Expects>

#light

[<EntryPoint>]
let main args =
   exit(if(args.Length=0) then 0 else 1)
   
