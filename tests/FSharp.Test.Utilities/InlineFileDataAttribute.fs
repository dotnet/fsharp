namespace FSharp.Test

open System
open System.Diagnostics
open System.Linq
open System.IO
open System.Reflection
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

open Xunit
open Xunit.Sdk

open FSharp.Compiler.IO
open FSharp.Test.Compiler
open FSharp.Test.Utilities
open TestFramework

type BooleanOptions =
    | True = 1
    | False = 2
    | Both = 3

/// Attribute to use with Xunit's TheoryAttribute.
/// Takes a file, relative to current test suite's root.
/// Returns a CompilationUnit with encapsulated source code, error baseline and IL baseline (if any).
[<NoComparison; NoEquality>]
type FileInlineDataAttribute (filename: string) =
    inherit DataAttribute()

    let mutable directory: string option = None
    let mutable optimize: BooleanOptions option = None
    let mutable realsig: BooleanOptions option = None

    static let getBaselineForOption booleanOption option prefix =
        let suffix =
            match booleanOption with
            | BooleanOptions.True -> "On"
            | BooleanOptions.False -> "Off"
            | _ -> failwith "Invalid value for booleanOption"

        match option with
        | Some BooleanOptions.Both -> Some $"{prefix}{suffix}"
        | Some v when v = booleanOption -> Some $"{prefix}{suffix}"
        | _ -> None

    static let getBaselinesForOption option prefix =
        let optionOn =   getBaselineForOption BooleanOptions.True  option prefix
        let optionOff =  getBaselineForOption BooleanOptions.False option prefix
        optionOn, optionOff

    static let getBaseline realsigBsl optimizeBsl =
        match realsigBsl, optimizeBsl with
        | Some rs, Some opt -> Some $"{rs}{opt}"
        | Some rs, None -> Some $"{rs}"
        | None, Some opt -> Some $"{opt}"
        | _ -> None

    static let fromBoolOption(value: bool) =
        match value with
        | true -> Some BooleanOptions.True
        | _ -> Some BooleanOptions.False

    member _.Directory with set v = directory <- Some v

    member _.Optimize with set v = optimize <- Some v

    member _.Realsig with set v = realsig <- Some v

    override _.GetData methodInfo =

        let realsigOn, realsigOff = getBaselinesForOption realsig "RealInternalSignature"
        let optimizeOn, optimizeOff = getBaselinesForOption optimize "Optimize"

        let getOptions realsigBsl optimizeBsl realsig optimize =

            let setRealInternalSignature compilation =
                match realsigBsl, realsig with
                | Some _, realsig -> compilation |> withRealInternalSignature realsig
                | _ -> compilation

            let setOptimization compilation =
                match optimizeBsl, optimize with
                | Some _, optimize -> compilation |> withOptimization optimize
                | _ -> compilation

            let directoryAttribute = DirectoryAttribute(directory |> Option.defaultValue "")
            directoryAttribute.Includes <- [| filename |]

            let compilation: CompilationUnit option =
                let results = directoryAttribute.GetData methodInfo
                if results.Count() <> 1 then failwith $"directoryAttribute returned incorrect number of results requires only 1: actual: {results.Count()}"
                let arguments = results.First()
                if arguments.Count() <> 1 then failwith $"directoryAttribute returned incorrect number of arguments requires only 1: actual: {arguments.Count()}"

                match arguments.First() with
                | :? FSharpCompilationSource as fsSource -> Some (FS fsSource)
                | :? CSharpCompilationSource as csSource -> Some (CS csSource)
                | :? ILCompilationSource as ilSource -> Some (IL ilSource)
                | _ -> None

            match compilation with
            | Some compilation ->
                let compilation = compilation |> setRealInternalSignature |> setOptimization
                [| (box compilation) |]
            | None -> [||]

        System.Diagnostics.Debugger.Launch() |> ignore
        let results = [|
            getOptions realsigOn  optimizeOn  true true
            getOptions realsigOn  optimizeOff true false
            getOptions realsigOff optimizeOn  false true
            getOptions realsigOff optimizeOff false false
        |]

        results

(*
    static member GetCompilation(filename, realsig: bool option, optimize: bool option) =

        let methodInfo = StackTrace().GetFrame(1).GetMethod()
        let fileInlineData = methodInfo.GetCustomAttribute(typeof<FileInlineDataAttribute>) :?> FileInlineDataAttribute

        let directoryAttribute = DirectoryAttribute(fileInlineData.Directory |> Option.defaultValue "")
        directoryAttribute.Includes <- [| fileInlineData.Filename |]

        let realsigBsl = 
            let realsigOn, realsigOff = getBaselinesForOption (fromBoolOption realsig) "RealInternalSignature"
            match realsig with
            | Some true -> realsigOn
            | Some false -> realsigOff 
            | None -> None

        let optimizeBsl =
            let optimizeOn, optimizeOff = getBaselinesForOption (fromBoolOption optimize) "Optimize"
            match optimize with
            | Some true -> optimizeOn
            | Some false -> optimizeOff
            | None -> None

        let baseline = getBaseline realsigBsl, optimizeBsl

        match baseline with
        | Some baseline -> directoryAttribute.BaselineSuffix <- baseline
        | _ -> ()
        let compilation =
            let results = directoryAttribute.GetData methodInfo
            let arguments =
                if results.Count() <> 1 then
                    failwith $"Directory returned incorrect number of results requires only 1: actual: {results.Count()}"
                results.First()
            if arguments.Count() <> 1 then
                failwith $"Directory returned incorrect number of arguments requires only 1: actual: {arguments.Count()}"
            let compilation =
                arguments.First()
            let compilation =
                match realsig with
                | Some realsig -> compilation.withRealsig realsig
                | None -> compilation
            let compilation =
                match optimize with
                | Some optimize -> compilation.withOptimization optimize
                | None -> compilation
            compilation

        compilation

        //    [|
        //        match baseline with
        //        | Some baseline ->
        //            directoryAttribute.BaselineSuffix <- baseline
        //            let compilation: obj =
        //                let results = directoryAttribute.GetData methodInfo
        //                let arguments =
        //                    if results.Count() <> 1 then
        //                        failwith $"Directory returned incorrect number of results requires only 1: actual: {results.Count()}"
        //                    results.First()
        //                if arguments.Count() <> 1 then
        //                    failwith $"Directory returned incorrect number of arguments requires only 1: actual: {arguments.Count()}"
        //                arguments.First()
        //            ignore compilation
        //            yield box filename
        //            match realsigBsl, realsig with
        //            | Some _, realsig -> yield box realsig
        //            | _ -> ()
        //            match optimizeBsl, optimize with
        //            | Some _, optimize -> yield box optimize
        //            | _ -> ()
        //        | _ -> ()
        //    |]

        //let realsigOn =   getBaseline BooleanOptions.True  realsig "RealInternalSignature"
        //let realsigOff =  getBaseline BooleanOptions.False realsig "RealInternalSignature"
        //let optimizeOn =  getBaseline BooleanOptions.True  optimize "Optimize"
        //let optimizeOff = getBaseline BooleanOptions.False optimize "Optimize"

        //let results = [|
        //    let opts = getOptions realsigOn  optimizeOn  true true in yield opts
        //    let opts = getOptions realsigOn  optimizeOff true false in yield opts
        //    let opts = getOptions realsigOff optimizeOn  false true in yield opts
        //    let opts = getOptions realsigOff optimizeOff false false in yield opts
        //|]
        //System.Diagnostics.Debugger.Launch() |> ignore
        //results














        //let getBaseline booleanOption option baseline = 
        //    let suffix =
        //        match booleanOption with
        //        | BooleanOptions.True -> "On"
        //        | BooleanOptions.False -> "Off"
        //        | _ -> failwith "Invalid value for booleanOption"

        //    match option with
        //    | Some BooleanOptions.Both -> Some $"{baseline}{suffix}"
        //    | Some v when v = booleanOption -> Some $"{baseline}{suffix}"
        //    | _ -> None

        //let getOptions realsigBsl optimizeBsl realsig optimize =
        //    let baseline =
        //        match realsigBsl, optimizeBsl with
        //        | Some rs, Some opt -> Some $"{rs}{opt}"
        //        | Some rs, None -> Some $"{rs}"
        //        | None, Some opt -> Some $"{opt}"
        //        | _ -> None

        //    let directoryAttribute = DirectoryAttribute(directory)
        //    directoryAttribute.Includes <- [| filename |]
        //    [|
        //        match baseline with
        //        | Some baseline ->
        //            directoryAttribute.BaselineSuffix <- baseline
        //            let compilation: obj =
        //                let results = directoryAttribute.GetData methodInfo
        //                let arguments =
        //                    if results.Count() <> 1 then
        //                        failwith $"Directory returned incorrect number of results requires only 1: actual: {results.Count()}"
        //                    results.First()
        //                if arguments.Count() <> 1 then
        //                    failwith $"Directory returned incorrect number of arguments requires only 1: actual: {arguments.Count()}"
        //                arguments.First()
        //            ignore compilation
        //            yield box filename
        //            match realsigBsl, realsig with
        //            | Some _, realsig -> yield box realsig
        //            | _ -> ()
        //            match optimizeBsl, optimize with
        //            | Some _, optimize -> yield box optimize
        //            | _ -> ()
        //        | _ -> ()
        //    |]

        //let realsigOn =   getBaseline BooleanOptions.True  realsig "RealInternalSignature"
        //let realsigOff =  getBaseline BooleanOptions.False realsig "RealInternalSignature"
        //let optimizeOn =  getBaseline BooleanOptions.True  optimize "Optimize"
        //let optimizeOff = getBaseline BooleanOptions.False optimize "Optimize"

        //let results = [|
        //    let opts = getOptions realsigOn  optimizeOn  true true in yield opts
        //    let opts = getOptions realsigOn  optimizeOff true false in yield opts
        //    let opts = getOptions realsigOff optimizeOn  false true in yield opts
        //    let opts = getOptions realsigOff optimizeOff false false in yield opts
        //|]
        //System.Diagnostics.Debugger.Launch() |> ignore
        //results

*)