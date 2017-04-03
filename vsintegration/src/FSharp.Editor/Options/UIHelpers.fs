namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.ComponentModel
open System.Windows
open System.Windows.Markup
open System.Windows.Controls

open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.ComponentModelHost

open SettingsPersistence

module internal OptionsUIHelpers =

    type AbstractOptionPage<'t>(view: FrameworkElement) =
        inherit UIElementDialogPage()
        member this.Store =
            let sc = this.GetService(typeof<SComponentModel>) :?> IComponentModel
            sc.DefaultExportProvider.GetExport<SettingsStore>().Value
        override this.Child = upcast view
        override this.SaveSettingsToStorage() = this.Store.SaveSettings this.Result
        override this.LoadSettingsFromStorage() =
            let settings: 't = this.Store.LoadSettings()
            view.DataContext <- settings
        member this.Result : 't = downcast view.DataContext

    let ( *** ) (control : #IAddChild) (children: UIElement list) =
        children |> List.iter control.AddChild
        control

    let ( +++ ) (control : #IAddChild) content = 
        control.AddChild content
        control

    let withDefaultStyles (element: FrameworkElement) =
        let groupBoxStyle = System.Windows.Style(typeof<GroupBox>)
        groupBoxStyle.Setters.Add(Setter(GroupBox.PaddingProperty, Thickness(Left = 7.0, Right = 7.0, Top = 7.0 )))
        groupBoxStyle.Setters.Add(Setter(GroupBox.MarginProperty, Thickness( Bottom = 3.0)))
        groupBoxStyle.Setters.Add(Setter(GroupBox.ForegroundProperty, DynamicResourceExtension(SystemColors.WindowTextBrushKey)))
        element.Resources.Add(typeof<GroupBox>, groupBoxStyle)
 
        let checkBoxStyle = new System.Windows.Style(typeof<CheckBox>)
        checkBoxStyle.Setters.Add(new Setter(CheckBox.MarginProperty, new Thickness(Bottom = 7.0 )))
        checkBoxStyle.Setters.Add(new Setter(CheckBox.ForegroundProperty, new DynamicResourceExtension(SystemColors.WindowTextBrushKey)))
        element.Resources.Add(typeof<CheckBox>, checkBoxStyle)
 
        let textBoxStyle = new System.Windows.Style(typeof<TextBox>)
        textBoxStyle.Setters.Add(new Setter(TextBox.MarginProperty, new Thickness( Left = 7.0, Right = 7.0 )))
        textBoxStyle.Setters.Add(new Setter(TextBox.ForegroundProperty, new DynamicResourceExtension(SystemColors.WindowTextBrushKey)))
        element.Resources.Add(typeof<TextBox>, textBoxStyle);
 
        let radioButtonStyle = new System.Windows.Style(typeof<RadioButton>)
        radioButtonStyle.Setters.Add(new Setter(RadioButton.MarginProperty, new Thickness(Bottom = 7.0 )))
        radioButtonStyle.Setters.Add(new Setter(RadioButton.ForegroundProperty, new DynamicResourceExtension(SystemColors.WindowTextBrushKey)))
        element.Resources.Add(typeof<RadioButton>, radioButtonStyle)
        element

