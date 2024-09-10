// #Regression #NoMT #EntryPoint 
// Regression test for FSHARP1.0:1793
// Explicit program entry point: [<EntryPoint>]
// Function does not have type string [] -> unit




module M =
    let func (args : int list) = 
        if(args.Length=0) then 0 else 1

    [<EntryPoint>]
    let main args =
       let res = func(args)
       exit(res)
