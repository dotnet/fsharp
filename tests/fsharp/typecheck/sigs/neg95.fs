namespace rec Neg95

    module rec RecImplied = 
      let x = 1

namespace Neg95B

    module rec ModuleAbbreviationsAfterOpen = 
      module M = List

      open System

    module rec ModuleAbbreviationsBeforeLet = 

      open System

      let x = 1

      module M = List


    module rec OpenBeforeLet = 

      let x = 1

      open System

    module rec OpenBeforeType = 

      type C() = class end

      open System


    module rec OpenBeforeException = 

      exception E of string

      open System

    //module rec RecNotAllowedOnModuleAbbreviation = List


    [<Struct>]
    type StructRecord =
        {
            X: float
            Y: StructRecord
        }

    [<Struct>]
    type StructUnion = StructUnion of float * StructUnion 
