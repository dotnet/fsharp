// #Conformance #ObjectOrientedTypes #Enums 

namespace NS
  module M = 
    type SimpleEnum =
        | A = 0

  module Test =
    (int M.SimpleEnum.A) |> exit
