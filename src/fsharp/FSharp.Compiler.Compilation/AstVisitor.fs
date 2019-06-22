namespace FSharp.Compiler.Compilation

open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.Ast
open FSharp.Compiler.Range

[<AutoOpen>]
module AstVisitorHelpers =
    
    let isZeroRange (r: range) =
        posEq r.Start r.End

    type SynModuleOrNamespace with

        member this.AdjustedRange =
            match this with
            | SynModuleOrNamespace (longId=longId;range=m) -> 
                match longId with
                | [] -> m
                | _ ->
                    let longIdRange =
                        longId
                        |> List.map (fun x -> x.idRange)
                        |> List.reduce unionRanges
                    unionRanges longIdRange m

    type ParsedInput with

        member this.AdjustedRange =
            match this with
            | ParsedInput.ImplFile (ParsedImplFileInput (modules=modules)) ->
                match modules with
                | [] -> range0
                | _ ->
                    modules
                    |> List.map (fun x -> x.AdjustedRange)
                    |> List.reduce (unionRanges)
            | ParsedInput.SigFile (ParsedSigFileInput (modules=modules)) ->
                range0 // TODO:

    type ParsedHashDirective with

        member this.Range =
            match this with
            | ParsedHashDirective (_, _, m) -> m

    type SynTypeConstraint with

        member this.Range =
            match this with
            | SynTypeConstraint.WhereTyparIsValueType (_, m) -> m
            | SynTypeConstraint.WhereTyparIsReferenceType (_, m) -> m
            | SynTypeConstraint.WhereTyparIsUnmanaged (_, m) -> m
            | SynTypeConstraint.WhereTyparSupportsNull (_, m) -> m
            | SynTypeConstraint.WhereTyparIsComparable (_, m) -> m
            | SynTypeConstraint.WhereTyparIsEquatable (_, m) -> m
            | SynTypeConstraint.WhereTyparDefaultsToType (_, _, m) -> m
            | SynTypeConstraint.WhereTyparSubtypeOfType (_, _, m) -> m
            | SynTypeConstraint.WhereTyparSupportsMember (_, _, m) -> m
            | SynTypeConstraint.WhereTyparIsEnum (_, _, m) -> m
            | SynTypeConstraint.WhereTyparIsDelegate (_, _, m) -> m

    type SynMemberSig with

        member this.Range =
            match this with
            | SynMemberSig.Member (_, _, m) -> m
            | SynMemberSig.Interface (_, m) -> m
            | SynMemberSig.Inherit (_, m) -> m
            | SynMemberSig.ValField (_, m) -> m
            | SynMemberSig.NestedType (_, m) -> m

    type SynValSig with

        member this.Range =
            match this with
            | ValSpfn (_, _, _, _, _, _, _, _, _, _, m) -> m

    type SynField with

        member this.Range =
            match this with
            | Field (_, _, _, _, _, _, _, m) -> m

    type SynTypeDefnSig with

        member this.Range =
            match this with
            | TypeDefnSig (_, _, _, m) -> m

    type SynMeasure with

        member this.PossibleRange =
            match this with
            | SynMeasure.Named (range=m) -> m
            | SynMeasure.Product (range=m) -> m
            | SynMeasure.Seq (range=m) -> m
            | SynMeasure.Divide (range=m) -> m
            | SynMeasure.Power (range=m) -> m
            | SynMeasure.One -> range0
            | SynMeasure.Anon (range=m) -> m
            | SynMeasure.Var (range=m) -> m

    type SynRationalConst with

        member this.PossibleRange =
            match this with
            | SynRationalConst.Integer _ -> range0
            | SynRationalConst.Rational (range=m) -> m
            | SynRationalConst.Negate rationalConst -> rationalConst.PossibleRange

    type SynConst with

        member this.PossibleRange =
            this.Range range0

    type SynArgInfo with

        member this.PossibleRange =
            match this with
            | SynArgInfo (attribs, _, idOpt) ->
                let ranges =
                    attribs
                    |> List.map (fun x -> x.Range)
                    |> List.append (match idOpt with | Some id -> [id.idRange] | _ -> [])

                match ranges with
                | [] -> range0
                | _ ->
                    ranges
                    |> List.reduce unionRanges

    type SynValInfo with

        member this.PossibleRange =
            match this with
            | SynValInfo (argInfos, argInfo) ->
                match argInfos with
                | [] -> range0
                | _ ->
                    let result =
                        argInfos
                        |> List.reduce (@)
                        |> List.append [argInfo]
                        |> List.map (fun x -> x.PossibleRange)
                        |> List.filter (fun x -> not (isZeroRange x))
                    match result with
                    | [] -> range0
                    | result ->
                        result
                        |> List.reduce unionRanges

    type SynTypeDefnKind with

        member this.PossibleRange =
            match this with
            | TyconUnspecified
            | TyconClass
            | TyconInterface
            | TyconStruct
            | TyconRecord
            | TyconUnion
            | TyconAbbrev
            | TyconHiddenRepr
            | TyconAugmentation
            | TyconILAssemblyCode ->
                range0
            | TyconDelegate (ty, valInfo) ->
                let valInfoPossibleRange = valInfo.PossibleRange
                if isZeroRange valInfoPossibleRange then
                    ty.Range
                else
                    unionRanges ty.Range valInfoPossibleRange

    type SynTyparDecl with

        member this.Range =
            match this with
            | TyparDecl (attribs, typar) ->
                match attribs with
                | [] -> typar.Range
                | _ ->
                    attribs
                    |> List.map (fun x -> x.Range)
                    |> List.append [typar.Range]
                    |> List.reduce unionRanges

    type SynValTyparDecls with

        member this.PossibleRange =
            match this with
            | SynValTyparDecls (typarDecls, _, constraints) ->
                match typarDecls with
                | [] -> range0
                | _ ->
                    typarDecls
                    |> List.map (fun x -> x.Range)
                    |> List.append (constraints |> List.map (fun x -> x.Range))
                    |> List.reduce unionRanges

    type SynSimplePat with

        member this.Range =
            match this with
            | SynSimplePat.Id (range=m) -> m
            | SynSimplePat.Typed (range=m) -> m
            | SynSimplePat.Attrib (range=m) -> m

    type SynSimplePats with

        member this.Range =
            match this with
            | SynSimplePats.SimplePats (range=m) -> m
            | SynSimplePats.Typed (range=m) -> m

    type SynValData with

        member this.Range =
            match this with
            | SynValData (_, valInfo, idOpt) ->
                match idOpt with
                | Some id ->
                    let valInfoPossibleRange = valInfo.PossibleRange
                    if isZeroRange valInfoPossibleRange then
                        id.idRange
                    else
                        unionRanges id.idRange valInfoPossibleRange
                | _ -> 
                    valInfo.PossibleRange

    type SynBindingReturnInfo with

        member this.Range =
            match this with
            | SynBindingReturnInfo (_, m, _) -> m

    type SynConstructorArgs with

        member this.PossibleRange =
            match this with
            | Pats pats ->
                match pats with
                | [] -> range0
                | _ ->
                    pats
                    |> List.map (fun x -> x.Range)
                    |> List.reduce unionRanges

            | NamePatPairs (_, m) -> m

    type SynInterfaceImpl with

        member this.Range =
            match this with
            | InterfaceImpl (_, _, m) -> m

    type SynSimplePatAlternativeIdInfo with

        member this.Range =
            match this with
            | Undecided id -> id.idRange
            | Decided id -> id.idRange

    type SynStaticOptimizationConstraint with

        member this.Range =
            match this with
            | WhenTyparTyconEqualsTycon (_, _, m) -> m
            | WhenTyparIsStruct (_, m) -> m

    type SynUnionCaseType with

        member this.PossibleRange =
            match this with
            | UnionCaseFields cases ->
                match cases with
                | [] -> range0
                | _ ->
                    cases
                    |> List.map (fun x -> x.Range)
                    |> List.reduce unionRanges

            | UnionCaseFullType (ty, valInfo) ->
                let valInfoPossibleRange = valInfo.PossibleRange
                if isZeroRange valInfoPossibleRange then
                    ty.Range
                else
                    unionRanges ty.Range valInfoPossibleRange

[<AbstractClass>]
type AstVisitor<'T> () as this =

    let tryVisitList xs : 'T option =
        xs
        |> List.tryPick (fun (getRange, visit) ->
            let r = getRange ()
            if this.CanVisit r then
                visit ()
            else
                None
        )

    let mapVisitList (getRange: 'Syn -> range) visit xs =
        xs
        |> List.map (fun x -> ((fun () -> getRange x), fun () -> visit x))

    let mapiVisitList (getRange: 'Syn -> range) visit xs =
        xs
        |> List.mapi (fun i x -> ((fun () -> getRange x), fun () -> visit (i, x)))

    member inline private this.TryVisit m visit item =
        if this.CanVisit m then
            visit item
        else
            None

    member inline private this.TryVisitList getRange visit items =
        items
        |> List.tryPick (fun item ->
            this.TryVisit (getRange item) visit item
        )

    member inline private this.TryVisitListIndex getRange visit items =
        let mutable i = 0
        items
        |> List.tryPick (fun item ->
            if this.CanVisit (getRange item) then
                visit i item
            else
                i <- i + 1
                None
        )

    abstract CanVisit: range -> bool  
    default this.CanVisit _ = true

    abstract VisitParsedInput: ParsedInput -> 'T option
    default this.VisitParsedInput parsedInput =
        match parsedInput with
        | ParsedInput.ImplFile (ParsedImplFileInput (_, _, _, _, _, modules, _)) -> 
            modules
            |> mapVisitList (fun x -> x.AdjustedRange) this.VisitModuleOrNamespace
            |> tryVisitList
        | ParsedInput.SigFile (ParsedSigFileInput (_, _, _, _, modules)) -> 
            None

    abstract VisitModuleOrNamespace: SynModuleOrNamespace -> 'T option
    default this.VisitModuleOrNamespace moduleOrNamespace =
        match moduleOrNamespace with
        | SynModuleOrNamespace (longId, _, _, decls, _, attribs, _, _) ->
            let longId =
                longId
                |> mapiVisitList (fun x -> x.idRange) this.VisitIdent

            let decls =
                decls
                |> mapVisitList (fun x -> x.Range) this.VisitModuleDecl

            let attribs =
                attribs
                |> mapVisitList (fun x -> x.Range) this.VisitAttributeList

            (longId @ decls @ attribs)
            |> tryVisitList

    abstract VisitModuleDecl: SynModuleDecl -> 'T option
    default this.VisitModuleDecl decl =
        match decl with

        | SynModuleDecl.ModuleAbbrev _ -> None

        | SynModuleDecl.NestedModule (info, _, decls, _, _) ->
            let infos =
                [info]
                |> mapVisitList (fun x -> x.Range) this.VisitComponentInfo

            let decls =
                decls
                |> mapVisitList (fun x -> x.Range) this.VisitModuleDecl

            (infos @ decls)
            |> tryVisitList

        | SynModuleDecl.Let (_, bindings, _) ->
            bindings
            |> mapVisitList (fun x -> x.RangeOfBindingAndRhs) this.VisitBinding
            |> tryVisitList

        | SynModuleDecl.DoExpr (_, expr, _) ->
            expr
            |> this.TryVisit expr.Range this.VisitExpr

        | SynModuleDecl.Types (typeDefns, _) ->
            typeDefns
            |> mapVisitList (fun x -> x.Range) this.VisitTypeDefn
            |> tryVisitList

        | SynModuleDecl.Exception (exDefn, _) ->
            exDefn
            |> this.TryVisit exDefn.Range this.VisitExceptionDefn

        | SynModuleDecl.Open _ ->
            None

        | SynModuleDecl.Attributes (attribs, _) ->
            attribs
            |> mapVisitList (fun x -> x.Range) this.VisitAttributeList
            |> tryVisitList

        | SynModuleDecl.HashDirective (hashDirective, _) ->
            hashDirective
            |> this.TryVisit hashDirective.Range this.VisitParsedHashDirective

        | SynModuleDecl.NamespaceFragment moduleOrNamespace ->
            moduleOrNamespace
            |> this.TryVisit moduleOrNamespace.Range this.VisitModuleOrNamespace

    abstract VisitLongIdentWithDots: LongIdentWithDots -> 'T option
    default this.VisitLongIdentWithDots longDotId =
        match longDotId with
        | LongIdentWithDots (longId, _) ->
            longId
            |> mapiVisitList (fun x -> x.idRange) this.VisitIdent
            |> tryVisitList

    abstract VisitIdent: index: int * Ident -> 'T option
    default this.VisitIdent (_, _) =
        None

    abstract VisitComponentInfo: SynComponentInfo -> 'T option
    default this.VisitComponentInfo info =
        match info with
        | ComponentInfo(attribs, typarDecls, constraints, longId, _, _, _, m) ->
            let attribs =
                attribs
                |> mapVisitList (fun x -> x.Range) this.VisitAttributeList

            let typarDecls =
                typarDecls
                |> mapVisitList (fun x -> x.Range) this.VisitTyparDecl

            let constraints =
                constraints
                |> mapVisitList (fun x -> x.Range) this.VisitTypeConstraint

            let longId =
                longId
                |> mapiVisitList (fun x -> x.idRange) this.VisitIdent

            (attribs @ typarDecls @ constraints @ longId)
            |> tryVisitList

    abstract VisitTypeConstraint: SynTypeConstraint -> 'T option
    default this.VisitTypeConstraint c =
        match c with
        | SynTypeConstraint.WhereTyparIsValueType (typar, _) ->
            typar
            |> this.TryVisit typar.Range this.VisitTypar

        | SynTypeConstraint.WhereTyparIsReferenceType (typar, _) ->
            typar
            |> this.TryVisit typar.Range this.VisitTypar

        | SynTypeConstraint.WhereTyparIsUnmanaged (typar, _) ->
            typar
            |> this.TryVisit typar.Range this.VisitTypar

        | SynTypeConstraint.WhereTyparSupportsNull (typar, _) ->
            typar
            |> this.TryVisit typar.Range this.VisitTypar

        | SynTypeConstraint.WhereTyparIsComparable (typar, _) ->
            typar
            |> this.TryVisit typar.Range this.VisitTypar

        | SynTypeConstraint.WhereTyparIsEquatable (typar, _) ->
            typar
            |> this.TryVisit typar.Range this.VisitTypar

        | SynTypeConstraint.WhereTyparDefaultsToType (typar, ty, _) ->
            let typar =
                [typar]
                |> mapVisitList (fun x -> x.Range) this.VisitTypar

            let ty =
                [ty]
                |> mapVisitList (fun x -> x.Range) this.VisitType

            (typar @ ty)
            |> tryVisitList

        | SynTypeConstraint.WhereTyparSubtypeOfType (typar, ty, _) ->
            let typar =
                [typar]
                |> mapVisitList (fun x -> x.Range) this.VisitTypar

            let ty =
                [ty]
                |> mapVisitList (fun x -> x.Range) this.VisitType

            (typar @ ty)
            |> tryVisitList

        | SynTypeConstraint.WhereTyparSupportsMember (tys, memberSig, _) ->
            let tys =
                tys
                |> mapVisitList (fun x -> x.Range) this.VisitType

            let memberSig =
                [memberSig]
                |> mapVisitList (fun x -> x.Range) this.VisitMemberSig

            (tys @ memberSig)
            |> tryVisitList

        | SynTypeConstraint.WhereTyparIsEnum (typar, tys, _) ->
            let typar =
                [typar]
                |> mapVisitList (fun x -> x.Range) this.VisitTypar

            let tys =
                tys
                |> mapVisitList (fun x -> x.Range) this.VisitType

            (typar @ tys)
            |> tryVisitList

        | SynTypeConstraint.WhereTyparIsDelegate (typar, tys, _) ->
            let typar =
                [typar]
                |> mapVisitList (fun x -> x.Range) this.VisitTypar

            let tys =
                tys
                |> mapVisitList (fun x -> x.Range) this.VisitType

            (typar @ tys)
            |> tryVisitList

    abstract VisitMemberSig: SynMemberSig -> 'T option
    default this.VisitMemberSig memberSig =
        match memberSig with
        | SynMemberSig.Member (valSig, _, _) ->
            valSig
            |> this.TryVisit valSig.Range this.VisitValSig

        | SynMemberSig.Interface (ty, _) ->
            ty
            |> this.TryVisit ty.Range this.VisitType

        | SynMemberSig.Inherit (ty, _) ->
            ty
            |> this.TryVisit ty.Range this.VisitType

        | SynMemberSig.ValField (field, _) ->
            field
            |> this.TryVisit field.Range this.VisitField

        | SynMemberSig.NestedType (typeDefnSig, _) ->
            typeDefnSig
            |> this.TryVisit typeDefnSig.Range this.VisitTypeDefnSig

    abstract VisitTypeDefnSig: SynTypeDefnSig -> 'T option
    default this.VisitTypeDefnSig typeDefnSig =
        match typeDefnSig with
        | TypeDefnSig (info, repr, memberSigs, _) ->
            let info =
                [info]
                |> mapVisitList (fun x -> x.Range) this.VisitComponentInfo

            let repr =
                [repr]
                |> mapVisitList (fun x -> x.Range) this.VisitTypeDefnSigRepr

            let memberSigs =
                memberSigs
                |> mapVisitList (fun x -> x.Range) this.VisitMemberSig

            (info @ repr @ memberSigs)
            |> tryVisitList

    abstract VisitTypeDefnSigRepr: SynTypeDefnSigRepr -> 'T option
    default this.VisitTypeDefnSigRepr repr =
        match repr with
        | SynTypeDefnSigRepr.ObjectModel (typeDefnKind, memberSigs, _) ->
            let typeDefnKind =
                [typeDefnKind]
                |> mapVisitList (fun x -> x.PossibleRange) this.VisitTypeDefnKind

            let memberSigs =
                memberSigs
                |> mapVisitList (fun x -> x.Range) this.VisitMemberSig

            (typeDefnKind @ memberSigs)
            |> tryVisitList

        | SynTypeDefnSigRepr.Simple (simpleRepr, _) ->
            simpleRepr
            |> this.TryVisit simpleRepr.Range this.VisitTypeDefnSimpleRepr

        | SynTypeDefnSigRepr.Exception exRepr ->
            exRepr
            |> this.TryVisit exRepr.Range this.VisitExceptionDefnRepr

    abstract VisitExceptionDefnRepr: SynExceptionDefnRepr -> 'T option
    default this.VisitExceptionDefnRepr exRepr =
        match exRepr with
        | SynExceptionDefnRepr (attribs, unionCase, longIdOpt, _, _, _) ->
            let attribs =
                attribs
                |> mapVisitList (fun x -> x.Range) this.VisitAttributeList

            let unionCase =
                [unionCase]
                |> mapVisitList (fun x -> x.Range) this.VisitUnionCase

            let longId =
                match longIdOpt with
                | Some longId ->
                    longId
                    |> mapiVisitList (fun x -> x.idRange) this.VisitIdent
                | _ ->
                    []

            (attribs @ unionCase @ longId)
            |> tryVisitList

    abstract VisitUnionCase: SynUnionCase -> 'T option
    default this.VisitUnionCase unionCase =
        match unionCase with
        | UnionCase (attribs, ident, unionCaseTy, _, _, m) ->
            let attribs =
                attribs
                |> mapVisitList (fun x -> x.Range) this.VisitAttributeList

            let ident =
                [ident]
                |> mapiVisitList (fun x -> x.idRange) this.VisitIdent

            let unionCaseTy =
                [unionCaseTy]
                |> mapVisitList (fun x -> x.PossibleRange) this.VisitUnionCaseType

            (attribs @ ident @ unionCaseTy)
            |> tryVisitList

    abstract VisitUnionCaseType: SynUnionCaseType -> 'T option
    default this.VisitUnionCaseType unionCaseTy =
        match unionCaseTy with
        | UnionCaseFields cases ->
            cases
            |> mapVisitList (fun x -> x.Range) this.VisitField
            |> tryVisitList

        | UnionCaseFullType (ty, valInfo) ->
            let ty =
                [ty]
                |> mapVisitList (fun x -> x.Range) this.VisitType

            let valInfo =
                [valInfo]
                |> mapVisitList (fun x -> x.PossibleRange) this.VisitValInfo

            (ty @ valInfo)
            |> tryVisitList

    abstract VisitArgInfo: SynArgInfo -> 'T option
    default this.VisitArgInfo argInfo =
        match argInfo with
        | SynArgInfo (attribs, _, idOpt) ->
            let attribs =
                attribs
                |> mapVisitList (fun x -> x.Range) this.VisitAttributeList

            let id =
                match idOpt with
                | Some id ->
                    [id]
                    |> mapiVisitList (fun x -> x.idRange) this.VisitIdent
                | _ ->
                    []

            (attribs @ id)
            |> tryVisitList

    abstract VisitTypeDefnSimpleRepr: SynTypeDefnSimpleRepr -> 'T option
    default this.VisitTypeDefnSimpleRepr simpleRepr =
        match simpleRepr with
        | SynTypeDefnSimpleRepr.Union (_, unionCases, _) ->
            unionCases
            |> mapVisitList (fun x -> x.Range) this.VisitUnionCase
            |> tryVisitList

        | SynTypeDefnSimpleRepr.Enum (enumCases, _) ->
            enumCases
            |> mapVisitList (fun x -> x.Range) this.VisitEnumCase
            |> tryVisitList

        | SynTypeDefnSimpleRepr.Record (_, fields, _) ->
            fields
            |> mapVisitList (fun x -> x.Range) this.VisitField
            |> tryVisitList

        | SynTypeDefnSimpleRepr.General (typeDefnKind, tys, valSigs, fields, _, _, patsOpt, m) ->
            let typeDefnKind =
                [typeDefnKind]
                |> mapVisitList (fun x -> x.PossibleRange) this.VisitTypeDefnKind

            let tys =
                tys
                |> mapVisitList (fun (_, m, _) -> m) (fun (ty, _, idOpt) ->
                    let ty =
                        [ty]
                        |> mapVisitList (fun x -> x.Range) this.VisitType

                    let id =
                        match idOpt with
                        | Some id ->
                            [id]
                            |> mapiVisitList (fun x -> x.idRange) this.VisitIdent
                        | _ ->
                            []

                    (ty @ id)
                    |> tryVisitList
                )

            let valSigs =
                valSigs
                |> mapVisitList (fun (valSig, _) -> valSig.Range) (fun (valSig, _) -> this.VisitValSig valSig)

            let fields =
                fields
                |> mapVisitList (fun field -> field.Range) this.VisitField

            let pats =
                match patsOpt with
                | Some pats ->
                    [pats]
                    |> mapVisitList (fun x -> x.Range) this.VisitSimplePats
                | _ ->
                    []

            (typeDefnKind @ tys @ valSigs @ fields @ pats)
            |> tryVisitList

        | SynTypeDefnSimpleRepr.LibraryOnlyILAssembly _ ->
            None

        | SynTypeDefnSimpleRepr.TypeAbbrev (_, ty, _) ->
            ty
            |> this.TryVisit ty.Range this.VisitType

        | SynTypeDefnSimpleRepr.None _ ->
            None

        | SynTypeDefnSimpleRepr.Exception exDefnRepr ->
            exDefnRepr
            |> this.TryVisit exDefnRepr.Range this.VisitExceptionDefnRepr

    abstract VisitSimplePat: SynSimplePat -> 'T option
    default this.VisitSimplePat simplePat =
        match simplePat with
        | SynSimplePat.Id (id, simplePatInfoOpt, _, _, _, m) ->
            let id =
                [id]
                |> mapiVisitList (fun x -> x.idRange) this.VisitIdent

            let simplePatInfo =
                match simplePatInfoOpt with
                | Some simplePatInfo ->
                    [simplePatInfo.contents]
                    |> mapVisitList (fun x -> x.Range) this.VisitSimplePatAlternativeIdInfo
                | _ ->
                    []

            (id @ simplePatInfo)
            |> tryVisitList

        | SynSimplePat.Typed (simplePat, ty, _) ->
            let simplePat =
                [simplePat]
                |> mapVisitList (fun x -> x.Range) this.VisitSimplePat

            let ty =
                [ty]
                |> mapVisitList (fun x -> x.Range) this.VisitType

            (simplePat @ ty)
            |> tryVisitList

        | SynSimplePat.Attrib (simplePat, attribs, _) ->
            let simplePat =
                [simplePat]
                |> mapVisitList (fun x -> x.Range) this.VisitSimplePat

            let attribs =
                attribs
                |> mapVisitList (fun x -> x.Range) this.VisitAttributeList

            (simplePat @ attribs)
            |> tryVisitList

    abstract VisitEnumCase: SynEnumCase -> 'T option
    default this.VisitEnumCase enumCase =
        match enumCase with
        | EnumCase (attribs, id, sconst, _, m) ->
            let attribs =
                attribs
                |> mapVisitList (fun x -> x.Range) this.VisitAttributeList

            let id =
                [id]
                |> mapiVisitList (fun x -> x.idRange) this.VisitIdent

            let sconst =
                [sconst]
                |> mapVisitList (fun x -> x.Range range0) this.VisitConst

            (attribs @ id @ sconst)
            |> tryVisitList

    abstract VisitConst: SynConst -> 'T option
    default this.VisitConst sconst =
        match sconst with
        | SynConst.Measure (sconst, measure) ->
            let sconst =
                [sconst]
                |> mapVisitList (fun x -> x.Range range0) this.VisitConst

            let measure =
                [measure]
                |> mapVisitList (fun x -> x.PossibleRange) this.VisitMeasure

            (sconst @ measure)
            |> tryVisitList
        | _ ->
            None

    abstract VisitMeasure: SynMeasure -> 'T option
    default this.VisitMeasure measure =
        match measure with
        | SynMeasure.Named (longId, _) ->
            longId
            |> mapiVisitList (fun x -> x.idRange) this.VisitIdent
            |> tryVisitList

        | SynMeasure.Product (measure1, measure2, _) ->
            [measure1; measure2]
            |> mapVisitList (fun x -> x.PossibleRange) this.VisitMeasure
            |> tryVisitList

        | SynMeasure.Seq (measures, _) ->
            measures
            |> mapVisitList (fun x -> x.PossibleRange) this.VisitMeasure
            |> tryVisitList

        | SynMeasure.Divide (measure1, measure2, _) ->
            [measure1; measure2]
            |> mapVisitList (fun x -> x.PossibleRange) this.VisitMeasure
            |> tryVisitList

        | SynMeasure.Power (measure, rationalCost, _) ->
            let measure =
                [measure]
                |> mapVisitList (fun x -> x.PossibleRange) this.VisitMeasure

            let rationalCost =
                [rationalCost]
                |> mapVisitList (fun x -> x.PossibleRange) this.VisitRationalConst

            (measure @ rationalCost)
            |> tryVisitList

        | SynMeasure.One ->
            None

        | SynMeasure.Anon _ ->
            None

        | SynMeasure.Var (typar, _) ->
            typar
            |> this.TryVisit typar.Range this.VisitTypar

    abstract VisitRationalConst: SynRationalConst -> 'T option
    default this.VisitRationalConst rationalConst =
        match rationalConst with
        | SynRationalConst.Integer _ ->
            None

        | SynRationalConst.Rational _ ->
            None

        | SynRationalConst.Negate rationalConst ->
            rationalConst
            |> this.TryVisit rationalConst.PossibleRange this.VisitRationalConst

    abstract VisitTypeDefnKind: SynTypeDefnKind -> 'T option
    default this.VisitTypeDefnKind typeDefnKind =
        match typeDefnKind with
        | TyconDelegate (ty, valInfo) ->
            let ty =
                [ty]
                |> mapVisitList (fun x -> x.Range) this.VisitType

            let valInfo =
                [valInfo]
                |> mapVisitList (fun x -> x.PossibleRange) this.VisitValInfo

            (ty @ valInfo)
            |> tryVisitList
        | _ ->
            None

    abstract VisitField: SynField -> 'T option
    default this.VisitField field =
        match field with
        | SynField.Field (attribs, _, idOpt, ty, _, _, _, _) ->
            let attribs =
                attribs
                |> mapVisitList (fun x -> x.Range) this.VisitAttributeList

            let id =
                match idOpt with
                | Some id ->
                    [id]
                    |> mapiVisitList (fun x -> x.idRange) this.VisitIdent
                | _ ->
                    []

            let ty =
                [ty]
                |> mapVisitList (fun x -> x.Range) this.VisitType

            (attribs @ id @ ty)
            |> tryVisitList

    abstract VisitValSig: SynValSig -> 'T option
    default this.VisitValSig valSig =
        match valSig with
        | ValSpfn (attribs, id, valTyparDecls, ty, valInfo, _, _, _, _, exprOpt, m) ->
            let attribs =
                attribs
                |> mapVisitList (fun x -> x.Range) this.VisitAttributeList

            let id =
                [id]
                |> mapiVisitList (fun x -> x.idRange) this.VisitIdent

            let valTyparDecls =
                [valTyparDecls]
                |> mapVisitList (fun x -> x.PossibleRange) this.VisitValTyparDecls

            let ty =
                [ty]
                |> mapVisitList (fun x -> x.Range) this.VisitType

            let valInfo =
                [valInfo]
                |> mapVisitList (fun x -> x.PossibleRange) this.VisitValInfo

            let expr =
                match exprOpt with
                | Some expr ->
                    [expr]
                    |> mapVisitList (fun x -> x.Range) this.VisitExpr
                | _ ->
                    []

            (attribs @ id @ valTyparDecls @ ty @ valInfo @ expr)
            |> tryVisitList

    abstract VisitValTyparDecls: SynValTyparDecls -> 'T option
    default this.VisitValTyparDecls valTyparDecls =
        match valTyparDecls with
        | SynValTyparDecls (typarDecls, _, constraints) ->
            let typarDecls =
                typarDecls
                |> mapVisitList (fun x -> x.Range) this.VisitTyparDecl

            let constraints =
                constraints
                |> mapVisitList (fun x -> x.Range) this.VisitTypeConstraint

            (typarDecls @ constraints)
            |> tryVisitList

    abstract VisitType: SynType -> 'T option
    default this.VisitType ty =
        match ty with
        | SynType.LongIdent longDotId ->
            longDotId
            |> this.TryVisit longDotId.Range this.VisitLongIdentWithDots

        | SynType.App (ty, _, tyArgs, _, _, _, _) ->
            let ty =
                [ty]
                |> mapVisitList (fun x -> x.Range) this.VisitType

            let tyArgs =
                tyArgs
                |> mapVisitList (fun x -> x.Range) this.VisitType

            (ty @ tyArgs)
            |> tryVisitList

        | SynType.LongIdentApp (_, longDotId, _, tyArgs, _, _, _) ->
            let longDotId =
                [longDotId]
                |> mapVisitList (fun x -> x.Range) this.VisitLongIdentWithDots

            let tyArgs =
                tyArgs
                |> mapVisitList (fun x -> x.Range) this.VisitType

            (longDotId @ tyArgs)
            |> tryVisitList

        | SynType.Tuple (_, tyNames, _) ->
            tyNames
            |> List.map (fun (_, ty) -> ty)
            |> mapVisitList (fun x -> x.Range) this.VisitType
            |> tryVisitList

        | SynType.AnonRecd (_, tyNames, _) ->
            let ids, tys = List.unzip tyNames

            let ids =
                ids
                |> mapiVisitList (fun x -> x.idRange) this.VisitIdent

            let tys =
                tys
                |> mapVisitList (fun x -> x.Range) this.VisitType

            (ids @ tys)
            |> tryVisitList

        | SynType.Array (_, ty, _) ->
            ty
            |> this.TryVisit ty.Range this.VisitType

        | SynType.Fun (argTy, returnTy, _) ->
            let argTy =
                [argTy]
                |> mapVisitList (fun x -> x.Range) this.VisitType

            let returnTy =
                [returnTy]
                |> mapVisitList (fun x -> x.Range) this.VisitType

            (argTy @ returnTy)
            |> tryVisitList

        | SynType.Var (typar, _) ->
            typar
            |> this.TryVisit typar.Range this.VisitTypar

        | SynType.Anon _ ->
            None

        | SynType.WithGlobalConstraints (ty, constraints, _) ->
            let ty =
                [ty]
                |> mapVisitList (fun x -> x.Range) this.VisitType

            let constraints =
                constraints
                |> mapVisitList (fun x -> x.Range) this.VisitTypeConstraint

            (ty @ constraints)
            |> tryVisitList

        | SynType.HashConstraint (ty, _) ->
            ty
            |> this.TryVisit ty.Range this.VisitType

        | SynType.MeasureDivide (dividendTy, divisorTy, _) ->
            let dividendTy =
                [dividendTy]
                |> mapVisitList (fun x -> x.Range) this.VisitType

            let divisorTy =
                [divisorTy]
                |> mapVisitList (fun x -> x.Range) this.VisitType

            (dividendTy @ divisorTy)
            |> tryVisitList

        | SynType.MeasurePower (measureTy, rationalConst, _) ->
            let measureTy =
                [measureTy]
                |> mapVisitList (fun x -> x.Range) this.VisitType

            let rationalConst =
                [rationalConst]
                |> mapVisitList (fun x -> x.PossibleRange) this.VisitRationalConst

            (measureTy @ rationalConst)
            |> tryVisitList

        | SynType.StaticConstant (sconst, _) ->
            sconst
            |> this.TryVisit sconst.PossibleRange this.VisitConst

        | _ ->
            None

    abstract VisitSimplePats: SynSimplePats -> 'T option
    default this.VisitSimplePats simplePats =
        match simplePats with
        | SynSimplePats.SimplePats (simplePats, _) ->
            simplePats
            |> mapVisitList (fun x -> x.Range) this.VisitSimplePat
            |> tryVisitList

        | SynSimplePats.Typed (simplePats, ty, _) ->
            let simplePats =
                [simplePats]
                |> mapVisitList (fun x -> x.Range) this.VisitSimplePats

            let ty =
                [ty]
                |> mapVisitList (fun x -> x.Range) this.VisitType

            (simplePats @ ty)
            |> tryVisitList

    abstract VisitTypar: SynTypar -> 'T option
    default this.VisitTypar typar =
        match typar with
        | SynTypar.Typar (id, _, _) ->
            id
            |> this.TryVisit id.idRange (fun x -> this.VisitIdent (0, x))

    abstract VisitTyparDecl: SynTyparDecl -> 'T option
    default this.VisitTyparDecl typarDecl =
        match typarDecl with
        | TyparDecl (attribs, typar) ->
            let attribs =
                attribs
                |> mapVisitList (fun x -> x.Range) this.VisitAttributeList

            let typar =
                [typar]
                |> mapVisitList (fun x -> x.Range) this.VisitTypar

            (attribs @ typar)
            |> tryVisitList

    abstract VisitBinding: SynBinding -> 'T option
    default this.VisitBinding binding =
        match binding with
        | Binding (_, _, _, _, attribs, _, valData, headPat, returnInfoOpt, expr, _, _) ->
            let attribs =
                attribs
                |> this.TryVisitList (fun x -> x.Range) this.VisitAttributeList

            if attribs.IsSome then attribs
            else

            let valData =
                valData
                |> this.TryVisit valData.Range this.VisitValData

            if valData.IsSome then valData
            else

            let headPat =
                headPat
                |> this.TryVisit headPat.Range this.VisitPat

            if headPat.IsSome then headPat
            else

            let returnInfoOpt =
                match returnInfoOpt with
                | Some returnInfo ->
                    returnInfo
                    |> this.TryVisit returnInfo.Range this.VisitBindingReturnInfo
                | _ ->
                    None

            if returnInfoOpt.IsSome then returnInfoOpt
            else

            expr
            |> this.TryVisit expr.Range this.VisitExpr

    abstract VisitValData: SynValData -> 'T option
    default this.VisitValData valData =
        match valData with
        | SynValData (_, valInfo, idOpt) ->
            let valInfo =
                [valInfo]
                |> mapVisitList (fun x -> x.PossibleRange) this.VisitValInfo

            let idOpt =
                match idOpt with
                | Some id ->
                    [id]
                    |> mapiVisitList (fun x -> x.idRange) this.VisitIdent
                | _ ->
                    []

            (valInfo @ idOpt)
            |> tryVisitList

    abstract VisitValInfo: SynValInfo -> 'T option
    default this.VisitValInfo valInfo =
        match valInfo with
        | SynValInfo (argInfos, returnInfo) ->
            let argInfos =
                match argInfos with
                | [] -> []
                | _ ->
                    argInfos
                    |> List.reduce (@)
                    |> mapVisitList (fun x -> x.PossibleRange) this.VisitArgInfo

            let returnInfo =
                [returnInfo]
                |> mapVisitList (fun x -> x.PossibleRange) this.VisitArgInfo

            (argInfos @ returnInfo)
            |> tryVisitList

    abstract VisitPat: SynPat -> 'T option
    default this.VisitPat pat =
        match pat with
        | SynPat.Const (sconst, _) ->
            sconst
            |> this.TryVisit sconst.PossibleRange this.VisitConst

        | SynPat.Wild _ ->
            None

        | SynPat.Named (pat, id, _, _, _) ->
            let pat =
                pat
                |> this.TryVisit pat.Range this.VisitPat

            if pat.IsSome then pat
            else

            id
            |> this.TryVisit id.idRange (fun item -> this.VisitIdent (0, item))

        | SynPat.Typed (pat, ty, _) ->
            let pat =
                pat
                |> this.TryVisit pat.Range this.VisitPat

            if pat.IsSome then pat
            else

            ty
            |> this.TryVisit ty.Range this.VisitType

        | SynPat.Attrib (pat, attribs, _) ->
            let pat =
                pat
                |> this.TryVisit pat.Range this.VisitPat

            if pat.IsSome then pat
            else

            attribs
            |> this.TryVisitList (fun x -> x.Range) this.VisitAttributeList

        | SynPat.Or (pat1, pat2, _) ->
            let pat1 =
                pat1
                |> this.TryVisit pat1.Range this.VisitPat

            if pat1.IsSome then pat1
            else

            pat2
            |> this.TryVisit pat2.Range this.VisitPat

        | SynPat.Ands (pats, _) ->
            pats
            |> this.TryVisitList (fun x -> x.Range) this.VisitPat

        | SynPat.LongIdent (longDotId, idOpt, valTyparDeclsOpt, ctorArgs, _, _) ->
            let longDotId =
                longDotId
                |> this.TryVisit longDotId.Range this.VisitLongIdentWithDots

            if longDotId.IsSome then longDotId
            else

            let idOpt =
                match idOpt with
                | Some id ->
                    id
                    |> this.TryVisit id.idRange (fun item -> this.VisitIdent (0, item))
                | _ ->
                    None

            if idOpt.IsSome then idOpt
            else

            let valTyparDeclsOpt =
                match valTyparDeclsOpt with
                | Some valTyparDecls ->
                    valTyparDecls
                    |> this.TryVisit valTyparDecls.PossibleRange this.VisitValTyparDecls
                | _ ->
                    None

            if valTyparDeclsOpt.IsSome then valTyparDeclsOpt
            else

            ctorArgs
            |> this.TryVisit ctorArgs.PossibleRange this.VisitConstructorArgs

        | SynPat.Tuple (_, pats, _) ->
            pats
            |> this.TryVisitList (fun x -> x.Range) this.VisitPat

        | SynPat.Paren (pat, _) ->
            pat
            |> this.TryVisit pat.Range this.VisitPat

        | SynPat.ArrayOrList (_, pats, _) ->
            pats
            |> this.TryVisitList (fun x -> x.Range) this.VisitPat

        | SynPat.Record (recdData, _) ->
            let recdData, pats = List.unzip recdData
            let longIds, ids = List.unzip recdData

            let pats =
                pats
                |> mapVisitList (fun x -> x.Range) this.VisitPat

            let longIds =
                longIds
                |> List.reduce (@)
                |> mapiVisitList (fun x -> x.idRange) this.VisitIdent

            let ids =
                ids
                |> mapiVisitList (fun x -> x.idRange) this.VisitIdent

            (pats @ longIds @ ids)
            |> tryVisitList

        | SynPat.Null _ ->
            None

        | SynPat.OptionalVal (id, _) ->
            id
            |> this.TryVisit id.idRange (fun x -> this.VisitIdent (0, x))

        | SynPat.IsInst (ty, _) ->
            ty
            |> this.TryVisit ty.Range this.VisitType

        | SynPat.QuoteExpr (expr, _) ->
            expr
            |> this.TryVisit expr.Range this.VisitExpr

        | SynPat.DeprecatedCharRange (_, _, _) ->
            None

        | SynPat.InstanceMember (id1, id2, idOpt, _, _) ->
            let id1 =
                id1
                |> this.TryVisit id1.idRange (fun item -> this.VisitIdent (0, item))

            if id1.IsSome then id1
            else

            let id2 =
                id2
                |> this.TryVisit id2.idRange (fun item -> this.VisitIdent (1, item))

            if id2.IsSome then id2
            else

            match idOpt with
            | Some id ->
                id
                |> this.TryVisit id.idRange (fun item -> this.VisitIdent (2, item))
            | _ ->
                None

        | SynPat.FromParseError (pat, _) ->
            pat
            |> this.TryVisit pat.Range this.VisitPat

    abstract VisitConstructorArgs: SynConstructorArgs -> 'T option
    default this.VisitConstructorArgs ctorArgs =
        match ctorArgs with
        | Pats pats ->
            pats
            |> mapVisitList (fun x -> x.Range) this.VisitPat
            |> tryVisitList

        | NamePatPairs (pairs, _) ->
            let ids, pats = List.unzip pairs

            let ids =
                ids
                |> mapiVisitList (fun x -> x.idRange) this.VisitIdent

            let pats =
                pats
                |> mapVisitList (fun x -> x.Range) this.VisitPat

            (ids @ pats)
            |> tryVisitList

    abstract VisitBindingReturnInfo: SynBindingReturnInfo -> 'T option
    default this.VisitBindingReturnInfo returnInfo =
        match returnInfo with
        | SynBindingReturnInfo (ty, _, attribs) ->
            let ty =
                [ty]
                |> mapVisitList (fun x -> x.Range) this.VisitType

            let attribs =
                attribs
                |> mapVisitList (fun x -> x.Range) this.VisitAttributeList

            (ty @ attribs)
            |> tryVisitList

    abstract VisitExpr: SynExpr -> 'T option
    default this.VisitExpr expr =
        match expr with
        | SynExpr.Paren (expr, _, _, _) ->
            expr
            |> this.TryVisit expr.Range this.VisitExpr

        | SynExpr.Quote (opExpr, _, quoteExpr, _, _) ->
            let opExpr =
                opExpr
                |> this.TryVisit opExpr.Range this.VisitExpr

            if opExpr.IsSome then opExpr
            else

            quoteExpr
            |> this.TryVisit quoteExpr.Range this.VisitExpr

        | SynExpr.Const (sconst, _) ->
            sconst
            |> this.TryVisit sconst.PossibleRange this.VisitConst

        | SynExpr.Typed (expr, ty, _) ->
            let expr =
                expr
                |> this.TryVisit expr.Range this.VisitExpr

            if expr.IsSome then expr
            else

            ty
            |> this.TryVisit ty.Range this.VisitType

        | SynExpr.Tuple (_, exprs, _, _) ->
            exprs
            |> this.TryVisitList (fun x -> x.Range) this.VisitExpr

        | SynExpr.AnonRecd (_, copyInfoOpt, recdFields, _) ->
            let copyInfoOpt =
                match copyInfoOpt with
                | Some (expr, _) ->
                    expr
                    |> this.TryVisit expr.Range this.VisitExpr
                | _ ->
                    None

            if copyInfoOpt.IsSome then copyInfoOpt
            else

            recdFields
            |> this.TryVisitListIndex (fun (id, expr) -> unionRanges id.idRange expr.Range) (fun i (id, expr) ->
                let id =
                    id
                    |> this.TryVisit id.idRange (fun item -> this.VisitIdent (i, item))

                if id.IsSome then id
                else

                expr
                |> this.TryVisit expr.Range this.VisitExpr
            )

        | SynExpr.ArrayOrList (_, exprs, _) ->
            exprs
            |> this.TryVisitList (fun x -> x.Range) this.VisitExpr

        | SynExpr.Record (baseInfoOpt, copyInfoOpt, recdFields, _) ->
            let baseInfoOpt =
                match baseInfoOpt with
                | Some (ty, expr, _, _, _) ->
                    let ty =
                        ty
                        |> this.TryVisit ty.Range this.VisitType

                    if ty.IsSome then ty
                    else

                    expr
                    |> this.TryVisit expr.Range this.VisitExpr
                | _ ->
                    None

            if baseInfoOpt.IsSome then baseInfoOpt
            else

            let copyInfoOpt =
                match copyInfoOpt with
                | Some (expr, _) ->
                    expr
                    |> this.TryVisit expr.Range this.VisitExpr
                | _ ->
                    None

            if copyInfoOpt.IsSome then copyInfoOpt
            else

            recdFields
            |> this.TryVisitList 
                (fun ((longDotId, _), exprOpt, _) -> 
                    match exprOpt with
                    | Some expr ->
                        unionRanges longDotId.Range expr.Range
                    | _ ->
                        longDotId.Range
                )
                (fun ((longDotId, _), exprOpt, _) ->
                    let longDotId =
                        longDotId
                        |> this.TryVisit longDotId.Range this.VisitLongIdentWithDots

                    if longDotId.IsSome then longDotId
                    else

                    match exprOpt with
                    | Some expr ->
                        expr
                        |> this.TryVisit expr.Range this.VisitExpr
                    | _ ->
                        None
                )

        | SynExpr.New (_, ty, expr, _) ->
            let ty =
                ty
                |> this.TryVisit ty.Range this.VisitType

            if ty.IsSome then ty
            else

            expr
            |> this.TryVisit expr.Range this.VisitExpr

        | SynExpr.ObjExpr (ty, argOpt, bindings, extraImpls, _, _) ->
            let ty =
                ty
                |> this.TryVisit ty.Range this.VisitType

            if ty.IsSome then ty
            else

            let argOpt =
                match argOpt with
                | Some (expr, idOpt) ->
                    let expr =
                        expr
                        |> this.TryVisit expr.Range this.VisitExpr

                    if expr.IsSome then expr
                    else

                    match idOpt with
                    | Some id ->
                        id
                        |> this.TryVisit id.idRange (fun item -> this.VisitIdent (0, item))
                    | _ ->
                        None
                | _ ->
                    None

            if argOpt.IsSome then argOpt
            else

            let bindings =
                bindings
                |> this.TryVisitList (fun x -> x.RangeOfBindingAndRhs) this.VisitBinding

            if bindings.IsSome then bindings
            else

            extraImpls
            |> this.TryVisitList (fun x -> x.Range) this.VisitInterfaceImpl

        | SynExpr.While (_, whileExpr, doExpr, _) ->
            let whileExpr =
                whileExpr
                |> this.TryVisit whileExpr.Range this.VisitExpr

            if whileExpr.IsSome then whileExpr
            else

            doExpr
            |> this.TryVisit doExpr.Range this.VisitExpr

        | SynExpr.For (_, id, expr, _, toBody, doBody, _) ->
            let id =
                id
                |> this.TryVisit id.idRange (fun item -> this.VisitIdent (0, item))

            if id.IsSome then id
            else

            let expr =
                expr
                |> this.TryVisit expr.Range this.VisitExpr

            if expr.IsSome then expr
            else

            let toBody =
                toBody
                |> this.TryVisit toBody.Range this.VisitExpr

            if toBody.IsSome then toBody
            else

            doBody
            |> this.TryVisit doBody.Range this.VisitExpr

        | SynExpr.ForEach (_, _, _, pat, enumExpr, bodyExpr, _) ->
            let pat =
                pat
                |> this.TryVisit pat.Range this.VisitPat

            if pat.IsSome then pat
            else

            let enumExpr =
                enumExpr
                |> this.TryVisit enumExpr.Range this.VisitExpr

            if enumExpr.IsSome then enumExpr
            else

            bodyExpr
            |> this.TryVisit bodyExpr.Range this.VisitExpr

        | SynExpr.ArrayOrListOfSeqExpr (_, expr, _) ->
            expr
            |> this.TryVisit expr.Range this.VisitExpr

        | SynExpr.CompExpr (_, _, expr, _) ->
            expr
            |> this.TryVisit expr.Range this.VisitExpr

        | SynExpr.Lambda (_, _, argSimplePats, bodyExpr, _) ->
            let argSimplePats =
                argSimplePats
                |> this.TryVisit argSimplePats.Range this.VisitSimplePats

            if argSimplePats.IsSome then argSimplePats
            else

            bodyExpr
            |> this.TryVisit bodyExpr.Range this.VisitExpr

        | SynExpr.MatchLambda (_, _, clauses, _, _) ->
            clauses
            |> this.TryVisitList (fun x -> x.Range) this.VisitMatchClause

        | SynExpr.Match (_, expr, clauses, _) ->
            let expr =
                expr
                |> this.TryVisit expr.Range this.VisitExpr

            if expr.IsSome then expr
            else

            clauses
            |> this.TryVisitList (fun x -> x.Range) this.VisitMatchClause

        | SynExpr.Do (expr, _) ->
            expr
            |> this.TryVisit expr.Range this.VisitExpr

        | SynExpr.Assert (expr, _) ->
            expr
            |> this.TryVisit expr.Range this.VisitExpr

        | SynExpr.App (_, _, funcExpr, argExpr, _) ->
            let funcExpr =
                funcExpr
                |> this.TryVisit funcExpr.Range this.VisitExpr

            if funcExpr.IsSome then funcExpr
            else

            argExpr
            |> this.TryVisit argExpr.Range this.VisitExpr

        | SynExpr.TypeApp (expr, _, tys, _, _, _, _) ->
            let expr =
                expr
                |> this.TryVisit expr.Range this.VisitExpr

            if expr.IsSome then expr
            else

            tys
            |> this.TryVisitList (fun x -> x.Range) this.VisitType

        | SynExpr.LetOrUse (_, _, bindings, bodyExpr, _) ->
            let bindings =
                bindings
                |> this.TryVisitList (fun x -> x.RangeOfBindingAndRhs) this.VisitBinding

            if bindings.IsSome then bindings
            else

            bodyExpr
            |> this.TryVisit bodyExpr.Range this.VisitExpr

        | SynExpr.TryWith (tryExpr, _, clauses, _, _, _, _) ->
            let tryExpr =
                tryExpr
                |> this.TryVisit tryExpr.Range this.VisitExpr

            if tryExpr.IsSome then tryExpr
            else

            clauses
            |> this.TryVisitList (fun x -> x.Range) this.VisitMatchClause

        | SynExpr.TryFinally (tryExpr, finallyExpr, _, _, _) ->
            let tryExpr =
                tryExpr
                |> this.TryVisit tryExpr.Range this.VisitExpr

            if tryExpr.IsSome then tryExpr
            else

            finallyExpr
            |> this.TryVisit finallyExpr.Range this.VisitExpr

        | SynExpr.Lazy (expr, _) ->
            expr
            |> this.TryVisit expr.Range this.VisitExpr

        | SynExpr.Sequential (_, _, expr1, expr2, _) ->
            let expr1 =
                expr1
                |> this.TryVisit expr1.Range this.VisitExpr

            if expr1.IsSome then expr1
            else

            expr2
            |> this.TryVisit expr2.Range this.VisitExpr

        | SynExpr.IfThenElse (ifExpr, thenExpr, elseExprOpt, _, _, _, _) ->
            let ifExpr =
                ifExpr
                |> this.TryVisit ifExpr.Range this.VisitExpr

            if ifExpr.IsSome then ifExpr
            else

            let thenExpr =
                thenExpr
                |> this.TryVisit thenExpr.Range this.VisitExpr

            if thenExpr.IsSome then thenExpr
            else

            match elseExprOpt with
            | Some elseExpr ->
                elseExpr
                |> this.TryVisit elseExpr.Range this.VisitExpr
            | _ ->
                None

        | SynExpr.Ident id ->
            id
            |> this.TryVisit id.idRange (fun x -> this.VisitIdent (0, x))

        | SynExpr.LongIdent (_, longDotId, simplePatAltIdInfoOpt, _) ->
            let longDotId =
                longDotId
                |> this.TryVisit longDotId.Range this.VisitLongIdentWithDots

            if longDotId.IsSome then longDotId
            else

            match simplePatAltIdInfoOpt with
            | Some { contents = simplePatAltIdInfo } ->
                simplePatAltIdInfo
                |> this.TryVisit simplePatAltIdInfo.Range this.VisitSimplePatAlternativeIdInfo
            | _ ->
                None

        | SynExpr.LongIdentSet (longDotId, expr, _) ->
            let longDotId =
                longDotId
                |> this.TryVisit longDotId.Range this.VisitLongIdentWithDots

            if longDotId.IsSome then longDotId
            else

            expr
            |> this.TryVisit expr.Range this.VisitExpr

        | SynExpr.DotGet (expr, _, longDotId, _) ->
            let expr =
                expr
                |> this.TryVisit expr.Range this.VisitExpr

            if expr.IsSome then expr
            else

            longDotId
            |> this.TryVisit longDotId.Range this.VisitLongIdentWithDots

        | SynExpr.DotSet (expr1, longDotId, expr2, _) ->
            let expr1 =
                expr1
                |> this.TryVisit expr1.Range this.VisitExpr

            if expr1.IsSome then expr1
            else

            let longDotId =
                longDotId
                |> this.TryVisit longDotId.Range this.VisitLongIdentWithDots

            if longDotId.IsSome then longDotId
            else

            expr2
            |> this.TryVisit expr2.Range this.VisitExpr

        | SynExpr.Set (expr1, expr2, _) ->
            let expr1 =
                expr1
                |> this.TryVisit expr1.Range this.VisitExpr

            if expr1.IsSome then expr1
            else

            expr2
            |> this.TryVisit expr2.Range this.VisitExpr

        | SynExpr.DotIndexedGet (expr, args, _, _) ->
            let expr =
                expr
                |> this.TryVisit expr.Range this.VisitExpr

            if expr.IsSome then expr
            else

            args
            |> this.TryVisitList (fun x -> x.Range) this.VisitIndexerArg

        | SynExpr.DotIndexedSet (objectExpr, args, valueExpr, _, _, _) ->
            let objectExpr =
                objectExpr
                |> this.TryVisit objectExpr.Range this.VisitExpr

            if objectExpr.IsSome then objectExpr
            else

            let args =
                args
                |> this.TryVisitList (fun x -> x.Range) this.VisitIndexerArg

            if args.IsSome then args
            else

            valueExpr
            |> this.TryVisit valueExpr.Range this.VisitExpr

        | SynExpr.NamedIndexedPropertySet (longDotId, expr1, expr2, _) ->
            let longDotId =
                longDotId
                |> this.TryVisit longDotId.Range this.VisitLongIdentWithDots

            if longDotId.IsSome then longDotId
            else

            let expr1 =
                expr1
                |> this.TryVisit expr1.Range this.VisitExpr

            if expr1.IsSome then expr1
            else

            expr2
            |> this.TryVisit expr2.Range this.VisitExpr

        | SynExpr.DotNamedIndexedPropertySet (expr1, longDotId, expr2, expr3, _) ->
            let expr1 =
                expr1
                |> this.TryVisit expr1.Range this.VisitExpr

            if expr1.IsSome then expr1
            else

            let longDotId =
                longDotId
                |> this.TryVisit longDotId.Range this.VisitLongIdentWithDots

            if longDotId.IsSome then longDotId
            else

            let expr2 =
                expr2
                |> this.TryVisit expr2.Range this.VisitExpr

            if expr2.IsSome then expr2
            else

            expr3
            |> this.TryVisit expr3.Range this.VisitExpr

        | SynExpr.TypeTest (expr, ty, _) ->
            let expr =
                expr
                |> this.TryVisit expr.Range this.VisitExpr

            if expr.IsSome then expr
            else

            ty
            |> this.TryVisit ty.Range this.VisitType

        | SynExpr.Upcast (expr, ty, _) ->
            let expr =
                expr
                |> this.TryVisit expr.Range this.VisitExpr

            if expr.IsSome then expr
            else

            ty
            |> this.TryVisit ty.Range this.VisitType

        | SynExpr.Downcast (expr, ty, _) ->
            let expr =
                expr
                |> this.TryVisit expr.Range this.VisitExpr

            if expr.IsSome then expr
            else

            ty
            |> this.TryVisit ty.Range this.VisitType

        | SynExpr.InferredUpcast (expr, _) ->
            expr
            |> this.TryVisit expr.Range this.VisitExpr

        | SynExpr.InferredDowncast (expr, _) ->
            expr
            |> this.TryVisit expr.Range this.VisitExpr

        | SynExpr.Null _ ->
            None

        | SynExpr.AddressOf (_, expr, _, _) ->
            expr
            |> this.TryVisit expr.Range this.VisitExpr

        | SynExpr.TraitCall (typars, memberSig, expr, _) ->
            let typars =
                typars
                |> this.TryVisitList (fun x -> x.Range) this.VisitTypar

            if typars.IsSome then typars
            else

            let memberSig =
                memberSig
                |> this.TryVisit memberSig.Range this.VisitMemberSig

            if memberSig.IsSome then memberSig
            else

            expr
            |> this.TryVisit expr.Range this.VisitExpr

        | SynExpr.JoinIn (expr1, _, expr2, _) ->
            let expr1 =
                expr1
                |> this.TryVisit expr1.Range this.VisitExpr

            if expr1.IsSome then expr1
            else

            expr2
            |> this.TryVisit expr2.Range this.VisitExpr

        | SynExpr.ImplicitZero _ ->
            None

        | SynExpr.YieldOrReturn (_, expr, _) ->
            expr
            |> this.TryVisit expr.Range this.VisitExpr

        | SynExpr.YieldOrReturnFrom (_, expr, _) ->
            expr
            |> this.TryVisit expr.Range this.VisitExpr

        | SynExpr.LetOrUseBang (_, _, _, pat, expr1, expr2, _) ->
            let pat =
                pat
                |> this.TryVisit pat.Range this.VisitPat

            if pat.IsSome then pat
            else

            let expr1 =
                expr1
                |> this.TryVisit expr1.Range this.VisitExpr

            if expr1.IsSome then expr1
            else

            expr2
            |> this.TryVisit expr2.Range this.VisitExpr

        | SynExpr.MatchBang (_, expr, clauses, _) ->
            let expr =
                expr
                |> this.TryVisit expr.Range this.VisitExpr

            if expr.IsSome then expr
            else

            clauses
            |> this.TryVisitList (fun x -> x.Range) this.VisitMatchClause

        | SynExpr.DoBang (expr, _) ->
            expr
            |> this.TryVisit expr.Range this.VisitExpr

        | SynExpr.LibraryOnlyILAssembly (_, tys1, exprs, tys2, _) ->
            let tys1 =
                tys1
                |> this.TryVisitList (fun x -> x.Range) this.VisitType

            if tys1.IsSome then tys1
            else

            let exprs =
                exprs
                |> this.TryVisitList (fun x -> x.Range) this.VisitExpr

            if exprs.IsSome then exprs
            else

            tys2
            |> this.TryVisitList (fun x -> x.Range) this.VisitType

        | SynExpr.LibraryOnlyStaticOptimization (staticOptConstraints, expr1, expr2, _) ->
            let staticOptConstraints =
                staticOptConstraints
                |> this.TryVisitList (fun x -> x.Range) this.VisitStaticOptimizationConstraint

            if staticOptConstraints.IsSome then staticOptConstraints
            else

            let expr1 =
                expr1
                |> this.TryVisit expr1.Range this.VisitExpr

            if expr1.IsSome then expr1
            else

            expr2
            |> this.TryVisit expr2.Range this.VisitExpr

        | SynExpr.LibraryOnlyUnionCaseFieldGet (expr, longId, _, _) ->
            let expr =
                expr
                |> this.TryVisit expr.Range this.VisitExpr

            if expr.IsSome then expr
            else

            longId
            |> this.TryVisitListIndex (fun x -> x.idRange) (fun i item -> this.VisitIdent (i, item))

        | SynExpr.LibraryOnlyUnionCaseFieldSet (expr1, longId, _, expr2, _) ->
            let expr1 =
                expr1
                |> this.TryVisit expr1.Range this.VisitExpr

            if expr1.IsSome then expr1
            else

            let longId =
                longId
                |> this.TryVisitListIndex (fun x -> x.idRange) (fun i item -> this.VisitIdent (i, item))

            if longId.IsSome then longId
            else

            expr2
            |> this.TryVisit expr2.Range this.VisitExpr

        | SynExpr.ArbitraryAfterError _ ->
            None

        | SynExpr.FromParseError (expr, _) ->
            expr
            |> this.TryVisit expr.Range this.VisitExpr

        | SynExpr.DiscardAfterMissingQualificationAfterDot (expr, _) ->
            expr
            |> this.TryVisit expr.Range this.VisitExpr

        | SynExpr.Fixed (expr, _) ->
            expr
            |> this.TryVisit expr.Range this.VisitExpr
                
    abstract VisitStaticOptimizationConstraint: SynStaticOptimizationConstraint -> 'T option
    default this.VisitStaticOptimizationConstraint staticOptConstraint =
        match staticOptConstraint with
        | WhenTyparTyconEqualsTycon (typar, ty, _) ->
            let typar =
                [typar]
                |> mapVisitList (fun x -> x.Range) this.VisitTypar

            let ty =
                [ty]
                |> mapVisitList (fun x -> x.Range) this.VisitType

            (typar @ ty)
            |> tryVisitList

        | WhenTyparIsStruct (typar, _) ->
            typar
            |> this.TryVisit typar.Range this.VisitTypar

    abstract VisitIndexerArg: SynIndexerArg -> 'T option
    default this.VisitIndexerArg indexerArg =
        match indexerArg with
        | SynIndexerArg.Two (expr1, expr2) ->
            [expr1;expr2]
            |> mapVisitList (fun x -> x.Range) this.VisitExpr
            |> tryVisitList

        | SynIndexerArg.One expr ->
            expr
            |> this.TryVisit expr.Range this.VisitExpr

    abstract VisitSimplePatAlternativeIdInfo: SynSimplePatAlternativeIdInfo -> 'T option
    default this.VisitSimplePatAlternativeIdInfo simplePatAltIdInfo =
        match simplePatAltIdInfo with
        | Undecided id ->
            id
            |> this.TryVisit id.idRange (fun x -> this.VisitIdent (0, x))

        | Decided id ->
            id
            |> this.TryVisit id.idRange (fun x -> this.VisitIdent (0, x))

    abstract VisitMatchClause: SynMatchClause -> 'T option
    default this.VisitMatchClause matchClause =
        match matchClause with
        | Clause (pat, whenExprOpt, expr, _, _) ->
            let pat =
                [pat]
                |> mapVisitList (fun x -> x.Range) this.VisitPat

            let whenExprOpt =
                match whenExprOpt with
                | Some whenExpr ->
                    [whenExpr]
                    |> mapVisitList (fun x -> x.Range) this.VisitExpr
                | _ ->
                    []

            let expr =
                [expr]
                |> mapVisitList (fun x -> x.Range) this.VisitExpr

            (pat @ whenExprOpt @ expr)
            |> tryVisitList

    abstract VisitInterfaceImpl: SynInterfaceImpl -> 'T option
    default this.VisitInterfaceImpl interfaceImpl =
        match interfaceImpl with
        | InterfaceImpl (ty, bindings, _) ->
            let ty =
                [ty]
                |> mapVisitList (fun x -> x.Range) this.VisitType
            
            let bindings =
                bindings
                |> mapVisitList (fun x -> x.RangeOfBindingAndRhs) this.VisitBinding

            (ty @ bindings)
            |> tryVisitList

    abstract VisitTypeDefn: SynTypeDefn -> 'T option
    default this.VisitTypeDefn typeDefn =
        match typeDefn with
        | TypeDefn (compInfo, typeDefnRepr, memberDefns, _) ->
            let compInfo =
                [compInfo]
                |> mapVisitList (fun x -> x.Range) this.VisitComponentInfo

            let typeDefnRepr =
                [typeDefnRepr]
                |> mapVisitList (fun x -> x.Range) this.VisitTypeDefnRepr

            let memberDefns =
                memberDefns
                |> mapVisitList (fun x -> x.Range) this.VisitMemberDefn

            (compInfo @ typeDefnRepr @ memberDefns)
            |> tryVisitList

    abstract VisitTypeDefnRepr: SynTypeDefnRepr -> 'T option
    default this.VisitTypeDefnRepr repr =
        match repr with
        | SynTypeDefnRepr.ObjectModel (kind, memberDefns, _) ->
            let kind =
                [kind]
                |> mapVisitList (fun x -> x.PossibleRange) this.VisitTypeDefnKind

            let memberDefns =
                memberDefns
                |> mapVisitList (fun x -> x.Range) this.VisitMemberDefn

            (kind @ memberDefns)
            |> tryVisitList

        | SynTypeDefnRepr.Simple (simpleRepr, _) ->
            simpleRepr
            |> this.TryVisit simpleRepr.Range this.VisitTypeDefnSimpleRepr

        | SynTypeDefnRepr.Exception exRepr ->
            exRepr
            |> this.TryVisit exRepr.Range this.VisitExceptionDefnRepr

    abstract VisitMemberDefn: SynMemberDefn -> 'T option
    default this.VisitMemberDefn memberDefn =
        match memberDefn with
        | SynMemberDefn.Open (longId, _) ->
            longId
            |> mapiVisitList (fun x -> x.idRange) this.VisitIdent
            |> tryVisitList

        | SynMemberDefn.Member (binding, _) ->
            binding
            |> this.TryVisit binding.RangeOfBindingAndRhs this.VisitBinding

        | SynMemberDefn.ImplicitCtor (_, attribs, ctorArgs, idOpt, _) ->
            let attribs =
                attribs
                |> mapVisitList (fun x -> x.Range) this.VisitAttributeList

            let ctorArgs =
                [ctorArgs]
                |> mapVisitList (fun x -> x.Range) this.VisitSimplePats

            let idOpt =
                match idOpt with
                | Some id ->
                    [id]
                    |> mapiVisitList (fun x -> x.idRange) this.VisitIdent
                | _ ->
                    []

            (attribs @ ctorArgs @ idOpt)
            |> tryVisitList

        | SynMemberDefn.ImplicitInherit (ty, expr, idOpt, _) ->
            let ty =
                [ty]
                |> mapVisitList (fun x -> x.Range) this.VisitType

            let expr =
                [expr]
                |> mapVisitList (fun x -> x.Range) this.VisitExpr

            let idOpt =
                match idOpt with
                | Some id ->
                    [id]
                    |> mapiVisitList (fun x -> x.idRange) this.VisitIdent
                | _ ->
                    []

            (ty @ expr @ idOpt)
            |> tryVisitList

        | SynMemberDefn.LetBindings (bindings, _, _, _) ->
            bindings
            |> mapVisitList (fun x -> x.RangeOfBindingAndRhs) this.VisitBinding
            |> tryVisitList

        | SynMemberDefn.AbstractSlot (valSig, _, _) ->
            valSig
            |> this.TryVisit valSig.Range this.VisitValSig

        | SynMemberDefn.Interface (ty, memberDefnsOpt, _) ->
            let ty =
                [ty]
                |> mapVisitList (fun x -> x.Range) this.VisitType

            let memberDefnsOpt =
                match memberDefnsOpt with
                | Some memberDefns ->
                    memberDefns
                    |> mapVisitList (fun x -> x.Range) this.VisitMemberDefn
                | _ ->
                    []

            (ty @ memberDefnsOpt)
            |> tryVisitList

        | SynMemberDefn.Inherit (ty, idOpt, _) ->
            let ty =
                [ty]
                |> mapVisitList (fun x -> x.Range) this.VisitType

            let idOpt =
                match idOpt with
                | Some id ->
                    [id]
                    |> mapiVisitList (fun x -> x.idRange) this.VisitIdent
                | _ ->
                    []

            (ty @ idOpt)
            |> tryVisitList

        | SynMemberDefn.ValField (field, _) ->
            field
            |> this.TryVisit field.Range this.VisitField

        | SynMemberDefn.NestedType (typeDefn, _, _) ->
            typeDefn
            |> this.TryVisit typeDefn.Range this.VisitTypeDefn

        | SynMemberDefn.AutoProperty (attribs, _, id, tyOpt, _, _, _, _, expr, _, _) ->
            let attribs =
                attribs
                |> mapVisitList (fun x -> x.Range) this.VisitAttributeList

            let id =
                [id]
                |> mapiVisitList (fun x -> x.idRange) this.VisitIdent

            let tyOpt =
                match tyOpt with
                | Some ty ->
                    [ty]
                    |> mapVisitList (fun x -> x.Range) this.VisitType
                | _ ->
                    []

            let expr =
                [expr]
                |> mapVisitList (fun x -> x.Range) this.VisitExpr

            (attribs @ id @ tyOpt @ expr)
            |> tryVisitList

    abstract VisitExceptionDefn: SynExceptionDefn -> 'T option
    default this.VisitExceptionDefn exDefn =
        match exDefn with
        | SynExceptionDefn (exRepr, memberDefns, _) ->
            let exRepr =
                [exRepr]
                |> mapVisitList (fun x -> x.Range) this.VisitExceptionDefnRepr

            let memberDefns =
                memberDefns
                |> mapVisitList (fun x -> x.Range) this.VisitMemberDefn

            (exRepr @ memberDefns)
            |> tryVisitList

    abstract VisitParsedHashDirective: ParsedHashDirective -> 'T option
    default this.VisitParsedHashDirective hashDirective =
        match hashDirective with
        ParsedHashDirective (_, _, _) ->
            None

    abstract VisitAttributeList: SynAttributeList -> 'T option
    default this.VisitAttributeList attribs =
        attribs.Attributes
        |> mapVisitList (fun x -> x.Range) this.VisitAttribute
        |> tryVisitList

    abstract VisitAttribute: SynAttribute -> 'T option
    default this.VisitAttribute attrib =
        let typeName =
            [attrib.TypeName]
            |> mapVisitList (fun x -> x.Range) this.VisitLongIdentWithDots

        let argExpr =
            [attrib.ArgExpr]
            |> mapVisitList (fun x -> x.Range) this.VisitExpr

        let targetOpt =
            match attrib.Target with
            | Some target ->
                [target]
                |> mapiVisitList (fun x -> x.idRange) this.VisitIdent
            | _ ->
                []

        (typeName @ argExpr @ targetOpt)
        |> tryVisitList