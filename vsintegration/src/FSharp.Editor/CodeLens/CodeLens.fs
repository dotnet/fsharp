// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

open System
open System.Windows.Controls
open System.Windows.Media
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.Text.Formatting
open System.ComponentModel.Composition
open Microsoft.VisualStudio.Utilities
open Microsoft.CodeAnalysis

type internal CodeLensAdornment(view:IWpfTextView) as this=
    /// <summary>
    /// Text view where the adornment is created.
    /// </summary>
    let view = view

    /// <summary>
    /// Get the interline layer. CodeLens belong there.
    /// </summary>
    let interlineLayer = view.GetAdornmentLayer(PredefinedAdornmentLayers.InterLine)
    do view.LayoutChanged.AddHandler (fun _ e -> this.OnLayoutChanged e)

    /// <summary>
    /// Entry point for CodeLens logic
    /// </summary>
    let needsCodeLens (line:ITextViewLine) = 
        // Dummy code. Giving lines which contain an a CodeLens
        let res = [0 .. line.Length - 1] |> Seq.exists(fun i -> line.Start.Add(i).GetChar() = 'a')

        (res, "Contains a")

    /// <summary>
    /// Handles required transformation depending on whether CodeLens are required or not required
    /// </summary>
    interface ILineTransformSource with
        override t.GetLineTransform(line, _, _) =
            let (applyCodeLens, _) = needsCodeLens line
            if applyCodeLens then
                // Give us space for CodeLens
                LineTransform(15., 1., 1.)
            else
                //Restore old transformation
                LineTransform()
                

    /// <summary>
    /// Handles whenever the text displayed in the view changes by adding the adornment to any reformatted lines
    /// </summary>
    /// <remarks><para>This event is raised whenever the rendered text displayed in the <see cref="ITextView"/> changes.</para>
    /// <para>It is raised whenever the view does a layout (which happens when DisplayTextLineContainingBufferPosition is called or in response to text or classification changes).</para>
    /// <para>It is also raised whenever the view scrolls horizontally or when its size changes.</para>
    /// </remarks>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event arguments.</param>
    member t.OnLayoutChanged (e:TextViewLayoutChangedEventArgs) =
        e.NewOrReformattedLines
        |> Seq.iter (fun i -> t.checkCodeLens(i) )

    /// <summary>
    /// Adds the CodeLens above the given line with the given result of needsCodeLens
    /// </summary>
    /// <param name="line">Line to check whether CodeLens are needed </param>
    member t.checkCodeLens line =
        let (applyCodeLens, text) = needsCodeLens line
        if applyCodeLens then
            let panel = StackPanel()
            let textBox = TextBlock(Width = 100., Background = Brushes.Transparent, Opacity = 0.5, Text = text)
            let border = Border()
            do border.BorderBrush <- SolidColorBrush(Colors.Black)
            do border.BorderThickness <- Windows.Thickness(1.)
            do border.Child <- textBox
            do panel.Children.Add(border) |> ignore
            let geometry = view.TextViewLines.GetMarkerGeometry(line.Extent)
            Canvas.SetLeft(panel, 0.)
            Canvas.SetTop(panel, geometry.Bounds.Top - 15.)
            do interlineLayer.AddAdornment(AdornmentPositioningBehavior.TextRelative, Nullable line.Extent, null, panel, null) |> ignore
        else
            // Code lens are not required anymore. Remove them
            interlineLayer.RemoveAdornmentsByVisualSpan(line.Extent)
    

[<Export(typeof<ILineTransformSourceProvider>); ContentType("text"); TextViewRole(PredefinedTextViewRoles.Document)>]
type internal CodeLensProvider() =
    let TextAdornments = Collections.Generic.List<IWpfTextView * CodeLensAdornment>()

    /// <summary>
    /// Returns an provider for the textView if already one has been created.
    /// Else create one.
    /// </summary>
    let getSuitableAdornmentProvider (textView:IWpfTextView) =
        let res = TextAdornments |> Seq.tryFind(fun (view, _) -> view = textView)
        match res with
        | Some (_, res) -> res
        | None -> 
            let provider = CodeLensAdornment(textView)
            TextAdornments.Add((textView, provider))
            provider

    interface ILineTransformSourceProvider with
        override t.Create textView = 
            getSuitableAdornmentProvider(textView) :> ILineTransformSource
