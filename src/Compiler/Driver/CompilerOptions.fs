// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// # FSComp.SR.opts

module internal FSharp.Compiler.CompilerOptions

open System
open System.Diagnostics
open System.IO
open FSharp.Compiler.Optimizer
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILPdbWriter
open FSharp.Compiler.AbstractIL.Diagnostics
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerDiagnostics
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.Features
open FSharp.Compiler.IO
open FSharp.Compiler.Text.Range
open FSharp.Compiler.Text
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.DiagnosticsLogger

open Internal.Utilities
open System.Text

module Attributes =
    open System.Runtime.CompilerServices

    //[<assembly: System.Security.SecurityTransparent>]
    [<Dependency("FSharp.Core", LoadHint.Always)>]
    do ()

//----------------------------------------------------------------------------
// Compiler option parser
//
// The argument parser is used by both the VS plug-in and the fsc.exe to
// parse the include file path and other front-end arguments.
//
// The language service uses this function too. It's important to continue
// processing flags even if an error is seen in one so that the best possible
// intellisense can be show.
//--------------------------------------------------------------------------

[<RequireQualifiedAccess>]
type OptionSwitch =
    | On
    | Off

type OptionSpec =
    | OptionClear of bool ref
    | OptionFloat of (float -> unit)
    | OptionInt of (int -> unit)
    | OptionSwitch of (OptionSwitch -> unit)
    | OptionIntList of (int -> unit)
    | OptionIntListSwitch of (int -> OptionSwitch -> unit)
    | OptionRest of (string -> unit)
    | OptionSet of bool ref
    | OptionString of (string -> unit)
    | OptionStringList of (string -> unit)
    | OptionStringListSwitch of (string -> OptionSwitch -> unit)
    | OptionUnit of (unit -> unit)
    | OptionConsoleOnly of (CompilerOptionBlock list -> unit)
    | OptionGeneral of (string list -> bool) * (string list -> string list) // Applies? * (ApplyReturningResidualArgs)

and CompilerOption =
    | CompilerOption of
        name: string *
        argumentDescriptionString: string *
        actionSpec: OptionSpec *
        deprecationError: exn option *
        helpText: string option

and CompilerOptionBlock =
    | PublicOptions of heading: string * options: CompilerOption list
    | PrivateOptions of options: CompilerOption list

let GetOptionsOfBlock block =
    match block with
    | PublicOptions(_, opts) -> opts
    | PrivateOptions opts -> opts

let FilterCompilerOptionBlock pred block =
    match block with
    | PublicOptions(heading, opts) -> PublicOptions(heading, List.filter pred opts)
    | PrivateOptions opts -> PrivateOptions(List.filter pred opts)

let compilerOptionUsage (CompilerOption(s, tag, spec, _, _)) =
    let s =
        if s = "--" then
            ""
        else
            s (* s="flag" for "--flag" options. s="--" for "--" option. Adjust printing here for "--" case. *)

    match spec with
    | OptionUnit _
    | OptionSet _
    | OptionClear _
    | OptionConsoleOnly _ -> sprintf "--%s" s
    | OptionStringList _ -> sprintf "--%s:%s" s tag
    | OptionIntList _ -> sprintf "--%s:%s" s tag
    | OptionSwitch _ -> sprintf "--%s[+|-]" s
    | OptionStringListSwitch _ -> sprintf "--%s[+|-]:%s" s tag
    | OptionIntListSwitch _ -> sprintf "--%s[+|-]:%s" s tag
    | OptionString _ -> sprintf "--%s:%s" s tag
    | OptionInt _ -> sprintf "--%s:%s" s tag
    | OptionFloat _ -> sprintf "--%s:%s" s tag
    | OptionRest _ -> sprintf "--%s ..." s
    | OptionGeneral _ ->
        if String.IsNullOrEmpty(tag) then
            sprintf "%s" s
        else
            sprintf "%s:%s" s tag (* still being decided *)

let nl = Environment.NewLine

let getCompilerOption (CompilerOption(_s, _tag, _spec, _, help) as compilerOption) width =
    let sb = StringBuilder()

    let flagWidth = 42 // fixed width for printing of flags, e.g. --debug:{full|pdbonly|portable|embedded}
    let defaultLineWidth = 80 // the fallback width

    let lineWidth =
        match width with
        | None ->
            try
                Console.BufferWidth
            with _ ->
                defaultLineWidth
        | Some w -> w

    let lineWidth =
        if lineWidth = 0 then
            defaultLineWidth
        else
            lineWidth (* Have seen BufferWidth=0 on Linux/Mono Coreclr for sure *)

    // Lines have this form: <flagWidth><space><description>
    //   flagWidth chars - for flags description or padding on continuation lines.
    //   single space    - space.
    //   description     - words upto but excluding the final character of the line.
    let _ = sb.Append $"{compilerOptionUsage compilerOption, -40}"

    let printWord column (word: string) =
        // Have printed upto column.
        // Now print the next word including any preceding whitespace.
        // Returns the column printed to (suited to folding).
        if column + 1 (*space*) + word.Length >= lineWidth then // NOTE: "equality" ensures final character of the line is never printed
            let _ = sb.Append $"{nl}"
            let _ = sb.Append $"{String.Empty, -40} {word}"
            flagWidth + 1 + word.Length
        else
            let _ = sb.Append $" {word}"
            column + 1 + word.Length

    let words =
        match help with
        | None -> [||]
        | Some s -> s.Split [| ' ' |]

    let _finalColumn = Array.fold printWord flagWidth words
    let _ = sb.Append $"{nl}"
    sb.ToString()

let getPublicOptions heading opts width =
    match opts with
    | [] -> ""
    | _ ->
        $"{nl}{nl}                {heading}{nl}"
        + (opts |> List.map (fun t -> getCompilerOption t width) |> String.concat "")

let GetCompilerOptionBlocks blocks width =
    let sb = new StringBuilder()

    let publicBlocks =
        blocks
        |> List.choose (function
            | PrivateOptions _ -> None
            | PublicOptions(heading, opts) -> Some(heading, opts))

    let consider doneHeadings (heading, _opts) =
        if Set.contains heading doneHeadings then
            doneHeadings
        else
            let headingOptions =
                publicBlocks |> List.filter (fun (h2, _) -> heading = h2) |> List.collect snd

            let _ = sb.Append(getPublicOptions heading headingOptions width)
            Set.add heading doneHeadings

    List.fold consider Set.empty publicBlocks |> ignore<Set<string>>
    sb.ToString()

(* For QA *)
let dumpCompilerOption prefix (CompilerOption(str, _, spec, _, _)) =
    printf "section='%-25s' ! option=%-30s kind=" prefix str

    match spec with
    | OptionUnit _ -> printf "OptionUnit"
    | OptionSet _ -> printf "OptionSet"
    | OptionClear _ -> printf "OptionClear"
    | OptionConsoleOnly _ -> printf "OptionConsoleOnly"
    | OptionStringList _ -> printf "OptionStringList"
    | OptionIntList _ -> printf "OptionIntList"
    | OptionSwitch _ -> printf "OptionSwitch"
    | OptionStringListSwitch _ -> printf "OptionStringListSwitch"
    | OptionIntListSwitch _ -> printf "OptionIntListSwitch"
    | OptionString _ -> printf "OptionString"
    | OptionInt _ -> printf "OptionInt"
    | OptionFloat _ -> printf "OptionFloat"
    | OptionRest _ -> printf "OptionRest"
    | OptionGeneral _ -> printf "OptionGeneral"

    printf "\n"

let dumpCompilerOptionBlock =
    function
    | PublicOptions(heading, opts) -> List.iter (dumpCompilerOption heading) opts
    | PrivateOptions opts -> List.iter (dumpCompilerOption "NoSection") opts

let DumpCompilerOptionBlocks blocks =
    List.iter dumpCompilerOptionBlock blocks

let isSlashOpt (opt: string) =
    opt[0] = '/' && (opt.Length = 1 || not (opt[1..].Contains "/"))

module ResponseFile =

    type ResponseFileData = ResponseFileLine list

    and ResponseFileLine =
        | CompilerOptionSpec of string
        | Comment of string

    let parseFile path : Choice<ResponseFileData, Exception> =
        let parseLine (l: string) =
            match l with
            | s when String.IsNullOrWhiteSpace s -> None
            | s when l.StartsWithOrdinal("#") -> Some(ResponseFileLine.Comment(s.TrimStart('#')))
            | s -> Some(ResponseFileLine.CompilerOptionSpec(s.Trim()))

        try
            use stream = FileSystem.OpenFileForReadShim(path)
            use reader = new StreamReader(stream, true)

            let data =
                seq {
                    while not reader.EndOfStream do
                        !!reader.ReadLine()
                }
                |> Seq.choose parseLine
                |> List.ofSeq

            Choice1Of2 data
        with e ->
            Choice2Of2 e

let ParseCompilerOptions (collectOtherArgument: string -> unit, blocks: CompilerOptionBlock list, args) =
    use _ = UseBuildPhase BuildPhase.Parameter

    let specs = List.collect GetOptionsOfBlock blocks

    // returns a tuple - the option minus switchchars, the option tokenand  the option argument string
    let parseOption (option: string) =

        // Get option arguments, I.e everything following first:
        let opts = option.Split([| ':' |])
        let optArgs = String.Join(":", opts[1..])

        let opt =
            if String.IsNullOrEmpty(option) then
                ""
            // if it doesn't start with a '-' or '/', reject outright
            elif option[0] <> '-' && option[0] <> '/' then
                ""
            elif option <> "--" then
                // is it an abbreviated or MSFT-style option?
                // if so, strip the first character and move on with your life
                // Weirdly a -- option can't have only a 1 character name
                if option.Length = 2 || isSlashOpt option then
                    option[1..]
                elif option.Length >= 3 && option[2] = ':' then
                    option[1..]
                elif option.StartsWithOrdinal("--") then
                    match option.Length with
                    | l when l >= 4 && option[3] = ':' -> ""
                    | l when l > 3 -> option[2..]
                    | _ -> ""
                else
                    ""
            else
                option

        // grab the option token
        let token = opt.Split([| ':' |])[0]
        opt, token, optArgs

    let getOptionArg compilerOption (argString: string) =
        if String.IsNullOrEmpty(argString) then
            errorR (Error(FSComp.SR.buildOptionRequiresParameter (compilerOptionUsage compilerOption), rangeCmdArgs))

        argString

    let getOptionArgList compilerOption (argString: string) =
        if String.IsNullOrEmpty(argString) then
            errorR (Error(FSComp.SR.buildOptionRequiresParameter (compilerOptionUsage compilerOption), rangeCmdArgs))
            []
        else
            argString.Split([| ','; ';' |]) |> List.ofArray

    let getSwitchOpt (opt: string) =
        // if opt is a switch, strip the  '+' or '-'
        if
            opt <> "--"
            && opt.Length > 1
            && (opt.EndsWithOrdinal("+") || opt.EndsWithOrdinal("-"))
        then
            opt[0 .. opt.Length - 2]
        else
            opt

    let getSwitch (s: string) =
        let s = (s.Split([| ':' |]))[0]

        if s <> "--" && s.EndsWithOrdinal("-") then
            OptionSwitch.Off
        else
            OptionSwitch.On

    let rec processArg args =
        match args with
        | [] -> ()
        | opt: string :: t when opt.StartsWithOrdinal("@") ->
            let responseFileOptions =
                let fullpath =
                    try
                        Some(opt.TrimStart('@') |> FileSystem.GetFullPathShim)
                    with _ ->
                        None

                match fullpath with
                | None ->
                    errorR (Error(FSComp.SR.optsResponseFileNameInvalid opt, rangeCmdArgs))
                    []
                | Some path when not (FileSystem.FileExistsShim path) ->
                    errorR (Error(FSComp.SR.optsResponseFileNotFound (opt, path), rangeCmdArgs))
                    []
                | Some path ->
                    match ResponseFile.parseFile path with
                    | Choice2Of2 _ ->
                        errorR (Error(FSComp.SR.optsInvalidResponseFile (opt, path), rangeCmdArgs))
                        []
                    | Choice1Of2 rspData ->
                        let onlyOptions l =
                            match l with
                            | ResponseFile.ResponseFileLine.Comment _ -> None
                            | ResponseFile.ResponseFileLine.CompilerOptionSpec opt -> Some opt

                        rspData |> List.choose onlyOptions

            processArg (responseFileOptions @ t)
        | opt :: t ->
            let option, optToken, argString = parseOption opt

            let reportDeprecatedOption errOpt =
                match errOpt with
                | Some e -> warning e
                | None -> ()

            let rec attempt l =
                match l with
                | CompilerOption(s, _, OptionConsoleOnly f, d, _) :: _ when option = s ->
                    reportDeprecatedOption d
                    f blocks
                    t
                | CompilerOption(s, _, OptionUnit f, d, _) :: _ when optToken = s && String.IsNullOrEmpty(argString) ->
                    reportDeprecatedOption d
                    f ()
                    t
                | CompilerOption(s, _, OptionSwitch f, d, _) :: _ when getSwitchOpt optToken = s && String.IsNullOrEmpty(argString) ->
                    reportDeprecatedOption d
                    f (getSwitch opt)
                    t
                | CompilerOption(s, _, OptionSet f, d, _) :: _ when optToken = s && String.IsNullOrEmpty(argString) ->
                    reportDeprecatedOption d
                    f.Value <- true
                    t
                | CompilerOption(s, _, OptionClear f, d, _) :: _ when optToken = s && String.IsNullOrEmpty(argString) ->
                    reportDeprecatedOption d
                    f.Value <- false
                    t
                | CompilerOption(s, _, OptionString f, d, _) as compilerOption :: _ when optToken = s ->
                    reportDeprecatedOption d
                    let oa = getOptionArg compilerOption argString

                    if oa <> "" then
                        f (getOptionArg compilerOption oa)

                    t
                | CompilerOption(s, _, OptionInt f, d, _) as compilerOption :: _ when optToken = s ->
                    reportDeprecatedOption d
                    let oa = getOptionArg compilerOption argString

                    if oa <> "" then
                        f (
                            try
                                int32 oa
                            with _ ->
                                errorR (Error(FSComp.SR.buildArgInvalidInt (getOptionArg compilerOption argString), rangeCmdArgs))
                                0
                        )

                    t
                | CompilerOption(s, _, OptionFloat f, d, _) as compilerOption :: _ when optToken = s ->
                    reportDeprecatedOption d
                    let oa = getOptionArg compilerOption argString

                    if oa <> "" then
                        f (
                            try
                                float oa
                            with _ ->
                                errorR (Error(FSComp.SR.buildArgInvalidFloat (getOptionArg compilerOption argString), rangeCmdArgs))
                                0.0
                        )

                    t
                | CompilerOption(s, _, OptionRest f, d, _) :: _ when optToken = s ->
                    reportDeprecatedOption d
                    List.iter f t
                    []
                | CompilerOption(s, _, OptionIntList f, d, _) as compilerOption :: _ when optToken = s ->
                    reportDeprecatedOption d
                    let al = getOptionArgList compilerOption argString

                    if al <> [] then
                        List.iter
                            (fun i ->
                                f (
                                    try
                                        int32 i
                                    with _ ->
                                        errorR (Error(FSComp.SR.buildArgInvalidInt i, rangeCmdArgs))
                                        0
                                ))
                            al

                    t
                | CompilerOption(s, _, OptionIntListSwitch f, d, _) as compilerOption :: _ when getSwitchOpt optToken = s ->
                    reportDeprecatedOption d
                    let al = getOptionArgList compilerOption argString

                    if al <> [] then
                        let switch = getSwitch opt

                        List.iter
                            (fun i ->
                                f
                                    (try
                                        int32 i
                                     with _ ->
                                         errorR (Error(FSComp.SR.buildArgInvalidInt i, rangeCmdArgs))
                                         0)
                                    switch)
                            al

                    t
                // here
                | CompilerOption(s, _, OptionStringList f, d, _) as compilerOption :: _ when optToken = s ->
                    reportDeprecatedOption d
                    let al = getOptionArgList compilerOption argString

                    if al <> [] then
                        List.iter f (getOptionArgList compilerOption argString)

                    t
                | CompilerOption(s, _, OptionStringListSwitch f, d, _) as compilerOption :: _ when getSwitchOpt optToken = s ->
                    reportDeprecatedOption d
                    let al = getOptionArgList compilerOption argString

                    if al <> [] then
                        let switch = getSwitch opt
                        List.iter (fun s -> f s switch) (getOptionArgList compilerOption argString)

                    t
                | CompilerOption(_, _, OptionGeneral(pred, exec), d, _) :: _ when pred args ->
                    reportDeprecatedOption d
                    let rest = exec args in
                    rest // arguments taken, rest remaining
                | _ :: more -> attempt more
                | [] ->
                    if opt.Length = 0 || opt[0] = '-' || isSlashOpt opt then
                        // want the whole opt token - delimiter and all
                        let unrecOpt = opt.Split([| ':' |]).[0]
                        errorR (Error(FSComp.SR.buildUnrecognizedOption unrecOpt, rangeCmdArgs))
                        t
                    else
                        (collectOtherArgument opt
                         t)

            let rest = attempt specs
            processArg rest

    processArg args

//----------------------------------------------------------------------------
// Compiler options
//--------------------------------------------------------------------------

let mutable enableConsoleColoring = true // global state

let setFlag r n =
    match n with
    | 0 -> r false
    | 1 -> r true
    | _ -> raise (Failure "expected 0/1")

let SetOptimizeOff (tcConfigB: TcConfigBuilder) =
    tcConfigB.optSettings <-
        { tcConfigB.optSettings with
            jitOptUser = Some false
            localOptUser = Some false
            crossAssemblyOptimizationUser = Some false
            lambdaInlineThreshold = 0
        }

    tcConfigB.onlyEssentialOptimizationData <- true
    tcConfigB.doDetuple <- false
    tcConfigB.doTLR <- false
    tcConfigB.doFinalSimplify <- false

let SetOptimizeOn (tcConfigB: TcConfigBuilder) =
    tcConfigB.optSettings <-
        { tcConfigB.optSettings with
            jitOptUser = Some true
        }

    tcConfigB.optSettings <-
        { tcConfigB.optSettings with
            localOptUser = Some true
        }

    tcConfigB.optSettings <-
        { tcConfigB.optSettings with
            crossAssemblyOptimizationUser = Some true
        }

    tcConfigB.optSettings <-
        { tcConfigB.optSettings with
            lambdaInlineThreshold = 6
        }

    tcConfigB.doDetuple <- true
    tcConfigB.doTLR <- true
    tcConfigB.doFinalSimplify <- true

let SetOptimizeSwitch (tcConfigB: TcConfigBuilder) switch =
    if (switch = OptionSwitch.On) then
        SetOptimizeOn tcConfigB
    else
        SetOptimizeOff tcConfigB

let SetTailcallSwitch (tcConfigB: TcConfigBuilder) switch =
    tcConfigB.emitTailcalls <- (switch = OptionSwitch.On)

let SetDeterministicSwitch (tcConfigB: TcConfigBuilder) switch =
    tcConfigB.deterministic <- (switch = OptionSwitch.On)

let SetRealsig (tcConfigB: TcConfigBuilder) switch =
    tcConfigB.realsig <- (switch = OptionSwitch.On)

let SetReferenceAssemblyOnlySwitch (tcConfigB: TcConfigBuilder) switch =
    match tcConfigB.emitMetadataAssembly with
    | MetadataAssemblyGeneration.None when (not tcConfigB.standalone) && tcConfigB.extraStaticLinkRoots.IsEmpty ->
        tcConfigB.emitMetadataAssembly <-
            if (switch = OptionSwitch.On) then
                MetadataAssemblyGeneration.ReferenceOnly
            else
                MetadataAssemblyGeneration.None
    | _ -> error (Error(FSComp.SR.optsInvalidRefAssembly (), rangeCmdArgs))

let SetReferenceAssemblyOutSwitch (tcConfigB: TcConfigBuilder) outputPath =
    match tcConfigB.emitMetadataAssembly with
    | MetadataAssemblyGeneration.None when (not tcConfigB.standalone) && tcConfigB.extraStaticLinkRoots.IsEmpty ->
        if FileSystem.IsInvalidPathShim outputPath then
            error (Error(FSComp.SR.optsInvalidRefOut (), rangeCmdArgs))
        else
            tcConfigB.emitMetadataAssembly <- MetadataAssemblyGeneration.ReferenceOut outputPath
    | _ -> error (Error(FSComp.SR.optsInvalidRefAssembly (), rangeCmdArgs))

let AddPathMapping (tcConfigB: TcConfigBuilder) (pathPair: string) =
    match pathPair.Split([| '=' |], 2) with
    | [| oldPrefix; newPrefix |] -> tcConfigB.AddPathMapping(oldPrefix, newPrefix)
    | _ -> error (Error(FSComp.SR.optsInvalidPathMapFormat (), rangeCmdArgs))

let jitoptimizeSwitch (tcConfigB: TcConfigBuilder) switch =
    tcConfigB.optSettings <-
        { tcConfigB.optSettings with
            jitOptUser = Some(switch = OptionSwitch.On)
        }

let localoptimizeSwitch (tcConfigB: TcConfigBuilder) switch =
    tcConfigB.optSettings <-
        { tcConfigB.optSettings with
            localOptUser = Some(switch = OptionSwitch.On)
        }

let crossOptimizeSwitch (tcConfigB: TcConfigBuilder) switch =
    tcConfigB.optSettings <-
        { tcConfigB.optSettings with
            crossAssemblyOptimizationUser = Some(switch = OptionSwitch.On)
        }

let splittingSwitch (tcConfigB: TcConfigBuilder) switch =
    tcConfigB.optSettings <-
        { tcConfigB.optSettings with
            abstractBigTargets = switch = OptionSwitch.On
        }

let callVirtSwitch (tcConfigB: TcConfigBuilder) switch =
    tcConfigB.alwaysCallVirt <- switch = OptionSwitch.On

let callParallelCompilationSwitch (tcConfigB: TcConfigBuilder) switch =
    tcConfigB.parallelIlxGen <- switch = OptionSwitch.On

    let (graphCheckingMode, optMode) =
        match switch with
        | OptionSwitch.On -> TypeCheckingMode.Graph, OptimizationProcessingMode.Parallel
        | OptionSwitch.Off -> TypeCheckingMode.Sequential, OptimizationProcessingMode.Sequential

    if tcConfigB.typeCheckingConfig.Mode <> graphCheckingMode then
        tcConfigB.typeCheckingConfig <-
            { tcConfigB.typeCheckingConfig with
                Mode = graphCheckingMode
            }

    if tcConfigB.optSettings.processingMode <> optMode then
        tcConfigB.optSettings <-
            { tcConfigB.optSettings with
                processingMode = optMode
            }

let useHighEntropyVASwitch (tcConfigB: TcConfigBuilder) switch =
    tcConfigB.useHighEntropyVA <- switch = OptionSwitch.On

let subSystemVersionSwitch (tcConfigB: TcConfigBuilder) (text: string) =
    let fail () =
        error (Error(FSComp.SR.optsInvalidSubSystemVersion text, rangeCmdArgs))

    // per spec for 357994: Validate input string, should be two positive integers x.y when x>=4 and y>=0 and both <= 65535
    if String.IsNullOrEmpty text then
        fail ()
    else
        match text.Split('.') with
        | [| majorStr; minorStr |] ->
            match (Int32.TryParse majorStr), (Int32.TryParse minorStr) with
            | (true, major), (true, minor) when major >= 4 && major <= 65535 && minor >= 0 && minor <= 65535 ->
                tcConfigB.subsystemVersion <- (major, minor)
            | _ -> fail ()
        | _ -> fail ()

let SetUseSdkSwitch (tcConfigB: TcConfigBuilder) switch =
    let useSdkRefs = (switch = OptionSwitch.On)
    tcConfigB.SetUseSdkRefs useSdkRefs

let (++) x s = x @ [ s ]

let SetTarget (tcConfigB: TcConfigBuilder) (s: string) =
    match s.ToLowerInvariant() with
    | "exe" -> tcConfigB.target <- CompilerTarget.ConsoleExe
    | "winexe" -> tcConfigB.target <- CompilerTarget.WinExe
    | "library" -> tcConfigB.target <- CompilerTarget.Dll
    | "module" -> tcConfigB.target <- CompilerTarget.Module
    | _ -> error (Error(FSComp.SR.optsUnrecognizedTarget s, rangeCmdArgs))

let SetDebugSwitch (tcConfigB: TcConfigBuilder) (dtype: string option) (s: OptionSwitch) =
    match dtype with
    | Some s ->
        tcConfigB.portablePDB <- true
        tcConfigB.jitTracking <- true

        match s with
        | "full"
        | "pdbonly"
        | "portable" -> tcConfigB.embeddedPDB <- false
        | "embedded" -> tcConfigB.embeddedPDB <- true
        | _ -> error (Error(FSComp.SR.optsUnrecognizedDebugType s, rangeCmdArgs))
    | None ->
        tcConfigB.portablePDB <- s = OptionSwitch.On
        tcConfigB.embeddedPDB <- false
        tcConfigB.jitTracking <- s = OptionSwitch.On

    tcConfigB.debuginfo <- s = OptionSwitch.On

let SetEmbedAllSourceSwitch (tcConfigB: TcConfigBuilder) switch =
    if (switch = OptionSwitch.On) then
        tcConfigB.embedAllSource <- true
    else
        tcConfigB.embedAllSource <- false

let setOutFileName tcConfigB (path: string) =
    let outputDir = !!Path.GetDirectoryName(path)
    tcConfigB.outputDir <- Some outputDir
    tcConfigB.outputFile <- Some path

let setSignatureFile tcConfigB s =
    tcConfigB.printSignature <- true
    tcConfigB.printSignatureFile <- s

let setAllSignatureFiles tcConfigB () =
    tcConfigB.printAllSignatureFiles <- true

let formatOptionSwitch (value: bool) = if value then "on" else "off"

// option tags
let tagString = "<string>"
let tagExe = "exe"
let tagWinExe = "winexe"
let tagLibrary = "library"
let tagModule = "module"
let tagFile = "<file>"
let tagFileList = "<file;...>"
let tagDirList = "<dir;...>"
let tagResInfo = "<resinfo>"
let tagFullPDBOnlyPortable = "{full|pdbonly|portable|embedded}"
let tagWarnList = "<warn;...>"
let tagAddress = "<address>"
let tagAlgorithm = "{SHA1|SHA256}"
let tagInt = "<n>"
let tagPathMap = "<path=sourcePath;...>"
let tagNone = ""
let tagLangVersionValues = "{version|latest|preview}"

// PrintOptionInfo
//----------------

/// Print internal "option state" information for diagnostics and regression tests.
let PrintOptionInfo (tcConfigB: TcConfigBuilder) =
    printfn "  jitOptUser . . . . . . : %+A" tcConfigB.optSettings.jitOptUser
    printfn "  localOptUser . . . . . : %+A" tcConfigB.optSettings.localOptUser
    printfn "  crossAssemblyOptimizationUser . . : %+A" tcConfigB.optSettings.crossAssemblyOptimizationUser
    printfn "  lambdaInlineThreshold  : %+A" tcConfigB.optSettings.lambdaInlineThreshold
    printfn "  doDetuple  . . . . . . : %+A" tcConfigB.doDetuple
    printfn "  doTLR  . . . . . . . . : %+A" tcConfigB.doTLR
    printfn "  doFinalSimplify. . . . : %+A" tcConfigB.doFinalSimplify
    printfn "  jitTracking  . . . . . : %+A" tcConfigB.jitTracking
    printfn "  portablePDB. . . . . . : %+A" tcConfigB.portablePDB
    printfn "  embeddedPDB. . . . . . : %+A" tcConfigB.embeddedPDB
    printfn "  embedAllSource . . . . : %+A" tcConfigB.embedAllSource
    printfn "  embedSourceList. . . . : %+A" tcConfigB.embedSourceList
    printfn "  sourceLink . . . . . . : %+A" tcConfigB.sourceLink
    printfn "  debuginfo  . . . . . . : %+A" tcConfigB.debuginfo
    printfn "  resolutionEnvironment  : %+A" tcConfigB.resolutionEnvironment
    printfn "  product  . . . . . . . : %+A" tcConfigB.productNameForBannerText
    printfn "  copyFSharpCore . . . . : %+A" tcConfigB.copyFSharpCore

    tcConfigB.includes
    |> List.sort
    |> List.iter (printfn "  include  . . . . . . . : %A")

// OptionBlock: Input files
//-------------------------

let inputFileFlagsBoth (tcConfigB: TcConfigBuilder) =
    [
        CompilerOption(
            "reference",
            tagFile,
            OptionString(fun s -> tcConfigB.AddReferencedAssemblyByPath(rangeStartup, s)),
            None,
            Some(FSComp.SR.optsReference ())
        )
        CompilerOption("compilertool", tagFile, OptionString tcConfigB.AddCompilerToolsByPath, None, Some(FSComp.SR.optsCompilerTool ()))
    ]

let inputFileFlagsFsc tcConfigB = inputFileFlagsBoth tcConfigB

let inputFileFlagsFsiBase (_tcConfigB: TcConfigBuilder) =
    [
        if FSharpEnvironment.isRunningOnCoreClr then
            yield CompilerOption("usesdkrefs", tagNone, OptionSwitch(SetUseSdkSwitch _tcConfigB), None, Some(FSComp.SR.useSdkRefs ()))
    ]

let inputFileFlagsFsi (tcConfigB: TcConfigBuilder) =
    List.append (inputFileFlagsBoth tcConfigB) (inputFileFlagsFsiBase tcConfigB)

// OptionBlock: Errors and warnings
//---------------------------------

let errorsAndWarningsFlags (tcConfigB: TcConfigBuilder) =
    let trimFS (s: string) =
        if s.StartsWithOrdinal "FS" then s.Substring 2 else s

    let trimFStoInt (s: string) =
        match Int32.TryParse(trimFS s) with
        | true, n -> Some n
        | false, _ -> None

    [
        CompilerOption(
            "warnaserror",
            tagNone,
            OptionSwitch(fun switch ->
                tcConfigB.diagnosticsOptions <-
                    { tcConfigB.diagnosticsOptions with
                        GlobalWarnAsError = switch <> OptionSwitch.Off
                    }),
            None,
            Some(FSComp.SR.optsWarnaserrorPM (formatOptionSwitch tcConfigB.diagnosticsOptions.GlobalWarnAsError))
        )

        CompilerOption(
            "warnaserror",
            tagWarnList,
            OptionStringListSwitch(fun n switch ->
                match trimFStoInt n with
                | Some n ->
                    let options = tcConfigB.diagnosticsOptions

                    tcConfigB.diagnosticsOptions <-
                        if switch = OptionSwitch.Off then
                            { options with
                                WarnAsError = ListSet.remove (=) n options.WarnAsError
                                WarnAsWarn = ListSet.insert (=) n options.WarnAsWarn
                            }
                        else
                            { options with
                                WarnAsError = ListSet.insert (=) n options.WarnAsError
                                WarnAsWarn = ListSet.remove (=) n options.WarnAsWarn
                            }
                | None -> ()),
            None,
            Some(FSComp.SR.optsWarnaserror ())
        )

        CompilerOption(
            "warn",
            tagInt,
            OptionInt(fun n ->
                tcConfigB.diagnosticsOptions <-
                    { tcConfigB.diagnosticsOptions with
                        WarnLevel =
                            if (n >= 0 && n <= 5) then
                                n
                            else
                                error (Error(FSComp.SR.optsInvalidWarningLevel n, rangeCmdArgs))
                    }),
            None,
            Some(FSComp.SR.optsWarn ())
        )

        CompilerOption(
            "nowarn",
            tagWarnList,
            OptionStringList(fun n -> tcConfigB.TurnWarningOff(rangeCmdArgs, n)),
            None,
            Some(FSComp.SR.optsNowarn ())
        )

        CompilerOption(
            "warnon",
            tagWarnList,
            OptionStringList(fun n -> tcConfigB.TurnWarningOn(rangeCmdArgs, n)),
            None,
            Some(FSComp.SR.optsWarnOn ())
        )

        CompilerOption(
            "checknulls",
            tagNone,
            OptionSwitch(fun switch -> tcConfigB.checkNullness <- switch = OptionSwitch.On),
            None,
            Some(FSComp.SR.optsCheckNulls (formatOptionSwitch tcConfigB.checkNullness))
        )

        CompilerOption(
            "consolecolors",
            tagNone,
            OptionSwitch(fun switch -> enableConsoleColoring <- switch = OptionSwitch.On),
            None,
            Some(FSComp.SR.optsConsoleColors (formatOptionSwitch enableConsoleColoring))
        )
    ]

// OptionBlock: Output files
//--------------------------

let outputFileFlagsFsi (_tcConfigB: TcConfigBuilder) = []

let outputFileFlagsFsc (tcConfigB: TcConfigBuilder) =
    [
        CompilerOption("out", tagFile, OptionString(setOutFileName tcConfigB), None, Some(FSComp.SR.optsNameOfOutputFile ()))

        CompilerOption("target", tagExe, OptionString(SetTarget tcConfigB), None, Some(FSComp.SR.optsBuildConsole ()))

        CompilerOption("target", tagWinExe, OptionString(SetTarget tcConfigB), None, Some(FSComp.SR.optsBuildWindows ()))

        CompilerOption("target", tagLibrary, OptionString(SetTarget tcConfigB), None, Some(FSComp.SR.optsBuildLibrary ()))

        CompilerOption("target", tagModule, OptionString(SetTarget tcConfigB), None, Some(FSComp.SR.optsBuildModule ()))

        CompilerOption(
            "delaysign",
            tagNone,
            OptionSwitch(fun s -> tcConfigB.delaysign <- (s = OptionSwitch.On)),
            None,
            Some(FSComp.SR.optsDelaySign (formatOptionSwitch tcConfigB.delaysign))
        )

        CompilerOption(
            "publicsign",
            tagNone,
            OptionSwitch(fun s -> tcConfigB.publicsign <- (s = OptionSwitch.On)),
            None,
            Some(FSComp.SR.optsPublicSign (formatOptionSwitch tcConfigB.publicsign))
        )

        CompilerOption("doc", tagFile, OptionString(fun s -> tcConfigB.xmlDocOutputFile <- Some s), None, Some(FSComp.SR.optsWriteXml ()))

        CompilerOption("keyfile", tagFile, OptionString(fun s -> tcConfigB.signer <- Some s), None, Some(FSComp.SR.optsStrongKeyFile ()))

        CompilerOption(
            "platform",
            tagString,
            OptionString(fun s ->
                tcConfigB.platform <-
                    match s with
                    | "x86" -> Some X86
                    | "x64" -> Some AMD64
                    | "arm" -> Some ARM
                    | "arm64" -> Some ARM64
                    | "Itanium" -> Some IA64
                    | "anycpu32bitpreferred" ->
                        tcConfigB.prefer32Bit <- true
                        None
                    | "anycpu" -> None
                    | _ -> error (Error(FSComp.SR.optsUnknownPlatform s, rangeCmdArgs))),
            None,
            Some(FSComp.SR.optsPlatform ())
        )

        CompilerOption(
            "compressmetadata",
            tagNone,
            OptionSwitch(fun switch -> tcConfigB.compressMetadata <- switch = OptionSwitch.On),
            None,
            Some(FSComp.SR.optsCompressMetadata (formatOptionSwitch tcConfigB.compressMetadata))
        )

        CompilerOption(
            "nooptimizationdata",
            tagNone,
            OptionUnit(fun () -> tcConfigB.onlyEssentialOptimizationData <- true),
            None,
            Some(FSComp.SR.optsNoOpt ())
        )

        CompilerOption(
            "nointerfacedata",
            tagNone,
            OptionUnit(fun () -> tcConfigB.noSignatureData <- true),
            None,
            Some(FSComp.SR.optsNoInterface ())
        )

        CompilerOption("sig", tagFile, OptionString(setSignatureFile tcConfigB), None, Some(FSComp.SR.optsSig ()))

        CompilerOption("allsigs", tagNone, OptionUnit(setAllSignatureFiles tcConfigB), None, Some(FSComp.SR.optsAllSigs ()))

        CompilerOption(
            "nocopyfsharpcore",
            tagNone,
            OptionUnit(fun () -> tcConfigB.copyFSharpCore <- CopyFSharpCoreFlag.No),
            None,
            Some(FSComp.SR.optsNoCopyFsharpCore ())
        )

        CompilerOption(
            "refonly",
            tagNone,
            OptionSwitch(SetReferenceAssemblyOnlySwitch tcConfigB),
            None,
            Some(FSComp.SR.optsRefOnly (formatOptionSwitch (tcConfigB.emitMetadataAssembly <> MetadataAssemblyGeneration.None)))
        )

        CompilerOption("refout", tagFile, OptionString(SetReferenceAssemblyOutSwitch tcConfigB), None, Some(FSComp.SR.optsRefOut ()))
    ]

// OptionBlock: Resources
//-----------------------

let resourcesFlagsFsi (_tcConfigB: TcConfigBuilder) = []

let resourcesFlagsFsc (tcConfigB: TcConfigBuilder) =
    [
        CompilerOption("win32icon", tagFile, OptionString(fun s -> tcConfigB.win32icon <- s), None, Some(FSComp.SR.optsWin32icon ()))
        CompilerOption("win32res", tagFile, OptionString(fun s -> tcConfigB.win32res <- s), None, Some(FSComp.SR.optsWin32res ()))

        CompilerOption(
            "win32manifest",
            tagFile,
            OptionString(fun s -> tcConfigB.win32manifest <- s),
            None,
            Some(FSComp.SR.optsWin32manifest ())
        )

        CompilerOption(
            "nowin32manifest",
            tagNone,
            OptionUnit(fun () -> tcConfigB.includewin32manifest <- false),
            None,
            Some(FSComp.SR.optsNowin32manifest ())
        )

        CompilerOption("resource", tagResInfo, OptionString tcConfigB.AddEmbeddedResource, None, Some(FSComp.SR.optsResource ()))

        CompilerOption(
            "linkresource",
            tagResInfo,
            OptionString(fun s -> tcConfigB.linkResources <- tcConfigB.linkResources ++ s),
            None,
            Some(FSComp.SR.optsLinkresource ())
        )
    ]

// OptionBlock: Code generation
//-----------------------------

let codeGenerationFlags isFsi (tcConfigB: TcConfigBuilder) =
    let debug =
        [
            CompilerOption(
                "debug",
                tagNone,
                OptionSwitch(SetDebugSwitch tcConfigB None),
                None,
                Some(FSComp.SR.optsDebugPM (formatOptionSwitch tcConfigB.debuginfo))
            )

            CompilerOption(
                "debug",
                tagFullPDBOnlyPortable,
                OptionString(fun s -> SetDebugSwitch tcConfigB (Some s) OptionSwitch.On),
                None,
                Some(FSComp.SR.optsDebug (if isFsi then "pdbonly" else "full"))
            )
        ]

    let embed =
        [
            CompilerOption(
                "embed",
                tagNone,
                OptionSwitch(SetEmbedAllSourceSwitch tcConfigB),
                None,
                Some(FSComp.SR.optsEmbedAllSource (formatOptionSwitch tcConfigB.embedAllSource))
            )

            CompilerOption("embed", tagFileList, OptionStringList tcConfigB.AddEmbeddedSourceFile, None, Some(FSComp.SR.optsEmbedSource ()))

            CompilerOption("sourcelink", tagFile, OptionString(fun f -> tcConfigB.sourceLink <- f), None, Some(FSComp.SR.optsSourceLink ()))
        ]

    let codegen =
        [
            CompilerOption(
                "optimize",
                tagNone,
                OptionSwitch(SetOptimizeSwitch tcConfigB),
                None,
                Some(FSComp.SR.optsOptimize (formatOptionSwitch (tcConfigB.optSettings <> OptimizationSettings.Defaults)))
            )

            CompilerOption(
                "tailcalls",
                tagNone,
                OptionSwitch(SetTailcallSwitch tcConfigB),
                None,
                Some(FSComp.SR.optsTailcalls (formatOptionSwitch tcConfigB.emitTailcalls))
            )

            CompilerOption(
                "deterministic",
                tagNone,
                OptionSwitch(SetDeterministicSwitch tcConfigB),
                None,
                Some(FSComp.SR.optsDeterministic (formatOptionSwitch tcConfigB.deterministic))
            )

            CompilerOption(
                "realsig",
                tagNone,
                OptionSwitch(SetRealsig tcConfigB),
                None,
                Some(FSComp.SR.optsRealsig (formatOptionSwitch tcConfigB.realsig))
            )

            CompilerOption("pathmap", tagPathMap, OptionStringList(AddPathMapping tcConfigB), None, Some(FSComp.SR.optsPathMap ()))

            CompilerOption(
                "crossoptimize",
                tagNone,
                OptionSwitch(crossOptimizeSwitch tcConfigB),
                None,
                Some(
                    FSComp.SR.optsCrossoptimize (
                        formatOptionSwitch (Option.defaultValue false tcConfigB.optSettings.crossAssemblyOptimizationUser)
                    )
                )
            )

            CompilerOption(
                "reflectionfree",
                tagNone,
                OptionUnit(fun () -> tcConfigB.useReflectionFreeCodeGen <- true),
                None,
                Some(FSComp.SR.optsReflectionFree ())
            )
        ]

    if isFsi then debug @ codegen else debug @ embed @ codegen

// OptionBlock: Language
//----------------------

let defineSymbol tcConfigB s =
    tcConfigB.conditionalDefines <- s :: tcConfigB.conditionalDefines

let mlCompatibilityFlag (tcConfigB: TcConfigBuilder) =
    CompilerOption(
        "mlcompatibility",
        tagNone,
        OptionUnit(fun () ->
            tcConfigB.mlCompatibility <- true
            tcConfigB.TurnWarningOff(rangeCmdArgs, "62")),
        None,
        Some(FSComp.SR.optsMlcompatibility ())
    )

let GetLanguageVersions () =
    seq {
        FSComp.SR.optsSupportedLangVersions ()
        yield! LanguageVersion.ValidOptions
        yield! LanguageVersion.ValidVersions
    }
    |> String.concat Environment.NewLine

let setLanguageVersion (specifiedVersion: string) =
    if specifiedVersion.ToUpperInvariant() = "PREVIEW" then
        ()
    elif not (LanguageVersion.ContainsVersion specifiedVersion) then
        error (Error(FSComp.SR.optsUnrecognizedLanguageVersion specifiedVersion, rangeCmdArgs))

    LanguageVersion(specifiedVersion)

let languageFlags tcConfigB =
    [
        // -langversion:?                Display the allowed values for language version
        CompilerOption(
            "langversion:?",
            tagNone,
            OptionConsoleOnly(fun _ ->
                Console.Write(GetLanguageVersions())
                tcConfigB.exiter.Exit 0),
            None,
            Some(FSComp.SR.optsGetLangVersions ())
        )

        // -langversion:<string>         Specify language version such as
        //                               'default' (latest major version), or
        //                               'latest' (latest version, including minor versions),
        //                               'preview' (features for preview)
        //                               or specific versions like '4.7'
        CompilerOption(
            "langversion",
            tagLangVersionValues,
            OptionString(fun switch -> tcConfigB.langVersion <- setLanguageVersion (switch)),
            None,
            Some(FSComp.SR.optsSetLangVersion ())
        )

        CompilerOption(
            "checked",
            tagNone,
            OptionSwitch(fun switch -> tcConfigB.checkOverflow <- (switch = OptionSwitch.On)),
            None,
            Some(FSComp.SR.optsChecked (formatOptionSwitch tcConfigB.checkOverflow))
        )

        CompilerOption("define", tagString, OptionString(defineSymbol tcConfigB), None, Some(FSComp.SR.optsDefine ()))

        mlCompatibilityFlag tcConfigB

        CompilerOption(
            "strict-indentation",
            tagNone,
            OptionSwitch(fun switch -> tcConfigB.strictIndentation <- Some(switch = OptionSwitch.On)),
            None,
            Some(FSComp.SR.optsStrictIndentation (formatOptionSwitch (Option.defaultValue false tcConfigB.strictIndentation)))
        )
    ]

// OptionBlock: Advanced user options
//-----------------------------------

let libFlag (tcConfigB: TcConfigBuilder) =
    CompilerOption(
        "lib",
        tagDirList,
        OptionStringList(fun s -> tcConfigB.AddIncludePath(rangeStartup, s, tcConfigB.implicitIncludeDir)),
        None,
        Some(FSComp.SR.optsLib ())
    )

let codePageFlag (tcConfigB: TcConfigBuilder) =
    CompilerOption(
        "codepage",
        tagInt,
        OptionInt(fun n ->
            try
                System.Text.Encoding.GetEncoding n |> ignore
            with :? ArgumentException as err ->
                error (Error(FSComp.SR.optsProblemWithCodepage (n, err.Message), rangeCmdArgs))

            tcConfigB.inputCodePage <- Some n),
        None,
        Some(FSComp.SR.optsCodepage ())
    )

let preferredUiLang (tcConfigB: TcConfigBuilder) =
    CompilerOption(
        "preferreduilang",
        tagString,
        OptionString(fun s -> tcConfigB.preferredUiLang <- Some s),
        None,
        Some(FSComp.SR.optsPreferredUiLang ())
    )

let utf8OutputFlag (tcConfigB: TcConfigBuilder) =
    CompilerOption("utf8output", tagNone, OptionUnit(fun () -> tcConfigB.utf8output <- true), None, Some(FSComp.SR.optsUtf8output ()))

let fullPathsFlag (tcConfigB: TcConfigBuilder) =
    CompilerOption("fullpaths", tagNone, OptionUnit(fun () -> tcConfigB.showFullPaths <- true), None, Some(FSComp.SR.optsFullpaths ()))

let cliRootFlag (_tcConfigB: TcConfigBuilder) =
    CompilerOption(
        "cliroot",
        tagString,
        OptionString(fun _ -> ()),
        Some(DeprecatedCommandLineOptionFull(FSComp.SR.optsClirootDeprecatedMsg (), rangeCmdArgs)),
        Some(FSComp.SR.optsClirootDescription ())
    )

let SetTargetProfile (tcConfigB: TcConfigBuilder) v =
    let primaryAssembly =
        match v with
        // Indicates we assume "mscorlib.dll", i.e .NET Framework, Mono and Profile 47
        | "mscorlib" -> PrimaryAssembly.Mscorlib
        // Indicates we assume "System.Runtime.dll", i.e .NET Standard 1.x, .NET Core App 1.x and above, and Profile 7/78/259
        | "netcore" -> PrimaryAssembly.System_Runtime
        // Indicates we assume "netstandard.dll", i.e .NET Standard 2.0 and above
        | "netstandard" -> PrimaryAssembly.NetStandard
        | _ -> error (Error(FSComp.SR.optsInvalidTargetProfile v, rangeCmdArgs))

    tcConfigB.SetPrimaryAssembly primaryAssembly

let advancedFlagsBoth tcConfigB =
    [
        codePageFlag tcConfigB
        utf8OutputFlag tcConfigB
        preferredUiLang tcConfigB
        fullPathsFlag tcConfigB
        libFlag tcConfigB
        CompilerOption(
            "simpleresolution",
            tagNone,
            OptionUnit(fun () -> tcConfigB.useSimpleResolution <- true),
            None,
            Some(FSComp.SR.optsSimpleresolution ())
        )

        CompilerOption("targetprofile", tagString, OptionString(SetTargetProfile tcConfigB), None, Some(FSComp.SR.optsTargetProfile ()))
    ]

let noFrameworkFlag isFsc tcConfigB =
    CompilerOption(
        "noframework",
        tagNone,
        OptionUnit(fun () ->
            // When the compilation is not fsi do nothing.
            // It is just not a useful option when running fsi on the coreclr or the desktop framework really.
            if isFsc then
                tcConfigB.implicitlyReferenceDotNetAssemblies <- false
                tcConfigB.implicitlyResolveAssemblies <- false),
        None,
        Some(FSComp.SR.optsNoframework ())
    )

let advancedFlagsFsi tcConfigB =
    advancedFlagsBoth tcConfigB
    @ [
        CompilerOption(
            "clearResultsCache",
            tagNone,
            OptionUnit(fun () -> tcConfigB.clearResultsCache <- true),
            None,
            Some(FSComp.SR.optsClearResultsCache ())
        )
    ]

let advancedFlagsFsc tcConfigB =
    advancedFlagsBoth tcConfigB
    @ [
        CompilerOption(
            "baseaddress",
            tagAddress,
            OptionString(fun s -> tcConfigB.baseAddress <- Some(int32 s)),
            None,
            Some(FSComp.SR.optsBaseaddress ())
        )

        CompilerOption(
            "checksumalgorithm",
            tagAlgorithm,
            OptionString(fun s ->
                tcConfigB.checksumAlgorithm <-
                    match s.ToUpperInvariant() with
                    | "SHA1" -> HashAlgorithm.Sha1
                    | "SHA256" -> HashAlgorithm.Sha256
                    | _ -> error (Error(FSComp.SR.optsUnknownChecksumAlgorithm s, rangeCmdArgs))),
            None,
            Some(FSComp.SR.optsChecksumAlgorithm ())
        )

        noFrameworkFlag true tcConfigB

        CompilerOption(
            "standalone",
            tagNone,
            OptionUnit(fun _ ->
                match tcConfigB.emitMetadataAssembly with
                | MetadataAssemblyGeneration.None ->
                    tcConfigB.openDebugInformationForLaterStaticLinking <- true
                    tcConfigB.standalone <- true
                    tcConfigB.implicitlyResolveAssemblies <- true
                | _ -> error (Error(FSComp.SR.optsInvalidRefAssembly (), rangeCmdArgs))),
            None,
            Some(FSComp.SR.optsStandalone ())
        )

        CompilerOption(
            "staticlink",
            tagFile,
            OptionString(fun s ->
                match tcConfigB.emitMetadataAssembly with
                | MetadataAssemblyGeneration.None ->
                    tcConfigB.extraStaticLinkRoots <- tcConfigB.extraStaticLinkRoots @ [ s ]
                    tcConfigB.implicitlyResolveAssemblies <- true
                | _ -> error (Error(FSComp.SR.optsInvalidRefAssembly (), rangeCmdArgs))),
            None,
            Some(FSComp.SR.optsStaticlink ())
        )

        CompilerOption("pdb", tagString, OptionString(fun s -> tcConfigB.debugSymbolFile <- Some s), None, Some(FSComp.SR.optsPdb ()))

        CompilerOption(
            "highentropyva",
            tagNone,
            OptionSwitch(useHighEntropyVASwitch tcConfigB),
            None,
            Some(FSComp.SR.optsUseHighEntropyVA (formatOptionSwitch tcConfigB.useHighEntropyVA))
        )

        CompilerOption(
            "subsystemversion",
            tagString,
            OptionString(subSystemVersionSwitch tcConfigB),
            None,
            Some(FSComp.SR.optsSubSystemVersion ())
        )

        CompilerOption(
            "quotations-debug",
            tagNone,
            OptionSwitch(fun switch -> tcConfigB.emitDebugInfoInQuotations <- switch = OptionSwitch.On),
            None,
            Some(FSComp.SR.optsEmitDebugInfoInQuotations (formatOptionSwitch tcConfigB.emitDebugInfoInQuotations))
        )
    ]

// OptionBlock: Internal options (test use only)
//--------------------------------------------------

let testFlag tcConfigB =
    CompilerOption(
        "test",
        tagString,
        OptionString(fun s ->
            match s with
            | "StackSpan" -> tcConfigB.internalTestSpanStackReferring <- true
            | "ErrorRanges" -> tcConfigB.diagnosticStyle <- DiagnosticStyle.Test
            | "Tracking" -> tracking <- true (* general purpose on/off diagnostics flag *)
            | "NoNeedToTailcall" ->
                tcConfigB.optSettings <-
                    { tcConfigB.optSettings with
                        reportNoNeedToTailcall = true
                    }
            | "FunctionSizes" ->
                tcConfigB.optSettings <-
                    { tcConfigB.optSettings with
                        reportFunctionSizes = true
                    }
            | "TotalSizes" ->
                tcConfigB.optSettings <-
                    { tcConfigB.optSettings with
                        reportTotalSizes = true
                    }
            | "HasEffect" ->
                tcConfigB.optSettings <-
                    { tcConfigB.optSettings with
                        reportHasEffect = true
                    }
            | "NoErrorText" -> FSComp.SR.SwallowResourceText <- true
            | "EmitFeeFeeAs100001" -> tcConfigB.testFlagEmitFeeFeeAs100001 <- true
            | "DumpDebugInfo" -> tcConfigB.dumpDebugInfo <- true
            | "ShowLoadedAssemblies" -> tcConfigB.showLoadedAssemblies <- true
            | "ContinueAfterParseFailure" -> tcConfigB.continueAfterParseFailure <- true
            | "ParallelOff" -> tcConfigB.parallelParsing <- false
            | "ParallelIlxGen" -> tcConfigB.parallelIlxGen <- true // Kept as --test:.. flag for temporary backwards compatibility during .NET10 period.
            | "GraphBasedChecking" -> // Kept as --test:.. flag for temporary backwards compatibility during .NET10 period.
                tcConfigB.typeCheckingConfig <-
                    { tcConfigB.typeCheckingConfig with
                        Mode = TypeCheckingMode.Graph
                    }
            | "DumpCheckingGraph" ->
                tcConfigB.typeCheckingConfig <-
                    { tcConfigB.typeCheckingConfig with
                        DumpGraph = true
                    }
            | "DumpSignatureData" -> tcConfigB.dumpSignatureData <- true
            | "ParallelOptimization" -> // Kept as --test:.. flag for temporary backwards compatibility during .NET10 period.
                tcConfigB.optSettings <-
                    { tcConfigB.optSettings with
                        processingMode = OptimizationProcessingMode.Parallel
                    }
#if DEBUG
            | "ShowParserStackOnParseError" -> showParserStackOnParseError <- true
#endif
            | str -> warning (Error(FSComp.SR.optsUnknownArgumentToTheTestSwitch str, rangeCmdArgs))),
        None,
        None
    )

// Not shown in fsc.exe help, no warning on use, motivation is for use from tooling.
let editorSpecificFlags (tcConfigB: TcConfigBuilder) =
    [
        CompilerOption("vserrors", tagNone, OptionUnit(fun () -> tcConfigB.diagnosticStyle <- DiagnosticStyle.VisualStudio), None, None)
        CompilerOption("richerrors", tagNone, OptionUnit(fun () -> tcConfigB.diagnosticStyle <- DiagnosticStyle.Rich), None, None)
        CompilerOption("validate-type-providers", tagNone, OptionUnit id, None, None) // preserved for compatibility's sake, no longer has any effect
        CompilerOption("LCID", tagInt, OptionInt ignore, None, None)
        CompilerOption("flaterrors", tagNone, OptionUnit(fun () -> tcConfigB.flatErrors <- true), None, None)
        CompilerOption("sqmsessionguid", tagNone, OptionString ignore, None, None)
        CompilerOption("gccerrors", tagNone, OptionUnit(fun () -> tcConfigB.diagnosticStyle <- DiagnosticStyle.Gcc), None, None)
        CompilerOption("exename", tagNone, OptionString(fun s -> tcConfigB.exename <- Some s), None, None)
        CompilerOption("maxerrors", tagInt, OptionInt(fun n -> tcConfigB.maxErrors <- n), None, None)
        CompilerOption("noconditionalerasure", tagNone, OptionUnit(fun () -> tcConfigB.noConditionalErasure <- true), None, None)
        CompilerOption("ignorelinedirectives", tagNone, OptionUnit(fun () -> tcConfigB.applyLineDirectives <- false), None, None)
    ]

let internalFlags (tcConfigB: TcConfigBuilder) =
    [
        CompilerOption(
            "typedtree",
            tagNone,
            OptionUnit(fun () -> tcConfigB.showTerms <- true),
            Some(InternalCommandLineOption("--typedtree", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "typedtreefile",
            tagNone,
            OptionUnit(fun () -> tcConfigB.writeTermsToFiles <- true),
            Some(InternalCommandLineOption("--typedtreefile", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "typedtreestamps",
            tagNone,
            OptionUnit(fun () -> DebugPrint.layoutStamps <- true),
            Some(InternalCommandLineOption("--typedtreestamps", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "typedtreeranges",
            tagNone,
            OptionUnit(fun () -> DebugPrint.layoutRanges <- true),
            Some(InternalCommandLineOption("--typedtreeranges", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "typedtreetypes",
            tagNone,
            OptionUnit(fun () -> DebugPrint.layoutTypes <- true),
            Some(InternalCommandLineOption("--typedtreetypes", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "typedtreevalreprinfo",
            tagNone,
            OptionUnit(fun () -> DebugPrint.layoutValReprInfo <- true),
            Some(InternalCommandLineOption("--typedtreevalreprinfo", rangeCmdArgs)),
            None
        )

#if DEBUG
        CompilerOption(
            "debug-parse",
            tagNone,
            OptionUnit(fun () -> Internal.Utilities.Text.Parsing.Flags.debug <- true),
            Some(InternalCommandLineOption("--debug-parse", rangeCmdArgs)),
            None
        )
#endif

        CompilerOption(
            "pause",
            tagNone,
            OptionUnit(fun () -> tcConfigB.pause <- true),
            Some(InternalCommandLineOption("--pause", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "bufferwidth",
            tagNone,
            OptionInt((fun v -> tcConfigB.bufferWidth <- Some v)),
            Some(InternalCommandLineOption("--bufferWidth", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "detuple",
            tagNone,
            OptionInt(setFlag (fun v -> tcConfigB.doDetuple <- v)),
            Some(InternalCommandLineOption("--detuple", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "simulateException",
            tagNone,
            OptionString(fun s -> tcConfigB.simulateException <- Some s),
            Some(InternalCommandLineOption("--simulateException", rangeCmdArgs)),
            Some "Simulate an exception from some part of the compiler"
        )

        CompilerOption(
            "stackReserveSize",
            tagNone,
            OptionString(fun s -> tcConfigB.stackReserveSize <- Some(int32 s)),
            Some(InternalCommandLineOption("--stackReserveSize", rangeCmdArgs)),
            Some "for an exe, set stack reserve size"
        )

        CompilerOption(
            "tlr",
            tagInt,
            OptionInt(setFlag (fun v -> tcConfigB.doTLR <- v)),
            Some(InternalCommandLineOption("--tlr", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "finalSimplify",
            tagInt,
            OptionInt(setFlag (fun v -> tcConfigB.doFinalSimplify <- v)),
            Some(InternalCommandLineOption("--finalSimplify", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "parseonly",
            tagNone,
            OptionUnit(fun () -> tcConfigB.parseOnly <- true),
            Some(InternalCommandLineOption("--parseonly", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "typecheckonly",
            tagNone,
            OptionUnit(fun () -> tcConfigB.typeCheckOnly <- true),
            Some(InternalCommandLineOption("--typecheckonly", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "ast",
            tagNone,
            OptionUnit(fun () -> tcConfigB.printAst <- true),
            Some(InternalCommandLineOption("--ast", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "tokenize",
            tagNone,
            OptionUnit(fun () -> tcConfigB.tokenize <- TokenizeOption.Only),
            Some(InternalCommandLineOption("--tokenize", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "tokenize-debug",
            tagNone,
            OptionUnit(fun () -> tcConfigB.tokenize <- TokenizeOption.Debug),
            Some(InternalCommandLineOption("--tokenize-debug", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "tokenize-unfiltered",
            tagNone,
            OptionUnit(fun () -> tcConfigB.tokenize <- TokenizeOption.Unfiltered),
            Some(InternalCommandLineOption("--tokenize-unfiltered", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "testInteractionParser",
            tagNone,
            OptionUnit(fun () -> tcConfigB.testInteractionParser <- true),
            Some(InternalCommandLineOption("--testInteractionParser", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "testparsererrorrecovery",
            tagNone,
            OptionUnit(fun () -> tcConfigB.reportNumDecls <- true),
            Some(InternalCommandLineOption("--testparsererrorrecovery", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "inlinethreshold",
            tagInt,
            OptionInt(fun n ->
                tcConfigB.optSettings <-
                    { tcConfigB.optSettings with
                        lambdaInlineThreshold = n
                    }),
            Some(InternalCommandLineOption("--inlinethreshold", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "extraoptimizationloops",
            tagNone,
            OptionInt(fun n -> tcConfigB.extraOptimizationIterations <- n),
            Some(InternalCommandLineOption("--extraoptimizationloops", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "abortonerror",
            tagNone,
            OptionUnit(fun () -> tcConfigB.abortOnError <- true),
            Some(InternalCommandLineOption("--abortonerror", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "implicitresolution",
            tagNone,
            OptionUnit(fun _ -> tcConfigB.implicitlyResolveAssemblies <- true),
            Some(InternalCommandLineOption("--implicitresolution", rangeCmdArgs)),
            None
        )

        // "Display assembly reference resolution information")
        CompilerOption(
            "resolutions",
            tagNone,
            OptionUnit(fun () -> tcConfigB.showReferenceResolutions <- true),
            Some(InternalCommandLineOption("", rangeCmdArgs)),
            None
        )

        // "The base registry key to use for assembly resolution. This part in brackets here: HKEY_LOCAL_MACHINE\[SOFTWARE\Microsoft\.NETFramework]\v2.0.50727\AssemblyFoldersEx")
        CompilerOption(
            "resolutionframeworkregistrybase",
            tagString,
            OptionString(fun _ -> ()),
            Some(InternalCommandLineOption("", rangeCmdArgs)),
            None
        )

        // "The base registry key to use for assembly resolution. This part in brackets here: HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework\v2.0.50727\[AssemblyFoldersEx]")
        CompilerOption(
            "resolutionassemblyfoldersuffix",
            tagString,
            OptionString(fun _ -> ()),
            Some(InternalCommandLineOption("resolutionassemblyfoldersuffix", rangeCmdArgs)),
            None
        )

        // "Additional reference resolution conditions. For example \"OSVersion=5.1.2600.0, PlatformID=id")
        CompilerOption(
            "resolutionassemblyfoldersconditions",
            tagString,
            OptionString(fun _ -> ()),
            Some(InternalCommandLineOption("resolutionassemblyfoldersconditions", rangeCmdArgs)),
            None
        )

        // "Resolve assembly references using MSBuild resolution rules rather than directory based (Default=true except when running fsc.exe under mono)")
        CompilerOption(
            "msbuildresolution",
            tagNone,
            OptionUnit(fun () -> tcConfigB.useSimpleResolution <- false),
            Some(InternalCommandLineOption("msbuildresolution", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "alwayscallvirt",
            tagNone,
            OptionSwitch(callVirtSwitch tcConfigB),
            Some(InternalCommandLineOption("alwayscallvirt", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "nodebugdata",
            tagNone,
            OptionUnit(fun () -> tcConfigB.noDebugAttributes <- true),
            Some(InternalCommandLineOption("nodebugdata", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "parallelreferenceresolution",
            tagNone,
            OptionUnit(fun () -> tcConfigB.parallelReferenceResolution <- ParallelReferenceResolution.On),
            Some(InternalCommandLineOption("--parallelreferenceresolution", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "parallelcompilation",
            tagNone,
            OptionSwitch(callParallelCompilationSwitch tcConfigB),
            Some(InternalCommandLineOption("--parallelcompilation", rangeCmdArgs)),
            None
        )

        testFlag tcConfigB
    ]
    @

    editorSpecificFlags tcConfigB
    @ [
        CompilerOption(
            "jit",
            tagNone,
            OptionSwitch(jitoptimizeSwitch tcConfigB),
            Some(InternalCommandLineOption("jit", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "localoptimize",
            tagNone,
            OptionSwitch(localoptimizeSwitch tcConfigB),
            Some(InternalCommandLineOption("localoptimize", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "splitting",
            tagNone,
            OptionSwitch(splittingSwitch tcConfigB),
            Some(InternalCommandLineOption("splitting", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "versionfile",
            tagString,
            OptionString(fun s -> tcConfigB.version <- VersionFile s),
            Some(InternalCommandLineOption("versionfile", rangeCmdArgs)),
            None
        )

        // "Display timing profiles for compilation"
        CompilerOption(
            "times",
            tagNone,
            OptionUnit(fun () -> tcConfigB.showTimes <- true),
            Some(InternalCommandLineOption("times", rangeCmdArgs)),
            None
        )

        // "Write timing profiles for compilation to a file"
        CompilerOption(
            "times",
            tagFile,
            OptionString(fun s -> tcConfigB.writeTimesToFile <- Some s),
            Some(InternalCommandLineOption("times", rangeCmdArgs)),
            None
        )

#if !NO_TYPEPROVIDERS
        // "Display information about extension type resolution")
        CompilerOption(
            "showextensionresolution",
            tagNone,
            OptionUnit(fun () -> tcConfigB.showExtensionTypeMessages <- true),
            Some(InternalCommandLineOption("showextensionresolution", rangeCmdArgs)),
            None
        )
#endif

        CompilerOption(
            "metadataversion",
            tagString,
            OptionString(fun s -> tcConfigB.metadataVersion <- Some s),
            Some(InternalCommandLineOption("metadataversion", rangeCmdArgs)),
            None
        )
    ]

// OptionBlock: Deprecated flags (fsc, service only)
//--------------------------------------------------

let compilingFsLibFlag (tcConfigB: TcConfigBuilder) =
    CompilerOption(
        "compiling-fslib",
        tagNone,
        OptionUnit(fun () ->
            tcConfigB.compilingFSharpCore <- true
            tcConfigB.TurnWarningOff(rangeStartup, "42")),
        Some(InternalCommandLineOption("--compiling-fslib", rangeCmdArgs)),
        None
    )

let compilingFsLib20Flag =
    CompilerOption(
        "compiling-fslib-20",
        tagNone,
        OptionString(fun _ -> ()),
        Some(DeprecatedCommandLineOptionNoDescription("--compiling-fslib-20", rangeCmdArgs)),
        None
    )

let compilingFsLib40Flag =
    CompilerOption(
        "compiling-fslib-40",
        tagNone,
        OptionUnit(fun () -> ()),
        Some(DeprecatedCommandLineOptionNoDescription("--compiling-fslib-40", rangeCmdArgs)),
        None
    )

let compilingFsLibNoBigIntFlag =
    CompilerOption(
        "compiling-fslib-nobigint",
        tagNone,
        OptionUnit(fun () -> ()),
        Some(DeprecatedCommandLineOptionNoDescription("compiling-fslib-nobigint", rangeCmdArgs)),
        None
    )

let mlKeywordsFlag =
    CompilerOption(
        "ml-keywords",
        tagNone,
        OptionUnit(fun () -> ()),
        Some(DeprecatedCommandLineOptionNoDescription("--ml-keywords", rangeCmdArgs)),
        None
    )

let gnuStyleErrorsFlag tcConfigB =
    CompilerOption(
        "gnu-style-errors",
        tagNone,
        OptionUnit(fun () -> tcConfigB.diagnosticStyle <- DiagnosticStyle.Emacs),
        Some(DeprecatedCommandLineOptionNoDescription("--gnu-style-errors", rangeCmdArgs)),
        None
    )

let deprecatedFlagsBoth tcConfigB =
    [
        CompilerOption(
            "light",
            tagNone,
            OptionUnit(fun () -> tcConfigB.indentationAwareSyntax <- Some true),
            Some(DeprecatedCommandLineOptionNoDescription("--light", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "indentation-syntax",
            tagNone,
            OptionUnit(fun () -> tcConfigB.indentationAwareSyntax <- Some true),
            Some(DeprecatedCommandLineOptionNoDescription("--indentation-syntax", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "no-indentation-syntax",
            tagNone,
            OptionUnit(fun () -> tcConfigB.indentationAwareSyntax <- Some false),
            Some(DeprecatedCommandLineOptionNoDescription("--no-indentation-syntax", rangeCmdArgs)),
            None
        )
    ]

let deprecatedFlagsFsi tcConfigB =
    [ noFrameworkFlag false tcConfigB; yield! deprecatedFlagsBoth tcConfigB ]

let deprecatedFlagsFsc tcConfigB =
    deprecatedFlagsBoth tcConfigB
    @ [
        cliRootFlag tcConfigB
        CompilerOption(
            "jit-optimize",
            tagNone,
            OptionUnit(fun _ ->
                tcConfigB.optSettings <-
                    { tcConfigB.optSettings with
                        jitOptUser = Some true
                    }),
            Some(DeprecatedCommandLineOptionNoDescription("--jit-optimize", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "no-jit-optimize",
            tagNone,
            OptionUnit(fun _ ->
                tcConfigB.optSettings <-
                    { tcConfigB.optSettings with
                        jitOptUser = Some false
                    }),
            Some(DeprecatedCommandLineOptionNoDescription("--no-jit-optimize", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "jit-tracking",
            tagNone,
            OptionUnit(fun _ -> tcConfigB.jitTracking <- true),
            Some(DeprecatedCommandLineOptionNoDescription("--jit-tracking", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "no-jit-tracking",
            tagNone,
            OptionUnit(fun _ -> tcConfigB.jitTracking <- false),
            Some(DeprecatedCommandLineOptionNoDescription("--no-jit-tracking", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "progress",
            tagNone,
            OptionUnit(fun () -> progress <- true),
            Some(DeprecatedCommandLineOptionNoDescription("--progress", rangeCmdArgs)),
            None
        )

        compilingFsLibFlag tcConfigB
        compilingFsLib20Flag
        compilingFsLib40Flag
        compilingFsLibNoBigIntFlag

        CompilerOption(
            "version",
            tagString,
            OptionString(fun s -> tcConfigB.version <- VersionString s),
            Some(DeprecatedCommandLineOptionNoDescription("--version", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "local-optimize",
            tagNone,
            OptionUnit(fun _ ->
                tcConfigB.optSettings <-
                    { tcConfigB.optSettings with
                        localOptUser = Some true
                    }),
            Some(DeprecatedCommandLineOptionNoDescription("--local-optimize", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "no-local-optimize",
            tagNone,
            OptionUnit(fun _ ->
                tcConfigB.optSettings <-
                    { tcConfigB.optSettings with
                        localOptUser = Some false
                    }),
            Some(DeprecatedCommandLineOptionNoDescription("--no-local-optimize", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "cross-optimize",
            tagNone,
            OptionUnit(fun _ ->
                tcConfigB.optSettings <-
                    { tcConfigB.optSettings with
                        crossAssemblyOptimizationUser = Some true
                    }),
            Some(DeprecatedCommandLineOptionNoDescription("--cross-optimize", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "no-cross-optimize",
            tagNone,
            OptionUnit(fun _ ->
                tcConfigB.optSettings <-
                    { tcConfigB.optSettings with
                        crossAssemblyOptimizationUser = Some false
                    }),
            Some(DeprecatedCommandLineOptionNoDescription("--no-cross-optimize", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "no-string-interning",
            tagNone,
            OptionUnit(fun () -> tcConfigB.internConstantStrings <- false),
            Some(DeprecatedCommandLineOptionNoDescription("--no-string-interning", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "statistics",
            tagNone,
            OptionUnit(fun () -> tcConfigB.stats <- true),
            Some(DeprecatedCommandLineOptionNoDescription("--statistics", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "generate-filter-blocks",
            tagNone,
            OptionUnit(fun () -> tcConfigB.generateFilterBlocks <- true),
            Some(DeprecatedCommandLineOptionNoDescription("--generate-filter-blocks", rangeCmdArgs)),
            None
        )

        //CompilerOption
        //    ("no-generate-filter-blocks", tagNone,
        //     OptionUnit (fun () -> tcConfigB.generateFilterBlocks <- false),
        //     Some(DeprecatedCommandLineOptionNoDescription("--generate-filter-blocks", rangeCmdArgs)), None)

        CompilerOption(
            "max-errors",
            tagInt,
            OptionInt(fun n -> tcConfigB.maxErrors <- n),
            Some(DeprecatedCommandLineOptionSuggestAlternative("--max-errors", "--maxerrors", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "debug-file",
            tagNone,
            OptionString(fun s -> tcConfigB.debugSymbolFile <- Some s),
            Some(DeprecatedCommandLineOptionSuggestAlternative("--debug-file", "--pdb", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "no-debug-file",
            tagNone,
            OptionUnit(fun () -> tcConfigB.debuginfo <- false),
            Some(DeprecatedCommandLineOptionSuggestAlternative("--no-debug-file", "--debug-", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "Ooff",
            tagNone,
            OptionUnit(fun () -> SetOptimizeOff tcConfigB),
            Some(DeprecatedCommandLineOptionSuggestAlternative("-Ooff", "--optimize-", rangeCmdArgs)),
            None
        )

        CompilerOption(
            "keycontainer",
            tagString,
            OptionString(fun s ->
                if FSharpEnvironment.isRunningOnCoreClr then
                    error (Error(FSComp.SR.containerSigningUnsupportedOnThisPlatform (), rangeCmdArgs))
                else
                    tcConfigB.container <- Some s),
            (if FSharpEnvironment.isRunningOnCoreClr then
                 None
             else
                 Some(DeprecatedCommandLineOptionSuggestAlternative("--keycontainer", "--keyfile", rangeCmdArgs))),
            None
        )

        mlKeywordsFlag
        gnuStyleErrorsFlag tcConfigB
    ]

// OptionBlock: Miscellaneous options
//-----------------------------------

let GetBannerText tcConfigB =
    if tcConfigB.showBanner then
        $"{tcConfigB.productNameForBannerText}{nl}"
        + $"{FSComp.SR.optsCopyright ()}{nl}"
    else
        ""

/// FSC only help. (FSI has its own help function).
let GetHelpFsc tcConfigB (blocks: CompilerOptionBlock list) =

    GetBannerText tcConfigB + GetCompilerOptionBlocks blocks tcConfigB.bufferWidth

let GetVersion tcConfigB =
    $"{tcConfigB.productNameForBannerText}{nl}"

let miscFlagsBoth tcConfigB =
    [
        CompilerOption("nologo", tagNone, OptionUnit(fun () -> tcConfigB.showBanner <- false), None, Some(FSComp.SR.optsNologo ()))
        CompilerOption(
            "version",
            tagNone,
            OptionConsoleOnly(fun _ ->
                Console.Write(GetVersion tcConfigB)
                tcConfigB.exiter.Exit 0),
            None,
            Some(FSComp.SR.optsVersion ())
        )
    ]

let miscFlagsFsc tcConfigB =
    miscFlagsBoth tcConfigB
    @ [
        CompilerOption(
            "help",
            tagNone,
            OptionConsoleOnly(fun blocks ->
                Console.Write(GetHelpFsc tcConfigB blocks)
                tcConfigB.exiter.Exit 0),
            None,
            Some(FSComp.SR.optsHelp ())
        )
        CompilerOption("@<file>", tagNone, OptionUnit ignore, None, Some(FSComp.SR.optsResponseFile ()))
    ]

let miscFlagsFsi tcConfigB = miscFlagsBoth tcConfigB

// OptionBlock: Abbreviations of existing options
//-----------------------------------------------

let abbreviatedFlagsBoth tcConfigB =
    [
        CompilerOption("d", tagString, OptionString(defineSymbol tcConfigB), None, Some(FSComp.SR.optsShortFormOf ("--define")))
        CompilerOption("O", tagNone, OptionSwitch(SetOptimizeSwitch tcConfigB), None, Some(FSComp.SR.optsShortFormOf ("--optimize[+|-]")))
        CompilerOption("g", tagNone, OptionSwitch(SetDebugSwitch tcConfigB None), None, Some(FSComp.SR.optsShortFormOf ("--debug")))
        CompilerOption(
            "i",
            tagString,
            OptionUnit(fun () -> tcConfigB.printSignature <- true),
            None,
            Some(FSComp.SR.optsShortFormOf ("--sig"))
        )
        CompilerOption(
            "r",
            tagFile,
            OptionString(fun s -> tcConfigB.AddReferencedAssemblyByPath(rangeStartup, s)),
            None,
            Some(FSComp.SR.optsShortFormOf ("--reference"))
        )
        CompilerOption(
            "I",
            tagDirList,
            OptionStringList(fun s -> tcConfigB.AddIncludePath(rangeStartup, s, tcConfigB.implicitIncludeDir)),
            None,
            Some(FSComp.SR.optsShortFormOf ("--lib"))
        )
    ]

let abbreviatedFlagsFsi tcConfigB = abbreviatedFlagsBoth tcConfigB

let abbreviatedFlagsFsc tcConfigB =
    abbreviatedFlagsBoth tcConfigB
    @ [ // FSC only abbreviated options
        CompilerOption("o", tagString, OptionString(setOutFileName tcConfigB), None, Some(FSComp.SR.optsShortFormOf ("--out")))

        CompilerOption(
            "a",
            tagString,
            OptionUnit(fun () -> tcConfigB.target <- CompilerTarget.Dll),
            None,
            Some(FSComp.SR.optsShortFormOf ("--target library"))
        )

        // FSC help abbreviations. FSI has its own help options...
        CompilerOption(
            "?",
            tagNone,
            OptionConsoleOnly(fun blocks ->
                Console.Write(GetHelpFsc tcConfigB blocks)
                tcConfigB.exiter.Exit 0),
            None,
            Some(FSComp.SR.optsShortFormOf ("--help"))
        )

        CompilerOption(
            "help",
            tagNone,
            OptionConsoleOnly(fun blocks ->
                Console.Write(GetHelpFsc tcConfigB blocks)
                tcConfigB.exiter.Exit 0),
            None,
            Some(FSComp.SR.optsShortFormOf ("--help"))
        )

        CompilerOption(
            "full-help",
            tagNone,
            OptionConsoleOnly(fun blocks ->
                Console.Write(GetHelpFsc tcConfigB blocks)
                tcConfigB.exiter.Exit 0),
            None,
            Some(FSComp.SR.optsShortFormOf ("--help"))
        )
    ]

let GetAbbrevFlagSet tcConfigB isFsc =
    let mutable argList: string list = []

    for c in ((if isFsc then abbreviatedFlagsFsc else abbreviatedFlagsFsi) tcConfigB) do
        match c with
        | CompilerOption(arg, _, OptionString _, _, _)
        | CompilerOption(arg, _, OptionStringList _, _, _) -> argList <- argList @ [ "-" + arg; "/" + arg ]
        | _ -> ()

    Set.ofList argList

// check for abbreviated options that accept spaces instead of colons, and replace the spaces
// with colons when necessary
let PostProcessCompilerArgs (abbrevArgs: string Set) (args: string[]) =
    let mutable i = 0
    let mutable idx = 0
    let len = args.Length
    let mutable arga: string[] = Array.create len ""

    while i < len do
        if not (abbrevArgs.Contains(args[i])) || i = (len - 1) then
            arga[idx] <- args[i]
            i <- i + 1
        else
            arga[idx] <- args[i] + ":" + args[i + 1]
            i <- i + 2

        idx <- idx + 1

    Array.toList arga[0 .. (idx - 1)]

// OptionBlock: QA options
//------------------------

let testingAndQAFlags _tcConfigB =
    [
        CompilerOption("dumpAllCommandLineOptions", tagNone, OptionConsoleOnly(DumpCompilerOptionBlocks), None, None) // "Command line options")
    ]

// Core compiler options, overview
//--------------------------------

(*  The "core" compiler options are "the ones defined here".
    Currently, fsi.exe has some additional options, defined in fsi.fs.

    The compiler options are put into blocks, named as <block>Flags.
    Some block options differ between fsc and fsi, in this case they split as <block>FlagsFsc and <block>FlagsFsi.

    The "service.fs" (language service) flags are the same as the fsc flags (except help options are removed).
    REVIEW: is this correct? what about fsx files in VS and fsi options?

    Block                      | notes
    ---------------------------|--------------------
    outputFileFlags            |
    inputFileFlags             |
    resourcesFlags             |
    codeGenerationFlags        |
    errorsAndWarningsFlags     |
    languageFlags              |
    miscFlags                  |
    advancedFlags              |
    internalFlags              |
    abbreviatedFlags           |
    deprecatedFlags            | REVIEW: some of these may have been valid for fsi.exe?
    fsiSpecificFlags           | These are defined later, in fsi.fs
    ---------------------------|--------------------
*)

// Core compiler options exported to fsc.fs, service.fs and fsi.fs
//----------------------------------------------------------------

/// The core/common options used by fsc.exe. [not currently extended by fsc.fs].
let GetCoreFscCompilerOptions (tcConfigB: TcConfigBuilder) =
    [
        PublicOptions(FSComp.SR.optsHelpBannerOutputFiles (), outputFileFlagsFsc tcConfigB)
        PublicOptions(FSComp.SR.optsHelpBannerInputFiles (), inputFileFlagsFsc tcConfigB)
        PublicOptions(FSComp.SR.optsHelpBannerResources (), resourcesFlagsFsc tcConfigB)
        PublicOptions(FSComp.SR.optsHelpBannerCodeGen (), codeGenerationFlags false tcConfigB)
        PublicOptions(FSComp.SR.optsHelpBannerErrsAndWarns (), errorsAndWarningsFlags tcConfigB)
        PublicOptions(FSComp.SR.optsHelpBannerLanguage (), languageFlags tcConfigB)
        PublicOptions(FSComp.SR.optsHelpBannerMisc (), miscFlagsFsc tcConfigB)
        PublicOptions(FSComp.SR.optsHelpBannerAdvanced (), advancedFlagsFsc tcConfigB)
        PrivateOptions(
            List.concat
                [
                    internalFlags tcConfigB
                    abbreviatedFlagsFsc tcConfigB
                    deprecatedFlagsFsc tcConfigB
                    testingAndQAFlags tcConfigB
                ]
        )
    ]

/// The core/common options used by the F# VS Language Service.
/// Filter out OptionConsoleOnly which do printing then exit (e.g --help or --version). This is not wanted in the context of VS!
let GetCoreServiceCompilerOptions (tcConfigB: TcConfigBuilder) =
    let isConsoleOnlyOption =
        function
        | CompilerOption(_, _, OptionConsoleOnly _, _, _) -> true
        | _ -> false

    List.map (FilterCompilerOptionBlock(isConsoleOnlyOption >> not)) (GetCoreFscCompilerOptions tcConfigB)

/// The core/common options used by fsi.exe. [note, some additional options are added in fsi.fs].
let GetCoreFsiCompilerOptions (tcConfigB: TcConfigBuilder) =
    [
        PublicOptions(FSComp.SR.optsHelpBannerOutputFiles (), outputFileFlagsFsi tcConfigB)
        PublicOptions(FSComp.SR.optsHelpBannerInputFiles (), inputFileFlagsFsi tcConfigB)
        PublicOptions(FSComp.SR.optsHelpBannerResources (), resourcesFlagsFsi tcConfigB)
        PublicOptions(FSComp.SR.optsHelpBannerCodeGen (), codeGenerationFlags true tcConfigB)
        PublicOptions(FSComp.SR.optsHelpBannerErrsAndWarns (), errorsAndWarningsFlags tcConfigB)
        PublicOptions(FSComp.SR.optsHelpBannerLanguage (), languageFlags tcConfigB)
        // Note: no HTML block for fsi.exe
        PublicOptions(FSComp.SR.optsHelpBannerMisc (), miscFlagsFsi tcConfigB)
        PublicOptions(FSComp.SR.optsHelpBannerAdvanced (), advancedFlagsFsi tcConfigB)
        PrivateOptions(
            List.concat
                [
                    internalFlags tcConfigB
                    abbreviatedFlagsFsi tcConfigB
                    deprecatedFlagsFsi tcConfigB
                    testingAndQAFlags tcConfigB
                ]
        )
    ]

let CheckAndReportSourceFileDuplicates (sourceFiles: ResizeArray<string>) =
    let visited = Dictionary.newWithSize (sourceFiles.Count * 2)
    let count = sourceFiles.Count

    [
        for i = 0 to (count - 1) do
            let source = sourceFiles[i]

            match visited.TryGetValue source with
            | true, duplicatePosition ->

                warning (Error(FSComp.SR.buildDuplicateFile (source, i + 1, count, duplicatePosition + 1, count), range0))
            | false, _ ->
                visited.Add(source, i)
                yield source
    ]

let ApplyCommandLineArgs (tcConfigB: TcConfigBuilder, sourceFiles: string list, argv) =
    try
        let sourceFilesAcc = ResizeArray sourceFiles

        let collect name =
            if not (FileSystemUtils.isDll name) then
                sourceFilesAcc.Add name

        ParseCompilerOptions(collect, GetCoreServiceCompilerOptions tcConfigB, argv)
        sourceFilesAcc |> CheckAndReportSourceFileDuplicates
    with RecoverableException e ->
        errorRecovery e range0
        sourceFiles

//----------------------------------------------------------------------------
// ReportTime
//----------------------------------------------------------------------------

let private SimulateException simulateConfig =
    match simulateConfig with
    | Some("fsc-oom") -> raise (OutOfMemoryException())
    | Some("fsc-an") -> raise (ArgumentNullException("simulated"))
    | Some("fsc-invop") -> raise (InvalidOperationException())
    | Some("fsc-av") -> raise (AccessViolationException())
    | Some("fsc-aor") -> raise (ArgumentOutOfRangeException())
    | Some("fsc-dv0") -> raise (DivideByZeroException())
    | Some("fsc-nfn") -> raise (NotFiniteNumberException())
    | Some("fsc-oe") -> raise (OverflowException())
    | Some("fsc-atmm") -> raise (ArrayTypeMismatchException())
    | Some("fsc-bif") -> raise (BadImageFormatException())
    | Some("fsc-knf") -> raise (System.Collections.Generic.KeyNotFoundException())
    | Some("fsc-ior") -> raise (IndexOutOfRangeException())
    | Some("fsc-ic") -> raise (InvalidCastException())
    | Some("fsc-ip") -> raise (InvalidProgramException())
    | Some("fsc-ma") -> raise (MemberAccessException())
    | Some("fsc-ni") -> raise (NotImplementedException())
    | Some("fsc-nr") -> raise (NullReferenceException())
    | Some("fsc-oc") -> raise (OperationCanceledException())
    | Some("fsc-fail") -> failwith "simulated"
    | _ -> ()

let ReportTime =
    let mutable nPrev = None

    fun (tcConfig: TcConfig) descr ->
        nPrev
        |> Option.iter (fun (prevDescr, prevAct) ->
            use _ = prevAct

            if tcConfig.pause then
                dprintf "[done '%s', entering '%s'] press <enter> to continue... " prevDescr descr
                Console.ReadLine() |> ignore
            // Intentionally putting this right after the pause so a debugger can be attached.
            SimulateException tcConfig.simulateException)

        if descr <> "Exiting" then
            nPrev <- Some(descr, Activity.Profiling.startAndMeasureEnvironmentStats descr)
        else
            nPrev <- None

let ignoreFailureOnMono1_1_16 f =
    try
        f ()
    with _ ->
        ()

let foreBackColor () =
    try
        let c = Console.ForegroundColor // may fail, perhaps on Mac, and maybe ForegroundColor is Black
        let b = Console.BackgroundColor // may fail, perhaps on Mac, and maybe BackgroundColor is White
        Some(c, b)
    with e ->
        None

let DoWithColor newColor f =
    match enableConsoleColoring, foreBackColor () with
    | false, _
    | true, None ->
        // could not get console colours, so no attempt to change colours, cannot set them back
        f ()
    | true, Some(c, _) ->
        try
            ignoreFailureOnMono1_1_16 (fun () -> Console.ForegroundColor <- newColor)
            f ()
        finally
            ignoreFailureOnMono1_1_16 (fun () -> Console.ForegroundColor <- c)

let DoWithDiagnosticColor severity f =
    match foreBackColor () with
    | None -> f ()
    | Some(_, backColor) ->
        let infoColor =
            if backColor = ConsoleColor.White then
                ConsoleColor.Blue
            else
                ConsoleColor.Green

        let warnColor =
            if backColor = ConsoleColor.White then
                ConsoleColor.DarkBlue
            else
                ConsoleColor.Cyan

        let errorColor = ConsoleColor.Red

        let color =
            match severity with
            | FSharpDiagnosticSeverity.Error -> errorColor
            | FSharpDiagnosticSeverity.Warning -> warnColor
            | _ -> infoColor

        DoWithColor color f
