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
open Microsoft.VisualStudio.FSharp.Editor.Logging
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
    
    // Use a single cache across text views
    static let xmlDocCache = Dictionary<string, IVsXMLMemberIndex>()
    let serviceProvider =  ServiceProvider.GlobalProvider                
    let xmlIndexService = serviceProvider.GetService<SVsXMLMemberIndexService,IVsXMLMemberIndexService>()

    let componentModel = serviceProvider.GetService<SComponentModel,IComponentModel>()
    let editorAdapterFactory = componentModel.GetService<IVsEditorAdaptersFactoryService>()

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
            projectOptions.ReferencedProjects 
            |> Array.map fst |> Set.ofArray

        let references = 
            projectOptions.OtherOptions
            |> Array.choose (fun arg -> // Filter out project references, which aren't necessary for the scenario
                if arg.StartsWith "-r:" 
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


    /// Find the range of the target system in the generated signature metadata file
    let tryFindExactLocation (filePath:string) (source:string) (currentSymbol:FSharpSymbolUse) (options:FSharpProjectOptions) = asyncMaybe {
        let checker = checkerProvider.Checker

        let! symbolUses = checker.GetAllUsesOfAllSymbolsInSourceString (options, filePath, source, false)  |> liftAsync

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
            |> Option.map (fun r -> String.Equals(r.FileName, filePath, StringComparison.OrdinalIgnoreCase)) 
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


    let createSigDocument (sigPath:string)(projectOptions:FSharpProjectOptions) =
        try 
            let windowFrame = // VSConstants.LOGVIEWID.Primary_guid opens the document as a preview tab as desired, but unfortunately causes 2 instances of the document to open
                match VsShellUtilities.IsDocumentOpen(serviceProvider, sigPath, VSConstants.LOGVIEWID.TextView_guid) with
                | true,_hierarchy,_itemId,windowFrame -> windowFrame
                | false,_,_,_ -> 
                    let (_,_,windowFrame) = VsShellUtilities.OpenDocument(serviceProvider,sigPath,VSConstants.LOGVIEWID.TextView_guid) 
                    windowFrame

            let vsTextView = VsShellUtilities.GetTextView windowFrame
            let vsTextBuffer = vsTextView.GetBuffer() |> snd :> IVsTextBuffer

            // TODO - Figure out a different way to set the document to readonly, using the method below causes a lock
            //match vsTextBuffer.GetStateFlags() with
            //| VSConstants.S_OK, currentFlags ->
            //    // Try to set buffer to read-only mode
            //    vsTextBuffer.SetStateFlags(currentFlags ||| uint32 BUFFERSTATEFLAGS.BSF_USER_READONLY) |> ignore
            //| _ -> ()

            let textBuffer = editorAdapterFactory.GetDataBuffer vsTextBuffer
            let container = textBuffer.AsTextContainer()
            
            let sigDocument = 
                workspace.GetRelatedDocumentIds container 
                |> Seq.map(fun docId -> workspace.CurrentSolution.GetDocument docId) |> Seq.head

            let sigProject=sigDocument.Project

            let projectOptions =
                { projectOptions with
                    ProjectFileName = sigProject.FilePath
                    ExtraProjectInfo = Some (box workspace)
                }
            projectInfoManager.AddSingleFileProject(sigDocument.Project.Id,(DateTime.Now,projectOptions))

            Some sigDocument
        with _ -> None


    // Now the input is an entity or a member/value.
    // We always generate the full enclosing entity signature if the symbol is a member/value
    let tryCreateMetadataContext (sourceDoc:Document) ast (fsSymbolUse: FSharpSymbolUse) : (Document * range) option Async = 
        asyncMaybe{
            let fsSymbol = fsSymbolUse.Symbol
            let fileName = SignatureGenerator.getFileNameFromSymbol fsSymbol
            // get the project options for the source file containing the target symbol
            let! fsProjectOptions = projectInfoManager.TryGetOptionsForDocumentOrProject sourceDoc

            // The file system is case-insensitive so list.fsi and List.fsi can clash
            // Thus, we generate a tmp subfolder based on the hash of the filename
            let subFolder = string (uint32 (hash fileName))

            let filePath = Path.GetDirectoryName sourceDoc.Project.FilePath</>"obj"</>"Signatures"</>subFolder</>fileName 
            let displayContext = fsSymbolUse.DisplayContext

            let openDeclarations = 
                OpenDeclarationGetter.getEffectiveOpenDeclarationsAtLocation  fsSymbolUse.RangeAlternate.Start ast

            let! options = sourceDoc.GetOptionsAsync() |> Async.AwaitTask  |> liftAsync
            let indentSize = options.GetOption(FormattingOptions.TabSize, FSharpCommonConstants.FSharpLanguageName)

            match SignatureGenerator.formatSymbol 
                    (getXmlDocBySignature fsSymbolUse) indentSize displayContext openDeclarations fsSymbol 
                        SignatureGenerator.Filterer.NoFilters SignatureGenerator.BlankLines.Default with
            | Some signatureText ->
                let directoryPath = Path.GetDirectoryName filePath
                Directory.CreateDirectory directoryPath |> ignore
                File.WriteAllText (filePath, signatureText)

                let fsProjectOptions = adjustOptionsForSignature filePath fsProjectOptions
                let! range = tryFindExactLocation filePath signatureText fsSymbolUse fsProjectOptions
                let! sigDocument = createSigDocument filePath fsProjectOptions

                return! Some (sigDocument,range)
            | None -> return! None
        }

        
    member __.TryFindMetadataRange (sourceDoc:Document, ast, fsSymbolUse) = async {
        return! tryCreateMetadataContext sourceDoc ast fsSymbolUse
    }

    static member ClearXmlDocCache () = xmlDocCache.Clear ()

          