// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Build

open System
open System.IO
open System.Linq
open System.Runtime.InteropServices
open System.Text
open System.Xml.Linq
open Microsoft.Build.Framework
open Microsoft.Build.Utilities

[<MSBuildMultiThreadableTask>]
type FSharpEmbedResXSource(taskEnvironment: TaskEnvironment) as this =
    inherit Task()
    let mutable _embeddedText: ITaskItem[] = [||]
    let mutable _generatedSource: ITaskItem[] = [||]
    let mutable _outputPath: string = ""
    let mutable _targetFramework: string = ""
    let mutable _taskEnvironment = taskEnvironment

    // Every File/Directory/stream/XDocument API consuming a (possibly relative) path must go through
    // this so paths are resolved against this task instance's TaskEnvironment rather than the ambient
    // process current directory. Original relative strings are preserved for messages and output items.
    let rootedPath (path: string) =
        _taskEnvironment.GetAbsolutePath(path).Value

    // Ordinal path comparison on Unix (case-sensitive file systems) and ordinal-ignore-case on Windows,
    // matching how each platform compares the paths the framework embeds in its exception text.
    let pathComparison =
        if RuntimeInformation.IsOSPlatform OSPlatform.Windows then
            StringComparison.OrdinalIgnoreCase
        else
            StringComparison.Ordinal

    // netstandard2.0 has no String.Replace(string, string, StringComparison) overload, so replace every
    // occurrence by hand under an explicit comparison rather than assuming an unavailable overload exists.
    let replaceOrdinal (source: string) (oldValue: string) (newValue: string) =
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

    // The framework exceptions thrown by File/stream/XDocument APIs embed an absolute path derived from
    // the (possibly relative) path the task passed in. GetAbsolutePath only prepends the project
    // directory - it does not collapse dot segments - but the framework APIs canonicalize before they
    // throw (e.g. 'sub/../Missing.resx' surfaces as '<project>/Missing.resx'). So for each original path
    // restore BOTH the raw rooted form and its canonicalized form back to the original spelling the task
    // was given, so the logged diagnostic surfaces the caller's own path and never leaks the project
    // root. Rooted forms are de-duplicated and applied longest first so a shorter rooted path that is a
    // prefix of a longer one cannot partially rewrite it.
    let restoreOriginalPaths (message: string) (originalPaths: string list) =
        // Compute the rooted and canonicalized absolute forms of one original. Always ask the
        // TaskEnvironment to root the path rather than second-guessing with Path.IsPathRooted: on Windows
        // a partially qualified input such as '\foo' (root-relative) or 'C:foo' (drive-relative) is
        // reported as rooted yet the framework still expands it against the project directory, so skipping
        // those would leak the expanded rooted form. GetAbsolutePath echoes the caller's own string back
        // unchanged only when it was already fully qualified; that case yields nothing, because rewriting
        // an already-absolute caller path (even into its own canonical form) would corrupt it. Any other
        // result differs from the original and is mapped back exactly as for a plain relative path. The
        // whole computation is guarded so that building a diagnostic can never itself throw for a
        // pathological path (e.g. characters the framework Path APIs reject on .NET Framework); such an
        // original is simply left unrestored.
        let rootedFormsOf (original: string) =
            try
                if String.IsNullOrEmpty original then
                    []
                else
                    let rooted = _taskEnvironment.GetAbsolutePath(original).Value

                    // Treat the path as already fully qualified only when rooting was a no-op under the
                    // platform path comparison; otherwise it (including a Windows partially qualified
                    // input) is restored just like a relative path.
                    if String.IsNullOrEmpty rooted || String.Equals(rooted, original, pathComparison) then
                        []
                    else
                        // GetCanonicalForm is internal to Microsoft.Build.Framework, so reproduce the
                        // canonicalization the framework applies with Path.GetFullPath. 'rooted' is already
                        // absolute, so this collapses dot segments without consulting the ambient current
                        // directory, keeping the task multithread-safe.
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

    let failTask fmt =
        Printf.ksprintf
            (fun msg ->
                this.Log.LogError msg
                raise TaskFailed)
            fmt

    let boilerplate =
        @"// <auto-generated>

namespace {0}

open System.Reflection
open System.Globalization

module internal {1} =
    type private C (_dummy:System.Int32) = class end
    let ResourceManager = new System.Resources.ResourceManager(""{2}"", C(0).GetType().GetTypeInfo().Assembly)
    let GetString(name:System.String) : System.String = ResourceManager.GetString(name, CultureInfo.CurrentUICulture)"

    let boilerplateGetObject =
        "    let GetObject(name:System.String) : System.Object = ResourceManager.GetObject(name, CultureInfo.CurrentUICulture)"

    let generateSource (resx: string) (fullModuleName: string) (generateLegacy: bool) (generateLiteral: bool) =
        // The catch below sanitizes every original path this call touches. Seed it with the input and
        // extend it once the derived source path exists, so path derivation stays inside the try - a
        // malformed resx then throws GetFileNameWithoutExtension/Path.Combine through the same handler
        // instead of escaping it - while the handler still knows every path to sanitize even if
        // derivation itself failed.
        let originalPaths = ResizeArray<string>()
        originalPaths.Add resx

        try
            let justFileName = Path.GetFileNameWithoutExtension(resx)
            let sourcePath = Path.Combine(_outputPath, justFileName + ".fs")
            originalPaths.Add sourcePath

            let printMessage fmt = Printf.ksprintf this.Log.LogMessage fmt

            // simple up-to-date check
            if
                File.Exists(rootedPath resx)
                && File.Exists(rootedPath sourcePath)
                && File.GetLastWriteTimeUtc(rootedPath resx)
                   <= File.GetLastWriteTimeUtc(rootedPath sourcePath)
            then
                printMessage "Skipping generation: '%s' since it is up-to-date." sourcePath
                Some(sourcePath)
            else
                let namespaceName, moduleName =
                    let parts = fullModuleName.Split('.')

                    if parts.Length = 1 then
                        ("global", parts.[0])
                    else
                        (String.Join(".", parts, 0, parts.Length - 1), parts.[parts.Length - 1])

                let generateGetObject =
                    not (
                        _targetFramework.StartsWith("netstandard1.")
                        || _targetFramework.StartsWith("netcoreapp1.")
                    )

                printMessage "Generating code for target framework %s" _targetFramework

                let sb =
                    StringBuilder().AppendLine(String.Format(boilerplate, namespaceName, moduleName, justFileName))

                if generateGetObject then
                    sb.AppendLine(boilerplateGetObject) |> ignore

                printMessage "Generating: %s" sourcePath

                let body =
                    let xname = XName.op_Implicit

                    XDocument.Load(rootedPath resx).Descendants(xname "data")
                    |> Seq.fold
                        (fun (sb: StringBuilder) (node: XElement) ->
                            let name =
                                match node.Attribute(xname "name") with
                                | null -> failTask "Missing resource name on element '%O'" node
                                | attr -> attr.Value

                            let docComment =
                                match node.Elements(xname "value").FirstOrDefault() with
                                | null -> failTask "Missing resource value for '%s'" name
                                | element -> element.Value.Trim()

                            let identifier =
                                if Char.IsLetter(name.[0]) || name.[0] = '_' then
                                    name
                                else
                                    "_" + name

                            let commentBody =
                                XElement(xname "summary", docComment).ToString().Split([| "\r\n"; "\r"; "\n" |], StringSplitOptions.None)
                                |> Array.fold (fun (sb: StringBuilder) line -> sb.AppendLine("    /// " + line)) (StringBuilder())
                            // add the resource
                            let accessorBody =
                                match (generateLegacy, generateLiteral) with
                                | (true, true) -> sprintf "    [<Literal>]\n    let %s = \"%s\"" identifier name
                                | (true, false) -> sprintf "    let %s = \"%s\"" identifier name // the [<Literal>] attribute can't be used for FSharp.Core
                                | (false, _) ->
                                    let isStringResource = node.Attribute(xname "type") |> isNull

                                    match (isStringResource, generateGetObject) with
                                    | (true, _) -> sprintf "    let %s() = GetString(\"%s\")" identifier name
                                    | (false, true) -> sprintf "    let %s() = GetObject(\"%s\")" identifier name
                                    | (false, false) -> "" // the target runtime doesn't support non-string resources
                            // TODO: When calling the `GetObject` version, parse the `type` attribute to discover the proper return type
                            sb.AppendLine().Append(commentBody).AppendLine(accessorBody))
                        sb

                File.WriteAllText(rootedPath sourcePath, body.ToString())
                printMessage "Done: %s" sourcePath
                Some(sourcePath)
        with
        | TaskFailed ->
            // failTask already logged the error via this.Log.LogError; logging again here would
            // duplicate the diagnostic, so just propagate the failure.
            None
        | e ->
            // Log via MSBuild's error reporting (never Console) and keep the diagnostic scoped to the
            // caller's own resx path. The exception text itself can also embed the rooted path (e.g. a
            // FileNotFoundException naming the file it tried to load), so the rooted values for both the
            // resx input and the generated source path are restored to the originals the task was given,
            // mirroring FSharpEmbedResourceText's approach.
            this.Log.LogError(
                sprintf
                    "An exception occurred when processing '%s': %s"
                    resx
                    (restoreOriginalPaths (e.ToString()) (List.ofSeq originalPaths))
            )

            None

    new() = FSharpEmbedResXSource(TaskEnvironment.Fallback)

    interface IMultiThreadableTask with
        member _.TaskEnvironment
            with get () = _taskEnvironment
            and set (value) = _taskEnvironment <- value

    [<Required>]
    member _.EmbeddedResource
        with get () = _embeddedText
        and set (value) = _embeddedText <- value

    [<Required>]
    member _.IntermediateOutputPath
        with get () = _outputPath
        and set (value) = _outputPath <- value

    member _.TargetFramework
        with get () = _targetFramework
        and set (value) = _targetFramework <- value

    [<Output>]
    member _.GeneratedSource = _generatedSource

    override this.Execute() =
        try
            let getBooleanMetadata (metadataName: string) (defaultValue: bool) (item: ITaskItem) =
                match item.GetMetadata(metadataName) with
                | value when String.IsNullOrWhiteSpace(value) -> defaultValue
                | value ->
                    match value.ToLowerInvariant() with
                    | "true" -> true
                    | "false" -> false
                    | _ -> failTask "Expected boolean value for '%s' found '%s'" metadataName value

            let mutable success = true

            let generatedSource =
                [|
                    for item in this.EmbeddedResource do
                        if getBooleanMetadata "GenerateSource" false item then
                            let moduleName =
                                match item.GetMetadata("GeneratedModuleName") with
                                | null
                                | "" -> Path.GetFileNameWithoutExtension(item.ItemSpec)
                                | value -> value

                            let generateLegacy = getBooleanMetadata "GenerateLegacyCode" false item
                            let generateLiteral = getBooleanMetadata "GenerateLiterals" true item

                            match generateSource item.ItemSpec moduleName generateLegacy generateLiteral with
                            | Some(source) -> yield TaskItem(source) :> ITaskItem
                            | None -> success <- false
                |]

            _generatedSource <- generatedSource
            success && not this.Log.HasLoggedErrors
        with TaskFailed ->
            false
