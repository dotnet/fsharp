namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.ComponentModel.Composition
open System.Runtime.InteropServices

open Microsoft.VisualStudio.FSharp.UIResources
open Microsoft.VisualStudio.Shell
open SettingsPersistence

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

type internal Settings() =

    static member Initialize(store: SettingsStore) =
        // Initialize default settings
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

    static member IntelliSense : IntelliSenseOptions = getSettings()
    static member QuickInfo : QuickInfoOptions = getSettings()
    static member CodeFixes : CodeFixesOptions = getSettings()
    static member LanguageServicePerformance : LanguageServicePerformanceOptions = getSettings()
    static member CodeLens : CodeLensOptions = getSettings()
    static member Advanced: AdvancedOptions = getSettings()

module internal OptionsUIHelpers =

    open System.Windows
    open System.Windows.Controls
    open System.Windows.Data
    open System.Windows.Markup

    open Microsoft.VisualStudio.ComponentModelHost

    [<AbstractClass>]
    type AbstractOptionPage<'t>() as this =
        inherit UIElementDialogPage()

        let view = lazy this.CreateView()
        
        let store =
            lazy
                let scm = this.Site.GetService(typeof<SComponentModel>) :?> IComponentModel
                // make sure settings are initialized to default values
                let settingsStore = scm.GetService<SettingsStore>()
                Settings.Initialize(settingsStore)
                settingsStore

        abstract CreateView : unit -> FrameworkElement

        member this.View = view.Value

        member this.Store = store.Value  

        override this.Child = upcast this.View

        override this.SaveSettingsToStorage() = 
            this.GetResult() |> this.Store.SaveSettings |> Async.StartImmediate

        override this.LoadSettingsFromStorage() = 
            this.Store.LoadSettings() |> this.SetViewModel

        //Override this method when using immutable settings type
        member this.SetViewModel(settings: 't) =
            // this is needed in case when settings are a CLIMutable record
            this.View.DataContext <- null
            this.View.DataContext <- settings

        //Override this method when using immutable settings type
        member this.GetResult() : 't =
            downcast this.View.DataContext

    //data binding helpers
    let radioButtonCoverter =
      { new IValueConverter with
            member this.Convert(value, _, parameter, _) =
                upcast value.Equals(parameter)
            member this.ConvertBack(value, _, parameter, _) =
                if value.Equals(true) then parameter else Binding.DoNothing }
                
    let bindRadioButton (radioButton: RadioButton) path value =
        let binding = Binding (path, Converter = radioButtonCoverter, ConverterParameter = value)
        radioButton.SetBinding(RadioButton.IsCheckedProperty, binding) |> ignore

    let bindCheckBox (checkBox: CheckBox) (path: string) =
        checkBox.SetBinding(CheckBox.IsCheckedProperty, path) |> ignore

    // some helpers to create option views in code instead of XAML
    let ( *** ) (control : #IAddChild) (children: UIElement list) =
        children |> List.iter control.AddChild
        control

    let ( +++ ) (control : #IAddChild) content = 
        control.AddChild content
        control

    let withDefaultStyles (element: FrameworkElement) =
        let groupBoxStyle = System.Windows.Style(typeof<GroupBox>)
        groupBoxStyle.Setters.Add(Setter(GroupBox.PaddingProperty, Thickness(Left = 7.0, Right = 7.0, Top = 7.0 )))
        groupBoxStyle.Setters.Add(Setter(GroupBox.MarginProperty, Thickness(Bottom = 3.0)))
        groupBoxStyle.Setters.Add(Setter(GroupBox.ForegroundProperty, DynamicResourceExtension(SystemColors.WindowTextBrushKey)))
        element.Resources.Add(typeof<GroupBox>, groupBoxStyle)
 
        let checkBoxStyle = new System.Windows.Style(typeof<CheckBox>)
        checkBoxStyle.Setters.Add(new Setter(CheckBox.MarginProperty, new Thickness(Bottom = 7.0 )))
        checkBoxStyle.Setters.Add(new Setter(CheckBox.ForegroundProperty, new DynamicResourceExtension(SystemColors.WindowTextBrushKey)))
        element.Resources.Add(typeof<CheckBox>, checkBoxStyle)
 
        let textBoxStyle = new System.Windows.Style(typeof<TextBox>)
        textBoxStyle.Setters.Add(new Setter(TextBox.MarginProperty, new Thickness(Left = 7.0, Right = 7.0 )))
        textBoxStyle.Setters.Add(new Setter(TextBox.ForegroundProperty, new DynamicResourceExtension(SystemColors.WindowTextBrushKey)))
        element.Resources.Add(typeof<TextBox>, textBoxStyle);
 
        let radioButtonStyle = new System.Windows.Style(typeof<RadioButton>)
        radioButtonStyle.Setters.Add(new Setter(RadioButton.MarginProperty, new Thickness(Bottom = 7.0 )))
        radioButtonStyle.Setters.Add(new Setter(RadioButton.ForegroundProperty, new DynamicResourceExtension(SystemColors.WindowTextBrushKey)))
        element.Resources.Add(typeof<RadioButton>, radioButtonStyle)
        element

module internal OptionsUI =

    open OptionsUIHelpers

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
