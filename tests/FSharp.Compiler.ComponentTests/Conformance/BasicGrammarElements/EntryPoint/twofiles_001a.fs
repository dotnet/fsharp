// #Regression #NoMT #EntryPoint 
// Regression test for FSHARP1.0:1304
// Explicit program entry point: [<EntryPoint>]
// 'main' function invoked with no arguments
//<CmdLine>Hello</CmdLine>
//<Expects status="success"></Expects>

module M

let func (args : string[]) = 
    if(args.Length=1 && args.[0]="Hello") then 0 else 1

