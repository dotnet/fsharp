open System
let ae = new AggregateException()

ae.Handle(fun e ->
    match e with
    | :? OperationCanceledException -> true
    | _ -> false
    )

ae.Handle(function
    | :? OperationCanceledException -> true
    | _ -> false
    )

ae.Handle(
    Func<exn,bool>(
        function
        | :? OperationCanceledException -> true
        | _ -> false
    ))

module M1 =
    type T =
        static member M (_ : Func<int, exn, int>) = ()

    T.M (function _ -> function :? ArgumentException -> 3 | _ -> 4)
    T.M (function 0 -> (function _ -> 3) | _ -> function :? ArgumentException -> 3 | _ -> 4)

module M2 =
    type T =
        static member M (_ : Func<int, int, int>) = ()

    T.M (function 0 -> (function _ -> 1) | _ -> (function 0 -> 3 | _ -> 4))
    T.M (function 0 -> id | _ -> (function 0 -> 3 | _ -> 4))
    T.M (function 0 -> (function 0 -> 3 | _ -> 4) | _ -> id)

module M3 =
    type T =
        static member M (_ : Func<int, int, int, int>) = ()

    T.M (function 0 -> (function 0 -> (function 0 -> 1 | _ -> 0) | _ -> (function 0 -> 2 | _ -> 3)) | _ -> (function 0 -> (function _ -> 3) | _ -> (function 3 -> 4 | _ -> 5)))
    T.M (function 0 -> (function 0 -> id | _ -> (function 0 -> 2 | _ -> 3)) | _ -> (function 0 -> (function _ -> 3) | _ -> (function 3 -> 4 | _ -> 5)))
