module Module

type IFace =
    interface
        abstract F : int -> int
        module M =
            let f () = f ()
    end