// #Regression #NoMT #EntryPoint 
// Regression test for FSHARP1.0:1793
// Explicit program entry point: [<EntryPoint>]
// Two attributes same function (compile with --all-warnings-as-errors to make sure we are totally silent)


module M =
    let func (args : string[]) = 
        if(args.Length=0) then 0 else 1

    [<EntryPoint>]
    [<EntryPoint>]
    let main args =
       let res = func(args)
       exit(res)
