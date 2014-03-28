// #Conformance #ObjectOrientedTypes #Enums 
#light

// 
namespace NS
  module M = 
    type EnumType  =
      class
        static member A = 0
      end

  module N =
    type EnumType = 
        | A = 0
    
  module Test =    
    let a = M.EnumType.A
    let b = int N.EnumType.A
    
    (a + b) |> exit
