// Expected: Warning for module inside union
module Module

type U =
    | A
    | B
    module M3 =
        let f () = ()
