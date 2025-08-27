// Expected: Warning for module after delegate
module Module

type MyDelegate = delegate of int * int -> int
    module InvalidModule =
        let x = 1
