// #Regression #Conformance #ObjectOrientedTypes #TypeExtensions  
//<Expects id="FS0909" span="(7,17-7,19)" status="error">All implemented interfaces should be declared on the initial declaration of the type</Expects>

namespace NS
  module M = 
    type Lib with
      interface IM 
          with member x.M i = i
  
  module F = exit 0



