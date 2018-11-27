namespace Microsoft.VisualStudio.FSharp.Editor

open System.Windows
open System.Windows.Data
open System.Windows.Controls

open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.ComponentModelHost

module internal OptionsUIHelpers =

    [<AbstractClass>]
    type AbstractOptionPage<'options>() as this =
        inherit UIElementDialogPage()

        let view = lazy this.CreateView()
        
        let optionService =
            // lazy, so GetService is called from UI thread
            lazy
                let scm = this.Site.GetService(typeof<SComponentModel>) :?> IComponentModel
                scm.GetService<IPersistSettings>()

        abstract CreateView : unit -> FrameworkElement

        override this.Child = upcast view.Value

        override this.SaveSettingsToStorage() = 
            downcast view.Value.DataContext |> optionService.Value.Write<'options>

        override this.LoadSettingsFromStorage() = 
            view.Value.DataContext <- optionService.Value.Read<'options>()

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
