namespace Microsoft.VisualStudio.FSharp.Editor

open System.ComponentModel.Composition
open System
open System.Windows
open System.Windows.Controls

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Editor.QuickInfo
open Microsoft.CodeAnalysis.Editor.Shared.Utilities
open Microsoft.CodeAnalysis.Editor.Shared.Extensions
open Microsoft.CodeAnalysis.Editor.Implementation.IntelliSense.QuickInfo

open Microsoft.VisualStudio.Language.Intellisense
open Microsoft.VisualStudio.Utilities
open Microsoft.VisualStudio.PlatformUI

open Microsoft.FSharp.Compiler

open Internal.Utilities.StructuredFormat
open Microsoft.VisualStudio.Text.Classification

module private SessionHandling =
    let mutable currentSession = None
    
    [<Export (typeof<IQuickInfoSourceProvider>)>]
    [<Name (FSharpProviderConstants.SessionCapturingProvider)>]
    [<Order (After = PredefinedQuickInfoProviderNames.Semantic)>]
    [<ContentType (FSharpConstants.FSharpContentTypeName)>]
    type SourceProviderForCapturingSession () =
        interface IQuickInfoSourceProvider with 
            member x.TryCreateQuickInfoSource _ =
              { new IQuickInfoSource with
                  member __.AugmentQuickInfoSession(session,_,_) = currentSession <- Some session
                  member __.Dispose() = () }

type internal FSharpQuickInfoDeferredContent (toTextBlock, layout) =
    member __.CreateFrameworkElement () = toTextBlock layout
    interface IDeferredQuickInfoContent

[<Export>]
type internal QuickInfoViewProvider
    [<ImportingConstructor>]
    (
        // lazy to try to mitigate #2756 (wrong tooltip font)
        typeMap: Lazy<ClassificationTypeMap>,
        classificationFormatMapService: IClassificationFormatMapService
    ) =

    let styles = ResourceDictionary(Source = Uri(@"/FSharp.UIResources;component/HyperlinkStyles.xaml", UriKind.Relative))

    let getStyle() : Style =
        let key =
            if Settings.QuickInfo.DisplayLinks then
                match Settings.QuickInfo.UnderlineStyle with
                | QuickInfoUnderlineStyle.Solid -> "solid_underline"
                | QuickInfoUnderlineStyle.Dot -> "dot_underline"
                | QuickInfoUnderlineStyle.Dash -> "dash_underline"
            else "no_underline"
        downcast styles.[key]

    let formatMap = lazy classificationFormatMapService.GetClassificationFormatMap "tooltip"

    let layoutTagToFormatting (layoutTag: LayoutTag) =
        layoutTag
        |> RoslynHelpers.roslynTag
        |> ClassificationTags.GetClassificationTypeName
        |> typeMap.Value.GetClassificationType
        |> formatMap.Value.GetTextProperties
    
    let formatText (navigation: QuickInfoNavigation) (content: #seq<Layout.TaggedText>) =

        let navigateAndDismiss range _ =
            navigation.NavigateTo range
            SessionHandling.currentSession |> Option.iter ( fun session -> session.Dismiss() )

        let secondaryToolTip range =
            let t = ToolTip(Content = navigation.RelativePath range)
            DependencyObjectExtensions.SetDefaultTextProperties(t, formatMap.Value)
            let color = VSColorTheme.GetThemedColor(EnvironmentColors.ToolTipBrushKey)
            t.Background <- Media.SolidColorBrush(Media.Color.FromRgb(color.R, color.G, color.B))
            t

        let toInline (taggedText: Layout.TaggedText) =
            let run = Documents.Run taggedText.Text
            let inl =
                match taggedText with
                | :? Layout.NavigableTaggedText as nav when navigation.IsTargetValid nav.Range ->                        
                    let h = Documents.Hyperlink(run, ToolTip = secondaryToolTip nav.Range)
                    h.Click.Add <| navigateAndDismiss nav.Range
                    h :> Documents.Inline
                | _ -> run :> _
            DependencyObjectExtensions.SetTextProperties (inl, layoutTagToFormatting taggedText.Tag)
            inl

        let tb = TextBlock(TextWrapping = TextWrapping.Wrap, TextTrimming = TextTrimming.None)
        DependencyObjectExtensions.SetDefaultTextProperties(tb, formatMap.Value)
        tb.Inlines.AddRange(content |> Seq.map toInline)
        if tb.Inlines.Count = 0 then tb.Visibility <- Visibility.Collapsed
        tb.Resources.[typeof<Documents.Hyperlink>] <- getStyle()
        tb

    let wrap (tb: TextBlock) =
        // Formula to make max width of the TextBlock proportional to the tooltip font size.
        // We need it, because the ascii-art divider inserted into xml documentation is of variable length and could wrap otherwise
        let maxWidth = formatMap.Value.DefaultTextProperties.FontRenderingEmSize * 60.0
        tb.MaxWidth <- maxWidth
        tb.HorizontalAlignment <- HorizontalAlignment.Left
        tb
        
    member __.ProvideContent(glyph: Glyph, description, documentation, typeParameterMap, usage, exceptions, navigation: QuickInfoNavigation) =
        let navigable x = FSharpQuickInfoDeferredContent((formatText navigation), x)
        let wrapped x = FSharpQuickInfoDeferredContent((formatText navigation >> wrap), x)
        let empty = FSharpQuickInfoDeferredContent((fun _ -> TextBlock(Visibility = Visibility.Collapsed)), Seq.empty)
        let glyphContent = SymbolGlyphDeferredContent(glyph)
        QuickInfoDisplayDeferredContent
            (glyphContent, null, 
             mainDescription = navigable description, 
             documentation = wrapped documentation,
             typeParameterMap = navigable typeParameterMap, 
             anonymousTypes = empty, 
             usageText = navigable usage, 
             exceptionText = navigable exceptions)
             //capturesText = empty)

[<Export (typeof<IDeferredQuickInfoContentToFrameworkElementConverter>)>] 
type FSharpDeferredContentConverter () =
    interface IDeferredQuickInfoContentToFrameworkElementConverter with
        member this.CreateFrameworkElement(deferredContent, _factory) =
            let fsharpDeferredContent = deferredContent :?> FSharpQuickInfoDeferredContent
            upcast fsharpDeferredContent.CreateFrameworkElement()
        member this.GetApplicableType () =
            typeof<FSharpQuickInfoDeferredContent>
