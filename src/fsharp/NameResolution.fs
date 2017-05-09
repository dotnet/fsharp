// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

//-------------------------------------------------------------------------
// Name environment and name resolution 
//------------------------------------------------------------------------- 


module internal Microsoft.FSharp.Compiler.NameResolution

open Internal.Utilities
open Microsoft.FSharp.Compiler 
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.ErrorLogger
open Microsoft.FSharp.Compiler.Tast
open Microsoft.FSharp.Compiler.Tastops
open Microsoft.FSharp.Compiler.Import
open Microsoft.FSharp.Compiler.TcGlobals
open Microsoft.FSharp.Compiler.Lib
open Microsoft.FSharp.Compiler.AbstractIL 
open Microsoft.FSharp.Compiler.AbstractIL.Internal 
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library
open Microsoft.FSharp.Compiler.AbstractIL.Internal.Library.ResultOrException
open Microsoft.FSharp.Compiler.AbstractIL.Diagnostics
open Microsoft.FSharp.Compiler.AbstractIL.IL
open Microsoft.FSharp.Compiler.Infos
open Microsoft.FSharp.Compiler.AccessibilityLogic
open Microsoft.FSharp.Compiler.AttributeChecking
open Microsoft.FSharp.Compiler.InfoReader
open Microsoft.FSharp.Compiler.Layout
open Microsoft.FSharp.Compiler.PrettyNaming
open System.Collections.Generic

#if EXTENSIONTYPING
open Microsoft.FSharp.Compiler.ExtensionTyping
#endif

/// An object that captures the logical context for name resolution.
type NameResolver(g:TcGlobals, 
                  amap: Import.ImportMap, 
                  infoReader: InfoReader, 
                  instantiationGenerator: (range -> Typars -> TypeInst)) =
    /// Used to transform typars into new inference typars 
    // instantiationGenerator is a function to help us create the
    // type parameters by copying them from type parameter specifications read
    // from IL code.  
    //
    // When looking up items in generic types we create a fresh instantiation 
    // of the type, i.e. instantiate the type with inference variables. 
    // This means the item is returned ready for use by the type inference engine 
    // without further freshening. However it does mean we end up plumbing 'instantiationGenerator' 
    // around a bit more than we would like to, which is a bit annoying. 
    member nr.InstantiationGenerator = instantiationGenerator
    member nr.g = g
    member nr.amap = amap
    member nr.InfoReader = infoReader
    
//-------------------------------------------------------------------------
// Helpers for unionconstrs and recdfields
//------------------------------------------------------------------------- 

/// Get references to all the union cases in the type definition
let UnionCaseRefsInTycon (modref: ModuleOrNamespaceRef) (tycon:Tycon) = 
    tycon.UnionCasesAsList |> List.map (mkModuleUnionCaseRef modref tycon)

/// Get references to all the union cases defined in the module
let UnionCaseRefsInModuleOrNamespace (modref:ModuleOrNamespaceRef) = 
    [ for x in modref.ModuleOrNamespaceType.AllEntities do yield! UnionCaseRefsInTycon modref x ]

/// Try to find a type with a union case of the given name
let TryFindTypeWithUnionCase (modref:ModuleOrNamespaceRef) (id: Ident) = 
    modref.ModuleOrNamespaceType.AllEntities
    |> QueueList.tryFind (fun tycon -> tycon.GetUnionCaseByName id.idText |> Option.isSome) 

/// Try to find a type with a record field of the given name
let TryFindTypeWithRecdField (modref:ModuleOrNamespaceRef) (id: Ident) = 
    modref.ModuleOrNamespaceType.AllEntities
    |> QueueList.tryFind (fun tycon -> tycon.GetFieldByName id.idText |> Option.isSome)

/// Get the active pattern elements defined by a given value, if any
let ActivePatternElemsOfValRef vref = 
    match TryGetActivePatternInfo vref with
    | Some apinfo -> apinfo.ActiveTags |> List.mapi (fun i _ -> APElemRef(apinfo,vref, i)) 
    | None -> [] 


/// Try to make a reference to a value in a module.
//
// mkNestedValRef may fail if the assembly load set is
// incomplete and the value is an extension member of a type that is not
// available. In some cases we can reasonably recover from this, e.g. by simply not adding 
// an entry to a table. Callsites have to cope with the error (None) condition
// sensibly, e.g. in a way that won't change the way things are compiled as the 
// assembly set is completed.
let TryMkValRefInModRef modref vspec =
    protectAssemblyExploration
        None
        (fun () -> Some (mkNestedValRef modref vspec))

/// Get the active pattern elements defined by a given value, if any
let ActivePatternElemsOfVal modref vspec = 
    // If the assembly load set is incomplete then don't add anything to the table
    match TryMkValRefInModRef modref vspec with 
    | None -> []
    | Some vref -> ActivePatternElemsOfValRef vref


/// Get the active pattern elements defined in a module, if any. Cache in the slot in the module type.
let ActivePatternElemsOfModuleOrNamespace (modref:ModuleOrNamespaceRef) : NameMap<ActivePatternElemRef> = 
    let mtyp = modref.ModuleOrNamespaceType
    cacheOptRef mtyp.ActivePatternElemRefLookupTable (fun () ->
        mtyp.AllValsAndMembers 
        |> Seq.collect (ActivePatternElemsOfVal modref) 
        |> Seq.fold (fun acc apref -> NameMap.add apref.Name apref acc) Map.empty)

//---------------------------------------------------------------------------
// Name Resolution Items
//------------------------------------------------------------------------- 

/// Detect a use of a nominal type, including type abbreviations.
///
/// When reporting symbols, we care about abbreviations, e.g. 'int' and 'int32' count as two separate symbols
let (|AbbrevOrAppTy|_|) (typ: TType) = 
    match stripTyparEqns typ with 
    | TType_app (tcref,_) -> Some tcref
    | _ -> None

[<NoEquality; NoComparison; RequireQualifiedAccess>]
/// Represents the item with which a named argument is associated.
type ArgumentContainer =
    /// The named argument is an argument of a method
    | Method of MethInfo
    /// The named argument is a static parameter to a provided type or a parameter to an F# exception constructor
    | Type of TyconRef
    /// The named argument is a static parameter to a union case constructor
    | UnionCase of UnionCaseInfo

// Note: Active patterns are encoded like this:
//   let (|A|B|) x = if x < 0 then A else B    // A and B are reported as results using 'Item.ActivePatternResult' 
//   match () with | A | B -> ()               // A and B are reported using 'Item.ActivePatternCase'

[<NoEquality; NoComparison; RequireQualifiedAccess>]
/// Represents an item that results from name resolution
type Item = 

    /// Represents the resolution of a name to an F# value or function. 
    | Value of  ValRef

    /// Represents the resolution of a name to an F# union case.
    | UnionCase of UnionCaseInfo * bool

    /// Represents the resolution of a name to an F# active pattern result.
    | ActivePatternResult of ActivePatternInfo * TType * int  * range

    /// Represents the resolution of a name to an F# active pattern case within the body of an active pattern.
    | ActivePatternCase of ActivePatternElemRef 

    /// Represents the resolution of a name to an F# exception definition.
    | ExnCase of TyconRef 

    /// Represents the resolution of a name to an F# record field.
    | RecdField of RecdFieldInfo

    // The following are never in the items table but are valid results of binding 
    // an identifier in different circumstances. 

    /// Represents the resolution of a name at the point of its own definition.
    | NewDef of Ident

    /// Represents the resolution of a name to a .NET field 
    | ILField of ILFieldInfo

    /// Represents the resolution of a name to an event
    | Event of EventInfo

    /// Represents the resolution of a name to a property
    | Property of string * PropInfo list

    /// Represents the resolution of a name to a group of methods. 
    | MethodGroup of displayName: string * methods: MethInfo list * uninstantiatedMethodOpt: MethInfo option

    /// Represents the resolution of a name to a constructor
    | CtorGroup of string * MethInfo list

    /// Represents the resolution of a name to the fake constructor simulated for an interface type.
    | FakeInterfaceCtor of TType

    /// Represents the resolution of a name to a delegate
    | DelegateCtor of TType

    /// Represents the resolution of a name to a group of types
    | Types of string * TType list

    /// CustomOperation(nm, helpText, methInfo)
    /// 
    /// Used to indicate the availability or resolution of a custom query operation such as 'sortBy' or 'where' in computation expression syntax
    | CustomOperation of string * (unit -> string option) * MethInfo option

    /// Represents the resolution of a name to a custom builder in the F# computation expression syntax
    | CustomBuilder of string * ValRef

    /// Represents the resolution of a name to a type variable
    | TypeVar of string * Typar

    /// Represents the resolution of a name to a module or namespace
    | ModuleOrNamespaces of Tast.ModuleOrNamespaceRef list

    /// Represents the resolution of a name to an operator
    | ImplicitOp of Ident * TraitConstraintSln option ref

    /// Represents the resolution of a name to a named argument
    | ArgName of Ident * TType * ArgumentContainer option

    /// Represents the resolution of a name to a named property setter
    | SetterArg of Ident * Item 

    /// Represents the potential resolution of an unqualified name to a type.
    | UnqualifiedType of TyconRef list

    static member MakeMethGroup (nm,minfos:MethInfo list) = 
        let minfos = minfos |> List.sortBy (fun minfo -> minfo.NumArgs |> List.sum)
        Item.MethodGroup (nm,minfos,None)

    static member MakeCtorGroup (nm,minfos:MethInfo list) = 
        let minfos = minfos |> List.sortBy (fun minfo -> minfo.NumArgs |> List.sum)
        Item.CtorGroup (nm,minfos)

    member d.DisplayName =
        match d with
        | Item.Value v -> v.DisplayName
        | Item.ActivePatternCase apref -> apref.Name
        | Item.UnionCase(uinfo,_) -> DecompileOpName uinfo.UnionCase.DisplayName
        | Item.ExnCase tcref -> tcref.LogicalName
        | Item.RecdField rfinfo -> DecompileOpName rfinfo.RecdField.Name
        | Item.NewDef id -> id.idText
        | Item.ILField finfo -> finfo.FieldName
        | Item.Event einfo -> einfo.EventName
        | Item.Property(_, FSProp(_,_, Some v,_) :: _)
        | Item.Property(_, FSProp(_,_,_, Some v) :: _) -> v.DisplayName
        | Item.Property(nm, _) -> PrettyNaming.DemangleOperatorName nm
        | Item.MethodGroup(_, (FSMeth(_,_, v,_) :: _), _) -> v.DisplayName
        | Item.MethodGroup(nm, _, _) -> PrettyNaming.DemangleOperatorName nm
        | Item.CtorGroup(nm,_) -> DemangleGenericTypeName nm
        | Item.FakeInterfaceCtor (AbbrevOrAppTy tcref)
        | Item.DelegateCtor (AbbrevOrAppTy tcref) -> DemangleGenericTypeName tcref.DisplayName
        | Item.Types(nm,_) -> DemangleGenericTypeName nm
        | Item.UnqualifiedType(tcref :: _) -> tcref.DisplayName
        | Item.TypeVar (nm,_) -> nm
        | Item.ModuleOrNamespaces(modref :: _) -> modref.DemangledModuleOrNamespaceName
        | Item.ArgName (id, _, _)  -> id.idText
        | Item.SetterArg (id, _) -> id.idText
        | Item.CustomOperation (customOpName,_,_) -> customOpName
        | Item.CustomBuilder (nm,_) -> nm
        | _ ->  ""

let valRefHash (vref: ValRef) = 
    match vref.TryDeref with 
    | VNone -> 0 
    | VSome v -> LanguagePrimitives.PhysicalHash v

[<RequireQualifiedAccess>]
/// Pairs an Item with a TyparInst showing how generic type variables of the item are instantiated at 
/// a particular usage point.
type ItemWithInst = 
    { Item : Item
      TyparInst: TyparInst }

let ItemWithNoInst item = ({ Item = item; TyparInst = emptyTyparInst } : ItemWithInst)

let (|ItemWithInst|) (x:ItemWithInst) = (x.Item, x.TyparInst)

/// Represents a record field resolution and the information if the usage is deprecated.
type FieldResolution = FieldResolution of RecdFieldRef * bool

/// Information about an extension member held in the name resolution environment
type ExtensionMember = 

   /// F#-style Extrinsic extension member, defined in F# code
   | FSExtMem of ValRef * ExtensionMethodPriority

   /// ILExtMem(declaringTyconRef, ilMetadata, pri)
   ///
   /// IL-style extension member, backed by some kind of method with an [<Extension>] attribute
   | ILExtMem of TyconRef * MethInfo * ExtensionMethodPriority

   /// Check if two extension members refer to the same definition
   static member Equality g e1 e2 = 
       match e1, e2 with 
       | FSExtMem (vref1,_), FSExtMem (vref2,_) -> valRefEq g vref1 vref2
       | ILExtMem (_,md1,_), ILExtMem (_,md2,_) -> MethInfo.MethInfosUseIdenticalDefinitions md1 md2
       | _ -> false

   static member Hash e1 =
       match e1 with
       | FSExtMem(vref, _) -> valRefHash vref
       | ILExtMem(_, m, _) -> 
           match m with
           | ILMeth(_, ilmeth, _) -> LanguagePrimitives.PhysicalHash ilmeth.RawMetadata
           | FSMeth(_, _, vref, _) -> valRefHash vref
           | _ -> 0
   
   static member Comparer g = HashIdentity.FromFunctions ExtensionMember.Hash (ExtensionMember.Equality g)
   
   /// Describes the sequence order of the introduction of an extension method. Extension methods that are introduced
   /// later through 'open' get priority in overload resolution.
   member x.Priority = 
       match x with 
       | FSExtMem (_,pri) -> pri
       | ILExtMem (_,_,pri) -> pri
       
type FullyQualifiedFlag = 
    /// Only resolve full paths
    | FullyQualified 
    /// Resolve any paths accessible via 'open'
    | OpenQualified 



[<NoEquality; NoComparison>]
/// The environment of information used to resolve names
type NameResolutionEnv =
    { /// Display environment information for output 
      eDisplayEnv: DisplayEnv 

      /// Values and Data Tags available by unqualified name 
      eUnqualifiedItems: LayeredMap<string,Item>

      /// Data Tags and Active Pattern Tags available by unqualified name 
      ePatItems: NameMap<Item>

      /// Modules accessible via "." notation. Note this is a multi-map. 
      /// Adding a module abbreviation adds it a local entry to this List.map. 
      /// Likewise adding a ccu or opening a path adds entries to this List.map. 
      
      
      /// REVIEW (old comment)
      /// "The boolean flag is means the namespace or module entry shouldn't 'really' be in the 
      ///  map, and if it is ever used to resolve a name then we give a warning. 
      ///  This is used to give warnings on unqualified namespace accesses, e.g. 
      ///    open System 
      ///    open Collections                            <--- give a warning 
      ///    let v = new Collections.Generic.List<int>() <--- give a warning" 
      
      eModulesAndNamespaces:  NameMultiMap<Tast.ModuleOrNamespaceRef>
      
      /// Fully qualified modules and namespaces. 'open' does not change this. 
      eFullyQualifiedModulesAndNamespaces:  NameMultiMap<Tast.ModuleOrNamespaceRef>
      
      /// RecdField labels in scope.  RecdField labels are those where type are inferred 
      /// by label rather than by known type annotation. 
      /// Bools indicate if from a record, where no warning is given on indeterminate lookup 
      eFieldLabels: NameMultiMap<Tast.RecdFieldRef>

      /// Tycons indexed by the various names that may be used to access them, e.g. 
      ///     "List" --> multiple TyconRef's for the various tycons accessible by this name. 
      ///     "List`1" --> TyconRef 
      eTyconsByAccessNames: LayeredMultiMap<string,TyconRef>

      eFullyQualifiedTyconsByAccessNames: LayeredMultiMap<string,TyconRef>

      /// Tycons available by unqualified, demangled names (i.e. (List,1) --> TyconRef) 
      eTyconsByDemangledNameAndArity: LayeredMap<NameArityPair,TyconRef>

      /// Tycons available by unqualified, demangled names (i.e. (List,1) --> TyconRef) 
      eFullyQualifiedTyconsByDemangledNameAndArity: LayeredMap<NameArityPair,TyconRef>

      /// Extension members by type and name 
      eIndexedExtensionMembers: TyconRefMultiMap<ExtensionMember>

      /// Other extension members unindexed by type
      eUnindexedExtensionMembers: ExtensionMember list

      /// Typars (always available by unqualified names). Further typars can be 
      /// in the tpenv, a structure folded through each top-level definition. 
      eTypars: NameMap<Typar>

    } 

    /// The initial, empty name resolution environment. The mother of all things.
    static member Empty g =
        { eDisplayEnv = DisplayEnv.Empty g
          eModulesAndNamespaces = Map.empty
          eFullyQualifiedModulesAndNamespaces = Map.empty
          eFieldLabels = Map.empty
          eUnqualifiedItems = LayeredMap.Empty
          ePatItems = Map.empty
          eTyconsByAccessNames = LayeredMultiMap.Empty
          eTyconsByDemangledNameAndArity = LayeredMap.Empty
          eFullyQualifiedTyconsByAccessNames = LayeredMultiMap.Empty
          eFullyQualifiedTyconsByDemangledNameAndArity = LayeredMap.Empty
          eIndexedExtensionMembers = TyconRefMultiMap<_>.Empty
          eUnindexedExtensionMembers = []
          eTypars = Map.empty }

    member nenv.DisplayEnv = nenv.eDisplayEnv

    member nenv.FindUnqualifiedItem nm = nenv.eUnqualifiedItems.[nm]

    /// Get the table of types, indexed by name and arity
    member nenv.TyconsByDemangledNameAndArity fq = 
        match fq with 
        | FullyQualified -> nenv.eFullyQualifiedTyconsByDemangledNameAndArity
        | OpenQualified  -> nenv.eTyconsByDemangledNameAndArity

    /// Get the table of types, indexed by name 
    member nenv.TyconsByAccessNames fq = 
        match fq with 
        | FullyQualified -> nenv.eFullyQualifiedTyconsByAccessNames
        | OpenQualified  -> nenv.eTyconsByAccessNames

    /// Get the table of modules and namespaces
    member nenv.ModulesAndNamespaces fq = 
        match fq with 
        | FullyQualified -> nenv.eFullyQualifiedModulesAndNamespaces 
        | OpenQualified -> nenv.eModulesAndNamespaces 

//-------------------------------------------------------------------------
// Helpers to do with extension members
//------------------------------------------------------------------------- 

/// Allocate the next extension method priority. This is an incrementing sequence of integers 
/// during type checking.
let NextExtensionMethodPriority() = uint64 (newStamp())

/// Get the info for all the .NET-style extension members listed as static members in the type.
let private GetCSharpStyleIndexedExtensionMembersForTyconRef (amap:Import.ImportMap) m  (tcrefOfStaticClass:TyconRef) = 
    let g = amap.g
    // Type must be non-generic and have 'Extension' attribute
    if isNil(tcrefOfStaticClass.Typars(m)) && TyconRefHasAttribute g m g.attrib_ExtensionAttribute tcrefOfStaticClass then
        let pri = NextExtensionMethodPriority()
        let typ = generalizedTyconRef tcrefOfStaticClass
        
        // Get the 'plain' methods, not interpreted as extension methods
        let minfos = GetImmediateIntrinsicMethInfosOfType (None, AccessorDomain.AccessibleFromSomeFSharpCode) g amap m typ
        [ for minfo in minfos do
            // Method must be static, have 'Extension' attribute, must not be curried, must have at least one argument
            if not minfo.IsInstance && 
               not minfo.IsExtensionMember && 
               minfo.NumArgs.Length = 1 && 
               minfo.NumArgs.Head >= 1 && 
               MethInfoHasAttribute g m g.attrib_ExtensionAttribute minfo
            then
                let ilExtMem = ILExtMem (tcrefOfStaticClass, minfo, pri)

                // The results are indexed by the TyconRef of the first 'this' argument, if any.
                // So we need to go and crack the type of the 'this' argument.
                //
                // This is convoluted because we only need the ILTypeRef of the first argument, and we don't
                // want to read any other metadata as it can trigger missing-assembly errors. It turns out ImportILTypeRef
                // is less eager in reading metadata than GetParamTypes.
                //
                // We don't use the index for the IL extension method for tuple of F# function types (e.g. if extension
                // methods for tuple occur in C# code)
                let thisTyconRef = 
                 try 
                    let rs = 
                        match metadataOfTycon tcrefOfStaticClass.Deref, minfo with 
                        | ILTypeMetadata (TILObjectReprData(scoref,_,_)), ILMeth(_,ILMethInfo(_,_,_,ilMethod,_),_) ->
                            match ilMethod.ParameterTypes with 
                            | firstTy :: _ -> 
                                match firstTy with 
                                | ILType.Boxed  tspec | ILType.Value tspec -> 
                                    let tref = (tspec |> rescopeILTypeSpec scoref).TypeRef
                                    if Import.CanImportILTypeRef amap m tref then
                                        let tcref = tref |> Import.ImportILTypeRef amap m
                                        if isCompiledTupleTyconRef g tcref || tyconRefEq g tcref g.fastFunc_tcr then None
                                        else Some tcref
                                    else None
                                | _ -> None
                            | _ -> None
                        | _ -> 
                            // The results are indexed by the TyconRef of the first 'this' argument, if any.
                            // So we need to go and crack the type of the 'this' argument.
                            let thisTy = minfo.GetParamTypes(amap,m,generalizeTypars minfo.FormalMethodTypars).Head.Head
                            match thisTy with
                            | AppTy amap.g (tcrefOfTypeExtended, _) -> Some tcrefOfTypeExtended
                            | _ -> None
                     
                    Some rs

                  with e -> // Import of the ILType may fail, if so report the error and skip on
                    errorRecovery e m
                    None

                match thisTyconRef with
                | None -> ()
                | Some (Some tcref) -> yield Choice1Of2(tcref, ilExtMem)
                | Some None -> yield Choice2Of2 ilExtMem ]
    else
        []       


//-------------------------------------------------------------------------
// Helpers to do with building environments
//------------------------------------------------------------------------- 

/// For the operations that build the overall name resolution 
/// tables, BulkAdd.Yes is set to true when "opening" a 
/// namespace. If BulkAdd is true then add-and-collapse 
/// is used for the backing maps.Multiple "open" operations are 
/// thus coalesced, and the first subsequent lookup after a sequence 
/// of opens will collapse the maps and build the backing dictionary. 
[<RequireQualifiedAccess>]
type BulkAdd = Yes | No


/// bulkAddMode: true when adding the values from the 'open' of a namespace
/// or module, when we collapse the value table down to a dictionary.
let AddValRefsToItems (bulkAddMode: BulkAdd) (eUnqualifiedItems: LayeredMap<_,_>) (vrefs:ValRef[]) =
    // Object model members are not added to the unqualified name resolution environment 
    let vrefs = vrefs |> Array.filter (fun vref -> vref.MemberInfo.IsNone)

    if vrefs.Length = 0 then eUnqualifiedItems else

    match bulkAddMode with 
    | BulkAdd.Yes -> 
        eUnqualifiedItems.AddAndMarkAsCollapsible(vrefs |> Array.map (fun vref -> KeyValuePair(vref.LogicalName, Item.Value vref)))
    | BulkAdd.No -> 
        assert (vrefs.Length = 1)
        let vref = vrefs.[0]
        eUnqualifiedItems.Add (vref.LogicalName, Item.Value vref)  

/// Add an F# value to the table of available extension members, if necessary, as an FSharp-style extension member
let AddValRefToExtensionMembers pri (eIndexedExtensionMembers: TyconRefMultiMap<_>) (vref:ValRef) =
    if vref.IsMember && vref.IsExtensionMember then
        eIndexedExtensionMembers.Add (vref.MemberApparentParent, FSExtMem (vref,pri)) 
    else
        eIndexedExtensionMembers


/// This entrypoint is used to add some extra items to the environment for Visual Studio, e.g. static members 
let AddFakeNamedValRefToNameEnv nm nenv vref =
    {nenv with eUnqualifiedItems = nenv.eUnqualifiedItems.Add (nm, Item.Value vref) }

/// This entrypoint is used to add some extra items to the environment for Visual Studio, e.g. record members
let AddFakeNameToNameEnv nm nenv item =
    {nenv with eUnqualifiedItems = nenv.eUnqualifiedItems.Add (nm, item) }

/// Add an F# value to the table of available active patterns
let AddValRefsToActivePatternsNameEnv ePatItems (vref:ValRef) =
    let ePatItems = 
        (ActivePatternElemsOfValRef vref, ePatItems) 
        ||> List.foldBack (fun apref tab -> 
            NameMap.add apref.Name (Item.ActivePatternCase apref) tab)

    // Add literal constants to the environment available for resolving items in patterns 
    let ePatItems = 
        match vref.LiteralValue with 
        | None -> ePatItems 
        | Some _ -> NameMap.add vref.LogicalName (Item.Value vref) ePatItems

    ePatItems

/// Add a set of F# values to the environment.
let AddValRefsToNameEnvWithPriority bulkAddMode pri nenv (vrefs: ValRef []) =
    if vrefs.Length = 0 then nenv else
    { nenv with 
        eUnqualifiedItems = AddValRefsToItems bulkAddMode nenv.eUnqualifiedItems vrefs
        eIndexedExtensionMembers = (nenv.eIndexedExtensionMembers,vrefs) ||> Array.fold (AddValRefToExtensionMembers pri)
        ePatItems = (nenv.ePatItems,vrefs) ||> Array.fold AddValRefsToActivePatternsNameEnv }

/// Add a single F# value to the environment.
let AddValRefToNameEnv nenv (vref:ValRef) = 
    let pri = NextExtensionMethodPriority()
    { nenv with 
        eUnqualifiedItems = 
            if vref.MemberInfo.IsNone then 
                nenv.eUnqualifiedItems.Add (vref.LogicalName, Item.Value vref) 
            else
                nenv.eUnqualifiedItems
        eIndexedExtensionMembers = AddValRefToExtensionMembers pri nenv.eIndexedExtensionMembers vref
        ePatItems = AddValRefsToActivePatternsNameEnv nenv.ePatItems vref }


/// Add a set of active pattern result tags to the environment.
let AddActivePatternResultTagsToNameEnv (apinfo: PrettyNaming.ActivePatternInfo) nenv ty m =
    if apinfo.Names.Length = 0 then nenv else
    let apresl = List.indexed apinfo.Names
    { nenv with
        eUnqualifiedItems = 
            (apresl,nenv.eUnqualifiedItems) 
            ||> List.foldBack (fun (j,nm) acc -> acc.Add(nm, Item.ActivePatternResult(apinfo,ty,j,m))) } 

/// Generalize a union case, from Cons --> List<T>.Cons
let GeneralizeUnionCaseRef (ucref:UnionCaseRef) = 
    UnionCaseInfo (fst (generalizeTyconRef ucref.TyconRef), ucref)
    
    
/// Add type definitions to the sub-table of the environment indexed by name and arity
let AddTyconsByDemangledNameAndArity (bulkAddMode: BulkAdd) (tcrefs: TyconRef[]) (tab: LayeredMap<NameArityPair,TyconRef>) = 
    if tcrefs.Length = 0 then tab else
    let entries = 
        tcrefs 
        |> Array.map (fun tcref -> KeyTyconByDemangledNameAndArity tcref.LogicalName tcref.TyparsNoRange tcref)

    match bulkAddMode with
    | BulkAdd.Yes -> tab.AddAndMarkAsCollapsible entries
    | BulkAdd.No -> (tab,entries) ||> Array.fold (fun tab (KeyValue(k,v)) -> tab.Add(k,v))

/// Add type definitions to the sub-table of the environment indexed by access name 
let AddTyconByAccessNames bulkAddMode (tcrefs:TyconRef[]) (tab: LayeredMultiMap<string,_>) =
    if tcrefs.Length = 0 then tab else
    let entries = 
        tcrefs
        |> Array.collect (fun tcref -> KeyTyconByAccessNames tcref.LogicalName tcref)

    match bulkAddMode with
    | BulkAdd.Yes -> tab.AddAndMarkAsCollapsible entries
    | BulkAdd.No -> (tab,entries) ||> Array.fold (fun tab (KeyValue(k,v)) -> tab.Add (k,v))

/// Add a record field to the corresponding sub-table of the name resolution environment 
let AddRecdField (rfref:RecdFieldRef) tab = NameMultiMap.add rfref.FieldName rfref tab

/// Add a set of union cases to the corresponding sub-table of the environment 
let AddUnionCases1 (tab:Map<_,_>) (ucrefs:UnionCaseRef list) = 
    (tab, ucrefs) ||> List.fold (fun acc ucref -> 
        let item = Item.UnionCase(GeneralizeUnionCaseRef ucref,false)
        acc.Add (ucref.CaseName, item))

/// Add a set of union cases to the corresponding sub-table of the environment 
let AddUnionCases2 bulkAddMode (eUnqualifiedItems: LayeredMap<_,_>) (ucrefs :UnionCaseRef list) = 
    match bulkAddMode with 
    | BulkAdd.Yes -> 
        let items = 
            ucrefs |> Array.ofList |> Array.map (fun ucref -> 
                let item = Item.UnionCase(GeneralizeUnionCaseRef ucref,false)
                KeyValuePair(ucref.CaseName,item))
        eUnqualifiedItems.AddAndMarkAsCollapsible items

    | BulkAdd.No -> 
        (eUnqualifiedItems,ucrefs) ||> List.fold (fun acc ucref -> 
            let item = Item.UnionCase(GeneralizeUnionCaseRef ucref,false)
            acc.Add (ucref.CaseName, item))

/// Add any implied contents of a type definition to the environment.
let private AddPartsOfTyconRefToNameEnv bulkAddMode ownDefinition (g:TcGlobals) amap m  nenv (tcref:TyconRef) = 

    let isIL = tcref.IsILTycon
    let ucrefs = if isIL then [] else tcref.UnionCasesAsList |> List.map tcref.MakeNestedUnionCaseRef 
    let flds =  if isIL then [| |] else tcref.AllFieldsArray

    let eIndexedExtensionMembers, eUnindexedExtensionMembers = 
        let ilStyleExtensionMeths = GetCSharpStyleIndexedExtensionMembersForTyconRef amap m  tcref 
        ((nenv.eIndexedExtensionMembers,nenv.eUnindexedExtensionMembers),ilStyleExtensionMeths) ||> List.fold (fun (tab1,tab2) extMemInfo -> 
            match extMemInfo with 
            | Choice1Of2 (tcref,extMemInfo) -> tab1.Add (tcref, extMemInfo), tab2
            | Choice2Of2 extMemInfo -> tab1, extMemInfo :: tab2)  

    let isILOrRequiredQualifiedAccess = isIL || (not ownDefinition && HasFSharpAttribute g g.attrib_RequireQualifiedAccessAttribute tcref.Attribs)
    let eFieldLabels = 
        if isILOrRequiredQualifiedAccess || not tcref.IsRecordTycon || flds.Length = 0 then 
            nenv.eFieldLabels 
        else 
            (nenv.eFieldLabels,flds) ||> Array.fold (fun acc f -> 
                   if f.IsStatic || f.IsCompilerGenerated then acc 
                   else AddRecdField (tcref.MakeNestedRecdFieldRef f) acc)
    
    let eUnqualifiedItems = 
        let tab = nenv.eUnqualifiedItems
        // add the type name for potential use as a constructor
        // The rules are
        // - The unqualified lookup table in the environment can contain map names to a set of type names (the set of type names is a new kind of "item"). 
        // - When the contents of a type definition is added to the environment, an entry is added in this table for all class and struct types.
        // - When opening a module, types are added first to the environment, then values, then auto-opened sub-modules. 
        // - When a value is added by an "open" previously available type names will become inaccessible by this table. 
        let tab = 
            // This may explore into an unreferenced assembly if the name
            // is a type abbreviation. If it does, assume the name does not
            // have a constructor.
            let mayHaveConstruction = 
                protectAssemblyExploration 
                    false 
                    (fun () -> 
                        let typ = generalizedTyconRef tcref
                        isClassTy g typ || isStructTy g typ)

            if mayHaveConstruction then 
                tab.LinearTryModifyThenLaterFlatten (tcref.DisplayName, (fun prev ->
                    match prev with 
                    | Some (Item.UnqualifiedType tcrefs) -> Item.UnqualifiedType (tcref::tcrefs)
                    | _ -> Item.UnqualifiedType [tcref]))
            else
                tab
        if isILOrRequiredQualifiedAccess || ucrefs.Length = 0 then 
            tab 
        else 
            AddUnionCases2 bulkAddMode tab ucrefs

    let ePatItems = 
        if isILOrRequiredQualifiedAccess || ucrefs.Length = 0 then 
            nenv.ePatItems 
        else 
            AddUnionCases1 nenv.ePatItems ucrefs

    { nenv with 
        eFieldLabels = eFieldLabels
        eUnqualifiedItems = eUnqualifiedItems
        ePatItems = ePatItems
        eIndexedExtensionMembers = eIndexedExtensionMembers 
        eUnindexedExtensionMembers = eUnindexedExtensionMembers }

let TryFindPatternByName name {ePatItems = patternMap} =
    NameMap.tryFind name patternMap

/// Add a set of type definitions to the name resolution environment 
let AddTyconRefsToNameEnv bulkAddMode ownDefinition g amap m root nenv tcrefs =
    if isNil tcrefs then nenv else
    let env = List.fold (AddPartsOfTyconRefToNameEnv bulkAddMode ownDefinition g amap m) nenv tcrefs
    // Add most of the contents of the tycons en-masse, then flatten the tables if we're opening a module or namespace
    let tcrefs = Array.ofList tcrefs
    { env with
        eFullyQualifiedTyconsByDemangledNameAndArity = 
            if root then 
                AddTyconsByDemangledNameAndArity bulkAddMode tcrefs nenv.eFullyQualifiedTyconsByDemangledNameAndArity
            else 
                nenv.eFullyQualifiedTyconsByDemangledNameAndArity
        eFullyQualifiedTyconsByAccessNames = 
            if root then 
                AddTyconByAccessNames bulkAddMode tcrefs nenv.eFullyQualifiedTyconsByAccessNames
            else
                nenv.eFullyQualifiedTyconsByAccessNames
        eTyconsByDemangledNameAndArity = 
            AddTyconsByDemangledNameAndArity bulkAddMode tcrefs nenv.eTyconsByDemangledNameAndArity 
        eTyconsByAccessNames = 
            AddTyconByAccessNames bulkAddMode tcrefs nenv.eTyconsByAccessNames } 

/// Add an F# exception definition to the name resolution environment 
let AddExceptionDeclsToNameEnv bulkAddMode nenv (ecref:TyconRef) = 
    assert ecref.IsExceptionDecl
    let item = Item.ExnCase ecref
    {nenv with 
       eUnqualifiedItems =
            match bulkAddMode with 
            | BulkAdd.Yes -> 
                nenv.eUnqualifiedItems.AddAndMarkAsCollapsible [| KeyValuePair(ecref.LogicalName, item) |]
            | BulkAdd.No -> 
                nenv.eUnqualifiedItems.Add (ecref.LogicalName, item)
                
       ePatItems = nenv.ePatItems.Add (ecref.LogicalName, item) }

/// Add a module abbreviation to the name resolution environment 
let AddModuleAbbrevToNameEnv (id:Ident) nenv modrefs =
    {nenv with
       eModulesAndNamespaces =
         let add old nw = nw @ old
         NameMap.layerAdditive add (Map.add id.idText modrefs Map.empty) nenv.eModulesAndNamespaces }


//-------------------------------------------------------------------------
// Open a structure or an IL namespace 
//------------------------------------------------------------------------- 

let MakeNestedModuleRefs (modref: ModuleOrNamespaceRef) = 
  modref.ModuleOrNamespaceType.ModuleAndNamespaceDefinitions  
     |> List.map modref.NestedTyconRef

/// Add a set of module or namespace to the name resolution environment, including any sub-modules marked 'AutoOpen'
//
// Recursive because of "AutoOpen", i.e. adding a module reference may automatically open further modules
let rec AddModuleOrNamespaceRefsToNameEnv g amap m root ad nenv (modrefs: ModuleOrNamespaceRef list) =
    if isNil modrefs then nenv else
    let modrefsMap = modrefs |> NameMap.ofKeyedList (fun modref -> modref.DemangledModuleOrNamespaceName)
    let addModrefs tab = 
         let add old nw = 
             if IsEntityAccessible amap m ad nw then  
                 nw :: old
             else 
                 old
         NameMap.layerAdditive add modrefsMap tab
    let nenv = 
        {nenv with
           eModulesAndNamespaces = addModrefs nenv.eModulesAndNamespaces
           eFullyQualifiedModulesAndNamespaces =
              if root then 
                  addModrefs nenv.eFullyQualifiedModulesAndNamespaces
              else 
                  nenv.eFullyQualifiedModulesAndNamespaces } 
    let nenv = 
        (nenv,modrefs) ||> List.fold (fun nenv modref ->  
            if modref.IsModule && TryFindFSharpBoolAttribute g g.attrib_AutoOpenAttribute modref.Attribs = Some true then
                AddModuleOrNamespaceContentsToNameEnv g amap ad m false nenv modref 
            else
                nenv)
    nenv

/// Add the contents of a module or namespace to the name resolution environment
and AddModuleOrNamespaceContentsToNameEnv (g:TcGlobals) amap (ad:AccessorDomain) m root nenv (modref:ModuleOrNamespaceRef) = 
    let pri = NextExtensionMethodPriority()
    let mty = modref.ModuleOrNamespaceType
     
    let nenv =
        let mutable state = { nenv with eDisplayEnv = nenv.eDisplayEnv.AddOpenModuleOrNamespace modref }
     
        for exnc in mty.ExceptionDefinitions do
           let tcref = modref.NestedTyconRef exnc
           if IsEntityAccessible amap m ad tcref then 
               state <- AddExceptionDeclsToNameEnv BulkAdd.Yes state tcref

        state

    let tcrefs = 
       mty.TypeAndExceptionDefinitions 
       |> List.choose (fun tycon -> 
           let tcref = modref.NestedTyconRef tycon
           if IsEntityAccessible amap m ad tcref then Some(tcref) else None)

    let nenv = (nenv,tcrefs) ||> AddTyconRefsToNameEnv BulkAdd.Yes false g amap m false 
    let vrefs = 
        mty.AllValsAndMembers.ToList() 
        |> List.choose (fun x -> if IsAccessible ad x.Accessibility then TryMkValRefInModRef modref x else None)
        |> List.toArray
    let nenv = AddValRefsToNameEnvWithPriority BulkAdd.Yes pri nenv vrefs
    let nestedModules = MakeNestedModuleRefs modref
    let nenv = (nenv,nestedModules) ||> AddModuleOrNamespaceRefsToNameEnv g amap m root ad 
    nenv

/// Add a set of modules or namespaces to the name resolution environment
//
// Note this is a 'foldBack' - the most recently added modules come first in the list, e.g.
//    module M1 = ... // M1a
//    module M1 = ... // M1b
//    open M1
// 
// The list contains [M1b; M1a]
and AddModulesAndNamespacesContentsToNameEnv g amap ad m root nenv modrefs =
   (modrefs, nenv) ||> List.foldBack (fun modref acc -> AddModuleOrNamespaceContentsToNameEnv g amap ad m root acc modref)

/// Add a single modules or namespace to the name resolution environment
let AddModuleOrNamespaceRefToNameEnv g amap m root ad nenv (modref:EntityRef) =  
    AddModuleOrNamespaceRefsToNameEnv g amap m root ad nenv [modref] 

  
/// A flag which indicates if it is an error to have two declared type parameters with identical names
/// in the name resolution environment.
type CheckForDuplicateTyparFlag = 
    | CheckForDuplicateTypars 
    | NoCheckForDuplicateTypars

/// Add some declared type parameters to the name resolution environment
let AddDeclaredTyparsToNameEnv check nenv typars = 
    let typarmap = 
      List.foldBack 
        (fun (tp:Typar) sofar -> 
          match check with
          | CheckForDuplicateTypars -> 
              if Map.containsKey tp.Name sofar then 
                errorR (Duplicate("type parameter",tp.DisplayName,tp.Range))
          | NoCheckForDuplicateTypars -> ()

          Map.add tp.Name tp sofar) typars Map.empty 
    {nenv with eTypars = NameMap.layer typarmap nenv.eTypars }


//-------------------------------------------------------------------------
// Generating fresh instantiations for type inference.
//------------------------------------------------------------------------- 

/// Convert a reference to a named type into a type that includes
/// a fresh set of inference type variables for the type parameters of the union type.
let FreshenTycon (ncenv: NameResolver) m (tcref:TyconRef) = 
    let tinst = ncenv.InstantiationGenerator m (tcref.Typars m)
    TType_app(tcref,tinst)

/// Convert a reference to a union case into a UnionCaseInfo that includes
/// a fresh set of inference type variables for the type parameters of the union type.
let FreshenUnionCaseRef (ncenv: NameResolver) m (ucref:UnionCaseRef) = 
    let tinst = ncenv.InstantiationGenerator m (ucref.TyconRef.Typars m)
    UnionCaseInfo(tinst,ucref)

/// This must be called after fetching unqualified items that may need to be freshened
let FreshenUnqualifiedItem (ncenv: NameResolver) m res = 
    match res with 
    | Item.UnionCase(UnionCaseInfo(_,ucref),_) -> Item.UnionCase(FreshenUnionCaseRef ncenv m ucref,false)
    | _ -> res


//-------------------------------------------------------------------------
// Resolve module paths, value, field etc. lookups.  Doing this involves
// searching through many possibilities and disambiguating.  Hence first
// define some ways of combining multiple results and for carrying
// error information.  Errors are generally undefined names and are
// reported by returning the error that occurs at greatest depth in the
// sequence of Identifiers. 
//------------------------------------------------------------------------- 

// Accumulate a set of possible results. 
// If neither operations succeed, return an approximate error. 
// If one succeeds, return that one. 
// Prefer the error associated with the first argument. 
let OneResult res = 
    match res with 
    | Result x -> Result [x]
    | Exception e -> Exception e

let OneSuccess x = Result [x]

let AddResults res1 res2 =
    match res1, res2 with 
    | Result [],_ -> res2
    | _,Result [] -> res1
    | Result x,Result l -> Result (x @ l)
    | Exception _,Result l -> Result l
    | Result x,Exception _ -> Result x
    // If we have error messages for the same symbol, then we can merge suggestions.
    | Exception (UndefinedName(n1,f,id1,suggestions1)),Exception (UndefinedName(n2,_,id2,suggestions2)) when n1 = n2 && id1.idText = id2.idText && id1.idRange = id2.idRange ->
        Exception(UndefinedName(n1,f,id1,fun () -> Set.union (suggestions1()) (suggestions2())))
    // This prefers error messages coming from deeper failing long identifier paths 
    | Exception (UndefinedName(n1,_,_,_) as e1),Exception (UndefinedName(n2,_,_,_) as e2) ->
        if n1 < n2 then Exception e2 else Exception e1
    // Prefer more concrete errors about things being undefined 
    | Exception (UndefinedName _ as e1),Exception (Error _) -> Exception e1
    | Exception (Error _),Exception (UndefinedName _ as e2) -> Exception e2
    | Exception e1,Exception _ -> Exception e1

let (+++) x y = AddResults x y
let NoResultsOrUsefulErrors = Result []

/// Indicates if we only need one result or all possible results from a resolution.
[<RequireQualifiedAccess>]
type ResultCollectionSettings =
| AllResults
| AtMostOneResult

let rec CollectResults f = function
    | [] -> NoResultsOrUsefulErrors
    | [h] -> OneResult (f h)
    | h :: t -> AddResults (OneResult (f h)) (CollectResults f t)

let rec CollectAtMostOneResult f = function
    | [] -> NoResultsOrUsefulErrors
    | [h] -> OneResult (f h)
    | h :: t ->
        match f h with
        | Result r -> Result [r]
        | Exception e -> AddResults (Exception e) (CollectAtMostOneResult f t)

let CollectResults2 resultCollectionSettings f = 
    match resultCollectionSettings with
    | ResultCollectionSettings.AtMostOneResult -> CollectAtMostOneResult f
    | _ -> CollectResults f

let MapResults f = function
    | Result xs -> Result (List.map f xs)
    | Exception err -> Exception err

let AtMostOneResult m res = 
    match res with 
    | Exception err -> raze err
    | Result [] -> raze (Error(FSComp.SR.nrInvalidModuleExprType(),m))
    | Result (res :: _) -> success res 

//-------------------------------------------------------------------------
// TypeNameResolutionInfo
//------------------------------------------------------------------------- 

/// Indicates whether we are resolving type names to type definitions or to constructor methods.
type TypeNameResolutionFlag = 
    | ResolveTypeNamesToCtors 
    | ResolveTypeNamesToTypeRefs

[<RequireQualifiedAccess>]
[<NoEquality; NoComparison>]
/// Represents information about the generic argument count of a type name when resolving it. 
///
/// In some situations we resolve "List" to any type definition with that name regardless of the number
/// of generic arguments. In others, we know precisely how many generic arguments are needed.
type TypeNameResolutionStaticArgsInfo = 
    /// Indicates indefinite knowledge of type arguments
    | Indefinite 
    /// Indicates definite knowledge of type arguments
    | Definite of int

    /// Indicates definite knowledge of empty type arguments
    static member DefiniteEmpty = TypeNameResolutionStaticArgsInfo.Definite 0

    static member FromTyArgs (numTyArgs: int) = TypeNameResolutionStaticArgsInfo.Definite numTyArgs

    member x.HasNoStaticArgsInfo = match x with TypeNameResolutionStaticArgsInfo.Indefinite -> true | _-> false

    member x.NumStaticArgs = match x with TypeNameResolutionStaticArgsInfo.Indefinite -> 0 | TypeNameResolutionStaticArgsInfo.Definite n -> n

    // Get the first possible mangled name of the type, assuming the args are generic args
    member x.MangledNameForType nm = 
        if IsMangledGenericName nm || x.NumStaticArgs = 0 then nm
        else nm+"`"+string x.NumStaticArgs



[<NoEquality; NoComparison>]
/// Represents information which guides name resolution of types.
type TypeNameResolutionInfo = 
    | TypeNameResolutionInfo of TypeNameResolutionFlag * TypeNameResolutionStaticArgsInfo

    static member Default = TypeNameResolutionInfo (ResolveTypeNamesToCtors,TypeNameResolutionStaticArgsInfo.Indefinite) 
    static member ResolveToTypeRefs statResInfo = TypeNameResolutionInfo (ResolveTypeNamesToTypeRefs,statResInfo) 
    member x.StaticArgsInfo = match x with TypeNameResolutionInfo(_,staticResInfo) -> staticResInfo 
    member x.ResolutionFlag = match x with TypeNameResolutionInfo(flag,_) -> flag
    member x.DropStaticArgsInfo = match x with TypeNameResolutionInfo(flag2,_) -> TypeNameResolutionInfo(flag2,TypeNameResolutionStaticArgsInfo.Indefinite)


//-------------------------------------------------------------------------
// Resolve (possibly mangled) type names 
//------------------------------------------------------------------------- 
 
/// Qualified lookups of type names where the number of generic arguments is known 
/// from context, e.g. Module.Type<args>.  The full names suh as ``List`1`` can 
/// be used to qualify access if needed 
let LookupTypeNameInEntityHaveArity nm (staticResInfo: TypeNameResolutionStaticArgsInfo) (mty:ModuleOrNamespaceType) = 
    let attempt1 = mty.TypesByMangledName.TryFind (staticResInfo.MangledNameForType nm)
    match attempt1 with 
    | Some _ as r ->  r
    | None -> mty.TypesByMangledName.TryFind nm

/// Unqualified lookups of type names where the number of generic arguments is known 
/// from context, e.g. List<arg>.  Rebindings due to 'open' may have rebound identifiers.
let LookupTypeNameInEnvHaveArity fq nm numTyArgs (nenv:NameResolutionEnv) = 
    let key = if IsMangledGenericName nm then DecodeGenericTypeName nm else NameArityPair(nm,numTyArgs)
    match nenv.TyconsByDemangledNameAndArity(fq).TryFind(key)  with
    | Some res -> Some res
    | None -> nenv.TyconsByAccessNames(fq).TryFind nm |> Option.map List.head

/// Implements unqualified lookups of type names where the number of generic arguments is NOT known 
/// from context. 
//
// This is used in five places: 
//     -  static member lookups, e.g. MyType.StaticMember(3) 
//     -                         e.g. MyModule.MyType.StaticMember(3) 
//     -  type-qualified field names, e.g. { RecordType.field = 3 } 
//     -  type-qualified constructor names, e.g. match x with UnionType.A -> 3 
//     -  identifiers to constructors for better error messages, e.g. 'String(3)' after 'open System' 
//     -  the special single-constructor rule in TcTyconCores
// 
// Because of the potential ambiguity multiple results can be returned. 
// Explicit type annotations can be added where needed to specify the generic arity. 
//  
// In theory the full names such as ``RecordType`1`` can 
// also be used to qualify access if needed, though this is almost never needed.  

let LookupTypeNameNoArity nm (byDemangledNameAndArity: LayeredMap<NameArityPair,_>) (byAccessNames: LayeredMultiMap<string,_>) = 
    if IsMangledGenericName nm then 
      match byDemangledNameAndArity.TryFind (DecodeGenericTypeName nm) with 
      | Some res -> [res]
      | None -> 
          match byAccessNames.TryFind nm with
          | Some res -> res
          | None -> []
    else 
      byAccessNames.[nm]

/// Qualified lookup of type names in the environment
let LookupTypeNameInEnvNoArity fq nm (nenv: NameResolutionEnv) = 
    LookupTypeNameNoArity nm (nenv.TyconsByDemangledNameAndArity fq) (nenv.TyconsByAccessNames fq)

/// Qualified lookup of type names in an entity
let LookupTypeNameInEntityNoArity m nm (mtyp:ModuleOrNamespaceType) = 
    LookupTypeNameNoArity nm (mtyp.TypesByDemangledNameAndArity m) mtyp.TypesByAccessNames 

/// Qualified lookup of type names in an entity where we may know a generic argument count
let LookupTypeNameInEnvMaybeHaveArity fq nm (typeNameResInfo: TypeNameResolutionInfo) nenv = 
    if typeNameResInfo.StaticArgsInfo.HasNoStaticArgsInfo then 
        LookupTypeNameInEnvNoArity fq nm nenv
    else 
        LookupTypeNameInEnvHaveArity fq nm typeNameResInfo.StaticArgsInfo.NumStaticArgs nenv |> Option.toList

/// A flag which indicates if direct references to generated provided types are allowed. Normally these
/// are disallowed.
[<RequireQualifiedAccess>]
type PermitDirectReferenceToGeneratedType = 
    | Yes 
    | No
    

#if EXTENSIONTYPING

/// Check for direct references to generated provided types.
let CheckForDirectReferenceToGeneratedType (tcref: TyconRef, genOk, m) =
  match genOk with 
  | PermitDirectReferenceToGeneratedType.Yes -> ()
  | PermitDirectReferenceToGeneratedType.No -> 
    match tcref.TypeReprInfo with 
    | TProvidedTypeExtensionPoint info when not info.IsErased -> 
         //printfn "checking direct reference to generated type '%s'" tcref.DisplayName
        if ExtensionTyping.IsGeneratedTypeDirectReference (info.ProvidedType, m) then 
            error (Error(FSComp.SR.etDirectReferenceToGeneratedTypeNotAllowed(tcref.DisplayName),m))
    |  _ -> ()


/// This adds a new entity for a lazily discovered provided type into the TAST structure.
let AddEntityForProvidedType (amap: Import.ImportMap, modref: ModuleOrNamespaceRef, resolutionEnvironment, st:Tainted<ProvidedType>, m) = 
    let importProvidedType t = Import.ImportProvidedType amap m t
    let isSuppressRelocate = amap.g.isInteractive || st.PUntaint((fun st -> st.IsSuppressRelocate),m) 
    let tycon = Construct.NewProvidedTycon(resolutionEnvironment, st, importProvidedType, isSuppressRelocate, m)
    modref.ModuleOrNamespaceType.AddProvidedTypeEntity(tycon)
    let tcref = modref.NestedTyconRef tycon
    System.Diagnostics.Debug.Assert modref.TryDeref.IsSome
    tcref


/// Given a provided type or provided namespace, resolve the type name using the type provider API.
/// If necessary, incorporate the provided type or namespace into the entity.
let ResolveProvidedTypeNameInEntity (amap, m, typeName, modref: ModuleOrNamespaceRef) = 
    match modref.TypeReprInfo with
    | TProvidedNamespaceExtensionPoint(resolutionEnvironment,resolvers) ->
        match modref.Deref.PublicPath with
        | Some(PubPath path) ->
            let matches = resolvers |> List.map (fun r-> ExtensionTyping.TryResolveProvidedType(r,m,path,typeName)) 
            let tcrefs = 
                [ for st in matches do 
                      match st with 
                      | None -> ()
                      | Some st -> 
                          yield AddEntityForProvidedType (amap, modref, resolutionEnvironment, st, m) ]
            tcrefs
        | None -> []

    // We have a provided type, look up its nested types (populating them on-demand if necessary)
    | TProvidedTypeExtensionPoint info ->
        let sty = info.ProvidedType
        let resolutionEnvironment = info.ResolutionEnvironment
            
#if DEBUG
        if resolutionEnvironment.showResolutionMessages then
            dprintfn "resolving name '%s' in TProvidedTypeExtensionPoint '%s'" typeName (sty.PUntaint((fun sty -> sty.FullName), m))
#endif

        match sty.PApply((fun sty -> sty.GetNestedType(typeName)), m) with
        | Tainted.Null -> 
            //if staticResInfo.NumStaticArgs > 0 then 
            //    error(Error(FSComp.SR.etNestedProvidedTypesDoNotTakeStaticArgumentsOrGenericParameters(),m))
            []
        | nestedSty -> 
            [AddEntityForProvidedType (amap, modref, resolutionEnvironment, nestedSty, m) ]
    | _ -> []
#endif

/// Lookup a type name in an entity.
let LookupTypeNameInEntityMaybeHaveArity (amap, m, ad, nm, staticResInfo:TypeNameResolutionStaticArgsInfo, modref: ModuleOrNamespaceRef) = 
    let mtyp = modref.ModuleOrNamespaceType    
    let tcrefs = 
        match staticResInfo with 
        | TypeNameResolutionStaticArgsInfo.Indefinite -> 
            LookupTypeNameInEntityNoArity m nm mtyp
            |> List.map modref.NestedTyconRef 
        | TypeNameResolutionStaticArgsInfo.Definite _ -> 
            match LookupTypeNameInEntityHaveArity nm staticResInfo mtyp with
            | Some tycon -> [modref.NestedTyconRef tycon] 
            | None -> []
#if EXTENSIONTYPING
    let tcrefs =
        match tcrefs with 
        | [] -> ResolveProvidedTypeNameInEntity (amap, m, nm, modref)
        | _ -> tcrefs
#else
    amap |> ignore
#endif
    let tcrefs = tcrefs |> List.filter (IsEntityAccessible amap m ad)
    tcrefs


/// Make a type that refers to a nested type.
///
/// Handle the .NET/C# business where nested generic types implicitly accumulate the type parameters 
/// from their enclosing types.
let MakeNestedType (ncenv:NameResolver) (tinst:TType list) m (tcrefNested:TyconRef) = 
    let tps = List.drop tinst.Length (tcrefNested.Typars m)
    let tinstNested = ncenv.InstantiationGenerator m tps
    mkAppTy tcrefNested (tinst @ tinstNested)

/// Get all the accessible nested types of an existing type.
let GetNestedTypesOfType (ad, ncenv:NameResolver, optFilter, staticResInfo, checkForGenerated, m) typ =
    let g = ncenv.g
    ncenv.InfoReader.GetPrimaryTypeHierachy(AllowMultiIntfInstantiations.Yes,m,typ) |> List.collect (fun typ -> 
        match typ with 
        | AppTy g (tcref,tinst) ->
            let tycon = tcref.Deref
            let mty = tycon.ModuleOrNamespaceType
            // No dotting through type generators to get to a nested type!
#if EXTENSIONTYPING
            if checkForGenerated then 
                CheckForDirectReferenceToGeneratedType (tcref, PermitDirectReferenceToGeneratedType.No, m)
#else
            checkForGenerated |> ignore
#endif

            match optFilter with 
            | Some nm -> 
                let tcrefs = LookupTypeNameInEntityMaybeHaveArity (ncenv.amap, m, ad, nm, staticResInfo, tcref)
                tcrefs |> List.map (MakeNestedType ncenv tinst m) 
            | None -> 
#if EXTENSIONTYPING
                match tycon.TypeReprInfo with 
                | TProvidedTypeExtensionPoint info ->
                    [ for nestedType in info.ProvidedType.PApplyArray((fun sty -> sty.GetNestedTypes()), "GetNestedTypes", m) do 
                        let nestedTypeName = nestedType.PUntaint((fun t -> t.Name), m)
                        for nestedTcref in LookupTypeNameInEntityMaybeHaveArity (ncenv.amap, m, ad, nestedTypeName, staticResInfo, tcref)  do
                             yield  MakeNestedType ncenv tinst m nestedTcref ]
                
                | _ -> 
#endif
                    mty.TypesByAccessNames.Values
                    |> List.choose (fun entity -> 
                        let typ = tcref.NestedTyconRef entity |> MakeNestedType ncenv tinst m
                        if IsTypeAccessible g ncenv.amap m ad typ then Some typ else None)
        | _ -> [])

//-------------------------------------------------------------------------
// Report environments to visual studio. We stuff intermediary results 
// into a global variable. A little unpleasant. 
//------------------------------------------------------------------------- 

/// Represents the kind of the occurrence when reporting a name in name resolution
[<RequireQualifiedAccess>]
type ItemOccurence = 
    /// This is a binding / declaration of the item
    | Binding 
    /// This is a usage of the item 
    | Use 
    /// This is a usage of a type name in a type
    | UseInType 
    /// This is a usage of a type name in an attribute
    | UseInAttribute 
    /// Inside pattern matching
    | Pattern 
    /// Abstract slot gets implemented
    | Implemented
    /// Result gets suppressed over this text range
    | RelatedText
  
/// An abstract type for reporting the results of name resolution and type checking.
type ITypecheckResultsSink =
    abstract NotifyEnvWithScope : range * NameResolutionEnv * AccessorDomain -> unit
    abstract NotifyExprHasType : pos * TType * Tastops.DisplayEnv * NameResolutionEnv * AccessorDomain * range -> unit
    abstract NotifyNameResolution : pos * Item * Item * TyparInst * ItemOccurence * Tastops.DisplayEnv * NameResolutionEnv * AccessorDomain * range * bool -> unit
    abstract NotifyFormatSpecifierLocation : range -> unit
    abstract CurrentSource : string option

let (|ValRefOfProp|_|) (pi : PropInfo) = pi.ArbitraryValRef
let (|ValRefOfMeth|_|) (mi : MethInfo) = mi.ArbitraryValRef
let (|ValRefOfEvent|_|) (evt : EventInfo) = evt.ArbitraryValRef

let rec (|RecordFieldUse|_|) (item : Item) = 
    match item with
    | Item.RecdField(RecdFieldInfo(_, RFRef(tcref, name))) -> Some (name, tcref)
    | Item.SetterArg(_, RecordFieldUse(f)) -> Some(f)
    | _ -> None

let rec (|ILFieldUse|_|) (item : Item) = 
    match item with
    | Item.ILField(finfo) -> Some(finfo)
    | Item.SetterArg(_, ILFieldUse(f)) -> Some(f)
    | _ -> None

let rec (|PropertyUse|_|) (item : Item) = 
    match item with
    | Item.Property(_, pinfo::_) -> Some(pinfo)
    | Item.SetterArg(_, PropertyUse(pinfo)) -> Some(pinfo)
    | _ -> None

let rec (|FSharpPropertyUse|_|) (item : Item) = 
    match item with
    | Item.Property(_, [ValRefOfProp vref]) -> Some(vref)
    | Item.SetterArg(_, FSharpPropertyUse(propDef)) -> Some(propDef)
    | _ -> None

let (|MethodUse|_|) (item : Item) = 
    match item with
    | Item.MethodGroup(_, [minfo],_) -> Some(minfo)
    | _ -> None

let (|FSharpMethodUse|_|) (item : Item) = 
    match item with
    | Item.MethodGroup(_, [ValRefOfMeth vref],_) -> Some(vref)
    | Item.Value(vref) when vref.IsMember -> Some(vref)
    | _ -> None

let (|EntityUse|_|) (item: Item) = 
    match item with 
    | Item.UnqualifiedType (tcref:: _) -> Some tcref
    | Item.ExnCase(tcref) -> Some tcref
    | Item.Types(_, [AbbrevOrAppTy tcref]) 
    | Item.DelegateCtor(AbbrevOrAppTy tcref) 
    | Item.FakeInterfaceCtor(AbbrevOrAppTy tcref) -> Some tcref
    | Item.CtorGroup(_, ctor::_) -> 
        match ctor.EnclosingType with 
        | AbbrevOrAppTy tcref -> Some tcref
        | _ -> None
    | _ -> None

let (|EventUse|_|) (item : Item) = 
    match item with
    | Item.Event(einfo) -> Some einfo
    | _ -> None

let (|FSharpEventUse|_|) (item : Item) = 
    match item with
    | Item.Event(ValRefOfEvent vref) -> Some vref
    | _ -> None

let (|UnionCaseUse|_|) (item : Item) = 
    match item with
    | Item.UnionCase(UnionCaseInfo(_, u1),_) -> Some u1
    | _ -> None

let (|ValUse|_|) (item:Item) = 
    match item with 
    | Item.Value vref 
    | FSharpPropertyUse vref
    | FSharpMethodUse vref
    | FSharpEventUse vref
    | Item.CustomBuilder(_, vref) -> Some vref
    | _ -> None

let (|ActivePatternCaseUse|_|) (item:Item) = 
    match item with 
    | Item.ActivePatternCase(APElemRef(_, vref, idx)) -> Some (vref.SigRange, vref.DefinitionRange, idx)
    | Item.ActivePatternResult(ap, _, idx,_) -> Some (ap.Range, ap.Range, idx)
    | _ -> None

let tyconRefDefnEq g (eref1:EntityRef) (eref2: EntityRef) =
    tyconRefEq g eref1 eref2 
    // Signature items considered equal to implementation items
    || ((eref1.DefinitionRange = eref2.DefinitionRange || eref1.SigRange = eref2.SigRange) &&
        (eref1.LogicalName = eref2.LogicalName))

let valRefDefnEq g (vref1:ValRef) (vref2: ValRef) =
    valRefEq g vref1 vref2 
    // Signature items considered equal to implementation items
    || ((vref1.DefinitionRange = vref2.DefinitionRange || vref1.SigRange = vref2.SigRange)) && 
        (vref1.LogicalName = vref2.LogicalName)

let unionCaseRefDefnEq g (uc1:UnionCaseRef) (uc2: UnionCaseRef) =
    uc1.CaseName = uc2.CaseName && tyconRefDefnEq g uc1.TyconRef uc2.TyconRef

/// Given the Item 'orig' - returns function 'other : Item -> bool', that will yield true if other and orig represents the same item and false - otherwise
let ItemsAreEffectivelyEqual g orig other = 
    match orig, other  with
    | EntityUse ty1, EntityUse ty2 -> 
        tyconRefDefnEq g ty1 ty2

    | Item.TypeVar (nm1,tp1), Item.TypeVar (nm2,tp2) -> 
        nm1 = nm2 && 
        (typeEquiv g (mkTyparTy tp1) (mkTyparTy tp2) || 
         match stripTyparEqns (mkTyparTy tp1), stripTyparEqns (mkTyparTy tp2) with 
         | TType_var tp1, TType_var tp2 -> 
            not tp1.IsCompilerGenerated && not tp1.IsFromError && 
            not tp2.IsCompilerGenerated && not tp2.IsFromError && 
            tp1.Range = tp2.Range
         | AbbrevOrAppTy tcref1, AbbrevOrAppTy tcref2 -> 
            tyconRefDefnEq g tcref1 tcref2
         | _ -> false)

    | ValUse vref1, ValUse vref2 -> 
        valRefDefnEq g vref1 vref2 

    | ActivePatternCaseUse (range1, range1i, idx1), ActivePatternCaseUse (range2, range2i, idx2) -> 
        (idx1 = idx2) && (range1 = range2 || range1i = range2i)

    | MethodUse minfo1, MethodUse minfo2 -> 
        MethInfo.MethInfosUseIdenticalDefinitions minfo1 minfo2 ||
        // Allow for equality up to signature matching
        match minfo1.ArbitraryValRef, minfo2.ArbitraryValRef with 
        | Some vref1, Some vref2 -> valRefDefnEq g vref1 vref2 
        | _ -> false

    | PropertyUse(pinfo1), PropertyUse(pinfo2) -> 
        PropInfo.PropInfosUseIdenticalDefinitions pinfo1 pinfo2 ||
        // Allow for equality up to signature matching
        match pinfo1.ArbitraryValRef, pinfo2.ArbitraryValRef with 
        | Some vref1, Some vref2 -> valRefDefnEq g vref1 vref2 
        | _ -> false

    | Item.ArgName (id1,_, _), Item.ArgName (id2,_, _) -> 
        (id1.idText = id2.idText && id1.idRange = id2.idRange)

    | (Item.ArgName (id,_, _), ValUse vref) | (ValUse vref, Item.ArgName (id, _, _)) -> 
        ((id.idRange = vref.DefinitionRange || id.idRange = vref.SigRange) && id.idText = vref.DisplayName)

    | ILFieldUse f1, ILFieldUse f2 -> 
        ILFieldInfo.ILFieldInfosUseIdenticalDefinitions f1 f2 

    | UnionCaseUse u1, UnionCaseUse u2 ->  
        unionCaseRefDefnEq g u1 u2

    | RecordFieldUse(name1, tcref1), RecordFieldUse(name2, tcref2) -> 
        name1 = name2 && tyconRefDefnEq g tcref1 tcref2

    | EventUse evt1, EventUse evt2 -> 
        EventInfo.EventInfosUseIdenticalDefintions evt1 evt2  ||
        // Allow for equality up to signature matching
        match evt1.ArbitraryValRef, evt2.ArbitraryValRef with 
        | Some vref1, Some vref2 -> valRefDefnEq g vref1 vref2 
        | _ -> false

    | Item.ModuleOrNamespaces modrefs1, Item.ModuleOrNamespaces modrefs2 ->
        modrefs1 |> List.exists (fun modref1 -> modrefs2 |> List.exists (fun r -> tyconRefDefnEq g modref1 r || fullDisplayTextOfModRef modref1 = fullDisplayTextOfModRef r))

    | _ -> false

[<System.Diagnostics.DebuggerDisplay("{DebugToString()}")>]
type CapturedNameResolution(p:pos, i:Item, tpinst, io:ItemOccurence, de:DisplayEnv, nre:NameResolutionEnv, ad:AccessorDomain, m:range) =
    member this.Pos = p
    member this.Item = i
    member this.ItemWithInst = ({ Item = i; TyparInst = tpinst } : ItemWithInst)
    member this.ItemOccurence = io
    member this.DisplayEnv = de
    member this.NameResolutionEnv = nre
    member this.AccessorDomain = ad
    member this.Range = m
    member this.DebugToString() = 
        sprintf "%A: %+A" (p.Line, p.Column) i

/// Represents container for all name resolutions that were met so far when typechecking some particular file
type TcResolutions
    (capturedEnvs : ResizeArray<range * NameResolutionEnv * AccessorDomain>,
     capturedExprTypes : ResizeArray<pos * TType * DisplayEnv * NameResolutionEnv * AccessorDomain * range>,
     capturedNameResolutions : ResizeArray<CapturedNameResolution>,
     capturedMethodGroupResolutions : ResizeArray<CapturedNameResolution>) = 

    static let empty = TcResolutions(ResizeArray(0),ResizeArray(0),ResizeArray(0),ResizeArray(0))
    
    member this.CapturedEnvs = capturedEnvs
    member this.CapturedExpressionTypings = capturedExprTypes
    member this.CapturedNameResolutions = capturedNameResolutions
    member this.CapturedMethodGroupResolutions = capturedMethodGroupResolutions

    static member Empty = empty


/// Represents container for all name resolutions that were met so far when typechecking some particular file
type TcSymbolUses(g, capturedNameResolutions : ResizeArray<CapturedNameResolution>, formatSpecifierLocations: range[]) = 
    
    // Make sure we only capture the information we really need to report symbol uses
    let cnrs = [| for cnr in capturedNameResolutions  -> struct (cnr.Item, cnr.ItemOccurence, cnr.DisplayEnv, cnr.Range) |]
    let capturedNameResolutions = () 
    do ignore capturedNameResolutions // don't capture this!

    member this.GetUsesOfSymbol(item) = 
        [| for (struct (cnrItem,occ,denv,m)) in cnrs do
               if protectAssemblyExploration false (fun () -> ItemsAreEffectivelyEqual g item cnrItem) then
                  yield occ, denv, m |]

    member this.GetAllUsesOfSymbols() = 
        [| for (struct (cnrItem,occ,denv,m)) in cnrs do
              yield (cnrItem, occ, denv, m) |]

    member this.GetFormatSpecifierLocations() =  formatSpecifierLocations


/// An accumulator for the results being emitted into the tcSink.
type TcResultsSinkImpl(g, ?source: string) =
    let capturedEnvs = ResizeArray<_>()
    let capturedExprTypings = ResizeArray<_>()
    let capturedNameResolutions = ResizeArray<_>()
    let capturedFormatSpecifierLocations = ResizeArray<_>()
    let capturedNameResolutionIdentifiers = 
        new System.Collections.Generic.HashSet<pos * string>
            ( { new IEqualityComparer<_> with 
                    member __.GetHashCode((p:pos,i)) = p.Line + 101 * p.Column + hash i
                    member __.Equals((p1,i1),(p2,i2)) = posEq p1 p2 && i1 =  i2 } )
    let capturedMethodGroupResolutions = ResizeArray<_>()
    let allowedRange (m:range) = not m.IsSynthetic       

    member this.GetResolutions() = 
        TcResolutions(capturedEnvs, capturedExprTypings, capturedNameResolutions, capturedMethodGroupResolutions)

    member this.GetSymbolUses() = 
        TcSymbolUses(g, capturedNameResolutions, capturedFormatSpecifierLocations.ToArray())

    interface ITypecheckResultsSink with
        member sink.NotifyEnvWithScope(m,nenv,ad) = 
            if allowedRange m then 
                capturedEnvs.Add((m,nenv,ad)) 

        member sink.NotifyExprHasType(endPos,ty,denv,nenv,ad,m) = 
            if allowedRange m then 
                capturedExprTypings.Add((endPos,ty,denv,nenv,ad,m))

        member sink.NotifyNameResolution(endPos,item,itemMethodGroup,tpinst,occurenceType,denv,nenv,ad,m,replace) = 
            // Desugaring some F# constructs (notably computation expressions with custom operators)
            // results in duplication of textual variables. So we ensure we never record two name resolutions 
            // for the same identifier at the same location.
            if allowedRange m then 
                let keyOpt = 
                    match item with
                    | Item.Value vref -> Some (endPos, vref.DisplayName)
                    | Item.ArgName (id, _, _) -> Some (endPos, id.idText)
                    | _ -> None

                let alreadyDone = 
                    match keyOpt with
                    | Some key -> not (capturedNameResolutionIdentifiers.Add key)
                    | _ -> false
                
                if replace then 
                    capturedNameResolutions.RemoveAll(fun cnr -> cnr.Range = m) |> ignore
                    capturedMethodGroupResolutions.RemoveAll(fun cnr -> cnr.Range = m) |> ignore

                if not alreadyDone then 
                    capturedNameResolutions.Add(CapturedNameResolution(endPos,item,tpinst,occurenceType,denv,nenv,ad,m)) 
                    capturedMethodGroupResolutions.Add(CapturedNameResolution(endPos,itemMethodGroup,[],occurenceType,denv,nenv,ad,m)) 

        member sink.NotifyFormatSpecifierLocation(m) = 
            capturedFormatSpecifierLocations.Add(m)

        member sink.CurrentSource = source


/// An abstract type for reporting the results of name resolution and type checking, and which allows
/// temporary suspension and/or redirection of reporting.
type TcResultsSink = 
    { mutable CurrentSink : ITypecheckResultsSink option }
    static member NoSink =  { CurrentSink = None }
    static member WithSink sink = { CurrentSink = Some sink }

/// Temporarily redirect reporting of name resolution and type checking results
let WithNewTypecheckResultsSink (newSink : ITypecheckResultsSink, sink:TcResultsSink) = 
    let old = sink.CurrentSink
    sink.CurrentSink <- Some newSink
    { new System.IDisposable with member x.Dispose() = sink.CurrentSink <- old }

/// Temporarily suspend reporting of name resolution and type checking results
let TemporarilySuspendReportingTypecheckResultsToSink (sink:TcResultsSink) = 
    let old = sink.CurrentSink
    sink.CurrentSink <- None
    { new System.IDisposable with member x.Dispose() = sink.CurrentSink <- old }


/// Report the active name resolution environment for a specific source range
let CallEnvSink (sink:TcResultsSink) (scopem,nenv,ad) = 
    match sink.CurrentSink with 
    | None -> () 
    | Some sink -> sink.NotifyEnvWithScope(scopem,nenv,ad)

/// Report a specific name resolution at a source range
let CallNameResolutionSink (sink:TcResultsSink) (m:range,nenv,item,itemMethodGroup,tpinst,occurenceType,denv,ad) = 
    match sink.CurrentSink with 
    | None -> () 
    | Some sink -> sink.NotifyNameResolution(m.End,item,itemMethodGroup,tpinst,occurenceType,denv,nenv,ad,m,false)  

let CallNameResolutionSinkReplacing (sink:TcResultsSink) (m:range,nenv,item,itemMethodGroup,tpinst,occurenceType,denv,ad) = 
    match sink.CurrentSink with 
    | None -> () 
    | Some sink -> sink.NotifyNameResolution(m.End,item,itemMethodGroup,tpinst,occurenceType,denv,nenv,ad,m,true)  

/// Report a specific expression typing at a source range
let CallExprHasTypeSink (sink:TcResultsSink) (m:range,nenv,typ,denv,ad) = 
    match sink.CurrentSink with 
    | None -> () 
    | Some sink -> sink.NotifyExprHasType(m.End,typ,denv,nenv,ad,m)

//-------------------------------------------------------------------------
// Check inferability of type parameters in resolved items.
//------------------------------------------------------------------------- 

/// Checks if the type variables associated with the result of a resolution are inferable,
/// i.e. occur in the arguments or return type of the resolution. If not give a warning
/// about a type instantiation being needed.
type ResultTyparChecker = ResultTyparChecker of (unit -> bool)

let CheckAllTyparsInferrable amap m item = 
    match item with
    | Item.Property(_,pinfos) -> 
        pinfos |> List.forall (fun pinfo -> 
            pinfo.IsExtensionMember ||
            let freeInDeclaringType = freeInType CollectTyparsNoCaching pinfo.EnclosingType
            let freeInArgsAndRetType = 
                accFreeInTypes CollectTyparsNoCaching (pinfo.GetParamTypes(amap,m)) 
                       (freeInType CollectTyparsNoCaching (pinfo.GetPropertyType(amap,m)))
            let free = Zset.diff freeInDeclaringType.FreeTypars  freeInArgsAndRetType.FreeTypars
            free.IsEmpty)

    | Item.MethodGroup(_,minfos,_) -> 
        minfos |> List.forall (fun minfo -> 
            minfo.IsExtensionMember ||
            let fminst = minfo.FormalMethodInst
            let freeInDeclaringType = freeInType CollectTyparsNoCaching minfo.EnclosingType
            let freeInArgsAndRetType = 
                List.foldBack (accFreeInTypes CollectTyparsNoCaching) (minfo.GetParamTypes(amap, m, fminst)) 
                   (accFreeInTypes CollectTyparsNoCaching (minfo.GetObjArgTypes(amap, m, fminst)) 
                       (freeInType CollectTyparsNoCaching (minfo.GetFSharpReturnTy(amap, m, fminst))))
            let free = Zset.diff freeInDeclaringType.FreeTypars  freeInArgsAndRetType.FreeTypars
            free.IsEmpty)

    | Item.CtorGroup _ 
    | Item.FakeInterfaceCtor _ 
    | Item.DelegateCtor _ 
    | Item.Types _ 
    | Item.ModuleOrNamespaces _
    | Item.CustomOperation _ 
    | Item.CustomBuilder _ 
    | Item.TypeVar _ 
    | Item.ArgName _ 
    | Item.ActivePatternResult _
    | Item.Value _ 
    | Item.ActivePatternCase _ 
    | Item.UnionCase _ 
    | Item.ExnCase _ 
    | Item.RecdField _ 
    | Item.NewDef _ 
    | Item.ILField _ 
    | Item.Event _ 
    | Item.ImplicitOp _ 
    | Item.UnqualifiedType _
    | Item.SetterArg _ -> true
    
//-------------------------------------------------------------------------
// Check inferability of type parameters in resolved items.
//------------------------------------------------------------------------- 

/// Keeps track of information relevant to the chosen resolution of a long identifier
///
/// When we resolve an item such as System.Console.In we
/// resolve it in one step to a property/val/method etc. item. However
/// Visual Studio needs to know about the exact resolutions of the names
/// System and Console, i.e. the 'entity path' of the resolution. 
///
/// Each of the resolution routines keeps track of the entity path and 
/// ultimately calls ResolutionInfo.Method to record it for 
/// later use by Visual Studio.
type ResolutionInfo = 
    | ResolutionInfo of (*entityPath, reversed*)(range * EntityRef) list * (*warnings/errors*)(ResultTyparChecker -> unit)

    static member SendEntityPathToSink(sink, ncenv: NameResolver, nenv, occ, ad, ResolutionInfo(entityPath,warnings), typarChecker) = 
        entityPath |> List.iter (fun (m,eref:EntityRef) -> 
            CheckEntityAttributes ncenv.g eref m |> CommitOperationResult        
            CheckTyconAccessible ncenv.amap m ad eref |> ignore
            let item = 
                if eref.IsModuleOrNamespace then 
                    Item.ModuleOrNamespaces [eref] 
                else 
                    Item.Types(eref.DisplayName,[FreshenTycon ncenv m eref])
            CallNameResolutionSink sink (m,nenv,item,item,emptyTyparInst,occ,nenv.eDisplayEnv,ad))
        warnings(typarChecker)
 
    static member Empty = 
        ResolutionInfo([],(fun _ -> ()))

    member x.AddEntity info = 
        let (ResolutionInfo(entityPath,warnings)) = x
        ResolutionInfo(info::entityPath,warnings)

    member x.AddWarning f = 
        let (ResolutionInfo(entityPath,warnings)) = x
        ResolutionInfo(entityPath,(fun typarChecker -> f typarChecker; warnings typarChecker))



/// Resolve ambiguities between types overloaded by generic arity, based on number of type arguments.
/// Also check that we're not returning direct references to generated provided types.
//
// Given ambiguous C<>, C<_>    we resolve the ambiguous 'C.M' to C<> without warning
// Given ambiguous C<_>, C<_,_> we resolve the ambiguous 'C.M' to C<_> with an ambiguity error
// Given C<_>                   we resolve the ambiguous 'C.M' to C<_> with a warning if the argument or return types can't be inferred

// Given ambiguous C<>, C<_>    we resolve the ambiguous 'C()' to C<> without warning
// Given ambiguous C<_>, C<_,_> we resolve the ambiguous 'C()' to C<_> with an ambiguity error
// Given C<_>                   we resolve the ambiguous 'C()' to C<_> with a warning if the argument or return types can't be inferred

let CheckForTypeLegitimacyAndMultipleGenericTypeAmbiguities 
        (tcrefs:(ResolutionInfo * TyconRef) list, 
         typeNameResInfo:TypeNameResolutionInfo, 
         genOk:PermitDirectReferenceToGeneratedType, 
         m) = 

    let tcrefs = 
        tcrefs 
        // remove later duplicates (if we've opened the same module more than once)
        |> List.distinctBy (fun (_,tcref) -> tcref.Stamp) 
        // List.sortBy is a STABLE sort (the order matters!)
        |> List.sortBy (fun (_,tcref) -> tcref.Typars(m).Length)

    let tcrefs = 
        match tcrefs with 
        | ((_resInfo,tcref) :: _) when 
                // multiple types
                tcrefs.Length > 1 && 
                // no explicit type instantiation
                typeNameResInfo.StaticArgsInfo.HasNoStaticArgsInfo && 
                // some type arguments required on all types (note sorted by typar count above)
                tcref.Typars(m).Length > 0 && 
                // plausible types have different arities
                (tcrefs |> Seq.distinctBy (fun (_,tcref) -> tcref.Typars(m).Length) |> Seq.length > 1)  ->
            [ for (resInfo,tcref) in tcrefs do 
                let resInfo = resInfo.AddWarning (fun _typarChecker -> errorR(Error(FSComp.SR.nrTypeInstantiationNeededToDisambiguateTypesWithSameName(tcref.DisplayName, tcref.DisplayNameWithStaticParametersAndUnderscoreTypars),m)))
                yield (resInfo,tcref) ]

        | [(resInfo,tcref)] when  typeNameResInfo.StaticArgsInfo.HasNoStaticArgsInfo && tcref.Typars(m).Length > 0 && typeNameResInfo.ResolutionFlag = ResolveTypeNamesToTypeRefs ->
            let resInfo = 
                resInfo.AddWarning (fun (ResultTyparChecker typarChecker) -> 
                    if not (typarChecker()) then 
                        warning(Error(FSComp.SR.nrTypeInstantiationIsMissingAndCouldNotBeInferred(tcref.DisplayName, tcref.DisplayNameWithStaticParametersAndUnderscoreTypars),m)))
            [(resInfo,tcref)]

        | _ -> 
            tcrefs

#if EXTENSIONTYPING
    for (_,tcref) in tcrefs do 
        // Type generators can't be returned by name resolution, unless PermitDirectReferenceToGeneratedType.Yes
        CheckForDirectReferenceToGeneratedType (tcref, genOk, m)
#else
    genOk |> ignore
#endif

    tcrefs    


//-------------------------------------------------------------------------
// Consume ids that refer to a namespace
//------------------------------------------------------------------------- 

/// Perform name resolution for an identifier which must resolve to be a namespace or module.
let rec ResolveLongIndentAsModuleOrNamespace atMostOne amap m fullyQualified (nenv:NameResolutionEnv) ad (lid:Ident list) =
    match lid with 
    | [] -> NoResultsOrUsefulErrors

    | id :: rest when id.idText = MangledGlobalName ->
        if isNil rest then
            error (Error(FSComp.SR.nrGlobalUsedOnlyAsFirstName(), id.idRange))
        else
            ResolveLongIndentAsModuleOrNamespace atMostOne amap m FullyQualified nenv ad rest

    | id :: rest -> 
        let moduleOrNamespaces = nenv.ModulesAndNamespaces fullyQualified
        let namespaceNotFound = lazy(
            let suggestModulesAndNamespaces() =
                moduleOrNamespaces
                |> Seq.collect (fun kv -> kv.Value)
                |> Seq.filter (fun modref -> IsEntityAccessible amap m ad modref)
                |> Seq.collect (fun e -> [e.DisplayName; e.DemangledModuleOrNamespaceName])
                |> Set.ofSeq

            UndefinedName(0,FSComp.SR.undefinedNameNamespaceOrModule,id,suggestModulesAndNamespaces))
        
        let moduleNotFoundErrorCache = ref None
        let moduleNotFound (modref: ModuleOrNamespaceRef) (mty:ModuleOrNamespaceType) id depth =
            match !moduleNotFoundErrorCache with
            | Some error -> error
            | None ->
                let suggestNames() =
                    mty.ModulesAndNamespacesByDemangledName
                    |> Seq.filter (fun kv -> IsEntityAccessible amap m ad (modref.NestedTyconRef kv.Value))
                    |> Seq.collect (fun e -> [e.Value.DisplayName; e.Value.DemangledModuleOrNamespaceName])
                    |> Set.ofSeq
                
                let error = raze (UndefinedName(depth,FSComp.SR.undefinedNameNamespace,id,suggestNames))
                moduleNotFoundErrorCache := Some error
                error

        match moduleOrNamespaces.TryFind id.idText with
        | Some modrefs ->
            /// Look through the sub-namespaces and/or modules
            let rec look depth (modref: ModuleOrNamespaceRef) (mty:ModuleOrNamespaceType) (lid:Ident list) =
                match lid with 
                | [] -> success (depth,modref,mty)
                | id :: rest ->
                    match mty.ModulesAndNamespacesByDemangledName.TryFind id.idText with
                    | Some mspec -> 
                        let subref = modref.NestedTyconRef mspec
                        if IsEntityAccessible amap m ad subref then
                            look (depth+1) subref mspec.ModuleOrNamespaceType rest
                        else
                            moduleNotFound modref mty id depth
                    | _ -> moduleNotFound modref mty id depth

            modrefs |> CollectResults2 atMostOne (fun modref -> 
                if IsEntityAccessible amap m ad modref then 
                    look 1 modref modref.ModuleOrNamespaceType rest
                else
                    raze (namespaceNotFound.Force())) 
        | None -> raze (namespaceNotFound.Force())


let ResolveLongIndentAsModuleOrNamespaceThen atMostOne amap m fullyQualified (nenv:NameResolutionEnv) ad lid f =
    match lid with 
    | [] -> NoResultsOrUsefulErrors
    | id :: rest -> 
        match ResolveLongIndentAsModuleOrNamespace ResultCollectionSettings.AllResults amap m fullyQualified nenv ad [id] with
        | Result modrefs -> 
            modrefs |> CollectResults2 atMostOne (fun (depth,modref,mty) ->  
                let resInfo = ResolutionInfo.Empty.AddEntity(id.idRange,modref) 
                f resInfo (depth+1) id.idRange modref mty rest)
        | Exception err -> Exception err 

//-------------------------------------------------------------------------
// Bind name used in "new Foo.Bar(...)" constructs
//------------------------------------------------------------------------- 

let private ResolveObjectConstructorPrim (ncenv:NameResolver) edenv resInfo m ad typ = 
    let g = ncenv.g
    let amap = ncenv.amap
    if isDelegateTy g typ then 
        success (resInfo,Item.DelegateCtor typ)
    else 
        let ctorInfos = GetIntrinsicConstructorInfosOfType ncenv.InfoReader m typ
        if isNil ctorInfos && isInterfaceTy g typ then 
            success (resInfo, Item.FakeInterfaceCtor typ)
        else 
            let defaultStructCtorInfo = 
                if (not (ctorInfos |> List.exists (fun x -> x.IsNullary)) &&
                    isStructTy g typ && 
                    not (isRecdTy g typ) && 
                    not (isUnionTy g typ)) 
                then 
                    [DefaultStructCtor(g,typ)]
                else []
            if (isNil defaultStructCtorInfo && isNil ctorInfos) || not (isAppTy g typ) then 
                raze (Error(FSComp.SR.nrNoConstructorsAvailableForType(NicePrint.minimalStringOfType edenv typ),m))
            else 
                let ctorInfos = ctorInfos |> List.filter (IsMethInfoAccessible amap m ad)  
                success (resInfo,Item.MakeCtorGroup ((tcrefOfAppTy g typ).LogicalName, (defaultStructCtorInfo@ctorInfos))) 

/// Perform name resolution for an identifier which must resolve to be an object constructor.
let ResolveObjectConstructor (ncenv:NameResolver) edenv m ad typ = 
    ResolveObjectConstructorPrim (ncenv:NameResolver) edenv [] m ad typ  |?> (fun (_resInfo,item) -> item)

//-------------------------------------------------------------------------
// Bind the "." notation (member lookup or lookup in a type)
//------------------------------------------------------------------------- 

/// Query the declared properties of a type (including inherited properties)
let IntrinsicPropInfosOfTypeInScope (infoReader:InfoReader) (optFilter, ad) findFlag m typ =
    let g = infoReader.g
    let amap = infoReader.amap
    let pinfos = GetIntrinsicPropInfoSetsOfType infoReader (optFilter, ad, AllowMultiIntfInstantiations.Yes) findFlag m typ
    let pinfos = pinfos |> ExcludeHiddenOfPropInfos g amap m 
    pinfos

/// Select from a list of extension properties 
let SelectPropInfosFromExtMembers (infoReader:InfoReader,ad,optFilter) declaringTy m extMemInfos = 
    let g = infoReader.g
    let amap = infoReader.amap
    // NOTE: multiple "open"'s push multiple duplicate values into eIndexedExtensionMembers, hence setify.
    let seen = HashSet(ExtensionMember.Comparer g)
    let propCollector = new PropertyCollector(g,amap,m,declaringTy,optFilter,ad)
    for emem in extMemInfos do
        if seen.Add emem then
            match emem with 
            | FSExtMem (vref,_pri) -> 
                match vref.MemberInfo with 
                | None -> ()
                | Some membInfo -> propCollector.Collect(membInfo,vref)
            | ILExtMem _ -> 
                // No extension properties coming from .NET
                ()
    propCollector.Close()

/// Query the available extension properties of a type (including extension properties for inherited types)
let ExtensionPropInfosOfTypeInScope (infoReader:InfoReader) (nenv: NameResolutionEnv) (optFilter, ad) m typ =
    let g = infoReader.g
    
    let extMemsFromHierarchy = 
        infoReader.GetEntireTypeHierachy(AllowMultiIntfInstantiations.Yes,m,typ) |> List.collect (fun typ -> 
             if (isAppTy g typ) then 
                let tcref = tcrefOfAppTy g typ
                let extMemInfos = nenv.eIndexedExtensionMembers.Find tcref
                SelectPropInfosFromExtMembers (infoReader,ad,optFilter) typ m extMemInfos
             else [])

    let extMemsDangling = SelectPropInfosFromExtMembers  (infoReader,ad,optFilter) typ m nenv.eUnindexedExtensionMembers 
    extMemsDangling @ extMemsFromHierarchy


/// Get all the available properties of a type (both intrinsic and extension)
let AllPropInfosOfTypeInScope infoReader nenv (optFilter, ad) findFlag m typ =
    IntrinsicPropInfosOfTypeInScope infoReader (optFilter, ad) findFlag m typ
    @ ExtensionPropInfosOfTypeInScope infoReader nenv (optFilter, ad) m typ 

/// Get the available methods of a type (both declared and inherited)
let IntrinsicMethInfosOfType (infoReader:InfoReader) (optFilter,ad,allowMultiIntfInst) findFlag m typ =
    let g = infoReader.g
    let amap = infoReader.amap
    let minfos = GetIntrinsicMethInfoSetsOfType infoReader (optFilter,ad,allowMultiIntfInst) findFlag m typ
    let minfos = minfos |> ExcludeHiddenOfMethInfos g amap m
    minfos

/// Select from a list of extension methods
let SelectMethInfosFromExtMembers (infoReader:InfoReader) optFilter apparentTy m extMemInfos = 
    let g = infoReader.g
    // NOTE: multiple "open"'s push multiple duplicate values into eIndexedExtensionMembers 
    let seen = HashSet(ExtensionMember.Comparer g)
    [
        for emem in extMemInfos do
            if seen.Add emem then
                match emem with 
                | FSExtMem (vref,pri) -> 
                    match vref.MemberInfo with 
                    | None -> ()
                    | Some membInfo -> 
                        match TrySelectMemberVal g optFilter apparentTy (Some pri) membInfo vref with
                        | Some m -> yield m
                        | _ -> ()
                | ILExtMem (actualParent,minfo,pri) when (match optFilter with None -> true | Some nm -> nm = minfo.LogicalName) ->
                    // Make a reference to the type containing the extension members
                    match minfo with 
                    | ILMeth(_,ilminfo,_) -> 
                         yield (MethInfo.CreateILExtensionMeth (infoReader.amap, m, apparentTy, actualParent, Some pri, ilminfo.RawMetadata))
                    // F#-defined IL-style extension methods are not seen as extension methods in F# code
                    | FSMeth(g,_,vref,_) -> 
                         yield (FSMeth(g, apparentTy, vref, Some pri))
#if EXTENSIONTYPING
                    // // Provided extension methods are not yet supported
                    | ProvidedMeth(amap,providedMeth,_,m) -> 
                         yield (ProvidedMeth(amap, providedMeth, Some pri,m))
#endif
                    | DefaultStructCtor _ -> 
                         ()
                | _ -> ()
    ]

/// Query the available extension properties of a methods (including extension methods for inherited types)
let ExtensionMethInfosOfTypeInScope (infoReader:InfoReader) (nenv: NameResolutionEnv) optFilter m typ =
    let extMemsDangling = SelectMethInfosFromExtMembers  infoReader optFilter typ  m nenv.eUnindexedExtensionMembers
    let extMemsFromHierarchy = 
        infoReader.GetEntireTypeHierachy(AllowMultiIntfInstantiations.Yes,m,typ) |> List.collect (fun typ -> 
            let g = infoReader.g
            if (isAppTy g typ) then 
                let tcref = tcrefOfAppTy g typ
                let extValRefs = nenv.eIndexedExtensionMembers.Find tcref
                SelectMethInfosFromExtMembers infoReader optFilter typ  m extValRefs
            else [])
    extMemsDangling @ extMemsFromHierarchy

/// Get all the available methods of a type (both intrinsic and extension)
let AllMethInfosOfTypeInScope infoReader nenv (optFilter,ad) findFlag m typ =
    IntrinsicMethInfosOfType infoReader (optFilter,ad,AllowMultiIntfInstantiations.Yes) findFlag m typ 
    @ ExtensionMethInfosOfTypeInScope infoReader nenv optFilter m typ          


/// Used to report an error condition where name resolution failed due to an indeterminate type
exception IndeterminateType of range

/// Indicates the kind of lookup being performed. Note, this type should be made private to nameres.fs.
[<RequireQualifiedAccess>]
type LookupKind = 
   | RecdField
   | Pattern
   | Expr
   | Type
   | Ctor


/// Try to find a union case of a type, with the given name
let TryFindUnionCaseOfType g typ nm =
    if isAppTy g typ then 
        let tcref,tinst = destAppTy g typ
        match tcref.GetUnionCaseByName nm with 
        | None -> None
        | Some ucase -> Some(UnionCaseInfo(tinst,tcref.MakeNestedUnionCaseRef ucase))
    else 
        None

let CoreDisplayName(pinfo:PropInfo) =   
    match pinfo with
    | FSProp(_,_,_,Some set) -> set.CoreDisplayName
    | FSProp(_,_,Some get,_) -> get.CoreDisplayName
    | FSProp _ -> failwith "unexpected (property must have either getter or setter)"
    | ILProp(_,ILPropInfo(_,def))  -> def.Name
#if EXTENSIONTYPING
    | ProvidedProp(_,pi,m) -> pi.PUntaint((fun pi -> pi.Name), m)
#endif

let DecodeFSharpEvent (pinfos:PropInfo list) ad g (ncenv:NameResolver) m =
    match pinfos with 
    | [pinfo] when pinfo.IsFSharpEventProperty -> 
        let nm = CoreDisplayName(pinfo)
        let minfos1 = GetImmediateIntrinsicMethInfosOfType (Some("add_"+nm),ad) g ncenv.amap m pinfo.EnclosingType 
        let minfos2 = GetImmediateIntrinsicMethInfosOfType (Some("remove_"+nm),ad) g ncenv.amap m pinfo.EnclosingType
        match  minfos1,minfos2 with 
        | [FSMeth(_,_,addValRef,_)],[FSMeth(_,_,removeValRef,_)] -> 
            // FOUND PROPERTY-AS-EVENT AND CORRESPONDING ADD/REMOVE METHODS
            Some(Item.Event(FSEvent(g,pinfo,addValRef,removeValRef)))
        | _ -> 
            // FOUND PROPERTY-AS-EVENT BUT DIDN'T FIND CORRESPONDING ADD/REMOVE METHODS
            Some(Item.Property (nm,pinfos))
    | pinfo :: _ -> 
        let nm = CoreDisplayName(pinfo)
        Some(Item.Property (nm,pinfos))
    | _ -> 
        None

/// Returns all record label names for the given type.
let GetRecordLabelsForType g nenv typ =
    if isRecdTy g typ then
        let typeName = NicePrint.minimalStringOfType nenv.eDisplayEnv typ
        nenv.eFieldLabels
        |> Seq.filter (fun kv -> 
            kv.Value 
            |> List.exists (fun r -> r.TyconRef.DisplayName = typeName))
        |> Seq.map (fun kv -> kv.Key)
        |> Set.ofSeq
    else
        Set.empty

// REVIEW: this shows up on performance logs. Consider for example endless resolutions of "List.map" to 
// the empty set of results, or "x.Length" for a list or array type. This indicates it could be worth adding a cache here.
let rec ResolveLongIdentInTypePrim (ncenv:NameResolver) nenv lookupKind (resInfo:ResolutionInfo) depth m ad (lid:Ident list) findFlag (typeNameResInfo: TypeNameResolutionInfo) typ =
    let g = ncenv.g
    match lid with 
    | [] -> error(InternalError("ResolveLongIdentInTypePrim",m))
    | id :: rest -> 
        let m = unionRanges m id.idRange
        let nm = id.idText // used to filter the searches of the tables 
        let optFilter = Some nm // used to filter the searches of the tables 
        let contentsSearchAccessible = 
           let unionCaseSearch = 
               match lookupKind with 
               | LookupKind.Expr | LookupKind.Pattern -> TryFindUnionCaseOfType g typ nm  
               | _ -> None

           // Lookup: datatype constructors take precedence 
           match unionCaseSearch with 
           | Some ucase -> 
               OneResult (success(resInfo,Item.UnionCase(ucase,false),rest))
           | None -> 
                let isLookUpExpr = lookupKind = LookupKind.Expr
                match TryFindIntrinsicNamedItemOfType ncenv.InfoReader (nm,ad) findFlag m typ with
                | Some (PropertyItem psets) when isLookUpExpr -> 
                    let pinfos = psets |> ExcludeHiddenOfPropInfos g ncenv.amap m
                    
                    // fold the available extension members into the overload resolution
                    let extensionPropInfos = ExtensionPropInfosOfTypeInScope ncenv.InfoReader nenv (optFilter,ad) m typ
                    
                    // make sure to keep the intrinsic pinfos before the extension pinfos in the list,
                    // since later on this logic is used when giving preference to intrinsic definitions
                    match DecodeFSharpEvent (pinfos@extensionPropInfos) ad g ncenv m with
                    | Some x -> success [resInfo, x, rest]
                    | None -> raze (UndefinedName (depth,FSComp.SR.undefinedNameFieldConstructorOrMember, id,NoSuggestions))
                | Some(MethodItem msets) when isLookUpExpr -> 
                    let minfos = msets |> ExcludeHiddenOfMethInfos g ncenv.amap m
                    
                    // fold the available extension members into the overload resolution
                    let extensionMethInfos = ExtensionMethInfosOfTypeInScope ncenv.InfoReader nenv optFilter m typ

                    success [resInfo,Item.MakeMethGroup (nm,minfos@extensionMethInfos),rest]
                | Some (ILFieldItem (finfo:: _))  when (match lookupKind with LookupKind.Expr | LookupKind.Pattern -> true | _ -> false) -> 
                    success [resInfo,Item.ILField finfo,rest]

                | Some (EventItem (einfo :: _)) when isLookUpExpr -> 
                    success [resInfo,Item.Event einfo,rest]
                | Some (RecdFieldItem (rfinfo)) when (match lookupKind with LookupKind.Expr | LookupKind.RecdField | LookupKind.Pattern -> true | _ -> false) -> 
                    success [resInfo,Item.RecdField(rfinfo),rest]
                | _ ->

                let pinfos = ExtensionPropInfosOfTypeInScope ncenv.InfoReader nenv (optFilter, ad) m typ
                if not (isNil pinfos) && isLookUpExpr then OneResult(success (resInfo,Item.Property (nm,pinfos),rest)) else
                let minfos = ExtensionMethInfosOfTypeInScope ncenv.InfoReader nenv optFilter m typ

                if not (isNil minfos) && isLookUpExpr then 
                    success [resInfo,Item.MakeMethGroup (nm,minfos),rest]
                elif isTyparTy g typ then raze (IndeterminateType(unionRanges m id.idRange))
                else NoResultsOrUsefulErrors

        match contentsSearchAccessible with
        | Result res when not (isNil res) -> contentsSearchAccessible
        | Exception _ -> contentsSearchAccessible
        | _ -> 
              
        let nestedSearchAccessible = 
            let nestedTypes = GetNestedTypesOfType (ad, ncenv, Some nm, (if isNil rest then typeNameResInfo.StaticArgsInfo else TypeNameResolutionStaticArgsInfo.Indefinite), true, m) typ
            if isNil rest then 
                if isNil nestedTypes then 
                    NoResultsOrUsefulErrors
                else 
                    match typeNameResInfo.ResolutionFlag with 
                    | ResolveTypeNamesToCtors -> 
                        nestedTypes 
                        |> CollectAtMostOneResult (ResolveObjectConstructorPrim ncenv nenv.eDisplayEnv resInfo m ad) 
                        |> MapResults (fun (resInfo,item) -> (resInfo,item,[]))
                    | ResolveTypeNamesToTypeRefs -> 
                        OneSuccess (resInfo,Item.Types (nm,nestedTypes),rest)
            else 
                ResolveLongIdentInNestedTypes ncenv nenv lookupKind resInfo (depth+1) id m ad rest findFlag typeNameResInfo nestedTypes

        match nestedSearchAccessible with
        | Result res when not (isNil res) -> nestedSearchAccessible
        | _ -> 
            let suggestMembers() = 
                let suggestions1 =
                    ExtensionPropInfosOfTypeInScope ncenv.InfoReader nenv (None, ad) m typ 
                    |> List.map (fun p -> p.PropertyName)
                    |> Set.ofList
                let suggestions2 =
                    ExtensionMethInfosOfTypeInScope ncenv.InfoReader nenv None m typ
                    |> List.map (fun m -> m.DisplayName)
                    |> Set.ofList
                let suggestions3 =
                    GetIntrinsicPropInfosOfType ncenv.InfoReader (None, ad, AllowMultiIntfInstantiations.No) findFlag m typ
                    |> List.map (fun p -> p.PropertyName)
                    |> Set.ofList
                let suggestions4 =
                    GetIntrinsicMethInfosOfType ncenv.InfoReader (None, ad, AllowMultiIntfInstantiations.No) findFlag m typ
                    |> List.filter (fun m -> not m.IsClassConstructor && not m.IsConstructor)
                    |> List.map (fun m -> m.DisplayName)
                    |> Set.ofList
                let suggestions5 = GetRecordLabelsForType g nenv typ
                let suggestions6 =
                    match lookupKind with 
                    | LookupKind.Expr | LookupKind.Pattern ->
                        if isAppTy g typ then 
                            let tcref,_ = destAppTy g typ
                            tcref.UnionCasesArray
                            |> Array.map (fun uc -> uc.DisplayName)
                            |> Set.ofArray
                        else 
                            Set.empty
                    | _ -> Set.empty
                        
                suggestions1 
                |> Set.union suggestions2
                |> Set.union suggestions3
                |> Set.union suggestions4
                |> Set.union suggestions5
                |> Set.union suggestions6

            raze (UndefinedName (depth,FSComp.SR.undefinedNameFieldConstructorOrMember, id, suggestMembers))
        
and ResolveLongIdentInNestedTypes (ncenv:NameResolver) nenv lookupKind resInfo depth id m ad lid findFlag typeNameResInfo typs = 
    typs |> CollectAtMostOneResult (fun typ -> 
        let resInfo = if isAppTy ncenv.g typ then resInfo.AddEntity(id.idRange,tcrefOfAppTy ncenv.g typ) else resInfo
        ResolveLongIdentInTypePrim ncenv nenv lookupKind resInfo depth m ad lid findFlag typeNameResInfo typ 
        |> AtMostOneResult m) 

/// Resolve a long identifier using type-qualified name resolution.
let ResolveLongIdentInType sink ncenv nenv lookupKind m ad lid findFlag typeNameResInfo typ =
    let resInfo,item,rest = 
        ResolveLongIdentInTypePrim (ncenv:NameResolver) nenv lookupKind ResolutionInfo.Empty 0 m ad lid findFlag typeNameResInfo typ
        |> AtMostOneResult m
        |> ForceRaise
    ResolutionInfo.SendEntityPathToSink (sink,ncenv,nenv,ItemOccurence.UseInType,ad,resInfo,ResultTyparChecker(fun () -> CheckAllTyparsInferrable ncenv.amap m item))
    item,rest

let private ResolveLongIdentInTyconRef (ncenv:NameResolver) nenv lookupKind resInfo depth m ad lid typeNameResInfo tcref =
#if EXTENSIONTYPING
    // No dotting through type generators to get to a member!
    CheckForDirectReferenceToGeneratedType (tcref, PermitDirectReferenceToGeneratedType.No, m)
#endif
    let typ = FreshenTycon ncenv m tcref
    typ |> ResolveLongIdentInTypePrim ncenv nenv lookupKind resInfo depth m ad lid IgnoreOverrides typeNameResInfo  

let private ResolveLongIdentInTyconRefs atMostOne (ncenv:NameResolver) nenv lookupKind depth m ad lid typeNameResInfo idRange tcrefs = 
    tcrefs |> CollectResults2 atMostOne (fun (resInfo:ResolutionInfo,tcref) -> 
        let resInfo = resInfo.AddEntity(idRange,tcref)
        tcref |> ResolveLongIdentInTyconRef ncenv nenv lookupKind resInfo depth m ad lid typeNameResInfo |> AtMostOneResult m) 

//-------------------------------------------------------------------------
// ResolveExprLongIdentInModuleOrNamespace 
//------------------------------------------------------------------------- 

let (|AccessibleEntityRef|_|) amap m ad (modref: ModuleOrNamespaceRef) mspec = 
    let eref = modref.NestedTyconRef mspec
    if IsEntityAccessible amap m ad eref then Some eref else None

let rec ResolveExprLongIdentInModuleOrNamespace (ncenv:NameResolver) nenv (typeNameResInfo: TypeNameResolutionInfo) ad resInfo depth m modref (mty:ModuleOrNamespaceType) (lid :Ident list) =
    // resInfo records the modules or namespaces actually relevant to a resolution
    match lid with 
    | [] -> raze(Error(FSComp.SR.nrUnexpectedEmptyLongId(),m))
    | id :: rest ->
        let m = unionRanges m id.idRange
        match mty.AllValsByLogicalName.TryFind(id.idText) with
        | Some vspec when IsValAccessible ad (mkNestedValRef modref vspec) -> 
            success(resInfo,Item.Value (mkNestedValRef modref vspec),rest)
        | _->
        match mty.ExceptionDefinitionsByDemangledName.TryFind(id.idText) with
        | Some excon when IsTyconReprAccessible ncenv.amap m ad (modref.NestedTyconRef excon) -> 
            success (resInfo,Item.ExnCase (modref.NestedTyconRef excon),rest)
        | _ ->
            // Something in a discriminated union without RequireQualifiedAccess attribute?
            let unionSearch,hasRequireQualifiedAccessAttribute =
                match TryFindTypeWithUnionCase modref id with
                | Some tycon when IsTyconReprAccessible ncenv.amap m ad (modref.NestedTyconRef tycon) -> 
                    let ucref = mkUnionCaseRef (modref.NestedTyconRef tycon) id.idText
                    let ucinfo = FreshenUnionCaseRef ncenv m ucref
                    let hasRequireQualifiedAccessAttribute = HasFSharpAttribute ncenv.g ncenv.g.attrib_RequireQualifiedAccessAttribute tycon.Attribs
                    success [resInfo,Item.UnionCase(ucinfo,hasRequireQualifiedAccessAttribute),rest],hasRequireQualifiedAccessAttribute
                | _ -> NoResultsOrUsefulErrors,false

            match unionSearch with
            | Result (res :: _) when not hasRequireQualifiedAccessAttribute -> success res
            | _ ->

            // Something in a type?
            let tyconSearch = 
                let tcrefs = LookupTypeNameInEntityMaybeHaveArity (ncenv.amap, id.idRange, ad, id.idText, (if isNil rest then typeNameResInfo.StaticArgsInfo else TypeNameResolutionStaticArgsInfo.Indefinite), modref)
                if isNil tcrefs then NoResultsOrUsefulErrors else
                let tcrefs = tcrefs |> List.map (fun tcref -> (resInfo,tcref))
                if not (isNil rest) then 
                    let tcrefs = CheckForTypeLegitimacyAndMultipleGenericTypeAmbiguities (tcrefs, TypeNameResolutionInfo (ResolveTypeNamesToTypeRefs,TypeNameResolutionStaticArgsInfo.Indefinite), PermitDirectReferenceToGeneratedType.No, unionRanges m id.idRange)
                    ResolveLongIdentInTyconRefs ResultCollectionSettings.AtMostOneResult ncenv nenv LookupKind.Expr (depth+1) m ad rest typeNameResInfo id.idRange tcrefs
                // Check if we've got some explicit type arguments 
                else 
                    let tcrefs = CheckForTypeLegitimacyAndMultipleGenericTypeAmbiguities (tcrefs, typeNameResInfo, PermitDirectReferenceToGeneratedType.No, unionRanges m id.idRange)
                    match typeNameResInfo.ResolutionFlag with 
                    | ResolveTypeNamesToTypeRefs -> 
                        success [ for (resInfo,tcref) in tcrefs do 
                                      let typ = FreshenTycon ncenv m tcref
                                      let item = (resInfo,Item.Types(id.idText,[typ]),[])
                                      yield item ]
                    | ResolveTypeNamesToCtors -> 
                        tcrefs 
                        |> List.map (fun (resInfo, tcref) -> resInfo, FreshenTycon ncenv m tcref) 
                        |> CollectAtMostOneResult (fun (resInfo,typ) -> ResolveObjectConstructorPrim ncenv nenv.eDisplayEnv resInfo id.idRange ad typ) 
                        |> MapResults (fun (resInfo,item) -> (resInfo,item,[]))

            match tyconSearch with
            | Result (res :: _) -> success res
            | _ ->

            // Something in a sub-namespace or sub-module 
            let moduleSearch = 
                if not (isNil rest) then 
                    match mty.ModulesAndNamespacesByDemangledName.TryFind(id.idText) with
                    | Some(AccessibleEntityRef ncenv.amap m ad modref submodref) -> 
                        let resInfo = resInfo.AddEntity(id.idRange,submodref)

                        OneResult (ResolveExprLongIdentInModuleOrNamespace ncenv nenv typeNameResInfo ad resInfo (depth+1) m submodref submodref.ModuleOrNamespaceType rest)
                    | _ -> 
                        NoResultsOrUsefulErrors
                else 
                    NoResultsOrUsefulErrors

            match tyconSearch +++ moduleSearch +++ unionSearch with
            | Result [] ->
                let suggestPossibleTypesAndNames() =
                    let types = 
                        modref.ModuleOrNamespaceType.AllEntities
                        |> Seq.filter (fun e -> IsEntityAccessible ncenv.amap m ad (modref.NestedTyconRef e))
                        |> Seq.map (fun e -> e.DisplayName)
                        |> Set.ofSeq

                    let submodules =
                        mty.ModulesAndNamespacesByDemangledName
                        |> Seq.filter (fun kv -> IsEntityAccessible ncenv.amap m ad (modref.NestedTyconRef kv.Value))
                        |> Seq.map (fun e -> e.Value.DisplayName)
                        |> Set.ofSeq
                        
                    let unions =
                        modref.ModuleOrNamespaceType.AllEntities
                        |> Seq.collect (fun tycon ->
                            let hasRequireQualifiedAccessAttribute = HasFSharpAttribute ncenv.g ncenv.g.attrib_RequireQualifiedAccessAttribute tycon.Attribs
                            if hasRequireQualifiedAccessAttribute then
                                [||]
                            else
                                tycon.UnionCasesArray)
                        |> Seq.map (fun uc -> uc.DisplayName)
                        |> Set.ofSeq

                    let vals = 
                        modref.ModuleOrNamespaceType.AllValsByLogicalName
                        |> Seq.filter (fun e -> IsValAccessible ad (mkNestedValRef modref e.Value))
                        |> Seq.map (fun e -> e.Value.DisplayName)
                        |> Set.ofSeq
                         
                    let exns =
                        modref.ModuleOrNamespaceType.ExceptionDefinitionsByDemangledName
                        |> Seq.filter (fun e -> IsTyconReprAccessible ncenv.amap m ad (modref.NestedTyconRef e.Value))
                        |> Seq.map (fun e -> e.Value.DisplayName)
                        |> Set.ofSeq
                            
                    types
                    |> Set.union submodules
                    |> Set.union unions
                    |> Set.union vals
                    |> Set.union exns

                raze (UndefinedName(depth,FSComp.SR.undefinedNameValueConstructorNamespaceOrType,id,suggestPossibleTypesAndNames))
            | results -> AtMostOneResult id.idRange results

/// An identifier has resolved to a type name in an expression (corresponding to one or more TyconRefs). 
/// Return either a set of constructors (later refined by overload resolution), or a set of TyconRefs.
let ChooseTyconRefInExpr (ncenv:NameResolver, m, ad, nenv, id:Ident, typeNameResInfo:TypeNameResolutionInfo, resInfo:ResolutionInfo, tcrefs) =
    let tcrefs = tcrefs |> List.map (fun tcref -> (resInfo,tcref))
    let tcrefs = CheckForTypeLegitimacyAndMultipleGenericTypeAmbiguities (tcrefs, typeNameResInfo, PermitDirectReferenceToGeneratedType.No, m)
    match typeNameResInfo.ResolutionFlag with 
    | ResolveTypeNamesToCtors ->
        let typs = tcrefs |> List.map (fun (resInfo,tcref) -> (resInfo,FreshenTycon ncenv m tcref)) 
        typs 
            |> CollectAtMostOneResult (fun (resInfo,typ) -> ResolveObjectConstructorPrim ncenv nenv.eDisplayEnv resInfo id.idRange ad typ) 
            |> MapResults (fun (resInfo,item) -> (resInfo,item,[]))
    | ResolveTypeNamesToTypeRefs ->
        let typs = tcrefs |> List.map (fun (resInfo,tcref) -> (resInfo,FreshenTycon ncenv m tcref)) 
        success (typs |> List.map (fun (resInfo,typ) -> (resInfo,Item.Types(id.idText,[typ]),[])))

/// Resolve F# "A.B.C" syntax in expressions
/// Not all of the sequence will necessarily be swallowed, i.e. we return some identifiers 
/// that may represent further actions, e.g. further lookups.
let rec ResolveExprLongIdentPrim sink (ncenv:NameResolver) fullyQualified m ad nenv (typeNameResInfo:TypeNameResolutionInfo) lid =
    let resInfo = ResolutionInfo.Empty
    match lid with 
    | [] -> error (Error(FSComp.SR.nrInvalidExpression(textOfLid lid), m))

    | [id] when id.idText = MangledGlobalName ->
         error (Error(FSComp.SR.nrGlobalUsedOnlyAsFirstName(), id.idRange))
         
    | [id;next] when id.idText = MangledGlobalName ->
          ResolveExprLongIdentPrim sink ncenv fullyQualified m ad nenv typeNameResInfo [next]

    | id :: lid when id.idText = MangledGlobalName ->
          ResolveExprLongIdentPrim sink ncenv FullyQualified m ad nenv typeNameResInfo lid

    | [id] when fullyQualified <> FullyQualified ->
          let typeError = ref None
          // Single identifier.  Lookup the unqualified names in the environment
          let envSearch = 
              match nenv.eUnqualifiedItems.TryFind(id.idText) with

              // The name is a type name and it has not been clobbered by some other name
              | Some (Item.UnqualifiedType tcrefs) -> 
                  
                  // Do not use type names from the environment if an explicit type instantiation is 
                  // given and the number of type parameters do not match
                  let tcrefs = 
                      tcrefs |> List.filter (fun tcref ->
                          typeNameResInfo.StaticArgsInfo.HasNoStaticArgsInfo || 
                          typeNameResInfo.StaticArgsInfo.NumStaticArgs = tcref.Typars(m).Length)
                  
                  let search = ChooseTyconRefInExpr (ncenv, m, ad, nenv, id, typeNameResInfo, resInfo, tcrefs)
                  match AtMostOneResult m search with 
                  | Result _ as res -> 
                      let resInfo,item,rest = ForceRaise res
                      ResolutionInfo.SendEntityPathToSink(sink,ncenv,nenv,ItemOccurence.Use,ad,resInfo,ResultTyparChecker(fun () -> CheckAllTyparsInferrable ncenv.amap m item))
                      Some(item,rest)
                  | Exception e -> typeError := Some e; None

              | Some res -> 
                  Some (FreshenUnqualifiedItem ncenv m res, [])
              | None -> 
                  None

          match envSearch with 
          | Some res -> res
          | None ->
              let innerSearch =
                  // Check if it's a type name, e.g. a constructor call or a type instantiation
                  let ctorSearch = 
                      let tcrefs = LookupTypeNameInEnvMaybeHaveArity fullyQualified id.idText typeNameResInfo nenv
                      ChooseTyconRefInExpr (ncenv, m, ad, nenv, id, typeNameResInfo, resInfo, tcrefs)

                  match ctorSearch with
                  | Result res when not (isNil res) -> ctorSearch
                  | _ -> 

                  let implicitOpSearch = 
                      if IsMangledOpName id.idText then 
                          success [(resInfo,Item.ImplicitOp(id, ref None),[])] 
                      else 
                          NoResultsOrUsefulErrors

                  ctorSearch +++ implicitOpSearch

              let resInfo,item,rest = 
                  match AtMostOneResult m innerSearch with
                  | Result _ as res -> ForceRaise res
                  | _ -> 
                      let failingCase = 
                          match !typeError with
                          | Some e -> raze e
                          | _ ->
                              let suggestNamesAndTypes() =
                                  let suggestedNames =
                                      nenv.eUnqualifiedItems
                                      |> Seq.map (fun e -> e.Value.DisplayName)
                                      |> Set.ofSeq

                                  let suggestedTypes =
                                      nenv.TyconsByDemangledNameAndArity fullyQualified
                                      |> Seq.filter (fun e -> IsEntityAccessible ncenv.amap m ad e.Value)
                                      |> Seq.map (fun e -> e.Value.DisplayName)
                                      |> Set.ofSeq

                                  let suggestedModulesAndNamespaces =
                                      nenv.ModulesAndNamespaces fullyQualified
                                      |> Seq.collect (fun kv -> kv.Value)
                                      |> Seq.filter (fun modref -> IsEntityAccessible ncenv.amap m ad modref)
                                      |> Seq.collect (fun e -> [e.DisplayName; e.DemangledModuleOrNamespaceName])
                                      |> Set.ofSeq

                                  let unions =
                                      // check if the user forgot to use qualified access
                                      nenv.eTyconsByDemangledNameAndArity
                                      |> Seq.choose (fun e ->
                                          let hasRequireQualifiedAccessAttribute = HasFSharpAttribute ncenv.g ncenv.g.attrib_RequireQualifiedAccessAttribute e.Value.Attribs
                                          if not hasRequireQualifiedAccessAttribute then 
                                              None
                                          else
                                              if e.Value.IsUnionTycon && e.Value.UnionCasesArray |> Array.exists (fun c -> c.DisplayName = id.idText) then
                                                  Some e.Value
                                              else
                                                  None)
                                      |> Seq.map (fun t -> t.DisplayName + "." + id.idText)
                                      |> Set.ofSeq
                                
                                  suggestedNames
                                  |> Set.union suggestedTypes
                                  |> Set.union suggestedModulesAndNamespaces
                                  |> Set.union unions

                              raze (UndefinedName(0,FSComp.SR.undefinedNameValueOfConstructor,id,suggestNamesAndTypes))
                      ForceRaise failingCase

              ResolutionInfo.SendEntityPathToSink(sink,ncenv,nenv,ItemOccurence.Use,ad,resInfo,ResultTyparChecker(fun () -> CheckAllTyparsInferrable ncenv.amap m item))
              item,rest
              
            
    // A compound identifier. 
    // It still might be a value in the environment, or something in an F# module, namespace, type, or nested type 
    | id :: rest -> 
    
        let m = unionRanges m id.idRange
        // Values in the environment take total priority, but constructors do NOT for compound lookups, e.g. if someone in some imported  
        // module has defined a constructor "String" (common enough) then "String.foo" doesn't give an error saying 'constructors have no members' 
        // Instead we go lookup the String module or type.
        let ValIsInEnv nm = 
            match fullyQualified with 
            | FullyQualified -> false
            | _ -> 
                match nenv.eUnqualifiedItems.TryFind(nm) with 
                | Some(Item.Value _) -> true 
                | _ -> false

        if ValIsInEnv id.idText then
          nenv.eUnqualifiedItems.[id.idText], rest
        else
          // Otherwise modules are searched first. REVIEW: modules and types should be searched together. 
          // For each module referenced by 'id', search the module as if it were an F# module and/or a .NET namespace. 
          let moduleSearch ad = 
               ResolveLongIndentAsModuleOrNamespaceThen ResultCollectionSettings.AtMostOneResult ncenv.amap m fullyQualified nenv ad lid 
                   (ResolveExprLongIdentInModuleOrNamespace ncenv nenv typeNameResInfo ad)

          // REVIEW: somewhat surprisingly, this shows up on performance traces, with tcrefs non-nil.
          // This seems strange since we would expect in the vast majority of cases tcrefs is empty here.
          let tyconSearch ad = 
              let tcrefs = LookupTypeNameInEnvNoArity fullyQualified id.idText nenv
              if isNil tcrefs then NoResultsOrUsefulErrors else
              let tcrefs = tcrefs |> List.map (fun tcref -> (resInfo,tcref))
              let tcrefs = CheckForTypeLegitimacyAndMultipleGenericTypeAmbiguities (tcrefs, TypeNameResolutionInfo.ResolveToTypeRefs (TypeNameResolutionStaticArgsInfo.Indefinite), PermitDirectReferenceToGeneratedType.No, unionRanges m id.idRange)
              ResolveLongIdentInTyconRefs ResultCollectionSettings.AtMostOneResult ncenv nenv LookupKind.Expr 1 m ad rest typeNameResInfo id.idRange tcrefs

          let search =
              let moduleSearch = moduleSearch ad 
              
              match moduleSearch with
              | Result res when not (isNil res) -> moduleSearch
              | _ ->

              let tyconSearch = tyconSearch ad

              match tyconSearch with
              | Result res when not (isNil res) -> tyconSearch
              | _ ->

              let envSearch = 
                  match fullyQualified with 
                  | FullyQualified -> 
                      NoResultsOrUsefulErrors
                  | OpenQualified -> 
                      match nenv.eUnqualifiedItems.TryFind id.idText with
                      | Some (Item.UnqualifiedType _) 
                      | None -> NoResultsOrUsefulErrors
                      | Some res -> OneSuccess (resInfo,FreshenUnqualifiedItem ncenv m res,rest)

              moduleSearch +++ tyconSearch +++ envSearch

          let resInfo,item,rest = 
              match AtMostOneResult m search with 
              | Result _ as res -> ForceRaise res
              | _ ->
                  let innerSearch =
                      let moduleSearch = moduleSearch AccessibleFromSomeFSharpCode
              
                      match moduleSearch with
                      | Result res when not (isNil res) -> moduleSearch
                      | _ ->

                      let tyconSearch = tyconSearch AccessibleFromSomeFSharpCode

                      match tyconSearch with
                      | Result res when not (isNil res) -> tyconSearch
                      | _ ->

                      search +++ moduleSearch +++ tyconSearch

                  let suggestEverythingInScope() =
                      let suggestedModulesAndNamespaces =
                          nenv.ModulesAndNamespaces fullyQualified
                          |> Seq.collect (fun kv -> kv.Value)
                          |> Seq.filter (fun modref -> IsEntityAccessible ncenv.amap m ad modref)
                          |> Seq.collect (fun e -> [e.DisplayName; e.DemangledModuleOrNamespaceName])
                          |> Set.ofSeq
                      
                      let suggestedTypes =
                          nenv.TyconsByDemangledNameAndArity fullyQualified
                          |> Seq.filter (fun e -> IsEntityAccessible ncenv.amap m ad e.Value)
                          |> Seq.map (fun e -> e.Value.DisplayName)
                          |> Set.ofSeq

                      let suggestedNames =
                          nenv.eUnqualifiedItems
                          |> Seq.map (fun e -> e.Value.DisplayName)
                          |> Set.ofSeq
                      
                      suggestedNames
                      |> Set.union suggestedTypes
                      |> Set.union suggestedModulesAndNamespaces

                  match innerSearch with
                  | Exception (UndefinedName(0,_,id1,suggestionsF)) when id.idRange = id1.idRange ->
                        let mergeSuggestions() =
                            suggestEverythingInScope()
                            |> Set.union (suggestionsF())

                        let failingCase = raze (UndefinedName(0,FSComp.SR.undefinedNameValueNamespaceTypeOrModule,id,mergeSuggestions))
                        ForceRaise failingCase
                  | Exception err -> ForceRaise(Exception err)
                  | Result (res :: _) -> ForceRaise(Result res)
                  | Result [] ->
                        let failingCase = raze (UndefinedName(0,FSComp.SR.undefinedNameValueNamespaceTypeOrModule,id,suggestEverythingInScope))
                        ForceRaise failingCase

          ResolutionInfo.SendEntityPathToSink(sink,ncenv,nenv,ItemOccurence.Use,ad,resInfo,ResultTyparChecker(fun () -> CheckAllTyparsInferrable ncenv.amap m item))
          item,rest

let ResolveExprLongIdent sink (ncenv:NameResolver) m ad nenv typeNameResInfo lid =
    ResolveExprLongIdentPrim sink ncenv OpenQualified m ad nenv typeNameResInfo lid 

//-------------------------------------------------------------------------
// Resolve F#/IL "." syntax in patterns
//------------------------------------------------------------------------- 

let rec ResolvePatternLongIdentInModuleOrNamespace (ncenv:NameResolver) nenv numTyArgsOpt ad resInfo depth m modref (mty:ModuleOrNamespaceType) (lid: Ident list) =
    match lid with 
    | [] -> raze (InternalError("ResolvePatternLongIdentInModuleOrNamespace",m))
    | id :: rest ->
        let m = unionRanges m id.idRange
        match TryFindTypeWithUnionCase modref id with
        | Some tycon when IsTyconReprAccessible ncenv.amap m ad (modref.NestedTyconRef tycon) -> 
            let tcref = modref.NestedTyconRef tycon
            let ucref = mkUnionCaseRef tcref id.idText
            let showDeprecated = HasFSharpAttribute ncenv.g ncenv.g.attrib_RequireQualifiedAccessAttribute tycon.Attribs
            let ucinfo = FreshenUnionCaseRef ncenv m ucref
            success (resInfo,Item.UnionCase(ucinfo,showDeprecated),rest)
        | _ -> 
        match mty.ExceptionDefinitionsByDemangledName.TryFind(id.idText) with
        | Some exnc when IsEntityAccessible ncenv.amap m ad (modref.NestedTyconRef exnc) -> 
            success (resInfo,Item.ExnCase (modref.NestedTyconRef exnc),rest)
        | _ ->
        // An active pattern constructor in a module 
        match (ActivePatternElemsOfModuleOrNamespace modref).TryFind(id.idText) with
        | Some ( APElemRef(_,vref,_) as apref) when IsValAccessible ad vref -> 
            success (resInfo,Item.ActivePatternCase apref,rest)
        | _ -> 
        match mty.AllValsByLogicalName.TryFind(id.idText) with
        | Some vspec  when IsValAccessible ad (mkNestedValRef modref vspec) -> 
            success(resInfo,Item.Value (mkNestedValRef modref vspec),rest)
        | _ ->
        let tcrefs = lazy (
            LookupTypeNameInEntityMaybeHaveArity (ncenv.amap, id.idRange, ad, id.idText, TypeNameResolutionStaticArgsInfo.Indefinite, modref)
            |> List.map (fun tcref -> (resInfo,tcref)))

        // Something in a type? e.g. a literal field 
        let tyconSearch = 
            match lid with 
            | _ :: rest when not (isNil rest) ->
                let tcrefs = tcrefs.Force()
                ResolveLongIdentInTyconRefs ResultCollectionSettings.AtMostOneResult (ncenv:NameResolver) nenv LookupKind.Pattern (depth+1) m ad rest numTyArgsOpt id.idRange tcrefs
            | _ -> 
                NoResultsOrUsefulErrors

        match tyconSearch with
        | Result (res :: _) -> success res
        | _ -> 

        // Constructor of a type? 
        let ctorSearch = 
            if isNil rest then
                tcrefs.Force()
                |> List.map (fun (resInfo,tcref) -> (resInfo,FreshenTycon ncenv m tcref)) 
                |> CollectAtMostOneResult (fun (resInfo,typ) -> ResolveObjectConstructorPrim ncenv nenv.eDisplayEnv resInfo id.idRange ad typ) 
                |> MapResults (fun (resInfo,item) -> (resInfo,item,[]))
            else
                NoResultsOrUsefulErrors

        match ctorSearch with
        | Result (res :: _) -> success res
        | _ -> 

        // Something in a sub-namespace or sub-module or nested-type 
        let moduleSearch = 
            if not (isNil rest) then 
                match mty.ModulesAndNamespacesByDemangledName.TryFind(id.idText) with
                | Some(AccessibleEntityRef ncenv.amap m ad modref submodref) -> 
                    let resInfo = resInfo.AddEntity(id.idRange,submodref)
                    OneResult (ResolvePatternLongIdentInModuleOrNamespace ncenv nenv numTyArgsOpt ad resInfo (depth+1) m submodref submodref.ModuleOrNamespaceType rest)
                | _ -> 
                    NoResultsOrUsefulErrors
             else NoResultsOrUsefulErrors

        match tyconSearch +++ ctorSearch +++ moduleSearch with
        | Result [] -> 
            let suggestPossibleTypes() =
                let submodules =
                    mty.ModulesAndNamespacesByDemangledName
                    |> Seq.filter (fun kv -> IsEntityAccessible ncenv.amap m ad (modref.NestedTyconRef kv.Value))
                    |> Seq.collect (fun e -> [e.Value.DisplayName; e.Value.DemangledModuleOrNamespaceName])
                    |> Set.ofSeq
                    
                let suggestedTypes =
                    nenv.TyconsByDemangledNameAndArity FullyQualifiedFlag.OpenQualified
                    |> Seq.filter (fun e -> IsEntityAccessible ncenv.amap m ad e.Value)
                    |> Seq.map (fun e -> e.Value.DisplayName)
                    |> Set.ofSeq

                Set.union submodules suggestedTypes

            raze (UndefinedName(depth,FSComp.SR.undefinedNameConstructorModuleOrNamespace,id,suggestPossibleTypes))
        | results -> AtMostOneResult id.idRange results
        
/// Used to report a warning condition for the use of upper-case identifiers in patterns
exception UpperCaseIdentifierInPattern of range

/// Indicates if a warning should be given for the use of upper-case identifiers in patterns
type WarnOnUpperFlag = WarnOnUpperCase | AllIdsOK

// Long ID in a pattern 
let rec ResolvePatternLongIdentPrim sink (ncenv:NameResolver) fullyQualified warnOnUpper newDef m ad nenv numTyArgsOpt (lid:Ident list) =
    match lid with 

    | [id] when id.idText = MangledGlobalName ->
         error (Error(FSComp.SR.nrGlobalUsedOnlyAsFirstName(), id.idRange))
         
    | id :: lid when id.idText = MangledGlobalName ->
        ResolvePatternLongIdentPrim sink ncenv FullyQualified warnOnUpper newDef m ad nenv numTyArgsOpt lid

    // Single identifiers in patterns 
    | [id] when fullyQualified <> FullyQualified ->
          // Single identifiers in patterns - bind to constructors and active patterns 
          // For the special case of 
          //   let C = x 
          match nenv.ePatItems.TryFind(id.idText) with
          | Some res when not newDef  -> FreshenUnqualifiedItem ncenv m res
          | _ -> 
          // Single identifiers in patterns - variable bindings 
          if not newDef &&
             (warnOnUpper = WarnOnUpperCase) && 
             id.idText.Length >= 3 && 
             System.Char.ToLowerInvariant id.idText.[0] <> id.idText.[0] then 
            warning(UpperCaseIdentifierInPattern(m))
          Item.NewDef id
        
    // Long identifiers in patterns 
    | _ -> 
        let moduleSearch ad = 
            ResolveLongIndentAsModuleOrNamespaceThen ResultCollectionSettings.AtMostOneResult ncenv.amap m fullyQualified nenv ad lid 
                (ResolvePatternLongIdentInModuleOrNamespace ncenv nenv numTyArgsOpt ad)
        let tyconSearch ad = 
            match lid with 
            | tn :: rest when not (isNil rest) ->
                let tcrefs = LookupTypeNameInEnvNoArity fullyQualified tn.idText nenv
                if isNil tcrefs then NoResultsOrUsefulErrors else
                let tcrefs = tcrefs |> List.map (fun tcref -> (ResolutionInfo.Empty,tcref))
                ResolveLongIdentInTyconRefs ResultCollectionSettings.AtMostOneResult ncenv nenv LookupKind.Pattern 1 tn.idRange ad rest numTyArgsOpt tn.idRange tcrefs 
            | _ -> 
                NoResultsOrUsefulErrors
        let resInfo,res,rest = 
            match AtMostOneResult m (tyconSearch ad +++ moduleSearch ad) with 
            | Result _ as res -> ForceRaise res
            | _ ->  
                ForceRaise (AtMostOneResult m (tyconSearch AccessibleFromSomeFSharpCode +++ moduleSearch AccessibleFromSomeFSharpCode))
        ResolutionInfo.SendEntityPathToSink(sink,ncenv,nenv,ItemOccurence.Use,ad,resInfo,ResultTyparChecker(fun () -> true))
  
        if not (isNil rest) then error(Error(FSComp.SR.nrIsNotConstructorOrLiteral(),(List.head rest).idRange))
        res


/// Resolve a long identifier when used in a pattern.
let ResolvePatternLongIdent sink (ncenv:NameResolver) warnOnUpper newDef m ad nenv numTyArgsOpt (lid:Ident list) =
    ResolvePatternLongIdentPrim sink ncenv OpenQualified warnOnUpper newDef m ad nenv numTyArgsOpt lid

//-------------------------------------------------------------------------
// Resolve F#/IL "." syntax in types
//------------------------------------------------------------------------- 

/// Resolve nested types referenced through a .NET abbreviation.
//
// Note the generic case is not supported by F#, so 
//    type X = List<int>
//
// X.ListEnumerator // does not resolve
//
let ResolveNestedTypeThroughAbbreviation (ncenv:NameResolver) (tcref: TyconRef) m =
    if tcref.IsTypeAbbrev && tcref.Typars(m).IsEmpty && isAppTy ncenv.g tcref.TypeAbbrev.Value && isNil (argsOfAppTy ncenv.g tcref.TypeAbbrev.Value) then 
        tcrefOfAppTy ncenv.g tcref.TypeAbbrev.Value
    else
        tcref

/// Resolve a long identifier representing a type name
let rec ResolveTypeLongIdentInTyconRefPrim (ncenv:NameResolver) (typeNameResInfo:TypeNameResolutionInfo) ad resInfo genOk depth m (tcref: TyconRef) (lid: Ident list) =
    let tcref = ResolveNestedTypeThroughAbbreviation ncenv tcref m
    match lid with 
    | [] -> error(Error(FSComp.SR.nrUnexpectedEmptyLongId(),m))
    | [id] -> 
#if EXTENSIONTYPING
        // No dotting through type generators to get to a nested type!
        CheckForDirectReferenceToGeneratedType (tcref, PermitDirectReferenceToGeneratedType.No, m)
#endif
        let m = unionRanges m id.idRange
        let tcrefs = LookupTypeNameInEntityMaybeHaveArity (ncenv.amap, id.idRange, ad, id.idText, typeNameResInfo.StaticArgsInfo, tcref)
        let tcrefs = tcrefs |> List.map (fun tcref -> (resInfo,tcref))
        let tcrefs = CheckForTypeLegitimacyAndMultipleGenericTypeAmbiguities (tcrefs, typeNameResInfo, genOk, m) 
        match tcrefs with 
        | tcref :: _ -> success tcref
        | [] ->
            let suggestTypes() =
                tcref.ModuleOrNamespaceType.TypesByDemangledNameAndArity id.idRange
                |> Seq.map (fun e -> e.Value.DisplayName)
                |> Set.ofSeq

            raze (UndefinedName(depth,FSComp.SR.undefinedNameType,id,suggestTypes))
    | id::rest ->
#if EXTENSIONTYPING
        // No dotting through type generators to get to a nested type!
        CheckForDirectReferenceToGeneratedType (tcref, PermitDirectReferenceToGeneratedType.No, m)
#endif
        let m = unionRanges m id.idRange
        // Search nested types
        let tyconSearch = 
            let tcrefs = LookupTypeNameInEntityMaybeHaveArity (ncenv.amap, id.idRange, ad, id.idText, TypeNameResolutionStaticArgsInfo.Indefinite, tcref)
            if isNil tcrefs then NoResultsOrUsefulErrors else
            let tcrefs = tcrefs |> List.map (fun tcref -> (resInfo,tcref))
            let tcrefs = CheckForTypeLegitimacyAndMultipleGenericTypeAmbiguities (tcrefs, typeNameResInfo.DropStaticArgsInfo, genOk, m)
            match tcrefs with 
            | _ :: _ -> tcrefs |> CollectAtMostOneResult (fun (resInfo,tcref) -> ResolveTypeLongIdentInTyconRefPrim ncenv typeNameResInfo ad resInfo genOk (depth+1) m tcref rest)
            | [] -> 
                let suggestTypes() =
                    tcref.ModuleOrNamespaceType.TypesByDemangledNameAndArity id.idRange
                    |> Seq.map (fun e -> e.Value.DisplayName)
                    |> Set.ofSeq

                raze (UndefinedName(depth,FSComp.SR.undefinedNameType,id,suggestTypes))
            
        AtMostOneResult m tyconSearch

/// Resolve a long identifier representing a type name and report the result
let ResolveTypeLongIdentInTyconRef sink (ncenv:NameResolver) nenv typeNameResInfo ad m tcref (lid: Ident list) =
    let resInfo,tcref = ForceRaise (ResolveTypeLongIdentInTyconRefPrim ncenv typeNameResInfo ad ResolutionInfo.Empty PermitDirectReferenceToGeneratedType.No 0 m tcref lid)
    ResolutionInfo.SendEntityPathToSink(sink,ncenv,nenv,ItemOccurence.Use,ad,resInfo,ResultTyparChecker(fun () -> true))
    let item = Item.Types(tcref.DisplayName,[FreshenTycon ncenv m tcref])
    CallNameResolutionSink sink (rangeOfLid lid,nenv,item,item,emptyTyparInst,ItemOccurence.UseInType,nenv.eDisplayEnv,ad)
    tcref

/// Create an UndefinedName error with details 
let SuggestTypeLongIdentInModuleOrNamespace depth (modref:ModuleOrNamespaceRef) amap ad m (id:Ident) =
    let suggestPossibleTypes() =
        modref.ModuleOrNamespaceType.AllEntities
        |> Seq.filter (fun e -> IsEntityAccessible amap m ad (modref.NestedTyconRef e))
        |> Seq.collect (fun e -> [e.DisplayName; e.DemangledModuleOrNamespaceName])
        |> Set.ofSeq

    let errorTextF s = FSComp.SR.undefinedNameTypeIn(s,fullDisplayTextOfModRef modref)    
    UndefinedName(depth,errorTextF,id,suggestPossibleTypes)

/// Resolve a long identifier representing a type in a module or namespace
let rec private ResolveTypeLongIdentInModuleOrNamespace (ncenv:NameResolver) (typeNameResInfo: TypeNameResolutionInfo) ad genOk (resInfo:ResolutionInfo) depth m modref _mty (lid: Ident list) =
    match lid with 
    | [] -> error(Error(FSComp.SR.nrUnexpectedEmptyLongId(),m))
    | [id] -> 
        // On all paths except error reporting we have isSome(staticResInfo), hence get at most one result back 
        let tcrefs = LookupTypeNameInEntityMaybeHaveArity (ncenv.amap, id.idRange, ad, id.idText, typeNameResInfo.StaticArgsInfo, modref)
        match tcrefs with 
        | _ :: _ -> tcrefs |> CollectResults (fun tcref -> success(resInfo,tcref))
        | [] -> raze (SuggestTypeLongIdentInModuleOrNamespace depth modref ncenv.amap ad m id)
    | id::rest ->
        let m = unionRanges m id.idRange
        let modulSearch = 
            match modref.ModuleOrNamespaceType.ModulesAndNamespacesByDemangledName.TryFind(id.idText) with
            | Some(AccessibleEntityRef ncenv.amap m ad modref submodref) -> 
                let resInfo = resInfo.AddEntity(id.idRange,submodref)
                ResolveTypeLongIdentInModuleOrNamespace ncenv typeNameResInfo ad genOk resInfo (depth+1) m submodref submodref.ModuleOrNamespaceType rest
            | _ ->
                let suggestPossibleModules() =
                    modref.ModuleOrNamespaceType.ModulesAndNamespacesByDemangledName
                    |> Seq.filter (fun kv -> IsEntityAccessible ncenv.amap m ad (modref.NestedTyconRef kv.Value))
                    |> Seq.collect (fun e -> [e.Value.DisplayName; e.Value.DemangledModuleOrNamespaceName])
                    |> Set.ofSeq
                raze (UndefinedName(depth,FSComp.SR.undefinedNameNamespaceOrModule,id,suggestPossibleModules))

        let tyconSearch = 
            let tcrefs = LookupTypeNameInEntityMaybeHaveArity (ncenv.amap, id.idRange, ad, id.idText, TypeNameResolutionStaticArgsInfo.Indefinite, modref)
            match tcrefs with 
            | _ :: _ -> tcrefs |> CollectResults (fun tcref -> ResolveTypeLongIdentInTyconRefPrim ncenv typeNameResInfo ad resInfo genOk (depth+1) m tcref rest)
            | [] ->
                let suggestTypes() =
                    modref.ModuleOrNamespaceType.TypesByDemangledNameAndArity id.idRange
                    |> Seq.map (fun e -> e.Value.DisplayName)
                    |> Set.ofSeq

                raze (UndefinedName(depth,FSComp.SR.undefinedNameType,id,suggestTypes))
        tyconSearch +++ modulSearch

/// Resolve a long identifier representing a type 
let rec ResolveTypeLongIdentPrim (ncenv:NameResolver) occurence fullyQualified m nenv ad (lid: Ident list) (staticResInfo: TypeNameResolutionStaticArgsInfo) genOk =
    let typeNameResInfo = TypeNameResolutionInfo.ResolveToTypeRefs staticResInfo
    match lid with 
    | [] -> error(Error(FSComp.SR.nrUnexpectedEmptyLongId(),m))

    | [id] when id.idText = MangledGlobalName ->
         error (Error(FSComp.SR.nrGlobalUsedOnlyAsFirstName(), id.idRange))
         
    | id :: lid when id.idText = MangledGlobalName ->
        ResolveTypeLongIdentPrim ncenv occurence FullyQualified m nenv ad lid staticResInfo genOk

    | [id]  ->  
        match LookupTypeNameInEnvHaveArity fullyQualified id.idText staticResInfo.NumStaticArgs nenv with
        | Some res -> 
            let res = CheckForTypeLegitimacyAndMultipleGenericTypeAmbiguities ([(ResolutionInfo.Empty,res)], typeNameResInfo, genOk, unionRanges m id.idRange)
            assert (res.Length = 1)
            success res.Head
        | None -> 
            // For Good Error Reporting! 
            let tcrefs = LookupTypeNameInEnvNoArity fullyQualified id.idText nenv
            match tcrefs with
            | tcref :: _tcrefs -> 
                // Note: This path is only for error reporting
                //CheckForTypeLegitimacyAndMultipleGenericTypeAmbiguities tcref rest typeNameResInfo m
                success(ResolutionInfo.Empty,tcref)
            | [] -> 
                let suggestPossibleTypes() =
                    nenv.TyconsByDemangledNameAndArity(fullyQualified)
                    |> Seq.filter (fun kv -> IsEntityAccessible ncenv.amap m ad kv.Value)
                    |> Seq.collect (fun e -> 
                        match occurence with
                        | ItemOccurence.UseInAttribute -> 
                            [yield e.Value.DisplayName
                             yield e.Value.DemangledModuleOrNamespaceName
                             if e.Value.DisplayName.EndsWith "Attribute" then
                                 yield e.Value.DisplayName.Replace("Attribute","")]
                        | _ -> [e.Value.DisplayName; e.Value.DemangledModuleOrNamespaceName])
                    |> Set.ofSeq

                raze (UndefinedName(0,FSComp.SR.undefinedNameType,id,suggestPossibleTypes))

    | id::rest ->
        let m = unionRanges m id.idRange
        let tyconSearch = 
            match fullyQualified with 
            | FullyQualified ->
                NoResultsOrUsefulErrors
            | OpenQualified -> 
                match LookupTypeNameInEnvHaveArity fullyQualified id.idText staticResInfo.NumStaticArgs nenv with
                | Some tcref when IsEntityAccessible ncenv.amap m ad tcref -> 
                    OneResult (ResolveTypeLongIdentInTyconRefPrim ncenv typeNameResInfo ad ResolutionInfo.Empty genOk 1 m tcref rest)
                | _ -> 
                    NoResultsOrUsefulErrors

        let modulSearch = 
            ResolveLongIndentAsModuleOrNamespaceThen ResultCollectionSettings.AllResults ncenv.amap m fullyQualified nenv ad lid 
                (ResolveTypeLongIdentInModuleOrNamespace ncenv typeNameResInfo ad genOk)
            |?> List.concat 

        let modulSearchFailed() = 
            ResolveLongIndentAsModuleOrNamespaceThen ResultCollectionSettings.AllResults ncenv.amap m fullyQualified nenv AccessibleFromSomeFSharpCode lid 
                (ResolveTypeLongIdentInModuleOrNamespace ncenv typeNameResInfo.DropStaticArgsInfo AccessibleFromSomeFSharpCode genOk)
            |?> List.concat 

        let searchSoFar = tyconSearch +++ modulSearch

        match searchSoFar with 
        | Result results -> 
            // NOTE: we delay checking the CheckForTypeLegitimacyAndMultipleGenericTypeAmbiguities condition until right at the end after we've
            // collected all possible resolutions of the type
            let tcrefs = CheckForTypeLegitimacyAndMultipleGenericTypeAmbiguities (results, typeNameResInfo, genOk, rangeOfLid lid)
            match tcrefs with 
            | (resInfo,tcref) :: _ -> 
                // We've already reported the ambiguity, possibly as an error. Now just take the first possible result.
                success(resInfo,tcref)
            | [] -> 
                // failing case - report nice ambiguity errors even in this case
                AtMostOneResult m ((searchSoFar +++ modulSearchFailed()) |?> (fun tcrefs -> CheckForTypeLegitimacyAndMultipleGenericTypeAmbiguities (tcrefs, typeNameResInfo, genOk, rangeOfLid lid)))
            
        | _ ->  
            // failing case - report nice ambiguity errors even in this case
            AtMostOneResult m ((searchSoFar +++ modulSearchFailed()) |?> (fun tcrefs -> CheckForTypeLegitimacyAndMultipleGenericTypeAmbiguities (tcrefs, typeNameResInfo, genOk, rangeOfLid lid)))


/// Resolve a long identifier representing a type and report it
let ResolveTypeLongIdent sink (ncenv:NameResolver) occurence fullyQualified nenv ad (lid: Ident list) staticResInfo genOk =
    let m = rangeOfLid lid
    let res = ResolveTypeLongIdentPrim ncenv occurence fullyQualified m nenv ad lid staticResInfo genOk 
    // Register the result as a name resolution
    match res with 
    | Result (resInfo,tcref) -> 
        ResolutionInfo.SendEntityPathToSink(sink,ncenv,nenv,ItemOccurence.UseInType, ad,resInfo,ResultTyparChecker(fun () -> true))
        let item = Item.Types(tcref.DisplayName,[FreshenTycon ncenv m tcref])
        CallNameResolutionSink sink (m,nenv,item,item,emptyTyparInst,occurence,nenv.eDisplayEnv,ad)
    | _ -> ()
    res |?> snd

//-------------------------------------------------------------------------
// Resolve F#/IL "." syntax in records etc.
//------------------------------------------------------------------------- 

/// Resolve a long identifier representing a record field in a module or namespace
let rec ResolveFieldInModuleOrNamespace (ncenv:NameResolver) nenv ad (resInfo:ResolutionInfo) depth m (modref: ModuleOrNamespaceRef) _mty (lid: Ident list) = 
    let typeNameResInfo = TypeNameResolutionInfo.Default
    match lid with 
    | id::rest -> 
        let m = unionRanges m id.idRange
        // search for module-qualified names, e.g. { Microsoft.FSharp.Core.contents = 1 } 
        let modulScopedFieldNames = 
            match TryFindTypeWithRecdField modref id  with
            | Some tycon when IsEntityAccessible ncenv.amap m ad (modref.NestedTyconRef tycon) -> 
                let showDeprecated = HasFSharpAttribute ncenv.g ncenv.g.attrib_RequireQualifiedAccessAttribute tycon.Attribs
                success [resInfo, FieldResolution(modref.RecdFieldRefInNestedTycon tycon id,showDeprecated), rest]
            | _ -> raze (UndefinedName(depth,FSComp.SR.undefinedNameRecordLabelOrNamespace,id,NoSuggestions))

        match modulScopedFieldNames with
        | Result (res :: _) -> success res
        | _ -> 

        // search for type-qualified names, e.g. { Microsoft.FSharp.Core.Ref.contents = 1 } 
        let tyconSearch = 
            match lid with 
            | _tn:: rest when not (isNil rest) ->
                let tcrefs = LookupTypeNameInEntityMaybeHaveArity (ncenv.amap, id.idRange, ad, id.idText, TypeNameResolutionStaticArgsInfo.Indefinite, modref)
                if isNil tcrefs then NoResultsOrUsefulErrors else
                let tcrefs = tcrefs |> List.map (fun tcref -> (ResolutionInfo.Empty,tcref))
                let tyconSearch = ResolveLongIdentInTyconRefs ResultCollectionSettings.AllResults ncenv nenv LookupKind.RecdField  (depth+1) m ad rest typeNameResInfo id.idRange tcrefs
                // choose only fields 
                let tyconSearch = tyconSearch |?> List.choose (function (resInfo,Item.RecdField(RecdFieldInfo(_,rfref)),rest) -> Some(resInfo,FieldResolution(rfref,false),rest) | _ -> None)
                tyconSearch
            | _ -> 
                NoResultsOrUsefulErrors

        match tyconSearch with
        | Result (res :: _) -> success res
        | _ -> 

        // search for names in nested modules, e.g. { Microsoft.FSharp.Core.contents = 1 } 
        let modulSearch = 
            if not (isNil rest) then 
                match modref.ModuleOrNamespaceType.ModulesAndNamespacesByDemangledName.TryFind(id.idText) with
                | Some(AccessibleEntityRef ncenv.amap m ad modref submodref) -> 
                    let resInfo = resInfo.AddEntity(id.idRange,submodref)
                    ResolveFieldInModuleOrNamespace ncenv nenv ad resInfo (depth+1) m submodref submodref.ModuleOrNamespaceType rest
                    |> OneResult
                | _ -> raze (UndefinedName(depth,FSComp.SR.undefinedNameRecordLabelOrNamespace,id,NoSuggestions))
            else raze (UndefinedName(depth,FSComp.SR.undefinedNameRecordLabelOrNamespace,id,NoSuggestions))

        AtMostOneResult m (modulScopedFieldNames +++ tyconSearch +++ modulSearch)
    | [] -> 
        error(InternalError("ResolveFieldInModuleOrNamespace",m))

/// Suggest other labels of the same record
let SuggestOtherLabelsOfSameRecordType g (nenv:NameResolutionEnv) typ (id:Ident) (allFields:Ident list) =    
    let labelsOfPossibleRecord = GetRecordLabelsForType g nenv typ

    let givenFields = 
        allFields 
        |> List.map (fun fld -> fld.idText) 
        |> List.filter ((<>) id.idText)
        |> Set.ofList

    Set.difference labelsOfPossibleRecord givenFields
    
      
let SuggestLabelsOfRelatedRecords g (nenv:NameResolutionEnv) (id:Ident) (allFields:Ident list) =
    let suggestLabels() =
        let givenFields = allFields |> List.map (fun fld -> fld.idText) |> List.filter ((<>) id.idText) |> Set.ofList
        let fullyQualfied =
            if Set.isEmpty givenFields then 
                // return labels from all records
                NameMap.domainL nenv.eFieldLabels |> Set.ofList |> Set.remove "contents"
            else
                let possibleRecords =
                    [for fld in givenFields do
                        match Map.tryFind fld nenv.eFieldLabels with
                        | None -> ()
                        | Some recordTypes -> yield! (recordTypes |> List.map (fun r -> r.TyconRef.DisplayName, fld)) ]
                    |> List.groupBy fst
                    |> List.map (fun (r,fields) -> r, fields |> List.map snd |> Set.ofList)
                    |> List.filter (fun (_,fields) -> Set.isSubset givenFields fields)
                    |> List.map fst
                    |> Set.ofList

                let labelsOfPossibleRecords =
                    nenv.eFieldLabels
                    |> Seq.filter (fun kv -> 
                        kv.Value 
                        |> List.map (fun r -> r.TyconRef.DisplayName)
                        |> List.exists possibleRecords.Contains)
                    |> Seq.map (fun kv -> kv.Key)
                    |> Set.ofSeq

                Set.difference labelsOfPossibleRecords givenFields
        
        if not (Set.isEmpty fullyQualfied) then fullyQualfied else

        // check if the user forgot to use qualified access
        nenv.eTyconsByDemangledNameAndArity
        |> Seq.choose (fun e ->
            let hasRequireQualifiedAccessAttribute = HasFSharpAttribute g g.attrib_RequireQualifiedAccessAttribute e.Value.Attribs
            if not hasRequireQualifiedAccessAttribute then 
                None
            else
                if e.Value.IsRecordTycon && e.Value.AllFieldsArray |> Seq.exists (fun x -> x.Name = id.idText) then
                    Some e.Value
                else
                    None)
        |> Seq.map (fun t -> t.DisplayName + "." + id.idText)
        |> Set.ofSeq

    UndefinedName(0,FSComp.SR.undefinedNameRecordLabel, id, suggestLabels)

/// Resolve a long identifier representing a record field 
let ResolveFieldPrim (ncenv:NameResolver) nenv ad typ (mp,id:Ident) allFields =
    let typeNameResInfo = TypeNameResolutionInfo.Default
    let g = ncenv.g
    let m = id.idRange
    match mp with 
    | [] -> 
        let lookup() =
            let frefs = 
                try Map.find id.idText nenv.eFieldLabels 
                with :? KeyNotFoundException ->
                    // record label is unknown -> suggest related labels and give a hint to the user
                    error(SuggestLabelsOfRelatedRecords g nenv id allFields)

            // Eliminate duplicates arising from multiple 'open' 
            frefs 
            |> ListSet.setify (fun fref1 fref2 -> tyconRefEq g fref1.TyconRef fref2.TyconRef)
            |> List.map (fun x -> ResolutionInfo.Empty, FieldResolution(x,false))

        if isAppTy g typ then 
            match ncenv.InfoReader.TryFindRecdOrClassFieldInfoOfType(id.idText,m,typ) with
            | Some (RecdFieldInfo(_,rfref)) -> [ResolutionInfo.Empty, FieldResolution(rfref,false)]
            | None ->
                if isRecdTy g typ then
                    // record label doesn't belong to record type -> suggest other labels of same record
                    let suggestLabels() = SuggestOtherLabelsOfSameRecordType g nenv typ id allFields
                    let typeName = NicePrint.minimalStringOfType nenv.eDisplayEnv typ
                    let errorText = FSComp.SR.nrRecordDoesNotContainSuchLabel(typeName,id.idText)
                    error(ErrorWithSuggestions(errorText, m, id.idText, suggestLabels))
                else
                    lookup()
        else 
            lookup()            
    | _ -> 
        let lid = (mp@[id])
        let tyconSearch ad = 
            match lid with 
            | tn:: (_ :: _ as rest) -> 
                let m = tn.idRange
                let tcrefs = LookupTypeNameInEnvNoArity OpenQualified tn.idText nenv
                if isNil tcrefs then NoResultsOrUsefulErrors else
                let tcrefs = tcrefs |> List.map (fun tcref -> (ResolutionInfo.Empty,tcref))
                let tyconSearch = ResolveLongIdentInTyconRefs ResultCollectionSettings.AllResults ncenv nenv LookupKind.RecdField 1 m ad rest typeNameResInfo tn.idRange tcrefs
                // choose only fields 
                let tyconSearch = tyconSearch |?> List.choose (function (resInfo,Item.RecdField(RecdFieldInfo(_,rfref)),rest) -> Some(resInfo,FieldResolution(rfref,false),rest) | _ -> None)
                tyconSearch
            | _ -> NoResultsOrUsefulErrors

        let modulSearch ad = 
            ResolveLongIndentAsModuleOrNamespaceThen ResultCollectionSettings.AtMostOneResult ncenv.amap m OpenQualified nenv ad lid 
                (ResolveFieldInModuleOrNamespace ncenv nenv ad)

        let search =
            let moduleSearch1 = modulSearch ad
        
            match moduleSearch1 with
            | Result (res :: _) -> success res
            | _ -> 

            let tyconSearch1 = tyconSearch ad

            match tyconSearch1 with
            | Result (res :: _) -> success res
            | _ -> 

            let moduleSearch2 = modulSearch AccessibleFromSomeFSharpCode

            match moduleSearch2 with
            | Result (res :: _) -> success res
            | _ -> 

            let tyconSearch2 = tyconSearch AccessibleFromSomeFSharpCode

            AtMostOneResult m (moduleSearch1 +++ tyconSearch1 +++ moduleSearch2 +++ tyconSearch2)

        let resInfo,item,rest = ForceRaise search
        if not (isNil rest) then 
            errorR(Error(FSComp.SR.nrInvalidFieldLabel(),(List.head rest).idRange))

        [(resInfo,item)]

let ResolveField sink ncenv nenv ad typ (mp,id) allFields =
    let res = ResolveFieldPrim ncenv nenv ad typ (mp,id) allFields
    // Register the results of any field paths "Module.Type" in "Module.Type.field" as a name resolution. (Note, the path resolution
    // info is only non-empty if there was a unique resolution of the field)
    let checker = ResultTyparChecker(fun () -> true)
    res 
    |> List.map (fun (resInfo,rfref) ->
        ResolutionInfo.SendEntityPathToSink(sink,ncenv,nenv,ItemOccurence.UseInType,ad,resInfo,checker)
        rfref)

/// Generate a new reference to a record field with a fresh type instantiation
let FreshenRecdFieldRef (ncenv:NameResolver) m (rfref:RecdFieldRef) =
    Item.RecdField(RecdFieldInfo(ncenv.InstantiationGenerator m (rfref.Tycon.Typars m), rfref))



/// Resolve F#/IL "." syntax in expressions (2).
///
/// We have an expr. on the left, and we do an access, e.g. 
/// (f obj).field or (f obj).meth.  The basic rule is that if l-r type 
/// inference has determined the outer type then we can proceed in a simple fashion. The exception 
/// to the rule is for field types, which applies if l-r was insufficient to 
/// determine any valid members 
//
// QUERY (instantiationGenerator cleanup): it would be really nice not to flow instantiationGenerator to here. 
let private ResolveExprDotLongIdent (ncenv:NameResolver) m ad nenv typ lid findFlag =
    let typeNameResInfo = TypeNameResolutionInfo.Default
    let adhoctDotSearchAccessible = AtMostOneResult m (ResolveLongIdentInTypePrim ncenv nenv LookupKind.Expr ResolutionInfo.Empty 1 m ad lid findFlag typeNameResInfo typ)
    match adhoctDotSearchAccessible with 
    | Exception _ ->
        // If the dot is not resolved by adhoc overloading then look for a record field 
        // that can resolve the name. 
        let dotFieldIdSearch = 
            match lid with 
            // A unique record label access, e.g  expr.field  
            | id::rest when nenv.eFieldLabels.ContainsKey(id.idText) -> 
                match nenv.eFieldLabels.[id.idText] with
                | [] -> NoResultsOrUsefulErrors
                | rfref :: _ ->
                    // NOTE (instantiationGenerator cleanup): we need to freshen here because we don't know the type. 
                    // But perhaps the caller should freshen?? 
                    let item = FreshenRecdFieldRef ncenv m rfref
                    OneSuccess (ResolutionInfo.Empty,item,rest)
            | _ -> NoResultsOrUsefulErrors 
        
        let search = dotFieldIdSearch 
        match AtMostOneResult m search with 
        | Result _ as res -> ForceRaise res
        | _ -> 
            let adhocDotSearchAll = ResolveLongIdentInTypePrim ncenv nenv LookupKind.Expr ResolutionInfo.Empty 1 m AccessibleFromSomeFSharpCode lid findFlag typeNameResInfo typ 
            ForceRaise (AtMostOneResult m (search +++ adhocDotSearchAll))

    | Result _ -> 
        ForceRaise adhoctDotSearchAccessible

let ComputeItemRange wholem (lid: Ident list) rest =
    match rest with
    | [] -> wholem
    | _ -> 
        let ids = List.take (max 0 (lid.Length - rest.Length)) lid
        match ids with 
        | [] -> wholem
        | _ -> rangeOfLid ids

/// Filters method groups that will be sent to Visual Studio IntelliSense
/// to include only static/instance members

let FilterMethodGroups (ncenv:NameResolver) itemRange item staticOnly =
    match item with
    | Item.MethodGroup(nm, minfos, orig) -> 
        let minfos = minfos |> List.filter  (fun minfo -> 
           staticOnly = isNil (minfo.GetObjArgTypes(ncenv.amap, itemRange, minfo.FormalMethodInst)))
        Item.MethodGroup(nm, minfos, orig)
    | item -> item

let NeedsWorkAfterResolution namedItem =
    match namedItem with
    | Item.MethodGroup(_,minfos,_) 
    | Item.CtorGroup(_,minfos) -> minfos.Length > 1 || minfos |> List.exists (fun minfo -> not (isNil minfo.FormalMethodInst))
    | Item.Property(_,pinfos) -> pinfos.Length > 1
    | Item.ImplicitOp(_, { contents = Some(TraitConstraintSln.FSMethSln(_, vref, _)) }) 
    | Item.Value vref | Item.CustomBuilder (_,vref) -> vref.Typars.Length > 0
    | Item.CustomOperation (_,_,Some minfo) -> not (isNil minfo.FormalMethodInst)
    | Item.ActivePatternCase apref -> apref.ActivePatternVal.Typars.Length > 0
    | _ -> false

/// Specifies additional work to do after an item has been processed further in type checking.
[<RequireQualifiedAccess>]
type AfterResolution =

    /// Notification is not needed
    | DoNothing

    /// Notify the tcSink of a precise resolution. The 'Item' contains the candidate overrides.
    | RecordResolution of Item option * (TyparInst -> unit) * (MethInfo * PropInfo option * TyparInst -> unit) * (unit -> unit)

/// Resolve a long identifier occurring in an expression position.
///
/// Called for 'TypeName.Bar' - for VS IntelliSense, we can filter out instance members from method groups
let ResolveLongIdentAsExprAndComputeRange (sink:TcResultsSink) (ncenv:NameResolver) wholem ad nenv typeNameResInfo lid = 
    let item1,rest = ResolveExprLongIdent sink ncenv wholem ad nenv typeNameResInfo lid
    let itemRange = ComputeItemRange wholem lid rest
    
    let item = FilterMethodGroups ncenv itemRange item1 true

    match item1,item with
    | Item.MethodGroup(name, minfos1, _), Item.MethodGroup(_, [], _) when not (isNil minfos1) -> 
        error(Error(FSComp.SR.methodIsNotStatic(name),wholem))
    | _ -> ()

    // Fake idents e.g. 'Microsoft.FSharp.Core.None' have identical ranges for each part
    let isFakeIdents =
        match lid with
        | [] | [_] -> false
        | head :: ids ->
            ids |> List.forall (fun id -> id.idRange = head.idRange)

    let callSink (refinedItem, tpinst) =
        if not isFakeIdents then
            let occurence = 
                match item with
                // It's r.h.s. `Case1` in `let (|Case1|Case1|) _ = if true then Case1 else Case2`
                // We return `Binding` for it because it's actually not usage, but definition. If we did not
                // it confuses detecting unused definitions.
                | Item.ActivePatternResult _ -> ItemOccurence.Binding 
                | _ -> ItemOccurence.Use

            CallNameResolutionSink sink (itemRange, nenv, refinedItem, item, tpinst, occurence, nenv.DisplayEnv, ad)

    let callSinkWithSpecificOverload (minfo: MethInfo, pinfoOpt: PropInfo option, tpinst) =
        let refinedItem = 
            match pinfoOpt with 
            | None when minfo.IsConstructor -> Item.CtorGroup(minfo.LogicalName,[minfo])
            | None -> Item.MethodGroup(minfo.LogicalName,[minfo],None)
            | Some pinfo -> Item.Property(pinfo.PropertyName,[pinfo])

        callSink (refinedItem, tpinst)

    let afterResolution =
        match sink.CurrentSink with
        | None -> AfterResolution.DoNothing
        | Some _ ->
            if NeedsWorkAfterResolution item then
                AfterResolution.RecordResolution(None, (fun tpinst -> callSink(item,tpinst)), callSinkWithSpecificOverload, (fun () -> callSink (item, emptyTyparInst)))
            else
               callSink (item, emptyTyparInst)
               AfterResolution.DoNothing

    item, itemRange, rest, afterResolution

let (|NonOverridable|_|) namedItem =
    match namedItem with
    |   Item.MethodGroup(_,minfos,_) when minfos |> List.exists(fun minfo -> minfo.IsVirtual || minfo.IsAbstract) -> None
    |   Item.Property(_,pinfos) when pinfos |> List.exists(fun pinfo -> pinfo.IsVirtualProperty) -> None
    |   _ -> Some ()



/// Called for 'expression.Bar' - for VS IntelliSense, we can filter out static members from method groups
/// Also called for 'GenericType<Args>.Bar' - for VS IntelliSense, we can filter out non-static members from method groups
let ResolveExprDotLongIdentAndComputeRange (sink:TcResultsSink) (ncenv:NameResolver) wholem ad nenv typ lid findFlag thisIsActuallyATyAppNotAnExpr = 
    let resolveExpr findFlag =
        let resInfo,item,rest = ResolveExprDotLongIdent ncenv wholem ad nenv typ lid findFlag
        let itemRange = ComputeItemRange wholem lid rest
        resInfo,item,rest,itemRange
    // "true" resolution
    let resInfo,item,rest,itemRange = resolveExpr findFlag 
    ResolutionInfo.SendEntityPathToSink(sink,ncenv,nenv,ItemOccurence.Use,ad,resInfo,ResultTyparChecker(fun () -> CheckAllTyparsInferrable ncenv.amap itemRange item))
    
    // Record the precise resolution of the field for intellisense/goto definition
    let afterResolution =
        match sink.CurrentSink with 
        | None -> AfterResolution.DoNothing // do not refine the resolution if nobody listens
        | Some _ ->
            // resolution for goto definition
            let unrefinedItem,itemRange,overrides = 
                match findFlag, item with
                | FindMemberFlag.PreferOverrides, _ 
                | _, NonOverridable() -> item,itemRange,false
                | FindMemberFlag.IgnoreOverrides,_ -> 
                    let _,item,_,itemRange = resolveExpr FindMemberFlag.PreferOverrides                
                    item, itemRange,true

            let callSink (refinedItem, tpinst) = 
                let staticOnly = thisIsActuallyATyAppNotAnExpr
                let refinedItem = FilterMethodGroups ncenv itemRange refinedItem staticOnly
                let unrefinedItem = FilterMethodGroups ncenv itemRange unrefinedItem staticOnly
                CallNameResolutionSink sink (itemRange, nenv, refinedItem, unrefinedItem, tpinst, ItemOccurence.Use, nenv.DisplayEnv, ad)                                

            let callSinkWithSpecificOverload (minfo: MethInfo, pinfoOpt: PropInfo option, tpinst) =
                let refinedItem = 
                    match pinfoOpt with 
                    | None when minfo.IsConstructor -> Item.CtorGroup(minfo.LogicalName,[minfo])
                    | None -> Item.MethodGroup(minfo.LogicalName,[minfo],None)
                    | Some pinfo -> Item.Property(pinfo.PropertyName,[pinfo])

                callSink (refinedItem, tpinst)

            match overrides, NeedsWorkAfterResolution unrefinedItem with
            | false, true -> 
                AfterResolution.RecordResolution (None, (fun tpinst -> callSink(item,tpinst)), callSinkWithSpecificOverload, (fun () -> callSink (unrefinedItem, emptyTyparInst)))
            | true, true  -> 
                AfterResolution.RecordResolution (Some unrefinedItem, (fun tpinst -> callSink(item,tpinst)), callSinkWithSpecificOverload, (fun () -> callSink (unrefinedItem, emptyTyparInst)))
            | _ , false   -> 
                callSink (unrefinedItem, emptyTyparInst)
                AfterResolution.DoNothing

    item, itemRange, rest, afterResolution


//-------------------------------------------------------------------------
// Given an nenv resolve partial paths to sets of names, used by interactive
// environments (Visual Studio)
//
// ptc = partial type check
// ptci = partial type check item
//
// There are some inefficiencies in this code - e.g. we often 
// create potentially large lists of methods/fields/properties and then
// immediately List.filter them.  We also use lots of "map/concats".  Doesn't
// seem to hit the interactive experience too badly though.
//------------------------------------------------------------------------- 

/// A generator of type instantiations used when no more specific type instantiation is known.
let FakeInstantiationGenerator (_m:range) gps = List.map mkTyparTy gps 

// note: using local refs is ok since it is only used by VS 
let ItemForModuleOrNamespaceRef v = Item.ModuleOrNamespaces [v]
let ItemForPropInfo (pinfo:PropInfo) = Item.Property (pinfo.PropertyName, [pinfo])

let IsTyconUnseenObsoleteSpec ad g amap m (x:TyconRef) allowObsolete = 
    not (IsEntityAccessible amap m ad x) ||
    ((not allowObsolete) &&
      (if x.IsILTycon then 
          CheckILAttributesForUnseen g x.ILTyconRawMetadata.CustomAttrs m
       else 
          CheckFSharpAttributesForUnseen g x.Attribs m))

let IsTyconUnseen ad g amap m (x:TyconRef) = IsTyconUnseenObsoleteSpec ad g amap m x false

let IsValUnseen ad g m (v:ValRef) = 
    v.IsCompilerGenerated ||
    v.Deref.IsClassConstructor ||
    not (IsValAccessible ad v) ||
    CheckFSharpAttributesForUnseen g v.Attribs m

let IsUnionCaseUnseen ad g amap m (ucref:UnionCaseRef) = 
    not (IsUnionCaseAccessible amap m ad ucref) ||
    IsTyconUnseen ad g amap m ucref.TyconRef || 
    CheckFSharpAttributesForUnseen g ucref.Attribs m

let ItemIsUnseen ad g amap m item = 
    match item with 
    | Item.Value x -> IsValUnseen ad  g m x
    | Item.UnionCase(x,_) -> IsUnionCaseUnseen ad g amap m x.UnionCaseRef
    | Item.ExnCase x -> IsTyconUnseen ad g amap m x
    | _ -> false

let ItemOfTyconRef ncenv m (x:TyconRef) = 
    Item.Types (x.DisplayName,[FreshenTycon ncenv m x])

let ItemOfTy g x = 
    let nm = if isAppTy g x then (tcrefOfAppTy g x).DisplayName else "?"
    Item.Types (nm,[x])

// Filter out 'PrivateImplementationDetail' classes 
let IsInterestingModuleName nm =
    String.length nm >= 1 &&
    String.sub nm 0 1 <> "<"

let rec PartialResolveLookupInModuleOrNamespaceAsModuleOrNamespaceThen f plid (modref:ModuleOrNamespaceRef) =
    let mty = modref.ModuleOrNamespaceType
    match plid with 
    | [] -> f modref
    | id:: rest -> 
        match mty.ModulesAndNamespacesByDemangledName.TryFind(id) with
        | Some mty -> PartialResolveLookupInModuleOrNamespaceAsModuleOrNamespaceThen f rest (modref.NestedTyconRef mty) 
        | None -> []

let PartialResolveLongIndentAsModuleOrNamespaceThen (nenv:NameResolutionEnv) plid f =
    match plid with 
    | id:: rest -> 
        match Map.tryFind id nenv.eModulesAndNamespaces with
        | Some modrefs -> 
            List.collect (PartialResolveLookupInModuleOrNamespaceAsModuleOrNamespaceThen f rest) modrefs
        | None ->
            []
    | [] -> []

/// Returns fields for the given class or record
let ResolveRecordOrClassFieldsOfType (ncenv: NameResolver) m ad typ statics = 
    ncenv.InfoReader.GetRecordOrClassFieldsOfType(None,ad,m,typ)
    |> List.filter (fun rfref -> rfref.IsStatic = statics && IsFieldInfoAccessible ad rfref)
    |> List.map Item.RecdField

[<RequireQualifiedAccess>]
type ResolveCompletionTargets =
    | All of (MethInfo -> TType -> bool)
    | SettablePropertiesAndFields
    member this.ResolveAll = 
        match this with
        | All _ -> true
        | SettablePropertiesAndFields -> false

/// Resolve a (possibly incomplete) long identifier to a set of possible resolutions, qualified by type.
let ResolveCompletionsInType (ncenv: NameResolver) nenv (completionTargets: ResolveCompletionTargets) m ad statics typ =
    let g = ncenv.g
    let amap = ncenv.amap
    
    let rfinfos = 
        ncenv.InfoReader.GetRecordOrClassFieldsOfType(None,ad,m,typ)
        |> List.filter (fun rfref -> rfref.IsStatic = statics && IsFieldInfoAccessible ad rfref)

    let ucinfos = 
        if completionTargets.ResolveAll && statics && isAppTy g typ then 
            let tc,tinst = destAppTy g typ
            tc.UnionCasesAsRefList 
            |> List.filter (IsUnionCaseUnseen ad g ncenv.amap m >> not)
            |> List.map (fun ucref ->  Item.UnionCase(UnionCaseInfo(tinst,ucref),false))
        else []

    let einfos = 
        if completionTargets.ResolveAll then
            ncenv.InfoReader.GetEventInfosOfType(None,ad,m,typ)
            |> List.filter (fun x -> 
                IsStandardEventInfo ncenv.InfoReader m ad x &&
                x.IsStatic = statics)
        else []

    let nestedTypes = 
        if completionTargets.ResolveAll && statics then
            typ
            |> GetNestedTypesOfType (ad, ncenv, None, TypeNameResolutionStaticArgsInfo.Indefinite, false, m) 
        else 
            []

    let finfos = 
        ncenv.InfoReader.GetILFieldInfosOfType(None,ad,m,typ)
        |> List.filter (fun x -> 
            not x.IsSpecialName &&
            x.IsStatic = statics && 
            IsILFieldInfoAccessible g amap m ad x)
    let pinfosIncludingUnseen = 
        AllPropInfosOfTypeInScope ncenv.InfoReader nenv (None,ad) PreferOverrides m typ
        |> List.filter (fun x -> 
            x.IsStatic = statics && 
            IsPropInfoAccessible g amap m ad x)

    // Exclude get_ and set_ methods accessed by properties 
    let pinfoMethNames = 
      (pinfosIncludingUnseen 
       |> List.filter (fun pinfo -> pinfo.HasGetter)
       |> List.map (fun pinfo -> pinfo.GetterMethod.LogicalName))
      @
      (pinfosIncludingUnseen 
       |> List.filter (fun pinfo -> pinfo.HasSetter)
       |> List.map (fun pinfo -> pinfo.SetterMethod.LogicalName))
    
    let einfoMethNames = 
        if completionTargets.ResolveAll then
            [ for einfo in einfos do 
                let delegateType = einfo.GetDelegateType(amap,m)
                let (SigOfFunctionForDelegate(invokeMethInfo,_,_,_)) = GetSigOfFunctionForDelegate ncenv.InfoReader delegateType m ad 
                // Only events with void return types are suppressed in intellisense.
                if slotSigHasVoidReturnTy (invokeMethInfo.GetSlotSig(amap, m)) then 
                  yield einfo.GetAddMethod().DisplayName
                  yield einfo.GetRemoveMethod().DisplayName ]
        else []

    let suppressedMethNames = Zset.ofList String.order (pinfoMethNames @ einfoMethNames)

    let pinfos = 
        pinfosIncludingUnseen
        |> List.filter (fun x -> not (PropInfoIsUnseen m x))

    let minfoFilter (minfo:MethInfo) = 
        let isApplicableMeth =
            match completionTargets with
            | ResolveCompletionTargets.All x -> x
            | _ -> failwith "internal error: expected completionTargets = ResolveCompletionTargets.All"
        // Only show the Finalize, MemberwiseClose etc. methods on System.Object for values whose static type really is 
        // System.Object. Few of these are typically used from F#.  
        //
        // Don't show GetHashCode or Equals for F# types that admit equality as an abnormal operation
        let isUnseenDueToBasicObjRules = 
            not (isObjTy g typ) &&
            not minfo.IsExtensionMember &&
            match minfo.LogicalName with
            | "GetType"  -> false
            | "GetHashCode"  -> isObjTy g minfo.EnclosingType && not (AugmentWithHashCompare.TypeDefinitelyHasEquality g typ)
            | "ToString" -> false
            | "Equals" ->                 
                if not (isObjTy g minfo.EnclosingType) then 
                    // declaring type is not System.Object - show it
                    false 
                elif minfo.IsInstance then
                    // System.Object has only one instance Equals method and we want to suppress it unless Augment.TypeDefinitelyHasEquality is true
                    not (AugmentWithHashCompare.TypeDefinitelyHasEquality g typ)
                else
                    // System.Object has only one static Equals method and we always want to suppress it
                    true
            | _ -> 
                // filter out self methods of obj type
                isObjTy g minfo.EnclosingType
        let result = 
            not isUnseenDueToBasicObjRules &&
            not minfo.IsInstance = statics &&
            IsMethInfoAccessible amap m ad minfo &&
            not (MethInfoIsUnseen g m typ minfo) &&
            not minfo.IsConstructor &&
            not minfo.IsClassConstructor &&
            not (minfo.LogicalName = ".cctor") &&
            not (minfo.LogicalName = ".ctor") &&
            isApplicableMeth minfo typ &&
            not (suppressedMethNames.Contains minfo.LogicalName)
        result

    let pinfoItems = 
        let pinfos = 
            match completionTargets with
            | ResolveCompletionTargets.SettablePropertiesAndFields -> pinfos |> List.filter (fun p -> p.HasSetter)
            | _ -> pinfos

        pinfos
        |> List.choose (fun pinfo->
            let pinfoOpt = DecodeFSharpEvent [pinfo] ad g ncenv m
            match pinfoOpt, completionTargets with
            | Some(Item.Event(einfo)), ResolveCompletionTargets.All _ -> if IsStandardEventInfo ncenv.InfoReader m ad einfo then pinfoOpt else None
            | _ -> pinfoOpt)

    // REVIEW: add a name filter here in the common cases?
    let minfos = 
        if completionTargets.ResolveAll then
            let minfos =
                AllMethInfosOfTypeInScope ncenv.InfoReader nenv (None,ad) PreferOverrides m typ 
                |> List.filter minfoFilter

            let minfos = 
                let addersAndRemovers = 
                    pinfoItems 
                    |> List.collect (function Item.Event(FSEvent(_,_,addValRef,removeValRef)) -> [addValRef.LogicalName;removeValRef.LogicalName] | _ -> [])
                    |> set

                if addersAndRemovers.IsEmpty then minfos
                else minfos |> List.filter (fun minfo -> not (addersAndRemovers.Contains minfo.LogicalName))

#if EXTENSIONTYPING
            // Filter out the ones with mangled names from applying static parameters
            let minfos = 
                let methsWithStaticParams = 
                    minfos 
                    |> List.filter (fun minfo -> 
                        match minfo.ProvidedStaticParameterInfo with 
                        | Some (_methBeforeArguments, staticParams) -> staticParams.Length <> 0
                        | _ -> false)
                    |> List.map (fun minfo -> minfo.DisplayName)

                if methsWithStaticParams.IsEmpty then minfos
                else minfos |> List.filter (fun minfo -> 
                        let nm = minfo.LogicalName
                        not (nm.Contains "," && methsWithStaticParams |> List.exists (fun m -> nm.StartsWith(m))))
#endif

            minfos 

        else []
    // Partition methods into overload sets
    let rec partitionl (l:MethInfo list) acc = 
        match l with
        | [] -> acc
        | h::t -> 
            let nm = h.LogicalName
            partitionl t (NameMultiMap.add nm h acc)

    // Build the results
    ucinfos @
    List.map Item.RecdField rfinfos @
    pinfoItems @
    List.map Item.ILField finfos @
    List.map Item.Event einfos @
    List.map (ItemOfTy g) nestedTypes @
    List.map Item.MakeMethGroup (NameMap.toList (partitionl minfos Map.empty))
      

let rec ResolvePartialLongIdentInType (ncenv: NameResolver) nenv isApplicableMeth m ad statics plid typ =
    let g = ncenv.g
    let amap = ncenv.amap
    match plid with
    | [] -> ResolveCompletionsInType ncenv nenv isApplicableMeth m ad statics typ
    | id :: rest ->
  
      let rfinfos = 
        ncenv.InfoReader.GetRecordOrClassFieldsOfType(None,ad,m,typ)
        |> List.filter (fun fref -> IsRecdFieldAccessible ncenv.amap m ad fref.RecdFieldRef)
        |> List.filter (fun fref -> fref.RecdField.IsStatic = statics)
  
      let nestedTypes = 
          typ 
          |> GetNestedTypesOfType (ad, ncenv, Some id, TypeNameResolutionStaticArgsInfo.Indefinite, false, m)  

      // e.g. <val-id>.<recdfield-id>.<more> 
      (rfinfos |> List.filter (fun x -> x.Name = id)
               |> List.collect (fun x -> x.FieldType |> ResolvePartialLongIdentInType ncenv nenv isApplicableMeth m ad false rest)) @

      // e.g. <val-id>.<property-id>.<more> 
      let FullTypeOfPinfo(pinfo:PropInfo) = 
        let rty = pinfo.GetPropertyType(amap,m) 
        let rty = if pinfo.IsIndexer then mkRefTupledTy g (pinfo.GetParamTypes(amap, m)) --> rty else  rty 
        rty      
      (typ
         |> AllPropInfosOfTypeInScope ncenv.InfoReader nenv (Some id,ad) IgnoreOverrides m
         |> List.filter (fun x -> x.IsStatic = statics)
         |> List.filter (IsPropInfoAccessible g amap m ad) 
         |> List.collect (fun pinfo -> (FullTypeOfPinfo pinfo) |> ResolvePartialLongIdentInType ncenv nenv isApplicableMeth m ad false rest)) @

      // e.g. <val-id>.<event-id>.<more> 
      (ncenv.InfoReader.GetEventInfosOfType(Some id,ad,m,typ)
         |> List.collect (PropTypOfEventInfo ncenv.InfoReader m ad >> ResolvePartialLongIdentInType ncenv nenv isApplicableMeth m ad false rest)) @

      // nested types! 
      (nestedTypes 
         |> List.collect (ResolvePartialLongIdentInType ncenv nenv isApplicableMeth m ad statics rest)) @

      // e.g. <val-id>.<il-field-id>.<more> 
      (ncenv.InfoReader.GetILFieldInfosOfType(Some id,ad,m,typ)
         |> List.filter (fun x -> 
             not x.IsSpecialName &&
             x.IsStatic = statics && 
             IsILFieldInfoAccessible g amap m ad x)
         |> List.collect (fun x -> x.FieldType(amap,m) |> ResolvePartialLongIdentInType ncenv nenv isApplicableMeth m ad false rest))
     
let InfosForTyconConstructors (ncenv:NameResolver) m ad (tcref:TyconRef) = 
    let g = ncenv.g
    let amap = ncenv.amap
    // Don't show constructors for type abbreviations. See FSharp 1.0 bug 2881
    if tcref.IsTypeAbbrev then 
        []
    else 
        let typ = FreshenTycon ncenv m tcref
        match ResolveObjectConstructor ncenv (DisplayEnv.Empty g) m ad typ with 
        | Result item -> 
            match item with 
            | Item.FakeInterfaceCtor _ -> []
            | Item.CtorGroup(nm,ctorInfos) -> 
                let ctors = 
                    ctorInfos 
                    |> List.filter (IsMethInfoAccessible amap m ad)
                    |> List.filter (MethInfoIsUnseen g m typ >> not)
                match ctors with 
                | [] -> []
                | _ -> [Item.MakeCtorGroup(nm,ctors)]
            | item -> 
                [item]
        | Exception _ -> []

/// import.fs creates somewhat fake modules for nested members of types (so that 
/// types never contain other types) 
let notFakeContainerModule tyconNames nm = 
    not (Set.contains nm tyconNames)

/// Check is a namespace or module contains something accessible 
let rec private EntityRefContainsSomethingAccessible (ncenv: NameResolver) m ad (modref:ModuleOrNamespaceRef) = 
    let g = ncenv.g
    let mty = modref.ModuleOrNamespaceType

    // Search the values in the module for an accessible value 
    (mty.AllValsAndMembers
     |> Seq.exists (fun v -> 
         // This may explore assemblies that are not in the reference set,
         // e.g. for extension members that extend a type not in the reference set. 
         // In this case assume it is accessible. The user may later explore this module 
         // but will not see the extension members anyway.
         //
         // Note: this is the only use of protectAssemblyExplorationNoReraise.
         // REVIEW: consider changing this to protectAssemblyExploration. We shouldn't need
         // to catch arbitrary exceptions here.
         protectAssemblyExplorationNoReraise  true false
             (fun () -> 
                 let vref = mkNestedValRef modref v
                 not vref.IsCompilerGenerated && 
                 not (IsValUnseen ad g m vref) &&
                 (vref.IsExtensionMember || vref.MemberInfo.IsNone)))) ||

    // Search the types in the namespace/module for an accessible tycon 
    (mty.AllEntities
     |> QueueList.exists (fun tc ->  
          not tc.IsModuleOrNamespace && 
          not (IsTyconUnseen ad g ncenv.amap m (modref.NestedTyconRef tc)))) ||

    // Search the sub-modules of the namespace/module for something accessible 
    (mty.ModulesAndNamespacesByDemangledName 
     |> NameMap.exists (fun _ submod -> 
        let submodref = modref.NestedTyconRef submod
        EntityRefContainsSomethingAccessible ncenv m ad submodref)) 

let rec ResolvePartialLongIdentInModuleOrNamespace (ncenv: NameResolver) nenv isApplicableMeth m ad (modref:ModuleOrNamespaceRef) plid allowObsolete =
    let g = ncenv.g
    let mty = modref.ModuleOrNamespaceType
    
    match plid with 
    | [] -> 
         let tycons = 
             mty.TypeDefinitions
             |> List.filter (fun tcref -> not (tcref.LogicalName.Contains(",")))
             |> List.filter (fun tycon -> not (IsTyconUnseen ad g ncenv.amap m (modref.NestedTyconRef tycon)))

         let ilTyconNames = 
             mty.TypesByAccessNames.Values
             |> List.choose (fun (tycon:Tycon) -> if tycon.IsILTycon then Some tycon.DisplayName else None)
             |> Set.ofList

         // Collect up the accessible values in the module, excluding the members
         (mty.AllValsAndMembers
          |> Seq.toList
          |> List.choose (TryMkValRefInModRef modref) // if the assembly load set is incomplete and we get a None value here, then ignore the value
          |> List.filter (fun v -> v.MemberInfo.IsNone)
          |> List.filter (IsValUnseen ad g m >> not) 
          |> List.map Item.Value)

         // Collect up the accessible discriminated union cases in the module 
       @ (UnionCaseRefsInModuleOrNamespace modref 
          |> List.filter (IsUnionCaseUnseen ad g ncenv.amap m >> not)
          |> List.map (fun x -> Item.UnionCase(GeneralizeUnionCaseRef x,false)))

         // Collect up the accessible active patterns in the module 
       @ (ActivePatternElemsOfModuleOrNamespace modref 
          |> NameMap.range
          |> List.filter (fun apref -> apref.ActivePatternVal |> IsValUnseen ad g m |> not) 
          |> List.map Item.ActivePatternCase)


         // Collect up the accessible F# exception declarations in the module 
       @ (mty.ExceptionDefinitionsByDemangledName 
          |> NameMap.range 
          |> List.map modref.NestedTyconRef
          |> List.filter (IsTyconUnseen ad g ncenv.amap m >> not)
          |> List.map Item.ExnCase)

         // Collect up the accessible sub-modules 
       @ (mty.ModulesAndNamespacesByDemangledName 
          |> NameMap.range 
          |> List.filter (fun x -> 
                let demangledName = x.DemangledModuleOrNamespaceName
                notFakeContainerModule ilTyconNames demangledName && IsInterestingModuleName demangledName)
          |> List.map modref.NestedTyconRef
          |> List.filter (IsTyconUnseen ad g ncenv.amap m >> not)
          |> List.filter (EntityRefContainsSomethingAccessible ncenv m ad)
          |> List.map ItemForModuleOrNamespaceRef)

    // Get all the types and .NET constructor groups accessible from here 
       @ (tycons 
          |> List.map (modref.NestedTyconRef >> ItemOfTyconRef ncenv m) )

       @ (tycons 
          |> List.collect (modref.NestedTyconRef >> InfosForTyconConstructors ncenv m ad))

    | id :: rest  -> 

        (match mty.ModulesAndNamespacesByDemangledName.TryFind(id) with
         | Some mspec 
             when not (IsTyconUnseenObsoleteSpec ad g ncenv.amap m (modref.NestedTyconRef mspec) allowObsolete) -> 
             let allowObsolete = rest <> [] && allowObsolete
             ResolvePartialLongIdentInModuleOrNamespace ncenv nenv isApplicableMeth m ad (modref.NestedTyconRef mspec) rest allowObsolete
         | _ -> [])

      @ (LookupTypeNameInEntityNoArity m id modref.ModuleOrNamespaceType
         |> List.collect (fun tycon ->
             let tcref = modref.NestedTyconRef tycon 
             if not (IsTyconUnseenObsoleteSpec ad g ncenv.amap m tcref allowObsolete) then 
                 tcref |> generalizedTyconRef |> ResolvePartialLongIdentInType ncenv nenv isApplicableMeth m ad true rest
             else 
                 []))

/// Try to resolve a long identifier as type.
let TryToResolveLongIdentAsType (ncenv: NameResolver) (nenv: NameResolutionEnv) m (plid: string list) =
    let g = ncenv.g

    match List.tryLast plid with
    | Some id ->
        // Look for values called 'id' that accept the dot-notation 
        let typ, isItemVal = 
            (match nenv.eUnqualifiedItems |> Map.tryFind id with
               // v.lookup : member of a value
             | Some v ->
                 match v with 
                 | Item.Value x -> 
                     let typ = x.Type
                     let typ = if x.BaseOrThisInfo = CtorThisVal && isRefCellTy g typ then destRefCellTy g typ else typ
                     Some typ, true
                 | _ -> None, false
             | None -> None, false)
        
        if isItemVal then typ
        else
            LookupTypeNameInEnvNoArity OpenQualified id nenv
            |> List.fold (fun resTyp tcref ->
                // type.lookup : lookup a static something in a type 
                let tcref = ResolveNestedTypeThroughAbbreviation ncenv tcref m
                let typ = FreshenTycon ncenv m tcref
                let resTyp =
                    match resTyp with
                    | Some _ -> resTyp
                    | None -> Some typ
                resTyp) typ
    | _ -> None

/// allowObsolete - specifies whether we should return obsolete types & modules 
///   as (no other obsolete items are returned)
let rec ResolvePartialLongIdentPrim (ncenv: NameResolver) (nenv: NameResolutionEnv) isApplicableMeth fullyQualified m ad plid allowObsolete : Item list = 
    let g = ncenv.g

    match plid with
    |  id :: plid when id = "global" -> // this is deliberately not the mangled name

       ResolvePartialLongIdentPrim ncenv nenv isApplicableMeth FullyQualified m ad plid allowObsolete

    |  [] -> 
    
       let ilTyconNames =
          nenv.TyconsByAccessNames(fullyQualified).Values
          |> List.choose (fun tyconRef -> if tyconRef.IsILTycon then Some tyconRef.DisplayName else None)
          |> Set.ofList
       
       /// Include all the entries in the eUnqualifiedItems table. 
       let unqualifiedItems = 
           match fullyQualified with 
           | FullyQualified -> []
           | OpenQualified ->
               nenv.eUnqualifiedItems.Values
               |> List.filter (function Item.UnqualifiedType _ -> false | _ -> true)
               |> List.filter (ItemIsUnseen ad g ncenv.amap m >> not)

       let activePatternItems = 
           match fullyQualified with 
           | FullyQualified -> []
           | OpenQualified ->
               nenv.ePatItems
               |> NameMap.range
               |> List.filter (function Item.ActivePatternCase _v -> true | _ -> false)

       let moduleAndNamespaceItems = 
           nenv.ModulesAndNamespaces(fullyQualified)
           |> NameMultiMap.range 
           |> List.filter (fun x -> 
                let demangledName = x.DemangledModuleOrNamespaceName
                IsInterestingModuleName demangledName && notFakeContainerModule ilTyconNames demangledName)
           |> List.filter (EntityRefContainsSomethingAccessible ncenv m ad)
           |> List.filter (IsTyconUnseen ad g ncenv.amap m >> not)
           |> List.map ItemForModuleOrNamespaceRef

       let tycons = 
           nenv.TyconsByDemangledNameAndArity(fullyQualified).Values
           |> List.filter (fun tcref -> not (tcref.LogicalName.Contains(",")))
           |> List.filter (fun tcref -> not tcref.IsExceptionDecl) 
           |> List.filter (IsTyconUnseen ad g ncenv.amap m >> not)
           |> List.map (ItemOfTyconRef ncenv m)

       // Get all the constructors accessible from here
       let constructors =  
           nenv.TyconsByDemangledNameAndArity(fullyQualified).Values
           |> List.filter (IsTyconUnseen ad g ncenv.amap m >> not)
           |> List.collect (InfosForTyconConstructors ncenv m ad)

       unqualifiedItems @ activePatternItems @ moduleAndNamespaceItems @ tycons @ constructors

    | id :: rest -> 
    
        // Look in the namespaces 'id' 
        let namespaces = 
            PartialResolveLongIndentAsModuleOrNamespaceThen nenv [id] (fun modref -> 
              let allowObsolete = rest <> [] && allowObsolete
              if EntityRefContainsSomethingAccessible ncenv m ad modref then 
                ResolvePartialLongIdentInModuleOrNamespace ncenv nenv isApplicableMeth m ad modref rest allowObsolete
              else 
                [])
        // Look for values called 'id' that accept the dot-notation 
        let values, isItemVal = 
            (match nenv.eUnqualifiedItems |> Map.tryFind id with
               // v.lookup : member of a value
             | Some v ->
                 match v with 
                 | Item.Value x -> 
                     let typ = x.Type
                     let typ = if x.BaseOrThisInfo = CtorThisVal && isRefCellTy g typ then destRefCellTy g typ else typ
                     (ResolvePartialLongIdentInType ncenv nenv isApplicableMeth m ad false rest typ), true
                 | _ -> [], false
             | None -> [], false)

        let staticSometingInType = 
            [ if not isItemVal then 
                // type.lookup : lookup a static something in a type 
                for tcref in LookupTypeNameInEnvNoArity OpenQualified id nenv do
                    let tcref = ResolveNestedTypeThroughAbbreviation ncenv tcref m
                    let typ = FreshenTycon ncenv m tcref
                    yield! ResolvePartialLongIdentInType ncenv nenv isApplicableMeth m ad true rest typ ]
        
        namespaces @ values @ staticSometingInType

/// Resolve a (possibly incomplete) long identifier to a set of possible resolutions.
let ResolvePartialLongIdent ncenv nenv isApplicableMeth m ad plid allowObsolete = 
    ResolvePartialLongIdentPrim ncenv nenv (ResolveCompletionTargets.All isApplicableMeth) OpenQualified m ad plid allowObsolete 

// REVIEW: has much in common with ResolvePartialLongIdentInModuleOrNamespace - probably they should be united
let rec ResolvePartialLongIdentInModuleOrNamespaceForRecordFields (ncenv: NameResolver) nenv m ad (modref:ModuleOrNamespaceRef) plid allowObsolete =
    let g = ncenv.g
    let mty = modref.ModuleOrNamespaceType

    match plid with 
    | [] -> 
       // get record type constructors
       let tycons = 
           mty.TypeDefinitions
           |> List.filter (fun tcref -> not (tcref.LogicalName.Contains(",")))
           |> List.filter (fun tycon -> tycon.IsRecordTycon)
           |> List.filter (fun tycon -> not (IsTyconUnseen ad g ncenv.amap m (modref.NestedTyconRef tycon)))

       let ilTyconNames = 
           mty.TypesByAccessNames.Values
           |> List.choose (fun (tycon:Tycon) -> if tycon.IsILTycon then Some tycon.DisplayName else None)
           |> Set.ofList
    
        // Collect up the accessible sub-modules 
       (mty.ModulesAndNamespacesByDemangledName 
          |> NameMap.range 
          |> List.filter (fun x -> 
                let demangledName = x.DemangledModuleOrNamespaceName
                notFakeContainerModule ilTyconNames demangledName && IsInterestingModuleName demangledName)
          |> List.map modref.NestedTyconRef
          |> List.filter (IsTyconUnseen ad g ncenv.amap m >> not)
          |> List.filter (EntityRefContainsSomethingAccessible ncenv m ad)
          |> List.map ItemForModuleOrNamespaceRef)

       // Collect all accessible record types
       @ (tycons |> List.map (modref.NestedTyconRef >> ItemOfTyconRef ncenv m) )
       @ [ // accessible record fields
            for tycon in tycons do
                if IsEntityAccessible ncenv.amap m ad (modref.NestedTyconRef tycon) then
                    let ttype = FreshenTycon ncenv m (modref.NestedTyconRef tycon)
                    yield! 
                        ncenv.InfoReader.GetRecordOrClassFieldsOfType(None, ad, m, ttype)
                        |> List.map Item.RecdField
         ]

    | id :: rest  -> 
        (match mty.ModulesAndNamespacesByDemangledName.TryFind(id) with
         | Some mspec 
             when not (IsTyconUnseenObsoleteSpec ad g ncenv.amap m (modref.NestedTyconRef mspec) allowObsolete) -> 
             let allowObsolete = rest <> [] && allowObsolete
             ResolvePartialLongIdentInModuleOrNamespaceForRecordFields ncenv nenv m ad (modref.NestedTyconRef mspec) rest allowObsolete
         | _ -> [])
        @ (
            match rest with
            | [] -> 
                // get all fields from the type named 'id' located in current modref
                let tycons = LookupTypeNameInEntityNoArity m id modref.ModuleOrNamespaceType
                tycons
                |> List.filter (fun tc -> tc.IsRecordTycon)
                |> List.collect (fun tycon ->
                    let tcref = modref.NestedTyconRef tycon
                    let ttype = FreshenTycon ncenv m tcref
                    ncenv.InfoReader.GetRecordOrClassFieldsOfType(None, ad, m, ttype))
                |> List.map Item.RecdField
            | _ -> []
        )

/// allowObsolete - specifies whether we should return obsolete types & modules 
///   as (no other obsolete items are returned)
let rec ResolvePartialLongIdentToClassOrRecdFields (ncenv: NameResolver) (nenv: NameResolutionEnv) m ad plid (allowObsolete : bool) = 
    ResolvePartialLongIdentToClassOrRecdFieldsImpl ncenv nenv OpenQualified m ad plid allowObsolete

and ResolvePartialLongIdentToClassOrRecdFieldsImpl (ncenv: NameResolver) (nenv: NameResolutionEnv) fullyQualified m ad plid allowObsolete = 
    let g = ncenv.g

    match  plid with
    |  id :: plid when id = "global" -> // this is deliberately not the mangled name
       // dive deeper
       ResolvePartialLongIdentToClassOrRecdFieldsImpl ncenv nenv FullyQualified m ad plid allowObsolete
    |  [] ->     
        
        // empty plid - return namespaces\modules\record types\accessible fields
       let iltyconNames =
          nenv.TyconsByAccessNames(fullyQualified).Values
          |> List.choose (fun tyconRef -> if tyconRef.IsILTycon then Some tyconRef.DisplayName else None)
          |> Set.ofList

       let mods = 
           nenv.ModulesAndNamespaces(fullyQualified)
           |> NameMultiMap.range 
           |> List.filter (fun x -> 
                let demangledName = x.DemangledModuleOrNamespaceName
                IsInterestingModuleName demangledName && notFakeContainerModule iltyconNames demangledName)
           |> List.filter (EntityRefContainsSomethingAccessible ncenv m ad)
           |> List.filter (IsTyconUnseen ad g ncenv.amap m >> not)
           |> List.map ItemForModuleOrNamespaceRef

       let recdTyCons = 
           nenv.TyconsByDemangledNameAndArity(fullyQualified).Values
           |> List.filter (fun tcref -> not (tcref.LogicalName.Contains(",")))
           |> List.filter (fun tcref -> tcref.IsRecordTycon) 
           |> List.filter (IsTyconUnseen ad g ncenv.amap m >> not)
           |> List.map (ItemOfTyconRef ncenv m)

       let recdFields = 
           nenv.eFieldLabels
           |> Seq.collect (fun (KeyValue(_, v)) -> v)
           |> Seq.map (fun fref -> 
                let typeInsts = fref.TyconRef.TyparsNoRange |> List.map (fun tyar -> tyar.AsType)
                Item.RecdField(RecdFieldInfo(typeInsts, fref)))
           |> List.ofSeq

       mods @ recdTyCons @ recdFields

    | id::rest -> 
        // Get results
        let modsOrNs = 
            PartialResolveLongIndentAsModuleOrNamespaceThen nenv [id] (fun modref -> 
              let allowObsolete = rest <> [] && allowObsolete
              if EntityRefContainsSomethingAccessible ncenv m ad modref then 
                ResolvePartialLongIdentInModuleOrNamespaceForRecordFields ncenv nenv m ad modref rest allowObsolete
              else 
                [])
        let qualifiedFields = 
            match rest with
            | [] ->
                // get record types accessible in given nenv
                let tycons = LookupTypeNameInEnvNoArity OpenQualified id nenv
                tycons
                |> List.collect (fun tcref ->
                    let ttype = FreshenTycon ncenv m tcref
                    ncenv.InfoReader.GetRecordOrClassFieldsOfType(None, ad, m, ttype))
                |> List.map Item.RecdField
            | _-> []
        modsOrNs @ qualifiedFields

(* Determining if an `Item` is resolvable at point by given `plid`. It's optimized by being lazy and early returning according to the given `Item` *)

let private ResolveCompletionsInTypeForItem (ncenv: NameResolver) nenv m ad statics typ (item: Item) : seq<Item> =
    seq {
        let g = ncenv.g
        let amap = ncenv.amap
        
        match item with
        | Item.RecdField _ ->
            yield!
                ncenv.InfoReader.GetRecordOrClassFieldsOfType(None,ad,m,typ)
                |> List.filter (fun rfref -> rfref.IsStatic = statics  &&  IsFieldInfoAccessible ad rfref)
                |> List.map Item.RecdField
        | Item.UnionCase _ ->
            if statics && isAppTy g typ then 
                let tc, tinst = destAppTy g typ
                yield!
                    tc.UnionCasesAsRefList 
                    |> List.filter (IsUnionCaseUnseen ad g ncenv.amap m >> not)
                    |> List.map (fun ucref ->  Item.UnionCase(UnionCaseInfo(tinst,ucref),false))
        | Item.Event _ ->
            yield!
                ncenv.InfoReader.GetEventInfosOfType(None,ad,m,typ)
                |> List.filter (fun x -> 
                    IsStandardEventInfo ncenv.InfoReader m ad x &&
                    x.IsStatic = statics)
                |> List.map Item.Event
        | Item.ILField _ ->
            yield!
                ncenv.InfoReader.GetILFieldInfosOfType(None,ad,m,typ)
                |> List.filter (fun x -> 
                    not x.IsSpecialName &&
                    x.IsStatic = statics && 
                    IsILFieldInfoAccessible g amap m ad x)
                |> List.map Item.ILField
        | Item.Types _ ->
            if statics then
                yield! typ |> GetNestedTypesOfType (ad, ncenv, None, TypeNameResolutionStaticArgsInfo.Indefinite, false, m) |> List.map (ItemOfTy g)
        | _ ->
            let pinfosIncludingUnseen = 
                AllPropInfosOfTypeInScope ncenv.InfoReader nenv (None,ad) PreferOverrides m typ
                |> List.filter (fun x -> 
                    x.IsStatic = statics && 
                    IsPropInfoAccessible g amap m ad x)
        
            // Exclude get_ and set_ methods accessed by properties 
            let pinfoMethNames = 
              (pinfosIncludingUnseen 
               |> List.filter (fun pinfo -> pinfo.HasGetter)
               |> List.map (fun pinfo -> pinfo.GetterMethod.LogicalName))
              @
              (pinfosIncludingUnseen 
               |> List.filter (fun pinfo -> pinfo.HasSetter)
               |> List.map (fun pinfo -> pinfo.SetterMethod.LogicalName))
            
            let einfoMethNames = 
                let einfos = 
                    ncenv.InfoReader.GetEventInfosOfType(None,ad,m,typ)
                    |> List.filter (fun x -> 
                        IsStandardEventInfo ncenv.InfoReader m ad x &&
                        x.IsStatic = statics)
                
                [ for einfo in einfos do 
                    let delegateType = einfo.GetDelegateType(amap, m)
                    let (SigOfFunctionForDelegate(invokeMethInfo,_,_,_)) = GetSigOfFunctionForDelegate ncenv.InfoReader delegateType m ad 
                    // Only events with void return types are suppressed in intellisense.
                    if slotSigHasVoidReturnTy (invokeMethInfo.GetSlotSig(amap, m)) then 
                      yield einfo.GetAddMethod().DisplayName
                      yield einfo.GetRemoveMethod().DisplayName ]
        
            let suppressedMethNames = Zset.ofList String.order (pinfoMethNames @ einfoMethNames)
        
            let pinfos = 
                pinfosIncludingUnseen
                |> List.filter (fun x -> not (PropInfoIsUnseen m x))
        
            let minfoFilter (minfo: MethInfo) = 
                // Only show the Finalize, MemberwiseClose etc. methods on System.Object for values whose static type really is 
                // System.Object. Few of these are typically used from F#.  
                //
                // Don't show GetHashCode or Equals for F# types that admit equality as an abnormal operation
                let isUnseenDueToBasicObjRules = 
                    not (isObjTy g typ) &&
                    not minfo.IsExtensionMember &&
                    match minfo.LogicalName with
                    | "GetType"  -> false
                    | "GetHashCode"  -> isObjTy g minfo.EnclosingType && not (AugmentWithHashCompare.TypeDefinitelyHasEquality g typ)
                    | "ToString" -> false
                    | "Equals" ->                 
                        if not (isObjTy g minfo.EnclosingType) then 
                            // declaring type is not System.Object - show it
                            false 
                        elif minfo.IsInstance then
                            // System.Object has only one instance Equals method and we want to suppress it unless Augment.TypeDefinitelyHasEquality is true
                            not (AugmentWithHashCompare.TypeDefinitelyHasEquality g typ)
                        else
                            // System.Object has only one static Equals method and we always want to suppress it
                            true
                    | _ -> 
                        // filter out self methods of obj type
                        isObjTy g minfo.EnclosingType
                let result = 
                    not isUnseenDueToBasicObjRules &&
                    not minfo.IsInstance = statics &&
                    IsMethInfoAccessible amap m ad minfo &&
                    not (MethInfoIsUnseen g m typ minfo) &&
                    not minfo.IsConstructor &&
                    not minfo.IsClassConstructor &&
                    not (minfo.LogicalName = ".cctor") &&
                    not (minfo.LogicalName = ".ctor") &&
                    not (suppressedMethNames.Contains minfo.LogicalName)
                result
        
            let pinfoItems =
                pinfos
                |> List.choose (fun pinfo->
                    let pinfoOpt = DecodeFSharpEvent [pinfo] ad g ncenv m
                    match pinfoOpt with
                    | Some(Item.Event einfo) -> if IsStandardEventInfo ncenv.InfoReader m ad einfo then pinfoOpt else None
                    | _ -> pinfoOpt)

            yield! pinfoItems

            match item with
            | Item.MethodGroup _ ->
                // REVIEW: add a name filter here in the common cases?
                let minfos = 
                    let minfos =
                        AllMethInfosOfTypeInScope ncenv.InfoReader nenv (None,ad) PreferOverrides m typ 
                        |> List.filter minfoFilter
        
                    let minfos = 
                        let addersAndRemovers = 
                            pinfoItems 
                            |> List.collect (function Item.Event(FSEvent(_,_,addValRef,removeValRef)) -> [addValRef.LogicalName;removeValRef.LogicalName] | _ -> [])
                            |> set
        
                        if addersAndRemovers.IsEmpty then minfos
                        else minfos |> List.filter (fun minfo -> not (addersAndRemovers.Contains minfo.LogicalName))
        
        #if EXTENSIONTYPING
                    // Filter out the ones with mangled names from applying static parameters
                    let minfos = 
                        let methsWithStaticParams = 
                            minfos 
                            |> List.filter (fun minfo -> 
                                match minfo.ProvidedStaticParameterInfo with 
                                | Some (_methBeforeArguments, staticParams) -> staticParams.Length <> 0
                                | _ -> false)
                            |> List.map (fun minfo -> minfo.DisplayName)
        
                        if methsWithStaticParams.IsEmpty then minfos
                        else minfos |> List.filter (fun minfo -> 
                                let nm = minfo.LogicalName
                                not (nm.Contains "," && methsWithStaticParams |> List.exists (fun m -> nm.StartsWith(m))))
        #endif
        
                    minfos 
    
                // Partition methods into overload sets
                let rec partitionl (l:MethInfo list) acc = 
                    match l with
                    | [] -> acc
                    | h::t -> 
                        let nm = h.LogicalName
                        partitionl t (NameMultiMap.add nm h acc)
            
                yield! List.map Item.MakeMethGroup (NameMap.toList (partitionl minfos Map.empty))
            | _ -> ()
    }

let rec private ResolvePartialLongIdentInTypeForItem (ncenv: NameResolver) nenv m ad statics plid (item: Item) typ =
    seq {
        let g = ncenv.g
        let amap = ncenv.amap
        
        match plid with
        | [] -> yield! ResolveCompletionsInTypeForItem ncenv nenv m ad statics typ item
        | id :: rest ->
      
          let rfinfos = 
            ncenv.InfoReader.GetRecordOrClassFieldsOfType(None,ad,m,typ)
            |> List.filter (fun fref -> IsRecdFieldAccessible ncenv.amap m ad fref.RecdFieldRef)
            |> List.filter (fun fref -> fref.RecdField.IsStatic = statics)
      
          let nestedTypes = typ |> GetNestedTypesOfType (ad, ncenv, Some id, TypeNameResolutionStaticArgsInfo.Indefinite, false, m)  
    
          // e.g. <val-id>.<recdfield-id>.<more> 
          for rfinfo in rfinfos do
              if rfinfo.Name = id then
                  yield! ResolvePartialLongIdentInTypeForItem ncenv nenv m ad false rest item rfinfo.FieldType
    
          // e.g. <val-id>.<property-id>.<more> 
          let fullTypeOfPinfo (pinfo: PropInfo) = 
              let rty = pinfo.GetPropertyType(amap,m) 
              let rty = if pinfo.IsIndexer then mkRefTupledTy g (pinfo.GetParamTypes(amap, m)) --> rty else  rty 
              rty      
          
          let pinfos =
              typ
              |> AllPropInfosOfTypeInScope ncenv.InfoReader nenv (Some id,ad) IgnoreOverrides m
              |> List.filter (fun x -> x.IsStatic = statics)
              |> List.filter (IsPropInfoAccessible g amap m ad) 

          for pinfo in pinfos do
              yield! (fullTypeOfPinfo pinfo) |> ResolvePartialLongIdentInTypeForItem ncenv nenv m ad false rest item
    
          // e.g. <val-id>.<event-id>.<more> 
          for einfo in ncenv.InfoReader.GetEventInfosOfType(Some id, ad, m, typ) do
              let tyinfo = PropTypOfEventInfo ncenv.InfoReader m ad einfo
              yield! ResolvePartialLongIdentInTypeForItem ncenv nenv m ad false rest item tyinfo
    
          // nested types!
          for ty in nestedTypes do
              yield! ResolvePartialLongIdentInTypeForItem ncenv nenv m ad statics rest item ty
    
          // e.g. <val-id>.<il-field-id>.<more> 
          for finfo in ncenv.InfoReader.GetILFieldInfosOfType(Some id, ad, m, typ) do
              if not finfo.IsSpecialName && finfo.IsStatic = statics && IsILFieldInfoAccessible g amap m ad finfo then
                  yield! finfo.FieldType(amap, m) |> ResolvePartialLongIdentInTypeForItem ncenv nenv m ad false rest item
    }

let rec private ResolvePartialLongIdentInModuleOrNamespaceForItem (ncenv: NameResolver) nenv m ad (modref: ModuleOrNamespaceRef) plid (item: Item) =
    let g = ncenv.g
    let mty = modref.ModuleOrNamespaceType
    
    seq {
        match plid with 
        | [] -> 
             match item with
             | Item.Value _ ->
                  // Collect up the accessible values in the module, excluding the members
                  yield!
                      mty.AllValsAndMembers
                      |> Seq.toList
                      |> List.choose (TryMkValRefInModRef modref) // if the assembly load set is incomplete and we get a None value here, then ignore the value
                      |> List.filter (fun v -> v.MemberInfo.IsNone)
                      |> List.filter (IsValUnseen ad g m >> not) 
                      |> List.map Item.Value
             | Item.UnionCase _ ->   
             // Collect up the accessible discriminated union cases in the module 
                  yield! 
                      UnionCaseRefsInModuleOrNamespace modref 
                      |> List.filter (IsUnionCaseUnseen ad g ncenv.amap m >> not)
                      |> List.map (fun x -> Item.UnionCase(GeneralizeUnionCaseRef x,  false))
             | Item.ActivePatternCase _ ->
             // Collect up the accessible active patterns in the module 
                 yield!
                      ActivePatternElemsOfModuleOrNamespace modref 
                      |> NameMap.range
                      |> List.filter (fun apref -> apref.ActivePatternVal |> IsValUnseen ad g m |> not) 
                      |> List.map Item.ActivePatternCase
             | Item.ExnCase _ ->
             // Collect up the accessible F# exception declarations in the module 
                 yield!
                     mty.ExceptionDefinitionsByDemangledName 
                     |> NameMap.range 
                     |> List.map modref.NestedTyconRef
                     |> List.filter (IsTyconUnseen ad g ncenv.amap m >> not)
                     |> List.map Item.ExnCase
             | _ ->
                 let ilTyconNames = 
                     mty.TypesByAccessNames.Values
                     |> List.choose (fun (tycon:Tycon) -> if tycon.IsILTycon then Some tycon.DisplayName else None)
                     |> Set.ofList

                 // Collect up the accessible sub-modules. We must yield them even though `item` is not a module or namespace, 
                 // otherwise we would not resolve long idents which have modules and namespaces in the middle (i.e. all long idents)
                 yield! 
                     mty.ModulesAndNamespacesByDemangledName 
                     |> NameMap.range 
                     |> List.filter (fun x -> 
                           let demangledName = x.DemangledModuleOrNamespaceName
                           notFakeContainerModule ilTyconNames demangledName && IsInterestingModuleName demangledName)
                     |> List.map modref.NestedTyconRef
                     |> List.filter (IsTyconUnseen ad g ncenv.amap m >> not)
                     |> List.filter (EntityRefContainsSomethingAccessible ncenv m ad)
                     |> List.map ItemForModuleOrNamespaceRef
                 let tycons = 
                     mty.TypeDefinitions
                     |> List.filter (fun tcref -> not (tcref.LogicalName.Contains(",")))
                     |> List.filter (fun tycon -> not (IsTyconUnseen ad g ncenv.amap m (modref.NestedTyconRef tycon)))

                 // Get all the types and .NET constructor groups accessible from here 
                 yield! tycons |> List.map (modref.NestedTyconRef >> ItemOfTyconRef ncenv m)
                 yield! tycons |> List.collect (modref.NestedTyconRef >> InfosForTyconConstructors ncenv m ad)
        
        | id :: rest  -> 
        
            match mty.ModulesAndNamespacesByDemangledName.TryFind(id) with
            | Some mspec 
                when not (IsTyconUnseenObsoleteSpec ad g ncenv.amap m (modref.NestedTyconRef mspec) true) -> 
                yield! ResolvePartialLongIdentInModuleOrNamespaceForItem ncenv nenv m ad (modref.NestedTyconRef mspec) rest item
            | _ -> ()
        
            for tycon in LookupTypeNameInEntityNoArity m id modref.ModuleOrNamespaceType do
                 let tcref = modref.NestedTyconRef tycon 
                 if not (IsTyconUnseenObsoleteSpec ad g ncenv.amap m tcref true) then 
                     yield! tcref |> generalizedTyconRef |> ResolvePartialLongIdentInTypeForItem ncenv nenv m ad true rest item
    }

let rec private PartialResolveLookupInModuleOrNamespaceAsModuleOrNamespaceThenLazy f plid (modref: ModuleOrNamespaceRef) =
    let mty = modref.ModuleOrNamespaceType
    match plid with 
    | [] -> f modref
    | id :: rest -> 
        match mty.ModulesAndNamespacesByDemangledName.TryFind id with
        | Some mty -> 
            PartialResolveLookupInModuleOrNamespaceAsModuleOrNamespaceThenLazy f rest (modref.NestedTyconRef mty) 
        | None -> Seq.empty

let private PartialResolveLongIndentAsModuleOrNamespaceThenLazy (nenv:NameResolutionEnv) plid f =
    seq {
        match plid with 
        | id :: rest -> 
            match Map.tryFind id nenv.eModulesAndNamespaces with
            | Some modrefs -> 
                for modref in modrefs do
                    yield! PartialResolveLookupInModuleOrNamespaceAsModuleOrNamespaceThenLazy f rest modref
            | None -> ()
        | [] -> ()
    }

let rec private GetCompletionForItem (ncenv: NameResolver) (nenv: NameResolutionEnv) m ad plid (item: Item) : seq<Item> =
    seq {
        let g = ncenv.g
        
        match plid with
        |  "global" :: plid -> // this is deliberately not the mangled name
        
           yield! GetCompletionForItem ncenv nenv m ad plid item
        
        |  [] -> 

           /// Include all the entries in the eUnqualifiedItems table. 
           for uitem in nenv.eUnqualifiedItems.Values do
               match uitem with
               | Item.UnqualifiedType _ -> ()
               | _ when not (ItemIsUnseen ad g ncenv.amap m uitem) ->
                   yield uitem
               | _ -> ()

           match item with
           | Item.ModuleOrNamespaces _ ->
               let ilTyconNames =
                  nenv.TyconsByAccessNames(OpenQualified).Values
                  |> List.choose (fun tyconRef -> if tyconRef.IsILTycon then Some tyconRef.DisplayName else None)
                  |> Set.ofList
               
               for ns in NameMultiMap.range (nenv.ModulesAndNamespaces(OpenQualified)) do
                   let demangledName = ns.DemangledModuleOrNamespaceName
                   if IsInterestingModuleName demangledName && notFakeContainerModule ilTyconNames demangledName
                      && EntityRefContainsSomethingAccessible ncenv m ad ns
                      && not (IsTyconUnseen ad g ncenv.amap m ns)
                   then yield ItemForModuleOrNamespaceRef ns
           
           | Item.Types _ ->
               for tcref in nenv.TyconsByDemangledNameAndArity(OpenQualified).Values do
                   if not tcref.IsExceptionDecl 
                      && not (tcref.LogicalName.Contains ",")
                      && not (IsTyconUnseen ad g ncenv.amap m tcref)
                   then yield ItemOfTyconRef ncenv m tcref

           | Item.ActivePatternCase _ ->
               for pitem in NameMap.range nenv.ePatItems do
                   match pitem with
                   | Item.ActivePatternCase _ ->
                       yield pitem
                   | _ -> ()

           | Item.DelegateCtor _
           | Item.FakeInterfaceCtor _
           | Item.CtorGroup _ 
           | Item.UnqualifiedType _ ->
               for tcref in nenv.TyconsByDemangledNameAndArity(OpenQualified).Values do
                   if not (IsTyconUnseen ad g ncenv.amap m tcref)
                   then yield! InfosForTyconConstructors ncenv m ad tcref
            
           | _ -> ()

        | id :: rest -> 
        
            // Look in the namespaces 'id' 
            yield!
                PartialResolveLongIndentAsModuleOrNamespaceThenLazy nenv [id] (fun modref -> 
                    if EntityRefContainsSomethingAccessible ncenv m ad modref then 
                        ResolvePartialLongIdentInModuleOrNamespaceForItem ncenv nenv m ad modref rest item
                    else Seq.empty)
            
            // Look for values called 'id' that accept the dot-notation 
            let values, isItemVal = 
                (if nenv.eUnqualifiedItems.ContainsKey(id) then 
                         // v.lookup : member of a value
                  let v = nenv.eUnqualifiedItems.[id]
                  match v with 
                  | Item.Value x -> 
                      let typ = x.Type
                      let typ = if x.BaseOrThisInfo = CtorThisVal && isRefCellTy g typ then destRefCellTy g typ else typ
                      (ResolvePartialLongIdentInTypeForItem ncenv nenv m ad false rest item typ), true
                  | _ -> Seq.empty, false
                 else Seq.empty, false)
            
            yield! values

            if not isItemVal then 
                // type.lookup : lookup a static something in a type 
                for tcref in LookupTypeNameInEnvNoArity OpenQualified id nenv do
                    let tcref = ResolveNestedTypeThroughAbbreviation ncenv tcref m
                    let typ = FreshenTycon ncenv m tcref
                    yield! ResolvePartialLongIdentInTypeForItem ncenv nenv m ad true rest item typ
    }

let IsItemResolvable (ncenv: NameResolver) (nenv: NameResolutionEnv) m ad plid (item: Item) : bool = 
    GetCompletionForItem ncenv nenv m ad plid item |> Seq.exists (ItemsAreEffectivelyEqual ncenv.g item)
