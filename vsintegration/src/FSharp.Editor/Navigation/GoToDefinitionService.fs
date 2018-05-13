// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.IO
open System.Collections.Immutable
open System.Linq
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Navigation
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.FindSymbols

open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open System
open System.Diagnostics

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

    static let userOpName = "GoToDefinition"
    let gotoDefinition = GoToDefinition(checkerProvider.Checker, projectInfoManager)
    let serviceProvider =  ServiceProvider.GlobalProvider
    let statusBar = StatusBar(serviceProvider.GetService<SVsStatusbar,IVsStatusbar>())

    /// Construct a task that will return a navigation target for the implementation definition of the symbol 
    /// at the provided position in the document.
    member __.FindDefinitionsTask(originDocument: Document, position: int, cancellationToken: CancellationToken) =
        asyncMaybe {
            let! parsingOptions, projectOptions = projectInfoManager.TryGetOptionsForEditingDocumentOrProject originDocument
            let! sourceText = originDocument.GetTextAsync () |> liftTaskAsync
            let defines = CompilerEnvironment.GetCompilationDefinesForEditing parsingOptions
            let textLine = sourceText.Lines.GetLineFromPosition position
            let textLinePos = sourceText.Lines.GetLinePosition position
            let fcsTextLineNumber = Line.fromZ textLinePos.Line
            let lineText = (sourceText.Lines.GetLineFromPosition position).ToString()  
            
            let preferSignature = isSignatureFile originDocument.FilePath

            let! _, _, checkFileResults = checkerProvider.Checker.ParseAndCheckDocument (originDocument, projectOptions, allowStaleResults=true, sourceText=sourceText, userOpName=userOpName)
                
            let! lexerSymbol = Tokenizer.getSymbolAtPosition (originDocument.Id, sourceText, position,originDocument.FilePath, defines, SymbolLookupKind.Greedy, false)
            let idRange = lexerSymbol.Ident.idRange

            let! declarations = checkFileResults.GetDeclarationLocation (fcsTextLineNumber, lexerSymbol.Ident.idRange.EndColumn, textLine.ToString(), lexerSymbol.FullIsland, preferSignature, userOpName=userOpName) |> liftAsync
            let! targetSymbolUse = checkFileResults.GetSymbolUseAtLocation (fcsTextLineNumber, idRange.EndColumn, lineText, lexerSymbol.FullIsland, userOpName=userOpName)

            match declarations with
            | FSharpFindDeclResult.ExternalDecl (assy, targetExternalSym) ->
                let! project = originDocument.Project.Solution.Projects |> Seq.tryFind (fun p -> p.AssemblyName.Equals(assy, StringComparison.OrdinalIgnoreCase))
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
                return FSharpNavigableItem(project.GetDocument(location.SourceTree), location.SourceSpan)

            | FSharpFindDeclResult.DeclFound targetRange -> 
                // if goto definition is called at we are alread at the declaration location of a symbol in
                // either a signature or an implementation file then we jump to it's respective postion in thethe
                if lexerSymbol.Range = targetRange then
                    // jump from signature to the corresponding implementation
                    if isSignatureFile originDocument.FilePath then
                        let implFilePath = Path.ChangeExtension (originDocument.FilePath,"fs")
                        if not (File.Exists implFilePath) then return! None else
                        let! implDocument = originDocument.Project.Solution.TryGetDocumentFromPath implFilePath
                        let! implSourceText = implDocument.GetTextAsync () |> liftTaskAsync
                        let! implVersion = implDocument.GetTextVersionAsync () |> liftTaskAsync
                        
                        let! targetRange = 
                            gotoDefinition.FindSymbolDeclarationInFile(targetSymbolUse, implFilePath, implSourceText.ToString(), projectOptions, implVersion.GetHashCode())

                        let! implTextSpan = RoslynHelpers.TryFSharpRangeToTextSpan (implSourceText, targetRange)
                        let navItem = FSharpNavigableItem (implDocument, implTextSpan)
                        return navItem
                    else // jump from implementation to the corresponding signature
                        let! declarations = checkFileResults.GetDeclarationLocation (fcsTextLineNumber, lexerSymbol.Ident.idRange.EndColumn, textLine.ToString(), lexerSymbol.FullIsland, true, userOpName=userOpName) |> liftAsync
                        match declarations with
                        | FSharpFindDeclResult.DeclFound targetRange -> 
                            let! sigDocument = originDocument.Project.Solution.TryGetDocumentFromPath targetRange.FileName
                            let! sigSourceText = sigDocument.GetTextAsync () |> liftTaskAsync
                            let! sigTextSpan = RoslynHelpers.TryFSharpRangeToTextSpan (sigSourceText, targetRange)
                            let navItem = FSharpNavigableItem (sigDocument, sigTextSpan)
                            return navItem
                        | _ -> return! None
                // when the target range is different follow the navigation convention of 
                // - gotoDefn origin = signature , gotoDefn destination = signature
                // - gotoDefn origin = implementation, gotoDefn destination = implementation 
                else
                    let! sigDocument = originDocument.Project.Solution.TryGetDocumentFromPath targetRange.FileName
                    let! sigSourceText = sigDocument.GetTextAsync () |> liftTaskAsync
                    let! sigTextSpan = RoslynHelpers.TryFSharpRangeToTextSpan (sigSourceText, targetRange)
                    // if the gotodef call originated from a signature and the returned target is a signature, navigate there
                    if isSignatureFile targetRange.FileName && preferSignature then 
                        let navItem = FSharpNavigableItem (sigDocument, sigTextSpan)
                        return navItem
                    else // we need to get an FSharpSymbol from the targetRange found in the signature
                         // that symbol will be used to find the destination in the corresponding implementation file
                        let implFilePath =
                            // Bugfix: apparently sigDocument not always is a signature file
                            if isSignatureFile sigDocument.FilePath then Path.ChangeExtension (sigDocument.FilePath, "fs") 
                            else sigDocument.FilePath

                        let! implDocument = originDocument.Project.Solution.TryGetDocumentFromPath implFilePath
                        let! implVersion = implDocument.GetTextVersionAsync () |> liftTaskAsync
                        let! implSourceText = implDocument.GetTextAsync () |> liftTaskAsync
                        let! _parsingOptions, projectOptions = projectInfoManager.TryGetOptionsForEditingDocumentOrProject implDocument
                        
                        let! targetRange = 
                            gotoDefinition.FindSymbolDeclarationInFile(targetSymbolUse, implFilePath, implSourceText.ToString(), projectOptions, implVersion.GetHashCode())                               
                        
                        let! implTextSpan = RoslynHelpers.TryFSharpRangeToTextSpan (implSourceText, targetRange)
                        let navItem = FSharpNavigableItem (implDocument, implTextSpan)
                        return navItem
                | _ -> return! None
        } 
        |> Async.map (Option.map (fun x -> x :> INavigableItem) >> Option.toArray >> Array.toSeq)
        |> RoslynHelpers.StartAsyncAsTask cancellationToken        
   
    interface IGoToDefinitionService with
        /// Invoked with Peak Definition.
        member this.FindDefinitionsAsync (document: Document, position: int, cancellationToken: CancellationToken) =
            this.FindDefinitionsTask (document, position, cancellationToken)

        /// Invoked with Go to Definition.
        /// Try to navigate to the definiton of the symbol at the symbolRange in the originDocument
        member this.TryGoToDefinition(document: Document, position: int, cancellationToken: CancellationToken) =
            let definitionTask = this.FindDefinitionsTask (document, position, cancellationToken)
            
            statusBar.Message (SR.LocatingSymbol())
            use _ = statusBar.Animate()

            // Wrap this in a try/with as if the user clicks "Cancel" on the thread dialog, we'll be cancelled
            // Task.Wait throws an exception if the task is cancelled, so be sure to catch it.
            let completionError =
                try
                    // REVIEW: document this use of a blocking wait on the cancellation token, explaining why it is ok
                    definitionTask.Wait()
                    None
                with exc -> Some <| Exception.flattenMessage exc
            
            match completionError with
            | Some message ->
                statusBar.TempMessage <| String.Format(SR.NavigateToFailed(), message)

                // Don't show the dialog box as it's most likely that the user cancelled.
                // Don't make them click twice.
                true
            | None ->
                if definitionTask.Status = TaskStatus.RanToCompletion && definitionTask.Result <> null && definitionTask.Result.Any() then
                    let item = definitionTask.Result.First() // F# API provides only one INavigableItem
                    GoToDefinitionHelpers.tryNavigateToItem (Some item) statusBar
                else 
                    statusBar.TempMessage (SR.CannotDetermineSymbol())
                    false