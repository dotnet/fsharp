// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.LanguageService

open System
open System.Collections.Generic
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Shell.Interop 
open Microsoft.VisualStudio.TextManager.Interop 
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.OLE.Interop
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices


module internal TaggedText =
    let appendTo (sb: System.Text.StringBuilder) (t: Layout.TaggedText) = sb.Append t.Text |> ignore 
 
/// Represents all the information necessary to display and navigate 
/// within a method tip (e.g. param info, overloads, ability to move thru overloads and params)
///
/// Completes the base class "MethodListForAMethodTip" with F#-specific implementations.
//
// Note, the MethodListForAMethodTip type inherited by this code is defined in the F# Project System C# code. This is the only implementation
// in the codebase, hence we are free to change it and refactor things (e.g. bring more things into F# code) 
// if we wish.
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

    override x.GetParameterNames() = nwpl.NamedParamNames |> Array.map Option.toObj

    override x.GetParameterRanges() = parameterRanges

    override x.GetCount() = methods.Length

    override x.GetDescription(methodIndex) = safe methodIndex "" (fun m -> 
        let buf = Text.StringBuilder()
        XmlDocumentation.BuildMethodOverloadTipText(documentationBuilder, TaggedText.appendTo buf, TaggedText.appendTo buf, m.StructuredDescription, true)
        buf.ToString()
        )
            
    override x.GetReturnTypeText(methodIndex) = safe methodIndex "" (fun m -> m.ReturnTypeText)

    override x.GetParameterCount(methodIndex) =  safe methodIndex 0 (fun m -> getParameters(m).Length)
            
    override x.GetParameterInfo(methodIndex, parameterIndex, nameOut, displayOut, descriptionOut) =
        let name,display = safe methodIndex ("","") (fun m -> let p = getParameters(m).[parameterIndex] in p.ParameterName, p.Display )
           
        nameOut <- name
        displayOut <- display
        descriptionOut <- ""

    override x.GetName(_index) = methodsName

    override x.OpenBracket = if isThisAStaticArgumentsTip then "<" else "("
    override x.CloseBracket = if isThisAStaticArgumentsTip then ">" else ")"

type internal ObsoleteGlyph =
    | Class = 0
    | Constant = 6
    | FunctionType = 12
    | Enum = 18
    | EnumMember = 24
    | Event =30
    | Exception = 36
    | Interface = 48
    | Method = 72
    | FunctionValue = 74
    | Module = 84
    | Namespace = 90
    | Property = 102
    | ValueType = 108
    | RareType = 120
    | Record = 126
    | DiscriminatedUnion = 132

/// A collections of declarations as would be returned by a dot-completion request.
//
// Note, the Declarations type inherited by this code is defined in the F# Project System C# code. This is the only implementation
// in the codebase, hence we are free to change it and refactor things (e.g. bring more things into F# code) 
// if we wish.
type internal FSharpDeclarations(documentationBuilder, declarations: FSharpDeclarationListItem[], reason: BackgroundRequestReason) = 
        
    inherit Declarations()  

    // Sort the declarations, NOTE: we used ORDINAL comparison here, this is "by design" from F# 2.0, partly because it puts lowercase last.
    let declarations = declarations |> Array.sortWith (fun d1 d2 -> compare d1.Name d2.Name)
    let mutable lastBestMatch = ""
    let isEmpty = (declarations.Length = 0)

    let tab = Dictionary<string,FSharpDeclarationListItem[]>()

    // Given a prefix, narrow the items to the include the ones containing that prefix, and store in a lookaside table
    // attached to this declaration set.
    let trimmedDeclarations filterText = 
        if reason = BackgroundRequestReason.DisplayMemberList then declarations 
        elif tab.ContainsKey filterText then tab.[filterText] 
        else 
            let matcher = AbstractPatternMatcher.Singleton
            let decls = 
                // Find the first prefix giving a non-empty declaration set after filtering
                seq { for i in filterText.Length-1 .. -1 .. 0 do 
                            let filterTextPrefix = filterText.[0..i]
                            match tab.TryGetValue filterTextPrefix with
                            | true, decls -> yield decls
                            | false, _ -> yield declarations |> Array.filter (fun s -> matcher.MatchSingleWordPattern(s.Name, filterTextPrefix)<>null) 
                      yield declarations }
                |> Seq.tryFind (fun arr -> arr.Length > 0)
                |> (function None -> declarations | Some s -> s)
            tab.[filterText] <- decls
            decls

    override decl.GetCount(filterText) = 
        let decls = trimmedDeclarations filterText
        decls.Length

    override decl.GetDisplayText(filterText, index) =
        let decls = trimmedDeclarations filterText
        if (index >= 0 && index < decls.Length) then
            decls.[index].Name
        else ""

    override decl.IsEmpty() = isEmpty

    override decl.GetName(filterText, index) =
        let decls = trimmedDeclarations filterText
        if (index >= 0 && index < decls.Length) then
            let item = decls.[index]
            if (item.Glyph = FSharpGlyph.Error) then
                ""
            else 
                item.Name
        else String.Empty
    
    override decl.GetNameInCode(filterText, index) =
        let decls = trimmedDeclarations filterText
        if (index >= 0 && index < decls.Length) then
            let item = decls.[index]
            if (item.Glyph = FSharpGlyph.Error) then
                ""
            else 
                item.NameInCode
        else String.Empty

    override decl.GetDescription(filterText, index) =
        let decls = trimmedDeclarations filterText
        if (index >= 0 && index < decls.Length) then
            let buf = Text.StringBuilder()
            XmlDocumentation.BuildDataTipText(documentationBuilder, TaggedText.appendTo buf, TaggedText.appendTo buf, decls.[index].StructuredDescriptionText) 
            buf.ToString()
        else ""

    override decl.GetGlyph(filterText, index) =
        let decls = trimmedDeclarations filterText
        //The following constants are the index of the various glyphs in the ressources of Microsoft.VisualStudio.Package.LanguageService.dll
        if index >= 0 && index < decls.Length then
            // Get old VS glyph indexes, just for tests.
            match decls.[index].Glyph with
            | FSharpGlyph.Class
            | FSharpGlyph.Typedef -> Some ObsoleteGlyph.Class
            | FSharpGlyph.Constant -> Some ObsoleteGlyph.Constant
            | FSharpGlyph.Enum -> Some ObsoleteGlyph.Enum
            | FSharpGlyph.EnumMember -> Some ObsoleteGlyph.EnumMember
            | FSharpGlyph.Event -> Some ObsoleteGlyph.Event
            | FSharpGlyph.Exception -> Some ObsoleteGlyph.Exception
            | FSharpGlyph.Interface -> Some ObsoleteGlyph.Interface
            | FSharpGlyph.ExtensionMethod
            | FSharpGlyph.Method
            | FSharpGlyph.OverridenMethod -> Some ObsoleteGlyph.Method
            | FSharpGlyph.Module -> Some ObsoleteGlyph.Module
            | FSharpGlyph.NameSpace -> Some ObsoleteGlyph.Namespace
            | FSharpGlyph.Property -> Some ObsoleteGlyph.Property
            | FSharpGlyph.Struct -> Some ObsoleteGlyph.ValueType
            | FSharpGlyph.Type -> Some ObsoleteGlyph.Class
            | FSharpGlyph.Union -> Some ObsoleteGlyph.DiscriminatedUnion
            | FSharpGlyph.Field
            | FSharpGlyph.Delegate
            | FSharpGlyph.Variable
            | FSharpGlyph.Error -> None
            |> Option.defaultValue ObsoleteGlyph.Class
            |> int
        else 0

    // This method is called to get the string to commit to the source buffer.
    // Note that the initial extent is only what the user has typed so far.
    override decl.OnCommit(filterText, index) =
        // We intercept this call only to get the initial extent
        // of what was committed to the source buffer.
        let result = decl.GetName(filterText, index)
        Microsoft.FSharp.Compiler.Lexhelp.Keywords.QuoteIdentifierIfNeeded result

    override decl.IsCommitChar(commitCharacter) =
        // Usual language identifier rules...
        not (Char.IsLetterOrDigit(commitCharacter) || commitCharacter = '_')
        
    // A helper to aid in determining how much text is relevant to the items chosen in the completion list.
    override decl.Reason = reason
        
    // Note, there is no real reason for this code to use byrefs, except that we're calling it from C#.
    override decl.GetBestMatch(filterText, textSoFar, index : int byref, uniqueMatch : bool byref, shouldSelectItem : bool byref) =
        let decls = trimmedDeclarations filterText
        let compareStrings(s,t,l,b : bool) = System.String.Compare(s,0,t,0,l,b)
        let tryFindDeclIndex text length ignoreCase = 
            decls 
            |> Array.tryFindIndex (fun d -> compareStrings(d.Name, text, length, ignoreCase) = 0)
        // The best match is the first item that begins with the longest prefix of the 
        // given word (value).  
        let rec findMatchOfLength len ignoreCase = 
            if len = 0 then
                let indexLastBestMatch = tryFindDeclIndex lastBestMatch lastBestMatch.Length ignoreCase
                match indexLastBestMatch with
                | Some index -> (index, false, false)
                | None -> (0,false, false)
            else 
                let firstMatchingLenChars = tryFindDeclIndex textSoFar len ignoreCase
                match firstMatchingLenChars with
                | Some index -> 
                    lastBestMatch <- decls.[index].Name
                    let select = len = textSoFar.Length
                    if (index <> decls.Length- 1) && (compareStrings(decls.[index+1].Name , textSoFar, len, ignoreCase) = 0) 
                    then (index, false, select)
                    else (index, select, select)
                | None -> 
                    match ignoreCase with
                    | false -> findMatchOfLength len true
                    | true -> findMatchOfLength (len-1) false
        let (i, u, p) = findMatchOfLength textSoFar.Length false
        index <- i
        uniqueMatch <- u
        let preselect =
            // select an item in the list if what the user has typed is a prefix...
            p || (
                // ... or if the list has filtered down to a single item, and the user's text is still a 'match'
                // for example, "System.Console.WrL" will filter down to one, and still be a match, whereas
                // "System.Console.WrLx" will filter down to one, but no longer be a match
                decls.Length = 1 &&
                AbstractPatternMatcher.Singleton.MatchSingleWordPattern(decls.[0].Name, textSoFar)<>null
            )
        shouldSelectItem <- preselect

    // This method is called after the string has been committed to the source buffer.
    //
    // Note: this override is a bit out of place as nothing in this type has anything to do with text buffers.
    override decl.OnAutoComplete(_textView, _committedText, _commitCharacter, _index) =
        // Would need special handling code for snippets.
        '\000'


                   
/// Implements the remainder of the IntellisenseInfo base type from MPF.
/// 
/// The scope object is the result of computing a particular typecheck. It may be queried for things like
/// data tip text, member completion and so forth.
type internal FSharpIntellisenseInfo 
                    (/// The recent result of parsing
                     untypedResults: FSharpParseFileResults,
                     /// Line/column/snapshot of BackgroundRequest that initiated creation of this scope
                     brLine:int, brCol:int, brSnapshot:ITextSnapshot,
                     /// The possibly staler result of typechecking
                     typedResults: FSharpCheckFileResults,
                     /// The project
                     projectSite: IProjectSite,
                     /// The text view
                     view: IVsTextView,
                     /// The colorizer for this view (though why do we need to be lazy about creating this?)
                     colorizer: Lazy<FSharpColorizer>,
                     /// A service that will provide Xml Content
                     documentationBuilder : IDocumentationBuilder,
                     provideMethodList : bool
                     ) = 
        inherit IntellisenseInfo() 

        let methodList = 
          if provideMethodList then 
            try
                // go ahead and compute this now, on this background thread, so will have info ready when UI thread asks
                let noteworthyParamInfoLocations = untypedResults.FindNoteworthyParamInfoLocations(Range.Pos.fromZ brLine brCol)

                // we need some typecheck info, even if stale, in order to look up e.g. method overload types/xmldocs
                if typedResults.HasFullTypeCheckInfo then 

                    // we need recent parse info to e.g. know how many commas and thus how many args there are
                    match noteworthyParamInfoLocations with
                    | Some nwpl -> 
                        // Note: this may alternatively workaround some parts of 90778 - the real fix for that is to have before-overload-resolution name-sink work correctly.
                        // However it also deals with stale typecheck info that may not have recorded name resolutions for a recently-typed long-id.
                        let names = nwpl.LongId
                        // "names" is a long-id that we can fallback-lookup in the local environment if captured name resolutions finds nothing at the location.
                        // This can happen e.g. if you are typing quickly and the typecheck results are stale enough that you don't have a captured resolution for
                        // the name you just typed, but fresh enough that you do have the right name-resolution-environment to look up the name.
                        let lidEnd = nwpl.LongIdEndLocation
                        let methods = typedResults.GetMethodsAlternate(lidEnd.Line, lidEnd.Column, "", Some names)  |> Async.RunSynchronously
                        
                        // If the name is an operator ending with ">" then it is a mistake 
                        // we can't tell whether "  >(" is a generic method call or an operator use 
                        // (it depends on the previous line), so we filter it
                        //
                        // Note: this test isn't particularly elegant - encoded operator name would be something like "( ...> )"                        
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
                    | _ -> 
                        None
                else
                    // GetMethodListForAMethodTip found no TypeCheckInfo in ParseResult.
                    None
            with e-> 
                Assert.Exception(e)
                reraise()
          else None

        let hasTextChangedSinceLastTypecheck (curTextSnapshot: ITextSnapshot, oldTextSnapshot: ITextSnapshot, ((sl:int,sc:int),(el:int,ec:int))) = 
            // compare the text from (sl,sc) to (el,ec) to see if it changed from the old snapshot to the current one
            // (sl,sc)-(el,ec) are line/col positions in the current snapshot
            if el >= oldTextSnapshot.LineCount then
                true  // old did not even have 'el' many lines, note 'el' is zero-based
            else
                assert(el < curTextSnapshot.LineCount)
                let oldFirstLine = oldTextSnapshot.GetLineFromLineNumber sl  
                let oldLastLine = oldTextSnapshot.GetLineFromLineNumber el
                if oldFirstLine.Length < sc || oldLastLine.Length < ec then
                    true  // one of old lines was not even long enough to contain the position we're looking at
                else
                    let posOfStartInOld = oldFirstLine.Start.Position + sc
                    let posOfEndInOld = oldLastLine.Start.Position + ec
                    let curFirstLine = curTextSnapshot.GetLineFromLineNumber sl  
                    let curLastLine = curTextSnapshot.GetLineFromLineNumber el  
                    assert(curFirstLine.Length >= sc)
                    assert(curLastLine.Length >= ec)
                    let posOfStartInCur = curFirstLine.Start.Position + sc
                    let posOfEndInCur = curLastLine.Start.Position + ec
                    if posOfEndInCur - posOfStartInCur <> posOfEndInOld - posOfStartInOld then
                        true  // length of text between two endpoints changed
                    else
                        let mutable oldPos = posOfStartInOld
                        let mutable curPos = posOfStartInCur
                        let mutable ok = true
                        while ok && oldPos < posOfEndInOld do
                            let oldChar = oldTextSnapshot.[oldPos]
                            let curChar = curTextSnapshot.[curPos]
                            if oldChar <> curChar then
                                ok <- false
                            oldPos <- oldPos + 1
                            curPos <- curPos + 1
                        not ok

        /// Implements the corresponding abstract member from IntellisenseInfo in MPF.
        override scope.GetDataTipText(line, col) =
            // in cases like 'A<int>' when cursor in on '<' there is an ambiguity that cannot be resolved based only on lexer information
            // '<' can be treated both as operator and as part of identifier
            // in this case we'll do 2 passes:
            // 1. treatTokenAsIdentifier=false - we'll pick raw token under the cursor and try find it among resolved names, is attempt was successful - great we are done, otherwise
            // 2. treatTokenAsIdentifier=true - even if raw token was recognized as operator we'll use different branch 
            // that calls QuickParse.GetCompleteIdentifierIsland and then tries previous column...
            let rec getDataTip alwaysTreatTokenAsIdentifier =
                let token = colorizer.Value.GetTokenInfoAt(VsTextLines.TextColorState (VsTextView.Buffer view),line,col)

                try
                    let lineText = VsTextLines.LineText (VsTextView.Buffer view) line
                    
                    // Try the actual column first...
                    let tokenTag, col, possibleIdentifier, makeSecondAttempt =
                      if token.Type = TokenType.Operator && not alwaysTreatTokenAsIdentifier then                      
                          let tag, startCol, endCol = OperatorToken.asIdentifier token                      
                          let op = lineText.Substring(startCol, endCol - startCol)
                          tag, startCol, Some(op, endCol, false), true
                      else
                          match (QuickParse.GetCompleteIdentifierIsland false lineText col) with
                          | None when col > 0 -> 
                              // Try the previous column & get the token info for it
                              let tokenTag = 
                                  let token = colorizer.Value.GetTokenInfoAt(VsTextLines.TextColorState (VsTextView.Buffer view),line,col - 1)
                                  token.Token 
                              let possibleIdentifier = QuickParse.GetCompleteIdentifierIsland false lineText (col - 1)
                              tokenTag, col - 1, possibleIdentifier, false
                          | _ as poss -> token.Token, col, poss, false

                    let diagnosticTipSpan = TextSpan(iStartLine=line, iEndLine=line, iStartIndex=col, iEndIndex=col+1)
                    match possibleIdentifier with 
                    | None -> "",diagnosticTipSpan
                    | Some (s,colAtEndOfNames, isQuotedIdentifier) -> 

                        if typedResults.HasFullTypeCheckInfo then 
                            let qualId  = PrettyNaming.GetLongNameFromString s
                                                
                            // Correct the identifier (e.g. to correctly handle active pattern names that end with "BAR" token)
                            let tokenTag = QuickParse.CorrectIdentifierToken s tokenTag
                            let dataTip = typedResults.GetStructuredToolTipTextAlternate(Range.Line.fromZ line, colAtEndOfNames, lineText, qualId, tokenTag) |> Async.RunSynchronously

                            match dataTip with
                            | FSharpStructuredToolTipText.FSharpToolTipText [] when makeSecondAttempt -> getDataTip true
                            | _ -> 
                                let buf = Text.StringBuilder()
                                XmlDocumentation.BuildDataTipText(documentationBuilder, TaggedText.appendTo buf, TaggedText.appendTo buf, dataTip)

                                // The data tip is located w.r.t. the start of the last identifier
                                let sizeFixup = if isQuotedIdentifier then 4 else 0
                                let lastStringLength = (qualId |> List.rev |> List.head).Length  + sizeFixup

                                // This is the span of text over which the data tip is active. If the mouse moves away from it then the
                                // data tip goes away
                                let dataTipSpan = TextSpan(iStartLine=line, iEndLine=line, iStartIndex=max 0 (colAtEndOfNames-lastStringLength), iEndIndex=colAtEndOfNames)
                                (buf.ToString(), dataTipSpan)                                
                        else
                            "Bug: TypeCheckInfo option was None", diagnosticTipSpan
                with e -> 
                    Assert.Exception(e)
                    reraise()

            getDataTip false
            

        /// Determine whether to force the use a synchronous parse 
        static member IsReasonRequiringSyncParse(reason) =
            match reason with
            | BackgroundRequestReason.MethodTip // param info...
            | BackgroundRequestReason.MatchBracesAndMethodTip // param info...
            | BackgroundRequestReason.CompleteWord | BackgroundRequestReason.MemberSelect | BackgroundRequestReason.DisplayMemberList // and intellisense-completion...
                -> true // ...require a sync parse (so as to call FindNoteworthyParamInfoLocations and GetRangeOfExprLeftOfDot, respectively)
            | _ -> false

        /// Implements the corresponding abstract member from IntellisenseInfo in MPF.
        override scope.GetDeclarations(textSnapshot, line, col, reason) =
            assert(FSharpIntellisenseInfo.IsReasonRequiringSyncParse(reason))
            async {
                try
                    let isInCommentOrString =
                        let tokenInfo = colorizer.Value.GetTokenInfoAt(VsTextLines.TextColorState (VsTextView.Buffer view),line,col)
                        let prevCol = max 0 (col - 1)
                        let prevTokenInfo = colorizer.Value.GetTokenInfoAt(VsTextLines.TextColorState (VsTextView.Buffer view),line,prevCol)
                        // denotes if we got token that matches exact specified position or it was just last token before EOF
                        let exactMatch = col >= tokenInfo.StartIndex && col <= tokenInfo.EndIndex
                        exactMatch && ((tokenInfo.Color = TokenColor.Comment && prevTokenInfo.Color = TokenColor.Comment) || 
                                       (tokenInfo.Color = TokenColor.String  && prevTokenInfo.Color = TokenColor.String))
                    if isInCommentOrString then
                        // We don't want to show info in comments & strings (in case of exact match)
                        // (but we want to show it if the thing before or after isn't comment/string)
                        return null 
                    
                    elif typedResults.HasFullTypeCheckInfo then 
                        let lineText = VsTextLines.LineText (VsTextView.Buffer view) line
                        let colorState = VsTextLines.TextColorState (VsTextView.Buffer view)
                        let state = VsTextColorState.GetColorStateAtStartOfLine colorState line
                        let tokens = colorizer.Value.GetFullLineInfo(lineText, state)
                        // An ugly check to suppress declaration lists at 'System.Int32.'
                        if reason = BackgroundRequestReason.MemberSelect && col > 1 && lineText.[col-2]='.' then
                            //           System.Int32..Parse("42")
                            // just pressed dot here  ^
                            // don't want any completion for that, we only trigger a MemberSelect on the ".." token in order to be able to get completion
                            //           System.Int32..Parse("42")
                            //                 here  ^
                            return null
                        // An ugly check to suppress declaration lists at 'member' declarations
                        elif QuickParse.TestMemberOrOverrideDeclaration tokens then  
                            return null
                        else
                            let untypedParseInfoOpt =
                                if reason = BackgroundRequestReason.MemberSelect || reason = BackgroundRequestReason.DisplayMemberList || reason = BackgroundRequestReason.CompleteWord then
                                    Some untypedResults
                                else
                                    None
                            // TODO don't use QuickParse below, we have parse info available
                            let plid = QuickParse.GetPartialLongNameEx(lineText, col-1) 
                            ignore plid // for breakpoint

                            let detectTextChange (oldTextSnapshotInfo: obj, range) = 
                                let oldTextSnapshot = oldTextSnapshotInfo :?> ITextSnapshot
                                hasTextChangedSinceLastTypecheck (textSnapshot, oldTextSnapshot, Range.Range.toZ range)

                            let! decls = typedResults.GetDeclarationListInfo(untypedParseInfoOpt, Range.Line.fromZ line, col, lineText, fst plid, snd plid, (fun() -> []), detectTextChange) 
                            return (new FSharpDeclarations(documentationBuilder, decls.Items, reason) :> Declarations) 
                    else
                        // no TypeCheckInfo in ParseResult.
                        return null 
                with e-> 
                    Assert.Exception(e)
                    raise e
                    return null
            }

        /// Get methods for parameter info
        override scope.GetMethodListForAMethodTip() = methodList

        override this.GetF1KeywordString(span : TextSpan, context : IVsUserContext) : unit =
            let shouldTryToFindIdentToTheLeft (token : FSharpTokenInfo) =
                match token.CharClass with
                | FSharpTokenCharKind.WhiteSpace -> true
                | FSharpTokenCharKind.Delimiter -> true
                | _ -> false

            let keyword =
                let line = span.iStartLine
                let lineText = VsTextLines.LineText (VsTextView.Buffer view) line                       
                let tokenInformation, col =
                    let col = 
                        if span.iStartIndex = lineText.Length && span.iStartIndex > 0 then
                            // if we are at the end of the line, we always step back one character
                            span.iStartIndex - 1
                        else
                            span.iStartIndex
                    let textColorState = VsTextLines.TextColorState (VsTextView.Buffer view)
                    match colorizer.Value.GetTokenInformationAt(textColorState,line,col) with
                    | Some token as original when col > 0 && shouldTryToFindIdentToTheLeft token ->
                        // try to step back one char
                        match colorizer.Value.GetTokenInformationAt(textColorState,line,col-1) with
                        | Some token as newInfo when token.CharClass <> FSharpTokenCharKind.WhiteSpace -> newInfo, col - 1
                        |   _ -> original, col
                    | otherwise -> otherwise, col

                match tokenInformation with
                |   None -> None
                |   Some token ->
                        match token.CharClass, token.ColorClass with
                        |   FSharpTokenCharKind.Keyword, _
                        |   FSharpTokenCharKind.Operator, _ 
                        |   _, FSharpTokenColorKind.PreprocessorKeyword ->
                                lineText.Substring(token.LeftColumn, token.RightColumn - token.LeftColumn + 1) + "_FS" |> Some
                                
                        |   (FSharpTokenCharKind.Comment|FSharpTokenCharKind.LineComment), _ -> Some "comment_FS"
                                
                        |   FSharpTokenCharKind.Identifier, _ ->            
                                try
                                    let lineText = VsTextLines.LineText (VsTextView.Buffer view) line
                                    let possibleIdentifier = QuickParse.GetCompleteIdentifierIsland false lineText col
                                    match possibleIdentifier with
                                    |   None -> None // no help keyword
                                    |   Some(s,colAtEndOfNames, _) ->
                                            if typedResults.HasFullTypeCheckInfo then 
                                                let qualId = PrettyNaming.GetLongNameFromString s
                                                match typedResults.GetF1KeywordAlternate(Range.Line.fromZ line,colAtEndOfNames, lineText, qualId) |> Async.RunSynchronously with
                                                | Some s -> Some s
                                                | None -> None 
                                            else None                           
                                with e ->
                                    Assert.Exception (e)
                                    reraise()
                        | _ -> None
            match keyword with
            |   Some f1Keyword ->
                    context.AddAttribute(VSUSERCONTEXTATTRIBUTEUSAGE.VSUC_Usage_Filter, "devlang", "fsharp") |> ignore
                    // TargetFrameworkMoniker is not set for files that are not part of project (scripts and orphan fs files)
                    if not (String.IsNullOrEmpty projectSite.TargetFrameworkMoniker) then
                        context.AddAttribute(VSUSERCONTEXTATTRIBUTEUSAGE.VSUC_Usage_Filter, "TargetFrameworkMoniker", projectSite.TargetFrameworkMoniker) |> ignore
                    context.AddAttribute(VSUSERCONTEXTATTRIBUTEUSAGE.VSUC_Usage_LookupF1_CaseSensitive, "keyword", f1Keyword) |> ignore
                    ()
            |   None -> ()

          
        // for tests
        member this.GotoDefinition (textView, line, column) =
            GotoDefinition.GotoDefinition (colorizer.Value, typedResults, textView, line, column)

        override this.Goto (textView, line, column) =
            GotoDefinition.GotoDefinition (colorizer.Value, typedResults, textView, line, column)

        // This is called on the UI thread after fresh full typecheck results are available
        member this.OnParseFileOrCheckFileComplete(source: IFSharpSource) =
            for line in colorizer.Value.SetExtraColorizations(typedResults.GetSemanticClassification None) do
                source.RecolorizeLine line

