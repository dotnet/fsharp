namespace Microsoft.VisualStudio.FSharp.Editor

open System.Windows
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Editor.Shared.Utilities
open Microsoft.CodeAnalysis.Editor.Shared.Extensions
open Microsoft.CodeAnalysis.Classification

open Microsoft.FSharp.Compiler.Layout
open Microsoft.FSharp.Compiler.Range
open CommonRoslynHelpers

type internal FSharpDeferredContent(content: NavigableRoslynText seq, typemap: ClassificationTypeMap, canGoto, goTo) =

    let formatMap = typemap.ClassificationFormatMapService.GetClassificationFormatMap("tooltip")

    let props = 
        ClassificationTags.GetClassificationTypeName
        >> typemap.GetClassificationType
        >> formatMap.GetTextProperties

    let inlines = 
        seq { 
            for NavigableRoslynText(tag, text, rangeOpt) in content do
                let run =
                    match rangeOpt with
                    | Some(range) when canGoto range ->
                        let h = Documents.Hyperlink(Documents.Run(text))
                        h.Click.Add (fun _ -> goTo range |> Async.StartImmediate)
                        h :> Documents.Inline
                    | _ -> 
                        Documents.Run(text) :> Documents.Inline

                DependencyObjectExtensions.SetTextProperties(run, props tag)
                yield run
        }
    
    interface IDeferredQuickInfoContent with
        member __.Create() =
            let tb = Controls.TextBlock(TextWrapping = TextWrapping.Wrap, TextTrimming = TextTrimming.None)
            DependencyObjectExtensions.SetDefaultTextProperties(tb, formatMap)
            tb.Inlines.AddRange(inlines)
            if tb.Inlines.Count = 0 then tb.Visibility <- Visibility.Collapsed
            tb :> FrameworkElement

type internal EmptyQuickInfoContent() =
    interface IDeferredQuickInfoContent with
        member __.Create() = Controls.TextBlock(Visibility = Visibility.Collapsed) :> FrameworkElement
