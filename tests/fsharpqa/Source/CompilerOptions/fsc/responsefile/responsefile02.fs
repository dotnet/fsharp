// #NoMT #CompilerOptions 
//<Expected status=success></Expects>

[<EntryPoint>]
let main args =
   let expected1 =
   #if FROM_RESPONSE_FILE_1
       "ok"
   #else
       "fail"
   #endif

   let expected2 =
   #if FROM_RESPONSE_FILE_2
       "ok"
   #else
       "fail"
   #endif

   exit(if(expected1 = "ok" && expected2 = "ok") then 0 else 1)
