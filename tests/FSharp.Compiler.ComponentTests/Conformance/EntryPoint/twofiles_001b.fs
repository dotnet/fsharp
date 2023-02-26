// #Regression #NoMT #EntryPoint 
// Regression test for FSHARP1.0:1304
// Explicit program entry point: [<ExtryPoint>]
// 'main' function invoked with no arguments
//<CmdLine>Hello</CmdLine>
//<Expects status="success"></Expects>

module TestModule

[<EntryPoint>]
let main args =
   let res = if(args.Length=1 && args.[0]="Hello") then 0 else 1
   exit(res)
