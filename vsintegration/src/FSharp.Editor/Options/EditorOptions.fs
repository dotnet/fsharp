namespace Microsoft.VisualStudio.FSharp.Editor

open System.ComponentModel.Composition
open System.Runtime.InteropServices

open Microsoft.VisualStudio.FSharp.UIResources
open SettingsPersistence
open OptionsUIHelpers

//autoproperties can be used to conveniently define defaults and faciliate data binding,
//but the properties shouldn't be mutated outside of the Options dialog UI
type IntelliSenseOptions() =
    member val ShowAfterCharIsTyped = true with get, set
    member val ShowAfterCharIsDeleted = false with get, set

[<RequireQualifiedAccess>]
type QuickInfoUnderlineStyle = Dot | Dash | Solid
type QuickInfoOptions() =
    member val DisplayLinks = true with get, set
    member val UnderlineStyle = QuickInfoUnderlineStyle.Solid with get, set

type internal Settings =
    static member IntelliSense : IntelliSenseOptions = getCachedSettings()
    static member QuickInfo : QuickInfoOptions = getCachedSettings()

[<Export(typeof<ISettingsToRegister>)>]
type private Registration() =
    interface ISettingsToRegister with 
        member __.RegisterAll (store: IRegisterSettings) =
            IntelliSenseOptions() |> store.RegisterSetting
            QuickInfoOptions() |> store.RegisterSetting

module internal OptionsUI =

    [<Guid("9b3c6b8a-754a-461d-9ebe-de1a682d57c1")>]
    type internal IntelliSenseOptionPage() =
        inherit AbstractOptionPage<IntelliSenseOptions>()
        override this.CreateView() = 
            let view = IntelliSenseOptionControl()
            view.charTyped.Unchecked.Add <| fun _ -> view.charDeleted.IsChecked <- System.Nullable false
            upcast view              
            
    [<Guid("1e2b3290-4d67-41ff-a876-6f41f868e28f")>]
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


