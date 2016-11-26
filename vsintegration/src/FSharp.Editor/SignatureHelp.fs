// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Concurrent
open System.Collections.Generic
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks
open System.Linq
open System.Runtime.CompilerServices

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Editor.Implementation.Debugging
open Microsoft.CodeAnalysis.Editor.Shared.Utilities
open Microsoft.CodeAnalysis.Formatting
open Microsoft.CodeAnalysis.Host
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Options
open Microsoft.CodeAnalysis.SignatureHelp
open Microsoft.CodeAnalysis.Text

open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Tagging
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop

open Microsoft.FSharp.Compiler.Parser
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.SourceCodeServices.ItemDescriptionIcons

(*
type internal FSharpCompletionProvider(workspace: Workspace, serviceProvider: SVsServiceProvider) =
    inherit CompletionProvider()

    static let completionTriggers = [| '.' |]
    static let declarationItemsCache = ConditionalWeakTable<string, FSharpDeclarationListItem>()
    
    let xmlMemberIndexService = serviceProvider.GetService(typeof<IVsXMLMemberIndexService>) :?> IVsXMLMemberIndexService
    let documentationBuilder = XmlDocumentation.CreateDocumentationBuilder(xmlMemberIndexService, serviceProvider.DTE)

    static member ShouldTriggerCompletionAux(sourceText: SourceText, caretPosition: int, trigger: CompletionTriggerKind, getInfo: (unit -> DocumentId * string * string list)) =
        // Skip if we are at the start of a document
        if caretPosition = 0 then
            false

        // Skip if it was triggered by an operation other than insertion
        elif not (trigger = CompletionTriggerKind.Insertion) then
            false

        // Skip if we are not on a completion trigger
        else
          let c = sourceText.[caretPosition - 1]
          if not (completionTriggers |> Array.contains c) then
            false

          // Trigger completion if we are on a valid classification type
          else
            let documentId, filePath,  defines = getInfo()
            let triggerPosition = caretPosition - 1
            let textLine = sourceText.Lines.GetLineFromPosition(triggerPosition)
            let classifiedSpanOption =
                FSharpColorizationService.GetColorizationData(documentId, sourceText, textLine.Span, Some(filePath), defines, CancellationToken.None)
                |> Seq.tryFind(fun classifiedSpan -> classifiedSpan.TextSpan.Contains(triggerPosition))

            match classifiedSpanOption with
            | None -> false
            | Some(classifiedSpan) ->
                match classifiedSpan.ClassificationType with
                | ClassificationTypeNames.Comment -> false
                | ClassificationTypeNames.StringLiteral -> false
                | ClassificationTypeNames.ExcludedCode -> false
                | _ -> true // anything else is a valid classification type

*)


[<Shared>]
[<ExportSignatureHelpProvider("FSharpSignatureHelpProvider", FSharpCommonConstants.FSharpLanguageName)>]
type FSharpSignatureHelpProvider() =
    let ProvideMethodsAsyncAux(sourceText: SourceText, caretPosition: int, options: FSharpProjectOptions, filePath: string, textVersionHash: int) = async {
        let! parseResults = FSharpLanguageService.Checker.ParseFileInProject(filePath, sourceText.ToString(), options)
        let! checkFileAnswer = FSharpLanguageService.Checker.CheckFileInProject(parseResults, filePath, textVersionHash, sourceText.ToString(), options)
        let checkFileResults = 
            match checkFileAnswer with
            | FSharpCheckFileAnswer.Aborted -> failwith "Compilation isn't complete yet or was cancelled"
            | FSharpCheckFileAnswer.Succeeded(results) -> results

        let textLine = sourceText.Lines.GetLineFromPosition(caretPosition)
        let textLinePos = sourceText.Lines.GetLinePosition(caretPosition)
        let fcsTextLineNumber = textLinePos.Line + 1 // Roslyn line numbers are zero-based, FSharp.Compiler.Service line numbers are 1-based
        let textLineColumn = textLinePos.Character

        let qualifyingNames, partialName = QuickParse.GetPartialLongNameEx(textLine.ToString(), textLineColumn - 1) 
        let paramLocations = parseResults.FindNoteworthyParamInfoLocations(Range.Pos.fromZ brLine brCol)
        match paramLocations with
        | Some nwpl -> 
            let names = nwpl.LongId
            let lidEnd = nwpl.LongIdEndLocation
            let! methods = checkFileResults.GetMethodsAlternate(lidEnd.Line, lidEnd.Column, "", Some names)
            if (methods.Methods.Length = 0 || methods.MethodName.EndsWith("> )")) then
                None
            else                    
                // "methods" contains both real methods for this longId, as well as static-parameters in the case of type providers.
                // They "conflict" for cases of TP(...) (calling a constructor, no static args provided) versus TP<...> (static args), since
                // both point to the same longId.  However we can look at the character at the 'OpenParen' location and see if it is a '(' or a '<' and then
                // filter the "methods" list accordingly.
                let isThisAStaticArgumentsTip =
                    let parenLine, parenCol = Pos.toZ nwpl.OpenParenLocation 
                    let textAtOpenParenLocation =
                        if brSnapshot=null then
                            // we are unit testing, use the view
                            let _hr, buf = view.GetBuffer()
                            let _hr, s = buf.GetLineText(parenLine, parenCol, parenLine, parenCol+1)  
                            s
                        else
                            // we are in the product, use the ITextSnapshot
                            brSnapshot.GetText(MakeSpan(brSnapshot, parenLine, parenCol, parenLine, parenCol+1))
                    if textAtOpenParenLocation = "<" then
                        true
                    else
                        false  // note: textAtOpenParenLocation is not necessarily otherwise "(", for example in "sin 42.0" it is "4"
                    let filteredMethods =
                        [| for m in methods.Methods do 
                                if (isThisAStaticArgumentsTip && m.StaticParameters.Length > 0) ||
                                    (not isThisAStaticArgumentsTip && m.HasParameters) then   // need to distinguish TP<...>(...)  angle brackets tip from parens tip
                                    yield m |]
                    if filteredMethods.Length <> 0 then
                        Some (FSharpMethodListForAMethodTip(documentationBuilder, methods.MethodName, filteredMethods, nwpl, brSnapshot, isThisAStaticArgumentsTip) :> MethodListForAMethodTip)
                    else
                        None

(*
                        // If the name is an operator ending with ">" then it is a mistake 
                        // we can't tell whether "  >(" is a generic method call or an operator use 
                        // (it depends on the previous line), so we filter it
                        //
                        // Note: this test isn't particularly elegant - encoded operator name would be something like "( ...> )"                        
                    | _ -> 
                        None

type internal FSharpMethodListForAMethodTip(documentationBuilder: IDocumentationBuilder, methodsName, methods: FSharpMethodGroupItem[], nwpl: FSharpNoteworthyParamInfoLocations, snapshot: ITextSnapshot, isThisAStaticArgumentsTip: bool) =
    inherit MethodListForAMethodTip() 

    // Compute the tuple end points
    let tupleEnds = 
        let oneColAfter ((l,c): Pos01) = (l,c+1)
        let oneColBefore ((l,c): Pos01) = (l,c-1)
        [| yield Pos.toZ nwpl.LongIdStartLocation
           yield Pos.toZ nwpl.LongIdEndLocation
           yield oneColAfter (Pos.toZ nwpl.OpenParenLocation)
           for i in 0..nwpl.TupleEndLocations.Length-2 do
                yield Pos.toZ nwpl.TupleEndLocations.[i]
           let last = Pos.toZ nwpl.TupleEndLocations.[nwpl.TupleEndLocations.Length-1]
           yield if nwpl.IsThereACloseParen then oneColBefore last else last  |]

    let safe i dflt f = if 0 <= i && i < methods.Length then f methods.[i] else dflt

    let parameterRanges =
        let ss = snapshot
        [|  // skip 2 because don't want longid start&end, just want open paren and tuple ends
            for (sl,sc),(el,ec) in tupleEnds |> Seq.skip 2 |> Seq.pairwise do
                let span = ss.CreateTrackingSpan(MakeSpan(ss,sl,sc,el,ec), SpanTrackingMode.EdgeInclusive)
                yield span  |]

    let getParameters (m : FSharpMethodGroupItem) =  if isThisAStaticArgumentsTip then m.StaticParameters else m.Parameters

    do assert(methods.Length > 0)

    override x.GetColumnOfStartOfLongId() = nwpl.LongIdStartLocation.Column

    override x.IsThereACloseParen() = nwpl.IsThereACloseParen

    override x.GetNoteworthyParamInfoLocations() = tupleEnds

    override x.GetParameterNames() = nwpl.NamedParamNames

    override x.GetParameterRanges() = parameterRanges

    override x.GetCount() = methods.Length

    override x.GetDescription(methodIndex) = safe methodIndex "" (fun m -> XmlDocumentation.BuildMethodOverloadTipText(documentationBuilder, m.Description))
            
    override x.GetType(methodIndex) = safe methodIndex "" (fun m -> m.TypeText)

    override x.GetParameterCount(methodIndex) =  safe methodIndex 0 (fun m -> getParameters(m).Length)
            
    override x.GetParameterInfo(methodIndex, parameterIndex, nameOut, displayOut, descriptionOut) =
        let name,display = safe methodIndex ("","") (fun m -> let p = getParameters(m).[parameterIndex] in p.ParameterName,p.Display )
           
        nameOut <- name
        displayOut <- display
        descriptionOut <- ""

    override x.GetName(_index) = methodsName

    override x.OpenBracket = if isThisAStaticArgumentsTip then "<" else "("
    override x.CloseBracket = if isThisAStaticArgumentsTip then ">" else ")"

*)
        let results = List<SignatureHelpItem>()

        //if triggerInfo.TriggerCharacter = Nullable('(') || triggerInfo.TriggerCharacter = Nullable('<') then 

        for method in methods.Methods do
            let completionItem =  SignatureHelpItem(isVariadic=false,documentationFactory=(fun ct -> Seq.empty),prefixParts=Seq.empty,separatorParts=Seq.empty,suffixParts=Seq.empty,parameters=Seq.empty,descriptionParts=Seq.empty)
            //declarationItemsCache.Remove(completionItem.DisplayText) |> ignore // clear out stale entries if they exist
            //declarationItemsCache.Add(completionItem.DisplayText, declarationItem)
            results.Add(completionItem)


        let items = SignatureHelpItems(results,applicableSpan,argumentIndex,argumentCount,argumentName,optionalSelectedItem)
        return items
    }

    interface ISignatureHelpProvider with
        member this.IsTriggerCharacter(c) = c ='(' || c = '<' || c = ','
        member this.IsRetriggerCharacter(c) = c = ')' || c = '>'

        member this.GetItemsAsync(document, position, triggerInfo, cancellationToken) = 
            async {
                match FSharpLanguageService.GetOptions(document.Project.Id) with
                | Some(options) ->
                    //let exists, declarationItem = declarationItemsCache.TryGetValue(completionItem.DisplayText)
                    //if exists then
                        let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                        let! textVersion = document.GetTextVersionAsync(cancellationToken) |> Async.AwaitTask
                        return! ProvideMethodsAsyncAux(sourceText, position, options, document.FilePath, textVersion.GetHashCode())
                   // else
                   //     return results
                | None -> 
                    return null // SignatureHelpItems([| |],
            } |> CommonRoslynHelpers.StartAsyncAsTask cancellationToken

