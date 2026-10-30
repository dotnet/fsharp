// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Discovers the non-source on-disk inputs a compilation reads (embedded resources, win32
/// resources, key files, loaded scripts) from its command-line options. The hot reload session
/// snapshots these when a project baseline is captured and compares them before emitting a
/// delta, so a tracked input edit that was not followed by a rebuild is detected as stale
/// build output instead of silently producing a delta that misses the change.
module internal FSharp.Compiler.HotReload.TrackedInputs

open System
open System.Collections.Generic
open System.IO
open Internal.Utilities.Library

open FSharp.Compiler.IO

/// One non-source on-disk input of a compilation, with the timestamp observed when the
/// session captured or compared it.
type TrackedInput =
    { Path: string; LastModified: DateTime }

let private trimEnclosingQuotes (value: string) =
    if String.IsNullOrWhiteSpace value then
        value
    else
        value.Trim().Trim('"')

let private tryNormalizeTrackedInputPath (projectDirectory: string) (path: string) =
    if String.IsNullOrWhiteSpace path then
        ValueNone
    else
        let candidatePath =
            let path = trimEnclosingQuotes path

            if Path.IsPathRooted path then path
            elif String.IsNullOrWhiteSpace projectDirectory then path
            else Path.Combine(projectDirectory, path)

        let fullPath =
            try
                Path.GetFullPath candidatePath
            with RecoverableException _ ->
                candidatePath

        if FileSystem.FileExistsShim fullPath then
            ValueSome fullPath
        else
            ValueNone

let private tryGetTrackedInputPath (projectDirectory: string) (option: string) =
    let startsWith (prefix: string) =
        option.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)

    let rec valueFromPrefix prefixes =
        match prefixes with
        | [] -> ValueNone
        | (prefix: string) :: rest ->
            if startsWith prefix && option.Length > prefix.Length then
                ValueSome(option.Substring(prefix.Length))
            else
                valueFromPrefix rest

    let normalizeResourceOptionValue (value: string) =
        let normalized = trimEnclosingQuotes value
        let logicalNameSeparator = normalized.IndexOf(',')

        if logicalNameSeparator > 0 then
            normalized.Substring(0, logicalNameSeparator)
        else
            normalized

    let tryPathFromPrefixedOption prefixes valueNormalizer =
        valueFromPrefix prefixes
        |> ValueOption.map valueNormalizer
        |> ValueOption.bind (tryNormalizeTrackedInputPath projectDirectory)

    if String.IsNullOrWhiteSpace option then
        ValueNone
    elif
        startsWith "-r:"
        || startsWith "--reference:"
        || startsWith "--out:"
        || startsWith "-o:"
    then
        ValueNone
    elif option.StartsWith("-", StringComparison.Ordinal) then
        tryPathFromPrefixedOption [ "--resource:"; "-resource:"; "--res:"; "-res:" ] normalizeResourceOptionValue
        |> ValueOption.orElseWith (fun () ->
            tryPathFromPrefixedOption [ "--win32res:"; "--keyfile:"; "--load:"; "--use:" ] trimEnclosingQuotes)
    else
        tryNormalizeTrackedInputPath projectDirectory option

let private isSplitOutputOption (option: string) =
    String.Equals(option, "-o", StringComparison.OrdinalIgnoreCase)
    || String.Equals(option, "--out", StringComparison.OrdinalIgnoreCase)

/// Parses the command line options of a project for existing on-disk non-source inputs and
/// records their current last-write timestamps. Relative paths resolve against the project
/// file's directory. References and output paths are never tracked; the path following a
/// split `-o`/`--out` switch is an output, not an input.
let compute (projectFileName: string) (otherOptions: string seq) : TrackedInput list =
    let projectDirectory =
        projectFileName
        |> Path.GetDirectoryName
        |> ValueOption.ofObj
        |> ValueOption.defaultValue ""

    let seen = HashSet<string>(StringComparer.Ordinal)
    let mutable skipNextOutput = false

    [
        for option in otherOptions do
            if skipNextOutput then
                skipNextOutput <- false
            elif isSplitOutputOption option then
                skipNextOutput <- true
            else
                match tryGetTrackedInputPath projectDirectory option with
                | ValueSome path when seen.Add path ->
                    yield
                        {
                            Path = path
                            LastModified = FileSystem.GetLastWriteTimeShim path
                        }
                | ValueSome _
                | ValueNone -> ()
    ]
