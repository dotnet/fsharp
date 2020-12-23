// #Regression #Conformance #ObjectOrientedTypes #TypeExtensions  
// Error when trying to use member overloading when some overloads are specified using curried arguments
//<Expects span="(17,13-17,23)" status="error" id="FS0816">One or more of the overloads of this method has curried arguments\. Consider redesigning these members to take arguments in tupled form</Expects>
//<Expects span="(17,28-17,33)" status="error" id="FS0816">One or more of the overloads of this method has curried arguments\. Consider redesigning these members to take arguments in tupled form</Expects>

namespace NS
  module M = 
    type Lib with
      member x.M i j = i + j
    
  
  module F =
    open M
    let mutable res = true
    
    let a = new Lib()
    if not (a.M (2, 3)  + (a.M 1) = 3) then
      printf "Lib.TypeExt_opt004.fs failed\n"
      res <- false
