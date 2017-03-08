namespace Microsoft.VisualStudio.FSharp.Editor

open System.Windows
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Editor.Shared.Utilities
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Editor.Shared.Extensions
open Microsoft.CodeAnalysis.Classification

open CommonRoslynHelpers

type internal FSharpDeferredContent(content: NavigableRoslynText seq, typemap: ClassificationTypeMap) =
    let formatMap = typemap.ClassificationFormatMapService.GetClassificationFormatMap("tooltip")
    let props = 
        ClassificationTags.GetClassificationTypeName
        >> typemap.GetClassificationType
        >> formatMap.GetTextProperties
    let inlines = seq { 
        for NavigableRoslynText(tag, text, xopt) in content do 
            let inl = match xopt with
                      | None -> Documents.Run(text) :> Documents.Inline
                      | Some xref ->
                        let h = Documents.Hyperlink(Documents.Run(text))
                        h.Click.Add <| fun _ ->
                            Logging.Logging.logInfof "click! %A" xref
                        h :> Documents.Inline
            DependencyObjectExtensions.SetTextProperties( inl, props tag)
            yield inl
    }
    
    interface IDeferredQuickInfoContent with
        member __.Create() =
            let tb = Controls.TextBlock(TextWrapping = TextWrapping.Wrap, TextTrimming = TextTrimming.None)
            DependencyObjectExtensions.SetDefaultTextProperties(tb, formatMap)
            tb.Inlines.AddRange(inlines)
            tb :> FrameworkElement



