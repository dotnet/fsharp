namespace Test

open System.Runtime.CompilerServices

type MyTy() =
    static member GetCallerLineNumber([<CallerLineNumber>] ?line : int) =
        line

module Program =
    [<EntryPoint>]
    let main (_:string[]) =
        match MyTy.GetCallerLineNumber(), MyTy.GetCallerLineNumber(42) with
        | Some(12), Some(42) -> 0
        | _ -> 1
