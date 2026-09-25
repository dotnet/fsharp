// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Build

open System
open System.IO
open System.Text
open Microsoft.Build.Framework
open Internal.Utilities
open Internal.Utilities.Library

module internal TaskEnvironmentPaths =

    let pathComparison =
        if FSharpEnvironment.isWindows then
            StringComparison.OrdinalIgnoreCase
        else
            StringComparison.Ordinal

    // ProcessStartInfo resolves relative tool paths against the host process current directory.
    let normalizePathToTool (taskEnvironment: TaskEnvironment) (pathToTool: string) =
        if String.IsNullOrWhiteSpace pathToTool then
            pathToTool
        else
            match Path.GetDirectoryName pathToTool with
            | NonEmptyString _ -> taskEnvironment.GetAbsolutePath(pathToTool).Value
            | _ -> pathToTool

    let defaultCompilerToolPath (taskEnvironment: TaskEnvironment) (taskType: Type) =
        let probePoint =
            try
                Some(Path.GetDirectoryName(taskType.Assembly.Location))
            with _ ->
                None

        FSharpEnvironment.BinFolderOfDefaultFSharpCompilerUsingEnvironment taskEnvironment.GetEnvironmentVariable probePoint
        |> Option.defaultValue ""

    // Quoted filenames can contain whitespace and punctuation that delimit unquoted diagnostics.
    let private isPathToken (source: string) start finish =
        let preceding =
            if start = 0 then
                ' '
            else
                source[start - 1]

        let startsToken =
            Char.IsWhiteSpace preceding
            || (match preceding with
                | '\''
                | '"'
                | '('
                | '['
                | '{'
                | '='
                | '>' -> true
                | _ -> false)

        startsToken
        && (finish = source.Length
            || (match source[finish] with
                | '/'
                | '\\' -> true
                | c when preceding = '\'' || preceding = '"' -> c = preceding
                | c ->
                    Char.IsWhiteSpace c
                    || (match c with
                        | '\''
                        | '"'
                        | ':'
                        | ';'
                        | ','
                        | ')'
                        | ']'
                        | '}'
                        | '>' -> true
                        | _ -> false)))

    // netstandard2.0 has no String.Replace(string, string, StringComparison) overload, and a plain replace
    // would also ignore the path-token boundary above, so both are handled here.
    let private replacePathToken (source: string) (oldValue: string) (newValue: string) =
        if String.IsNullOrEmpty oldValue then
            source
        else
            let builder = StringBuilder()

            let rec loop searchStart =
                match source.IndexOf(oldValue, searchStart, pathComparison) with
                | -1 -> builder.Append(source, searchStart, source.Length - searchStart)
                | matchIndex ->
                    let afterMatch = matchIndex + oldValue.Length
                    builder.Append(source, searchStart, matchIndex - searchStart) |> ignore

                    if isPathToken source matchIndex afterMatch then
                        builder.Append(newValue) |> ignore
                    else
                        builder.Append(source, matchIndex, oldValue.Length) |> ignore

                    loop afterMatch

            (loop 0).ToString()

    let restoreOriginalPaths (taskEnvironment: TaskEnvironment) (message: string) (originalPaths: string list) =
        let rootedFormsOf (original: string) =
            try
                if String.IsNullOrEmpty original then
                    []
                else
                    let rooted = taskEnvironment.GetAbsolutePath(original).Value

                    if String.IsNullOrEmpty rooted || String.Equals(rooted, original, pathComparison) then
                        []
                    else
                        let canonical =
                            try
                                Path.GetFullPath rooted
                            with _ ->
                                rooted

                        [
                            (rooted, original)
                            if not (String.IsNullOrEmpty canonical) then
                                (canonical, original)
                        ]
            with _ ->
                []

        let replacements =
            originalPaths
            |> List.collect rootedFormsOf
            |> List.distinct
            |> List.sortByDescending (fun (rooted, _) -> rooted.Length)

        (message, replacements)
        ||> List.fold (fun message (rooted, original) -> replacePathToken message rooted original)

    // Task-facing helpers that read the injected TaskEnvironment late (partially apply against `this`).
    let rootedPath (task: #IMultiThreadableTask) path =
        task.TaskEnvironment.GetAbsolutePath(path).Value

    let restoreTaskPaths (task: #IMultiThreadableTask) message paths =
        restoreOriginalPaths task.TaskEnvironment message paths
