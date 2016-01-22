// #NoMT #CompilerOptions 
//<Expected status=success></Expects>

[<EntryPoint>]
let main args =
   let expected =
   #if FROM_RESPONSE_FILE_1
       "ok"
   #else
       "fail"
   #endif

   exit(if(expected = "ok") then 0 else 1)
