// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Moving what a declaration's check left by a number of lines, for a declaration whose text is unchanged but which
/// starts on another line (W1, DeclarationChecker.Shift).
module internal FSharp.Compiler.CodeAnalysis.Declarations.DeclarationShift

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.Reflection
open Microsoft.FSharp.Reflection

open Internal.Utilities.Library

open FSharp.Compiler
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.NameResolution
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Position
open FSharp.Compiler.Text.Range
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeOps

let private finishMisses = ref 0L

/// Counts a check the finish ran for a shifted result that reported at a range of the file outside the declaration.
let CountFinishMiss () =
    System.Threading.Interlocked.Increment(&finishMisses.contents) |> ignore

/// How many such checks have run in the process. For tests.
let FinishMisses () =
    System.Threading.Interlocked.Read(&finishMisses.contents)

/// Whether `name` carries a compiler-generated `@line` or `@line-count` suffix (`CompilerGlobalState.mkName`),
/// which changes in place if the declaration is freshly checked at another line.
let private hasLineNumberedGeneratedName (name: string) =
    let at = name.LastIndexOf '@'

    if at < 0 || at + 1 >= name.Length then
        false
    else
        let rec allDigits i last =
            i > last || (Char.IsDigit name[i] && allDigits (i + 1) last)

        let dash = name.IndexOf('-', at + 1)

        if dash >= 0 then
            dash > at + 1
            && dash + 1 < name.Length
            && allDigits (at + 1) (dash - 1)
            && allDigits (dash + 1) (name.Length - 1)
        else
            allDigits (at + 1) (name.Length - 1)

/// The parameter of the file's method `container` names `name`, as it is now: a named argument's resolution copies
/// the parameter's identifier, which a shift of the method's declaration moves.
let calleeName (container: ArgumentContainer option) (name: string) =
    match container with
    | Some(ArgumentContainer.Method(FSharp.Compiler.Infos.FSMeth(_, _, vref, _))) when vref.IsLocalRef ->
        match vref.Deref.ValReprInfo with
        | Some(ValReprInfo(_, args, _)) ->
            args
            |> List.concat
            |> List.tryPick (fun arg ->
                match arg.Name with
                | Some n when n.idText = name -> Some n
                | _ -> None)
        | None -> None
    | _ -> None

/// A name resolution with what it copied from another declaration's item as that item is now: a named argument's
/// parameter (see calleeName), and an active pattern case's tags, which the value's range gives.
let currentItems (r: CapturedNameResolution) =
    let withItem item =
        CapturedNameResolution(item, r.ItemWithInst.TyparInstantiation, r.ItemOccurrence, r.NameResolutionEnv, r.AccessorDomain, r.Range)

    match r.Item with
    | Item.ActivePatternCase(APElemRef(info, vref, idx, kind)) when vref.IsLocalRef ->
        match TryGetActivePatternInfo vref with
        | Some current when not (Range.equals current.Range info.Range) ->
            withItem (Item.ActivePatternCase(APElemRef(current, vref, idx, kind)))
        | _ -> r
    | Item.OtherName(Some id, ty, None, container, m) when Range.equals m id.idRange ->
        match calleeName container id.idText with
        | Some current when not (Range.equals current.idRange id.idRange) ->
            withItem (Item.OtherName(Some current, ty, None, container, current.idRange))
        | _ -> r
    | _ -> r

/// Moves every range of one declaration's syntax by `delta` lines. A range belongs to the declaration when it is in
/// its file and starts inside its old range: a scope that starts there and runs to the end of the module is moved
/// whole, as every line after the declaration moved with it.
///
/// What the declaration's check made (its values, entities, type parameters, local values) is moved in place, once
/// each. What it only refers to (another declaration's values, types and type parameters) is left: that declaration
/// is moved by its own shift, or did not move. A range of the declaration's own syntax, or of an item it made, that
/// is in the file but outside the declaration cannot be told apart from another declaration's, so the shift is
/// refused (Refused): the declaration is checked again. A few ranges no diagnostic reads are neither moved nor refused
/// (XML docs, measures in types, anonymous record field idents, names in captured environments).
///
/// Which items a declaration made is decided by identity, not by where they are now: the first shift of a check's
/// result records each item it moves under the check's key (`owner`, its ShareKey), and every later shift asks that
/// record first. Only an item no shift has moved yet is decided by position, against ranges as its check left them.
/// A position alone is not enough: an earlier declaration's item moved by its own shift in the same edit can land
/// inside the next declaration's old range.
///
/// The writes in place happen under the file's FileStart lock and check lock only. Later files' checks and lazily
/// built results read the same values, entities and type parameters on other threads without those locks, so such a
/// reader can see some of a declaration's items moved and others not yet (or a range written while it reads it); it
/// sees them all moved once the shift has returned. Nothing a check computes depends on these ranges.
[<Sealed>]
type Shifter(fileIndex: int, oldRange: range, scopeEnd: pos, delta: int, owner: obj) =
    static let owners = System.Runtime.CompilerServices.ConditionalWeakTable<obj, obj>()
    let moved = HashSet<obj>(HashIdentity.Reference)
    let exprs = Dictionary<Expr, Expr>(HashIdentity.Reference)
    let links = Dictionary<Expr ref, Expr ref>(HashIdentity.Reference)
    let undo = ResizeArray<unit -> unit>()
    let mutable refused: string option = None

    /// A shifter for what a shifted result's deferred checks report at the finish: what it needs is copied, so the
    /// closures keep no shifter, and no chain of shifters, alive.
    static member Create(fileIndex, oldRange, scopeEnd, delta) =
        Shifter(fileIndex, oldRange, scopeEnd, delta, obj ())

    member _.Parameters = fileIndex, oldRange, scopeEnd, delta

    /// Drops what the shift built up, once its result is made.
    member _.Release() =
        moved.Clear()
        exprs.Clear()
        links.Clear()
        undo.Clear()

    /// The first reason the shift cannot be made, if any.
    member _.Refused = refused

    /// Whether the declaration made `o`: the record of an earlier shift, or for an item no shift has moved yet,
    /// `inside` (its range, as its check left it, is inside the declaration).
    member _.Owns(o: obj, inside: bool) =
        match owners.TryGetValue o with
        | true, k -> obj.ReferenceEquals(k, owner)
        | _ -> inside

    /// Records that the declaration made `o`; false when another declaration's shift recorded it.
    member _.Claim(o: obj) =
        let k =
            owners.GetValue(o, System.Runtime.CompilerServices.ConditionalWeakTable<obj, obj>.CreateValueCallback(fun _ -> owner))

        obj.ReferenceEquals(k, owner)

    /// Records how to restore a value it is about to write in place.
    member _.Save(restore: unit -> unit) = undo.Add restore

    /// Restores everything it moved in place, latest first.
    member _.Undo() =
        for i in undo.Count - 1 .. -1 .. 0 do
            undo[i]()

        undo.Clear()

    member _.Refuse(reason: string) =
        if refused.IsNone then
            refused <- Some reason

    member _.Belongs(m: range) =
        m.FileIndex = fileIndex
        && posGeq m.Start oldRange.Start
        && posGeq oldRange.End m.Start

    /// The range `delta` lines lower, keeping its columns, height and flags.
    member _.Move(m: range) =
        let moved =
            mkFileIndexRange m.FileIndex (mkPos (m.StartLine + delta) m.StartColumn) (mkPos (m.EndLine + delta) m.EndColumn)

        let moved = if m.IsSynthetic then moved.MakeSynthetic() else moved
        moved.NoteSourceConstruct m.NotedSourceConstruct

    /// A range of the declaration's own syntax: moved, or the shift refused when it is elsewhere in the file.
    member x.Own(m: range) =
        if m.FileIndex <> fileIndex then
            m
        elif x.Belongs m then
            x.Move m
        else
            x.Refuse $"a range of its own outside it: %s{stringOfRange m}"
            m

    /// The scope of an environment or an open the declaration's check notified: it can start after the declaration,
    /// where the declarations after it start, and runs to the end of the enclosing module; it is moved when it starts
    /// in the declaration's scope. Only completions and unused-open analysis read these scopes, and the declarations
    /// after this one moved with it unless the same text change also moved them by another number of lines.
    member x.Scope(m: range) =
        if
            m.FileIndex = fileIndex
            && posGeq m.Start oldRange.Start
            && posGeq scopeEnd m.Start
        then
            x.Move m
        else
            x.Own m

    /// An identifier of the declaration's own syntax or items, as Own.
    member x.IdentOwn(id: Ident) =
        let m = x.Own id.idRange

        if Range.equals m id.idRange then
            id
        else
            Ident(id.idText, m)

    member x.OtherRange(other: (range * bool) option) =
        other |> Option.map (fun (m, isImpl) -> x.Own m, isImpl)

    /// Whether `o` has not been moved yet, marking it moved.
    member _.First(o: obj) = moved.Add o

    // ---- Attributes, type parameters and argument infos ----

    /// An attribute of an item the declaration made: in the declaration, or in another file (a signature's)
    member x.Attrib(attrib: Attrib) =
        let (Attrib(tcref, kind, args, props, appliedToGetterOrSetter, targets, m)) = attrib

        if not (x.Belongs m) then
            x.Own m |> ignore
            attrib
        else
            let attribExpr (AttribExpr(source, evaluated)) =
                AttribExpr(x.Expr source, x.Expr evaluated)

            Attrib(
                tcref,
                kind,
                List.map attribExpr args,
                props
                |> List.map (fun (AttribNamedArg(name, ty, isField, e)) -> AttribNamedArg(name, ty, isField, attribExpr e)),
                appliedToGetterOrSetter,
                targets,
                x.Move m
            )

    member x.Attribs(attribs: Attribs) = List.map x.Attrib attribs

    member x.WellKnown(attribs: WellKnownAttribs<Attrib, 'F>) : WellKnownAttribs<Attrib, 'F> =
        WellKnownAttribs(x.Attribs(attribs.AsList()), attribs.Flags)

    member x.Constraint(c: TyparConstraint) =
        match c with
        | TyparConstraint.CoercesTo(ty, m) -> TyparConstraint.CoercesTo(ty, x.Own m)
        | TyparConstraint.DefaultsTo(priority, ty, m) -> TyparConstraint.DefaultsTo(priority, ty, x.Own m)
        | TyparConstraint.SupportsNull m -> TyparConstraint.SupportsNull(x.Own m)
        | TyparConstraint.NotSupportsNull m -> TyparConstraint.NotSupportsNull(x.Own m)
        | TyparConstraint.MayResolveMember(info, m) -> TyparConstraint.MayResolveMember(info, x.Own m)
        | TyparConstraint.IsNonNullableStruct m -> TyparConstraint.IsNonNullableStruct(x.Own m)
        | TyparConstraint.IsReferenceType m -> TyparConstraint.IsReferenceType(x.Own m)
        | TyparConstraint.SimpleChoice(tys, m) -> TyparConstraint.SimpleChoice(tys, x.Own m)
        | TyparConstraint.RequiresDefaultConstructor m -> TyparConstraint.RequiresDefaultConstructor(x.Own m)
        | TyparConstraint.IsEnum(ty, m) -> TyparConstraint.IsEnum(ty, x.Own m)
        | TyparConstraint.SupportsComparison m -> TyparConstraint.SupportsComparison(x.Own m)
        | TyparConstraint.SupportsEquality m -> TyparConstraint.SupportsEquality(x.Own m)
        | TyparConstraint.IsDelegate(aty, bty, m) -> TyparConstraint.IsDelegate(aty, bty, x.Own m)
        | TyparConstraint.IsUnmanaged m -> TyparConstraint.IsUnmanaged(x.Own m)
        | TyparConstraint.AllowsRefStruct m -> TyparConstraint.AllowsRefStruct(x.Own m)

    /// A type parameter this declaration declares or inferred, moved in place. One another declaration made (its name
    /// is there, or its shift recorded it) is that declaration's to move, by its own shift or by checking it again,
    /// which checks again what depends on it: a shifted row does not keep results of a re-checked one (FileGraph).
    member x.Typar(tp: Typar) =
        if x.First tp && x.Owns(tp, x.Belongs tp.Range) && x.Claim tp then
            let id = tp.typar_id
            x.Save(fun () -> tp.typar_id <- id)
            tp.typar_id <- Ident(id.idText, x.Move id.idRange)

            match tp.typar_opt_data with
            | Some data ->
                let constraints, attribs = data.typar_constraints, data.typar_attribs

                x.Save(fun () ->
                    data.typar_constraints <- constraints
                    data.typar_attribs <- attribs)

                data.typar_constraints <- List.map x.Constraint constraints
                data.typar_attribs <- x.Attribs attribs
            | None -> ()

    member x.Typars(tps: Typars) = List.iter x.Typar tps

    /// The argument info of a value this declaration made.
    member x.ArgReprInfo(info: ArgReprInfo) = x.ArgReprInfoWith(info, true)

    /// An argument info a name resolution holds (a named argument's): another declaration's when that one's value
    /// holds it, which is decided as for type parameters.
    member x.ArgReprInfoWith(info: ArgReprInfo, ofOwnValue: bool) =
        let inside =
            ofOwnValue || (info.Name |> Option.exists (fun id -> x.Belongs id.idRange))

        if x.First info && x.Owns(info, inside) && x.Claim info then
            let name, otherRange, attribs = info.Name, info.OtherRange, info.Attribs

            x.Save(fun () ->
                info.Name <- name
                info.OtherRange <- otherRange
                info.Attribs <- attribs)

            info.Name <- info.Name |> Option.map x.IdentOwn
            info.OtherRange <- info.OtherRange |> Option.map x.Own
            info.Attribs <- x.WellKnown info.Attribs

    member x.ValReprInfo(ValReprInfo(typars, args, ret)) =
        for group in args do
            List.iter x.ArgReprInfo group

        x.ArgReprInfo ret

        ValReprInfo(
            typars
            |> List.map (fun (TyparReprInfo(id, kind)) -> TyparReprInfo(x.IdentOwn id, kind)),
            args,
            ret
        )

    // ---- Values and entities ----

    /// A value this declaration made: a module value or member it publishes, or a local value its expressions bind.
    member x.Val(v: Val) =
        if x.First v then
            // A local value is reachable from its own declaration only, so only the others need a record
            let shared = v.IsModuleBinding || v.IsMember

            if shared && not (x.Owns(v, true)) then
                x.Refuse $"its value %s{v.LogicalName} was moved by another declaration's shift"
            elif v.val_range.FileIndex = fileIndex && not (x.Belongs v.val_range) then
                x.Refuse $"its value %s{v.LogicalName} is outside it: %s{stringOfRange v.val_range}"
            elif shared && v.IsCompilerGenerated && hasLineNumberedGeneratedName v.LogicalName then
                x.Refuse $"its value %s{v.LogicalName} has a line-numbered compiler-generated name"
            else
                if shared then
                    x.Claim v |> ignore

                let m = v.val_range
                x.Save(fun () -> v.val_range <- m)
                v.val_range <- x.Own m

                match v.Type with
                | TType_forall(tps, _) -> x.Typars tps
                | _ -> ()

                match v.val_opt_data with
                | Some data ->
                    let saved =
                        { data with
                            val_compiled_name = data.val_compiled_name
                        }

                    x.Save(fun () ->
                        data.val_other_range <- saved.val_other_range
                        data.val_repr_info <- saved.val_repr_info
                        data.val_repr_info_for_display <- saved.val_repr_info_for_display
                        data.val_attribs <- saved.val_attribs
                        data.val_defn <- saved.val_defn)

                    data.val_other_range <- data.val_other_range |> Option.map (fun (m, isImpl) -> x.Own m, isImpl)
                    data.val_repr_info <- data.val_repr_info |> Option.map x.ValReprInfo
                    data.val_repr_info_for_display <- data.val_repr_info_for_display |> Option.map x.ValReprInfo
                    data.arg_repr_info_for_display |> Option.iter x.ArgReprInfo
                    data.val_attribs <- x.WellKnown data.val_attribs
                    data.val_defn <- data.val_defn |> Option.map x.Expr
                | None -> ()

    member x.RecdField(f: RecdField) =
        if x.First f then
            if f.IsCompilerGenerated && hasLineNumberedGeneratedName f.LogicalName then
                x.Refuse $"its field %s{f.LogicalName} has a line-numbered compiler-generated name"
            else
                let id, otherRange, pattribs, fattribs =
                    f.rfield_id, f.rfield_other_range, f.rfield_pattribs, f.rfield_fattribs

                x.Save(fun () ->
                    f.rfield_id <- id
                    f.rfield_other_range <- otherRange
                    f.rfield_pattribs <- pattribs
                    f.rfield_fattribs <- fattribs)

                f.rfield_id <- x.IdentOwn f.rfield_id
                f.rfield_other_range <- f.rfield_other_range |> Option.map (fun (m, isImpl) -> x.Own m, isImpl)
                f.rfield_pattribs <- x.Attribs f.rfield_pattribs
                f.rfield_fattribs <- x.Attribs f.rfield_fattribs

    /// An entity this declaration made, with what it holds: its type parameters, cases, fields, members, and for a
    /// module held whole (`withContents`) the values and entities in it. A module header holds only the module: its
    /// declarations hold the rest.
    member x.EntityWith(e: Entity, withContents: bool) =
        if x.First e then
            if not (x.Owns(e, true)) then
                x.Refuse $"its entity %s{e.LogicalName} was moved by another declaration's shift"
            elif e.entity_range.FileIndex = fileIndex && not (x.Belongs e.entity_range) then
                x.Refuse $"its entity %s{e.LogicalName} is outside it: %s{stringOfRange e.entity_range}"
            else
                x.Claim e |> ignore
                let m, attribs = e.entity_range, e.EntityAttribs

                x.Save(fun () ->
                    e.entity_range <- m
                    e.SetEntityAttribs attribs)

                e.entity_range <- x.Own m
                x.Typars e.Typars
                e.SetEntityAttribs(x.WellKnown attribs)

                match e.entity_opt_data with
                | Some data when x.First data ->
                    let otherRange = data.entity_other_range
                    x.Save(fun () -> data.entity_other_range <- otherRange)
                    data.entity_other_range <- otherRange |> Option.map (fun (m, isImpl) -> x.Own m, isImpl)

                    match data.entity_exn_info with
                    | TExnFresh fields -> Array.iter x.RecdField fields.FieldsByIndex
                    | _ -> ()
                | _ -> ()

                for case in e.UnionCasesArray do
                    if x.First case then
                        let id, otherRange, caseAttribs = case.Id, case.OtherRangeOpt, case.Attribs

                        x.Save(fun () ->
                            case.Id <- id
                            case.OtherRangeOpt <- otherRange
                            case.Attribs <- caseAttribs)

                        case.Id <- x.IdentOwn case.Id
                        case.OtherRangeOpt <- case.OtherRangeOpt |> Option.map (fun (m, isImpl) -> x.Own m, isImpl)
                        case.Attribs <- x.Attribs case.Attribs
                        Array.iter x.RecdField case.RecdFieldsArray

                Array.iter x.RecdField e.AllFieldsArray

                if not e.IsModuleOrNamespace then
                    let aug = e.TypeContents
                    let interfaces = aug.tcaug_interfaces
                    x.Save(fun () -> aug.tcaug_interfaces <- interfaces)
                    aug.tcaug_interfaces <- aug.tcaug_interfaces |> List.map (fun (ty, compGen, m) -> ty, compGen, x.Own m)

                    // An augmentation elsewhere in the file adds its members here too: its own shift moves them
                    match aug.tcaug_adhoc_list with
                    | null -> ()
                    | members ->
                        for _, vref in members do
                            x.ValIfOwn vref
                elif withContents then
                    x.ModuleType e.ModuleOrNamespaceType

    member x.Entity(e: Entity) = x.EntityWith(e, true)

    member x.ModuleType(mty: ModuleOrNamespaceType) =
        for v in mty.AllValsAndMembers do
            x.Val v

        for e in mty.AllEntities do
            x.Entity e

    // ---- Expressions ----

    member x.DebugPointAtBinding(sp: DebugPointAtBinding) =
        match sp with
        | DebugPointAtBinding.Yes m -> DebugPointAtBinding.Yes(x.Own m)
        | sp -> sp

    member x.Binding(TBind(v, e, sp)) =
        x.Val v
        TBind(v, x.Expr e, x.DebugPointAtBinding sp)

    member x.Op(op: TOp) =
        match op with
        | TOp.While(DebugPointAtWhile.Yes m, marker) -> TOp.While(DebugPointAtWhile.Yes(x.Own m), marker)
        | TOp.IntegerForLoop(spFor, spTo, style) ->
            let spFor =
                match spFor with
                | DebugPointAtFor.Yes m -> DebugPointAtFor.Yes(x.Own m)
                | sp -> sp

            let spTo =
                match spTo with
                | DebugPointAtInOrTo.Yes m -> DebugPointAtInOrTo.Yes(x.Own m)
                | sp -> sp

            TOp.IntegerForLoop(spFor, spTo, style)
        | TOp.TryWith(spTry, spWith) ->
            let spTry =
                match spTry with
                | DebugPointAtTry.Yes m -> DebugPointAtTry.Yes(x.Own m)
                | sp -> sp

            let spWith =
                match spWith with
                | DebugPointAtWith.Yes m -> DebugPointAtWith.Yes(x.Own m)
                | sp -> sp

            TOp.TryWith(spTry, spWith)
        | TOp.TryFinally(spTry, spFinally) ->
            let spTry =
                match spTry with
                | DebugPointAtTry.Yes m -> DebugPointAtTry.Yes(x.Own m)
                | sp -> sp

            let spFinally =
                match spFinally with
                | DebugPointAtFinally.Yes m -> DebugPointAtFinally.Yes(x.Own m)
                | sp -> sp

            TOp.TryFinally(spTry, spFinally)
        | op -> op

    member x.ObjExprMethod(TObjExprMethod(slotsig, attribs, typars, vss, body, m)) =
        x.Typars typars

        for vs in vss do
            List.iter x.Val vs

        TObjExprMethod(slotsig, x.Attribs attribs, typars, vss, x.Expr body, x.Own m)

    member x.DecisionTree(tree: DecisionTree) =
        match tree with
        | TDSwitch(input, cases, dflt, m) ->
            let case (TCase(test, tree)) =
                let test =
                    match test with
                    | DecisionTreeTest.ActivePatternCase(e, tys, kind, identity, idx, info) ->
                        DecisionTreeTest.ActivePatternCase(x.Expr e, tys, kind, identity, idx, info)
                    | DecisionTreeTest.Error m -> DecisionTreeTest.Error(x.Own m)
                    | test -> test

                TCase(test, x.DecisionTree tree)

            TDSwitch(x.Expr input, List.map case cases, Option.map x.DecisionTree dflt, x.Own m)
        | TDSuccess(es, n) -> TDSuccess(List.map x.Expr es, n)
        | TDBind(bind, rest) -> TDBind(x.Binding bind, x.DecisionTree rest)

    member x.Target(TTarget(vs, e, flags)) =
        List.iter x.Val vs
        TTarget(vs, x.Expr e, flags)

    /// The expression with every range moved: a new tree, sharing what it shared.
    member x.Expr(e: Expr) : Expr =
        match exprs.TryGetValue e with
        | true, shifted -> shifted
        | _ ->
            let shifted =
                match e with
                | Expr.Const(c, m, ty) -> Expr.Const(c, x.Own m, ty)
                | Expr.Val(vref, flags, m) -> Expr.Val(vref, flags, x.Own m)
                | Expr.Sequential(e1, e2, kind, m) -> Expr.Sequential(x.Expr e1, x.Expr e2, kind, x.Own m)
                | Expr.Lambda(uniq, ctorThis, baseVal, vs, body, m, ty) ->
                    Option.iter x.Val ctorThis
                    Option.iter x.Val baseVal
                    List.iter x.Val vs
                    Expr.Lambda(uniq, ctorThis, baseVal, vs, x.Expr body, x.Own m, ty)
                | Expr.TyLambda(uniq, tps, body, m, ty) ->
                    x.Typars tps
                    Expr.TyLambda(uniq, tps, x.Expr body, x.Own m, ty)
                | Expr.App(f, fty, tyargs, args, m) -> Expr.App(x.Expr f, fty, tyargs, List.map x.Expr args, x.Own m)
                | Expr.LetRec(binds, body, m, frees) -> Expr.LetRec(List.map x.Binding binds, x.Expr body, x.Own m, frees)
                | Expr.Let(bind, body, m, frees) -> Expr.Let(x.Binding bind, x.Expr body, x.Own m, frees)
                | Expr.Obj(uniq, ty, baseVal, ctorCall, overrides, iimpls, m) ->
                    Option.iter x.Val baseVal

                    Expr.Obj(
                        uniq,
                        ty,
                        baseVal,
                        x.Expr ctorCall,
                        List.map x.ObjExprMethod overrides,
                        iimpls
                        |> List.map (fun (ty, overrides) -> ty, List.map x.ObjExprMethod overrides),
                        x.Own m
                    )
                | Expr.Match(sp, mInput, tree, targets, m, ty) ->
                    Expr.Match(x.DebugPointAtBinding sp, x.Own mInput, x.DecisionTree tree, Array.map x.Target targets, x.Own m, ty)
                | Expr.StaticOptimization(conds, e2, e3, m) -> Expr.StaticOptimization(conds, x.Expr e2, x.Expr e3, x.Own m)
                | Expr.Op(op, tyargs, args, m) -> Expr.Op(x.Op op, tyargs, List.map x.Expr args, x.Own m)
                // The quotation's data is computed from the expression when it is compiled, not when it is checked
                | Expr.Quote(quoted, data, isFromQuery, m, ty) -> Expr.Quote(x.Expr quoted, data, isFromQuery, x.Own m, ty)
                | Expr.WitnessArg(info, m) -> Expr.WitnessArg(info, x.Own m)
                | Expr.TyChoose(tps, body, m) ->
                    x.Typars tps
                    Expr.TyChoose(tps, x.Expr body, x.Own m)
                | Expr.Link link ->
                    match links.TryGetValue link with
                    | true, shiftedLink -> Expr.Link shiftedLink
                    | _ ->
                        let shiftedLink = ref Unchecked.defaultof<Expr>
                        links[link] <- shiftedLink
                        shiftedLink.Value <- x.Expr link.Value
                        Expr.Link shiftedLink
                | Expr.DebugPoint(DebugPointAtLeafExpr.Yes(hidden, m), inner) ->
                    Expr.DebugPoint(DebugPointAtLeafExpr.Yes(hidden, x.Own m), x.Expr inner)

            exprs[e] <- shifted
            shifted

    member x.OpenDeclaration(o: OpenDeclaration) =
        { o with
            Range = o.Range |> Option.map x.Own
            AppliedScope = x.Scope o.AppliedScope
        }

    member x.Contents(contents: ModuleOrNamespaceContents) =
        match contents with
        | TMDefs defs -> TMDefs(List.map x.Contents defs)
        | TMDefOpens opens -> TMDefOpens(List.map x.OpenDeclaration opens)
        | TMDefLet(bind, m) -> TMDefLet(x.Binding bind, x.Own m)
        | TMDefDo(e, m) -> TMDefDo(x.Expr e, x.Own m)
        | TMDefRec(isRec, opens, tycons, bindings, m) ->
            List.iter x.Entity tycons

            let binding b =
                match b with
                | ModuleOrNamespaceBinding.Binding bind -> ModuleOrNamespaceBinding.Binding(x.Binding bind)
                | ModuleOrNamespaceBinding.Module(entity, def) ->
                    x.Entity entity
                    ModuleOrNamespaceBinding.Module(entity, x.Contents def)

            TMDefRec(isRec, List.map x.OpenDeclaration opens, tycons, List.map binding bindings, x.Own m)

    // ---- Name resolutions ----

    /// A value a name resolution refers to: moved when this declaration made it. A class's `let` value is bound
    /// nowhere in its typed contents (its uses read the class's field), so only its resolutions reach it.
    member x.ValIfOwn(vref: ValRef) =
        if vref.IsLocalRef && x.Owns(vref.Deref, x.Belongs vref.Deref.val_range) then
            x.Val vref.Deref

    member x.Item(item: Item) =
        match item with
        | Item.Value vref ->
            x.ValIfOwn vref
            item
        | Item.ActivePatternCase(APElemRef(_, vref, _, _)) ->
            x.ValIfOwn vref
            item
        | Item.ActivePatternResult(PrettyNaming.APInfo(total, tags, whole), ty, idx, m) ->
            let info =
                PrettyNaming.APInfo(total, tags |> List.map (fun (tag, tagRange) -> tag, x.Own tagRange), x.Own whole)

            Item.ActivePatternResult(info, ty, idx, x.Own m)
        | Item.AnonRecdField(info, tys, idx, m) -> Item.AnonRecdField(info, tys, idx, x.Own m)
        | Item.NewDef id -> Item.NewDef(x.IdentOwn id)
        | Item.Property(name, infos, Some m) -> Item.Property(name, infos, Some(x.Own m))
        | Item.ImplicitOp(id, sln) -> Item.ImplicitOp(x.IdentOwn id, sln)
        // A named argument of a method of the file names the method's parameter, which the method's declaration owns and
        // its shift moves: the resolution is given the parameter's current name when the results are built (calleeName)
        | Item.OtherName(Some id, _, None, container, m) when
            not (x.Belongs id.idRange)
            && Range.equals m id.idRange
            && (calleeName container id.idText).IsSome
            ->
            item
        | Item.OtherName(id, ty, argInfo, container, m) ->
            argInfo |> Option.iter (fun info -> x.ArgReprInfoWith(info, false))
            Item.OtherName(Option.map x.IdentOwn id, ty, argInfo, container, x.Own m)
        | Item.SetterArg(id, inner) -> Item.SetterArg(x.IdentOwn id, x.Item inner)
        | Item.TypeVar(_, tp) ->
            x.Typar tp
            item
        | item -> item

    member x.Resolution(r: CapturedNameResolution) =
        CapturedNameResolution(
            x.Item r.Item,
            r.ItemWithInst.TyparInstantiation,
            r.ItemOccurrence,
            r.NameResolutionEnv,
            r.AccessorDomain,
            x.Own r.Range
        )

    /// A sink holding what `sink` holds, at the moved ranges.
    member x.Sink(g, sink: TcResultsSinkImpl) =
        let shifted = TcResultsSinkImpl(g)
        let source = sink.GetResolutions()
        let target = shifted.GetResolutions()
        target.CapturedEnvs.AddRange(source.CapturedEnvs |> Seq.map (fun (m, nenv, ad) -> x.Scope m, nenv, ad))

        target.CapturedExpressionTypings.AddRange(
            source.CapturedExpressionTypings
            |> Seq.map (fun (ty, nenv, ad, m) -> ty, nenv, ad, x.Own m)
        )

        target.CapturedNameResolutions.AddRange(source.CapturedNameResolutions |> Seq.map x.Resolution)
        target.CapturedMethodGroupResolutions.AddRange(source.CapturedMethodGroupResolutions |> Seq.map x.Resolution)

        target.CapturedRelatedSymbolUses.AddRange(
            source.CapturedRelatedSymbolUses
            |> Seq.map (fun (m, item, kind) -> x.Own m, x.Item item, kind)
        )

        let notify = shifted :> ITypecheckResultsSink

        for m, args in sink.GetFormatSpecifierLocations() do
            notify.NotifyFormatSpecifierLocation(x.Own m, args)

        for o in sink.GetOpenDeclarations() do
            notify.NotifyOpenDeclaration(x.OpenDeclaration o)

        shifted

    // ---- Diagnostics ----

    member x.Diagnostic(d: PhasedDiagnostic) =
        { d with Exception = x.Exn d.Exception }

    member x.Exn(e: exn) : exn = ObjectShift.Exn(x, e)

/// Moves the ranges a diagnostic's exception holds, which its message and location are rendered from, by reflection
/// over its fields: ranges and identifiers, other exceptions, and the F# unions, immutable records and tuples of
/// compiler and core types that hold them. Typed-tree, syntax and display types are not entered: what a diagnostic
/// renders of them is moved in place or is another declaration's, and a copy of a type would hold copies of its
/// inference variables, which the finish's defaults never solve.
and [<Sealed>] ObjectShift private () =
    static let fieldsOf = ConcurrentDictionary<Type, FieldInfo[]>()

    static let instanceFields (t: Type) =
        fieldsOf.GetOrAdd(
            t,
            fun t ->
                [|
                    let mutable t: Type | null = t

                    while not (isNull t) && t <> typeof<exn> && t <> typeof<obj> do
                        let current = !!t

                        yield!
                            current.GetFields(
                                BindingFlags.Instance
                                ||| BindingFlags.Public
                                ||| BindingFlags.NonPublic
                                ||| BindingFlags.DeclaredOnly
                            )

                        t <- current.BaseType
                |]
        )

    static let excluded =
        [|
            "FSharp.Compiler.TypedTree"
            "FSharp.Compiler.Syntax"
            "FSharp.Compiler.TcGlobals"
            "FSharp.Compiler.Import"
        |]

    static let entered = ConcurrentDictionary<Type, bool>()

    // Types are in modules, so the full name is the module's path, then '+' and the type
    static let enters (t: Type) =
        entered.GetOrAdd(
            t,
            fun t ->
                match t.FullName with
                | null -> false
                | name ->
                    (t.Assembly = typeof<range>.Assembly || t.Assembly = typeof<option<int>>.Assembly)
                    && not (
                        excluded
                        |> Array.exists (fun prefix -> name.StartsWith(prefix, StringComparison.Ordinal))
                    )
                    && not (name.Contains "+DisplayEnv")
                    && not (
                        FSharpType.IsRecord(t, true)
                        && FSharpType.GetRecordFields(t, true) |> Array.exists _.CanWrite
                    )
        )

    static let cloneMethod =
        !!(typeof<obj>.GetMethod("MemberwiseClone", BindingFlags.Instance ||| BindingFlags.NonPublic))

    static let same (values: objnull[]) (shifted: objnull[]) =
        Array.forall2 (fun a b -> obj.ReferenceEquals(a, b)) values shifted

    /// The value with its ranges moved; the same object when nothing in it moved.
    static member Value(s: Shifter, value: objnull, depth: int) : objnull =
        match value with
        | null -> null
        | :? range as m ->
            let moved = s.Own m
            if Range.equals moved m then value else box moved
        | :? Ident as id ->
            let moved = s.Own id.idRange

            if Range.equals moved id.idRange then
                value
            else
                box (Ident(id.idText, moved))
        | :? exn as e -> box (ObjectShift.ExnAt(s, e, depth))
        | _ when depth > 16 -> value
        | value ->
            let t = value.GetType()

            if not (enters t) then
                value
            elif t.IsGenericType && t.GetGenericTypeDefinition() = typedefof<list<_>> then
                let items = [| for item in (value :?> System.Collections.IEnumerable) -> item |]
                let shifted = items |> Array.map (fun item -> ObjectShift.Value(s, item, depth + 1))

                if same items shifted then
                    value
                else
                    let cons = !!(t.GetMethod "Cons")
                    let empty = (!!(t.GetProperty "Empty")).GetValue null
                    Array.foldBack (fun item acc -> cons.Invoke(null, [| item; acc |])) shifted empty
            elif FSharpType.IsTuple t then
                let fields = FSharpValue.GetTupleFields value
                let shifted = fields |> Array.map (fun f -> ObjectShift.Value(s, f, depth + 1))

                if same fields shifted then
                    value
                else
                    FSharpValue.MakeTuple(shifted, t)
            elif FSharpType.IsUnion(t, true) then
                let case, fields = FSharpValue.GetUnionFields(value, t, true)
                let shifted = fields |> Array.map (fun f -> ObjectShift.Value(s, f, depth + 1))

                if same fields shifted then
                    value
                else
                    FSharpValue.MakeUnion(case, shifted, true)
            elif FSharpType.IsRecord(t, true) then
                let fields = FSharpValue.GetRecordFields(value, true)
                let shifted = fields |> Array.map (fun f -> ObjectShift.Value(s, f, depth + 1))

                if same fields shifted then
                    value
                else
                    FSharpValue.MakeRecord(t, shifted, true)
            else
                value

    static member ExnAt(s: Shifter, e: exn, depth: int) : exn =
        let fields = instanceFields (e.GetType())
        let values = fields |> Array.map (fun f -> f.GetValue e)
        let shifted = values |> Array.map (fun v -> ObjectShift.Value(s, v, depth + 1))

        if same values shifted then
            e
        else
            match cloneMethod.Invoke(e, [||]) with
            | :? exn as copy ->
                for f, v in Array.zip fields shifted do
                    f.SetValue(copy, v)

                copy
            | _ -> e

    static member Exn(s: Shifter, e: exn) = ObjectShift.ExnAt(s, e, 0)

/// Passes each diagnostic on with its ranges moved.
[<Sealed>]
type ShiftingLogger(next: DiagnosticsLogger, shift: PhasedDiagnostic -> PhasedDiagnostic) =
    inherit DiagnosticsLogger("ShiftingLogger")
    override _.DiagnosticSink diagnostic = next.DiagnosticSink(shift diagnostic)
    override _.ErrorCount = next.ErrorCount
    override _.CheckForRealErrorsIgnoringWarnings = next.CheckForRealErrorsIgnoringWarnings
