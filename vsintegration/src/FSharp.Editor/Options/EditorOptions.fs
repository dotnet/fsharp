namespace Microsoft.VisualStudio.FSharp.Editor

open System.ComponentModel.Composition
open System.Runtime.InteropServices

open Microsoft.VisualStudio.FSharp.UIResources
open SettingsPersistence
open OptionsUIHelpers

// CLIMutable to make the record work also as a view model
[<CLIMutable>]
type IntelliSenseOptions =
  { ShowAfterCharIsTyped: bool
    ShowAfterCharIsDeleted: bool }

[<RequireQualifiedAccess>]
type QuickInfoUnderlineStyle = Dot | Dash | Solid

//autoproperties can be used to both define defaults and faciliate data binding in WPF controls,
//but the type should otherwise be treated as immutable.
type QuickInfoOptions() =
    member val DisplayLinks = true with get, set
    member val UnderlineStyle = QuickInfoUnderlineStyle.Solid with get, set

type internal Settings =
    static member IntelliSense : IntelliSenseOptions = getCachedSettings()
    static member QuickInfo : QuickInfoOptions = getCachedSettings()

[<Export(typeof<ISettingsToRegister>)>]
type private Registration() =
    interface ISettingsToRegister with 
        /// Provides default settings values 
        /// and registers settings to be tracked for updates
        member __.RegisterAll (store: IRegisterSettings) =
            { ShowAfterCharIsTyped = true
              ShowAfterCharIsDeleted = false } |> store.RegisterSetting
            QuickInfoOptions() |> store.RegisterSetting

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


