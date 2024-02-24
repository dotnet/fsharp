type Enum1 = | E = 1

module Ext =
    type Enum1 with
        member this.Foo() = 42 // ok - extrinsic extension

open Ext
let r = Enum1.E.Foo()
exit(if r = 42 then 0 else 1)
