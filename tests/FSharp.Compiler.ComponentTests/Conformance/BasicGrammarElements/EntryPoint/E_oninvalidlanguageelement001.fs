// #Regression #NoMT #EntryPoint 
// Regression test for FSHARP1.0:1304 - FSHARP1.0:3783
// Explicit program entry point: [<EntryPoint>]
// attribute on invalid language element (error)


#light

[<EntryPoint>]
module M =
    let func (args : string[]) = 
        if(args.Length=0) then 0 else 1

    [<EntryPoint>]
    let main args =
       let res = func(args)
       exit(res)
