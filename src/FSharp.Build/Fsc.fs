// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Build

open System
open System.Diagnostics
open System.Globalization
open System.IO
open System.Reflection
open Microsoft.Build.Framework
open Microsoft.Build.Utilities
open Internal.Utilities

//There are a lot of flags on fsc.exe.
//For now, not all of them are represented in the "Fsc class" object model.
//The goal is to have the most common/important flags available via the Fsc class, and the
//rest can be "backdoored" through the .OtherFlags property.

type public Fsc() as this =

    inherit ToolTask()

    let mutable baseAddress: string MaybeNull = null
    let mutable capturedArguments: string list = [] // list of individual args, to pass to HostObject Compile()
    let mutable capturedFilenames: string list = [] // list of individual source filenames, to pass to HostObject Compile()
    let mutable checksumAlgorithm: string MaybeNull = null
    let mutable codePage: string MaybeNull = null
    let mutable commandLineArgs: ITaskItem list = []
    let mutable compilerTools: ITaskItem[] = [||]
    let mutable compressMetadata = false
    let mutable debugSymbols = false
    let mutable debugType: string MaybeNull = null
    let mutable defineConstants: ITaskItem[] = [||]
    let mutable delaySign: bool = false
    let mutable deterministic: bool = false
    let mutable disabledWarnings: string MaybeNull = null
    let mutable documentationFile: string MaybeNull = null
    let mutable dotnetFscCompilerPath: string MaybeNull = null
    let mutable embedAllSources = false
    let mutable embeddedFiles: ITaskItem[] = [||]
    let mutable generateInterfaceFile: string MaybeNull = null
    let mutable highEntropyVA: bool = false
    let mutable keyFile: string MaybeNull = null
    let mutable langVersion: string MaybeNull = null
    let mutable noFramework = false
    let mutable noInterfaceData = false
    let mutable noOptimizationData = false
    let mutable optimize: bool = true
    let mutable otherFlags: string MaybeNull = null
    let mutable outputAssembly: string MaybeNull = null
    let mutable outputRefAssembly: string MaybeNull = null
    let mutable pathMap: string MaybeNull = null
    let mutable pdbFile: string MaybeNull = null
    let mutable platform: string MaybeNull = null
    let mutable prefer32bit: bool = false
    let mutable preferredUILang: string MaybeNull = null
    let mutable publicSign: bool = false
    let mutable provideCommandLineArgs: bool = false
    let mutable references: ITaskItem[] = [||]
    let mutable referencePath: string MaybeNull = null
    let mutable refOnly: bool = false
    let mutable resources: ITaskItem[] = [||]
    let mutable skipCompilerExecution: bool = false
    let mutable sources: ITaskItem[] = [||]
    let mutable sourceLink: string MaybeNull = null
    let mutable subsystemVersion: string MaybeNull = null
    let mutable tailcalls: bool = true
    let mutable targetProfile: string MaybeNull = null
    let mutable targetType: string MaybeNull = null
    let mutable toolExe: string = "fsc.exe"

    let defaultToolPath =
        let locationOfThisDll =
            try
                Some(Path.GetDirectoryName(typeof<Fsc>.Assembly.Location))
            with _ ->
                None

        match FSharpEnvironment.BinFolderOfDefaultFSharpCompiler(locationOfThisDll) with
        | Some s -> s
        | None -> ""

    let mutable treatWarningsAsErrors: bool = false
    let mutable useStandardResourceNames: bool = false
    let mutable warningsAsErrors: string MaybeNull = null
    let mutable warningsNotAsErrors: string MaybeNull = null
    let mutable versionFile: string MaybeNull = null
    let mutable warningLevel: string MaybeNull = null
    let mutable warnOn: string MaybeNull = null
    let mutable win32icon: string MaybeNull = null
    let mutable win32res: string MaybeNull = null
    let mutable win32manifest: string MaybeNull = null
    let mutable vserrors: bool = false
    let mutable vslcid: string MaybeNull = null
    let mutable utf8output: bool = false
    let mutable useReflectionFreeCodeGen: bool = false

    /// Trim whitespace ... spaces, tabs, newlines,returns, Double quotes and single quotes
    let wsCharsToTrim = [| ' '; '\t'; '\"'; '\'' |]

    let splitAndWsTrim (s: string MaybeNull) =
        match s with
        | Null -> [||]
        | NonNull s ->
            let array =
                s.Split([| ';'; ','; '\r'; '\n' |], StringSplitOptions.RemoveEmptyEntries)

            array
            |> Array.map (fun item -> item.Trim(wsCharsToTrim))
            |> Array.filter (fun s -> not (String.IsNullOrEmpty s))

    // See bug 6483; this makes parallel build faster, and is fine to set unconditionally
    do this.YieldDuringToolExecution <- true

    let generateCommandLineBuilder () =
        let builder = new FSharpCommandLineBuilder()

        // OutputAssembly
        builder.AppendSwitchIfNotNull("-o:", outputAssembly)

        // CodePage
        builder.AppendSwitchIfNotNull("--codepage:", codePage)

        // Debug
        if debugSymbols then
            builder.AppendSwitch("-g")

        // DebugType
        builder.AppendSwitchIfNotNull(
            "--debug:",
            match debugType with
            | Null -> null
            | NonNull debugType ->
                match debugType.ToUpperInvariant() with
                | "NONE" -> null
                | "PORTABLE" -> "portable"
                | "PDBONLY" -> "pdbonly"
                | "EMBEDDED" -> "embedded"
                | "FULL" -> "full"
                | _ -> null
        )

        if embedAllSources then
            builder.AppendSwitch("--embed+")

        for item in embeddedFiles do
            builder.AppendSwitchIfNotNull("--embed:", item.ItemSpec)

        builder.AppendSwitchIfNotNull("--sourcelink:", sourceLink)

        builder.AppendSwitchIfNotNull("--langversion:", langVersion)

        // NoFramework
        if noFramework then
            builder.AppendSwitch("--noframework")

        // NoInterfaceData
        if noInterfaceData then
            builder.AppendSwitch("--nointerfacedata")

        // NoOptimizationData
        if noOptimizationData then
            builder.AppendSwitch("--nooptimizationdata")

        // BaseAddress
        builder.AppendSwitchIfNotNull("--baseaddress:", baseAddress)

        // CompressMetadata
        if compressMetadata then
            builder.AppendSwitch("--compressmetadata")

        // DefineConstants
        for item in defineConstants do
            builder.AppendSwitchIfNotNull("--define:", item.ItemSpec)

        // DocumentationFile
        builder.AppendSwitchIfNotNull("--doc:", documentationFile)

        // GenerateInterfaceFile
        builder.AppendSwitchIfNotNull("--sig:", generateInterfaceFile)

        // KeyFile
        builder.AppendSwitchIfNotNull("--keyfile:", keyFile)

        if delaySign then
            builder.AppendSwitch("--delaysign+")

        if publicSign then
            builder.AppendSwitch("--publicsign+")

        // Optimize
        if optimize then
            builder.AppendSwitch("--optimize+")
        else
            builder.AppendSwitch("--optimize-")

        // Tailcalls
        if not tailcalls then
            builder.AppendSwitch("--tailcalls-")

        // PdbFile
        builder.AppendSwitchIfNotNull("--pdb:", pdbFile)
        // Platform
        builder.AppendSwitchIfNotNull(
            "--platform:",

            let ToUpperInvariant (s: string MaybeNull) =
                match s with
                | Null -> null
                | NonNull s -> s.ToUpperInvariant()

            match ToUpperInvariant(platform), prefer32bit, ToUpperInvariant(targetType) with
            | "ANYCPU", true, "EXE"
            | "ANYCPU", true, "WINEXE" -> "anycpu32bitpreferred"
            | "ANYCPU", _, _ -> "anycpu"
            | "X86", _, _ -> "x86"
            | "X64", _, _ -> "x64"
            | "ARM", _, _ -> "arm"
            | "ARM64", _, _ -> "arm64"
            | _ -> null
        )

        // checksumAlgorithm
        builder.AppendSwitchIfNotNull(
            "--checksumalgorithm:",
            let ToUpperInvariant (s: string) =
                if s = null then
                    null
                else
                    s.ToUpperInvariant()

            match ToUpperInvariant(checksumAlgorithm) with
            | "SHA1" -> "Sha1"
            | "SHA256" -> "Sha256"
            | _ -> null
        )

        // Resources
        for item in resources do
            match useStandardResourceNames with
            | true ->
                builder.AppendSwitchIfNotNull(
                    "--resource:",
                    item.ItemSpec,
                    [| item.GetMetadata("LogicalName"); item.GetMetadata("Access") |]
                )
            | false -> builder.AppendSwitchIfNotNull("--resource:", item.ItemSpec)

        // VersionFile
        builder.AppendSwitchIfNotNull("--versionfile:", versionFile)

        // CompilerTools
        for item in compilerTools do
            builder.AppendSwitchIfNotNull("--compilertool:", item.ItemSpec)

        // References
        for item in references do
            builder.AppendSwitchIfNotNull("-r:", item.ItemSpec)

        builder.AppendSwitchesIfNotNull("--lib:", referencePath |> splitAndWsTrim, ",")

        // TargetType
        builder.AppendSwitchIfNotNull(
            "--target:",
            match targetType with
            | Null -> null
            | NonNull targetType ->
                match targetType.ToUpperInvariant() with
                | "LIBRARY" -> "library"
                | "EXE" -> "exe"
                | "WINEXE" -> "winexe"
                | "MODULE" -> "module"
                | _ -> null
        )

        // NoWarn
        builder.AppendSwitchesIfNotNull("--nowarn:", disabledWarnings |> splitAndWsTrim, ",")

        // WarningLevel
        builder.AppendSwitchIfNotNull("--warn:", warningLevel)

        builder.AppendSwitchesIfNotNull("--warnon:", warnOn |> splitAndWsTrim, ",")

        // TreatWarningsAsErrors
        if treatWarningsAsErrors then
            builder.AppendSwitch("--warnaserror")

        // WarningsAsErrors
        builder.AppendSwitchesIfNotNull("--warnaserror:", warningsAsErrors |> splitAndWsTrim, ",")

        // WarningsNotAsErrors
        builder.AppendSwitchesIfNotNull("--warnaserror-:", warningsNotAsErrors |> splitAndWsTrim, ",")

        // Win32IconFile
        builder.AppendSwitchIfNotNull("--win32icon:", win32icon)

        // Win32ResourceFile
        builder.AppendSwitchIfNotNull("--win32res:", win32res)

        // Win32ManifestFile
        builder.AppendSwitchIfNotNull("--win32manifest:", win32manifest)

        // VisualStudioStyleErrors
        if vserrors then
            builder.AppendSwitch("--vserrors")

        builder.AppendSwitchIfNotNull("--LCID:", vslcid)
        builder.AppendSwitchIfNotNull("--preferreduilang:", preferredUILang)

        if utf8output then
            builder.AppendSwitch("--utf8output")

        if useReflectionFreeCodeGen then
            builder.AppendSwitch("--reflectionfree")

        // When building using the fsc task, always emit the "fullpaths" flag to make the output easier
        // for the user to parse
        builder.AppendSwitch("--fullpaths")

        // When building using the fsc task, also emit "flaterrors" to ensure that multi-line error messages
        // aren't trimmed
        builder.AppendSwitch("--flaterrors")

        builder.AppendSwitchIfNotNull("--subsystemversion:", subsystemVersion)

        if highEntropyVA then
            builder.AppendSwitch("--highentropyva+")
        else
            builder.AppendSwitch("--highentropyva-")

        builder.AppendSwitchIfNotNull("--targetprofile:", targetProfile)

        builder.AppendSwitch("--nocopyfsharpcore")

        builder.AppendSwitchesIfNotNull("--pathmap:", pathMap |> splitAndWsTrim, ",")

        if deterministic then
            builder.AppendSwitch("--deterministic+")

        // OtherFlags - must be second-to-last
        builder.AppendSwitchUnquotedIfNotNull("", otherFlags)
        capturedArguments <- builder.CapturedArguments()

        // Sources - these have to go last
        builder.AppendFileNamesIfNotNull(sources, " ")
        capturedFilenames <- builder.CapturedFilenames()

        // Ref assemblies
        builder.AppendSwitchIfNotNull("--refout:", outputRefAssembly)

        if refOnly then
            builder.AppendSwitch("--refonly")

        builder

    // --baseaddress
    member _.BaseAddress
        with get () = baseAddress
        and set (s) = baseAddress <- s

    // --checksumalgorithm
    member _.ChecksumAlgorithm
        with get () = checksumAlgorithm
        and set (s) = checksumAlgorithm <- s

    // --codepage <int>: Specify the codepage to use when opening source files
    member _.CodePage
        with get () = codePage
        and set (s) = codePage <- s

    // -r <string>: Reference an F# or .NET assembly.
    member _.CompilerTools
        with get () = compilerTools
        and set (a) = compilerTools <- a

    // CompressMetadata
    member _.CompressMetadata
        with get () = compressMetadata
        and set (v) = compressMetadata <- v

    // -g: Produce debug file. Disables optimizations if a -O flag is not given.
    member _.DebugSymbols
        with get () = debugSymbols
        and set (b) = debugSymbols <- b

    // --debug <none/portable/embedded/pdbonly/full>: Emit debugging information
    member _.DebugType
        with get () = debugType
        and set (s) = debugType <- s

    member _.Deterministic
        with get () = deterministic
        and set (p) = deterministic <- p

    member _.DelaySign
        with get () = delaySign
        and set (s) = delaySign <- s

    // --nowarn <string>: Do not report the given specific warning.
    member _.DisabledWarnings
        with get () = disabledWarnings
        and set (a) = disabledWarnings <- a

    // --define <string>: Define the given conditional compilation symbol.
    member _.DefineConstants
        with get () = defineConstants
        and set (a) = defineConstants <- a

    // --doc <string>: Write the xmldoc of the assembly to the given file.
    member _.DocumentationFile
        with get () = documentationFile
        and set (s) = documentationFile <- s

    member _.DotnetFscCompilerPath
        with get () = dotnetFscCompilerPath
        and set (p) = dotnetFscCompilerPath <- p

    member _.EmbedAllSources
        with get () = embedAllSources
        and set (s) = embedAllSources <- s

    member _.Embed
        with get () = embeddedFiles
        and set (e) = embeddedFiles <- e

    member _.EmbeddedFiles
        with get () = embeddedFiles
        and set (e) = embeddedFiles <- e

    // --generate-interface-file <string>:
    //     Print the inferred interface of the
    //     assembly to a file.

    member _.GenerateInterfaceFile
        with get () = generateInterfaceFile
        and set (s) = generateInterfaceFile <- s

    // --keyfile <string>:
    //     Sign the assembly the given keypair file, as produced
    //     by the .NET Framework SDK 'sn.exe' tool. This produces
    //     an assembly with a strong name. This is only relevant if producing
    //     an assembly to be shared amongst programs from different
    //     directories, e.g. to be installed in the Global Assembly Cache.
    member _.KeyFile
        with get () = keyFile
        and set (s) = keyFile <- s

    member _.LangVersion
        with get () = langVersion
        and set (s) = langVersion <- s

    member _.LCID
        with get () = vslcid
        and set (p) = vslcid <- p

    // --noframework
    member _.NoFramework
        with get () = noFramework
        and set (b) = noFramework <- b

    // --nointerfacedata
    member _.NoInterfaceData
        with get () = noInterfaceData
        and set (b) = noInterfaceData <- b

    // --nooptimizationdata
    member _.NoOptimizationData
        with get () = noOptimizationData
        and set (b) = noOptimizationData <- b

    // --optimize
    member _.Optimize
        with get () = optimize
        and set (p) = optimize <- p

    // --tailcalls
    member _.Tailcalls
        with get () = tailcalls
        and set (p) = tailcalls <- p

    // REVIEW: decide whether to keep this, for now is handy way to deal with as-yet-unimplemented features
    member _.OtherFlags
        with get () = otherFlags
        and set (s) = otherFlags <- s

    // -o <string>: Name the output file
    member _.OutputAssembly
        with get () = outputAssembly
        and set (s) = outputAssembly <- s

    // --refout <string>: Name the output ref file
    member _.OutputRefAssembly
        with get () = outputRefAssembly
        and set (s) = outputRefAssembly <- s

    // --pathmap <string>: Paths to rewrite when producing deterministic builds
    member _.PathMap
        with get () = pathMap
        and set (s) = pathMap <- s

    // --pdb <string>:
    //     Name the debug output file
    member _.PdbFile
        with get () = pdbFile
        and set (s) = pdbFile <- s

    // --platform <string>: Limit which platforms this code can run on:
    //            x86
    //            x64
    //            anycpu
    //            anycpu32bitpreferred
    member _.Platform
        with get () = platform
        and set (s) = platform <- s

    // indicator whether anycpu32bitpreferred is applicable or not
    member _.Prefer32Bit
        with get () = prefer32bit
        and set (s) = prefer32bit <- s

    member _.PreferredUILang
        with get () = preferredUILang
        and set (s) = preferredUILang <- s

    member _.ProvideCommandLineArgs
        with get () = provideCommandLineArgs
        and set (p) = provideCommandLineArgs <- p

    member _.PublicSign
        with get () = publicSign
        and set (s) = publicSign <- s

    // -r <string>: Reference an F# or .NET assembly.
    member _.References
        with get () = references
        and set (a) = references <- a

    // --lib
    member _.ReferencePath
        with get () = referencePath
        and set (s) = referencePath <- s

    // --refonly
    member _.RefOnly
        with get () = refOnly
        and set (b) = refOnly <- b

    // --resource <string>: Embed the specified managed resources (.resource).
    //   Produce .resource files from .resx files using resgen.exe or resxc.exe.
    member _.Resources
        with get () = resources
        and set (a) = resources <- a

    member _.SkipCompilerExecution
        with get () = skipCompilerExecution
        and set (p) = skipCompilerExecution <- p

    // SourceLink
    member _.SourceLink
        with get () = sourceLink
        and set (s) = sourceLink <- s

    // source files
    member _.Sources
        with get () = sources
        and set (a) = sources <- a

    member _.TargetProfile
        with get () = targetProfile
        and set (p) = targetProfile <- p

    // --target exe: Produce an executable with a console
    // --target winexe: Produce an executable which does not have a
    //      stdin/stdout/stderr
    // --target library: Produce a DLL
    // --target module: Produce a module that can be added to another assembly
    member _.TargetType
        with get () = targetType
        and set (s) = targetType <- s

    member _.TreatWarningsAsErrors
        with get () = treatWarningsAsErrors
        and set (p) = treatWarningsAsErrors <- p

    // When set to true, generate resource names in the same way as C# with root namespace and folder names
    member _.UseStandardResourceNames
        with get () = useStandardResourceNames
        and set (s) = useStandardResourceNames <- s

    // --version-file <string>:
    member _.VersionFile
        with get () = versionFile
        and set (s) = versionFile <- s

    // For specifying a win32 icon file (.ico)
    member _.Win32IconFile
        with get () = win32icon
        and set (s) = win32icon <- s

    // For specifying a win32 native resource file (.res)
    member _.Win32ResourceFile
        with get () = win32res
        and set (s) = win32res <- s

    // For specifying a win32 manifest file
    member _.Win32ManifestFile
        with get () = win32manifest
        and set (m) = win32manifest <- m

    // For specifying the warning level (0-4)
    member _.WarningLevel
        with get () = warningLevel
        and set (s) = warningLevel <- s

    member _.WarningsAsErrors
        with get () = warningsAsErrors
        and set (s) = warningsAsErrors <- s

    member _.WarningsNotAsErrors
        with get () = warningsNotAsErrors
        and set (s) = warningsNotAsErrors <- s

    member _.WarnOn
        with get () = warnOn
        and set (s) = warnOn <- s

    member _.VisualStudioStyleErrors
        with get () = vserrors
        and set (p) = vserrors <- p

    member _.Utf8Output
        with get () = utf8output
        and set (p) = utf8output <- p

    member _.ReflectionFree
        with get () = useReflectionFreeCodeGen
        and set (p) = useReflectionFreeCodeGen <- p

    member _.SubsystemVersion
        with get () = subsystemVersion
        and set (p) = subsystemVersion <- p

    member _.HighEntropyVA
        with get () = highEntropyVA
        and set (p) = highEntropyVA <- p

    [<Output>]
    member _.CommandLineArgs
        with get () = List.toArray commandLineArgs
        and set (p) = commandLineArgs <- (List.ofArray p)

    // ToolTask methods
    override _.ToolName = "fsc.exe"

    override _.StandardErrorEncoding =
        if utf8output then
            System.Text.Encoding.UTF8
        else
            base.StandardErrorEncoding

    override _.StandardOutputEncoding =
        if utf8output then
            System.Text.Encoding.UTF8
        else
            base.StandardOutputEncoding

    override fsc.GenerateFullPathToTool() =
        if defaultToolPath = "" then
            raise (new System.InvalidOperationException(FSBuild.SR.toolpathUnknown ()))

        System.IO.Path.Combine(defaultToolPath, fsc.ToolExe)

    override fsc.LogToolCommand(message: string) =
        fsc.Log.LogMessageFromText(message, MessageImportance.Normal) |> ignore

    member internal fsc.InternalGenerateFullPathToTool() = fsc.GenerateFullPathToTool() // expose for unit testing

    member internal _.BaseExecuteTool(pathToTool, responseFileCommands, commandLineCommands) = // F# does not allow protected members to be captured by lambdas, this is the standard workaround
        base.ExecuteTool(pathToTool, responseFileCommands, commandLineCommands)

    /// Intercept the call to ExecuteTool to handle the host compile case.
    override fsc.ExecuteTool(pathToTool, responseFileCommands, commandLineCommands) =
        if provideCommandLineArgs then
            commandLineArgs <-
                fsc.GetCapturedArguments()
                |> Array.map (fun (arg: string) -> TaskItem(arg) :> ITaskItem)
                |> Array.toList

        if skipCompilerExecution then
            0
        else
            let host = box fsc.HostObject

            match host with
            | null -> base.ExecuteTool(pathToTool, responseFileCommands, commandLineCommands)
            | _ ->
                let sources = sources |> Array.map (fun i -> i.ItemSpec)

                let invokeCompiler baseCallDelegate =
                    try
                        let ret =
                            (host.GetType())
                                .InvokeMember(
                                    "Compile",
                                    BindingFlags.Public
                                    ||| BindingFlags.NonPublic
                                    ||| BindingFlags.InvokeMethod
                                    ||| BindingFlags.Instance,
                                    null,
                                    host,
                                    [|
                                        baseCallDelegate
                                        box (capturedArguments |> List.toArray)
                                        box (capturedFilenames |> List.toArray)
                                    |],
                                    CultureInfo.InvariantCulture
                                )

                        unbox ret
                    with
                    // ok, this is what happens when VS IDE cancels the build, no need to assert, just log the build-canceled error and return -1 to denote task failed
                    // Do a string compare on the type name to do eliminate a compile time dependency on Microsoft.Build.dll
                    | :? TargetInvocationException as tie when
                        not (isNull tie.InnerException)
                        && (tie.InnerException).GetType().FullName = "Microsoft.Build.Exceptions.BuildAbortedException"
                        ->
                        fsc.Log.LogError(tie.InnerException.Message, [||])
                        -1
                    | e -> reraise ()

                let baseCallDelegate =
                    Func<int>(fun () -> fsc.BaseExecuteTool(pathToTool, responseFileCommands, commandLineCommands))

                try
                    invokeCompiler baseCallDelegate
                with e ->
                    Debug.Fail(
                        "HostObject received by Fsc task did not have a Compile method or the compile method threw an exception. "
                        + (e.ToString())
                    )

                    reraise ()

    override _.GenerateCommandLineCommands() =
        let builder = new FSharpCommandLineBuilder()

        match dotnetFscCompilerPath with
        | Null
        | "" -> ()
        | NonNull dotnetFscCompilerPath -> builder.AppendSwitch(dotnetFscCompilerPath)

        builder.ToString()

    override _.GenerateResponseFileCommands() =
        let builder = generateCommandLineBuilder ()
        builder.GetCapturedArguments() |> String.concat Environment.NewLine

    // expose this to internal components (for nunit testing)
    member internal fsc.InternalGenerateCommandLineCommands() = fsc.GenerateCommandLineCommands()

    // expose this to internal components (for nunit testing)
    member internal fsc.InternalGenerateResponseFileCommands() = fsc.GenerateResponseFileCommands()

    member internal fsc.InternalExecuteTool(pathToTool, responseFileCommands, commandLineCommands) =
        fsc.ExecuteTool(pathToTool, responseFileCommands, commandLineCommands)

    member internal fsc.GetCapturedArguments() =
        [| yield! capturedArguments; yield! capturedFilenames |]
