// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.LanguageService

open Internal.Utilities.Collections
open Microsoft.FSharp.Compiler.SourceCodeServices
open System
open System.Text
open System.IO
open System.Collections.Generic
open System.Collections
open System.Configuration
open System.Diagnostics
open System.Globalization
open System.Threading
open System.ComponentModel.Design
open System.Runtime.InteropServices
open Microsoft.VisualStudio
open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop 
open Microsoft.VisualStudio.TextManager.Interop 
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.OLE.Interop
open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.FSharp.Compiler.AbstractIL.Diagnostics 
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library
open Microsoft.FSharp.Compiler.Lib
open Internal.Utilities.Debug

#nowarn "45" // This method will be made public in the underlying IL because it may implement an interface or override a method
#nowarn "47" // Self-referential uses within object constructors will be checked for initialization soundness at runtime. Consider placing self-references within 'do' statements after the last 'let' binding in a class

#if DEBUG
module ParserState = 
    open System.Text.RegularExpressions
    let private rxState = Regex("state (\d+):", RegexOptions.Compiled)
    let private (|StateLine|_|) l = 
        let m = rxState.Match l
        if m.Success then m.Groups.[1].Value |> int |> Some
        else None
    
    let FsyListingVariableName = "FSY_LISTING"
    let private cache = 
        lazy
            match Environment.GetEnvironmentVariable FsyListingVariableName with
            | null -> None
            | x when not (File.Exists x) -> None
            | path ->
                let d = Dictionary()
                try
                    (
                        use f = File.OpenText(path)
                        let rec doRead () = 
                            match f.ReadLine() with
                            | null -> ()
                            | StateLine l ->
                                match readStates [] with
                                | Some states ->
                                    d.Add(l, states)
                                    doRead ()
                                | None -> ()
                            | _ -> doRead ()
                        and readStates acc = 
                            match f.ReadLine() with
                            | null -> None
                            | x when String.IsNullOrEmpty x -> Some (List.rev acc)
                            | x -> readStates (x::acc)
                        doRead ()
                    )
                    let lastWriteTime = File.GetLastWriteTime(path)
                    Some (d, lastWriteTime, path)
                with
                    _ -> None                

    let Get s = 
        match cache.Value with
        | None -> None
        | Some (dict, lastWriteTime, path) ->
            match dict.TryGetValue s with
            | true, v -> Some (v, lastWriteTime, path)
            | _ -> None
#endif

module Implementation =

    module FSharpConstants = 
        let fsharpCodeDomProviderName       = "FSharp"  

        // These are the IDs from fslangservice.dll
        let packageGuidString               = "871D2A70-12A2-4e42-9440-425DD92A4116"
        [<Literal>]
        let languageServiceGuidString       = Microsoft.VisualStudio.FSharp.Shared.FSharpCommonConstants.languageServiceGuidString

        // These are the IDs from the Python sample:
        let intellisenseProviderGuidString  = "8b1807ea-d222-4765-afa8-c092d480e451"
        
        // These are the entries from fslangservice.dll
        let PLKMinEdition                   = "standard"
        let PLKCompanyName                  = "Microsoft" // "Microsoft Corporation"
        let PLKProductName                  = "f#" // "Visual Studio Integration of FSharp Language Service"
        let PLKProductVersion               = "1.0"
        let PLKResourceID                   = 1s
        
        // App.config keys for determining whether not-shipping features are turned on or off
        // Edit devenv.exe.config and place these at the end.
        //   <appSettings>
        //      <add key="fsharp-regions-enabled" value="true" />
        //      <add key="fsharp-navigationbar-enabled" value="true" />
        //      <add key="fsharp-standalone-file-intellisense-enabled" value="false" />
        //   </appSettings>
        let enableNavBar = "fsharp-navigationbar-enabled"
        let enableRegions = "fsharp-regions-enabled"
        let enableStandaloneFileIntellisense = "fsharp-standalone-file-intellisense-enabled"
        let enableLanguageService = "fsharp-language-service-enabled"

    type FSharpColorableItem(canonicalName: string, displayName : Lazy<string>, foreground, background) =
        interface IVsColorableItem with 
            member x.GetDefaultColors(piForeground, piBackground) =
#if DEBUG
                Check.ArrayArgumentNotNullOrEmpty piForeground "piForeground"
                Check.ArrayArgumentNotNullOrEmpty piBackground "piBackground"
#endif
                piForeground.[0] <- foreground
                piBackground.[0] <- background
                VSConstants.S_OK
            member x.GetDefaultFontFlags(pdwFontFlags) =
                pdwFontFlags <- 0u
                VSConstants.S_OK
            member x.GetDisplayName(pbstrName) =
                pbstrName <- displayName.Force()
                VSConstants.S_OK 
        interface IVsMergeableUIItem with
            member this.GetCanonicalName(s) =
                s <- canonicalName
                VSConstants.S_OK 
            member this.GetDescription(s) =
                s <- ""
                VSConstants.S_OK 
            member x.GetDisplayName(s) =
                s <- displayName.Force()
                VSConstants.S_OK 
            member x.GetMergingPriority(i) =
                i <- 0x1000  // as per docs, MS products should use a value between 0x1000 and 0x2000
                VSConstants.S_OK 

    /// A Single declaration.
    type FSharpDeclaration( documentationProvider : IdealDocumentationProvider,
                            decl:Declaration ) = 
        member d.Kind with get() = decl.Glyph // Note: Snippet is Kind=205
        member d.Shortcut with get() = ""
        member d.Title with get() = decl.Name
        member d.Description 
            with get() = 
                XmlDocumentation.BuildDataTipText(documentationProvider,decl.DescriptionText)
 
    type FSharpMethodListForAMethodTip(documentationProvider : IdealDocumentationProvider, methodsName, methods: Method[], nwpl : NoteworthyParamInfoLocations, snapshot : ITextSnapshot, isThisAStaticArgumentsTip : bool) =
        inherit MethodListForAMethodTip() 

        let tupleEnds = [| 
            yield nwpl.LongIdStartLocation
            yield nwpl.LongIdEndLocation
            yield nwpl.OpenParenLocation
            for i in 0..nwpl.TupleEndLocations.Length-2 do
                let line,col = nwpl.TupleEndLocations.[i]
                yield line, col-1  // col is the location of the comma, we want param to end just before it
            let line, col = nwpl.TupleEndLocations.[nwpl.TupleEndLocations.Length-1]
            yield line,(if nwpl.IsThereACloseParen then col-1 else col) 
            |]

        let safe i dflt f = if 0 <= i && i < methods.Length then f methods.[i] else dflt

        let parameterRanges =
            let ss = snapshot
            [| 
                // skip 2 because don't want longid start&end, just want open paren and tuple ends
                for (sl,sc),(el,ec) in tupleEnds |> Seq.skip 2 |> Seq.pairwise do
                    let span = ss.CreateTrackingSpan(FSharpMethodListForAMethodTip.MakeSpan(ss,sl,sc,el,ec), SpanTrackingMode.EdgeInclusive)
                    yield span 
            |]

        do assert(methods.Length > 0)

        static member MakeSpan(ss:ITextSnapshot, sl, sc, el, ec) =
            let makeSnapshotPoint l c =
                let lineNum, fsharpRangeIsPastEOF =
                    // -1 because F# reports 1-based line nums, whereas VS wants 0-based
                    if l - 1 <= ss.LineCount - 1 then
                        l - 1, false
                    else
                        ss.LineCount - 1, true
                let line = ss.GetLineFromLineNumber(lineNum)
                line.Start.Add(if fsharpRangeIsPastEOF then line.Length else Math.Min(c, line.Length))
            let start = makeSnapshotPoint sl sc
            let end_  = makeSnapshotPoint el ec
            assert(start.CompareTo(end_) <= 0)
            (new SnapshotSpan(start, end_)).Span

        override x.GetColumnOfStartOfLongId() = (snd nwpl.LongIdStartLocation)-1 // is 1-based, wants 0-based

        override x.IsThereACloseParen() = nwpl.IsThereACloseParen

        override x.GetNoteworthyParamInfoLocations() = tupleEnds

        override x.GetParameterNames() = nwpl.NamedParamNames

        override x.GetParameterRanges() = parameterRanges

        override x.GetCount() = methods.Length

        override x.GetDescription(index) = 
            safe index "" (fun m -> XmlDocumentation.BuildMethodOverloadTipText(documentationProvider, m.Description))
            
        override x.GetType(index) = safe index "" (fun m -> m.Type)

        override x.GetParameterCount(index) =  safe index 0 (fun m -> m.Parameters.Length) 
            
        override x.GetParameterInfo(index, parameter, nameOut, displayOut, descriptionOut) =
            let name,display,description = safe index ("","","") (fun m -> 
                                                                        let p = m.Parameters.[parameter]
                                                                        p.Name,p.Display,p.Description )
           
            nameOut <- name
            displayOut <- display
            descriptionOut <- description

        override x.GetName(_index) = methodsName

        override x.OpenBracket = if isThisAStaticArgumentsTip then "<" else "("
        override x.CloseBracket = if isThisAStaticArgumentsTip then ">" else ")"

    /// A collections of declarations as would be returned by a dot-completion request.
    //
    // Note, the Declarations type inherited by this code is defined in the F# Project System C# code. This is the only implementation
    // in the codebase, hence we are free to change it and refactor things (e.g. bring more things into F# code) 
    // if we wish.
    type FSharpDeclarations(declarations: FSharpDeclaration[], reason : BackgroundRequestReason) = 
        
        inherit Declarations()  

        // Sort the declarations, NOTE: we used ORDINAL comparison here, this is "by design" from F# 2.0, partly because it puts lowercase last.
        let declarations = declarations |> Array.sortWith (fun d1 d2 -> compare d1.Title d2.Title)
        let mutable lastBestMatch = ""
        let isEmpty = (declarations.Length = 0)

        let tab = Dictionary<string,FSharpDeclaration[]>()

        // Given a prefix, narrow the items to the include the ones containing that prefix, and store in a lookaside table
        // attached to this declaration set.
        let trimmedDeclarations filterText : FSharpDeclaration[] = 
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
                                | false, _ -> yield declarations |> Array.filter (fun s -> matcher.MatchSingleWordPattern(s.Title, filterTextPrefix)<>null) 
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
                decls.[index].Title
            else ""

        override decl.IsEmpty() = isEmpty

        override decl.GetName(filterText, index) =
            let decls = trimmedDeclarations filterText
            if (index >= 0 && index < decls.Length) then
                let item = decls.[index]
                if (item.Kind = 205) then
                    decls.[index].Shortcut
                else 
                    item.Title
            else String.Empty

        override decl.GetDescription(filterText, index) =
            let decls = trimmedDeclarations filterText
            if (index >= 0 && index < decls.Length) then
                decls.[index].Description
            else ""

        override decl.GetGlyph(filterText, index) =
            let decls = trimmedDeclarations filterText
            //The following constants are the index of the various glyphs in the ressources of Microsoft.VisualStudio.Package.LanguageService.dll
            if (index >= 0 && index < decls.Length) then
                let item = decls.[index]
                item.Kind
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
                |> Array.tryFindIndex (fun d -> compareStrings(d.Title, text, length, ignoreCase) = 0)
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
                        lastBestMatch <- decls.[index].Title
                        let select = len = textSoFar.Length
                        if (index <> decls.Length- 1) && (compareStrings(decls.[index+1].Title , textSoFar, len, ignoreCase) = 0) 
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
                    AbstractPatternMatcher.Singleton.MatchSingleWordPattern(decls.[0].Title, textSoFar)<>null
                )
            shouldSelectItem <- preselect

        // This method is called after the string has been committed to the source buffer.
        //
        // Note: this override is a bit out of place as nothing in this type has anything to do with text buffers.
        override decl.OnAutoComplete(_textView, _committedText, _commitCharacter, _index) =
            // Would need special handling code for snippets.
            '\000'


    // ----------------------------------------------------------------------------------
    // Provides functionality that is available based  on the untyped AST

    // This type ia a little complex. 
    //    -- Each time we get a new UntypedParseInfo, we create a new UntypedFSharpScope that "folds in" the
    //       new region information while keeping the same unique identifiers for the regions in the text
    //
    //    -- The navigation items in the object are computed lazily 
    
    type UntypedFSharpScope(untypedParse:UntypedParseInfo, prevRegions, regionGenerator) =
        
        // Do we need to update the list?
        let mutable navigationItems : NavigationItems option = None
        let mutable displayedRegions = prevRegions

        // Utilities
        let copyTo (target:ArrayList) arr selector =
            target.Clear()
            for m in arr do 
                let (m:DeclarationItem) = selector m
                let (sc, sl), (ec, el) = m.Range 
                let memb = new DropDownMember(m.Name, new TextSpan(iStartLine=sl - 1,iStartIndex=sc,iEndLine=el - 1,iEndIndex=ec), 
                                              m.Glyph, DROPDOWNFONTATTR.FONTATTR_PLAIN)
                target.Add(memb) |> ignore

        let findDeclaration (declarations:'a[]) allowEqualEndLine (selector:'a -> DeclarationItem) line _col = 
            let _, sel, _ = 
              declarations
              |> Array.fold (fun (n, idx, size) decl -> 
                  // TODO this looks like an algorithm that was ad-hoc'd to deal with bad ranges from the interactiveChecker, maybe can be simplified now
                  let (_, sl), (_, el) = (selector decl).Range
                  if ((line >= sl) && (line < el || (allowEqualEndLine && el = line))) && (el - sl) < size then
                    (n+1, n, el - sl) 
                  else 
                    (n+1, idx, size)                    
                ) (0, -1, Int32.MaxValue)
                
            if sel<> -1 then sel else
                let mutable lastBefore = -1
                let mutable lastLine = -1
                for i in 0 .. declarations.Length - 1 do
                  let decl = declarations.[i]
                  let (_, _sl), (_, el) = (selector decl).Range
                  if el < line && el > lastLine then 
                    lastBefore <- i
                    lastLine <- el
                if (lastBefore = -1 && declarations.Length > 0) 
                then 0 else lastBefore 
        
        let ensureNavigationItemsUpToDate() =
            if navigationItems.IsNone then
                navigationItems <- Some(untypedParse.GetNavigationItems())
        
        member this.FileName  = untypedParse.FileName
        member this.Regions = displayedRegions
        member this.RegionGenerator = regionGenerator
        
        static member WithNewParseInfo(untypedParse:UntypedParseInfo, prev:UntypedFSharpScope option) =
            match prev with
            | Some(prev) -> 
                let regs = 
                    if (prev.FileName = untypedParse.FileName) then 
                        prev.Regions 
                    else 
                        Map.empty 
                new UntypedFSharpScope(untypedParse, regs, prev.RegionGenerator)
            | None -> 
                let generator = 
                  let count = ref 0u
                  (fun () -> count := !count + 1u; !count) // unchecked? overflow?
                new UntypedFSharpScope(untypedParse, Map.empty, generator)
                
        // Synchronize...
        member this.SynchronizeNavigationDropDown(file, line, col:int, dropDownTypes:ArrayList, dropDownMembers:ArrayList, selectedType:int byref, selectedMember:int byref) =    
#if DEBUG            
            use t = Trace.Call("LanguageService", "SynchronizeNavigationDropDown", fun _->sprintf " line=%d col=%d" line col)
#endif
            
            try
                let current = untypedParse.FileName
                
                if file <> current then
                    dropDownTypes.Clear()
                    dropDownTypes.Add(new DropDownMember("(Parsing project files)", new TextSpan(), -1, DROPDOWNFONTATTR.FONTATTR_GRAY)) |> ignore
                    dropDownMembers.Clear()
                    selectedType <- 0
                    selectedMember <- -1
                    true
                else
                    ensureNavigationItemsUpToDate () 
                    
                    // Test whether things have changed so that we don't update the dropdown every time
                    copyTo dropDownTypes navigationItems.Value.Declarations (fun decl -> decl.Declaration)    
                    let line = line + 1
                    let selLeft = findDeclaration navigationItems.Value.Declarations true (fun decl -> decl.Declaration) line col
                    selectedType <- selLeft
                    match selLeft with 
                    | n when n >= 0 -> 
                        copyTo dropDownMembers (navigationItems.Value.Declarations.[n].Nested) id
                        selectedMember <- findDeclaration navigationItems.Value.Declarations.[n].Nested true id line col
                    | _ -> 
                        selectedMember <- -1
                    true
            with e-> 
                Assert.Exception(e)
                reraise()        


        member x.ValidateBreakpointLocation(line,col) = 
            untypedParse.ValidateBreakpointLocation(line,col)
                
        member x.GetHiddenRegions(file) =
            ensureNavigationItemsUpToDate()
            let current = untypedParse.FileName
            match navigationItems with 
            | Some(res) when file = current ->
                res.Declarations 
                  |> Array.filter(fun decl -> not(decl.Declaration.IsSingleTopLevel))
                  |> Array.fold (fun (toCreate, toUpdate:Map<_,_>) decl ->
                    let declKey = decl.Declaration.UniqueName
                    let (sc, sl), (ec, el) = decl.Declaration.BodyRange                    
                    let context = new TextSpan(iEndIndex = ec, iEndLine = el-1, iStartIndex = sc, iStartLine = sl-1)
                    match (Map.tryFind declKey displayedRegions) with
                    | Some(uniqueId) ->
                        // do not add if the region hasn't changed
                        (toCreate, toUpdate.Add(uniqueId, context))
                    | None ->
                        let id = regionGenerator()
                        let reg = 
                          new NewHiddenRegion
                            (iType = int HIDDEN_REGION_TYPE.hrtCollapsible, dwBehavior = uint32 HIDDEN_REGION_BEHAVIOR.hrbClientControlled,
                             dwState = uint32 HIDDEN_REGION_STATE.hrsExpanded, tsHiddenText = context, pszBanner = null, dwClient = id)
                        displayedRegions <- displayedRegions.Add(declKey, id)
                        (reg::toCreate, toUpdate)
                        ) ([], Map.empty)
            | _ -> 
                displayedRegions <- Map.empty
                [], Map.empty
        
        member x.ClearDisplayedRegions() =
            displayedRegions <- Map.empty
            
                   
    /// The scope object is the result of computing a particular typecheck. It may be queried for things like
    /// data tip text, member completion and so forth.
    type FSharpScope(/// The recent result of parsing
                     untypedResults: UntypedParseInfo,
                     /// Line/column/snapshot of BackgroundRequest that initiated creation of this scope
                     brLine:int, brCol:int, brSnapshot:ITextSnapshot,
                     /// The possibly staler result of typechecking
                     typedResults: TypeCheckResults,
                     /// The project
                     projectSite: IProjectSite,
                     /// The text view
                     view: IVsTextView,
                     /// The colorizer for this view (though why do we need to be lazy about creating this?)
                     colorizer: Lazy<FSharpColorizer>,
                     /// A service that will provide Xml Content
                     documentationProvider : IdealDocumentationProvider
                     ) = 
        inherit AuthoringScope() 

        // go ahead and compute this now, on this background thread, so will have info ready when UI thread asks
        let noteworthyParamInfoLocations = untypedResults.FindNoteworthyParamInfoLocations(brLine, brCol)

        let lastRequestedMethodListForMethodTip : MethodListForAMethodTip option ref = ref None

        member scope.LastRequestedMethodListForMethodTipUsingFallback() = 
            lastRequestedMethodListForMethodTip := None

        member scope.InitLastRequestedMethodListForMethodTipUsingFallback() = 
            lastRequestedMethodListForMethodTip := Some (scope.DoGetMethodListForAMethodTip(true))

        static member HasTextChangedSinceLastTypecheck (curTextSnapshot: ITextSnapshot, oldTextSnapshot: ITextSnapshot, ((sl:int,sc:int),(el:int,ec:int))) = 
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

        member __.GetExtraColorizations() =  typedResults.GetExtraColorizations()

        override scope.GetDataTipText(line, col) =
            // in cases like 'A<int>' when cursor in on '<' there is an ambiguity that cannot be resolved based only on lexer information
            // '<' can be treated both as operator and as part of identifier
            // in this case we'll do 2 passes:
            // 1. treatTokenAsIdentifier=false - we'll pick raw token under the cursor and try find it among resolved names, is attempt was successful - great we are done, otherwise
            // 2. treatTokenAsIdentifier=true - even if raw token was recognized as operator we'll use different branch 
            // that calls QuickParse.GetCompleteIdentifierIsland and then tries previous column...
            let rec getDataTip(alwaysTreatTokenAsIdentifier) =
                let token = colorizer.Value.GetTokenInfoAt(VsTextLines.TextColorState (VsTextView.Buffer view),line,col)
#if DEBUG
                use t = Trace.Call("LanguageService",
                                   "GetDataTipText",
                                   fun _->sprintf " line=%d col=%d tokeninfo=%A" line col token.Token)
#endif

                try
                    let lineText = VsTextLines.LineText (VsTextView.Buffer view) line
                    
                    // If we're not on the first column; we don't find any identifier, we also look at the previous one
                    // This allows us to do Ctrl+K, I in this case:  let f$ x = x  
                    // Note: this is triggered by hovering over the next thing after 'f' as well - even in 
                    //   case like "f(x)" when hovering over "(", but MPF doesn't show tooltip in that case
                    // Note: MPF also doesn't show the tooltip if we're past the end of the line (Ctrl+K, I after 
                    //  the last character on the line), so tooltip isn't shown in that case (suggestion 4371)
                    
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

#if DEBUG
                    let isDiagnostic = Keyboard.IsKeyPressed Keyboard.Keys.Shift
#else
                    let isDiagnostic = false
#endif 
                    let diagnosticTipSpan = TextSpan(iStartLine=line, iEndLine=line, iStartIndex=col, iEndIndex=col+1)
                    match possibleIdentifier with 
                    | None -> (if isDiagnostic then "No identifier found at this position." else ""),diagnosticTipSpan
                    | Some (s,colAtEndOfNames, isQuotedIdentifier) -> 

                        // REVIEW: Need to capture and display XML
                        let diagnosticText lead = 
                            let errorText = String.Concat(typedResults.Errors |> Seq.truncate 5 |> Seq.map(fun pi->sprintf "%s\n" pi.Message)|>Seq.toArray)
                            let errorText = match errorText.Length with 0->"" | _->"Errors:\n"+errorText
                            let dataTipText = sprintf "%s\nIsland(col=%d,token=%d):\n%A\n%s%s" lead col tokenTag possibleIdentifier (projectSite.DescriptionOfProject()) errorText
                            dataTipText

                        if typedResults.HasFullTypeCheckInfo then 
                            let qualId  = PrettyNaming.GetLongNameFromString s
#if DEBUG                            
                            Trace.PrintLine("LanguageService", (fun () -> sprintf "Got qualId = %A" qualId))
#endif
                            let parserState = 
#if DEBUG
                                match qualId with
                                | [x] ->
                                    match Int32.TryParse x with
                                    | true, v -> 
                                        match ParserState.Get v with
                                        | Some (lines, lastWriteTime, path) ->
                                            Some [ yield sprintf "Listing file: %s" path
                                                   yield sprintf "Last write time: %A" lastWriteTime
                                                   yield! lines ]
                                        | None -> 
                                            Some [ "Grammar debugging requires FSYacc listing file"
                                                   "FSYacc puts listing in the same folder with output file but with extension 'fsyacc.output'."
                                                   "If output file is not set - then listing will be placed in the folder with input file and will use the name of input file (with extension fsyacc.output)"
                                                   "run 'fsyacc -v <input-file>"
                                                   sprintf "Create %s env variable with value - path to the listing file" ParserState.FsyListingVariableName ]
                                    | _ -> None
                                | _ -> None
#else
                                None
#endif
                                                
                            // Corrrect the identifier (e.g. to correctly handle active pattern names that end with "BAR" token)
                            let tokenTag = QuickParse.CorrectIdentifierToken s tokenTag
                            let dataTip = typedResults.GetDataTipText((line, colAtEndOfNames), lineText, qualId, tokenTag)
#if DEBUG
                            Trace.PrintLine("LanguageService", fun () -> sprintf "Got datatip=%A" dataTip)
#endif
                            match dataTip with
                            | DataTipText [] when makeSecondAttempt -> getDataTip true
                            | _ -> 
                            if isDiagnostic then 
                                let text = sprintf "plid:%A\ndataTip:\n%A" qualId dataTip
                                let text = 
                                    match parserState with
                                    | None -> text
                                    | Some lines ->
                                        sprintf "%s\n%s\n" text (String.concat "\n" lines)
                                diagnosticText text, diagnosticTipSpan
                            else
                                let dataTipText =  XmlDocumentation.BuildDataTipText(documentationProvider, dataTip)

                                // The data tip is located w.r.t. the start of the last identifier
                                let sizeFixup = if isQuotedIdentifier then 4 else 0
                                let lastStringLength = (qualId |> List.rev |> List.head).Length  + sizeFixup
#if DEBUG
                                Trace.PrintLine("LanguageService", (fun () -> sprintf "Got dataTip = %A, colOfEndOfText = %d, lastStringLength = %d, line = %d" dataTipText colAtEndOfNames lastStringLength line))
#endif

                                // This is the span of text over which the data tip is active. If the mouse moves away from it then the
                                // data tip goes away
                                let dataTipSpan = TextSpan(iStartLine=line, iEndLine=line, iStartIndex=max 0 (colAtEndOfNames-lastStringLength), iEndIndex=colAtEndOfNames)
                                (dataTipText, dataTipSpan)                                
                        else
                            "Bug: TypeCheckInfo option was None", diagnosticTipSpan
                with e-> 
                    Assert.Exception(e)
                    reraise()

            getDataTip false
            

        static member IsReasonRequiringSyncParse(reason) =
            match reason with
            | BackgroundRequestReason.MethodTip // param info...
            | BackgroundRequestReason.MatchBracesAndMethodTip // param info...
            | BackgroundRequestReason.CompleteWord | BackgroundRequestReason.MemberSelect | BackgroundRequestReason.DisplayMemberList // and intellisense-completion...
                -> true // ...require a sync parse (so as to call FindNoteworthyParamInfoLocations and GetRangeOfExprLeftOfDot, respectively)
            | _ -> false

        /// Intellisense autocompletions
        [<CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId="col-1")>]
        override scope.GetDeclarations(textSnapshot, line, col, reason) =
            ignore () // to be able to place a breakpoint
            assert(FSharpScope.IsReasonRequiringSyncParse(reason))
            async {
                let tokenInfo = colorizer.Value.GetTokenInfoAt(VsTextLines.TextColorState (VsTextView.Buffer view),line,col)
                let prevCol = if (col <= 0) then 0 else col - 1  // Note: check <= 0 to show FxCop that there is no underflow risk
                let prevTokenInfo = colorizer.Value.GetTokenInfoAt(VsTextLines.TextColorState (VsTextView.Buffer view),line,prevCol)
                // denotes if we got token that matches exact specified position or it was just last token before EOF
                let exactMatch = col >= tokenInfo.StartIndex && col <= tokenInfo.EndIndex
#if DEBUG
                use _t = Trace.Call("LanguageService",
                                   "GetDeclarations",
                                   fun _->sprintf " line=%d col=%d reason=%A" line col reason)
#endif
                try
                    if exactMatch && 
                                (
                                    (tokenInfo.Color = TokenColor.Comment && prevTokenInfo.Color = TokenColor.Comment) || 
                                    (tokenInfo.Color = TokenColor.String  && prevTokenInfo.Color = TokenColor.String)
                                ) then
                        // We don't want to show info in comments & strings (in case of exact match)
                        // (but we want to show it if the thing before or after isn't comment/string)
                        dprintf "GetDeclarations: We won't show anything in comment or string.\n"
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
                            let plid = QuickParse.GetPartialLongNameEx(lineText, col-1) // Subtract one to convert to zero-relative
                            ignore plid // for breakpoint

                            let detectTextChange (oldTextSnapshotInfo: obj, range) = 
                                let oldTextSnapshot = oldTextSnapshotInfo :?> ITextSnapshot
                                FSharpScope.HasTextChangedSinceLastTypecheck (textSnapshot, oldTextSnapshot, range)
                            let! decls = typedResults.GetDeclarations(untypedParseInfoOpt,(line, col), lineText, plid, detectTextChange)
                            let declarations =  decls.Items |> Array.map (fun d -> FSharpDeclaration(documentationProvider, d))
                            return (new FSharpDeclarations(declarations, reason) :> Declarations) 
                    else
                        dprintf "GetDeclarations found no TypeCheckInfo in ParseResult.\n"
                        return null 
                with e-> 
                    Assert.Exception(e)
                    raise e
                    return null
            }

        /// Get methods for parameter info
        override scope.GetMethodListForAMethodTip(useNameResolutionFallback) =
            if useNameResolutionFallback then
                // lastRequestedMethodListForMethodTip should be initialized in the ExecuteBackgroundRequest
                // if not - just return null
                defaultArg lastRequestedMethodListForMethodTip.Value null
            else
                // false is passed only from unit tests
                scope.DoGetMethodListForAMethodTip(false)
        
        member scope.DoGetMethodListForAMethodTip(useNameResolutionFallback) =
#if DEBUG
            use t = Trace.Call("LanguageService",
                               "GetMethodListForAMethodTip",
                               fun _->sprintf " line=%d col=%d" brLine brCol)
#endif
            try
                // we need some typecheck info, even if stale, in order to look up e.g. method overload types/xmldocs
                if typedResults.HasFullTypeCheckInfo then 

                    // we need recent parse info to e.g. know how many commas and thus how many args there are
                    match noteworthyParamInfoLocations with
                    | Some nwpl -> 
                        // Note: this may alternatively workaround some parts of 90778 - the real fix for that is to have before-overload-resolution name-sink work correctly.
                        // However it also deals with stale typecheck info that may not have recorded name resolutions for a recently-typed long-id.
                        let names = 
                            if not useNameResolutionFallback then
                                None
                            else
                                Some nwpl.LongId
                        // "names" is a long-id that we can fallback-lookup in the local environment if captured name resolutions finds nothing at the location.
                        // This can happen e.g. if you are typing quickly and the typecheck results are stale enough that you don't have a captured resolution for
                        // the name you just typed, but fresh enough that you do have the right name-resolution-environment to look up the name.
                        let lidEndLine,lidEndCol = nwpl.LongIdEndLocation
                        let methods = typedResults.GetMethods((lidEndLine-1, lidEndCol-1), "", names)  // -1 because wants 0-based
                        
                        // If the name is an operator ending with ">" then it is a mistake 
                        // we can't tell whether "  >(" is a generic method call or an operator use 
                        // (it depends on the previous line), so we fitler it
                        //
                        // Note: this test isn't particularly elegant - encoded operator name would be something like "( ...> )"                        
                        if (methods.Methods.Length = 0 || methods.Name.EndsWith("> )")) then
                            null
                        else                    
                            // "methods" contains both real methods for this longId, as well as static-parameters in the case of type providers.
                            // They "conflict" for cases of TP(...) (calling a constructor, no static args provided) versus TP<...> (static args), since
                            // both point to the same longId.  However we can look at the character at the 'OpenParen' location and see if it is a '(' or a '<' and then
                            // filter the "methods" list accordingly.
                            let isThisAStaticArgumentsTip =
                                let parenLine, parenCol = nwpl.OpenParenLocation 
                                let textAtOpenParenLocation =
                                    if brSnapshot=null then
                                        // we are unit testing, use the view
                                        let _hr, buf = view.GetBuffer()
                                        let _hr, s = buf.GetLineText(parenLine-1, parenCol-1, parenLine-1, parenCol)  // -1 because F# reports 1-based line nums, whereas VS wants 0-based
                                        s
                                    else
                                        // we are in the product, use the ITextSnapshot
                                        brSnapshot.GetText(FSharpMethodListForAMethodTip.MakeSpan(brSnapshot, parenLine, parenCol-1, parenLine, parenCol))
                                if textAtOpenParenLocation = "<" then
                                    true
                                else
                                    false  // note: textAtOpenParenLocation is not necessarily otherwise "(", for example in "sin 42.0" it is "4"
                            let filteredMethods =
                                [| for m in methods.Methods do 
                                        if m.IsStaticArguments = isThisAStaticArgumentsTip then   // need to distinguish TP<...>(...)  angle brackets tip from parens tip
                                            yield m |]
                            if filteredMethods.Length <> 0 then
                                FSharpMethodListForAMethodTip(documentationProvider, methods.Name, filteredMethods, nwpl, brSnapshot, isThisAStaticArgumentsTip) :> MethodListForAMethodTip
                            else
                                null
                    | _ -> 
                        null
                else
                    dprintf "GetMethodListForAMethodTip found no TypeCheckInfo in ParseResult.\n"
                    null
            with e-> 
                Assert.Exception(e)
                reraise()

        override this.GetF1KeywordString(span : TextSpan, context : IVsUserContext) : unit =
            let shouldTryToFindIdentToTheLeft (token : TokenInformation) =
                match token.CharClass with
                |   TokenCharKind.WhiteSpace -> true
                |   TokenCharKind.Delimiter -> true
                |   _ -> false
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
                    | (Some token) as original when col > 0 && shouldTryToFindIdentToTheLeft token ->
                        // try to step back one char
                        match colorizer.Value.GetTokenInformationAt(textColorState,line,col-1) with
                        | (Some token) as newInfo when token.CharClass <> TokenCharKind.WhiteSpace -> newInfo, col - 1
                        |   _ -> original, col
                    | otherwise -> otherwise, col
                match tokenInformation with
                |   None -> None
                |   Some token ->
                        match token.CharClass, token.ColorClass with
                        |   TokenCharKind.Keyword, _
                        |   TokenCharKind.Operator, _ 
                        |   _, TokenColorKind.PreprocessorKeyword ->
                                lineText.Substring(token.LeftColumn, token.RightColumn - token.LeftColumn + 1) + "_FS" |> Some
                                
                        |   (TokenCharKind.Comment|TokenCharKind.LineComment), _ -> Some "comment_FS"
                                
                        |   TokenCharKind.Identifier, _ ->            
                                try
                                    let lineText = VsTextLines.LineText (VsTextView.Buffer view) line
                                    let possibleIdentifier = QuickParse.GetCompleteIdentifierIsland false lineText col
                                    match possibleIdentifier with
                                    |   None -> None // no help keyword
                                    |   Some(s,colAtEndOfNames, _) ->
                                            if typedResults.HasFullTypeCheckInfo then 
                                                let qualId = PrettyNaming.GetLongNameFromString s
                                                match typedResults.GetF1Keyword((line,colAtEndOfNames), lineText, qualId) with
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


                
open Implementation

/// This class defines capabilities of the language service. 
/// CodeSense = true\false, for example
type FSharpLanguagePreferences(site, langSvc, name) = 
    inherit LanguagePreferences(site, langSvc, name) 

type ExecuteBackgroundRequestData =
    {
        projectSite : IProjectSite
        checkOptions : CheckOptions
        projectFileName : string
        interactiveChecker : InteractiveChecker
        colorizer : Lazy<FSharpColorizer>
    }
type FSharpBackgroundRequest(line, col, info, sourceText, snapshot : ITextSnapshot, 
                             methodTipMiscellany : MethodTipMiscellany, fileName, reason, view, sink, 
                             source:ISource, timestamp:int, synchronous:bool,
                             executeBackgroundRequestData : Lazy<ExecuteBackgroundRequestData> option) = 
    inherit BackgroundRequest(line, col, info, sourceText, snapshot, methodTipMiscellany, fileName, reason, view, sink, source, timestamp, synchronous)

    member this.ExecuteBackgroundRequestData = executeBackgroundRequestData
    member this.TryGetColorizer() = match executeBackgroundRequestData with None -> None | Some data -> Some (data.Force().colorizer.Force())

// Container class that delays loading of FSharp.Compiler.dll compiler types until they're actually needed
type internal InteractiveCheckerContainer(interactiveChecker) =
    member this.InteractiveChecker = interactiveChecker

/// LanguageService state. 
//
// Note: It appears that a load of this type (+ a construction of an instance) should not load FSharp.Compiler.dll. This is subtle
// because it depends on deferred loading characteristics of the CLR. The type InteractiveCheckerContainer is an (otherwise 
// unnecessary) indirection holding the types referenced in FSharp.Compiler.dll. This is sufficient to delay loading.
type internal LanguageServiceState() =
    static let colorizerGuid = new Guid("{A2976312-7D71-4BB4-A5F8-66A08EBF46C8}") // Guid for colorizwed user data on IVsTextBuffer
    let mutable serviceProvider:ServiceProvider option = None
    let mutable interactiveCheckerContainerOpt:InteractiveCheckerContainer option = None
    let mutable artifacts:Artifacts option = None
    let mutable preferences:LanguagePreferences option = None
    let mutable documentationProvider:IdealDocumentationProvider option = None
    let mutable sourceFactory : (IVsTextLines -> IdealSource) option = None
    let mutable dirtyForTypeCheckFiles : Set<string> = Set.empty
    let mutable isInitialized = false
    let mutable unhooked = false
    let mutable untypedParseScope : UntypedFSharpScope option = None
    let mutable enableStandaloneFileIntellisense = true
    let outOfDateProjectFileNames = new System.Collections.Generic.HashSet<string>()
    
    member this.InteractiveChecker = 
        if not this.IsInitialized then raise (Error.UseOfUninitializedLanguageServiceState)
        
        match interactiveCheckerContainerOpt with 
        | Some interactiveCheckerContainer -> interactiveCheckerContainer.InteractiveChecker 
        | None ->
            let notifyFileTypeCheckStateIsDirty = NotifyFileTypeCheckStateIsDirty (fun filename -> UIThread.Run(fun () -> this.NotifyFileTypeCheckStateIsDirty(filename)))
            let interactiveChecker = InteractiveChecker.Create(notifyFileTypeCheckStateIsDirty)
            let pc = InteractiveCheckerContainer(interactiveChecker)
            interactiveCheckerContainerOpt <- Some pc
            interactiveChecker

    member this.Artifacts = 
        if not this.IsInitialized then raise (Error.UseOfUninitializedLanguageServiceState)
        match artifacts with 
        | Some artifacts -> artifacts
        | None-> 
            let a = new Artifacts()
            artifacts <- Some a
            a
        
    member this.Preferences = 
        if this.Unhooked then raise (Error.UseOfUnhookedLanguageServiceState)
        if not this.IsInitialized then raise (Error.UseOfUninitializedLanguageServiceState)
        preferences.Value
        
    member this.ServiceProvider = 
        if not this.IsInitialized then raise (Error.UseOfUninitializedLanguageServiceState)
        match serviceProvider with 
        | Some serviceProvider -> serviceProvider
        | None->failwith "ServiceProvider not available"   
        
    member this.SourceFactory = 
        if not this.IsInitialized then raise (Error.UseOfUninitializedLanguageServiceState)
        match sourceFactory with 
        | Some sourceFactory -> sourceFactory
        | None->failwith "SourceFactory not available"           
        
    member this.UntypedParseScope with get() = untypedParseScope and set v = untypedParseScope <- v
    member this.IsInitialized = isInitialized
    member this.Unhooked = unhooked
    member this.DocumentationProvider = documentationProvider.Value
    
    /// Construct a new LanguageService state
    static member Create() = LanguageServiceState()
    
    /// Handle late intialization pieces
    member this.Initialize 
                (sp:ServiceProvider,
                 dp:IdealDocumentationProvider, 
                 prefs:LanguagePreferences,
                 enableStandaloneFileIntellisenseFlag:bool,
                 createSource:IVsTextLines -> IdealSource) = 
#if DEBUG                
        use t = Trace.CallByThreadNamed("LanguageService",
                                        "LanguageServiceState::Initialize",
                                        "UI", // Name this thread.
                                        fun _->"")
#endif                                        
        if this.Unhooked then raise (Error.UseOfUnhookedLanguageServiceState)        
        
        isInitialized<-true
        unhooked<-false
        
        serviceProvider<-Some sp
        documentationProvider<-Some dp
        preferences<-Some prefs
        enableStandaloneFileIntellisense <- enableStandaloneFileIntellisenseFlag

        sourceFactory<-Some createSource

    
    // This method is executed on the UI thread
    member this.CreateBackgroundRequest(line : int, col : int, info : TokenInfo, sourceText : string, snapshot : ITextSnapshot, methodTipMiscellany : MethodTipMiscellany, 
                                         fileName : string, reason : BackgroundRequestReason, view : IVsTextView,
                                         sink : AuthoringSink, source : ISource, timestamp : int, synchronous : bool) : BackgroundRequest =
        let backgroundRequestData =
            match sourceText with
            |   null -> 
                    // sourceText being null indicates that the cached results for this request will be used, so 
                    // ExecuteBackgroundRequest will not be called.                    
                    None 
            |   _ ->       
                    let colorizer =
                        lazy                                
                            let tl = Com.ThrowOnFailure1(view.GetBuffer())
                            this.GetColorizer(tl)                         
                    if SourceFile.MustBeSingleFileProject(fileName) then
                        let data = 
                            lazy // For scripts, GetCheckOptionsFromScriptRoot involves parsing and sync op, so do it on language service thread later
                                // This portion is executed on the language service thread
                                let timestamp = if source=null then System.DateTime(2000,1,1) else source.OpenedTime // source is null in unit tests
                                let checkOptions = this.InteractiveChecker.GetCheckOptionsFromScriptRoot(fileName, sourceText, timestamp) // REVIEW: Could pass in version for caching. SHOULD ALLOW CACHING when not Full Parse
                                let projectSite = ProjectSiteOptions.ToProjectSite(fileName, checkOptions)
                                { projectSite = projectSite
                                  checkOptions = checkOptions 
                                  projectFileName = projectSite.ProjectFileName()
                                  interactiveChecker = this.InteractiveChecker
                                  colorizer = colorizer } 
                        Some data
                    else 
                        // This portion is executed on the UI thread.
                        // For projects, we need to access RDT on UI thread, so do it now
                        let projectSite = this.Artifacts.FindOwningProject(this.ServiceProvider.Rdt,fileName,enableStandaloneFileIntellisense)
                        let checkOptions = ProjectSiteOptions.Create(projectSite, fileName)                            
                        let projectFileName = projectSite.ProjectFileName()
                        let data = 
                            {   projectSite = projectSite
                                checkOptions = checkOptions 
                                projectFileName = projectFileName 
                                interactiveChecker = this.InteractiveChecker
                                colorizer = colorizer
                            } 
                        Some (Lazy<_>.CreateFromValue data)

        let br =
            new FSharpBackgroundRequest(line, col, info, sourceText, snapshot, methodTipMiscellany, fileName, reason, view, sink, source, timestamp, synchronous, 
                                        backgroundRequestData)
        br :> BackgroundRequest 

    member this.NotifyFileTypeCheckStateIsDirty(filename) = 
#if DEBUG
        Trace.PrintLine("ChangeEvents", fun _ -> sprintf "HandleBackgroundBeforeTypeCheckFile(%s)" filename)
#endif
        dirtyForTypeCheckFiles <- dirtyForTypeCheckFiles.Add filename

    /// Clear all language service caches and finalize all transient references to compiler objects
    member this.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients() = 
        if this.Unhooked then raise (Error.UseOfUnhookedLanguageServiceState)
        if this.IsInitialized then
            this.InteractiveChecker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()

    /// Unhook the object. These are the held resources that need to be disposed.
    member this.Unhook() =
        if this.Unhooked then raise (Error.UseOfUnhookedLanguageServiceState)
        if (this.IsInitialized) then
            // Dispose the preferences.
            if this.Preferences<>null then this.Preferences.Dispose()
            // Stop the background compile.
            // here we refer to interactiveCheckerContainerOpt directly to avoid triggering its creation
            match interactiveCheckerContainerOpt with
            | Some container -> 
                let checker = container.InteractiveChecker
                checker.StopBackgroundCompile()
                checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()
            | None -> ()
            
            serviceProvider <- None
            interactiveCheckerContainerOpt <- None  // TODO: DISPOSAL: Dispose here
            artifacts<-None
            preferences<-None
            documentationProvider <- None
            unhooked<-true
    
    /// Respond to project settings changes
    member this.OnProjectSettingsChanged(site:IProjectSite) = 
        // The project may have changed its references.  These would be represented as 'dependency files' of each source file.  Each source file will eventually start listening
        // for changes to those dependencies, at which point we'll get OnFilesChanged notifications.  Until then, though, we just 'make a note' that this project is out of date.
        outOfDateProjectFileNames.Add(site.ProjectFileName()) |> ignore
        for filename in site.SourceFilesOnDisk() do
            match this.Artifacts.GetSourceOfFilename(this.ServiceProvider.Rdt,filename) with
            | Some source -> 
                source.RecolorizeWholeFile()
                source.RecordChangeToView()
            | None -> ()

    /// Respond to project being cleaned/rebuilt (any live type providers in the project should be refreshed)
    member this.OnProjectCleaned(projectSite:IProjectSite) = 
        let checkOptions = ProjectSiteOptions.Create(projectSite, "")
        this.InteractiveChecker.NotifyProjectCleaned(checkOptions)

    /// Invalidate the configuration if we notice any changes for any files that were listed
    /// in the DependencyFiles set for the last untypedParse for a project
    member this.OnFilesChanged(projectSite:IProjectSite) = 
#if DEBUG
        Trace.PrintLine("ChangeEvents", fun _ -> sprintf "OnFilesChanged")
#endif
        let checkOptions = ProjectSiteOptions.Create(projectSite, "")
        this.InteractiveChecker.InvalidateConfiguration(checkOptions)
    
    /// Unittestable complement to LanguageServce.CreateSource
    member this.CreateSource(buffer:IVsTextLines) : IdealSource =
    
        // Each time a source is created, also verify that the IProjectSite has been initialized to listen to changes to the project.
        // We can't listen to OnProjectLoaded because the language service is not guaranteed to be loaded when this is called.
        let filename = VsTextLines.GetFilename buffer
        let result = VsRunningDocumentTable.FindDocumentWithoutLocking(this.ServiceProvider.Rdt,filename)
        match result with 
        | Some(hier,_) -> 
            match hier with 
            | :? IProvideProjectSite as siteProvider ->
                let site = siteProvider.GetProjectSite()
                site.AdviseProjectSiteChanges(KnownAdviseProjectSiteChangesCallbackOwners.LanguageService, 
                                              new AdviseProjectSiteChanges(fun () -> this.OnProjectSettingsChanged(site))) 
                site.AdviseProjectSiteCleaned(KnownAdviseProjectSiteChangesCallbackOwners.LanguageService, 
                                              new AdviseProjectSiteChanges(fun () -> this.OnProjectCleaned(site))) 
            | _ -> 
                // This can happen when the file is in a solution folder or in, say, a C# project.
                ()
        | _ ->
            // This can happen when renaming a file from a different language service into .fs or fsx.
            // This naturally won't have an associated project.
            ()
        
        // Create the source and register file change callbacks there.       
        let source = this.SourceFactory(buffer)
        source.SetDependencyFileChangeCallback(this.OnFilesChanged, this.NotifyFileTypeCheckStateIsDirty)
        this.Artifacts.SetSource(buffer, source)
        source
    

    member this.ExecuteBackgroundRequest(req:BackgroundRequest, source:IdealSource)= 
        if not req.Terminate then
            let freq = req :?> FSharpBackgroundRequest
            this.ExecuteFSharpBackgroundRequest(freq, source)

    /// Handle an incoming request to parse the source code. 
    /// BackgroundRequest.Text -- the full text of the source file.   
    ///
    /// isFreshFullTypeCheck should be set to true if the returned AuthoringScope represents
    /// the results of a fresh typecheck of the file.
    member this.ExecuteFSharpBackgroundRequest(req:FSharpBackgroundRequest, source:IdealSource)= 
#if DEBUG
        use t = Trace.CallByThreadNamed("LanguageService",
                                        "LanguageServiceState.ExecuteBackgroundRequest", 
                                        "MPF Worker", 
                                        fun _->sprintf " location=(%d:%d), reason=%A, filename=%s" req.Line req.Col req.Reason req.FileName)
#endif
        try
#if DEBUG
            Check.ArgumentNotNull req "req"
#endif        
            let data =
                    match req.ExecuteBackgroundRequestData with
                    |   Some lazyData -> lazyData.Force()
                    |   None -> failwith "ExecuteFSharpBackgroundRequest called for supposedly cached request"
            let projectSite = data.projectSite
            let checkOptions = data.checkOptions
            let projectFileName = data.projectFileName
            let interactiveChecker = data.interactiveChecker
            let colorizer = data.colorizer 
            source.ProjectSite <- Some projectSite
            
            // Do brace matching if required
            if req.ResultSink.BraceMatching then  
                // Record brace-matching
                let braceMatches = interactiveChecker.MatchBraces(req.FileName,req.Text,checkOptions)
                    
                let mutable pri = 0
                for (((l1,c1),(l2,c2)),((l1e,c1e),(l2e,c2e))) in braceMatches do
                    let span = TextSpan(iStartLine=l1,iStartIndex=c1,iEndLine=l2,iEndIndex=c2)
                    let endContext = TextSpan(iStartLine=l1e,iStartIndex=c1e,iEndLine=l2e,iEndIndex=c2e)
                    req.ResultSink.MatchPair(span, endContext, pri)
                    pri<-pri+1
                          
            match req.Reason with 
            | BackgroundRequestReason.MatchBraces -> 
                ()
                // work has already been done above

            | BackgroundRequestReason.UntypedParse ->

                // invoke UntypedParse directly - relying on cache inside the interactiveChecker
                let untypedParse = interactiveChecker.UntypedParse(req.FileName, req.Text, checkOptions)
#if DEBUG
                Trace.PrintLine("LanguageService", (fun () -> sprintf " dependencies for UntypedParse: %A" (untypedParse.DependencyFiles())))
#endif
                untypedParseScope <- Some(UntypedFSharpScope.WithNewParseInfo(untypedParse, untypedParseScope))                  
                ()

            | _ -> 
                let sync_UntypedParseOpt = 
                    if FSharpScope.IsReasonRequiringSyncParse(req.Reason) then
                        Some(interactiveChecker.UntypedParse(req.FileName,req.Text,checkOptions))
                    else None

                // Try to grab recent results, unless BackgroundRequestReason = Check
                // This may fail if the CompilerServices API decides that
                // it would like a chance to really check the contents of the file again,
                let untypedParse,typedResults,containsFreshFullTypeCheck,aborted,resultTimestamp = 
                    let possibleShortcutResults = 
                        if (req.Reason = BackgroundRequestReason.FullTypeCheck) then 
                           None
                        else 
                            if req.RequireFreshResults = RequireFreshResults.Yes then
                                // Getting here means we're in second chance intellisense. For example, the user has pressed dot 
                                // we tried stale results and got nothing. Now we need real results even if we have to wait.
                                None
                            else                            
                                // This line represents a critical decision in the LS. If we're _not_
                                // doing a full typecheck, and some stale typecheck results are available, then
                                // use the stale results. This means, for example, that completion is fast,
                                // but less accurate (since we can't possibly afford to typecheck while generating a completion)
                                interactiveChecker.TryGetRecentTypeCheckResultsForFile((req.FileName,checkOptions))
                    
                    match possibleShortcutResults with 
                    | Some (untypedParse,typedResults,fileversion) -> 
                        defaultArg sync_UntypedParseOpt untypedParse,Some typedResults, false, false, fileversion // Note: untypedparse and typed results have different timestamps/snapshots, typed may be staler
                    | None -> 
                        // Perform a fresh two-phase parse of the source file
                        let untypedParse = 
                            match sync_UntypedParseOpt with 
                            | Some x -> x
                            | None -> interactiveChecker.UntypedParse(req.FileName,req.Text,checkOptions)
#if DEBUG
                        Trace.PrintLine("LanguageService", (fun () -> sprintf " dependencies for other: %A" (untypedParse.DependencyFiles())))
#endif
                        
                        // Should never matter but don't let anything in FSharp.Compiler extend the lifetime of 'source'
                        let sr = ref (Some source)
                        let isResultObsolete() = 
                            match !sr with
                            | None -> false
                            | Some(source) -> req.Timestamp <> source.ChangeCount
                        
                        // Type-checking
                        let typedResults,aborted = 
                            match interactiveChecker.TypeCheckSource(untypedParse,req.FileName,req.Timestamp,req.Text,checkOptions,IsResultObsolete(isResultObsolete),req.Snapshot) with 
                            | NoAntecedant -> None,false
                            | Aborted -> 
                                // isResultObsolete returned true during the type check.
                                None,true
                            | TypeCheckSucceeded results -> Some results, false

                        sr := None
                        untypedParse,typedResults,true,aborted,req.Timestamp
                
                // Now that we have the untypedParse, we can SetDependencyFiles().
                // 
                // We may have missed a file change event for a dependent file (e.g. after the user added a project reference, he might immediately build that project, but 
                // we haven't yet started listening for changes to that file on disk - the call to SetDependencyFiles() below starts listening).  This can only happen if 
                // the set of dependencies has changed (otherwise we were _already_ listening and would not have missed the event)...
                let anyDependenciesChanged = source.SetDependencyFiles(Microsoft.FSharp.Compiler.ExtensionTyping.ApprovalIO.ApprovalsAbsoluteFileName :: untypedParse.DependencyFiles())
                // .. so if dependencies have changed, and we may have missed an event, let's behave as though this typecheck is failing and the file still needs to be re-type-checked
                if anyDependenciesChanged then
                    req.ResultClearsDirtinessOfFile <- false
                    // Furthermore, if we know our project is out-of-date, this may mean that dependent DLLs may have changed on disk without us knowing, 
                    // since we only just started listening for those changes a moment ago in the SetDependencyFiles() call.  So behave just as if we were 
                    // just notified that those dependency files changed.  (In the future, it would be good to partition a source file's dependencies into
                    // 'project' dependencies (that only depend on the IProjectSite, e.g. project/asseembly references) and 'source' dependencies (e.g. #r's).)
                    if outOfDateProjectFileNames.Contains(projectFileName) then
                        interactiveChecker.InvalidateConfiguration(checkOptions)
                        interactiveChecker.StartBackgroundCompile(checkOptions) 
                        outOfDateProjectFileNames.Remove(projectFileName) |> ignore

                // (Note also that source's dependency files are not 'per configuration', which you might think means we'll miss updates when
                //  - create solution with 2 projects, app and lib, and p2p reference between them
                //  - start in Debug mode (incremental builder will look at project and note the missing project reference)
                //  - switch Debug -> Release
                //  - go outside VS and build the lib on disk with msbuild (Debug version)
                //     - indeed, we are not curently watching for bin\Debug\Lib.dll on disk, we're only watching bin\Release\Lib.dll
                //  - switch back to Debug in VS
                // That is, do we now see errors about the DLL not existing?  No, because here:
                //        let buildCache = MruCache(Flags.buildCacheSize, CreateIncrementalBuilder, areSame = CheckOptions.AreSameProject, areSameForSubsumption = CheckOptions.AreSameProjectName)
                // the 'areSameForSubsumption' means effectively that switching to 'Release' kicks out the info for 'Debug', which means when switching back to Debug, 
                // we recompute build cache based on state of world now.)

                else
                    untypedParseScope <- Some(UntypedFSharpScope.WithNewParseInfo(untypedParse, untypedParseScope))                  
                    
                    match typedResults with 
                    | None -> 
                        // OK, the typed results were not available because the background state to typecheck the file is not yet
                        // ready.  However, we will be notified when it _is_ ready, courtesy of the background builder. Hence
                        // we can clear the dirty bit and wait for that notification.
                        req.ResultClearsDirtinessOfFile <- not aborted
                        req.IsAborted <- aborted
                        // On 'FullTypeCheck', send a message to the reactor to start the background compile for this project, just in case
                        if req.Reason = BackgroundRequestReason.FullTypeCheck then    
                            interactiveChecker.StartBackgroundCompile(checkOptions) 
                    | Some typedResults -> 
                        // Post the parse errors. 
                        if containsFreshFullTypeCheck then 
                            for error in typedResults.Errors do
                                let span = new TextSpan(iStartLine=error.StartLine,iStartIndex=error.StartColumn,iEndLine=error.EndLine,iEndIndex=error.EndColumn)                             
                                let sev = 
                                    match error.Severity with 
                                    | Microsoft.FSharp.Compiler.Severity.Warning -> Severity.Warning
                                    | Microsoft.FSharp.Compiler.Severity.Error -> Severity.Error
                                req.ResultSink.AddError(req.FileName, error.Subcategory, error.Message, span, sev)
                          

                        let scope = new FSharpScope(untypedParse, req.Line, req.Col, req.Snapshot, typedResults, projectSite, req.View, colorizer, this.DocumentationProvider) 

                        req.ResultScope <- scope
                        req.ResultTimestamp <- resultTimestamp  // This will be different from req.Timestamp when we're using stale results.
                        req.ResultClearsDirtinessOfFile <- containsFreshFullTypeCheck

                        if req.Reason = BackgroundRequestReason.MethodTip || req.Reason = BackgroundRequestReason.MatchBracesAndMethodTip then
                            scope.InitLastRequestedMethodListForMethodTipUsingFallback()

                        // On 'FullTypeCheck', send a message to the reactor to start the background compile for this project, just in case
                        if req.Reason = BackgroundRequestReason.FullTypeCheck then    
                            interactiveChecker.StartBackgroundCompile(checkOptions) 
                            
                        // On 'QuickInfo', get the text for the quick info while we're off the UI thread, instead of doing it later
                        if req.Reason = BackgroundRequestReason.QuickInfo then 
                            let text,span = scope.GetDataTipText(req.Line, req.Col)
                            req.ResultQuickInfoText <- text
                            req.ResultQuickInfoSpan <- span 

                        // NOTE: if you do any more "work" here for any of the background requests, then 
                        // make sure you update IsRecentScopeSufficientForBackgroundRequest
                
        with
        | e ->
            req.IsAborted <- true
            Assert.Exception(e)
            reraise()                


    /// Do OnIdle processing       
    member this.OnIdle() = 
        for file in dirtyForTypeCheckFiles do
            match this.Artifacts.GetSourceOfFilename(this.ServiceProvider.Rdt, file) with
            | Some(source) -> 
#if DEBUG
                Trace.PrintLine("ChangeEvents", fun _ -> sprintf "OnIdle found source for file %s to dirty" file)
#endif
                source.RecordChangeToView()
            | None -> 
#if DEBUG
                Trace.PrintLine("ChangeEvents", fun _ -> sprintf "OnIdle did not find source for file %s to dirty" file)
#else
                ()
#endif
        dirtyForTypeCheckFiles<-Set.empty
        

    /// Remove a colorizer.
    member this.CloseColorizer(colorizer:FSharpColorizer) = 
        let buffer = colorizer.Buffer
        let mutable guid = colorizerGuid
        (buffer :?> IVsUserData).SetData(&guid, null) |> ErrorHandler.ThrowOnFailure |> ignore

    /// Get a colorizer for a particular buffer.
    member this.GetColorizer(buffer:IVsTextLines) : FSharpColorizer = 
        let mutable guid = colorizerGuid
        let mutable colorizerObj = null
        
        (buffer :?> IVsUserData).GetData(&guid, &colorizerObj) |> ignore
        match colorizerObj with
        | null ->
            let scanner = 
                new FSharpScanner(fun source ->
                    // Note: in theory, the next few lines do not need to be recomputed every line.  Instead we could just cache the tokenizer
                    // and only update it when e.g. the project system notifies us there is an important change (e.g. a file rename, etc).
                    // In practice we have been there, and always screwed up some non-unit-tested/testable corner-cases.
                    // So this is not ideal from a perf perspective, but it is easy to reason about the correctness.
                    let filename = VsTextLines.GetFilename buffer
                    let defines = this.Artifacts.GetDefinesForFile(this.ServiceProvider.Rdt, filename, enableStandaloneFileIntellisense)
                    let sourceTokenizer = SourceTokenizer(defines,filename)
                    sourceTokenizer.CreateLineTokenizer(source))

            let colorizer = new FSharpColorizer(this.CloseColorizer, buffer, scanner) 
            (buffer :?> IVsUserData).SetData(&guid, colorizer) |> ErrorHandler.ThrowOnFailure |> ignore
            colorizer
        | _ -> colorizerObj :?> FSharpColorizer
    
    /// Block until the background compile finishes.
    //
    // This is for unit testing only
    member this.WaitForBackgroundCompile() =
        this.InteractiveChecker.WaitForBackgroundCompile()            

module VsConstants =
    let guidStdEditor = new Guid("9ADF33D0-8AAD-11D0-B606-00A0C922E851")
    let guidCodeCloneProvider = new Guid("38fd587e-d4b7-4030-9a95-806ff0d5c2c6")

    let cmdidGotoDecl = 936u // "Go To Declaration"
    let cmdidGotoRef = 1107u // "Go To Reference"
    
    let IDM_VS_EDITOR_CSCD_OUTLINING_MENU = 773u // "Outlining"
    let ECMD_OUTLN_HIDE_SELECTION = 128u // "Hide Selection" - 
    let ECMD_OUTLN_TOGGLE_CURRENT = 129u // "Toggle Outlining Expansion" - 
    let ECMD_OUTLN_TOGGLE_ALL = 130u // "Toggle All Outlining"
    let ECMD_OUTLN_STOP_HIDING_ALL = 131u // "Stop Outlining"
    let ECMD_OUTLN_STOP_HIDING_CURRENT = 132u // "Stop Hiding Current"

type QueryStatusResult =
    | NOTSUPPORTED = 0
    | SUPPORTED = 1
    | ENABLED = 2
    | LATCHED = 4
    | NINCHED = 8
    | INVISIBLE = 16

type FsharpViewFilter(mgr:CodeWindowManager,view:IVsTextView) =
    inherit ViewFilter(mgr,view)

    override this.Dispose() = base.Dispose()

    member this.IsSupportedCommand(guidCmdGroup:byref<Guid>,cmd:uint32) =
        if guidCmdGroup = VsMenus.guidStandardCommandSet97 && (cmd = VsConstants.cmdidGotoDecl || cmd = VsConstants.cmdidGotoRef) then false
        elif guidCmdGroup = VsConstants.guidCodeCloneProvider then false // disable commands for CodeClone package
        else
            // These are all the menu options in the "Outlining" cascading menu. We need to disable all the individual
            // items to disable the cascading menu. QueryCommandStatus does not get called for the cascading menu itself.
            assert((guidCmdGroup = VsConstants.guidStdEditor && cmd = VsConstants.IDM_VS_EDITOR_CSCD_OUTLINING_MENU) = false)
            if guidCmdGroup = VsMenus.guidStandardCommandSet2K && (cmd = VsConstants.ECMD_OUTLN_HIDE_SELECTION ||
                                                                   cmd = VsConstants.ECMD_OUTLN_TOGGLE_CURRENT ||
                                                                   cmd = VsConstants.ECMD_OUTLN_TOGGLE_ALL ||
                                                                   cmd = VsConstants.ECMD_OUTLN_STOP_HIDING_ALL ||
                                                                   cmd = VsConstants.ECMD_OUTLN_STOP_HIDING_CURRENT) then false
            else true

    override this.QueryCommandStatus(guidCmdGroup:byref<Guid>,cmd:uint32) =        
        if this.IsSupportedCommand(&guidCmdGroup,cmd) then
            base.QueryCommandStatus(&guidCmdGroup,cmd)
        else
            // Hide the menu item. Just returning QueryStatusResult.NOTSUPPORTED does not work
            QueryStatusResult.INVISIBLE ||| QueryStatusResult.SUPPORTED |> int


// Forward request for navigation info to the scope built after parsing untyped AST
type FSharpNavigation(svc:LanguageService, stateFunc:unit -> LanguageServiceState) = 
    inherit TypeAndMemberDropdownBars(svc)        
    override x.OnSynchronizeDropdowns(_, textView, line, col, dropDownTypes, dropDownMembers, selectedType:int byref, selectedMember:int byref) =
        match (stateFunc()).UntypedParseScope with
        | Some(scope) -> 
            let file = FilePathUtilities.GetFilePath(VsTextView.Buffer textView)
            scope.SynchronizeNavigationDropDown(file, line, col, dropDownTypes, dropDownMembers, &selectedType, &selectedMember)
        | _ -> 
            dropDownTypes.Clear()
            dropDownTypes.Add(new DropDownMember("(Parsing project files)", new TextSpan(), -1, DROPDOWNFONTATTR.FONTATTR_GRAY)) |> ignore
            dropDownMembers.Clear()
            selectedType <- 0
            selectedMember <- -1
            true
        
        
and [<Guid(FSharpConstants.languageServiceGuidString)>]
    FSharpLanguageService() = 
    inherit LanguageService() 
    
    // In case the config file is incorrect, we silently recover and disable the feature
    let enableRegions = 
        try "true" = System.Configuration.ConfigurationManager.AppSettings.[FSharpConstants.enableRegions]
        with e ->  
            System.Diagnostics.Debug.Assert
              (false, sprintf "Error while loading 'devenv.exe.config' configuration: %A" e)
            false
        
    // In case the config file is incorrect, we silently recover and disable the feature
    let enableNavBar = 
        try "true" = ConfigurationManager.AppSettings.[FSharpConstants.enableNavBar]
        with e -> 
            System.Diagnostics.Debug.Assert
              (false, sprintf "Error while loading 'devenv.exe.config' configuration: %A" e)
            false
            
    // In case the config file is incorrect, we silently recover and leave the feature enabled
    let enableStandaloneFileIntellisense = 
        try "false" <> ConfigurationManager.AppSettings.[FSharpConstants.enableStandaloneFileIntellisense]
        with e -> 
            System.Diagnostics.Debug.Assert
              (false, sprintf "Error while loading 'devenv.exe.config' configuration: %A" e)
            true

    let ls = LanguageServiceState.Create()
    let mutable rdtCookie = VSConstants.VSCOOKIE_NIL
    
    let mutable lastUntypedParseRequest : BackgroundRequest = null

    let thisAssembly = typeof<FSharpLanguageService>.Assembly 
    let resources = lazy (new System.Resources.ResourceManager("VSPackage", thisAssembly))
    let GetString(name:string) = 
        resources.Force().GetString(name, CultureInfo.CurrentUICulture)

    let formatFilterList = lazy(
                                    let fsFile = GetString("FSharpFile")
                                    let fsInterfaceFile = GetString("FSharpInterfaceFile")
                                    let fsxFile = GetString("FSXFile")
                                    let fsScriptFile = GetString("FSharpScriptFile")
                                    let result = sprintf "%s|*.fs\n%s|*.fsi\n%s|*.fsx\n%s|*.fsscript"
                                                             fsFile fsInterfaceFile fsxFile fsScriptFile
                                    result)
    
    // This array contains the definition of the colorable items provided by this
    // language service.
    let colorableItems = [|
            // See e.g. the TokenColor type defined in Scanner.cs.  Position 0 is implicit and always means "Plain Text".
            // The next 5 items in this list MUST be these default items in this order:
            new FSharpColorableItem("Keyword",              lazy (GetString("Keyword")),             COLORINDEX.CI_BLUE,         COLORINDEX.CI_USERTEXT_BK)
            new FSharpColorableItem("Comment",              lazy (GetString("Comment")),             COLORINDEX.CI_DARKGREEN,    COLORINDEX.CI_USERTEXT_BK)
            new FSharpColorableItem("Identifier",           lazy (GetString("Identifier")),          COLORINDEX.CI_USERTEXT_FG,  COLORINDEX.CI_USERTEXT_BK)
            new FSharpColorableItem("String",               lazy (GetString("String")),              COLORINDEX.CI_MAROON,       COLORINDEX.CI_USERTEXT_BK)
            new FSharpColorableItem("Number",               lazy (GetString("Number")),              COLORINDEX.CI_USERTEXT_FG,  COLORINDEX.CI_USERTEXT_BK)
            // User-defined color classes go here:
            new FSharpColorableItem("Excluded Code",        lazy (GetString("ExcludedCode")),         COLORINDEX.CI_DARKGRAY,     COLORINDEX.CI_USERTEXT_BK)
            new FSharpColorableItem("Preprocessor Keyword", lazy (GetString("PreprocessorKeyword")),  COLORINDEX.CI_BLUE,         COLORINDEX.CI_USERTEXT_BK)
            new FSharpColorableItem("Operator",             lazy (GetString("Operator")),             COLORINDEX.CI_USERTEXT_FG,  COLORINDEX.CI_USERTEXT_BK)
#if COLORIZE_TYPES
            new FSharpColorableItem("User Types",           lazy (GetString("UserTypes")),                 COLORINDEX.CI_AQUAMARINE,  COLORINDEX.CI_USERTEXT_BK)
#endif
        |]

    /// Initialize the language service
    override fls.Initialize() =
        if not ls.IsInitialized then 
#if DEBUG            
            Flags.init()
            use t = Trace.Call("LanguageService", "FSharpLanguageService::Initialize", fun _->"FLS being initialized for first time and registering for RDT events")
#endif
            let sp = ServiceProvider(fls.GetService)
            let rdt = sp.Rdt
            let preferences = new FSharpLanguagePreferences(fls.Site, (typeof<FSharpLanguageService>).GUID, fls.Name)
            preferences.Init() // Reads preferences from the VS registry.
            preferences.MaxErrorMessages <- 1000
            let fileChangeEx = fls.GetService(typeof<SVsFileChangeEx >) :?> IVsFileChangeEx
            let createSource(buffer:IVsTextLines) =
                Source.CreateSource(fls, buffer, fls.GetColorizer(buffer), fileChangeEx)                
            ls.Initialize
               (sp,
                (XmlDocumentation.Provider(sp.XmlService, sp.DTE) :> IdealDocumentationProvider),
                (preferences :> LanguagePreferences),
                enableStandaloneFileIntellisense,
                createSource)
            rdtCookie <- (Com.ThrowOnFailure1 (rdt.AdviseRunningDocTableEvents (fls:>IVsRunningDocTableEvents)))

                          
    override fls.Dispose() =
        try
            try
                if rdtCookie <> VSConstants.VSCOOKIE_NIL then 
                    let sp = ServiceProvider(fls.GetService)
                    Com.ThrowOnFailure0 (sp.Rdt.UnadviseRunningDocTableEvents rdtCookie)
            finally
                if ls.IsInitialized then 
                    ls.Unhook()
        finally 
            base.Dispose()

    member fls.LanguageServiceState = ls

    override fls.GetInteractiveChecker() = box ls.InteractiveChecker        

    member fls.AddSpecialSource(_source, _view) = ()

    override fls.OnActiveViewChanged(textView) =
        base.OnActiveViewChanged(textView)
        match fls.LanguageServiceState.UntypedParseScope with
        | Some scope -> scope.ClearDisplayedRegions()
        | None -> ()
        fls.LanguageServiceState.UntypedParseScope <- None
        lastUntypedParseRequest <- null // abandon any request for untyped parse information, without cancellation
        if enableNavBar || enableRegions then
           fls.TriggerUntypedParse() |> ignore
      
    /// Do OnIdle processing       
    member fls.TrySynchronizeUntypedParseInformation(millisecondsTimeout:int) =

            //Source s = this.GetSource(view)
        let view = fls.LastActiveTextView
        if (view <> null) then
            let s = fls.GetSource(view)
            if (s <> null) then 

                if (lastUntypedParseRequest = null || lastUntypedParseRequest.Timestamp <> s.ChangeCount) then
                    let req = fls.TriggerUntypedParse()
                    
                    if req <> null && (req.IsSynchronous || req.Result <> null) then
                        // This blocks the UI thread. Give it a slice of time (1000ms) and then just give up on this particular synchronization.
                        // If we end up aborting here then the caller has the option of just using the old untyped parse information 
                        // for the active view if one is available. Sooner or later the request may complete and the new untyped parse information
                        // will become available.
                        lastUntypedParseRequest <- req
                        req.Result.TryWaitForBackgroundRequestCompletion(millisecondsTimeout) 
                    else
                        false
                else
                    // OK, the last request is still active, so try to wait again
                    lastUntypedParseRequest.Result.TryWaitForBackgroundRequestCompletion(millisecondsTimeout) 
            else
                false
        else
            false
                    
      
    /// This is used as part of the registry key to read user preferences.
    override fls.Name = "F#"

    /// This is used to return the expression evaluator language to the debugger
    override fls.GetLanguageID(_buffer,_line, _col, langId) =
        langId <- DebuggerEnvironment.GetLanguageID()
        VSConstants.S_OK

    override fls.CurFileExtensionFormat(filename) = 
        // These indexes match the "GetFormatFilterList" function
        match Path.GetExtension(filename) with
        | ".fs" -> 0
        | ".ml" -> 1
        | ".fsi" -> 2
        | ".mli" -> 3
        | ".fsx" -> 4
        | ".fsscript" -> 5
        | _ -> -1

    override fls.GetFormatFilterList() = 
        formatFilterList.Value 

    // This seems to be called by codeWinMan.OnNewView(textView) to install a ViewFilter on the TextView.    
    override this.CreateViewFilter(mgr:CodeWindowManager,newView:IVsTextView) = new FsharpViewFilter(mgr,newView) :> ViewFilter

    override fls.CreateSource(buffer:IVsTextLines) =
        ls.CreateSource(buffer) :?> ISource
        
    override fls.GetLanguagePreferences() = ls.Preferences

    override fls.CreateBackgroundRequest(line, col, info, sourceText, snapshot, methodTipMiscellany, fname, reason, view, sink, source, timestamp, synchronous) =
        ls.CreateBackgroundRequest(line, col, info, sourceText, snapshot, methodTipMiscellany, fname,reason, view,sink, source, timestamp, synchronous)                                                
        
    override fls.ExecuteBackgroundRequest(req:BackgroundRequest) = 
#if DEBUG
        use t = Trace.CallByThreadNamed("FSharpLanguageService",
                                        "ExecuteBackgroundRequest", 
                                        "MPF Worker", 
                                        fun _->sprintf " location=(%d:%d), reason=%A, filename=%s" req.Line req.Col req.Reason req.FileName)
#endif

        let idealSource = req.Source :?> IdealSource
        ls.ExecuteBackgroundRequest(req, idealSource)

    // Check if we can shortcut executing the background request and just fill in the latest
    // cached scope for the active view from this.service.RecentFullTypeCheckResults.
    //
    // THIS MUST ONLY RETURN TRUE IF ---> ExecuteBackgroundRequest is equivalent to fetching a recent,
    // perhaps out-of-date scope.
    override fls.IsRecentScopeSufficientForBackgroundRequest(reason:BackgroundRequestReason) = 
    
        match reason with 
        | BackgroundRequestReason.MatchBraces 
        | BackgroundRequestReason.MatchBracesAndMethodTip
        | BackgroundRequestReason.UntypedParse 
        | BackgroundRequestReason.FullTypeCheck -> false
            
        // For QuickInfo, we grab the result while we're on the background thread,
        // so returning the scope alone is not sufficient
        | BackgroundRequestReason.QuickInfo -> false
        // For MethodTip, we need a fresh parse to get accurate position info for arguments
        | BackgroundRequestReason.MethodTip -> false
        // For all others, the request is identical to using the latest cached scope
        | BackgroundRequestReason.MemberSelect 
        | BackgroundRequestReason.MemberSelectAndHighlightBraces 
        | BackgroundRequestReason.CompleteWord 
        | BackgroundRequestReason.DisplayMemberList
        | BackgroundRequestReason.Goto
        | _ -> true
      
    // This is called on the UI thread after fresh full typecheck results are available
    override fls.OnUntypedParseOrFullTypeCheckComplete( req:BackgroundRequest) =
        base.OnUntypedParseOrFullTypeCheckComplete(req)

        match req, req.Source, req.ResultScope, req.View with 
        | (:? FSharpBackgroundRequest as fbreq), (:? IdealSource as source), (:? FSharpScope as scope), textView when textView <> null && not req.Source.IsClosed -> 
             match fbreq.TryGetColorizer() with 
             | Some colorizer -> 
                 //let oldSnapshot = fbreq.Snapshot

#if COLORIZE_TYPES
                 //let checkValidity (newSnapshot, range) =SharpScope.HasTextChangedSinceLastTypecheck (newSnapshot, oldSnapshot, range)
#endif
                 for line in colorizer.SetExtraColorizations((* checkValidity, *) scope.GetExtraColorizations()) do
                     source.RecolorizeLine line
             | None -> ()
             
        | _ -> ()

        // Process regions only if they are enabled
        if enableRegions then
            // REVIEW: Do we need to update regions during every parse request?
            match fls.LanguageServiceState.UntypedParseScope with
            | Some scope -> fls.UpdateHiddenRegions(scope, req.Source)
            | None -> ()

    member fls.UpdateHiddenRegions(scope:UntypedFSharpScope,source:ISource) =
#if DEBUG
        use t = Trace.CallByThreadNamed("FSharpLanguageService",
                                        "UpdateHiddenRegions", 
                                        "UI", 
                                        fun _-> "")
#endif

        let toCreate, toUpdate = scope.GetHiddenRegions(FilePathUtilities.GetFilePath(VsTextView.Buffer fls.LastActiveTextView))
        if not (toCreate = [] && toUpdate = Map.empty) then
            // Compare the existing regions with the new regions and 
            // remove any that do not match the new regions.
            let session = source.GetHiddenTextSession()
            let (aregion:IVsHiddenRegion[]) = Array.zeroCreate(1)
            
            // Get current regions from Visual Studio        
            let ppenum = Com.ThrowOnFailure1(session.EnumHiddenRegions(uint32 FIND_HIDDEN_REGION_FLAGS.FHR_ALL_REGIONS, 0u, Array.zeroCreate(1)))
            let regions = 
              seq { let fetched = ref 0u
                    while (ppenum.Next(1u, aregion, fetched) = VSConstants.S_OK && !fetched = 1u) do
                      yield aregion.[0] }
            
            for reg in regions do
                let unique = Com.ThrowOnFailure1(reg.GetClientData())
                match toUpdate.TryFind(unique) with
                | Some(span) -> reg.SetSpan( [| span |]) |> Com.ThrowOnFailure0
                | _ -> reg.Invalidate(uint32 CHANGE_HIDDEN_REGION_FLAGS.chrNonUndoable) |> Com.ThrowOnFailure0
                    
            // TODO: this is what MPF comment says...
            //    For very large documents this can take a while, so add them in chunks of 
            //    1000 and stop after 5 seconds. 
            
            if (toCreate.Length > 0) then
                let arr = toCreate |> Array.ofList
                let mutable (outEnum:IVsEnumHiddenRegions[]) = Array.zeroCreate(arr.Length)
                session.AddHiddenRegions(uint32 CHANGE_HIDDEN_REGION_FLAGS.chrNonUndoable, arr.Length, arr, outEnum) |> Com.ThrowOnFailure0
            
    override fls.GetColorizer(buffer) = 
        fls.Initialize()
        ls.GetColorizer(buffer) :> Colorizer

    override fls.ValidateBreakpointLocation(buffer:IVsTextBuffer, line, col, pCodeSpan:TextSpan[]) =
        let result = 
            if (pCodeSpan <> null) && (pCodeSpan.Length > 0) && (buffer :? IVsTextLines) then
                let syncOk = fls.TrySynchronizeUntypedParseInformation(millisecondsTimeout = 100) 
                let lineText = VsTextLines.LineText (buffer :?> IVsTextLines) line
                let firstNonWhitespace = lineText.Length - (lineText.TrimStart [| ' '; '\t' |]).Length 
                let lastNonWhitespace = (lineText.TrimEnd [| ' '; '\t' |]).Length 
                // If the column is before the range of text then zap it to the text
                // If the column is after the range of text then zap it to the _start_ of the text (like C#)
                let attempt1, haveScope  = 
                    let col = if col > lastNonWhitespace || col < firstNonWhitespace then firstNonWhitespace else col
                    match fls.LanguageServiceState.UntypedParseScope with
                    | Some(scope) -> 
                        match scope.ValidateBreakpointLocation(line,col) with
                        | Some ((a,b),(c,d)) -> Some (TextSpan(iStartLine = a, iStartIndex = b, iEndLine = c, iEndIndex = d)), true
                        | None -> None, true
                    | None ->   
                        None, false
                match attempt1 with 
                | Some r -> Some r
                | None -> 
                    if syncOk || haveScope then 
                        None
                    else 
                        // If we didn't sync OK AND we don't even have an UntypedParseScope then just accept the whole line.
                        // This is unfortunate but necessary.
                        let span = new TextSpan(iStartLine = line, iStartIndex = firstNonWhitespace, iEndLine = line, iEndIndex = lastNonWhitespace)
                        Some span
            else 
                None
                
        match result with 
        | Some span -> 
            pCodeSpan.[0] <- span
            VSConstants.S_OK
        | None -> 
            VSConstants.S_FALSE                
            
    override fls.CreateDropDownHelper(_view) =
        if enableNavBar then 
            (new FSharpNavigation(fls, fun () -> fls.LanguageServiceState)) :> TypeAndMemberDropdownBars
        else null

    override fls.OnIdle(periodic, mgr : IOleComponentManager) =
        try
            let r = base.OnIdle(periodic, mgr)
            ls.OnIdle()
            r
        with e-> 
            Assert.Exception(e)
            reraise()
                        
    interface IVsProvideColorableItems with

        override x.GetItemCount(count: int byref) =
            count <- colorableItems.Length
            VSConstants.S_OK

        override x.GetColorableItem(index, item: IVsColorableItem byref) =
                if (index < 1) then 
                    raise (Error.ArgumentOutOfRange "index")
                item <- colorableItems.[index - 1]
                VSConstants.S_OK

    /// Respond to changes to documents in the Running Document Table.
    interface IVsRunningDocTableEvents with
        override this.OnAfterAttributeChange(_docCookie, _grfAttribs) = VSConstants.S_OK
        override this.OnAfterDocumentWindowHide(_docCookie, _frame) = VSConstants.S_OK
        override this.OnAfterFirstDocumentLock(_docCookie,_dwRDTLockType,_dwReadLocks,_dwEditLocks) = VSConstants.S_OK
        override this.OnAfterSave(_docCookie) = VSConstants.S_OK
        override this.OnBeforeDocumentWindowShow(_docCookie,_isFirstShow,_frame) = VSConstants.S_OK
        override this.OnBeforeLastDocumentUnlock(docCookie,_dwRDTLockType,dwReadLocksRemaining,dwEditLocksRemaining) =
            let (_, _, _, _, file, _, _, unkdoc) = (ServiceProvider this.GetService).Rdt.GetDocumentInfo docCookie // see here http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.shell.interop.ivsrunningdocumenttable.getdocumentinfo(VS.80).aspx for info on the `GetDocumentInfo` results we're ignoring
            try
                if int dwReadLocksRemaining = 0 && int dwEditLocksRemaining = 0 then // check that this is there are no other read / edit locks
                    if SourceFile.IsCompilable file then
                        if IntPtr.Zero<>unkdoc then
                            match Marshal.GetObjectForIUnknown(unkdoc) with
                            | :? IVsTextLines as tl ->
                                ls.Artifacts.UnsetSource(tl)
                            | _ -> ()
            finally 
                if IntPtr.Zero <> unkdoc then Marshal.Release(unkdoc)|>ignore
            VSConstants.S_OK

module FSharpIntellisenseProvider = 
    /// <summary>
    /// This class is used to store the information about one specific instance
    /// of EnvDTE.FileCodeModel.
    /// </summary>
    [<StructuralEquality; NoComparison>]
    type FileCodeModelInfo =
        {  CodeModel: EnvDTE.FileCodeModel
           ItemId : uint32 }

    type SourceFileInfo(filename:string, itemId:uint32) =
        let mutable hostProject : IVsIntellisenseProjectHost = null
        let mutable fileCode : EnvDTE.FileCodeModel = null
        // let mutable codeProvider : CodeDomProvider = null

        member x.HostProject 
            with get() = hostProject
            and set(value) = 
                if (hostProject <> value) then
                    fileCode <- null
                hostProject <- value

        member x.FileCodeModel =
            // Don't build the object more than once.
            if (null <> fileCode) then
                fileCode
            else
                // Verify that the host project is set.
                if (null = hostProject) then raise (Error.NoHostObject)

                // Get the hierarchy from the host project.
                let propValue = Com.ThrowOnFailure1(hostProject.GetHostProperty(uint32(HOSTPROPID.HOSTPROPID_HIERARCHY)))
                let hierarchy = (propValue :?> IVsHierarchy)
                if (null = hierarchy) then null else

                // Try to get the extensibility object for the item.
                // NOTE: here we assume that the __VSHPROPID.VSHPROPID_ExtObject property returns a VSLangProj.VSProjectItem
                // or a EnvDTE.ProjectItem object. No other kind of extensibility is supported.
                let propValue = Com.ThrowOnFailure1(hierarchy.GetProperty(itemId, int32(__VSHPROPID.VSHPROPID_ExtObject)))
                let vsprojItem = (propValue :?> VSLangProj.VSProjectItem)
                let projectItem = 
                    match vsprojItem with
                    | null -> (propValue :?> EnvDTE.ProjectItem)
                    | _ -> vsprojItem.ProjectItem
                if (null = projectItem) then null else

                null

        member x.ItemId = itemId

        member x.Name = filename

#if UNUSED
module Setup = 
    /// This attribute adds a intellisense provider for a specific language 
    /// type. 
    /// For Example:
    ///   [HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\VisualStudio\8.0Exp\Languages\IntellisenseProviders\
    ///         [Custom_Provider]
    /// 
    [<AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)>]
    type ProvideIntellisenseProviderAttribute(provider:Type, providerName:string, addItemLanguageName:string, defaultExtension:string, shortLanguageName:string, templateFolderName:string) =
        inherit RegistrationAttribute() 
        let mutable additionalExtensions : Option<string> = None

        /// Private function to get the provider base key name
        let providerRegKey() = String.Format(CultureInfo.InvariantCulture, @"Languages\IntellisenseProviders\{0}", [| box providerName |])
        
            /// Gets the Type of the intellisense provider.
        member x.Provider = provider
            /// Get the Guid representing the generator type
        member x.ProviderGuid = provider.GUID
            /// Get the ProviderName
        member x.ProviderName = providerName
            /// Get item language
        member x.AddItemLanguageName = addItemLanguageName
            /// Get the Default extension
        member x.DefaultExtension = defaultExtension
            /// Get the short language name
        member x.ShortLanguageName = shortLanguageName
            /// Get the tempalte folder name
        member x.TemplateFolderName = templateFolderName
            /// Get/Set Additional extensions
        member x.AdditionalExtensions 
            with get() = additionalExtensions
            and set v = additionalExtensions <- Some(v)
            ///     Called to register this attribute with the given context.  The context
            ///     contains the location where the registration inforomation should be placed.
            ///     It also contains other information such as the type being registered and path information.
        override x.Register(context:RegistrationAttribute.RegistrationContext) =
            Check.ArgumentNotNull context "context"
            use childKey = context.CreateKey(providerRegKey())
            let mutable providerGuid = provider.GUID
            childKey.SetValue("GUID", providerGuid.ToString("B"))
            childKey.SetValue("AddItemLanguageName", addItemLanguageName)
            childKey.SetValue("DefaultExtension", defaultExtension)
            childKey.SetValue("ShortLanguageName", shortLanguageName)
            childKey.SetValue("TemplateFolderName", templateFolderName)
            match additionalExtensions with 
            | None | Some "" -> ()
            | Some(s) ->  childKey.SetValue("AdditionalExtensions", s)

            /// <summary>
            /// Unregister this file extension.
            /// </summary>
            /// <param name="context"></param>
        override x.Unregister(context:RegistrationAttribute.RegistrationContext) =
            if (null <> context) then
                context.RemoveKey(providerRegKey())
#endif

[<Guid("871D2A70-12A2-4e42-9440-425DD92A4116")>]
type FSharpPackage() as self =
    inherit Package()
    
    // In case the config file is incorrect, we silently recover and leave the feature enabled
    let enableLanguageService = 
        try 
            "false" <> ConfigurationManager.AppSettings.[FSharpConstants.enableLanguageService]
        with e -> 
            System.Diagnostics.Debug.Assert
              (false, sprintf "Error while loading 'devenv.exe.config' configuration: %A" e)
            true         

    let mutable componentID = 0u
    
    let CreateIfEnabled container serviceType = 
        if enableLanguageService then 
            self.CreateService(container,serviceType) 
        else 
            null
            
    let callback = new ServiceCreatorCallback(CreateIfEnabled)
    
    let mutable mgr : IOleComponentManager = null
    
    override self.Initialize() =
        UIThread.CaptureSynchronizationContext()
        (self :> IServiceContainer).AddService(typeof<FSharpLanguageService>, callback, true)
        base.Initialize()

    member self.RegisterForIdleTime() =
        mgr <- (self.GetService(typeof<SOleComponentManager>) :?> IOleComponentManager)
        if (componentID = 0u && mgr <> null) then
            let crinfo = Array.zeroCreate<OLECRINFO>(1)
            let mutable crinfo0 = crinfo.[0]
            crinfo0.cbSize <- Marshal.SizeOf(typeof<OLECRINFO>) |> uint32
            crinfo0.grfcrf <- uint32 (_OLECRF.olecrfNeedIdleTime ||| _OLECRF.olecrfNeedPeriodicIdleTime)
            crinfo0.grfcadvf <- uint32 (_OLECADVF.olecadvfModal ||| _OLECADVF.olecadvfRedrawOff ||| _OLECADVF.olecadvfWarningsOff)
            crinfo0.uIdleTimeInterval <- 1000u
            crinfo.[0] <- crinfo0 
            let componentID_out = ref componentID
            let _hr = mgr.FRegisterComponent(self, crinfo, componentID_out)
            componentID <- componentID_out.Value
            ()

    member self.CreateService(_container:IServiceContainer, serviceType:Type) =
        match serviceType with 
        | x when x = typeof<FSharpLanguageService> -> 
            let language = new FSharpLanguageService()
            language.SetSite(self)
            language.Initialize()
            TypeProviderSecurityGlobals.invalidationCallback <- fun () -> language.LanguageServiceState.InteractiveChecker.InvalidateAll()
            let showDialog typeProviderRunTimeAssemblyFileName =
                let pubInfo = GetVerifiedPublisherInfo.GetVerifiedPublisherInfo typeProviderRunTimeAssemblyFileName
                let filename = 
                    match Microsoft.FSharp.Compiler.ExtensionTyping.GlobalsTheLanguageServiceCanPoke.theMostRecentFileNameWeChecked with
                    | None -> assert false; ""  // this should never happen
                    | Some fn -> fn
                UIThread.RunSync(fun() ->
                    // need to access the RDT on the UI thread
                    match language.LanguageServiceState.Artifacts.TryFindOwningProject((ServiceProvider language.GetService).Rdt, filename) with
                    | Some owningProjectSite ->
                        let projectName = Path.GetFileNameWithoutExtension(owningProjectSite.ProjectFileName())
                        TypeProviderSecurityDialog.ShowModal(TypeProviderSecurityDialogKind.A, null, projectName, typeProviderRunTimeAssemblyFileName, pubInfo) 
                    | None -> 
                        TypeProviderSecurityDialog.ShowModal(TypeProviderSecurityDialogKind.A, filename, null, typeProviderRunTimeAssemblyFileName, pubInfo) 
                    )
                // the 'displayLSTypeProviderSecurityDialogBlockingUI' callback is run async to the background typecheck, so after the user has interacted with the dialog, request a re-typecheck
                TypeProviderSecurityGlobals.invalidationCallback() 

            let unlessAlwaysTrustShowDialog path =
                if not (Microsoft.FSharp.Compiler.ExtensionTyping.ApprovalsChecking.isAlwaysTrust ())
                then showDialog path

            Microsoft.FSharp.Compiler.ExtensionTyping.GlobalsTheLanguageServiceCanPoke.displayLSTypeProviderSecurityDialogBlockingUI <- Some unlessAlwaysTrustShowDialog

            self.RegisterForIdleTime()
            box language
        | _ -> null

    override self.Dispose(disposing) =
        try 
            if (componentID <> 0u) then
                begin match self.GetService(typeof<SOleComponentManager>) with 
                | :? IOleComponentManager as mgr -> 
                    mgr.FRevokeComponent(componentID) |> ignore
                | _ -> ()
                end
                componentID <- 0u
        finally
            base.Dispose(disposing)

    interface IOleComponent with

        override x.FContinueMessageLoop(_uReason:uint32, _pvLoopData:IntPtr, _pMsgPeeked:MSG[]) = 
            1

        override x.FDoIdle(grfidlef:uint32) =
            // see e.g "C:\Program Files\Microsoft Visual Studio 2008 SDK\VisualStudioIntegration\Common\IDL\olecm.idl" for details
            //Trace.Print("CurrentDirectoryDebug", (fun () -> sprintf "curdir='%s'\n" (System.IO.Directory.GetCurrentDirectory())))  // can be useful for watching how GetCurrentDirectory changes
            match x.GetService(typeof<FSharpLanguageService>) with 
            | :? FSharpLanguageService as pl -> 
                let periodic = (grfidlef &&& (uint32 _OLEIDLEF.oleidlefPeriodic)) <> 0u
                let mutable r = pl.OnIdle(periodic, mgr)
                if r = 0 && periodic && mgr.FContinueIdle() <> 0 then
                    r <- TaskReporterIdleRegistration.DoIdle(mgr)
                r
            | _ -> 0

        override x.FPreTranslateMessage(_pMsg) = 0

        override x.FQueryTerminate(_fPromptUser) = 1

        override x.FReserved1(_dwReserved, _message, _wParam, _lParam) = 1

        override x.HwndGetWindow(_dwWhich, _dwReserved) = 0n

        override x.OnActivationChange(_pic, _fSameComponent, _pcrinfo, _fHostIsActivating, _pchostinfo, _dwReserved) = ()

        override x.OnAppActivate(_fActive, _dwOtherThreadID) = ()

        override x.OnEnterState(_uStateID, _fEnter)  = ()
        
        override x.OnLoseActivation() = ()

        override x.Terminate() = ()

     
