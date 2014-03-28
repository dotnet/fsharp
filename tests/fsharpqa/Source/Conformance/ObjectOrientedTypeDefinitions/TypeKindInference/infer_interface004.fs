// #Conformance #ObjectOrientedTypes #TypeInference 
#light
// Verify inference based on type members
module TypeInference 

type TK_I_003 = interface end

type TK_I_004 =
  inherit TK_I_003
  
type TK_I_005 =
  abstract M  : unit -> unit
 
type TK_I_006 = 
  inherit TK_I_005

// this is a negative testcase
//[<AbstractClass>]
//type TK_C_000 =
//   abstract M : int -> int
//type TK_I_007 = 
//  inherit TK_C_000
  
let mutable a = false
try 
  a <- (System.Reflection.Assembly.GetExecutingAssembly().GetTypes() |> Array.find (fun t -> t.FullName = "TypeInference+TK_I_003")).IsInterface
  a <- (System.Reflection.Assembly.GetExecutingAssembly().GetTypes() |> Array.find (fun t -> t.FullName = "TypeInference+TK_I_004")).IsInterface
  a <- (System.Reflection.Assembly.GetExecutingAssembly().GetTypes() |> Array.find (fun t -> t.FullName = "TypeInference+TK_I_005")).IsInterface
  a <- (System.Reflection.Assembly.GetExecutingAssembly().GetTypes() |> Array.find (fun t -> t.FullName = "TypeInference+TK_I_006")).IsInterface
with 
  | _ as e -> 
    printfn "%A" e
    a <- false
    
(if (a) then 0 else 1) |> exit


