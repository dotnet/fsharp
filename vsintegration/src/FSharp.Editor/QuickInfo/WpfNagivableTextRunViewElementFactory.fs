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
        viewElementFactoryService:IViewElementFactoryService
    ) =
    let styles = ResourceDictionary(Source = Uri(@"/FSharp.UIResources;component/HyperlinkStyles.xaml", UriKind.Relative))
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
                        if Settings.QuickInfo.DisplayLinks then
                            match Settings.QuickInfo.UnderlineStyle with
                            | QuickInfoUnderlineStyle.Solid -> "solid_underline"
                            | QuickInfoUnderlineStyle.Dash -> "dash_underline"
                            | QuickInfoUnderlineStyle.Dot -> "dot_underline"
                        else
                            "no_underline"
                    styles.[key] :?> Style
                tb.Resources.[typeof<TextBlock>] <- underlineStyle
            | _ -> ()

            // add navigation
            convertedElement.MouseDown.Add(fun _ -> navigableTextRun.NavigateAction())
            convertedElement :> obj :?> 'TView
