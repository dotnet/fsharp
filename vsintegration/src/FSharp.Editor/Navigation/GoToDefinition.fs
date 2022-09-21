// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Threading
open System.Threading.Tasks
open System.Collections.Immutable
open System.Diagnostics
open System.IO
open System.Linq

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.FindSymbols
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Navigation

open Microsoft.VisualStudio
open Microsoft.VisualStudio.Shell.Interop

open FSharp.Compiler
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.Symbols
open FSharp.Compiler.Tokenization


module private Symbol =
    let fullName (root: ISymbol) : string =
        let rec inner parts (sym: ISymbol) =
            match sym with
            | null ->
                parts
            // TODO: do we have any other terminating cases?
            | sym when sym.Kind = SymbolKind.NetModule || sym.Kind = SymbolKind.Assembly ->
                parts
            | sym when sym.MetadataName <> "" ->
                inner (sym.MetadataName :: parts) sym.ContainingSymbol
            | sym ->
                inner parts sym.ContainingSymbol

        inner [] root |> String.concat "."

module private FindDeclExternalType =
    let rec tryOfRoslynType (typesym: ITypeSymbol): FindDeclExternalType option =
        match typesym with
        | :? IPointerTypeSymbol as ptrparam ->
            tryOfRoslynType ptrparam.PointedAtType |> Option.map FindDeclExternalType.Pointer
        | :? IArrayTypeSymbol as arrparam ->
            tryOfRoslynType arrparam.ElementType |> Option.map FindDeclExternalType.Array
        | :? ITypeParameterSymbol as typaram ->
            Some (FindDeclExternalType.TypeVar typaram.Name)
        | :? INamedTypeSymbol as namedTypeSym ->
            namedTypeSym.TypeArguments
            |> Seq.map tryOfRoslynType
            |> List.ofSeq
            |> Option.ofOptionList
            |> Option.map (fun genericArgs ->
                FindDeclExternalType.Type (Symbol.fullName typesym, genericArgs))
        | _ ->
            Debug.Assert(false, sprintf "GoToDefinitionService: Unexpected Roslyn type symbol subclass: %O" (typesym.GetType()))
            None

module private FindDeclExternalParam =

    let tryOfRoslynParameter (param: IParameterSymbol): FindDeclExternalParam option =
        FindDeclExternalType.tryOfRoslynType param.Type
        |> Option.map (fun ty -> FindDeclExternalParam.Create(ty, param.RefKind <> RefKind.None))

    let tryOfRoslynParameters (paramSyms: ImmutableArray<IParameterSymbol>): FindDeclExternalParam list option =
        paramSyms
        |> Seq.map tryOfRoslynParameter
        |> Seq.toList
        |> Option.ofOptionList

module private ExternalSymbol =
    let rec ofRoslynSymbol (symbol: ISymbol) : (ISymbol * FindDeclExternalSymbol) list =
        let container = Symbol.fullName symbol.ContainingSymbol

        match symbol with
        | :? INamedTypeSymbol as typesym ->
            let fullTypeName = Symbol.fullName typesym

            let constructors =
                typesym.InstanceConstructors
                |> Seq.choose<_,ISymbol * FindDeclExternalSymbol> (fun methsym ->
                    FindDeclExternalParam.tryOfRoslynParameters methsym.Parameters
                    |> Option.map (fun args -> upcast methsym, FindDeclExternalSymbol.Constructor(fullTypeName, args))
                    )
                |> List.ofSeq
                
            (symbol, FindDeclExternalSymbol.Type fullTypeName) :: constructors

        | :? IMethodSymbol as methsym ->
            FindDeclExternalParam.tryOfRoslynParameters methsym.Parameters
            |> Option.map (fun args ->
                symbol, FindDeclExternalSymbol.Method(container, methsym.MetadataName, args, methsym.TypeParameters.Length))
            |> Option.toList

        | :? IPropertySymbol as propsym ->
            [upcast propsym, FindDeclExternalSymbol.Property(container, propsym.MetadataName)]

        | :? IFieldSymbol as fieldsym ->
            [upcast fieldsym, FindDeclExternalSymbol.Field(container, fieldsym.MetadataName)]

        | :? IEventSymbol as eventsym ->
            [upcast eventsym, FindDeclExternalSymbol.Event(container, eventsym.MetadataName)]

        | _ -> []

// TODO: Uncomment code when VS has a fix for updating the status bar.
type internal StatusBar(statusBar: IVsStatusbar) =
    let mutable _searchIcon = int16 Microsoft.VisualStudio.Shell.Interop.Constants.SBAI_Find :> obj

    let _clear() =
        // unfreeze the statusbar
        statusBar.FreezeOutput 0 |> ignore  
        statusBar.Clear() |> ignore
        
    member _.Message(_msg: string) =
        ()
        //let _, frozen = statusBar.IsFrozen()
        //// unfreeze the status bar
        //if frozen <> 0 then statusBar.FreezeOutput 0 |> ignore
        //statusBar.SetText msg |> ignore
        //// freeze the status bar
        //statusBar.FreezeOutput 1 |> ignore

    member this.TempMessage(_msg: string) =
        ()
        //this.Message msg
        //async {
        //    do! Async.Sleep 4000
        //    match statusBar.GetText() with
        //    | 0, currentText when currentText <> msg -> ()
        //    | _ -> clear()
        //}|> Async.Start
    
    member _.Clear() = () //clear()

    /// Animated magnifying glass that displays on the status bar while a symbol search is in progress.
    member _.Animate() : IDisposable = 
        //statusBar.Animation (1, &searchIcon) |> ignore
        { new IDisposable with
            member _.Dispose() = () } //statusBar.Animation(0, &searchIcon) |> ignore }

type internal FSharpGoToDefinitionNavigableItem(document, sourceSpan) =
    inherit FSharpNavigableItem(Glyph.BasicFile, ImmutableArray.Empty, document, sourceSpan)

[<RequireQualifiedAccess>]
type internal FSharpGoToDefinitionResult =
    | NavigableItem of FSharpNavigableItem
    | ExternalAssembly of FSharpSymbolUse * MetadataReference seq

type internal GoToDefinition(metadataAsSource: FSharpMetadataAsSourceService) =

    /// Use an origin document to provide the solution & workspace used to 
    /// find the corresponding textSpan and INavigableItem for the range
    let rangeToNavigableItem (range: range, document: Document) = 
        async {
            let fileName = try System.IO.Path.GetFullPath range.FileName with _ -> range.FileName
            let refDocumentIds = document.Project.Solution.GetDocumentIdsWithFilePath fileName
            if not refDocumentIds.IsEmpty then 
                let refDocumentId = refDocumentIds.First()
                let refDocument = document.Project.Solution.GetDocument refDocumentId
                let! cancellationToken = Async.CancellationToken
                let! refSourceText = refDocument.GetTextAsync(cancellationToken) |> Async.AwaitTask
                match RoslynHelpers.TryFSharpRangeToTextSpan (refSourceText, range) with 
                | None -> return None
                | Some refTextSpan -> return Some (FSharpGoToDefinitionNavigableItem (refDocument, refTextSpan))
            else return None
        }

    /// Helper function that is used to determine the navigation strategy to apply, can be tuned towards signatures or implementation files.
    member private _.FindSymbolHelper (originDocument: Document, originRange: range, sourceText: SourceText, preferSignature: bool) =
        asyncMaybe {
            let userOpName = "FindSymbolHelper"
            let! originTextSpan = RoslynHelpers.TryFSharpRangeToTextSpan (sourceText, originRange)
            let position = originTextSpan.Start
            let! lexerSymbol = originDocument.TryFindFSharpLexerSymbolAsync(position, SymbolLookupKind.Greedy, false, false, userOpName)
            let textLinePos = sourceText.Lines.GetLinePosition position
            let fcsTextLineNumber = Line.fromZ textLinePos.Line
            let lineText = (sourceText.Lines.GetLineFromPosition position).ToString()  
            let idRange = lexerSymbol.Ident.idRange

            let! _, checkFileResults = originDocument.GetFSharpParseAndCheckResultsAsync(nameof(GoToDefinition)) |> liftAsync
            let! fsSymbolUse = checkFileResults.GetSymbolUseAtLocation (fcsTextLineNumber, idRange.EndColumn, lineText, lexerSymbol.FullIsland)
            let symbol = fsSymbolUse.Symbol
            // if the tooltip was spawned in an implementation file and we have a range targeting
            // a signature file, try to find the corresponding implementation file and target the
            // desired symbol
            if isSignatureFile fsSymbolUse.FileName && preferSignature = false then 
                let fsfilePath = Path.ChangeExtension (originRange.FileName,"fs")
                if not (File.Exists fsfilePath) then return! None else
                let! implDoc = originDocument.Project.Solution.TryGetDocumentFromPath fsfilePath
                let! implSourceText = implDoc.GetTextAsync ()
                let! _, checkFileResults = implDoc.GetFSharpParseAndCheckResultsAsync(userOpName) |> liftAsync
                let symbolUses = checkFileResults.GetUsesOfSymbolInFile symbol
                let! implSymbol  = symbolUses |> Array.tryHead 
                let! implTextSpan = RoslynHelpers.TryFSharpRangeToTextSpan (implSourceText, implSymbol.Range)
                return FSharpGoToDefinitionNavigableItem (implDoc, implTextSpan)
            else
                let! targetDocument = originDocument.Project.Solution.TryGetDocumentFromFSharpRange fsSymbolUse.Range
                return! rangeToNavigableItem (fsSymbolUse.Range, targetDocument)
        }

    /// if the symbol is defined in the given file, return its declaration location, otherwise use the targetSymbol to find the first 
    /// instance of its presence in the provided source file. The first case is needed to return proper declaration location for
    /// recursive type definitions, where the first its usage may not be the declaration.
    member _.FindSymbolDeclarationInDocument(targetSymbolUse: FSharpSymbolUse, document: Document) = 
        asyncMaybe {
            let filePath = document.FilePath
            match targetSymbolUse.Symbol.DeclarationLocation with
            | Some decl when decl.FileName = filePath -> return decl
            | _ ->
                let! _, checkFileResults = document.GetFSharpParseAndCheckResultsAsync("FindSymbolDeclarationInDocument") |> liftAsync
                let symbolUses = checkFileResults.GetUsesOfSymbolInFile targetSymbolUse.Symbol
                let! implSymbol  = symbolUses |> Array.tryHead 
                return implSymbol.Range
        }

    member private this.FindDefinitionAtPosition(originDocument: Document, position: int, cancellationToken: CancellationToken) =
        asyncMaybe {
            let userOpName = "FindDefinitionAtPosition"
            let! sourceText = originDocument.GetTextAsync(cancellationToken)
            let textLine = sourceText.Lines.GetLineFromPosition position
            let textLinePos = sourceText.Lines.GetLinePosition position
            let textLineString = textLine.ToString()
            let fcsTextLineNumber = Line.fromZ textLinePos.Line
            let lineText = (sourceText.Lines.GetLineFromPosition position).ToString()  
            
            let preferSignature = isSignatureFile originDocument.FilePath
                
            let! lexerSymbol = originDocument.TryFindFSharpLexerSymbolAsync(position, SymbolLookupKind.Greedy, false, false, userOpName)
            let idRange = lexerSymbol.Ident.idRange

            let! _, checkFileResults = originDocument.GetFSharpParseAndCheckResultsAsync(userOpName) |> liftAsync
            let declarations = checkFileResults.GetDeclarationLocation (fcsTextLineNumber, lexerSymbol.Ident.idRange.EndColumn, textLineString, lexerSymbol.FullIsland, preferSignature)
            let! targetSymbolUse = checkFileResults.GetSymbolUseAtLocation (fcsTextLineNumber, idRange.EndColumn, lineText, lexerSymbol.FullIsland)

            match declarations with
            | FindDeclResult.ExternalDecl (assembly, targetExternalSym) ->
                let projectOpt = originDocument.Project.Solution.Projects |> Seq.tryFind (fun p -> p.AssemblyName.Equals(assembly, StringComparison.OrdinalIgnoreCase))
                match projectOpt with
                | Some project ->
                    let! symbols = SymbolFinder.FindSourceDeclarationsAsync(project, fun _ -> true)

                    let roslynSymbols =
                        symbols
                        |> Seq.collect ExternalSymbol.ofRoslynSymbol
                        |> Array.ofSeq

                    let! symbol =
                        roslynSymbols
                        |> Seq.tryPick (fun (sym, externalSym) ->
                            if externalSym = targetExternalSym then Some sym
                            else None
                            )

                    let! location = symbol.Locations |> Seq.tryHead
                    return (FSharpGoToDefinitionResult.NavigableItem(FSharpGoToDefinitionNavigableItem(project.GetDocument(location.SourceTree), location.SourceSpan)), idRange)
                | _ ->
                    let metadataReferences = originDocument.Project.MetadataReferences                        
                    return (FSharpGoToDefinitionResult.ExternalAssembly(targetSymbolUse, metadataReferences), idRange)

            | FindDeclResult.DeclFound targetRange -> 
                // If the file is not associated with a document, it's considered external.
                if not (originDocument.Project.Solution.ContainsDocumentWithFilePath(targetRange.FileName)) then
                    let metadataReferences = originDocument.Project.MetadataReferences
                    return (FSharpGoToDefinitionResult.ExternalAssembly(targetSymbolUse, metadataReferences), idRange)
                else
                    // if goto definition is called at we are alread at the declaration location of a symbol in
                    // either a signature or an implementation file then we jump to it's respective postion in thethe
                    if lexerSymbol.Range = targetRange then
                        // jump from signature to the corresponding implementation
                        if isSignatureFile originDocument.FilePath then
                            let implFilePath = Path.ChangeExtension (originDocument.FilePath,"fs")
                            if not (File.Exists implFilePath) then return! None else
                            let! implDocument = originDocument.Project.Solution.TryGetDocumentFromPath implFilePath
                        
                            let! targetRange = this.FindSymbolDeclarationInDocument(targetSymbolUse, implDocument)
                            let! implSourceText = implDocument.GetTextAsync(cancellationToken) |> liftTaskAsync
                            let! implTextSpan = RoslynHelpers.TryFSharpRangeToTextSpan (implSourceText, targetRange)
                            let navItem = FSharpGoToDefinitionNavigableItem (implDocument, implTextSpan)
                            return (FSharpGoToDefinitionResult.NavigableItem(navItem), idRange)
                        else // jump from implementation to the corresponding signature
                            let declarations = checkFileResults.GetDeclarationLocation (fcsTextLineNumber, lexerSymbol.Ident.idRange.EndColumn, textLineString, lexerSymbol.FullIsland, true)
                            match declarations with
                            | FindDeclResult.DeclFound targetRange -> 
                                let! sigDocument = originDocument.Project.Solution.TryGetDocumentFromPath targetRange.FileName
                                let! sigSourceText = sigDocument.GetTextAsync(cancellationToken) |> liftTaskAsync
                                let! sigTextSpan = RoslynHelpers.TryFSharpRangeToTextSpan (sigSourceText, targetRange)
                                let navItem = FSharpGoToDefinitionNavigableItem (sigDocument, sigTextSpan)
                                return (FSharpGoToDefinitionResult.NavigableItem(navItem), idRange)
                            | _ ->
                                return! None
                    // when the target range is different follow the navigation convention of 
                    // - gotoDefn origin = signature , gotoDefn destination = signature
                    // - gotoDefn origin = implementation, gotoDefn destination = implementation 
                    else
                        let! sigDocument = originDocument.Project.Solution.TryGetDocumentFromPath targetRange.FileName
                        let! sigSourceText = sigDocument.GetTextAsync(cancellationToken) |> liftTaskAsync
                        let! sigTextSpan = RoslynHelpers.TryFSharpRangeToTextSpan (sigSourceText, targetRange)
                        // if the gotodef call originated from a signature and the returned target is a signature, navigate there
                        if isSignatureFile targetRange.FileName && preferSignature then 
                            let navItem = FSharpGoToDefinitionNavigableItem (sigDocument, sigTextSpan)
                            return (FSharpGoToDefinitionResult.NavigableItem(navItem), idRange)
                        else // we need to get an FSharpSymbol from the targetRange found in the signature
                             // that symbol will be used to find the destination in the corresponding implementation file
                            let implFilePath =
                                // Bugfix: apparently sigDocument not always is a signature file
                                if isSignatureFile sigDocument.FilePath then Path.ChangeExtension (sigDocument.FilePath, "fs") 
                                else sigDocument.FilePath

                            let! implDocument = originDocument.Project.Solution.TryGetDocumentFromPath implFilePath
                        
                            let! targetRange = this.FindSymbolDeclarationInDocument(targetSymbolUse, implDocument)    
                        
                            let! implSourceText = implDocument.GetTextAsync () |> liftTaskAsync
                            let! implTextSpan = RoslynHelpers.TryFSharpRangeToTextSpan (implSourceText, targetRange)
                            let navItem = FSharpGoToDefinitionNavigableItem (implDocument, implTextSpan)
                            return (FSharpGoToDefinitionResult.NavigableItem(navItem), idRange)
                | _ ->
                    return! None
        }

    /// find the declaration location (signature file/.fsi) of the target symbol if possible, fall back to definition 
    member this.FindDeclarationOfSymbolAtRange(targetDocument: Document, symbolRange: range, targetSource: SourceText) =
        this.FindSymbolHelper(targetDocument, symbolRange, targetSource, preferSignature=true)

    /// find the definition location (implementation file/.fs) of the target symbol
    member this.FindDefinitionOfSymbolAtRange(targetDocument: Document, symbolRange: range, targetSourceText: SourceText) =
        this.FindSymbolHelper(targetDocument, symbolRange, targetSourceText, preferSignature=false)

    member this.FindDefinitionsForPeekTask(originDocument: Document, position: int, cancellationToken: CancellationToken) =
        this.FindDefinitionAtPosition(originDocument, position, cancellationToken)
        |> Async.map (
                Option.toArray
                >> Array.toSeq)
        |> RoslynHelpers.StartAsyncAsTask cancellationToken

    /// Construct a task that will return a navigation target for the implementation definition of the symbol 
    /// at the provided position in the document.
    member this.FindDefinitionTask(originDocument: Document, position: int, cancellationToken: CancellationToken) =
        this.FindDefinitionAtPosition(originDocument, position, cancellationToken)
        |> RoslynHelpers.StartAsyncAsTask cancellationToken

    /// Navigate to the positon of the textSpan in the provided document
    /// used by quickinfo link navigation when the tooltip contains the correct destination range.
    member _.TryNavigateToTextSpan(document: Document, textSpan: Microsoft.CodeAnalysis.Text.TextSpan, statusBar: StatusBar, cancellationToken: CancellationToken) =
        let navigableItem = FSharpGoToDefinitionNavigableItem(document, textSpan)
        let workspace = document.Project.Solution.Workspace
        let navigationService = workspace.Services.GetService<IFSharpDocumentNavigationService>()
        let navigationSucceeded = navigationService.TryNavigateToSpan(workspace, navigableItem.Document.Id, navigableItem.SourceSpan, cancellationToken)

        if not navigationSucceeded then 
            statusBar.TempMessage (SR.CannotNavigateUnknown())

    member _.NavigateToItem(navigableItem: FSharpNavigableItem, statusBar: StatusBar, cancellationToken: CancellationToken) =
        use __ = statusBar.Animate()

        statusBar.Message (SR.NavigatingTo())

        let workspace = navigableItem.Document.Project.Solution.Workspace
        let navigationService = workspace.Services.GetService<IFSharpDocumentNavigationService>()

        // Prefer open documents in the preview tab.
        let result = navigationService.TryNavigateToSpan(workspace, navigableItem.Document.Id, navigableItem.SourceSpan, cancellationToken)
            
        if result then 
            statusBar.Clear()
        else 
            statusBar.TempMessage (SR.CannotNavigateUnknown())

    /// Find the declaration location (signature file/.fsi) of the target symbol if possible, fall back to definition 
    member this.NavigateToSymbolDeclarationAsync(targetDocument: Document, targetSourceText: SourceText, symbolRange: range, statusBar: StatusBar, cancellationToken: CancellationToken) =
        asyncMaybe {
            let! item = this.FindDeclarationOfSymbolAtRange(targetDocument, symbolRange, targetSourceText)
            return this.NavigateToItem(item, statusBar, cancellationToken)
        }

    /// Find the definition location (implementation file/.fs) of the target symbol
    member this.NavigateToSymbolDefinitionAsync(targetDocument: Document, targetSourceText: SourceText, symbolRange: range, statusBar: StatusBar, cancellationToken: CancellationToken) =
        asyncMaybe {
            let! item = this.FindDefinitionOfSymbolAtRange(targetDocument, symbolRange, targetSourceText)
            return this.NavigateToItem(item, statusBar, cancellationToken)
        }

    member this.NavigateToExternalDeclaration(targetSymbolUse: FSharpSymbolUse, metadataReferences: seq<MetadataReference>, cancellationToken: CancellationToken, statusBar: StatusBar) =
        use __ = statusBar.Animate()
        statusBar.Message (SR.NavigatingTo())

        let textOpt =
            match targetSymbolUse.Symbol with
            | :? FSharpEntity as symbol ->
                symbol.TryGetMetadataText()
                |> Option.map (fun text -> text, symbol.DisplayName)
            | :? FSharpMemberOrFunctionOrValue as symbol ->
                symbol.ApparentEnclosingEntity.TryGetMetadataText()
                |> Option.map (fun text -> text, symbol.ApparentEnclosingEntity.DisplayName)
            | _ ->
                None

        let result =
            match textOpt with
            | Some (text, fileName) ->
                let tmpProjInfo, tmpDocInfo = 
                    MetadataAsSource.generateTemporaryDocument(
                        AssemblyIdentity(targetSymbolUse.Symbol.Assembly.QualifiedName), 
                        fileName, 
                        metadataReferences)
                let tmpShownDocOpt = metadataAsSource.ShowDocument(tmpProjInfo, tmpDocInfo.FilePath, SourceText.From(text.ToString()))
                match tmpShownDocOpt with
                | Some tmpShownDoc ->
                    let goToAsync =
                        asyncMaybe {
                            let! _, checkResults = tmpShownDoc.GetFSharpParseAndCheckResultsAsync("NavigateToExternalDeclaration") |> liftAsync
                            let! r =
                                let rec areTypesEqual (ty1: FSharpType) (ty2: FSharpType) =
                                    let ty1 = ty1.StripAbbreviations()
                                    let ty2 = ty2.StripAbbreviations()
                                    let generic =
                                        ty1.IsGenericParameter && ty2.IsGenericParameter ||
                                        (
                                            ty1.GenericArguments.Count = ty2.GenericArguments.Count &&
                                            (ty1.GenericArguments, ty2.GenericArguments)
                                            ||> Seq.forall2 areTypesEqual
                                        )                                                    
                                    if generic then
                                        true
                                    else
                                        let namesEqual = ty1.TypeDefinition.DisplayName = ty2.TypeDefinition.DisplayName
                                        let accessPathsEqual = ty1.TypeDefinition.AccessPath = ty2.TypeDefinition.AccessPath
                                        namesEqual && accessPathsEqual

                                // This tries to find the best possible location of the target symbol's location in the metadata source.
                                // We really should rely on symbol equality within FCS instead of doing it here, 
                                //     but the generated metadata as source isn't perfect for symbol equality.
                                checkResults.GetAllUsesOfAllSymbolsInFile(cancellationToken)
                                |> Seq.tryFind (fun x ->
                                    match x.Symbol, targetSymbolUse.Symbol with
                                    | (:? FSharpEntity as symbol1), (:? FSharpEntity as symbol2) when x.IsFromDefinition ->
                                        symbol1.DisplayName = symbol2.DisplayName
                                    | (:? FSharpMemberOrFunctionOrValue as symbol1), (:? FSharpMemberOrFunctionOrValue as symbol2) ->
                                        symbol1.DisplayName = symbol2.DisplayName &&
                                        symbol1.GenericParameters.Count = symbol2.GenericParameters.Count &&
                                        symbol1.CurriedParameterGroups.Count = symbol2.CurriedParameterGroups.Count &&
                                        (
                                            (symbol1.CurriedParameterGroups, symbol2.CurriedParameterGroups)
                                            ||> Seq.forall2 (fun pg1 pg2 ->
                                                pg1.Count = pg2.Count &&
                                                (
                                                    (pg1, pg2)
                                                    ||> Seq.forall2 (fun p1 p2 ->
                                                        areTypesEqual p1.Type p2.Type
                                                    )
                                                )
                                            )
                                        ) &&
                                        areTypesEqual symbol1.ReturnParameter.Type symbol2.ReturnParameter.Type
                                    | _ ->
                                        false
                                )
                                |> Option.map (fun x -> x.Range)

                            let span =
                                match RoslynHelpers.TryFSharpRangeToTextSpan(tmpShownDoc.GetTextAsync(cancellationToken).Result, r) with
                                | Some span -> span
                                | _ -> TextSpan()

                            return span                             
                        }

                    let span =
                        match Async.RunImmediateExceptOnUI(goToAsync, cancellationToken = cancellationToken) with
                        | Some span -> span
                        | _ -> TextSpan()

                    let navItem = FSharpGoToDefinitionNavigableItem(tmpShownDoc, span)                               
                    this.NavigateToItem(navItem, statusBar, cancellationToken)
                    true
                | _ ->
                    false
            | _ ->
                false

        if result then 
            statusBar.Clear()
        else 
            statusBar.TempMessage (SR.CannotNavigateUnknown())

type internal QuickInfo =
    { StructuredText: ToolTipText
      Span: TextSpan
      Symbol: FSharpSymbol option
      SymbolKind: LexerSymbolKind }

module internal FSharpQuickInfo =

    let userOpName = "QuickInfo"

    // when a construct has been declared in a signature file the documentation comments that are
    // written in that file are the ones that go into the generated xml when the project is compiled
    // therefore we should include these doccoms in our design time quick info
    let getQuickInfoFromRange
        (
            document: Document,
            declRange: range,
            cancellationToken: CancellationToken
        )
        : Async<QuickInfo option> =

        asyncMaybe {
            let userOpName = "getQuickInfoFromRange"
            let solution = document.Project.Solution
            // ascertain the location of the target declaration in the signature file
            let! extDocId = solution.GetDocumentIdsWithFilePath declRange.FileName |> Seq.tryHead
            let extDocument = solution.GetProject(extDocId.ProjectId).GetDocument extDocId
            let! extSourceText = extDocument.GetTextAsync cancellationToken
            let! extSpan = RoslynHelpers.TryFSharpRangeToTextSpan (extSourceText, declRange)
            let extLineText = (extSourceText.Lines.GetLineFromPosition extSpan.Start).ToString()

            // project options need to be retrieved because the signature file could be in another project
            let! extLexerSymbol = extDocument.TryFindFSharpLexerSymbolAsync(extSpan.Start, SymbolLookupKind.Greedy, true, true, userOpName)
            let! _, extCheckFileResults = extDocument.GetFSharpParseAndCheckResultsAsync(userOpName) |> liftAsync

            let extQuickInfoText = 
                extCheckFileResults.GetToolTip
                    (declRange.StartLine, extLexerSymbol.Ident.idRange.EndColumn, extLineText, extLexerSymbol.FullIsland, FSharpTokenTag.IDENT)

            match extQuickInfoText with
            | ToolTipText []
            | ToolTipText [ToolTipElement.None] -> return! None
            | extQuickInfoText  ->
                let! extSymbolUse =
                    extCheckFileResults.GetSymbolUseAtLocation(declRange.StartLine, extLexerSymbol.Ident.idRange.EndColumn, extLineText, extLexerSymbol.FullIsland)
                let! span = RoslynHelpers.TryFSharpRangeToTextSpan (extSourceText, extLexerSymbol.Range)

                return { StructuredText = extQuickInfoText
                         Span = span
                         Symbol = Some extSymbolUse.Symbol
                         SymbolKind = extLexerSymbol.Kind }
        }

    /// Get QuickInfo combined from doccom of Signature and definition
    let getQuickInfo
        (
            document: Document,
            position: int,
            cancellationToken: CancellationToken
        )
        : Async<(range * QuickInfo option * QuickInfo option) option> =

        asyncMaybe {
            let userOpName = "getQuickInfo"
            let! lexerSymbol = document.TryFindFSharpLexerSymbolAsync(position, SymbolLookupKind.Greedy, true, true, userOpName)
            let! _, checkFileResults = document.GetFSharpParseAndCheckResultsAsync(userOpName) |> liftAsync
            let! sourceText = document.GetTextAsync cancellationToken
            let idRange = lexerSymbol.Ident.idRange  
            let textLinePos = sourceText.Lines.GetLinePosition position
            let fcsTextLineNumber = Line.fromZ textLinePos.Line
            let lineText = (sourceText.Lines.GetLineFromPosition position).ToString()

            /// Gets the QuickInfo information for the orignal target
            let getTargetSymbolQuickInfo (symbol, tag) =
                asyncMaybe {
                    let targetQuickInfo =
                        match lexerSymbol.Kind with
                        | LexerSymbolKind.Keyword -> checkFileResults.GetKeywordTooltip(fcsTextLineNumber, idRange.EndColumn, lineText, lexerSymbol.FullIsland)
                        | _ ->
                            checkFileResults.GetToolTip
                                (fcsTextLineNumber, idRange.EndColumn, lineText, lexerSymbol.FullIsland,tag)

                    match targetQuickInfo with
                    | ToolTipText []
                    | ToolTipText [ToolTipElement.None] -> return! None
                    | _ ->
                        let! targetTextSpan = RoslynHelpers.TryFSharpRangeToTextSpan (sourceText, lexerSymbol.Range)
                        return { StructuredText = targetQuickInfo
                                 Span = targetTextSpan
                                 Symbol = symbol
                                 SymbolKind = lexerSymbol.Kind }
                }

            match lexerSymbol.Kind with 
            | LexerSymbolKind.Keyword
            | LexerSymbolKind.String ->
                let! targetQuickInfo = getTargetSymbolQuickInfo (None, FSharpTokenTag.STRING)
                return lexerSymbol.Range, None, Some targetQuickInfo
            
            | _ -> 
            let! symbolUse = checkFileResults.GetSymbolUseAtLocation (fcsTextLineNumber, idRange.EndColumn, lineText, lexerSymbol.FullIsland)

            // if the target is in a signature file, adjusting the quick info is unnecessary
            if isSignatureFile document.FilePath then
                let! targetQuickInfo = getTargetSymbolQuickInfo (Some symbolUse.Symbol, FSharpTokenTag.IDENT)
                return symbolUse.Range, None, Some targetQuickInfo
            else
                // find the declaration location of the target symbol, with a preference for signature files
                let findSigDeclarationResult = checkFileResults.GetDeclarationLocation (idRange.StartLine, idRange.EndColumn, lineText, lexerSymbol.FullIsland, preferFlag=true)

                // it is necessary to retrieve the backup quick info because this acquires
                // the textSpan designating where we want the quick info to appear.
                let! targetQuickInfo = getTargetSymbolQuickInfo (Some symbolUse.Symbol, FSharpTokenTag.IDENT)

                let! result =
                    match findSigDeclarationResult with 
                    | FindDeclResult.DeclFound declRange when isSignatureFile declRange.FileName ->
                        asyncMaybe {
                            let! sigQuickInfo = getQuickInfoFromRange(document, declRange, cancellationToken)

                            // if the target was declared in a signature file, and the current file
                            // is not the corresponding module implementation file for that signature,
                            // the doccoms from the signature will overwrite any doccoms that might be
                            // present on the definition/implementation
                            let findImplDefinitionResult = checkFileResults.GetDeclarationLocation (idRange.StartLine, idRange.EndColumn, lineText, lexerSymbol.FullIsland, preferFlag=false)

                            match findImplDefinitionResult  with
                            | FindDeclResult.DeclNotFound _
                            | FindDeclResult.ExternalDecl _ ->
                                return symbolUse.Range, Some sigQuickInfo, None
                            | FindDeclResult.DeclFound declRange ->
                                let! implQuickInfo = getQuickInfoFromRange(document, declRange, cancellationToken)
                                return symbolUse.Range, Some sigQuickInfo, Some { implQuickInfo with Span = targetQuickInfo.Span }
                        }
                    | _ -> async.Return None
                    |> liftAsync

                return result |> Option.defaultValue (symbolUse.Range, None, Some targetQuickInfo)
        }

type internal FSharpNavigation
    (
        statusBar: StatusBar,
        metadataAsSource: FSharpMetadataAsSourceService,
        initialDoc: Document,
        thisSymbolUseRange: range
    ) =

    let workspace = initialDoc.Project.Solution.Workspace
    let solution = workspace.CurrentSolution

    member _.IsTargetValid (range: range) =
        range <> rangeStartup &&
        range <> thisSymbolUseRange &&
        solution.TryGetDocumentIdFromFSharpRange (range, initialDoc.Project.Id) |> Option.isSome

    member _.RelativePath (range: range) =
        let relativePathEscaped =
            match solution.FilePath with
            | null -> range.FileName
            | sfp ->
                let targetUri = Uri(range.FileName)
                Uri(sfp).MakeRelativeUri(targetUri).ToString()
        relativePathEscaped |> Uri.UnescapeDataString

    member _.NavigateTo (range: range, cancellationToken: CancellationToken) =
        asyncMaybe {
            let targetPath = range.FileName
            let! targetDoc = solution.TryGetDocumentFromFSharpRange (range, initialDoc.Project.Id)
            let! targetSource = targetDoc.GetTextAsync(cancellationToken)
            let! targetTextSpan = RoslynHelpers.TryFSharpRangeToTextSpan (targetSource, range)
            let gtd = GoToDefinition(metadataAsSource)

            // To ensure proper navigation decsions, we need to check the type of document the navigation call
            // is originating from and the target we're provided by default:
            //  - signature files (.fsi) should navigate to other signature files 
            //  - implementation files (.fs) should navigate to other implementation files
            let (|Signature|Implementation|) filepath =
                if isSignatureFile filepath then Signature else Implementation
               
            match initialDoc.FilePath, targetPath with
            | Signature, Signature
            | Implementation, Implementation ->
                return gtd.TryNavigateToTextSpan(targetDoc, targetTextSpan, statusBar, cancellationToken)

            // Adjust the target from signature to implementation.
            | Implementation, Signature  ->
                return! gtd.NavigateToSymbolDefinitionAsync(targetDoc, targetSource, range, statusBar, cancellationToken)
                    
            // Adjust the target from implmentation to signature.
            | Signature, Implementation ->
                return! gtd.NavigateToSymbolDeclarationAsync(targetDoc, targetSource, range, statusBar, cancellationToken)
        }
        |> Async.Ignore |> Async.StartImmediate

    member _.FindDefinitions(position, cancellationToken) =
        let gtd = GoToDefinition(metadataAsSource)
        let task = gtd.FindDefinitionsForPeekTask(initialDoc, position, cancellationToken)
        task.Wait(cancellationToken)
        let results = task.Result
        results
        |> Seq.choose(fun (result, _) ->
            match result with
            | FSharpGoToDefinitionResult.NavigableItem(navItem) -> Some navItem
            | _ -> None
        )
        |> Task.FromResult

    member this.TryGoToDefinition(position, cancellationToken) =
        let gtd = GoToDefinition(metadataAsSource)
        let gtdTask = gtd.FindDefinitionTask(initialDoc, position, cancellationToken)
        
        // Wrap this in a try/with as if the user clicks "Cancel" on the thread dialog, we'll be cancelled.
        // Task.Wait throws an exception if the task is cancelled, so be sure to catch it.
        try
            // This call to Wait() is fine because we want to be able to provide the error message in the status bar.
            gtdTask.Wait(cancellationToken)
            if gtdTask.Status = TaskStatus.RanToCompletion && gtdTask.Result.IsSome then
                match gtdTask.Result.Value with
                | FSharpGoToDefinitionResult.NavigableItem(navItem), _ ->
                    gtd.NavigateToItem(navItem, statusBar, cancellationToken)
                    // 'true' means do it, like Sheev Palpatine would want us to.
                    true
                | FSharpGoToDefinitionResult.ExternalAssembly(targetSymbolUse, metadataReferences), _ ->
                    gtd.NavigateToExternalDeclaration(targetSymbolUse, metadataReferences, cancellationToken, statusBar)
                    // 'true' means do it, like Sheev Palpatine would want us to.
                    true
            else 
                statusBar.TempMessage (SR.CannotDetermineSymbol())
                false
        with exc -> 
            statusBar.TempMessage(String.Format(SR.NavigateToFailed(), Exception.flattenMessage exc))
        
            // Don't show the dialog box as it's most likely that the user cancelled.
            // Don't make them click twice.
            true