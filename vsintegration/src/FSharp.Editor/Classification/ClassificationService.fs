// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Generic
open System.Diagnostics
open System.Threading
open System.Runtime.Caching

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Classification

// IEditorClassificationService is marked as Obsolete, but is still supported. The replacement (IClassificationService)
// is internal to Microsoft.CodeAnalysis.Workspaces which we don't have internals visible to. Rather than add yet another
// IVT, we'll maintain the status quo.
#nowarn "44"

open FSharp.Compiler.SourceCodeServices

type SemanticClassificationData = (struct(FSharp.Compiler.Range.range * SemanticClassificationType)[])
type SemanticClassificationLookup = IReadOnlyDictionary<int, ResizeArray<struct(FSharp.Compiler.Range.range * SemanticClassificationType)>>

[<Sealed>]
type DocumentCache<'Value when 'Value : not struct>() =
    let cache = new MemoryCache("fsharp-cache")
    let policy = CacheItemPolicy(SlidingExpiration = TimeSpan.FromSeconds 2.)

    member _.TryGetValueAsync(doc: Document) = async {
        let! ct = Async.CancellationToken
        let! currentVersion = doc.GetTextVersionAsync ct |> Async.AwaitTask

        match cache.Get(doc.Id.ToString()) with
        | null -> return ValueNone
        | :? (Microsoft.CodeAnalysis.VersionStamp * 'Value) as value ->
            if fst value = currentVersion then
                return ValueSome(snd value)
            else
                return ValueNone
        | _ ->
            return ValueNone }

    member _.SetAsync(doc: Document, value: 'Value) = async {
            let! ct = Async.CancellationToken
            let! currentVersion = doc.GetTextVersionAsync ct |> Async.AwaitTask
            cache.Set(doc.Id.ToString(), (currentVersion, value), policy) }

    interface IDisposable with

        member _.Dispose() = cache.Dispose()

[<Export(typeof<IFSharpClassificationService>)>]
type internal FSharpClassificationService
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: FSharpProjectOptionsManager
    ) =
    static let userOpName = "SemanticColorization"

    static let addSemanticClassification sourceText (targetSpan: TextSpan) items (outputResult: List<ClassifiedSpan>) =
        for struct(range, classificationType) in items do
            match RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, range) with
            | None -> ()
            | Some span -> 
                let span = 
                    match classificationType with
                    | SemanticClassificationType.Printf -> span
                    | _ -> Tokenizer.fixupSpan(sourceText, span)
                if targetSpan.Contains span then
                    outputResult.Add(ClassifiedSpan(span, FSharpClassificationTypes.getClassificationTypeName(classificationType)))

    static let addSemanticClassificationByLookup sourceText (targetSpan: TextSpan) (lookup: SemanticClassificationLookup) (outputResult: List<ClassifiedSpan>) =
        let r = RoslynHelpers.TextSpanToFSharpRange("", targetSpan, sourceText)
        for i = r.StartLine to r.EndLine do
            match lookup.TryGetValue i with
            | true, items -> addSemanticClassification sourceText targetSpan items outputResult
            | _ -> ()

    static let toSemanticClassificationLookup (data: SemanticClassificationData) =
        let lookup = System.Collections.Generic.Dictionary<int, ResizeArray<struct(FSharp.Compiler.Range.range * SemanticClassificationType)>>()
        for i = 0 to data.Length - 1 do
            let (struct(r, _) as dataItem) = data.[i]
            let items =
                match lookup.TryGetValue r.StartLine with
                | true, items -> items
                | _ ->
                    let items = ResizeArray()
                    lookup.[r.StartLine] <- items
                    items
            items.Add dataItem
        System.Collections.ObjectModel.ReadOnlyDictionary lookup :> IReadOnlyDictionary<_, _>

    let semanticClassificationCache = new DocumentCache<SemanticClassificationLookup>()

    interface IFSharpClassificationService with
        // Do not perform classification if we don't have project options (#defines matter)
        member __.AddLexicalClassifications(_: SourceText, _: TextSpan, _: List<ClassifiedSpan>, _: CancellationToken) = ()
        
        member __.AddSyntacticClassificationsAsync(document: Document, textSpan: TextSpan, result: List<ClassifiedSpan>, cancellationToken: CancellationToken) =
            async {
                use _logBlock = Logger.LogBlock(LogEditorFunctionId.Classification_Syntactic)

                let defines = projectInfoManager.GetCompilationDefinesForEditingDocument(document)  
                let! sourceText = document.GetTextAsync(cancellationToken)  |> Async.AwaitTask

                // For closed documents, only get classification for the text within the span.
                // This may be inaccurate for multi-line tokens such as string literals, but this is ok for now
                //     as it's better than having to tokenize a big part of a file.
                // This is to handle syntactic classification for find all references.
                if not (document.Project.Solution.Workspace.IsDocumentOpen document.Id) then
                    result.AddRange(Tokenizer.getClassifiedSpans(document.Id, sourceText.GetSubText(textSpan), TextSpan(0, textSpan.Length), None, defines, cancellationToken))
                else
                    result.AddRange(Tokenizer.getClassifiedSpans(document.Id, sourceText, textSpan, Some(document.FilePath), defines, cancellationToken))
            } |> RoslynHelpers.StartAsyncUnitAsTask cancellationToken

        member __.AddSemanticClassificationsAsync(document: Document, textSpan: TextSpan, result: List<ClassifiedSpan>, cancellationToken: CancellationToken) =
            asyncMaybe {
                use _logBlock = Logger.LogBlock(LogEditorFunctionId.Classification_Semantic)

                let! _, _, projectOptions = projectInfoManager.TryGetOptionsForDocumentOrProject(document, cancellationToken)
                let! sourceText = document.GetTextAsync(cancellationToken)

                // If we are trying to get semantic classification for a document that is not open, get the results from the background and cache it.
                // We do this for find all references when it is populating results. 
                // We cache it temporarily so we do not have to continously call into the checker and perform a background operation.
                if not (document.Project.Solution.Workspace.IsDocumentOpen document.Id) then
                    match! semanticClassificationCache.TryGetValueAsync document |> liftAsync with
                    | ValueSome classificationDataLookup ->
                        addSemanticClassificationByLookup sourceText textSpan classificationDataLookup result
                    | _ ->
                        let! classificationData = checkerProvider.Checker.GetBackgroundSemanticClassificationForFile(document.FilePath, projectOptions, userOpName=userOpName) |> liftAsync
                        let classificationDataLookup = toSemanticClassificationLookup classificationData
                        do! semanticClassificationCache.SetAsync(document, classificationDataLookup) |> liftAsync
                        addSemanticClassificationByLookup sourceText textSpan classificationDataLookup result
                else
                    let! _, _, checkResults = checkerProvider.Checker.ParseAndCheckDocument(document, projectOptions, sourceText = sourceText, allowStaleResults = false, userOpName=userOpName) 
                    let targetRange = RoslynHelpers.TextSpanToFSharpRange(document.FilePath, textSpan, sourceText)
                    let classificationData = checkResults.GetSemanticClassification (Some targetRange)
                    addSemanticClassification sourceText textSpan classificationData result
            } 
            |> Async.Ignore |> RoslynHelpers.StartAsyncUnitAsTask cancellationToken

        // Do not perform classification if we don't have project options (#defines matter)
        member __.AdjustStaleClassification(_: SourceText, classifiedSpan: ClassifiedSpan) : ClassifiedSpan = classifiedSpan


