// #Regression #NoMT #EntryPoint 
// Regression test for FSHARP1.0:1304 - FSHARP1.0:3783
// Explicit program entry point: [<ExtryPoint>]
// attribute on invalid language element (error)
//<Expects id="FS0842" span="(9,3-9,13)" status="error">This attribute is not valid for use on this language element</Expects>

#light

[<EntryPoint>]
module M =
    let func (args : string[]) = 
        if(args.Length=0) then 0 else 1

    [<EntryPoint>]
    let main args =
       let res = func(args)
       exit(res)
