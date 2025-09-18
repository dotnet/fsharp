// Expected: Warning for module inside nested type
module Level1

module Level2 =
    module Level3 =
        type MyType =
            | A
            module InvalidModule = 
                let x = 1
