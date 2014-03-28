// #Conformance #ObjectOrientedTypes #TypeExtensions 
// verify that hiding an interface implementation is allowed 
#light
namespace NS
  module M = 
    type Lib with
          member x.M i = i
    
  
  module F =
    open M
    let mutable res = true
    
    let a = new Lib()
    if not (a.M 1 = 1) then
      printf "Lib.TypeExt_int003.fs failed\n"
      res <- false

    let b = a :> IM
    if not (b.M 1 = 0) then
      printf "Lib.TypeExt_int003.fs failed\n"
      res <- false
      
    (if (res) then 0 else 1) |> exit
