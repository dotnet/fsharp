namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.IO
open System.Text
open System.Security
open System.Composition
open System.Collections.Generic
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.Shell
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.Range
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open System.ComponentModel.Composition
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.CodeAnalysis.Formatting
open Microsoft.VisualStudio.ComponentModelHost
open Microsoft.VisualStudio.Editor
open Microsoft.VisualStudio.TextManager.Interop
open Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem 

[<Export>][<Shared>]
type internal NavigateToSignatureMetadataService [<ImportingConstructor>] 
    (   checkerProvider: FSharpCheckerProvider
    ,   projectInfoManager: ProjectInfoManager
    ,   workspace : VisualStudioWorkspaceImpl
    ) =
    
    let xmlDocCache = Dictionary<string, IVsXMLMemberIndex>()
    let serviceProvider =  ServiceProvider.GlobalProvider                
    let xmlIndexService = serviceProvider.GetService<SVsXMLMemberIndexService,IVsXMLMemberIndexService>()
    let componentModel = serviceProvider.GetService<SComponentModel,IComponentModel>()
    let editorAdapterFactory = componentModel.GetService<IVsEditorAdaptersFactoryService>()

    
    /// Stores the editor window displaying signature metadata if one has already been opened.
    /// Ensures that only one editor window is open for signature metadata at a time.
    let mutable currentWindow: IVsWindowFrame option = None 

    // Close the signature metadata window with the solution
    do workspace.WorkspaceChanged.Add (fun workspaceChange ->
       match workspaceChange.Kind with 
       | WorkspaceChangeKind.SolutionCleared
       | WorkspaceChangeKind.SolutionReloaded
       | WorkspaceChangeKind.SolutionRemoved ->
            currentWindow
            |> Option.bind Option.ofNull
            |> Option.iter (fun window -> window.CloseFrame (uint32 __FRAMECLOSE.FRAMECLOSE_NoSave) |> ignore)
       | _ -> ()
       )

    /// If the XML comment starts with '<' not counting whitespace then treat it as a literal XML comment.
    /// Otherwise, escape it and surround it with <summary></summary>
    let processXml (xml: string) =
        if String.IsNullOrEmpty xml then xml else
        let trimmedXml = xml.TrimStart([|' ';'\r';'\n'|])
        if String.IsNullOrEmpty trimmedXml then xml else
        match trimmedXml.[0] with
        | '<' ->    String.Join ("", "<root>", xml, "</root>")
        | _   ->    let escapedXml = SecurityElement.Escape xml
                    String.Join ("", "<summary>", escapedXml, "</summary>")
            
    let getXmlDocBySignature (fsSymbolUse: FSharpSymbolUse) signature =
        match fsSymbolUse.Symbol.Assembly.FileName with
        | Some assemblyName ->
            match xmlDocCache.TryGetValue assemblyName with
            | true, xmlIndex -> 
                match xmlIndex.ParseMemberSignature signature with
                | _, 0u -> []
                | _, index ->
                    match xmlIndex.GetMemberXML index with
                    | VSConstants.S_OK, xml ->
                        let processedXml = processXml xml
                        let xmlDocs = ResizeArray ()
                        match xmlIndexService.GetMemberDataFromXML processedXml with
                        | VSConstants.S_OK, memberData ->
                            match memberData.GetSummaryText () with
                            | VSConstants.S_OK, xmlSummary -> xmlDocs.Add xmlSummary
                            | _ -> ()
                            match memberData.GetParamCount () with
                            | VSConstants.S_OK, count when count > 0 ->
                                for i in 0..count-1 do
                                    match memberData.GetParamTextAt(i) with
                                    | VSConstants.S_OK, name, text ->
                                        let xmlDoc = sprintf "%s: %s" name text
                                        xmlDocs.Add xmlDoc
                                    | _ -> ()
                            | _ -> ()
                            xmlDocs |> Seq.toList
                        | _ -> []                            
                    | _ -> []
            | false, _ -> 
                match xmlIndexService.CreateXMLMemberIndex assemblyName with
                | VSConstants.S_OK, xmlIndex ->
                    match xmlIndex.BuildMemberIndex () with
                    | VSConstants.S_OK ->
                        xmlDocCache.Add (assemblyName, xmlIndex)
                        []
                    | _ -> []
                | _ -> []
        | None -> []


    let adjustOptionsForSignature filePath (projectOptions:FSharpProjectOptions)= 
        let projectFileName = filePath + ".fsproj"
        let sourceFiles = [| filePath |]
        let outputArg = sprintf "-o:%s" (Path.ChangeExtension(projectFileName, ".dll"))
        let flags = [|outputArg; "--noframework"; "--debug-"; "--optimize-"; "--tailcalls-" |]
        
        let refProjectsOutPaths =
            projectOptions.ReferencedProjects |> Array.map fst |> Set.ofArray

        let references = 
            projectOptions.OtherOptions
            |> Array.choose (fun arg -> // Filter out project references, which aren't necessary for the scenario
                if  arg.StartsWith "-r:" 
                 && not (Set.contains (arg.[3..].Trim()) refProjectsOutPaths) 
                then Some arg 
                else None)

        { projectOptions with
            ProjectFileName = projectFileName
            ProjectFileNames = sourceFiles
            OtherOptions = Array.append flags references
            IsIncompleteTypeCheckEnvironment = false
            UseScriptResolutionRules = false
            LoadTime = fakeDateTimeRepresentingTimeLoaded projectFileName
            UnresolvedReferences = None
            ReferencedProjects = [||]
            OriginalLoadReferences = []
            ExtraProjectInfo = None
        }


    let tryFindExactLocation (filePath:string) (source:string) (currentSymbol:FSharpSymbolUse) (options:FSharpProjectOptions) = maybe {
        let checker = checkerProvider.Checker
        let symbolUses = checker.GetAllUsesOfAllSymbolsInSourceString (options, filePath, source, false)  |> Async.RunSynchronously

        /// Try to reconstruct fully qualified name for the purpose of matching symbols
        let tryGetFullyQualifiedName (fsSymbol:FSharpSymbol) =
            let rec tryGetFullyQualifiedName (symbol: FSharpSymbol) = 
                Option.attempt (fun _ -> 
                    match symbol with
                    | TypedAstPatterns.FSharpEntity (entity, _, _) ->
                        Some (sprintf "%s.%s" entity.AccessPath entity.DisplayName)

                    | TypedAstPatterns.MemberFunctionOrValue mem ->
                        tryGetFullyQualifiedName mem.EnclosingEntity
                        |> Option.map (fun parent -> sprintf "%s.%s" parent mem.DisplayName)

                    | TypedAstPatterns.Field (field, _) ->
                        tryGetFullyQualifiedName field.DeclaringEntity
                        |> Option.map (fun parent -> sprintf "%s.%s" parent field.DisplayName)

                    | TypedAstPatterns.UnionCase uc ->
                        match uc.ReturnType with
                        | TypedAstPatterns.TypeWithDefinition entity ->
                            tryGetFullyQualifiedName entity
                            |> Option.map (fun parent -> sprintf "%s.%s" parent uc.DisplayName)
                        | _ -> None

                    | TypedAstPatterns.ActivePatternCase case ->
                        let group = case.Group
                        group.EnclosingEntity
                        |> Option.bind tryGetFullyQualifiedName
                        |> Option.map (fun parent -> 
                            let sb = StringBuilder().Append "|"                                
                            for name in group.Names do
                                sb.AppendFormat("{0}|", name) |> ignore
                            if not group.IsTotal then
                                sb.Append "_|" |> ignore
                            sprintf "%s.( %O )" parent sb)
                    | _ ->
                        None)
                |> Option.flatten
                |> Option.orTry (fun _ -> Option.attempt (fun _ -> symbol.FullName))
            try tryGetFullyQualifiedName fsSymbol
            with _ -> None

        let isLocalSymbol filePath (symbol: FSharpSymbol) =
            symbol.DeclarationLocation 
            |> Option.map (fun r -> String.Equals (r.FileName, filePath, StringComparison.OrdinalIgnoreCase)) 
            |> Option.defaultValue false

        let! currentSymbolFullName = tryGetFullyQualifiedName currentSymbol.Symbol

        let! matchedSymbol = 
            symbolUses 
            |> Seq.groupBy (fun symbolUse  -> symbolUse.SymbolUse.Symbol)
            |> Seq.collect (fun (_, uses) -> Seq.truncate 1 uses)
            |> Seq.choose  (fun  symbolUse -> 
                match symbolUse.SymbolUse.Symbol with
                | TypedAstPatterns.FSharpEntity _
                | TypedAstPatterns.MemberFunctionOrValue _
                | TypedAstPatterns.ActivePatternCase _                   
                | TypedAstPatterns.UnionCase _
                | TypedAstPatterns.Field _ as symbol -> 
                    match tryGetFullyQualifiedName symbol with
                    | Some symbolFullName ->
                        if symbolFullName = currentSymbolFullName 
                            && isLocalSymbol filePath symbol then
                            Some symbol
                        else None
                    | None -> None
                | _ -> None)
            |> Seq.sortBy (fun symbol -> 
                symbol.DeclarationLocation |> Option.map (fun r -> r.StartLine, r.StartColumn))
            |> Seq.tryHead
        return! matchedSymbol.DeclarationLocation
    }

    /// use the provided window frame to find the Roslyn Document associated with its content
    let documentFromWindowFrame (windowFrame:IVsWindowFrame) =
        // TODO -   
        //  Figure out a way to set the frame as a provisional editor (preview) 
        //  When the method below is used the frame dumps its content register  losing
        //  its classifiers, readonly status, and stored filepath 
        // windowFrame.SetProperty(int __VSFPROPID5.VSFPROPID_IsProvisional, true) |> ignore
        
        let vsTextView = VsShellUtilities.GetTextView windowFrame
        let vsTextBuffer = vsTextView.GetBuffer() |> snd :> IVsTextBuffer
        let (result,currentFlags) = vsTextBuffer.GetStateFlags ()

        // Try to set buffer to read-only mode
        if result = VSConstants.S_OK then vsTextBuffer.SetStateFlags(currentFlags ||| uint32 BUFFERSTATEFLAGS.BSF_USER_READONLY) |> ignore
        let container = (editorAdapterFactory.GetDataBuffer vsTextBuffer).AsTextContainer()        
        workspace.GetRelatedDocumentIds container 
        |> Seq.map(fun docId -> workspace.CurrentSolution.GetDocument docId) |> Seq.head



    /// Creates a new Roslyn Document for signature metadata and either opens an editor window to display it
    /// or clears the currently open editor and replaces its contents
    let createSigDocument (sigPath:string) (projectOptions:FSharpProjectOptions) =
        currentWindow 
        |> Option.bind Option.ofNull
        |> Option.iter (fun window -> window.CloseFrame (uint32 __FRAMECLOSE.FRAMECLOSE_NoSave) |> ignore)
  
        let (_,_,windowFrame) = VsShellUtilities.OpenDocument(serviceProvider,sigPath,VSConstants.LOGVIEWID.Primary_guid)
        currentWindow <- Some windowFrame
        windowFrame.Show () |> ignore    
        
        let sigDocument = documentFromWindowFrame windowFrame
        let sigProject = sigDocument.Project

        let projectOptions =
            { projectOptions with
                ProjectFileName = sigProject.FilePath
                ExtraProjectInfo = Some (box workspace)
            }
        projectInfoManager.AddSingleFileProject (sigDocument.Project.Id, (projectOptions.LoadTime, projectOptions))
        sigDocument


    // Now the input is an entity or a member/value.
    // We always generate the full enclosing entity signature if the symbol is a member/value
    let tryCreateMetadataContext (sourceDoc:Document) ast (fsSymbolUse: FSharpSymbolUse) : (Document * range) option = 
        maybe{
            let fsSymbol = fsSymbolUse.Symbol
            let fileName = SignatureGenerator.getFileNameFromSymbol fsSymbol
            // get the project options for the source file containing the target symbol
            let! fsProjectOptions = projectInfoManager.TryGetOptionsForDocumentOrProject sourceDoc |> Async.RunSynchronously

            // The file system is case-insensitive so list.fsi and List.fsi can clash
            // Thus, we generate a tmp subfolder based on the hash of the filename
            let subFolder = string (uint32 (hash fileName))
            let filePath = Path.GetDirectoryName sourceDoc.Project.FilePath</>"obj"</>"Signatures"</>subFolder</>fileName 
            let fsProjectOptions = adjustOptionsForSignature filePath fsProjectOptions

            // If the target document is already open, extract its content to find the symbol location
            match VsShellUtilities.IsDocumentOpen (serviceProvider, filePath, VSConstants.LOGVIEWID.Primary_guid) with
            | true,_hierarchy,_itemId,windowFrame -> 
                let sigDocument = documentFromWindowFrame windowFrame
                let signatureText = (sigDocument.GetTextAsync() |> Async.RunTaskSynchronously).ToString()
                let! range = tryFindExactLocation filePath signatureText fsSymbolUse fsProjectOptions
                return! Some (sigDocument, range)

            // Otherwise generate the signature file, save it to disk, load it into the workspace, and extract the Roslyn Document 
            | false,_,_,_ ->
                let displayContext = fsSymbolUse.DisplayContext

                let openDeclarations = 
                    OpenDeclarationGetter.getEffectiveOpenDeclarationsAtLocation  fsSymbolUse.RangeAlternate.Start ast

                let options = sourceDoc.GetOptionsAsync () |> Async.RunTaskSynchronously
                let indentSize = options.GetOption (FormattingOptions.TabSize, FSharpCommonConstants.FSharpLanguageName)

                match SignatureGenerator.formatSymbol 
                        (getXmlDocBySignature fsSymbolUse) indentSize displayContext openDeclarations fsSymbol 
                            SignatureGenerator.Filterer.NoFilters SignatureGenerator.BlankLines.Default with
                | Some signatureText ->
                    let directoryPath = Path.GetDirectoryName filePath
                    Directory.CreateDirectory directoryPath |> ignore
                    File.WriteAllText(filePath, signatureText,Encoding.UTF8)

                    let! range = tryFindExactLocation filePath signatureText fsSymbolUse fsProjectOptions
                    let sigDocument = createSigDocument filePath fsProjectOptions

                    return! Some (sigDocument,range)
                | None -> return! None
        }

        
    member __.TryFindMetadataRange (sourceDoc:Document, ast, fsSymbolUse) = 
        tryCreateMetadataContext sourceDoc ast fsSymbolUse

    member __.ClearXmlDocCache () = xmlDocCache.Clear ()

          