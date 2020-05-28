// #Regression #NoMT #EntryPoint 
// Regression test for FSHARP1.0:1793
// Explicit program entry point: [<ExtryPoint>]
// Entry point function does not have to be called 'main'
//<Expects status="success"></Expects>

#light

module M =
    let func (a : string []) = 
        if(a.Length=0) then 0 else 1

    [<EntryPoint>]
    let main_does_not_need_to_be_called_main a =
       let res = func(a)
       exit(res)
