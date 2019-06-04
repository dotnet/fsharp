namespace FSharp.Compiler.Compilation

open System.IO
open System.Threading
open System.Collections.Immutable
open System.Runtime.CompilerServices
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Host
open FSharp.Compiler.Compilation.Utilities
open FSharp.Compiler.AbstractIL.Internal.Library
open FSharp.Compiler
open FSharp.Compiler.Text
open FSharp.Compiler.CompileOps
open FSharp.Compiler.Ast
open FSharp.Compiler.Range

[<AutoOpen>]
module ParsedInputVisitorHelpers =

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

    type SynSimplePat with

        member this.Range =
            match this with
            | SynSimplePat.Id (range=m) -> m
            | SynSimplePat.Typed (range=m) -> m
            | SynSimplePat.Attrib (range=m) -> m

    type SynMeasure with

        member this.Range defaultM =
            match this with
            | SynMeasure.Named (range=m) -> m
            | SynMeasure.Product (range=m) -> m
            | SynMeasure.Seq (range=m) -> m
            | SynMeasure.Divide (range=m) -> m
            | SynMeasure.Power (range=m) -> m
            | SynMeasure.One -> defaultM
            | SynMeasure.Anon (range=m) -> m
            | SynMeasure.Var (range=m) -> m

    type SynRationalConst with

        member this.Range defaultM =
            match this with
            | SynRationalConst.Integer _ -> defaultM
            | SynRationalConst.Rational (range=m) -> m
            | SynRationalConst.Negate rationalConst -> rationalConst.Range defaultM

[<AbstractClass>]
type ParseInputVisitor<'T> (p: pos) =

    static let isZeroRange (r: range) =
        posEq r.Start r.End

    let tryVisit m visit item =
        if rangeContainsPos m p && not (isZeroRange m) then
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
            if rangeContainsPos r p && not (isZeroRange r) then
                visit ()
            else
                None
        )
        |> Option.orElseWith (fun () ->
            xs
            |> List.tryPick (fun (getRange, visit) ->
                let r = getRange ()
                if posGt p r.Start && not (isZeroRange r) then
                    visit ()
                else
                    None
            )
        )

    let mapVisitList getRange visit xs =
        xs
        |> List.map (fun x -> ((fun () -> getRange x), fun () -> visit x))

    member this.Traverse parsedInput =
        match parsedInput with
        | ParsedInput.ImplFile (ParsedImplFileInput (_, _, _, _, _, modules, _)) -> 
            modules
            |> mapVisitList (fun x -> x.Range) this.VisitModuleOrNamespace
            |> tryVisitList
        | ParsedInput.SigFile _ -> None

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
                |> mapVisitList (fun x -> x.Range) this.VisitAttribute

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
            |> mapVisitList (fun x -> x.Range) this.VisitAttribute
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
        | LongIdentWithDots (longId, dotms) ->
            let longId =
                longId
                |> mapVisitList (fun x -> x.idRange) this.VisitIdent

            let dotms =
                dotms
                |> mapVisitList id this.VisitDot

            (longId @ dotms)
            |> tryVisitList

    abstract VisitIdent: Ident -> 'T option
    default this.VisitIdent _id =
        None

    abstract VisitDot: range -> 'T option
    default this.VisitDot _m =
        None

    abstract VisitComponentInfo: SynComponentInfo -> 'T option
    default this.VisitComponentInfo info =
        match info with
        | ComponentInfo(attribs, typarDecls, constraints, longId, _, _, _, m) ->
            let attribs =
                attribs
                |> mapVisitList (fun x -> x.Range) this.VisitAttribute

            let typarDecls =
                typarDecls
                |> mapVisitList (fun _ -> m) (fun x -> this.VisitTyparDecl (x, m))

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
        | SynTypeDefnSigRepr.ObjectModel (typeDefnKind, memberSigs, m) ->
            let typeDefnKind =
                [typeDefnKind]
                |> mapVisitList (fun _ -> m) (fun x -> this.VisitTypeDefnKind (x, m))

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
                |> mapVisitList (fun x -> x.Range) this.VisitAttribute

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
                |> mapVisitList (fun x -> x.Range) this.VisitAttribute

            let ident =
                [ident]
                |> mapVisitList (fun x -> x.idRange) this.VisitIdent

            let unionCaseTy =
                [unionCaseTy]
                |> mapVisitList (fun _ -> m) (fun x -> this.VisitUnionCaseType (x, m))

            (attribs @ ident @ unionCaseTy)
            |> tryVisitList

    abstract VisitUnionCaseType: SynUnionCaseType * range -> 'T option
    default this.VisitUnionCaseType (unionCaseTy, m) =
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
                |> mapVisitList (fun _ -> m) (fun x -> this.VisitValInfo (x, m))

            (ty @ valInfo)
            |> tryVisitList

    abstract VisitValInfo: SynValInfo * range -> 'T option
    default this.VisitValInfo (valInfo, m) =
        match valInfo with
        | SynValInfo (curriedArgInfos, returnInfo) ->
            let curriedArgInfos =
                curriedArgInfos
                |> List.reduce (@)
                |> mapVisitList (fun _ -> m) this.VisitArgInfo

            let returnInfo =
                [returnInfo]
                |> mapVisitList (fun _ -> m) this.VisitArgInfo

            (curriedArgInfos @ returnInfo)
            |> tryVisitList

    abstract VisitArgInfo: SynArgInfo -> 'T option
    default this.VisitArgInfo argInfo =
        match argInfo with
        | SynArgInfo (attribs, _, idOpt) ->
            let attribs =
                attribs
                |> mapVisitList (fun x -> x.Range) this.VisitAttribute

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
                |> mapVisitList (fun _ -> m) (fun x -> this.VisitTypeDefnKind (x, m))

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
                    pats
                    |> mapVisitList (fun x -> x.Range) this.VisitSimplePat
                | _ ->
                    []

            (typeDefnKind @ tys @ valSigs @ fields @ pats)
            |> tryVisitList

        | SynTypeDefnSimpleRepr.LibraryOnlyILAssembly (ilType, _) ->
            this.VisitILType ilType

        | SynTypeDefnSimpleRepr.TypeAbbrev (_, ty, _) ->
            ty
            |> tryVisit ty.Range this.VisitType

        | SynTypeDefnSimpleRepr.None _ ->
            None

        | SynTypeDefnSimpleRepr.Exception exDefnRepr ->
            exDefnRepr
            |> tryVisit exDefnRepr.Range this.VisitExceptionDefnRepr

    abstract VisitILType: AbstractIL.IL.ILType -> 'T option
    default this.VisitILType _ =
        None

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
                    |> mapVisitList (fun _ -> m) (fun x -> this.VisitSimplePatAlternativeIdInfo (x, m))
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
                |> mapVisitList (fun x -> x.Range) this.VisitAttribute

            (simplePat @ attribs)
            |> tryVisitList

    abstract VisitSimplePatAlternativeIdInfo: SynSimplePatAlternativeIdInfo * range -> 'T option
    default this.VisitSimplePatAlternativeIdInfo (simplePatInfo, _) =
        match simplePatInfo with
        | Undecided id ->
            id
            |> tryVisit id.idRange this.VisitIdent
        | Decided id ->
            id
            |> tryVisit id.idRange this.VisitIdent

    abstract VisitEnumCase: SynEnumCase -> 'T option
    default this.VisitEnumCase enumCase =
        match enumCase with
        | EnumCase (attribs, id, sconst, _, m) ->
            let attribs =
                attribs
                |> mapVisitList (fun x -> x.Range) this.VisitAttribute

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
                |> mapVisitList (fun x -> x.Range range0) this.VisitMeasure

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
            |> mapVisitList (fun x -> x.Range range0) this.VisitMeasure
            |> tryVisitList

        | SynMeasure.Seq (measures, _) ->
            measures
            |> mapVisitList (fun x -> x.Range range0) this.VisitMeasure
            |> tryVisitList

        | SynMeasure.Divide (measure1, measure2, _) ->
            [measure1; measure2]
            |> mapVisitList (fun x -> x.Range range0) this.VisitMeasure
            |> tryVisitList

        | SynMeasure.Power (measure, rationalCost, _) ->
            let measure =
                [measure]
                |> mapVisitList (fun x -> x.Range range0) this.VisitMeasure

            let rationalCost =
                [rationalCost]
                |> mapVisitList (fun x -> x.Range range0) this.VisitRationalConst

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
            this.VisitRationalConst rationalConst

    abstract VisitTypeDefnKind: SynTypeDefnKind * range -> 'T option
    default this.VisitTypeDefnKind (typeDefnKind, m) =
        match typeDefnKind with
        | TyconDelegate (ty, valInfo) ->
            let ty =
                [ty]
                |> mapVisitList (fun x -> x.Range) this.VisitType

            let valInfo =
                [valInfo]
                |> mapVisitList (fun _ -> m) (fun x -> this.VisitValInfo (x, m))

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
                |> mapVisitList (fun x -> x.Range) this.VisitAttribute

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
                |> mapVisitList (fun x -> x.Range) this.VisitAttribute

            let id =
                [id]
                |> mapVisitList (fun x -> x.idRange) this.VisitIdent

            let valTyparDecls =
                [valTyparDecls]
                |> mapVisitList (fun _ -> m) (fun x -> this.VisitValTyparDecls (x, m))

            let ty =
                [ty]
                |> mapVisitList (fun x -> x.Range) this.VisitType

            let valInfo =
                [valInfo]
                |> mapVisitList (fun _ -> m) (fun x -> this.VisitValInfo (x, m))

            let expr =
                match exprOpt with
                | Some expr ->
                    [expr]
                    |> mapVisitList (fun x -> x.Range) this.VisitExpr
                | _ ->
                    []

            (attribs @ id @ valTyparDecls @ ty @ valInfo @ expr)
            |> tryVisitList

    abstract VisitValTyparDecls: SynValTyparDecls * range -> 'T option
    default this.VisitValTyparDecls (valTyparDecls, m) =
        match valTyparDecls with
        | SynValTyparDecls (typarDecls, _, constraints) ->
            let typarDecls =
                typarDecls
                |> mapVisitList (fun _ -> m) (fun x -> this.VisitTyparDecl (x, m))

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

     //   | SynType.

        | _ ->
            None



    abstract VisitTypar: SynTypar -> 'T option

    abstract VisitTyparDecl: SynTyparDecl * range -> 'T option

    abstract VisitBinding: SynBinding -> 'T option

    abstract VisitExpr: SynExpr -> 'T option

    abstract VisitTypeDefn: SynTypeDefn -> 'T option

    abstract VisitExceptionDefn: SynExceptionDefn -> 'T option

    abstract VisitParsedHashDirective: ParsedHashDirective -> 'T option

    abstract VisitAttribute: SynAttribute -> 'T option
    default this.VisitAttribute attrib = None

[<RequireQualifiedAccess; NoEquality; NoComparison>]
type SyntaxNodeKind =
    | Expr of SynExpr
    | ModuleDecl of SynModuleDecl
    | Binding of SynBinding
    | ComponentInfo of SynComponentInfo
    | HashDirective of Range.range
    | ImplicitInherit of SynType * SynExpr * Range.range
    | InheritSynMemberDefn of SynComponentInfo * SynTypeDefnKind * SynType * SynMemberDefns * Range.range
    | InterfaceSynMemberDefnType of SynType
    | LetOrUse of SynBinding list * Range.range
    | MatchClause of SynMatchClause
    | ModuleOrNamespace of SynModuleOrNamespace
    | Pat of SynPat
    | RecordField of SynExpr option * LongIdentWithDots option
    | SimplePats of SynSimplePat list
    | Type of SynType
    | TypeAbbrev of SynType * Range.range

[<Sealed>]
type SyntaxNode internal (kind, parentOpt: SyntaxNode option) =
    
    member __.Kind = kind

    member __.Parent = parentOpt