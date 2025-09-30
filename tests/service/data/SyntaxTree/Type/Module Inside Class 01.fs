// Expected: Warning for module inside class
module Module

type C () =
    member _.F () = 3
    module M2 =
        let f () = ()
