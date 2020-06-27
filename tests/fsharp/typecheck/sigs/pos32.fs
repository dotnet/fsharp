module Pos32

    let inline test (arg: ^T when ^T : struct) = 
        ()

    let f () =
        let a = test struct (1, 2)
        ()
    
