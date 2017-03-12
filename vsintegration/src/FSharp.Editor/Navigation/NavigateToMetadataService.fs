namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.IO
open System.Text
open System.Security
open System.Collections.Generic
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.Shell
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.Range
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open System.ComponentModel.Composition
open Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.CodeAnalysis.Formatting


[<Export>]
type internal NavigateToMetadataService [<ImportingConstructor>] 
    (   checkerProvider: FSharpCheckerProvider
    ,   [<Import(typeof<SVsServiceProvider>)>]serviceProvider: IServiceProvider
    ,   projectInfoManager: ProjectInfoManager
    ,   workspace: VisualStudioWorkspaceImpl
    ) =
    
    // Use a single cache across text views
    static let xmlDocCache = Dictionary<string, IVsXMLMemberIndex>()
    let xmlIndexService = serviceProvider.GetService<IVsXMLMemberIndexService, SVsXMLMemberIndexService>()

    /// If the XML comment starts with '<' not counting whitespace then treat it as a literal XML comment.
    /// Otherwise, escape it and surround it with <summary></summary>
    let processXml (xml: string) =
        if String.IsNullOrEmpty(xml) then xml
        else
            let trimmedXml = xml.TrimStart([|' ';'\r';'\n'|])
            if String.IsNullOrEmpty(trimmedXml) then xml
            else
                match trimmedXml.[0] with
                | '<' ->
                    String.Join("", "<root>", xml, "</root>")
                | _ ->
                    let escapedXml = SecurityElement.Escape(xml)
                    String.Join("", "<summary>", escapedXml, "</summary>")
            
    let getXmlDocBySignature (fsSymbol: FSharpSymbol) signature =
        match fsSymbol.Assembly.FileName with
        | Some assemblyName ->
            match xmlDocCache.TryGetValue(assemblyName) with
            | true, xmlIndex -> 
                match xmlIndex.ParseMemberSignature(signature) with
                | _, 0u -> []
                | _, index ->
                    match xmlIndex.GetMemberXML(index) with
                    | VSConstants.S_OK, xml ->
                        let processedXml = processXml xml
                        let xmlDocs = ResizeArray()
                        match xmlIndexService.GetMemberDataFromXML(processedXml) with
                        | VSConstants.S_OK, memberData ->
                            match memberData.GetSummaryText() with
                            | VSConstants.S_OK, xmlSummary ->
                                xmlDocs.Add(xmlSummary)
                            | _ -> ()
                            match memberData.GetParamCount() with
                            | VSConstants.S_OK, count when count > 0 ->
                                for i in 0..count-1 do
                                    match memberData.GetParamTextAt(i) with
                                    | VSConstants.S_OK, name, text ->
                                        let xmlDoc = sprintf "%s: %s" name text
                                        xmlDocs.Add(xmlDoc)
                                    | _ -> ()
                            | _ -> ()
                            xmlDocs |> Seq.toList
                        | _ -> []                            
                    | _ -> []
            | false, _ -> 
                match xmlIndexService.CreateXMLMemberIndex(assemblyName) with
                | VSConstants.S_OK, xmlIndex ->
                    match xmlIndex.BuildMemberIndex() with
                    | VSConstants.S_OK ->
                        xmlDocCache.Add(assemblyName, xmlIndex)
                        []
                    | _ -> []
                | _ -> []
        | None -> []


    /// Find the range of the target system in the generated signature metadata file
    let tryFindExactLocation (sigDocument:Document) (sigSourceText:SourceText) currentSymbol =
        asyncMaybe {
            let checker = checkerProvider.Checker
            let! options = projectInfoManager.TryGetOptionsForDocumentOrProject (sigDocument)
            let! _, _, checkFileResults = checker.ParseAndCheckDocument (sigDocument, options, sourceText = sigSourceText, allowStaleResults = true)
            let! symbolUses = checkFileResults.GetAllUsesOfAllSymbolsInFile() |> liftAsync
                //vsLanguageService.GetAllUsesOfAllSymbolsInSourceString(signature, filePath, signatureProject, AllowStaleResults.No, checkForUnusedOpens=false)

            /// Try to reconstruct fully qualified name for the purpose of matching symbols
            let rec tryGetFullyQualifiedName (symbol: FSharpSymbol) = 
                Option.attempt (fun _ -> 
                    match symbol with
                    | TypedAstPatterns.FSharpEntity (entity, _, _) ->
                        Some (sprintf "%s.%s" entity.AccessPath entity.DisplayName)
                    | TypedAstPatterns.MemberFunctionOrValue mem ->
                        tryGetFullyQualifiedName mem.EnclosingEntity
                        |> Option.map (fun parent -> sprintf "%s.%s" parent mem.DisplayName)
                    | TypedAstPatterns.Field(field, _) ->
                        tryGetFullyQualifiedName field.DeclaringEntity
                        |> Option.map (fun parent -> sprintf "%s.%s" parent field.DisplayName)
                    | TypedAstPatterns.UnionCase uc ->
                        match uc.ReturnType with
                        | TypedAstPatterns.TypeWithDefinition entity ->
                            tryGetFullyQualifiedName entity
                            |> Option.map (fun parent -> sprintf "%s.%s" parent uc.DisplayName)
                        | _ -> 
                            None
                    | TypedAstPatterns.ActivePatternCase case ->
                        let group = case.Group
                        group.EnclosingEntity
                        |> Option.bind tryGetFullyQualifiedName
                        |> Option.map (fun parent -> 
                            let sb = StringBuilder()
                            sb.Append("|") |> ignore
                            for name in group.Names do
                                sb.AppendFormat("{0}|", name) |> ignore
                            if not group.IsTotal then
                                sb.Append("_|") |> ignore
                            sprintf "%s.( %O )" parent sb)
                    | _ ->
                        None)
                |> Option.flatten
                |> Option.orTry (fun _ -> Option.attempt (fun _ -> symbol.FullName))

            let isLocalSymbol filePath (symbol: FSharpSymbol) =
                symbol.DeclarationLocation 
                |> Option.map (fun r -> String.Equals(r.FileName, filePath, StringComparison.OrdinalIgnoreCase)) 
                |> Option.defaultValue false

            let! currentSymbolFullName = tryGetFullyQualifiedName currentSymbol

            let! matchedSymbol = 
                symbolUses 
                |> Seq.groupBy (fun symbolUse  -> symbolUse.Symbol)
                |> Seq.collect (fun (_, uses) -> Seq.truncate 1 uses)
                |> Seq.choose (fun  symbolUse -> 
                    match symbolUse.Symbol with
                    | TypedAstPatterns.FSharpEntity _
                    | TypedAstPatterns.MemberFunctionOrValue _
                    | TypedAstPatterns.ActivePatternCase _                   
                    | TypedAstPatterns.UnionCase _
                    | TypedAstPatterns.Field _ as symbol -> 
                        match tryGetFullyQualifiedName symbol with
                        | Some symbolFullName ->
                            if symbolFullName = currentSymbolFullName && isLocalSymbol sigDocument.FilePath symbol then
                                Some symbol
                            else None
                        | None -> None
                    | _ -> None)
                |> Seq.sortBy (fun symbol -> 
                    symbol.DeclarationLocation |> Option.map (fun r -> r.StartLine, r.StartColumn))
                |> Seq.tryHead

            return! matchedSymbol.DeclarationLocation
        }

    // Now the input is an entity or a member/value.
    // We always generate the full enclosing entity signature if the symbol is a member/value
    let tryCreateMetadataContext (sourceDoc:Document) ast (fsSymbolUse: FSharpSymbolUse) : (DocumentId * string * range option) option Async = 
        async{
            
            let fsSymbol = fsSymbolUse.Symbol
            let fileName = SignatureGenerator.getFileNameFromSymbol fsSymbol

            // The file system is case-insensitive so list.fsi and List.fsi can clash
            // Thus, we generate a tmp subfolder based on the hash of the filename
            let subFolder = string (uint32 (hash fileName))

            let filePath = Path.Combine(Path.GetTempPath(), subFolder, fileName)  
            let displayContext = fsSymbolUse.DisplayContext

            match projectInfoManager.TryGetSignatureDocId filePath with
            | Some sigDocId ->
                if not (workspace.IsDocumentOpen sigDocId) then
                    workspace.OpenDocument sigDocId 

                let document = workspace.CurrentSolution.GetDocument sigDocId
                let! sourceText = document.GetTextAsync()
                let! range = tryFindExactLocation document sourceText fsSymbol
                return Some (sigDocId,filePath,range)
                    
            | None -> 
                let openDeclarations = 
                    OpenDeclarationGetter.getEffectiveOpenDeclarationsAtLocation  fsSymbolUse.RangeAlternate.Start ast
                let project = sourceDoc.Project
                let! options = sourceDoc.GetOptionsAsync() |> Async.AwaitTask
                let indentSize = options.GetOption(FormattingOptions.TabSize, FSharpCommonConstants.FSharpLanguageName)

                match SignatureGenerator.formatSymbol 
                        (getXmlDocBySignature fsSymbol) indentSize displayContext openDeclarations fsSymbol 
                        SignatureGenerator.Filterer.NoFilters SignatureGenerator.BlankLines.Default with
                | Some signatureText ->
                    let sigSourceText = SourceText.From(signatureText,Encoding.UTF8)
                    let sigDocument = project.AddDocument(fileName,sigSourceText,filePath=filePath)
                    projectInfoManager.RegisterSignature filePath sigDocument.Id                    
                    let! range = tryFindExactLocation sigDocument sigSourceText fsSymbol 
                    return Some (sigDocument.Id,filePath,range)
                | None -> 
                    return None
        }

        
    member __.TryFindMetadataRange(sourceDoc:Document, ast, fsSymbolUse) = 
        async {
            let! context = tryCreateMetadataContext sourceDoc ast fsSymbolUse
            return 
                context
                |> Option.map (fun (sigDocId,filePath, range) -> 
                    let zeroPos = mkPos 1 0
                    let r = range |> Option.getOrTry (fun _ -> mkRange filePath zeroPos zeroPos) 
                    sigDocId, r
                )
        }

    static member ClearXmlDocCache() =
        xmlDocCache.Clear()
          