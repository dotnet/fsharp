// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.QuickInfo

open System.ComponentModel.Composition
open System.Windows
open System.Windows.Controls

open Microsoft.VisualStudio.Text.Adornments
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.Utilities

open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.Text.Classification

type Separator =
    | Separator of visible: bool
    // preserve old behavior on mac
    override this.ToString() =
        match this with
        | Separator true -> XmlDocumentation.separatorText
        | _ -> System.Environment.NewLine

[<Export(typeof<IViewElementFactory>)>]
[<Name("ClassifiedTextElement to UIElement")>]
[<TypeConversion(typeof<ClassifiedTextElement>, typeof<UIElement>)>]
type WpfClassifiedTextElementFactory [<ImportingConstructor>]
    (
        classificationformatMapService: IClassificationFormatMapService,
        classificationTypeRegistry: IClassificationTypeRegistryService,
        settings: EditorOptions
    ) =
    let resources = Microsoft.VisualStudio.FSharp.UIResources.NavStyles().Resources
    let formatMap = classificationformatMapService.GetClassificationFormatMap("tooltip")

    interface IViewElementFactory with
        member _.CreateViewElement(_textView: ITextView, model: obj) =
            match model with
            | :? ClassifiedTextElement as text ->
                let tb = TextBlock()
                tb.FontSize <- formatMap.DefaultTextProperties.FontRenderingEmSize
                tb.FontFamily <- formatMap.DefaultTextProperties.Typeface.FontFamily
                tb.TextWrapping <- TextWrapping.Wrap

                for run in text.Runs do
                    let ctype =
                        classificationTypeRegistry.GetClassificationType(run.ClassificationTypeName)

                    let props = formatMap.GetTextProperties(ctype)
                    let inl = Documents.Run(run.Text, Foreground = props.ForegroundBrush)

                    match run.NavigationAction |> Option.ofObj with
                    | Some action ->
                        let link =
                            { new Documents.Hyperlink(inl) with
                                override _.OnClick() = action.Invoke()
                            }

                        let key =
                            match settings.QuickInfo.UnderlineStyle with
                            | QuickInfoUnderlineStyle.Solid -> "solid_underline"
                            | QuickInfoUnderlineStyle.Dash -> "dash_underline"
                            | QuickInfoUnderlineStyle.Dot -> "dot_underline"

                        link.Style <- downcast resources[key]
                        link.Foreground <- props.ForegroundBrush
                        tb.Inlines.Add(link)
                    | _ -> tb.Inlines.Add(inl)

                box tb :?> _
            | _ ->
                failwith
                    $"Invalid type conversion.  Supported conversion is {typeof<ClassifiedTextElement>.Name} to {typeof<UIElement>.Name}."

[<Export(typeof<IViewElementFactory>)>]
[<Name("Separator to UIElement")>]
[<TypeConversion(typeof<Separator>, typeof<UIElement>)>]
type WpfSeparatorFactory() =
    interface IViewElementFactory with
        member _.CreateViewElement(_, model: obj) =
            match model with
            | :? Separator as Separator visible ->
                if visible then
                    Controls.Separator(Opacity = 0.3, Margin = Thickness(0, 8, 0, 8))
                else
                    Controls.Separator(Opacity = 0)
                |> box
                :?> _
            | _ -> failwith $"Invalid type conversion.  Supported conversion is {typeof<Separator>.Name} to {typeof<UIElement>.Name}."
