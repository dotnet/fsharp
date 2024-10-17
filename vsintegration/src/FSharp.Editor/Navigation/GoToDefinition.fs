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
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.LanguageServices

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.Symbols
open System.Composition
open System.Text.RegularExpressions
open CancellableTasks
open Microsoft.VisualStudio.FSharp.Editor.Telemetry
open Microsoft.VisualStudio.Telemetry
open Microsoft.VisualStudio.Threading

module private Symbol =
    let fullName (root: ISymbol) : string =
        let rec inner parts (sym: ISymbol) =
            match sym with
            | null -> parts
            // TODO: do we have any other terminating cases?
            | sym when sym.Kind = SymbolKind.NetModule || sym.Kind = SymbolKind.Assembly -> parts
            | sym when sym.MetadataName <> "" -> inner (sym.MetadataName :: parts) sym.ContainingSymbol
            | sym -> inner parts sym.ContainingSymbol

        inner [] root |> String.concat "."

module private FindDeclExternalType =
    let rec tryOfRoslynType (typesym: ITypeSymbol) : FindDeclExternalType option =
        match typesym with
        | :? IPointerTypeSymbol as ptrparam ->
            tryOfRoslynType ptrparam.PointedAtType
            |> Option.map FindDeclExternalType.Pointer
        | :? IArrayTypeSymbol as arrparam -> tryOfRoslynType arrparam.ElementType |> Option.map FindDeclExternalType.Array
        | :? ITypeParameterSymbol as typaram -> Some(FindDeclExternalType.TypeVar typaram.Name)
        | :? INamedTypeSymbol as namedTypeSym ->
            namedTypeSym.TypeArguments
            |> Seq.map tryOfRoslynType
            |> List.ofSeq
            |> Option.ofOptionList
            |> Option.map (fun genericArgs -> FindDeclExternalType.Type(Symbol.fullName typesym, genericArgs))
        | _ ->
            Debug.Assert(false, sprintf "GoToDefinitionService: Unexpected Roslyn type symbol subclass: %O" (typesym.GetType()))
            None

module private FindDeclExternalParam =

    let tryOfRoslynParameter (param: IParameterSymbol) : FindDeclExternalParam option =
        FindDeclExternalType.tryOfRoslynType param.Type
        |> Option.map (fun ty -> FindDeclExternalParam.Create(ty, param.RefKind <> RefKind.None))

    let tryOfRoslynParameters (paramSyms: ImmutableArray<IParameterSymbol>) : FindDeclExternalParam list option =
        paramSyms |> Seq.map tryOfRoslynParameter |> Seq.toList |> Option.ofOptionList

module private ExternalSymbol =
    let rec ofRoslynSymbol (symbol: ISymbol) : (ISymbol * FindDeclExternalSymbol) list =
        let container = Symbol.fullName symbol.ContainingSymbol

        match symbol with
        | :? INamedTypeSymbol as typesym ->
            let fullTypeName = Symbol.fullName typesym

            let constructors =
                typesym.InstanceConstructors
                |> Seq.choose<_, ISymbol * FindDeclExternalSymbol> (fun methsym ->
                    FindDeclExternalParam.tryOfRoslynParameters methsym.Parameters
                    |> Option.map (fun args -> upcast methsym, FindDeclExternalSymbol.Constructor(fullTypeName, args)))
                |> List.ofSeq

            (symbol, FindDeclExternalSymbol.Type fullTypeName) :: constructors

        | :? IMethodSymbol as methsym ->
            FindDeclExternalParam.tryOfRoslynParameters methsym.Parameters
            |> Option.map (fun args ->
                symbol, FindDeclExternalSymbol.Method(container, methsym.MetadataName, args, methsym.TypeParameters.Length))
            |> Option.toList

        | :? IPropertySymbol as propsym ->
            [
                upcast propsym, FindDeclExternalSymbol.Property(container, propsym.MetadataName)
            ]

        | :? IFieldSymbol as fieldsym ->
            [
                upcast fieldsym, FindDeclExternalSymbol.Field(container, fieldsym.MetadataName)
            ]

        | :? IEventSymbol as eventsym ->
            [
                upcast eventsym, FindDeclExternalSymbol.Event(container, eventsym.MetadataName)
            ]

        | _ -> []

type internal FSharpGoToDefinitionNavigableItem(document, sourceSpan) =
    inherit FSharpNavigableItem(Glyph.BasicFile, ImmutableArray.Empty, document, sourceSpan)

[<RequireQualifiedAccess>]
type internal FSharpGoToDefinitionResult =
    | NavigableItem of FSharpNavigableItem
    | ExternalAssembly of FSharpSymbolUse * MetadataReference seq

type internal GoToDefinition(metadataAsSource: FSharpMetadataAsSourceService) =

    let rec areTypesEqual (ty1: FSharpType) (ty2: FSharpType) =
        let ty1 = ty1.StripAbbreviations()
        let ty2 = ty2.StripAbbreviations()

        let generic =
            ty1.IsGenericParameter && ty2.IsGenericParameter
            || (ty1.GenericArguments.Count = ty2.GenericArguments.Count
                && (ty1.GenericArguments, ty2.GenericArguments) ||> Seq.forall2 areTypesEqual)

        if generic then
            true
        else
            let namesEqual = ty1.TypeDefinition.DisplayName = ty2.TypeDefinition.DisplayName
            let accessPathsEqual = ty1.TypeDefinition.AccessPath = ty2.TypeDefinition.AccessPath
            namesEqual && accessPathsEqual

    let tryFindExternalSymbolUse (targetSymbolUse: FSharpSymbolUse) (x: FSharpSymbolUse) =
        match x.Symbol, targetSymbolUse.Symbol with
        | (:? FSharpEntity as symbol1), (:? FSharpEntity as symbol2) when x.IsFromDefinition -> symbol1.DisplayName = symbol2.DisplayName

        | (:? FSharpMemberOrFunctionOrValue as symbol1), (:? FSharpMemberOrFunctionOrValue as symbol2) ->
            symbol1.DisplayName = symbol2.DisplayName
            && (match symbol1.DeclaringEntity, symbol2.DeclaringEntity with
                | Some e1, Some e2 -> e1.CompiledName = e2.CompiledName
                | _ -> false)
            && symbol1.GenericParameters.Count = symbol2.GenericParameters.Count
            && symbol1.CurriedParameterGroups.Count = symbol2.CurriedParameterGroups.Count
            && ((symbol1.CurriedParameterGroups, symbol2.CurriedParameterGroups)
                ||> Seq.forall2 (fun pg1 pg2 ->
                    let pg1, pg2 = pg1.ToArray(), pg2.ToArray()
                    // We filter out/fixup first "unit" parameter in the group, since it just represents the `()` call notation, for example `"string".Clone()` will have one curried group with one parameter which type is unit.
                    let pg1 = // If parameter has no name and it's unit type, filter it out
                        if
                            pg1.Length > 0
                            && Option.isNone pg1[0].Name
                            && pg1[0].Type.StripAbbreviations().TypeDefinition.DisplayName = "Unit"
                        then
                            pg1[1..]
                        else
                            pg1

                    pg1.Length = pg2.Length
                    && ((pg1, pg2) ||> Seq.forall2 (fun p1 p2 -> areTypesEqual p1.Type p2.Type))))
            && areTypesEqual symbol1.ReturnParameter.Type symbol2.ReturnParameter.Type
        | (:? FSharpField as symbol1), (:? FSharpField as symbol2) when x.IsFromDefinition ->
            symbol1.DisplayName = symbol2.DisplayName
            && (match symbol1.DeclaringEntity, symbol2.DeclaringEntity with
                | Some e1, Some e2 -> e1.CompiledName = e2.CompiledName
                | _ -> false)
        | (:? FSharpUnionCase as symbol1), (:? FSharpUnionCase as symbol2) ->
            symbol1.DisplayName = symbol2.DisplayName
            && symbol1.DeclaringEntity.CompiledName = symbol2.DeclaringEntity.CompiledName
        | _ -> false

    /// Use an origin document to provide the solution & workspace used to
    /// find the corresponding textSpan and INavigableItem for the range
    let rangeToNavigableItem (range: range, document: Document) =
        cancellableTask {
            let fileName =
                try
                    System.IO.Path.GetFullPath range.FileName
                with _ ->
                    range.FileName

            let refDocumentIds = document.Project.Solution.GetDocumentIdsWithFilePath fileName

            if not refDocumentIds.IsEmpty then
                let refDocumentId = refDocumentIds.First()
                let refDocument = document.Project.Solution.GetDocument refDocumentId
                let! cancellationToken = Async.CancellationToken
                let! refSourceText = refDocument.GetTextAsync(cancellationToken) |> Async.AwaitTask

                match RoslynHelpers.TryFSharpRangeToTextSpan(refSourceText, range) with
                | ValueNone -> return None
                | ValueSome refTextSpan -> return Some(FSharpGoToDefinitionNavigableItem(refDocument, refTextSpan))
            else
                return None
        }

    member _.TryGetExternalDeclarationAsync(targetSymbolUse: FSharpSymbolUse, metadataReferences: seq<MetadataReference>) =
        let textOpt =
            match targetSymbolUse.Symbol with
            | :? FSharpEntity as symbol -> symbol.TryGetMetadataText() |> Option.map (fun text -> text, symbol.DisplayName)
            | :? FSharpMemberOrFunctionOrValue as symbol ->
                symbol.ApparentEnclosingEntity.TryGetMetadataText()
                |> Option.map (fun text -> text, symbol.ApparentEnclosingEntity.DisplayName)
            | :? FSharpField as symbol ->
                match symbol.DeclaringEntity with
                | Some entity ->
                    let text = entity.TryGetMetadataText()

                    match text with
                    | Some text -> Some(text, entity.DisplayName)
                    | None -> None
                | None -> None
            | :? FSharpUnionCase as symbol ->
                symbol.DeclaringEntity.TryGetMetadataText()
                |> Option.map (fun text -> text, symbol.DisplayName)
            | _ -> None

        match textOpt with
        | None -> CancellableTask.singleton None
        | Some(text, fileName) ->
            foregroundCancellableTask {
                let! cancellationToken = CancellableTask.getCancellationToken ()
                do! ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken)

                let tmpProjInfo, tmpDocInfo =
                    MetadataAsSource.generateTemporaryDocument (
                        AssemblyIdentity(targetSymbolUse.Symbol.Assembly.QualifiedName),
                        fileName,
                        metadataReferences
                    )

                let tmpShownDocOpt =
                    metadataAsSource.ShowDocument(tmpProjInfo, tmpDocInfo.FilePath, SourceText.From(text.ToString()))

                match tmpShownDocOpt with
                | ValueNone -> return None
                | ValueSome tmpShownDoc ->
                    let! _, checkResults = tmpShownDoc.GetFSharpParseAndCheckResultsAsync("NavigateToExternalDeclaration")

                    let r =
                        // This tries to find the best possible location of the target symbol's location in the metadata source.
                        // We really should rely on symbol equality within FCS instead of doing it here,
                        //     but the generated metadata as source isn't perfect for symbol equality.
                        let symbols = checkResults.GetAllUsesOfAllSymbolsInFile(cancellationToken)

                        symbols
                        |> Seq.tryFindV (tryFindExternalSymbolUse targetSymbolUse)
                        |> ValueOption.map (fun x -> x.Range)

                    let! span =
                        cancellableTask {
                            let! cancellationToken = CancellableTask.getCancellationToken ()

                            match r with
                            | ValueNone -> return TextSpan.empty
                            | ValueSome r ->
                                let! text = tmpShownDoc.GetTextAsync(cancellationToken)

                                match RoslynHelpers.TryFSharpRangeToTextSpan(text, r) with
                                | ValueSome span -> return span
                                | _ -> return TextSpan.empty
                        }

                    return Some(FSharpGoToDefinitionNavigableItem(tmpShownDoc, span) :> FSharpNavigableItem)
            }

    /// Helper function that is used to determine the navigation strategy to apply, can be tuned towards signatures or implementation files.
    member private _.FindSymbolHelper(originDocument: Document, originRange: range, sourceText: SourceText, preferSignature: bool) =
        let originTextSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, originRange)

        match originTextSpan with
        | ValueNone -> CancellableTask.singleton None
        | ValueSome originTextSpan ->
            cancellableTask {
                let userOpName = "FindSymbolHelper"

                let position = originTextSpan.Start
                let! lexerSymbol = originDocument.TryFindFSharpLexerSymbolAsync(position, SymbolLookupKind.Greedy, false, false, userOpName)

                match lexerSymbol with
                | None -> return None
                | Some lexerSymbol ->
                    let textLinePos = sourceText.Lines.GetLinePosition position
                    let fcsTextLineNumber = Line.fromZ textLinePos.Line
                    let lineText = (sourceText.Lines.GetLineFromPosition position).ToString()
                    let idRange = lexerSymbol.Ident.idRange

                    let! ct = CancellableTask.getCancellationToken ()

                    let! _, checkFileResults = originDocument.GetFSharpParseAndCheckResultsAsync(nameof (GoToDefinition))

                    let fsSymbolUse =
                        checkFileResults.GetSymbolUseAtLocation(fcsTextLineNumber, idRange.EndColumn, lineText, lexerSymbol.FullIsland)

                    match fsSymbolUse with
                    | None -> return None
                    | Some fsSymbolUse ->

                        let symbol = fsSymbolUse.Symbol
                        // if the tooltip was spawned in an implementation file and we have a range targeting
                        // a signature file, try to find the corresponding implementation file and target the
                        // desired symbol
                        if isSignatureFile fsSymbolUse.FileName && preferSignature = false then
                            let fsfilePath = Path.ChangeExtension(originRange.FileName, "fs")

                            if not (File.Exists fsfilePath) then
                                return None
                            else
                                let implDoc = originDocument.Project.Solution.TryGetDocumentFromPath fsfilePath

                                match implDoc with
                                | ValueNone -> return None
                                | ValueSome implDoc ->
                                    let! implSourceText = implDoc.GetTextAsync(ct)

                                    let! _, checkFileResults = implDoc.GetFSharpParseAndCheckResultsAsync(userOpName)

                                    let symbolUses = checkFileResults.GetUsesOfSymbolInFile(symbol, ct)

                                    let implSymbol = Array.tryHeadV symbolUses

                                    match implSymbol with
                                    | ValueNone -> return None
                                    | ValueSome implSymbol ->
                                        let implTextSpan =
                                            RoslynHelpers.TryFSharpRangeToTextSpan(implSourceText, implSymbol.Range)

                                        match implTextSpan with
                                        | ValueNone -> return None
                                        | ValueSome implTextSpan -> return Some(FSharpGoToDefinitionNavigableItem(implDoc, implTextSpan))
                        else
                            let targetDocument =
                                originDocument.Project.Solution.TryGetDocumentFromFSharpRange fsSymbolUse.Range

                            match targetDocument with
                            | None -> return None
                            | Some targetDocument ->
                                let! navItem = rangeToNavigableItem (fsSymbolUse.Range, targetDocument)
                                return navItem
            }

    /// if the symbol is defined in the given file, return its declaration location, otherwise use the targetSymbol to find the first
    /// instance of its presence in the provided source file. The first case is needed to return proper declaration location for
    /// recursive type definitions, where the first its usage may not be the declaration.
    member _.FindSymbolDeclarationInDocument(targetSymbolUse: FSharpSymbolUse, document: Document) =
        asyncMaybe {
            let filePath = document.FilePath
            let! ct = Async.CancellationToken |> liftAsync

            match targetSymbolUse.Symbol.DeclarationLocation with
            | Some decl when decl.FileName = filePath -> return decl
            | _ ->
                let! _, checkFileResults =
                    document.GetFSharpParseAndCheckResultsAsync("FindSymbolDeclarationInDocument")
                    |> CancellableTask.start ct
                    |> Async.AwaitTask
                    |> liftAsync

                let symbolUses = checkFileResults.GetUsesOfSymbolInFile targetSymbolUse.Symbol

                let! implSymbol =
                    symbolUses
                    |> Array.sortByDescending (fun x -> x.IsFromDefinition)
                    |> Array.tryHead

                return implSymbol.Range
        }

    member internal this.FindDefinitionAtPosition(originDocument: Document, position: int) =
        cancellableTask {
            let userOpName = "FindDefinitionAtPosition"
            let! cancellationToken = CancellableTask.getCancellationToken ()
            let! sourceText = originDocument.GetTextAsync(cancellationToken)
            let textLine = sourceText.Lines.GetLineFromPosition position
            let textLinePos = sourceText.Lines.GetLinePosition position
            let textLineString = textLine.ToString()
            let fcsTextLineNumber = Line.fromZ textLinePos.Line
            let lineText = (sourceText.Lines.GetLineFromPosition position).ToString()

            let! cancellationToken = CancellableTask.getCancellationToken ()

            let preferSignature = isSignatureFile originDocument.FilePath

            let! lexerSymbol = originDocument.TryFindFSharpLexerSymbolAsync(position, SymbolLookupKind.Greedy, false, false, userOpName)

            match lexerSymbol with
            | None -> return ValueNone
            | Some lexerSymbol ->

                let idRange = lexerSymbol.Ident.idRange

                let! _, checkFileResults = originDocument.GetFSharpParseAndCheckResultsAsync(userOpName)

                let declarations =
                    checkFileResults.GetDeclarationLocation(
                        fcsTextLineNumber,
                        idRange.EndColumn,
                        textLineString,
                        lexerSymbol.FullIsland,
                        preferSignature
                    )

                let targetSymbolUse =
                    checkFileResults.GetSymbolUseAtLocation(fcsTextLineNumber, idRange.EndColumn, lineText, lexerSymbol.FullIsland)

                match targetSymbolUse with
                | None -> return ValueNone
                | Some targetSymbolUse ->

                    match declarations with
                    | FindDeclResult.ExternalDecl(assembly, targetExternalSym) ->
                        let projectOpt =
                            originDocument.Project.Solution.Projects
                            |> Seq.tryFindV (fun p -> p.AssemblyName.Equals(assembly, StringComparison.OrdinalIgnoreCase))

                        match projectOpt with
                        | ValueSome project ->
                            let! symbols = SymbolFinder.FindSourceDeclarationsAsync(project, (fun _ -> true), cancellationToken)

                            let roslynSymbols = Seq.collect ExternalSymbol.ofRoslynSymbol symbols

                            let symbol =
                                Seq.tryPickV
                                    (fun (sym, externalSym) ->
                                        if externalSym = targetExternalSym then
                                            ValueSome sym
                                        else
                                            ValueNone)
                                    roslynSymbols

                            let location =
                                symbol
                                |> ValueOption.map (fun s -> s.Locations)
                                |> ValueOption.bind Seq.tryHeadV

                            match location with
                            | ValueNone -> return ValueNone
                            | ValueSome location ->

                                return
                                    ValueSome(
                                        FSharpGoToDefinitionResult.NavigableItem(
                                            FSharpGoToDefinitionNavigableItem(project.GetDocument(location.SourceTree), location.SourceSpan)
                                        ),
                                        idRange
                                    )
                        | _ ->
                            let metadataReferences = originDocument.Project.MetadataReferences
                            return ValueSome(FSharpGoToDefinitionResult.ExternalAssembly(targetSymbolUse, metadataReferences), idRange)

                    | FindDeclResult.DeclFound targetRange ->
                        // If the file is not associated with a document, it's considered external.
                        if not (originDocument.Project.Solution.ContainsDocumentWithFilePath(targetRange.FileName)) then
                            let metadataReferences = originDocument.Project.MetadataReferences
                            return ValueSome(FSharpGoToDefinitionResult.ExternalAssembly(targetSymbolUse, metadataReferences), idRange)
                        else if
                            // if goto definition is called as we are already at the declaration location of a symbol in
                            // either a signature or an implementation file then we jump to its respective position in the document
                            lexerSymbol.Range = targetRange
                        then
                            // jump from signature to the corresponding implementation
                            if isSignatureFile originDocument.FilePath then
                                let implFilePath = Path.ChangeExtension(originDocument.FilePath, "fs")

                                if not (File.Exists implFilePath) then
                                    return ValueNone
                                else
                                    let implDocument =
                                        originDocument.Project.Solution.TryGetDocumentFromPath implFilePath

                                    match implDocument with
                                    | ValueNone -> return ValueNone
                                    | ValueSome implDocument ->
                                        let! targetRange = this.FindSymbolDeclarationInDocument(targetSymbolUse, implDocument)

                                        match targetRange with
                                        | None -> return ValueNone
                                        | Some targetRange ->
                                            let! implSourceText = implDocument.GetTextAsync(cancellationToken)

                                            let implTextSpan =
                                                RoslynHelpers.TryFSharpRangeToTextSpan(implSourceText, targetRange)

                                            match implTextSpan with
                                            | ValueNone -> return ValueNone
                                            | ValueSome implTextSpan ->
                                                let navItem = FSharpGoToDefinitionNavigableItem(implDocument, implTextSpan)
                                                return ValueSome(FSharpGoToDefinitionResult.NavigableItem(navItem), idRange)

                            else // jump from implementation to the corresponding signature
                                let declarations =
                                    checkFileResults.GetDeclarationLocation(
                                        fcsTextLineNumber,
                                        idRange.EndColumn,
                                        textLineString,
                                        lexerSymbol.FullIsland,
                                        true
                                    )

                                match declarations with
                                | FindDeclResult.DeclFound targetRange ->
                                    let sigDocument =
                                        originDocument.Project.Solution.TryGetDocumentFromPath targetRange.FileName

                                    match sigDocument with
                                    | ValueNone -> return ValueNone
                                    | ValueSome sigDocument ->
                                        let! sigSourceText = sigDocument.GetTextAsync(cancellationToken)

                                        let sigTextSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sigSourceText, targetRange)

                                        match sigTextSpan with
                                        | ValueNone -> return ValueNone
                                        | ValueSome sigTextSpan ->
                                            let navItem = FSharpGoToDefinitionNavigableItem(sigDocument, sigTextSpan)
                                            return ValueSome(FSharpGoToDefinitionResult.NavigableItem(navItem), idRange)
                                | _ -> return ValueNone
                        // when the target range is different follow the navigation convention of
                        // - gotoDefn origin = signature , gotoDefn destination = signature
                        // - gotoDefn origin = implementation, gotoDefn destination = implementation
                        else
                            let sigDocument =
                                originDocument.Project.Solution.TryGetDocumentFromPath targetRange.FileName

                            match sigDocument with
                            | ValueNone -> return ValueNone
                            | ValueSome sigDocument ->
                                let! sigSourceText = sigDocument.GetTextAsync(cancellationToken)
                                let sigTextSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sigSourceText, targetRange)

                                match sigTextSpan with
                                | ValueNone -> return ValueNone
                                | ValueSome sigTextSpan ->
                                    // if the gotodef call originated from a signature and the returned target is a signature, navigate there
                                    if isSignatureFile targetRange.FileName && preferSignature then
                                        let navItem = FSharpGoToDefinitionNavigableItem(sigDocument, sigTextSpan)
                                        return ValueSome(FSharpGoToDefinitionResult.NavigableItem(navItem), idRange)
                                    else // we need to get an FSharpSymbol from the targetRange found in the signature
                                        // that symbol will be used to find the destination in the corresponding implementation file
                                        let implFilePath =
                                            // Bugfix: apparently sigDocument not always is a signature file
                                            if isSignatureFile sigDocument.FilePath then
                                                Path.ChangeExtension(sigDocument.FilePath, "fs")
                                            else
                                                sigDocument.FilePath

                                        let implDocument =
                                            originDocument.Project.Solution.TryGetDocumentFromPath implFilePath

                                        match implDocument with
                                        | ValueNone -> return ValueNone
                                        | ValueSome implDocument ->
                                            let! targetRange = this.FindSymbolDeclarationInDocument(targetSymbolUse, implDocument)

                                            match targetRange with
                                            | None -> return ValueNone
                                            | Some targetRange ->
                                                let! implSourceText = implDocument.GetTextAsync(cancellationToken)

                                                let implTextSpan =
                                                    RoslynHelpers.TryFSharpRangeToTextSpan(implSourceText, targetRange)

                                                match implTextSpan with
                                                | ValueNone -> return ValueNone
                                                | ValueSome implTextSpan ->
                                                    let navItem = FSharpGoToDefinitionNavigableItem(implDocument, implTextSpan)
                                                    return ValueSome(FSharpGoToDefinitionResult.NavigableItem(navItem), idRange)
                    | _ -> return ValueNone
        }

    /// find the declaration location (signature file/.fsi) of the target symbol if possible, fall back to definition
    member this.FindDeclarationOfSymbolAtRange(targetDocument: Document, symbolRange: range, targetSource: SourceText) =
        this.FindSymbolHelper(targetDocument, symbolRange, targetSource, preferSignature = true)

    /// find the definition location (implementation file/.fs) of the target symbol
    member this.FindDefinitionOfSymbolAtRange(targetDocument: Document, symbolRange: range, targetSourceText: SourceText) =
        this.FindSymbolHelper(targetDocument, symbolRange, targetSourceText, preferSignature = false)

    /// Construct a task that will return a navigation target for the implementation definition of the symbol
    /// at the provided position in the document.
    member this.FindDefinitionAsync(originDocument: Document, position: int) =
        this.FindDefinitionAtPosition(originDocument, position)

    /// Navigate to the position of the textSpan in the provided document
    /// used by quickinfo link navigation when the tooltip contains the correct destination range.
    member _.TryNavigateToTextSpan(document: Document, textSpan: TextSpan, cancellationToken: CancellationToken) =
        let navigableItem = FSharpGoToDefinitionNavigableItem(document, textSpan)
        let workspace = document.Project.Solution.Workspace

        let navigationService =
            workspace.Services.GetService<IFSharpDocumentNavigationService>()

        navigationService.TryNavigateToSpan(workspace, navigableItem.Document.Id, navigableItem.SourceSpan, cancellationToken)
        |> ignore

    member _.NavigateToItem(navigableItem: FSharpNavigableItem, cancellationToken: CancellationToken) =

        let workspace = navigableItem.Document.Project.Solution.Workspace

        let navigationService =
            workspace.Services.GetService<IFSharpDocumentNavigationService>()

        // Prefer open documents in the preview tab.
        let result =
            navigationService.TryNavigateToSpan(workspace, navigableItem.Document.Id, navigableItem.SourceSpan, cancellationToken)

        result

    /// Find the declaration location (signature file/.fsi) of the target symbol if possible, fall back to definition
    member this.NavigateToSymbolDeclarationAsync(targetDocument: Document, targetSourceText: SourceText, symbolRange: range) =
        cancellableTask {
            let! item = this.FindDeclarationOfSymbolAtRange(targetDocument, symbolRange, targetSourceText)

            match item with
            | None -> return false
            | Some item ->
                let! cancellationToken = CancellableTask.getCancellationToken ()
                return this.NavigateToItem(item, cancellationToken)
        }

    /// Find the definition location (implementation file/.fs) of the target symbol
    member this.NavigateToSymbolDefinitionAsync(targetDocument: Document, targetSourceText: SourceText, symbolRange: range) =
        cancellableTask {
            let! item = this.FindDefinitionOfSymbolAtRange(targetDocument, symbolRange, targetSourceText)

            match item with
            | None -> return false
            | Some item ->
                let! cancellationToken = CancellableTask.getCancellationToken ()
                return this.NavigateToItem(item, cancellationToken)
        }

    member this.NavigateToExternalDeclarationAsync(targetSymbolUse: FSharpSymbolUse, metadataReferences: seq<MetadataReference>) =
        foregroundCancellableTask {
            let! cancellationToken = CancellableTask.getCancellationToken ()

            match! this.TryGetExternalDeclarationAsync(targetSymbolUse, metadataReferences) with
            | Some navItem -> return this.NavigateToItem(navItem, cancellationToken)
            | None -> return false
        }

    member this.NavigateToExternalDeclaration
        (
            targetSymbolUse: FSharpSymbolUse,
            metadataReferences: seq<MetadataReference>,
            cancellationToken: CancellationToken
        ) =

        let textOpt =
            match targetSymbolUse.Symbol with
            | :? FSharpEntity as symbol -> symbol.TryGetMetadataText() |> Option.map (fun text -> text, symbol.DisplayName)
            | :? FSharpMemberOrFunctionOrValue as symbol ->
                symbol.ApparentEnclosingEntity.TryGetMetadataText()
                |> Option.map (fun text -> text, symbol.ApparentEnclosingEntity.DisplayName)
            | :? FSharpField as symbol ->
                match symbol.DeclaringEntity with
                | Some entity ->
                    let text = entity.TryGetMetadataText()

                    match text with
                    | Some text -> Some(text, entity.DisplayName)
                    | None -> None
                | None -> None
            | :? FSharpUnionCase as symbol ->
                symbol.DeclaringEntity.TryGetMetadataText()
                |> Option.map (fun text -> text, symbol.DisplayName)
            | _ -> None

        match textOpt with
        | Some(text, fileName) ->
            let tmpProjInfo, tmpDocInfo =
                MetadataAsSource.generateTemporaryDocument (
                    AssemblyIdentity(targetSymbolUse.Symbol.Assembly.QualifiedName),
                    fileName,
                    metadataReferences
                )

            let tmpShownDocOpt =
                metadataAsSource.ShowDocument(tmpProjInfo, tmpDocInfo.FilePath, SourceText.From(text.ToString()))

            match tmpShownDocOpt with
            | ValueSome tmpShownDoc ->
                let goToAsync =
                    cancellableTask {

                        let! cancellationToken = CancellableTask.getCancellationToken ()

                        let! _, checkResults = tmpShownDoc.GetFSharpParseAndCheckResultsAsync("NavigateToExternalDeclaration")

                        let r =
                            // This tries to find the best possible location of the target symbol's location in the metadata source.
                            // We really should rely on symbol equality within FCS instead of doing it here,
                            //     but the generated metadata as source isn't perfect for symbol equality.
                            let symbols = checkResults.GetAllUsesOfAllSymbolsInFile(cancellationToken)

                            symbols
                            |> Seq.tryFindV (tryFindExternalSymbolUse targetSymbolUse)
                            |> ValueOption.map (fun x -> x.Range)
                            |> ValueOption.toOption

                        match r with
                        | None -> return TextSpan.empty
                        | Some r ->
                            let! text = tmpShownDoc.GetTextAsync(cancellationToken)

                            match RoslynHelpers.TryFSharpRangeToTextSpan(text, r) with
                            | ValueSome span -> return span
                            | _ -> return TextSpan.empty

                    }

                let span = CancellableTask.runSynchronously cancellationToken goToAsync

                let navItem = FSharpGoToDefinitionNavigableItem(tmpShownDoc, span)
                this.NavigateToItem(navItem, cancellationToken)
            | _ -> false
        | _ -> false

type internal FSharpNavigation(metadataAsSource: FSharpMetadataAsSourceService, initialDoc: Document, thisSymbolUseRange: range) =

    let workspace = initialDoc.Project.Solution.Workspace
    let solution = workspace.CurrentSolution

    member _.IsTargetValid(range: range) =
        range <> rangeStartup
        && range <> thisSymbolUseRange
        && solution.TryGetDocumentIdFromFSharpRange(range, initialDoc.Project.Id)
           |> Option.isSome

    member _.NavigateTo(range: range) =
        try
            ThreadHelper.JoinableTaskFactory.Run(
                SR.NavigatingTo(),
                (fun _progress cancellationToken ->
                    cancellableTask {
                        let targetDoc = solution.TryGetDocumentFromFSharpRange(range, initialDoc.Project.Id)

                        match targetDoc with
                        | None -> ()
                        | Some targetDoc ->

                            let! cancellationToken = CancellableTask.getCancellationToken ()

                            let! targetSource = targetDoc.GetTextAsync(cancellationToken)
                            let targetTextSpan = RoslynHelpers.TryFSharpRangeToTextSpan(targetSource, range)

                            match targetTextSpan with
                            | ValueNone -> ()
                            | ValueSome targetTextSpan ->

                                let gtd = GoToDefinition(metadataAsSource)

                                // Whenever possible:
                                //  - signature files (.fsi) should navigate to other signature files
                                //  - implementation files (.fs) should navigate to other implementation files
                                if isSignatureFile initialDoc.FilePath then
                                    // Target range will point to .fsi file if only there is one so we can just use Roslyn navigation service.
                                    do gtd.TryNavigateToTextSpan(targetDoc, targetTextSpan, cancellationToken)
                                else
                                    // Navigation request was made in a .fs file, so we try to find the implementation of the symbol at target range.
                                    // This is the part that may take some time, because of type checks involved.
                                    let! result = gtd.NavigateToSymbolDefinitionAsync(targetDoc, targetSource, range)

                                    if not result then
                                        // In case the above fails, we just navigate to target range.
                                        do gtd.TryNavigateToTextSpan(targetDoc, targetTextSpan, cancellationToken)
                    }
                    |> CancellableTask.start cancellationToken),
                // Default wait time before VS shows the dialog allowing to cancel the long running task is 2 seconds.
                // This seems a bit too long to leave the user without any feedback, so we shorten it to 1 second.
                // Note: it seems anything less than 1 second will get rounded down to zero, resulting in flashing dialog
                // on each navigation, so 1 second is as low as we cen get from JoinableTaskFactory.
                TimeSpan.FromSeconds 1
            )
        with :? OperationCanceledException ->
            ()

    member _.FindDefinitionsAsync(position) =
        cancellableTask {
            let gtd = GoToDefinition(metadataAsSource)
            let! result = gtd.FindDefinitionAtPosition(initialDoc, position)

            match result with
            | ValueSome(FSharpGoToDefinitionResult.NavigableItem(navItem), _) -> return ImmutableArray.create navItem
            | ValueSome(FSharpGoToDefinitionResult.ExternalAssembly(targetSymbolUse, metadataReferences), _) ->
                match! gtd.TryGetExternalDeclarationAsync(targetSymbolUse, metadataReferences) with
                | Some navItem -> return ImmutableArray.Create navItem
                | _ -> return ImmutableArray.empty
            | _ -> return ImmutableArray.empty
        }

    member _.TryGoToDefinition(position, cancellationToken) =
        // Once we migrate to Roslyn-exposed MAAS and sourcelink (https://github.com/dotnet/fsharp/issues/13951), this can be a "normal" task
        // Wrap this in a try/with as if the user clicks "Cancel" on the thread dialog, we'll be cancelled.
        // Task.Wait throws an exception if the task is cancelled, so be sure to catch it.
        try
            use _ =
                TelemetryReporter.ReportSingleEventWithDuration(TelemetryEvents.GoToDefinition, [||])

            let gtd = GoToDefinition(metadataAsSource)
            let gtdTask = gtd.FindDefinitionAsync (initialDoc, position) cancellationToken

            gtdTask.Wait()

            if gtdTask.Status = TaskStatus.RanToCompletion && gtdTask.Result.IsSome then
                match gtdTask.Result with
                | ValueSome(FSharpGoToDefinitionResult.NavigableItem(navItem), _) ->
                    gtd.NavigateToItem(navItem, cancellationToken) |> ignore
                    true
                | ValueSome(FSharpGoToDefinitionResult.ExternalAssembly(targetSymbolUse, metadataReferences), _) ->
                    gtd.NavigateToExternalDeclaration(targetSymbolUse, metadataReferences, cancellationToken)
                    |> ignore

                    true
                | _ -> false
            else
                false
        with exc ->
            TelemetryReporter.ReportFault(TelemetryEvents.GoToDefinition, FaultSeverity.General, exc)
            false

[<RequireQualifiedAccess>]
type internal SymbolMemberType =
    | Event
    | Property
    | Method
    | Constructor
    | Other

    static member FromString(s: string) =
        match s with
        | "E" -> Event
        | "P" -> Property
        | "CTOR" -> Constructor // That one is "artificial one", so we distinguish constructors.
        | "M" -> Method
        | _ -> Other

type internal SymbolPath =
    {
        EntityPath: string list
        MemberOrValName: string
        GenericParameters: int
    }

[<RequireQualifiedAccess>]
type internal DocCommentId =
    | Member of SymbolPath * SymbolMemberType: SymbolMemberType
    | Field of SymbolPath
    | Type of EntityPath: string list
    | None

type FSharpNavigableLocation(metadataAsSource: FSharpMetadataAsSourceService, symbolRange: range, project: Project) =
    interface IFSharpNavigableLocation with
        member _.NavigateToAsync(_options: FSharpNavigationOptions2, cancellationToken: CancellationToken) : Task<bool> =
            cancellableTask {
                let targetPath = symbolRange.FileName

                let! cancellationToken = CancellableTask.getCancellationToken ()

                let targetDoc =
                    project.Solution.TryGetDocumentFromFSharpRange(symbolRange, project.Id)

                match targetDoc with
                | None -> return false
                | Some targetDoc ->
                    let! targetSource = targetDoc.GetTextAsync(cancellationToken)
                    let gtd = GoToDefinition(metadataAsSource)

                    let (|Signature|Implementation|) filepath =
                        if isSignatureFile filepath then
                            Signature
                        else
                            Implementation

                    match targetPath with
                    | Signature -> return! gtd.NavigateToSymbolDefinitionAsync(targetDoc, targetSource, symbolRange)
                    | Implementation -> return! gtd.NavigateToSymbolDeclarationAsync(targetDoc, targetSource, symbolRange)
            }
            |> CancellableTask.start cancellationToken

[<Export(typeof<IFSharpCrossLanguageSymbolNavigationService>)>]
[<Export(typeof<FSharpCrossLanguageSymbolNavigationService>)>]
type FSharpCrossLanguageSymbolNavigationService() =
    let componentModel =
        Package.GetGlobalService(typeof<ComponentModelHost.SComponentModel>) :?> ComponentModelHost.IComponentModel

    let workspace = componentModel.GetService<VisualStudioWorkspace>()

    let metadataAsSource =
        componentModel.DefaultExportProvider
            .GetExport<FSharpMetadataAsSourceService>()
            .Value

    let tryFindFieldByName (name: string) (e: FSharpEntity) =
        let fields =
            e.FSharpFields
            |> Seq.filter (fun x -> x.DisplayName = name && not x.IsCompilerGenerated)
            |> Seq.map (fun e -> e.DeclarationLocation)

        if fields.Count() <= 0 && (e.IsFSharpUnion || e.IsFSharpRecord) then
            Seq.singleton e.DeclarationLocation
        else
            fields

    let tryFindValByNameAndType
        (name: string)
        (symbolMemberType: SymbolMemberType)
        (genericParametersCount: int)
        (e: FSharpEntity)
        (entities: FSharpMemberOrFunctionOrValue seq)
        =

        let defaultFilter (e: FSharpMemberOrFunctionOrValue) =
            (e.DisplayName = name || e.CompiledName = name)
            && e.GenericParameters.Count = genericParametersCount

        let isProperty (e: FSharpMemberOrFunctionOrValue) = defaultFilter e && e.IsProperty
        let isConstructor (e: FSharpMemberOrFunctionOrValue) = defaultFilter e && e.IsConstructor

        let getLocation (e: FSharpMemberOrFunctionOrValue) = e.DeclarationLocation

        let filteredEntities: range seq =
            match symbolMemberType with
            | SymbolMemberType.Other
            | SymbolMemberType.Method -> entities |> Seq.filter defaultFilter |> Seq.map getLocation
            // F# record-specific logic, if navigating to the record's ctor, then navigate to record declaration.
            // If we navigating to F# record property, we first check if it's "custom" property, if it's one of the record fields, we search for it in the fields.
            | SymbolMemberType.Constructor when e.IsFSharpRecord -> Seq.singleton e.DeclarationLocation
            | SymbolMemberType.Property when e.IsFSharpRecord ->
                let properties = entities |> Seq.filter isProperty |> Seq.map getLocation
                let fields = tryFindFieldByName name e
                Seq.append properties fields
            | SymbolMemberType.Constructor -> entities |> Seq.filter isConstructor |> Seq.map getLocation
            // When navigating to property for the record, it will be in members bag for custom ones, but will be in the fields in fields.
            | SymbolMemberType.Event // Events are just properties`
            | SymbolMemberType.Property -> entities |> Seq.filter isProperty |> Seq.map getLocation

        filteredEntities

    let tryFindVal
        (name: string)
        (documentCommentId: string)
        (symbolMemberType: SymbolMemberType)
        (genericParametersCount: int)
        (e: FSharpEntity)
        =
        let entities = e.TryGetMembersFunctionsAndValues()

        // First, try and find entity by exact xml signature, return if found,
        // otherwise, just try and match by parsed name and number of arguments.

        let entitiesByXmlSig =
            entities
            |> Seq.filter (fun e -> e.XmlDocSig = documentCommentId)
            |> Seq.map (fun e -> e.DeclarationLocation)

        if Seq.isEmpty entitiesByXmlSig then
            tryFindValByNameAndType name symbolMemberType genericParametersCount e entities
        else
            entitiesByXmlSig

    static member internal DocCommentIdToPath(docId: string) =
        // The groups are following:
        //   1 - type (see below).
        //   2 - Path - a dotted path to a symbol.
        //   3 - parameters, optional, only for methods and properties.
        //   4 - return type, optional, only for methods.
        let docCommentIdRx =
            Regex(@"^(?<kind>\w):(?<entity>[\w\d#`.]+)(?<args>\(.+\))?(?:~([\w\d.]+))?$", RegexOptions.Compiled)

        // Parse generic args out of the function name
        let fnGenericArgsRx =
            Regex(@"^(?<entity>.+)``(?<typars>\d+)$", RegexOptions.Compiled)
        // docCommentId is in the following format:
        //
        // "T:" prefix for types
        // "T:N.X.Nested" - type
        // "T:N.X.D" - delegate
        //
        // "M:" prefix is for methods
        // "M:N.X.#ctor" - constructor
        // "M:N.X.#ctor(System.Int32)" - constructor with one parameter
        // "M:N.X.f" - method with unit parameter
        // "M:N.X.bb(System.String,System.Int32@)" - method with two parameters
        // "M:N.X.gg(System.Int16[],System.Int32[0:,0:])" - method with two parameters, 1d and 2d array
        // "M:N.X.op_Addition(N.X,N.X)" - operator
        // "M:N.X.op_Explicit(N.X)~System.Int32" - operator with return type
        // "M:N.GenericMethod.WithNestedType``1(N.GenericType{``0}.NestedType)" - generic type with one parameter
        // "M:N.GenericMethod.WithIntOfNestedType``1(N.GenericType{System.Int32}.NestedType)" - generic type with one parameter
        // "M:N.X.N#IX{N#KVP{System#String,System#Int32}}#IXA(N.KVP{System.String,System.Int32})" - explicit interface implementation
        //
        // "E:" prefix for events
        //
        // "E:N.X.d".
        //
        // "F:" prefix for fields
        // "F:N.X.q" - field
        //
        // "P:" prefix for properties
        // "P:N.X.prop" - property with getter and setter

        let m = docCommentIdRx.Match(docId)
        let t = m.Groups["kind"].Value

        match m.Success, t with
        | true, ("M" | "P" | "E") ->
            // TODO: Probably, there's less janky way of dealing with those.
            let parts = m.Groups["entity"].Value.Split('.')
            let entityPath = parts[.. (parts.Length - 2)] |> List.ofArray
            let memberOrVal = parts[parts.Length - 1]

            // Try and parse generic params count from the name (e.g. NameOfTheFunction``1, where ``1 is amount of type parameters)
            let genericM = fnGenericArgsRx.Match(memberOrVal)

            let (memberOrVal, genericParametersCount) =
                if genericM.Success then
                    (genericM.Groups["entity"].Value, int genericM.Groups["typars"].Value)
                else
                    memberOrVal, 0

            // A hack/fixup for the constructor name (#ctor in doccommentid and ``.ctor`` in F#)
            if memberOrVal = "#ctor" then
                DocCommentId.Member(
                    {
                        EntityPath = entityPath
                        MemberOrValName = "``.ctor``"
                        GenericParameters = 0
                    },
                    SymbolMemberType.Constructor
                )
            else
                DocCommentId.Member(
                    {
                        EntityPath = entityPath
                        MemberOrValName = memberOrVal
                        GenericParameters = genericParametersCount
                    },
                    (SymbolMemberType.FromString t)
                )
        | true, "T" ->
            let entityPath = m.Groups["entity"].Value.Split('.') |> List.ofArray
            DocCommentId.Type entityPath
        | true, "F" ->
            let parts = m.Groups["entity"].Value.Split('.')
            let entityPath = parts[.. (parts.Length - 2)] |> List.ofArray
            let memberOrVal = parts[parts.Length - 1]

            DocCommentId.Field
                {
                    EntityPath = entityPath
                    MemberOrValName = memberOrVal
                    GenericParameters = 0
                }
        | _ -> DocCommentId.None

    interface IFSharpCrossLanguageSymbolNavigationService with
        member _.TryGetNavigableLocationAsync
            (
                assemblyName: string,
                documentationCommentId: string,
                cancellationToken: CancellationToken
            ) : Task<IFSharpNavigableLocation> =
            let path =
                FSharpCrossLanguageSymbolNavigationService.DocCommentIdToPath documentationCommentId

            cancellableTask {
                let projects =
                    workspace.CurrentSolution.Projects
                    |> Seq.filter (fun p -> p.IsFSharp && p.AssemblyName = assemblyName)

                let mutable locations = Seq.empty

                for project in projects do
                    let! checker, _, _, options = project.GetFSharpCompilationOptionsAsync()
                    let! result = checker.ParseAndCheckProject(options)

                    match path with
                    | DocCommentId.Member({
                                              EntityPath = entityPath
                                              MemberOrValName = memberOrVal
                                              GenericParameters = genericParametersCount
                                          },
                                          memberType) ->
                        let entity = result.AssemblySignature.FindEntityByPath(entityPath)

                        entity
                        |> Option.iter (fun e ->
                            locations <-
                                e
                                |> tryFindVal memberOrVal documentationCommentId memberType genericParametersCount
                                |> Seq.map (fun m -> (m, project))
                                |> Seq.append locations)
                    | DocCommentId.Field {
                                             EntityPath = entityPath
                                             MemberOrValName = memberOrVal
                                         } ->
                        let entity = result.AssemblySignature.FindEntityByPath(entityPath)

                        entity
                        |> Option.iter (fun e ->
                            locations <-
                                e
                                |> tryFindFieldByName memberOrVal
                                |> Seq.map (fun m -> (m, project))
                                |> Seq.append locations)
                    | DocCommentId.Type entityPath ->
                        let entity = result.AssemblySignature.FindEntityByPath(entityPath)

                        entity
                        |> Option.iter (fun e -> locations <- Seq.append locations [ e.DeclarationLocation, project ])
                    | DocCommentId.None -> ()

                // TODO: Figure out the way of giving the user choice where to navigate, if there are more than one result
                // For now, we only take 1st one, since it's usually going to be only one result (given we process names correctly).
                // More results can theoretically be returned in case of method overloads, or when we have both signature and implementation files.
                if locations.Count() >= 1 then
                    let (location, project) = locations.First()
                    return FSharpNavigableLocation(metadataAsSource, location, project) :> IFSharpNavigableLocation
                else
                    return Unchecked.defaultof<_> // returning null here, so Roslyn can fallback to default source-as-metadata implementation.
            }
            |> CancellableTask.start cancellationToken
