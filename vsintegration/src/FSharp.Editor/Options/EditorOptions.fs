namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.ComponentModel.Composition
open System.Runtime.InteropServices
open Microsoft.VisualStudio.Shell

open Microsoft.VisualStudio.FSharp.UIResources
open OptionsUIHelpers

module DefaultTuning = 
    let UnusedDeclarationsAnalyzerInitialDelay = 0 (* 1000 *) (* milliseconds *)
    let UnusedOpensAnalyzerInitialDelay = 0 (* 2000 *) (* milliseconds *)
    let SimplifyNameInitialDelay = 2000 (* milliseconds *)
    let SimplifyNameEachItemDelay = 0 (* milliseconds *)

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
        ShowAfterCharIsDeleted = true
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
      UnusedDeclarations: bool }
    static member Default =
      { // We have this off by default, disable until we work out how to make this low priority 
        // See https://github.com/Microsoft/visualfsharp/pull/3238#issue-237699595
        SimplifyName = false 
        AlwaysPlaceOpensAtTopLevel = true
        UnusedOpens = true 
        UnusedDeclarations = true }

[<CLIMutable>]
type LanguageServicePerformanceOptions = 
    { EnableInMemoryCrossProjectReferences: bool
      AllowStaleCompletionResults: bool
      TimeUntilStaleCompletion: int
      ProjectCheckCacheSize: int }
    static member Default =
      { EnableInMemoryCrossProjectReferences = true
        AllowStaleCompletionResults = true
        TimeUntilStaleCompletion = 2000 // In ms, so this is 2 seconds
        ProjectCheckCacheSize = 200 }

[<CLIMutable>]
type CodeLensOptions =
  { Enabled : bool
    ReplaceWithLineLens: bool
    UseColors: bool
    Prefix : string }
    static member Default =
      { Enabled = false
        UseColors = false
        ReplaceWithLineLens = true
        Prefix = "// " }

[<CLIMutable>]
type AdvancedOptions =
    { IsBlockStructureEnabled: bool
      IsOutliningEnabled: bool }
    static member Default =
      { IsBlockStructureEnabled = true
        IsOutliningEnabled = true }

[<CLIMutable>]
type FormattingOptions =
    { FormatOnPaste: bool }
    static member Default =
        { FormatOnPaste = true }

[<Export>]
[<Export(typeof<IPersistSettings>)>]
type EditorOptions 
    [<ImportingConstructor>] 
    (
      [<Import(typeof<SVsServiceProvider>)>] serviceProvider: IServiceProvider
    ) =

    let store = SettingsStore(serviceProvider)
        
    do
        store.Register QuickInfoOptions.Default
        store.Register CodeFixesOptions.Default
        store.Register LanguageServicePerformanceOptions.Default
        store.Register AdvancedOptions.Default
        store.Register IntelliSenseOptions.Default
        store.Register CodeLensOptions.Default
        store.Register FormattingOptions.Default

    member __.IntelliSense : IntelliSenseOptions = store.Read()
    member __.QuickInfo : QuickInfoOptions = store.Read()
    member __.CodeFixes : CodeFixesOptions = store.Read()
    member __.LanguageServicePerformance : LanguageServicePerformanceOptions = store.Read()
    member __.Advanced: AdvancedOptions = store.Read()
    member __.CodeLens: CodeLensOptions = store.Read()
    member __.Formatting : FormattingOptions = store.Read()

    interface Microsoft.CodeAnalysis.Host.IWorkspaceService

    interface IPersistSettings with
        member __.Read() = store.Read()
        member __.Write(settings) = store.Write(settings)


[<AutoOpen>]
module internal WorkspaceSettingFromDocumentExtension =
    type Microsoft.CodeAnalysis.Document with
        member this.FSharpOptions =
            this.Project.Solution.Workspace.Services.GetService() : EditorOptions

module internal OptionsUI =

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
    
    [<Guid(Guids.codeLensOptionPageIdString)>]
    type internal CodeLensOptionPage() =
        inherit AbstractOptionPage<CodeLensOptions>()
        override this.CreateView() =
            upcast CodeLensOptionControl()

    [<Guid(Guids.advancedSettingsPageIdSring)>]
    type internal AdvancedSettingsOptionPage() =
        inherit AbstractOptionPage<AdvancedOptions>()
        override __.CreateView() =
            upcast AdvancedOptionsControl()

    [<Guid(Guids.formattingOptionPageIdString)>]
    type internal FormattingOptionPage() =
        inherit AbstractOptionPage<FormattingOptions>()
        override __.CreateView() =
            upcast FormattingOptionsControl()
