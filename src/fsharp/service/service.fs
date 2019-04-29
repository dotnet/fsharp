// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Open up the compiler as an incremental service for parsing,
// type checking and intellisense-like environment-reporting.

namespace FSharp.Compiler.SourceCodeServices

open System
open System.Collections.Generic
open System.Collections.Concurrent
open System.Diagnostics
open System.IO
open System.Reflection
open System.Text

open Microsoft.FSharp.Core.Printf
open FSharp.Compiler
open FSharp.Compiler.AbstractIL
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.AbstractIL.Diagnostics
open FSharp.Compiler.AbstractIL.Internal
open FSharp.Compiler.AbstractIL.Internal.Library

open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.Ast
open FSharp.Compiler.CompileOps
open FSharp.Compiler.CompileOptions
open FSharp.Compiler.Driver
open FSharp.Compiler.ErrorLogger
open FSharp.Compiler.Lib
open FSharp.Compiler.PrettyNaming
open FSharp.Compiler.Parser
open FSharp.Compiler.Range
open FSharp.Compiler.Lexhelp
open FSharp.Compiler.Layout
open FSharp.Compiler.Tast
open FSharp.Compiler.Tastops
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.Infos
open FSharp.Compiler.InfoReader
open FSharp.Compiler.NameResolution
open FSharp.Compiler.TypeChecker
open FSharp.Compiler.SourceCodeServices.SymbolHelpers

open Internal.Utilities
open Internal.Utilities.Collections
open FSharp.Compiler.Layout.TaggedTextOps

type internal Layout = StructuredFormat.Layout

[<AutoOpen>]
module EnvMisc =
    let getToolTipTextSize = GetEnvInteger "FCS_GetToolTipTextCacheSize" 5
    let maxTypeCheckErrorsOutOfProjectContext = GetEnvInteger "FCS_MaxErrorsOutOfProjectContext" 3
    let braceMatchCacheSize = GetEnvInteger "FCS_BraceMatchCacheSize" 5
    let parseFileCacheSize = GetEnvInteger "FCS_ParseFileCacheSize" 2
    let checkFileInProjectCacheSize = GetEnvInteger "FCS_CheckFileInProjectCacheSize" 10

    let projectCacheSizeDefault   = GetEnvInteger "FCS_ProjectCacheSizeDefault" 3
    let frameworkTcImportsCacheStrongSize = GetEnvInteger "FCS_frameworkTcImportsCacheStrongSizeDefault" 8
    let maxMBDefault =  GetEnvInteger "FCS_MaxMB" 1000000 // a million MB = 1TB = disabled
    //let maxMBDefault = GetEnvInteger "FCS_maxMB" (if sizeof<int> = 4 then 1700 else 3400)

    /// Maximum time share for a piece of background work before it should (cooperatively) yield
    /// to enable other requests to be serviced. Yielding means returning a continuation function
    /// (via an Eventually<_> value of case NotYetDone) that can be called as the next piece of work. 
    let maxTimeShareMilliseconds = 
        match System.Environment.GetEnvironmentVariable("FCS_MaxTimeShare") with 
        | null | "" -> 100L
        | s -> int64 s


//----------------------------------------------------------------------------
// Scopes. 
//--------------------------------------------------------------------------

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
[<RequireQualifiedAccess>]
[<NoEquality; NoComparison>]
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

[<RequireQualifiedAccess>]
type SemanticClassificationType =
    | ReferenceType
    | ValueType
    | UnionCase
    | Function
    | Property
    | MutableVar
    | Module
    | Printf
    | ComputationExpression
    | IntrinsicFunction
    | Enumeration
    | Interface
    | TypeArgument
    | Operator
    | Disposable
    
/// A TypeCheckInfo represents everything we get back from the typecheck of a file.
/// It acts like an in-memory database about the file.
/// It is effectively immutable and not updated: when we re-typecheck we just drop the previous
/// scope object on the floor and make a new one.
[<Sealed>]
type TypeCheckInfo
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
           checkAlive : (unit -> bool),
           textSnapshotInfo:obj option,
           implFileOpt: TypedImplFile option,
           openDeclarations: OpenDeclaration[]) = 

    let textSnapshotInfo = defaultArg textSnapshotInfo null
    let (|CNR|) (cnr:CapturedNameResolution) =
        (cnr.Pos, cnr.Item, cnr.ItemOccurence, cnr.DisplayEnv, cnr.NameResolutionEnv, cnr.AccessorDomain, cnr.Range)

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

        let bestAlmostIncludedSoFar = ref None 

        sResolutions.CapturedEnvs |> ResizeArray.iter (fun (possm,env,ad) -> 
            // take only ranges that strictly do not include cursorPos (all ranges that touch cursorPos were processed during 'Strict Inclusion' part)
            if rangeBeforePos possm cursorPos && not (posEq possm.End cursorPos) then 
                let contained = 
                    match mostDeeplyNestedEnclosingScope with 
                    | Some (bestm,_,_) -> rangeContainsRange bestm possm 
                    | None -> true 
                
                if contained then 
                    match !bestAlmostIncludedSoFar with 
                    | Some (rightm:range,_,_) -> 
                        if posGt possm.End rightm.End || 
                          (posEq possm.End rightm.End && posGt possm.Start rightm.Start) then
                            bestAlmostIncludedSoFar := Some (possm,env,ad)
                    | _ -> bestAlmostIncludedSoFar := Some (possm,env,ad))
        
        let resEnv = 
            match !bestAlmostIncludedSoFar, mostDeeplyNestedEnclosingScope with 
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

    let GetCapturedNameResolutions endOfNamesPos resolveOverloads =

        let quals = 
            match resolveOverloads with 
            | ResolveOverloads.Yes -> sResolutions.CapturedNameResolutions 
            | ResolveOverloads.No -> sResolutions.CapturedMethodGroupResolutions

        let quals = quals |> ResizeArray.filter (fun cnr ->  posEq cnr.Pos endOfNamesPos)
        
        quals

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
        | CNR(_,Item.Types(_,(ty :: _)), _, denv, nenv, ad, m) :: _, Some _ -> 
            let items = ResolveCompletionsInType ncenv nenv (ResolveCompletionTargets.All(ConstraintSolver.IsApplicableMethApprox g amap m)) m ad true ty 
            let items = List.map ItemWithNoInst items
            ReturnItemsOfType items g denv m filterCtors hasTextChangedSinceLastTypecheck 
        
        // Value reference from the name resolution. Primarily to disallow "let x.$ = 1"
        // In most of the cases, value references can be obtained from expression typings or from environment,
        // so we wouldn't have to handle values here. However, if we have something like:
        //   let varA = "string"
        //   let varA = if b then 0 else varA.
        // then the expression typings get confused (thinking 'varA:int'), so we use name resolution even for usual values.
        
        | CNR(_, Item.Value(vref), occurence, denv, nenv, ad, m) :: _, Some _ ->
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
        | CNR(_, _, _, denv, _, _, m) :: _, None -> 
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
        | CNR(_,Item.Types(_,(ty :: _)),_,_,_,_,_) :: _, Some _ -> Some ty
        | CNR(_, Item.Value(vref), occurence,_,_,_,_) :: _, Some _ ->
            if (occurence = ItemOccurence.Binding || occurence = ItemOccurence.Pattern) then None
            else Some (StripSelfRefCell(g, vref.BaseOrThisInfo, vref.TauType))
        | _, _ -> None

    let CollectParameters (methods: MethInfo list) amap m: Item list = 
        methods
        |> List.collect (fun meth ->
            match meth.GetParamDatas(amap, m, meth.FormalMethodInst) with
            | x :: _ -> x |> List.choose(fun (ParamData(_isParamArray, _isInArg, _isOutArg, _optArgInfo, _callerInfo, name, _, ty)) -> 
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
            | CNR(_, Item.CtorGroup(_, ((ctor :: _) as ctors)), _, denv, nenv, ad, m) :: _ ->
                let props = ResolveCompletionsInType ncenv nenv ResolveCompletionTargets.SettablePropertiesAndFields m ad false ctor.ApparentEnclosingType
                let parameters = CollectParameters ctors amap m
                let items = props @ parameters
                Some (denv, m, items)
            | CNR(_, Item.MethodGroup(_, methods, _), _, denv, nenv, ad, m) :: _ ->
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
            |> Seq.filter (fun (pos,ty,denv,_,_,_) -> 
                    // We only want expression types that end at the particular position in the file we are looking at.
                    let isLocationWeCareAbout = posEq pos endOfExprPos
                    // Get rid of function types.  True, given a 2-arg curried function "f x y", it is legal to do "(f x).GetType()",
                    // but you almost never want to do this in practice, and we choose not to offer up any intellisense for 
                    // F# function types.
                    let isFunction = isFunTy denv.g ty
                    isLocationWeCareAbout && not isFunction)
            |> Seq.toArray

        let thereWereSomeQuals = not (Array.isEmpty quals)
        // filter out errors

        let quals = quals 
                    |> Array.filter (fun (_,ty,denv,_,_,_) -> not (isTyparTy denv.g ty && (destTyparTy denv.g ty).IsFromError))
        thereWereSomeQuals, quals
    
    /// obtains captured typing for the given position
    /// if type of captured typing is record - returns list of record fields
    let GetRecdFieldsForExpr(r : range) = 
        let _, quals = GetExprTypingForPosition(r.End)
        let bestQual = 
            match quals with
            | [||] -> None
            | quals ->  
                quals |> Array.tryFind (fun (_,_,_,_,_,rq) -> 
                                            ignore(r)  // for breakpoint
                                            posEq r.Start rq.Start)
        match bestQual with
        | Some (_,ty,denv,_nenv,ad,m) when isRecdTy denv.g ty ->
            let items = NameResolution.ResolveRecordOrClassFieldsOfType ncenv m ad ty false
            Some (items, denv, m)
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
                                quals |> Array.tryFind (fun (_,_,_,_,_,r) -> 
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
                let (_,ty,denv,nenv,ad,m) = bestQual 
                let items = ResolveCompletionsInType ncenv nenv (ResolveCompletionTargets.All(ConstraintSolver.IsApplicableMethApprox g amap m)) m ad false ty 
                let items = items |> List.map ItemWithNoInst
                let items = items |> RemoveDuplicateItems g
                let items = items |> RemoveExplicitlySuppressed g
                let items = items |> FilterItemsForCtors filterCtors 
                GetPreciseCompletionListFromExprTypingsResult.Some((items,denv,m), ty)
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
        | Item.Types(_, ty :: _) when (isClassTy g ty) && not (isSealedTy g ty) -> true
        | _ -> false   

    let GetInterfaceCandidates = function
        | Item.ModuleOrNamespaces _ -> true
        | Item.Types(_, ty :: _) when (isInterfaceTy g ty) -> true
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
                | Some x -> tryDestAppTy g x
                | None ->
                    match lastDotPos |> Option.orElseWith (fun _ -> FindFirstNonWhitespacePosition lineStr (colAtEndOfNamesAndResidue - 1)) with
                    | Some p when lineStr.[p] = '.' ->
                        match FindFirstNonWhitespacePosition lineStr (p - 1) with
                        | Some colAtEndOfNames ->                 
                            let colAtEndOfNames = colAtEndOfNames + 1 // convert 0-based to 1-based
                            match TryGetTypeFromNameResolution(line, colAtEndOfNames, residueOpt, resolveOverloads) with
                            | Some x -> tryDestAppTy g x
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
                        Some (items |> List.map (CompletionItem (tryDestAppTy g ty) ValueNone), denv, m)
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
                               ValueSome(items |> List.map (CompletionItem (tryDestAppTy g ty) ValueNone), denv, m)
                           
                           | _ -> ValueNone

                       let globalResult =
                           match origLongIdentOpt with
                           | None | Some [] ->
                               let globalItems = 
                                   allSymbols() 
                                   |> List.filter (fun x -> not x.Symbol.IsExplicitlySuppressed)
                                   |> List.filter (fun x -> 
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
                    items |> List.filter (fun x -> match x.Item with Item.ModuleOrNamespaces _ -> true | _ -> false), denv, m)
            
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
                    GetDeclaredItems (parseResultsOpt, lineStr, origLongIdentOpt, colAtEndOfNamesAndResidue, residueOpt, lastDotPos, line, loc, filterCtors,resolveOverloads, hasTextChangedSinceLastTypecheck, isInRangeOperator, getAllSymbols)
        
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
                match GetDeclItemsForNamesAtPosition(ctok, parseResultsOpt, Some partialName.QualifyingIdents, Some partialName.PartialIdent, partialName.LastDotPos, line, lineStr, partialName.EndColumn + 1, ResolveTypeNamesToCtors, ResolveOverloads.Yes, getAllEntities, hasTextChangedSinceLastTypecheck) with
                | None -> FSharpDeclarationListInfo.Empty  
                | Some (items, denv, ctx, m) ->
                    let items = if isInterfaceFile then items |> List.filter (fun x -> IsValidSignatureFileItem x.Item) else items
                    let getAccessibility item = FSharpSymbol.GetAccessibility (FSharpSymbol.Create(cenv, item))
                    let currentNamespaceOrModule =
                        parseResultsOpt
                        |> Option.bind (fun x -> x.ParseTree)
                        |> Option.map (fun parsedInput -> UntypedParseImpl.GetFullNameOfSmallestModuleOrNamespaceAtPoint(parsedInput, mkPos line 0))
                    let isAttributeApplication = ctx = Some CompletionContext.AttributeApplication
                    FSharpDeclarationListInfo.Create(infoReader,m,denv,getAccessibility,items,reactorOps,currentNamespaceOrModule,isAttributeApplication,checkAlive))
            (fun msg -> 
                Trace.TraceInformation(sprintf "FCS: recovering from error in GetDeclarations: '%s'" msg)
                FSharpDeclarationListInfo.Error msg)

    /// Get the symbols for auto-complete items at a location
    member __.GetDeclarationListSymbols (ctok, parseResultsOpt, line, lineStr, partialName, getAllEntities, hasTextChangedSinceLastTypecheck) =
        let isInterfaceFile = SourceFileImpl.IsInterfaceFile mainInputFileName
        ErrorScope.Protect Range.range0 
            (fun () -> 
                match GetDeclItemsForNamesAtPosition(ctok, parseResultsOpt, Some partialName.QualifyingIdents, Some partialName.PartialIdent, partialName.LastDotPos, line, lineStr, partialName.EndColumn + 1, ResolveTypeNamesToCtors, ResolveOverloads.Yes, getAllEntities, hasTextChangedSinceLastTypecheck) with
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
                            (d.Item.DisplayName,n))

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
            | resolved :: _ // Take the first seen
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
                    match GetDeclItemsForNamesAtPosition(ctok, None,Some(names),None,None,line,lineStr,colAtEndOfNames,ResolveTypeNamesToCtors,ResolveOverloads.Yes,(fun() -> []),fun _ -> false) with
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
                match GetDeclItemsForNamesAtPosition(ctok, None, Some names, None, None, line, lineStr, colAtEndOfNames, ResolveTypeNamesToCtors, ResolveOverloads.No,(fun() -> []), fun _ -> false) with // F1 Keywords do not distinguish between overloads
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
                match GetDeclItemsForNamesAtPosition(ctok, None,namesOpt,None,None,line,lineStr,colAtEndOfNames,ResolveTypeNamesToCtors,ResolveOverloads.No,(fun() -> []),fun _ -> false) with
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
        match GetDeclItemsForNamesAtPosition (ctok, None,Some(names), None, None,line, lineStr, colAtEndOfNames, ResolveTypeNamesToCtors, ResolveOverloads.No,(fun() -> []),fun _ -> false) with
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
          match GetDeclItemsForNamesAtPosition (ctok, None,Some(names), None, None, line, lineStr, colAtEndOfNames, ResolveTypeNamesToCtors,ResolveOverloads.Yes,(fun() -> []), fun _ -> false) with
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
                  | Item.CtorGroup  (name, ProvidedMeth (_) :: _   )
                  | Item.MethodGroup(name, ProvidedMeth (_) :: _, _)
                  | Item.Property   (name, ProvidedProp (_) :: _   ) -> FSharpFindDeclFailureReason.ProvidedMember name             
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
        match GetDeclItemsForNamesAtPosition (ctok, None,Some(names), None, None, line, lineStr, colAtEndOfNames, ResolveTypeNamesToCtors, ResolveOverloads.Yes,(fun() -> []), fun _ -> false) with
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

    member __.GetSemanticClassification(range: range option) : (range * SemanticClassificationType) [] =
      ErrorScope.Protect Range.range0 
       (fun () -> 
        let (|LegitTypeOccurence|_|) = function
            | ItemOccurence.UseInType
            | ItemOccurence.UseInAttribute
            | ItemOccurence.Use _
            | ItemOccurence.Binding _
            | ItemOccurence.Pattern _ -> Some()
            | _ -> None

        let (|OptionalArgumentAttribute|_|) ttype =
            match ttype with
            | TType.TType_app(tref, _) when tref.Stamp = g.attrib_OptionalArgumentAttribute.TyconRef.Stamp -> Some()
            | _ -> None

        let (|KeywordIntrinsicValue|_|) (vref: ValRef) =
            if valRefEq g g.raise_vref vref ||
               valRefEq g g.reraise_vref vref ||
               valRefEq g g.typeof_vref vref ||
               valRefEq g g.typedefof_vref vref ||
               valRefEq g g.sizeof_vref vref 
               // TODO uncomment this after `nameof` operator is implemented
               // || valRefEq g g.nameof_vref vref
            then Some()
            else None
        
        let (|EnumCaseFieldInfo|_|) (rfinfo : RecdFieldInfo) =
            match rfinfo.TyconRef.TypeReprInfo with
            | TFSharpObjectRepr x ->
                match x.fsobjmodel_kind with
                | TTyconEnum -> Some ()
                | _ -> None
            | _ -> None

        let resolutions =
            match range with
            | Some range ->
                sResolutions.CapturedNameResolutions
                |> Seq.filter (fun cnr -> rangeContainsPos range cnr.Range.Start || rangeContainsPos range cnr.Range.End)
            | None -> 
                sResolutions.CapturedNameResolutions :> seq<_>

        let isDisposableTy (ty: TType) =
            protectAssemblyExplorationNoReraise false false (fun () -> Infos.ExistsHeadTypeInEntireHierarchy g amap range0 ty g.tcref_System_IDisposable)

        let isStructTyconRef (tyconRef: TyconRef) = 
            let ty = generalizedTyconRef tyconRef
            let underlyingTy = stripTyEqnsAndMeasureEqns g ty
            isStructTy g underlyingTy

        let isValRefMutable (vref: ValRef) =
            // Mutable values, ref cells, and non-inref byrefs are mutable.
            vref.IsMutable
            || Tastops.isRefCellTy g vref.Type
            || (Tastops.isByrefTy g vref.Type && not (Tastops.isInByrefTy g vref.Type))

        let isRecdFieldMutable (rfinfo: RecdFieldInfo) =
            (rfinfo.RecdField.IsMutable && rfinfo.LiteralValue.IsNone)
            || Tastops.isRefCellTy g rfinfo.RecdField.FormalType

        resolutions
        |> Seq.choose (fun cnr ->
            match cnr with
            // 'seq' in 'seq { ... }' gets colored as keywords
            | CNR(_, (Item.Value vref), ItemOccurence.Use, _, _, _, m) when valRefEq g g.seq_vref vref ->
                Some (m, SemanticClassificationType.ComputationExpression)
            | CNR(_, (Item.Value vref), _, _, _, _, m) when isValRefMutable vref ->
                Some (m, SemanticClassificationType.MutableVar)
            | CNR(_, Item.Value KeywordIntrinsicValue, ItemOccurence.Use, _, _, _, m) ->
                Some (m, SemanticClassificationType.IntrinsicFunction)
            | CNR(_, (Item.Value vref), _, _, _, _, m) when isFunction g vref.Type ->
                if valRefEq g g.range_op_vref vref || valRefEq g g.range_step_op_vref vref then 
                    None
                elif vref.IsPropertyGetterMethod || vref.IsPropertySetterMethod then
                    Some (m, SemanticClassificationType.Property)
                elif IsOperatorName vref.DisplayName then
                    Some (m, SemanticClassificationType.Operator)
                else
                    Some (m, SemanticClassificationType.Function)
            | CNR(_, Item.RecdField rfinfo, _, _, _, _, m) when isRecdFieldMutable rfinfo ->
                Some (m, SemanticClassificationType.MutableVar)
            | CNR(_, Item.RecdField rfinfo, _, _, _, _, m) when isFunction g rfinfo.FieldType ->
               Some (m, SemanticClassificationType.Function)
            | CNR(_, Item.RecdField EnumCaseFieldInfo, _, _, _, _, m) ->
                Some (m, SemanticClassificationType.Enumeration)
            | CNR(_, Item.MethodGroup _, _, _, _, _, m) ->
                Some (m, SemanticClassificationType.Function)
            // custom builders, custom operations get colored as keywords
            | CNR(_, (Item.CustomBuilder _ | Item.CustomOperation _), ItemOccurence.Use, _, _, _, m) ->
                Some (m, SemanticClassificationType.ComputationExpression)
            // types get colored as types when they occur in syntactic types or custom attributes
            // typevariables get colored as types when they occur in syntactic types custom builders, custom operations get colored as keywords
            | CNR(_, Item.Types (_, [OptionalArgumentAttribute]), LegitTypeOccurence, _, _, _, _) -> None
            | CNR(_, Item.CtorGroup(_, [MethInfo.FSMeth(_, OptionalArgumentAttribute, _, _)]), LegitTypeOccurence, _, _, _, _) -> None
            | CNR(_, Item.Types(_, types), LegitTypeOccurence, _, _, _, m) when types |> List.exists (isInterfaceTy g) -> 
                Some (m, SemanticClassificationType.Interface)
            | CNR(_, Item.Types(_, types), LegitTypeOccurence, _, _, _, m) when types |> List.exists (isStructTy g) -> 
                Some (m, SemanticClassificationType.ValueType)
            | CNR(_, Item.Types(_, TType_app(tyconRef, TType_measure _ :: _) :: _), LegitTypeOccurence, _, _, _, m) when isStructTyconRef tyconRef ->
                Some (m, SemanticClassificationType.ValueType)
            | CNR(_, Item.Types(_, types), LegitTypeOccurence, _, _, _, m) when types |> List.exists isDisposableTy ->
                Some (m, SemanticClassificationType.Disposable)
            | CNR(_, Item.Types _, LegitTypeOccurence, _, _, _, m) -> 
                Some (m, SemanticClassificationType.ReferenceType)
            | CNR(_, (Item.TypeVar _ ), LegitTypeOccurence, _, _, _, m) ->
                Some (m, SemanticClassificationType.TypeArgument)
            | CNR(_, Item.UnqualifiedType tyconRefs, LegitTypeOccurence, _, _, _, m) ->
                if tyconRefs |> List.exists (fun tyconRef -> tyconRef.Deref.IsStructOrEnumTycon) then
                    Some (m, SemanticClassificationType.ValueType)
                else Some (m, SemanticClassificationType.ReferenceType)
            | CNR(_, Item.CtorGroup(_, minfos), LegitTypeOccurence, _, _, _, m) ->
                if minfos |> List.exists (fun minfo -> isStructTy g minfo.ApparentEnclosingType) then
                    Some (m, SemanticClassificationType.ValueType)
                else Some (m, SemanticClassificationType.ReferenceType)
            | CNR(_, Item.ExnCase _, LegitTypeOccurence, _, _, _, m) ->
                Some (m, SemanticClassificationType.ReferenceType)
            | CNR(_, Item.ModuleOrNamespaces refs, LegitTypeOccurence, _, _, _, m) when refs |> List.exists (fun x -> x.IsModule) ->
                Some (m, SemanticClassificationType.Module)
            | CNR(_, (Item.ActivePatternCase _ | Item.UnionCase _ | Item.ActivePatternResult _), _, _, _, _, m) ->
                Some (m, SemanticClassificationType.UnionCase)
            | _ -> None)
        |> Seq.toArray
        |> Array.append (sSymbolUses.GetFormatSpecifierLocationsAndArity() |> Array.map (fun m -> fst m, SemanticClassificationType.Printf))
       ) 
       (fun msg -> 
           Trace.TraceInformation(sprintf "FCS: recovering from error in GetSemanticClassification: '%s'" msg)
           Array.empty)

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

    static member FromTcConfigBuidler(tcConfigB: TcConfigBuilder, sourceFiles, isInteractive: bool) =
        {
          SourceFiles = sourceFiles
          ConditionalCompilationDefines = tcConfigB.conditionalCompilationDefines
          ErrorSeverityOptions = tcConfigB.errorSeverityOptions
          IsInteractive = isInteractive
          LightSyntax = tcConfigB.light
          CompilingFsLib = tcConfigB.compilingFslib
          IsExe = tcConfigB.target.IsExe
        }

module internal Parser = 

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
        member x.ErrorLogger = errorLogger
        member x.CollectedDiagnostics = errorsAndWarningsCollector.ToArray()
        member x.ErrorCount = errorCount
        member x.ErrorSeverityOptions with set opts = options <- opts
        member x.AnyErrors = errorCount > 0

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
        let lexargs = mkLexargs(fileName, defines, lightSyntaxStatus, lexResourceManager, ref [], errHandler.ErrorLogger, PathMap.empty)
        let lexargs = { lexargs with applyLineDirectives = false }

        let tokenizer = LexFilter.LexFilter(lightSyntaxStatus, options.CompilingFsLib, Lexer.token lexargs true, lexbuf)
        tokenizer.Lexer

    let createLexbuf sourceText =
        UnicodeLexing.SourceTextAsLexbuf(sourceText)

    let matchBraces(sourceText, fileName, options: FSharpParsingOptions, userOpName: string, suggestNamesForErrors: bool) =
        let delayedLogger = CapturingErrorLogger("matchBraces")
        use _unwindEL = PushErrorLoggerPhaseUntilUnwind (fun _ -> delayedLogger)
        use _unwindBP = PushThreadBuildPhaseUntilUnwind BuildPhase.Parse

        Trace.TraceInformation("FCS: {0}.{1} ({2})", userOpName, "matchBraces", fileName)

        // Make sure there is an ErrorLogger installed whenever we do stuff that might record errors, even if we ultimately ignore the errors
        let delayedLogger = CapturingErrorLogger("matchBraces")
        use _unwindEL = PushErrorLoggerPhaseUntilUnwind (fun _ -> delayedLogger)
        use _unwindBP = PushThreadBuildPhaseUntilUnwind BuildPhase.Parse
        
        let matchingBraces = new ResizeArray<_>()
        Lexhelp.usingLexbufForParsing(createLexbuf sourceText, fileName) (fun lexbuf ->
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
            Lexhelp.usingLexbufForParsing(createLexbuf sourceText, fileName) (fun lexbuf ->
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

    /// Indicates if the type check got aborted because it is no longer relevant.
    type TypeCheckAborted = Yes | No of TypeCheckInfo

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
           checkAlive : (unit -> bool),
           textSnapshotInfo : obj option,
           userOpName: string,
           suggestNamesForErrors: bool) = 
        
        async {
            use _logBlock = Logger.LogBlock LogCompilerFunctionId.Service_CheckOneFile

            match parseResults.ParseTree with 
            // When processing the following cases, we don't need to type-check
            | None -> return [||], TypeCheckAborted.Yes
                   
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
                             
                            return result |> Option.map (fun ((tcEnvAtEnd, _, implFiles, ccuSigsForFiles), tcState) -> tcEnvAtEnd, implFiles, ccuSigsForFiles, tcState)
                        with e ->
                            errorR e
                            return Some(tcState.TcEnvFromSignatures, [], [NewEmptyModuleOrNamespaceType Namespace], tcState)
                    }
                
                let errors = errHandler.CollectedDiagnostics
                
                match resOpt with
                | Some (tcEnvAtEnd, implFiles, ccuSigsForFiles, tcState) ->
                    let scope = 
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
                                      checkAlive,
                                      textSnapshotInfo,
                                      List.tryHead implFiles,
                                      sink.GetOpenDeclarations())     
                    return errors, TypeCheckAborted.No scope
                | None -> 
                    return errors, TypeCheckAborted.Yes
        }

type UnresolvedReferencesSet = UnresolvedReferencesSet of UnresolvedAssemblyReference list

// NOTE: may be better just to move to optional arguments here
type FSharpProjectOptions =
    { 
      ProjectFileName: string
      ProjectId: string option
      SourceFiles: string[]
      OtherOptions: string[]
      ReferencedProjects: (string * FSharpProjectOptions)[]
      IsIncompleteTypeCheckEnvironment : bool
      UseScriptResolutionRules : bool      
      LoadTime : System.DateTime
      UnresolvedReferences : UnresolvedReferencesSet option
      OriginalLoadReferences: (range * string) list
      ExtraProjectInfo : obj option
      Stamp : int64 option
    }
    member x.ProjectOptions = x.OtherOptions
    /// Whether the two parse options refer to the same project.
    static member UseSameProject(options1,options2) =
        match options1.ProjectId, options2.ProjectId with
        | Some(projectId1), Some(projectId2) when not (String.IsNullOrWhiteSpace(projectId1)) && not (String.IsNullOrWhiteSpace(projectId2)) -> 
            projectId1 = projectId2
        | Some(_), Some(_)
        | None, None -> options1.ProjectFileName = options2.ProjectFileName
        | _ -> false

    /// Compare two options sets with respect to the parts of the options that are important to building.
    static member AreSameForChecking(options1,options2) =
        match options1.Stamp, options2.Stamp with 
        | Some x, Some y -> (x = y)
        | _ -> 
        FSharpProjectOptions.UseSameProject(options1, options2) &&
        options1.SourceFiles = options2.SourceFiles &&
        options1.OtherOptions = options2.OtherOptions &&
        options1.UnresolvedReferences = options2.UnresolvedReferences &&
        options1.OriginalLoadReferences = options2.OriginalLoadReferences &&
        options1.ReferencedProjects.Length = options2.ReferencedProjects.Length &&
        Array.forall2 (fun (n1,a) (n2,b) ->
            n1 = n2 && 
            FSharpProjectOptions.AreSameForChecking(a,b)) options1.ReferencedProjects options2.ReferencedProjects &&
        options1.LoadTime = options2.LoadTime

    /// Compute the project directory.
    member po.ProjectDirectory = System.IO.Path.GetDirectoryName(po.ProjectFileName)
    override this.ToString() = "FSharpProjectOptions(" + this.ProjectFileName + ")"
 

[<Sealed>] 
type FSharpProjectContext(thisCcu: CcuThunk, assemblies: FSharpAssembly list, ad: AccessorDomain) =

    /// Get the assemblies referenced
    member __.GetReferencedAssemblies() = assemblies

    member __.AccessibilityRights = FSharpAccessibilityRights(thisCcu, ad)


[<Sealed>]
// 'details' is an option because the creation of the tcGlobals etc. for the project may have failed.
type FSharpCheckProjectResults(projectFileName:string, tcConfigOption, keepAssemblyContents, errors: FSharpErrorInfo[], 
                               details:(TcGlobals * TcImports * CcuThunk * ModuleOrNamespaceType * TcSymbolUses list * TopAttribs option * CompileOps.IRawFSharpAssemblyData option * ILAssemblyRef * AccessorDomain * TypedImplFile list option * string[]) option) =

    let getDetails() = 
        match details with 
        | None -> invalidOp ("The project has no results due to critical errors in the project options. Check the HasCriticalErrors before accessing the detailed results. Errors: " + String.concat "\n" [ for e in errors -> e.Message ])
        | Some d -> d

    let getTcConfig() = 
        match tcConfigOption with 
        | None -> invalidOp ("The project has no results due to critical errors in the project options. Check the HasCriticalErrors before accessing the detailed results. Errors: " + String.concat "\n" [ for e in errors -> e.Message ])
        | Some d -> d

    member info.Errors = errors

    member info.HasCriticalErrors = details.IsNone

    member info.AssemblySignature =  
        let (tcGlobals, tcImports, thisCcu, ccuSig, _tcSymbolUses, topAttribs, _tcAssemblyData, _ilAssemRef, _ad, _tcAssemblyExpr, _dependencyFiles) = getDetails()
        FSharpAssemblySignature(tcGlobals, thisCcu, ccuSig, tcImports, topAttribs, ccuSig)

    member info.TypedImplementionFiles =
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

    member info.GetOptimizedAssemblyContents() =  
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
    member info.GetUsesOfSymbol(symbol:FSharpSymbol) = 
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

    override info.ToString() = "FSharpCheckProjectResults(" + projectFileName + ")"

[<Sealed>]
/// A live object of this type keeps the background corresponding background builder (and type providers) alive (through reference-counting).
//
// There is an important property of all the objects returned by the methods of this type: they do not require 
// the corresponding background builder to be alive. That is, they are simply plain-old-data through pre-formatting of all result text.
type FSharpCheckFileResults(filename: string, errors: FSharpErrorInfo[], scopeOptX: TypeCheckInfo option, dependencyFiles: string[], builderX: IncrementalBuilder option, reactorOpsX:IReactorOperations, keepAssemblyContents: bool) =

    // This may be None initially, or may be set to None when the object is disposed or finalized
    let mutable details = match scopeOptX with None -> None | Some scopeX -> Some (scopeX, builderX, reactorOpsX)

    // Increment the usage count on the IncrementalBuilder. We want to keep the IncrementalBuilder and all associated
    // resources and type providers alive for the duration of the lifetime of this object.
    let decrementer = 
        match details with 
        | Some (_,builderOpt,_) -> IncrementalBuilder.KeepBuilderAlive builderOpt
        | _ -> { new System.IDisposable with member x.Dispose() = () } 

    let mutable disposed = false

    let dispose() = 
       if not disposed then 
           disposed <- true 
           match details with 
           | Some (_,_,reactor) -> 
               // Make sure we run disposal in the reactor thread, since it may trigger type provider disposals etc.
               details <- None
               reactor.EnqueueOp ("GCFinalizer","FSharpCheckFileResults.DecrementUsageCountOnIncrementalBuilder", filename, fun ctok -> 
                   RequireCompilationThread ctok
                   decrementer.Dispose())
           | _ -> () 

    // Run an operation that needs to access a builder and be run in the reactor thread
    let reactorOp userOpName opName dflt f = 
      async {
        match details with
        | None -> 
            return dflt
        | Some (_, Some builder, _) when not builder.IsAlive -> 
            System.Diagnostics.Debug.Assert(false,"unexpected dead builder") 
            return dflt
        | Some (scope, builderOpt, reactor) -> 
            // Increment the usage count to ensure the builder doesn't get released while running operations asynchronously. 
            use _unwind = IncrementalBuilder.KeepBuilderAlive builderOpt
            let! res = reactor.EnqueueAndAwaitOpAsync(userOpName, opName, filename, fun ctok ->  f ctok scope |> cancellable.Return)
            return res
      }

    // Run an operation that can be called from any thread
    let threadSafeOp dflt f = 
        match details with
        | None -> dflt()
        | Some (scope, _builderOpt, _ops) -> f scope

    // At the moment we only dispose on finalize - we never explicitly dispose these objects. Explicitly disposing is not
    // really worth much since the underlying project builds are likely to still be in the incrementalBuilder cache.
    override info.Finalize() = dispose() 

    member info.Errors = errors

    member info.HasFullTypeCheckInfo = details.IsSome
    
    /// Intellisense autocompletions
    member info.GetDeclarationListInfo(parseResultsOpt, line, lineStr, partialName, ?getAllEntities, ?hasTextChangedSinceLastTypecheck, ?userOpName: string) = 
        let userOpName = defaultArg userOpName "Unknown"
        let getAllEntities = defaultArg getAllEntities (fun() -> [])
        let hasTextChangedSinceLastTypecheck = defaultArg hasTextChangedSinceLastTypecheck (fun _ -> false)
        reactorOp userOpName "GetDeclarations" FSharpDeclarationListInfo.Empty (fun ctok scope -> 
            scope.GetDeclarations(ctok, parseResultsOpt, line, lineStr, partialName, getAllEntities, hasTextChangedSinceLastTypecheck))

    member info.GetDeclarationListSymbols(parseResultsOpt, line, lineStr, partialName, ?getAllEntities, ?hasTextChangedSinceLastTypecheck, ?userOpName: string) = 
        let userOpName = defaultArg userOpName "Unknown"
        let hasTextChangedSinceLastTypecheck = defaultArg hasTextChangedSinceLastTypecheck (fun _ -> false)
        let getAllEntities = defaultArg getAllEntities (fun() -> [])
        reactorOp userOpName "GetDeclarationListSymbols" List.empty (fun ctok scope -> scope.GetDeclarationListSymbols(ctok, parseResultsOpt, line, lineStr, partialName, getAllEntities, hasTextChangedSinceLastTypecheck))

    /// Resolve the names at the given location to give a data tip 
    member info.GetStructuredToolTipText(line, colAtEndOfNames, lineStr, names, tokenTag, ?userOpName: string) = 
        let userOpName = defaultArg userOpName "Unknown"
        let dflt = FSharpToolTipText []
        match tokenTagToTokenId tokenTag with 
        | TOKEN_IDENT -> 
            reactorOp userOpName "GetStructuredToolTipText" dflt (fun ctok scope -> scope.GetStructuredToolTipText(ctok, line, lineStr, colAtEndOfNames, names))
        | TOKEN_STRING | TOKEN_STRING_TEXT -> 
            reactorOp userOpName "GetReferenceResolutionToolTipText" dflt (fun ctok scope -> scope.GetReferenceResolutionStructuredToolTipText(ctok, line, colAtEndOfNames) )
        | _ -> 
            async.Return dflt

    
    member info.GetToolTipText(line, colAtEndOfNames, lineStr, names, tokenTag, userOpName) = 
        info.GetStructuredToolTipText(line, colAtEndOfNames, lineStr, names, tokenTag, ?userOpName=userOpName)
        |> Tooltips.Map Tooltips.ToFSharpToolTipText

    member info.GetF1Keyword (line, colAtEndOfNames, lineStr, names, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        reactorOp userOpName "GetF1Keyword" None (fun ctok scope -> 
            scope.GetF1Keyword (ctok, line, lineStr, colAtEndOfNames, names))

    // Resolve the names at the given location to a set of methods
    member info.GetMethods(line, colAtEndOfNames, lineStr, names, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        let dflt = FSharpMethodGroup("",[| |])
        reactorOp userOpName "GetMethods" dflt (fun ctok scope -> 
            scope.GetMethods (ctok, line, lineStr, colAtEndOfNames, names))
            
    member info.GetDeclarationLocation (line, colAtEndOfNames, lineStr, names, ?preferFlag, ?userOpName: string) = 
        let userOpName = defaultArg userOpName "Unknown"
        let dflt = FSharpFindDeclResult.DeclNotFound (FSharpFindDeclFailureReason.Unknown "")
        reactorOp userOpName "GetDeclarationLocation" dflt (fun ctok scope -> 
            scope.GetDeclarationLocation (ctok, line, lineStr, colAtEndOfNames, names, preferFlag))

    member info.GetSymbolUseAtLocation (line, colAtEndOfNames, lineStr, names, ?userOpName: string) = 
        let userOpName = defaultArg userOpName "Unknown"
        reactorOp userOpName "GetSymbolUseAtLocation" None (fun ctok scope -> 
            scope.GetSymbolUseAtLocation (ctok, line, lineStr, colAtEndOfNames, names)
            |> Option.map (fun (sym,denv,m) -> FSharpSymbolUse(scope.TcGlobals,denv,sym,ItemOccurence.Use,m)))

    member info.GetMethodsAsSymbols (line, colAtEndOfNames, lineStr, names, ?userOpName: string) = 
        let userOpName = defaultArg userOpName "Unknown"
        reactorOp userOpName "GetMethodsAsSymbols" None (fun ctok scope -> 
            scope.GetMethodsAsSymbols (ctok, line, lineStr, colAtEndOfNames, names)
            |> Option.map (fun (symbols,denv,m) ->
                symbols |> List.map (fun sym -> FSharpSymbolUse(scope.TcGlobals,denv,sym,ItemOccurence.Use,m))))

    member info.GetSymbolAtLocation (line, colAtEndOfNames, lineStr, names, ?userOpName: string) = 
        let userOpName = defaultArg userOpName "Unknown"
        reactorOp userOpName "GetSymbolAtLocation" None (fun ctok scope -> 
            scope.GetSymbolUseAtLocation (ctok, line, lineStr, colAtEndOfNames, names)
            |> Option.map (fun (sym,_,_) -> sym))

    member info.GetFormatSpecifierLocations() = 
        info.GetFormatSpecifierLocationsAndArity() |> Array.map fst

    member info.GetFormatSpecifierLocationsAndArity() = 
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

    member info.GetAllUsesOfAllSymbolsInFile() = 
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

    member info.GetUsesOfSymbolInFile(symbol:FSharpSymbol) = 
        threadSafeOp 
            (fun () -> [| |]) 
            (fun scope -> 
                [| for symbolUse in scope.ScopeSymbolUses.GetUsesOfSymbol(symbol.Item) |> Seq.distinctBy (fun symbolUse -> symbolUse.ItemOccurence, symbolUse.Range) do
                     if symbolUse.ItemOccurence <> ItemOccurence.RelatedText then
                        yield FSharpSymbolUse(scope.TcGlobals, symbolUse.DisplayEnv, symbol, symbolUse.ItemOccurence, symbolUse.Range) |])
         |> async.Return 

    member info.GetVisibleNamespacesAndModulesAtPoint(pos: pos) = 
        threadSafeOp 
            (fun () -> [| |]) 
            (fun scope -> scope.GetVisibleNamespacesAndModulesAtPosition(pos) |> List.toArray)
         |> async.Return 

    member info.IsRelativeNameResolvable(pos: pos, plid: string list, item: Item, ?userOpName: string) = 
        let userOpName = defaultArg userOpName "Unknown"
        reactorOp userOpName "IsRelativeNameResolvable" true (fun ctok scope -> 
            RequireCompilationThread ctok
            scope.IsRelativeNameResolvable(pos, plid, item))

    member info.IsRelativeNameResolvableFromSymbol(pos: pos, plid: string list, symbol: FSharpSymbol, ?userOpName: string) = 
        let userOpName = defaultArg userOpName "Unknown"
        reactorOp userOpName "IsRelativeNameResolvableFromSymbol" true (fun ctok scope -> 
            RequireCompilationThread ctok
            scope.IsRelativeNameResolvableFromSymbol(pos, plid, symbol))
    
    member info.GetDisplayEnvForPos(pos: pos) : Async<DisplayEnv option> =
        let userOpName = "CodeLens"
        reactorOp userOpName "GetDisplayContextAtPos" None (fun ctok scope -> 
            DoesNotRequireCompilerThreadTokenAndCouldPossiblyBeMadeConcurrent ctok
            let (nenv, _), _ = scope.GetBestDisplayEnvForPos pos
            Some nenv.DisplayEnv)
            
    member info.ImplementationFile =
        if not keepAssemblyContents then invalidOp "The 'keepAssemblyContents' flag must be set to true on the FSharpChecker in order to access the checked contents of assemblies"
        scopeOptX 
        |> Option.map (fun scope -> 
            let cenv = SymbolEnv(scope.TcGlobals, scope.ThisCcu, Some scope.CcuSigForFile, scope.TcImports)
            scope.ImplementationFile |> Option.map (fun implFile -> FSharpImplementationFileContents(cenv, implFile)))
        |> Option.defaultValue None

    member info.OpenDeclarations =
        scopeOptX 
        |> Option.map (fun scope -> 
            let cenv = scope.SymbolEnv
            scope.OpenDeclarations |> Array.map (fun x -> FSharpOpenDeclaration(x.LongId, x.Range, (x.Modules |> List.map (fun x -> FSharpEntity(cenv, x))), x.AppliedScope, x.IsOwnNamespace)))
        |> Option.defaultValue [| |]

    override info.ToString() = "FSharpCheckFileResults(" + filename + ")"

//----------------------------------------------------------------------------
// BackgroundCompiler
//

[<NoComparison>]
type FSharpCheckFileAnswer =
    | Aborted
    | Succeeded of FSharpCheckFileResults   
        

/// Callback that indicates whether a requested result has become obsolete.    
[<NoComparison;NoEquality>]
type IsResultObsolete = 
    | IsResultObsolete of (unit->bool)


[<AutoOpen>]
module Helpers = 

    // Look for DLLs in the location of the service DLL first.
    let defaultFSharpBinariesDir = FSharpEnvironment.BinFolderOfDefaultFSharpCompiler(Some(typeof<FSharpCheckFileAnswer>.Assembly.Location)).Value

    /// Determine whether two (fileName,options) keys are identical w.r.t. affect on checking
    let AreSameForChecking2((fileName1: string, options1: FSharpProjectOptions), (fileName2, options2)) =
        (fileName1 = fileName2) 
        && FSharpProjectOptions.AreSameForChecking(options1,options2)
        
    /// Determine whether two (fileName,options) keys should be identical w.r.t. resource usage
    let AreSubsumable2((fileName1:string,o1:FSharpProjectOptions),(fileName2:string,o2:FSharpProjectOptions)) =
        (fileName1 = fileName2)
        && FSharpProjectOptions.UseSameProject(o1,o2)

    /// Determine whether two (fileName,sourceText,options) keys should be identical w.r.t. parsing
    let AreSameForParsing((fileName1: string, source1Hash: int, options1), (fileName2, source2Hash, options2)) =
        fileName1 = fileName2 && options1 = options2 && source1Hash = source2Hash

    let AreSimilarForParsing((fileName1, _, _), (fileName2, _, _)) =
        fileName1 = fileName2
        
    /// Determine whether two (fileName,sourceText,options) keys should be identical w.r.t. checking
    let AreSameForChecking3((fileName1: string, source1Hash: int, options1: FSharpProjectOptions), (fileName2, source2Hash, options2)) =
        (fileName1 = fileName2) 
        && FSharpProjectOptions.AreSameForChecking(options1,options2)
        && source1Hash = source2Hash

    /// Determine whether two (fileName,sourceText,options) keys should be identical w.r.t. resource usage
    let AreSubsumable3((fileName1:string,_,o1:FSharpProjectOptions),(fileName2:string,_,o2:FSharpProjectOptions)) =
        (fileName1 = fileName2)
        && FSharpProjectOptions.UseSameProject(o1,o2)

module CompileHelpers =
    let mkCompilationErorHandlers() = 
        let errors = ResizeArray<_>()

        let errorSink isError exn = 
            let mainError, relatedErrors = SplitRelatedDiagnostics exn
            let oneError e = errors.Add(FSharpErrorInfo.CreateFromException (e, isError, Range.range0, true)) // Suggest names for errors
            oneError mainError
            List.iter oneError relatedErrors

        let errorLogger = 
            { new ErrorLogger("CompileAPI") with 
                member x.DiagnosticSink(exn, isError) = errorSink isError exn
                member x.ErrorCount = errors |> Seq.filter (fun e -> e.Severity = FSharpErrorSeverity.Error) |> Seq.length }

        let loggerProvider = 
            { new ErrorLoggerProvider() with 
                member x.CreateErrorLoggerUpToMaxErrors(_tcConfigBuilder, _exiter) = errorLogger    }
        errors, errorLogger, loggerProvider

    let tryCompile errorLogger f = 
        use unwindParsePhase = PushThreadBuildPhaseUntilUnwind (BuildPhase.Parse)            
        use unwindEL_2 = PushErrorLoggerPhaseUntilUnwind (fun _ -> errorLogger)
        let exiter = { new Exiter with member x.Exit n = raise StopProcessing }
        try 
            f exiter
            0
        with e -> 
            stopProcessingRecovery e Range.range0
            1

    /// Compile using the given flags.  Source files names are resolved via the FileSystem API. The output file must be given by a -o flag. 
    let compileFromArgs (ctok, argv: string[], legacyReferenceResolver, tcImportsCapture, dynamicAssemblyCreator)  = 
    
        let errors, errorLogger, loggerProvider = mkCompilationErorHandlers()
        let result = 
            tryCompile errorLogger (fun exiter -> 
                mainCompile (ctok, argv, legacyReferenceResolver, (*bannerAlreadyPrinted*)true, ReduceMemoryFlag.Yes, CopyFSharpCoreFlag.No, exiter, loggerProvider, tcImportsCapture, dynamicAssemblyCreator) )
    
        errors.ToArray(), result

    let compileFromAsts (ctok, legacyReferenceResolver, asts, assemblyName, outFile, dependencies, noframework, pdbFile, executable, tcImportsCapture, dynamicAssemblyCreator) =

        let errors, errorLogger, loggerProvider = mkCompilationErorHandlers()
    
        let executable = defaultArg executable true
        let target = if executable then CompilerTarget.ConsoleExe else CompilerTarget.Dll
    
        let result = 
            tryCompile errorLogger (fun exiter -> 
                compileOfAst (ctok, legacyReferenceResolver, ReduceMemoryFlag.Yes, assemblyName, target, outFile, pdbFile, dependencies, noframework, exiter, loggerProvider, asts, tcImportsCapture, dynamicAssemblyCreator))

        errors.ToArray(), result

    let createDynamicAssembly (ctok, debugInfo: bool, tcImportsRef: TcImports option ref, execute: bool, assemblyBuilderRef: _ option ref) (tcGlobals:TcGlobals, outfile, ilxMainModule) =

        // Create an assembly builder
        let assemblyName = System.Reflection.AssemblyName(System.IO.Path.GetFileNameWithoutExtension outfile)
        let flags = System.Reflection.Emit.AssemblyBuilderAccess.Run
#if FX_NO_APP_DOMAINS
        let assemblyBuilder = System.Reflection.Emit.AssemblyBuilder.DefineDynamicAssembly(assemblyName, flags)
        let moduleBuilder = assemblyBuilder.DefineDynamicModule("IncrementalModule")
#else
        let assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, flags)
        let moduleBuilder = assemblyBuilder.DefineDynamicModule("IncrementalModule", debugInfo)
#endif            
        // Omit resources in dynamic assemblies, because the module builder is constructed without a filename the module 
        // is tagged as transient and as such DefineManifestResource will throw an invalid operation if resources are present.
        // 
        // Also, the dynamic assembly creator can't currently handle types called "<Module>" from statically linked assemblies.
        let ilxMainModule = 
            { ilxMainModule with 
                TypeDefs = ilxMainModule.TypeDefs.AsList |> List.filter (fun td -> not (isTypeNameForGlobalFunctions td.Name)) |> mkILTypeDefs
                Resources=mkILResources [] }

        // The function used to resolve typees while emitting the code
        let assemblyResolver s = 
            match tcImportsRef.Value.Value.TryFindExistingFullyQualifiedPathByExactAssemblyRef (ctok, s) with 
            | Some res -> Some (Choice1Of2 res)
            | None -> None

        // Emit the code
        let _emEnv,execs = ILRuntimeWriter.emitModuleFragment(tcGlobals.ilg, ILRuntimeWriter.emEnv0, assemblyBuilder, moduleBuilder, ilxMainModule, debugInfo, assemblyResolver, tcGlobals.TryFindSysILTypeRef)

        // Execute the top-level initialization, if requested
        if execute then 
            for exec in execs do 
                match exec() with 
                | None -> ()
                | Some exn -> 
                    PreserveStackTrace(exn)
                    raise exn

        // Register the reflected definitions for the dynamically generated assembly
        for resource in ilxMainModule.Resources.AsList do 
            if IsReflectedDefinitionsResource resource then 
                Quotations.Expr.RegisterReflectedDefinitions (assemblyBuilder, moduleBuilder.Name, resource.GetBytes())

        // Save the result
        assemblyBuilderRef := Some assemblyBuilder
        
    let setOutputStreams execute = 
        // Set the output streams, if requested
        match execute with
        | Some (writer,error) -> 
            System.Console.SetOut writer
            System.Console.SetError error
        | None -> ()

type SourceTextHash = int        
type FileName = string      
type FilePath = string
type ProjectPath = string
type FileVersion = int

type ParseCacheLockToken() = interface LockToken
type ScriptClosureCacheToken() = interface LockToken


// There is only one instance of this type, held in FSharpChecker
type BackgroundCompiler(legacyReferenceResolver, projectCacheSize, keepAssemblyContents, keepAllBackgroundResolutions, tryGetMetadataSnapshot, suggestNamesForErrors) as self =
    // STATIC ROOT: FSharpLanguageServiceTestable.FSharpChecker.backgroundCompiler.reactor: The one and only Reactor
    let reactor = Reactor.Singleton
    let beforeFileChecked = Event<string * obj option>()
    let fileParsed = Event<string * obj option>()
    let fileChecked = Event<string * obj option>()
    let projectChecked = Event<string * obj option>()


    let mutable implicitlyStartBackgroundWork = true
    let reactorOps = 
        { new IReactorOperations with 
                member __.EnqueueAndAwaitOpAsync (userOpName, opName, opArg, op) = reactor.EnqueueAndAwaitOpAsync (userOpName, opName, opArg, op)
                member __.EnqueueOp (userOpName, opName, opArg, op) = reactor.EnqueueOp (userOpName, opName, opArg, op) }

    // STATIC ROOT: FSharpLanguageServiceTestable.FSharpChecker.backgroundCompiler.scriptClosureCache 
    /// Information about the derived script closure.
    let scriptClosureCache = 
        MruCache<ScriptClosureCacheToken, FSharpProjectOptions, LoadClosure>(projectCacheSize, 
            areSame=FSharpProjectOptions.AreSameForChecking, 
            areSimilar=FSharpProjectOptions.UseSameProject)

    let scriptClosureCacheLock = Lock<ScriptClosureCacheToken>()
    let frameworkTcImportsCache = FrameworkImportsCache(frameworkTcImportsCacheStrongSize)

    /// CreateOneIncrementalBuilder (for background type checking). Note that fsc.fs also
    /// creates an incremental builder used by the command line compiler.
    let CreateOneIncrementalBuilder (ctok, options:FSharpProjectOptions, userOpName) = 
      cancellable {
        Trace.TraceInformation("FCS: {0}.{1} ({2})", userOpName, "CreateOneIncrementalBuilder", options.ProjectFileName)
        let projectReferences =  
            [ for (nm,opts) in options.ReferencedProjects do
               
               // Don't use cross-project references for FSharp.Core, since various bits of code require a concrete FSharp.Core to exist on-disk.
               // The only solutions that have these cross-project references to FSharp.Core are VisualFSharp.sln and FSharp.sln. The only ramification
               // of this is that you need to build FSharp.Core to get intellisense in those projects.

               if (try Path.GetFileNameWithoutExtension(nm) with _ -> "") <> GetFSharpCoreLibraryName() then

                 yield
                    { new IProjectReference with 
                        member x.EvaluateRawContents(ctok) = 
                          cancellable {
                            Trace.TraceInformation("FCS: {0}.{1} ({2})", userOpName, "ParseAndCheckProjectImpl", nm)
                            let! r = self.ParseAndCheckProjectImpl(opts, ctok, userOpName + ".CheckReferencedProject("+nm+")")
                            return r.RawFSharpAssemblyData 
                          }
                        member x.TryGetLogicalTimeStamp(cache, ctok) = 
                            self.TryGetLogicalTimeStampForProject(cache, ctok, opts, userOpName + ".TimeStampReferencedProject("+nm+")")
                        member x.FileName = nm } ]

        let loadClosure = scriptClosureCacheLock.AcquireLock (fun ltok -> scriptClosureCache.TryGet (ltok, options))
        let! builderOpt, diagnostics = 
            IncrementalBuilder.TryCreateBackgroundBuilderForProjectOptions
                  (ctok, legacyReferenceResolver, defaultFSharpBinariesDir, frameworkTcImportsCache, loadClosure, Array.toList options.SourceFiles, 
                   Array.toList options.OtherOptions, projectReferences, options.ProjectDirectory, 
                   options.UseScriptResolutionRules, keepAssemblyContents, keepAllBackgroundResolutions, maxTimeShareMilliseconds,
                   tryGetMetadataSnapshot, suggestNamesForErrors)

        // We're putting the builder in the cache, so increment its count.
        let decrement = IncrementalBuilder.KeepBuilderAlive builderOpt

        match builderOpt with 
        | None -> ()
        | Some builder -> 

            // Register the behaviour that responds to CCUs being invalidated because of type
            // provider Invalidate events. This invalidates the configuration in the build.
            builder.ImportedCcusInvalidated.Add (fun _ -> 
                self.InvalidateConfiguration(options, None, userOpName))

            // Register the callback called just before a file is typechecked by the background builder (without recording
            // errors or intellisense information).
            //
            // This indicates to the UI that the file type check state is dirty. If the file is open and visible then 
            // the UI will sooner or later request a typecheck of the file, recording errors and intellisense information.
            builder.BeforeFileChecked.Add (fun file -> beforeFileChecked.Trigger(file, options.ExtraProjectInfo))
            builder.FileParsed.Add (fun file -> fileParsed.Trigger(file, options.ExtraProjectInfo))
            builder.FileChecked.Add (fun file -> fileChecked.Trigger(file, options.ExtraProjectInfo))
            builder.ProjectChecked.Add (fun () -> projectChecked.Trigger (options.ProjectFileName, options.ExtraProjectInfo))

        return (builderOpt, diagnostics, decrement)
      }

    // STATIC ROOT: FSharpLanguageServiceTestable.FSharpChecker.backgroundCompiler.incrementalBuildersCache. This root typically holds more 
    // live information than anything else in the F# Language Service, since it holds up to 3 (projectCacheStrongSize) background project builds
    // strongly.
    // 
    /// Cache of builds keyed by options.        
    let incrementalBuildersCache = 
        MruCache<CompilationThreadToken, FSharpProjectOptions, (IncrementalBuilder option * FSharpErrorInfo[] * IDisposable)>
                (keepStrongly=projectCacheSize, keepMax=projectCacheSize, 
                 areSame =  FSharpProjectOptions.AreSameForChecking, 
                 areSimilar =  FSharpProjectOptions.UseSameProject,
                 requiredToKeep=(fun (builderOpt,_,_) -> match builderOpt with None -> false | Some (b:IncrementalBuilder) -> b.IsBeingKeptAliveApartFromCacheEntry),
                 onDiscard = (fun (_, _, decrement:IDisposable) -> decrement.Dispose()))

    let getOrCreateBuilderAndKeepAlive (ctok, options, userOpName) =
      cancellable {
          RequireCompilationThread ctok
          match incrementalBuildersCache.TryGet (ctok, options) with
          | Some (builderOpt,creationErrors,_) -> 
              Logger.Log LogCompilerFunctionId.Service_IncrementalBuildersCache_BuildingNewCache
              let decrement = IncrementalBuilder.KeepBuilderAlive builderOpt
              return builderOpt,creationErrors, decrement
          | None -> 
              Logger.Log LogCompilerFunctionId.Service_IncrementalBuildersCache_GettingCache
              let! (builderOpt,creationErrors,_) as info = CreateOneIncrementalBuilder (ctok, options, userOpName)
              incrementalBuildersCache.Set (ctok, options, info)
              let decrement = IncrementalBuilder.KeepBuilderAlive builderOpt
              return builderOpt, creationErrors, decrement
      }

    let parseCacheLock = Lock<ParseCacheLockToken>()
    

    // STATIC ROOT: FSharpLanguageServiceTestable.FSharpChecker.parseFileInProjectCache. Most recently used cache for parsing files.
    let parseFileCache = MruCache<ParseCacheLockToken,_,_>(parseFileCacheSize, areSimilar = AreSimilarForParsing, areSame = AreSameForParsing)

    // STATIC ROOT: FSharpLanguageServiceTestable.FSharpChecker.checkFileInProjectCachePossiblyStale 
    // STATIC ROOT: FSharpLanguageServiceTestable.FSharpChecker.checkFileInProjectCache
    //
    /// Cache which holds recently seen type-checks.
    /// This cache may hold out-of-date entries, in two senses
    ///    - there may be a more recent antecedent state available because the background build has made it available
    ///    - the source for the file may have changed
    
    let checkFileInProjectCachePossiblyStale = 
        MruCache<ParseCacheLockToken,string * FSharpProjectOptions, FSharpParseFileResults * FSharpCheckFileResults * int>
            (keepStrongly=checkFileInProjectCacheSize,
             areSame=AreSameForChecking2,
             areSimilar=AreSubsumable2)

    // Also keyed on source. This can only be out of date if the antecedent is out of date
    let checkFileInProjectCache = 
        MruCache<ParseCacheLockToken,FileName * SourceTextHash * FSharpProjectOptions, FSharpParseFileResults * FSharpCheckFileResults * FileVersion * DateTime>
            (keepStrongly=checkFileInProjectCacheSize,
             areSame=AreSameForChecking3,
             areSimilar=AreSubsumable3)

    /// Holds keys for files being currently checked. It's used to prevent checking same file in parallel (interleaving chunck queued to Reactor).
    let beingCheckedFileTable = 
        ConcurrentDictionary<FilePath * FSharpProjectOptions * FileVersion, unit>
            (HashIdentity.FromFunctions
                hash
                (fun (f1, o1, v1) (f2, o2, v2) -> f1 = f2 && v1 = v2 && FSharpProjectOptions.AreSameForChecking(o1, o2)))

    static let mutable foregroundParseCount = 0
    static let mutable foregroundTypeCheckCount = 0

    let MakeCheckFileResultsEmpty(filename, creationErrors) = 
        FSharpCheckFileResults (filename, creationErrors, None, [| |], None, reactorOps, keepAssemblyContents)

    let MakeCheckFileResults(filename, options:FSharpProjectOptions, builder, scope, dependencyFiles, creationErrors, parseErrors, tcErrors) = 
        let errors = 
            [| yield! creationErrors 
               yield! parseErrors
               if options.IsIncompleteTypeCheckEnvironment then 
                    yield! Seq.truncate maxTypeCheckErrorsOutOfProjectContext tcErrors
               else 
                    yield! tcErrors |]
                
        FSharpCheckFileResults (filename, errors, Some scope, dependencyFiles, Some builder, reactorOps, keepAssemblyContents)

    let MakeCheckFileAnswer(filename, tcFileResult, options:FSharpProjectOptions, builder, dependencyFiles, creationErrors, parseErrors, tcErrors) = 
        match tcFileResult with 
        | Parser.TypeCheckAborted.Yes  ->  FSharpCheckFileAnswer.Aborted                
        | Parser.TypeCheckAborted.No scope -> FSharpCheckFileAnswer.Succeeded(MakeCheckFileResults(filename, options, builder, scope, dependencyFiles, creationErrors, parseErrors, tcErrors))

    member bc.RecordTypeCheckFileInProjectResults(filename,options,parsingOptions,parseResults,fileVersion,priorTimeStamp,checkAnswer,sourceText) =        
        match checkAnswer with 
        | None
        | Some FSharpCheckFileAnswer.Aborted -> ()
        | Some (FSharpCheckFileAnswer.Succeeded typedResults) -> 
            foregroundTypeCheckCount <- foregroundTypeCheckCount + 1
            parseCacheLock.AcquireLock (fun ltok -> 
                checkFileInProjectCachePossiblyStale.Set(ltok, (filename,options),(parseResults,typedResults,fileVersion))  
                checkFileInProjectCache.Set(ltok, (filename,sourceText,options),(parseResults,typedResults,fileVersion,priorTimeStamp))
                parseFileCache.Set(ltok, (filename, sourceText, parsingOptions), parseResults))

    member bc.ImplicitlyStartCheckProjectInBackground(options, userOpName) =        
        if implicitlyStartBackgroundWork then 
            bc.CheckProjectInBackground(options, userOpName + ".ImplicitlyStartCheckProjectInBackground")

    member bc.ParseFile(filename: string, sourceText: ISourceText, options: FSharpParsingOptions, userOpName: string) =
        async {
            let hash = sourceText.GetHashCode()
            match parseCacheLock.AcquireLock(fun ltok -> parseFileCache.TryGet(ltok, (filename, hash, options))) with
            | Some res -> return res
            | None ->
                foregroundParseCount <- foregroundParseCount + 1
                let parseErrors, parseTreeOpt, anyErrors = Parser.parseFile(sourceText, filename, options, userOpName, suggestNamesForErrors)
                let res = FSharpParseFileResults(parseErrors, parseTreeOpt, anyErrors, options.SourceFiles)
                parseCacheLock.AcquireLock(fun ltok -> parseFileCache.Set(ltok, (filename, hash, options), res))
                return res
        }

    /// Fetch the parse information from the background compiler (which checks w.r.t. the FileSystem API)
    member bc.GetBackgroundParseResultsForFileInProject(filename, options, userOpName) =
        reactor.EnqueueAndAwaitOpAsync(userOpName, "GetBackgroundParseResultsForFileInProject ", filename, fun ctok -> 
            cancellable {
                let! builderOpt, creationErrors, decrement = getOrCreateBuilderAndKeepAlive (ctok, options, userOpName)
                use _unwind = decrement
                match builderOpt with
                | None -> return FSharpParseFileResults(creationErrors, None, true, [| |])
                | Some builder -> 
                    let! parseTreeOpt,_,_,parseErrors = builder.GetParseResultsForFile (ctok, filename)
                    let errors = [| yield! creationErrors; yield! ErrorHelpers.CreateErrorInfos (builder.TcConfig.errorSeverityOptions, false, filename, parseErrors, suggestNamesForErrors) |]
                    return FSharpParseFileResults(errors = errors, input = parseTreeOpt, parseHadErrors = false, dependencyFiles = builder.AllDependenciesDeprecated)
            }
        )

    member bc.GetCachedCheckFileResult(builder: IncrementalBuilder,filename,sourceText: ISourceText,options) =
            // Check the cache. We can only use cached results when there is no work to do to bring the background builder up-to-date
            let cachedResults = parseCacheLock.AcquireLock (fun ltok -> checkFileInProjectCache.TryGet(ltok, (filename,sourceText.GetHashCode(),options)))

            match cachedResults with 
//            | Some (parseResults, checkResults, _, _) when builder.AreCheckResultsBeforeFileInProjectReady(filename) -> 
            | Some (parseResults, checkResults,_,priorTimeStamp) 
                 when 
                    (match builder.GetCheckResultsBeforeFileInProjectEvenIfStale filename with 
                    | None -> false
                    | Some(tcPrior) -> 
                        tcPrior.TimeStamp = priorTimeStamp &&
                        builder.AreCheckResultsBeforeFileInProjectReady(filename)) -> 
                Some (parseResults,checkResults)
            | _ -> None

    /// 1. Repeatedly try to get cached file check results or get file "lock". 
    /// 
    /// 2. If it've got cached results, returns them.
    ///
    /// 3. If it've not got the lock for 1 minute, returns `FSharpCheckFileAnswer.Aborted`.
    ///
    /// 4. Type checks the file.
    ///
    /// 5. Records results in `BackgroundCompiler` caches.
    ///
    /// 6. Starts whole project background compilation.
    ///
    /// 7. Releases the file "lock".
    member private bc.CheckOneFileImpl
        (parseResults: FSharpParseFileResults,
         sourceText: ISourceText,
         fileName: string,
         options: FSharpProjectOptions,
         textSnapshotInfo: obj option,
         fileVersion: int,
         builder: IncrementalBuilder,
         tcPrior: PartialCheckResults,
         creationErrors: FSharpErrorInfo[],
         userOpName: string) = 
    
        async {
            let beingCheckedFileKey = fileName, options, fileVersion
            let stopwatch = Stopwatch.StartNew()
            let rec loop() =
                async {
                    // results may appear while we were waiting for the lock, let's recheck if it's the case
                    let cachedResults = bc.GetCachedCheckFileResult(builder, fileName, sourceText, options) 
            
                    match cachedResults with
                    | Some (_, checkResults) -> return FSharpCheckFileAnswer.Succeeded checkResults
                    | None ->
                        if beingCheckedFileTable.TryAdd(beingCheckedFileKey, ()) then
                            try
                                // Get additional script #load closure information if applicable.
                                // For scripts, this will have been recorded by GetProjectOptionsFromScript.
                                let loadClosure = scriptClosureCacheLock.AcquireLock (fun ltok -> scriptClosureCache.TryGet (ltok, options))
                                let! tcErrors, tcFileResult = 
                                    Parser.CheckOneFile(parseResults, sourceText, fileName, options.ProjectFileName, tcPrior.TcConfig, tcPrior.TcGlobals, tcPrior.TcImports, 
                                                        tcPrior.TcState, tcPrior.ModuleNamesDict, loadClosure, tcPrior.TcErrors, reactorOps, (fun () -> builder.IsAlive), textSnapshotInfo, userOpName, suggestNamesForErrors)
                                let parsingOptions = FSharpParsingOptions.FromTcConfig(tcPrior.TcConfig, Array.ofList builder.SourceFiles, options.UseScriptResolutionRules)
                                let checkAnswer = MakeCheckFileAnswer(fileName, tcFileResult, options, builder, Array.ofList tcPrior.TcDependencyFiles, creationErrors, parseResults.Errors, tcErrors)
                                bc.RecordTypeCheckFileInProjectResults(fileName, options, parsingOptions, parseResults, fileVersion, tcPrior.TimeStamp, Some checkAnswer, sourceText.GetHashCode())
                                return checkAnswer
                            finally
                                let dummy = ref ()
                                beingCheckedFileTable.TryRemove(beingCheckedFileKey, dummy) |> ignore
                        else 
                            do! Async.Sleep 100
                            if stopwatch.Elapsed > TimeSpan.FromMinutes 1. then 
                                return FSharpCheckFileAnswer.Aborted
                            else
                                return! loop()
                }
            return! loop()
        }

    /// Type-check the result obtained by parsing, but only if the antecedent type checking context is available. 
    member bc.CheckFileInProjectAllowingStaleCachedResults(parseResults: FSharpParseFileResults, filename, fileVersion, sourceText: ISourceText, options, textSnapshotInfo: obj option, userOpName) =
        let execWithReactorAsync action = reactor.EnqueueAndAwaitOpAsync(userOpName, "CheckFileInProjectAllowingStaleCachedResults ", filename, action)
        async {
            try
                if implicitlyStartBackgroundWork then 
                    reactor.CancelBackgroundOp() // cancel the background work, since we will start new work after we're done

                let! cachedResults = 
                  execWithReactorAsync <| fun ctok ->   
                   cancellable {
                    let! _builderOpt,_creationErrors,decrement = getOrCreateBuilderAndKeepAlive (ctok, options, userOpName)
                    use _unwind = decrement

                    match incrementalBuildersCache.TryGetAny (ctok, options) with
                    | Some (Some builder, creationErrors, _) ->
                        match bc.GetCachedCheckFileResult(builder, filename, sourceText, options) with
                        | Some (_, checkResults) -> return Some (builder, creationErrors, Some (FSharpCheckFileAnswer.Succeeded checkResults))
                        | _ -> return Some (builder, creationErrors, None)
                    | _ -> return None // the builder wasn't ready
                   }
                        
                match cachedResults with
                | None -> return None
                | Some (_, _, Some x) -> return Some x
                | Some (builder, creationErrors, None) ->
                    Trace.TraceInformation("FCS: {0}.{1} ({2})", userOpName, "CheckFileInProjectAllowingStaleCachedResults.CacheMiss", filename)
                    let! tcPrior = 
                        execWithReactorAsync <| fun ctok -> 
                          cancellable {
                            DoesNotRequireCompilerThreadTokenAndCouldPossiblyBeMadeConcurrent  ctok
                            return  builder.GetCheckResultsBeforeFileInProjectEvenIfStale filename
                          }
                            
                    match tcPrior with
                    | Some tcPrior -> 
                        let! checkResults = bc.CheckOneFileImpl(parseResults, sourceText, filename, options, textSnapshotInfo, fileVersion, builder, tcPrior, creationErrors, userOpName)
                        return Some checkResults
                    | None -> return None  // the incremental builder was not up to date
            finally 
                bc.ImplicitlyStartCheckProjectInBackground(options, userOpName)
        }

    /// Type-check the result obtained by parsing. Force the evaluation of the antecedent type checking context if needed.
    member bc.CheckFileInProject(parseResults: FSharpParseFileResults, filename, fileVersion, sourceText: ISourceText, options, textSnapshotInfo, userOpName) =
        let execWithReactorAsync action = reactor.EnqueueAndAwaitOpAsync(userOpName, "CheckFileInProject", filename, action)
        async {
            try 
                if implicitlyStartBackgroundWork then 
                    reactor.CancelBackgroundOp() // cancel the background work, since we will start new work after we're done
                let! builderOpt,creationErrors, decrement = execWithReactorAsync (fun ctok -> getOrCreateBuilderAndKeepAlive (ctok, options, userOpName))
                use _unwind = decrement
                match builderOpt with
                | None -> return FSharpCheckFileAnswer.Succeeded (MakeCheckFileResultsEmpty(filename, creationErrors))
                | Some builder -> 
                    // Check the cache. We can only use cached results when there is no work to do to bring the background builder up-to-date
                    let cachedResults = bc.GetCachedCheckFileResult(builder, filename, sourceText, options)

                    match cachedResults with
                    | Some (_, checkResults) -> return FSharpCheckFileAnswer.Succeeded checkResults
                    | _ ->
                        Trace.TraceInformation("FCS: {0}.{1} ({2})", userOpName, "CheckFileInProject.CacheMiss", filename)
                        let! tcPrior = execWithReactorAsync <| fun ctok -> builder.GetCheckResultsBeforeFileInProject (ctok, filename)
                        let! checkAnswer = bc.CheckOneFileImpl(parseResults, sourceText, filename, options, textSnapshotInfo, fileVersion, builder, tcPrior, creationErrors, userOpName)
                        return checkAnswer
            finally 
                bc.ImplicitlyStartCheckProjectInBackground(options, userOpName)
        }

    /// Parses and checks the source file and returns untyped AST and check results.
    member bc.ParseAndCheckFileInProject (filename:string, fileVersion, sourceText: ISourceText, options:FSharpProjectOptions, textSnapshotInfo, userOpName) =
        let execWithReactorAsync action = reactor.EnqueueAndAwaitOpAsync(userOpName, "ParseAndCheckFileInProject", filename, action)
        async {
            try 
                let strGuid = "_ProjectId=" + (options.ProjectId |> Option.defaultValue "null")
                Logger.LogBlockMessageStart (filename + strGuid) LogCompilerFunctionId.Service_ParseAndCheckFileInProject

                if implicitlyStartBackgroundWork then 
                    Logger.LogMessage (filename + strGuid + "-Cancelling background work") LogCompilerFunctionId.Service_ParseAndCheckFileInProject
                    reactor.CancelBackgroundOp() // cancel the background work, since we will start new work after we're done

                let! builderOpt,creationErrors,decrement = execWithReactorAsync (fun ctok -> getOrCreateBuilderAndKeepAlive (ctok, options, userOpName))
                use _unwind = decrement
                match builderOpt with
                | None -> 
                    Logger.LogBlockMessageStop (filename + strGuid + "-Failed_Aborted") LogCompilerFunctionId.Service_ParseAndCheckFileInProject

                    let parseResults = FSharpParseFileResults(creationErrors, None, true, [| |])
                    return (parseResults, FSharpCheckFileAnswer.Aborted)

                | Some builder -> 
                    let cachedResults = bc.GetCachedCheckFileResult(builder, filename, sourceText, options)

                    match cachedResults with 
                    | Some (parseResults, checkResults) -> 
                        Logger.LogBlockMessageStop (filename + strGuid + "-Successful_Cached") LogCompilerFunctionId.Service_ParseAndCheckFileInProject

                        return parseResults, FSharpCheckFileAnswer.Succeeded checkResults
                    | _ ->
                        // todo this blocks the Reactor queue until all files up to the current are type checked. It's OK while editing the file,
                        // but results with non cooperative blocking when a firts file from a project opened.
                        let! tcPrior = execWithReactorAsync <| fun ctok -> builder.GetCheckResultsBeforeFileInProject (ctok, filename) 
                    
                        // Do the parsing.
                        let parsingOptions = FSharpParsingOptions.FromTcConfig(builder.TcConfig, Array.ofList (builder.SourceFiles), options.UseScriptResolutionRules)
                        let parseErrors, parseTreeOpt, anyErrors = Parser.parseFile (sourceText, filename, parsingOptions, userOpName, suggestNamesForErrors)
                        let parseResults = FSharpParseFileResults(parseErrors, parseTreeOpt, anyErrors, builder.AllDependenciesDeprecated)
                        let! checkResults = bc.CheckOneFileImpl(parseResults, sourceText, filename, options, textSnapshotInfo, fileVersion, builder, tcPrior, creationErrors, userOpName)

                        Logger.LogBlockMessageStop (filename + strGuid + "-Successful") LogCompilerFunctionId.Service_ParseAndCheckFileInProject

                        return parseResults, checkResults
            finally 
                bc.ImplicitlyStartCheckProjectInBackground(options, userOpName)
        }

    /// Fetch the check information from the background compiler (which checks w.r.t. the FileSystem API)
    member bc.GetBackgroundCheckResultsForFileInProject(filename, options, userOpName) =
        reactor.EnqueueAndAwaitOpAsync(userOpName, "GetBackgroundCheckResultsForFileInProject", filename, fun ctok -> 
          cancellable {
            let! builderOpt, creationErrors, decrement = getOrCreateBuilderAndKeepAlive (ctok, options, userOpName)
            use _unwind = decrement
            match builderOpt with
            | None -> 
                let parseResults = FSharpParseFileResults(creationErrors, None, true, [| |])
                let typedResults = MakeCheckFileResultsEmpty(filename, creationErrors)
                return (parseResults, typedResults)
            | Some builder -> 
                let! (parseTreeOpt, _, _, untypedErrors) = builder.GetParseResultsForFile (ctok, filename)
                let! tcProj = builder.GetCheckResultsAfterFileInProject (ctok, filename)
                let errorOptions = builder.TcConfig.errorSeverityOptions
                let untypedErrors = [| yield! creationErrors; yield! ErrorHelpers.CreateErrorInfos (errorOptions, false, filename, untypedErrors, suggestNamesForErrors) |]
                let tcErrors = [| yield! creationErrors; yield! ErrorHelpers.CreateErrorInfos (errorOptions, false, filename, tcProj.TcErrors, suggestNamesForErrors) |]
                let parseResults = FSharpParseFileResults(errors = untypedErrors, input = parseTreeOpt, parseHadErrors = false, dependencyFiles = builder.AllDependenciesDeprecated)
                let loadClosure = scriptClosureCacheLock.AcquireLock (fun ltok -> scriptClosureCache.TryGet (ltok, options) )
                let scope = 
                    TypeCheckInfo(tcProj.TcConfig, tcProj.TcGlobals, 
                                  Option.get tcProj.LastestCcuSigForFile, 
                                  tcProj.TcState.Ccu, tcProj.TcImports, tcProj.TcEnvAtEnd.AccessRights,
                                  options.ProjectFileName, filename, 
                                  List.head tcProj.TcResolutionsRev, 
                                  List.head tcProj.TcSymbolUsesRev,
                                  tcProj.TcEnvAtEnd.NameEnv,
                                  loadClosure, reactorOps, (fun () -> builder.IsAlive), None, 
                                  tcProj.LatestImplementationFile,
                                  List.head tcProj.TcOpenDeclarationsRev)     
                let typedResults = MakeCheckFileResults(filename, options, builder, scope, Array.ofList tcProj.TcDependencyFiles, creationErrors, parseResults.Errors, tcErrors)
                return (parseResults, typedResults)
           })


    /// Try to get recent approximate type check results for a file. 
    member bc.TryGetRecentCheckResultsForFile(filename: string, options:FSharpProjectOptions, sourceText: ISourceText option, _userOpName: string) =
        match sourceText with 
        | Some sourceText -> 
            parseCacheLock.AcquireLock (fun ltok -> 
                match checkFileInProjectCache.TryGet(ltok,(filename,sourceText.GetHashCode(),options)) with
                | Some (a,b,c,_) -> Some (a,b,c)
                | None -> parseCacheLock.AcquireLock (fun ltok -> checkFileInProjectCachePossiblyStale.TryGet(ltok,(filename,options))))
        | None -> parseCacheLock.AcquireLock (fun ltok -> checkFileInProjectCachePossiblyStale.TryGet(ltok,(filename,options)))

    /// Parse and typecheck the whole project (the implementation, called recursively as project graph is evaluated)
    member private bc.ParseAndCheckProjectImpl(options, ctok, userOpName) : Cancellable<FSharpCheckProjectResults> =
      cancellable {
        let! builderOpt,creationErrors,decrement = getOrCreateBuilderAndKeepAlive (ctok, options, userOpName)
        use _unwind = decrement
        match builderOpt with 
        | None -> 
            return FSharpCheckProjectResults (options.ProjectFileName, None, keepAssemblyContents, creationErrors, None)
        | Some builder -> 
            let! (tcProj, ilAssemRef, tcAssemblyDataOpt, tcAssemblyExprOpt)  = builder.GetCheckResultsAndImplementationsForProject(ctok)
            let errorOptions = tcProj.TcConfig.errorSeverityOptions
            let fileName = TcGlobals.DummyFileNameForRangesWithoutASpecificLocation
            let errors = [| yield! creationErrors; yield! ErrorHelpers.CreateErrorInfos (errorOptions, true, fileName, tcProj.TcErrors, suggestNamesForErrors) |]
            return FSharpCheckProjectResults (options.ProjectFileName, Some tcProj.TcConfig, keepAssemblyContents, errors, 
                                              Some(tcProj.TcGlobals, tcProj.TcImports, tcProj.TcState.Ccu, tcProj.TcState.CcuSig, 
                                                   tcProj.TcSymbolUses, tcProj.TopAttribs, tcAssemblyDataOpt, ilAssemRef, 
                                                   tcProj.TcEnvAtEnd.AccessRights, tcAssemblyExprOpt, Array.ofList tcProj.TcDependencyFiles))
      }

    /// Get the timestamp that would be on the output if fully built immediately
    member private bc.TryGetLogicalTimeStampForProject(cache, ctok, options, userOpName: string) =

        // NOTE: This creation of the background builder is currently run as uncancellable.  Creating background builders is generally
        // cheap though the timestamp computations look suspicious for transitive project references.
        let builderOpt,_creationErrors,decrement = getOrCreateBuilderAndKeepAlive (ctok, options, userOpName + ".TryGetLogicalTimeStampForProject") |> Cancellable.runWithoutCancellation
        use _unwind = decrement
        match builderOpt with 
        | None -> None
        | Some builder -> Some (builder.GetLogicalTimeStampForProject(cache, ctok))

    /// Keep the projet builder alive over a scope
    member bc.KeepProjectAlive(options, userOpName) =
        reactor.EnqueueAndAwaitOpAsync(userOpName, "KeepProjectAlive", options.ProjectFileName, fun ctok -> 
          cancellable {
            let! _builderOpt,_creationErrors,decrement = getOrCreateBuilderAndKeepAlive (ctok, options, userOpName)
            return decrement
          })

    /// Parse and typecheck the whole project.
    member bc.ParseAndCheckProject(options, userOpName) =
        reactor.EnqueueAndAwaitOpAsync(userOpName, "ParseAndCheckProject", options.ProjectFileName, fun ctok -> bc.ParseAndCheckProjectImpl(options, ctok, userOpName))

    member bc.GetProjectOptionsFromScript(filename, sourceText, loadedTimeStamp, otherFlags, useFsiAuxLib: bool option, useSdkRefs: bool option, assumeDotNetFramework: bool option, extraProjectInfo: obj option, optionsStamp: int64 option, userOpName) = 
        reactor.EnqueueAndAwaitOpAsync (userOpName, "GetProjectOptionsFromScript", filename, fun ctok -> 
          cancellable {
            use errors = new ErrorScope()

            // Do we add a reference to FSharp.Compiler.Interactive.Settings by default?
            let useFsiAuxLib = defaultArg useFsiAuxLib true
            let useSdkRefs =  defaultArg useSdkRefs true
            let reduceMemoryUsage = ReduceMemoryFlag.Yes

            // Do we assume .NET Framework references for scripts?
            let assumeDotNetFramework = defaultArg assumeDotNetFramework true
            let otherFlags = defaultArg otherFlags [| |]
            let useSimpleResolution = 
#if ENABLE_MONO_SUPPORT
                runningOnMono || otherFlags |> Array.exists (fun x -> x = "--simpleresolution")
#else
                true
#endif
            let loadedTimeStamp = defaultArg loadedTimeStamp DateTime.MaxValue // Not 'now', we don't want to force reloading
            let applyCompilerOptions tcConfigB  = 
                let fsiCompilerOptions = CompileOptions.GetCoreFsiCompilerOptions tcConfigB 
                CompileOptions.ParseCompilerOptions (ignore, fsiCompilerOptions, Array.toList otherFlags)

            let loadClosure = 
                LoadClosure.ComputeClosureOfScriptText(ctok, legacyReferenceResolver, 
                    defaultFSharpBinariesDir, filename, sourceText, 
                    CodeContext.Editing, useSimpleResolution, useFsiAuxLib, useSdkRefs, new Lexhelp.LexResourceManager(), 
                    applyCompilerOptions, assumeDotNetFramework, 
                    tryGetMetadataSnapshot=tryGetMetadataSnapshot, 
                    reduceMemoryUsage=reduceMemoryUsage)

            let otherFlags = 
                [| yield "--noframework"; yield "--warn:3"; 
                   yield! otherFlags 
                   for r in loadClosure.References do yield "-r:" + fst r
                   for (code,_) in loadClosure.NoWarns do yield "--nowarn:" + code
                |]

            let options = 
                {
                    ProjectFileName = filename + ".fsproj" // Make a name that is unique in this directory.
                    ProjectId = None
                    SourceFiles = loadClosure.SourceFiles |> List.map fst |> List.toArray
                    OtherOptions = otherFlags 
                    ReferencedProjects= [| |]  
                    IsIncompleteTypeCheckEnvironment = false
                    UseScriptResolutionRules = true 
                    LoadTime = loadedTimeStamp
                    UnresolvedReferences = Some (UnresolvedReferencesSet(loadClosure.UnresolvedReferences))
                    OriginalLoadReferences = loadClosure.OriginalLoadReferences
                    ExtraProjectInfo=extraProjectInfo
                    Stamp = optionsStamp
                }
            scriptClosureCacheLock.AcquireLock (fun ltok -> scriptClosureCache.Set(ltok, options, loadClosure)) // Save the full load closure for later correlation.
            return options, errors.Diagnostics
          })
            
    member bc.InvalidateConfiguration(options : FSharpProjectOptions, startBackgroundCompileIfAlreadySeen, userOpName) =
        let startBackgroundCompileIfAlreadySeen = defaultArg startBackgroundCompileIfAlreadySeen implicitlyStartBackgroundWork
        // This operation can't currently be cancelled nor awaited
        reactor.EnqueueOp(userOpName, "InvalidateConfiguration: Stamp(" + (options.Stamp |> Option.defaultValue 0L).ToString() + ")", options.ProjectFileName, fun ctok -> 
            // If there was a similar entry then re-establish an empty builder .  This is a somewhat arbitrary choice - it
            // will have the effect of releasing memory associated with the previous builder, but costs some time.
            if incrementalBuildersCache.ContainsSimilarKey (ctok, options) then

                // We do not need to decrement here - the onDiscard function is called each time an entry is pushed out of the build cache,
                // including by incrementalBuildersCache.Set.
                let newBuilderInfo = CreateOneIncrementalBuilder (ctok, options, userOpName) |> Cancellable.runWithoutCancellation
                incrementalBuildersCache.Set(ctok, options, newBuilderInfo)

                // Start working on the project.  Also a somewhat arbitrary choice
                if startBackgroundCompileIfAlreadySeen then 
                   bc.CheckProjectInBackground(options, userOpName + ".StartBackgroundCompile"))

    member bc.NotifyProjectCleaned (options : FSharpProjectOptions, userOpName) =
        reactor.EnqueueAndAwaitOpAsync(userOpName, "NotifyProjectCleaned", options.ProjectFileName, fun ctok -> 
         cancellable {
            // If there was a similar entry (as there normally will have been) then re-establish an empty builder .  This 
            // is a somewhat arbitrary choice - it will have the effect of releasing memory associated with the previous 
            // builder, but costs some time.
            if incrementalBuildersCache.ContainsSimilarKey (ctok, options) then
                // We do not need to decrement here - the onDiscard function is called each time an entry is pushed out of the build cache,
                // including by incrementalBuildersCache.Set.
                let! newBuilderInfo = CreateOneIncrementalBuilder (ctok, options, userOpName) 
                incrementalBuildersCache.Set(ctok, options, newBuilderInfo)
          })

    member bc.CheckProjectInBackground (options, userOpName) =
        reactor.SetBackgroundOp (Some (userOpName, "CheckProjectInBackground", options.ProjectFileName, (fun ctok ct -> 
            // The creation of the background builder can't currently be cancelled
            match getOrCreateBuilderAndKeepAlive (ctok, options, userOpName) |> Cancellable.run ct with
            | ValueOrCancelled.Cancelled _ -> false
            | ValueOrCancelled.Value (builderOpt,_,decrement) -> 
                use _unwind = decrement
                match builderOpt with 
                | None -> false
                | Some builder -> 
                    // The individual steps of the background build 
                    match builder.Step(ctok) |> Cancellable.run ct with
                    | ValueOrCancelled.Value v -> v
                    | ValueOrCancelled.Cancelled _ -> false)))

    member bc.StopBackgroundCompile   () =
        reactor.SetBackgroundOp(None)

    member bc.WaitForBackgroundCompile() =
        reactor.WaitForBackgroundOpCompletion() 

    member bc.CompleteAllQueuedOps() =
        reactor.CompleteAllQueuedOps() 

    member bc.Reactor  = reactor
    member bc.ReactorOps  = reactorOps
    member bc.BeforeBackgroundFileCheck = beforeFileChecked.Publish
    member bc.FileParsed = fileParsed.Publish
    member bc.FileChecked = fileChecked.Publish
    member bc.ProjectChecked = projectChecked.Publish

    member bc.CurrentQueueLength = reactor.CurrentQueueLength

    member bc.ClearCachesAsync (userOpName) =
        reactor.EnqueueAndAwaitOpAsync (userOpName, "ClearCachesAsync", "", fun ctok -> 
            parseCacheLock.AcquireLock (fun ltok -> 
                checkFileInProjectCachePossiblyStale.Clear ltok
                checkFileInProjectCache.Clear ltok
                parseFileCache.Clear(ltok))
            incrementalBuildersCache.Clear ctok
            frameworkTcImportsCache.Clear ctok
            scriptClosureCacheLock.AcquireLock (fun ltok -> scriptClosureCache.Clear ltok)
            cancellable.Return ())

    member bc.DownsizeCaches(userOpName) =
        reactor.EnqueueAndAwaitOpAsync (userOpName, "DownsizeCaches", "", fun ctok -> 
            parseCacheLock.AcquireLock (fun ltok -> 
                checkFileInProjectCachePossiblyStale.Resize(ltok, keepStrongly=1)
                checkFileInProjectCache.Resize(ltok, keepStrongly=1)
                parseFileCache.Resize(ltok, keepStrongly=1))
            incrementalBuildersCache.Resize(ctok, keepStrongly=1, keepMax=1)
            frameworkTcImportsCache.Downsize(ctok)
            scriptClosureCacheLock.AcquireLock (fun ltok -> scriptClosureCache.Resize(ltok,keepStrongly=1, keepMax=1))
            cancellable.Return ())
         
    member __.FrameworkImportsCache = frameworkTcImportsCache
    member __.ImplicitlyStartBackgroundWork with get() = implicitlyStartBackgroundWork and set v = implicitlyStartBackgroundWork <- v
    static member GlobalForegroundParseCountStatistic = foregroundParseCount
    static member GlobalForegroundTypeCheckCountStatistic = foregroundTypeCheckCount


//----------------------------------------------------------------------------
// FSharpChecker
//

[<Sealed>]
[<AutoSerializable(false)>]
// There is typically only one instance of this type in a Visual Studio process.
type FSharpChecker(legacyReferenceResolver, projectCacheSize, keepAssemblyContents, keepAllBackgroundResolutions, tryGetMetadataSnapshot, suggestNamesForErrors) =

    let backgroundCompiler = BackgroundCompiler(legacyReferenceResolver, projectCacheSize, keepAssemblyContents, keepAllBackgroundResolutions, tryGetMetadataSnapshot, suggestNamesForErrors)

    static let globalInstance = lazy FSharpChecker.Create()
        
    
    // STATIC ROOT: FSharpLanguageServiceTestable.FSharpChecker.braceMatchCache. Most recently used cache for brace matching. Accessed on the
    // background UI thread, not on the compiler thread.
    //
    // This cache is safe for concurrent access because there is no onDiscard action for the items in the cache.
    let braceMatchCache = MruCache<AnyCallerThreadToken,_,_>(braceMatchCacheSize, areSimilar = AreSimilarForParsing, areSame = AreSameForParsing) 

    let mutable maxMemoryReached = false
    let mutable maxMB = maxMBDefault
    let maxMemEvent = new Event<unit>()

    /// Instantiate an interactive checker.    
    static member Create(?projectCacheSize, ?keepAssemblyContents, ?keepAllBackgroundResolutions, ?legacyReferenceResolver, ?tryGetMetadataSnapshot, ?suggestNamesForErrors) = 

        let legacyReferenceResolver = 
            match legacyReferenceResolver with 
            | None -> SimulatedMSBuildReferenceResolver.GetBestAvailableResolver()
            | Some rr -> rr

        let keepAssemblyContents = defaultArg keepAssemblyContents false
        let keepAllBackgroundResolutions = defaultArg keepAllBackgroundResolutions true
        let projectCacheSizeReal = defaultArg projectCacheSize projectCacheSizeDefault
        let tryGetMetadataSnapshot = defaultArg tryGetMetadataSnapshot (fun _ -> None)
        let suggestNamesForErrors = defaultArg suggestNamesForErrors false
        new FSharpChecker(legacyReferenceResolver, projectCacheSizeReal,keepAssemblyContents, keepAllBackgroundResolutions, tryGetMetadataSnapshot, suggestNamesForErrors)

    member ic.ReferenceResolver = legacyReferenceResolver

    member ic.MatchBraces(filename, sourceText: ISourceText, options: FSharpParsingOptions, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        let hash = sourceText.GetHashCode()
        async {
            match braceMatchCache.TryGet(AssumeAnyCallerThreadWithoutEvidence(), (filename, hash, options)) with
            | Some res -> return res
            | None ->
                let res = Parser.matchBraces(sourceText, filename, options, userOpName, suggestNamesForErrors)
                braceMatchCache.Set(AssumeAnyCallerThreadWithoutEvidence(), (filename, hash, options), res)
                return res
        }

    member ic.MatchBraces(filename, source: string, options: FSharpProjectOptions, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        let parsingOptions, _ = ic.GetParsingOptionsFromProjectOptions(options)
        ic.MatchBraces(filename, SourceText.ofString source, parsingOptions, userOpName)

    member ic.GetParsingOptionsFromProjectOptions(options): FSharpParsingOptions * _ =
        let sourceFiles = List.ofArray options.SourceFiles
        let argv = List.ofArray options.OtherOptions
        ic.GetParsingOptionsFromCommandLineArgs(sourceFiles, argv, options.UseScriptResolutionRules)

    member ic.ParseFile(filename, sourceText, options, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        ic.CheckMaxMemoryReached()
        backgroundCompiler.ParseFile(filename, sourceText, options, userOpName)

    member ic.ParseFileInProject(filename, source: string, options, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        let parsingOptions, _ = ic.GetParsingOptionsFromProjectOptions(options)
        ic.ParseFile(filename, SourceText.ofString source, parsingOptions, userOpName)

    member ic.GetBackgroundParseResultsForFileInProject (filename,options, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.GetBackgroundParseResultsForFileInProject(filename, options, userOpName)
        
    member ic.GetBackgroundCheckResultsForFileInProject (filename,options, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.GetBackgroundCheckResultsForFileInProject(filename,options, userOpName)
        
    /// Try to get recent approximate type check results for a file. 
    member ic.TryGetRecentCheckResultsForFile(filename: string, options:FSharpProjectOptions, ?sourceText, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.TryGetRecentCheckResultsForFile(filename,options,sourceText, userOpName)

    member ic.Compile(argv: string[], ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.Reactor.EnqueueAndAwaitOpAsync (userOpName, "Compile", "", fun ctok -> 
            cancellable {
                return CompileHelpers.compileFromArgs (ctok, argv, legacyReferenceResolver, None, None)
            })

    member ic.Compile (ast:ParsedInput list, assemblyName:string, outFile:string, dependencies:string list, ?pdbFile:string, ?executable:bool, ?noframework:bool, ?userOpName: string) =
      let userOpName = defaultArg userOpName "Unknown"
      backgroundCompiler.Reactor.EnqueueAndAwaitOpAsync (userOpName, "Compile", assemblyName, fun ctok -> 
       cancellable {
            let noframework = defaultArg noframework false
            return CompileHelpers.compileFromAsts (ctok, legacyReferenceResolver, ast, assemblyName, outFile, dependencies, noframework, pdbFile, executable, None, None)
       }
      )

    member ic.CompileToDynamicAssembly (otherFlags: string[], execute: (TextWriter * TextWriter) option, ?userOpName: string)  = 
      let userOpName = defaultArg userOpName "Unknown"
      backgroundCompiler.Reactor.EnqueueAndAwaitOpAsync (userOpName, "CompileToDynamicAssembly", "<dynamic>", fun ctok -> 
       cancellable {
        CompileHelpers.setOutputStreams execute
        
        // References used to capture the results of compilation
        let tcImportsRef = ref (None: TcImports option)
        let assemblyBuilderRef = ref None
        let tcImportsCapture = Some (fun tcImports -> tcImportsRef := Some tcImports)

        // Function to generate and store the results of compilation 
        let debugInfo =  otherFlags |> Array.exists (fun arg -> arg = "-g" || arg = "--debug:+" || arg = "/debug:+")
        let dynamicAssemblyCreator = Some (CompileHelpers.createDynamicAssembly (ctok, debugInfo, tcImportsRef, execute.IsSome, assemblyBuilderRef))

        // Perform the compilation, given the above capturing function.
        let errorsAndWarnings, result = CompileHelpers.compileFromArgs (ctok, otherFlags, legacyReferenceResolver, tcImportsCapture, dynamicAssemblyCreator)

        // Retrieve and return the results
        let assemblyOpt = 
            match assemblyBuilderRef.Value with 
            | None -> None
            | Some a ->  Some (a :> System.Reflection.Assembly)

        return errorsAndWarnings, result, assemblyOpt
       }
      )

    member ic.CompileToDynamicAssembly (asts:ParsedInput list, assemblyName:string, dependencies:string list, execute: (TextWriter * TextWriter) option, ?debug:bool, ?noframework:bool, ?userOpName: string) =
      let userOpName = defaultArg userOpName "Unknown"
      backgroundCompiler.Reactor.EnqueueAndAwaitOpAsync (userOpName, "CompileToDynamicAssembly", assemblyName, fun ctok -> 
       cancellable {
        CompileHelpers.setOutputStreams execute

        // References used to capture the results of compilation
        let tcImportsRef = ref (None: TcImports option)
        let assemblyBuilderRef = ref None
        let tcImportsCapture = Some (fun tcImports -> tcImportsRef := Some tcImports)

        let debugInfo = defaultArg debug false
        let noframework = defaultArg noframework false
        let location = Path.Combine(Path.GetTempPath(),"test"+string(hash assemblyName))
        try Directory.CreateDirectory(location) |> ignore with _ -> ()

        let outFile = Path.Combine(location, assemblyName + ".dll")

        // Function to generate and store the results of compilation 
        let dynamicAssemblyCreator = Some (CompileHelpers.createDynamicAssembly (ctok, debugInfo, tcImportsRef, execute.IsSome, assemblyBuilderRef))

        // Perform the compilation, given the above capturing function.
        let errorsAndWarnings, result = 
            CompileHelpers.compileFromAsts (ctok, legacyReferenceResolver, asts, assemblyName, outFile, dependencies, noframework, None, Some execute.IsSome, tcImportsCapture, dynamicAssemblyCreator)

        // Retrieve and return the results
        let assemblyOpt = 
            match assemblyBuilderRef.Value with 
            | None -> None
            | Some a ->  Some (a :> System.Reflection.Assembly)

        return errorsAndWarnings, result, assemblyOpt
       }
      )

    /// This function is called when the entire environment is known to have changed for reasons not encoded in the ProjectOptions of any project/compilation.
    /// For example, the type provider approvals file may have changed.
    member ic.InvalidateAll() =
        ic.ClearCaches()
            
    member ic.ClearCachesAsync(?userOpName: string) =
        let utok = AssumeAnyCallerThreadWithoutEvidence()
        let userOpName = defaultArg userOpName "Unknown"
        braceMatchCache.Clear(utok)
        backgroundCompiler.ClearCachesAsync(userOpName) 

    member ic.ClearCaches(?userOpName) =
        ic.ClearCachesAsync(?userOpName=userOpName) |> Async.Start // this cache clearance is not synchronous, it will happen when the background op gets run

    member ic.CheckMaxMemoryReached() =
        if not maxMemoryReached && System.GC.GetTotalMemory(false) > int64 maxMB * 1024L * 1024L then 
            Trace.TraceWarning("!!!!!!!! MAX MEMORY REACHED, DOWNSIZING F# COMPILER CACHES !!!!!!!!!!!!!!!")
            // If the maxMB limit is reached, drastic action is taken
            //   - reduce strong cache sizes to a minimum
            let userOpName = "MaxMemoryReached"
            backgroundCompiler.CompleteAllQueuedOps()
            maxMemoryReached <- true
            braceMatchCache.Resize(AssumeAnyCallerThreadWithoutEvidence(), keepStrongly=10)
            backgroundCompiler.DownsizeCaches(userOpName) |> Async.RunSynchronously
            maxMemEvent.Trigger( () )

    // This is for unit testing only
    member ic.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients() =
        backgroundCompiler.CompleteAllQueuedOps() // flush AsyncOp
        ic.ClearCachesAsync() |> Async.RunSynchronously
        System.GC.Collect()
        System.GC.WaitForPendingFinalizers() 
        backgroundCompiler.CompleteAllQueuedOps() // flush AsyncOp
            
    /// This function is called when the configuration is known to have changed for reasons not encoded in the ProjectOptions.
    /// For example, dependent references may have been deleted or created.
    member ic.InvalidateConfiguration(options: FSharpProjectOptions, ?startBackgroundCompile, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.InvalidateConfiguration(options, startBackgroundCompile, userOpName)

    /// This function is called when a project has been cleaned, and thus type providers should be refreshed.
    member ic.NotifyProjectCleaned(options: FSharpProjectOptions, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.NotifyProjectCleaned (options, userOpName)
              
    /// Typecheck a source code file, returning a handle to the results of the 
    /// parse including the reconstructed types in the file.
    member ic.CheckFileInProjectAllowingStaleCachedResults(parseResults:FSharpParseFileResults, filename:string, fileVersion:int, source:string, options:FSharpProjectOptions,  ?textSnapshotInfo:obj, ?userOpName: string) =        
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.CheckFileInProjectAllowingStaleCachedResults(parseResults,filename,fileVersion,SourceText.ofString source,options,textSnapshotInfo, userOpName)

    /// Typecheck a source code file, returning a handle to the results of the 
    /// parse including the reconstructed types in the file.
    member ic.CheckFileInProject(parseResults:FSharpParseFileResults, filename:string, fileVersion:int, sourceText:ISourceText, options:FSharpProjectOptions, ?textSnapshotInfo:obj, ?userOpName: string) =        
        let userOpName = defaultArg userOpName "Unknown"
        ic.CheckMaxMemoryReached()
        backgroundCompiler.CheckFileInProject(parseResults,filename,fileVersion,sourceText,options,textSnapshotInfo, userOpName)

    /// Typecheck a source code file, returning a handle to the results of the 
    /// parse including the reconstructed types in the file.
    member ic.ParseAndCheckFileInProject(filename:string, fileVersion:int, sourceText:ISourceText, options:FSharpProjectOptions, ?textSnapshotInfo:obj, ?userOpName: string) =        
        let userOpName = defaultArg userOpName "Unknown"
        ic.CheckMaxMemoryReached()
        backgroundCompiler.ParseAndCheckFileInProject(filename, fileVersion, sourceText, options, textSnapshotInfo, userOpName)
            
    member ic.ParseAndCheckProject(options, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        ic.CheckMaxMemoryReached()
        backgroundCompiler.ParseAndCheckProject(options, userOpName)

    member ic.KeepProjectAlive(options, ?userOpName: string) =
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.KeepProjectAlive(options, userOpName)

    /// For a given script file, get the ProjectOptions implied by the #load closure
    member ic.GetProjectOptionsFromScript(filename, source, ?loadedTimeStamp, ?otherFlags, ?useFsiAuxLib, ?useSdkRefs, ?assumeDotNetFramework, ?extraProjectInfo: obj, ?optionsStamp: int64, ?userOpName: string) = 
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.GetProjectOptionsFromScript(filename, source, loadedTimeStamp, otherFlags, useFsiAuxLib, useSdkRefs, assumeDotNetFramework, extraProjectInfo, optionsStamp, userOpName)

    member ic.GetProjectOptionsFromCommandLineArgs(projectFileName, argv, ?loadedTimeStamp, ?extraProjectInfo: obj) = 
        let loadedTimeStamp = defaultArg loadedTimeStamp DateTime.MaxValue // Not 'now', we don't want to force reloading
        { ProjectFileName = projectFileName
          ProjectId = None
          SourceFiles = [| |] // the project file names will be inferred from the ProjectOptions
          OtherOptions = argv 
          ReferencedProjects= [| |]  
          IsIncompleteTypeCheckEnvironment = false
          UseScriptResolutionRules = false
          LoadTime = loadedTimeStamp
          UnresolvedReferences = None
          OriginalLoadReferences=[]
          ExtraProjectInfo=extraProjectInfo
          Stamp = None }

    member ic.GetParsingOptionsFromCommandLineArgs(initialSourceFiles, argv, ?isInteractive) =
        let isInteractive = defaultArg isInteractive false
        use errorScope = new ErrorScope()
        let tcConfigBuilder = TcConfigBuilder.Initial

        // Apply command-line arguments and collect more source files if they are in the arguments
        let sourceFilesNew = ApplyCommandLineArgs(tcConfigBuilder, initialSourceFiles, argv)
        FSharpParsingOptions.FromTcConfigBuidler(tcConfigBuilder, Array.ofList sourceFilesNew, isInteractive), errorScope.Diagnostics

    member ic.GetParsingOptionsFromCommandLineArgs(argv, ?isInteractive: bool) =
        ic.GetParsingOptionsFromCommandLineArgs([], argv, ?isInteractive=isInteractive)

    /// Begin background parsing the given project.
    member ic.StartBackgroundCompile(options, ?userOpName) = 
        let userOpName = defaultArg userOpName "Unknown"
        backgroundCompiler.CheckProjectInBackground(options, userOpName) 

    /// Begin background parsing the given project.
    member ic.CheckProjectInBackground(options, ?userOpName) = 
        ic.StartBackgroundCompile(options, ?userOpName=userOpName)

    /// Stop the background compile.
    member ic.StopBackgroundCompile() = 
        backgroundCompiler.StopBackgroundCompile()

    /// Block until the background compile finishes.
    //
    // This is for unit testing only
    member ic.WaitForBackgroundCompile() = backgroundCompiler.WaitForBackgroundCompile()

    // Publish the ReactorOps from the background compiler for internal use
    member ic.ReactorOps = backgroundCompiler.ReactorOps
    member ic.CurrentQueueLength = backgroundCompiler.CurrentQueueLength


    member ic.BeforeBackgroundFileCheck  = backgroundCompiler.BeforeBackgroundFileCheck
    member ic.FileParsed  = backgroundCompiler.FileParsed
    member ic.FileChecked  = backgroundCompiler.FileChecked
    member ic.ProjectChecked = backgroundCompiler.ProjectChecked
    member ic.ImplicitlyStartBackgroundWork with get() = backgroundCompiler.ImplicitlyStartBackgroundWork and set v = backgroundCompiler.ImplicitlyStartBackgroundWork <- v
    member ic.PauseBeforeBackgroundWork with get() = Reactor.Singleton.PauseBeforeBackgroundWork and set v = Reactor.Singleton.PauseBeforeBackgroundWork <- v

    static member GlobalForegroundParseCountStatistic = BackgroundCompiler.GlobalForegroundParseCountStatistic
    static member GlobalForegroundTypeCheckCountStatistic = BackgroundCompiler.GlobalForegroundTypeCheckCountStatistic
          
    member ic.MaxMemoryReached = maxMemEvent.Publish
    member ic.MaxMemory with get() = maxMB and set v = maxMB <- v
    
    static member Instance with get() = globalInstance.Force()
    member internal __.FrameworkImportsCache = backgroundCompiler.FrameworkImportsCache

    /// Tokenize a single line, returning token information and a tokenization state represented by an integer
    member x.TokenizeLine (line: string, state: FSharpTokenizerLexState) = 
        let tokenizer = FSharpSourceTokenizer([], None)
        let lineTokenizer = tokenizer.CreateLineTokenizer line
        let mutable state = (None, state)
        let tokens = 
            [| while (state <- lineTokenizer.ScanToken (snd state); (fst state).IsSome) do
                    yield (fst state).Value |]
        tokens, snd state 

    /// Tokenize an entire file, line by line
    member x.TokenizeFile (source: string) : FSharpTokenInfo[][] = 
        let lines = source.Split('\n')
        let tokens = 
            [| let mutable state = FSharpTokenizerLexState.Initial
               for line in lines do 
                   let tokens, n = x.TokenizeLine(line, state) 
                   state <- n 
                   yield tokens |]
        tokens


type FsiInteractiveChecker(legacyReferenceResolver, reactorOps: IReactorOperations, tcConfig: TcConfig, tcGlobals, tcImports, tcState) =
    let keepAssemblyContents = false

    member __.ParseAndCheckInteraction (ctok, sourceText: ISourceText, ?userOpName: string) =
        async {
            let userOpName = defaultArg userOpName "Unknown"
            let filename = Path.Combine(tcConfig.implicitIncludeDir, "stdin.fsx")
            let suggestNamesForErrors = true // Will always be true, this is just for readability
            // Note: projectSourceFiles is only used to compute isLastCompiland, and is ignored if Build.IsScript(mainInputFileName) is true (which it is in this case).
            let parsingOptions = FSharpParsingOptions.FromTcConfig(tcConfig, [| filename |], true)
            let parseErrors, parseTreeOpt, anyErrors = Parser.parseFile (sourceText, filename, parsingOptions, userOpName, suggestNamesForErrors)
            let dependencyFiles = [| |] // interactions have no dependencies
            let parseResults = FSharpParseFileResults(parseErrors, parseTreeOpt, parseHadErrors = anyErrors, dependencyFiles = dependencyFiles)
            
            let backgroundDiagnostics = [| |]
            let reduceMemoryUsage = ReduceMemoryFlag.Yes
            let assumeDotNetFramework = true

            let applyCompilerOptions tcConfigB  = 
                let fsiCompilerOptions = CompileOptions.GetCoreFsiCompilerOptions tcConfigB 
                CompileOptions.ParseCompilerOptions (ignore, fsiCompilerOptions, [ ])

            let loadClosure = LoadClosure.ComputeClosureOfScriptText(ctok, legacyReferenceResolver, defaultFSharpBinariesDir, filename, sourceText, CodeContext.Editing, tcConfig.useSimpleResolution, tcConfig.useFsiAuxLib, tcConfig.useSdkRefs, new Lexhelp.LexResourceManager(), applyCompilerOptions, assumeDotNetFramework, tryGetMetadataSnapshot=(fun _ -> None), reduceMemoryUsage=reduceMemoryUsage)
            let! tcErrors, tcFileResult =  Parser.CheckOneFile(parseResults, sourceText, filename, "project", tcConfig, tcGlobals, tcImports,  tcState, Map.empty, Some loadClosure, backgroundDiagnostics, reactorOps, (fun () -> true), None, userOpName, suggestNamesForErrors)

            return
                match tcFileResult with 
                | Parser.TypeCheckAborted.No tcFileInfo ->
                    let errors = [|  yield! parseErrors; yield! tcErrors |]
                    let typeCheckResults = FSharpCheckFileResults (filename, errors, Some tcFileInfo, dependencyFiles, None, reactorOps, false)   
                    let projectResults = 
                        FSharpCheckProjectResults (filename, Some tcConfig, keepAssemblyContents, errors, 
                                                   Some(tcGlobals, tcImports, tcFileInfo.ThisCcu, tcFileInfo.CcuSigForFile,
                                                        [tcFileInfo.ScopeSymbolUses], None, None, mkSimpleAssemblyRef "stdin", 
                                                        tcState.TcEnvFromImpls.AccessRights, None, dependencyFiles))
                    parseResults, typeCheckResults, projectResults
                | _ -> 
                    failwith "unexpected aborted"
        }

//----------------------------------------------------------------------------
// CompilerEnvironment, DebuggerEnvironment
//

type CompilerEnvironment =
  static member BinFolderOfDefaultFSharpCompiler(?probePoint) =
      FSharpEnvironment.BinFolderOfDefaultFSharpCompiler(probePoint)

/// Information about the compilation environment
[<CompilationRepresentation(CompilationRepresentationFlags.ModuleSuffix)>]
module CompilerEnvironment =
    /// These are the names of assemblies that should be referenced for .fs, .ml, .fsi, .mli files that
    /// are not associated with a project
    let DefaultReferencesForOrphanSources assumeDotNetFramework = DefaultReferencesForScriptsAndOutOfProjectSources assumeDotNetFramework
    
    /// Publish compiler-flags parsing logic. Must be fast because its used by the colorizer.
    let GetCompilationDefinesForEditing (parsingOptions: FSharpParsingOptions) =
        SourceFileImpl.AdditionalDefinesForUseInEditor(parsingOptions.IsInteractive) @
        parsingOptions.ConditionalCompilationDefines
            
    /// Return true if this is a subcategory of error or warning message that the language service can emit
    let IsCheckerSupportedSubcategory(subcategory:string) =
        // Beware: This code logic is duplicated in DocumentTask.cs in the language service
        PhasedDiagnostic.IsSubcategoryOfCompile(subcategory)

/// Information about the debugging environment
module DebuggerEnvironment =
    /// Return the language ID, which is the expression evaluator id that the
    /// debugger will use.
    let GetLanguageID() =
        System.Guid(0xAB4F38C9u, 0xB6E6us, 0x43baus, 0xBEuy, 0x3Buy, 0x58uy, 0x08uy, 0x0Buy, 0x2Cuy, 0xCCuy, 0xE3uy)
        
module PrettyNaming =
    let IsIdentifierPartCharacter     x = FSharp.Compiler.PrettyNaming.IsIdentifierPartCharacter x
    let IsLongIdentifierPartCharacter x = FSharp.Compiler.PrettyNaming.IsLongIdentifierPartCharacter x
    let IsOperatorName                x = FSharp.Compiler.PrettyNaming.IsOperatorName x
    let GetLongNameFromString         x = FSharp.Compiler.PrettyNaming.SplitNamesForILPath x
    let FormatAndOtherOverloadsString remainingOverloads = FSComp.SR.typeInfoOtherOverloads(remainingOverloads)
    let QuoteIdentifierIfNeeded id = Lexhelp.Keywords.QuoteIdentifierIfNeeded id
    let KeywordNames = Lexhelp.Keywords.keywordNames

