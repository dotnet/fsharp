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
let inline combineHash acc y  : Hash= (acc <<< 1) + y + 631
let inline pipeToHash (value:Hash) (acc:Hash)   = combineHash acc value  
let inline hashAndAdd (value) (acc:Hash) = combineHash (acc) (hash value) 
let inline combineHashes (hashes: Hash list) = (0,hashes) ||> List.fold (fun acc curr -> combineHash acc curr)
let inline hashListOrderMatters ([<InlineIfLambda>]func) (items:#seq<'T>) : Hash =
    let mutable acc = 0
    for i in items do
        let valHash = func i
        // We are calling hashListOrderMatters for things like list of types, list of properties, list of fields etc. The ones which are visibility-hidden will return 0, and are ommited.
        if valHash <> 0 then
            acc <- combineHash acc valHash
    acc
let inline hashListOrderIndependent ([<InlineIfLambda>]func) (items:#seq<'T>) : Hash =
    let mutable acc = 0
    for i in items do
        let valHash = func i
        // We are calling hashListOrderMatters for things like list of types, list of properties, list of fields etc. The ones which are visibility-hidden will return 0, and are ommited.
        if valHash <> 0 then
            acc <- acc ^^^ valHash
    acc


let (@@) (h1:Hash) (h2:Hash) = combineHash h1 h2


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

        combineHash (hash tag) (hashText name)

    let hashTyconRefImpl (g:TcGlobals) (tcref: TyconRef) =
        let demangled = tcref.DisplayNameWithStaticParameters
        let tyconHash = hashEntityRefName g tcref demangled

        tcref.CompilationPath.AccessPath
        |> hashListOrderMatters (fst >> hashText) 
        |> pipeToHash tyconHash
        
module HashIL = 

    let hashILTypeRef (tref: ILTypeRef) =
        tref.Enclosing
        |> hashListOrderMatters hashText      
        |> hashAndAdd tref.Name

    let private hashILArrayShape ( sh:ILArrayShape) = sh.Rank  
 
    let rec hashILType (ty: ILType) : Hash =
        match ty with
        | ILType.Void -> hash ILType.Void
        | ILType.Array (sh, t) -> hashILType  t @@ hashILArrayShape sh
        | ILType.Value t
        | ILType.Boxed t -> hashILTypeRef  t.TypeRef @@ (t.GenericArgs |> hashListOrderMatters (hashILType ))
        | ILType.Ptr t
        | ILType.Byref t -> hashILType   t
        | ILType.FunctionPointer t -> hashILCallingSignature t
        | ILType.TypeVar n -> hash n 
        | ILType.Modified (_, _, t) -> hashILType  t

    and hashILCallingSignature   (signature: ILCallingSignature) =       
        let res = signature.ReturnType |> hashILType     
        signature.ArgTypes
        |> hashListOrderMatters (hashILType ) 
        |> pipeToHash res

module rec HashTypes = 

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
    let private hashAttrib (g:TcGlobals) (Attrib(tyconRef = tcref)) = 
        hashTyconRefImpl g tcref
            
    let hashAttributeList (g:TcGlobals) attrs  =
        attrs
        |> hashListOrderIndependent (hashAttrib g)     

    let private hashTyparRef (typar: Typar) =
        hashText typar.DisplayName
        |> hashAndAdd (typar.Rigidity)
        |> hashAndAdd (typar.StaticReq) 
    
    let private hashTyparRefWithInfo (g:TcGlobals) (typar: Typar) =
        hashTyparRef typar @@ hashAttributeList g typar.Attribs 

    let private hashConstraint (g:TcGlobals) struct(tp, tpc) =
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
            tpHash @@ 12 @@ (tys |> hashListOrderIndependent (hashTType g))  
        | TyparConstraint.RequiresDefaultConstructor _ -> 
            tpHash @@ 13

    /// Hash type parameter constraints
    let private hashConstraints (g:TcGlobals) cxs = 
        cxs
        |> hashListOrderIndependent (hashConstraint g)

    let private hashTraitWithInfo  (g:TcGlobals)  traitInfo =
        let nameHash = hashText traitInfo.MemberLogicalName
        let memberHash = hashMemberFlags traitInfo.MemberFlags    
        let returnTypeHash = match traitInfo.CompiledReturnType with Some t -> hashTType g t | _ -> -1

        traitInfo.CompiledObjectAndArgumentTypes
        |> hashListOrderIndependent (hashTType g)
        |> pipeToHash (nameHash)
        |> pipeToHash (returnTypeHash)
        |> pipeToHash memberHash
       

    /// Hash a unit of measure expression 
    let private hashMeasure unt =
        let measuresWithExponents = ListMeasureVarOccsWithNonZeroExponents unt |> List.sortBy (fun (tp: Typar, _) -> tp.DisplayName) 
        measuresWithExponents
        |> hashListOrderIndependent (fun (typar,exp: Rational) -> hashTyparRef typar @@ hash exp)
    
    /// Hash a type, taking precedence into account to insert brackets where needed
    let hashTType (g:TcGlobals)  ty =  
      
        match stripTyparEqns ty with 
        | TType_ucase (UnionCaseRef(tc, _), args)
        | TType_app (tc, args, _) ->
          args
          |> hashListOrderMatters (hashTType g )        
          |> pipeToHash (hashTyconRef g tc)
        | TType_anon (anonInfo, tys) ->
            tys
            |> hashListOrderMatters (hashTType g )
            |> pipeToHash (anonInfo.SortedNames |> hashListOrderMatters hashText)            
            |> hashAndAdd (evalAnonInfoIsStruct anonInfo)    
        | TType_tuple (tupInfo, t) ->
            t
            |> hashListOrderMatters (hashTType g )
            |> hashAndAdd (evalTupInfoIsStruct tupInfo) 
        // Hash a first-class generic type. 
        | TType_forall (tps, tau) ->
            tps
            |> hashListOrderMatters (hashTyparRef)
            |> pipeToHash (hashTType g  tau)  
        | TType_fun _ ->
            let argTys, retTy = stripFunTy g ty    
            argTys
            |> hashListOrderMatters (hashTType g )
            |> pipeToHash (hashTType g  retTy)
        | TType_var (r, _) -> hashTyparRefWithInfo g r
        | TType_measure unt -> hashMeasure unt

    // Hash a single argument, including its name and type 
    let private hashArgInfo (g:TcGlobals) (ty, argInfo: ArgReprInfo) = 
       
        let attributesHash = hashAttributeList g argInfo.Attribs
        let nameHash = match argInfo.Name with | Some i -> hashText i.idText | _ -> -1
        let typeHash = hashTType g ty

        typeHash @@ nameHash @@ attributesHash

    let private hashCurriedArgInfos (g:TcGlobals) argInfos =
        argInfos
        |> hashListOrderMatters(fun l -> l |> hashListOrderMatters (hashArgInfo g))

    /// Hash a single type used as the type of a member or value 
    let hashTopType (g:TcGlobals) argInfos retTy cxs =
        let retTypeHash = hashTType g retTy
        let cxsHash = hashConstraints g cxs
        let argHash = hashCurriedArgInfos g argInfos

        retTypeHash @@ cxsHash @@ argHash  
        
    let private hashTyparInclConstraints  (g:TcGlobals) (typar: Typar) = 
        typar.Constraints |> hashListOrderIndependent (fun tpc -> hashConstraint g (typar,tpc))
        |> pipeToHash (hashTyparRef typar)

    /// Hash type parameters
    let hashTyparDecls (g:TcGlobals) (typars: Typars) =      
        typars 
        |> hashListOrderMatters (hashTyparInclConstraints g)

    let private hashUncurriedSig (g:TcGlobals) typarInst argInfos retTy = 
        typarInst
        |> hashListOrderMatters (fun (typar,ttype) -> hashTyparInclConstraints g typar @@ hashTType g ttype)
        |> pipeToHash (hashTopType g argInfos retTy [])     
 
    let private hashMemberSigCore (g:TcGlobals) memberToParentInst (typarInst, methTypars: Typars, argInfos, retTy) = 
        typarInst
        |> hashListOrderMatters (fun (typar,ttype) -> hashTyparInclConstraints g typar @@ hashTType g ttype)
        |> pipeToHash (hashTopType g argInfos retTy []) 
        |> pipeToHash (memberToParentInst |> hashListOrderMatters (fun (typar,ty) -> hashTyparRef typar @@ hashTType g ty))
        |> pipeToHash (hashTyparDecls g methTypars)

    let hashMemberType (g:TcGlobals) vref typarInst argInfos retTy = 
        match PartitionValRefTypars g vref with
        | Some(_, _, memberMethodTypars, memberToParentInst, _) ->
            hashMemberSigCore g memberToParentInst (typarInst, memberMethodTypars, argInfos, retTy)
        | None -> 
            hashUncurriedSig g typarInst argInfos retTy 

/// Printing TAST objects
module HashTastMemberOrVals =
    open HashTypes 

    let private hashMember (g:TcGlobals) typarInst (v: Val)  =
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

    let private hashNonMemberVal (g:TcGlobals) (tps, v: Val, tau, cxs) =
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
            let cxs = 
                tps 
                |> Seq.collect (fun tp -> tp.Constraints |> Seq.map (fun cx -> struct(tp,cx))) 
                
            hashNonMemberVal g (tps, vref.Deref, tau, cxs)               
        | Some _ -> 
            hashMember g emptyTyparInst vref.Deref


//-------------------------------------------------------------------------

/// Printing TAST objects
module TashDefinitionHashes = 
    open HashTypes

    let private hashRecdField (g:TcGlobals) (fld: RecdField) =
        let nameHash = hashText fld.DisplayNameCore
        let attribHash = hashAttributeList g  fld.FieldAttribs @@ hashAttributeList g fld.PropertyAttribs
        let typeHash = hashTType g fld.FormalType

        let combined = nameHash @@ attribHash @@ typeHash @@ (hash fld.IsStatic) @@ (hash fld.IsVolatile) @@ (hash fld.IsMutable)        

        hashAccessibility fld.Accessibility combined

    let private hashUnionCase (g:TcGlobals) (ucase: UnionCase) =
        let nameHash = hashText ucase.Id.idText
        let attribHash = hashAttributeList g ucase.Attribs

        ucase.RecdFieldsArray
        |> hashListOrderMatters (fun rf -> hashRecdField g rf)
        |> pipeToHash nameHash
        |> pipeToHash attribHash
        |> hashAccessibility ucase.Accessibility

    let private hashUnionCases (g:TcGlobals)  ucases =
        ucases
        // Why order matters here?
        // Union cases come with generated Tag members, on which code in higher-level project can depend -> if order of union cases changes, higher-level project has to be also recompiled.
        // Correct me if I am wrong here pls.
        |> hashListOrderMatters (hashUnionCase g)

    let private hashFsharpDelegate g slotSig = 
        let (TSlotSig(_, _, _, _, paraml, retTy)) = slotSig  
        (paraml |> hashListOrderMatters (fun pl -> pl |> hashListOrderMatters (fun sp -> hashTType g sp.Type)))
        |> pipeToHash (hashTType g (GetFSharpViewOfReturnType g retTy))

    let private hashFsharpEnum (tycon:Tycon) =
        tycon.AllFieldsArray
        |> hashListOrderIndependent (fun f -> hashText f.DisplayNameCore) 

    let private hashTyconDefn (g:TcGlobals)   (tcref: TyconRef) =  
        let tycon = tcref.Deref
        let repr = tycon.TypeReprInfo

        let tyconHash = HashTypes.hashTyconRef g tcref
        let attribHash = hashAttributeList g tcref.Attribs
        let typarsHash = hashTyparDecls g tycon.TyparsNoRange
        let topLevelDeclarationHash = tyconHash @@ attribHash @@ typarsHash

        // Interface implementation
        let iimplsHash() =
            tycon.ImmediateInterfacesOfFSharpTycon
            |> hashListOrderIndependent (fun (ttype,_,_) -> hashTType g ttype)

        // Fields, static fields, val declarations
        let fieldsHash() =
            tycon.AllFieldsArray
            |> hashListOrderIndependent (hashRecdField g)

        /// Properties, methods, constructors
        let membersHash() =
            tycon.MembersOfFSharpTyconByName       
            |> hashListOrderIndependent (fun kvp -> kvp.Value |> hashListOrderIndependent (HashTastMemberOrVals.hashValOrMemberNoInst g) )

        /// Super type or obj
        let inheritsHash() = 
            superOfTycon g tycon 
            |> hashTType g 

        let specializedHash = 
            match repr with 
            | TFSharpRecdRepr _ -> fieldsHash()
            | TFSharpUnionRepr _ ->  hashUnionCases g tycon.UnionCasesArray  
            | TFSharpObjectRepr { fsobjmodel_kind = TFSharpDelegate slotSig } -> hashFsharpDelegate g slotSig
            | TFSharpObjectRepr { fsobjmodel_kind = TFSharpEnum } -> hashFsharpEnum tycon             
            | TFSharpObjectRepr { fsobjmodel_kind = TFSharpClass | TFSharpInterface | TFSharpStruct as tfor}  -> 
                iimplsHash() @@ fieldsHash() @@ membersHash() @@ inheritsHash()
                |> pipeToHash 
                    (match tfor with
                    | TFSharpClass -> 1
                    | TFSharpInterface -> 2
                    | TFSharpStruct -> 3
                    | _ -> 4)
            | TAsmRepr ilType -> HashIL.hashILType ilType  
            | TMeasureableRepr ty -> hashTType g ty 
            | TILObjectRepr _ -> iimplsHash() @@ fieldsHash() @@ membersHash() @@ inheritsHash()
            | TNoRepr when tycon.TypeAbbrev.IsSome ->
                let abbreviatedTy = tycon.TypeAbbrev.Value
                hashTType g abbreviatedTy
            | TNoRepr when tycon.IsFSharpException -> 
                match tycon.ExceptionInfo with
                | TExnAbbrevRepr exnTcRef -> hashTyconRef g exnTcRef
                | TExnAsmRepr iLTypeRef -> HashIL.hashILTypeRef iLTypeRef
                | TExnNone -> 0
                | TExnFresh _ -> fieldsHash()
               
#if !NO_TYPEPROVIDERS
            | TProvidedNamespaceRepr _
            | TProvidedTypeRepr _
#endif
            | TNoRepr -> iimplsHash() @@ fieldsHash() @@ membersHash() @@ inheritsHash()


        specializedHash 
        |> pipeToHash topLevelDeclarationHash
        |> hashAccessibility tycon.Accessibility

    // Hash: module spec 

    let hashTyconDefns (g:TcGlobals)  (tycons: Tycon list) =
        tycons
        |> hashListOrderIndependent (mkLocalEntityRef >> (hashTyconDefn g))
      

    let rec fullPath (mspec: ModuleOrNamespace) acc =
        if mspec.IsNamespace then
            match mspec.ModuleOrNamespaceType.ModuleAndNamespaceDefinitions |> List.tryHead with
            | Some next when next.IsNamespace ->
                fullPath next (acc @ [next.DisplayNameCore])
            | _ ->
                acc, mspec
        else
            acc, mspec


open HashTypes

/// Hash the inferred signature of a compilation unit
let calculateHashOfImpliedSignature (g:TcGlobals) (infoReader:InfoReader) (ad:AccessorDomain) (m:range) (expr:ModuleOrNamespaceContents) =

    let rec isConcreteNamespace x = 
        match x with 
        | TMDefRec(_, _opens, tycons, mbinds, _) -> 
            not (isNil tycons) || (mbinds |> List.exists (function ModuleOrNamespaceBinding.Binding _ -> true | ModuleOrNamespaceBinding.Module(x, _) -> not x.IsNamespace))
        | TMDefLet _ -> true
        | TMDefDo _ -> true
        | TMDefOpens _ -> false
        | TMDefs defs -> defs |> List.exists isConcreteNamespace 

    and hashModuleOrNameSpaceBinding (monb:ModuleOrNamespaceBinding) =
        match monb with
        | ModuleOrNamespaceBinding.Binding b -> HashTastMemberOrVals.hashValOrMemberNoInst g (mkLocalValRef b.Var)
        | ModuleOrNamespaceBinding.Module moduleInfo,contents -> hashSingleModuleOrNameSpaceIncludingName (moduleInfo,contents)

    and hashSingleModuleOrNamespaceContents  x =        
        match x with 
        | TMDefRec(_, _opens, tycons, mbinds, _) -> 
            mbinds
            |> hashListOrderIndependent (hashModuleOrNameSpaceBinding)
            |> pipeToHash (TashDefinitionHashes.hashTyconDefns g tycons)
        | TMDefLet(bind, _) -> HashTastMemberOrVals.hashValOrMemberNoInst g (mkLocalValRef bind.Var)
        | TMDefOpens _ -> 0 (* empty hash *)
        | TMDefs defs -> defs |> hashListOrderIndependent hashSingleModuleOrNamespaceContents
        | TMDefDo _ -> 0 (* empty hash *)

    and hashSingleModuleOrNameSpaceIncludingName (* denv *) (mspec, def) = 
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
            let basic = hashSingleModuleOrNamespaceContents (* denv *) def
            let modNameL = hashText ((* string to tag was here *) "module") @@ nmL
            let basicL = modNameL @@ basic
            basicL
        elif mspec.IsNamespace then
            let basic = hashSingleModuleOrNamespaceContents (* denv *) def
            let basicL =
                // Check if this namespace contains anything interesting
                if isConcreteNamespace def then
                    let pathL = innerPath |> List.map (fst >> ConvertLogicalNameToDisplayLayout (tagNamespace >> hashText))
                    // This is a container namespace. We print the header when we get to the first concrete module.
                    let headerL =
                        hashText ((* string to tag was here *) "namespace") @@ sepListL SepL.dot pathL
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
            let basic = hashSingleModuleOrNamespaceContents (* denv *) def
            let modNameL =
                hashText ((* string to tag was here *) "module") @@ nmL
                |> hashAttribs (* denv *) None false mspec.TypeOrMeasureKind mspec.Attribs
            let modNameEqualsL = modNameL @@ WordL.equals
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
                                modNameEqualsL @@ WordL.keywordBegin @@ WordL.keywordEnd
                            else
                                modNameEqualsL @@* basic
                    else
                        // OK, we're in F# Interactive, presumably the implicit module for each interaction.
                        basic
                else
                    // OK, this is a nested module, with indentation
                    if isEmptyL basic then 
                        ((modNameEqualsL @@ hashText ((* string to tag was here *)"begin")) @@* basic) @@ WordL.keywordEnd
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

        hashText ((* string to tag was here *) keyword) @@ sepListL SepL.dot pathL

    match expr with
    | EmptyModuleOrNamespaces mspecs when showHeader ->
        List.map emptyModuleOrNamespace mspecs
        |> aboveListL
    | expr -> hashSingleModuleOrNamespaceContents denv expr

//--------------------------------------------------------------------------
