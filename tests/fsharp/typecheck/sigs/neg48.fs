
module Neg48 

// No errors expected in here
module PositiveTests = 
    type tree3 = 
      | NodeA of int * int
      member x.Item0 = 1 // ok

    type tree3static = 
      | NodeA of int * int
      static member Item0 = 1 // ok

    type tree3B =
      | NodeA of int * int
      member x.Item3 = 1 // ok

    type tree4A = 
      | NodeA 
      member x.Item = 1 // ok

    type tree5 = 
      | NodeA of int

    module M = 
        type tree5 with 
          member x.Tag = 1 // ok - extrinsic extension

        type tree5 with 
          member x.Tags = 1 // ok - extrinsic extension

        type tree5 with 
          member x.Item = 1 // ok - extrinsic extension


module NegativeTEsts =
    // more union type checks
    type tree1A = 
      | NodeA of int * int
      member x.Item1 = 1 // not ok

    type tree1Astatic = 
      | NodeA of int * int
      static member Item1 = 1 // not ok

    type tree1B = 
      | NodeA of int  * int
      member x.Item2 = 1 // not ok

    type tree1Bstatic = 
      | NodeA of int  * int
      static member Item2 = 1 // not ok

    type tree1C = 
      | NodeA of int * int
      member x.Tags = 1 // not ok

    type tree1Cstatic = 
      | NodeA of int * int
      static member Tags = 1 // not ok

    type tree1D = 
      | NodeA of int * int
      member x.Tag = 1 // not ok

    type tree2A = 
      | NodeA of int 
      member x.Item = 1 // not ok

    type tree2Astatic = 
      | NodeA of int 
      static member Item = 1 // not ok

    type tree2B = 
      | NodeA of int
      member x.Tags = 1 // not ok

    type tree2Bstatic  = 
      | NodeA of int
      static member Tags = 1 // not ok

    type tree2C = 
      | NodeA of int 
      member x.Tag = 1 // not ok

    type tree2Cstatic = 
      | NodeA of int 
      static member Tag = 1 // not ok

    type tree4B = 
      | NodeA 
      member x.Tags = 1 // not ok

    type tree4C = 
      | NodeA 
      member x.Tag = 1 // not ok

    type tree5 = 
      | NodeA of int

    type tree5 with 
      member x.Tag = 1 // not ok - intrinsic extension

    type tree5 with 
      member x.Tags = 1 // not ok - intrinsic extension

    type tree5 with 
      member x.Item = 1 // not ok - intrinsic extension
