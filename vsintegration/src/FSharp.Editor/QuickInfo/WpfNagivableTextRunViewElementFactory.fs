// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.ComponentModel.Composition
open System.Windows
open System.Windows.Controls

open Microsoft.VisualStudio.Text.Adornments
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.Utilities

[<Export(typeof<IViewElementFactory>)>]
[<Name("NavigableTextRun to UIElement")>]
[<TypeConversion(typeof<NavigableTextRun>, typeof<UIElement>)>]
[<Order>]
type WpfNavigableTextRunViewElementFactory
    [<ImportingConstructor>]
    (
        viewElementFactoryService:IViewElementFactoryService,
        settings: EditorOptions
    ) =
    let styles = Microsoft.VisualStudio.FSharp.UIResources.NavStyles()
    interface IViewElementFactory with
        override __.CreateViewElement<'TView when 'TView: not struct>(textView:ITextView, model:obj) : 'TView =
            if not (model :? NavigableTextRun) || typeof<'TView> <> typeof<UIElement> then
                failwith <| sprintf "Invalid type conversion.  Supported conversion is `%s` to `%s`." typeof<NavigableTextRun>.Name typeof<UIElement>.Name

            // use the default converters to get a UIElement
            let navigableTextRun = model :?> NavigableTextRun
            let classifiedTextRun = ClassifiedTextRun(navigableTextRun.ClassificationTypeName, navigableTextRun.Text)
            let classifiedTextElement = ClassifiedTextElement([classifiedTextRun])
            let convertedElement = viewElementFactoryService.CreateViewElement<UIElement>(textView, classifiedTextElement)

            // apply HTML-like styles
            match convertedElement with
            | :? TextBlock as tb ->
                let underlineStyle =
                    let key =
                        if settings.QuickInfo.DisplayLinks then
                            match settings.QuickInfo.UnderlineStyle with
                            | QuickInfoUnderlineStyle.Solid -> "solid_underline"
                            | QuickInfoUnderlineStyle.Dash -> "dash_underline"
                            | QuickInfoUnderlineStyle.Dot -> "dot_underline"
                        else
                            "no_underline"
                    styles.Resources.[key] :?> Style

                // we need to enclose the generated Inline, which is actually a Run element, 
                // because fancy styling does not seem to work properly with Runs
                let inl = tb.Inlines.FirstInline
                let color = inl.Foreground
                // clear the color here to make it inherit
                inl.ClearValue(Documents.TextElement.ForegroundProperty) |> ignore
                // this constructor inserts into TextBlock
                Documents.Underline(tb.Inlines.FirstInline, tb.ContentStart, Foreground = color) |> ignore
                tb.Resources.[typeof<Documents.Underline>] <- underlineStyle
            | _ -> ()

            // add navigation
            convertedElement.MouseDown.Add(fun _ -> navigableTextRun.NavigateAction())
            convertedElement :> obj :?> 'TView
