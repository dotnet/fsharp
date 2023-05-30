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
        acc <- addToHash acc (func i)
    acc
let inline hashArrayVia func (items: 'T[]) : Hash =
    let mutable acc = 0
    for i in items do
        acc <- addToHash acc (func i)
    acc
let (@@) (h1:Hash) (h2:Hash) = combineHash h1 h2
let (^^) (h1:Hash) (h2:Hash) = combineHash h1 h2


[<AutoOpen>]
module internal HashUtilities =   
 
    let hashCurriedFunc (argTysL: Hash list) (retTyL: Hash) =
        combineHashes argTysL
        |> pipeToHash retTyL
    
    let hashEntityRefName (g:TcGlobals) (xref: EntityRef) name =    
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


    let hashBuiltinAttribute g (attrib: BuiltinAttribInfo) = hashTyconRefImpl g attrib.TyconRef
        
module HashIL = 

    let hashILTypeRef (tref: ILTypeRef) =
        tref.Enclosing
        |> hashAllVia hashText      
        |> hashAndAdd tref.Name

    let hashILArrayShape ( sh:ILArrayShape) = sh.Rank  

    let paramsL (ps: Hash list) : Hash = ps |> combineHashes 
 
    let rec hashILType (ilTyparSubst: Hash list) (ty: ILType) : Hash =
        match ty with
        | ILType.Void -> hash ILType.Void
        | ILType.Array (sh, t) -> hashILType ilTyparSubst t ^^ hashILArrayShape sh
        | ILType.Value t
        | ILType.Boxed t -> hashILTypeRef  t.TypeRef ^^ (t.GenericArgs |> hashAllVia (hashILType ilTyparSubst))
        | ILType.Ptr t
        | ILType.Byref t -> hashILType  ilTyparSubst t
        | ILType.FunctionPointer t -> hashILCallingSignature ilTyparSubst None t
        | ILType.TypeVar n -> List.item (int n) ilTyparSubst
        | ILType.Modified (_, _, t) -> hashILType ilTyparSubst t

    and hashILCallingSignature (* denv *) ilTyparSubst cons (signature: ILCallingSignature) =       
        let res = signature.ReturnType |> hashILType  ilTyparSubst   
        signature.ArgTypes
        |> hashAllVia (hashILType ilTyparSubst) 
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


    let hashILAttrib (ty, args) = HashIL.hashILType [] ty
   
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
    let hashMeasure unt =
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

    /// Hash type parameters
    let hashTyparDecls (g:TcGlobals) (typars: Typars) =      
        let tpcs = typars |> List.collect (fun tp -> List.map (fun tpc -> tp, tpc) tp.Constraints) 
        tpcs 
        |> hashAllVia (hashConstraint g)  

    let hashTrait (g:TcGlobals) traitInfo =
        hashTraitWithInfo g  traitInfo

    //let hashTyparConstraint (g:TcGlobals)  (tp, tpc) =
    //    hashConstraint g (tp, tpc)  

    let prettyLayoutOfInstAndSig (* denv *) (typarInst, tys, retTy) =
        let (prettyTyparInst, prettyTys, prettyRetTy), cxs = PrettyTypes.PrettifyInstAndSig (* denv *).g (typarInst, tys, retTy)
        let env = SimplifyTypes.CollectInfo true (prettyRetTy :: prettyTys) cxs
        let prettyTysL = List.map (hashTypeWithInfo (* denv *) env) prettyTys
        let prettyRetTyL = hashTopType (* denv *) env [[]] prettyRetTy []
        prettyTyparInst, (prettyTys, prettyRetTy), (prettyTysL, prettyRetTyL), hashConstraints (* denv *) env env.postfixConstraints

    let prettyLayoutOfTopTypeInfoAux (* denv *) prettyArgInfos prettyRetTy cxs = 
        let env = SimplifyTypes.CollectInfo true (prettyRetTy :: List.collect (List.map fst) prettyArgInfos) cxs
        hashTopType (* denv *) env prettyArgInfos prettyRetTy env.postfixConstraints

    // Oddly this is called in multiple places with argInfos=[] and (* denv *).useColonForReturnType=true, as a complex
    // way of giving give ": ty"
    let prettyLayoutOfUncurriedSig (* denv *) typarInst argInfos retTy = 
        let (prettyTyparInst, prettyArgInfos, prettyRetTy), cxs = PrettyTypes.PrettifyInstAndUncurriedSig (* denv *).g (typarInst, argInfos, retTy)
        prettyTyparInst, prettyLayoutOfTopTypeInfoAux (* denv *) [prettyArgInfos] prettyRetTy cxs

    let prettyLayoutOfCurriedMemberSig (* denv *) typarInst argInfos retTy parentTyparTys = 
        let (prettyTyparInst, parentTyparTys, argInfos, retTy), cxs = PrettyTypes.PrettifyInstAndCurriedSig (* denv *).g (typarInst, parentTyparTys, argInfos, retTy)
        // Filter out the parent typars, which don't get shown in the member signature 
        let cxs = cxs |> List.filter (fun (tp, _) -> not (parentTyparTys |> List.exists (fun ty -> match tryDestTyparTy (* denv *).g ty with ValueSome destTypar -> typarEq tp destTypar | _ -> false))) 
        prettyTyparInst, prettyLayoutOfTopTypeInfoAux (* denv *) argInfos retTy cxs

    let prettyArgInfos (* denv *) allTyparInst =
        function 
        | [] -> [((* denv *).g.unit_ty, ValReprInfo.unnamedTopArg1)] 
        | infos -> infos |> List.map (map1Of2 (instType allTyparInst)) 

    // Hash: type spec - class, datatype, record, abbrev 
    let prettyLayoutOfMemberSigCore (* denv *) memberToParentInst (typarInst, methTypars: Typars, argInfos, retTy) = 
        let niceMethodTypars, allTyparInst = 
            let methTyparNames = methTypars |> List.mapi (fun i tp -> if (PrettyTypes.NeedsPrettyTyparName tp) then sprintf "a%d" (List.length memberToParentInst + i) else tp.Name)
            PrettyTypes.NewPrettyTypars memberToParentInst methTypars methTyparNames

        let retTy = instType allTyparInst retTy
        let argInfos = argInfos |> List.map (prettyArgInfos (* denv *) allTyparInst) 

        // Also format dummy types corresponding to any type variables on the container to make sure they 
        // aren't chosen as names for displayed variables. 
        let memberParentTypars = List.map fst memberToParentInst
        let parentTyparTys = List.map (mkTyparTy >> instType allTyparInst) memberParentTypars
        let prettyTyparInst, hash = prettyLayoutOfCurriedMemberSig (* denv *) typarInst argInfos retTy parentTyparTys

        prettyTyparInst, niceMethodTypars, hash

    let prettyLayoutOfMemberType (* denv *) vref typarInst argInfos retTy = 
        match PartitionValRefTypars (* denv *).g vref with
        | Some(_, _, memberMethodTypars, memberToParentInst, _) ->
            prettyLayoutOfMemberSigCore (* denv *) memberToParentInst (typarInst, memberMethodTypars, argInfos, retTy)
        | None -> 
            let prettyTyparInst, hash = prettyLayoutOfUncurriedSig (* denv *) typarInst (List.concat argInfos) retTy 
            prettyTyparInst, [], hash

    let prettyLayoutOfMemberSig (* denv *) (memberToParentInst, nm, methTypars, argInfos, retTy) = 
        let _, niceMethodTypars, tauL = prettyLayoutOfMemberSigCore (* denv *) memberToParentInst (emptyTyparInst, methTypars, argInfos, retTy)
        let nameL = ConvertValLogicalNameToDisplayLayout false (tagMember >> hashText) nm
        let nameL =
            if (* denv *).showTyparBinding then
                hashTyparDecls (* denv *) nameL true niceMethodTypars
            else
                nameL
        (nameL |> addColonL) ^^ tauL

    /// hashs the elements of an unresolved overloaded method call:
    /// argInfos: unammed and named arguments
    /// retTy: return type
    /// genParamTy: generic parameter types
    let prettyLayoutsOfUnresolvedOverloading (* denv *) argInfos retTy genParamTys =

        let _niceMethodTypars, typarInst =
            let memberToParentInst = List.empty
            let typars = argInfos |> List.choose (function TType_var (typar, _),_ -> Some typar | _ -> None)
            let methTyparNames = typars |> List.mapi (fun i tp -> if (PrettyTypes.NeedsPrettyTyparName tp) then sprintf "a%d" (List.length memberToParentInst + i) else tp.Name)
            PrettyTypes.NewPrettyTypars memberToParentInst typars methTyparNames

        let retTy = instType typarInst retTy
        let argInfos = prettyArgInfos (* denv *) typarInst argInfos
        let argInfos,retTy,genParamTys, cxs =
            // using 0, 1, 2 as discriminant for return, arguments and generic parameters
            // respectively, in order to easily retrieve each of the types with their
            // expected quality below.
            let typesWithDiscrimants =
                [
                    yield 0, retTy 
                    for ty,_ in argInfos do
                        yield 1, ty
                    for ty in genParamTys do
                        yield 2, ty
                ]
            let typesWithDiscrimants,typarsAndCxs = PrettyTypes.PrettifyDiscriminantAndTypePairs (* denv *).g typesWithDiscrimants
            let retTy = typesWithDiscrimants |> List.find (function 0, _ -> true | _ -> false) |> snd
            let argInfos = 
                typesWithDiscrimants
                |> List.choose (function 1,ty -> Some ty | _ -> None)
                |> List.map2 (fun (_, argInfo) tTy -> tTy, argInfo) argInfos
            let genParamTys = 
                typesWithDiscrimants
                |> List.choose (function 2,ty -> Some ty | _ -> None)
              
            argInfos, retTy, genParamTys, typarsAndCxs

        let env = SimplifyTypes.CollectInfo true (List.collect (List.map fst) [argInfos]) cxs
        let cxsL = hashConstraints (* denv *) env env.postfixConstraints

        (List.foldBack (---) (hashCurriedArgInfos (* denv *) env [argInfos]) cxsL,
            hashReturnType (* denv *) env retTy,
            hashGenericParameterTypes (* denv *) env genParamTys)

    let prettyLayoutOfType (* denv *) ty = 
        let ty, cxs = PrettyTypes.PrettifyType (* denv *).g ty
        let env = SimplifyTypes.CollectInfo true [ty] cxs
        let cxsL = hashConstraints (* denv *) env env.postfixConstraints
        hashTType (* denv *) env 2 ty --- cxsL

    let prettyLayoutOfTrait (* denv *) traitInfo =
        let compgenId = SyntaxTreeOps.mkSynId Range.range0 unassignedTyparName
        let fakeTypar = Construct.NewTypar (TyparKind.Type, TyparRigidity.Flexible, SynTypar(compgenId, TyparStaticReq.None, true), false, TyparDynamicReq.No, [], false, false)
        fakeTypar.SetConstraints [TyparConstraint.MayResolveMember(traitInfo, Range.range0)]
        let ty, cxs = PrettyTypes.PrettifyType (* denv *).g (mkTyparTy fakeTypar)
        let env = SimplifyTypes.CollectInfo true [ty] cxs
        // We expect one constraint, since we put one in.
        match env.postfixConstraints with
        | cx :: _ ->
             // We expect at most one per constraint
             sepListL 0 (* empty hash *) (hashConstraint (* denv *) env cx)
        | [] -> 0 (* empty hash *)

    let prettyLayoutOfTypeNoConstraints (* denv *) ty =
        let ty, _cxs = PrettyTypes.PrettifyType (* denv *).g ty
        hashTType (* denv *) SimplifyTypes.typeSimplificationInfo0 5 ty

    let hashOfValReturnType (* denv *) (vref: ValRef) =
        match vref.ValReprInfo with 
        | None ->
            let tau = vref.TauType
            let _argTysl, retTy = stripFunTy (* denv *).g tau
            hashReturnType (* denv *) SimplifyTypes.typeSimplificationInfo0 retTy
        | Some (ValReprInfo(_typars, argInfos, _retInfo)) -> 
            let tau = vref.TauType
            let _c, retTy = GetTopTauTypeInFSharpForm (* denv *).g argInfos tau Range.range0
            hashReturnType (* denv *) SimplifyTypes.typeSimplificationInfo0 retTy

    let hashAssemblyName _(* denv *) (ty: TType) =
        ty.GetAssemblyName()

/// Printing TAST objects
module PrintTastMemberOrVals =
    open PrintTypes 

    let mkInlineL (* denv *) (v: Val) nameL = 
        if v.MustInline && not (* denv *).suppressInlineKeyword then 
            hashText ((* string to tag was here *) "inline") ++ nameL 
        else 
            nameL

    let hashMemberName ((* denv *): DisplayEnv) (vref: ValRef) niceMethodTypars tagFunction name =
        let nameL = ConvertValLogicalNameToDisplayLayout vref.IsBaseVal (tagFunction >> mkNav vref.DefinitionRange >> hashText) name
        let nameL =
            if (* denv *).showMemberContainers then 
                hashTyconRef (* denv *) vref.MemberApparentEntity ^^ SepL.dot ^^ nameL
            else
                nameL
        let nameL = if (* denv *).showTyparBinding then hashTyparDecls (* denv *) nameL true niceMethodTypars else nameL
        let nameL = hashAccessibility (* denv *) vref.Accessibility nameL
        nameL

    let prettyLayoutOfMemberShortOption (* denv *) typarInst (v: Val) short =
        let vref = mkLocalValRef v
        let membInfo = Option.get vref.MemberInfo
        let stat = hashMemberFlags membInfo.MemberFlags
        let _tps, argInfos, retTy, _ = GetTypeOfMemberInFSharpForm (* denv *).g vref
        
        if short then
            for argInfo in argInfos do
                for _,info in argInfo do
                    info.Attribs <- []
                    info.Name <- None

        let prettyTyparInst, memberL =
            match membInfo.MemberFlags.MemberKind with
            | SynMemberKind.Member ->
                let prettyTyparInst, niceMethodTypars,tauL = prettyLayoutOfMemberType (* denv *) vref typarInst argInfos retTy
                let resL =
                    if short then tauL
                    else
                        let nameL = hashMemberName (* denv *) vref niceMethodTypars tagMember vref.DisplayNameCoreMangled
                        let nameL = if short then nameL else mkInlineL (* denv *) vref.Deref nameL
                        stat --- ((nameL  |> addColonL) ^^ tauL)
                prettyTyparInst, resL

            | SynMemberKind.ClassConstructor
            | SynMemberKind.Constructor ->
                let prettyTyparInst, _, tauL = prettyLayoutOfMemberType (* denv *) vref typarInst argInfos retTy
                let resL = 
                    if short then tauL
                    else
                        let newL = hashAccessibility (* denv *) vref.Accessibility WordL.keywordNew
                        stat ++ (newL |> addColonL) ^^ tauL
                prettyTyparInst, resL

            | SynMemberKind.PropertyGetSet ->
                emptyTyparInst, stat

            | SynMemberKind.PropertyGet ->
                if isNil argInfos then
                    // use error recovery because intellisense on an incomplete file will show this
                    errorR(Error(FSComp.SR.tastInvalidFormForPropertyGetter(), vref.Id.idRange))
                    let nameL = hashMemberName (* denv *) vref [] tagProperty vref.DisplayNameCoreMangled
                    let resL =
                        if short then nameL --- (WordL.keywordWith ^^ WordL.keywordGet)
                        else stat --- nameL --- (WordL.keywordWith ^^ WordL.keywordGet)
                    emptyTyparInst, resL
                else
                    let argInfos =
                        match argInfos with
                        | [[(ty, _)]] when isUnitTy (* denv *).g ty -> []
                        | _ -> argInfos
                    let prettyTyparInst, niceMethodTypars,tauL = prettyLayoutOfMemberType (* denv *) vref typarInst argInfos retTy
                    let resL =
                        if short then
                            if isNil argInfos then tauL
                            else tauL --- (WordL.keywordWith ^^ WordL.keywordGet)
                        else
                            let nameL = hashMemberName (* denv *) vref niceMethodTypars tagProperty vref.DisplayNameCoreMangled
                            stat --- ((nameL  |> addColonL) ^^ (if isNil argInfos then tauL else tauL --- (WordL.keywordWith ^^ WordL.keywordGet)))
                    prettyTyparInst, resL

            | SynMemberKind.PropertySet ->
                if argInfos.Length <> 1 || isNil argInfos.Head then
                    // use error recovery because intellisense on an incomplete file will show this
                    errorR(Error(FSComp.SR.tastInvalidFormForPropertySetter(), vref.Id.idRange))
                    let nameL = hashMemberName (* denv *) vref [] tagProperty vref.DisplayNameCoreMangled
                    let resL = stat --- nameL --- (WordL.keywordWith ^^ WordL.keywordSet)
                    emptyTyparInst, resL
                else
                    let argInfos, valueInfo = List.frontAndBack argInfos.Head
                    let prettyTyparInst, niceMethodTypars, tauL = prettyLayoutOfMemberType (* denv *) vref typarInst (if isNil argInfos then [] else [argInfos]) (fst valueInfo)
                    let resL =
                        if short then
                            (tauL --- (WordL.keywordWith ^^ WordL.keywordSet))
                        else
                            let nameL = hashMemberName (* denv *) vref niceMethodTypars tagProperty vref.DisplayNameCoreMangled
                            stat --- ((nameL |> addColonL) ^^ (tauL --- (WordL.keywordWith ^^ WordL.keywordSet)))
                    prettyTyparInst, resL

        prettyTyparInst, memberL

    let prettyLayoutOfMember (* denv *) typarInst (v:Val) = prettyLayoutOfMemberShortOption (* denv *) typarInst v false

    let prettyLayoutOfMemberNoInstShort (* denv *) v = 
        prettyLayoutOfMemberShortOption (* denv *) emptyTyparInst v true |> snd

    let hashOfLiteralValue literalValue =
        let literalValue =
            match literalValue with
            | Const.Bool value -> if value then WordL.keywordTrue else WordL.keywordFalse
            | Const.SByte _
            | Const.Byte _
            | Const.Int16 _
            | Const.UInt16 _
            | Const.Int32 _
            | Const.UInt32 _
            | Const.Int64 _
            | Const.UInt64 _
            | Const.IntPtr _
            | Const.UIntPtr _
            | Const.Single _
            | Const.Double _
            | Const.Decimal _ -> literalValue.ToString() |> tagNumericLiteral |> hashText
            | Const.Char _
            | Const.String _ -> literalValue.ToString() |> tagStringLiteral |> hashText
            | Const.Unit
            | Const.Zero -> literalValue.ToString() |> tagText |> hashText
        WordL.equals ++ literalValue

    let hashNonMemberVal (* denv *) (tps, v: Val, tau, cxs) =
        let env = SimplifyTypes.CollectInfo true [tau] cxs
        let cxs = env.postfixConstraints
        let valReprInfo = arityOfValForDisplay v
        let argInfos, retTy = GetTopTauTypeInFSharpForm (* denv *).g valReprInfo.ArgInfos tau v.Range
        let nameL =

            let tagF =
                if isForallFunctionTy (* denv *).g v.Type && not (isDiscard v.DisplayNameCore) then
                    if IsOperatorDisplayName v.DisplayName then
                        tagOperator
                    else
                        tagFunction
                elif not v.IsCompiledAsTopLevel && not(isDiscard v.DisplayNameCore) then
                    tagLocal
                elif v.IsModuleBinding then
                    tagModuleBinding
                else
                    tagUnknownEntity

            v.DisplayName
            |> tagF
            |> mkNav v.DefinitionRange
            |> hashText 
        let nameL = hashAccessibility (* denv *) v.Accessibility nameL
        let nameL = 
            if v.IsMutable && not (* denv *).suppressMutableKeyword then 
                hashText ((* string to tag was here *) "mutable") ++ nameL 
                else 
                    nameL
        let nameL = mkInlineL (* denv *) v nameL

        let isOverGeneric = List.length (Zset.elements (freeInType CollectTyparsNoCaching tau).FreeTypars) < List.length tps // Bug: 1143 
        let isTyFunction = v.IsTypeFunction // Bug: 1143, and innerpoly tests 
        let typarBindingsL = 
            if isTyFunction || isOverGeneric || (* denv *).showTyparBinding then 
                hashTyparDecls (* denv *) nameL true tps 
            else nameL
        let valAndTypeL = (WordL.keywordVal ^^ (typarBindingsL |> addColonL)) --- hashTopType (* denv *) env argInfos retTy cxs
        let valAndTypeL =
            match (* denv *).generatedValueLayout v with
            | None -> valAndTypeL
            | Some rhsL -> (valAndTypeL ++ WordL.equals) --- rhsL
        match v.LiteralValue with
        | Some literalValue -> valAndTypeL --- hashOfLiteralValue literalValue
        | None -> valAndTypeL

    let prettyLayoutOfValOrMember (* denv *) infoReader typarInst (vref: ValRef) =
        let prettyTyparInst, valL =
            match vref.MemberInfo with 
            | None ->
                let tps, tau = vref.GeneralizedType

                // adjust the type in case this is the 'this' pointer stored in a reference cell
                let tau = StripSelfRefCell((* denv *).g, vref.BaseOrThisInfo, tau)

                let (prettyTyparInst, prettyTypars, prettyTauTy), cxs = PrettyTypes.PrettifyInstAndTyparsAndType (* denv *).g (typarInst, tps, tau)
                let resL = hashNonMemberVal (* denv *) (prettyTypars, vref.Deref, prettyTauTy, cxs)
                prettyTyparInst, resL
            | Some _ -> 
                prettyLayoutOfMember (* denv *) typarInst vref.Deref

        let valL =
            valL
            |> hashAttribs (* denv *) None vref.LiteralValue.IsSome TyparKind.Type vref.Attribs     

        prettyTyparInst, valL

    let prettyLayoutOfValOrMemberNoInst (* denv *) infoReader v =
        prettyLayoutOfValOrMember (* denv *) infoReader emptyTyparInst v |> snd

let hashTyparConstraint (* denv *) x = x |> PrintTypes.hashConstraint (* denv *)

let outputType (* denv *) os x = x |> PrintTypes.hashType (* denv *) |> bufferL os

let hashType (* denv *) x = x |> PrintTypes.hashType (* denv *)

let outputTypars (* denv *) nm os x = x |> PrintTypes.hashTyparDecls (* denv *) (hashText nm) true |> bufferL os

let outputTyconRef (* denv *) os x = x |> PrintTypes.hashTyconRef (* denv *) |> bufferL os

let hashTyconRef (* denv *) x = x |> PrintTypes.hashTyconRef (* denv *)

let hashConst g ty c = PrintTypes.hashConst g ty c

let prettyLayoutOfMemberSig (* denv *) x = x |> PrintTypes.prettyLayoutOfMemberSig (* denv *) 

let prettyLayoutOfUncurriedSig (* denv *) argInfos tau = PrintTypes.prettyLayoutOfUncurriedSig (* denv *) argInfos tau

let prettyLayoutsOfUnresolvedOverloading (* denv *) argInfos retTy genericParameters = PrintTypes.prettyLayoutsOfUnresolvedOverloading (* denv *) argInfos retTy genericParameters

//-------------------------------------------------------------------------

/// Printing info objects
module InfoMemberPrinting = 

    /// Format the arguments of a method
    let hashParamData (* denv *) (ParamData(isParamArray, _isInArg, _isOutArg, optArgInfo, _callerInfo, nmOpt, _reflArgInfo, pty)) =
        let isOptArg = optArgInfo.IsOptional
        // detect parameter type, if ptyOpt is None - this is .NET style optional argument
        let ptyOpt = tryDestOptionTy (* denv *).g pty

        match isParamArray, nmOpt, isOptArg with 
        // Hash an optional argument 
        | _, Some id, true -> 
            let idL = ConvertValLogicalNameToDisplayLayout false (tagParameter >> rightL) id.idText
            let pty = match ptyOpt with ValueSome x -> x | _ -> pty
            LeftL.questionMark ^^
            (idL |> addColonL) ^^
            PrintTypes.hashType (* denv *) pty

        // Hash an unnamed argument 
        | _, None, _ -> 
            PrintTypes.hashType (* denv *) pty

        // Hash a named ParamArray argument 
        | true, Some id, _ -> 
            let idL = ConvertValLogicalNameToDisplayLayout false (tagParameter >> hashText) id.idText
            hashBuiltinAttribute (* denv *) (* denv *).g.attrib_ParamArrayAttribute ^^
            (idL  |> addColonL) ^^
            PrintTypes.hashType (* denv *) pty

        // Hash a named normal argument 
        | false, Some id, _ -> 
            let idL = ConvertValLogicalNameToDisplayLayout false (tagParameter >> hashText) id.idText
            (idL  |> addColonL) ^^
            PrintTypes.hashType (* denv *) pty

    let formatParamDataToBuffer (* denv *) os pd =
        hashParamData (* denv *) pd |> bufferL os
        
    /// Format a method info using "F# style".
    //
    // That is, this style:
    //     new: argName1: argType1 * ... * argNameN: argTypeN -> retType
    //     Method: argName1: argType1 * ... * argNameN: argTypeN -> retType
    //
    let hashMethInfoFSharpStyleCore (infoReader: InfoReader) m (* denv *) (minfo: MethInfo) minst =
        let amap = infoReader.amap

        match minfo.ArbitraryValRef with
        | Some vref ->
            PrintTastMemberOrVals.prettyLayoutOfValOrMemberNoInst (* denv *) infoReader vref
        | None ->
            let hash = 
                if not minfo.IsConstructor && not minfo.IsInstance then WordL.keywordStatic
                else 0 (* empty hash *)

            let nameL =
                if minfo.IsConstructor then
                    WordL.keywordNew
                else
                    let idL = ConvertValLogicalNameToDisplayLayout false (tagMethod >> tagNavArbValRef minfo.ArbitraryValRef >> hashText) minfo.LogicalName
                    WordL.keywordMember ^^
                    PrintTypes.hashTyparDecls (* denv *) idL true minfo.FormalMethodTypars

            let hash = hash ^^ (nameL |> addColonL)        

            let paramsL =
                let paramDatas = minfo.GetParamDatas(amap, m, minst)
                if List.forall isNil paramDatas then
                    WordL.structUnit
                else
                    sepListL WordL.arrow (List.map ((List.map (hashParamData (* denv *))) >> sepListL WordL.star) paramDatas)

            let hash = hash ^^ paramsL
            
            let retL =
                let retTy = minfo.GetFSharpReturnType(amap, m, minst)
                WordL.arrow ^^
                PrintTypes.hashType (* denv *) retTy

            hash ^^ retL 

    /// Format a method info using "half C# style".
    //
    // That is, this style:
    //          Container(argName1: argType1, ..., argNameN: argTypeN) : retType
    //          Container.Method(argName1: argType1, ..., argNameN: argTypeN) : retType
    let hashMethInfoCSharpStyle amap m (* denv *) (minfo: MethInfo) minst =
        let retTy = if minfo.IsConstructor then minfo.ApparentEnclosingType else minfo.GetFSharpReturnType(amap, m, minst) 
        let hash = 
            if minfo.IsExtensionMember then
                LeftL.leftParen ^^ hashText ((* string to tag was here *) (FSComp.SR.typeInfoExtension())) ^^ RightL.rightParen
            else 0 (* empty hash *)
        let hash = 
            hash ^^
                if isAppTy minfo.TcGlobals minfo.ApparentEnclosingAppType then
                    let tcref = minfo.ApparentEnclosingTyconRef 
                    PrintTypes.hashTyconRef (* denv *) tcref
                else
                    0 (* empty hash *)
        let hash = 
            hash ^^
                if minfo.IsConstructor then
                    SepL.leftParen
                else
                    let idL = ConvertValLogicalNameToDisplayLayout false (tagMethod >> tagNavArbValRef minfo.ArbitraryValRef >> hashText) minfo.LogicalName
                    SepL.dot ^^
                    PrintTypes.hashTyparDecls (* denv *) idL true minfo.FormalMethodTypars ^^
                    SepL.leftParen

        let paramDatas = minfo.GetParamDatas (amap, m, minst)
        let hash = hash ^^ sepListL RightL.comma ((List.concat >> List.map (hashParamData (* denv *))) paramDatas)
        hash ^^ RightL.rightParen ^^ WordL.colon ^^ PrintTypes.hashType (* denv *) retTy

    // Prettify an ILMethInfo
    let prettifyILMethInfo (amap: Import.ImportMap) m (minfo: MethInfo) typarInst ilMethInfo = 
        let (ILMethInfo(_, apparentTy, dty, mdef, _)) = ilMethInfo
        let (prettyTyparInst, prettyTys), _ = PrettyTypes.PrettifyInstAndTypes amap.g (typarInst, (apparentTy :: minfo.FormalMethodInst))
        match prettyTys with
        | prettyApparentTy :: prettyFormalMethInst ->
            let prettyMethInfo = 
                match dty with 
                | None -> MethInfo.CreateILMeth (amap, m, prettyApparentTy, mdef)
                | Some declaringTyconRef -> MethInfo.CreateILExtensionMeth(amap, m, prettyApparentTy, declaringTyconRef, minfo.ExtensionMemberPriorityOption, mdef)
            prettyTyparInst, prettyMethInfo, prettyFormalMethInst
        | _ -> failwith "prettifyILMethInfo - prettyTys empty"

    /// Format a method to a buffer using "standalone" display style. 
    /// For example, these are the formats used when printing signatures of methods that have not been overridden,
    /// and the format used when showing the individual member in QuickInfo and DeclarationInfo.
    /// The formats differ between .NET/provided methods and F# methods. Surprisingly people don't really seem 
    /// to notice this, or they find it helpful. It feels that moving from this position should not be done lightly.
    //
    // For F# members:
    //          new: unit -> retType
    //          new: argName1: argType1 * ... * argNameN: argTypeN -> retType
    //          Container.Method: unit -> retType
    //          Container.Method: argName1: argType1 * ... * argNameN: argTypeN -> retType
    //
    // For F# extension members:
    //          ApparentContainer.Method: argName1: argType1 * ... * argNameN: argTypeN -> retType
    //
    // For C# and provided members:
    //          Container(argName1: argType1, ..., argNameN: argTypeN) : retType
    //          Container.Method(argName1: argType1, ..., argNameN: argTypeN) : retType
    //
    // For C# extension members:
    //          ApparentContainer.Method(argName1: argType1, ..., argNameN: argTypeN) : retType
    let prettyLayoutOfMethInfoFreeStyle (infoReader: InfoReader) m (* denv *) typarInst methInfo =
        let amap = infoReader.amap

        match methInfo with 
        | DefaultStructCtor _ -> 
            let prettyTyparInst, _ = PrettyTypes.PrettifyInst amap.g typarInst 
            let resL = PrintTypes.hashTyconRef (* denv *) methInfo.ApparentEnclosingTyconRef ^^ hashText (tagPunctuation "()")
            prettyTyparInst, resL
        | FSMeth(_, _, vref, _) -> 
            let prettyTyparInst, resL = PrintTastMemberOrVals.prettyLayoutOfValOrMember { (* denv *) with showMemberContainers=true } infoReader typarInst vref
            prettyTyparInst, resL
        | ILMeth(_, ilminfo, _) -> 
            let prettyTyparInst, prettyMethInfo, minst = prettifyILMethInfo amap m methInfo typarInst ilminfo
            let resL = hashMethInfoCSharpStyle amap m (* denv *) prettyMethInfo minst
            prettyTyparInst, resL
#if !NO_TYPEPROVIDERS
        | ProvidedMeth _ -> 
            let prettyTyparInst, _ = PrettyTypes.PrettifyInst amap.g typarInst 
            prettyTyparInst, hashMethInfoCSharpStyle amap m (* denv *) methInfo methInfo.FormalMethodInst
    #endif

    let prettyLayoutOfPropInfoFreeStyle g amap m (* denv *) (pinfo: PropInfo) =
        let retTy = pinfo.GetPropertyType(amap, m) 
        let retTy = if pinfo.IsIndexer then mkFunTy g (mkRefTupledTy g (pinfo.GetParamTypes(amap, m))) retTy else  retTy 
        let retTy, _ = PrettyTypes.PrettifyType g retTy
        let nameL = ConvertValLogicalNameToDisplayLayout false (tagProperty >> tagNavArbValRef pinfo.ArbitraryValRef >> hashText) pinfo.PropertyName
        let getterSetter =
            match pinfo.HasGetter, pinfo.HasSetter with
            | true, false ->
                hashText ((* string to tag was here *) "with") ^^ hashText (tagText "get")
            | false, true ->
                hashText ((* string to tag was here *) "with") ^^ hashText (tagText "set")
            | true, true ->
                hashText ((* string to tag was here *) "with") ^^ hashText (tagText "get, set")
            | false, false ->
                0 (* empty hash *)

        hashText (tagText (FSComp.SR.typeInfoProperty())) ^^
        hashTyconRef (* denv *) pinfo.ApparentEnclosingTyconRef ^^
        SepL.dot ^^
        (nameL  |> addColonL) ^^
        hashType (* denv *) retTy ^^
        getterSetter

    let formatPropInfoToBufferFreeStyle g amap m (* denv *) os (pinfo: PropInfo) = 
        let resL = prettyLayoutOfPropInfoFreeStyle g amap m (* denv *) pinfo 
        resL |> bufferL os

    let formatMethInfoToBufferFreeStyle amap m (* denv *) os (minfo: MethInfo) = 
        let _, resL = prettyLayoutOfMethInfoFreeStyle amap m (* denv *) emptyTyparInst minfo 
        resL |> bufferL os

    /// Format a method to a hash (actually just containing a string) using "free style" (aka "standalone"). 
    let hashMethInfoFSharpStyle amap m (* denv *) (minfo: MethInfo) =
        hashMethInfoFSharpStyleCore amap m (* denv *) minfo minfo.FormalMethodInst

//-------------------------------------------------------------------------

/// Printing TAST objects
module TashDefinitionHashes = 
    open PrintTypes

    let hashExtensionMember (* denv *) infoReader (vref: ValRef) =
        let (@@*) = if (* denv *).printVerboseSignatures then (@@----) else (@@--)
        let tycon = vref.MemberApparentEntity.Deref
        let nameL = hashTyconRefImpl false (* denv *) vref.MemberApparentEntity
        let nameL = hashAccessibility (* denv *) tycon.Accessibility nameL // "type-accessibility"
        let tps =
            match PartitionValTyparsForApparentEnclosingType (* denv *).g vref.Deref with
              | Some(_, memberParentTypars, _, _, _) -> memberParentTypars
              | None -> []
        let lhsL = WordL.keywordType ^^ hashTyparDecls (* denv *) nameL tycon.IsPrefixDisplay tps
        let memberL = PrintTastMemberOrVals.prettyLayoutOfValOrMemberNoInst (* denv *) infoReader vref
        (lhsL ^^ WordL.keywordWith) @@* memberL

    let hashExtensionMembers (* denv *) infoReader vs =
        combineHashes (List.map (hashExtensionMember (* denv *) infoReader) vs) 

    let hashRecdField prefix isClassDecl (* denv *) infoReader (enclosingTcref: TyconRef) (fld: RecdField) =
        let lhs = ConvertLogicalNameToDisplayLayout (tagRecordField >> mkNav fld.DefinitionRange >> hashText) fld.DisplayNameCore
        let lhs = (if isClassDecl then hashAccessibility (* denv *) fld.Accessibility lhs else lhs)
        let lhs = if fld.IsMutable then hashText ((* string to tag was here *) "mutable") --- lhs else lhs
        let fieldL =
            let rhs =
                match stripTyparEqns fld.FormalType with
                | TType_fun _ -> LeftL.leftParen ^^ hashType (* denv *) fld.FormalType ^^ RightL.rightParen
                | _ -> hashType (* denv *) fld.FormalType
            
            (lhs |> addColonL) --- rhs
        let fieldL = prefix fieldL
        let fieldL = fieldL |> hashAttribs (* denv *) None false TyparKind.Type (fld.FieldAttribs @ fld.PropertyAttribs)

        // The enclosing TyconRef might be a union and we can only get fields from union cases, so we need ignore unions here.
        fieldL

    let hashUnionOrExceptionField (* denv *) infoReader isGenerated enclosingTcref i (fld: RecdField) =
        if isGenerated i fld then
            hashTType (* denv *) SimplifyTypes.typeSimplificationInfo0 2 fld.FormalType
        else
            hashRecdField id false (* denv *) infoReader enclosingTcref fld
    
    let isGeneratedUnionCaseField pos (f: RecdField) = 
        if pos < 0 then f.LogicalName = "Item"
        else f.LogicalName = "Item" + string (pos + 1)

    let isGeneratedExceptionField pos (f: RecdField) = 
        f.LogicalName = "Data" + (string pos)

    let hashUnionCaseFields (* denv *) infoReader isUnionCase enclosingTcref fields = 
        match fields with
        | [f] when isUnionCase ->
            hashUnionOrExceptionField (* denv *) infoReader isGeneratedUnionCaseField enclosingTcref -1 f
        | _ -> 
            let isGenerated = if isUnionCase then isGeneratedUnionCaseField else isGeneratedExceptionField
            sepListL WordL.star (List.mapi (hashUnionOrExceptionField (* denv *) infoReader isGenerated enclosingTcref) fields)

    let hashUnionCase (* denv *) infoReader prefixL enclosingTcref (ucase: UnionCase) =
        let nmL = ConvertLogicalNameToDisplayLayout (tagUnionCase >> mkNav ucase.DefinitionRange >> hashText) ucase.Id.idText
        //let nmL = hashAccessibility (* denv *) ucase.Accessibility nmL
        let caseL =
            match ucase.RecdFields with
            | []     -> (prefixL ^^ nmL)
            | fields -> (prefixL ^^ nmL ^^ WordL.keywordOf) --- hashUnionCaseFields (* denv *) infoReader true enclosingTcref fields
        caseL

    let hashUnionCases (* denv *) infoReader enclosingTcref ucases =
        let prefixL = WordL.bar // See bug://2964 - always prefix in case preceded by accessibility modifier
        List.map (hashUnionCase (* denv *) infoReader prefixL enclosingTcref) ucases

    /// When to force a break? "type tyname = <HERE> repn"
    /// When repn is class or datatype constructors (not single one).
    let breakTypeDefnEqn repr =
        match repr with 
        | TILObjectRepr _ -> true
        | TFSharpObjectRepr _ -> true
        | TFSharpRecdRepr _ -> true
        | TFSharpUnionRepr r ->
             not (isNilOrSingleton r.CasesTable.UnionCasesAsList) ||
             r.CasesTable.UnionCasesAsList |> List.exists (fun uc -> not uc.XmlDoc.IsEmpty)
        | TAsmRepr _ 
        | TMeasureableRepr _ 
#if !NO_TYPEPROVIDERS
        | TProvidedTypeRepr _
        | TProvidedNamespaceRepr _
#endif
        | TNoRepr -> false
      
    let hashILFieldInfo (* denv *) (infoReader: InfoReader) m (finfo: ILFieldInfo) =
        let staticL = if finfo.IsStatic then WordL.keywordStatic else 0 (* empty hash *)
        let nameL = ConvertLogicalNameToDisplayLayout (tagField >> hashText) finfo.FieldName
        let typL = hashType (* denv *) (finfo.FieldType(infoReader.amap, m))
        let fieldL = staticL ^^ WordL.keywordVal ^^ (nameL |> addColonL) ^^ typL
        fieldL

    let hashEventInfo (* denv *) (infoReader: InfoReader) m (einfo: EventInfo) =
        let amap = infoReader.amap
        let staticL = if einfo.IsStatic then WordL.keywordStatic else 0 (* empty hash *)
        let nameL = ConvertValLogicalNameToDisplayLayout false (tagEvent >> tagNavArbValRef einfo.ArbitraryValRef >> hashText) einfo.EventName
        let typL = hashType (* denv *) (einfo.GetDelegateType(amap, m))
        let overallL = staticL ^^ WordL.keywordMember ^^ (nameL |> addColonL) ^^ typL
        overallL

    let hashPropInfo (* denv *) (infoReader: InfoReader) m (pinfo: PropInfo) =
        let amap = infoReader.amap

        let isPublicGetterSetter (getter: MethInfo) (setter: MethInfo) =
            let isPublicAccess access = access = TAccess []
            match getter.ArbitraryValRef, setter.ArbitraryValRef with
            | Some gRef, Some sRef -> isPublicAccess gRef.Accessibility && isPublicAccess sRef.Accessibility
            | _ -> false

        match pinfo.ArbitraryValRef with
        | Some vref ->
            let propL = PrintTastMemberOrVals.prettyLayoutOfValOrMemberNoInst (* denv *) infoReader vref
            if pinfo.HasGetter && pinfo.HasSetter && not pinfo.IsIndexer && isPublicGetterSetter pinfo.GetterMethod pinfo.SetterMethod then
                propL ^^ hashText ((* string to tag was here *) "with") ^^ hashText (tagText "get, set")
            else
                propL
        | None ->

            let modifierAndMember =
                if pinfo.IsStatic then
                    WordL.keywordStatic ^^ WordL.keywordMember
                else
                    WordL.keywordMember
        
            let nameL = ConvertValLogicalNameToDisplayLayout false (tagProperty >> tagNavArbValRef pinfo.ArbitraryValRef >> hashText) pinfo.PropertyName
            let typL = hashType (* denv *) (pinfo.GetPropertyType(amap, m))
            let overallL = modifierAndMember ^^ (nameL |> addColonL) ^^ typL
            overallL

    let hashTyconDefn ((* denv *): DisplayEnv) (infoReader: InfoReader) ad m simplified isFirstType (tcref: TyconRef) =        
        let g = (* denv *).g
        // use 4-indent 
        let (-*) = if (* denv *).printVerboseSignatures then (-----) else (---)
        let (@@*) = if (* denv *).printVerboseSignatures then (@@----) else (@@--)
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
                if (* denv *).printVerboseSignatures then
                    Some "interface", tagInterface
                else
                    None, tagInterface
            elif isMeasure then
                None, tagClass
            elif isClassTy g ty then
                if (* denv *).printVerboseSignatures then
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
        let (-*) = if (* denv *).printVerboseSignatures then (-----) else (---)
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

    let rec hashModuleOrNamespace ((* denv *): DisplayEnv) (infoReader: InfoReader) ad m isFirstTopLevel (mspec: ModuleOrNamespace) =
        let (@@*) = if (* denv *).printVerboseSignatures then (@@----) else (@@--)

        let outerPath = mspec.CompilationPath.AccessPath

        let path, mspec = fullPath mspec [mspec.DisplayNameCore]

        let (* denv *) =
            let outerPath = outerPath |> List.map fst
            (* denv *).AddOpenPath (outerPath @ path)

        let headerL =
            if mspec.IsNamespace then
                // This is a container namespace. We print the header when we get to the first concrete module.
                let pathL = path |> List.map (ConvertLogicalNameToDisplayLayout (tagNamespace >> hashText))
                hashText ((* string to tag was here *) "namespace") ^^ sepListL SepL.dot pathL
            else
                // This is a module 
                let name = path |> List.last
                let nameL = ConvertLogicalNameToDisplayLayout (tagModule >> mkNav mspec.DefinitionRange >> hashText) name
                let nameL = 
                    match path with
                    | [_] -> 
                        nameL
                    | _ ->
                        let innerPath = path[..path.Length - 2]
                        let innerPathL = innerPath |> List.map (ConvertLogicalNameToDisplayLayout (tagNamespace >> hashText))
                        sepListL SepL.dot innerPathL ^^ SepL.dot ^^ nameL

                let modNameL = hashText ((* string to tag was here *) "module") ^^ nameL
                let modNameEqualsL = modNameL ^^ WordL.equals
                let modIsEmpty =
                    mspec.ModuleOrNamespaceType.AllEntities |> Seq.isEmpty &&
                    mspec.ModuleOrNamespaceType.AllValsAndMembers |> Seq.isEmpty

                // Check if its an outer module or a nested module
                let isNamespace = function | Namespace _ -> true | _ -> false
                if (outerPath |> List.forall (fun (_, istype) -> isNamespace istype)) && isNil outerPath then
                    // If so print a "module" declaration
                    modNameL
                elif modIsEmpty then
                    modNameEqualsL ^^ hashText ((* string to tag was here *) "begin") ^^ WordL.keywordEnd
                else
                    // Otherwise this is an outer module contained immediately in a namespace
                    // We already printed the namespace declaration earlier. So just print the 
                    // module now.
                    modNameEqualsL

        let headerL =
            hashAttribs (* denv *) None false mspec.TypeOrMeasureKind mspec.Attribs headerL

        let shouldShow (v: Val) =
            ((* denv *).showObsoleteMembers || not (CheckFSharpAttributesForObsolete (* denv *).g v.Attribs)) &&
            ((* denv *).showHiddenMembers || not (CheckFSharpAttributesForHidden (* denv *).g v.Attribs))

        let entityLs =
            if mspec.IsNamespace then []
            else
                mspec.ModuleOrNamespaceType.AllEntities
                |> QueueList.toList
                |> List.map (fun entity -> hashEntityDefn (* denv *) infoReader ad m (mkLocalEntityRef entity))
            
        let valLs =
            if mspec.IsNamespace then []
            else
                mspec.ModuleOrNamespaceType.AllValsAndMembers
                |> QueueList.toList
                |> List.filter shouldShow
                |> List.sortBy (fun v -> v.DisplayNameCore)
                |> List.map (mkLocalValRef >> PrintTastMemberOrVals.prettyLayoutOfValOrMemberNoInst (* denv *) infoReader)

        if List.isEmpty entityLs && List.isEmpty valLs then
            headerL
        else
            let entitiesL =
                entityLs
                |> combineHashes

            let valsL =
                valLs
                |> combineHashes

            if isFirstTopLevel then
                combineHashes
                    [
                        headerL
                        entitiesL
                        valsL
                    ]
            else
                headerL @@* entitiesL @@ valsL

    and hashEntityDefn ((* denv *): DisplayEnv) (infoReader: InfoReader) ad m (eref: EntityRef) =
        if eref.IsModuleOrNamespace then
            hashModuleOrNamespace (* denv *) infoReader ad m false eref.Deref       
        elif eref.IsFSharpException then
            hashExnDefn (* denv *) infoReader eref
        else
            hashTyconDefn (* denv *) infoReader ad m true true eref

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
                |> List.map (PrintTastMemberOrVals.prettyLayoutOfValOrMemberNoInst (* denv *) infoReader)
                |> combineHashes) @@

            (mbinds 
                |> List.choose (function ModuleOrNamespaceBinding.Module (mspec, def) -> Some (mspec, def) | _ -> None) 
                |> List.map (imbindL (* denv *)) 
                |> combineHashes)

        | TMDefLet(bind, _) -> 
            ([bind.Var] 
                |> List.filter filterVal 
                |> List.map mkLocalValRef
                |> List.map (PrintTastMemberOrVals.prettyLayoutOfValOrMemberNoInst (* denv *) infoReader) 
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

    imdefL (* denv *) expr

//--------------------------------------------------------------------------
