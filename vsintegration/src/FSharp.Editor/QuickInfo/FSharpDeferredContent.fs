namespace Microsoft.VisualStudio.FSharp.Editor

open System.Windows
open System.Windows.Controls
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Editor.Shared.Utilities
open Microsoft.CodeAnalysis.Editor.Shared.Extensions
open Microsoft.CodeAnalysis.Editor.Implementation.IntelliSense.QuickInfo
open Microsoft.CodeAnalysis.Classification

open Microsoft.FSharp.Compiler.Range
open CommonRoslynHelpers

type internal FSharpDeferredContent(content: NavigableRoslynText seq, typemap: ClassificationTypeMap, canGoto, goTo) =

    let formatMap = typemap.ClassificationFormatMapService.GetClassificationFormatMap("tooltip")

    let props = 
        ClassificationTags.GetClassificationTypeName
        >> typemap.GetClassificationType
        >> formatMap.GetTextProperties

    let inlines = seq { 
        for NavigableRoslynText(tag, text, rangeOpt) in content do
            let run =
                match rangeOpt with
                | Some(range) when canGoto range ->
                    let h = Documents.Hyperlink(Documents.Run(text), ToolTip = range.FileName)
                    h.Click.Add <| fun _ ->
                        Forms.SendKeys.Send("{ESC}") //TODO: Dismiss QuickInfo properly, maybe using IQuickInfoSession.Dismiss()
                        goTo range |> Async.StartImmediate
                    h :> Documents.Inline
                | _ -> 
                    Documents.Run(text) :> Documents.Inline

            DependencyObjectExtensions.SetTextProperties(run, props tag)
            yield run
    }
    
    interface IDeferredQuickInfoContent with
        member x.Create() =
            let tb = TextBlock(TextWrapping = TextWrapping.Wrap, TextTrimming = TextTrimming.None)
            DependencyObjectExtensions.SetDefaultTextProperties(tb, formatMap)
            tb.Inlines.AddRange(inlines)
            if tb.Inlines.Count = 0 then tb.Visibility <- Visibility.Collapsed
            tb :> FrameworkElement


type internal FSharpQuickInfoDisplayDeferredContent(symbolGlyph, mainDescription, documentation) =

    let empty = { 
        new IDeferredQuickInfoContent with 
        member x.Create() = TextBlock(Visibility = Visibility.Collapsed) :> FrameworkElement
    }
    let roslynQuickInfo = QuickInfoDisplayDeferredContent(symbolGlyph, null, mainDescription, documentation, empty, empty, empty, empty)

    interface IDeferredQuickInfoContent with
        member x.Create() = 
            let qi = roslynQuickInfo.Create()
            let style = Style(typeof<Documents.Hyperlink>)
            style.Setters.Add(Setter(Documents.Inline.TextDecorationsProperty, null))
            let trigger = DataTrigger(Binding = Data.Binding("IsMouseOver", Source = qi), Value = true)
            trigger.Setters.Add(Setter(Documents.Inline.TextDecorationsProperty, TextDecorations.Underline))
            style.Triggers.Add(trigger)
            qi.Resources.Add(typeof<Documents.Hyperlink>, style)
            qi
        