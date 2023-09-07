open Prelude

module Bug820b = 

    type  Bug820x() = 
        let f (x, r:byref<_>) = r <- x
        let mutable x = Unchecked.defaultof<_>
        member __.P = f (0, &x)