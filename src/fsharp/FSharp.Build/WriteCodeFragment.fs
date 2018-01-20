// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Build

open System
open System.Collections
open System.Collections.Generic
open System.Globalization
open System.IO
open System.Linq
open System.Text
open Microsoft.Build.Framework
open Microsoft.Build.Utilities

type WriteCodeFragment() =
    let mutable _buildEngine : IBuildEngine = null
    let mutable _hostObject : ITaskHost = null
    let mutable _language : string = ""
    let mutable _assemblyAttributes : ITaskItem[] = [||]
    let mutable _outputDirectory : ITaskItem = null
    let mutable _outputFile : ITaskItem = null

    static let escapeString (str:string) =
        let sb = str.ToCharArray() |> Seq.fold (fun (sb:StringBuilder) (c:char) ->
            match c with
            | '\n' | '\u2028' | '\u2028' -> sb.Append("\\n")
            | '\r' -> sb.Append("\\r")
            | '\t' -> sb.Append("\\t")
            | '\'' -> sb.Append("\\'")
            | '\\' -> sb.Append("\\\\")
            | '"' -> sb.Append("\\\"")
            | '\u0000' -> sb.Append("\\0")
            | _ -> sb.Append(c)) (StringBuilder().Append("\""))
        sb.Append("\"").ToString()

    static member GenerateAttribute (item:ITaskItem) =
        let attributeName = item.ItemSpec
        let args =
            // mimicking the behavior from https://github.com/Microsoft/msbuild/blob/70ce7e9ccb891b63f0859f1f7f0b955693ed3742/src/Tasks/WriteCodeFragment.cs#L355-L415
            // Split parameters into unnamed positional (e.g., key is "_Parameter1", etc.) and proper named parameters.
            let customMetadata = item.CloneCustomMetadata()
            let parameterPairs =
                // normalize everything to strings
                seq { for entry in customMetadata -> entry :?> DictionaryEntry }
                |> Seq.toList
                |> List.map (fun entry ->
                    let key = entry.Key :?> string
                    let value = match entry.Value with
                                | null -> "null"
                                | :? string as value -> escapeString value
                                | value -> value.ToString()
                    (key, value))
            let orderedParameters, namedParameters = parameterPairs |> List.partition (fun (key, _) -> key.StartsWith("_Parameter"))
            let orderedParametersWithIndex =
                orderedParameters
                |> List.map (fun (key, value) ->
                    let indexString = key.Substring("_Parameter".Length)
                    match Int32.TryParse indexString with
                    | (true, index) -> (index, value)
                    | (false, _) -> failwith (sprintf "Unable to parse '%s' as an index" indexString))
                |> List.sortBy fst
            // assign ordered parameters to array
            let orderedParametersArray =
                if List.isEmpty orderedParametersWithIndex then [||]
                else Array.create (List.last orderedParametersWithIndex |> fst) "null"
            List.iter (fun (index, value) -> orderedParametersArray.[index - 1] <- value) orderedParametersWithIndex
            // construct ordered parameter lists
            let combinedOrderedParameters = String.Join(", ", orderedParametersArray)
            let combinedNamedParameters = String.Join(", ", List.map (fun (key, value) -> sprintf "%s = %s" key value) namedParameters)
            // construct the final argument string; positional arguments followed by named
            match (combinedOrderedParameters.Length, combinedNamedParameters.Length) with
            | (0, 0) -> "" // no arguments
            | (0, _) -> combinedNamedParameters // only named arguments
            | (_, 0) -> combinedOrderedParameters // only positional arguments
            | (_, _) -> combinedOrderedParameters + ", " + combinedNamedParameters // both positional and named arguments
        sprintf "[<assembly: %s(%s)>]" attributeName args

    // adding this property to maintain API equivalence with the MSBuild task
    member this.Language
        with get() = _language
         and set(value) = _language <- value

    member this.AssemblyAttributes
        with get() = _assemblyAttributes
         and set(value) = _assemblyAttributes <- value

    member this.OutputDirectory
        with get() = _outputDirectory
         and set(value) = _outputDirectory <- value

    [<Output>]
    member this.OutputFile
        with get() = _outputFile
         and set(value) = _outputFile <- value

    interface ITask with
        member this.BuildEngine
            with get() = _buildEngine
             and set(value) = _buildEngine <- value
        member this.HostObject
            with get() = _hostObject
             and set(value) = _hostObject <- value
        member this.Execute() =
            try
                if isNull _outputFile && isNull _outputDirectory then failwith "Output location must be specified"
                if _language.ToLowerInvariant() <> "f#" then failwith "Language name must be F#"
                let boilerplate = @"// <auto-generated>

namespace FSharp

open System
open System.Reflection"
                let sb = StringBuilder().AppendLine(boilerplate).AppendLine()
                let code = Array.fold (fun (sb:StringBuilder) (item:ITaskItem) -> sb.AppendLine(WriteCodeFragment.GenerateAttribute item)) sb _assemblyAttributes
                code.AppendLine().AppendLine("do()") |> ignore
                let fileName = _outputFile.ItemSpec
                let outputFileItem =
                    if not (isNull _outputFile) && not (isNull _outputDirectory) && not (Path.IsPathRooted(fileName)) then
                        TaskItem(Path.Combine(_outputDirectory.ItemSpec, fileName)) :> ITaskItem
                    elif isNull _outputFile then
                        let tempFile = Path.Combine(Path.GetTempPath(), sprintf "tmp%s.fs" (Guid.NewGuid().ToString("N")))
                        TaskItem(tempFile) :> ITaskItem
                    else
                        _outputFile
                let codeText = code.ToString()
                let alreadyExists = (try File.Exists fileName && File.ReadAllText(fileName) = codeText with _ -> false)
                if not alreadyExists then
                    File.WriteAllText(fileName, codeText)
                _outputFile <- outputFileItem
                true
            with e ->
                printf "Error writing code fragment: %s" (e.ToString())
                false
