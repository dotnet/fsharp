namespace Microsoft.VisualStudio.FSharp.Editor

open System.Windows
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Editor.Shared.Utilities
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Editor.Shared.Extensions
open Microsoft.CodeAnalysis.Classification

type internal FSharpDeferredContent(content: TaggedText seq, typemap: ClassificationTypeMap) =
    let formatMap = typemap.ClassificationFormatMapService.GetClassificationFormatMap("tooltip")
    let inlines =
        seq { for t in content do
              let inl =
                  if t.Tag = TextTags.Class then  
                    let h = Documents.Hyperlink(Documents.Run(t.Text)) 
                    h.Click.Add <| fun _ ->
                        h.Foreground <- Media.Brushes.Red
                    h :> Documents.Inline
                  else
                    Documents.Run(t.Text) :> Documents.Inline
              let props = ClassificationTags.GetClassificationTypeName t.Tag
                          |> typemap.GetClassificationType
                          |> formatMap.GetTextProperties
              DependencyObjectExtensions.SetTextProperties(inl, props)
              yield inl }           
    
    interface IDeferredQuickInfoContent with
        member __.Create() =
            let tb = Controls.TextBlock(TextWrapping = TextWrapping.Wrap, TextTrimming = TextTrimming.None)
            DependencyObjectExtensions.SetDefaultTextProperties(tb, formatMap)
            tb.Inlines.AddRange(inlines)
            tb :> FrameworkElement



