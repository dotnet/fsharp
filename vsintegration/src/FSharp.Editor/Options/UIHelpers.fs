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

        // this replicates the logic used by Roslyn option pages. Some following comments have been copied from
        // https://github.com/dotnet/roslyn/blob/5b125935f891b3c20405459f8f7e1cdfdc2cfa3d/src/VisualStudio/Core/Impl/Options/AbstractOptionPage.cs
        let mutable needsLoadOnNextActivate = true

        let view = lazy this.CreateView()

        let optionService =
            // lazy, so GetService is called from UI thread
            lazy
                let scm = this.Site.GetService(typeof<SComponentModel>) :?> IComponentModel
                scm.GetService<SettingsStore.ISettingsStore>()

        abstract CreateView : unit -> FrameworkElement

        override this.Child = upcast view.Value

        override this.OnActivate _ =
            if needsLoadOnNextActivate then
                // It looks like the bindings do not always pick up new source, unless we cycle the DataContext like this
                view.Value.DataContext <- DependencyProperty.UnsetValue
                view.Value.DataContext <- optionService.Value.LoadSettings<'options>()
                needsLoadOnNextActivate <- false

        override this.SaveSettingsToStorage() = 
            downcast view.Value.DataContext |> optionService.Value.SaveSettings<'options>
            // Make sure we load the next time the page is activated, in case if options changed
            // programmatically between now and the next time the page is activated
            needsLoadOnNextActivate <- true

        override this.LoadSettingsFromStorage() =
            // This gets called in two situations:
            //
            // 1) during the initial page load when you first activate the page, before OnActivate
            //    is called.
            // 2) during the closing of the dialog via Cancel/close when options don't need to be
            //    saved. The intent here is the settings get reloaded so the next time you open the
            //    page they are properly populated.
            //
            // This second one is tricky, because we don't actually want to update our controls
            // right then, because they'd be wrong the next time the page opens -- it's possible
            // they may have been changed programmatically. Therefore, we'll set a flag so we load
            // next time         
            needsLoadOnNextActivate <- true

        member this.GetService<'T when 'T : not struct>() =
            let scm = this.Site.GetService(typeof<SComponentModel>) :?> IComponentModel
            scm.GetService<'T>()

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
