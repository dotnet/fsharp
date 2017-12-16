// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Compiler.SourceCodeServices

module PathUtils = 
    open System.IO
    [<Sealed>]
    type Path =
        static member GetFullPathSafe path =
            try Path.GetFullPath path
            with _ -> path

        static member GetFileNameSafe path =
            try Path.GetFileName path
            with _ -> path

    let (</>) a b = Path.Combine(a, b)

module HashDirectiveInfo =
    open System.IO
    open PathUtils
    open Microsoft.FSharp.Compiler.Range
    open Microsoft.FSharp.Compiler.Ast

    type IncludeDirective =
        | ResolvedDirectory of string

    type LoadDirective =
        | ExistingFile of string
        | UnresolvableFile of string * previousIncludes : string array

    [<NoComparison>]
    type Directive =
        | Include of IncludeDirective * range
        | Load of LoadDirective * range

    /// returns an array of LoadScriptResolutionEntries
    /// based on #I and #load directives
    let getIncludeAndLoadDirectives ast =
        // the Load items are resolved using fallback resolution relying on previously parsed #I directives
        // (this behaviour is undocumented in F# but it seems to be how it works).

        // list of #I directives so far (populated while encountering those in order)
        // TODO: replace by List.fold if possible
        let includesSoFar = new System.Collections.Generic.List<_>()
        let pushInclude = includesSoFar.Add

        // those might need to be abstracted away from real filesystem operations
        let fileExists = File.Exists
        let directoryExists = Directory.Exists
        let isPathRooted = Path.IsPathRooted
        let getDirectoryOfFile = Path.GetFullPathSafe >> Path.GetDirectoryName
        let getRootedDirectory = Path.GetFullPathSafe
        let makeRootedDirectoryIfNecessary baseDirectory directory =
            if not (isPathRooted directory) then
                getRootedDirectory (baseDirectory </> directory)
            else
                directory

        // separate function to reduce nesting one level
        let parseDirectives modules file =
            [|
            let baseDirectory = getDirectoryOfFile file
            for (SynModuleOrNamespace (_, _, _, declarations, _, _, _, _)) in modules do
                for decl in declarations do
                    match decl with
                    | SynModuleDecl.HashDirective (ParsedHashDirective("I",[directory],range),_) ->
                        let directory = makeRootedDirectoryIfNecessary (getDirectoryOfFile file) directory

                        if directoryExists directory then
                            let includeDirective = ResolvedDirectory(directory)
                            pushInclude includeDirective
                            yield Include (includeDirective, range)

                    | SynModuleDecl.HashDirective (ParsedHashDirective ("load",files,range),_) ->
                        for f in files do
                            if isPathRooted f && fileExists f then
                                // this is absolute reference to an existing script, easiest case
                                yield Load (ExistingFile f, range)
                            else
                                // I'm not sure if the order is correct, first checking relative to file containing the #load directive
                                // then checking for undocumented resolution using previously parsed #I directives
                                let fileRelativeToCurrentFile = baseDirectory </> f
                                if fileExists fileRelativeToCurrentFile then
                                    // this is existing file relative to current file
                                    yield Load (ExistingFile fileRelativeToCurrentFile, range)

                                else
                                    // match file against first include which seemingly have it found
                                    let maybeFile =
                                        includesSoFar
                                        |> Seq.tryPick (function
                                            | (ResolvedDirectory d) ->
                                                let filePath = d </> f
                                                if fileExists filePath then Some filePath else None
                                        )
                                    match maybeFile with
                                    | None -> () // can't load this file even using any of the #I directives...
                                    | Some f ->
                                        yield Load (ExistingFile f,range)
                    | _ -> ()
            |]

        match ast with
        | ParsedInput.ImplFile (ParsedImplFileInput(fn,_,_,_,_,modules,_)) -> parseDirectives modules fn
        | _ -> [||]

    /// returns the Some (complete file name of a resolved #load directive at position) or None
    let getHashLoadDirectiveResolvedPathAtPosition (pos: pos) (ast: ParsedInput) : string option =
        getIncludeAndLoadDirectives ast
        |> Array.tryPick (
            function
            | Load (ExistingFile f,range)
                // check the line is within the range
                // (doesn't work when there are multiple files given to a single #load directive)
                when rangeContainsPos range pos
                    -> Some f
            | _     -> None
        )
