namespace Microsoft.VisualStudio.FSharp.Editor

open System.ComponentModel.Composition
open System.Runtime.InteropServices
open System.Windows
open System.Windows.Controls
open OptionsUIHelpers
open SettingsPersistence

type IntellisenseOptions() =
    member val ShowCompletionAfterType = true with get, set
    member val ShowCompletionAfterDelete = false with get, set

[<RequireQualifiedAccess>]
type QuickInfoUnderlineStyle = Dotted | Dashed | Solid | None
type QuickInfoOptions() =
    member val UnderlineStyle = QuickInfoUnderlineStyle.Solid with get, set

[<Export>]
type internal KnownSettings [<ImportingConstructor>] (store: SettingsPersistence.SettingsStore) =
    member this.Intellisense : IntellisenseOptions = store.GetSettings()
    member this.QuickInfo : QuickInfoOptions = store.GetSettings()

module internal Settings = 
    let intelliSenseOptions() : IntellisenseOptions = getSettings()
    let quickInfoOptions() : QuickInfoOptions = getSettings()

module internal OptionsUI = 
    let intellisenseOptionsView =
        let cb1 = CheckBox(Content = SR.GetString "Show_completion_list_after_a_character_is_typed")
        let cb2 = CheckBox(Content = SR.GetString "Show_completion_list_after_a_character_is_deleted")
        cb1.SetBinding(CheckBox.IsCheckedProperty, "ShowCompletionAfterType") |> ignore
        cb2.SetBinding(CheckBox.IsCheckedProperty, "ShowCompletionAfterDelete") |> ignore
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
        inherit AbstractOptionPage<IntellisenseOptions>(intellisenseOptionsView)

