// #Conformance #ObjectOrientedTypes #TypeExtensions 
#light
namespace NS
    type IM = 
      interface
        abstract M : int -> int
      end
      
    type Lib() =
      class
        interface IM with
          member x.M i = 0
     end
 
