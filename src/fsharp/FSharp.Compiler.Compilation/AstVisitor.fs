namespace FSharp.Compiler.Compilation

open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler.Ast
open FSharp.Compiler.Range

[<AutoOpen>]
module AstVisitorHelpers =
    
    let isZeroRange (r: range) =
        posEq r.Start r.End

    type ParsedInput with

        member this.PossibleRange =
            match this with
            | ParsedInput.ImplFile (ParsedImplFileInput (modules=modules)) ->
                match modules with
                | [] -> range0
                | _ ->
                    modules
                    |> List.map (fun (SynModuleOrNamespace (longId=longId;range=m)) -> 
                        match longId with
                        | [] -> m
                        | _ ->
                            let longIdRange =
                                longId
                                |> List.map (fun x -> x.idRange)
                                |> List.reduce unionRanges
                            unionRanges longIdRange m
                    )
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
                let ranges =
                    argInfos
                    |> List.reduce (@)
                    |> List.append [argInfo]
                    |> List.map (fun x -> x.PossibleRange)
                    |> List.filter (fun x -> not (isZeroRange x))

                match ranges with
                | [] -> range0
                | _ ->
                    ranges
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
                attribs
                |> List.map (fun x -> x.Range)
                |> List.append [typar.Range]
                |> List.reduce unionRanges

    type SynValTyparDecls with

        member this.PossibleRange =
            match this with
            | SynValTyparDecls (typarDecls, _, constraints) ->
                let ranges =
                    typarDecls
                    |> List.map (fun x -> x.Range)
                    |> List.append (constraints |> List.map (fun x -> x.Range))

                match ranges with
                | [] -> range0
                | _ ->
                    ranges
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

    let tryVisit m visit item =
        if this.CanVisit m then
            visit item
        else
            None

    let tryVisitList xs : 'T option =
        xs
        |> List.sortWith (fun (getRange1, _) (getRange2, _)->
            let r1 = getRange1 ()
            let r2 = getRange2 ()
            rangeOrder.Compare (r1, r2)
        )
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

    abstract CanVisit: range -> bool  
    default this.CanVisit _ = true

    abstract VisitParsedInput: ParsedInput -> 'T option
    default this.VisitParsedInput parsedInput =
        match parsedInput with
        | ParsedInput.ImplFile (ParsedImplFileInput (_, _, _, _, _, modules, _)) -> 
            modules
            |> mapVisitList (fun x -> x.Range) this.VisitModuleOrNamespace
            |> tryVisitList
        | ParsedInput.SigFile (ParsedSigFileInput (_, _, _, _, modules)) -> 
            None

    abstract VisitModuleOrNamespace: SynModuleOrNamespace -> 'T option
    default this.VisitModuleOrNamespace moduleOrNamespace =
        match moduleOrNamespace with
        | SynModuleOrNamespace (longId, _, _, decls, _, attribs, _, _) ->
            let longId =
                longId
                |> mapVisitList (fun x -> x.idRange) this.VisitIdent

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
            |> tryVisit expr.Range this.VisitExpr

        | SynModuleDecl.Types (typeDefns, _) ->
            typeDefns
            |> mapVisitList (fun x -> x.Range) this.VisitTypeDefn
            |> tryVisitList

        | SynModuleDecl.Exception (exDefn, _) ->
            exDefn
            |> tryVisit exDefn.Range this.VisitExceptionDefn

        | SynModuleDecl.Open _ ->
            None

        | SynModuleDecl.Attributes (attribs, _) ->
            attribs
            |> mapVisitList (fun x -> x.Range) this.VisitAttributeList
            |> tryVisitList

        | SynModuleDecl.HashDirective (hashDirective, _) ->
            hashDirective
            |> tryVisit hashDirective.Range this.VisitParsedHashDirective

        | SynModuleDecl.NamespaceFragment moduleOrNamespace ->
            moduleOrNamespace
            |> tryVisit moduleOrNamespace.Range this.VisitModuleOrNamespace

    abstract VisitLongIdentWithDots: LongIdentWithDots -> 'T option
    default this.VisitLongIdentWithDots longDotId =
        match longDotId with
        | LongIdentWithDots (longId, _) ->
            longId
            |> mapVisitList (fun x -> x.idRange) this.VisitIdent
            |> tryVisitList

    abstract VisitIdent: Ident -> 'T option
    default this.VisitIdent _id =
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
                |> mapVisitList (fun x -> x.idRange) this.VisitIdent

            (attribs @ typarDecls @ constraints @ longId)
            |> tryVisitList

    abstract VisitTypeConstraint: SynTypeConstraint -> 'T option
    default this.VisitTypeConstraint c =
        match c with
        | SynTypeConstraint.WhereTyparIsValueType (typar, _) ->
            typar
            |> tryVisit typar.Range this.VisitTypar

        | SynTypeConstraint.WhereTyparIsReferenceType (typar, _) ->
            typar
            |> tryVisit typar.Range this.VisitTypar

        | SynTypeConstraint.WhereTyparIsUnmanaged (typar, _) ->
            typar
            |> tryVisit typar.Range this.VisitTypar

        | SynTypeConstraint.WhereTyparSupportsNull (typar, _) ->
            typar
            |> tryVisit typar.Range this.VisitTypar

        | SynTypeConstraint.WhereTyparIsComparable (typar, _) ->
            typar
            |> tryVisit typar.Range this.VisitTypar

        | SynTypeConstraint.WhereTyparIsEquatable (typar, _) ->
            typar
            |> tryVisit typar.Range this.VisitTypar

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
            |> tryVisit valSig.Range this.VisitValSig

        | SynMemberSig.Interface (ty, _) ->
            ty
            |> tryVisit ty.Range this.VisitType

        | SynMemberSig.Inherit (ty, _) ->
            ty
            |> tryVisit ty.Range this.VisitType

        | SynMemberSig.ValField (field, _) ->
            field
            |> tryVisit field.Range this.VisitField

        | SynMemberSig.NestedType (typeDefnSig, _) ->
            typeDefnSig
            |> tryVisit typeDefnSig.Range this.VisitTypeDefnSig

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
            |> tryVisit simpleRepr.Range this.VisitTypeDefnSimpleRepr

        | SynTypeDefnSigRepr.Exception exRepr ->
            exRepr
            |> tryVisit exRepr.Range this.VisitExceptionDefnRepr

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
                    |> mapVisitList (fun x -> x.idRange) this.VisitIdent
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
                |> mapVisitList (fun x -> x.idRange) this.VisitIdent

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
                    |> mapVisitList (fun x -> x.idRange) this.VisitIdent
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
                            |> mapVisitList (fun x -> x.idRange) this.VisitIdent
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
            |> tryVisit ty.Range this.VisitType

        | SynTypeDefnSimpleRepr.None _ ->
            None

        | SynTypeDefnSimpleRepr.Exception exDefnRepr ->
            exDefnRepr
            |> tryVisit exDefnRepr.Range this.VisitExceptionDefnRepr

    abstract VisitSimplePat: SynSimplePat -> 'T option
    default this.VisitSimplePat simplePat =
        match simplePat with
        | SynSimplePat.Id (id, simplePatInfoOpt, _, _, _, m) ->
            let id =
                [id]
                |> mapVisitList (fun x -> x.idRange) this.VisitIdent

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
                |> mapVisitList (fun x -> x.idRange) this.VisitIdent

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
            |> mapVisitList (fun x -> x.idRange) this.VisitIdent
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
            |> tryVisit typar.Range this.VisitTypar

    abstract VisitRationalConst: SynRationalConst -> 'T option
    default this.VisitRationalConst rationalConst =
        match rationalConst with
        | SynRationalConst.Integer _ ->
            None

        | SynRationalConst.Rational _ ->
            None

        | SynRationalConst.Negate rationalConst ->
            rationalConst
            |> tryVisit rationalConst.PossibleRange this.VisitRationalConst

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
                    |> mapVisitList (fun x -> x.idRange) this.VisitIdent
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
                |> mapVisitList (fun x -> x.idRange) this.VisitIdent

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
            |> tryVisit longDotId.Range this.VisitLongIdentWithDots

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
                |> mapVisitList (fun x -> x.idRange) this.VisitIdent

            let tys =
                tys
                |> mapVisitList (fun x -> x.Range) this.VisitType

            (ids @ tys)
            |> tryVisitList

        | SynType.Array (_, ty, _) ->
            ty
            |> tryVisit ty.Range this.VisitType

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
            |> tryVisit typar.Range this.VisitTypar

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
            |> tryVisit ty.Range this.VisitType

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
            |> tryVisit sconst.PossibleRange this.VisitConst

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
            |> tryVisit id.idRange this.VisitIdent

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
                |> mapVisitList (fun x -> x.Range) this.VisitAttributeList

            let valData =
                [valData]
                |> mapVisitList (fun x -> x.Range) this.VisitValData

            let headPat =
                [headPat]
                |> mapVisitList (fun x -> x.Range) this.VisitPat

            let returnInfoOpt =
                match returnInfoOpt with
                | Some returnInfo ->
                    [returnInfo]
                    |> mapVisitList (fun x -> x.Range) this.VisitBindingReturnInfo
                | _ ->
                    []

            let expr =
                [expr]
                |> mapVisitList (fun x -> x.Range) this.VisitExpr

            (attribs @ valData @ headPat @ returnInfoOpt @ expr)
            |> tryVisitList

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
                    |> mapVisitList (fun x -> x.idRange) this.VisitIdent
                | _ ->
                    []

            (valInfo @ idOpt)
            |> tryVisitList

    abstract VisitValInfo: SynValInfo -> 'T option
    default this.VisitValInfo valInfo =
        match valInfo with
        | SynValInfo (argInfos, returnInfo) ->
            let argInfos =
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
            |> tryVisit sconst.PossibleRange this.VisitConst

        | SynPat.Wild _ ->
            None

        | SynPat.Named (pat, id, _, _, _) ->
            let pat =
                [pat]
                |> mapVisitList (fun x -> x.Range) this.VisitPat

            let id =
                [id]
                |> mapVisitList (fun x -> x.idRange) this.VisitIdent

            (pat @ id)
            |> tryVisitList

        | SynPat.Typed (pat, ty, _) ->
            let pat =
                [pat]
                |> mapVisitList (fun x -> x.Range) this.VisitPat

            let ty =
                [ty]
                |> mapVisitList (fun x -> x.Range) this.VisitType

            (pat @ ty)
            |> tryVisitList

        | SynPat.Attrib (pat, attribs, _) ->
            let pat =
                [pat]
                |> mapVisitList (fun x -> x.Range) this.VisitPat

            let attribs =
                attribs
                |> mapVisitList (fun x -> x.Range) this.VisitAttributeList

            (pat @ attribs)
            |> tryVisitList

        | SynPat.Or (pat1, pat2, _) ->
            [pat1;pat2]
            |> mapVisitList (fun x -> x.Range) this.VisitPat
            |> tryVisitList

        | SynPat.Ands (pats, _) ->
            pats
            |> mapVisitList (fun x -> x.Range) this.VisitPat
            |> tryVisitList

        | SynPat.LongIdent (longDotId, idOpt, valTyparDeclsOpt, ctorArgs, _, _) ->
            let longDotId =
                [longDotId]
                |> mapVisitList (fun x -> x.Range) this.VisitLongIdentWithDots

            let idOpt =
                match idOpt with
                | Some id ->
                    [id]
                    |> mapVisitList (fun x -> x.idRange) this.VisitIdent
                | _ ->
                    []

            let valTyparDeclsOpt =
                match valTyparDeclsOpt with
                | Some valTyparDecls ->
                    [valTyparDecls]
                    |> mapVisitList (fun x -> x.PossibleRange) this.VisitValTyparDecls
                | _ ->
                    []

            let ctorArgs =
                [ctorArgs]
                |> mapVisitList (fun x -> x.PossibleRange) this.VisitConstructorArgs

            (longDotId @ idOpt @ valTyparDeclsOpt @ ctorArgs)
            |> tryVisitList

        | SynPat.Tuple (_, pats, _) ->
            pats
            |> mapVisitList (fun x -> x.Range) this.VisitPat
            |> tryVisitList

        | SynPat.Paren (pat, _) ->
            pat
            |> tryVisit pat.Range this.VisitPat

        | SynPat.ArrayOrList (_, pats, _) ->
            pats
            |> mapVisitList (fun x -> x.Range) this.VisitPat
            |> tryVisitList

        | SynPat.Record (recdData, _) ->
            let recdData, pats = List.unzip recdData
            let longIds, ids = List.unzip recdData

            let pats =
                pats
                |> mapVisitList (fun x -> x.Range) this.VisitPat

            let longIds =
                longIds
                |> List.reduce (@)
                |> mapVisitList (fun x -> x.idRange) this.VisitIdent

            let ids =
                ids
                |> mapVisitList (fun x -> x.idRange) this.VisitIdent

            (pats @ longIds @ ids)
            |> tryVisitList

        | SynPat.Null _ ->
            None

        | SynPat.OptionalVal (id, _) ->
            id
            |> tryVisit id.idRange this.VisitIdent

        | SynPat.IsInst (ty, _) ->
            ty
            |> tryVisit ty.Range this.VisitType

        | SynPat.QuoteExpr (expr, _) ->
            expr
            |> tryVisit expr.Range this.VisitExpr

        | SynPat.DeprecatedCharRange (_, _, _) ->
            None

        | SynPat.InstanceMember (id1, id2, idOpt, _, _) ->
            let ids =
                [id1;id2]
                |> mapVisitList (fun x -> x.idRange) this.VisitIdent

            let idOpt =
                match idOpt with
                | Some id ->
                    [id]
                    |> mapVisitList (fun x -> x.idRange) this.VisitIdent
                | _ ->
                    []

            (ids @ idOpt)
            |> tryVisitList

        | SynPat.FromParseError (pat, _) ->
            pat
            |> tryVisit pat.Range this.VisitPat

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
                |> mapVisitList (fun x -> x.idRange) this.VisitIdent

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
            |> tryVisit expr.Range this.VisitExpr

        | SynExpr.Quote (opExpr, _, quoteExpr, _, _) ->
            [opExpr;quoteExpr]
            |> mapVisitList (fun x -> x.Range) this.VisitExpr
            |> tryVisitList

        | SynExpr.Const (sconst, _) ->
            sconst
            |> tryVisit sconst.PossibleRange this.VisitConst

        | SynExpr.Typed (expr, ty, _) ->
            let expr =
                [expr]
                |> mapVisitList (fun x -> x.Range) this.VisitExpr

            let ty =
                [ty]
                |> mapVisitList (fun x -> x.Range) this.VisitType

            (expr @ ty)
            |> tryVisitList

        | SynExpr.Tuple (_, exprs, _, _) ->
            exprs
            |> mapVisitList (fun x -> x.Range) this.VisitExpr
            |> tryVisitList

        | SynExpr.AnonRecd (_, copyInfoOpt, recdFields, _) ->
            let copyInfoOpt =
                match copyInfoOpt with
                | Some (expr, _) ->
                    [expr]
                    |> mapVisitList (fun x -> x.Range) this.VisitExpr
                | _ ->
                    []

            let ids, exprs = List.unzip recdFields

            let ids =
                ids
                |> mapVisitList (fun x -> x.idRange) this.VisitIdent

            let exprs =
                exprs
                |> mapVisitList (fun x -> x.Range) this.VisitExpr

            (copyInfoOpt @ ids @ exprs)
            |> tryVisitList

        | SynExpr.ArrayOrList (_, exprs, _) ->
            exprs
            |> mapVisitList (fun x -> x.Range) this.VisitExpr
            |> tryVisitList

        | SynExpr.Record (baseInfoOpt, copyInfoOpt, recdFields, _) ->
            let baseInfoOpt =
                match baseInfoOpt with
                | Some (ty, expr, _, _, _) ->
                    let ty =
                        [ty]
                        |> mapVisitList (fun x -> x.Range) this.VisitType

                    let expr =
                        [expr]
                        |> mapVisitList (fun x -> x.Range) this.VisitExpr

                    (ty @ expr)
                | _ ->
                    []

            let copyInfoOpt =
                match copyInfoOpt with
                | Some (expr, _) ->
                    [expr]
                    |> mapVisitList (fun x -> x.Range) this.VisitExpr
                | _ ->
                    []

            let recdFieldNames, exprOpts, _ = List.unzip3 recdFields

            let recdFieldNames =
                recdFieldNames
                |> List.map (fun (longDotId, _) -> longDotId)
                |> mapVisitList (fun x -> x.Range) this.VisitLongIdentWithDots

            let exprOpts =
                exprOpts
                |> List.choose id
                |> mapVisitList (fun x -> x.Range) this.VisitExpr

            (baseInfoOpt @ copyInfoOpt @ recdFieldNames @ exprOpts)
            |> tryVisitList

        | SynExpr.New (_, ty, expr, _) ->
            let ty =
                [ty]
                |> mapVisitList (fun x -> x.Range) this.VisitType

            let expr =
                [expr]
                |> mapVisitList (fun x -> x.Range) this.VisitExpr

            (ty @ expr)
            |> tryVisitList

        | SynExpr.ObjExpr (ty, argOpt, bindings, extraImpls, _, _) ->
            let ty =
                [ty]
                |> mapVisitList (fun x -> x.Range) this.VisitType

            let argOpt =
                match argOpt with
                | Some (expr, idOpt) ->
                    let expr =
                        [expr]
                        |> mapVisitList (fun x -> x.Range) this.VisitExpr

                    let idOpt =
                        match idOpt with
                        | Some id ->
                            [id]
                            |> mapVisitList (fun x -> x.idRange) this.VisitIdent
                        | _ ->
                            []

                    (expr @ idOpt)
                | _ ->
                    []

            let bindings =
                bindings
                |> mapVisitList (fun x -> x.RangeOfBindingAndRhs) this.VisitBinding

            let extraImpls =
                extraImpls
                |> mapVisitList (fun x -> x.Range) this.VisitInterfaceImpl

            (ty @ argOpt @ bindings @ extraImpls)
            |> tryVisitList

        | SynExpr.While (_, whileExpr, doExpr, _) ->
            [whileExpr;doExpr]
            |> mapVisitList (fun x -> x.Range) this.VisitExpr
            |> tryVisitList

        | SynExpr.For (_, id, expr, _, toBody, doBody, _) ->
            let id =
                [id]
                |> mapVisitList (fun x -> x.idRange) this.VisitIdent

            let exprs =
                [expr;toBody;doBody]
                |> mapVisitList (fun x -> x.Range) this.VisitExpr

            (id @ exprs)
            |> tryVisitList

        | SynExpr.ForEach (_, _, _, pat, enumExpr, bodyExpr, _) ->
            let pat =
                [pat]
                |> mapVisitList (fun x -> x.Range) this.VisitPat

            let exprs =
                [enumExpr;bodyExpr]
                |> mapVisitList (fun x -> x.Range) this.VisitExpr

            (pat @ exprs)
            |> tryVisitList

        | SynExpr.ArrayOrListOfSeqExpr (_, expr, _) ->
            expr
            |> tryVisit expr.Range this.VisitExpr

        | SynExpr.CompExpr (_, _, expr, _) ->
            expr
            |> tryVisit expr.Range this.VisitExpr

        | SynExpr.Lambda (_, _, argSimplePats, bodyExpr, _) ->
            let argSimplePats =
                [argSimplePats]
                |> mapVisitList (fun x -> x.Range) this.VisitSimplePats

            let bodyExpr =
                [bodyExpr]
                |> mapVisitList (fun x -> x.Range) this.VisitExpr

            (argSimplePats @ bodyExpr)
            |> tryVisitList

        | SynExpr.MatchLambda (_, _, clauses, _, _) ->
            clauses
            |> mapVisitList (fun x -> x.Range) this.VisitMatchClause
            |> tryVisitList

        | SynExpr.Match (_, expr, clauses, _) ->
            let expr =
                [expr]
                |> mapVisitList (fun x -> x.Range) this.VisitExpr

            let clauses =
                clauses
                |> mapVisitList (fun x -> x.Range) this.VisitMatchClause

            (expr @ clauses)
            |> tryVisitList

        | SynExpr.Do (expr, _) ->
            expr
            |> tryVisit expr.Range this.VisitExpr

        | SynExpr.Assert (expr, _) ->
            expr
            |> tryVisit expr.Range this.VisitExpr

        | SynExpr.App (_, _, funcExpr, argExpr, _) ->
            [funcExpr;argExpr]
            |> mapVisitList (fun x -> x.Range) this.VisitExpr
            |> tryVisitList

        | SynExpr.TypeApp (expr, _, tys, _, _, _, _) ->
            let expr =
                [expr]
                |> mapVisitList (fun x -> x.Range) this.VisitExpr

            let tys =
                tys
                |> mapVisitList (fun x -> x.Range) this.VisitType

            (expr @ tys)
            |> tryVisitList

        | SynExpr.LetOrUse (_, _, bindings, bodyExpr, _) ->
            let bindings =
                bindings
                |> mapVisitList (fun x -> x.RangeOfBindingAndRhs) this.VisitBinding

            let bodyExpr =
                [bodyExpr]
                |> mapVisitList (fun x -> x.Range) this.VisitExpr

            (bindings @ bodyExpr)
            |> tryVisitList

        | SynExpr.TryWith (tryExpr, _, clauses, _, _, _, _) ->
            let tryExpr =
                [tryExpr]
                |> mapVisitList (fun x -> x.Range) this.VisitExpr

            let clauses =
                clauses
                |> mapVisitList (fun x -> x.Range) this.VisitMatchClause

            (tryExpr @ clauses)
            |> tryVisitList

        | SynExpr.TryFinally (tryExpr, finallyExpr, _, _, _) ->
            [tryExpr;finallyExpr]
            |> mapVisitList (fun x -> x.Range) this.VisitExpr
            |> tryVisitList

        | SynExpr.Lazy (expr, _) ->
            expr
            |> tryVisit expr.Range this.VisitExpr

        | SynExpr.Sequential (_, _, expr1, expr2, _) ->
            [expr1;expr2]
            |> mapVisitList (fun x -> x.Range) this.VisitExpr
            |> tryVisitList

        | SynExpr.IfThenElse (ifExpr, thenExpr, elseExprOpt, _, _, _, _) ->
            let exprs =
                [ifExpr;thenExpr]
                |> mapVisitList (fun x -> x.Range) this.VisitExpr

            let elseExprOpt =
                match elseExprOpt with
                | Some elseExpr ->
                    [elseExpr]
                    |> mapVisitList (fun x -> x.Range) this.VisitExpr
                | _ ->
                    []

            (exprs @ elseExprOpt)
            |> tryVisitList

        | SynExpr.Ident id ->
            id
            |> tryVisit id.idRange this.VisitIdent

        | SynExpr.LongIdent (_, longDotId, simplePatAltIdInfoOpt, _) ->
            let longDotId =
                [longDotId]
                |> mapVisitList (fun x -> x.Range) this.VisitLongIdentWithDots

            let simplePatAltIdInfoOpt =
                match simplePatAltIdInfoOpt with
                | Some simplePatAltIdInfo ->
                    [simplePatAltIdInfo.contents]
                    |> mapVisitList (fun x -> x.Range) this.VisitSimplePatAlternativeIdInfo
                | _ ->
                    []

            (longDotId @ simplePatAltIdInfoOpt)
            |> tryVisitList

        | SynExpr.LongIdentSet (longDotId, expr, _) ->
            let longDotId =
                [longDotId]
                |> mapVisitList (fun x -> x.Range) this.VisitLongIdentWithDots

            let expr =
                [expr]
                |> mapVisitList (fun x -> x.Range) this.VisitExpr

            (longDotId @ expr)
            |> tryVisitList

        | SynExpr.DotGet (expr, _, longDotId, _) ->
            let expr =
                [expr]
                |> mapVisitList (fun x -> x.Range) this.VisitExpr

            let longDotId =
                [longDotId]
                |> mapVisitList (fun x -> x.Range) this.VisitLongIdentWithDots

            (expr @ longDotId)
            |> tryVisitList

        | SynExpr.DotSet (expr1, longDotId, expr2, _) ->
            let exprs =
                [expr1;expr2]
                |> mapVisitList (fun x -> x.Range) this.VisitExpr

            let longDotId =
                [longDotId]
                |> mapVisitList (fun x -> x.Range) this.VisitLongIdentWithDots

            (exprs @ longDotId)
            |> tryVisitList

        | SynExpr.Set (expr1, expr2, _) ->
            [expr1;expr2]
            |> mapVisitList (fun x -> x.Range) this.VisitExpr
            |> tryVisitList

        | SynExpr.DotIndexedGet (expr, args, _, _) ->
            let expr =
                [expr]
                |> mapVisitList (fun x -> x.Range) this.VisitExpr

            let args =
                args
                |> mapVisitList (fun x -> x.Range) this.VisitIndexerArg

            (expr @ args)
            |> tryVisitList

        | SynExpr.DotIndexedSet (objectExpr, args, valueExpr, _, _, _) ->
            let exprs =
                [objectExpr;valueExpr]
                |> mapVisitList (fun x -> x.Range) this.VisitExpr

            let args =
                args
                |> mapVisitList (fun x -> x.Range) this.VisitIndexerArg

            (exprs @ args)
            |> tryVisitList

        | SynExpr.NamedIndexedPropertySet (longDotId, expr1, expr2, _) ->
            let longDotId =
                [longDotId]
                |> mapVisitList (fun x -> x.Range) this.VisitLongIdentWithDots

            let exprs =
                [expr1;expr2]
                |> mapVisitList (fun x -> x.Range) this.VisitExpr

            (longDotId @ exprs)
            |> tryVisitList

        | SynExpr.DotNamedIndexedPropertySet (expr1, longDotId, expr2, expr3, _) ->
            let exprs =
                [expr1;expr;expr3]
                |> mapVisitList (fun x -> x.Range) this.VisitExpr

            let longDotId =
                [longDotId]
                |> mapVisitList (fun x -> x.Range) this.VisitLongIdentWithDots

            (exprs @ longDotId)
            |> tryVisitList

        | SynExpr.TypeTest (expr, ty, _) ->
            let expr =
                [expr]
                |> mapVisitList (fun x -> x.Range) this.VisitExpr

            let ty =
                [ty]
                |> mapVisitList (fun x -> x.Range) this.VisitType

            (expr @ ty)
            |> tryVisitList

        | SynExpr.Upcast (expr, ty, _) ->
            let expr =
                [expr]
                |> mapVisitList (fun x -> x.Range) this.VisitExpr

            let ty =
                [ty]
                |> mapVisitList (fun x -> x.Range) this.VisitType

            (expr @ ty)
            |> tryVisitList

        | SynExpr.Downcast (expr, ty, _) ->
            let expr =
                [expr]
                |> mapVisitList (fun x -> x.Range) this.VisitExpr

            let ty =
                [ty]
                |> mapVisitList (fun x -> x.Range) this.VisitType

            (expr @ ty)
            |> tryVisitList

        | SynExpr.InferredUpcast (expr, _) ->
            expr
            |> tryVisit expr.Range this.VisitExpr

        | SynExpr.InferredDowncast (expr, _) ->
            expr
            |> tryVisit expr.Range this.VisitExpr

        | SynExpr.Null _ ->
            None

        | SynExpr.AddressOf (_, expr, _, _) ->
            expr
            |> tryVisit expr.Range this.VisitExpr

        | SynExpr.TraitCall (typars, memberSig, expr, _) ->
            let typars =
                typars
                |> mapVisitList (fun x -> x.Range) this.VisitTypar

            let memberSig =
                [memberSig]
                |> mapVisitList (fun x -> x.Range) this.VisitMemberSig

            let expr =
                [expr]
                |> mapVisitList (fun x -> x.Range) this.VisitExpr

            (typars @ memberSig @ expr)
            |> tryVisitList

        | SynExpr.JoinIn (expr1, _, expr2, _) ->
            [expr1;expr2]
            |> mapVisitList (fun x -> x.Range) this.VisitExpr
            |> tryVisitList

        | SynExpr.ImplicitZero _ ->
            None

        | SynExpr.YieldOrReturn (_, expr, _) ->
            expr
            |> tryVisit expr.Range this.VisitExpr

        | SynExpr.YieldOrReturnFrom (_, expr, _) ->
            expr
            |> tryVisit expr.Range this.VisitExpr

        | SynExpr.LetOrUseBang (_, _, _, pat, expr1, expr2, _) ->
            let pat =
                [pat]
                |> mapVisitList (fun x -> x.Range) this.VisitPat

            let exprs =
                [expr1;expr2]
                |> mapVisitList (fun x -> x.Range) this.VisitExpr

            (pat @ exprs)
            |> tryVisitList

        | SynExpr.MatchBang (_, expr, clauses, _) ->
            let expr =
                [expr]
                |> mapVisitList (fun x -> x.Range) this.VisitExpr

            let clauses =
                clauses
                |> mapVisitList (fun x -> x.Range) this.VisitMatchClause

            (expr @ clauses)
            |> tryVisitList

        | SynExpr.DoBang (expr, _) ->
            expr
            |> tryVisit expr.Range this.VisitExpr

        | SynExpr.LibraryOnlyILAssembly (_, tys1, exprs, tys2, _) ->
            let tys =
                (tys1 @ tys2)
                |> mapVisitList (fun x -> x.Range) this.VisitType

            let exprs =
                exprs
                |> mapVisitList (fun x -> x.Range) this.VisitExpr

            (tys @ exprs)
            |> tryVisitList

        | SynExpr.LibraryOnlyStaticOptimization (staticOptConstraints, expr1, expr2, _) ->
            let staticOptConstraints =
                staticOptConstraints
                |> mapVisitList (fun x -> x.Range) this.VisitStaticOptimizationConstraint

            let exprs =
                [expr1;expr2]
                |> mapVisitList (fun x -> x.Range) this.VisitExpr

            (staticOptConstraints @ exprs)
            |> tryVisitList

        | SynExpr.LibraryOnlyUnionCaseFieldGet (expr, longId, _, _) ->
            let expr =
                [expr]
                |> mapVisitList (fun x -> x.Range) this.VisitExpr

            let longId =
                longId
                |> mapVisitList (fun x -> x.idRange) this.VisitIdent

            (expr @ longId)
            |> tryVisitList

        | SynExpr.LibraryOnlyUnionCaseFieldSet (expr1, longId, _, expr2, _) ->
            let exprs =
                [expr1;expr2]
                |> mapVisitList (fun x -> x.Range) this.VisitExpr

            let longId =
                longId
                |> mapVisitList (fun x -> x.idRange) this.VisitIdent

            (exprs @ longId)
            |> tryVisitList

        | SynExpr.ArbitraryAfterError _ ->
            None

        | SynExpr.FromParseError (expr, _) ->
            expr
            |> tryVisit expr.Range this.VisitExpr

        | SynExpr.DiscardAfterMissingQualificationAfterDot (expr, _) ->
            expr
            |> tryVisit expr.Range this.VisitExpr

        | SynExpr.Fixed (expr, _) ->
            expr
            |> tryVisit expr.Range this.VisitExpr
                
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
            |> tryVisit typar.Range this.VisitTypar

    abstract VisitIndexerArg: SynIndexerArg -> 'T option
    default this.VisitIndexerArg indexerArg =
        match indexerArg with
        | SynIndexerArg.Two (expr1, expr2) ->
            [expr1;expr2]
            |> mapVisitList (fun x -> x.Range) this.VisitExpr
            |> tryVisitList

        | SynIndexerArg.One expr ->
            expr
            |> tryVisit expr.Range this.VisitExpr

    abstract VisitSimplePatAlternativeIdInfo: SynSimplePatAlternativeIdInfo -> 'T option
    default this.VisitSimplePatAlternativeIdInfo simplePatAltIdInfo =
        match simplePatAltIdInfo with
        | Undecided id ->
            id
            |> tryVisit id.idRange this.VisitIdent

        | Decided id ->
            id
            |> tryVisit id.idRange this.VisitIdent

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
            |> tryVisit simpleRepr.Range this.VisitTypeDefnSimpleRepr

        | SynTypeDefnRepr.Exception exRepr ->
            exRepr
            |> tryVisit exRepr.Range this.VisitExceptionDefnRepr

    abstract VisitMemberDefn: SynMemberDefn -> 'T option
    default this.VisitMemberDefn memberDefn =
        match memberDefn with
        | SynMemberDefn.Open (longId, _) ->
            longId
            |> mapVisitList (fun x -> x.idRange) this.VisitIdent
            |> tryVisitList

        | SynMemberDefn.Member (binding, _) ->
            binding
            |> tryVisit binding.RangeOfBindingAndRhs this.VisitBinding

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
                    |> mapVisitList (fun x -> x.idRange) this.VisitIdent
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
                    |> mapVisitList (fun x -> x.idRange) this.VisitIdent
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
            |> tryVisit valSig.Range this.VisitValSig

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
                    |> mapVisitList (fun x -> x.idRange) this.VisitIdent
                | _ ->
                    []

            (ty @ idOpt)
            |> tryVisitList

        | SynMemberDefn.ValField (field, _) ->
            field
            |> tryVisit field.Range this.VisitField

        | SynMemberDefn.NestedType (typeDefn, _, _) ->
            typeDefn
            |> tryVisit typeDefn.Range this.VisitTypeDefn

        | SynMemberDefn.AutoProperty (attribs, _, id, tyOpt, _, _, _, _, expr, _, _) ->
            let attribs =
                attribs
                |> mapVisitList (fun x -> x.Range) this.VisitAttributeList

            let id =
                [id]
                |> mapVisitList (fun x -> x.idRange) this.VisitIdent

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
                |> mapVisitList (fun x -> x.idRange) this.VisitIdent
            | _ ->
                []

        (typeName @ argExpr @ targetOpt)
        |> tryVisitList