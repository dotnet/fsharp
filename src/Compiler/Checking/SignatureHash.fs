module internal Fsharp.Compiler.SignatureHash

open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps
open FSharp.Compiler.CheckDeclarations

open Internal.Utilities.Library
open Internal.Utilities.TypeHashing
open Internal.Utilities.TypeHashing.HashTypes

//-------------------------------------------------------------------------

/// Printing TAST objects
module TyconDefinitionHash =

    let private hashRecdField (g: TcGlobals, observer) (fld: RecdField) =
        if HashAccessibility.isHiddenToObserver fld.Accessibility observer then
            0
        else
            let nameHash = hashText fld.DisplayNameCore

            let attribHash =
                hashAttributeList fld.FieldAttribs @@ hashAttributeList fld.PropertyAttribs

            let typeHash = hashTType g fld.FormalType

            let combined =
                nameHash
                @@ attribHash
                @@ typeHash
                @@ (hash fld.IsStatic)
                @@ (hash fld.IsVolatile)
                @@ (hash fld.IsMutable)

            combined

    let private hashUnionCase (g: TcGlobals, observer) (ucase: UnionCase) =
        if HashAccessibility.isHiddenToObserver ucase.Accessibility observer then
            0
        else
            let nameHash = hashText ucase.Id.idText
            let attribHash = hashAttributeList ucase.Attribs

            ucase.RecdFieldsArray
            |> hashListOrderMatters (fun rf -> hashRecdField (g, observer) rf)
            |> pipeToHash nameHash
            |> pipeToHash attribHash

    let private hashUnionCases (g, obs) ucases =
        ucases
        // Why order matters here?
        // Union cases come with generated Tag members, on which code in higher-level project can depend -> if order of union cases changes, higher-level project has to be also recompiled.
        // Correct me if I am wrong here pls.
        |> hashListOrderMatters (hashUnionCase (g, obs))

    let private hashFsharpDelegate g slotSig =
        let (TSlotSig(_, _, _, _, paraml, retTy)) = slotSig

        (paraml
         |> hashListOrderMatters (fun pl -> pl |> hashListOrderMatters (fun sp -> hashTType g sp.Type)))
        |> pipeToHash (hashTType g (GetFSharpViewOfReturnType g retTy))

    let private hashFsharpEnum (tycon: Tycon) =
        tycon.AllFieldsArray
        |> hashListOrderIndependent (fun f -> hashText f.DisplayNameCore)

    let private hashTyconDefn (g, observer) (tcref: TyconRef) =
        let tycon = tcref.Deref

        if HashAccessibility.isHiddenToObserver tycon.Accessibility observer then
            0
        else

            let repr = tycon.TypeReprInfo

            let tyconHash = HashTypes.hashTyconRef tcref
            let attribHash = hashAttributeList tcref.Attribs
            let typarsHash = hashTyparDecls g tycon.TyparsNoRange
            let topLevelDeclarationHash = tyconHash @@ attribHash @@ typarsHash

            // Interface implementation
            let iimplsHash () =
                tycon.ImmediateInterfacesOfFSharpTycon
                |> hashListOrderIndependent (fun (ttype, _, _) -> hashTType g ttype)

            // Fields, static fields, val declarations
            let fieldsHash () =
                tycon.AllFieldsArray |> hashListOrderIndependent (hashRecdField (g, observer))

            /// Properties, methods, constructors
            let membersHash () =
                tycon.MembersOfFSharpTyconByName
                |> hashListOrderIndependent (fun kvp ->
                    kvp.Value
                    |> hashListOrderIndependent (HashTastMemberOrVals.hashValOrMemberNoInst (g, observer)))

            /// Super type or obj
            let inheritsHash () = superOfTycon g tycon |> hashTType g

            let specializedHash =
                match repr with
                | TFSharpTyconRepr { fsobjmodel_kind = TFSharpRecord } -> fieldsHash ()
                | TFSharpTyconRepr { fsobjmodel_kind = TFSharpUnion } -> hashUnionCases (g, observer) tycon.UnionCasesArray
                | TFSharpTyconRepr {
                                       fsobjmodel_kind = TFSharpDelegate slotSig
                                   } -> hashFsharpDelegate g slotSig
                | TFSharpTyconRepr { fsobjmodel_kind = TFSharpEnum } -> hashFsharpEnum tycon
                | TFSharpTyconRepr {
                                       fsobjmodel_kind = TFSharpClass | TFSharpInterface | TFSharpStruct as tfor
                                   } ->
                    iimplsHash () @@ fieldsHash () @@ membersHash () @@ inheritsHash ()
                    |> pipeToHash (
                        match tfor with
                        | TFSharpClass -> 1
                        | TFSharpInterface -> 2
                        | TFSharpStruct -> 3
                        | _ -> 4
                    )
                | TAsmRepr ilType -> HashIL.hashILType ilType
                | TMeasureableRepr ty -> hashTType g ty
                | TILObjectRepr _ -> iimplsHash () @@ fieldsHash () @@ membersHash () @@ inheritsHash ()
                | TNoRepr when tycon.TypeAbbrev.IsSome ->
                    let abbreviatedTy = tycon.TypeAbbrev.Value
                    hashTType g abbreviatedTy
                | TNoRepr when tycon.IsFSharpException ->
                    match tycon.ExceptionInfo with
                    | TExnAbbrevRepr exnTcRef -> hashTyconRef exnTcRef
                    | TExnAsmRepr iLTypeRef -> HashIL.hashILTypeRef iLTypeRef
                    | TExnNone -> 0
                    | TExnFresh _ -> fieldsHash ()

#if !NO_TYPEPROVIDERS
                | TProvidedNamespaceRepr _
                | TProvidedTypeRepr _
#endif
                | TNoRepr -> iimplsHash () @@ fieldsHash () @@ membersHash () @@ inheritsHash ()

            specializedHash |> pipeToHash topLevelDeclarationHash

    // Hash: module spec

    let hashTyconDefns (g, obs) (tycons: Tycon list) =
        tycons
        |> hashListOrderIndependent (mkLocalEntityRef >> (hashTyconDefn (g, obs)))

    let rec fullPath (mspec: ModuleOrNamespace) acc =
        if mspec.IsNamespace then
            match mspec.ModuleOrNamespaceType.ModuleAndNamespaceDefinitions |> List.tryHead with
            | Some next when next.IsNamespace -> fullPath next (acc @ [ next.DisplayNameCore ])
            | _ -> acc, mspec
        else
            acc, mspec

let calculateHashOfImpliedSignature g observer (expr: ModuleOrNamespaceContents) =

    let rec hashModuleOrNameSpaceBinding (monb: ModuleOrNamespaceBinding) =
        match monb with
        | ModuleOrNamespaceBinding.Binding b when b.Var.LogicalName.StartsWithOrdinal("doval@") -> 0
        | ModuleOrNamespaceBinding.Binding b -> HashTastMemberOrVals.hashValOrMemberNoInst (g, observer) (mkLocalValRef b.Var)
        | ModuleOrNamespaceBinding.Module(moduleInfo, contents) -> hashSingleModuleOrNameSpaceIncludingName (moduleInfo, contents)

    and hashSingleModuleOrNamespaceContents x =
        match x with
        | TMDefRec(_, _opens, tycons, mbinds, _) ->
            let mbindsHash = mbinds |> hashListOrderIndependent (hashModuleOrNameSpaceBinding)

            let tyconsHash = TyconDefinitionHash.hashTyconDefns (g, observer) tycons

            if mbindsHash <> 0 || tyconsHash <> 0 then
                mbindsHash @@ tyconsHash
            else
                0

        | TMDefLet(bind, _) -> HashTastMemberOrVals.hashValOrMemberNoInst (g, observer) (mkLocalValRef bind.Var)
        | TMDefOpens _ -> 0 (* empty hash *)
        | TMDefs defs -> defs |> hashListOrderIndependent hashSingleModuleOrNamespaceContents
        | TMDefDo _ -> 0 (* empty hash *)

    and hashSingleModuleOrNameSpaceIncludingName (mspec, def) =
        if HashAccessibility.isHiddenToObserver mspec.Accessibility observer then
            0
        else
            let outerPathHash =
                mspec.CompilationPath.MangledPath |> hashListOrderMatters hashText

            let thisNameHash = hashText mspec.entity_logical_name

            let fullNameHash = outerPathHash @@ thisNameHash @@ (hash mspec.IsModule)
            let contentHash = hashSingleModuleOrNamespaceContents def

            if contentHash = 0 then 0 else fullNameHash @@ contentHash

    hashSingleModuleOrNamespaceContents expr

let calculateSignatureHashOfFiles (files: CheckedImplFile list) g observer =
    use _ =
        FSharp.Compiler.Diagnostics.Activity.startNoTags "calculateSignatureHashOfFiles"

    files
    |> hashListOrderMatters (fun f -> calculateHashOfImpliedSignature g observer f.Contents)

let calculateHashOfAssemblyTopAttributes (attrs: TopAttribs) (platform: ILPlatform option) =
    let platformHash =
        match platform with
        | None -> 0
        | Some AMD64 -> 1
        | Some IA64 -> 2
        | Some ARM -> 3
        | Some ARM64 -> 4
        | Some X86 -> 5

    HashTypes.hashAttributeList attrs.assemblyAttrs
    @@ HashTypes.hashAttributeList attrs.mainMethodAttrs
    @@ HashTypes.hashAttributeList attrs.netModuleAttrs
    @@ platformHash
