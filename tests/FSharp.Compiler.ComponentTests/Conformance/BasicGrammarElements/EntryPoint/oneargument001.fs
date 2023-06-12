// #Regression #NoMT #EntryPoint 
// Regression test for FSHARP1.0:1304
// Explicit program entry point: [<ExtryPoint>]
// 'main' function invoked with 1 argument
//<CmdLine>Hello</CmdLine>
//<Expects status="success"></Expects>

#light

[<EntryPoint>]
let main args =
   exit(if(args.Length=1 && args.[0]="Hello") then 0 else 1)
