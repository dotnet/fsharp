namespace Microsoft.VisualStudio.FSharp.Editor

open System

open Microsoft.CodeAnalysis

open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler

type internal QuickInfoNavigation
    (
        gotoDefinitionService: FSharpGoToDefinitionService,
        initialDoc: Document,
        thisSymbolUseRange: range
    ) =
    let workspace = initialDoc.Project.Solution.Workspace
    let solution = workspace.CurrentSolution

    member __.IsTargetValid (range: range) =
        range <> rangeStartup &&
        range <> thisSymbolUseRange &&
        solution.TryGetDocumentIdFromFSharpRange (range, initialDoc.Project.Id) |> Option.isSome

    member __.RelativePath (range: range) =
        let targetUri = Uri(range.FileName)
        Uri(solution.FilePath).MakeRelativeUri(targetUri).ToString()
        |> Uri.UnescapeDataString

    member __.NavigateTo (range: range) = 
        asyncMaybe { 
            let targetPath = range.FileName 
            let! targetDoc = solution.TryGetDocumentFromFSharpRange (range, initialDoc.Project.Id)
            let! targetSource = targetDoc.GetTextAsync() 
            let! targetTextSpan = RoslynHelpers.TryFSharpRangeToTextSpan (targetSource, range)
            // to ensure proper navigation decsions we need to check the type of document the navigation call
            // is originating from and the target we're provided by default
            //  - signature files (.fsi) should navigate to other signature files 
            //  - implementation files (.fs) should navigate to other implementation files
            let (|Signature|Implementation|) filepath =
                if isSignatureFile filepath then Signature else Implementation
           
            match initialDoc.FilePath, targetPath with 
            | Signature, Signature 
            | Implementation, Implementation ->
                return gotoDefinitionService.TryNavigateToTextSpan (targetDoc, targetTextSpan)
            // adjust the target from signature to implementation
            | Implementation, Signature  ->
                return! gotoDefinitionService.NavigateToSymbolDefinitionAsync (targetDoc, targetSource, range) |> liftAsync
            // adjust the target from implmentation to signature
            | Signature, Implementation -> 
                return! gotoDefinitionService.NavigateToSymbolDeclarationAsync (targetDoc, targetSource, range) |> liftAsync
        } |> Async.Ignore |> Async.StartImmediate
