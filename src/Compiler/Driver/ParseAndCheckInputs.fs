// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// Contains logic to coordinate the parsing and checking of one or a group of files
module internal FSharp.Compiler.ParseAndCheckInputs

open System
open System.Diagnostics
open System.IO
open System.Threading
open System.Collections.Generic

open Internal.Utilities.Collections
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras
open Internal.Utilities.Text.Lexing

open FSharp.Compiler
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.CheckBasics
open FSharp.Compiler.CheckDeclarations
open FSharp.Compiler.CompilerGlobalState
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerDiagnostics
open FSharp.Compiler.CompilerImports
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Features
open FSharp.Compiler.IO
open FSharp.Compiler.Lexhelp
open FSharp.Compiler.NameResolution
open FSharp.Compiler.ParseHelpers
open FSharp.Compiler.Syntax
open FSharp.Compiler.SyntaxTrivia
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Position
open FSharp.Compiler.Text.Range
open FSharp.Compiler.Xml
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TcGlobals

let CanonicalizeFilename fileName =
    let basic = FileSystemUtils.fileNameOfPath fileName

    String.capitalize (
        try
            FileSystemUtils.chopExtension basic
        with _ ->
            basic
    )

let IsScript fileName =
    FSharpScriptFileSuffixes |> List.exists (FileSystemUtils.checkSuffix fileName)

let IsMLCompatFile fileName =
    FSharpMLCompatFileSuffixes |> List.exists (FileSystemUtils.checkSuffix fileName)

// Give a unique name to the different kinds of inputs. Used to correlate signature and implementation files
//   QualFileNameOfModuleName - files with a single module declaration or an anonymous module
let QualFileNameOfModuleName m fileName modname =
    QualifiedNameOfFile(mkSynId m (textOfLid modname + (if IsScript fileName then "$fsx" else "")))

let QualFileNameOfFilename m fileName =
    QualifiedNameOfFile(mkSynId m (CanonicalizeFilename fileName + (if IsScript fileName then "$fsx" else "")))

// Interactive fragments
let ComputeQualifiedNameOfFileFromUniquePath (m, p: string list) =
    QualifiedNameOfFile(mkSynId m (String.concat "_" p))

let QualFileNameOfSpecs fileName specs =
    match specs with
    | [ SynModuleOrNamespaceSig (longId = modname; kind = kind; range = m) ] when kind.IsModule ->
        QualFileNameOfModuleName m fileName modname
    | [ SynModuleOrNamespaceSig (kind = kind; range = m) ] when not kind.IsModule -> QualFileNameOfFilename m fileName
    | _ -> QualFileNameOfFilename (mkRange fileName pos0 pos0) fileName

let QualFileNameOfImpls fileName specs =
    match specs with
    | [ SynModuleOrNamespace (longId = modname; kind = kind; range = m) ] when kind.IsModule -> QualFileNameOfModuleName m fileName modname
    | [ SynModuleOrNamespace (kind = kind; range = m) ] when not kind.IsModule -> QualFileNameOfFilename m fileName
    | _ -> QualFileNameOfFilename (mkRange fileName pos0 pos0) fileName

let PrependPathToQualFileName x (QualifiedNameOfFile q) =
    ComputeQualifiedNameOfFileFromUniquePath(q.idRange, pathOfLid x @ [ q.idText ])

let PrependPathToImpl x (SynModuleOrNamespace (longId, isRecursive, kind, decls, xmlDoc, attribs, accessibility, range, trivia)) =
    SynModuleOrNamespace(x @ longId, isRecursive, kind, decls, xmlDoc, attribs, accessibility, range, trivia)

let PrependPathToSpec x (SynModuleOrNamespaceSig (longId, isRecursive, kind, decls, xmlDoc, attribs, accessibility, range, trivia)) =
    SynModuleOrNamespaceSig(x @ longId, isRecursive, kind, decls, xmlDoc, attribs, accessibility, range, trivia)

let PrependPathToInput x inp =
    match inp with
    | ParsedInput.ImplFile (ParsedImplFileInput (b, c, q, d, hd, impls, e, trivia, i)) ->
        ParsedInput.ImplFile(
            ParsedImplFileInput(b, c, PrependPathToQualFileName x q, d, hd, List.map (PrependPathToImpl x) impls, e, trivia, i)
        )

    | ParsedInput.SigFile (ParsedSigFileInput (b, q, d, hd, specs, trivia, i)) ->
        ParsedInput.SigFile(ParsedSigFileInput(b, PrependPathToQualFileName x q, d, hd, List.map (PrependPathToSpec x) specs, trivia, i))

let IsValidAnonModuleName (modname: string) =
    modname |> String.forall (fun c -> Char.IsLetterOrDigit c || c = '_')

let ComputeAnonModuleName check defaultNamespace fileName (m: range) =
    let modname = CanonicalizeFilename fileName

    if check && not (IsValidAnonModuleName modname) && not (IsScript fileName) then
        warning (Error(FSComp.SR.buildImplicitModuleIsNotLegalIdentifier (modname, (FileSystemUtils.fileNameOfPath fileName)), m))

    let combined =
        match defaultNamespace with
        | None -> modname
        | Some ns -> textOfPath [ ns; modname ]

    let anonymousModuleNameRange =
        let fileName = m.FileName
        mkRange fileName pos0 pos0

    pathToSynLid anonymousModuleNameRange (splitNamespace combined)

let FileRequiresModuleOrNamespaceDecl isLast isExe fileName =
    not (isLast && isExe) && not (IsScript fileName || IsMLCompatFile fileName)

let PostParseModuleImpl (_i, defaultNamespace, isLastCompiland, fileName, impl) =
    match impl with
    | ParsedImplFileFragment.NamedModule (SynModuleOrNamespace (lid, isRec, kind, decls, xmlDoc, attribs, access, m, trivia)) ->
        let lid =
            match lid with
            | [ id ] when kind.IsModule && id.idText = MangledGlobalName ->
                error (Error(FSComp.SR.buildInvalidModuleOrNamespaceName (), id.idRange))
            | id :: rest when id.idText = MangledGlobalName -> rest
            | _ -> lid

        SynModuleOrNamespace(lid, isRec, kind, decls, xmlDoc, attribs, access, m, trivia)

    | ParsedImplFileFragment.AnonModule (defs, m) ->
        let isLast, isExe = isLastCompiland

        if FileRequiresModuleOrNamespaceDecl isLast isExe fileName then
            match defs with
            | SynModuleDecl.NestedModule _ :: _ -> errorR (Error(FSComp.SR.noEqualSignAfterModule (), trimRangeToLine m))
            | _ -> errorR (Error(FSComp.SR.buildMultiFileRequiresNamespaceOrModule (), trimRangeToLine m))

        let modname =
            ComputeAnonModuleName (not (isNil defs)) defaultNamespace fileName (trimRangeToLine m)

        let trivia: SynModuleOrNamespaceTrivia =
            {
                LeadingKeyword = SynModuleOrNamespaceLeadingKeyword.None
            }

        SynModuleOrNamespace(modname, false, SynModuleOrNamespaceKind.AnonModule, defs, PreXmlDoc.Empty, [], None, m, trivia)

    | ParsedImplFileFragment.NamespaceFragment (lid, isRecursive, kind, decls, xmlDoc, attributes, range, trivia) ->
        let lid, kind =
            match lid with
            | id :: rest when id.idText = MangledGlobalName ->
                let kind =
                    if rest.IsEmpty then
                        SynModuleOrNamespaceKind.GlobalNamespace
                    else
                        kind

                rest, kind
            | _ -> lid, kind

        SynModuleOrNamespace(lid, isRecursive, kind, decls, xmlDoc, attributes, None, range, trivia)

let PostParseModuleSpec (_i, defaultNamespace, isLastCompiland, fileName, intf) =
    match intf with
    | ParsedSigFileFragment.NamedModule (SynModuleOrNamespaceSig (lid, isRec, kind, decls, xmlDoc, attribs, access, m, trivia)) ->
        let lid =
            match lid with
            | [ id ] when kind.IsModule && id.idText = MangledGlobalName ->
                error (Error(FSComp.SR.buildInvalidModuleOrNamespaceName (), id.idRange))
            | id :: rest when id.idText = MangledGlobalName -> rest
            | _ -> lid

        SynModuleOrNamespaceSig(lid, isRec, SynModuleOrNamespaceKind.NamedModule, decls, xmlDoc, attribs, access, m, trivia)

    | ParsedSigFileFragment.AnonModule (defs, m) ->
        let isLast, isExe = isLastCompiland

        if FileRequiresModuleOrNamespaceDecl isLast isExe fileName then
            match defs with
            | SynModuleSigDecl.NestedModule _ :: _ -> errorR (Error(FSComp.SR.noEqualSignAfterModule (), m))
            | _ -> errorR (Error(FSComp.SR.buildMultiFileRequiresNamespaceOrModule (), m))

        let modname =
            ComputeAnonModuleName (not (isNil defs)) defaultNamespace fileName (trimRangeToLine m)

        let trivia: SynModuleOrNamespaceSigTrivia =
            {
                LeadingKeyword = SynModuleOrNamespaceLeadingKeyword.None
            }

        SynModuleOrNamespaceSig(modname, false, SynModuleOrNamespaceKind.AnonModule, defs, PreXmlDoc.Empty, [], None, m, trivia)

    | ParsedSigFileFragment.NamespaceFragment (lid, isRecursive, kind, decls, xmlDoc, attributes, range, trivia) ->
        let lid, kind =
            match lid with
            | id :: rest when id.idText = MangledGlobalName ->
                let kind =
                    if rest.IsEmpty then
                        SynModuleOrNamespaceKind.GlobalNamespace
                    else
                        kind

                rest, kind
            | _ -> lid, kind

        SynModuleOrNamespaceSig(lid, isRecursive, kind, decls, xmlDoc, attributes, None, range, trivia)

let GetScopedPragmasForHashDirective hd =
    [
        match hd with
        | ParsedHashDirective ("nowarn", numbers, m) ->
            for s in numbers do
                match s with
                | ParsedHashDirectiveArgument.SourceIdentifier _ -> ()
                | ParsedHashDirectiveArgument.String (s, _, _) ->
                    match GetWarningNumber(m, s) with
                    | None -> ()
                    | Some n -> ScopedPragma.WarningOff(m, n)
        | _ -> ()
    ]

let private collectCodeComments (lexbuf: UnicodeLexing.Lexbuf) (tripleSlashComments: range list) =
    [
        yield! LexbufCommentStore.GetComments(lexbuf)
        yield! (List.map CommentTrivia.LineComment tripleSlashComments)
    ]
    |> List.sortBy (function
        | CommentTrivia.LineComment r
        | CommentTrivia.BlockComment r -> r.StartLine, r.StartColumn)

let PostParseModuleImpls
    (
        defaultNamespace,
        fileName,
        isLastCompiland,
        ParsedImplFile (hashDirectives, impls),
        lexbuf: UnicodeLexing.Lexbuf,
        tripleSlashComments: range list,
        identifiers: Set<string>
    ) =
    let othersWithSameName =
        impls
        |> List.rev
        |> List.tryPick (function
            | ParsedImplFileFragment.NamedModule (SynModuleOrNamespace (longId = lid)) -> Some lid
            | _ -> None)

    match othersWithSameName with
    | Some lid when impls.Length > 1 -> errorR (Error(FSComp.SR.buildMultipleToplevelModules (), rangeOfLid lid))
    | _ -> ()

    let impls =
        impls
        |> List.mapi (fun i x -> PostParseModuleImpl(i, defaultNamespace, isLastCompiland, fileName, x))

    let qualName = QualFileNameOfImpls fileName impls
    let isScript = IsScript fileName

    let scopedPragmas =
        [
            for SynModuleOrNamespace (decls = decls) in impls do
                for d in decls do
                    match d with
                    | SynModuleDecl.HashDirective (hd, _) -> yield! GetScopedPragmasForHashDirective hd
                    | _ -> ()
            for hd in hashDirectives do
                yield! GetScopedPragmasForHashDirective hd
        ]

    let conditionalDirectives = LexbufIfdefStore.GetTrivia(lexbuf)
    let codeComments = collectCodeComments lexbuf tripleSlashComments

    let trivia: ParsedImplFileInputTrivia =
        {
            ConditionalDirectives = conditionalDirectives
            CodeComments = codeComments
        }

    ParsedInput.ImplFile(
        ParsedImplFileInput(fileName, isScript, qualName, scopedPragmas, hashDirectives, impls, isLastCompiland, trivia, identifiers)
    )

let PostParseModuleSpecs
    (
        defaultNamespace,
        fileName,
        isLastCompiland,
        ParsedSigFile (hashDirectives, specs),
        lexbuf: UnicodeLexing.Lexbuf,
        tripleSlashComments: range list,
        identifiers: Set<string>
    ) =
    let othersWithSameName =
        specs
        |> List.rev
        |> List.tryPick (function
            | ParsedSigFileFragment.NamedModule (SynModuleOrNamespaceSig (longId = lid)) -> Some lid
            | _ -> None)

    match othersWithSameName with
    | Some lid when specs.Length > 1 -> errorR (Error(FSComp.SR.buildMultipleToplevelModules (), rangeOfLid lid))
    | _ -> ()

    let specs =
        specs
        |> List.mapi (fun i x -> PostParseModuleSpec(i, defaultNamespace, isLastCompiland, fileName, x))

    let qualName = QualFileNameOfSpecs fileName specs

    let scopedPragmas =
        [
            for SynModuleOrNamespaceSig (decls = decls) in specs do
                for d in decls do
                    match d with
                    | SynModuleSigDecl.HashDirective (hd, _) -> yield! GetScopedPragmasForHashDirective hd
                    | _ -> ()
            for hd in hashDirectives do
                yield! GetScopedPragmasForHashDirective hd
        ]

    let conditionalDirectives = LexbufIfdefStore.GetTrivia(lexbuf)
    let codeComments = collectCodeComments lexbuf tripleSlashComments

    let trivia: ParsedSigFileInputTrivia =
        {
            ConditionalDirectives = conditionalDirectives
            CodeComments = codeComments
        }

    ParsedInput.SigFile(ParsedSigFileInput(fileName, qualName, scopedPragmas, hashDirectives, specs, trivia, identifiers))

type ModuleNamesDict = Map<string, Map<string, QualifiedNameOfFile>>

/// Checks if a module name is already given and deduplicates the name if needed.
let DeduplicateModuleName (moduleNamesDict: ModuleNamesDict) fileName (qualNameOfFile: QualifiedNameOfFile) =
    let path = Path.GetDirectoryName fileName

    let path =
        if FileSystem.IsPathRootedShim path then
            try
                FileSystem.GetFullPathShim path
            with _ ->
                path
        else
            path

    match moduleNamesDict.TryGetValue qualNameOfFile.Text with
    | true, paths ->
        if paths.ContainsKey path then
            paths[path], moduleNamesDict
        else
            let count = paths.Count + 1
            let id = qualNameOfFile.Id

            let qualNameOfFileT =
                if count = 1 then
                    qualNameOfFile
                else
                    QualifiedNameOfFile(Ident(id.idText + "___" + count.ToString(), id.idRange))

            let moduleNamesDictT =
                moduleNamesDict.Add(qualNameOfFile.Text, paths.Add(path, qualNameOfFileT))

            qualNameOfFileT, moduleNamesDictT
    | _ ->
        let moduleNamesDictT =
            moduleNamesDict.Add(qualNameOfFile.Text, Map.empty.Add(path, qualNameOfFile))

        qualNameOfFile, moduleNamesDictT

/// Checks if a ParsedInput is using a module name that was already given and deduplicates the name if needed.
let DeduplicateParsedInputModuleName (moduleNamesDict: ModuleNamesDict) input =
    match input with
    | ParsedInput.ImplFile implFile ->
        let (ParsedImplFileInput (fileName, isScript, qualNameOfFile, scopedPragmas, hashDirectives, modules, flags, trivia, identifiers)) =
            implFile

        let qualNameOfFileR, moduleNamesDictR =
            DeduplicateModuleName moduleNamesDict fileName qualNameOfFile

        let implFileR =
            ParsedImplFileInput(fileName, isScript, qualNameOfFileR, scopedPragmas, hashDirectives, modules, flags, trivia, identifiers)

        let inputR = ParsedInput.ImplFile implFileR
        inputR, moduleNamesDictR
    | ParsedInput.SigFile sigFile ->
        let (ParsedSigFileInput (fileName, qualNameOfFile, scopedPragmas, hashDirectives, modules, trivia, identifiers)) =
            sigFile

        let qualNameOfFileR, moduleNamesDictR =
            DeduplicateModuleName moduleNamesDict fileName qualNameOfFile

        let sigFileR =
            ParsedSigFileInput(fileName, qualNameOfFileR, scopedPragmas, hashDirectives, modules, trivia, identifiers)

        let inputT = ParsedInput.SigFile sigFileR
        inputT, moduleNamesDictR

let ParseInput
    (
        lexer,
        diagnosticOptions: FSharpDiagnosticOptions,
        diagnosticsLogger: DiagnosticsLogger,
        lexbuf: UnicodeLexing.Lexbuf,
        defaultNamespace,
        fileName,
        isLastCompiland,
        identCapture,
        userOpName
    ) =

    use _ =
        Activity.start
            "ParseAndCheckFile.parseFile"
            [|
                Activity.Tags.fileName, fileName
                Activity.Tags.buildPhase, BuildPhase.Parse.ToString()
                Activity.Tags.userOpName, userOpName |> Option.defaultValue ""
            |]

    // The assert below is almost ok, but it fires in two cases:
    //  - fsi.exe sometimes passes "stdin" as a dummy file name
    //  - if you have a #line directive, e.g.
    //        # 1000 "Line01.fs"
    //    then it also asserts. But these are edge cases that can be fixed later, e.g. in bug 4651.

    // Delay sending errors and warnings until after the file is parsed. This gives us a chance to scrape the
    // #nowarn declarations for the file
    let delayLogger = CapturingDiagnosticsLogger("Parsing")
    use _ = UseDiagnosticsLogger delayLogger
    use _ = UseBuildPhase BuildPhase.Parse

    let mutable scopedPragmas = []

    try
        let input =
            let identStore = HashSet<string>()

            let lexer =
                if identCapture then
                    (fun x ->
                        let token = lexer x

                        match token with
                        | Parser.token.PERCENT_OP ident
                        | Parser.token.FUNKY_OPERATOR_NAME ident
                        | Parser.token.ADJACENT_PREFIX_OP ident
                        | Parser.token.PLUS_MINUS_OP ident
                        | Parser.token.INFIX_AMP_OP ident
                        | Parser.token.INFIX_STAR_DIV_MOD_OP ident
                        | Parser.token.PREFIX_OP ident
                        | Parser.token.INFIX_BAR_OP ident
                        | Parser.token.INFIX_AT_HAT_OP ident
                        | Parser.token.INFIX_COMPARE_OP ident
                        | Parser.token.INFIX_STAR_STAR_OP ident
                        | Parser.token.IDENT ident -> identStore.Add ident |> ignore
                        | _ -> ()

                        token)
                else
                    lexer

            if FSharpMLCompatFileSuffixes |> List.exists (FileSystemUtils.checkSuffix fileName) then
                if lexbuf.SupportsFeature LanguageFeature.MLCompatRevisions then
                    errorR (Error(FSComp.SR.buildInvalidSourceFileExtensionML fileName, rangeStartup))
                else
                    mlCompatWarning (FSComp.SR.buildCompilingExtensionIsForML ()) rangeStartup

            // Call the appropriate parser - for signature files or implementation files
            if FSharpImplFileSuffixes |> List.exists (FileSystemUtils.checkSuffix fileName) then
                let impl = Parser.implementationFile lexer lexbuf

                let tripleSlashComments =
                    LexbufLocalXmlDocStore.ReportInvalidXmlDocPositions(lexbuf)

                PostParseModuleImpls(defaultNamespace, fileName, isLastCompiland, impl, lexbuf, tripleSlashComments, Set identStore)
            elif FSharpSigFileSuffixes |> List.exists (FileSystemUtils.checkSuffix fileName) then
                let intfs = Parser.signatureFile lexer lexbuf

                let tripleSlashComments =
                    LexbufLocalXmlDocStore.ReportInvalidXmlDocPositions(lexbuf)

                PostParseModuleSpecs(defaultNamespace, fileName, isLastCompiland, intfs, lexbuf, tripleSlashComments, Set identStore)
            else if lexbuf.SupportsFeature LanguageFeature.MLCompatRevisions then
                error (Error(FSComp.SR.buildInvalidSourceFileExtensionUpdated fileName, rangeStartup))
            else
                error (Error(FSComp.SR.buildInvalidSourceFileExtension fileName, rangeStartup))

        scopedPragmas <- input.ScopedPragmas
        input
    finally
        // OK, now commit the errors, since the ScopedPragmas will (hopefully) have been scraped
        let filteringDiagnosticsLogger =
            GetDiagnosticsLoggerFilteringByScopedPragmas(false, scopedPragmas, diagnosticOptions, diagnosticsLogger)

        delayLogger.CommitDelayedDiagnostics filteringDiagnosticsLogger

type Tokenizer = unit -> Parser.token

// Show all tokens in the stream, for testing purposes
let ShowAllTokensAndExit (shortFilename, tokenizer: Tokenizer, lexbuf: LexBuffer<char>, exiter: Exiter) =
    while true do
        printf "tokenize - getting one token from %s\n" shortFilename
        let t = tokenizer ()
        printf "tokenize - got %s @ %a\n" (Parser.token_to_string t) outputRange lexbuf.LexemeRange

        match t with
        | Parser.EOF _ -> exiter.Exit 0
        | _ -> ()

        if lexbuf.IsPastEndOfStream then
            printf "!!! at end of stream\n"

// Test one of the parser entry points, just for testing purposes
let TestInteractionParserAndExit (tokenizer: Tokenizer, lexbuf: LexBuffer<char>, exiter: Exiter) =
    while true do
        match (Parser.interaction (fun _ -> tokenizer ()) lexbuf) with
        | ParsedScriptInteraction.Definitions (l, m) -> printfn "Parsed OK, got %d defs @ %a" l.Length outputRange m

    exiter.Exit 0

// Report the statistics for testing purposes
let ReportParsingStatistics res =
    let rec flattenSpecs specs =
        specs
        |> List.collect (function
            | SynModuleSigDecl.NestedModule (moduleDecls = subDecls) -> flattenSpecs subDecls
            | spec -> [ spec ])

    let rec flattenDefns specs =
        specs
        |> List.collect (function
            | SynModuleDecl.NestedModule (decls = subDecls) -> flattenDefns subDecls
            | defn -> [ defn ])

    let flattenModSpec (SynModuleOrNamespaceSig (decls = decls)) = flattenSpecs decls
    let flattenModImpl (SynModuleOrNamespace (decls = decls)) = flattenDefns decls

    match res with
    | ParsedInput.SigFile sigFile -> printfn "parsing yielded %d specs" (List.collect flattenModSpec sigFile.Contents).Length
    | ParsedInput.ImplFile implFile -> printfn "parsing yielded %d definitions" (List.collect flattenModImpl implFile.Contents).Length

let EmptyParsedInput (fileName, isLastCompiland) =
    if FSharpSigFileSuffixes |> List.exists (FileSystemUtils.checkSuffix fileName) then
        ParsedInput.SigFile(
            ParsedSigFileInput(
                fileName,
                QualFileNameOfImpls fileName [],
                [],
                [],
                [],
                {
                    ConditionalDirectives = []
                    CodeComments = []
                },
                Set.empty
            )
        )
    else
        ParsedInput.ImplFile(
            ParsedImplFileInput(
                fileName,
                false,
                QualFileNameOfImpls fileName [],
                [],
                [],
                [],
                isLastCompiland,
                {
                    ConditionalDirectives = []
                    CodeComments = []
                },
                Set.empty
            )
        )

/// Parse an input, drawing tokens from the LexBuffer
let ParseOneInputLexbuf (tcConfig: TcConfig, lexResourceManager, lexbuf, fileName, isLastCompiland, diagnosticsLogger) =
    use unwindbuildphase = UseBuildPhase BuildPhase.Parse

    try

        // Don't report whitespace from lexer
        let skipWhitespaceTokens = true

        // Set up the initial status for indentation-aware processing
        let indentationSyntaxStatus =
            IndentationAwareSyntaxStatus(tcConfig.ComputeIndentationAwareSyntaxInitialStatus fileName, true)

        // Set up the initial lexer arguments
        let lexargs =
            mkLexargs (
                tcConfig.conditionalDefines,
                indentationSyntaxStatus,
                lexResourceManager,
                [],
                diagnosticsLogger,
                tcConfig.pathMap,
                tcConfig.applyLineDirectives
            )

        // Set up the initial lexer arguments
        let shortFilename = SanitizeFileName fileName tcConfig.implicitIncludeDir

        let input =
            usingLexbufForParsing (lexbuf, fileName) (fun lexbuf ->

                // Set up the LexFilter over the token stream
                let tokenizer, tokenizeOnly =
                    match tcConfig.tokenize with
                    | TokenizeOption.Unfiltered -> (fun () -> Lexer.token lexargs skipWhitespaceTokens lexbuf), true
                    | TokenizeOption.Only ->
                        LexFilter
                            .LexFilter(
                                indentationSyntaxStatus,
                                tcConfig.compilingFSharpCore,
                                Lexer.token lexargs skipWhitespaceTokens,
                                lexbuf
                            )
                            .GetToken,
                        true
                    | _ ->
                        LexFilter
                            .LexFilter(
                                indentationSyntaxStatus,
                                tcConfig.compilingFSharpCore,
                                Lexer.token lexargs skipWhitespaceTokens,
                                lexbuf
                            )
                            .GetToken,
                        false

                // If '--tokenize' then show the tokens now and exit
                if tokenizeOnly then
                    ShowAllTokensAndExit(shortFilename, tokenizer, lexbuf, tcConfig.exiter)

                // Test hook for one of the parser entry points
                if tcConfig.testInteractionParser then
                    TestInteractionParserAndExit(tokenizer, lexbuf, tcConfig.exiter)

                // Parse the input
                let res =
                    ParseInput(
                        (fun _ -> tokenizer ()),
                        tcConfig.diagnosticsOptions,
                        diagnosticsLogger,
                        lexbuf,
                        None,
                        fileName,
                        isLastCompiland,
                        tcConfig.captureIdentifiersWhenParsing,
                        None
                    )

                // Report the statistics for testing purposes
                if tcConfig.reportNumDecls then
                    ReportParsingStatistics res

                res)

        input

    with exn ->
        errorRecovery exn rangeStartup
        EmptyParsedInput(fileName, isLastCompiland)

let ValidSuffixes = FSharpSigFileSuffixes @ FSharpImplFileSuffixes

let checkInputFile (tcConfig: TcConfig) fileName =
    if List.exists (FileSystemUtils.checkSuffix fileName) ValidSuffixes then
        if not (FileSystem.FileExistsShim fileName) then
            error (Error(FSComp.SR.buildCouldNotFindSourceFile fileName, rangeStartup))
    else
        error (Error(FSComp.SR.buildInvalidSourceFileExtension (SanitizeFileName fileName tcConfig.implicitIncludeDir), rangeStartup))

let parseInputStreamAux
    (
        tcConfig: TcConfig,
        lexResourceManager,
        fileName,
        isLastCompiland,
        diagnosticsLogger,
        retryLocked,
        stream: Stream
    ) =
    use reader = stream.GetReader(tcConfig.inputCodePage, retryLocked)

    // Set up the LexBuffer for the file
    let lexbuf =
        UnicodeLexing.StreamReaderAsLexbuf(not tcConfig.compilingFSharpCore, tcConfig.langVersion, reader)

    // Parse the file drawing tokens from the lexbuf
    ParseOneInputLexbuf(tcConfig, lexResourceManager, lexbuf, fileName, isLastCompiland, diagnosticsLogger)

let parseInputSourceTextAux
    (
        tcConfig: TcConfig,
        lexResourceManager,
        fileName,
        isLastCompiland,
        diagnosticsLogger,
        sourceText: ISourceText
    ) =
    // Set up the LexBuffer for the file
    let lexbuf =
        UnicodeLexing.SourceTextAsLexbuf(not tcConfig.compilingFSharpCore, tcConfig.langVersion, sourceText)

    // Parse the file drawing tokens from the lexbuf
    ParseOneInputLexbuf(tcConfig, lexResourceManager, lexbuf, fileName, isLastCompiland, diagnosticsLogger)

let parseInputFileAux (tcConfig: TcConfig, lexResourceManager, fileName, isLastCompiland, diagnosticsLogger, retryLocked) =
    // Get a stream reader for the file
    use fileStream = FileSystem.OpenFileForReadShim(fileName)
    use reader = fileStream.GetReader(tcConfig.inputCodePage, retryLocked)

    // Set up the LexBuffer for the file
    let lexbuf =
        UnicodeLexing.StreamReaderAsLexbuf(not tcConfig.compilingFSharpCore, tcConfig.langVersion, reader)

    // Parse the file drawing tokens from the lexbuf
    ParseOneInputLexbuf(tcConfig, lexResourceManager, lexbuf, fileName, isLastCompiland, diagnosticsLogger)

/// Parse an input from stream
let ParseOneInputStream
    (
        tcConfig: TcConfig,
        lexResourceManager,
        fileName,
        isLastCompiland,
        diagnosticsLogger,
        retryLocked,
        stream: Stream
    ) =
    try
        parseInputStreamAux (tcConfig, lexResourceManager, fileName, isLastCompiland, diagnosticsLogger, retryLocked, stream)
    with exn ->
        errorRecovery exn rangeStartup
        EmptyParsedInput(fileName, isLastCompiland)

/// Parse an input from source text
let ParseOneInputSourceText
    (
        tcConfig: TcConfig,
        lexResourceManager,
        fileName,
        isLastCompiland,
        diagnosticsLogger,
        sourceText: ISourceText
    ) =
    try
        parseInputSourceTextAux (tcConfig, lexResourceManager, fileName, isLastCompiland, diagnosticsLogger, sourceText)
    with exn ->
        errorRecovery exn rangeStartup
        EmptyParsedInput(fileName, isLastCompiland)

/// Parse an input from disk
let ParseOneInputFile (tcConfig: TcConfig, lexResourceManager, fileName, isLastCompiland, diagnosticsLogger, retryLocked) =
    try
        checkInputFile tcConfig fileName
        parseInputFileAux (tcConfig, lexResourceManager, fileName, isLastCompiland, diagnosticsLogger, retryLocked)
    with exn ->
        errorRecovery exn rangeStartup
        EmptyParsedInput(fileName, isLastCompiland)

/// Prepare to process inputs independently, e.g. partially in parallel.
///
/// To do this we create one CapturingDiagnosticLogger for each input and
/// then ensure the diagnostics are presented in deterministic order after processing completes.
/// On completion all diagnostics are forwarded to the DiagnosticLogger given as input.
///
/// NOTE: Max errors is currently counted separately for each logger. When max errors is reached on one compilation
/// the given Exiter will be called.
///
/// NOTE: this needs to be improved to commit diagnotics as soon as possible
///
/// NOTE: If StopProcessing is raised by any piece of work then the overall function raises StopProcessing.
let UseMultipleDiagnosticLoggers (inputs, diagnosticsLogger, eagerFormat) f =

    // Check input files and create delayed error loggers before we try to parallel parse.
    let delayLoggers =
        inputs
        |> List.map (fun _ -> CapturingDiagnosticsLogger("TcDiagnosticsLogger", ?eagerFormat = eagerFormat))

    try
        f (List.zip inputs delayLoggers)
    finally
        for logger in delayLoggers do
            logger.CommitDelayedDiagnostics diagnosticsLogger

let ParseInputFilesInParallel (tcConfig: TcConfig, lexResourceManager, sourceFiles, delayLogger: DiagnosticsLogger, retryLocked) =

    let isLastCompiland, isExe = sourceFiles |> tcConfig.ComputeCanContainEntryPoint

    for fileName in sourceFiles do
        checkInputFile tcConfig fileName

    let sourceFiles = List.zip sourceFiles isLastCompiland

    UseMultipleDiagnosticLoggers (sourceFiles, delayLogger, None) (fun sourceFilesWithDelayLoggers ->
        sourceFilesWithDelayLoggers
        |> ListParallel.map (fun ((fileName, isLastCompiland), delayLogger) ->
            let directoryName = Path.GetDirectoryName fileName

            let input =
                parseInputFileAux (tcConfig, lexResourceManager, fileName, (isLastCompiland, isExe), delayLogger, retryLocked)

            (input, directoryName)))

let ParseInputFilesSequential (tcConfig: TcConfig, lexResourceManager, sourceFiles, diagnosticsLogger: DiagnosticsLogger, retryLocked) =
    let isLastCompiland, isExe = sourceFiles |> tcConfig.ComputeCanContainEntryPoint
    let sourceFiles = isLastCompiland |> List.zip sourceFiles |> Array.ofList

    sourceFiles
    |> Array.map (fun (fileName, isLastCompiland) ->
        let directoryName = Path.GetDirectoryName fileName

        let input =
            ParseOneInputFile(tcConfig, lexResourceManager, fileName, (isLastCompiland, isExe), diagnosticsLogger, retryLocked)

        (input, directoryName))
    |> List.ofArray

/// Parse multiple input files from disk
let ParseInputFiles (tcConfig: TcConfig, lexResourceManager, sourceFiles, diagnosticsLogger: DiagnosticsLogger, retryLocked) =
    try
        if tcConfig.concurrentBuild then
            ParseInputFilesInParallel(tcConfig, lexResourceManager, sourceFiles, diagnosticsLogger, retryLocked)
        else
            ParseInputFilesSequential(tcConfig, lexResourceManager, sourceFiles, diagnosticsLogger, retryLocked)

    with e ->
        errorRecoveryNoRange e
        tcConfig.exiter.Exit 1

let ProcessMetaCommandsFromInput
    (nowarnF: 'state -> range * string -> 'state,
     hashReferenceF: 'state -> range * string * Directive -> 'state,
     loadSourceF: 'state -> range * string -> unit)
    (tcConfig: TcConfigBuilder, inp: ParsedInput, pathOfMetaCommandSource, state0)
    =

    use _ = UseBuildPhase BuildPhase.Parse

    let canHaveScriptMetaCommands =
        match inp with
        | ParsedInput.SigFile _ -> false
        | ParsedInput.ImplFile file -> file.IsScript

    let ProcessDependencyManagerDirective directive args m state =
        if not canHaveScriptMetaCommands then
            errorR (HashReferenceNotAllowedInNonScript m)

        match args with
        | [ path ] ->
            let p = if String.IsNullOrWhiteSpace(path) then "" else path

            hashReferenceF state (m, p, directive)

        | _ ->
            errorR (Error(FSComp.SR.buildInvalidHashrDirective (), m))
            state

    let ProcessMetaCommand state hash =
        let mutable matchedm = range0

        try
            match hash with
            | ParsedHashDirective ("I", ParsedHashDirectiveArguments args, m) ->
                if not canHaveScriptMetaCommands then
                    errorR (HashIncludeNotAllowedInNonScript m)

                match args with
                | [ path ] ->
                    matchedm <- m
                    tcConfig.AddIncludePath(m, path, pathOfMetaCommandSource)
                    state
                | _ ->
                    errorR (Error(FSComp.SR.buildInvalidHashIDirective (), m))
                    state
            | ParsedHashDirective ("nowarn", ParsedHashDirectiveArguments numbers, m) ->
                List.fold (fun state d -> nowarnF state (m, d)) state numbers

            | ParsedHashDirective (("reference" | "r"), ParsedHashDirectiveArguments args, m) ->
                matchedm <- m
                ProcessDependencyManagerDirective Directive.Resolution args m state

            | ParsedHashDirective ("i", ParsedHashDirectiveArguments args, m) ->
                matchedm <- m
                ProcessDependencyManagerDirective Directive.Include args m state

            | ParsedHashDirective ("load", ParsedHashDirectiveArguments args, m) ->
                if not canHaveScriptMetaCommands then
                    errorR (HashDirectiveNotAllowedInNonScript m)

                match args with
                | _ :: _ ->
                    matchedm <- m
                    args |> List.iter (fun path -> loadSourceF state (m, path))
                | _ -> errorR (Error(FSComp.SR.buildInvalidHashloadDirective (), m))

                state
            | ParsedHashDirective ("time", ParsedHashDirectiveArguments args, m) ->
                if not canHaveScriptMetaCommands then
                    errorR (HashDirectiveNotAllowedInNonScript m)

                match args with
                | [] -> ()
                | [ "on" | "off" ] -> ()
                | _ -> errorR (Error(FSComp.SR.buildInvalidHashtimeDirective (), m))

                state

            | _ ->

                (* warning(Error("This meta-command has been ignored", m)) *)
                state
        with e ->
            errorRecovery e matchedm
            state

    let rec WarnOnIgnoredSpecDecls decls =
        decls
        |> List.iter (fun d ->
            match d with
            | SynModuleSigDecl.HashDirective (_, m) -> warning (Error(FSComp.SR.buildDirectivesInModulesAreIgnored (), m))
            | SynModuleSigDecl.NestedModule (moduleDecls = subDecls) -> WarnOnIgnoredSpecDecls subDecls
            | _ -> ())

    let rec WarnOnIgnoredImplDecls decls =
        decls
        |> List.iter (fun d ->
            match d with
            | SynModuleDecl.HashDirective (_, m) -> warning (Error(FSComp.SR.buildDirectivesInModulesAreIgnored (), m))
            | SynModuleDecl.NestedModule (decls = subDecls) -> WarnOnIgnoredImplDecls subDecls
            | _ -> ())

    let ProcessMetaCommandsFromModuleSpec state (SynModuleOrNamespaceSig (decls = decls)) =
        List.fold
            (fun s d ->
                match d with
                | SynModuleSigDecl.HashDirective (h, _) -> ProcessMetaCommand s h
                | SynModuleSigDecl.NestedModule (moduleDecls = subDecls) ->
                    WarnOnIgnoredSpecDecls subDecls
                    s
                | _ -> s)
            state
            decls

    let ProcessMetaCommandsFromModuleImpl state (SynModuleOrNamespace (decls = decls)) =
        List.fold
            (fun s d ->
                match d with
                | SynModuleDecl.HashDirective (h, _) -> ProcessMetaCommand s h
                | SynModuleDecl.NestedModule (decls = subDecls) ->
                    WarnOnIgnoredImplDecls subDecls
                    s
                | _ -> s)
            state
            decls

    match inp with
    | ParsedInput.SigFile sigFile ->
        let state = List.fold ProcessMetaCommand state0 sigFile.HashDirectives
        let state = List.fold ProcessMetaCommandsFromModuleSpec state sigFile.Contents
        state
    | ParsedInput.ImplFile implFile ->
        let state = List.fold ProcessMetaCommand state0 implFile.HashDirectives
        let state = List.fold ProcessMetaCommandsFromModuleImpl state implFile.Contents
        state

let ApplyNoWarnsToTcConfig (tcConfig: TcConfig, inp: ParsedInput, pathOfMetaCommandSource) =
    // Clone
    let tcConfigB = tcConfig.CloneToBuilder()
    let addNoWarn = fun () (m, s) -> tcConfigB.TurnWarningOff(m, s)
    let addReference = fun () (_m, _s, _) -> ()
    let addLoadedSource = fun () (_m, _s) -> ()
    ProcessMetaCommandsFromInput (addNoWarn, addReference, addLoadedSource) (tcConfigB, inp, pathOfMetaCommandSource, ())
    TcConfig.Create(tcConfigB, validate = false)

let ApplyMetaCommandsFromInputToTcConfig (tcConfig: TcConfig, inp: ParsedInput, pathOfMetaCommandSource, dependencyProvider) =
    // Clone
    let tcConfigB = tcConfig.CloneToBuilder()
    let getWarningNumber = fun () _ -> ()

    let addReferenceDirective =
        fun () (m, path, directive) -> tcConfigB.AddReferenceDirective(dependencyProvider, m, path, directive)

    let addLoadedSource =
        fun () (m, s) -> tcConfigB.AddLoadedSource(m, s, pathOfMetaCommandSource)

    ProcessMetaCommandsFromInput (getWarningNumber, addReferenceDirective, addLoadedSource) (tcConfigB, inp, pathOfMetaCommandSource, ())
    TcConfig.Create(tcConfigB, validate = false)

/// Build the initial type checking environment
let GetInitialTcEnv (assemblyName: string, initm: range, tcConfig: TcConfig, tcImports: TcImports, tcGlobals) =
    let initm = initm.StartRange

    let ccus =
        tcImports.GetImportedAssemblies()
        |> List.map (fun asm -> asm.FSharpViewOfMetadata, asm.AssemblyAutoOpenAttributes, asm.AssemblyInternalsVisibleToAttributes)

    let amap = tcImports.GetImportMap()

    let openDecls0, tcEnv =
        CreateInitialTcEnv(tcGlobals, amap, initm, assemblyName, ccus)

    if tcConfig.checkOverflow then
        try
            let checkOperatorsModule =
                pathToSynLid initm (splitNamespace CoreOperatorsCheckedName)

            let tcEnv, openDecls1 =
                TcOpenModuleOrNamespaceDecl TcResultsSink.NoSink tcGlobals amap initm tcEnv (checkOperatorsModule, initm)

            tcEnv, openDecls0 @ openDecls1
        with e ->
            errorRecovery e initm
            tcEnv, openDecls0
    else
        tcEnv, openDecls0

/// Inject faults into checking
let CheckSimulateException (tcConfig: TcConfig) =
    match tcConfig.simulateException with
    | Some ("tc-oom") -> raise (OutOfMemoryException())
    | Some ("tc-an") -> raise (ArgumentNullException("simulated"))
    | Some ("tc-invop") -> raise (InvalidOperationException())
    | Some ("tc-av") -> raise (AccessViolationException())
    | Some ("tc-nfn") -> raise (NotFiniteNumberException())
    | Some ("tc-aor") -> raise (ArgumentOutOfRangeException())
    | Some ("tc-dv0") -> raise (DivideByZeroException())
    | Some ("tc-oe") -> raise (OverflowException())
    | Some ("tc-atmm") -> raise (ArrayTypeMismatchException())
    | Some ("tc-bif") -> raise (BadImageFormatException())
    | Some ("tc-knf") -> raise (KeyNotFoundException())
    | Some ("tc-ior") -> raise (IndexOutOfRangeException())
    | Some ("tc-ic") -> raise (InvalidCastException())
    | Some ("tc-ip") -> raise (InvalidProgramException())
    | Some ("tc-ma") -> raise (MemberAccessException())
    | Some ("tc-ni") -> raise (NotImplementedException())
    | Some ("tc-nr") -> raise (NullReferenceException())
    | Some ("tc-oc") -> raise (OperationCanceledException())
    | Some ("tc-fail") -> failwith "simulated"
    | _ -> ()

//----------------------------------------------------------------------------
// Type-check sets of files
//--------------------------------------------------------------------------

type RootSigs = Zmap<QualifiedNameOfFile, ModuleOrNamespaceType>

type RootImpls = Zset<QualifiedNameOfFile>

let qnameOrder = Order.orderBy (fun (q: QualifiedNameOfFile) -> q.Text)

type TcState =
    {
        /// The assembly thunk for the assembly being compiled.
        tcsCcu: CcuThunk

        /// The typing environment implied by the set of signature files and/or inferred signatures of implementation files checked so far
        tcsTcSigEnv: TcEnv

        /// The typing environment implied by the set of implementation files checked so far
        tcsTcImplEnv: TcEnv

        /// Indicates if any implementation file so far includes use of generative provided types
        tcsCreatesGeneratedProvidedTypes: bool

        /// A table of signature files processed so far, indexed by QualifiedNameOfFile, to help give better diagnostics
        /// if there are mismatches in module names between signature and implementation files with the same name.
        tcsRootSigs: RootSigs

        /// A table of implementation files processed so far, indexed by QualifiedNameOfFile, to help give better diagnostics
        /// if there are mismatches in module names between signature and implementation files with the same name.
        tcsRootImpls: RootImpls

        /// The combined partial assembly signature resulting from all the signatures and/or inferred signatures of implementation files
        /// so far.
        tcsCcuSig: ModuleOrNamespaceType

        /// The collected implicit open declarations implied by '/checked' flag and processing F# interactive fragments that have an implied module.
        tcsImplicitOpenDeclarations: OpenDeclaration list
    }

    member x.TcEnvFromSignatures = x.tcsTcSigEnv

    member x.TcEnvFromImpls = x.tcsTcImplEnv

    member x.Ccu = x.tcsCcu

    member x.CreatesGeneratedProvidedTypes = x.tcsCreatesGeneratedProvidedTypes

    // a.fsi + b.fsi + c.fsi (after checking implementation file for c.fs)
    member x.CcuSig = x.tcsCcuSig

    member x.NextStateAfterIncrementalFragment tcEnvAtEndOfLastInput =
        { x with
            tcsTcSigEnv = tcEnvAtEndOfLastInput
            tcsTcImplEnv = tcEnvAtEndOfLastInput
        }

/// Create the initial type checking state for compiling an assembly
let GetInitialTcState (m, ccuName, tcConfig: TcConfig, tcGlobals, tcImports: TcImports, tcEnv0, openDecls0) =
    ignore tcImports

    // Create a ccu to hold all the results of compilation
    let ccuContents =
        Construct.NewCcuContents ILScopeRef.Local m ccuName (Construct.NewEmptyModuleOrNamespaceType(Namespace true))

    let ccuData: CcuData =
        {
            IsFSharp = true
            UsesFSharp20PlusQuotations = false
#if !NO_TYPEPROVIDERS
            InvalidateEvent = (Event<_>()).Publish
            IsProviderGenerated = false
            ImportProvidedType = (fun ty -> Import.ImportProvidedType (tcImports.GetImportMap()) m ty)
#endif
            TryGetILModuleDef = (fun () -> None)
            FileName = None
            Stamp = newStamp ()
            QualifiedName = None
            SourceCodeDirectory = tcConfig.implicitIncludeDir
            ILScopeRef = ILScopeRef.Local
            Contents = ccuContents
            MemberSignatureEquality = typeEquivAux EraseAll tcGlobals
            TypeForwarders = CcuTypeForwarderTable.Empty
            XmlDocumentationInfo = None
        }

    let ccu = CcuThunk.Create(ccuName, ccuData)

    // OK, is this is the FSharp.Core CCU then fix it up.
    if tcConfig.compilingFSharpCore then
        tcGlobals.fslibCcu.Fixup ccu

    {
        tcsCcu = ccu
        tcsTcSigEnv = tcEnv0
        tcsTcImplEnv = tcEnv0
        tcsCreatesGeneratedProvidedTypes = false
        tcsRootSigs = Zmap.empty qnameOrder
        tcsRootImpls = Zset.empty qnameOrder
        tcsCcuSig = Construct.NewEmptyModuleOrNamespaceType(Namespace true)
        tcsImplicitOpenDeclarations = openDecls0
    }

let AddCheckResultsToTcState
    (tcGlobals, amap, hadSig, prefixPathOpt, tcSink, tcImplEnv, qualNameOfFile, implFileSigType)
    (tcState: TcState)
    =
    let rootImpls = Zset.add qualNameOfFile tcState.tcsRootImpls

    // Only add it to the environment if it didn't have a signature
    let m = qualNameOfFile.Range

    // Add the implementation as to the implementation env
    let tcImplEnv =
        AddLocalRootModuleOrNamespace TcResultsSink.NoSink tcGlobals amap m tcImplEnv implFileSigType

    // Add the implementation as to the signature env (unless it had an explicit signature)
    let tcSigEnv =
        if hadSig then
            tcState.tcsTcSigEnv
        else
            AddLocalRootModuleOrNamespace TcResultsSink.NoSink tcGlobals amap m tcState.tcsTcSigEnv implFileSigType

    // Open the prefixPath for fsi.exe (tcImplEnv)
    let tcImplEnv, openDecls =
        match prefixPathOpt with
        | Some prefixPath -> TcOpenModuleOrNamespaceDecl tcSink tcGlobals amap m tcImplEnv (prefixPath, m)
        | _ -> tcImplEnv, []

    // Open the prefixPath for fsi.exe (tcSigEnv)
    let tcSigEnv, _ =
        match prefixPathOpt with
        | Some prefixPath when not hadSig -> TcOpenModuleOrNamespaceDecl tcSink tcGlobals amap m tcSigEnv (prefixPath, m)
        | _ -> tcSigEnv, []

    let ccuSigForFile =
        CombineCcuContentFragments [ implFileSigType; tcState.tcsCcuSig ]

    let tcState =
        { tcState with
            tcsTcSigEnv = tcSigEnv
            tcsTcImplEnv = tcImplEnv
            tcsRootImpls = rootImpls
            tcsCcuSig = ccuSigForFile
            tcsImplicitOpenDeclarations = tcState.tcsImplicitOpenDeclarations @ openDecls
        }

    ccuSigForFile, tcState

type PartialResult = TcEnv * TopAttribs * CheckedImplFile option * ModuleOrNamespaceType

/// Typecheck a single file (or interactive entry into F# Interactive). If skipImplIfSigExists is set to true
/// then implementations with signature files give empty results.
let CheckOneInput
    (
        checkForErrors,
        tcConfig: TcConfig,
        tcImports: TcImports,
        tcGlobals: TcGlobals,
        prefixPathOpt: LongIdent option,
        tcSink: TcResultsSink,
        tcState: TcState,
        input: ParsedInput
    ) : Cancellable<PartialResult * TcState> =
    cancellable {
        try
            use _ =
                Activity.start "ParseAndCheckInputs.CheckOneInput" [| Activity.Tags.fileName, input.FileName |]

            CheckSimulateException tcConfig

            let m = input.Range
            let amap = tcImports.GetImportMap()

            let conditionalDefines =
                if tcConfig.noConditionalErasure then
                    None
                else
                    Some tcConfig.conditionalDefines

            match input with
            | ParsedInput.SigFile file ->
                let qualNameOfFile = file.QualifiedName

                // Check if we've seen this top module signature before.
                if Zmap.mem qualNameOfFile tcState.tcsRootSigs then
                    errorR (Error(FSComp.SR.buildSignatureAlreadySpecified (qualNameOfFile.Text), m.StartRange))

                // Check if the implementation came first in compilation order
                if Zset.contains qualNameOfFile tcState.tcsRootImpls then
                    errorR (Error(FSComp.SR.buildImplementationAlreadyGivenDetail (qualNameOfFile.Text), m))

                // Typecheck the signature file
                let! tcEnv, sigFileType, createsGeneratedProvidedTypes =
                    CheckOneSigFile
                        (tcGlobals,
                         amap,
                         tcState.tcsCcu,
                         checkForErrors,
                         conditionalDefines,
                         tcSink,
                         tcConfig.internalTestSpanStackReferring,
                         tcConfig.diagnosticsOptions)
                        tcState.tcsTcSigEnv
                        file

                let rootSigs = Zmap.add qualNameOfFile sigFileType tcState.tcsRootSigs

                // Add the signature to the signature env (unless it had an explicit signature)
                let ccuSigForFile = CombineCcuContentFragments [ sigFileType; tcState.tcsCcuSig ]

                // Open the prefixPath for fsi.exe
                let tcEnv, _openDecls1 =
                    match prefixPathOpt with
                    | None -> tcEnv, []
                    | Some prefixPath ->
                        let m = qualNameOfFile.Range
                        TcOpenModuleOrNamespaceDecl tcSink tcGlobals amap m tcEnv (prefixPath, m)

                let tcState =
                    { tcState with
                        tcsTcSigEnv = tcEnv
                        tcsTcImplEnv = tcState.tcsTcImplEnv
                        tcsRootSigs = rootSigs
                        tcsCreatesGeneratedProvidedTypes = tcState.tcsCreatesGeneratedProvidedTypes || createsGeneratedProvidedTypes
                    }

                return (tcEnv, EmptyTopAttrs, None, ccuSigForFile), tcState

            | ParsedInput.ImplFile file ->
                let qualNameOfFile = file.QualifiedName

                // Check if we've got an interface for this fragment
                let rootSigOpt = tcState.tcsRootSigs.TryFind qualNameOfFile

                // Check if we've already seen an implementation for this fragment
                if Zset.contains qualNameOfFile tcState.tcsRootImpls then
                    errorR (Error(FSComp.SR.buildImplementationAlreadyGiven (qualNameOfFile.Text), m))

                let hadSig = rootSigOpt.IsSome

                // Typecheck the implementation file
                let! topAttrs, implFile, tcEnvAtEnd, createsGeneratedProvidedTypes =
                    CheckOneImplFile(
                        tcGlobals,
                        amap,
                        tcState.tcsCcu,
                        tcState.tcsImplicitOpenDeclarations,
                        checkForErrors,
                        conditionalDefines,
                        tcSink,
                        tcConfig.internalTestSpanStackReferring,
                        tcState.tcsTcImplEnv,
                        rootSigOpt,
                        file,
                        tcConfig.diagnosticsOptions
                    )

                let tcState =
                    { tcState with
                        tcsCreatesGeneratedProvidedTypes = tcState.tcsCreatesGeneratedProvidedTypes || createsGeneratedProvidedTypes
                    }

                let ccuSigForFile, tcState =
                    AddCheckResultsToTcState
                        (tcGlobals, amap, hadSig, prefixPathOpt, tcSink, tcState.tcsTcImplEnv, qualNameOfFile, implFile.Signature)
                        tcState

                let result = (tcEnvAtEnd, topAttrs, Some implFile, ccuSigForFile)
                return result, tcState

        with e ->
            errorRecovery e range0
            return (tcState.TcEnvFromSignatures, EmptyTopAttrs, None, tcState.tcsCcuSig), tcState
    }

// Within a file, equip loggers to locally filter w.r.t. scope pragmas in each input
let DiagnosticsLoggerForInput (tcConfig: TcConfig, input: ParsedInput, oldLogger) =
    GetDiagnosticsLoggerFilteringByScopedPragmas(false, input.ScopedPragmas, tcConfig.diagnosticsOptions, oldLogger)

/// Typecheck a single file (or interactive entry into F# Interactive)
let CheckOneInputEntry (ctok, checkForErrors, tcConfig: TcConfig, tcImports, tcGlobals, prefixPathOpt) tcState input =
    // Equip loggers to locally filter w.r.t. scope pragmas in each input
    use _ =
        UseTransformedDiagnosticsLogger(fun oldLogger -> DiagnosticsLoggerForInput(tcConfig, input, oldLogger))

    use _ = UseBuildPhase BuildPhase.TypeCheck

    RequireCompilationThread ctok

    CheckOneInput(checkForErrors, tcConfig, tcImports, tcGlobals, prefixPathOpt, TcResultsSink.NoSink, tcState, input)
    |> Cancellable.runWithoutCancellation

/// Finish checking multiple files (or one interactive entry into F# Interactive)
let CheckMultipleInputsFinish (results, tcState: TcState) =
    let tcEnvsAtEndFile, topAttrs, implFiles, ccuSigsForFiles = List.unzip4 results
    let topAttrs = List.foldBack CombineTopAttrs topAttrs EmptyTopAttrs
    let implFiles = List.choose id implFiles
    // This is the environment required by fsi.exe when incrementally adding definitions
    let tcEnvAtEndOfLastFile =
        (match tcEnvsAtEndFile with
         | h :: _ -> h
         | _ -> tcState.TcEnvFromSignatures)

    (tcEnvAtEndOfLastFile, topAttrs, implFiles, ccuSigsForFiles), tcState

let CheckOneInputAndFinish (checkForErrors, tcConfig: TcConfig, tcImports, tcGlobals, prefixPathOpt, tcSink, tcState, input) =
    cancellable {
        let! result, tcState = CheckOneInput(checkForErrors, tcConfig, tcImports, tcGlobals, prefixPathOpt, tcSink, tcState, input)
        let finishedResult = CheckMultipleInputsFinish([ result ], tcState)
        return finishedResult
    }

let CheckClosedInputSetFinish (declaredImpls: CheckedImplFile list, tcState) =
    // Latest contents to the CCU
    let ccuContents =
        Construct.NewCcuContents ILScopeRef.Local range0 tcState.tcsCcu.AssemblyName tcState.tcsCcuSig

    // Check all interfaces have implementations
    tcState.tcsRootSigs
    |> Zmap.iter (fun qualNameOfFile _ ->
        if not (Zset.contains qualNameOfFile tcState.tcsRootImpls) then
            errorR (Error(FSComp.SR.buildSignatureWithoutImplementation (qualNameOfFile.Text), qualNameOfFile.Range)))

    tcState, declaredImpls, ccuContents

let CheckMultipleInputsSequential (ctok, checkForErrors, tcConfig, tcImports, tcGlobals, prefixPathOpt, tcState, inputs) =
    (tcState, inputs)
    ||> List.mapFold (CheckOneInputEntry(ctok, checkForErrors, tcConfig, tcImports, tcGlobals, prefixPathOpt))

open FSharp.Compiler.GraphChecking

type State = TcState * bool
type FinalFileResult = TcEnv * TopAttribs * CheckedImplFile option * ModuleOrNamespaceType

/// Auxiliary type for re-using signature information in TcEnvFromImpls.
///
/// TcState has two typing environments: TcEnvFromSignatures && TcEnvFromImpls
/// When type checking a file, depending on the type (implementation or signature), it will use one of these typing environments (TcEnv).
/// Checking a file will populate the respective TcEnv.
///
/// When a file has a dependencies, the information of the signature file in case a pair (implementation file backed by a signature) will suffice to type-check that file.
/// Example: if `B.fs` has a dependency on `A`, the information of `A.fsi` is enough for `B.fs` to type-check, on condition that information is available in the TcEnvFromImpls.
/// We introduce a special ArtificialImplFile node in the graph to satisfy this. `B.fs -> [ A.fsi ]` becomes `B.fs -> [ ArtificialImplFile A ].
/// The `ArtificialImplFile A` node will duplicate the signature information which A.fsi provided earlier.
/// Processing a `ArtificialImplFile` node will add the information from the TcEnvFromSignatures to the TcEnvFromImpls.
/// This means `A` will be known in both TcEnvs and therefor `B.fs` can be type-checked.
/// By doing this, we can speed up the graph processing as type checking a signature file is less expensive than its implementation counterpart.
///
/// When we need to actually type-check an implementation file backed by a signature, we cannot have the duplicate information of the signature file present in TcEnvFromImpls.
/// Example `A.fs -> [ A.fsi ]`. An implementation file always depends on its signature.
/// Type-checking `A.fs` will add the actual information to TcEnvFromImpls and we do not depend on the `ArtificialImplFile A` for `A.fs`.
///
/// In order to deal correctly with the `ArtificialImplFile` logic, we need to transform the resolved graph to contain the additional pair nodes.
/// After we have type-checked the graph, we exclude the ArtificialImplFile nodes as they are not actual physical files in our project.
[<RequireQualifiedAccess>]
type NodeToTypeCheck =
    /// A real physical file in the current project.
    /// This can be either an implementation or a signature file.
    | PhysicalFile of fileIndex: FileIndex
    /// An artificial node that will add the earlier processed signature information to the TcEnvFromImpls.
    /// Dependants on this type of node will perceive that a file is known in both TcEnvFromSignatures and TcEnvFromImpls.
    /// Even though the actual implementation file was not type-checked.
    | ArtificialImplFile of signatureFileIndex: FileIndex

let folder (state: State) (finisher: Finisher<State, FinalFileResult>) : FinalFileResult * State = finisher.Invoke(state)

/// Typecheck a single file (or interactive entry into F# Interactive)
/// <returns>a callback functions that takes a `TcState` and will add the checked result to it.</returns>
let CheckOneInputWithCallback
    ((checkForErrors,
      tcConfig: TcConfig,
      tcImports: TcImports,
      tcGlobals,
      prefixPathOpt,
      tcSink,
      tcState: TcState,
      inp: ParsedInput,
      _skipImplIfSigExists: bool): (unit -> bool) * TcConfig * TcImports * TcGlobals * LongIdent option * TcResultsSink * TcState * ParsedInput * bool)
    : Cancellable<Finisher<TcState, PartialResult>> =
    cancellable {
        try
            CheckSimulateException tcConfig

            let m = inp.Range
            let amap = tcImports.GetImportMap()

            let conditionalDefines =
                if tcConfig.noConditionalErasure then
                    None
                else
                    Some tcConfig.conditionalDefines

            match inp with
            | ParsedInput.SigFile file ->
                let qualNameOfFile = file.QualifiedName

                // Check if we've seen this top module signature before.
                if Zmap.mem qualNameOfFile tcState.tcsRootSigs then
                    errorR (Error(FSComp.SR.buildSignatureAlreadySpecified (qualNameOfFile.Text), m.StartRange))

                // Check if the implementation came first in compilation order
                if Zset.contains qualNameOfFile tcState.tcsRootImpls then
                    errorR (Error(FSComp.SR.buildImplementationAlreadyGivenDetail (qualNameOfFile.Text), m))

                // Typecheck the signature file
                let! tcEnv, sigFileType, createsGeneratedProvidedTypes =
                    CheckOneSigFile
                        (tcGlobals,
                         amap,
                         tcState.tcsCcu,
                         checkForErrors,
                         conditionalDefines,
                         tcSink,
                         tcConfig.internalTestSpanStackReferring,
                         tcConfig.diagnosticsOptions)
                        tcState.tcsTcSigEnv
                        file

                // Open the prefixPath for fsi.exe
                let tcEnv, _openDecls1 =
                    match prefixPathOpt with
                    | None -> tcEnv, []
                    | Some prefixPath ->
                        let m = qualNameOfFile.Range
                        TcOpenModuleOrNamespaceDecl tcSink tcGlobals amap m tcEnv (prefixPath, m)

                return
                    Finisher(fun tcState ->
                        let rootSigs = Zmap.add qualNameOfFile sigFileType tcState.tcsRootSigs

                        let tcSigEnv =
                            AddLocalRootModuleOrNamespace TcResultsSink.NoSink tcGlobals amap m tcState.tcsTcSigEnv sigFileType

                        // Add the signature to the signature env (unless it had an explicit signature)
                        let ccuSigForFile = CombineCcuContentFragments [ sigFileType; tcState.tcsCcuSig ]

                        let partialResult = tcEnv, EmptyTopAttrs, None, ccuSigForFile

                        let tcState =
                            { tcState with
                                tcsTcSigEnv = tcSigEnv
                                tcsRootSigs = rootSigs
                                tcsCreatesGeneratedProvidedTypes = tcState.tcsCreatesGeneratedProvidedTypes || createsGeneratedProvidedTypes
                            }

                        partialResult, tcState)

            | ParsedInput.ImplFile file ->
                let qualNameOfFile = file.QualifiedName

                // Check if we've got an interface for this fragment
                let rootSigOpt = tcState.tcsRootSigs.TryFind qualNameOfFile

                // Typecheck the implementation file
                let! topAttrs, implFile, tcEnvAtEnd, createsGeneratedProvidedTypes =
                    CheckOneImplFile(
                        tcGlobals,
                        amap,
                        tcState.tcsCcu,
                        tcState.tcsImplicitOpenDeclarations,
                        checkForErrors,
                        conditionalDefines,
                        tcSink,
                        tcConfig.internalTestSpanStackReferring,
                        tcState.tcsTcImplEnv,
                        rootSigOpt,
                        file,
                        tcConfig.diagnosticsOptions
                    )

                return
                    Finisher(fun tcState ->
                        // Check if we've already seen an implementation for this fragment
                        if Zset.contains qualNameOfFile tcState.tcsRootImpls then
                            errorR (Error(FSComp.SR.buildImplementationAlreadyGiven (qualNameOfFile.Text), m))

                        let ccuSigForFile, fsTcState =
                            AddCheckResultsToTcState
                                (tcGlobals, amap, false, prefixPathOpt, tcSink, tcState.tcsTcImplEnv, qualNameOfFile, implFile.Signature)
                                tcState

                        let partialResult = tcEnvAtEnd, topAttrs, Some implFile, ccuSigForFile

                        let tcState =
                            { fsTcState with
                                tcsCreatesGeneratedProvidedTypes =
                                    fsTcState.tcsCreatesGeneratedProvidedTypes || createsGeneratedProvidedTypes
                            }

                        partialResult, tcState)

        with e ->
            errorRecovery e range0
            return Finisher(fun tcState -> (tcState.TcEnvFromSignatures, EmptyTopAttrs, None, tcState.tcsCcuSig), tcState)
    }

let AddSignatureResultToTcImplEnv (tcImports: TcImports, tcGlobals, prefixPathOpt, tcSink, tcState, input: ParsedInput) =
    let qualNameOfFile = input.QualifiedName
    let rootSigOpt = tcState.tcsRootSigs.TryFind qualNameOfFile

    match rootSigOpt with
    | None -> failwithf $"No signature data was found for %s{input.FileName}"
    | Some rootSig ->
        fun (tcState: TcState) ->
            let amap = tcImports.GetImportMap()

            // Add the results of type checking the signature file to the TcEnv of implementation files.
            let ccuSigForFile, tcState =
                AddCheckResultsToTcState
                    (tcGlobals, amap, true, prefixPathOpt, tcSink, tcState.tcsTcImplEnv, qualNameOfFile, rootSig)
                    tcState

            // This partial result will be discarded in the end of the graph resolution.
            let partialResult: PartialResult =
                tcState.tcsTcSigEnv, EmptyTopAttrs, None, ccuSigForFile

            partialResult, tcState

/// Constructs a file dependency graph and type-checks the files in parallel where possible.
let CheckMultipleInputsUsingGraphMode
    ((ctok, checkForErrors, tcConfig: TcConfig, tcImports: TcImports, tcGlobals, prefixPathOpt, tcState, eagerFormat, inputs): 'a * (unit -> bool) * TcConfig * TcImports * TcGlobals * LongIdent option * TcState * (PhasedDiagnostic -> PhasedDiagnostic) * ParsedInput list)
    : FinalFileResult list * TcState =
    use cts = new CancellationTokenSource()

    let sourceFiles: FileInProject array =
        inputs
        |> List.toArray
        |> Array.mapi (fun idx (input: ParsedInput) ->
            {
                Idx = idx
                FileName = input.FileName
                ParsedInput = input
            })

    let filePairs = FilePairMap(sourceFiles)

    let graph =
        DependencyResolution.mkGraph tcConfig.compilingFSharpCore filePairs sourceFiles

    let nodeGraph =
        let mkArtificialImplFile n = NodeToTypeCheck.ArtificialImplFile n
        let mkPhysicalFile n = NodeToTypeCheck.PhysicalFile n

        /// Map any signature dependencies to the ArtificialImplFile counterparts,
        /// unless the signature dependency is the backing file of the current (implementation) file.
        let mapDependencies idx deps =
            Array.map
                (fun dep ->
                    if filePairs.IsSignature dep then
                        let implIdx = filePairs.GetImplementationIndex dep

                        if implIdx = idx then
                            // This is the matching signature for the implementation.
                            // Retain the direct dependency onto the signature file.
                            mkPhysicalFile dep
                        else
                            mkArtificialImplFile dep
                    else
                        mkPhysicalFile dep)
                deps

        // Transform the graph to include ArtificialImplFile nodes when necessary.
        graph
        |> Seq.collect (fun (KeyValue (fileIdx, deps)) ->
            if filePairs.IsSignature fileIdx then
                // Add an additional ArtificialImplFile node for the signature file.
                [|
                    // Mark the current file as physical and map the dependencies.
                    mkPhysicalFile fileIdx, mapDependencies fileIdx deps
                    // Introduce a new node that depends on the signature.
                    mkArtificialImplFile fileIdx, [| mkPhysicalFile fileIdx |]
                |]
            else
                [| mkPhysicalFile fileIdx, mapDependencies fileIdx deps |])
        |> Graph.make

    // Persist the graph to a Mermaid diagram if specified.
    if tcConfig.typeCheckingConfig.DumpGraph then
        tcConfig.outputFile
        |> Option.iter (fun outputFile ->
            let outputFile = FileSystem.GetFullPathShim(outputFile)
            let graphFile = FileSystem.ChangeExtensionShim(outputFile, ".graph.md")

            graph
            |> Graph.map (fun idx ->
                let friendlyFileName =
                    sourceFiles[idx]
                        .FileName.Replace(tcConfig.implicitIncludeDir, "")
                        .TrimStart([| '\\'; '/' |])

                (idx, friendlyFileName))
            |> Graph.serialiseToMermaid graphFile)

    let _ = ctok // TODO Use it
    let diagnosticsLogger = DiagnosticsThreadStatics.DiagnosticsLogger

    // In the first linear part of parallel checking, we use a 'checkForErrors' that checks either for errors
    // somewhere in the files processed prior to each one, or in the processing of this particular file.
    let priorErrors = checkForErrors ()

    let processArtificialImplFile (input: ParsedInput) ((currentTcState, _currentPriorErrors): State) : Finisher<State, PartialResult> =
        Finisher(fun (state: State) ->
            let tcState, currentPriorErrors = state

            let f =
                // Retrieve the type-checked signature information and add it to the TcEnvFromImpls.
                AddSignatureResultToTcImplEnv(tcImports, tcGlobals, prefixPathOpt, TcResultsSink.NoSink, currentTcState, input)

            // The `partialResult` will be excluded at the end of `GraphProcessing.processGraph`.
            // The important thing is that `nextTcState` will populated the necessary information to TcEnvFromImpls.
            let partialResult, nextTcState = f tcState
            partialResult, (nextTcState, currentPriorErrors))

    let processFile
        ((input, logger): ParsedInput * DiagnosticsLogger)
        ((currentTcState, _currentPriorErrors): State)
        : Finisher<State, PartialResult> =
        use _ = UseDiagnosticsLogger logger
        let checkForErrors2 () = priorErrors || (logger.ErrorCount > 0)
        let tcSink = TcResultsSink.NoSink

        let finisher =
            CheckOneInputWithCallback(checkForErrors2, tcConfig, tcImports, tcGlobals, prefixPathOpt, tcSink, currentTcState, input, false)
            |> Cancellable.runWithoutCancellation

        Finisher(fun (state: State) ->
            let tcState, priorErrors = state
            let (partialResult: PartialResult, tcState) = finisher.Invoke(tcState)
            let hasErrors = logger.ErrorCount > 0
            let priorOrCurrentErrors = priorErrors || hasErrors
            let state: State = tcState, priorOrCurrentErrors
            partialResult, state)

    UseMultipleDiagnosticLoggers (inputs, diagnosticsLogger, Some eagerFormat) (fun inputsWithLoggers ->
        // Equip loggers to locally filter w.r.t. scope pragmas in each input
        let inputsWithLoggers =
            inputsWithLoggers
            |> List.toArray
            |> Array.map (fun (input, oldLogger) ->
                let logger = DiagnosticsLoggerForInput(tcConfig, input, oldLogger)
                input, logger)

        let processFile (node: NodeToTypeCheck) (state: State) : Finisher<State, PartialResult> =
            match node with
            | NodeToTypeCheck.ArtificialImplFile idx ->
                let parsedInput, _ = inputsWithLoggers[idx]
                processArtificialImplFile parsedInput state
            | NodeToTypeCheck.PhysicalFile idx ->
                let parsedInput, logger = inputsWithLoggers[idx]
                processFile (parsedInput, logger) state

        let state: State = tcState, priorErrors

        let finalStateItemChooser node =
            match node with
            | NodeToTypeCheck.ArtificialImplFile _ -> None
            | NodeToTypeCheck.PhysicalFile file -> Some file

        let partialResults, (tcState, _) =
            TypeCheckingGraphProcessing.processTypeCheckingGraph<NodeToTypeCheck, int, State, FinalFileResult>
                nodeGraph
                processFile
                folder
                finalStateItemChooser
                state
                cts.Token

        let partialResults =
            partialResults
            // Bring back the original, index-based file order.
            |> List.sortBy fst
            |> List.map snd

        partialResults, tcState)

let CheckClosedInputSet (ctok, checkForErrors, tcConfig: TcConfig, tcImports, tcGlobals, prefixPathOpt, tcState, eagerFormat, inputs) =
    // tcEnvAtEndOfLastFile is the environment required by fsi.exe when incrementally adding definitions
    let results, tcState =
        match tcConfig.typeCheckingConfig.Mode with
        | TypeCheckingMode.Graph when (not tcConfig.isInteractive && not tcConfig.deterministic) ->
            CheckMultipleInputsUsingGraphMode(
                ctok,
                checkForErrors,
                tcConfig,
                tcImports,
                tcGlobals,
                prefixPathOpt,
                tcState,
                eagerFormat,
                inputs
            )
        | _ -> CheckMultipleInputsSequential(ctok, checkForErrors, tcConfig, tcImports, tcGlobals, prefixPathOpt, tcState, inputs)

    let (tcEnvAtEndOfLastFile, topAttrs, implFiles, _), tcState =
        CheckMultipleInputsFinish(results, tcState)

    let tcState, declaredImpls, ccuContents =
        CheckClosedInputSetFinish(implFiles, tcState)

    tcState.Ccu.Deref.Contents <- ccuContents
    tcState, topAttrs, declaredImpls, tcEnvAtEndOfLastFile
