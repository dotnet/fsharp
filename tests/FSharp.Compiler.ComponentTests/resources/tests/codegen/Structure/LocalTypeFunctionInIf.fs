// #Regression #CodeGen #ControlFlow
// Test for 917383, this used to fail verification

let f inp = 
   let lf s v = List.ofSeq s |> List.findIndex (fun y -> y = v)
   match inp with 
   | 0 -> 1
   | _ -> 
   match inp with 
   | 0 -> 1
   | _ -> 
   match inp with 
   | 0 -> 1
   | _ -> 
   match inp with 
   | 0 -> 1
   | _ -> 
   match inp with 
   | 0 -> 1
   | _ -> 
   match inp with 
   | 0 -> 1
   | _ -> 
   match inp with 
   | 0 -> 1
   | _ -> 
   match inp with 
   | 0 -> 1
   | _ -> 
   match inp with 
   | 0 -> 1
   | _ -> 
   match inp with 
   | 0 -> 1
   | _ -> 
   match inp with 
   | 0 -> 1
   | _ -> 
   match inp with 
   | 0 -> 1
   | _ -> 
   match inp with 
   | 0 -> 1
   | _ -> 
   match inp with 
   | 0 -> 1
   | _ -> 
   match inp with 
   | 0 -> 1
   | _ -> 
   match inp with 
   | 0 -> 1
   | _ -> 
   match inp with 
   | 0 -> 1
   | _ -> 
   match inp with 
   | 0 -> 1
   | _ -> 
   match inp with 
   | 0 -> 1
   | _ -> 
   match inp with 
   | 0 -> 1
   | _ -> 
       printfn "res = %d" (lf [1;2] 1)
       4

exit 0
