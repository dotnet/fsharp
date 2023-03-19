// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.QuickInfo

open System.ComponentModel.Composition
open System.Windows
open System.Windows.Controls

open Microsoft.VisualStudio.Text.Adornments
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.Utilities

open Microsoft.VisualStudio.FSharp.Editor

type Separator = Separator

[<Export(typeof<IViewElementFactory>)>]
[<Name("QuickInfoElement to UIElement")>]
[<TypeConversion(typeof<ClassifiedTextRun>, typeof<UIElement>)>]
type WpfNavigableTextRunFactory [<ImportingConstructor>] (viewElementFactoryService: IViewElementFactoryService, settings: EditorOptions) =
    let resources = Microsoft.VisualStudio.FSharp.UIResources.NavStyles().Resources

    interface IViewElementFactory with
        member _.CreateViewElement(textView: ITextView, model: obj) =
            match model with
            | :? ClassifiedTextRun as classifiedTextRun ->
                // use the default converters to get a UIElement
                let classifiedTextElement = ClassifiedTextElement([ classifiedTextRun ])

                let convertedElement =
                    viewElementFactoryService.CreateViewElement<UIElement>(textView, classifiedTextElement)
                // Apply custom underline.
                match convertedElement with
                | :? TextBlock as tb when classifiedTextRun.NavigationAction <> null && settings.QuickInfo.DisplayLinks ->
                    match tb.Inlines.FirstInline with
                    | :? Documents.Hyperlink as hyperlink ->
                        let key =
                            match settings.QuickInfo.UnderlineStyle with
                            | QuickInfoUnderlineStyle.Solid -> "solid_underline"
                            | QuickInfoUnderlineStyle.Dash -> "dash_underline"
                            | QuickInfoUnderlineStyle.Dot -> "dot_underline"
                        // Fix color and apply styles.
                        hyperlink.Foreground <- hyperlink.Inlines.FirstInline.Foreground
                        hyperlink.Style <- downcast resources[key]
                    | _ -> ()
                | _ -> ()

                box convertedElement :?> _
            | _ ->
                failwith $"Invalid type conversion.  Supported conversion is {typeof<ClassifiedTextRun>.Name} to {typeof<UIElement>.Name}."

[<Export(typeof<IViewElementFactory>)>]
[<Name("Separator to UIElement")>]
[<TypeConversion(typeof<Separator>, typeof<UIElement>)>]
type WpfSeparatorFactory() =
    interface IViewElementFactory with
        member _.CreateViewElement(_, model: obj) =
            match model with
            | :? Separator -> Controls.Separator(Opacity = 0.4, Margin = Thickness(0, 10, 0, 10)) |> box :?> _
            | _ -> failwith $"Invalid type conversion.  Supported conversion is {typeof<Separator>.Name} to {typeof<UIElement>.Name}."
