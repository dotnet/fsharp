module internal Fsharp.Compiler.SignatureHash

let x = 42

open System
open System.Globalization
open System.IO
open Internal.Utilities.Collections
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras
open Internal.Utilities.Rational
open FSharp.Compiler 
open FSharp.Compiler.AbstractIL.IL 
open FSharp.Compiler.AccessibilityLogic
open FSharp.Compiler.AttributeChecking
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Infos
open FSharp.Compiler.InfoReader
open FSharp.Compiler.Syntax
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Layout
open FSharp.Compiler.Text.LayoutRender
open FSharp.Compiler.Text.TaggedText
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.TypeHierarchy
open FSharp.Compiler.Xml

open FSharp.Core.Printf

type Hash = int

let inline hashText (s:string) : Hash = hash s
let inline combineHash x y  : Hash= (x <<< 1) + y + 631
let inline addToHash (acc:Hash) (value:Hash)  = combineHash acc value
let inline pipeToHash (value:Hash) (acc:Hash)   = combineHash acc value  
let inline hashAndAdd (value) (acc:Hash) = addToHash (hash value) (acc)
let inline combineHashes (hashes: Hash list) = (0,hashes) ||> List.fold (fun acc curr -> combineHash acc curr)
let inline hashAllVia func (items: 'T list) : Hash =
    let mutable acc = 0
    for i in items do
        let valHash = func i
        // We are calling hashallVia for things like list of types, list of properties, list of fields etc. The ones which are visibility-hidden will return 0, and are ommited.
        if valHash <> 0 then
            acc <- addToHash acc valHash
    acc

let inline hashArrayVia func (items: 'T[]) : Hash =
    let mutable acc = 0
    for i in items do
        let valHash = func i
        // We are calling hashallVia for things like list of types, list of properties, list of fields etc. The ones which are visibility-hidden will return 0, and are ommited.
        if valHash <> 0 then
            acc <- addToHash acc valHash
    acc
let (@@) (h1:Hash) (h2:Hash) = combineHash h1 h2
let (^^) (h1:Hash) (h2:Hash) = combineHash h1 h2


[<AutoOpen>]
module internal HashUtilities =  
    
    let private hashEntityRefName (g:TcGlobals) (xref: EntityRef) name =    
        let tag = 
            if xref.IsNamespace then TextTag.Namespace
            elif xref.IsModule then TextTag.Module
            elif xref.IsTypeAbbrev then
                let ty = xref.TypeAbbrev.Value
                match stripTyEqns g ty with
                | TType_app(tcref, _, _) when tcref.IsStructOrEnumTycon ->
                    TextTag.Struct
                | _ ->
                    TextTag.Alias
            elif xref.IsFSharpDelegateTycon then TextTag.Delegate
            elif xref.IsILEnumTycon || xref.IsFSharpEnumTycon then TextTag.Enum
            elif xref.IsStructOrEnumTycon then TextTag.Struct
            elif isInterfaceTyconRef xref then TextTag.Interface
            elif xref.IsUnionTycon then TextTag.Union
            elif xref.IsRecordTycon then TextTag.Record
            else TextTag.Class

        combineHash (hash tag) (hash name)

    let hashTyconRefImpl (g:TcGlobals) (tcref: TyconRef) =
        let demangled = tcref.DisplayNameWithStaticParameters
        let tyconHash = hashEntityRefName g tcref demangled

        tcref.CompilationPath.AccessPath
        |> hashAllVia (fst >> hashText) 
        |> pipeToHash tyconHash
        
module HashIL = 

    let private hashILTypeRef (tref: ILTypeRef) =
        tref.Enclosing
        |> hashAllVia hashText      
        |> hashAndAdd tref.Name

    let private hashILArrayShape ( sh:ILArrayShape) = sh.Rank  
 
    let rec hashILType (ty: ILType) : Hash =
        match ty with
        | ILType.Void -> hash ILType.Void
        | ILType.Array (sh, t) -> hashILType  t ^^ hashILArrayShape sh
        | ILType.Value t
        | ILType.Boxed t -> hashILTypeRef  t.TypeRef ^^ (t.GenericArgs |> hashAllVia (hashILType ))
        | ILType.Ptr t
        | ILType.Byref t -> hashILType   t
        | ILType.FunctionPointer t -> hashILCallingSignature t
        | ILType.TypeVar n -> n 
        | ILType.Modified (_, _, t) -> hashILType  t

    and hashILCallingSignature   (signature: ILCallingSignature) =       
        let res = signature.ReturnType |> hashILType     
        signature.ArgTypes
        |> hashAllVia (hashILType ) 
        |> addToHash res

module rec PrintTypes = 

    let hashAccessibility (TAccess access) itemHash =
        let isInternalCompPath x = 
            match x with 
            | CompPath(ILScopeRef.Local, []) -> true 
            | _ -> false

        match  access with     
            | [] -> itemHash 
            | _ when List.forall isInternalCompPath access  -> itemHash * 0 // TODO handle internals visible to attribute 
            | _ -> 0

    /// Hash a reference to a type 
    let hashTyconRef (g:TcGlobals) tcref = hashTyconRefImpl g tcref

    /// Hash the flags of a member 
    let hashMemberFlags (memFlags: SynMemberFlags) = hash memFlags 

    /// Hash an attribute 'Type(arg1, ..., argN)' 
    let hashAttrib (g:TcGlobals) (Attrib(tyconRef = tcref)) = 
        hashTyconRefImpl g tcref


    let hashILAttrib (ty, args) = HashIL.hashILType  ty
   
    let hashAttribs (g:TcGlobals) startOpt isLiteral kind attrs restL = 
        let mutable acc = 0
        for a in attrs do
            acc <- addToHash acc  (hashAttrib g a)

        acc <- addToHash acc (hash startOpt)
        acc <- addToHash acc (hash isLiteral)
        acc <- addToHash acc (hash kind)

        acc
            
    let hashAttributeList (g:TcGlobals) attrs  =
        attrs
        |> hashAllVia (hashAttrib g)     

    let hashTyparRef (typar: Typar) =
        hashText typar.DisplayName
        |> hashAndAdd (typar.Rigidity)
        |> hashAndAdd (typar.StaticReq) 
    
    let hashTyparRefWithInfo (g:TcGlobals) (typar: Typar) =
        hashTyparRef typar @@ hashAttributeList g typar.Attribs 

    let hashConstraint (g:TcGlobals) (tp, tpc) =
        let tpHash = hashTyparRefWithInfo g tp      
        match tpc with 
        | TyparConstraint.CoercesTo(tgtTy, _) -> 
            tpHash @@ 1 @@ hashTType g tgtTy
        | TyparConstraint.MayResolveMember(traitInfo, _) ->
            tpHash @@ 2 @@ hashTraitWithInfo (* denv *) g traitInfo
        | TyparConstraint.DefaultsTo(_, ty, _) ->
            tpHash @@ 3 @@ hashTType g ty  
        | TyparConstraint.IsEnum(ty, _) ->
            tpHash @@ 4 @@ hashTType g ty            
        | TyparConstraint.SupportsComparison _ ->
            tpHash @@ 5
        | TyparConstraint.SupportsEquality _ ->
            tpHash @@ 6
        | TyparConstraint.IsDelegate(aty, bty, _) ->
            tpHash @@ 7 @@ hashTType g aty @@ hashTType g bty             
        | TyparConstraint.SupportsNull _ ->
            tpHash @@ 8
        | TyparConstraint.IsNonNullableStruct _ ->
            tpHash @@ 9
        | TyparConstraint.IsUnmanaged _ ->
            tpHash @@ 10
        | TyparConstraint.IsReferenceType _ ->
            tpHash @@ 11
        | TyparConstraint.SimpleChoice(tys, _) ->
            tpHash @@ 12 @@ (tys |> hashAllVia (hashTType g))  
        | TyparConstraint.RequiresDefaultConstructor _ -> 
            tpHash @@ 13

    /// Hash type parameter constraints
    let hashConstraints (g:TcGlobals) cxs = 
        cxs
        |> hashAllVia (hashConstraint g)

    let hashTraitWithInfo  (g:TcGlobals)  traitInfo =
        let nameHash = hashText traitInfo.MemberLogicalName
        let memberHash = hashMemberFlags traitInfo.MemberFlags    
        let returnTypeHash = match traitInfo.CompiledReturnType with Some t -> hashTType g t | _ -> -1

        traitInfo.CompiledObjectAndArgumentTypes
        |> hashAllVia (hashTType g)
        |> pipeToHash (nameHash)
        |> pipeToHash (returnTypeHash)
        |> pipeToHash memberHash
       

    /// Hash a unit of measure expression 
    let private hashMeasure unt =
        let measuresWithExponents = ListMeasureVarOccsWithNonZeroExponents unt |> List.sortBy (fun (tp: Typar, _) -> tp.DisplayName) 
        measuresWithExponents
        |> hashAllVia (fun (typar,exp: Rational) -> hashTyparRef typar @@ hash exp)
    
    /// Hash a type, taking precedence into account to insert brackets where needed
    let hashTType (g:TcGlobals)  ty =  
      
        match stripTyparEqns ty with 
        | TType_ucase (UnionCaseRef(tc, _), args)
        | TType_app (tc, args, _) ->
          args
          |> hashAllVia (hashTType g )        
          |> pipeToHash (hashTyconRef g tc)
        | TType_anon (anonInfo, tys) ->
            tys
            |> hashAllVia (hashTType g )
            |> pipeToHash (anonInfo.SortedNames |> hashArrayVia hashText)            
            |> hashAndAdd (evalAnonInfoIsStruct anonInfo)    
        | TType_tuple (tupInfo, t) ->
            t
            |> hashAllVia (hashTType g )
            |> hashAndAdd (evalTupInfoIsStruct tupInfo) 
        // Hash a first-class generic type. 
        | TType_forall (tps, tau) ->
            tps
            |> hashAllVia (hashTyparRef)
            |> pipeToHash (hashTType g  tau)  
        | TType_fun _ ->
            let argTys, retTy = stripFunTy g ty    
            argTys
            |> hashAllVia (hashTType g )
            |> pipeToHash (hashTType g  retTy)
        | TType_var (r, _) -> hashTyparRefWithInfo g r
        | TType_measure unt -> hashMeasure unt

    ///// Hash a list of types, separated with the given separator, either '*' or ','
    //let hashTypesWithInfoAndPrec (* denv *) (g:TcGlobals)  prec sep typl = 
    //    sepListL sep (List.map (hashTType (* denv *) env prec) typl)

    //let hashReturnType (* denv *) env retTy = hashTType (* denv *) env 4 retTy

    ///// Hash a single type, taking TypeSimplificationInfo into account 
    //let hashTypeWithInfo (* denv *) env ty = 
    //    hashTType (* denv *) env 5 ty

    //let hashType (* denv *) ty = 
    //    hashTypeWithInfo (* denv *) SimplifyTypes.typeSimplificationInfo0 ty

    // Format each argument, including its name and type 
    let hashArgInfo (g:TcGlobals) (ty, argInfo: ArgReprInfo) = 
       
        let attributesHash = hashAttributeList g argInfo.Attribs
        let nameHash = match argInfo.Name with | Some i -> hashText i.idText | _ -> -1
        let typeHash = hashTType g ty

        typeHash @@ nameHash @@ attributesHash

    let hashCurriedArgInfos (g:TcGlobals) argInfos =
        argInfos
        |> hashAllVia(fun l -> l |> hashAllVia (hashArgInfo g))
      

    let hashGenericParameterTypes (g:TcGlobals) genParamTys = 
        genParamTys
        |> hashAllVia (hashTType g)     

    /// Hash a single type used as the type of a member or value 
    let hashTopType (g:TcGlobals) argInfos retTy cxs =
        let retTypeHash = hashTType g retTy
        let cxsHash = hashConstraints g cxs
        let argHash = hashCurriedArgInfos g argInfos

        retTypeHash @@ cxsHash @@ argHash  
        
    let hashTyparInclConstraints  (g:TcGlobals) (typar: Typar) = 
        typar.Constraints |> hashAllVia (fun tpc -> hashConstraint g (typar,tpc))
        |> pipeToHash (hashTyparRef typar)

    /// Hash type parameters
    let hashTyparDecls (g:TcGlobals) (typars: Typars) =      
        typars 
        |> hashAllVia (hashTyparInclConstraints g)


    let hashTrait (g:TcGlobals) traitInfo =
        hashTraitWithInfo g  traitInfo

    //let hashTyparConstraint (g:TcGlobals)  (tp, tpc) =
    //    hashConstraint g (tp, tpc)  

    //let prettyLayoutOfInstAndSig (g:TcGlobals) (typarInst:TyparInstantiation, tys, retTy) =
    //    typarInst
    //    |> hashAllVia (fun (typar,ttype) -> hashTyparRef typar @@ hashTType g ttype)
    //    |> pipeToHash (hashTType g retTy)
    //    |> pipeToHash (tys |> hashAllVia (hashTType g))

    //let prettyLayoutOfTopTypeInfoAux (g:TcGlobals) prettyArgInfos prettyRetTy cxs =        
    //    hashTopType g prettyArgInfos prettyRetTy cxs

    let hashUncurriedSig (g:TcGlobals) typarInst argInfos retTy = 
        typarInst
        |> hashAllVia (fun (typar,ttype) -> hashTyparInclConstraints g typar @@ hashTType g ttype)
        |> pipeToHash (hashTopType g argInfos retTy [])     

    // Hash: type spec - class, datatype, record, abbrev 
    let hashMemberSigCore (g:TcGlobals) memberToParentInst (typarInst, methTypars: Typars, argInfos, retTy) = 
        typarInst
        |> hashAllVia (fun (typar,ttype) -> hashTyparInclConstraints g typar @@ hashTType g ttype)
        |> pipeToHash (hashTopType g argInfos retTy []) 
        |> pipeToHash (memberToParentInst |> hashAllVia (fun (typar,ty) -> hashTyparRef typar @@ hashTType g ty))
        |> pipeToHash (hashTyparDecls g methTypars)

    let hashMemberType (g:TcGlobals) vref typarInst argInfos retTy = 
        match PartitionValRefTypars g vref with
        | Some(_, _, memberMethodTypars, memberToParentInst, _) ->
            hashMemberSigCore g memberToParentInst (typarInst, memberMethodTypars, argInfos, retTy)
        | None -> 
            hashUncurriedSig g typarInst argInfos retTy 

/// Printing TAST objects
module HashTastMemberOrVals =
    open PrintTypes 

    let hashMember (g:TcGlobals) typarInst (v: Val)  =
        let vref = mkLocalValRef v
        let membInfo = Option.get vref.MemberInfo
        let _tps, argInfos, retTy, _ = GetTypeOfMemberInFSharpForm g vref     

        let memberFlagsHash = hashMemberFlags membInfo.MemberFlags
        let parentTypeHash = hashTyconRef g membInfo.ApparentEnclosingEntity
        let memberTypeHash = hashMemberType g vref typarInst argInfos retTy
        let flagsHash = hash v.val_flags.PickledBits
        let nameHash = hashText v.DisplayNameCoreMangled
        let attribsHash = hashAttributeList g v.Attribs

        let combinedHash = memberFlagsHash @@ parentTypeHash @@ memberTypeHash @@ flagsHash @@ nameHash @@ attribsHash

        hashAccessibility vref.Accessibility combinedHash

    let hashNonMemberVal (g:TcGlobals) (tps, v: Val, tau, cxs) =
        let valReprInfo = arityOfValForDisplay v

        let nameHash = hashText v.DisplayNameCoreMangled
        let typarHash = hashTyparDecls g tps
        let argInfos, retTy = GetTopTauTypeInFSharpForm g valReprInfo.ArgInfos tau v.Range
        let typeHash = hashTopType g argInfos retTy cxs
        let flagsHash = hash v.val_flags.PickledBits
        let attribsHash = hashAttributeList g v.Attribs

        let combinedHash = nameHash @@ typarHash @@ typeHash @@ flagsHash @@ attribsHash

        hashAccessibility v.Accessibility combinedHash


    let hashValOrMemberNoInst (g:TcGlobals) (vref: ValRef) =
        match vref.MemberInfo with 
        | None ->
            let tps, tau = vref.GeneralizedType
            // adjust the type in case this is the 'this' pointer stored in a reference cell
            let tau = StripSelfRefCell(g, vref.BaseOrThisInfo, tau)
            let (_prettyTyparInst, prettyTypars, prettyTauTy), cxs = PrettyTypes.PrettifyInstAndTyparsAndType g (emptyTyparInst, tps, tau)
            hashNonMemberVal g (tps, vref.Deref, prettyTauTy, cxs)               
        | Some _ -> 
            hashMember g emptyTyparInst vref.Deref

//-------------------------------------------------------------------------

/// Printing info objects
module InfoMemberPrinting =         

    let hashMethInfoFSharpStyle  (g:TcGlobals) (minfo: MethInfo) =
        match minfo.ArbitraryValRef with
        | Some vref ->
            HashTastMemberOrVals.hashValOrMemberNoInst g vref
        | None ->
            minfo.ComputeHashCode()

//-------------------------------------------------------------------------

/// Printing TAST objects
module TashDefinitionHashes = 
    open PrintTypes

    let hashExtensionMember (g:TcGlobals) (vref: ValRef) =
        HashTastMemberOrVals.hashValOrMemberNoInst g vref

    let hashExtensionMembers (g:TcGlobals) vs =
        vs 
        |> hashAllVia (hashExtensionMember g)      

    let hashRecdField (g:TcGlobals) (fld: RecdField) =
        let nameHash = hashText fld.DisplayNameCore
        let attribHash = hashAttributeList g  fld.FieldAttribs @@ hashAttributeList g fld.PropertyAttribs
        let typeHash = hashTType g fld.FormalType

        let combined = nameHash @@ attribHash @@ typeHash @@ (hash fld.IsStatic) @@ (hash fld.IsVolatile) @@ (hash fld.IsMutable)        

        hashAccessibility fld.Accessibility combined


    //let hashUnionOrExceptionField (g:TcGlobals) infoReader isGenerated enclosingTcref i (fld: RecdField) =
        
    //    if isGenerated i fld then
    //        hashTType (* denv *) SimplifyTypes.typeSimplificationInfo0 2 fld.FormalType
    //    else
    //        hashRecdField id false (* denv *) infoReader enclosingTcref fld
    
    //let isGeneratedUnionCaseField pos (f: RecdField) = 
    //    if pos < 0 then f.LogicalName = "Item"
    //    else f.LogicalName = "Item" + string (pos + 1)

    //let isGeneratedExceptionField pos (f: RecdField) = 
    //    f.LogicalName = "Data" + (string pos)

    //let hashUnionCaseFields (* denv *) infoReader isUnionCase enclosingTcref fields = 
    //    match fields with
    //    | [f] when isUnionCase ->
    //        hashUnionOrExceptionField (* denv *) infoReader isGeneratedUnionCaseField enclosingTcref -1 f
    //    | _ -> 
    //        let isGenerated = if isUnionCase then isGeneratedUnionCaseField else isGeneratedExceptionField
    //        sepListL WordL.star (List.mapi (hashUnionOrExceptionField (* denv *) infoReader isGenerated enclosingTcref) fields)

    let hashUnionCase (g:TcGlobals) (ucase: UnionCase) =
        let nameHash = hashText ucase.Id.idText
        let attribHash = hashAttributeList g ucase.Attribs

        ucase.RecdFieldsArray
        |> hashArrayVia (fun rf -> hashRecdField g rf)
        |> pipeToHash nameHash
        |> pipeToHash attribHash

    let hashUnionCases (g:TcGlobals)  ucases =
        ucases
        |> hashAllVia (hashUnionCase g)
      
    let hashILFieldInfo (finfo: ILFieldInfo) =    
        finfo.ComputeHashCode() @@ HashIL.hashILType  finfo.ILFieldType

    let hashEventInfo (g:TcGlobals) (einfo: EventInfo) =
        einfo.ComputeHashCode() @@
        ( match einfo.ArbitraryValRef with | Some vref -> HashTastMemberOrVals.hashValOrMemberNoInst g vref | _ -> -1)

    let hashPropInfo (g:TcGlobals) (pinfo: PropInfo) =
        let getterHash = 
            match pinfo.HasGetter, pinfo.GetterMethod.ArbitraryValRef with
            | true, Some avr -> hashAccessibility avr.Accessibility 17
            | _ -> 0
        let setterHash = 
            match pinfo.HasSetter, pinfo.SetterMethod.ArbitraryValRef with
            | true, Some avr -> hashAccessibility avr.Accessibility 19
            | _ -> 0 


        if getterHash = 0 && setterHash = 0 then
            0
        else
            (hash pinfo.IsIndexer @@ getterHash @@ setterHash @@ hash pinfo.IsStatic)
            @@
            match pinfo.ArbitraryValRef with
            | Some vref -> HashTastMemberOrVals.hashValOrMemberNoInst g vref         
            | None ->
                pinfo.ComputeHashCode()

    let hashTyconDefn ((* denv *): DisplayEnv) (infoReader: InfoReader) ad m simplified isFirstType (tcref: TyconRef) =        
        let g = (* denv *).g
        // use 4-indent 
        let (-*) = if true then (-----) else (---)
        let (@@*) = if true then (@@----) else (@@--)
        let amap = infoReader.amap
        let tycon = tcref.Deref
        let repr = tycon.TypeReprInfo
        let isMeasure = (tycon.TypeOrMeasureKind = TyparKind.Measure)
        let ty = generalizedTyconRef g tcref 

        let start, tagger =
            if isStructTy g ty && not tycon.TypeAbbrev.IsSome then
                // Always show [<Struct>] whether verbose or not
                Some "struct", tagStruct
            elif isInterfaceTy g ty then
                if true then
                    Some "interface", tagInterface
                else
                    None, tagInterface
            elif isMeasure then
                None, tagClass
            elif isClassTy g ty then
                if true then
                    (if simplified then None else Some "class"), tagClass
                else
                    None, tagClass
            else
                None, tagUnknownType

        let typehashText =
            if isFirstType then
                WordL.keywordType
            else
                hashText ((* string to tag was here *) "and") ^^ hashAttribs (* denv *) start false tycon.TypeOrMeasureKind tycon.Attribs 0 (* empty hash *)

        let nameL = ConvertLogicalNameToDisplayLayout (tagger >> mkNav tycon.DefinitionRange >> hashText) tycon.DisplayNameCore

        let nameL = hashAccessibility (* denv *) tycon.Accessibility nameL
        let (* denv *) = (* denv *).AddAccessibility tycon.Accessibility 

        let lhsL =
            let tps = tycon.TyparsNoRange
            let tpsL = hashTyparDecls (* denv *) nameL tycon.IsPrefixDisplay tps
            typehashText ^^ tpsL


        let sortKey (minfo: MethInfo) = 
            (not minfo.IsConstructor,
             not minfo.IsInstance, // instance first
             minfo.DisplayNameCore, // sort by name 
             List.sum minfo.NumArgs, // sort by #curried
             minfo.NumArgs.Length)     // sort by arity 

        let shouldShow (vrefOpt: ValRef option) =
            match vrefOpt with
            | None -> true
            | Some vref ->
                ((* denv *).showObsoleteMembers || not (CheckFSharpAttributesForObsolete (* denv *).g vref.Attribs)) &&
                ((* denv *).showHiddenMembers || not (CheckFSharpAttributesForHidden (* denv *).g vref.Attribs))
                
        let ctors =
            GetIntrinsicConstructorInfosOfType infoReader m ty
            |> List.filter (fun minfo -> IsMethInfoAccessible amap m ad minfo && not minfo.IsClassConstructor && shouldShow minfo.ArbitraryValRef)

        let iimpls =
            if isRecdTy g ty || isUnionTy g ty || tycon.IsStructOrEnumTycon then
                tycon.ImmediateInterfacesOfFSharpTycon
                |> List.filter (fun (_, compgen, _) -> not compgen)
                |> List.map p13
            else 
                GetImmediateInterfacesOfType SkipUnrefInterfaces.Yes g amap m ty

        let iimplsLs =
            iimpls
            |> List.map (fun intfTy -> hashText ((* string to tag was here *) (if isInterfaceTy g ty then "inherit" else "interface")) -* hashType (* denv *) intfTy)

        let props =
            GetImmediateIntrinsicPropInfosOfType (None, ad) g amap m ty
            |> List.filter (fun pinfo -> shouldShow pinfo.ArbitraryValRef)

        let events = 
            infoReader.GetEventInfosOfType(None, ad, m, ty)
            |> List.filter (fun einfo -> shouldShow einfo.ArbitraryValRef && typeEquiv g ty einfo.ApparentEnclosingType)

        let impliedNames = 
            try 
                [ for p in props do 
                    if p.HasGetter then p.GetterMethod.DisplayName
                    if p.HasSetter then p.SetterMethod.DisplayName
                  for e in events do 
                    e.AddMethod.DisplayName 
                    e.RemoveMethod.DisplayName ]
                |> Set.ofList 
            with _ ->
                Set.empty

        let meths =
            GetImmediateIntrinsicMethInfosOfType (None, ad) g amap m ty
            |> List.filter (fun minfo ->
                not minfo.IsClassConstructor &&
                not minfo.IsConstructor &&
                shouldShow minfo.ArbitraryValRef &&
                not (impliedNames.Contains minfo.DisplayName) &&
                IsMethInfoAccessible amap m ad minfo &&
                // Discard method impls such as System.IConvertible.ToBoolean
                not (minfo.IsILMethod && minfo.DisplayName.Contains(".")) &&
                not (minfo.DisplayName.Split('.') |> Array.exists (fun part -> isDiscard part)))

        let ilFields =
            infoReader.GetILFieldInfosOfType (None, ad, m, ty)
            |> List.filter (fun fld -> 
                IsILFieldInfoAccessible g amap m ad fld &&
                not (isDiscard fld.FieldName) &&
                typeEquiv g ty fld.ApparentEnclosingType)

        let ctorLs =
            ctors
            |> List.map (fun ctor -> InfoMemberPrinting.hashMethInfoFSharpStyle infoReader m (* denv *) ctor)

        let methLs = 
            meths
            |> List.groupBy (fun md -> md.DisplayNameCore)
            |> List.collect (fun (_, group) ->
                group
                |> List.sortBy sortKey
                |> List.map (fun methinfo -> ((not methinfo.IsConstructor, methinfo.IsInstance, methinfo.DisplayName, List.sum methinfo.NumArgs, methinfo.NumArgs.Length), InfoMemberPrinting.hashMethInfoFSharpStyle infoReader m (* denv *) methinfo)))
            |> List.sortBy fst
            |> List.map snd

        let ilFieldsL =
            ilFields
            |> List.map (fun x -> (true, x.IsStatic, x.FieldName, 0, 0), hashILFieldInfo (* denv *) infoReader m x)
            |> List.sortBy fst
            |> List.map snd

        let staticVals =
            if isRecdTy g ty then
                []
            else
                tycon.TrueFieldsAsList
                |> List.filter (fun f -> IsAccessible ad f.Accessibility && f.IsStatic && not (isDiscard f.DisplayNameCore))

        let staticValLs =
            staticVals
            |> List.map (fun f -> hashRecdField (fun l -> WordL.keywordStatic ^^ WordL.keywordVal ^^ l) true (* denv *) infoReader tcref f)

        let instanceVals =
            if isRecdTy g ty then
                []
            else
                tycon.TrueInstanceFieldsAsList
                |> List.filter (fun f -> IsAccessible ad f.Accessibility && not (isDiscard f.DisplayNameCore))

        let instanceValLs =
            instanceVals
            |> List.map (fun f -> hashRecdField (fun l -> WordL.keywordVal ^^ l) true (* denv *) infoReader tcref f)
    
        let propLs =
            props
            |> List.map (fun x -> (true, x.IsStatic, x.PropertyName, 0, 0), hashPropInfo (* denv *) infoReader m x)
            |> List.sortBy fst
            |> List.map snd

        let eventLs = 
            events
            |> List.map (fun x -> (true, x.IsStatic, x.EventName, 0, 0), hashEventInfo (* denv *) infoReader m x)
            |> List.sortBy fst
            |> List.map snd

        let nestedTypeLs =
#if !NO_TYPEPROVIDERS
            match tryTcrefOfAppTy g ty with
            | ValueSome tcref ->
                match tcref.TypeReprInfo with 
                | TProvidedTypeRepr info ->
                    [ 
                        for nestedType in info.ProvidedType.PApplyArray((fun sty -> sty.GetNestedTypes() |> Array.filter (fun t -> t.IsPublic || t.IsNestedPublic)), "GetNestedTypes", m) do 
                            yield nestedType.PUntaint((fun t -> t.IsClass, t.Name), m)
                    ] 
                    |> List.sortBy snd
                    |> List.map (fun (isClass, t) -> WordL.keywordNested ^^ WordL.keywordType ^^ hashText ((if isClass then tagClass else tagStruct) t))
                | _ ->
                    []
            | ValueNone ->
#endif
                []

        let inherits = 
            [ 
                match GetSuperTypeOfType g amap m ty with 
                | Some superTy when not (isObjTy g superTy) && not (isValueTypeTy g superTy) ->
                    superTy
                | _ -> ()
            ]

        let inheritsL = 
            inherits
            |> List.map (fun super -> hashText ((* string to tag was here *) "inherit") ^^ (hashType (* denv *) super))

        let allDecls = inheritsL @ iimplsLs @ ctorLs @ instanceValLs @ methLs @ ilFieldsL @ propLs @ eventLs @ staticValLs @ nestedTypeLs

        let needsStartEnd =
            match start with 
            | Some "class" ->
                // 'inherits' is not enough for F# type kind inference to infer a class
                // inherits.IsEmpty &&
                ilFields.IsEmpty &&
                // 'abstract' is not enough for F# type kind inference to infer a class by default in signatures
                // 'static member' is surprisingly not enough for F# type kind inference to infer a class by default in signatures
                // 'overrides' is surprisingly not enough for F# type kind inference to infer a class by default in signatures
                //(meths |> List.forall (fun m -> m.IsAbstract || m.IsDefiniteFSharpOverride || not m.IsInstance)) &&
                //(props |> List.forall (fun m -> (not m.HasGetter || m.GetterMethod.IsAbstract))) &&
                //(props |> List.forall (fun m -> (not m.HasSetter || m.SetterMethod.IsAbstract))) &&
                ctors.IsEmpty &&
                instanceVals.IsEmpty &&
                staticVals.IsEmpty
            | Some "struct" -> 
                true
            | Some "interface" -> 
                meths.IsEmpty &&
                props.IsEmpty
            | _ ->
                false

        let start = if needsStartEnd then start else None
        
        let addMaxMembers reprL =
            if isNil allDecls then
                reprL
            else
                let memberLs = allDecls
                reprL @@ combineHashes memberLs

        let addReprAccessL l =
            hashAccessibility (* denv *) tycon.TypeReprAccessibility l 

        let addReprAccessRecord l =
            hashAccessibilityCore (* denv *) tycon.TypeReprAccessibility --- l
        
        let addLhs rhsL =
            let brk = not (isNil allDecls) || breakTypeDefnEqn repr
            if brk then 
                (lhsL ^^ WordL.equals) @@* rhsL 
            else 
                (lhsL ^^ WordL.equals) -* rhsL

        let typeDeclL = 

            match repr with 
            | TFSharpRecdRepr _ ->
                let (* denv *) = (* denv *).AddAccessibility tycon.TypeReprAccessibility 

                // For records, use multi-line hash as soon as there is XML doc 
                //   type R =
                //     { 
                //         /// ABC
                //         Field1: int 
                //
                //         /// ABC
                //         Field2: int 
                //     }
                //
                // For records, use multi-line hash as soon as there is more than one field
                //   type R =
                //     { 
                //         Field1: int 
                //         Field2: int 
                //     }
                let useMultiLine =
                    let members =
                        match (* denv *).maxMembers with 
                        | None -> tycon.TrueFieldsAsList
                        | Some n -> tycon.TrueFieldsAsList |> List.truncate n
                    members.Length > 1 ||
                    members |> List.exists (fun m -> not m.XmlDoc.IsEmpty)

                tycon.TrueFieldsAsList
                |> List.map (hashRecdField id false (* denv *) infoReader tcref)           
                |> combineHashes
                |> (if useMultiLine then braceMultiLineL else braceL)
                |> addReprAccessRecord
                |> addMaxMembers
                |> addLhs

            | TFSharpUnionRepr _ -> 
                let (* denv *) = (* denv *).AddAccessibility tycon.TypeReprAccessibility 
                tycon.UnionCasesAsList
                |> hashUnionCases (* denv *) infoReader tcref           
                |> combineHashes
                |> addReprAccessL
                |> addMaxMembers
                |> addLhs
                  
            | TFSharpObjectRepr { fsobjmodel_kind = TFSharpDelegate slotSig } ->
                let (TSlotSig(_, _, _, _, paraml, retTy)) = slotSig
                let retTy = GetFSharpViewOfReturnType (* denv *).g retTy
                let delegateL = WordL.keywordDelegate ^^ WordL.keywordOf -* hashTopType (* denv *) SimplifyTypes.typeSimplificationInfo0 (paraml |> List.mapSquared (fun sp -> (sp.Type, ValReprInfo.unnamedTopArg1))) retTy []
                delegateL
                |> addLhs

            // Measure declarations are '[<Measure>] type kg' unless abbreviations
            | TFSharpObjectRepr _ when isMeasure ->
                lhsL

            | TFSharpObjectRepr { fsobjmodel_kind = TFSharpEnum } ->
                tycon.TrueFieldsAsList
                |> List.map (fun f -> 
                    match f.LiteralValue with 
                    | None -> 0 (* empty hash *)
                    | Some c ->
                        WordL.bar ^^
                        hashText (tagField f.DisplayName) ^^
                        WordL.equals ^^ 
                        hashConst (* denv *).g ty c)
                |> combineHashes
                |> addLhs

            | TFSharpObjectRepr objRepr when isNil allDecls ->
                match objRepr.fsobjmodel_kind with
                | TFSharpClass ->
                    WordL.keywordClass ^^ WordL.keywordEnd
                    |> addLhs
                | TFSharpInterface ->
                    WordL.keywordInterface ^^ WordL.keywordEnd
                    |> addLhs
                | TFSharpStruct ->
                    WordL.keywordStruct ^^ WordL.keywordEnd
                    |> addLhs
                | _ -> lhsL

            | TFSharpObjectRepr _ ->
                allDecls          
                |> combineHashes
                |> addLhs

            | TAsmRepr _ -> 
                let asmL = hashText (tagText "(# \"<Common IL Type Omitted>\" #)")
                asmL
                |> addLhs

            | TMeasureableRepr ty ->
                hashType (* denv *) ty
                |> addLhs

            | TILObjectRepr _ when tycon.ILTyconRawMetadata.IsEnum ->
                infoReader.GetILFieldInfosOfType (None, ad, m, ty) 
                |> List.filter (fun x -> x.FieldName <> "value__")
                |> List.map (fun x -> hash x.FieldName)
                |> combineHashes
                |> addLhs

            | TILObjectRepr _ ->
                allDecls          
                |> combineHashes
                |> addLhs

            | TNoRepr when tycon.TypeAbbrev.IsSome ->
                let abbreviatedTy = tycon.TypeAbbrev.Value
                (lhsL ^^ WordL.equals) -* (hashType { (* denv *) with shortTypeNames = false } abbreviatedTy)

            | _ when isNil allDecls ->
                lhsL
#if !NO_TYPEPROVIDERS
            | TProvidedNamespaceRepr _
            | TProvidedTypeRepr _
#endif
            | TNoRepr -> 
                allDecls      
                |> combineHashes
                |> addLhs

        typeDeclL 
        |> fun tdl -> if isFirstType then hashAttribs (* denv *) start false tycon.TypeOrMeasureKind tycon.Attribs tdl else tdl  

    // Hash: exception definition
    let hashExnDefn (* denv *) infoReader (exncref: EntityRef) =
        let (-*) = if true then (-----) else (---)
        let exnc = exncref.Deref
        let nameL = ConvertLogicalNameToDisplayLayout (tagClass >> mkNav exncref.DefinitionRange >> hashText) exnc.DisplayNameCore
        let nameL = hashAccessibility (* denv *) exnc.TypeReprAccessibility nameL
        let exnL = hashText ((* string to tag was here *) "exception") ^^ nameL // need to tack on the Exception at the right of the name for goto definition
        let reprL = 
            match exnc.ExceptionInfo with 
            | TExnAbbrevRepr ecref -> WordL.equals -* hashTyconRef (* denv *) ecref
            | TExnAsmRepr _ -> WordL.equals -* hashText (tagText "(# ... #)")
            | TExnNone -> 0 (* empty hash *)
            | TExnFresh r -> 
                match r.TrueFieldsAsList with
                | [] -> 0 (* empty hash *)
                | r -> WordL.keywordOf -* hashUnionCaseFields (* denv *) infoReader false exncref r

        let overallL = exnL ^^ reprL
        overallL

    // Hash: module spec 

    let hashTyconDefns (* denv *) infoReader ad m (tycons: Tycon list) =
        match tycons with 
        | [] -> 0 (* empty hash *)
        | [h] when h.IsFSharpException -> hashExnDefn (* denv *) infoReader (mkLocalEntityRef h)
        | h :: t -> 
            let x = hashTyconDefn (* denv *) infoReader ad m false true (mkLocalEntityRef h)
            let xs = List.map (mkLocalEntityRef >> hashTyconDefn (* denv *) infoReader ad m false false) t
            combineHashes (x :: xs)

    let rec fullPath (mspec: ModuleOrNamespace) acc =
        if mspec.IsNamespace then
            match mspec.ModuleOrNamespaceType.ModuleAndNamespaceDefinitions |> List.tryHead with
            | Some next when next.IsNamespace ->
                fullPath next (acc @ [next.DisplayNameCore])
            | _ ->
                acc, mspec
        else
            acc, mspec


open PrintTypes

/// Hash the inferred signature of a compilation unit
let calculateHashOfImpliedSignature (infoReader:InfoReader) (ad:AccessorDomain) (m:range) (expr:ModuleOrNamespaceContents) =

    let rec isConcreteNamespace x = 
        match x with 
        | TMDefRec(_, _opens, tycons, mbinds, _) -> 
            not (isNil tycons) || (mbinds |> List.exists (function ModuleOrNamespaceBinding.Binding _ -> true | ModuleOrNamespaceBinding.Module(x, _) -> not x.IsNamespace))
        | TMDefLet _ -> true
        | TMDefDo _ -> true
        | TMDefOpens _ -> false
        | TMDefs defs -> defs |> List.exists isConcreteNamespace 

    let rec imdefsL (* denv *) x = combineHashes (x |> List.map imdefL )

    and imdefL  x = 
        let filterVal (v: Val) = not v.IsCompilerGenerated && Option.isNone v.MemberInfo
        let filterExtMem (v: Val) = v.IsExtensionMember

        match x with 
        | TMDefRec(_, _opens, tycons, mbinds, _) -> 
            TashDefinitionHashes.hashTyconDefns (* denv *) infoReader ad m tycons @@ 
            (mbinds 
                |> List.choose (function ModuleOrNamespaceBinding.Binding bind -> Some bind | _ -> None) 
                |> valsOfBinds 
                |> List.filter filterExtMem
                |> List.map mkLocalValRef
                |> TashDefinitionHashes.hashExtensionMembers (* denv *) infoReader) @@

            (mbinds 
                |> List.choose (function ModuleOrNamespaceBinding.Binding bind -> Some bind | _ -> None) 
                |> valsOfBinds 
                |> List.filter filterVal
                |> List.map mkLocalValRef
                |> List.map (HashTastMemberOrVals.hashValOrMemberNoInst (* denv *) infoReader)
                |> combineHashes) @@

            (mbinds 
                |> List.choose (function ModuleOrNamespaceBinding.Module (mspec, def) -> Some (mspec, def) | _ -> None) 
                |> List.map (imbindL (* denv *)) 
                |> combineHashes)

        | TMDefLet(bind, _) -> 
            ([bind.Var] 
                |> List.filter filterVal 
                |> List.map mkLocalValRef
                |> List.map (HashTastMemberOrVals.hashValOrMemberNoInst (* denv *) infoReader) 
                |> combineHashes)

        | TMDefOpens _ -> 0 (* empty hash *)

        | TMDefs defs -> imdefsL (* denv *) defs

        | TMDefDo _ -> 0 (* empty hash *)

    and imbindL (* denv *) (mspec, def) = 
        let innerPath = (fullCompPathOfModuleOrNamespace mspec).AccessPath
        let outerPath = mspec.CompilationPath.AccessPath


        if mspec.IsImplicitNamespace then
            // The current mspec is a namespace that belongs to the `def` child (nested) module(s).                
            let fullModuleName, def, (* denv *) =
                let rec (|NestedModule|_|) (currentContents:ModuleOrNamespaceContents) =
                    match currentContents with
                    | ModuleOrNamespaceContents.TMDefRec (bindings = [ ModuleOrNamespaceBinding.Module(mn, NestedModule(path, contents)) ]) ->
                        Some ([ yield mn.DisplayNameCore; yield! path ], contents)
                    | ModuleOrNamespaceContents.TMDefs [ ModuleOrNamespaceContents.TMDefRec (bindings = [ ModuleOrNamespaceBinding.Module(mn, NestedModule(path, contents)) ]) ] ->
                        Some ([ yield mn.DisplayNameCore; yield! path ], contents)
                    | ModuleOrNamespaceContents.TMDefs [ ModuleOrNamespaceContents.TMDefRec (bindings = [ ModuleOrNamespaceBinding.Module(mn, nestedModuleContents) ]) ] ->
                        Some ([ mn.DisplayNameCore ], nestedModuleContents)
                    | _ ->
                        None

                match def with
                | NestedModule(path, nestedModuleContents) ->
                    let fullPath = mspec.DisplayNameCore :: path
                    fullPath, nestedModuleContents, (* denv *).AddOpenPath(fullPath)
                | _ -> [ mspec.DisplayNameCore ], def, (* denv *)
                
            let nmL = List.map (tagModule >> hashText) fullModuleName |> sepListL SepL.dot
            let nmL = hashAccessibility (* denv *) mspec.Accessibility nmL
            let (* denv *) = (* denv *).AddAccessibility mspec.Accessibility
            let basic = imdefL (* denv *) def
            let modNameL = hashText ((* string to tag was here *) "module") ^^ nmL
            let basicL = modNameL @@ basic
            basicL
        elif mspec.IsNamespace then
            let basic = imdefL (* denv *) def
            let basicL =
                // Check if this namespace contains anything interesting
                if isConcreteNamespace def then
                    let pathL = innerPath |> List.map (fst >> ConvertLogicalNameToDisplayLayout (tagNamespace >> hashText))
                    // This is a container namespace. We print the header when we get to the first concrete module.
                    let headerL =
                        hashText ((* string to tag was here *) "namespace") ^^ sepListL SepL.dot pathL
                    headerL @@* basic
                else
                    // This is a namespace that only contains namespaces. Skip the header
                    basic
            // NOTE: explicitly not calling `hashXmlDoc` here, because even though
            // `ModuleOrNamespace` has a field for XmlDoc, it is never present at the parser
            // level for namespaces.  This should be changed if the parser/spec changes.
            basicL
        else
            // This is a module 
            let nmL = ConvertLogicalNameToDisplayLayout (tagModule >> mkNav mspec.DefinitionRange >> hashText) mspec.DisplayNameCore
            let nmL = hashAccessibility (* denv *) mspec.Accessibility nmL
            let (* denv *) = (* denv *).AddAccessibility mspec.Accessibility
            let basic = imdefL (* denv *) def
            let modNameL =
                hashText ((* string to tag was here *) "module") ^^ nmL
                |> hashAttribs (* denv *) None false mspec.TypeOrMeasureKind mspec.Attribs
            let modNameEqualsL = modNameL ^^ WordL.equals
            let isNamespace = function | Namespace _ -> true | _ -> false
            let modIsOuter = (outerPath |> List.forall (fun (_, istype) -> isNamespace istype) )
            let basicL =
                // Check if its an outer module or a nested module
                if modIsOuter then
                    // OK, this is an outer module
                    if showHeader then
                        // OK, we're not in F# Interactive
                        // Check if this is an outer module with no namespace
                        if isNil outerPath then
                            // If so print a "module" declaration, no indentation
                            modNameL @@ basic
                        else
                            // Otherwise this is an outer module contained immediately in a namespace
                            // We already printed the namespace declaration earlier. So just print the
                            // module now.
                            if isEmptyL basic then 
                                modNameEqualsL ^^ WordL.keywordBegin ^^ WordL.keywordEnd
                            else
                                modNameEqualsL @@* basic
                    else
                        // OK, we're in F# Interactive, presumably the implicit module for each interaction.
                        basic
                else
                    // OK, this is a nested module, with indentation
                    if isEmptyL basic then 
                        ((modNameEqualsL ^^ hashText ((* string to tag was here *)"begin")) @@* basic) @@ WordL.keywordEnd
                    else
                        modNameEqualsL @@* basic
            basicL

    let emptyModuleOrNamespace mspec =
        let innerPath = (fullCompPathOfModuleOrNamespace mspec).AccessPath
        let pathL = innerPath |> List.map (fst >> ConvertLogicalNameToDisplayLayout (tagNamespace >> hashText))

        let keyword =
            if not mspec.IsImplicitNamespace && mspec.IsNamespace then
                "namespace"
            else
                "module"

        hashText ((* string to tag was here *) keyword) ^^ sepListL SepL.dot pathL

    match expr with
    | EmptyModuleOrNamespaces mspecs when showHeader ->
        List.map emptyModuleOrNamespace mspecs
        |> aboveListL
    | expr -> imdefL denv expr

//--------------------------------------------------------------------------
