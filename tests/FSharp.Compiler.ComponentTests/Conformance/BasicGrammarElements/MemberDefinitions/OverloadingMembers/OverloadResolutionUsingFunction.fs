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

let m () =
    fun x ->(function x -> function y -> function z -> <@ x + y + z @>)

let f () = 
    (function
        | None -> fun x -> x + 2
        | Some f -> f)