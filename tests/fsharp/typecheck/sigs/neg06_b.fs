module Test

module Regression_4688 =
    type BMap<'v>(anyX : int) =
      struct
        static let    empty: BMap<'v> = failwith""
        static member Empty: BMap<'v> = empty
      end

    // This type is OK, note 'v instance of BMap        
    type CMap<'v> (bmap: BMap<'v>) =
      struct
        member x.bmap = bmap
      end

    // This type is OK, note 'v option instance of BMap
    // Prefix: it looped (non-termination, but no stack overflow)
    type DMap<'v> (bmap: BMap<'v option>) =
      struct
        member x.bmap = bmap
      end 
