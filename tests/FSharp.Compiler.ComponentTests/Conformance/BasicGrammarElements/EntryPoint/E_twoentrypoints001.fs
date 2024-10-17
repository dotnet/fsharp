// #Regression #NoMT #EntryPoint 
// Regression test for FSHARP1.0:1304
// Explicit program entry point: [<EntryPoint>]
// attribute on multiple functions
//<CmdLine>Hello</CmdLine>


#light
module M =
    let func (args : string[]) = 
        if(args.Length=1 && args.[0]="Hello") then 0 else 1

    [<EntryPoint>]
    let main args =
       let res = func(args)
       exit(res)

    [<EntryPoint>]
    let main2 args =
       let res = func(args)
       exit(res)
