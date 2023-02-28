// #Regression #NoMT #EntryPoint 
// Regression test for FSHARP1.0:1793
// Explicit program entry point: [<ExtryPoint>]
// Function does not have type string [] -> unit
//<Expects id="FS0001" span="(15,23-15,27)" status="error"></Expects>



module M =
    let func (args : int list) = 
        if(args.Length=0) then 0 else 1

    [<EntryPoint>]
    let main args =
       let res = func(args)
       exit(res)
