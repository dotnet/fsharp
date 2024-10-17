namespace Test

open System.Runtime.CompilerServices
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns

type MyTy() =
    static member GetCallerLineNumber([<CallerLineNumber>] ?line : int) =
        line

module Program =
    [<EntryPoint>]
    let main (_:string[]) =
        let expr = <@ MyTy.GetCallerLineNumber () @>
        
        match expr with
        | Call(None, methodInfo, e::[]) 
            when methodInfo.Name = "GetCallerLineNumber" ->
                match e with
                | NewUnionCase(uci, value::[]) 
                    when uci.Name = "Some" ->
                        match value with
                        | Value(obj, ty) when ty = typeof<System.Int32> && obj :?> int = 14 -> ()
                        | _ -> failwith "Unexpected F# CallerLineNumber"
                | _ ->
                    failwith "Unexpected F# CallerLineNumber"
        | _ ->
            failwith "Unexpected F# CallerLineNumber"
            
        0