// Testing: Cascading invalid nested constructs
module Module

type A =
    type B = int
        module C = ()
            exception D
