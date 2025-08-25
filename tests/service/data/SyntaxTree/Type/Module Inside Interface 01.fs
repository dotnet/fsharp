// Expected: Warning for module inside interface
module Module

type IFace =
    abstract F : int -> int
    module M =
        let f () = ()
