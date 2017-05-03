namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Windows
open System.Windows.Data
open System.Windows.Markup
open System.Windows.Controls

open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.ComponentModelHost

open SettingsPersistence

module internal OptionsUIHelpers =

    [<AbstractClass>]
    type AbstractOptionPage<'t>() as this =
        inherit UIElementDialogPage()

        let view = lazy this.CreateView()
        
        let store =
            lazy
                let scm = this.Site.GetService(typeof<SComponentModel>) :?> IComponentModel
                // make sure settings are initialized to default values
                scm.GetService<ISettings>() |> ignore
                scm.GetService<SettingsStore>()

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
