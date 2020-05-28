// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Core

    open System
    open System.Text
    open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
    open Microsoft.FSharp.Core.Operators
    open Microsoft.FSharp.Core.Operators.Checked
    open Microsoft.FSharp.Collections

    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    [<RequireQualifiedAccess>]
    module String =
        [<CompiledName("Concat")>]
        let concat sep (strings : seq<string>) =  
            String.Join(sep, strings)

        [<CompiledName("Iterate")>]
        let iter (action : (char -> unit)) (str:string) =
            if not (String.IsNullOrEmpty str) then
                for i = 0 to str.Length - 1 do
                    action str.[i] 

        [<CompiledName("IterateIndexed")>]
        let iteri action (str:string) =
            if not (String.IsNullOrEmpty str) then
                let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(action)
                for i = 0 to str.Length - 1 do
                    f.Invoke(i, str.[i]) 

        [<CompiledName("Map")>]
        let map (mapping: char -> char) (str:string) =
            if String.IsNullOrEmpty str then
                String.Empty
            else
                let res = StringBuilder str.Length
                str |> iter (fun c -> res.Append(mapping c) |> ignore)
                res.ToString()

        [<CompiledName("MapIndexed")>]
        let mapi (mapping: int -> char -> char) (str:string) =
            if String.IsNullOrEmpty str then
                String.Empty
            else
                let res = StringBuilder str.Length
                let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(mapping)
                str |> iteri (fun i c -> res.Append(f.Invoke(i, c)) |> ignore)
                res.ToString()

        [<CompiledName("Filter")>]
        let filter (predicate: char -> bool) (str:string) =
            if String.IsNullOrEmpty str then
                String.Empty
            else
                let res = StringBuilder str.Length
                str |> iter (fun c -> if predicate c then res.Append c |> ignore)
                res.ToString()

        [<CompiledName("Collect")>]
        let collect (mapping: char -> string) (str:string) =
            if String.IsNullOrEmpty str then
                String.Empty
            else
                let res = StringBuilder str.Length
                str |> iter (fun c -> res.Append(mapping c) |> ignore)
                res.ToString()

        [<CompiledName("Initialize")>]
        let init (count:int) (initializer: int-> string) =
            if count < 0 then invalidArgInputMustBeNonNegative "count" count
            let res = StringBuilder count
            for i = 0 to count - 1 do 
               res.Append(initializer i) |> ignore
            res.ToString()

        [<CompiledName("Replicate")>]
        let replicate (count:int) (str:string) =
            if count < 0 then invalidArgInputMustBeNonNegative "count" count

            if String.IsNullOrEmpty str then
                String.Empty
            else
                let res = StringBuilder(count * str.Length)
                for i = 0 to count - 1 do 
                   res.Append str |> ignore
                res.ToString()

        [<CompiledName("ForAll")>]
        let forall predicate (str:string) =
            if String.IsNullOrEmpty str then
                true
            else
                let rec check i = (i >= str.Length) || (predicate str.[i] && check (i+1)) 
                check 0

        [<CompiledName("Exists")>]
        let exists predicate (str:string) =
            if String.IsNullOrEmpty str then
                false
            else
                let rec check i = (i < str.Length) && (predicate str.[i] || check (i+1)) 
                check 0  

        [<CompiledName("Length")>]
        let length (str:string) =
            if String.IsNullOrEmpty str then
                0
            else
                str.Length
