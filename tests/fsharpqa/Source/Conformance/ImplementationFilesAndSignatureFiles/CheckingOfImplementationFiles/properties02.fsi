// #Conformance #SignatureFiles 
#light

namespace Properties01
  type AbstractPropertiesTest =
    interface
      abstract member GetProperty : int
      abstract member GetSetProperty : string
      abstract member GetSetProperty : string with set
      abstract member SetProperty : string with set
    end
  type Implementation =
    class
      interface AbstractPropertiesTest
      new : unit -> Implementation
    end
  module Test = begin
    val t : AbstractPropertiesTest
  end
