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

open Microsoft.FSharp.Compiler.Range
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

module internal HyperlinkStyles =
    // TODO: move this one time initialization to a more suitable spot
    do Application.ResourceAssembly <- typeof<Microsoft.VisualStudio.FSharp.UIResources.Strings>.Assembly
    let private styles = ResourceDictionary(Source = Uri("HyperlinkStyles.xaml", UriKind.Relative))
    let getCurrent() : Style =
        let key =
            if Settings.QuickInfo.DisplayLinks then
                match Settings.QuickInfo.UnderlineStyle with
                | QuickInfoUnderlineStyle.Solid -> "solid_underline"
                | QuickInfoUnderlineStyle.Dot -> "dot_underline"
                | QuickInfoUnderlineStyle.Dash -> "dash_underline"
            else "no_underline"
        downcast styles.[key]

[<Export>]
type internal QuickInfoViewProvider
    [<ImportingConstructor>]
    (
        // lazy to try to mitigate #2756 (wrong tooltip font)
        typeMap: Lazy<ClassificationTypeMap>,
        glyphService: IGlyphService
    ) =

    let formatMap = lazy typeMap.Value.ClassificationFormatMapService.GetClassificationFormatMap "tooltip"

    let layoutTagToFormatting (layoutTag: LayoutTag) =
        layoutTag
        |> RoslynHelpers.roslynTag
        |> ClassificationTags.GetClassificationTypeName
        |> typeMap.Value.GetClassificationType
        |> formatMap.Value.GetTextProperties
    
    let formatText (navigation: QuickInfoNavigation) (content: Layout.TaggedText seq) : IDeferredQuickInfoContent =

        let navigateAndDismiss range _ =
            navigation.NavigateTo range
            SessionHandling.currentSession |> Option.iter ( fun session -> session.Dismiss() )

        let inlines = 
            seq { 
                for taggedText in content do
                    let run = Documents.Run taggedText.Text
                    let inl =
                        match taggedText with
                        | :? Layout.NavigableTaggedText as nav when navigation.IsTargetValid nav.Range ->                        
                            let h = Documents.Hyperlink(run, ToolTip = nav.Range.FileName)
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
            tb.Resources.[typeof<Documents.Hyperlink>] <- HyperlinkStyles.getCurrent()
            tb :> FrameworkElement
            
        { new IDeferredQuickInfoContent with member x.Create() = createTextLinks() }

    let empty = 
        { new IDeferredQuickInfoContent with 
            member x.Create() = TextBlock(Visibility = Visibility.Collapsed) :> FrameworkElement }

    let createDeferredContent (symbolGlyph, mainDescription, documentation) =
        QuickInfoDisplayDeferredContent(symbolGlyph, null, mainDescription, documentation, empty, empty, empty, empty)

    member __.ProvideContent(glyph: Glyph, description: TaggedText seq, documentation: TaggedText seq, navigation: QuickInfoNavigation) =
        let navigableText = formatText navigation
        let glyphContent = SymbolGlyphDeferredContent(glyph, glyphService)
        createDeferredContent(glyphContent, navigableText description, navigableText documentation)
