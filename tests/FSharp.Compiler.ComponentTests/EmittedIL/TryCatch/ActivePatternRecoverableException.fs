module ActivePatternTestCase

open System

[<return: Struct>]
let (|RecoverableException|_|) (exn: Exception) =
    match exn with
    | :? OperationCanceledException -> ValueNone
    | _ ->
        ValueSome exn

let addWithActivePattern (a:int) (b:int) =
    try
        a / b
    with
    | RecoverableException e -> a + b