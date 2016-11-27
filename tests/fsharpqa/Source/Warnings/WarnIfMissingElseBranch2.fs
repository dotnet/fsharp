// #Warnings
//<Expects status="Error" id="FS0001">This expression was expected to have type</Expects>

let x = 10
let y =
   if x > 10 then 
     if x <> "test" then printfn "test"
     ()
    
exit 0