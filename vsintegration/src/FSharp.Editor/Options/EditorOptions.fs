namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Runtime.InteropServices
open Microsoft.VisualStudio.FSharp.UIResources
open Microsoft.CodeAnalysis

module DefaultTuning =
    /// How long is the per-document data saved before it is eligible for eviction from the cache? 10 seconds.
    /// Re-tokenizing is fast so we don't need to save this data long.
    let PerDocumentSavedDataSlidingWindow = TimeSpan(0,0,10)(* seconds *)

type EnterKeySetting =
    | NeverNewline
    | NewlineOnCompleteWord
    | AlwaysNewline

// CLIMutable to make the record work also as a view model
[<CLIMutable>]
type IntelliSenseOptions =
  { ShowAfterCharIsTyped: bool
    ShowAfterCharIsDeleted: bool
    IncludeSymbolsFromUnopenedNamespacesOrModules : bool
    EnterKeySetting : EnterKeySetting }
    static member Default =
      { ShowAfterCharIsTyped = true
        ShowAfterCharIsDeleted = false
        IncludeSymbolsFromUnopenedNamespacesOrModules = false
        EnterKeySetting = EnterKeySetting.NeverNewline}

[<RequireQualifiedAccess>]
type QuickInfoUnderlineStyle = Dot | Dash | Solid

[<CLIMutable>]
type QuickInfoOptions =
    { DisplayLinks: bool
      UnderlineStyle: QuickInfoUnderlineStyle }
    static member Default =
      { DisplayLinks = true
        UnderlineStyle = QuickInfoUnderlineStyle.Solid }

[<CLIMutable>]
type CodeFixesOptions =
    { SimplifyName: bool
      AlwaysPlaceOpensAtTopLevel: bool
      UnusedOpens: bool 
      UnusedDeclarations: bool
      SuggestNamesForErrors: bool }
    static member Default =
      { // We have this off by default, disable until we work out how to make this low priority 
        // See https://github.com/dotnet/fsharp/pull/3238#issue-237699595
        SimplifyName = false 
        AlwaysPlaceOpensAtTopLevel = true
        UnusedOpens = true
        UnusedDeclarations = true
        SuggestNamesForErrors = true }

[<CLIMutable>]
type LanguageServicePerformanceOptions =
    { EnableInMemoryCrossProjectReferences: bool
      AllowStaleCompletionResults: bool
      TimeUntilStaleCompletion: int
      EnableParallelReferenceResolution: bool
      EnableFastFindReferences: bool
      UseSyntaxTreeCache: bool }
    static member Default =
      { EnableInMemoryCrossProjectReferences = true
        AllowStaleCompletionResults = true
        TimeUntilStaleCompletion = 2000 // In ms, so this is 2 seconds
        EnableParallelReferenceResolution = false
        EnableFastFindReferences = FSharpExperimentalFeaturesEnabledAutomatically
        UseSyntaxTreeCache = FSharpExperimentalFeaturesEnabledAutomatically }

[<CLIMutable>]
type AdvancedOptions =
    { IsBlockStructureEnabled: bool
      IsOutliningEnabled: bool
      IsInlineTypeHintsEnabled: bool
      IsInlineParameterNameHintsEnabled: bool
      IsLiveBuffersEnabled: bool }
    static member Default =
      { IsBlockStructureEnabled = true
        IsOutliningEnabled = true
        IsInlineTypeHintsEnabled = false 
        IsInlineParameterNameHintsEnabled = false
        IsLiveBuffersEnabled = FSharpExperimentalFeaturesEnabledAutomatically }

[<CLIMutable>]
type FormattingOptions =
    { FormatOnPaste: bool }
    static member Default =
        { FormatOnPaste = false }

[<Shared; Export; Export(typeof<Settings.IPersistSettings>)>]
type EditorOptions() =
    // we use in-memory store when outside of VS, e.g. in unit tests
    let store = Settings.CreateStore()
        
    do
        store.Register QuickInfoOptions.Default
        store.Register CodeFixesOptions.Default
        store.Register LanguageServicePerformanceOptions.Default
        store.Register AdvancedOptions.Default
        store.Register IntelliSenseOptions.Default
        store.Register FormattingOptions.Default

    member _.IntelliSense : IntelliSenseOptions = store.Get()
    member _.QuickInfo : QuickInfoOptions = store.Get()
    member _.CodeFixes : CodeFixesOptions = store.Get()
    member _.LanguageServicePerformance : LanguageServicePerformanceOptions = store.Get()
    member _.Advanced: AdvancedOptions = store.Get()
    member _.Formatting : FormattingOptions = store.Get()

    interface Microsoft.CodeAnalysis.Host.IWorkspaceService

    interface Settings.IPersistSettings with
        member _.LoadSettings() = store.LoadSettings()
        member _.SaveSettings(settings) = store.SaveSettings(settings)

module internal OptionsUI =

    open OptionsUIHelpers

    [<Guid(Guids.intelliSenseOptionPageIdString)>]
    type internal IntelliSenseOptionPage() =
        inherit AbstractOptionPage<IntelliSenseOptions>()
        override this.CreateView() =
            let view = IntelliSenseOptionControl()
            view.charTyped.Unchecked.Add <| fun _ -> view.charDeleted.IsChecked <- System.Nullable false

            let path = "EnterKeySetting"
            bindRadioButton view.nevernewline path EnterKeySetting.NeverNewline 
            bindRadioButton view.newlinecompleteline path EnterKeySetting.NewlineOnCompleteWord 
            bindRadioButton view.alwaysnewline path EnterKeySetting.AlwaysNewline

            upcast view

    [<Guid(Guids.quickInfoOptionPageIdString)>]
    type internal QuickInfoOptionPage() =
        inherit AbstractOptionPage<QuickInfoOptions>()
        override this.CreateView() = 
            let view = QuickInfoOptionControl()
            let path = "UnderlineStyle"
            bindRadioButton view.solid path QuickInfoUnderlineStyle.Solid
            bindRadioButton view.dot path QuickInfoUnderlineStyle.Dot
            bindRadioButton view.dash path QuickInfoUnderlineStyle.Dash
            bindCheckBox view.displayLinks "DisplayLinks"
            upcast view

    [<Guid(Guids.codeFixesOptionPageIdString)>]
    type internal CodeFixesOptionPage() =
        inherit AbstractOptionPage<CodeFixesOptions>()
        override this.CreateView() =
            upcast CodeFixesOptionControl()

    [<Guid(Guids.languageServicePerformanceOptionPageIdString)>]
    type internal LanguageServicePerformanceOptionPage() =
        inherit AbstractOptionPage<LanguageServicePerformanceOptions>()
        override this.CreateView() =
            upcast LanguageServicePerformanceOptionControl()

    [<Guid(Guids.advancedSettingsPageIdSring)>]
    type internal AdvancedSettingsOptionPage() =
        inherit AbstractOptionPage<AdvancedOptions>()
        override _.CreateView() =
            upcast AdvancedOptionsControl()

    [<Guid(Guids.formattingOptionPageIdString)>]
    type internal FormattingOptionPage() =
        inherit AbstractOptionPage<FormattingOptions>()
        override _.CreateView() =
            upcast FormattingOptionsControl()

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

        member this.IsFSharpBlockStructureEnabled =
            this.EditorOptions.Advanced.IsBlockStructureEnabled

        member this.IsFastFindReferencesEnabled =
            this.EditorOptions.LanguageServicePerformance.EnableFastFindReferences
