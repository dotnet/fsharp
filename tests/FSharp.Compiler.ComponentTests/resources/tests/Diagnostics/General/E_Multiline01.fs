// #Regression #Diagnostics 
// Regression test for FSHARP1.0:1269
// Make sure that error spans correctly across multiple lines
//<Expects status="error" id="FS0001" span="(13,8-15,9)">This expression was expected to have type</Expects>

type Schedule = 
   | Seq of Schedule[]
   | Simple of (int -> int)
   
let add n v = Simple (fun x -> x + n)
   
let rec sched = 
   Seq [ add 1 "v";
         Seq [ add 1 "u"; add 1 "t" ] 
       ]
