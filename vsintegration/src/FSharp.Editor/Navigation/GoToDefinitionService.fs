// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Host.Mef

open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open System
open System.Diagnostics

type internal FSharpNavigableItem(document: Document, textSpan: TextSpan) =
    interface INavigableItem with
        member this.Glyph = Glyph.BasicFile
        member this.DisplayFileLocation = true
        member this.IsImplicitlyDeclared = false
        member this.Document = document
        member this.SourceSpan = textSpan
        member this.DisplayTaggedParts = ImmutableArray<TaggedText>.Empty
        member this.ChildItems = ImmutableArray<INavigableItem>.Empty

type internal GoToDefinition(checker: FSharpChecker, projectInfoManager: FSharpProjectOptionsManager) =

    static let userOpName = "GoToDefinition"

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
                | Some refTextSpan -> return Some (FSharpNavigableItem (refDocument, refTextSpan))
            else return None
        }

    /// Helper function that is used to determine the navigation strategy to apply, can be tuned towards signatures or implementation files.
    let findSymbolHelper (originDocument: Document, originRange: range, sourceText: SourceText, preferSignature: bool) : Async<FSharpNavigableItem option> =
        asyncMaybe {
            let! parsingOptions, projectOptions = projectInfoManager.TryGetOptionsForEditingDocumentOrProject originDocument
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing parsingOptions
            let! originTextSpan = RoslynHelpers.TryFSharpRangeToTextSpan (sourceText, originRange)
            let position = originTextSpan.Start
            let! lexerSymbol = Tokenizer.getSymbolAtPosition (originDocument.Id, sourceText, position, originDocument.FilePath, defines, SymbolLookupKind.Greedy, false)
            
            let textLinePos = sourceText.Lines.GetLinePosition position
            let fcsTextLineNumber = Line.fromZ textLinePos.Line
            let lineText = (sourceText.Lines.GetLineFromPosition position).ToString()  
            
            let! _, _, checkFileResults = checker.ParseAndCheckDocument (originDocument, projectOptions, allowStaleResults=true,sourceText=sourceText, userOpName = userOpName)
            let idRange = lexerSymbol.Ident.idRange
            let! fsSymbolUse = checkFileResults.GetSymbolUseAtLocation (fcsTextLineNumber, idRange.EndColumn, lineText, lexerSymbol.FullIsland, userOpName=userOpName)
            let symbol = fsSymbolUse.Symbol
            // if the tooltip was spawned in an implementation file and we have a range targeting
            // a signature file, try to find the corresponding implementation file and target the
            // desired symbol
            if isSignatureFile fsSymbolUse.FileName && preferSignature = false then 
                let fsfilePath = Path.ChangeExtension (originRange.FileName,"fs")
                if not (File.Exists fsfilePath) then return! None else
                let! implDoc = originDocument.Project.Solution.TryGetDocumentFromPath fsfilePath
                let! implSourceText = implDoc.GetTextAsync ()
                let! _parsingOptions, projectOptions = projectInfoManager.TryGetOptionsForEditingDocumentOrProject implDoc
                let! _, _, checkFileResults = checker.ParseAndCheckDocument (implDoc, projectOptions, allowStaleResults=true, sourceText=implSourceText, userOpName = userOpName)
                let! symbolUses = checkFileResults.GetUsesOfSymbolInFile symbol |> liftAsync
                let! implSymbol  = symbolUses |> Array.tryHead 
                let! implTextSpan = RoslynHelpers.TryFSharpRangeToTextSpan (implSourceText, implSymbol.RangeAlternate)
                return FSharpNavigableItem (implDoc, implTextSpan)
            else
                let! targetDocument = originDocument.Project.Solution.TryGetDocumentFromFSharpRange fsSymbolUse.RangeAlternate
                return! rangeToNavigableItem (fsSymbolUse.RangeAlternate, targetDocument)
        }  

    /// find the declaration location (signature file/.fsi) of the target symbol if possible, fall back to definition 
    member __.FindDeclarationOfSymbolAtRange(targetDocument: Document, symbolRange: range, targetSource: SourceText) =
        findSymbolHelper (targetDocument, symbolRange, targetSource, true)

    /// find the definition location (implementation file/.fs) of the target symbol
    member __.FindDefinitionOfSymbolAtRange(targetDocument: Document, symbolRange: range, targetSourceText: SourceText) =
        findSymbolHelper (targetDocument, symbolRange, targetSourceText, false)
    
    /// if the symbol is defined in the given file, return its declaration location, otherwise use the targetSymbol to find the first 
    /// instance of its presence in the provided source file. The first case is needed to return proper declaration location for
    /// recursive type definitions, where the first its usage may not be the declaration.
    member __.FindSymbolDeclarationInFile(targetSymbolUse: FSharpSymbolUse, filePath: string, source: string, options: FSharpProjectOptions, fileVersion:int) = 
        asyncMaybe {
            match targetSymbolUse.Symbol.DeclarationLocation with
            | Some decl when decl.FileName = filePath -> return decl
            | _ ->
                let! _, checkFileAnswer = checker.ParseAndCheckFileInProject (filePath, fileVersion, source, options, userOpName = userOpName) |> liftAsync
                match checkFileAnswer with 
                | FSharpCheckFileAnswer.Aborted -> return! None
                | FSharpCheckFileAnswer.Succeeded checkFileResults ->
                    let! symbolUses = checkFileResults.GetUsesOfSymbolInFile targetSymbolUse.Symbol |> liftAsync
                    let! implSymbol  = symbolUses |> Array.tryHead 
                    return implSymbol.RangeAlternate
        }

type private StatusBar(statusBar: IVsStatusbar) =

    let mutable searchIcon = int16 Microsoft.VisualStudio.Shell.Interop.Constants.SBAI_Find :> obj
        
    let clear() =
        // unfreeze the statusbar
        statusBar.FreezeOutput 0 |> ignore  
        statusBar.Clear() |> ignore
        
    member __.Message(msg: string) =
        let _, frozen = statusBar.IsFrozen()
        // unfreeze the status bar
        if frozen <> 0 then statusBar.FreezeOutput 0 |> ignore
        statusBar.SetText msg |> ignore
        // freeze the status bar
        statusBar.FreezeOutput 1 |> ignore

    member this.TempMessage(msg: string) =
        this.Message msg
        async {
            do! Async.Sleep 4000
            match statusBar.GetText() with
            | 0, currentText when currentText <> msg -> ()
            | _ -> clear()
        }|> Async.Start
    
    member __.Clear() = clear()

    /// Animated magnifying glass that displays on the status bar while a symbol search is in progress.
    member __.Animate() : IDisposable = 
        statusBar.Animation (1, &searchIcon) |> ignore
        { new IDisposable with
            member __.Dispose() = statusBar.Animation(0, &searchIcon) |> ignore }

module internal Symbol =

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

module internal ExternalType =

    let rec tryOfRoslynType (typesym: ITypeSymbol): ExternalType option =
        match typesym with
        | :? IPointerTypeSymbol as ptrparam ->
            tryOfRoslynType ptrparam.PointedAtType |> Option.map ExternalType.Pointer
        | :? IArrayTypeSymbol as arrparam ->
            tryOfRoslynType arrparam.ElementType |> Option.map ExternalType.Array
        | :? ITypeParameterSymbol as typaram ->
            Some (ExternalType.TypeVar typaram.Name)
        | :? INamedTypeSymbol as namedTypeSym ->
            namedTypeSym.TypeArguments
            |> Seq.map tryOfRoslynType
            |> List.ofSeq
            |> Option.ofOptionList
            |> Option.map (fun genericArgs ->
                ExternalType.Type (Symbol.fullName typesym, genericArgs)
                )
        | _ ->
            Debug.Assert(false, sprintf "GoToDefinitionService: Unexpected Roslyn type symbol subclass: %O" (typesym.GetType()))
            None

module internal ParamTypeSymbol =

    let tryOfRoslynParameter (param: IParameterSymbol): ParamTypeSymbol option =
        ExternalType.tryOfRoslynType param.Type
        |> Option.map (
            if param.RefKind = RefKind.None then ParamTypeSymbol.Param
            else ParamTypeSymbol.Byref
            )

    let tryOfRoslynParameters (paramSyms: ImmutableArray<IParameterSymbol>): ParamTypeSymbol list option =
        paramSyms |> Seq.map tryOfRoslynParameter |> Seq.toList |> Option.ofOptionList

module internal ExternalSymbol =
    
    let rec ofRoslynSymbol (symbol: ISymbol) : (ISymbol * ExternalSymbol) list =
        let container = Symbol.fullName symbol.ContainingSymbol

        match symbol with
        | :? INamedTypeSymbol as typesym ->
            let fullTypeName = Symbol.fullName typesym

            let constructors =
                typesym.InstanceConstructors
                |> Seq.choose<_,ISymbol * ExternalSymbol> (fun methsym ->
                    ParamTypeSymbol.tryOfRoslynParameters methsym.Parameters
                    |> Option.map (fun args -> upcast methsym, ExternalSymbol.Constructor(fullTypeName, args))
                    )
                |> List.ofSeq
                
            (symbol, ExternalSymbol.Type fullTypeName) :: constructors

        | :? IMethodSymbol as methsym ->
            ParamTypeSymbol.tryOfRoslynParameters methsym.Parameters
            |> Option.map (fun args ->
                symbol, ExternalSymbol.Method(container, methsym.MetadataName, args, methsym.TypeParameters.Length))
            |> Option.toList

        | :? IPropertySymbol as propsym ->
            [upcast propsym, ExternalSymbol.Property(container, propsym.MetadataName)]

        | :? IFieldSymbol as fieldsym ->
            [upcast fieldsym, ExternalSymbol.Field(container, fieldsym.MetadataName)]

        | :? IEventSymbol as eventsym ->
            [upcast eventsym, ExternalSymbol.Event(container, eventsym.MetadataName)]

        | _ -> []

[<ExportLanguageService(typeof<IGoToDefinitionService>, FSharpConstants.FSharpLanguageName)>]
[<Export(typeof<FSharpGoToDefinitionService>)>]
type internal FSharpGoToDefinitionService 
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: FSharpProjectOptionsManager
    ) =

    let gtd = GoToDefinition(checkerProvider.Checker, projectInfoManager)
    let statusBar = StatusBar(ServiceProvider.GlobalProvider.GetService<SVsStatusbar,IVsStatusbar>())  
   
    interface IGoToDefinitionService with
        /// Invoked with Peek Definition.
        member __.FindDefinitionsAsync (document: Document, position: int, cancellationToken: CancellationToken) =
            gtd.FindDefinitionsForPeekTask(document, position, cancellationToken)

        /// Invoked with Go to Definition.
        /// Try to navigate to the definiton of the symbol at the symbolRange in the originDocument
        member __.TryGoToDefinition(document: Document, position: int, cancellationToken: CancellationToken) =
            statusBar.Message(SR.LocatingSymbol())
            use __ = statusBar.Animate()

            let gtdTask = gtd.FindDefinitionTask(document, position, cancellationToken)

            // Wrap this in a try/with as if the user clicks "Cancel" on the thread dialog, we'll be cancelled
            // Task.Wait throws an exception if the task is cancelled, so be sure to catch it.
            let gtdCompletionOrError =
                try
                    // This call to Wait() is fine because we want to be able to provide the error message in the status bar.
                    gtdTask.Wait()
                    Ok gtdTask
                with exc -> 
                    Error(Exception.flattenMessage exc)
            
            match gtdCompletionOrError with
            | Ok task ->
                if task.Status = TaskStatus.RanToCompletion && task.Result.IsSome then
                    let item, _ = task.Result.Value
                    gtd.NavigateToItem(item, statusBar)

                    // 'true' means do it, like Sheev Palpatine would want us to.
                    true
                else 
                    statusBar.TempMessage (SR.CannotDetermineSymbol())
                    false
            | Error message ->
                statusBar.TempMessage(String.Format(SR.NavigateToFailed(), message))

                // Don't show the dialog box as it's most likely that the user cancelled.
                // Don't make them click twice.
                true