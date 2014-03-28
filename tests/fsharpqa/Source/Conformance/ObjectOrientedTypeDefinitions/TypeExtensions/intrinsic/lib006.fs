// #Regression #Conformance #ObjectOrientedTypes #TypeExtensions 
//<Expects id="FS0909" span="(7,17-7,19)" status="error">All implemented interfaces should be declared on the initial declaration of the type</Expects>

namespace NS
    type IM = 
      interface
        abstract M : int -> int
      end
      
    type Lib() =
      class
          member x.Prop = 1
     end
 
