namespace Microsoft.VisualStudio.FSharp.Editor

open System.Windows
open System.Windows.Data
open System.Windows.Controls

open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.ComponentModelHost

module internal OptionsUIHelpers =

    [<AbstractClass>]
    type AbstractOptionPage<'t>() as this =
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
            this.GetResult() |> optionService.Value.Write

        override this.LoadSettingsFromStorage() = 
            optionService.Value.Read() |> this.SetViewModel

        //Override this method when using immutable settings type
        member __.SetViewModel(settings: 't) =
            // in case settings are a CLIMutable record
            view.Value.DataContext <- null
            view.Value.DataContext <- settings

        //Override this method when using immutable settings type
        member __.GetResult() : 't =
            downcast view.Value.DataContext

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
