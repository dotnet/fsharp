// #Regression #NoMT #EntryPoint 
// Regression test for FSHARP1.0:1793
// Explicit program entry point: [<ExtryPoint>]
// Two attributes same function (compile with --all-warnings-as-errors to make sure we are totally silent)
//<Expects id="FS0429" span="(12,7-12,17)" status="error">The attribute type 'EntryPointAttribute' has 'AllowMultiple=false'\. Multiple instances of this attribute cannot be attached to a single language element\.$</Expects>

module M =
    let func (args : string[]) = 
        if(args.Length=0) then 0 else 1

    [<EntryPoint>]
    [<EntryPoint>]
    let main args =
       let res = func(args)
       exit(res)
