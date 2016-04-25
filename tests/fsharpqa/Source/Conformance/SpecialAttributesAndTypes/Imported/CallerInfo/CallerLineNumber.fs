namespace Test

open System.Runtime.CompilerServices
open CSharpLib
[<MyCallerInfo>]
type MyTy() =
    static member GetCallerLineNumber([<CallerLineNumber>] ?line : int) =
        line

module Program =
    [<EntryPoint>]
    let main (_:string[]) =
        if MyTy.GetCallerLineNumber() <> Some(13) then
            failwith "Unexpected F# CallerLineNumber"
        
        if MyTy.GetCallerLineNumber(42) <> Some(42) then
            failwith "Unexpected F# CallerLineNumber"
            
        if CallerInfoTest.LineNumber() <> 19 then
            failwith "Unexpected C# CallerLineNumber"
            
        if CallerInfoTest.LineNumber(88) <> 88 then
            failwith "Unexpected C# CallerLineNumber"
            
        match CallerInfoTest.AllInfo(21) with
        | (_, 25) -> ()
        | x -> failwithf "Unexpected C# result with multiple parameter types: %A" x
        
        if (typeof<MyTy>.GetCustomAttributes(typeof<MyCallerInfoAttribute>, false).[0] :?> MyCallerInfoAttribute).LineNumber <> 5 then
            failwith "Unexpected C# MyCallerInfoAttribute"

# 345 "qwerty"
        match CallerInfoTest.AllInfo(123) with
        | (_, 345) -> ()
        | x -> failwithf "Unexpected C# result with multiple parameter types: %A" x
# 456 "qwerty"
        match CallerInfoTest.AllInfo(123) with
        | (_, 456) -> ()
        | x -> failwithf "Unexpected C# result with multiple parameter types: %A" x

        0