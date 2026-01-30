// #Conformance #ObjectOrientedTypes #TypeExtensions 
// verify that hiding a member is allowed.

namespace NS
  module M = 
    type Lib with
      member x.M i = i
    
  
  module F =
    open M
    let mutable res = true
    
    let a = new Lib()
    if not (a.M 1 = 1) then
      printf "Lib.TypeExt_int004.fs failed\n"
      res <- false
