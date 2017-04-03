namespace Microsoft.VisualStudio.FSharp.Editor

open System.ComponentModel.Composition
open System.Runtime.InteropServices
open System.Windows
open System.Windows.Controls

open OptionsUIHelpers
open SettingsPersistence

//for convenient UI data binding we can use CLIMutable records
[<CLIMutable>]
type IntelliSenseOptions = 
    { ShowAfterCharIsTyped: bool
      ShowAfterCharIsDeleted: bool }

[<RequireQualifiedAccess>]
type QuickInfoUnderlineStyle = Dotted | Dashed | Solid | None

//autoproperties can be used to easily define defaults and faciliate data binding,
//but the properties shouldn't be mutated outside of the Options dialog UI
type QuickInfoOptions() = 
    member val UnderlineStyle = QuickInfoUnderlineStyle.Solid with get, set

type internal Settings =
    static member IntelliSense : IntelliSenseOptions = getCachedSettings()
    static member QuickInfo : QuickInfoOptions = getCachedSettings()

[<Export(typeof<ISettingsRegistration>)>]
type private SettingsRegistration() =
    interface ISettingsRegistration with
        member __.RegisterAll() =
            registerSettings <| { ShowAfterCharIsTyped = true; ShowAfterCharIsDeleted = false }
            registerSettings <| QuickInfoOptions()


module internal OptionsUI = 
    let intellisenseOptionsView =
        let cb1 = CheckBox(Content = SR.GetString "Show_completion_list_after_a_character_is_typed")
        let cb2 = CheckBox(Content = SR.GetString "Show_completion_list_after_a_character_is_deleted")
        cb1.SetBinding(CheckBox.IsCheckedProperty, "ShowAfterCharIsTyped") |> ignore
        cb2.SetBinding(CheckBox.IsCheckedProperty, "ShowAfterCharIsDeleted") |> ignore
        ScrollViewer (VerticalScrollBarVisibility = ScrollBarVisibility.Auto) 
        +++ StackPanel()
            *** [ GroupBox (Header = SR.GetString "Completion_Lists")
                    +++ StackPanel() 
                        *** [ cb1
                              StackPanel (Margin = Thickness(15.0, 0.0, 0.0, 0.0)) 
                              *** [ cb2 ] ] ]
        |> withDefaultStyles

    [<Guid("9b3c6b8a-754a-461d-9ebe-de1a682d57c1")>]
    type internal IntelliSenseOptionPage() =
        inherit AbstractOptionPage<IntelliSenseOptions>(intellisenseOptionsView)


