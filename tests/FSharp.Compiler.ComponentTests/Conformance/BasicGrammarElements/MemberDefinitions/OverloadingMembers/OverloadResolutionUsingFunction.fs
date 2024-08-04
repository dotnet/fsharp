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