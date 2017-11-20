namespace Microsoft.VisualStudio.FSharp.Editor

open System.ComponentModel.Composition
open System.Runtime.InteropServices

open Microsoft.VisualStudio.FSharp.UIResources
open SettingsPersistence
open OptionsUIHelpers


module DefaultTuning = 
    let SemanticColorizationInitialDelay = 0 (* milliseconds *)
    let UnusedDeclarationsAnalyzerInitialDelay = 0 (* 1000 *) (* milliseconds *)
    let UnusedOpensAnalyzerInitialDelay = 0 (* 2000 *) (* milliseconds *)
    let SimplifyNameInitialDelay = 2000 (* milliseconds *)
    let SimplifyNameEachItemDelay = 0 (* milliseconds *)

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
      UnusedOpens: bool }

[<CLIMutable>]
type LanguageServicePerformanceOptions = 
    { EnableInMemoryCrossProjectReferences: bool
      ProjectCheckCacheSize: int }

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
              UnusedOpens = true }

        store.RegisterDefault
            { EnableInMemoryCrossProjectReferences = true
              ProjectCheckCacheSize = 200 }

    interface ISettings

    static member IntelliSense : IntelliSenseOptions = getSettings()
    static member QuickInfo : QuickInfoOptions = getSettings()
    static member CodeFixes : CodeFixesOptions = getSettings()
    static member LanguageServicePerformance : LanguageServicePerformanceOptions = getSettings()

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