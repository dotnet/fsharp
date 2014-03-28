// #Conformance #SignatureFiles 
#light

namespace Properties01

namespace Properties01
  type PropertiesTest =
    class
      new : unit -> PropertiesTest
      member GetProperty : int
      member GetSetProperty : string
      member GetSetProperty : string with set
      member SetProperty : string with set
    end
