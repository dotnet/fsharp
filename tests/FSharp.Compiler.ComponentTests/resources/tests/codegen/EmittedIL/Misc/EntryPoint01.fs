// #Regression #NoMono #NoMT #CodeGen #EmittedIL 
// Regression test for FSHARP1.0:1792

let static_initializer = 10
 
[<EntryPoint>]
let main (argsz:string []) = 
   exit(if(static_initializer=10) then 0 else 1)
