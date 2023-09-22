open Prelude

module Bug820 = 

    let inline f (x, r:byref<_>) = r <- x
    let mutable x = Unchecked.defaultof<_>
    f (0, &x)