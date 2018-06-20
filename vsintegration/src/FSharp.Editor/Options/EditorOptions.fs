namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.ComponentModel.Composition
open System.Runtime.InteropServices

open Microsoft.VisualStudio.FSharp.UIResources
open SettingsPersistence
open OptionsUIHelpers

module DefaultTuning = 
    let UnusedDeclarationsAnalyzerInitialDelay = 0 (* 1000 *) (* milliseconds *)
    let UnusedOpensAnalyzerInitialDelay = 0 (* 2000 *) (* milliseconds *)
    let SimplifyNameInitialDelay = 2000 (* milliseconds *)
    let SimplifyNameEachItemDelay = 0 (* milliseconds *)

    /// How long is the per-document data saved before it is eligible for eviction from the cache? 10 seconds.
    /// Re-tokenizing is fast so we don't need to save this data long.
    let PerDocumentSavedDataSlidingWindow = TimeSpan(0,0,10)(* seconds *)

// CLIMutable to make the record work also as a view model
[<CLIMutable>]
type IntelliSenseOptions =
  { ShowAfterCharIsTyped: bool
    ShowAfterCharIsDeleted: bool
    ShowAllSymbols : bool }

[<RequireQualifiedAccess>]
type QuickInfoUnderlineStyle = Dot | Dash | Solid

[<CLIMutable>]
type QuickInfoOptions =
    { DisplayLinks: bool
      UnderlineStyle: QuickInfoUnderlineStyle }

[<CLIMutable>]
type CodeFixesOptions =
    { SimplifyName: bool
      AlwaysPlaceOpensAtTopLevel: bool
      UnusedOpens: bool 
      UnusedDeclarations: bool }

[<CLIMutable>]
type LanguageServicePerformanceOptions = 
    { EnableInMemoryCrossProjectReferences: bool
      AllowStaleCompletionResults: bool
      TimeUntilStaleCompletion: int
      ProjectCheckCacheSize: int }

[<CLIMutable>]
type CodeLensOptions =
  { Enabled : bool
    ReplaceWithLineLens: bool 
    UseColors: bool
    Prefix : string }

[<CLIMutable>]
type AdvancedOptions =
    { IsBlockStructureEnabled: bool 
      IsOutliningEnabled: bool }

[<Export(typeof<ISettings>)>]
type internal Settings [<ImportingConstructor>](store: SettingsStore) =
    do  // Initialize default settings
        
        store.RegisterDefault
            { ShowAfterCharIsTyped = true
              ShowAfterCharIsDeleted = true
              ShowAllSymbols = true }

        store.RegisterDefault
            { DisplayLinks = true
              UnderlineStyle = QuickInfoUnderlineStyle.Solid }

        store.RegisterDefault
            { // We have this off by default, disable until we work out how to make this low priority 
              // See https://github.com/Microsoft/visualfsharp/pull/3238#issue-237699595
              SimplifyName = false 
              AlwaysPlaceOpensAtTopLevel = false
              UnusedOpens = true 
              UnusedDeclarations = true }

        store.RegisterDefault
            { EnableInMemoryCrossProjectReferences = true
              AllowStaleCompletionResults = true
              TimeUntilStaleCompletion = 2000 // In ms, so this is 2 seconds
              ProjectCheckCacheSize = 200 }

        store.RegisterDefault
            { IsBlockStructureEnabled = true 
              IsOutliningEnabled = true }

        store.RegisterDefault
            { Enabled = false
              UseColors = false
              ReplaceWithLineLens = true
              Prefix = "// " }

    interface ISettings

    static member IntelliSense : IntelliSenseOptions = getSettings()
    static member QuickInfo : QuickInfoOptions = getSettings()
    static member CodeFixes : CodeFixesOptions = getSettings()
    static member LanguageServicePerformance : LanguageServicePerformanceOptions = getSettings()
    static member CodeLens : CodeLensOptions = getSettings()
    static member Advanced: AdvancedOptions = getSettings()

module internal OptionsUI =

    [<Guid(Guids.intelliSenseOptionPageIdString)>]
    type internal IntelliSenseOptionPage() =
        inherit AbstractOptionPage<IntelliSenseOptions>()
        override this.CreateView() =
            let view = IntelliSenseOptionControl()
            view.charTyped.Unchecked.Add <| fun _ -> view.charDeleted.IsChecked <- System.Nullable false
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
