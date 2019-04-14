
module M

    let inline test (arg: ^T when ^T : struct) = 
        (^T : (member Item1: _) (arg))

    let f () =
        let a = test struct (1, 2)
        ()