namespace Microsoft.VisualStudio.FSharp.Editor

open System.ComponentModel.Composition
open System
open System.Windows
open System.Windows.Controls

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Editor.Shared.Utilities
open Microsoft.CodeAnalysis.Editor.Shared.Extensions
open Microsoft.CodeAnalysis.Editor.Implementation.IntelliSense.QuickInfo

open Microsoft.VisualStudio.Language.Intellisense
open Microsoft.VisualStudio.Utilities
open Microsoft.VisualStudio.PlatformUI

open Microsoft.FSharp.Compiler

open Internal.Utilities.StructuredFormat

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


[<Export>]
type internal QuickInfoViewProvider
    [<ImportingConstructor>]
    (
        // lazy to try to mitigate #2756 (wrong tooltip font)
        typeMap: Lazy<ClassificationTypeMap>,
        glyphService: IGlyphService
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

    let formatMap = lazy typeMap.Value.ClassificationFormatMapService.GetClassificationFormatMap "tooltip"

    let layoutTagToFormatting (layoutTag: LayoutTag) =
        layoutTag
        |> RoslynHelpers.roslynTag
        |> ClassificationTags.GetClassificationTypeName
        |> typeMap.Value.GetClassificationType
        |> formatMap.Value.GetTextProperties
    
    let formatText (navigation: QuickInfoNavigation) (content: seq<Layout.TaggedText>) : IDeferredQuickInfoContent =

        let navigateAndDismiss range _ =
            navigation.NavigateTo range
            SessionHandling.currentSession |> Option.iter ( fun session -> session.Dismiss() )

        let secondaryToolTip range =
            let t = ToolTip(Content = navigation.RelativePath range)
            DependencyObjectExtensions.SetDefaultTextProperties(t, formatMap.Value)
            let color = VSColorTheme.GetThemedColor(EnvironmentColors.ToolTipBrushKey)
            t.Background <- Media.SolidColorBrush(Media.Color.FromRgb(color.R, color.G, color.B))
            t

        let inlines = 
            seq { 
                for taggedText in content do
                    let run = Documents.Run taggedText.Text
                    let inl =
                        match taggedText with
                        | :? Layout.NavigableTaggedText as nav when navigation.IsTargetValid nav.Range ->                        
                            let h = Documents.Hyperlink(run, ToolTip = secondaryToolTip nav.Range)
                            h.Click.Add <| navigateAndDismiss nav.Range
                            h :> Documents.Inline
                        | _ -> run :> _
                    DependencyObjectExtensions.SetTextProperties (inl, layoutTagToFormatting taggedText.Tag)
                    yield inl
            }

        let createTextLinks () =
            let tb = TextBlock(TextWrapping = TextWrapping.Wrap, TextTrimming = TextTrimming.None)
            DependencyObjectExtensions.SetDefaultTextProperties(tb, formatMap.Value)
            tb.Inlines.AddRange inlines
            if tb.Inlines.Count = 0 then tb.Visibility <- Visibility.Collapsed
            tb.Resources.[typeof<Documents.Hyperlink>] <- getStyle()
            tb :> FrameworkElement
            
        { new IDeferredQuickInfoContent with member x.Create() = createTextLinks() }

    let empty = 
        { new IDeferredQuickInfoContent with 
            member x.Create() = TextBlock(Visibility = Visibility.Collapsed) :> FrameworkElement }

    member __.ProvideContent(glyph: Glyph, description, documentation, typeParameterMap, usage, exceptions, navigation: QuickInfoNavigation) =
        let navigableText x = formatText navigation x
        let glyphContent = SymbolGlyphDeferredContent(glyph, glyphService)
        QuickInfoDisplayDeferredContent
            (glyphContent, null, 
             mainDescription = navigableText description, 
             documentation = navigableText documentation,
             typeParameterMap = navigableText typeParameterMap, 
             anonymousTypes = empty, 
             usageText = navigableText usage, 
             exceptionText = navigableText exceptions)
