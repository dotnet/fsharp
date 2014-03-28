// #Conformance #ObjectOrientedTypes #TypeExtensions  
// verify that overloading a member is allowed.

namespace NS

  module M = 
    type Lib with
      member x.M(i,j) = i + j
    
  
  module F =
    open M
    let mutable res = true
    
    let a = new Lib()
    if not (a.M(2, 3)  + (a.M 1) = 3) then
      printf "Lib.TypeExt_opt004.fs failed\n"
      res <- false
