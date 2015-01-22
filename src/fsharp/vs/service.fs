// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

// Open up the compiler as an incremental service for parsing,
// type checking and intellisense-like environment-reporting.

namespace Microsoft.FSharp.Compiler.SourceCodeServices

open Internal.Utilities
open System
open System.IO
open System.Text
open System.Threading
open System.Collections.Generic
 
open Microsoft.FSharp.Core.Printf
open Microsoft.FSharp.Compiler.AbstractIL
open Microsoft.FSharp.Compiler.AbstractIL.Internal  
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library  
open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.MSBuildResolver
open Microsoft.FSharp.Compiler.AbstractIL.Diagnostics 
open Microsoft.FSharp.Compiler.PrettyNaming
open Internal.Utilities.Collections
open Internal.Utilities.Debug
open System.Security.Permissions

open Microsoft.FSharp.Compiler.Env 
open Microsoft.FSharp.Compiler.Parser
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.Lexhelp
open Microsoft.FSharp.Compiler.Build
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.Tastops
open Microsoft.FSharp.Compiler.Tastops.DebugPrint
open Microsoft.FSharp.Compiler.Lib
open Microsoft.FSharp.Compiler.AbstractIL.IL 
open Microsoft.FSharp.Compiler.Layout
open Microsoft.FSharp.Compiler.TypeChecker
open Microsoft.FSharp.Compiler.Infos
open Microsoft.FSharp.Compiler.Nameres
open Internal.Utilities.StructuredFormat
open ItemDescriptionIcons 
open ItemDescriptionsImpl 

[<AutoOpen>]
module EnvMisc =
    let GetEnvInteger e dflt = match System.Environment.GetEnvironmentVariable(e) with null -> dflt | t -> try int t with _ -> dflt
    let buildCacheSize   = GetEnvInteger "mFSharp_BuildCacheSize" 3
    let recentForgroundTypeCheckLookupSize = GetEnvInteger "mFSharp_RecentForegroundTypeCheckCacheSize" 5
    let braceMatchCacheSize = GetEnvInteger "mFSharp_BraceMatchCacheSize" 5
    let untypedCheckMruSize = GetEnvInteger "mFSharp_UntypedCheckMruCacheSize" 2
    let maxTypeCheckErrorsOutOfProjectContext = GetEnvInteger "mFSharp_MaxErrorsOutOfProjectContext" 3



//----------------------------------------------------------------------------
// Methods
//--------------------------------------------------------------------------

type Param = 
    { Name: string
      CanonicalTypeTextForSorting: string
      Display: string
      Description: string }

/// Format parameters for Intellisense completion
module internal Params = 
    let printCanonicalizedTypeName g (denv:DisplayEnv) tau =
        // get rid of F# abbreviations and such
        let strippedType = stripTyEqnsWrtErasure EraseAll g tau
        // pretend no namespaces are open
        let denv = denv.SetOpenPaths([])
        // now printing will see a .NET-like canonical representation, that is good for sorting overloads into a reasonable order (see bug 94520)
        NicePrint.stringOfTy denv strippedType

    let ParamOfRecdField g denv f =
        { Name = f.rfield_id.idText
          CanonicalTypeTextForSorting = printCanonicalizedTypeName g denv f.rfield_type
          Display = NicePrint.prettyStringOfTy denv f.rfield_type
          Description = "" }
    
    let ParamOfUnionCaseField g denv isGenerated (i : int) f = 
        let initial = ParamOfRecdField g denv f
        if isGenerated i f then initial
        else
        { initial with Display = NicePrint.stringOfParamData denv (ParamData(false, false, NotOptional, Some initial.Name, ReflectedArgInfo.None, f.rfield_type)) }

    let ParamOfParamData g denv (ParamData(_isParamArrayArg, _isOutArg, _optArgInfo, nmOpt, _reflArgInfo, pty) as paramData) =
        { Name = match nmOpt with None -> "" | Some pn -> pn
          CanonicalTypeTextForSorting = printCanonicalizedTypeName g denv pty
          Display = NicePrint.stringOfParamData denv paramData
          Description = "" }

    // TODO this code is similar to NicePrint.fs:formatParamDataToBuffer, refactor or figure out why different?
    let ParamsOfParamDatas g denv (paramDatas:ParamData list) rty = 
        let paramNames,paramPrefixes,paramTypes = 
            paramDatas 
            |> List.map (fun (ParamData(isParamArrayArg, _isOutArg, optArgInfo, nmOpt, _reflArgInfo, pty)) -> 
                let isOptArg = optArgInfo.IsOptional
                match nmOpt, isOptArg, tryDestOptionTy denv.g pty with 
                // Layout an optional argument 
                | Some(nm), true, ptyOpt -> 
                    // detect parameter type, if ptyOpt is None - this is .NET style optional argument
                    let pty = defaultArg ptyOpt pty
                    nm, (sprintf "?%s:" nm),  pty
                // Layout an unnamed argument 
                | None, _,_ -> 
                    "", "", pty
                // Layout a named argument 
                | Some nm,_,_ -> 
                    let prefix = if isParamArrayArg then sprintf "params %s: " nm else sprintf "%s: " nm
                    nm, prefix,pty)
            |> List.unzip3 
        let paramTypeAndRetLs,_ = NicePrint.layoutPrettifiedTypes denv (paramTypes@[rty])
        let paramTypeLs,_ = List.frontAndBack  paramTypeAndRetLs
        (paramNames,paramPrefixes,(paramTypes,paramTypeLs)||>List.zip) |||> List.map3 (fun nm paramPrefix (tau,tyL) -> 
            { Name = nm
              CanonicalTypeTextForSorting = printCanonicalizedTypeName g denv tau
              Display = paramPrefix+(showL tyL)
              Description = "" })

    let ParamsOfTypes g denv args rtau = 
        let ptausL, _ = NicePrint.layoutPrettifiedTypes denv (args@[rtau]) 
        let argsL,_ = List.frontAndBack ptausL 
        let mkParam (tau,tyL) =
            { Name = ""
              CanonicalTypeTextForSorting = printCanonicalizedTypeName g denv tau
              Display =  Layout.showL tyL
              Description = "" }
        (args,argsL) ||> List.zip |> List.map mkParam

#if EXTENSIONTYPING
    let (|ItemIsTypeWithStaticArguments|_|) g item =
        match item with
        | Item.Types(_name,tys) ->
            match tys with
            | [Microsoft.FSharp.Compiler.Tastops.AppTy g (tyconRef,_typeInst)] ->
                if tyconRef.IsProvidedErasedTycon || tyconRef.IsProvidedGeneratedTycon then
                    Some tyconRef
                else
                    None
            | _ -> None
        | _ -> None
#endif

    let rec ParamsOfItem (infoReader:InfoReader) m denv d = 
        let amap = infoReader.amap
        let g = infoReader.g
        match d with
        | Item.Value vref -> 
            let getParamsOfTypes() = 
                let _, tau = vref.TypeScheme
                if isFunTy denv.g tau then 
                    let arg,rtau = destFunTy denv.g tau 
                    let args = tryDestTupleTy denv.g arg 
                    ParamsOfTypes g denv args rtau
                else []
            match vref.ValReprInfo with
            | None -> 
                // ValReprInfo = None i.e. in let bindings defined in types or in local functions
                // in this case use old approach and return only information about types
                getParamsOfTypes ()
            | Some valRefInfo ->
                // ValReprInfo will exist for top-level syntactic functions
                // per spec: binding is considered to define a syntactic function if it is either a function or its immediate right-hand-side is a anonymous function
                let (_, argInfos,  returnTy, _) = GetTopValTypeInFSharpForm  g valRefInfo vref.Type m
                match argInfos with
                | [] -> 
                    // handles cases like 'let foo = List.map'
                    getParamsOfTypes() 
                | argInfo::_ ->
                    // result 'paramDatas' collection corresponds to the first argument of curried function
                    // i.e. let func (a : int) (b : int) = a + b
                    // paramDatas will contain information about a and returnTy will be: int -> int
                    // This is good enough as we don't provide ways to display info for the second curried argument
                    let paramDatas = 
                        argInfo
                        |> List.map ParamNameAndType.FromArgInfo
                        |> List.map (fun (ParamNameAndType(nmOpt, pty)) -> ParamData(false, false, NotOptional, nmOpt, ReflectedArgInfo.None, pty))
                    ParamsOfParamDatas g denv paramDatas returnTy
        | Item.UnionCase(ucr)   -> 
            match ucr.UnionCase.RecdFields with
            | [f] -> [ParamOfUnionCaseField g denv NicePrint.isGeneratedUnionCaseField -1 f]
            | fs -> fs |> List.mapi (ParamOfUnionCaseField g denv NicePrint.isGeneratedUnionCaseField)
        | Item.ActivePatternCase(apref)   -> 
            let v = apref.ActivePatternVal 
            let _,tau = v.TypeScheme
            let args, _ = stripFunTy denv.g tau 
            ParamsOfTypes g denv args tau
        | Item.ExnCase(ecref)     -> 
            ecref |> recdFieldsOfExnDefRef |> List.mapi (ParamOfUnionCaseField g denv NicePrint.isGeneratedExceptionField) 
        | Item.Property(_,pinfo :: _) -> 
            let paramDatas = pinfo.GetParamDatas(amap,m)
            let rty = pinfo.GetPropertyType(amap,m) 
            ParamsOfParamDatas g denv paramDatas rty
        | Item.CtorGroup(_,(minfo :: _)) 
        | Item.MethodGroup(_,(minfo :: _)) -> 
            let paramDatas = minfo.GetParamDatas(amap, m, minfo.FormalMethodInst) |> List.head
            let rty = minfo.GetFSharpReturnTy(amap, m, minfo.FormalMethodInst)
            ParamsOfParamDatas g denv paramDatas rty
        | Item.CustomBuilder (_,vref) -> ParamsOfItem infoReader m denv (Item.Value vref)
        | Item.TypeVar _ -> []

        | Item.CustomOperation (_,usageText, Some minfo) -> 
            match usageText() with 
            | None -> 
                let argNamesAndTys = ItemDescriptionsImpl.ParamNameAndTypesOfUnaryCustomOperation g minfo 
                let _, argTys, _ = PrettyTypes.PrettifyTypesN g (argNamesAndTys |> List.map (fun (ParamNameAndType(_,ty)) -> ty))
                let paramDatas = (argNamesAndTys, argTys) ||> List.map2 (fun (ParamNameAndType(nmOpt, _)) argTy -> ParamData(false, false, NotOptional, nmOpt, ReflectedArgInfo.None,argTy))
                let rty = minfo.GetFSharpReturnTy(amap, m, minfo.FormalMethodInst)
                ParamsOfParamDatas g denv paramDatas rty
            | Some _ -> 
                [] // no parameter data available for binary operators like 'zip', 'join' and 'groupJoin' since they use bespoke syntax 

        | Item.FakeInterfaceCtor _ -> []
        | Item.DelegateCtor delty -> 
            let (SigOfFunctionForDelegate(_, _, _, fty)) = GetSigOfFunctionForDelegate infoReader delty m AccessibleFromSomeFSharpCode
            ParamsOfParamDatas g denv [ParamData(false, false, NotOptional, None, ReflectedArgInfo.None, fty)] delty
#if EXTENSIONTYPING
        | ItemIsTypeWithStaticArguments g tyconRef ->
            // similar code to TcProvidedTypeAppToStaticConstantArgs 
            let typeBeforeArguments = 
                match tyconRef.TypeReprInfo with 
                | TProvidedTypeExtensionPoint info -> info.ProvidedType
                | _ -> failwith "unreachable"
            let staticParameters = typeBeforeArguments.PApplyWithProvider((fun (typeBeforeArguments,provider) -> typeBeforeArguments.GetStaticParameters(provider)), range=m) 
            let staticParameters = staticParameters.PApplyArray(id, "GetStaticParameters",m)
            staticParameters 
                |> Array.map (fun sp -> 
                    let typ = Import.ImportProvidedType amap m (sp.PApply((fun x -> x.ParameterType),m))
                    let spKind = NicePrint.stringOfTy denv typ
                    let spName = sp.PUntaint((fun sp -> sp.Name), m)
                    let spOpt = sp.PUntaint((fun sp -> sp.IsOptional), m)
                    { Name = spName
                      CanonicalTypeTextForSorting = spKind
                      Display = sprintf "%s%s: %s" (if spOpt then "?" else "") spName spKind
                      Description = "" })
                |> Array.toList 
#endif
        |  _ -> []


/// A single method for Intellisense completion
[<NoEquality; NoComparison>]
// Note: instances of this type do not hold any references to any compiler resources.
type internal Method = 
    { 
        Description: DataTipText
        Type: string
        Parameters: Param[]
        IsStaticArguments: bool    // is this not really a method, but actually a static arguments list, like TP<42,"foo"> ?
    }


/// A table of methods for Intellisense completion
//
// Note: this type does not hold any strong references to any compiler resources, nor does evaluating any of the properties execute any
// code on the compiler thread.  
[<Sealed>]
type MethodOverloads( name: string, unsortedMethods: Method[] ) = 
    // BUG 413009 : [ParameterInfo] takes about 3 seconds to move from one overload parameter to another
    // cache allows to avoid recomputing parameterinfo for the same item
    static let methodOverloadsCache = System.Runtime.CompilerServices.ConditionalWeakTable()

    let methods = 
        unsortedMethods 
        // Methods with zero arguments show up here as taking a single argument of type 'unit'.  Patch them now to appear as having zero arguments.
        |> Array.map (fun ({Parameters=parms} as meth) -> if parms.Length = 1 && parms.[0].CanonicalTypeTextForSorting="Microsoft.FSharp.Core.Unit" then {meth with Parameters=[||]} else meth)
        // Fix the order of methods, to be stable for unit testing.
        |> Array.sortBy (fun {Parameters=parms} -> parms.Length, (parms |> Array.map (fun p -> p.CanonicalTypeTextForSorting)))
    member x.Name = name
    member x.Methods = methods

    static member Create(infoReader:InfoReader,m,denv,items:Item list) = 
        let g = infoReader.g
        if isNil items then new MethodOverloads("", [| |]) else
        let name = items.Head.DisplayName g 
        let getOverloadsForItem item =
            match methodOverloadsCache.TryGetValue item with
            | true, overloads -> overloads
            | false, _ ->
                let items =
                    match item with 
                    | Item.MethodGroup(nm,minfos) -> List.map (fun minfo -> Item.MethodGroup(nm,[minfo])) minfos 
                    | Item.CtorGroup(nm,cinfos) -> List.map (fun minfo -> Item.CtorGroup(nm,[minfo])) cinfos 
                    | Item.FakeInterfaceCtor _
                    | Item.DelegateCtor _ -> [item]
                    | Item.NewDef _ 
                    | Item.ILField _ -> []
                    | Item.Event _ -> []
                    | Item.RecdField(rfinfo) -> 
                        if isFunction g rfinfo.FieldType then [item] else []
                    | Item.Value v -> 
                        if isFunction g v.Type then [item] else []
                    | Item.UnionCase(ucr) -> 
                        if not ucr.UnionCase.IsNullary then [item] else []
                    | Item.ExnCase(ecr) -> 
                        if recdFieldsOfExnDefRef ecr |> nonNil then [item] else []
                    | Item.Property(_,pinfos) -> 
                        let pinfo = List.head pinfos 
                        if pinfo.IsIndexer then [item] else []
#if EXTENSIONTYPING
                    | Params.ItemIsTypeWithStaticArguments g _ -> [item] // we pretend that provided-types-with-static-args are method-like in order to get ParamInfo for them
#endif
                    | Item.CustomOperation(_name, _helpText, _minfo) -> [item]
                    | Item.TypeVar _ -> []
                    | Item.CustomBuilder _ -> []
                    | _ -> []

                let methods = 
                    items |> Array.ofList |> Array.map (fun item -> 
                        { Description= DataTipText [FormatDescriptionOfItem true infoReader m denv item]
                          Type= (FormatReturnTypeOfItem infoReader m denv item)
                          Parameters = Array.ofList (Params.ParamsOfItem infoReader m denv item) 
                          IsStaticArguments = match item with | Item.Types _ -> true | _ -> false
                          })
                methodOverloadsCache.Add(item, methods)
                methods
        let methods = [| for item in items do yield! getOverloadsForItem item |]

        new MethodOverloads(name, methods)

//----------------------------------------------------------------------------
// Scopes. 
//--------------------------------------------------------------------------

[<RequireQualifiedAccess>]
type internal FindDeclFailureReason = 
    // generic reason: no particular information about error
    | Unknown
    // source code file is not available
    | NoSourceCode
    // trying to find declaration of ProvidedType without TypeProviderDefinitionLocationAttribute
    | ProvidedType of string
    // trying to find declaration of ProvidedMember without TypeProviderDefinitionLocationAttribute
    | ProvidedMember of string

[<NoEquality; NoComparison>]
type internal FindDeclResult = 
    /// declaration not found + reason
    | DeclNotFound of FindDeclFailureReason
    /// found declaration; return (position-in-file, name-of-file)
    | DeclFound of Position * string


/// This type is used to describe what was found during the name resolution.
/// (Depending on the kind of the items, we may stop processing or continue to find better items)
[<RequireQualifiedAccess>]
[<NoEquality; NoComparison>]
type internal NameResResult = 
    | Members of (Item list * DisplayEnv * range)
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
    | Some of (Item list * DisplayEnv * range)

type Names = string list 
type NamesWithResidue = Names * string 

[<System.Diagnostics.DebuggerDisplay("{DebugToString()}")>]
type CapturedNameResolution(p:pos, i:Item, io:ItemOccurence, de:DisplayEnv, nre:Nameres.NameResolutionEnv, ad:AccessorDomain, m:range) =
    member this.Pos = p
    member this.Item = i
    member this.ItemOccurence = io
    member this.DisplayEnv = de
    member this.NameResolutionEnv = nre
    member this.AccessorDomain = ad
    member this.Range = m
    member this.DebugToString() = 
        sprintf "%A: %+A" (p.Line, p.Column) i

// A scope represents everything we get back from the typecheck of a file.
// It acts like an in-memory database about the file.
// It is effectively immutable and not updated: when we re-typecheck we just drop the previous
// scope object on the floor and make a new one.
[<Sealed>]
type TypeCheckInfo
          (/// Information corresponding to miscellaneous command-line options (--define, etc).
           _sTcConfig: Build.TcConfig,
           g: Env.TcGlobals,
           /// AssemblyName -> IL-Module 
           amap: Import.ImportMap,
           /// project directory, or directory containing the file that generated this scope if no project directory given 
           sProjectDir: string ,
           sFile:string,
           /// Name resolution environments for every interesting region in the file. These regions may
           /// overlap, in which case the smallest region applicable should be used.
           sEnvs: ResizeArray<range * Nameres.NameResolutionEnv * AccessorDomain>,
           /// This is a name resolution environment to use if no better match can be found.
           sFallback:Nameres.NameResolutionEnv,
           /// Information of exact types found for expressions, that can be to the left of a dot.
           /// Also for exact name resolutions
           /// pos -- line and column
           /// typ - the inferred type for an expression
           /// Item -- named item
           /// DisplayEnv -- information about printing. For example, should redundant keywords be hidden?
           /// NameResolutionEnv -- naming environment--for example, currently open namespaces.
           /// range -- the starting and ending position      
           capturedExprTypings: ResizeArray<(pos * TType * DisplayEnv * Nameres.NameResolutionEnv * AccessorDomain * range)>,
           capturedNameResolutions: ResizeArray<(pos * Item * ItemOccurence * DisplayEnv * Nameres.NameResolutionEnv * AccessorDomain * range)>,
           capturedResolutionsWithMethodGroups: ResizeArray<(pos * Item * ItemOccurence * DisplayEnv * Nameres.NameResolutionEnv * AccessorDomain * range)>,
           loadClosure : LoadClosure option,
           syncop:(unit->unit)->unit,
           checkAlive : (unit -> bool),
           textSnapshotInfo:obj) = 

    let capturedNameResolutions = capturedNameResolutions |> ResizeArray.map (fun (a,b,c,d,e,f,g) -> new CapturedNameResolution(a,b,c,d,e,f,g))
    let capturedResolutionsWithMethodGroups = capturedResolutionsWithMethodGroups  |> ResizeArray.map (fun (a,b,c,d,e,f,g) -> new CapturedNameResolution(a,b,c,d,e,f,g))

    let (|CNR|) (cnr:CapturedNameResolution) =
        (cnr.Pos, cnr.Item, cnr.ItemOccurence, cnr.DisplayEnv, cnr.NameResolutionEnv, cnr.AccessorDomain, cnr.Range)

    // These strings are potentially large and the editor may choose to hold them for a while.
    // Use this cache to fold together data tip text results that are the same. 
    // Is not keyed on 'Names' collection because this is invariant for the current position in 
    // this unchanged file. Keyed on lineStr though to prevent a change to the currently line
    // being available against a stale scope.
    let getDataTipTextCache = AgedLookup<int*int*string,DataTipText>(recentForgroundTypeCheckLookupSize,areSame=(fun (x,y) -> x = y))
    
    let infoReader = new InfoReader(g,amap)
    let ncenv = new NameResolver(g,amap,infoReader,Nameres.FakeInstantiationGenerator)
    
    /// Find the most precise naming environment for the given line and column
    let GetBestEnvForPos cursorPos  =
        
        let bestSoFar = ref None

        // Find the most deeply nested enclosing scope that contains given position
        sEnvs |> ResizeArray.iter (fun (possm,env,ad) -> 
            Trace.PrintLine("CompilerServicesVerbose", fun () -> sprintf "Examining range %s for strict inclusion\n" (stringOfRange possm))
            if rangeContainsPos possm cursorPos then
                match !bestSoFar with 
                | Some (bestm,_,_) -> 
                    if rangeContainsRange bestm possm then 
                      bestSoFar := Some (possm,env,ad)
                | None -> 
                    bestSoFar := Some (possm,env,ad))

        let mostDeeplyNestedEnclosingScope = !bestSoFar 
        
        match mostDeeplyNestedEnclosingScope with 
        | Some (m,_,_) -> Trace.PrintLine("CompilerServicesVerbose", fun () -> sprintf "Strict Inclusion found env for range %s\n" (stringOfRange m))
        | None ->Trace.PrintLine("CompilerServicesVerbose", fun () -> sprintf "Strict Inclusion found no environment, cursorPos = %s\n" (stringOfPos cursorPos))
        
        // Look for better subtrees on the r.h.s. of the subtree to the left of where we are 
        // Should really go all the way down the r.h.s. of the subtree to the left of where we are 
        // This is all needed when the index is floating free in the area just after the environment we really want to capture 
        // We guarantee to only refine to a more nested environment.  It may not be strictly  
        // the right environment, but will alwauys be at least as rich 

        let bestAlmostIncludedSoFar = ref None 

        sEnvs |> ResizeArray.iter (fun (possm,env,ad) -> 
            // take only ranges that strictly do not include cursorPos (all ranges that touch cursorPos were processed during 'Strict Inclusion' part)
            if rangeBeforePos possm cursorPos && not (posEq possm.End cursorPos) then 
                let contained = 
                    match mostDeeplyNestedEnclosingScope with 
                    | Some (bestm,_,_) -> rangeContainsRange bestm possm 
                    | None -> true 
                
                if contained then 
                    match  !bestAlmostIncludedSoFar with 
                    | Some (rightm:range,_,_) -> 
                        if posGt possm.End rightm.End || 
                          (posEq possm.End rightm.End && posGt possm.Start rightm.Start) then
                            bestAlmostIncludedSoFar := Some (possm,env,ad)
                    | _ -> bestAlmostIncludedSoFar := Some (possm,env,ad))
        
        let resEnv = 
            match !bestAlmostIncludedSoFar with 
            | Some (m,env,ad) -> 
                Trace.PrintLine("CompilerServicesVerbose", fun () -> sprintf "Chose refined-rightmost env covering range %s\n" (stringOfRange m))
                env,ad
            | None -> 
                match mostDeeplyNestedEnclosingScope with 
                | Some (m,env,ad) -> 
                    Trace.PrintLine("CompilerServicesVerbose", fun () -> sprintf "Chose refined env covering range %s\n" (stringOfRange m))
                    env,ad
                | None -> 
                    Trace.PrintLine("CompilerServicesVerbose", fun () -> "Using fallback global env\n")
                    (sFallback,AccessibleFromSomeFSharpCode)
        let pm = mkRange sFile cursorPos cursorPos 

        resEnv,pm

    /// The items that come back from ResolveCompletionsInType are a bit
    /// noisy. Filter a few things out.
    ///
    /// e.g. prefer types to constructors for DataTipText 
    let FilterItemsForCtors filterCtors items = 
        let items = items |> List.filter (function (Item.CtorGroup _) when filterCtors = ResolveTypeNamesToTypeRefs -> false | _ -> true) 
        items
        
    /// Looks at the exact name resolutions that occurred during type checking
    /// If 'membersByResidue' is specified, we look for members of the item obtained 
    /// from the name resolution and filter them by the specified residue (?)
    let GetPreciseItemsFromNameResolution(line,colAtEndOfNames,membersByResidue,filterCtors, resolveOverloads, hasTextChangedSinceLastTypecheck) = 
        let endOfNamesPos = Pos.fromVS line colAtEndOfNames
        Trace.PrintLine("CompilerServicesVerbose", fun () -> sprintf "GetPreciseItemsFromNameResolution: line = %d, colAtEndOfNames = %d, endOfNamesPos = %s\n" line colAtEndOfNames (stringOfPos endOfNamesPos))

        let quals = 
            (match resolveOverloads with ResolveOverloads.Yes ->  capturedNameResolutions | ResolveOverloads.No -> capturedResolutionsWithMethodGroups)
            |> ResizeArray.filter (fun cnr -> 
                Trace.PrintLine("CompilerServicesVerbose", fun () -> sprintf "Checking position %s = %s\n" (stringOfPos endOfNamesPos) (stringOfPos cnr.Pos))
                posEq cnr.Pos endOfNamesPos)
        Trace.PrintLine("CompilerServicesVerbose", fun () -> sprintf "GetPreciseItemsFromNameResolution: Found %d relevant quals\n" quals.Count)
        
        let items = quals |> ResizeArray.toList |> List.rev  // Logic below expects the list to be in reverse order of resolution
        
        // Filter items to show only valid & return Some if there are any
        let returnItemsOfType items g denv (m:range) f =
            let items = items |> RemoveDuplicateItems g
            let items = items |> RemoveExplicitlySuppressed g
            let items = items |> FilterItemsForCtors filterCtors
            if nonNil items then
                Trace.PrintLine("CompilerServicesVerbose", fun () -> sprintf "GetPreciseItemsFromNameResolution: Results in %d items!\n" items.Length)
                if hasTextChangedSinceLastTypecheck(textSnapshotInfo, Range.toVS m) then
                    NameResResult.TypecheckStaleAndTextChanged // typecheck is stale, wait for second-chance IntelliSense to bring up right result
                else
                    f(items, denv, m) 
            else NameResResult.Empty        

        match items, membersByResidue with 
        
        // If we're looking for members using a residue, we'd expect only
        // a single item (pick the first one) and we need the residue (which may be "")
        | CNR(_,Item.Types(_,(typ::_)),_,denv,nenv,ad,m)::_, Some _ -> 
            let items = ResolveCompletionsInType ncenv nenv (ConstraintSolver.IsApplicableMethApprox g amap m) m ad true typ 
            returnItemsOfType items g denv m NameResResult.Members
        
        // Value reference from the name resolution. Primarilly to disallow "let x.$ = 1"
        // In most of the cases, value references can be obtained from expression typings or from environment,
        // so we wouldn't have to handle values here. However, if we have something like:
        //   let varA = "string"
        //   let varA = if b then 0 else varA.
        // then the expression typings get confused (thinking 'varA:int'), so we use name resolution even for usual values.
        
        | CNR(_, Item.Value(vref), occurence, denv, nenv, ad, m)::_, Some _ ->
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
                    if Microsoft.FSharp.Compiler.Typrelns.TypeFeasiblySubsumesType 0 g amap m tcref Microsoft.FSharp.Compiler.Typrelns.CanCoerce ty then
                        ad
                    else
                        AccessibleFrom(paths, None)
                | _ -> ad

              let items = ResolveCompletionsInType ncenv nenv (ConstraintSolver.IsApplicableMethApprox g amap m) m ad false ty
              returnItemsOfType items g denv m NameResResult.Members
        
        // No residue, so the items are the full resolution of the name
        | CNR(_,_,_,denv,_,_,m) :: _, None -> 
            Trace.PrintLine("CompilerServicesVerbose", fun () -> sprintf "GetPreciseItemsFromNameResolution: No residue, precise results found\n")
            let items = items |> List.map (fun (CNR(_,item,_,_,_,_,_)) -> item) 
                              // "into" is special magic syntax, not an identifier or a library call.  It is part of capturedNameResolutions as an 
                              // implementation detail of syntax coloring, but we should not report name resolution results for it, to prevent spurious QuickInfo.
                              |> List.filter (function Item.CustomOperation(CustomOperations.Into,_,_) -> false | _ -> true) 
            returnItemsOfType items g denv m NameResResult.Members
        | _ , _ -> NameResResult.Empty

    /// finds captured typing for the given position
    let GetExprTypingForPosition(endOfExprPos) = 
        let quals = 
            capturedExprTypings 
            |> Seq.filter (fun (pos,typ,denv,_,_,_) -> 
                    // We only want expression types that end at the particular position in the file we are looking at.
                    let isLocationWeCareAbout = posEq pos endOfExprPos
                    // Get rid of function types.  True, given a 2-arg curried function "f x y", it is legal to do "(f x).GetType()",
                    // but you almost never want to do this in practice, and we choose not to offer up any intellisense for 
                    // F# function types.
                    let isFunction = isFunTy denv.g typ
                    Trace.PrintLine("CompilerServicesVerbose", fun () -> sprintf "GetPreciseCompletionListFromExprTypings: isFunction=%A, isLocationWeCareAbout=%A\n" isFunction isLocationWeCareAbout)
                    isLocationWeCareAbout && not isFunction)
            |> Seq.toArray

        let thereWereSomeQuals = not (Array.isEmpty quals)
        // filter out errors

        let quals = quals 
                    |> Array.filter (fun (_,typ,denv,_,_,_) -> not (isTyparTy denv.g typ && (destTyparTy denv.g typ).IsFromError))
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
        | Some (_,typ,denv,_nenv,ad,m) when isRecdTy denv.g typ ->
            let items = Nameres.ResolveRecordOrClassFieldsOfType ncenv m ad typ false
            Some (items, denv, m)
        | _ -> None


    /// Looks at the exact expression types at the position to the left of the 
    /// residue then the source when it was typechecked.
    let GetPreciseCompletionListFromExprTypings(untypedParseInfo:UntypedParseInfo, line, colAtEndOfNames, filterCtors, hasTextChangedSinceLastTypecheck: (obj * Range -> bool)) = 
        let endOfExprPos = Pos.fromVS line colAtEndOfNames
        Trace.PrintLine("CompilerServicesVerbose", fun () -> sprintf "GetPreciseCompletionListFromExprTypings: line = %d, colAtEndOfNames = %d, endOfExprPos = %s\n" line colAtEndOfNames (stringOfPos endOfExprPos))
        
        let thereWereSomeQuals, quals = GetExprTypingForPosition(endOfExprPos)

        match quals with
        | [| |] -> 
            if thereWereSomeQuals then
                GetPreciseCompletionListFromExprTypingsResult.NoneBecauseThereWereTypeErrors 
            else
                GetPreciseCompletionListFromExprTypingsResult.None
        | _ ->
            let bestQual, textChanged = 
                match UntypedParseInfoImpl.GetUntypedParseResults untypedParseInfo with
                | { Input=Some(input) } -> 
                    match UntypedParseInfoImpl.GetRangeOfExprLeftOfDot(line,colAtEndOfNames,Some(input)) with   // TODO we say "colAtEndOfNames" everywhere, but that's not really a good name ("foo  .  $" hit Ctrl-Space at $)
                    | Some( ((sl,sc),_) as exprRange) ->
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
                                                            posEq (Pos.fromVS sl sc) r.Start)
                            qual, false
                    | None -> 
                        // TODO In theory I think we should never get to this code path; it would be nice to add an assert.
                        // In practice, we do get here in some weird cases like "2.0 .. 3.0" and hitting Ctrl-Space in between the two dots of the range operator.
                        // I wasn't able to track down what was happening in those werid cases, not worth worrying about, it doesn't manifest as a product bug or anything.
                        None, false
                | _ -> None, false

            match bestQual with
            | Some bestQual ->
                let (_,typ,denv,nenv,ad,m) = bestQual 
                let items = ResolveCompletionsInType ncenv nenv (ConstraintSolver.IsApplicableMethApprox g amap m) m ad false typ 
                Trace.PrintLine("CompilerServicesVerbose", fun () -> sprintf "GetPreciseCompletionListFromExprTypings: Results in %d items!\n" items.Length)
                let items = items |> RemoveDuplicateItems g
                let items = items |> RemoveExplicitlySuppressed g
                let items = items |> FilterItemsForCtors filterCtors 
                GetPreciseCompletionListFromExprTypingsResult.Some(items,denv,m)
            | None -> 
                if textChanged then GetPreciseCompletionListFromExprTypingsResult.NoneBecauseTypecheckIsStaleAndTextChanged
                else GetPreciseCompletionListFromExprTypingsResult.None

    /// Find items in the best naming environment.
    let GetEnvironmentLookupResolutions(line,colAtEndOfNamesAndResidue,plid,filterCtors,showObsolete) = 
        Trace.PrintLine("CompilerServicesVerbose", fun () -> sprintf "GetEnvironmentLookupResolutions: line = %d, colAtEndOfNamesAndResidue = %d, plid = %+A, showObsolete = %b\n" line colAtEndOfNamesAndResidue plid showObsolete)
        let cursorPos = Pos.fromVS line colAtEndOfNamesAndResidue
        let (nenv,ad),m = GetBestEnvForPos cursorPos
        let items = Nameres.ResolvePartialLongIdent ncenv nenv (ConstraintSolver.IsApplicableMethApprox g amap m) m ad plid showObsolete
        let items = items |> RemoveDuplicateItems g 
        let items = items |> RemoveExplicitlySuppressed g
        let items = items |> FilterItemsForCtors filterCtors 
         
        Trace.PrintLine("CompilerServicesVerbose", fun () -> sprintf "GetEnvironmentLookupResolutions: found %d item groups by looking up long identifier chain in environment\n" (List.length items))
        items, nenv.DisplayEnv, m 

    /// Find record fields in the best naming environment.
    let GetClassOrRecordFieldsEnvironmentLookupResolutions(line,colAtEndOfNamesAndResidue, plid, (_residue : string option)) = 
        let cursorPos = Pos.fromVS line colAtEndOfNamesAndResidue
        let (nenv, ad),m = GetBestEnvForPos cursorPos
        let items = Nameres.ResolvePartialLongIdentToClassOrRecdFields ncenv nenv m ad plid false
        let items = items |> RemoveDuplicateItems g 
        let items = items |> RemoveExplicitlySuppressed g
        items, nenv.DisplayEnv,m 

    /// Resolve a location and/or text to items.
    //   Three techniques are used
    //        - look for an exact known name resolution from type checking
    //        - use the known type of an expression, e.g. (expr).Name, to generate an item list  
    //        - lookup an entire name in the name resolution environment, e.g. A.B.Name, to generate an item list
    //
    // The overall aim is to resolve as accurately as possible based on what we know from type inference
    
    let GetDeclItemsForNamesAtPosition(untypedParseInfoOpt : UntypedParseInfo option,
                                       origLongIdentOpt: string list option, residueOpt, line, lineStr:string, colAtEndOfNamesAndResidue, filterCtors, resolveOverloads, hasTextChangedSinceLastTypecheck: (obj * Range -> bool)) = 
        use t = Trace.Call("CompilerServices","GetDeclItemsForNamesAtPosition", fun _->sprintf " plid=%+A residueOpt=%+A line=%d colAtEndOfNames=%d" origLongIdentOpt (residueOpt:option<string>) line colAtEndOfNamesAndResidue)

        let GetBaseClassCandidates (denv : DisplayEnv) = function
            | Item.ModuleOrNamespaces _ -> true
            | Item.Types(_, ty::_) when (isClassTy denv.g ty) && not (isSealedTy denv.g ty) -> true
            | _ -> false   

        let GetInterfaceCandidates (denv : DisplayEnv) = function
            | Item.ModuleOrNamespaces _ -> true
            | Item.Types(_, ty::_) when (isInterfaceTy denv.g ty) -> true
            | _ -> false   
        /// Post-filter items to make sure they have precisely the right name
        /// This also checks that there are some remaining results 
        /// exactMatchResidue = Some _ -- means that we are looking for exact matches
        let FilterRelevantItemsBy (exactMatchResidue : _ option) f (items, denv, m) =
            
            // can throw if type is in located in non-resolved CCU: i.e. bigint if reference to System.Numerics is absent
            let f denv item = try f denv item with _ -> false
                                                
            // Return only items with the specified name
            let filterDeclItemsByResidue residue (items: Item list) = 
                items |> List.filter (fun item -> 
                    let n1 =  item.DisplayName g 
                    Trace.PrintLine("CompilerServicesVerbose", fun () -> sprintf "\nn1 = <<<%s>>>\nn2 = <<<%s>>>\n" n1 residue)
                    if not (f denv item) then false
                    else
                        match item with
                        | Item.Types _ | Item.CtorGroup _ -> residue + "Attribute" = n1 || residue = n1
                        | _ -> residue = n1 )
            
            // Are we looking for items with precisely the given name?
            if nonNil items && exactMatchResidue.IsSome then
                Trace.PrintLine("CompilerServices", fun _ -> sprintf "looking through %d items before filtering by residue\n" (List.length items))       
                let items = items |> filterDeclItemsByResidue exactMatchResidue.Value
                if nonNil items then Some(items,denv,m) else None        
            else 
                // When (items = []) we must returns Some([],..) and not None
                // because this value is used if we want to stop further processing (e.g. let x.$ = ...)
                let items = List.filter (f denv) items
                Some(items, denv, m) 

        let loc = 
            match colAtEndOfNamesAndResidue with
            | pastEndOfLine when pastEndOfLine >= lineStr.Length -> lineStr.Length
            | atDot when lineStr.[atDot] = '.' -> atDot + 1
            | atStart when atStart = 0 -> 0
            | otherwise -> otherwise - 1

        let FindInEnv(plid, showObsolete) = GetEnvironmentLookupResolutions(line,loc,plid,filterCtors, showObsolete) 

        let FindRecordFieldsInEnv(plid, residue) = GetClassOrRecordFieldsEnvironmentLookupResolutions(line, loc, plid, residue)

        match UntypedParseInfoImpl.TryGetCompletionContext(line, colAtEndOfNamesAndResidue, untypedParseInfoOpt) with
        | Some Invalid -> None
        | Some (CompletionContext.Inherit(InheritanceContext.Class, (plid, _))) ->
            FindInEnv(plid, false) 
            |> FilterRelevantItemsBy None GetBaseClassCandidates
        | Some (CompletionContext.Inherit(InheritanceContext.Interface, (plid, _))) ->
            FindInEnv(plid, false) 
            |> FilterRelevantItemsBy None GetInterfaceCandidates
        | Some (CompletionContext.Inherit(InheritanceContext.Unknown, (plid, _))) ->
            FindInEnv(plid, false) 
            |> FilterRelevantItemsBy None (fun denv t -> (GetBaseClassCandidates denv t) || (GetInterfaceCandidates denv t))
        | Some(CompletionContext.RecordField(RecordContext.New(plid, residue))) ->
            FindRecordFieldsInEnv(plid, residue)
            |> Some            
        | Some(CompletionContext.RecordField(RecordContext.CopyOnUpdate(r, (plid, residue)))) -> 
            match GetRecdFieldsForExpr(r) with
            | None -> 
                FindRecordFieldsInEnv(plid, residue)
                |> Some
            | x -> x
        | Some(CompletionContext.RecordField(RecordContext.Constructor(typeName))) ->
            FindRecordFieldsInEnv([typeName], None)
            |> Some
        | cc ->

        let findFirstNonWsPos i = 
            if i >= lineStr.Length then None
            else
            let mutable p = i
            while p >= 0 && System.Char.IsWhiteSpace(lineStr.[p]) do
                p <- p - 1
            if p >= 0 then Some p else None
        
        // are last two chars (except whitespaces) = ".."
        let isLikeRangeOp = 
            match findFirstNonWsPos (colAtEndOfNamesAndResidue - 1) with
            | Some x when x >= 1 && lineStr.[x] = '.' && lineStr.[x - 1] = '.' -> true
            | _ -> false

        // if last two chars are .. and we are not in range operator context - no completion
        if isLikeRangeOp && not (cc = Some (CompletionContext.RangeOperator)) then None
        else
                                    
        // Try to use the exact results of name resolution during type checking to generate the results
        // This is based on position (i.e. colAtEndOfNamesAndResidue). This is not used if a residueOpt is given.
        let nameResItems = 
            match residueOpt with 
            | None -> GetPreciseItemsFromNameResolution(line, colAtEndOfNamesAndResidue, None, filterCtors,resolveOverloads, hasTextChangedSinceLastTypecheck)
            | Some residue ->
                // deals with cases when we have spaces between dot and\or identifier, like A  . $
                // if this is our case - then wen need to locate end position of the name skipping whitespaces
                // this allows us to handle cases like: let x . $ = 1 

                // colAtEndOfNamesAndResidue is 1-based so at first we need to convert it to 0-based 
                match findFirstNonWsPos (colAtEndOfNamesAndResidue - 1) with
                | Some p when lineStr.[p] = '.' ->
                    match findFirstNonWsPos (p - 1) with
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
                assert (nonNil origLongIdent)
                // note: as above, this happens when we are called for "precise" resolution - (F1 keyword, data tip etc..)
                let plid, residue = List.frontAndBack origLongIdent
                plid, Some residue
                
        /// Post-filter items to make sure they have precisely the right name
        /// This also checks that there are some remaining results 
        let (|FilterRelevantItems|_|) orig =
            FilterRelevantItemsBy exactMatchResidueOpt (fun _ _ -> true) orig

        match nameResItems with            
        | NameResResult.TypecheckStaleAndTextChanged -> None // second-chance intellisense will try again
        | NameResResult.Cancel(denv,m) -> Some([], denv, m)
        | NameResResult.Members(FilterRelevantItems(items)) -> 
            Trace.PrintLine("CompilerServices", fun _ -> sprintf "GetDeclItemsForNamesAtPosition: lookup based on name resolution results successful, #items = %d, exists ctor = %b\n" (p13 items).Length (items |> p13 |> List.exists (function Item.CtorGroup _ -> true | _ -> false)))       
            Some items
        | _ ->
        
        match origLongIdentOpt with
        | None -> None
        | Some _ -> 
            Trace.PrintLine("CompilerServices", fun _ -> sprintf "GetDeclItemsForNamesAtPosition: plid = %+A, residue = %+A, colAtEndOfNamesAndResidue = %+A\n" plid exactMatchResidueOpt colAtEndOfNamesAndResidue)       

            // Try to use the type of the expression on the left to help generate a completion list
            let mutable thereIsADotInvolved = false
            let qualItems = 
                match untypedParseInfoOpt with
                | None -> 
                    // Note, you will get here if the 'reason' is not CompleteWord/MemberSelect/DisplayMemberList, as those are currently the 
                    // only reasons we do a sync parse to have the most precise and likely-to-be-correct-and-up-to-date info.  So for example,
                    // if you do QuickInfo hovering over A in "f(x).A()", you will only get a tip if typechecking has a name-resolution recorded
                    // for A, not if merely we know the capturedExpressionTyping of f(x) and you very recently typed ".A()" - in that case, 
                    // you won't won't get a tip until the typechecking catches back up.
                    GetPreciseCompletionListFromExprTypingsResult.None
                | Some(upi) -> 

// See ServiceUntypedParse - GetRangeOfExprLeftOfDot and TryFindExpressionASTLeftOfDotLeftOfCursor are similar, but different, can we refactor commonality?
//                match UntypedParseInfoImpl.GetRangeOfExprLeftOfDot(line,colAtEndOfNamesAndResidue,upi.ParseTree) with
//                | Some((_,_),(el,ec)) ->
//                    thereIsADotInvolved <- true
//                    GetPreciseCompletionListFromExprTypings(upi, el-1, ec, filterCtors)
                match UntypedParseInfoImpl.TryFindExpressionASTLeftOfDotLeftOfCursor(line,colAtEndOfNamesAndResidue,upi.ParseTree) with
                | Some(pos,_) ->
                    thereIsADotInvolved <- true
                    GetPreciseCompletionListFromExprTypings(upi, pos.Line-1, pos.Column, filterCtors, hasTextChangedSinceLastTypecheck)
                | None -> 
                    // Can get here in a case like: if "f xxx yyy" is legal, and we do "f xxx y"
                    // We have no interest in expression typings, those are only useful for dot-completion.  We want to fallback
                    // to "Use an environment lookup as the last resort" below
                    GetPreciseCompletionListFromExprTypingsResult.None

            match qualItems,thereIsADotInvolved with            
            | GetPreciseCompletionListFromExprTypingsResult.Some(FilterRelevantItems(items)), _
                    // Initially we only use the expression typings when looking up, e.g. (expr).Nam or (expr).Name1.Nam
                    // These come through as an empty plid and residue "". Otherwise we try an environment lookup
                    // and then return to the qualItems. This is because the expression typings are a little inaccurate, primarily because
                    // it appears we're getting some typings recorded for non-atomic expressions like "f x"
                    when (match plid with [] -> true | _ -> false)  -> 
                Trace.PrintLine("CompilerServices", fun _ -> sprintf "GetDeclItemsForNamesAtPosition: lookup based on expression typings successful\n")       
                Some items
            | GetPreciseCompletionListFromExprTypingsResult.NoneBecauseThereWereTypeErrors, _ ->
                // There was an error, e.g. we have "<expr>." and there is an error determining the type of <expr>  
                // In this case, we don't want any of the fallback logic, rather, we want to produce zero results.
                None
            | GetPreciseCompletionListFromExprTypingsResult.NoneBecauseTypecheckIsStaleAndTextChanged, _ ->         
                // we want to report no result and let second-chance intellisense kick in
                None
            | _, true when (match plid with [] -> true | _ -> false)  -> 
                // If the user just pressed '.' after an _expression_ (not a plid), it is never right to show environment-lookup top-level completions.
                // The user might by typing quickly, and the LS didn't have an expression type right before the dot yet.
                // Second-chance intellisense will bring up the correct list in a moment.
                None
            | _ ->         
            // Use an environment lookup as the last resort

            let envItems =  FindInEnv(plid, residueOpt.IsSome)
            match nameResItems, envItems, qualItems with            
            
            // First, use unfiltered name resolution items, if they're not empty
            | NameResResult.Members(items, denv, m), _, _ when nonNil items -> 
                Trace.PrintLine("CompilerServices", fun _ -> sprintf "GetDeclItemsForNamesAtPosition: lookup based on name resolution results successful, #items = %d, exists ctor = %b\n" (items).Length (items |> List.exists (function Item.CtorGroup _ -> true | _ -> false)))       
                Some(items, denv, m)                
            
            // If we have nonempty items from environment that were resolved from a type, then use them... 
            // (that's better than the next case - here we'd return 'int' as a type)
            | _, FilterRelevantItems(items, denv, m), _ when nonNil items ->
                Trace.PrintLine("CompilerServices", fun _ -> sprintf "GetDeclItemsForNamesAtPosition: lookup based on name and environment successful\n")       
                Some(items, denv, m)

            // Try again with the qualItems
            | _, _, GetPreciseCompletionListFromExprTypingsResult.Some(FilterRelevantItems(items)) ->
                Some(items)
                
            | _ -> None


    // Return 'false' if this is not a completion item valid in an interface file.
    let IsValidSignatureFileItem item =
        match item with
        | Item.Types _ | Item.ModuleOrNamespaces _ -> true
        | _ -> false

    let filterIntellisenseCompletionsBasedOnParseContext untypedParseInfoOpt line col items = 
        match untypedParseInfoOpt with
        | None -> items
        | Some t ->
            // visitor to see if we are in an "open" declaration in the parse tree
            let visitor = { new AstTraversal.AstVisitorBase<int>() with
                                override this.VisitExpr(_path, _traverseSynExpr, defaultTraverse, expr) = None  // don't need to keep going, 'open' declarations never appear inside Exprs
                                override this.VisitModuleDecl(defaultTraverse, decl) =
                                    match decl with
                                    | SynModuleDecl.Open(_longIdent, m) -> 
                                        // in theory, this means we're "in an open"
                                        // in practice, because the parse tree/walkers do not handle attributes well yet, need extra check below to ensure not e.g. $here$
                                        //     open System
                                        //     [<Attr$
                                        //     let f() = ()
                                        // inside an attribute on the next item
                                        let pos = Pos.fromVS line (col-1) // -1 because for e.g. "open System." the dot does not show up in the parse tree
                                        if rangeContainsPos m pos then  
                                            Some 0
                                        else
                                            None
                                    | _ -> defaultTraverse decl }
            match AstTraversal.Traverse(line, col, t, visitor) with
            | None -> items
            | Some _ ->
                items |> List.filter (function | Item.ModuleOrNamespaces _ -> true | _ -> false)

    member x.GetDeclarations (untypedParseInfoOpt:UntypedParseInfo option, line, lineStr, colAtEndOfNames, (names,residue):NamesWithResidue, hasTextChangedSinceLastTypecheck: (obj * Range -> bool)) : DeclarationSet =
        use t = Trace.Call("CompilerServices","GetDeclarations", fun _->sprintf " line=%+A,colAtEndOfNames=%+A,names=%+A" line colAtEndOfNames names)
        let isInterfaceFile = SourceFileImpl.IsInterfaceFile sFile
        ErrorScope.Protect 
            Range.range0 
            (fun () -> 
                match GetDeclItemsForNamesAtPosition(untypedParseInfoOpt, Some names, Some residue, line, lineStr, colAtEndOfNames, ResolveTypeNamesToCtors, ResolveOverloads.Yes, hasTextChangedSinceLastTypecheck) with
                | None -> DeclarationSet.Empty  
                | Some(items,denv,m) -> 
                    let items = items |> filterIntellisenseCompletionsBasedOnParseContext (untypedParseInfoOpt |> Option.bind (fun x -> x.ParseTree)) line colAtEndOfNames 
                    let items = if isInterfaceFile then items |> List.filter IsValidSignatureFileItem else items
                    DeclarationSet.Create(infoReader,m,denv,items,syncop,checkAlive))
            (fun msg -> DeclarationSet.Error msg)
            
    member scope.GetReferenceResolutionDataTipText(line,col) : DataTipText = 
        let pos = Pos.fromVS line col
        let lineIfExists(append) =
            if not(String.IsNullOrEmpty(append)) then append.Trim([|' '|])+"\n"
            else ""     
        let isPosMatch(pos, ar:AssemblyReference) : bool = 
            let isRangeMatch = (Range.rangeContainsPos ar.Range pos) 
            let isNotSpecialRange = (ar.Range <> rangeStartup) && (ar.Range <> range0) && (ar.Range <> rangeCmdArgs)
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
                let originalReferenceName = match resolved.originalReference with AssemblyReference (_,nm) -> nm
                let resolvedPath = // Don't show the resolved path if it is identical to what was referenced.
                    if originalReferenceName = resolved.resolvedPath then String.Empty
                    else resolved.resolvedPath
                let tip =                 
                    match resolved.resolvedFrom with 
                    | AssemblyFolders ->
                        lineIfExists(resolvedPath)
                        + lineIfExists(resolved.fusionName)
                        + (FSComp.SR.assemblyResolutionFoundByAssemblyFoldersKey())
                    | AssemblyFoldersEx -> 
                        lineIfExists(resolvedPath)
                        + lineIfExists(resolved.fusionName)
                        + (FSComp.SR.assemblyResolutionFoundByAssemblyFoldersExKey())
                    | TargetFrameworkDirectory -> 
                        lineIfExists(resolvedPath)
                        + lineIfExists(resolved.fusionName)
                        + (FSComp.SR.assemblyResolutionNetFramework())
                    | Unknown ->
                        // Unknown when resolved by plain directory search without help from MSBuild resolver.
                        lineIfExists(resolvedPath)
                        + lineIfExists(resolved.fusionName)
                    | RawFileName -> 
                        lineIfExists(resolved.fusionName)
                    | GlobalAssemblyCache -> 
                        lineIfExists(resolved.fusionName)
                        + (FSComp.SR.assemblyResolutionGAC())+ "\n"
                        + lineIfExists(resolved.redist)
                    | Path _ ->
                        lineIfExists(resolvedPath)
                        + lineIfExists(resolved.fusionName)  
                                                  
                DataTipText [DataTipElement(tip.TrimEnd([|'\n'|]) ,XmlCommentNone)]

            | [] -> DataTipText []
                                    
        ErrorScope.Protect 
            Range.range0 
            dataTipOfReferences
            (fun err -> DataTipText [DataTipElementCompositionError err])

    // GetDataTipText: return the "pop up" (or "Quick Info") text given a certain context.
    member x.GetDataTipText line lineStr colAtEndOfNames names : DataTipText  = 
        use t = Trace.Call("CompilerServices","GetDataTipText", fun _->sprintf " line=%+A,idx=%+A,names=%+A" line colAtEndOfNames names)
        
        let Compute() = 
            ErrorScope.Protect 
                Range.range0 
                (fun () -> 
                    match GetDeclItemsForNamesAtPosition(None,Some(names),None,line,lineStr,colAtEndOfNames,ResolveTypeNamesToCtors,ResolveOverloads.Yes,fun _ -> false) with
                    | None -> DataTipText []
                    | Some(items,denv,m) ->
                         DataTipText(items |> List.map (FormatDescriptionOfItem false infoReader m denv )))
                (fun err -> DataTipText [DataTipElementCompositionError err])
               
        // See devdiv bug 646520 for rationale behind truncating and caching these quick infos (they can be big!)
        let key = line,colAtEndOfNames,lineStr
        match getDataTipTextCache.TryGet key with 
        | Some res -> res
        | None ->
             let res = Compute()
             getDataTipTextCache.Put(key,res)
             res

    member x.GetF1Keyword (line, lineStr, colAtEndOfNames, names) : string option =
       use t = Trace.Call("CompilerServices","GetF1Keyword", fun _->sprintf " line=%+A,idx=%+A,names=%+A" line colAtEndOfNames names) 
       ErrorScope.Protect
            Range.range0
            (fun () ->
                match GetDeclItemsForNamesAtPosition(None, Some names, None, line, lineStr, colAtEndOfNames, ResolveTypeNamesToCtors, ResolveOverloads.No, fun _ -> false) with // F1 Keywords do not distiguish between overloads
                | None -> None
                | Some(items,_,_) ->
                    match items with
                    | [] -> None
                    | [item] ->
                        GetF1Keyword item                        
                    | _ ->
                        // handle new Type()
                        let allTypes, constr, typ =
                            List.fold 
                                (fun (allTypes,constr,typ) item ->
                                    match item, constr, typ with
                                    |   (Item.Types _) as t, _, None  -> allTypes, constr, Some t
                                    |   (Item.Types _), _, _ -> allTypes, constr, typ
                                    |   (Item.CtorGroup _), None, _ -> allTypes, Some item, typ
                                    |   _ -> false, None, None) 
                                (true,None,None) items
                        match allTypes, constr, typ with
                        |   true, Some (Item.CtorGroup(_, _) as item), _    
                                -> GetF1Keyword item                        
                        |   true, _, Some typ
                                -> GetF1Keyword typ
                        |   _ -> None
            )    
            (fun _ -> None)

    member scope.GetMethods (line, lineStr, colAtEndOfNames, namesOpt) : MethodOverloads =
        ErrorScope.Protect 
            Range.range0 
            (fun () -> 
                use t = Trace.Call("CompilerServices", "GetMethods", fun _ -> sprintf "line = %d, idx = %d, names = %+A" line colAtEndOfNames namesOpt)
                match GetDeclItemsForNamesAtPosition(None,namesOpt,None,line,lineStr,colAtEndOfNames,ResolveTypeNamesToCtors,ResolveOverloads.No, fun _ -> false) with
                | None -> MethodOverloads("",[| |])
                | Some(items,denv,m) -> MethodOverloads.Create(infoReader,m,denv,items))
            (fun msg -> 
                MethodOverloads(msg,[| |]))

    member scope.GetDeclarationLocation (line : int, lineStr : string, idx : int, names : Names, tag : tokenId, isDecl : bool) : FindDeclResult =
      match tag with
      | TOKEN_IDENT -> 
          match GetDeclItemsForNamesAtPosition (None,Some(names), None, line, lineStr, idx, ResolveTypeNamesToCtors,ResolveOverloads.Yes, fun _ -> false) with
          | None
          | Some ([], _, _) -> FindDeclResult.DeclNotFound FindDeclFailureReason.Unknown
          | Some (item :: _ , _, _) -> 

              // For IL-based entities, switch to a different item. This is because
              // rangeOfItem, ccuOfItem don't work on IL methods or fields.
              //
              // Later comment: to be honest, they aren't going to work on these new items either.
              // This is probably old code from when we supported 'go to definition' generating IL metadata.
              let item =
                  match item with
                  | Item.MethodGroup (_, (ILMeth (_,ilinfo,_)) :: _) 
                  | Item.CtorGroup (_, (ILMeth (_,ilinfo,_)) :: _) -> Item.Types ("", [ ilinfo.ApparentEnclosingType ])
                  | Item.ILField (ILFieldInfo (typeInfo, _)) -> Item.Types ("", [ typeInfo.ToType ])
                  | Item.ImplicitOp(_, {contents = Some(TraitConstraintSln.FSMethSln(_, vref, _))}) -> Item.Value(vref)
                  | _                                         -> item

              let fail defaultReason = 
                  match item with            
#if EXTENSIONTYPING
                  | Params.ItemIsTypeWithStaticArguments g (tcref) -> FindDeclResult.DeclNotFound (FindDeclFailureReason.ProvidedType(tcref.DisplayName))
                  | Item.CtorGroup(name, ProvidedMeth(_)::_)
                  | Item.MethodGroup(name, ProvidedMeth(_)::_)
                  | Item.Property(name, ProvidedProp(_)::_) -> FindDeclResult.DeclNotFound (FindDeclFailureReason.ProvidedMember(name))
                  | Item.Event(ProvidedEvent(_) as e) -> FindDeclResult.DeclNotFound (FindDeclFailureReason.ProvidedMember(e.EventName))
                  | Item.ILField(ProvidedField(_) as f) -> FindDeclResult.DeclNotFound (FindDeclFailureReason.ProvidedMember(f.FieldName))
#endif
                  | _ -> FindDeclResult.DeclNotFound defaultReason

              match rangeOfItem g isDecl item with
              | None   -> fail FindDeclFailureReason.Unknown 
              | Some itemRange -> 

                  let filename = fileNameOfItem g (Some sProjectDir) itemRange item
                  if FileSystem.SafeExists filename then 
                      FindDeclResult.DeclFound ((itemRange.StartLine - 1, itemRange.StartColumn), filename)
                  else 
                      fail FindDeclFailureReason.NoSourceCode // provided items may have TypeProviderDefinitionLocationAttribute that binds them to some location

      | _ -> FindDeclResult.DeclNotFound FindDeclFailureReason.Unknown


    // Not, this does not have to be a SyncOp, it can be called from any thread
    member scope.GetExtraColorizations() = 
         [| for cnr in capturedNameResolutions do  
               match cnr with 
               /// 'seq' in 'seq { ... }' gets colored as keywords
               | CNR(_, (Item.Value vref), ItemOccurence.Use, _, _, _, m) when valRefEq g g.seq_vref vref -> 
                   yield ((Pos.toVS m.Start, Pos.toVS m.End), TokenColorKind.Keyword) 
               /// custom builders, custom operations get colored as keywords
               | CNR(_, (Item.CustomBuilder _ | Item.CustomOperation _), ItemOccurence.Use, _, _, _, m) -> 
                   yield ((Pos.toVS m.Start, Pos.toVS m.End), TokenColorKind.Keyword) 
#if COLORIZE_TYPES
               /// types get colored as types when they occur in syntactic types or custom attributes
               /// typevariables get colored as types when they occur in syntactic types custom builders, custom operations get colored as keywords
               | CNR(_, (Item.TypeVar  _ | Item.Types _ | Item.UnqualifiedType _) , (ItemOccurence.UseInType | ItemOccurence.UseInAttribute), _, _, _, m) -> 
                   yield ((Pos.toVS m.Start, Pos.toVS m.End), TokenColorKind.TypeName) 
#endif
               | _ -> () 
           |]

module internal Parser = 

    /// Error handler for parsing & type checking
    type ErrorHandler(reportErrors, mainInputFileName, tcConfig: TcConfig, source: string) =
        let mutable tcConfig = tcConfig
        let errorsAndWarningsCollector = new ResizeArray<_>()
        let mutable errorCount = 0
         
        // We'll need number of lines for adjusting error messages at EOF
        let fileInfo = 
            (source |> Seq.sumBy (fun c -> if c = '\n' then 1 else 0), // number of lines in the source file
                source.Length - source.LastIndexOf("\n",StringComparison.Ordinal) - 1)             // length of the last line
         
        // This function gets called whenever an error happens during parsing or checking
        let errorSink warn (exn:PhasedError) = 
            // Sanity check here. The phase of an error should be in a phase known to the language service.
            let exn =
                if not(exn.IsPhaseInCompile()) then
                    // Reaching this point means that the error would be sticky if we let it prop up to the language service.
                    // Assert and recover by replacing phase with one known to the language service.
                    System.Diagnostics.Debug.Assert(false, sprintf "The subcategory '%s' seen in an error should not be seen by the language service" (exn.Subcategory()))
                    {exn with Phase=BuildPhase.TypeCheck}
                else exn
            if reportErrors then 
                let report exn = 
                    let warn = warn && not (ReportWarningAsError tcConfig.globalWarnLevel tcConfig.specificWarnOff tcConfig.specificWarnOn tcConfig.specificWarnAsError tcConfig.specificWarnAsWarn tcConfig.globalWarnAsError exn)                
                    if (not warn || ReportWarning tcConfig.globalWarnLevel tcConfig.specificWarnOff tcConfig.specificWarnOn exn) then 
                        let oneError trim exn = 
                            // We use the first line of the file as a fallbackRange for reporting unexpected errors.
                            // Not ideal, but it's hard to see what else to do.
                            let fallbackRange = rangeN mainInputFileName 1
                            let ei = ErrorInfo.CreateFromExceptionAndAdjustEof(exn,warn,trim,fallbackRange,fileInfo)
                            if (ei.FileName=mainInputFileName) || (ei.FileName=Microsoft.FSharp.Compiler.Env.DummyFileNameForRangesWithoutASpecificLocation) then
                                Trace.PrintLine("UntypedParseAux", fun _ -> sprintf "Reporting one error: %s\n" (ei.ToString()))
                                errorsAndWarningsCollector.Add ei
                                if not warn then 
                                    errorCount <- errorCount + 1
                      
                        let mainError,relatedErrors = Build.SplitRelatedErrors exn 
                        oneError false mainError
                        List.iter (oneError true) relatedErrors
                match exn with
#if EXTENSIONTYPING
                | {Exception = (:? TypeProviderError as tpe)} ->
                    tpe.Iter (fun e ->
                        let newExn = {exn with Exception = e}
                        report newExn
                    )
#endif
                | e -> report e
      
        let errorLogger = 
            { new ErrorLogger("ErrorHandler") with 
                member x.WarnSinkImpl exn = errorSink true exn
                member x.ErrorSinkImpl exn = errorSink false exn
                member x.ErrorCount = errorCount }
      
      
        // Public members
        member x.ErrorLogger = errorLogger
        member x.CollectedErrorsAndWarnings = errorsAndWarningsCollector.ToArray()
        member x.ErrorCount = errorCount
        member x.TcConfig with set tc = tcConfig <- tc
        member x.AnyErrors = errorCount > 0

    /// Report an unexpect (bug) exception.
    let ReportUnexpectedException exn = 
        match exn with
        | WrappedError(wrappedExn,_) ->
            System.Diagnostics.Debug.Assert(false, sprintf "Bug seen in service-level request. Underlying wrapped exception was %s\n"  (wrappedExn.ToString()))
            Trace.PrintLine("CompilerServices", fun _ -> sprintf "Underlying wrapped exception was %s\n" (wrappedExn.ToString()))
        | _ -> ()
        System.Diagnostics.Debug.Assert(false, sprintf "Bug seen in service-level request: %s\n"  (exn.ToString()))
        Trace.PrintLine("CompilerServices", fun _ -> sprintf "Unexpected error %s\n" (exn.ToString()))
                

    /// ParseSource builds all the information necessary to report errors, match braces and build scopes 
    let ParseSource (source: string, matchBracesOnly: bool, reportErrors: bool, mainInputFileName: string, projectSourceFiles: string list, tcConfig: TcConfig) =

      try 
          Trace.PrintLine("CompilerServices", fun _ -> sprintf "Service.parseSource %s, matchBraces = %b, reportErrors = %b" mainInputFileName matchBracesOnly reportErrors)

          // Initialize the error handler 
          let errHandler = new ErrorHandler(reportErrors, mainInputFileName, tcConfig, source)

          // Very old comment: This helps reason=MethodTip to work. reason=MethodTip 
          // calls with only partial text.  Preumably adding this causes the final EndParameters 
          // call to refer to a different line than the StartParameters call we're really interested in 
          // Or something like that.  
          let source = source + "\n\n\n"
          let lexbuf = UnicodeLexing.StringAsLexbuf source

          // Colelctor for parens matching
          let matchPairRef = new ResizeArray<_>()

          use unwindEL = PushErrorLoggerPhaseUntilUnwind(fun _oldLogger -> errHandler.ErrorLogger)
          use unwindBP = PushThreadBuildPhaseUntilUnwind (BuildPhase.Parse)

          // Errors on while parsing project arguments 

          let parseResult = 

              // If we're editing a script then we define INTERACTIVE otherwise COMPILED. Since this parsing for intellisense we always
              // define EDITING
              let conditionalCompilationDefines =
                SourceFileImpl.AdditionalDefinesForUseInEditor(mainInputFileName) @ tcConfig.conditionalCompilationDefines 
        
              let lightSyntaxStatusInital = tcConfig.ComputeLightSyntaxInitialStatus mainInputFileName
              let lightSyntaxStatus = LightSyntaxStatus(lightSyntaxStatusInital,true)

              // Note: we don't really attempt to intern strings across a large scope
              let lexResourceManager = new Lexhelp.LexResourceManager()
              let lexargs = mkLexargs(mainInputFileName,
                                      conditionalCompilationDefines,
                                      lightSyntaxStatus,
                                      lexResourceManager,
                                      ref [],
                                      errHandler.ErrorLogger)
              Lexhelp.usingLexbufForParsing (lexbuf, mainInputFileName) (fun lexbuf -> 
                  try 
                    let skip = true
                    let tokenizer = Lexfilter.LexFilter (lightSyntaxStatus, tcConfig.compilingFslib, Lexer.token lexargs skip, lexbuf)
                    let lexfun = tokenizer.Lexer
                    if matchBracesOnly then 
                        // Quick bracket matching parse  
                        let parenTokensBalance t1 t2 = 
                            match t1,t2 with 
                            | (LPAREN,RPAREN) 
                            | (LPAREN,RPAREN_IS_HERE) 
                            | (LBRACE,RBRACE) 
                            | (LBRACE,RBRACE_IS_HERE) 
                            | (SIG,END) 
                            | (STRUCT,END) 
                            | (LBRACK_BAR,BAR_RBRACK)
                            | (LBRACK,RBRACK)
                            | (LBRACK_LESS,GREATER_RBRACK)
                            | (BEGIN,END) -> true 
                            | (LQUOTE q1,RQUOTE q2) when q1 = q2 -> true 
                            | _ -> false
                        let rec matchBraces stack = 
                            match lexfun lexbuf,stack with 
                            | tok2,((tok1,m1) :: stack') when parenTokensBalance tok1 tok2-> 
                                if matchBracesOnly then 
                                    matchPairRef.Add (Range.toVS m1, Range.toVS lexbuf.LexemeRange)
                                matchBraces stack'
                            | ((LPAREN | LBRACE | LBRACK | LBRACK_BAR | LQUOTE _ | LBRACK_LESS) as tok),_ -> matchBraces ((tok,lexbuf.LexemeRange) :: stack)
                            | (EOF _ | LEX_FAILURE _),_ -> ()
                            | _ -> matchBraces stack

                        matchBraces []
                        None
                    else 
                        let isLastCompiland = 
                            tcConfig.target.IsExe && 
                            projectSourceFiles.Length >= 1 && 
                            System.String.Compare(List.last projectSourceFiles,mainInputFileName,StringComparison.CurrentCultureIgnoreCase)=0
                        let isLastCompiland = isLastCompiland || Build.IsScript(mainInputFileName)  

                        let parseResult = ParseInput(lexfun,errHandler.ErrorLogger,lexbuf,None,mainInputFileName,isLastCompiland)
                        Some parseResult
                  with e -> 
                    Trace.PrintLine("CompilerServices", fun _ -> sprintf "Could not recover from errors while parsing: %s\n" (e.ToString()))
                    errHandler.ErrorLogger.ErrorR(e)
                    None)
                
          Trace.PrintLine("CompilerServices", fun _ -> sprintf "#errors = %d\n" errHandler.CollectedErrorsAndWarnings.Length)

          errHandler.CollectedErrorsAndWarnings,
          matchPairRef.ToArray(),
          parseResult,
          errHandler.AnyErrors
      with exn -> 
          ReportUnexpectedException exn
          reraise()

    /// An accumulator for the results being emitted into the tcSink.
    type TcResultsSinkImpl() =
        let capturedEnvs = new ResizeArray<_>(100)
        let capturedExprTypings = new ResizeArray<_>(100)
        let capturedNameResolutions = new ResizeArray<_>(100)
        let capturedNameResolutionIdentifiers = 
            new HashSet<pos * string>( { new IEqualityComparer<_> with 
                                            member __.GetHashCode((p:pos,i)) = p.Line + 101 * p.Column + hash i
                                            member __.Equals((p1,i1),(p2,i2)) = posEq p1 p2 && i1 =  i2 } )
        let capturedMethodGroupResolutions = new ResizeArray<_>(100)
        let allowedRange (m:range) = not m.IsSynthetic
        interface Nameres.ITypecheckResultsSink with
            member sink.NotifyEnvWithScope(m,nenv,ad) = 
                if allowedRange m then 
                    capturedEnvs.Add((m,nenv,ad)) 
            member sink.NotifyExprHasType(endPos,ty,denv,nenv,ad,m) = 
                if allowedRange m then 
                    capturedExprTypings.Add((endPos,ty,denv,nenv,ad,m))
            member sink.NotifyNameResolution(endPos,item,itemMethodGroup,occurenceType,denv,nenv,ad,m) = 
                // Desugaring some F# constructs (notably computation expressions with custom operators)
                // results in duplication of textual variables. So we ensure we never record two name resolutions 
                // for the same identifier at the same location.
                if allowedRange m then 
                    let keyOpt = match item with
                                 | Item.Value vref -> Some (endPos, vref.DisplayName)
                                 | Item.ArgName (id, _, _) -> Some (endPos, id.idText)
                                 | _ -> None

                    let alreadyDone = 
                        match keyOpt with
                        | Some key ->
                            let res = capturedNameResolutionIdentifiers.Contains key
                            if not res then capturedNameResolutionIdentifiers.Add key |> ignore
                            res
                        | _ -> false
                
                    if not alreadyDone then 
                        capturedNameResolutions.Add((endPos,item,occurenceType,denv,nenv,ad,m)) 
                        capturedMethodGroupResolutions.Add((endPos,itemMethodGroup,occurenceType,denv,nenv,ad,m)) 
        member x.CapturedEnvs = capturedEnvs
        member x.CapturedExprTypings = capturedExprTypings
        member x.CapturedNameResolutions = capturedNameResolutions
        member x.CapturedMethodGroupResolutions = capturedMethodGroupResolutions


    /// Indicates if the type check got aborted because it is no longer relevant.
    type TypeCheckAborted = Yes | No 

    // Type check a single file against an initial context, gleaning both errors and intellisense information.
    let TypeCheckSource
          (parseResult: ParsedInput option,
           source: string,
           mainInputFileName: string,
           projectFileName: string,
           tcConfig: TcConfig,
           tcGlobals: TcGlobals,
           tcImports: TcImports,
           tcState: TcState,
           loadClosure: LoadClosure option,
           // These are the errors and warnings seen by the background compiler for the entire antecedant 
           backgroundErrors: (PhasedError * bool) list,    
           syncop: (unit->unit)->unit,
           checkAlive : (unit -> bool),
           isResultObsolete: unit->bool,
           parseHadErrors : bool,
           textSnapshotInfo : obj) = 
      try
        let projectDir = Filename.directoryName (if projectFileName = "" then mainInputFileName else projectFileName)
        Trace.PrintLine("CompilerServices", fun _ -> sprintf "Parser.TypeCheckSource %s, projectDir = %s" mainInputFileName projectDir)
        match parseResult with 
        // When processing the following cases, we don't need to type-check
        | None -> 
            [| |], None, TypeCheckAborted.Yes
               
        // Run the type checker...
        | Some parsedMainInput ->

            // Initialize the error handler 
            let errHandler = new ErrorHandler(true,mainInputFileName,tcConfig, source)

            use unwindEL = PushErrorLoggerPhaseUntilUnwind (fun _oldLogger -> errHandler.ErrorLogger)
            use unwindBP = PushThreadBuildPhaseUntilUnwind (BuildPhase.TypeCheck)      
        
            // Apply nowarns to tcConfig (may generate errors, so ensure errorLogger is installed)
            let tcConfig = ApplyNoWarnsToTcConfig tcConfig (parsedMainInput,Path.GetDirectoryName mainInputFileName)
                    
            // update the error handler with the modified tcConfig
            errHandler.TcConfig <- tcConfig

            // Play background errors and warnings for this file.
            backgroundErrors
            |> List.iter(fun (err,iserr) ->if iserr then errorSink err else warnSink err)


            // If additional references were brought in by the preprocessor then we need to process them
            match loadClosure with
            | Some loadClosure ->
                // Play unresolved references for this file.
                tcImports.ReportUnresolvedAssemblyReferences(loadClosure.UnresolvedReferences)

                // If there was a loadClosure, replay the errors and warnings
                loadClosure.RootErrors |> List.iter errorSink
                loadClosure.RootWarnings |> List.iter warnSink
                

                let fileOfBackgroundError err = (match RangeOfError (fst err) with Some m-> m.FileName | None -> null)
                let sameFile file hashLoadInFile = 
                    (0 = String.Compare(fst hashLoadInFile, file, StringComparison.OrdinalIgnoreCase))

                //  walk the list of #loads and keep the ones for this file.
                let hashLoadsInFile = 
                    loadClosure.SourceFiles 
                    |> List.filter(fun (_,ms) -> ms<>[]) // #loaded file, ranges of #load

                let hashLoadBackgroundErrors, otherBackgroundErrors = 
                    backgroundErrors |> List.partition (fun backgroundError -> hashLoadsInFile |> List.exists (sameFile (fileOfBackgroundError backgroundError)))

                // Create single errors for the #load-ed files.
                // Group errors and warnings by file name.
                let hashLoadBackgroundErrorsGroupedByFileName = 
                    hashLoadBackgroundErrors 
                    |> List.map(fun err -> fileOfBackgroundError err,err) 
                    |> List.groupByFirst  // fileWithErrors, error list

                //  Join the sets and report errors. 
                //  It is by-design that these messages are only present in the language service. A true build would report the errors at their
                //  spots in the individual source files.
                for hashLoadInFile in hashLoadsInFile do
                    for errorGroupedByFileName in hashLoadBackgroundErrorsGroupedByFileName do
                        if sameFile (fst errorGroupedByFileName) hashLoadInFile then
                            for rangeOfHashLoad in snd hashLoadInFile do // Handle the case of two #loads of the same file
                                let errorsAndWarnings = snd errorGroupedByFileName |> List.map(fun (pe,f)->pe.Exception,f) // Strip the build phase here. It will be replaced, in total, with TypeCheck
                                let errors,warnings = errorsAndWarnings |> List.partition snd 
                                let errors,warnings = errors |> List.map fst, warnings |> List.map fst
                                
                                let message = HashLoadedSourceHasIssues(warnings,errors,rangeOfHashLoad)
                                if errors=[] then warning(message)
                                else errorR(message)

                // Replay other background errors.
                for (phasedError,isWarning) in otherBackgroundErrors do
                    if isWarning then warning phasedError.Exception else errorR phasedError.Exception

            | None -> 
                // For non-scripts, check for disallow #r and #load.
                ApplyMetaCommandsFromInputToTcConfig tcConfig (parsedMainInput,Path.GetDirectoryName mainInputFileName) |> ignore
                
            if verbose then 
                tcConfig.includes |> List.iter (fun p -> Trace.PrintLine("CompilerServicesVerbose", fun _ -> sprintf "include directory '%s'\n" p)) 
                tcConfig.implicitOpens |> List.iter (fun p -> Trace.PrintLine("CompilerServicesVerbose", fun _ -> sprintf "implicit open '%s'\n" p)) 
                tcConfig.referencedDLLs |> List.iter (fun r -> Trace.PrintLine("CompilerServicesVerbose", fun _ -> sprintf "dll from flags '%s'\n" r.Text)) 
            
            // A problem arises with nice name generation, which really should only 
            // be done in the backend, but is also done in the typechecker for better or worse. 
            // If we don't do this the NNG accumulates data and we get a memory leak. 
            tcState.NiceNameGenerator.Reset()
            
            // Typecheck the real input.  
            let sink = TcResultsSinkImpl()

            let amap = tcImports.GetImportMap()
            let tcEnvAtEndOpt =
                try
                    let checkForErrors() = (parseHadErrors || errHandler.ErrorCount > 0)
                    // Typecheck is potentially a long running operation. We chop it up here with an Eventually continuation and, at each slice, give a chance
                    // for the client to claim the result as obsolete and have the typecheck abort.
                    let computation = TypecheckSingleInputAndFinishEventually(checkForErrors,tcConfig, tcImports, tcGlobals, None, TcResultsSink.WithSink sink, tcState, parsedMainInput)
                    match computation |> Eventually.forceWhile (fun () -> not (isResultObsolete())) with
                    | Some((tcEnvAtEnd,_,_),_) -> Some tcEnvAtEnd
                    | None -> None // Means 'aborted'
                with
                | e ->
                    errorR e
                    Some(tcState.TcEnvFromSignatures)
            
            let errors = errHandler.CollectedErrorsAndWarnings
            
            match tcEnvAtEndOpt with
            | Some tcEnvAtEnd ->
                let scope = 
                    TypeCheckInfo(tcConfig, tcGlobals, amap, projectDir, mainInputFileName, 
                                sink.CapturedEnvs, tcEnvAtEnd.NameEnv,
                                sink.CapturedExprTypings,
                                sink.CapturedNameResolutions,
                                sink.CapturedMethodGroupResolutions,
                                loadClosure,
                                syncop,
                                checkAlive,
                                textSnapshotInfo)     
                errors, Some scope, TypeCheckAborted.No
            | None -> 
                errors, None, TypeCheckAborted.Yes
      with 
      | e -> 
        ReportUnexpectedException(e)
        reraise()

type internal UnresolvedReferencesSet = 
    val private set : System.Collections.Generic.HashSet<Build.UnresolvedAssemblyReference>
    new(unresolved) = {set = System.Collections.Generic.HashSet(unresolved, HashIdentity.Structural)}

    override this.Equals(o) = 
        match o with
        | :? UnresolvedReferencesSet as urs -> this.set.SetEquals(urs.set)
        | _ -> false

    override this.GetHashCode() = 
        // this code is copy from prim-types.fs: family of GenericHash...Array functions
        // it seems that it is never called, but we implement GetHashCode just to make the compiler happy
        let inline HashCombine nr x y = (x <<< 1) + y + 631 * nr
        ((0,0), this.set) 
        ||> Seq.fold (fun (acc, n) v -> (HashCombine n acc (hash v)), (n + 1))
        |> fst

// NOTE: may be better just to move to optional arguments here
type CheckOptions =
    { 
      ProjectFileName: string
      ProjectFileNames: string[]
      ProjectOptions: string[]
      IsIncompleteTypeCheckEnvironment : bool
      UseScriptResolutionRules : bool      
      LoadTime : System.DateTime
      UnresolvedReferences : UnresolvedReferencesSet option
    }
    /// Whether the two parse options refer to the same project.
    static member AreSameProjectName(options1,options2) =
        options1.ProjectFileName = options2.ProjectFileName          
    /// Compare two options sets with respect to the parts of the options that are important to parsing.
    static member AreSameProjectForParsing(options1,options2) =
        CheckOptions.AreSameProjectName(options1,options2) &&
        options1.ProjectOptions = options2.ProjectOptions &&
        (
            match options1.UnresolvedReferences, options2.UnresolvedReferences with
            | None, None -> true
            | (Some r1), (Some r2) -> r1.Equals(r2)
            | _ -> false
        )
    /// Compare two options sets with respect to the parts of the options that are important to building.
    static member AreSameProjectForBuilding(options1,options2) =
        CheckOptions.AreSameProjectForParsing(options1,options2) &&
        options1.ProjectFileNames = options2.ProjectFileNames &&
        options1.LoadTime = options2.LoadTime
    /// Compute the project directory.
    member po.ProjectDirectory = System.IO.Path.GetDirectoryName(po.ProjectFileName)
    override this.ToString() =
        let files =
            let sb = new StringBuilder()
            this.ProjectFileNames |> Array.iter (fun file -> sb.AppendFormat("    {0}\n", file) |> ignore)
            sb.ToString()
        let options =
            let sb = new StringBuilder()
            this.ProjectOptions |> Array.iter (fun op -> sb.AppendFormat("{0} ", op) |> ignore)
            sb.ToString()
        sprintf "CheckOptions(%s)\n  Files:\n%s  Options: %s" this.ProjectFileName files options
 
[<Sealed>]
/// A live object of this type keeps the background corresponding background builder (and type providers) alive (through reference-counting).
//
// There is an important property of all the objects returned by the methods of this type: they do not require 
// the corresponding background builder to be alive. That is, they are simply plain-old-data through pre-formatting of all result text.
type TypeCheckResults(errors: ErrorInfo[], details: (TypeCheckInfo * IncrementalFSharpBuild.IncrementalBuilder * Reactor.Reactor) option ) =

    // This may be None initially, or may be set to None when the object is disposed or finalized
    let mutable details = details

    let decrementer = 
        match details with 
        | None -> { new System.IDisposable with member x.Dispose() = () } 
        | Some (_,builder,_) -> 
            // Increment the usage count on the IncrementalBuilder. We want to keep the IncrementalBuilder and all associated
            // resources and type providers alive for the duration of the lifetime of this object.
            builder.IncrementUsageCount()

    let mutable disposed = false

    let dispose() = 
       if not disposed then 
           disposed <- true 
           match details with 
           | None -> () 
           | Some (_,_,reactor) -> 
               // Make sure we run disposal in the reactor thread, since it may trigger type provider disposals etc.
               details <- None
               reactor.AsyncOp (fun () -> decrementer.Dispose())

    let checkBuilder dflt f = 
        match details with
        | None -> 
            dflt
        | Some (_ , builder, _) when not builder.IsAlive -> 
            System.Diagnostics.Debug.Assert(false,"unexpected dead builder") 
            dflt
        | Some (scope, builder, reactor) -> 
            f(scope, builder, reactor)

    // At the moment we only dispose on finalize - we never explicitly dispose these objects. Explicitly disposing is not
    // really worth much since the underlying project builds are likely to still be in the incrementalBuilder cache.
    override info.Finalize() = dispose() 

    member info.Errors = errors

    member info.HasFullTypeCheckInfo = details.IsSome
    
    /// Intellisense autocompletions
    member info.GetDeclarations(untypedParseInfoOpt:UntypedParseInfo option, (line,colAtEndOfNames), lineStr, names:NamesWithResidue, hasTextChangedSinceLastTypecheck: (obj * Range -> bool)) = 
        checkBuilder (async.Return DeclarationSet.Empty) (fun (scope, builder, reactor) -> 
            async { // Ensure the builder doesn't get released while running GetDeclarations asynchronously. In synchronous operations,
                    // the builder is kept alive at least because the TypeCheckResults object itself is alive (note it is almsot certain to 
                    // be alive for other reasons too anyway, e.g. in the incrementalBuildersCache).
                    use _unwind = builder.IncrementUsageCount()
                    return! reactor.RunAsyncOp(fun () -> scope.GetDeclarations(untypedParseInfoOpt, line, lineStr, colAtEndOfNames, names, hasTextChangedSinceLastTypecheck))  
                  })

    /// Resolve the names at the given location to give a data tip 
    member info.GetDataTipText((x1,x2),lineStr,names:Names,tokenTag:int) : DataTipText = 
        use t = Trace.Call("SyncOp","GetDataTipText", fun _->sprintf " at=(%d:%d),names=%+A tag=%d tokenId=%+A" x1 x2 names tokenTag (tokenTagToTokenId tokenTag))
        let dflt = DataTipText []
        checkBuilder dflt (fun (scope, _builder, reactor) -> 
            match tokenTagToTokenId tokenTag with 
            | TOKEN_IDENT -> 
                reactor.RunSyncOp (fun () -> scope.GetDataTipText x1 lineStr x2 names)
            | TOKEN_STRING | TOKEN_STRING_TEXT -> 
                reactor.RunSyncOp (fun () -> scope.GetReferenceResolutionDataTipText(x1,x2))        
            | _ -> DataTipText [])

    member info.GetF1Keyword ((line,colAtEndOfNames),lineStr,names) : string option =
        use t = Trace.Call("SyncOp","GetF1Keyword", fun _->sprintf " at=(%d:%d),names=%+A" line colAtEndOfNames names)
        checkBuilder None (fun (scope, _builder, reactor) -> 
            reactor.RunSyncOp (fun () -> scope.GetF1Keyword (line, lineStr, colAtEndOfNames, names)))

    // Resolve the names at the given location to a set of methods
    member info.GetMethods((x1,x2):Position,lineStr:string,names:Names option) =
        use t = Trace.Call("SyncOp","GetMethods", fun _->sprintf " at=(%d:%d),names=%+A" x1 x2 names)
        let dflt = MethodOverloads("",[| |])
        checkBuilder dflt (fun (scope, _builder, reactor) -> 
            reactor.RunSyncOp (fun () -> scope.GetMethods (x1, lineStr, x2, names)))
            
    member info.GetDeclarationLocation ((x1, x2) : Position, lineStr:string, names : Names, tokenTag : int, flag : bool) = 
        use t = Trace.Call("SyncOp","GetDeclarationLocation", fun _->sprintf " at=(%d:%d),names=%+A,flag=%+A" x1 x2 names flag)
        let dflt = FindDeclResult.DeclNotFound FindDeclFailureReason.Unknown
        checkBuilder dflt (fun (scope, _builder, reactor) -> 
            reactor.RunSyncOp (fun () -> scope.GetDeclarationLocation (x1, lineStr, x2, names, tokenTagToTokenId tokenTag, flag)))

    member info.GetExtraColorizations() = 
        use t = Trace.Call("SyncOp","GetExtraColorizations", fun _->sprintf "")
        checkBuilder [| |] (fun (scope, _builder, _reactor) -> 
            scope.GetExtraColorizations())
     

/// Information about the compilation environment    
module internal CompilerEnvironment =
    /// These are the names of assemblies that should be referenced for .fs, .ml, .fsi, .mli files that
    /// are not asscociated with a project
    let DefaultReferencesForOrphanSources = DefaultBasicReferencesForOutOfProjectSources
    
    /// Publish compiler-flags parsing logic. Must be fast because its used by the colorizer.
    let GetCompilationDefinesForEditing(filename:string, compilerFlags : string list) =
        let defines = ref(SourceFileImpl.AdditionalDefinesForUseInEditor(filename))
        let MatchAndExtract(flag:string,prefix:string) =
            if flag.StartsWith(prefix) then 
                let sub = flag.Substring(prefix.Length)
                let trimmed = sub.Trim()
                defines := trimmed :: !defines
        let rec QuickParseDefines = function
            | hd :: tail ->
               MatchAndExtract(hd,"-d:")
               MatchAndExtract(hd,"--define:")
               QuickParseDefines tail
            | _ -> ()
        QuickParseDefines compilerFlags
        !defines
            
    /// Return true if this is a subcategory of error or warning message that the language service can emit
    let IsCheckerSupportedSubcategory(subcategory:string) =
        // Beware: This code logic is duplicated in DocumentTask.cs in the language service
        PhasedError.IsSubcategoryOfCompile(subcategory)

/// Information about the debugging environment
module internal DebuggerEnvironment =
    /// Return the language ID, which is the expression evaluator id that the
    /// debugger will use.
    let GetLanguageID() =
        System.Guid(0xAB4F38C9u, 0xB6E6us, 0x43baus, 0xBEuy, 0x3Buy, 0x58uy, 0x08uy, 0x0Buy, 0x2Cuy, 0xCCuy, 0xE3uy)
        
    
[<NoComparison>]
type internal TypeCheckAnswer =
    | NoAntecedant
    | Aborted
    | TypeCheckSucceeded of TypeCheckResults   
        
/// This file has become eligible to be re-typechecked.
type internal NotifyFileTypeCheckStateIsDirty = NotifyFileTypeCheckStateIsDirty of (string -> unit)
        
// Identical to _VSFILECHANGEFLAGS in vsshell.idl
type internal DependencyChangeCode =
    | NoChange = 0x00000000
    | FileChanged = 0x00000001
    | TimeChanged = 0x00000002
    | Size = 0x00000004
    | Deleted = 0x00000008
    | Added = 0x00000010        

/// Callback that indicates whether a requested result has become obsolete.    
[<NoComparison;NoEquality>]
type internal IsResultObsolete = 
    | IsResultObsolete of (unit->bool)

        
// There is only one instance of this type, held in InteractiveChecker
type BackgroundCompiler(notifyFileTypeCheckStateIsDirty:NotifyFileTypeCheckStateIsDirty) as self =
    // STATIC ROOT: LanguageServiceState.InteractiveChecker.backgroundCompiler.reactor: The one and only Reactor
    let reactor = Reactor.Reactor()

    // STATIC ROOT: LanguageServiceState.InteractiveChecker.backgroundCompiler.scriptClosure 
    /// Information about the derived script closure.
    let scriptClosure = AgedLookup<CheckOptions,LoadClosure>(buildCacheSize, areSame=CheckOptions.AreSameProjectForBuilding)

    /// CreateOneIncrementalBuilder (for background type checking). Note that fsc.fs also
    /// creates an incremental builder used by the command line compiler.
    let CreateOneIncrementalBuilder (options:CheckOptions) = 
        use t = Trace.Call("Reactor","CreateOneIncrementalBuilder", fun () -> sprintf "options = %+A" options)
        let builder, errorsAndWarnings = 
            IncrementalFSharpBuild.IncrementalBuilder.CreateBackgroundBuilderForProjectOptions
                  (scriptClosure.TryGet options, Array.toList options.ProjectFileNames, 
                   Array.toList options.ProjectOptions, options.ProjectDirectory, 
                   options.UseScriptResolutionRules, options.IsIncompleteTypeCheckEnvironment)

        // We're putting the builder in the cache, so increment its count.
        let decrement = builder.IncrementUsageCount()
        // Register the behaviour that responds to CCUs being invalidated because of type
        // provider Invalidate events. This invalidates the configuration in the build.
        builder.ImportedCcusInvalidated.Add (fun msg -> 
            System.Diagnostics.Debugger.Log(100, "service", sprintf "A build cache entry is being invalidated because of a : %s" msg)
            self.InvalidateConfiguration options)

        // Register the callback called just before a file is typechecked by the background builder (without recording
        // errors or intellisense information).
        //
        // This indicates to the UI that the file type check state is dirty. If the file is open and visible then 
        // the UI will sooner or later request a typecheck of the file, recording errors and intellisense information.
        builder.BeforeTypeCheckFile.Add (fun msg -> match notifyFileTypeCheckStateIsDirty with NotifyFileTypeCheckStateIsDirty f -> f msg)

        (builder, errorsAndWarnings, decrement)

    // STATIC ROOT: LanguageServiceState.InteractiveChecker.backgroundCompiler.incrementalBuildersCache. This root typically holds more 
    // live information than anything else in the F# Language Service, since it holds up to 3 (buildCacheSize) background project builds
    // strongly.
    // 
    /// Cache of builds keyed by options.        
    let incrementalBuildersCache = 
        MruCache(keepStrongly=buildCacheSize, keepMax=buildCacheSize, compute=CreateOneIncrementalBuilder, 
                 areSame =  CheckOptions.AreSameProjectForBuilding, 
                 areSameForSubsumption =  CheckOptions.AreSameProjectName,
                 onDiscard = (fun (_, _, decrement) -> decrement.Dispose()))

    
    /// Helper: get the antecedant typecheck state for the give file (in the options). Return none if not available.        
    let GetAntecendantResultWithoutSideEffects(filename:string, options:CheckOptions) = 
        match incrementalBuildersCache.GetAvailable options with
        | Some(builder, createErrors, _) ->
            let slotOfFile = builder.GetSlotOfFileName(filename)
            Some (builder, createErrors, builder.GetAntecedentTypeCheckResultsBySlot slotOfFile)
        | None->
            None        
    

    /// Helper: do one step of the build for the given options.
    let DoStep(options:CheckOptions) = 
        // Do the step.
        let builder,_,_ = incrementalBuildersCache.Get(options)
        if builder.Step() then 
            Trace.PrintLine("ChangeEvents", fun _ -> sprintf "CheckOptions(%s) now maps to Build(%s)" (options.ToString()) (builder.ToString()))
            true
        else
            false

    /// Parses the source file and returns untyped AST
    member bc.UntypedParse(filename:string, source,options:CheckOptions)=
        use t = Trace.Call("SyncOp","UntypedParse", fun _->"")
        reactor.RunSyncOp (fun () -> 
            Trace.PrintLine("CompilerServices", fun _ -> "Service.UntypedParseImpl")
            use t = Trace.CallByThreadNamed("Reactor", "UntypedParseImpl", "ThreadPool", fun _->"")  
        
#if TYPE_PROVIDER_SECURITY
            ExtensionTyping.GlobalsTheLanguageServiceCanPoke.theMostRecentFileNameWeChecked <- Some filename
#endif
            let builder,_,_ = incrementalBuildersCache.Get(options) // Q: Whis it it ok to ignore createErrors in the build cache? A: These errors will be appended into the typecheck results
            
            // Do the parsing.
            // REVIEW: _matchPairs is being ignored here
            let parseErrors, _matchPairs, inputOpt, anyErrors = 
               Parser.ParseSource (source, false, true, filename, (options.ProjectFileNames |> Array.toList), builder.TcConfig)
                 
            // Strip everything but the file name.
            let dependencyFiles = builder.Dependencies |> List.map (fun dep->dep.Filename)

            UntypedParseInfo(parsed = { Errors = parseErrors 
                                        Input = inputOpt
                                        ParseHadErrors = anyErrors
                                        DependencyFiles = dependencyFiles})) 
     
#if NO_QUICK_SEARCH_HELPERS // only used in QuickSearch prototype
#else
    member bc.UntypedParseForSlot(slot,options) =
        use t = Trace.Call("SyncOp","UntypedParseForSlot", fun _->"")
        reactor.RunSyncOp (fun () -> 
            Trace.PrintLine("CompilerServices", fun _ -> "Service.UntypedParseForSlotImpl")
            use t = Trace.CallByThreadNamed("Reactor", "UntypedParseImpl", "ThreadPool", fun _->"")  
            let builder,_,_= incrementalBuildersCache.Get(options)
            let inputOpt,_,_ = builder.GetParseResultsBySlot slot            
            Trace.PrintLine("ChangeEvents", fun _ -> sprintf "CheckOptions(%s) now maps to Build(%s)" (options.ToString()) (builder.ToString()))
            // Strip everything but the file name.
            let dependencyFiles = builder.Dependencies |> List.map (fun dep->dep.Filename)
        
            UntypedParseInfo(parsed = { Errors = [| |] 
                                        Input = inputOpt
                                        ParseHadErrors = false
                                        DependencyFiles = dependencyFiles})) 
        
    member bc.GetSlotsCount(options) =
        use t = Trace.Call("SyncOp","GetSlotsCount", fun _->"")
        reactor.RunSyncOp (fun () -> 
            Trace.PrintLine("CompilerServices", fun _ -> "Service.GetSlotsCountImpl")
            use t = Trace.CallByThreadNamed("Reactor", "GetSlotsCountImpl", "ThreadPool", fun _->"")  
            let builder,_,_ = incrementalBuildersCache.Get options
            builder.GetSlotsCount ())
#endif // QUICK_SEARCH
     
    member bc.MatchBraces(filename:string, source,options)=
        use t = Trace.Call("SyncOp","MatchBraces", fun _->"")
        reactor.RunSyncOp (fun () -> 
            Trace.PrintLine("CompilerServices", fun _ -> "Service.MatchBracesImpl")
            use t = Trace.CallByThreadNamed("Reactor", "MatchBracesImpl", "ThreadPool", fun _->"")  
            let builder,_,_ = incrementalBuildersCache.Get(options)
            // Do the parsing.
            let _parseErrors, matchPairs, _inputOpt, _anyErrors = 
               Parser.ParseSource (source, true, false, filename, (options.ProjectFileNames |> Array.toList), builder.TcConfig)
                 
            matchPairs)

    /// Type-check the result obtained by parsing
    /// The input should be first parsed using 'UntypedParseImpl'
    member bc.TypeCheckSource(parseResult,filename:string,source,options,isResultObsolete:unit->bool,textSnapshotInfo)=
        use t = Trace.Call("SyncOp","TypeCheckSource", fun _->"")
        reactor.RunSyncOp (fun () -> 
            use t = Trace.CallByThreadNamed("Reactor", "TypeCheckSourceImpl", "ThreadPool", fun _->"")  
        
            match GetAntecendantResultWithoutSideEffects(filename,options) with
            | Some(builder,createErrors,Some(tcPriorState,tcImports,tcGlobals,tcConfig,backgroundErrors,_antecedantTimeStamp)) -> 
        
                // Get additional script #load closure information if applicable.
                // For scripts, this will have been recorded by GetCheckOptionsFromScriptRoot.
                let loadClosure = scriptClosure.TryGet options 
        
                let parseHadErrors = parseResult.ParseHadErrors 

                // Run the type checking.
                let tcErrors, scopeOpt, aborted = 
                    Parser.TypeCheckSource(parseResult.Input,source,filename,options.ProjectFileName,tcConfig,tcGlobals,tcImports,  tcPriorState,
                                           loadClosure,backgroundErrors,reactor.SyncOp,(fun () -> builder.IsAlive),isResultObsolete,parseHadErrors,textSnapshotInfo)

                if aborted = Parser.TypeCheckAborted.No then                           
                                                    
                    // Append all the errors together.
                    let errors = 
                        [| yield! createErrors 
                           yield! parseResult.Errors
                           if options.IsIncompleteTypeCheckEnvironment then 
                               yield! Seq.truncate maxTypeCheckErrorsOutOfProjectContext tcErrors
                           else 
                               yield! tcErrors |]
                
                    let res = TypeCheckResults (errors,(match scopeOpt with None -> None | Some scope -> Some (scope, builder, reactor)))   
                    TypeCheckSucceeded res
                else Aborted                
            | _ -> 
                // Either the builder did not exist or the antecedent to the slot was not ready. Return 'None'.
                // The caller will send a request for a background build of this project. This
                // will create the builder and notify the UI when the antecedent to the slot is ready. 
                NoAntecedant)

    member bc.GetCheckOptionsFromScriptRoot(filename, source, loadedTimestamp, otherFlags) = 
        reactor.RunSyncOp (fun () -> 
            // REVIEW: Opportunity to cache by filename, version?
            // REVIEW: Opportunity to save script 'input' which is about to be generated including children.
            let fas = LoadClosure.ComputeClosureOfSourceText(filename, source, CodeContext.Editing, new Lexhelp.LexResourceManager())
            let baseFlags =  ["--noframework"; "--warn:3"]  @ Array.toList otherFlags
            let references = fas.References |> List.map (fun r->"-r:" + fst r)
            let nowarns = fas.NoWarns |> List.map (fun (code,_)->"--nowarn:" + code)
            let allFlags = baseFlags @ references @ nowarns @ Array.toList otherFlags
            let co = 
                {
                    ProjectFileName = filename + ".fsproj" // Make a name that is unique in this directory.
                    ProjectFileNames = fas.SourceFiles |> List.map(fun s->fst s) |> List.toArray
                    ProjectOptions = allFlags |> List.toArray
                    IsIncompleteTypeCheckEnvironment = false
                    UseScriptResolutionRules = true 
                    LoadTime = loadedTimestamp
                    UnresolvedReferences = Some (UnresolvedReferencesSet(fas.UnresolvedReferences))
                }
            scriptClosure.Put(co,fas) // Save the full load closure for later correlation.
            co)
            
    member bc.InvalidateConfiguration(options : CheckOptions) =
        use t = Trace.Call("SyncOp","InvalidateConfiguration", fun _->"")
        reactor.AsyncOp (fun () -> 
            use t = Trace.Call("ChangeEvents","InvalidateBuildCacheEntry", fun _ -> sprintf "Received notification to invalidate build for options: %A" options)
            match incrementalBuildersCache.GetAvailable options with
            | None -> ()
            | Some (_oldBuilder, _, _) ->
                    // We do not need to decrement here - the onDiscard function is called each time an entry is pushed out of the build cache,
                    // including by SetAlternate.
                    Trace.PrintLine("ChangeEvents", fun _ -> "Refreshing configuration")
                    let builderB, errorsB, decrementB = CreateOneIncrementalBuilder options
                    Trace.PrintLine("ChangeEvents", fun _ -> sprintf "CheckOptions(%s) now maps to Build(%s)" (options.ToString()) (builderB.ToString()))
                    incrementalBuildersCache.SetAlternate(options, (builderB, errorsB, decrementB)))
        reactor.StartBuilding(fun () -> DoStep options) 

    member bc.NotifyProjectCleaned(options : CheckOptions) =
        match incrementalBuildersCache.GetAvailable options with
        | None -> ()
        | Some (incrementalBuilder, _, _) ->
#if EXTENSIONTYPING
            if incrementalBuilder.ThereAreLiveTypeProviders then
                bc.InvalidateConfiguration(options)
#else
            ()
#endif

    member bc.InvalidateAll() =
        use t = Trace.Call("SyncOp","InvalidateAll", fun _->"")
        reactor.AsyncOp (fun () -> incrementalBuildersCache.Clear())


    member bc.StartBuilding(options) =
        reactor.StartBuilding(fun () -> DoStep options) 

    member bc.StopBuilding() =
        reactor.StopBuilding() 

    // This is for unit testing only
    member bc.WaitForBackgroundCompile() =
        reactor.WaitForBackgroundCompile() 

[<Sealed>]
[<AutoSerializable(false)>]
// There is typically only one instance of this type in a Visual Studio process.
type (* internal *) InteractiveChecker(notifyFileTypeCheckStateIsDirty) =
    // STATIC ROOT: LanguageServiceState.InteractiveChecker.backgroundCompiler. See BackgroundCompiler above.
    let backgroundCompiler = BackgroundCompiler(notifyFileTypeCheckStateIsDirty)

    static let mutable foregroundParseCount = 0
    static let mutable foregroundTypeCheckCount = 0
    
    /// Determine whether two sets of sources and parse options are the same.
    let AreSameForParsing((f1: string, s1: string, o1: CheckOptions), (f2, s2, o2)) =
        (f1 = f2) 
        && CheckOptions.AreSameProjectForParsing(o1,o2)
        && (s1 = s2)
        
    /// Determine whether two sets of sources and parse options should be subsumed under the same project.
    let AreSubsumableForParsing((_,_,o1:CheckOptions),(_,_,o2:CheckOptions)) =
        CheckOptions.AreSameProjectName(o1,o2)
        
    // Parse using backgroundCompiler
    let ComputeBraceMatching(filename:string,source,options:CheckOptions) = 
        Trace.PrintLine("CompilerServices", fun () -> sprintf "ComputeBraceMatching, FileName = %s\n  " filename) 
        backgroundCompiler.MatchBraces(filename,source,options)
    
    /// Parse a source code file, returning an information about the untyped results
    /// and the results needed for further processing using 'TypeCheckSource'
    let ComputeUntypedParse(filename: string, source, options) =
        Trace.PrintLine("CompilerServices", fun () -> sprintf "UntypedParse, FileName = %s\n  " filename) 
        foregroundParseCount <- foregroundParseCount + 1
        backgroundCompiler.UntypedParse(filename, source, options)

    // STATIC ROOT: LanguageServiceState.InteractiveChecker.braceMatchMru. Most recently used cache for brace matching. Accessed on the
    // background UI thread, not on the compiler thread.
    let braceMatchMru = MruCache<_,_>(braceMatchCacheSize,ComputeBraceMatching,areSame=AreSameForParsing,areSameForSubsumption=AreSubsumableForParsing,isStillValid=(fun _ -> true)) 

    // STATIC ROOT: LanguageServiceState.InteractiveChecker.untypedCheckMru. Most recently used cache for parsing files.
    let untypedCheckMru = MruCache<_, _>(untypedCheckMruSize, ComputeUntypedParse, areSame=AreSameForParsing,areSameForSubsumption=AreSubsumableForParsing,isStillValid=(fun _ -> true))

    // STATIC ROOT: LanguageServiceState.InteractiveChecker.typeCheckLookup. 
    //
    /// Cache which holds recently seen type-checks, no more than one for each file.
    /// This cache may hold out-of-date entries, in two senses
    ///    - there may be a more recent antecedent state available because the background build has made it available
    ///    - the source for the file may have changed
    
    let typeCheckLookup = 
        AgedLookup<string * CheckOptions,UntypedParseInfo * TypeCheckResults *int>(keepStrongly=recentForgroundTypeCheckLookupSize,
                                                                                   areSame=fun (x,y)->x=y) 

    /// Instantiate an interactive checker.    
    static member Create(notifyFileTypeCheckStateIsDirty) = new InteractiveChecker(notifyFileTypeCheckStateIsDirty)

    /// Parse a source code file, returning an information about the untyped results
    /// and the results needed for further processing using 'TypeCheckSource'
    member ic.MatchBraces(filename, source, options) =
        braceMatchMru.Get((filename, source, options))

    /// Parse a source code file, returning an information about the untyped results
    /// and the results needed for further processing using 'TypeCheckSource'
    member ic.UntypedParse(filename, source, options) =
        untypedCheckMru.Get((filename, source, options))
        
#if NO_QUICK_SEARCH_HELPERS // only used in QuickSearch prototype
#else
    member ic.GetSlotsCount options =
        Trace.PrintLine("CompilerServices", fun () -> sprintf "GetSlotsCount, ProjectName = %s\n  " options.ProjectFileName)         
        backgroundCompiler.GetSlotsCount(options)
        
    member ic.UntypedParseForSlot (slot,options) =
        Trace.PrintLine("CompilerServices", fun () -> sprintf "UntypedParseForSlot, ProjectName = %s, slot = %d\n  " options.ProjectFileName slot)         
        backgroundCompiler.UntypedParseForSlot(slot,options)
#endif // QUICK_SEARCH
        
    /// Try to get recent approximate type check results for a file. 
    member ic.TryGetRecentTypeCheckResultsForFile(filename: string, options:CheckOptions) =
        match typeCheckLookup.TryGet((filename,options)) with
        | Some res -> 
            Some res
        | _ -> 
            None

    /// This function is called when the entire environment is known to have changed for reasons not encoded in the CheckOptions of any project/compilation.
    /// For example, the type provider approvals file may have changed.
    member ic.InvalidateAll() =
        backgroundCompiler.InvalidateAll()
            
    /// This function is called when the entire environment is known to have changed for reasons not encoded in the CheckOptions of any project/compilation.
    /// For example, the type provider approvals file may have changed.
    //
    // This is for unit testing only
    member ic.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients() =
        ic.InvalidateAll()
        typeCheckLookup.Clear()
        braceMatchMru.Clear()
        untypedCheckMru.Clear()
        for i in 0 .. 2 do 
            System.GC.Collect()
            System.GC.WaitForPendingFinalizers() 
            backgroundCompiler.WaitForBackgroundCompile() // flush AsyncOp
            
    /// This function is called when the configuration is known to have changed for reasons not encoded in the CheckOptions.
    /// For example, dependent references may have been deleted or created.
    member ic.InvalidateConfiguration(options: CheckOptions) =
        backgroundCompiler.InvalidateConfiguration options

    /// This function is called when a project has been cleaned, and thus type providers should be refreshed.
    member ic.NotifyProjectCleaned(options: CheckOptions) =
        backgroundCompiler.NotifyProjectCleaned options
              
    /// TypeCheck a source code file, returning a handle to the results of the 
    /// parse including the reconstructed types in the file.
    member ic.TypeCheckSource(parsed:UntypedParseInfo, filename:string, fileVersion:int, source:string, options:CheckOptions, IsResultObsolete(isResultObsolete), textSnapshotInfo:obj) =        
        Trace.PrintLine("CompilerServices", fun () -> sprintf "TypeCheckSource, FileName = %s\n  " filename) 
        let answer = backgroundCompiler.TypeCheckSource(UntypedParseInfoImpl.GetUntypedParseResults parsed,filename,source,options,isResultObsolete,textSnapshotInfo)
        match answer with 
        | Aborted
        | NoAntecedant ->
            backgroundCompiler.StartBuilding(options) 
            answer
        | TypeCheckSucceeded typedResults -> 
            foregroundTypeCheckCount <- foregroundTypeCheckCount + 1
            typeCheckLookup.Put((filename,options),(parsed,typedResults,fileVersion))            
            // JAF: Why don't we kick the backgroundCompiler off here like we do for Aborted and NoAntecedant? 
            // Because we expect the second half of the request (GetMethods or whatever) soon and would like that to have a chance to start that request quickly
            answer
            
    /// For a given script file, get the CheckOptions implied by the #load closure
    member ic.GetCheckOptionsFromScriptRoot(filename, source, loadedTimestamp) = 
        ic.GetCheckOptionsFromScriptRoot(filename, source, loadedTimestamp, [| |]) 

    member ic.GetCheckOptionsFromScriptRoot(filename : string, source : string, loadedTimestamp : DateTime, otherFlags) :  CheckOptions = 
        backgroundCompiler.GetCheckOptionsFromScriptRoot(filename,source,loadedTimestamp, otherFlags)
        
    /// Begin background parsing the given project.
    member ic.StartBackgroundCompile(options) = backgroundCompiler.StartBuilding(options) 
    /// Stop the background compile.
    member ic.StopBackgroundCompile() = backgroundCompiler.StopBuilding()

    /// Block until the background compile finishes.
    //
    // This is for unit testing only
    member ic.WaitForBackgroundCompile() = backgroundCompiler.WaitForBackgroundCompile()

    static member GlobalForegroundParseCountStatistic = foregroundParseCount
    static member GlobalForegroundTypeCheckCountStatistic = foregroundTypeCheckCount
          
module internal PrettyNaming =
    let IsIdentifierPartCharacter     = Microsoft.FSharp.Compiler.PrettyNaming.IsIdentifierPartCharacter
    let IsLongIdentifierPartCharacter = Microsoft.FSharp.Compiler.PrettyNaming.IsLongIdentifierPartCharacter
    let GetLongNameFromString         = Microsoft.FSharp.Compiler.PrettyNaming.SplitNamesForILPath
    let FormatAndOtherOverloadsString(remainingOverloads) = FSComp.SR.typeInfoOtherOverloads(remainingOverloads)
        

#if EXTENSIBLE_DUMPER
#if DEBUG

namespace Internal.Utilities.Diagnostic
open Microsoft.FSharp.Compiler.Env
open Microsoft.FSharp.Compiler.Tastops 
open Microsoft.FSharp.Compiler.Infos
open Microsoft.FSharp.Compiler
open System.Text

type internal typDumper(dumpTarget:Microsoft.FSharp.Compiler.Tast.TType) =
    override self.ToString() = 
        match !global_g with
        | Some g -> 
            let denv = DisplayEnv.Empty g
            let sb = StringBuilder()
            NicePrint.outputTy denv sb dumpTarget
            sb.ToString()
        | None -> "No global environment"
    
#endif    
#endif
