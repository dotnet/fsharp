// #Regression #NoMT #CodeGen #Interop 
// Regression test for FSHARP1.0:4040
// "Signature files do not prevent compiler-generated public constructors from leaking out of discriminated unions"
// Note that the corresponsing .fsi file is NOT missing the "| C of int" part of the DU

namespace N

type T = | C of int

module M =
 
 open CodeGenHelper

 // The public constructor called 'C' is now gone 
 let res1 = System.Reflection.Assembly.GetExecutingAssembly() 
            |> getType "N.T" 
            |> getMembers
            |> Array.tryFind (fun a -> a.Name = "C")
           
 if res1.IsSome then 
          printfn "Unexpected member N.T.C!"
          exit 1

 // Instead, there is a C#-facing method N.T.NewC for each non-nullary union case
 let res2 = System.Reflection.Assembly.GetExecutingAssembly() 
            |> getType "N.T" 
            |> getMethods
            |> Array.tryFind (fun a -> a.Name = "NewC")

 if res2.IsNone then 
          raise (new Exception("static Method N.T.NewC not found!")

 printfn "Finished"