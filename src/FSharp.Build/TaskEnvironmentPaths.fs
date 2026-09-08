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
        // Guard empty/whitespace before Path.GetDirectoryName: on net472 it throws on whitespace-only
        // input, and a bare/whitespace tool name must stay unchanged so the host resolves it via PATH.
        match pathToTool with
        | NonEmptyString path when not (String.IsNullOrWhiteSpace path) ->
            match Path.GetDirectoryName path with
            | NonEmptyString _ -> taskEnvironment.GetAbsolutePath(path).Value
            | _ -> pathToTool
        | _ -> pathToTool

    let defaultCompilerToolPath (taskEnvironment: TaskEnvironment) (taskType: Type) =
        let probePoint =
            try
                Some(Path.GetDirectoryName(taskType.Assembly.Location))
            with _ ->
                None

        FSharpEnvironment.BinFolderOfDefaultFSharpCompilerUsingEnvironment taskEnvironment.GetEnvironmentVariable probePoint
        |> Option.defaultValue ""

    // A rooted path only stands for the original when the match ends a path token: at end-of-string, at a
    // quote, or before a separator (a directory child). Otherwise the match is a mere lexical prefix of an
    // unrelated path (e.g. rooted "p" inside rooted "parts/x") and rewriting it would corrupt that path.
    let private endsPathToken (source: string) index =
        index >= source.Length
        || (match source[index] with
            | '\''
            | '"' -> true
            | c -> c = '/' || c = '\\')

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

                    if endsPathToken source afterMatch then
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
