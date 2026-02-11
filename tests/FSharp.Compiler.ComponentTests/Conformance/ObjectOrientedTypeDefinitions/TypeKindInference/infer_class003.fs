// #Conformance #ObjectOrientedTypes #TypeInference 
// Verify the use of class/end
module TypeInference 

type TK_C_002 =
 class
 end

  
let mutable a = false
try 
  a <- (System.Reflection.Assembly.GetExecutingAssembly().GetTypes() |> Array.find (fun t -> t.FullName = "TypeInference+TK_C_002")).IsClass
with 
  | _ as e -> 
    printfn "%A" e
    a <- false
    
(if (a) then 0 else 1) |> exit
