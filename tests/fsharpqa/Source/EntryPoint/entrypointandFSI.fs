// #Regression #NoMT #EntryPoint 
// Regression test for FSHARP1.0:2209 - existence of this attribute should not confuse FSI
// Explicit program entry point: [<ExtryPoint>]
//<Expects status="success"></Expects>

#light

module M =
 [<EntryPoint>]
 let main_does_not_need_to_be_called_main a =
   printfn "Yo"
   exit 0;;
#q;;
