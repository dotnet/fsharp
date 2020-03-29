// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Open up the compiler as an incremental service for parsing,
// type checking and intellisense-like environment-reporting.

namespace FSharp.Compiler.SourceCodeServices

open System
open System.Diagnostics
open System.IO
open System.Reflection

open FSharp.Core.Printf
open FSharp.Compiler 
open FSharp.Compiler.AbstractIL
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.Internal.Library  
open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.CompileOps
open FSharp.Compiler.CompileOptions
open FSharp.Compiler.CompilerGlobalState
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.Features
open FSharp.Compiler.Layout
open FSharp.Compiler.Lexhelp
open FSharp.Compiler.Lib
open FSharp.Compiler.PrettyNaming
open FSharp.Compiler.Parser
open FSharp.Compiler.ParseHelpers
open FSharp.Compiler.Range
open FSharp.Compiler.SyntaxTree
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TcGlobals 
open FSharp.Compiler.Text
open FSharp.Compiler.Infos
open FSharp.Compiler.InfoReader
open FSharp.Compiler.NameResolution
open FSharp.Compiler.TypeChecker
open FSharp.Compiler.SourceCodeServices.SymbolHelpers 

open Internal.Utilities
open Internal.Utilities.Collections
open FSharp.Compiler.AbstractIL.ILBinaryReader

[<AutoOpen>]
module internal FSharpCheckerResultsSettings =

    let getToolTipTextSize = GetEnvInteger "FCS_GetToolTipTextCacheSize" 5

    let maxTypeCheckErrorsOutOfProjectContext = GetEnvInteger "FCS_MaxErrorsOutOfProjectContext" 3

    /// Maximum time share for a piece of background work before it should (cooperatively) yield
    /// to enable other requests to be serviced. Yielding means returning a continuation function
    /// (via an Eventually<_> value of case NotYetDone) that can be called as the next piece of work. 
    let maxTimeShareMilliseconds = 
        match System.Environment.GetEnvironmentVariable("FCS_MaxTimeShare") with 
        | null | "" -> 100L
        | s -> int64 s

    // Look for DLLs in the location of the service DLL first.
    let defaultFSharpBinariesDir = FSharpEnvironment.BinFolderOfDefaultFSharpCompiler(Some(Path.GetDirectoryName(typeof<IncrementalBuilder>.Assembly.Location))).Value

[<RequireQualifiedAccess>]
type FSharpFindDeclFailureReason = 

    // generic reason: no particular information about error
    | Unknown of message: string

    // source code file is not available
    | NoSourceCode

    // trying to find declaration of ProvidedType without TypeProviderDefinitionLocationAttribute
    | ProvidedType of string

    // trying to find declaration of ProvidedMember without TypeProviderDefinitionLocationAttribute
    | ProvidedMember of string

[<RequireQualifiedAccess>]
type FSharpFindDeclResult = 

    /// declaration not found + reason
    | DeclNotFound of FSharpFindDeclFailureReason

    /// found declaration
    | DeclFound of range

    /// Indicates an external declaration was found
    | ExternalDecl of assembly : string * externalSym : ExternalSymbol

/// This type is used to describe what was found during the name resolution.
/// (Depending on the kind of the items, we may stop processing or continue to find better items)
[<RequireQualifiedAccess; NoEquality; NoComparison>]
type internal NameResResult = 
    | Members of (ItemWithInst list * DisplayEnv * range)
    | Cancel of DisplayEnv * range
    | Empty
    | TypecheckStaleAndTextChanged

[<RequireQualifiedAccess>]
type ResolveOverloads = 
|   Yes
|   No

[<RequireQualifiedAccess>]
type GetPreciseCompletionListFromExprTypingsResult =
    | NoneBecauseTypecheckIsStaleAndTextChanged
    | NoneBecauseThereWereTypeErrors
    | None
    | Some of (ItemWithInst list * DisplayEnv * range) * TType

type Names = string list 
    
/// A TypeCheckInfo represents everything we get back from the typecheck of a file.
/// It acts like an in-memory database about the file.
/// It is effectively immutable and not updated: when we re-typecheck we just drop the previous
/// scope object on the floor and make a new one.
[<Sealed>]
type internal TypeCheckInfo
          (// Information corresponding to miscellaneous command-line options (--define, etc).
           _sTcConfig: TcConfig,
           g: TcGlobals,
           // The signature of the assembly being checked, up to and including the current file
           ccuSigForFile: ModuleOrNamespaceType,
           thisCcu: CcuThunk,
           tcImports: TcImports,
           tcAccessRights: AccessorDomain,
           projectFileName: string,
           mainInputFileName: string,
           sResolutions: TcResolutions,
           sSymbolUses: TcSymbolUses,
           // This is a name resolution environment to use if no better match can be found.
           sFallback: NameResolutionEnv,
           loadClosure : LoadClosure option,
           reactorOps : IReactorOperations,
           textSnapshotInfo:obj option,
           implFileOpt: TypedImplFile option,
           openDeclarations: OpenDeclaration[]) = 

    let textSnapshotInfo = defaultArg textSnapshotInfo null

    // These strings are potentially large and the editor may choose to hold them for a while.
    // Use this cache to fold together data tip text results that are the same. 
    // Is not keyed on 'Names' collection because this is invariant for the current position in 
    // this unchanged file. Keyed on lineStr though to prevent a change to the currently line
    // being available against a stale scope.
    let getToolTipTextCache = AgedLookup<CompilationThreadToken, int*int*string, FSharpToolTipText<Layout>>(getToolTipTextSize,areSimilar=(fun (x,y) -> x = y))
    
    let amap = tcImports.GetImportMap()
    let infoReader = new InfoReader(g,amap)
    let ncenv = new NameResolver(g,amap,infoReader,NameResolution.FakeInstantiationGenerator)
    let cenv = SymbolEnv(g, thisCcu, Some ccuSigForFile, tcImports, amap, infoReader)
    
    /// Find the most precise naming environment for the given line and column
    let GetBestEnvForPos cursorPos  =
        
        let mutable bestSoFar = None

        // Find the most deeply nested enclosing scope that contains given position
        sResolutions.CapturedEnvs |> ResizeArray.iter (fun (possm,env,ad) -> 
            if rangeContainsPos possm cursorPos then
                match bestSoFar with 
                | Some (bestm,_,_) -> 
                    if rangeContainsRange bestm possm then 
                      bestSoFar <- Some (possm,env,ad)
                | None -> 
                    bestSoFar <- Some (possm,env,ad))

        let mostDeeplyNestedEnclosingScope = bestSoFar 
        
        // Look for better subtrees on the r.h.s. of the subtree to the left of where we are 
        // Should really go all the way down the r.h.s. of the subtree to the left of where we are 
        // This is all needed when the index is floating free in the area just after the environment we really want to capture 
        // We guarantee to only refine to a more nested environment.  It may not be strictly  
        // the right environment, but will always be at least as rich 

        let mutable bestAlmostIncludedSoFar = None 

        sResolutions.CapturedEnvs |> ResizeArray.iter (fun (possm,env,ad) -> 
            // take only ranges that strictly do not include cursorPos (all ranges that touch cursorPos were processed during 'Strict Inclusion' part)
            if rangeBeforePos possm cursorPos && not (posEq possm.End cursorPos) then 
                let contained = 
                    match mostDeeplyNestedEnclosingScope with 
                    | Some (bestm,_,_) -> rangeContainsRange bestm possm 
                    | None -> true 
                
                if contained then 
                    match bestAlmostIncludedSoFar with 
                    | Some (rightm:range,_,_) -> 
                        if posGt possm.End rightm.End || 
                          (posEq possm.End rightm.End && posGt possm.Start rightm.Start) then
                            bestAlmostIncludedSoFar <- Some (possm,env,ad)
                    | _ -> bestAlmostIncludedSoFar <- Some (possm,env,ad))
        
        let resEnv = 
            match bestAlmostIncludedSoFar, mostDeeplyNestedEnclosingScope with 
            | Some (_,env,ad), None -> env, ad
            | Some (_,almostIncludedEnv,ad), Some (_,mostDeeplyNestedEnv,_) 
                when almostIncludedEnv.eFieldLabels.Count >= mostDeeplyNestedEnv.eFieldLabels.Count -> 
                almostIncludedEnv,ad
            | _ -> 
                match mostDeeplyNestedEnclosingScope with 
                | Some (_,env,ad) -> 
                    env,ad
                | None -> 
                    sFallback,AccessibleFromSomeFSharpCode
        let pm = mkRange mainInputFileName cursorPos cursorPos 

        resEnv,pm

    /// The items that come back from ResolveCompletionsInType are a bit
    /// noisy. Filter a few things out.
    ///
    /// e.g. prefer types to constructors for FSharpToolTipText 
    let FilterItemsForCtors filterCtors (items: ItemWithInst list) =
        let items = items |> List.filter (fun item -> match item.Item with (Item.CtorGroup _) when filterCtors = ResolveTypeNamesToTypeRefs -> false | _ -> true) 
        items
        
    // Filter items to show only valid & return Some if there are any
    let ReturnItemsOfType (items: ItemWithInst list) g denv (m:range) filterCtors hasTextChangedSinceLastTypecheck =
        let items = 
            items 
            |> RemoveDuplicateItems g
            |> RemoveExplicitlySuppressed g
            |> FilterItemsForCtors filterCtors

        if not (isNil items) then
            if hasTextChangedSinceLastTypecheck(textSnapshotInfo, m) then
                NameResResult.TypecheckStaleAndTextChanged // typecheck is stale, wait for second-chance IntelliSense to bring up right result
            else
                NameResResult.Members (items, denv, m) 
        else NameResResult.Empty

    let GetCapturedNameResolutions (endOfNamesPos: pos) resolveOverloads =
        let filter (endPos: pos) items =
            items |> ResizeArray.filter (fun (cnr: CapturedNameResolution) ->
                let range = cnr.Range
                range.EndLine = endPos.Line && range.EndColumn = endPos.Column)

        match resolveOverloads with 
        | ResolveOverloads.Yes ->
            filter endOfNamesPos sResolutions.CapturedNameResolutions 

        | ResolveOverloads.No ->
            let items = filter endOfNamesPos sResolutions.CapturedMethodGroupResolutions
            if items.Count <> 0 then
                items
            else
                filter endOfNamesPos sResolutions.CapturedNameResolutions

    /// Looks at the exact name resolutions that occurred during type checking
    /// If 'membersByResidue' is specified, we look for members of the item obtained 
    /// from the name resolution and filter them by the specified residue (?)
    let GetPreciseItemsFromNameResolution(line, colAtEndOfNames, membersByResidue, filterCtors, resolveOverloads, hasTextChangedSinceLastTypecheck) = 
        let endOfNamesPos = mkPos line colAtEndOfNames

        // Logic below expects the list to be in reverse order of resolution
        let cnrs = GetCapturedNameResolutions endOfNamesPos resolveOverloads |> ResizeArray.toList |> List.rev

        match cnrs, membersByResidue with 
        
        // If we're looking for members using a residue, we'd expect only
        // a single item (pick the first one) and we need the residue (which may be "")
        | CNR(Item.Types(_,(ty::_)), _, denv, nenv, ad, m)::_, Some _ -> 
            let items = ResolveCompletionsInType ncenv nenv (ResolveCompletionTargets.All(ConstraintSolver.IsApplicableMethApprox g amap m)) m ad true ty 
            let items = List.map ItemWithNoInst items
            ReturnItemsOfType items g denv m filterCtors hasTextChangedSinceLastTypecheck 
        
        // Value reference from the name resolution. Primarily to disallow "let x.$ = 1"
        // In most of the cases, value references can be obtained from expression typings or from environment,
        // so we wouldn't have to handle values here. However, if we have something like:
        //   let varA = "string"
        //   let varA = if b then 0 else varA.
        // then the expression typings get confused (thinking 'varA:int'), so we use name resolution even for usual values.
        
        | CNR(Item.Value(vref), occurence, denv, nenv, ad, m)::_, Some _ ->
            if (occurence = ItemOccurence.Binding || occurence = ItemOccurence.Pattern) then 
              // Return empty list to stop further lookup - for value declarations
              NameResResult.Cancel(denv, m)
            else 
              // If we have any valid items for the value, then return completions for its type now.
              // Adjust the type in case this is the 'this' pointer stored in a reference cell.
              let ty = StripSelfRefCell(g, vref.BaseOrThisInfo, vref.TauType) 
              // patch accessibility domain to remove protected members if accessing NormalVal
              let ad = 
                match vref.BaseOrThisInfo, ad with
                | ValBaseOrThisInfo.NormalVal, AccessibleFrom(paths, Some tcref) ->
                    let tcref = generalizedTyconRef tcref
                    // check that type of value is the same or subtype of tcref
                    // yes - allow access to protected members
                    // no - strip ability to access protected members
                    if FSharp.Compiler.TypeRelations.TypeFeasiblySubsumesType 0 g amap m tcref FSharp.Compiler.TypeRelations.CanCoerce ty then
                        ad
                    else
                        AccessibleFrom(paths, None)
                | _ -> ad

              let items = ResolveCompletionsInType ncenv nenv (ResolveCompletionTargets.All(ConstraintSolver.IsApplicableMethApprox g amap m)) m ad false ty
              let items = List.map ItemWithNoInst items
              ReturnItemsOfType items g denv m filterCtors hasTextChangedSinceLastTypecheck
        
        // No residue, so the items are the full resolution of the name
        | CNR(_, _, denv, _, _, m) :: _, None -> 
            let items = 
                cnrs 
                |> List.map (fun cnr -> cnr.ItemWithInst)
                // "into" is special magic syntax, not an identifier or a library call.  It is part of capturedNameResolutions as an 
                // implementation detail of syntax coloring, but we should not report name resolution results for it, to prevent spurious QuickInfo.
                |> List.filter (fun item -> match item.Item with Item.CustomOperation(CustomOperations.Into,_,_) -> false | _ -> true) 
            ReturnItemsOfType items g denv m filterCtors hasTextChangedSinceLastTypecheck
        | _, _ -> NameResResult.Empty
    
    let TryGetTypeFromNameResolution(line, colAtEndOfNames, membersByResidue, resolveOverloads) = 
        let endOfNamesPos = mkPos line colAtEndOfNames
        let items = GetCapturedNameResolutions endOfNamesPos resolveOverloads |> ResizeArray.toList |> List.rev
        
        match items, membersByResidue with 
        | CNR(Item.Types(_,(ty::_)),_,_,_,_,_)::_, Some _ -> Some ty
        | CNR(Item.Value(vref), occurence,_,_,_,_)::_, Some _ ->
            if (occurence = ItemOccurence.Binding || occurence = ItemOccurence.Pattern) then None
            else Some (StripSelfRefCell(g, vref.BaseOrThisInfo, vref.TauType))
        | _, _ -> None

    let CollectParameters (methods: MethInfo list) amap m: Item list = 
        methods
        |> List.collect (fun meth ->
            match meth.GetParamDatas(amap, m, meth.FormalMethodInst) with
            | x::_ -> x |> List.choose(fun (ParamData(_isParamArray, _isInArg, _isOutArg, _optArgInfo, _callerInfo, name, _, ty)) -> 
                match name with
                | Some n -> Some (Item.ArgName(n, ty, Some (ArgumentContainer.Method meth)))
                | None -> None
                )
            | _ -> []
        )

    let GetNamedParametersAndSettableFields endOfExprPos hasTextChangedSinceLastTypecheck =
        let cnrs = GetCapturedNameResolutions endOfExprPos ResolveOverloads.No |> ResizeArray.toList |> List.rev
        let result =
            match cnrs with
            | CNR(Item.CtorGroup(_, ((ctor::_) as ctors)), _, denv, nenv, ad, m) ::_ ->
                let props = ResolveCompletionsInType ncenv nenv ResolveCompletionTargets.SettablePropertiesAndFields m ad false ctor.ApparentEnclosingType
                let parameters = CollectParameters ctors amap m
                let items = props @ parameters
                Some (denv, m, items)
            | CNR(Item.MethodGroup(_, methods, _), _, denv, nenv, ad, m) ::_ ->
                let props = 
                    methods
                    |> List.collect (fun meth ->
                        let retTy = meth.GetFSharpReturnTy(amap, m, meth.FormalMethodInst)
                        ResolveCompletionsInType ncenv nenv ResolveCompletionTargets.SettablePropertiesAndFields m ad false retTy
                    )
                let parameters = CollectParameters methods amap m
                let items = props @ parameters
                Some (denv, m, items)
            | _ -> 
                None
        match result with
        | None -> 
            NameResResult.Empty
        | Some (denv, m, items) -> 
            let items = List.map ItemWithNoInst items
            ReturnItemsOfType items g denv m TypeNameResolutionFlag.ResolveTypeNamesToTypeRefs hasTextChangedSinceLastTypecheck
    
    /// finds captured typing for the given position
    let GetExprTypingForPosition(endOfExprPos) = 
        let quals = 
            sResolutions.CapturedExpressionTypings 
            |> Seq.filter (fun (ty,nenv,_,m) -> 
                    // We only want expression types that end at the particular position in the file we are looking at.
                    posEq m.End endOfExprPos &&

                    // Get rid of function types.  True, given a 2-arg curried function "f x y", it is legal to do "(f x).GetType()",
                    // but you almost never want to do this in practice, and we choose not to offer up any intellisense for 
                    // F# function types.
                    not (isFunTy nenv.DisplayEnv.g ty))
            |> Seq.toArray

        let thereWereSomeQuals = not (Array.isEmpty quals)
        // filter out errors

        let quals = quals 
                    |> Array.filter (fun (ty,nenv,_,_) ->
                        let denv = nenv.DisplayEnv
                        not (isTyparTy denv.g ty && (destTyparTy denv.g ty).IsFromError))
        thereWereSomeQuals, quals
    
    /// obtains captured typing for the given position
    /// if type of captured typing is record - returns list of record fields
    let GetRecdFieldsForExpr(r : range) = 
        let _, quals = GetExprTypingForPosition(r.End)
        let bestQual = 
            match quals with
            | [||] -> None
            | quals ->  
                quals |> Array.tryFind (fun (_,_,_,rq) -> 
                                            ignore(r)  // for breakpoint
                                            posEq r.Start rq.Start)
        match bestQual with
        | Some (ty,nenv,ad,m) when isRecdTy nenv.DisplayEnv.g ty ->
            let items = NameResolution.ResolveRecordOrClassFieldsOfType ncenv m ad ty false
            Some (items, nenv.DisplayEnv, m)
        | _ -> None

    /// Looks at the exact expression types at the position to the left of the 
    /// residue then the source when it was typechecked.
    let GetPreciseCompletionListFromExprTypings(parseResults:FSharpParseFileResults, endOfExprPos, filterCtors, hasTextChangedSinceLastTypecheck: (obj * range -> bool)) = 
        
        let thereWereSomeQuals, quals = GetExprTypingForPosition(endOfExprPos)

        match quals with
        | [| |] -> 
            if thereWereSomeQuals then
                GetPreciseCompletionListFromExprTypingsResult.NoneBecauseThereWereTypeErrors 
            else
                GetPreciseCompletionListFromExprTypingsResult.None
        | _ ->
            let bestQual, textChanged = 
                match parseResults.ParseTree with
                | Some(input) -> 
                    match UntypedParseImpl.GetRangeOfExprLeftOfDot(endOfExprPos,Some(input)) with   // TODO we say "colAtEndOfNames" everywhere, but that's not really a good name ("foo  .  $" hit Ctrl-Space at $)
                    | Some( exprRange) ->
                        if hasTextChangedSinceLastTypecheck(textSnapshotInfo, exprRange) then
                            None, true // typecheck is stale, wait for second-chance IntelliSense to bring up right result
                        else
                            // See bug 130733.  We have an up-to-date sync parse, and know the exact range of the prior expression.
                            // The quals all already have the same ending position, so find one with a matching starting position, if it exists.
                            // If not, then the stale typecheck info does not have a capturedExpressionTyping for this exact expression, and the
                            // user can wait for typechecking to catch up and second-chance intellisense to give the right result.
                            let qual = 
                                quals |> Array.tryFind (fun (_,_,_,r) -> 
                                                            ignore(r)  // for breakpoint
                                                            posEq exprRange.Start r.Start)
                            qual, false
                    | None -> 
                        // TODO In theory I think we should never get to this code path; it would be nice to add an assert.
                        // In practice, we do get here in some weird cases like "2.0 .. 3.0" and hitting Ctrl-Space in between the two dots of the range operator.
                        // I wasn't able to track down what was happening in those weird cases, not worth worrying about, it doesn't manifest as a product bug or anything.
                        None, false
                | _ -> None, false

            match bestQual with
            | Some bestQual ->
                let (ty,nenv,ad,m) = bestQual 
                let items = ResolveCompletionsInType ncenv nenv (ResolveCompletionTargets.All(ConstraintSolver.IsApplicableMethApprox g amap m)) m ad false ty 
                let items = items |> List.map ItemWithNoInst
                let items = items |> RemoveDuplicateItems g
                let items = items |> RemoveExplicitlySuppressed g
                let items = items |> FilterItemsForCtors filterCtors 
                GetPreciseCompletionListFromExprTypingsResult.Some((items,nenv.DisplayEnv,m), ty)
            | None -> 
                if textChanged then GetPreciseCompletionListFromExprTypingsResult.NoneBecauseTypecheckIsStaleAndTextChanged
                else GetPreciseCompletionListFromExprTypingsResult.None

    /// Find items in the best naming environment.
    let GetEnvironmentLookupResolutions(nenv, ad, m, plid, filterCtors, showObsolete) = 
        let items = NameResolution.ResolvePartialLongIdent ncenv nenv (ConstraintSolver.IsApplicableMethApprox g amap m) m ad plid showObsolete
        let items = items |> List.map ItemWithNoInst
        let items = items |> RemoveDuplicateItems g 
        let items = items |> RemoveExplicitlySuppressed g
        let items = items |> FilterItemsForCtors filterCtors 
        (items, nenv.DisplayEnv, m)

    /// Find items in the best naming environment.
    let GetEnvironmentLookupResolutionsAtPosition(cursorPos, plid, filterCtors, showObsolete) = 
        let (nenv,ad),m = GetBestEnvForPos cursorPos
        GetEnvironmentLookupResolutions(nenv, ad, m, plid, filterCtors, showObsolete)

    /// Find record fields in the best naming environment.
    let GetClassOrRecordFieldsEnvironmentLookupResolutions(cursorPos, plid) = 
        let (nenv, ad),m = GetBestEnvForPos cursorPos
        let items = NameResolution.ResolvePartialLongIdentToClassOrRecdFields ncenv nenv m ad plid false
        let items = items |> List.map ItemWithNoInst
        let items = items |> RemoveDuplicateItems g 
        let items = items |> RemoveExplicitlySuppressed g
        items, nenv.DisplayEnv, m 

    /// Resolve a location and/or text to items.
    //   Three techniques are used
    //        - look for an exact known name resolution from type checking
    //        - use the known type of an expression, e.g. (expr).Name, to generate an item list  
    //        - lookup an entire name in the name resolution environment, e.g. A.B.Name, to generate an item list
    //
    // The overall aim is to resolve as accurately as possible based on what we know from type inference
    
    let GetBaseClassCandidates = function
        | Item.ModuleOrNamespaces _ -> true
        | Item.Types(_, ty::_) when (isClassTy g ty) && not (isSealedTy g ty) -> true
        | _ -> false   

    let GetInterfaceCandidates = function
        | Item.ModuleOrNamespaces _ -> true
        | Item.Types(_, ty::_) when (isInterfaceTy g ty) -> true
        | _ -> false   


    // Return only items with the specified name
    let FilterDeclItemsByResidue (getItem: 'a -> Item) residue (items: 'a list) = 
        let attributedResidue = residue + "Attribute"
        let nameMatchesResidue name = (residue = name) || (attributedResidue = name)

        items |> List.filter (fun x -> 
            let item = getItem x
            let n1 =  item.DisplayName 
            match item with
            | Item.Types _ -> nameMatchesResidue n1
            | Item.CtorGroup (_, meths) ->
                nameMatchesResidue n1 ||
                meths |> List.exists (fun meth ->
                    let tcref = meth.ApparentEnclosingTyconRef
                    tcref.IsProvided || nameMatchesResidue tcref.DisplayName)
            | _ -> residue = n1)
            
    /// Post-filter items to make sure they have precisely the right name
    /// This also checks that there are some remaining results 
    /// exactMatchResidueOpt = Some _ -- means that we are looking for exact matches
    let FilterRelevantItemsBy (getItem: 'a -> Item) (exactMatchResidueOpt : _ option) check (items: 'a list, denv, m) =
            
        // can throw if type is in located in non-resolved CCU: i.e. bigint if reference to System.Numerics is absent
        let safeCheck item = try check item with _ -> false
                                                
        // Are we looking for items with precisely the given name?
        if not (isNil items) && exactMatchResidueOpt.IsSome then
            let items = items |> FilterDeclItemsByResidue getItem exactMatchResidueOpt.Value |> List.filter safeCheck 
            if not (isNil items) then Some(items, denv, m) else None        
        else 
            // When (items = []) we must returns Some([],..) and not None
            // because this value is used if we want to stop further processing (e.g. let x.$ = ...)
            let items = items |> List.filter safeCheck
            Some(items, denv, m) 

    /// Post-filter items to make sure they have precisely the right name
    /// This also checks that there are some remaining results 
    let (|FilterRelevantItems|_|) getItem exactMatchResidueOpt orig =
        FilterRelevantItemsBy getItem exactMatchResidueOpt (fun _ -> true) orig
    
    /// Find the first non-whitespace position in a line prior to the given character
    let FindFirstNonWhitespacePosition (lineStr: string) i = 
        if i >= lineStr.Length then None
        else
        let mutable p = i
        while p >= 0 && System.Char.IsWhiteSpace(lineStr.[p]) do
            p <- p - 1
        if p >= 0 then Some p else None
    
    let CompletionItem (ty: ValueOption<TyconRef>) (assemblySymbol: ValueOption<AssemblySymbol>) (item: ItemWithInst) =
        let kind = 
            match item.Item with
            | Item.MethodGroup (_, minfo :: _, _) -> CompletionItemKind.Method minfo.IsExtensionMember
            | Item.RecdField _
            | Item.Property _ -> CompletionItemKind.Property
            | Item.Event _ -> CompletionItemKind.Event
            | Item.ILField _ 
            | Item.Value _ -> CompletionItemKind.Field
            | Item.CustomOperation _ -> CompletionItemKind.CustomOperation
            | _ -> CompletionItemKind.Other

        { ItemWithInst = item
          MinorPriority = 0
          Kind = kind
          IsOwnMember = false
          Type = match ty with ValueSome x -> Some x | _ -> None
          Unresolved = match assemblySymbol with ValueSome x -> Some x.UnresolvedSymbol | _ -> None }

    let DefaultCompletionItem item = CompletionItem ValueNone ValueNone item
    
    let getItem (x: ItemWithInst) = x.Item
    let GetDeclaredItems (parseResultsOpt: FSharpParseFileResults option, lineStr: string, origLongIdentOpt, colAtEndOfNamesAndResidue, residueOpt, lastDotPos, line, loc, 
                          filterCtors, resolveOverloads, hasTextChangedSinceLastTypecheck, isInRangeOperator, allSymbols: unit -> AssemblySymbol list) =

            // Are the last two chars (except whitespaces) = ".."
            let isLikeRangeOp = 
                match FindFirstNonWhitespacePosition lineStr (colAtEndOfNamesAndResidue - 1) with
                | Some x when x >= 1 && lineStr.[x] = '.' && lineStr.[x - 1] = '.' -> true
                | _ -> false

            // if last two chars are .. and we are not in range operator context - no completion
            if isLikeRangeOp && not isInRangeOperator then None else
                                    
            // Try to use the exact results of name resolution during type checking to generate the results
            // This is based on position (i.e. colAtEndOfNamesAndResidue). This is not used if a residueOpt is given.
            let nameResItems = 
                match residueOpt with 
                | None -> GetPreciseItemsFromNameResolution(line, colAtEndOfNamesAndResidue, None, filterCtors,resolveOverloads, hasTextChangedSinceLastTypecheck)
                | Some residue ->
                    // deals with cases when we have spaces between dot and\or identifier, like A  . $
                    // if this is our case - then we need to locate end position of the name skipping whitespaces
                    // this allows us to handle cases like: let x . $ = 1 
                    match lastDotPos |> Option.orElseWith (fun _ -> FindFirstNonWhitespacePosition lineStr (colAtEndOfNamesAndResidue - 1)) with
                    | Some p when lineStr.[p] = '.' ->
                        match FindFirstNonWhitespacePosition lineStr (p - 1) with
                        | Some colAtEndOfNames ->                 
                           let colAtEndOfNames = colAtEndOfNames + 1 // convert 0-based to 1-based
                           GetPreciseItemsFromNameResolution(line, colAtEndOfNames, Some(residue), filterCtors,resolveOverloads, hasTextChangedSinceLastTypecheck)
                        | None -> NameResResult.Empty
                    | _ -> NameResResult.Empty        
        
            // Normalize to form A.B.C.D where D is the residue. It may be empty for "A.B.C."
            // residueOpt = Some when we are looking for the exact match
            let plid, exactMatchResidueOpt = 
                match origLongIdentOpt, residueOpt with
                | None, _ -> [], None
                | Some(origLongIdent), Some _ -> origLongIdent, None
                | Some(origLongIdent), None ->
                    System.Diagnostics.Debug.Assert(not (isNil origLongIdent), "origLongIdent is empty")
                    // note: as above, this happens when we are called for "precise" resolution - (F1 keyword, data tip etc..)
                    let plid, residue = List.frontAndBack origLongIdent
                    plid, Some residue

            let pos = mkPos line loc
            let (nenv, ad), m = GetBestEnvForPos pos

            let getType() =
                match NameResolution.TryToResolveLongIdentAsType ncenv nenv m plid with
                | Some x -> tryTcrefOfAppTy g x
                | None ->
                    match lastDotPos |> Option.orElseWith (fun _ -> FindFirstNonWhitespacePosition lineStr (colAtEndOfNamesAndResidue - 1)) with
                    | Some p when lineStr.[p] = '.' ->
                        match FindFirstNonWhitespacePosition lineStr (p - 1) with
                        | Some colAtEndOfNames ->                 
                            let colAtEndOfNames = colAtEndOfNames + 1 // convert 0-based to 1-based
                            match TryGetTypeFromNameResolution(line, colAtEndOfNames, residueOpt, resolveOverloads) with
                            | Some x -> tryTcrefOfAppTy g x
                            | _ -> ValueNone
                        | None -> ValueNone
                    | _ -> ValueNone

            match nameResItems with            
            | NameResResult.TypecheckStaleAndTextChanged -> None // second-chance intellisense will try again
            | NameResResult.Cancel(denv,m) -> Some([], denv, m)
            | NameResResult.Members(FilterRelevantItems getItem exactMatchResidueOpt (items, denv, m)) -> 
                // lookup based on name resolution results successful
                Some (items |> List.map (CompletionItem (getType()) ValueNone), denv, m)
            | _ ->
                match origLongIdentOpt with
                | None -> None
                | Some _ -> 
                
                    // Try to use the type of the expression on the left to help generate a completion list
                    let qualItems, thereIsADotInvolved = 
                        match parseResultsOpt with
                        | None -> 
                            // Note, you will get here if the 'reason' is not CompleteWord/MemberSelect/DisplayMemberList, as those are currently the 
                            // only reasons we do a sync parse to have the most precise and likely-to-be-correct-and-up-to-date info.  So for example,
                            // if you do QuickInfo hovering over A in "f(x).A()", you will only get a tip if typechecking has a name-resolution recorded
                            // for A, not if merely we know the capturedExpressionTyping of f(x) and you very recently typed ".A()" - in that case, 
                            // you won't won't get a tip until the typechecking catches back up.
                            GetPreciseCompletionListFromExprTypingsResult.None, false
                        | Some parseResults -> 
                
                        match UntypedParseImpl.TryFindExpressionASTLeftOfDotLeftOfCursor(mkPos line colAtEndOfNamesAndResidue,parseResults.ParseTree) with
                        | Some(pos,_) ->
                            GetPreciseCompletionListFromExprTypings(parseResults, pos, filterCtors, hasTextChangedSinceLastTypecheck), true
                        | None -> 
                            // Can get here in a case like: if "f xxx yyy" is legal, and we do "f xxx y"
                            // We have no interest in expression typings, those are only useful for dot-completion.  We want to fallback
                            // to "Use an environment lookup as the last resort" below
                            GetPreciseCompletionListFromExprTypingsResult.None, false
                
                    match qualItems,thereIsADotInvolved with            
                    | GetPreciseCompletionListFromExprTypingsResult.Some(FilterRelevantItems getItem exactMatchResidueOpt (items, denv, m), ty), _
                            // Initially we only use the expression typings when looking up, e.g. (expr).Nam or (expr).Name1.Nam
                            // These come through as an empty plid and residue "". Otherwise we try an environment lookup
                            // and then return to the qualItems. This is because the expression typings are a little inaccurate, primarily because
                            // it appears we're getting some typings recorded for non-atomic expressions like "f x"
                            when isNil plid ->
                        // lookup based on expression typings successful
                        Some (items |> List.map (CompletionItem (tryTcrefOfAppTy g ty) ValueNone), denv, m)
                    | GetPreciseCompletionListFromExprTypingsResult.NoneBecauseThereWereTypeErrors, _ ->
                        // There was an error, e.g. we have "<expr>." and there is an error determining the type of <expr>  
                        // In this case, we don't want any of the fallback logic, rather, we want to produce zero results.
                        None
                    | GetPreciseCompletionListFromExprTypingsResult.NoneBecauseTypecheckIsStaleAndTextChanged, _ ->         
                        // we want to report no result and let second-chance intellisense kick in
                        None
                    | _, true when isNil plid ->
                        // If the user just pressed '.' after an _expression_ (not a plid), it is never right to show environment-lookup top-level completions.
                        // The user might by typing quickly, and the LS didn't have an expression type right before the dot yet.
                        // Second-chance intellisense will bring up the correct list in a moment.
                        None
                    | _ ->         
                       // Use an environment lookup as the last resort
                       let envItems, denv, m = GetEnvironmentLookupResolutions(nenv, ad, m, plid, filterCtors, residueOpt.IsSome)
                       
                       let envResult =
                           match nameResItems, (envItems, denv, m), qualItems with
                           
                           // First, use unfiltered name resolution items, if they're not empty
                           | NameResResult.Members(items, denv, m), _, _ when not (isNil items) -> 
                               // lookup based on name resolution results successful
                               ValueSome(items |> List.map (CompletionItem (getType()) ValueNone), denv, m)                
                           
                           // If we have nonempty items from environment that were resolved from a type, then use them... 
                           // (that's better than the next case - here we'd return 'int' as a type)
                           | _, FilterRelevantItems getItem exactMatchResidueOpt (items, denv, m), _ when not (isNil items) ->
                               // lookup based on name and environment successful
                               ValueSome(items |> List.map (CompletionItem (getType()) ValueNone), denv, m)
                           
                           // Try again with the qualItems
                           | _, _, GetPreciseCompletionListFromExprTypingsResult.Some(FilterRelevantItems getItem exactMatchResidueOpt (items, denv, m), ty) ->
                               ValueSome(items |> List.map (CompletionItem (tryTcrefOfAppTy g ty) ValueNone), denv, m)
                           
                           | _ -> ValueNone

                       let globalResult =
                           match origLongIdentOpt with
                           | None | Some [] ->
                               let globalItems = 
                                   allSymbols() 
                                   |> List.filter (fun x ->
                                        not x.Symbol.IsExplicitlySuppressed &&

                                        match x.Symbol with
                                        | :? FSharpMemberOrFunctionOrValue as m when m.IsConstructor && filterCtors = ResolveTypeNamesToTypeRefs -> false 
                                        | _ -> true)
                                   
                               let getItem (x: AssemblySymbol) = x.Symbol.Item
                               
                               match globalItems, denv, m with
                               | FilterRelevantItems getItem exactMatchResidueOpt (globalItemsFiltered, denv, m) when not (isNil globalItemsFiltered) ->
                                   globalItemsFiltered 
                                   |> List.map(fun globalItem -> CompletionItem (getType()) (ValueSome globalItem) (ItemWithNoInst globalItem.Symbol.Item))
                                   |> fun r -> ValueSome(r, denv, m)
                               | _ -> ValueNone
                           | _ -> ValueNone // do not return unresolved items after dot

                       match envResult, globalResult with
                       | ValueSome (items, denv, m), ValueSome (gItems,_,_) -> Some (items @ gItems, denv, m)
                       | ValueSome x, ValueNone -> Some x
                       | ValueNone, ValueSome y -> Some y
                       | ValueNone, ValueNone -> None


    let toCompletionItems (items: ItemWithInst list, denv: DisplayEnv, m: range ) =
        items |> List.map DefaultCompletionItem, denv, m

    /// Get the auto-complete items at a particular location.
    let GetDeclItemsForNamesAtPosition(ctok: CompilationThreadToken, parseResultsOpt: FSharpParseFileResults option, origLongIdentOpt: string list option, 
                                       residueOpt:string option, lastDotPos: int option, line:int, lineStr:string, colAtEndOfNamesAndResidue, filterCtors, resolveOverloads, 
                                       getAllSymbols: unit -> AssemblySymbol list, hasTextChangedSinceLastTypecheck: (obj * range -> bool)) 
                                       : (CompletionItem list * DisplayEnv * CompletionContext option * range) option = 
        RequireCompilationThread ctok // the operations in this method need the reactor thread

        let loc = 
            match colAtEndOfNamesAndResidue with
            | pastEndOfLine when pastEndOfLine >= lineStr.Length -> lineStr.Length
            | atDot when lineStr.[atDot] = '.' -> atDot + 1
            | atStart when atStart = 0 -> 0
            | otherwise -> otherwise - 1

        // Look for a "special" completion context
        let completionContext = 
            parseResultsOpt 
            |> Option.bind (fun x -> x.ParseTree)
            |> Option.bind (fun parseTree -> UntypedParseImpl.TryGetCompletionContext(mkPos line colAtEndOfNamesAndResidue, parseTree, lineStr))
        
        let res =
            match completionContext with
            // Invalid completion locations
            | Some CompletionContext.Invalid -> None
            
            // Completion at 'inherit C(...)"
            | Some (CompletionContext.Inherit(InheritanceContext.Class, (plid, _))) ->
                GetEnvironmentLookupResolutionsAtPosition(mkPos line loc, plid, filterCtors, false)
                |> FilterRelevantItemsBy getItem None (getItem >> GetBaseClassCandidates)
                |> Option.map toCompletionItems
             
            // Completion at 'interface ..."
            | Some (CompletionContext.Inherit(InheritanceContext.Interface, (plid, _))) ->
                GetEnvironmentLookupResolutionsAtPosition(mkPos line loc, plid, filterCtors, false)
                |> FilterRelevantItemsBy getItem None (getItem >> GetInterfaceCandidates)
                |> Option.map toCompletionItems
            
            // Completion at 'implement ..."
            | Some (CompletionContext.Inherit(InheritanceContext.Unknown, (plid, _))) ->
                GetEnvironmentLookupResolutionsAtPosition(mkPos line loc, plid, filterCtors, false) 
                |> FilterRelevantItemsBy getItem None (getItem >> (fun t -> GetBaseClassCandidates t || GetInterfaceCandidates t))
                |> Option.map toCompletionItems
            
            // Completion at ' { XXX = ... } "
            | Some(CompletionContext.RecordField(RecordContext.New(plid, _))) ->
                // { x. } can be either record construction or computation expression. Try to get all visible record fields first
                match GetClassOrRecordFieldsEnvironmentLookupResolutions(mkPos line loc, plid) |> toCompletionItems with
                | [],_,_ -> 
                    // no record fields found, return completion list as if we were outside any computation expression
                    GetDeclaredItems (parseResultsOpt, lineStr, origLongIdentOpt, colAtEndOfNamesAndResidue, residueOpt, lastDotPos, line, loc, filterCtors,resolveOverloads, hasTextChangedSinceLastTypecheck, false, fun() -> [])
                | result -> Some(result)
            
            // Completion at ' { XXX = ... with ... } "
            | Some(CompletionContext.RecordField(RecordContext.CopyOnUpdate(r, (plid, _)))) -> 
                match GetRecdFieldsForExpr(r) with
                | None -> 
                    Some (GetClassOrRecordFieldsEnvironmentLookupResolutions(mkPos line loc, plid))
                    |> Option.map toCompletionItems
                | Some (items, denv, m) -> 
                    Some (List.map ItemWithNoInst items, denv, m) 
                    |> Option.map toCompletionItems
            
            // Completion at ' { XXX = ... with ... } "
            | Some(CompletionContext.RecordField(RecordContext.Constructor(typeName))) ->
                Some(GetClassOrRecordFieldsEnvironmentLookupResolutions(mkPos line loc, [typeName]))
                |> Option.map toCompletionItems
            
            // Completion at ' SomeMethod( ... ) ' with named arguments 
            | Some(CompletionContext.ParameterList (endPos, fields)) ->
                let results = GetNamedParametersAndSettableFields endPos hasTextChangedSinceLastTypecheck
            
                let declaredItems = 
                    GetDeclaredItems (parseResultsOpt, lineStr, origLongIdentOpt, colAtEndOfNamesAndResidue, residueOpt, lastDotPos, line, loc, filterCtors, resolveOverloads, 
                                      hasTextChangedSinceLastTypecheck, false, getAllSymbols)
            
                match results with
                | NameResResult.Members(items, denv, m) -> 
                    let filtered = 
                        items 
                        |> RemoveDuplicateItems g
                        |> RemoveExplicitlySuppressed g
                        |> List.filter (fun item -> not (fields.Contains item.Item.DisplayName))
                        |> List.map (fun item -> 
                            { ItemWithInst = item
                              Kind = CompletionItemKind.Argument
                              MinorPriority = 0
                              IsOwnMember = false
                              Type = None 
                              Unresolved = None })
                    match declaredItems with
                    | None -> Some (toCompletionItems (items, denv, m))
                    | Some (declItems, declaredDisplayEnv, declaredRange) -> Some (filtered @ declItems, declaredDisplayEnv, declaredRange)
                | _ -> declaredItems
            
            | Some(CompletionContext.AttributeApplication) ->
                GetDeclaredItems (parseResultsOpt, lineStr, origLongIdentOpt, colAtEndOfNamesAndResidue, residueOpt, lastDotPos, line, loc, filterCtors, resolveOverloads, hasTextChangedSinceLastTypecheck, false, getAllSymbols)
                |> Option.map (fun (items, denv, m) -> 
                     items 
                     |> List.filter (fun cItem ->
                         match cItem.Item with
                         | Item.ModuleOrNamespaces _ -> true
                         | _ when IsAttribute infoReader cItem.Item -> true
                         | _ -> false), denv, m)
            
            | Some(CompletionContext.OpenDeclaration) ->
                GetDeclaredItems (parseResultsOpt, lineStr, origLongIdentOpt, colAtEndOfNamesAndResidue, residueOpt, lastDotPos, line, loc, filterCtors, resolveOverloads, hasTextChangedSinceLastTypecheck, false, getAllSymbols)
                |> Option.map (fun (items, denv, m) ->
                    items 
                    |> List.filter (fun x ->
                        match x.Item with
                        | Item.ModuleOrNamespaces _ -> true
                        | Item.Types (_, tcrefs) when tcrefs |> List.exists (fun ty -> isAppTy g ty && isStaticClass g (tcrefOfAppTy g ty)) -> true
                        | _ -> false), denv, m)
            
            // Completion at '(x: ...)"
            | Some (CompletionContext.PatternType) ->
                GetDeclaredItems (parseResultsOpt, lineStr, origLongIdentOpt, colAtEndOfNamesAndResidue, residueOpt, lastDotPos, line, loc, filterCtors, resolveOverloads, hasTextChangedSinceLastTypecheck, false, getAllSymbols)
                |> Option.map (fun (items, denv, m) ->
                     items 
                     |> List.filter (fun cItem ->
                         match cItem.Item with
                         | Item.ModuleOrNamespaces _
                         | Item.Types _
                         | Item.UnqualifiedType _
                         | Item.ExnCase _ -> true
                         | _ -> false), denv, m)

            // Other completions
            | cc ->
                match residueOpt |> Option.bind Seq.tryHead with
                | Some ''' ->
                    // The last token in 
                    //    let x = 'E
                    // is Ident with text "'E", however it's either unfinished char literal or generic parameter. 
                    // We should not provide any completion in the former case, and we don't provide it for the latter one for now
                    // because providing generic parameters list is context aware, which we don't have here (yet).
                    None
                | _ ->
                    let isInRangeOperator = (match cc with Some (CompletionContext.RangeOperator) -> true | _ -> false)
                    GetDeclaredItems (parseResultsOpt, lineStr, origLongIdentOpt, colAtEndOfNamesAndResidue,
                        residueOpt, lastDotPos, line, loc, filterCtors, resolveOverloads,
                        hasTextChangedSinceLastTypecheck, isInRangeOperator, getAllSymbols)
        
        res |> Option.map (fun (items, denv, m) -> items, denv, completionContext, m)

    /// Return 'false' if this is not a completion item valid in an interface file.
    let IsValidSignatureFileItem item =
        match item with
        | Item.Types _ | Item.ModuleOrNamespaces _ -> true
        | _ -> false
    
    /// Find the most precise display context for the given line and column.
    member __.GetBestDisplayEnvForPos cursorPos  = GetBestEnvForPos cursorPos

    member __.GetVisibleNamespacesAndModulesAtPosition(cursorPos: pos) : ModuleOrNamespaceRef list =
        let (nenv, ad), m = GetBestEnvForPos cursorPos
        NameResolution.GetVisibleNamespacesAndModulesAtPoint ncenv nenv m ad

    /// Determines if a long ident is resolvable at a specific point.
    member __.IsRelativeNameResolvable(cursorPos: pos, plid: string list, item: Item) : bool =
        ErrorScope.Protect
            Range.range0
            (fun () ->
                /// Find items in the best naming environment.
                let (nenv, ad), m = GetBestEnvForPos cursorPos
                NameResolution.IsItemResolvable ncenv nenv m ad plid item)
            (fun msg -> 
                Trace.TraceInformation(sprintf "FCS: recovering from error in IsRelativeNameResolvable: '%s'" msg)
                false)

    /// Determines if a long ident is resolvable at a specific point.
    member scope.IsRelativeNameResolvableFromSymbol(cursorPos: pos, plid: string list, symbol: FSharpSymbol) : bool =
        scope.IsRelativeNameResolvable(cursorPos, plid, symbol.Item)
        
    /// Get the auto-complete items at a location
    member __.GetDeclarations (ctok, parseResultsOpt, line, lineStr, partialName, getAllEntities, hasTextChangedSinceLastTypecheck) =
        let isInterfaceFile = SourceFileImpl.IsInterfaceFile mainInputFileName
        ErrorScope.Protect Range.range0 
            (fun () ->

                let declItemsOpt =
                    GetDeclItemsForNamesAtPosition(ctok, parseResultsOpt, Some partialName.QualifyingIdents,
                        Some partialName.PartialIdent, partialName.LastDotPos, line,
                        lineStr, partialName.EndColumn + 1, ResolveTypeNamesToCtors, ResolveOverloads.Yes,
                        getAllEntities, hasTextChangedSinceLastTypecheck)

                match declItemsOpt with
                | None -> FSharpDeclarationListInfo.Empty  
                | Some (items, denv, ctx, m) ->
                    let items = if isInterfaceFile then items |> List.filter (fun x -> IsValidSignatureFileItem x.Item) else items
                    let getAccessibility item = FSharpSymbol.GetAccessibility (FSharpSymbol.Create(cenv, item))
                    let currentNamespaceOrModule =
                        parseResultsOpt
                        |> Option.bind (fun x -> x.ParseTree)
                        |> Option.map (fun parsedInput -> UntypedParseImpl.GetFullNameOfSmallestModuleOrNamespaceAtPoint(parsedInput, mkPos line 0))
                    let isAttributeApplication = ctx = Some CompletionContext.AttributeApplication
                    FSharpDeclarationListInfo.Create(infoReader,m,denv,getAccessibility,items,reactorOps,currentNamespaceOrModule,isAttributeApplication))
            (fun msg -> 
                Trace.TraceInformation(sprintf "FCS: recovering from error in GetDeclarations: '%s'" msg)
                FSharpDeclarationListInfo.Error msg)

    /// Get the symbols for auto-complete items at a location
    member __.GetDeclarationListSymbols (ctok, parseResultsOpt, line, lineStr, partialName, getAllEntities, hasTextChangedSinceLastTypecheck) =
        let isInterfaceFile = SourceFileImpl.IsInterfaceFile mainInputFileName
        ErrorScope.Protect Range.range0 
            (fun () -> 

                let declItemsOpt =
                    GetDeclItemsForNamesAtPosition(ctok, parseResultsOpt, Some partialName.QualifyingIdents,
                        Some partialName.PartialIdent, partialName.LastDotPos, line, lineStr,
                        partialName.EndColumn + 1, ResolveTypeNamesToCtors, ResolveOverloads.Yes,
                        getAllEntities, hasTextChangedSinceLastTypecheck)

                match declItemsOpt with
                | None -> List.Empty  
                | Some (items, denv, _, m) -> 
                    let items = if isInterfaceFile then items |> List.filter (fun x -> IsValidSignatureFileItem x.Item) else items

                    //do filtering like Declarationset
                    let items = items |> RemoveExplicitlySuppressedCompletionItems g
                    
                    // Sort by name. For things with the same name, 
                    //     - show types with fewer generic parameters first
                    //     - show types before over other related items - they usually have very useful XmlDocs 
                    let items = 
                        items |> List.sortBy (fun d ->
                            let n = 
                                match d.Item with  
                                | Item.Types (_,(TType_app(tcref,_) :: _)) -> 1 + tcref.TyparsNoRange.Length
                                // Put delegate ctors after types, sorted by #typars. RemoveDuplicateItems will remove FakeInterfaceCtor and DelegateCtor if an earlier type is also reported with this name
                                | Item.FakeInterfaceCtor (TType_app(tcref,_)) 
                                | Item.DelegateCtor (TType_app(tcref,_)) -> 1000 + tcref.TyparsNoRange.Length
                                // Put type ctors after types, sorted by #typars. RemoveDuplicateItems will remove DefaultStructCtors if a type is also reported with this name
                                | Item.CtorGroup (_, (cinfo :: _)) -> 1000 + 10 * cinfo.DeclaringTyconRef.TyparsNoRange.Length 
                                | _ -> 0
                            (d.Item.DisplayName, n))

                    // Remove all duplicates. We've put the types first, so this removes the DelegateCtor and DefaultStructCtor's.
                    let items = items |> RemoveDuplicateCompletionItems g

                    // Group by compiled name for types, display name for functions
                    // (We don't want types with the same display name to be grouped as overloads)
                    let items =
                        items |> List.groupBy (fun d ->
                            match d.Item with  
                            | Item.Types (_,(TType_app(tcref,_) :: _))
                            | Item.ExnCase tcref -> tcref.LogicalName
                            | Item.UnqualifiedType(tcref :: _)
                            | Item.FakeInterfaceCtor (TType_app(tcref,_)) 
                            | Item.DelegateCtor (TType_app(tcref,_)) -> tcref.CompiledName
                            | Item.CtorGroup (_, (cinfo :: _)) ->
                                cinfo.ApparentEnclosingTyconRef.CompiledName
                            | _ -> d.Item.DisplayName)

                    // Filter out operators (and list)
                    let items = 
                        // Check whether this item looks like an operator.
                        let isOpItem(nm, item: CompletionItem list) = 
                            match item |> List.map (fun x -> x.Item) with 
                            | [Item.Value _]
                            | [Item.MethodGroup(_,[_],_)] -> IsOperatorName nm
                            | [Item.UnionCase _] -> IsOperatorName nm
                            | _ -> false              

                        let isFSharpList nm = (nm = "[]") // list shows up as a Type and a UnionCase, only such entity with a symbolic name, but want to filter out of intellisense

                        items |> List.filter (fun (nm,items) -> not (isOpItem(nm,items)) && not(isFSharpList nm)) 

                    let items = 
                        // Filter out duplicate names
                        items |> List.map (fun (_nm,itemsWithSameName) -> 
                            match itemsWithSameName with
                            | [] -> failwith "Unexpected empty bag"
                            | items ->
                                items 
                                |> List.map (fun item -> let symbol = FSharpSymbol.Create(cenv, item.Item)
                                                         FSharpSymbolUse(g, denv, symbol, ItemOccurence.Use, m)))

                    //end filtering
                    items)
            (fun msg -> 
                Trace.TraceInformation(sprintf "FCS: recovering from error in GetDeclarationListSymbols: '%s'" msg)
                [])
            
    /// Get the "reference resolution" tooltip for at a location
    member __.GetReferenceResolutionStructuredToolTipText(ctok, line,col) = 

        RequireCompilationThread ctok // the operations in this method need the reactor thread but the reasons why are not yet grounded

        let pos = mkPos line col
        let isPosMatch(pos, ar:AssemblyReference) : bool = 
            let isRangeMatch = (Range.rangeContainsPos ar.Range pos) 
            let isNotSpecialRange = not (Range.equals ar.Range rangeStartup) && not (Range.equals ar.Range range0) && not (Range.equals ar.Range rangeCmdArgs)
            let isMatch = isRangeMatch && isNotSpecialRange
            isMatch      
        
        let dataTipOfReferences() = 
            let matches =
                match loadClosure with
                | None -> []
                | Some(loadClosure) -> 
                    loadClosure.References
                        |> List.map snd
                        |> List.concat 
                        |> List.filter(fun ar->isPosMatch(pos, ar.originalReference))

            match matches with 
            | resolved::_ // Take the first seen
            | [resolved] -> 
                let tip = wordL (TaggedTextOps.tagStringLiteral((resolved.prepareToolTip ()).TrimEnd([|'\n'|])))
                FSharpStructuredToolTipText.FSharpToolTipText [FSharpStructuredToolTipElement.Single(tip, FSharpXmlDoc.None)]

            | [] -> FSharpStructuredToolTipText.FSharpToolTipText []
                                    
        ErrorScope.Protect Range.range0 
            dataTipOfReferences
            (fun err -> 
                Trace.TraceInformation(sprintf "FCS: recovering from error in GetReferenceResolutionStructuredToolTipText: '%s'" err)
                FSharpToolTipText [FSharpStructuredToolTipElement.CompositionError err])

    // GetToolTipText: return the "pop up" (or "Quick Info") text given a certain context.
    member __.GetStructuredToolTipText(ctok, line, lineStr, colAtEndOfNames, names) = 
        let Compute() = 
            ErrorScope.Protect Range.range0 
                (fun () -> 
                    let declItemsOpt =
                        GetDeclItemsForNamesAtPosition(ctok, None, Some names, None, None,
                            line, lineStr, colAtEndOfNames, ResolveTypeNamesToCtors,
                            ResolveOverloads.Yes, (fun() -> []), (fun _ -> false))

                    match declItemsOpt with
                    | None -> FSharpToolTipText []
                    | Some(items, denv, _, m) ->
                         FSharpToolTipText(items |> List.map (fun x -> FormatStructuredDescriptionOfItem false infoReader m denv x.ItemWithInst)))

                (fun err -> 
                    Trace.TraceInformation(sprintf "FCS: recovering from error in GetStructuredToolTipText: '%s'" err)
                    FSharpToolTipText [FSharpStructuredToolTipElement.CompositionError err])
               
        // See devdiv bug 646520 for rationale behind truncating and caching these quick infos (they can be big!)
        let key = line,colAtEndOfNames,lineStr
        match getToolTipTextCache.TryGet (ctok, key) with 
        | Some res -> res
        | None ->
             let res = Compute()
             getToolTipTextCache.Put(ctok, key,res)
             res

    member __.GetF1Keyword (ctok, line, lineStr, colAtEndOfNames, names) : string option =
        ErrorScope.Protect Range.range0
            (fun () ->

                let declItemsOpt =
                    GetDeclItemsForNamesAtPosition(ctok, None, Some names, None, None,
                        line, lineStr, colAtEndOfNames, ResolveTypeNamesToCtors,
                        ResolveOverloads.No, (fun() -> []), (fun _ -> false))

                match declItemsOpt with 
                | None -> None
                | Some (items: CompletionItem list, _,_, _) ->
                    match items with
                    | [] -> None
                    | [item] ->
                        GetF1Keyword g item.Item                       
                    | _ ->
                        // handle new Type()
                        let allTypes, constr, ty =
                            List.fold 
                                (fun (allTypes,constr,ty) (item: CompletionItem) ->
                                    match item.Item, constr, ty with
                                    |   (Item.Types _) as t, _, None  -> allTypes, constr, Some t
                                    |   (Item.Types _), _, _ -> allTypes, constr, ty
                                    |   (Item.CtorGroup _), None, _ -> allTypes, Some item.Item, ty
                                    |   _ -> false, None, None) 
                                (true,None,None) items
                        match allTypes, constr, ty with
                        |   true, Some (Item.CtorGroup(_, _) as item), _    
                                -> GetF1Keyword g item                        
                        |   true, _, Some ty
                                -> GetF1Keyword g ty
                        |   _ -> None
            )    
            (fun msg -> 
                Trace.TraceInformation(sprintf "FCS: recovering from error in GetF1Keyword: '%s'" msg)
                None)

    member __.GetMethods (ctok, line, lineStr, colAtEndOfNames, namesOpt) =
        ErrorScope.Protect Range.range0
            (fun () -> 

                let declItemsOpt =
                    GetDeclItemsForNamesAtPosition(ctok, None, namesOpt, None, None,
                        line, lineStr, colAtEndOfNames, ResolveTypeNamesToCtors,
                        ResolveOverloads.No, (fun() -> []), (fun _ -> false))

                match declItemsOpt with
                | None -> FSharpMethodGroup("",[| |])
                | Some (items, denv, _, m) -> 
                    // GetDeclItemsForNamesAtPosition returns Items.Types and Item.CtorGroup for `new T(|)`, 
                    // the Item.Types is not needed here as it duplicates (at best) parameterless ctor.
                    let ctors = items |> List.filter (fun x -> match x.Item with Item.CtorGroup _ -> true | _ -> false)
                    let items =
                        match ctors with
                        | [] -> items
                        | ctors -> ctors
                    FSharpMethodGroup.Create(infoReader, m, denv, items |> List.map (fun x -> x.ItemWithInst)))
            (fun msg -> 
                Trace.TraceInformation(sprintf "FCS: recovering from error in GetMethods: '%s'" msg)
                FSharpMethodGroup(msg,[| |]))

    member __.GetMethodsAsSymbols (ctok, line, lineStr, colAtEndOfNames, names) =
        ErrorScope.Protect Range.range0
            (fun () -> 
                let declItemsOpt =
                    GetDeclItemsForNamesAtPosition (ctok, None, Some names, None,
                        None, line, lineStr, colAtEndOfNames,
                        ResolveTypeNamesToCtors, ResolveOverloads.No,
                        (fun() -> []), (fun _ -> false))

                match declItemsOpt with
                | None | Some ([],_,_,_) -> None
                | Some (items, denv, _, m) ->
                    let allItems = items |> List.collect (fun item -> SymbolHelpers.FlattenItems g m item.Item)
                    let symbols = allItems |> List.map (fun item -> FSharpSymbol.Create(cenv, item))
                    Some (symbols, denv, m)
            )
            (fun msg -> 
                Trace.TraceInformation(sprintf "FCS: recovering from error in GetMethodsAsSymbols: '%s'" msg)
                None)
           
    member __.GetDeclarationLocation (ctok, line, lineStr, colAtEndOfNames, names, preferFlag) =
        ErrorScope.Protect Range.range0 
            (fun () -> 
                
                let declItemsOpt =
                    GetDeclItemsForNamesAtPosition (ctok, None, Some names, None, None,
                        line, lineStr, colAtEndOfNames, ResolveTypeNamesToCtors,
                        ResolveOverloads.Yes, (fun() -> []), (fun _ -> false))

                match declItemsOpt with
                | None
                | Some ([], _, _, _) -> FSharpFindDeclResult.DeclNotFound (FSharpFindDeclFailureReason.Unknown "")
                | Some (item :: _, _, _, _) ->
                let getTypeVarNames (ilinfo: ILMethInfo) =
                    let classTypeParams = ilinfo.DeclaringTyconRef.ILTyconRawMetadata.GenericParams |> List.map (fun paramDef -> paramDef.Name)
                    let methodTypeParams = ilinfo.FormalMethodTypars |> List.map (fun ty -> ty.Name)
                    classTypeParams @ methodTypeParams |> Array.ofList

                let result =
                    match item.Item with
                    | Item.CtorGroup (_, (ILMeth (_,ilinfo,_)) :: _) ->
                        match ilinfo.MetadataScope with
                        | ILScopeRef.Assembly assemblyRef ->
                            let typeVarNames = getTypeVarNames ilinfo
                            ParamTypeSymbol.tryOfILTypes typeVarNames ilinfo.ILMethodRef.ArgTypes
                            |> Option.map (fun args ->
                                let externalSym = ExternalSymbol.Constructor (ilinfo.ILMethodRef.DeclaringTypeRef.FullName, args)
                                FSharpFindDeclResult.ExternalDecl (assemblyRef.Name, externalSym))
                        | _ -> None

                    | Item.MethodGroup (name, (ILMeth (_,ilinfo,_)) :: _, _) ->
                        match ilinfo.MetadataScope with
                        | ILScopeRef.Assembly assemblyRef ->
                            let typeVarNames = getTypeVarNames ilinfo
                            ParamTypeSymbol.tryOfILTypes typeVarNames ilinfo.ILMethodRef.ArgTypes
                            |> Option.map (fun args ->
                                let externalSym = ExternalSymbol.Method (ilinfo.ILMethodRef.DeclaringTypeRef.FullName, name, args, ilinfo.ILMethodRef.GenericArity)
                                FSharpFindDeclResult.ExternalDecl (assemblyRef.Name, externalSym))
                        | _ -> None

                    | Item.Property (name, ILProp propInfo :: _) ->
                        let methInfo = 
                            if propInfo.HasGetter then Some propInfo.GetterMethod
                            elif propInfo.HasSetter then Some propInfo.SetterMethod
                            else None
                      
                        match methInfo with
                        | Some methInfo ->
                            match methInfo.MetadataScope with
                            | ILScopeRef.Assembly assemblyRef ->
                                let externalSym = ExternalSymbol.Property (methInfo.ILMethodRef.DeclaringTypeRef.FullName, name)
                                Some (FSharpFindDeclResult.ExternalDecl (assemblyRef.Name, externalSym))
                            | _ -> None
                        | None -> None
                  
                    | Item.ILField (ILFieldInfo (typeInfo, fieldDef)) when not typeInfo.TyconRefOfRawMetadata.IsLocalRef ->
                        match typeInfo.ILScopeRef with
                        | ILScopeRef.Assembly assemblyRef ->
                            let externalSym = ExternalSymbol.Field (typeInfo.ILTypeRef.FullName, fieldDef.Name)
                            Some (FSharpFindDeclResult.ExternalDecl (assemblyRef.Name, externalSym))
                        | _ -> None
                  
                    | Item.Event (ILEvent (ILEventInfo (typeInfo, eventDef))) when not typeInfo.TyconRefOfRawMetadata.IsLocalRef ->
                        match typeInfo.ILScopeRef with
                        | ILScopeRef.Assembly assemblyRef ->
                            let externalSym = ExternalSymbol.Event (typeInfo.ILTypeRef.FullName, eventDef.Name)
                            Some (FSharpFindDeclResult.ExternalDecl (assemblyRef.Name, externalSym))
                        | _ -> None

                    | Item.ImplicitOp(_, {contents = Some(TraitConstraintSln.FSMethSln(_, _vref, _))}) ->
                        //Item.Value(vref)
                        None

                    | Item.Types (_, TType_app (tr, _) :: _) when tr.IsLocalRef && tr.IsTypeAbbrev -> None

                    | Item.Types (_, [ AppTy g (tr, _) ]) when not tr.IsLocalRef ->
                        match tr.TypeReprInfo, tr.PublicPath with
                        | TILObjectRepr(TILObjectReprData (ILScopeRef.Assembly assemblyRef, _, _)), Some (PubPath parts) ->
                            let fullName = parts |> String.concat "."
                            Some (FSharpFindDeclResult.ExternalDecl (assemblyRef.Name, ExternalSymbol.Type fullName))
                        | _ -> None
                    | _ -> None
                match result with
                | Some x -> x
                | None   ->
                match rangeOfItem g preferFlag item.Item with
                | Some itemRange -> 
                    let projectDir = Filename.directoryName (if projectFileName = "" then mainInputFileName else projectFileName)
                    let range = fileNameOfItem g (Some projectDir) itemRange item.Item
                    mkRange range itemRange.Start itemRange.End              
                    |> FSharpFindDeclResult.DeclFound
                | None -> 
                    match item.Item with 
#if !NO_EXTENSIONTYPING
// provided items may have TypeProviderDefinitionLocationAttribute that binds them to some location
                    | Item.CtorGroup  (name, ProvidedMeth (_)::_   )
                    | Item.MethodGroup(name, ProvidedMeth (_)::_, _)
                    | Item.Property   (name, ProvidedProp (_)::_   ) -> FSharpFindDeclFailureReason.ProvidedMember name             
                    | Item.Event      (      ProvidedEvent(_) as e ) -> FSharpFindDeclFailureReason.ProvidedMember e.EventName        
                    | Item.ILField    (      ProvidedField(_) as f ) -> FSharpFindDeclFailureReason.ProvidedMember f.FieldName        
                    | SymbolHelpers.ItemIsProvidedType g (tcref)     -> FSharpFindDeclFailureReason.ProvidedType   tcref.DisplayName
#endif
                    | _                                              -> FSharpFindDeclFailureReason.Unknown ""                      
                    |> FSharpFindDeclResult.DeclNotFound
            )
            (fun msg -> 
                Trace.TraceInformation(sprintf "FCS: recovering from error in GetDeclarationLocation: '%s'" msg)
                FSharpFindDeclResult.DeclNotFound (FSharpFindDeclFailureReason.Unknown msg))

    member __.GetSymbolUseAtLocation (ctok, line, lineStr, colAtEndOfNames, names) =
        ErrorScope.Protect Range.range0 
            (fun () -> 
                let declItemsOpt =
                    GetDeclItemsForNamesAtPosition (ctok, None, Some names, None, None,
                        line, lineStr, colAtEndOfNames, ResolveTypeNamesToCtors,
                        ResolveOverloads.Yes, (fun() -> []), (fun _ -> false))

                match declItemsOpt with
                | None | Some ([], _, _, _) -> None
                | Some (item :: _, denv, _, m) -> 
                    let symbol = FSharpSymbol.Create(cenv, item.Item)
                    Some (symbol, denv, m)
            ) 
            (fun msg -> 
                Trace.TraceInformation(sprintf "FCS: recovering from error in GetSymbolUseAtLocation: '%s'" msg)
                None)

    member __.PartialAssemblySignatureForFile = 
        FSharpAssemblySignature(g, thisCcu, ccuSigForFile, tcImports, None, ccuSigForFile)

    member __.AccessRights =  tcAccessRights

    member __.GetReferencedAssemblies() = 
        [ for x in tcImports.GetImportedAssemblies() do 
                yield FSharpAssembly(g, tcImports, x.FSharpViewOfMetadata) ]

    member __.GetFormatSpecifierLocationsAndArity() = 
         sSymbolUses.GetFormatSpecifierLocationsAndArity()

    member __.GetSemanticClassification(range: range option) : struct (range * SemanticClassificationType) [] =
        sResolutions.GetSemanticClassification(g, amap, sSymbolUses.GetFormatSpecifierLocationsAndArity(), range)

    /// The resolutions in the file
    member __.ScopeResolutions = sResolutions

    /// The uses of symbols in the analyzed file
    member __.ScopeSymbolUses = sSymbolUses

    member __.TcGlobals = g

    member __.TcImports = tcImports

    /// The inferred signature of the file
    member __.CcuSigForFile = ccuSigForFile

    /// The assembly being analyzed
    member __.ThisCcu = thisCcu

    member __.ImplementationFile = implFileOpt

    /// All open declarations in the file, including auto open modules
    member __.OpenDeclarations = openDeclarations

    member __.SymbolEnv = cenv

    override __.ToString() = "TypeCheckInfo(" + mainInputFileName + ")"

type FSharpParsingOptions =
    { SourceFiles: string []
      ConditionalCompilationDefines: string list
      ErrorSeverityOptions: FSharpErrorSeverityOptions
      IsInteractive: bool
      LightSyntax: bool option
      CompilingFsLib: bool
      IsExe: bool }

    member x.LastFileName =
        Debug.Assert(not (Array.isEmpty x.SourceFiles), "Parsing options don't contain any file")
        Array.last x.SourceFiles

    static member Default =
        { SourceFiles = Array.empty
          ConditionalCompilationDefines = []
          ErrorSeverityOptions = FSharpErrorSeverityOptions.Default
          IsInteractive = false
          LightSyntax = None
          CompilingFsLib = false
          IsExe = false }

    static member FromTcConfig(tcConfig: TcConfig, sourceFiles, isInteractive: bool) =
        { SourceFiles = sourceFiles
          ConditionalCompilationDefines = tcConfig.conditionalCompilationDefines
          ErrorSeverityOptions = tcConfig.errorSeverityOptions
          IsInteractive = isInteractive
          LightSyntax = tcConfig.light
          CompilingFsLib = tcConfig.compilingFslib
          IsExe = tcConfig.target.IsExe }

    static member FromTcConfigBuilder(tcConfigB: TcConfigBuilder, sourceFiles, isInteractive: bool) =
        {
          SourceFiles = sourceFiles
          ConditionalCompilationDefines = tcConfigB.conditionalCompilationDefines
          ErrorSeverityOptions = tcConfigB.errorSeverityOptions
          IsInteractive = isInteractive
          LightSyntax = tcConfigB.light
          CompilingFsLib = tcConfigB.compilingFslib
          IsExe = tcConfigB.target.IsExe
        }

module internal ParseAndCheckFile = 

    /// Error handler for parsing & type checking while processing a single file
    type ErrorHandler(reportErrors, mainInputFileName, errorSeverityOptions: FSharpErrorSeverityOptions, sourceText: ISourceText, suggestNamesForErrors: bool) =
        let mutable options = errorSeverityOptions
        let errorsAndWarningsCollector = new ResizeArray<_>()
        let mutable errorCount = 0

        // We'll need number of lines for adjusting error messages at EOF
        let fileInfo = sourceText.GetLastCharacterPosition()

        // This function gets called whenever an error happens during parsing or checking
        let diagnosticSink sev (exn: PhasedDiagnostic) =
            // Sanity check here. The phase of an error should be in a phase known to the language service.
            let exn =
                if not(exn.IsPhaseInCompile()) then
                    // Reaching this point means that the error would be sticky if we let it prop up to the language service.
                    // Assert and recover by replacing phase with one known to the language service.
                    Trace.TraceInformation(sprintf "The subcategory '%s' seen in an error should not be seen by the language service" (exn.Subcategory()))
                    { exn with Phase = BuildPhase.TypeCheck }
                else exn
            if reportErrors then
                let report exn =
                    for ei in ErrorHelpers.ReportError (options, false, mainInputFileName, fileInfo, (exn, sev), suggestNamesForErrors) do
                        errorsAndWarningsCollector.Add ei
                        if sev = FSharpErrorSeverity.Error then
                            errorCount <- errorCount + 1

                match exn with
#if !NO_EXTENSIONTYPING
                | { Exception = (:? TypeProviderError as tpe) } -> tpe.Iter(fun e -> report { exn with Exception = e })
#endif
                | e -> report e

        let errorLogger =
            { new ErrorLogger("ErrorHandler") with
                member x.DiagnosticSink (exn, isError) = diagnosticSink (if isError then FSharpErrorSeverity.Error else FSharpErrorSeverity.Warning) exn
                member x.ErrorCount = errorCount }

        // Public members
        member __.ErrorLogger = errorLogger

        member __.CollectedDiagnostics = errorsAndWarningsCollector.ToArray()

        member __.ErrorCount = errorCount

        member __.ErrorSeverityOptions with set opts = options <- opts

        member __.AnyErrors = errorCount > 0

    let getLightSyntaxStatus fileName options =
        let lower = String.lowercase fileName
        let lightOnByDefault = List.exists (Filename.checkSuffix lower) FSharpLightSyntaxFileSuffixes
        let lightSyntaxStatus = if lightOnByDefault then (options.LightSyntax <> Some false) else (options.LightSyntax = Some true)
        LightSyntaxStatus(lightSyntaxStatus, true)

    let createLexerFunction fileName options lexbuf (errHandler: ErrorHandler) =
        let lightSyntaxStatus = getLightSyntaxStatus fileName options

        // If we're editing a script then we define INTERACTIVE otherwise COMPILED.
        // Since this parsing for intellisense we always define EDITING.
        let defines = (SourceFileImpl.AdditionalDefinesForUseInEditor options.IsInteractive) @ options.ConditionalCompilationDefines

        // Note: we don't really attempt to intern strings across a large scope.
        let lexResourceManager = new Lexhelp.LexResourceManager()
        
        // When analyzing files using ParseOneFile, i.e. for the use of editing clients, we do not apply line directives.
        // TODO(pathmap): expose PathMap on the service API, and thread it through here
        let lexargs = mkLexargs(fileName, defines, lightSyntaxStatus, lexResourceManager, [], errHandler.ErrorLogger, PathMap.empty)
        let lexargs = { lexargs with applyLineDirectives = false }

        let tokenizer = LexFilter.LexFilter(lightSyntaxStatus, options.CompilingFsLib, Lexer.token lexargs true, lexbuf)
        tokenizer.Lexer

    // Public callers are unable to answer LanguageVersion feature support questions.
    // External Tools including the VS IDE will enable the default LanguageVersion 
    let isFeatureSupported (_featureId:LanguageFeature) = true

    let createLexbuf sourceText isFeatureSupported =
        UnicodeLexing.SourceTextAsLexbuf(isFeatureSupported, sourceText)

    let matchBraces(sourceText: ISourceText, fileName, options: FSharpParsingOptions, userOpName: string, suggestNamesForErrors: bool) =
        let delayedLogger = CapturingErrorLogger("matchBraces")
        use _unwindEL = PushErrorLoggerPhaseUntilUnwind (fun _ -> delayedLogger)
        use _unwindBP = PushThreadBuildPhaseUntilUnwind BuildPhase.Parse

        Trace.TraceInformation("FCS: {0}.{1} ({2})", userOpName, "matchBraces", fileName)
        
        // Make sure there is an ErrorLogger installed whenever we do stuff that might record errors, even if we ultimately ignore the errors
        let delayedLogger = CapturingErrorLogger("matchBraces")
        use _unwindEL = PushErrorLoggerPhaseUntilUnwind (fun _ -> delayedLogger)
        use _unwindBP = PushThreadBuildPhaseUntilUnwind BuildPhase.Parse
        
        let matchingBraces = new ResizeArray<_>()
        Lexhelp.usingLexbufForParsing(createLexbuf sourceText isFeatureSupported, fileName) (fun lexbuf ->
            let errHandler = ErrorHandler(false, fileName, options.ErrorSeverityOptions, sourceText, suggestNamesForErrors)
            let lexfun = createLexerFunction fileName options lexbuf errHandler
            let parenTokensBalance t1 t2 =
                match t1, t2 with
                | (LPAREN, RPAREN)
                | (LPAREN, RPAREN_IS_HERE)
                | (LBRACE, RBRACE)
                | (LBRACE, RBRACE_IS_HERE)
                | (SIG, END)
                | (STRUCT, END)
                | (LBRACK_BAR, BAR_RBRACK)
                | (LBRACK, RBRACK)
                | (LBRACK_LESS, GREATER_RBRACK)
                | (BEGIN, END) -> true
                | (LQUOTE q1, RQUOTE q2) -> q1 = q2
                | _ -> false
            let rec matchBraces stack =
                match lexfun lexbuf, stack with
                | tok2, ((tok1, m1) :: stack') when parenTokensBalance tok1 tok2 ->
                    matchingBraces.Add(m1, lexbuf.LexemeRange)
                    matchBraces stack'
                | ((LPAREN | LBRACE | LBRACK | LBRACK_BAR | LQUOTE _ | LBRACK_LESS) as tok), _ ->
                     matchBraces ((tok, lexbuf.LexemeRange) :: stack)
                | (EOF _ | LEX_FAILURE _), _ -> ()
                | _ -> matchBraces stack
            matchBraces [])
        matchingBraces.ToArray()

    let parseFile(sourceText: ISourceText, fileName, options: FSharpParsingOptions, userOpName: string, suggestNamesForErrors: bool) =
        Trace.TraceInformation("FCS: {0}.{1} ({2})", userOpName, "parseFile", fileName)
        let errHandler = new ErrorHandler(true, fileName, options.ErrorSeverityOptions, sourceText, suggestNamesForErrors)
        use unwindEL = PushErrorLoggerPhaseUntilUnwind (fun _oldLogger -> errHandler.ErrorLogger)
        use unwindBP = PushThreadBuildPhaseUntilUnwind BuildPhase.Parse

        let parseResult =
            Lexhelp.usingLexbufForParsing(createLexbuf sourceText isFeatureSupported, fileName) (fun lexbuf ->
                let lexfun = createLexerFunction fileName options lexbuf errHandler
                let isLastCompiland =
                    fileName.Equals(options.LastFileName, StringComparison.CurrentCultureIgnoreCase) ||
                    CompileOps.IsScript(fileName)
                let isExe = options.IsExe
                try Some (ParseInput(lexfun, errHandler.ErrorLogger, lexbuf, None, fileName, (isLastCompiland, isExe)))
                with e ->
                    errHandler.ErrorLogger.StopProcessingRecovery e Range.range0 // don't re-raise any exceptions, we must return None.
                    None)
        errHandler.CollectedDiagnostics, parseResult, errHandler.AnyErrors


    let ApplyLoadClosure(tcConfig, parsedMainInput, mainInputFileName, loadClosure: LoadClosure option, tcImports: TcImports, backgroundDiagnostics) = 

        // If additional references were brought in by the preprocessor then we need to process them
        match loadClosure with
        | Some loadClosure ->
            // Play unresolved references for this file.
            tcImports.ReportUnresolvedAssemblyReferences(loadClosure.UnresolvedReferences)
            
            // If there was a loadClosure, replay the errors and warnings from resolution, excluding parsing
            loadClosure.LoadClosureRootFileDiagnostics |> List.iter diagnosticSink
            
            let fileOfBackgroundError err = (match GetRangeOfDiagnostic (fst err) with Some m-> m.FileName | None -> null)
            let sameFile file hashLoadInFile = 
                (0 = String.Compare(hashLoadInFile, file, StringComparison.OrdinalIgnoreCase))
            
            //  walk the list of #loads and keep the ones for this file.
            let hashLoadsInFile = 
                loadClosure.SourceFiles 
                |> List.filter(fun (_,ms) -> ms<>[]) // #loaded file, ranges of #load
            
            let hashLoadBackgroundDiagnostics, otherBackgroundDiagnostics = 
                backgroundDiagnostics 
                |> Array.partition (fun backgroundError -> 
                    hashLoadsInFile 
                    |>  List.exists (fst >> sameFile (fileOfBackgroundError backgroundError)))
            
            // Create single errors for the #load-ed files.
            // Group errors and warnings by file name.
            let hashLoadBackgroundDiagnosticsGroupedByFileName = 
                hashLoadBackgroundDiagnostics 
                |> Array.map(fun err -> fileOfBackgroundError err,err) 
                |> Array.groupBy fst  // fileWithErrors, error list
            
            //  Join the sets and report errors. 
            //  It is by-design that these messages are only present in the language service. A true build would report the errors at their
            //  spots in the individual source files.
            for (fileOfHashLoad, rangesOfHashLoad) in hashLoadsInFile do
                for (file, errorGroupedByFileName) in hashLoadBackgroundDiagnosticsGroupedByFileName do
                    if sameFile file fileOfHashLoad then
                        for rangeOfHashLoad in rangesOfHashLoad do // Handle the case of two #loads of the same file
                            let diagnostics = errorGroupedByFileName |> Array.map(fun (_,(pe,f)) -> pe.Exception,f) // Strip the build phase here. It will be replaced, in total, with TypeCheck
                            let errors = [ for (err,sev) in diagnostics do if sev = FSharpErrorSeverity.Error then yield err ]
                            let warnings = [ for (err,sev) in diagnostics do if sev = FSharpErrorSeverity.Warning then yield err ]
                                    
                            let message = HashLoadedSourceHasIssues(warnings,errors,rangeOfHashLoad)
                            if errors=[] then warning(message)
                            else errorR(message)
            
            // Replay other background errors.
            for (phasedError,sev) in otherBackgroundDiagnostics do
                if sev = FSharpErrorSeverity.Warning then 
                    warning phasedError.Exception 
                else errorR phasedError.Exception
            
        | None -> 
            // For non-scripts, check for disallow #r and #load.
            ApplyMetaCommandsFromInputToTcConfig (tcConfig, parsedMainInput,Path.GetDirectoryName mainInputFileName) |> ignore
                    
    // Type check a single file against an initial context, gleaning both errors and intellisense information.
    let CheckOneFile
          (parseResults: FSharpParseFileResults,
           sourceText: ISourceText,
           mainInputFileName: string,
           projectFileName: string,
           tcConfig: TcConfig,
           tcGlobals: TcGlobals,
           tcImports: TcImports,
           tcState: TcState,
           moduleNamesDict: ModuleNamesDict,
           loadClosure: LoadClosure option,
           // These are the errors and warnings seen by the background compiler for the entire antecedent 
           backgroundDiagnostics: (PhasedDiagnostic * FSharpErrorSeverity)[],    
           reactorOps: IReactorOperations,
           // Used by 'FSharpDeclarationListInfo' to check the IncrementalBuilder is still alive.
           textSnapshotInfo : obj option,
           userOpName: string,
           suggestNamesForErrors: bool) = async {

        use _logBlock = Logger.LogBlock LogCompilerFunctionId.Service_CheckOneFile

        match parseResults.ParseTree with 
        // When processing the following cases, we don't need to type-check
        | None -> return [||], Result.Error()
                   
        // Run the type checker...
        | Some parsedMainInput ->

        // Initialize the error handler 
        let errHandler = new ErrorHandler(true, mainInputFileName, tcConfig.errorSeverityOptions, sourceText, suggestNamesForErrors)
                
        use _unwindEL = PushErrorLoggerPhaseUntilUnwind (fun _oldLogger -> errHandler.ErrorLogger)
        use _unwindBP = PushThreadBuildPhaseUntilUnwind BuildPhase.TypeCheck
            
        // Apply nowarns to tcConfig (may generate errors, so ensure errorLogger is installed)
        let tcConfig = ApplyNoWarnsToTcConfig (tcConfig, parsedMainInput,Path.GetDirectoryName mainInputFileName)
                        
        // update the error handler with the modified tcConfig
        errHandler.ErrorSeverityOptions <- tcConfig.errorSeverityOptions
            
        // Play background errors and warnings for this file.
        for (err,sev) in backgroundDiagnostics do
            diagnosticSink (err, (sev = FSharpErrorSeverity.Error))
            
        // If additional references were brought in by the preprocessor then we need to process them
        ApplyLoadClosure(tcConfig, parsedMainInput, mainInputFileName, loadClosure, tcImports, backgroundDiagnostics)
                    
        // A problem arises with nice name generation, which really should only 
        // be done in the backend, but is also done in the typechecker for better or worse. 
        // If we don't do this the NNG accumulates data and we get a memory leak. 
        tcState.NiceNameGenerator.Reset()
                
        // Typecheck the real input.  
        let sink = TcResultsSinkImpl(tcGlobals, sourceText = sourceText)

        let! ct = Async.CancellationToken
            
        let! resOpt =
            async {
                try
                    let checkForErrors() = (parseResults.ParseHadErrors || errHandler.ErrorCount > 0)

                    let parsedMainInput, _moduleNamesDict = DeduplicateParsedInputModuleName moduleNamesDict parsedMainInput

                    // Typecheck is potentially a long running operation. We chop it up here with an Eventually continuation and, at each slice, give a chance
                    // for the client to claim the result as obsolete and have the typecheck abort.
                            
                    let! result = 
                        TypeCheckOneInputAndFinishEventually(checkForErrors, tcConfig, tcImports, tcGlobals, None, TcResultsSink.WithSink sink, tcState, parsedMainInput)
                        |> Eventually.repeatedlyProgressUntilDoneOrTimeShareOverOrCanceled maxTimeShareMilliseconds ct (fun ctok f -> f ctok)
                        |> Eventually.forceAsync  
                            (fun work ->
                                reactorOps.EnqueueAndAwaitOpAsync(userOpName, "CheckOneFile.Fragment", mainInputFileName, 
                                    fun ctok -> 
                                        // This work is not cancellable
                                        let res = 
                                            // Reinstall the compilation globals each time we start or restart
                                            use unwind = new CompilationGlobalsScope (errHandler.ErrorLogger, BuildPhase.TypeCheck)
                                            work ctok
                                        cancellable.Return(res)
                                        ))
                             
                    return result
                with e ->
                    errorR e
                    let mty = Construct.NewEmptyModuleOrNamespaceType Namespace
                    return Some((tcState.TcEnvFromSignatures, EmptyTopAttrs, [], [ mty ]), tcState)
            }
                
        let errors = errHandler.CollectedDiagnostics
                
        let res = 
            match resOpt with
            | Some ((tcEnvAtEnd, _, implFiles, ccuSigsForFiles), tcState) ->
                TypeCheckInfo(tcConfig, tcGlobals, 
                              List.head ccuSigsForFiles, 
                              tcState.Ccu,
                              tcImports,
                              tcEnvAtEnd.AccessRights,
                              projectFileName, 
                              mainInputFileName, 
                              sink.GetResolutions(), 
                              sink.GetSymbolUses(),
                              tcEnvAtEnd.NameEnv,
                              loadClosure,
                              reactorOps,
                              textSnapshotInfo,
                              List.tryHead implFiles,
                              sink.GetOpenDeclarations())     
                     |> Result.Ok
            | None -> 
                Result.Error()
        return errors, res
        }


[<Sealed>] 
type FSharpProjectContext(thisCcu: CcuThunk, assemblies: FSharpAssembly list, ad: AccessorDomain) =

    /// Get the assemblies referenced
    member __.GetReferencedAssemblies() = assemblies

    member __.AccessibilityRights = FSharpAccessibilityRights(thisCcu, ad)


[<Sealed>]
/// A live object of this type keeps the background corresponding background builder (and type providers) alive (through reference-counting).
//
// There is an important property of all the objects returned by the methods of this type: they do not require 
// the corresponding background builder to be alive. That is, they are simply plain-old-data through pre-formatting of all result text.
type FSharpCheckFileResults
        (filename: string, 
         errors: FSharpErrorInfo[], 
         scopeOptX: TypeCheckInfo option, 
         dependencyFiles: string[], 
         builderX: IncrementalBuilder option, 
         reactorOpsX:IReactorOperations, 
         keepAssemblyContents: bool) =

    // This may be None initially
    let mutable details = match scopeOptX with None -> None | Some scopeX -> Some (scopeX, builderX, reactorOpsX)

    // Run an operation that needs to access a builder and be run in the reactor thread
    let reactorOp userOpName opName dflt f = 
      async {
        match details with
        | None -> 
            return dflt
        | Some (scope, _, reactor) -> 
            // Increment the usage count to ensure the builder doesn't get released while running operations asynchronously. 
            let! res = reactor.EnqueueAndAwaitOpAsync(userOpName, opName, filename, fun ctok ->  f ctok scope |> cancellable.Return)
            return res
      }

    // Run an operation that can be called from any thread
    let threadSafeOp dflt f = 
        match details with
        | None -> dflt()
        | Some (scope, _builderOpt, _ops) -> f scope

    member __.Errors = errors

    member __.HasFullTypeCheckInfo = details.IsSome
    
    member info.TryGetCurrentTcImports () =
        match builderX with
        | Some builder -> builder.TryGetCurrentTcImports ()
        | _ -> None

    /// Intellisense autocompletions
    member __.GetDeclarationListInfo(parseResultsOpt, line, lineStr, partialName, ?getAllEntities, ?hasTextChangedSinceLastTypecheck, ?userOpName: string) = 
        let userOpName = defaultArg userOpName "Unknown"
        let getAllEntities = defaultArg getAllEntities (fun() -> [])
        let hasTextChangedSinceLastTypecheck = defaultArg hasTextChangedSinceLastTypecheck (fun _ -> false)
        reactorOp userOpName "GetDeclarations" FSharpDeclarationListInfo.Empty (fun ctok scope -> 
            scope.GetDeclarations(ctok, parseResultsOpt, line, lineStr, partialName, getAllEntities, hasTextChangedSinceLastTypecheck))

    member __.GetDeclarationListSymbols(parseResultsOpt, line, lineStr, partialName, ?getAllEntities, ?hasTextChangedSinceLastTypecheck, ?userOpName: string) = 
        let userOpName = defaultArg userOpName "Unknown"
        let hasTextChangedSinceLastTypecheck = defaultArg hasTextChangedSinceLastTypecheck (fun _ -> false)
        let getAllEntities = defaultArg getAllEntities (fun() -> [])
        reactorOp userOpName "GetDeclarationListSymbols" List.empty (fun ctok scope -> 
            scope.GetDeclarationListSymbols(ctok, parseResultsOpt, line, lineStr, partialName, getAllEntities, hasTextChangedSinceLastTypecheck))

    /// Resolve the names at the given location to give a data tip 
    member __.GetStructuredToolTipText(line, colAtEndOfNames, lineStr, names, tokenTag, ?userOpName: string) = 
        let userOpName = defaultArg userOpName "Unknown"
        let dflt = FSharpToolTipText []
        match tokenTagToTokenId tokenTag with 
        | TOKEN_IDENT -> 
            reactorOp userOpName "GetStructuredToolTipText" dflt (fun ctok scope -> 
                scope.GetStructuredToolTipText(ctok, line, lineStr, colAtEndOfNames, names))
        | TOKEN_STRING | TOKEN_STRING_TEXT -> 
            reactorOp userOpName "GetReferenceResolutionToolTipText" dflt (fun ctok scope ->
                scope.GetReferenceResolutionStructuredToolTipText(ctok, line, colAtEndOfNames) )
        | _ -> 
            async.Return dflt

    member info.GetToolTipText(line, colAtEndOfNames, lineStr, names, tokenTag, userOpName) = 
        info.GetStructuredToolTipText(line, colAtEndOfNames, lineStr, names, tokenTag, ?userOpName=userOpName)
        |> Tooltips.Map Tooltips.ToFSharpToolTipText

    member __.GetF1Keyword (line, colAtEndOfNames, lineStr, names, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        reactorOp userOpName "GetF1Keyword" None (fun ctok scope -> 
            scope.GetF1Keyword (ctok, line, lineStr, colAtEndOfNames, names))

    // Resolve the names at the given location to a set of methods
    member __.GetMethods(line, colAtEndOfNames, lineStr, names, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        let dflt = FSharpMethodGroup("",[| |])
        reactorOp userOpName "GetMethods" dflt (fun ctok scope -> 
            scope.GetMethods (ctok, line, lineStr, colAtEndOfNames, names))
            
    member __.GetDeclarationLocation (line, colAtEndOfNames, lineStr, names, ?preferFlag, ?userOpName: string) = 
        let userOpName = defaultArg userOpName "Unknown"
        let dflt = FSharpFindDeclResult.DeclNotFound (FSharpFindDeclFailureReason.Unknown "")
        reactorOp userOpName "GetDeclarationLocation" dflt (fun ctok scope -> 
            scope.GetDeclarationLocation (ctok, line, lineStr, colAtEndOfNames, names, preferFlag))

    member __.GetSymbolUseAtLocation (line, colAtEndOfNames, lineStr, names, ?userOpName: string) = 
        let userOpName = defaultArg userOpName "Unknown"
        reactorOp userOpName "GetSymbolUseAtLocation" None (fun ctok scope -> 
            scope.GetSymbolUseAtLocation (ctok, line, lineStr, colAtEndOfNames, names)
            |> Option.map (fun (sym,denv,m) -> FSharpSymbolUse(scope.TcGlobals,denv,sym,ItemOccurence.Use,m)))

    member __.GetMethodsAsSymbols (line, colAtEndOfNames, lineStr, names, ?userOpName: string) = 
        let userOpName = defaultArg userOpName "Unknown"
        reactorOp userOpName "GetMethodsAsSymbols" None (fun ctok scope -> 
            scope.GetMethodsAsSymbols (ctok, line, lineStr, colAtEndOfNames, names)
            |> Option.map (fun (symbols,denv,m) ->
                symbols |> List.map (fun sym -> FSharpSymbolUse(scope.TcGlobals,denv,sym,ItemOccurence.Use,m))))

    member __.GetSymbolAtLocation (line, colAtEndOfNames, lineStr, names, ?userOpName: string) = 
        let userOpName = defaultArg userOpName "Unknown"
        reactorOp userOpName "GetSymbolAtLocation" None (fun ctok scope -> 
            scope.GetSymbolUseAtLocation (ctok, line, lineStr, colAtEndOfNames, names)
            |> Option.map (fun (sym,_,_) -> sym))

    member info.GetFormatSpecifierLocations() = 
        info.GetFormatSpecifierLocationsAndArity() |> Array.map fst

    member __.GetFormatSpecifierLocationsAndArity() = 
        threadSafeOp 
            (fun () -> [| |]) 
            (fun scope -> 
                // This operation is not asynchronous - GetFormatSpecifierLocationsAndArity can be run on the calling thread
                scope.GetFormatSpecifierLocationsAndArity())

    member __.GetSemanticClassification(range: range option) =
        threadSafeOp 
            (fun () -> [| |]) 
            (fun scope -> 
                // This operation is not asynchronous - GetSemanticClassification can be run on the calling thread
                scope.GetSemanticClassification(range))
     
    member __.PartialAssemblySignature = 
        threadSafeOp 
            (fun () -> failwith "not available") 
            (fun scope -> 
                // This operation is not asynchronous - PartialAssemblySignature can be run on the calling thread
                scope.PartialAssemblySignatureForFile)

    member __.ProjectContext = 
        threadSafeOp 
            (fun () -> failwith "not available") 
            (fun scope -> 
                // This operation is not asynchronous - GetReferencedAssemblies can be run on the calling thread
                FSharpProjectContext(scope.ThisCcu, scope.GetReferencedAssemblies(), scope.AccessRights))

    member __.DependencyFiles = dependencyFiles

    member __.GetAllUsesOfAllSymbolsInFile() = 
        threadSafeOp 
            (fun () -> [| |])
            (fun scope ->
                let cenv = scope.SymbolEnv
                [| for symbolUseChunk in scope.ScopeSymbolUses.AllUsesOfSymbols do
                    for symbolUse in symbolUseChunk do
                    if symbolUse.ItemOccurence <> ItemOccurence.RelatedText then
                        let symbol = FSharpSymbol.Create(cenv, symbolUse.Item)
                        yield FSharpSymbolUse(scope.TcGlobals, symbolUse.DisplayEnv, symbol, symbolUse.ItemOccurence, symbolUse.Range) |])
         |> async.Return 

    member __.GetUsesOfSymbolInFile(symbol:FSharpSymbol) = 
        threadSafeOp 
            (fun () -> [| |]) 
            (fun scope -> 
                [| for symbolUse in scope.ScopeSymbolUses.GetUsesOfSymbol(symbol.Item) |> Seq.distinctBy (fun symbolUse -> symbolUse.ItemOccurence, symbolUse.Range) do
                     if symbolUse.ItemOccurence <> ItemOccurence.RelatedText then
                        yield FSharpSymbolUse(scope.TcGlobals, symbolUse.DisplayEnv, symbol, symbolUse.ItemOccurence, symbolUse.Range) |])
         |> async.Return 

    member __.GetVisibleNamespacesAndModulesAtPoint(pos: pos) = 
        threadSafeOp 
            (fun () -> [| |]) 
            (fun scope -> scope.GetVisibleNamespacesAndModulesAtPosition(pos) |> List.toArray)
         |> async.Return 

    member __.IsRelativeNameResolvable(pos: pos, plid: string list, item: Item, ?userOpName: string) = 
        let userOpName = defaultArg userOpName "Unknown"
        reactorOp userOpName "IsRelativeNameResolvable" true (fun ctok scope -> 
            RequireCompilationThread ctok
            scope.IsRelativeNameResolvable(pos, plid, item))

    member __.IsRelativeNameResolvableFromSymbol(pos: pos, plid: string list, symbol: FSharpSymbol, ?userOpName: string) = 
        let userOpName = defaultArg userOpName "Unknown"
        reactorOp userOpName "IsRelativeNameResolvableFromSymbol" true (fun ctok scope -> 
            RequireCompilationThread ctok
            scope.IsRelativeNameResolvableFromSymbol(pos, plid, symbol))
    
    member __.GetDisplayContextForPos(pos: pos) : Async<FSharpDisplayContext option> =
        let userOpName = "CodeLens"
        reactorOp userOpName "GetDisplayContextAtPos" None (fun ctok scope -> 
            DoesNotRequireCompilerThreadTokenAndCouldPossiblyBeMadeConcurrent ctok
            let (nenv, _), _ = scope.GetBestDisplayEnvForPos pos
            Some(FSharpDisplayContext(fun _ -> nenv.DisplayEnv)))
            
    member __.ImplementationFile =
        if not keepAssemblyContents then invalidOp "The 'keepAssemblyContents' flag must be set to true on the FSharpChecker in order to access the checked contents of assemblies"
        scopeOptX 
        |> Option.map (fun scope -> 
            let cenv = SymbolEnv(scope.TcGlobals, scope.ThisCcu, Some scope.CcuSigForFile, scope.TcImports)
            scope.ImplementationFile |> Option.map (fun implFile -> FSharpImplementationFileContents(cenv, implFile)))
        |> Option.defaultValue None

    member __.OpenDeclarations =
        scopeOptX 
        |> Option.map (fun scope -> 
            let cenv = scope.SymbolEnv
            scope.OpenDeclarations |> Array.map (fun x -> FSharpOpenDeclaration(x.LongId, x.Range, (x.Modules |> List.map (fun x -> FSharpEntity(cenv, x))), x.AppliedScope, x.IsOwnNamespace)))
        |> Option.defaultValue [| |]

    override __.ToString() = "FSharpCheckFileResults(" + filename + ")"

    static member MakeEmpty(filename: string, creationErrors: FSharpErrorInfo[], reactorOps, keepAssemblyContents) = 
        FSharpCheckFileResults (filename, creationErrors, None, [| |], None, reactorOps, keepAssemblyContents)

    static member JoinErrors(isIncompleteTypeCheckEnvironment, 
                             creationErrors: FSharpErrorInfo[], 
                             parseErrors: FSharpErrorInfo[], 
                             tcErrors: FSharpErrorInfo[]) =
        [| yield! creationErrors 
           yield! parseErrors
           if isIncompleteTypeCheckEnvironment then 
               yield! Seq.truncate maxTypeCheckErrorsOutOfProjectContext tcErrors
           else 
               yield! tcErrors |]

    static member Make
        (mainInputFileName: string, 
         projectFileName, 
         tcConfig, tcGlobals, 
         isIncompleteTypeCheckEnvironment: bool, 
         builder: IncrementalBuilder, 
         dependencyFiles, 
         creationErrors: FSharpErrorInfo[], 
         parseErrors: FSharpErrorInfo[], 
         tcErrors: FSharpErrorInfo[], 
         reactorOps, 
         keepAssemblyContents,
         ccuSigForFile, 
         thisCcu, tcImports, tcAccessRights, 
         sResolutions, sSymbolUses, 
         sFallback, loadClosure,
         implFileOpt, 
         openDeclarations) = 

        let tcFileInfo = 
            TypeCheckInfo(tcConfig, tcGlobals, ccuSigForFile, thisCcu, tcImports, tcAccessRights, 
                          projectFileName, mainInputFileName, sResolutions, sSymbolUses, 
                          sFallback, loadClosure, reactorOps,
                          None, implFileOpt, openDeclarations) 
                
        let errors = FSharpCheckFileResults.JoinErrors(isIncompleteTypeCheckEnvironment, creationErrors, parseErrors, tcErrors)
        FSharpCheckFileResults (mainInputFileName, errors, Some tcFileInfo, dependencyFiles, Some builder, reactorOps, keepAssemblyContents)

    static member CheckOneFile
        (parseResults: FSharpParseFileResults,
         sourceText: ISourceText,
         mainInputFileName: string,
         projectFileName: string,
         tcConfig: TcConfig,
         tcGlobals: TcGlobals,
         tcImports: TcImports,
         tcState: TcState,
         moduleNamesDict: ModuleNamesDict,
         loadClosure: LoadClosure option,
         backgroundDiagnostics: (PhasedDiagnostic * FSharpErrorSeverity)[],    
         reactorOps: IReactorOperations,
         textSnapshotInfo : obj option,
         userOpName: string,
         isIncompleteTypeCheckEnvironment: bool, 
         builder: IncrementalBuilder, 
         dependencyFiles: string[], 
         creationErrors: FSharpErrorInfo[], 
         parseErrors: FSharpErrorInfo[], 
         keepAssemblyContents: bool,
         suggestNamesForErrors: bool) = 
        async {
            let! tcErrors, tcFileInfo = 
                ParseAndCheckFile.CheckOneFile
                    (parseResults, sourceText, mainInputFileName, projectFileName, tcConfig, tcGlobals, tcImports, 
                     tcState, moduleNamesDict, loadClosure, backgroundDiagnostics, reactorOps, 
                     textSnapshotInfo, userOpName, suggestNamesForErrors)
            match tcFileInfo with 
            | Result.Error ()  ->  
                return FSharpCheckFileAnswer.Aborted                
            | Result.Ok tcFileInfo -> 
                let errors = FSharpCheckFileResults.JoinErrors(isIncompleteTypeCheckEnvironment, creationErrors, parseErrors, tcErrors)
                let results = FSharpCheckFileResults (mainInputFileName, errors, Some tcFileInfo, dependencyFiles, Some builder, reactorOps, keepAssemblyContents)
                return FSharpCheckFileAnswer.Succeeded(results)
        }

and [<NoComparison>] FSharpCheckFileAnswer =
    | Aborted
    | Succeeded of FSharpCheckFileResults   
        

[<Sealed>]
// 'details' is an option because the creation of the tcGlobals etc. for the project may have failed.
type FSharpCheckProjectResults
         (projectFileName:string, 
          tcConfigOption: TcConfig option, 
          keepAssemblyContents: bool, 
          errors: FSharpErrorInfo[], 
          details:(TcGlobals * TcImports * CcuThunk * ModuleOrNamespaceType * TcSymbolUses list *
                   TopAttribs option * CompileOps.IRawFSharpAssemblyData option * ILAssemblyRef *
                   AccessorDomain * TypedImplFile list option * string[]) option) =

    let getDetails() = 
        match details with 
        | None -> invalidOp ("The project has no results due to critical errors in the project options. Check the HasCriticalErrors before accessing the detailed results. Errors: " + String.concat "\n" [ for e in errors -> e.Message ])
        | Some d -> d

    let getTcConfig() = 
        match tcConfigOption with 
        | None -> invalidOp ("The project has no results due to critical errors in the project options. Check the HasCriticalErrors before accessing the detailed results. Errors: " + String.concat "\n" [ for e in errors -> e.Message ])
        | Some d -> d

    member __.Errors = errors

    member __.HasCriticalErrors = details.IsNone

    member __.AssemblySignature =  
        let (tcGlobals, tcImports, thisCcu, ccuSig, _tcSymbolUses, topAttribs, _tcAssemblyData, _ilAssemRef, _ad, _tcAssemblyExpr, _dependencyFiles) = getDetails()
        FSharpAssemblySignature(tcGlobals, thisCcu, ccuSig, tcImports, topAttribs, ccuSig)

    member __.TypedImplementationFiles =
        if not keepAssemblyContents then invalidOp "The 'keepAssemblyContents' flag must be set to true on the FSharpChecker in order to access the checked contents of assemblies"
        let (tcGlobals, tcImports, thisCcu, _ccuSig, _tcSymbolUses, _topAttribs, _tcAssemblyData, _ilAssemRef, _ad, tcAssemblyExpr, _dependencyFiles) = getDetails()
        let mimpls = 
            match tcAssemblyExpr with 
            | None -> []
            | Some mimpls -> mimpls
        tcGlobals, thisCcu, tcImports, mimpls

    member info.AssemblyContents = 
        if not keepAssemblyContents then invalidOp "The 'keepAssemblyContents' flag must be set to true on the FSharpChecker in order to access the checked contents of assemblies"
        let (tcGlobals, tcImports, thisCcu, ccuSig, _tcSymbolUses, _topAttribs, _tcAssemblyData, _ilAssemRef, _ad, tcAssemblyExpr, _dependencyFiles) = getDetails()
        let mimpls = 
            match tcAssemblyExpr with 
            | None -> []
            | Some mimpls -> mimpls
        FSharpAssemblyContents(tcGlobals, thisCcu, Some ccuSig, tcImports, mimpls)

    member __.GetOptimizedAssemblyContents() =  
        if not keepAssemblyContents then invalidOp "The 'keepAssemblyContents' flag must be set to true on the FSharpChecker in order to access the checked contents of assemblies"
        let (tcGlobals, tcImports, thisCcu, ccuSig, _tcSymbolUses, _topAttribs, _tcAssemblyData, _ilAssemRef, _ad, tcAssemblyExpr, _dependencyFiles) = getDetails()
        let mimpls = 
            match tcAssemblyExpr with 
            | None -> []
            | Some mimpls -> mimpls
        let outfile = "" // only used if tcConfig.writeTermsToFiles is true
        let importMap = tcImports.GetImportMap()
        let optEnv0 = GetInitialOptimizationEnv (tcImports, tcGlobals)
        let tcConfig = getTcConfig()
        let optimizedImpls, _optimizationData, _ = ApplyAllOptimizations (tcConfig, tcGlobals, (LightweightTcValForUsingInBuildMethodCall tcGlobals), outfile, importMap, false, optEnv0, thisCcu, mimpls)                
        let mimpls =
            match optimizedImpls with
            | TypedAssemblyAfterOptimization files ->
                files |> List.map fst

        FSharpAssemblyContents(tcGlobals, thisCcu, Some ccuSig, tcImports, mimpls)

    // Not, this does not have to be a SyncOp, it can be called from any thread
    member __.GetUsesOfSymbol(symbol:FSharpSymbol) = 
        let (tcGlobals, _tcImports, _thisCcu, _ccuSig, tcSymbolUses, _topAttribs, _tcAssemblyData, _ilAssemRef, _ad, _tcAssemblyExpr, _dependencyFiles) = getDetails()

        tcSymbolUses
        |> Seq.collect (fun r -> r.GetUsesOfSymbol symbol.Item)
        |> Seq.distinctBy (fun symbolUse -> symbolUse.ItemOccurence, symbolUse.Range) 
        |> Seq.filter (fun symbolUse -> symbolUse.ItemOccurence <> ItemOccurence.RelatedText) 
        |> Seq.map (fun symbolUse -> FSharpSymbolUse(tcGlobals, symbolUse.DisplayEnv, symbol, symbolUse.ItemOccurence, symbolUse.Range)) 
        |> Seq.toArray
        |> async.Return

    // Not, this does not have to be a SyncOp, it can be called from any thread
    member __.GetAllUsesOfAllSymbols() = 
        let (tcGlobals, tcImports, thisCcu, ccuSig, tcSymbolUses, _topAttribs, _tcAssemblyData, _ilAssemRef, _ad, _tcAssemblyExpr, _dependencyFiles) = getDetails()
        let cenv = SymbolEnv(tcGlobals, thisCcu, Some ccuSig, tcImports)

        [| for r in tcSymbolUses do
             for symbolUseChunk in r.AllUsesOfSymbols do
                for symbolUse in symbolUseChunk do
                if symbolUse.ItemOccurence <> ItemOccurence.RelatedText then
                  let symbol = FSharpSymbol.Create(cenv, symbolUse.Item)
                  yield FSharpSymbolUse(tcGlobals, symbolUse.DisplayEnv, symbol, symbolUse.ItemOccurence, symbolUse.Range) |]
        |> async.Return

    member __.ProjectContext = 
        let (tcGlobals, tcImports, thisCcu, _ccuSig, _tcSymbolUses, _topAttribs, _tcAssemblyData, _ilAssemRef, ad, _tcAssemblyExpr, _dependencyFiles) = getDetails()
        let assemblies = 
            [ for x in tcImports.GetImportedAssemblies() do
                yield FSharpAssembly(tcGlobals, tcImports, x.FSharpViewOfMetadata) ]
        FSharpProjectContext(thisCcu, assemblies, ad) 

    member __.RawFSharpAssemblyData = 
        let (_tcGlobals, _tcImports, _thisCcu, _ccuSig, _tcSymbolUses, _topAttribs, tcAssemblyData, _ilAssemRef, _ad, _tcAssemblyExpr, _dependencyFiles) = getDetails()
        tcAssemblyData

    member __.DependencyFiles = 
        let (_tcGlobals, _tcImports, _thisCcu, _ccuSig, _tcSymbolUses, _topAttribs, _tcAssemblyData, _ilAssemRef, _ad, _tcAssemblyExpr, dependencyFiles) = getDetails()
        dependencyFiles

    member __.AssemblyFullName = 
        let (_tcGlobals, _tcImports, _thisCcu, _ccuSig, _tcSymbolUses, _topAttribs, _tcAssemblyData, ilAssemRef, _ad, _tcAssemblyExpr, _dependencyFiles) = getDetails()
        ilAssemRef.QualifiedName

    override __.ToString() = "FSharpCheckProjectResults(" + projectFileName + ")"

type FsiInteractiveChecker(legacyReferenceResolver, 
                           reactorOps: IReactorOperations,
                           tcConfig: TcConfig,
                           tcGlobals,
                           tcImports,
                           tcState) =

    let keepAssemblyContents = false

    member __.ParseAndCheckInteraction (ctok, sourceText: ISourceText, ?userOpName: string) =
        async {
            let userOpName = defaultArg userOpName "Unknown"
            let filename = Path.Combine(tcConfig.implicitIncludeDir, "stdin.fsx")
            let suggestNamesForErrors = true // Will always be true, this is just for readability
            // Note: projectSourceFiles is only used to compute isLastCompiland, and is ignored if Build.IsScript(mainInputFileName) is true (which it is in this case).
            let parsingOptions = FSharpParsingOptions.FromTcConfig(tcConfig, [| filename |], true)
            let parseErrors, parseTreeOpt, anyErrors = ParseAndCheckFile.parseFile (sourceText, filename, parsingOptions, userOpName, suggestNamesForErrors)
            let dependencyFiles = [| |] // interactions have no dependencies
            let parseResults = FSharpParseFileResults(parseErrors, parseTreeOpt, parseHadErrors = anyErrors, dependencyFiles = dependencyFiles)
            
            let backgroundDiagnostics = [| |]
            let reduceMemoryUsage = ReduceMemoryFlag.Yes
            let assumeDotNetFramework = tcConfig.primaryAssembly = PrimaryAssembly.Mscorlib

            let applyCompilerOptions tcConfigB  = 
                let fsiCompilerOptions = CompileOptions.GetCoreFsiCompilerOptions tcConfigB 
                CompileOptions.ParseCompilerOptions (ignore, fsiCompilerOptions, [ ])

            let loadClosure =
                LoadClosure.ComputeClosureOfScriptText(ctok, legacyReferenceResolver, defaultFSharpBinariesDir,
                    filename, sourceText, CodeContext.Editing,
                    tcConfig.useSimpleResolution, tcConfig.useFsiAuxLib,
                    tcConfig.useSdkRefs, new Lexhelp.LexResourceManager(),
                    applyCompilerOptions, assumeDotNetFramework,
                    tryGetMetadataSnapshot=(fun _ -> None), reduceMemoryUsage=reduceMemoryUsage)

            let! tcErrors, tcFileInfo =  
                ParseAndCheckFile.CheckOneFile
                    (parseResults, sourceText, filename, "project",
                     tcConfig, tcGlobals, tcImports,  tcState, 
                     Map.empty, Some loadClosure, backgroundDiagnostics,
                     reactorOps, None, userOpName, suggestNamesForErrors)

            return
                match tcFileInfo with 
                | Result.Ok tcFileInfo ->
                    let errors = [|  yield! parseErrors; yield! tcErrors |]
                    let typeCheckResults = FSharpCheckFileResults (filename, errors, Some tcFileInfo, dependencyFiles, None, reactorOps, false)   
                    let projectResults = 
                        FSharpCheckProjectResults (filename, Some tcConfig,
                            keepAssemblyContents, errors, 
                            Some(tcGlobals, tcImports, tcFileInfo.ThisCcu, tcFileInfo.CcuSigForFile,
                                 [tcFileInfo.ScopeSymbolUses], None, None, mkSimpleAssemblyRef "stdin", 
                                 tcState.TcEnvFromImpls.AccessRights, None, dependencyFiles))

                    parseResults, typeCheckResults, projectResults

                | Result.Error () ->
                    failwith "unexpected aborted"
        }

