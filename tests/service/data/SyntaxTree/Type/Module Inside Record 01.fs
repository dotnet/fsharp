// Expected: Warning for module inside record
module Module

type R =
    { A : int }
    module M4 =
        let f () = ()
