// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Build

open System
open System.IO
open System.Runtime.InteropServices
open System.Text
open Microsoft.Build.Framework
open Internal.Utilities

module internal TaskEnvironmentPaths =

    let pathComparison =
        if RuntimeInformation.IsOSPlatform OSPlatform.Windows then
            StringComparison.OrdinalIgnoreCase
        else
            StringComparison.Ordinal

    // ProcessStartInfo resolves relative tool paths against the host process current directory.
    let normalizePathToTool (taskEnvironment: TaskEnvironment) (pathToTool: string) =
        if String.IsNullOrEmpty(Path.GetDirectoryName pathToTool) then
            pathToTool
        else
            taskEnvironment.GetAbsolutePath(pathToTool).Value

    let defaultCompilerToolPath (taskEnvironment: TaskEnvironment) (taskType: Type) =
        let probePoint =
            try
                Some(Path.GetDirectoryName(taskType.Assembly.Location))
            with _ ->
                None

        FSharpEnvironment.BinFolderOfDefaultFSharpCompilerUsingEnvironment taskEnvironment.GetEnvironmentVariable probePoint
        |> Option.defaultValue ""

    // netstandard2.0 has no String.Replace(string, string, StringComparison) overload.
    let private replaceOrdinal (source: string) (oldValue: string) (newValue: string) =
        if String.IsNullOrEmpty oldValue then
            source
        else
            let builder = StringBuilder()
            let mutable searchStart = 0
            let mutable matchIndex = source.IndexOf(oldValue, searchStart, pathComparison)

            while matchIndex >= 0 do
                builder.Append(source, searchStart, matchIndex - searchStart).Append(newValue)
                |> ignore

                searchStart <- matchIndex + oldValue.Length
                matchIndex <- source.IndexOf(oldValue, searchStart, pathComparison)

            builder.Append(source, searchStart, source.Length - searchStart).ToString()

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
        ||> List.fold (fun message (rooted, original) -> replaceOrdinal message rooted original)

type internal TaskEnvironmentState() =
    let mutable value = TaskEnvironment.Fallback

    member _.Value
        with get () = value
        and set environment = value <- environment

    member _.RootedPath(path: string) = value.GetAbsolutePath(path).Value

    member _.RestoreOriginalPaths (message: string) (originalPaths: string list) =
        TaskEnvironmentPaths.restoreOriginalPaths value message originalPaths
