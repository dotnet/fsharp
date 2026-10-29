namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Concurrent
open System.Composition

open Microsoft.CodeAnalysis
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Utilities.UnifiedSettings

module DefaultTuning =
    /// How long is the per-document data saved before it is eligible for eviction from the cache? 10 seconds.
    /// Re-tokenizing is fast so we don't need to save this data long.
    let PerDocumentSavedDataSlidingWindow = TimeSpan(0, 0, 10) (* seconds *)

type EnterKeySetting =
    | NeverNewline
    | NewlineOnCompleteWord
    | AlwaysNewline

type IntelliSenseOptions =
    {
        ShowAfterCharIsTyped: bool
        ShowAfterCharIsDeleted: bool
        IncludeSymbolsFromUnopenedNamespacesOrModules: bool
        EnterKeySetting: EnterKeySetting
        GenerateBodyForOverriddenMethod: bool
    }

    static member Default =
        {
            ShowAfterCharIsTyped = true
            ShowAfterCharIsDeleted = false
            IncludeSymbolsFromUnopenedNamespacesOrModules = false
            EnterKeySetting = EnterKeySetting.NeverNewline
            GenerateBodyForOverriddenMethod = true
        }

[<RequireQualifiedAccess>]
type QuickInfoUnderlineStyle =
    | Dot
    | Dash
    | Solid

type QuickInfoOptions =
    {
        DisplayLinks: bool
        UnderlineStyle: QuickInfoUnderlineStyle
        DescriptionWidth: int option
        ShowRemarks: bool
    }

    static member Default =
        {
            DisplayLinks = true
            UnderlineStyle = QuickInfoUnderlineStyle.Solid
            DescriptionWidth = None
            ShowRemarks = true
        }

type CodeFixesOptions =
    {
        SimplifyName: bool
        AlwaysPlaceOpensAtTopLevel: bool
        UnusedOpens: bool
        UnusedDeclarations: bool
        SuggestNamesForErrors: bool
        RemoveParens: bool
    }

    static member Default =
        { // We have this off by default, disable until we work out how to make this low priority
            // See https://github.com/dotnet/fsharp/pull/3238#issue-237699595
            SimplifyName = false
            AlwaysPlaceOpensAtTopLevel = true
            UnusedOpens = true
            UnusedDeclarations = true
            SuggestNamesForErrors = true
            RemoveParens = false
        }

type LanguageServicePerformanceOptions =
    {
        EnableInMemoryCrossProjectReferences: bool
        TransparentCompilerCacheFactor: int
        AllowStaleCompletionResults: bool
        TimeUntilStaleCompletion: int
        EnableParallelReferenceResolution: bool
        EnableFastFindReferencesAndRename: bool
        EnablePartialTypeChecking: bool
        KeepAllBackgroundResolutions: bool
        KeepAllBackgroundSymbolUses: bool
        EnableBackgroundItemKeyStoreAndSemanticClassification: bool
    }

    static member Default =
        {
            EnableInMemoryCrossProjectReferences = true
            TransparentCompilerCacheFactor = 100
            AllowStaleCompletionResults = true
            TimeUntilStaleCompletion = 2000 // In ms, so this is 2 seconds
            EnableParallelReferenceResolution = false
            EnableFastFindReferencesAndRename = true
            EnablePartialTypeChecking = true
            KeepAllBackgroundResolutions = false
            KeepAllBackgroundSymbolUses = false
            EnableBackgroundItemKeyStoreAndSemanticClassification = true
        }

type AdvancedOptions =
    {
        IsBlockStructureEnabled: bool
        IsOutliningEnabled: bool
        IsInlayTypeHintsEnabled: bool
        IsInlayParameterNameHintsEnabled: bool
        IsInlayReturnTypeHintsEnabled: bool
        IsUseLiveBuffersEnabled: bool
        UseTransparentCompiler: bool
        TransparentCompilerSnapshotReuse: bool
        SendAdditionalTelemetry: bool
        SolutionBackgroundAnalysis: bool
    }

    static member Default =
        {
            IsBlockStructureEnabled = true
            IsOutliningEnabled = true
            IsInlayTypeHintsEnabled = false
            IsInlayParameterNameHintsEnabled = false
            IsInlayReturnTypeHintsEnabled = false
            UseTransparentCompiler = false
            TransparentCompilerSnapshotReuse = false
            IsUseLiveBuffersEnabled = true
            SendAdditionalTelemetry = true
            SolutionBackgroundAnalysis = false
        }

type FormattingOptions =
    {
        FormatOnPaste: bool
    }

    static member Default = { FormatOnPaste = false }

[<Shared; Export>]
type EditorOptions private (reader: ISettingsReader option) =
    let cache = ConcurrentDictionary<Type, obj>()

    let register (value: 'T) = cache[typeof<'T>] <- value

    let refresh reader =
        let boolean suffix =
            UnifiedSettings.read<bool> reader suffix

        let integer suffix = UnifiedSettings.read<int> reader suffix

        let text suffix =
            UnifiedSettings.read<string> reader suffix

        register
            {
                ShowAfterCharIsTyped = boolean "intelliSense.showAfterCharIsTyped"
                ShowAfterCharIsDeleted = boolean "intelliSense.showAfterCharIsDeleted"
                IncludeSymbolsFromUnopenedNamespacesOrModules = boolean "intelliSense.includeSymbolsFromUnopenedNamespacesOrModules"
                EnterKeySetting =
                    match text "intelliSense.enterKeySetting" with
                    | "NeverNewline" -> NeverNewline
                    | "NewlineOnCompleteWord" -> NewlineOnCompleteWord
                    | "AlwaysNewline" -> AlwaysNewline
                    | value -> invalidOp $"Unknown F# Enter key setting: {value}."
                GenerateBodyForOverriddenMethod = boolean "intelliSense.generateBodyForOverriddenMethod"
            }

        register
            {
                DisplayLinks = boolean "quickInfo.displayLinks"
                UnderlineStyle =
                    match text "quickInfo.underlineStyle" with
                    | "Dot" -> QuickInfoUnderlineStyle.Dot
                    | "Dash" -> QuickInfoUnderlineStyle.Dash
                    | "Solid" -> QuickInfoUnderlineStyle.Solid
                    | value -> invalidOp $"Unknown F# underline style: {value}."
                DescriptionWidth =
                    if boolean "quickInfo.descriptionWidth" then
                        Some(integer "quickInfo.descriptionWidthInCharacters")
                    else
                        None
                ShowRemarks = boolean "quickInfo.showRemarks"
            }

        register
            {
                SimplifyName = boolean "codeFixes.simplifyName"
                AlwaysPlaceOpensAtTopLevel = boolean "codeFixes.alwaysPlaceOpensAtTopLevel"
                UnusedOpens = boolean "codeFixes.unusedOpens"
                UnusedDeclarations = boolean "codeFixes.unusedDeclarations"
                SuggestNamesForErrors = boolean "codeFixes.suggestNamesForErrors"
                RemoveParens = boolean "codeFixes.removeParens"
            }

        register
            {
                EnableInMemoryCrossProjectReferences = boolean "performance.enableInMemoryCrossProjectReferences"
                TransparentCompilerCacheFactor = integer "performance.transparentCompilerCacheFactor"
                AllowStaleCompletionResults = boolean "performance.allowStaleCompletionResults"
                TimeUntilStaleCompletion = integer "performance.timeUntilStaleCompletion"
                EnableParallelReferenceResolution = boolean "performance.enableParallelReferenceResolution"
                EnableFastFindReferencesAndRename = boolean "performance.enableFastFindReferencesAndRename"
                EnablePartialTypeChecking = boolean "performance.enablePartialTypeChecking"
                KeepAllBackgroundResolutions = boolean "performance.keepAllBackgroundResolutions"
                KeepAllBackgroundSymbolUses = boolean "performance.keepAllBackgroundSymbolUses"
                EnableBackgroundItemKeyStoreAndSemanticClassification =
                    boolean "performance.enableBackgroundItemKeyStoreAndSemanticClassification"
            }

        register
            {
                IsBlockStructureEnabled = boolean "advanced.isBlockStructureEnabled"
                IsOutliningEnabled = boolean "advanced.isOutliningEnabled"
                IsInlayTypeHintsEnabled = boolean "advanced.isInlayTypeHintsEnabled"
                IsInlayParameterNameHintsEnabled = boolean "advanced.isInlayParameterNameHintsEnabled"
                IsInlayReturnTypeHintsEnabled = boolean "advanced.isInlayReturnTypeHintsEnabled"
                IsUseLiveBuffersEnabled = boolean "advanced.isUseLiveBuffersEnabled"
                UseTransparentCompiler = boolean "advanced.useTransparentCompiler"
                TransparentCompilerSnapshotReuse = boolean "advanced.transparentCompilerSnapshotReuse"
                SendAdditionalTelemetry = boolean "advanced.sendAdditionalTelemetry"
                SolutionBackgroundAnalysis = boolean "advanced.solutionBackgroundAnalysis"
            }

        register
            {
                FormatOnPaste = boolean "formatting.formatOnPaste"
            }

    do
        register QuickInfoOptions.Default
        register CodeFixesOptions.Default
        register LanguageServicePerformanceOptions.Default
        register AdvancedOptions.Default
        register IntelliSenseOptions.Default
        register FormattingOptions.Default

    let subscription =
        reader
        |> Option.map (fun reader -> UnifiedSettings.subscribeToChanges reader "languages.fsharp.*" (fun () -> refresh reader))

    new() = new EditorOptions(None)

    internal new(reader: ISettingsReader) = new EditorOptions(Some reader)

    [<ImportingConstructor>]
    new([<Import(AllowDefault = true)>] serviceProvider: SVsServiceProvider) =
        new EditorOptions(
            if isNull serviceProvider then
                None
            else
                let manager =
                    serviceProvider.GetService(typeof<Microsoft.Internal.VisualStudio.Shell.Interop.SVsUnifiedSettingsManager>)
                    :?> ISettingsManager

                Some(manager.GetReader())
        )

    member _.IntelliSense = unbox<IntelliSenseOptions> cache[typeof<IntelliSenseOptions>]
    member _.QuickInfo = unbox<QuickInfoOptions> cache[typeof<QuickInfoOptions>]
    member _.CodeFixes = unbox<CodeFixesOptions> cache[typeof<CodeFixesOptions>]

    member _.LanguageServicePerformance =
        unbox<LanguageServicePerformanceOptions> cache[typeof<LanguageServicePerformanceOptions>]

    member _.Advanced = unbox<AdvancedOptions> cache[typeof<AdvancedOptions>]
    member _.Formatting = unbox<FormattingOptions> cache[typeof<FormattingOptions>]

    member _.With value = register value

    interface Microsoft.CodeAnalysis.Host.IWorkspaceService

    interface IDisposable with
        member _.Dispose() = subscription |> Option.iter _.Dispose()

[<AutoOpen>]
module EditorOptionsExtensions =

    type Project with

        member private this.EditorOptions =
            this.Solution.Workspace.Services.GetService<EditorOptions>()

        member this.AreFSharpInMemoryCrossProjectReferencesEnabled =
            this.EditorOptions.LanguageServicePerformance.EnableInMemoryCrossProjectReferences

        member this.IsFSharpCodeFixesAlwaysPlaceOpensAtTopLevelEnabled =
            this.EditorOptions.CodeFixes.AlwaysPlaceOpensAtTopLevel

        member this.IsFSharpCodeFixesUnusedDeclarationsEnabled =
            this.EditorOptions.CodeFixes.UnusedDeclarations

        member this.IsFSharpStaleCompletionResultsEnabled =
            this.EditorOptions.LanguageServicePerformance.AllowStaleCompletionResults

        member this.FSharpTimeUntilStaleCompletion =
            this.EditorOptions.LanguageServicePerformance.TimeUntilStaleCompletion

        member this.IsFSharpCodeFixesSimplifyNameEnabled =
            this.EditorOptions.CodeFixes.SimplifyName

        member this.IsFSharpCodeFixesUnusedOpensEnabled =
            this.EditorOptions.CodeFixes.UnusedOpens

        member this.IsFsharpRemoveParensEnabled = this.EditorOptions.CodeFixes.RemoveParens

        member this.IsFSharpCodeFixesSuggestNamesForErrorsEnabled =
            this.EditorOptions.CodeFixes.SuggestNamesForErrors

        member this.IsFSharpBlockStructureEnabled =
            this.EditorOptions.Advanced.IsBlockStructureEnabled

        member this.IsFastFindReferencesEnabled =
            this.EditorOptions.LanguageServicePerformance.EnableFastFindReferencesAndRename

        member this.UseTransparentCompiler = this.EditorOptions.Advanced.UseTransparentCompiler

        member this.IsTransparentCompilerSnapshotReuseEnabled =
            this.EditorOptions.Advanced.TransparentCompilerSnapshotReuse

        member this.TransparentCompilerCacheFactor =
            this.EditorOptions.LanguageServicePerformance.TransparentCompilerCacheFactor
