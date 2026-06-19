// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Discovers the non-source on-disk inputs a compilation reads (embedded resources, win32
/// resources, key files, loaded scripts) from its command-line options. The hot reload session
/// snapshots these when a project baseline is captured and compares them before emitting a
/// delta, so a tracked input edit that was not followed by a rebuild is detected as stale
/// build output instead of silently producing a delta that misses the change.
module internal FSharp.Compiler.HotReload.TrackedInputs

open System
open System.IO
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
        None
    else
        let candidatePath =
            let path = trimEnclosingQuotes path

            if Path.IsPathRooted path then path
            elif String.IsNullOrWhiteSpace projectDirectory then path
            else Path.Combine(projectDirectory, path)

        let fullPath =
            try
                Path.GetFullPath candidatePath
            with _ ->
                candidatePath

        if FileSystem.FileExistsShim fullPath then
            Some fullPath
        else
            None

let private tryGetTrackedInputPath (projectDirectory: string) (option: string) =
    let startsWith (prefix: string) =
        option.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)

    let valueFromPrefix prefixes =
        prefixes
        |> Seq.tryPick (fun (prefix: string) ->
            if startsWith prefix && option.Length > prefix.Length then
                Some(option.Substring(prefix.Length))
            else
                None)

    let normalizeResourceOptionValue (value: string) =
        let normalized = trimEnclosingQuotes value
        let logicalNameSeparator = normalized.IndexOf(',')

        if logicalNameSeparator > 0 then
            normalized.Substring(0, logicalNameSeparator)
        else
            normalized

    let tryPathFromPrefixedOption prefixes valueNormalizer =
        valueFromPrefix prefixes
        |> Option.map valueNormalizer
        |> Option.bind (tryNormalizeTrackedInputPath projectDirectory)

    if String.IsNullOrWhiteSpace option then
        None
    elif
        startsWith "-r:"
        || startsWith "--reference:"
        || startsWith "--out:"
        || startsWith "-o:"
    then
        None
    elif option.StartsWith("-", StringComparison.Ordinal) then
        tryPathFromPrefixedOption [ "--resource:"; "-resource:"; "--res:"; "-res:" ] normalizeResourceOptionValue
        |> Option.orElseWith (fun () -> tryPathFromPrefixedOption [ "--win32res:"; "--keyfile:"; "--load:"; "--use:" ] trimEnclosingQuotes)
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
        |> Option.ofObj
        |> Option.defaultValue ""

    let rec collectTrackedInputs skipNextOutput tracked options =
        match options with
        | [] -> tracked
        | _ :: tail when skipNextOutput -> collectTrackedInputs false tracked tail
        | option :: tail when isSplitOutputOption option ->
            // Split output flags are followed by an output path that should not be tracked as an input dependency.
            collectTrackedInputs true tracked tail
        | option :: tail ->
            let updatedTracked =
                match tryGetTrackedInputPath projectDirectory option with
                | Some path -> path :: tracked
                | None -> tracked

            collectTrackedInputs false updatedTracked tail

    otherOptions
    |> Seq.toList
    |> collectTrackedInputs false []
    |> List.rev
    |> Seq.distinct
    |> Seq.map (fun path ->
        {
            Path = path
            LastModified = FileSystem.GetLastWriteTimeShim path
        })
    |> Seq.toList
