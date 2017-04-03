namespace Microsoft.VisualStudio.FSharp.Editor

open System.ComponentModel.Composition
open System.Runtime.InteropServices
open System.Windows
open System.Windows.Controls
open OptionsUIHelpers

open SettingsPersistence
open Microsoft.VisualStudio.ComponentModelHost

type IntelliSenseOptions() =
    member val ShowAfterCharIsTyped = true with get, set
    member val ShowAfterCharIsDeleted = false with get, set

[<RequireQualifiedAccess>]
type QuickInfoUnderlineStyle = Dotted | Dashed | Solid | None
type QuickInfoOptions() =
    member val UnderlineStyle = QuickInfoUnderlineStyle.Solid with get, set

[<Export(typeof<ISettings>)>]
type internal Settings() =
    static member IntelliSense : IntelliSenseOptions = getCachedSettings()
    static member QuickInfo : QuickInfoOptions = getCachedSettings()

    interface ISettings with
        member __.RegisterAll() =
            registerSetting <| IntelliSenseOptions()
            registerSetting <| QuickInfoOptions()


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


