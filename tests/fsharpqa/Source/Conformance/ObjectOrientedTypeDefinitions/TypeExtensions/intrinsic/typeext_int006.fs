// #Conformance #ObjectOrientedTypes #TypeExtensions 
// verify that implementing an interface is not allowed 

namespace NS
  module M = 
    type Lib with
      interface IM 
          with member x.M i = i
    
 
  module F = exit 0



