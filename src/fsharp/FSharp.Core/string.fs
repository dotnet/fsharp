// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Core

    open System.Diagnostics
    open Microsoft.FSharp.Core
    open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
    open Microsoft.FSharp.Core.Operators
    open Microsoft.FSharp.Core.Operators.Checked
    open Microsoft.FSharp.Collections

    [<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
    [<RequireQualifiedAccess>]
    module String =

        let inline emptyIfNull str = 
            if str = null then "" else str

        [<CompiledName("Concat")>]
        let concat sep (strings : seq<string>) =  
            System.String.Join(sep, Seq.toArray strings)

        [<CompiledName("Iterate")>]
        let iter (f : (char -> unit)) (str:string) =
            let str = emptyIfNull str
            for i = 0 to str.Length - 1 do
                f str.[i] 

        [<CompiledName("IterateIndexed")>]
        let iteri f (str:string) =
            let str = emptyIfNull str
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)
            for i = 0 to str.Length - 1 do
                f.Invoke(i, str.[i]) 

        [<CompiledName("Map")>]
        let map (f: char -> char) (str:string) =
            let str = emptyIfNull str
            let res = new System.Text.StringBuilder(str.Length)
            str |> iter (fun c -> res.Append(f c) |> ignore);
            res.ToString()

        [<CompiledName("MapIndexed")>]
        let mapi (f: int -> char -> char) (str:string) =
            let str = emptyIfNull str
            let res = new System.Text.StringBuilder(str.Length)
            let f = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)
            str |> iteri (fun i c -> res.Append(f.Invoke(i, c)) |> ignore);
            res.ToString()

        [<CompiledName("Filter")>]
        let filter (f: char -> bool) (str:string) =
            let str = emptyIfNull str
            let res = new System.Text.StringBuilder(str.Length)
            str |> iter (fun c -> if f c then res.Append(c) |> ignore)
            res.ToString()

        [<CompiledName("Collect")>]
        let collect (f: char -> string) (str:string) =
            let str = emptyIfNull str
            let res = new System.Text.StringBuilder(str.Length)
            str |> iter (fun c -> res.Append(f c) |> ignore);
            res.ToString()

        [<CompiledName("Initialize")>]
        let init (count:int) (initializer: int-> string) =
            if count < 0 then  invalidArg "count" (SR.GetString(SR.inputMustBeNonNegative))
            let res = new System.Text.StringBuilder(count)
            for i = 0 to count - 1 do 
               res.Append(initializer i) |> ignore;
            res.ToString()

        [<CompiledName("Replicate")>]
        let replicate (count:int) (str:string) =
            if count < 0 then  invalidArg "count" (SR.GetString(SR.inputMustBeNonNegative)) 
            let str = emptyIfNull str
            let res = new System.Text.StringBuilder(str.Length)
            for i = 0 to count - 1 do 
               res.Append(str) |> ignore;
            res.ToString()

        [<CompiledName("ForAll")>]
        let forall f (str:string) =
            let str = emptyIfNull str
            let rec check i = (i >= str.Length) || (f str.[i] && check (i+1)) 
            check 0

        [<CompiledName("Exists")>]
        let exists f (str:string) =
            let str = emptyIfNull str
            let rec check i = (i < str.Length) && (f str.[i] || check (i+1)) 
            check 0  

        [<CompiledName("Length")>]
        let length (str:string) =
            let str = emptyIfNull str
            str.Length


