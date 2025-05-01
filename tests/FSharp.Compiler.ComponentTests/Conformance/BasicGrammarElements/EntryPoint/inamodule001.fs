// #Regression #NoMT #EntryPoint 
// Regression test for FSHARP1.0:1304
// Explicit program entry point: [<EntryPoint>]
// 'main' function is in a module
//<CmdLine>Hello</CmdLine>
//<Expects status="success"></Expects>

#light
module M =
    let func (args : string[]) = 
        if(args.Length=1 && args.[0]="Hello") then 0 else 1

    [<EntryPoint>]
    let main args =
       let res = func(args)
       exit(res)
