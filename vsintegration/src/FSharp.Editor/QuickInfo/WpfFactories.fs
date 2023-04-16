// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor.QuickInfo

open System.ComponentModel.Composition
open System.Windows
open System.Windows.Controls

open Microsoft.VisualStudio.Text.Adornments
open Microsoft.VisualStudio.Utilities

open Microsoft.VisualStudio.FSharp

type FSharpStyle =
    | Separator
    | Paragraph
    | CustomLinkStyle

    // Render as strings for cross platform look.
    override this.ToString() =
        match this with
        | Separator -> Editor.XmlDocumentation.separatorText
        | Paragraph -> System.Environment.NewLine
        | CustomLinkStyle -> ""

// Provide nicer look for the QuickInfo on Windows.
[<Export(typeof<IViewElementFactory>)>]
[<Name("FSharpStyle to UIElement")>]
[<TypeConversion(typeof<FSharpStyle>, typeof<UIElement>)>]
type WpfFSharpStyleFactory [<ImportingConstructor>] (settings: Editor.EditorOptions) =
    let linkStyleUpdater () =
        let key =
            if settings.QuickInfo.DisplayLinks then
                $"{settings.QuickInfo.UnderlineStyle.ToString().ToLower()}_underline"
            else
                "no_underline"

        let style = UIResources.NavStyles().Resources[key] :?> Style

        // Some assumptions are made here about the shape of QuickInfo visual tree rendered by VS.
        // If some future VS update were to render QuickInfo with different WPF elements
        // the links will still work, just without their custom styling.
        let rec styleLinks (element: DependencyObject) =
            match element with
            | :? TextBlock as t ->
                for run in t.Inlines do
                    if run :? Documents.Hyperlink then
                        run.Style <- style
            | :? Panel as p ->
                for e in p.Children do
                    styleLinks e
            | _ -> ()

        // Return an invisible FrameworkElement which will traverse it's siblings
        // to find HyperLinks and update their style, when inserted into the visual tree.
        { new FrameworkElement() with
            override this.OnVisualParentChanged _ = styleLinks this.Parent
        }

    interface IViewElementFactory with
        member _.CreateViewElement(_, model: obj) =
            match model with
            | :? FSharpStyle as fSharpStyle ->
                match fSharpStyle with
                | CustomLinkStyle -> linkStyleUpdater ()
                | Separator -> Controls.Separator(Opacity = 0.3, Margin = Thickness(0, 8, 0, 8))
                | Paragraph -> Controls.Separator(Opacity = 0)
                |> box
                :?> _
            | _ -> failwith $"Invalid type conversion.  Supported conversion is {typeof<FSharpStyle>.Name} to {typeof<UIElement>.Name}."
