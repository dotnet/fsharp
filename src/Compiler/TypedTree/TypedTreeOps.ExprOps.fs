// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

/// TypedTreeOps.ExprOps: address-of operations, expression folding, intrinsic call wrappers, and higher-level expression helpers.
namespace FSharp.Compiler.TypedTreeOps

open System
open System.CodeDom.Compiler
open System.Collections.Generic
open System.Collections.Immutable
open Internal.Utilities
open Internal.Utilities.Collections
open Internal.Utilities.Library
open Internal.Utilities.Library.Extras
open Internal.Utilities.Rational
open FSharp.Compiler.IO
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.CompilerGlobalState
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Features
open FSharp.Compiler.Syntax
open FSharp.Compiler.Syntax.PrettyNaming
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.TcGlobals
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.Text.Layout
open FSharp.Compiler.Text.LayoutRender
open FSharp.Compiler.Text.TaggedText
open FSharp.Compiler.Xml
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
#if !NO_TYPEPROVIDERS
open FSharp.Compiler.TypeProviders
#endif

[<AutoOpen>]
module internal AddressOps =

    //-------------------------------------------------------------------------
    // mkExprAddrOfExprAux
    //-------------------------------------------------------------------------

    type Mutates =
        | AddressOfOp
        | DefinitelyMutates
        | PossiblyMutates
        | NeverMutates

    exception DefensiveCopyWarning of string * range

    let isRecdOrStructTyconRefAssumedImmutable (g: TcGlobals) (tcref: TyconRef) =
        (tcref.CanDeref && not (isRecdOrUnionOrStructTyconRefDefinitelyMutable tcref))
        || tyconRefEq g tcref g.decimal_tcr
        || tyconRefEq g tcref g.date_tcr

    let isTyconRefReadOnly g (m: range) (tcref: TyconRef) =
        ignore m

        tcref.CanDeref
        && if
               match tcref.TryIsReadOnly with
               | ValueSome res -> res
               | _ ->
                   let res =
                       TyconRefHasWellKnownAttribute g WellKnownILAttributes.IsReadOnlyAttribute tcref

                   tcref.SetIsReadOnly res
                   res
           then
               true
           else
               tcref.IsEnumTycon

    let isTyconRefAssumedReadOnly g (tcref: TyconRef) =
        tcref.CanDeref
        && match tcref.TryIsAssumedReadOnly with
           | ValueSome res -> res
           | _ ->
               let res = isRecdOrStructTyconRefAssumedImmutable g tcref
               tcref.SetIsAssumedReadOnly res
               res

    let isRecdOrStructTyconRefReadOnlyAux g m isInref (tcref: TyconRef) =
        if isInref && tcref.IsILStructOrEnumTycon then
            isTyconRefReadOnly g m tcref
        else
            isTyconRefReadOnly g m tcref || isTyconRefAssumedReadOnly g tcref

    let isRecdOrStructTyconRefReadOnly g m tcref =
        isRecdOrStructTyconRefReadOnlyAux g m false tcref

    let isRecdOrStructTyReadOnlyAux (g: TcGlobals) m isInref ty =
        match tryTcrefOfAppTy g ty with
        | ValueNone -> false
        | ValueSome tcref -> isRecdOrStructTyconRefReadOnlyAux g m isInref tcref

    let isRecdOrStructTyReadOnly g m ty =
        isRecdOrStructTyReadOnlyAux g m false ty

    let CanTakeAddressOf g m isInref ty mut =
        match mut with
        | NeverMutates -> true
        | PossiblyMutates -> isRecdOrStructTyReadOnlyAux g m isInref ty
        | DefinitelyMutates -> false
        | AddressOfOp -> true // you can take the address but you might get a (readonly) inref<T> as a result

    // We can take the address of values of struct type even if the value is immutable
    // under certain conditions
    //   - all instances of the type are known to be immutable; OR
    //   - the operation is known not to mutate
    //
    // Note this may be taking the address of a closure field, i.e. a copy
    // of the original struct, e.g. for
    //    let f () =
    //        let g1 = A.G(1)
    //        (fun () -> g1.x1)
    //
    // Note: isRecdOrStructTyReadOnly implies PossiblyMutates or NeverMutates
    //
    // We only do this for true local or closure fields because we can't take addresses of immutable static
    // fields across assemblies.
    let CanTakeAddressOfImmutableVal (g: TcGlobals) m (vref: ValRef) mut =
        // We can take the address of values of struct type if the operation doesn't mutate
        // and the value is a true local or closure field.
        not vref.IsMutable
        && not vref.IsMemberOrModuleBinding
        &&
        // Note: We can't add this:
        //    || valRefInThisAssembly g.compilingFSharpCore vref
        // This is because we don't actually guarantee to generate static backing fields for all values like these, e.g. simple constants "let x = 1".
        // We always generate a static property but there is no field to take an address of
        CanTakeAddressOf g m false vref.Type mut

    let MustTakeAddressOfVal (g: TcGlobals) (vref: ValRef) =
        vref.IsMutable
        &&
        // We can only take the address of mutable values in the same assembly
        valRefInThisAssembly g.compilingFSharpCore vref

    let MustTakeAddressOfByrefGet (g: TcGlobals) (vref: ValRef) =
        isByrefTy g vref.Type && not (isInByrefTy g vref.Type)

    let CanTakeAddressOfByrefGet (g: TcGlobals) (vref: ValRef) mut =
        isInByrefTy g vref.Type
        && CanTakeAddressOf g vref.Range true (destByrefTy g vref.Type) mut

    let MustTakeAddressOfRecdField (rfref: RecdField) =
        // Static mutable fields must be private, hence we don't have to take their address
        not rfref.IsStatic && rfref.IsMutable

    let MustTakeAddressOfRecdFieldRef (rfref: RecdFieldRef) =
        MustTakeAddressOfRecdField rfref.RecdField

    let CanTakeAddressOfRecdFieldRef (g: TcGlobals) m (rfref: RecdFieldRef) tinst mut =
        // We only do this if the field is defined in this assembly because we can't take addresses across assemblies for immutable fields
        entityRefInThisAssembly g.compilingFSharpCore rfref.TyconRef
        && not rfref.RecdField.IsMutable
        && CanTakeAddressOf g m false (actualTyOfRecdFieldRef rfref tinst) mut

    let CanTakeAddressOfUnionFieldRef (g: TcGlobals) m (uref: UnionCaseRef) cidx tinst mut =
        // We only do this if the field is defined in this assembly because we can't take addresses across assemblies for immutable fields
        entityRefInThisAssembly g.compilingFSharpCore uref.TyconRef
        && let rfref = uref.FieldByIndex cidx in

           not rfref.IsMutable
           && CanTakeAddressOf g m false (actualTyOfUnionFieldRef uref cidx tinst) mut

    let mkDerefAddrExpr mAddrGet expr mExpr exprTy =
        let v, _ = mkCompGenLocal mAddrGet "byrefReturn" exprTy
        mkCompGenLet mExpr v expr (mkAddrGet mAddrGet (mkLocalValRef v))

    /// Make the address-of expression and return a wrapper that adds any allocated locals at an appropriate scope.
    /// Also return a flag that indicates if the resulting pointer is a not a pointer where writing is allowed and will
    /// have intended effect (i.e. is a readonly pointer and/or a defensive copy).
    let rec mkExprAddrOfExprAux g mustTakeAddress useReadonlyForGenericArrayAddress mut expr addrExprVal m =
        if mustTakeAddress then
            let isNativePtr =
                match addrExprVal with
                | Some vf -> valRefEq g vf g.addrof2_vref
                | _ -> false

            // If we are taking the native address using "&&" to get a nativeptr, disallow if it's readonly.
            let checkTakeNativeAddress readonly =
                if isNativePtr && readonly then
                    error (Error(FSComp.SR.tastValueMustBeMutable (), m))

            match expr with
            // LVALUE of "*x" where "x" is byref is just the byref itself
            | Expr.Op(TOp.LValueOp(LByrefGet, vref), _, [], m) when MustTakeAddressOfByrefGet g vref || CanTakeAddressOfByrefGet g vref mut ->
                let readonly = not (MustTakeAddressOfByrefGet g vref)
                let writeonly = isOutByrefTy g vref.Type
                None, exprForValRef m vref, readonly, writeonly

            // LVALUE of "x" where "x" is mutable local, mutable intra-assembly module/static binding, or operation doesn't mutate.
            // Note: we can always take the address of mutable intra-assembly values
            | Expr.Val(vref, _, m) when MustTakeAddressOfVal g vref || CanTakeAddressOfImmutableVal g m vref mut ->
                let readonly = not (MustTakeAddressOfVal g vref)
                let writeonly = false
                checkTakeNativeAddress readonly
                None, mkValAddr m readonly vref, readonly, writeonly

            // LVALUE of "e.f" where "f" is an instance F# field or record field.
            | Expr.Op(TOp.ValFieldGet rfref, tinst, [ objExpr ], m) when
                MustTakeAddressOfRecdFieldRef rfref
                || CanTakeAddressOfRecdFieldRef g m rfref tinst mut
                ->
                let objTy = tyOfExpr g objExpr
                let takeAddrOfObjExpr = isStructTy g objTy // It seems this will always be false - the address will already have been taken

                let wrap, expra, readonly, writeonly =
                    mkExprAddrOfExprAux g takeAddrOfObjExpr false mut objExpr None m

                let readonly =
                    readonly || isInByrefTy g objTy || not (MustTakeAddressOfRecdFieldRef rfref)

                let writeonly = writeonly || isOutByrefTy g objTy
                wrap, mkRecdFieldGetAddrViaExprAddr (readonly, expra, rfref, tinst, m), readonly, writeonly

            // LVALUE of "f" where "f" is a static F# field.
            | Expr.Op(TOp.ValFieldGet rfref, tinst, [], m) when
                MustTakeAddressOfRecdFieldRef rfref
                || CanTakeAddressOfRecdFieldRef g m rfref tinst mut
                ->
                let readonly = not (MustTakeAddressOfRecdFieldRef rfref)
                let writeonly = false
                None, mkStaticRecdFieldGetAddr (readonly, rfref, tinst, m), readonly, writeonly

            // LVALUE of "e.f" where "f" is an F# union field.
            | Expr.Op(TOp.UnionCaseFieldGet(uref, cidx), tinst, [ objExpr ], m) when
                MustTakeAddressOfRecdField(uref.FieldByIndex cidx)
                || CanTakeAddressOfUnionFieldRef g m uref cidx tinst mut
                ->
                let objTy = tyOfExpr g objExpr
                let takeAddrOfObjExpr = isStructTy g objTy // It seems this will always be false - the address will already have been taken

                let wrap, expra, readonly, writeonly =
                    mkExprAddrOfExprAux g takeAddrOfObjExpr false mut objExpr None m

                let readonly =
                    readonly
                    || isInByrefTy g objTy
                    || not (MustTakeAddressOfRecdField(uref.FieldByIndex cidx))

                let writeonly = writeonly || isOutByrefTy g objTy
                wrap, mkUnionCaseFieldGetAddrProvenViaExprAddr (readonly, expra, uref, tinst, cidx, m), readonly, writeonly

            // LVALUE of "f" where "f" is a .NET static field.
            | Expr.Op(TOp.ILAsm([ I_ldsfld(_vol, fspec) ], [ ty2 ]), tinst, [], m) ->
                let readonly = false // we never consider taking the address of a .NET static field to give an inref pointer
                let writeonly = false
                None, Expr.Op(TOp.ILAsm([ I_ldsflda fspec ], [ mkByrefTy g ty2 ]), tinst, [], m), readonly, writeonly

            // LVALUE of "e.f" where "f" is a .NET instance field.
            | Expr.Op(TOp.ILAsm([ I_ldfld(_align, _vol, fspec) ], [ ty2 ]), tinst, [ objExpr ], m) ->
                let objTy = tyOfExpr g objExpr
                let takeAddrOfObjExpr = isStructTy g objTy // It seems this will always be false - the address will already have been taken
                // we never consider taking the address of an .NET instance field to give an inref pointer, unless the object pointer is an inref pointer
                let wrap, expra, readonly, writeonly =
                    mkExprAddrOfExprAux g takeAddrOfObjExpr false mut objExpr None m

                let readonly = readonly || isInByrefTy g objTy
                let writeonly = writeonly || isOutByrefTy g objTy
                wrap, Expr.Op(TOp.ILAsm([ I_ldflda fspec ], [ mkByrefTyWithFlag g readonly ty2 ]), tinst, [ expra ], m), readonly, writeonly

            // LVALUE of "e.[n]" where e is an array of structs
            | Expr.App(Expr.Val(vf, _, _), _, [ elemTy ], [ aexpr; nexpr ], _) when (valRefEq g vf g.array_get_vref) ->

                let readonly = false // array address is never forced to be readonly
                let writeonly = false
                let shape = ILArrayShape.SingleDimensional

                let ilInstrReadOnlyAnnotation =
                    if isTyparTy g elemTy && useReadonlyForGenericArrayAddress then
                        ReadonlyAddress
                    else
                        NormalAddress

                None,
                mkArrayElemAddress g (readonly, ilInstrReadOnlyAnnotation, isNativePtr, shape, elemTy, [ aexpr; nexpr ], m),
                readonly,
                writeonly

            // LVALUE of "e.[n1, n2]", "e.[n1, n2, n3]", "e.[n1, n2, n3, n4]" where e is an array of structs
            | Expr.App(Expr.Val(vref, _, _), _, [ elemTy ], aexpr :: args, _) when
                (valRefEq g vref g.array2D_get_vref
                 || valRefEq g vref g.array3D_get_vref
                 || valRefEq g vref g.array4D_get_vref)
                ->

                let readonly = false // array address is never forced to be readonly
                let writeonly = false
                let shape = ILArrayShape.FromRank args.Length

                let ilInstrReadOnlyAnnotation =
                    if isTyparTy g elemTy && useReadonlyForGenericArrayAddress then
                        ReadonlyAddress
                    else
                        NormalAddress

                None,
                mkArrayElemAddress g (readonly, ilInstrReadOnlyAnnotation, isNativePtr, shape, elemTy, (aexpr :: args), m),
                readonly,
                writeonly

            // LVALUE: "&meth(args)" where meth has a byref or inref return. Includes "&span.[idx]".
            | Expr.Let(TBind(vref, e, _), Expr.Op(TOp.LValueOp(LByrefGet, vref2), _, _, _), _, _) when
                (valRefEq g (mkLocalValRef vref) vref2)
                && (MustTakeAddressOfByrefGet g vref2 || CanTakeAddressOfByrefGet g vref2 mut)
                ->
                let ty = tyOfExpr g e
                let readonly = isInByrefTy g ty
                let writeonly = isOutByrefTy g ty
                None, e, readonly, writeonly

            // Give a nice error message for address-of-byref
            | Expr.Val(vref, _, m) when isByrefTy g vref.Type -> error (Error(FSComp.SR.tastUnexpectedByRef (), m))

            // Give a nice error message for DefinitelyMutates of address-of on mutable values in other assemblies
            | Expr.Val(vref, _, m) when (mut = DefinitelyMutates || mut = AddressOfOp) && vref.IsMutable ->
                error (Error(FSComp.SR.tastInvalidAddressOfMutableAcrossAssemblyBoundary (), m))

            // Give a nice error message for AddressOfOp on immutable values
            | Expr.Val _ when mut = AddressOfOp -> error (Error(FSComp.SR.tastValueMustBeLocal (), m))

            // Give a nice error message for mutating a value we can't take the address of
            | Expr.Val _ when mut = DefinitelyMutates -> error (Error(FSComp.SR.tastValueMustBeMutable (), m))

            | _ ->
                let ty = tyOfExpr g expr

                if isStructTy g ty then
                    match mut with
                    | NeverMutates
                    | AddressOfOp -> ()
                    | DefinitelyMutates ->
                        // Give a nice error message for mutating something we can't take the address of
                        errorR (Error(FSComp.SR.tastInvalidMutationOfConstant (), m))
                    | PossiblyMutates ->
                        // Warn on defensive copy of something we can't take the address of
                        warning (DefensiveCopyWarning(FSComp.SR.tastValueHasBeenCopied (), m))

                match mut with
                | NeverMutates
                | DefinitelyMutates
                | PossiblyMutates -> ()
                | AddressOfOp ->
                    // we get an inref
                    errorR (Error(FSComp.SR.tastCantTakeAddressOfExpression (), m))

                // Take a defensive copy
                let tmp, _ =
                    match mut with
                    | NeverMutates -> mkCompGenLocal m WellKnownNames.CopyOfStruct ty
                    | _ -> mkMutableCompGenLocal m WellKnownNames.CopyOfStruct ty

                // This local is special in that it ignore byref scoping rules.
                tmp.SetIgnoresByrefScope()

                let readonly = true
                let writeonly = false
                Some(tmp, expr), (mkValAddr m readonly (mkLocalValRef tmp)), readonly, writeonly
        else
            None, expr, false, false

    let mkExprAddrOfExpr g mustTakeAddress useReadonlyForGenericArrayAddress mut e addrExprVal m =
        let optBind, addre, readonly, writeonly =
            mkExprAddrOfExprAux g mustTakeAddress useReadonlyForGenericArrayAddress mut e addrExprVal m

        match optBind with
        | None -> id, addre, readonly, writeonly
        | Some(tmp, rval) -> (fun x -> mkCompGenLet m tmp rval x), addre, readonly, writeonly

    let mkTupleFieldGet g (tupInfo, e, tinst, i, m) =
        let wrap, eR, _readonly, _writeonly =
            mkExprAddrOfExpr g (evalTupInfoIsStruct tupInfo) false NeverMutates e None m

        wrap (mkTupleFieldGetViaExprAddr (tupInfo, eR, tinst, i, m))

    let mkAnonRecdFieldGet g (anonInfo: AnonRecdTypeInfo, e, tinst, i, m) =
        let wrap, eR, _readonly, _writeonly =
            mkExprAddrOfExpr g (evalAnonInfoIsStruct anonInfo) false NeverMutates e None m

        wrap (mkAnonRecdFieldGetViaExprAddr (anonInfo, eR, tinst, i, m))

    let mkRecdFieldGet g (e, fref: RecdFieldRef, tinst, m) =
        assert (not (isByrefTy g (tyOfExpr g e)))

        let wrap, eR, _readonly, _writeonly =
            mkExprAddrOfExpr g fref.Tycon.IsStructOrEnumTycon false NeverMutates e None m

        wrap (mkRecdFieldGetViaExprAddr (eR, fref, tinst, m))

    let mkUnionCaseFieldGetUnproven g (e, cref: UnionCaseRef, tinst, j, m) =
        assert (not (isByrefTy g (tyOfExpr g e)))

        let wrap, eR, _readonly, _writeonly =
            mkExprAddrOfExpr g cref.Tycon.IsStructOrEnumTycon false NeverMutates e None m

        wrap (mkUnionCaseFieldGetUnprovenViaExprAddr (eR, cref, tinst, j, m))

[<AutoOpen>]
module internal ExprFolding =

    //---------------------------------------------------------------------------
    // Compute fixups for letrec's.
    //
    // Generate an assignment expression that will fixup the recursion
    // amongst the vals on the r.h.s. of a letrec. The returned expressions
    // include disorderly constructs such as expressions/statements
    // to set closure environments and non-mutable fields. These are only ever
    // generated by the backend code-generator when processing a "letrec"
    // construct.
    //
    // [self] is the top level value that is being fixed
    // [exprToFix] is the r.h.s. expression
    // [rvs] is the set of recursive vals being bound.
    // [acc] accumulates the expression right-to-left.
    //
    // Traversal of the r.h.s. term must happen back-to-front to get the
    // uniq's for the lambdas correct in the very rare case where the same lambda
    // somehow appears twice on the right.
    //---------------------------------------------------------------------------

    let rec IterateRecursiveFixups g (selfv: Val option) rvs (access: Expr, set) exprToFix =
        let exprToFix = stripExpr exprToFix

        match exprToFix with
        | Expr.Const _ -> ()
        | Expr.Op(TOp.Tuple tupInfo, argTys, args, m) when not (evalTupInfoIsStruct tupInfo) ->
            args
            |> List.iteri (fun n ->
                IterateRecursiveFixups
                    g
                    None
                    rvs
                    (mkTupleFieldGet g (tupInfo, access, argTys, n, m),
                     (fun e ->
                         // NICE: it would be better to do this check in the type checker
                         errorR (Error(FSComp.SR.tastRecursiveValuesMayNotBeInConstructionOfTuple (), m))
                         e)))

        | Expr.Op(TOp.UnionCase c, tinst, args, m) ->
            args
            |> List.iteri (fun n ->
                IterateRecursiveFixups
                    g
                    None
                    rvs
                    (mkUnionCaseFieldGetUnprovenViaExprAddr (access, c, tinst, n, m),
                     (fun e ->
                         // NICE: it would be better to do this check in the type checker
                         let tcref = c.TyconRef

                         if
                             not (c.FieldByIndex n).IsMutable
                             && not (entityRefInThisAssembly g.compilingFSharpCore tcref)
                         then
                             errorR (Error(FSComp.SR.tastRecursiveValuesMayNotAppearInConstructionOfType (tcref.LogicalName), m))

                         mkUnionCaseFieldSet (access, c, tinst, n, e, m))))

        | Expr.Op(TOp.Recd(_, tcref), tinst, args, m) ->
            (tcref.TrueInstanceFieldsAsRefList, args)
            ||> List.iter2 (fun fref arg ->
                let fspec = fref.RecdField

                IterateRecursiveFixups
                    g
                    None
                    rvs
                    (mkRecdFieldGetViaExprAddr (access, fref, tinst, m),
                     (fun e ->
                         // NICE: it would be better to do this check in the type checker
                         if not fspec.IsMutable && not (entityRefInThisAssembly g.compilingFSharpCore tcref) then
                             errorR (
                                 Error(
                                     FSComp.SR.tastRecursiveValuesMayNotBeAssignedToNonMutableField (
                                         fspec.rfield_id.idText,
                                         tcref.LogicalName
                                     ),
                                     m
                                 )
                             )

                         mkRecdFieldSetViaExprAddr (access, fref, tinst, e, m)))
                    arg)
        | Expr.Val _
        | Expr.Lambda _
        | Expr.Obj _
        | Expr.TyChoose _
        | Expr.TyLambda _ -> rvs selfv access set exprToFix
        | _ -> ()

    //--------------------------------------------------------------------------
    // computations on constraints
    //--------------------------------------------------------------------------

    let JoinTyparStaticReq r1 r2 =
        match r1, r2 with
        | TyparStaticReq.None, r
        | r, TyparStaticReq.None -> r
        | TyparStaticReq.HeadType, r
        | r, TyparStaticReq.HeadType -> r

    //-------------------------------------------------------------------------
    // ExprFolder - fold steps
    //-------------------------------------------------------------------------

    type ExprFolder<'State> =
        {
            exprIntercept (* recurseF *) :
                ('State -> Expr -> 'State) -> (* noInterceptF *) ('State -> Expr -> 'State) -> 'State -> Expr -> 'State
            // the bool is 'bound in dtree'
            valBindingSiteIntercept: 'State -> bool * Val -> 'State
            // these values are always bound to these expressions. bool indicates 'recursively'
            nonRecBindingsIntercept: 'State -> Binding -> 'State
            recBindingsIntercept: 'State -> Bindings -> 'State
            dtreeIntercept: 'State -> DecisionTree -> 'State
            targetIntercept (* recurseF *) : ('State -> Expr -> 'State) -> 'State -> DecisionTreeTarget -> 'State option
            tmethodIntercept (* recurseF *) : ('State -> Expr -> 'State) -> 'State -> ObjExprMethod -> 'State option
        }

    let ExprFolder0 =
        {
            exprIntercept = (fun _recurseF noInterceptF z x -> noInterceptF z x)
            valBindingSiteIntercept = (fun z _b -> z)
            nonRecBindingsIntercept = (fun z _bs -> z)
            recBindingsIntercept = (fun z _bs -> z)
            dtreeIntercept = (fun z _dt -> z)
            targetIntercept = (fun _exprF _z _x -> None)
            tmethodIntercept = (fun _exprF _z _x -> None)
        }

    //-------------------------------------------------------------------------
    // FoldExpr
    //-------------------------------------------------------------------------

    /// Adapted from usage info folding.
    /// Collecting from exprs at moment.
    /// To collect ids etc some additional folding needed, over formals etc.
    type ExprFolders<'State>(folders: ExprFolder<'State>) =
        let mutable exprFClosure = Unchecked.defaultof<'State -> Expr -> 'State> // prevent reallocation of closure
        let mutable exprNoInterceptFClosure = Unchecked.defaultof<'State -> Expr -> 'State> // prevent reallocation of closure
        let stackGuard = StackGuard("FoldExprStackGuardDepth")

        let rec exprsF z xs = List.fold exprFClosure z xs

        and exprF (z: 'State) (x: Expr) =
            stackGuard.Guard
            <| fun () -> folders.exprIntercept exprFClosure exprNoInterceptFClosure z x

        and exprNoInterceptF (z: 'State) (x: Expr) =
            match x with

            | Expr.Const _ -> z

            | Expr.Val _ -> z

            | LinearOpExpr(_op, _tyargs, argsHead, argLast, _m) ->
                let z = exprsF z argsHead
                // tailcall
                exprF z argLast

            | Expr.Op(_c, _tyargs, args, _) -> exprsF z args

            | Expr.Sequential(x0, x1, _dir, _) ->
                let z = exprF z x0
                exprF z x1

            | Expr.Lambda(_lambdaId, _ctorThisValOpt, _baseValOpt, _argvs, body, _m, _rty) -> exprF z body

            | Expr.TyLambda(_lambdaId, _tps, body, _m, _rty) -> exprF z body

            | Expr.TyChoose(_, body, _) -> exprF z body

            | Expr.App(f, _fty, _tys, argTys, _) ->
                let z = exprF z f
                exprsF z argTys

            | Expr.LetRec(binds, body, _, _) ->
                let z = valBindsF false z binds
                exprF z body

            | Expr.Let(bind, body, _, _) ->
                let z = valBindF false z bind
                exprF z body

            | Expr.Link rX -> exprF z rX.Value

            | Expr.DebugPoint(_, innerExpr) -> exprF z innerExpr

            | Expr.Match(_spBind, _exprm, dtree, targets, _m, _ty) ->
                let z = dtreeF z dtree
                let z = Array.fold targetF z targets[0 .. targets.Length - 2]
                // tailcall
                targetF z targets[targets.Length - 1]

            | Expr.Quote(e, dataCell, _, _, _) ->
                let z = exprF z e

                match dataCell.Value with
                | None -> z
                | Some((_typeDefs, _argTypes, argExprs, _), _) -> exprsF z argExprs

            | Expr.Obj(_n, _typ, _basev, basecall, overrides, iimpls, _m) ->
                let z = exprF z basecall
                let z = List.fold tmethodF z overrides
                List.fold (foldOn snd (List.fold tmethodF)) z iimpls

            | Expr.StaticOptimization(_tcs, csx, x, _) -> exprsF z [ csx; x ]

            | Expr.WitnessArg(_witnessInfo, _m) -> z

        and valBindF dtree z bind =
            let z = folders.nonRecBindingsIntercept z bind
            bindF dtree z bind

        and valBindsF dtree z binds =
            let z = folders.recBindingsIntercept z binds
            List.fold (bindF dtree) z binds

        and bindF dtree z (bind: Binding) =
            let z = folders.valBindingSiteIntercept z (dtree, bind.Var)
            exprF z bind.Expr

        and dtreeF z dtree =
            let z = folders.dtreeIntercept z dtree

            match dtree with
            | TDBind(bind, rest) ->
                let z = valBindF true z bind
                dtreeF z rest
            | TDSuccess(args, _) -> exprsF z args
            | TDSwitch(test, dcases, dflt, _) ->
                let z = exprF z test
                let z = List.fold dcaseF z dcases
                let z = Option.fold dtreeF z dflt
                z

        and dcaseF z =
            function
            | TCase(_, dtree) -> dtreeF z dtree (* not collecting from test *)

        and targetF z x =
            match folders.targetIntercept exprFClosure z x with
            | Some z -> z // intercepted
            | None -> // structurally recurse
                let (TTarget(_, body, _)) = x
                exprF z body

        and tmethodF z x =
            match folders.tmethodIntercept exprFClosure z x with
            | Some z -> z // intercepted
            | None -> // structurally recurse
                let (TObjExprMethod(_, _, _, _, e, _)) = x
                exprF z e

        and mdefF z x =
            match x with
            | TMDefRec(_, _, _, mbinds, _) ->
                // REVIEW: also iterate the abstract slot vspecs hidden in the _vslots field in the tycons
                let z = List.fold mbindF z mbinds
                z
            | TMDefLet(bind, _) -> valBindF false z bind
            | TMDefOpens _ -> z
            | TMDefDo(e, _) -> exprF z e
            | TMDefs defs -> List.fold mdefF z defs

        and mbindF z x =
            match x with
            | ModuleOrNamespaceBinding.Binding b -> valBindF false z b
            | ModuleOrNamespaceBinding.Module(_, def) -> mdefF z def

        let implF z (x: CheckedImplFile) = mdefF z x.Contents

        do exprFClosure <- exprF // allocate one instance of this closure
        do exprNoInterceptFClosure <- exprNoInterceptF // allocate one instance of this closure

        member x.FoldExpr = exprF

        member x.FoldImplFile = implF

    let FoldExpr folders state expr =
        ExprFolders(folders).FoldExpr state expr

    let FoldImplFile folders state implFile =
        ExprFolders(folders).FoldImplFile state implFile

#if DEBUG
    //-------------------------------------------------------------------------
    // ExprStats
    //-------------------------------------------------------------------------

    let ExprStats x =
        let mutable count = 0

        let folders =
            { ExprFolder0 with
                exprIntercept =
                    (fun _ noInterceptF z x ->
                        (count <- count + 1
                         noInterceptF z x))
            }

        let () = FoldExpr folders () x
        string count + " TExpr nodes"
#endif

[<AutoOpen>]
module internal Makers =

    //-------------------------------------------------------------------------
    // Make expressions
    //-------------------------------------------------------------------------

    let mkString (g: TcGlobals) m n =
        Expr.Const(Const.String n, m, g.string_ty)

    let mkByte (g: TcGlobals) m b = Expr.Const(Const.Byte b, m, g.byte_ty)

    let mkUInt16 (g: TcGlobals) m b =
        Expr.Const(Const.UInt16 b, m, g.uint16_ty)

    let mkUnit (g: TcGlobals) m = Expr.Const(Const.Unit, m, g.unit_ty)

    let mkInt32 (g: TcGlobals) m n =
        Expr.Const(Const.Int32 n, m, g.int32_ty)

    let mkInt g m n = mkInt32 g m n

    let mkZero g m = mkInt g m 0

    let mkOne g m = mkInt g m 1

    let mkTwo g m = mkInt g m 2

    let mkMinusOne g m = mkInt g m -1

    let mkTypedZero g m ty =
        if typeEquivAux EraseMeasures g ty g.int32_ty then
            Expr.Const(Const.Int32 0, m, ty)
        elif typeEquivAux EraseMeasures g ty g.int64_ty then
            Expr.Const(Const.Int64 0L, m, ty)
        elif typeEquivAux EraseMeasures g ty g.uint64_ty then
            Expr.Const(Const.UInt64 0UL, m, ty)
        elif typeEquivAux EraseMeasures g ty g.uint32_ty then
            Expr.Const(Const.UInt32 0u, m, ty)
        elif typeEquivAux EraseMeasures g ty g.nativeint_ty then
            Expr.Const(Const.IntPtr 0L, m, ty)
        elif typeEquivAux EraseMeasures g ty g.unativeint_ty then
            Expr.Const(Const.UIntPtr 0UL, m, ty)
        elif typeEquivAux EraseMeasures g ty g.int16_ty then
            Expr.Const(Const.Int16 0s, m, ty)
        elif typeEquivAux EraseMeasures g ty g.uint16_ty then
            Expr.Const(Const.UInt16 0us, m, ty)
        elif typeEquivAux EraseMeasures g ty g.sbyte_ty then
            Expr.Const(Const.SByte 0y, m, ty)
        elif typeEquivAux EraseMeasures g ty g.byte_ty then
            Expr.Const(Const.Byte 0uy, m, ty)
        elif typeEquivAux EraseMeasures g ty g.char_ty then
            Expr.Const(Const.Char '\000', m, ty)
        elif typeEquivAux EraseMeasures g ty g.float32_ty then
            Expr.Const(Const.Single 0.0f, m, ty)
        elif typeEquivAux EraseMeasures g ty g.float_ty then
            Expr.Const(Const.Double 0.0, m, ty)
        elif typeEquivAux EraseMeasures g ty g.decimal_ty then
            Expr.Const(Const.Decimal 0m, m, ty)
        else
            error (InternalError($"Unrecognized numeric type '{ty}'.", m))

    let mkTypedOne g m ty =
        if typeEquivAux EraseMeasures g ty g.int32_ty then
            Expr.Const(Const.Int32 1, m, ty)
        elif typeEquivAux EraseMeasures g ty g.int64_ty then
            Expr.Const(Const.Int64 1L, m, ty)
        elif typeEquivAux EraseMeasures g ty g.uint64_ty then
            Expr.Const(Const.UInt64 1UL, m, ty)
        elif typeEquivAux EraseMeasures g ty g.uint32_ty then
            Expr.Const(Const.UInt32 1u, m, ty)
        elif typeEquivAux EraseMeasures g ty g.nativeint_ty then
            Expr.Const(Const.IntPtr 1L, m, ty)
        elif typeEquivAux EraseMeasures g ty g.unativeint_ty then
            Expr.Const(Const.UIntPtr 1UL, m, ty)
        elif typeEquivAux EraseMeasures g ty g.int16_ty then
            Expr.Const(Const.Int16 1s, m, ty)
        elif typeEquivAux EraseMeasures g ty g.uint16_ty then
            Expr.Const(Const.UInt16 1us, m, ty)
        elif typeEquivAux EraseMeasures g ty g.sbyte_ty then
            Expr.Const(Const.SByte 1y, m, ty)
        elif typeEquivAux EraseMeasures g ty g.byte_ty then
            Expr.Const(Const.Byte 1uy, m, ty)
        elif typeEquivAux EraseMeasures g ty g.char_ty then
            Expr.Const(Const.Char '\001', m, ty)
        elif typeEquivAux EraseMeasures g ty g.float32_ty then
            Expr.Const(Const.Single 1.0f, m, ty)
        elif typeEquivAux EraseMeasures g ty g.float_ty then
            Expr.Const(Const.Double 1.0, m, ty)
        elif typeEquivAux EraseMeasures g ty g.decimal_ty then
            Expr.Const(Const.Decimal 1m, m, ty)
        else
            error (InternalError($"Unrecognized numeric type '{ty}'.", m))

    let mkRefCellContentsRef (g: TcGlobals) =
        mkRecdFieldRef g.refcell_tcr_canon "contents"

    let mkSequential m e1 e2 = Expr.Sequential(e1, e2, NormalSeq, m)

    let mkCompGenSequential m stmt expr = mkSequential m stmt expr

    let mkThenDoSequential m expr stmt =
        Expr.Sequential(expr, stmt, ThenDoSeq, m)

    let mkCompGenThenDoSequential m expr stmt = mkThenDoSequential m expr stmt

    let rec mkSequentials g m es =
        match es with
        | [ e ] -> e
        | e :: es -> mkSequential m e (mkSequentials g m es)
        | [] -> mkUnit g m

    let mkGetArg0 m ty =
        mkAsmExpr ([ mkLdarg0 ], [], [], [ ty ], m)

    //-------------------------------------------------------------------------
    // Tuples...
    //-------------------------------------------------------------------------

    let mkAnyTupled g m tupInfo es tys =
        match es with
        | [] -> mkUnit g m
        | [ e ] -> e
        | _ -> Expr.Op(TOp.Tuple tupInfo, tys, es, m)

    let mkRefTupled g m es tys = mkAnyTupled g m tupInfoRef es tys

    let mkRefTupledNoTypes g m args =
        mkRefTupled g m args (List.map (tyOfExpr g) args)

    let mkRefTupledVars g m vs =
        mkRefTupled g m (List.map (exprForVal m) vs) (typesOfVals vs)

    //--------------------------------------------------------------------------
    // Permute expressions
    //--------------------------------------------------------------------------

    let inversePerm (sigma: int array) =
        let n = sigma.Length
        let invSigma = Array.create n -1

        for i = 0 to n - 1 do
            let sigma_i = sigma[i]
            // assert( invSigma.[sigma_i] = -1 )
            invSigma[sigma_i] <- i

        invSigma

    let permute (sigma: int[]) (data: 'T[]) =
        let n = sigma.Length
        let invSigma = inversePerm sigma
        Array.init n (fun i -> data[invSigma[i]])

    let rec existsR a b pred =
        if a <= b then pred a || existsR (a + 1) b pred else false

    // Given a permutation for record fields, work out the highest entry that we must lift out
    // of a record initialization. Lift out xi if xi goes to position that will be preceded by an expr with an effect
    // that originally followed xi. If one entry gets lifted then everything before it also gets lifted.
    let liftAllBefore sigma =
        let invSigma = inversePerm sigma

        let lifted =
            [
                for i in 0 .. sigma.Length - 1 do
                    let iR = sigma[i]

                    if existsR 0 (iR - 1) (fun jR -> invSigma[jR] > i) then
                        yield i
            ]

        if lifted.IsEmpty then 0 else List.max lifted + 1

    ///  Put record field assignments in order.
    //
    let permuteExprList (sigma: int[]) (exprs: Expr list) (ty: TType list) (names: string list) =
        let ty, names = (Array.ofList ty, Array.ofList names)

        let liftLim = liftAllBefore sigma

        let rewrite rbinds (i, expri: Expr) =
            if i < liftLim then
                let tmpvi, tmpei = mkCompGenLocal expri.Range names[i] ty[i]
                let bindi = mkCompGenBind tmpvi expri
                tmpei, bindi :: rbinds
            else
                expri, rbinds

        let newExprs, reversedBinds = List.mapFold rewrite [] (exprs |> List.indexed)
        let binds = List.rev reversedBinds
        let reorderedExprs = permute sigma (Array.ofList newExprs)
        binds, Array.toList reorderedExprs

    /// Evaluate the expressions in the original order, but build a record with the results in field order
    /// Note some fields may be static. If this were not the case we could just use
    ///     let sigma = Array.map #Index ()
    /// However the presence of static fields means .Index may index into a non-compact set of instance field indexes.
    /// We still need to sort by index.
    let mkRecordExpr g (lnk, tcref, tinst, unsortedRecdFields: RecdFieldRef list, unsortedFieldExprs, m) =
        // Remove any abbreviations
        let tcref, tinst = destAppTy g (mkWoNullAppTy tcref tinst)

        let sortedRecdFields =
            unsortedRecdFields
            |> List.indexed
            |> Array.ofList
            |> Array.sortBy (fun (_, r) -> r.Index)

        let sigma = Array.create sortedRecdFields.Length -1

        sortedRecdFields
        |> Array.iteri (fun sortedIdx (unsortedIdx, _) ->
            if sigma[unsortedIdx] <> -1 then
                error (InternalError("bad permutation", m))

            sigma[unsortedIdx] <- sortedIdx)

        let unsortedArgTys =
            unsortedRecdFields |> List.map (fun rfref -> actualTyOfRecdFieldRef rfref tinst)

        let unsortedArgNames = unsortedRecdFields |> List.map (fun rfref -> rfref.FieldName)

        let unsortedArgBinds, sortedArgExprs =
            permuteExprList sigma unsortedFieldExprs unsortedArgTys unsortedArgNames

        let core = Expr.Op(TOp.Recd(lnk, tcref), tinst, sortedArgExprs, m)
        mkLetsBind m unsortedArgBinds core

    let mkAnonRecd (_g: TcGlobals) m (anonInfo: AnonRecdTypeInfo) (unsortedIds: Ident[]) (unsortedFieldExprs: Expr list) unsortedArgTys =
        let sortedRecdFields =
            unsortedFieldExprs
            |> List.indexed
            |> Array.ofList
            |> Array.sortBy (fun (i, _) -> unsortedIds[i].idText)

        let sortedArgTys =
            unsortedArgTys
            |> List.indexed
            |> List.sortBy (fun (i, _) -> unsortedIds[i].idText)
            |> List.map snd

        let sigma = Array.create sortedRecdFields.Length -1

        sortedRecdFields
        |> Array.iteri (fun sortedIdx (unsortedIdx, _) ->
            if sigma[unsortedIdx] <> -1 then
                error (InternalError("bad permutation", m))

            sigma[unsortedIdx] <- sortedIdx)

        let unsortedArgNames = unsortedIds |> Array.toList |> List.map (fun id -> id.idText)

        let unsortedArgBinds, sortedArgExprs =
            permuteExprList sigma unsortedFieldExprs unsortedArgTys unsortedArgNames

        let core = Expr.Op(TOp.AnonRecd anonInfo, sortedArgTys, sortedArgExprs, m)
        mkLetsBind m unsortedArgBinds core

    //-------------------------------------------------------------------------
    // List builders
    //-------------------------------------------------------------------------

    let mkRefCell g m ty e =
        mkRecordExpr g (RecdExpr, g.refcell_tcr_canon, [ ty ], [ mkRefCellContentsRef g ], [ e ], m)

    let mkRefCellGet g m ty e =
        mkRecdFieldGetViaExprAddr (e, mkRefCellContentsRef g, [ ty ], m)

    let mkRefCellSet g m ty e1 e2 =
        mkRecdFieldSetViaExprAddr (e1, mkRefCellContentsRef g, [ ty ], e2, m)

    let mkNil (g: TcGlobals) m ty =
        mkUnionCaseExpr (g.nil_ucref, [ ty ], [], m)

    let mkCons (g: TcGlobals) ty h t =
        mkUnionCaseExpr (g.cons_ucref, [ ty ], [ h; t ], unionRanges h.Range t.Range)

    let mkArray (argTy, args, m) = Expr.Op(TOp.Array, [ argTy ], args, m)

    let mkCompGenLocalAndInvisibleBind g nm m e =
        let locv, loce = mkCompGenLocal m nm (tyOfExpr g e)
        locv, loce, mkInvisibleBind locv e

    //----------------------------------------------------------------------------
    // Make some fragments of code
    //----------------------------------------------------------------------------

    let box = I_box(mkILTyvarTy 0us)

    let isinst = I_isinst(mkILTyvarTy 0us)

    let unbox = I_unbox_any(mkILTyvarTy 0us)

    let mkUnbox ty e m =
        mkAsmExpr ([ unbox ], [ ty ], [ e ], [ ty ], m)

    let mkBox ty e m =
        mkAsmExpr ([ box ], [], [ e ], [ ty ], m)

    let mkIsInst ty e m =
        mkAsmExpr ([ isinst ], [ ty ], [ e ], [ ty ], m)

    let mspec_Type_GetTypeFromHandle (g: TcGlobals) =
        mkILNonGenericStaticMethSpecInTy (g.ilg.typ_Type, "GetTypeFromHandle", [ g.iltyp_RuntimeTypeHandle ], g.ilg.typ_Type)

    let mspec_String_Length (g: TcGlobals) =
        mkILNonGenericInstanceMethSpecInTy (g.ilg.typ_String, "get_Length", [], g.ilg.typ_Int32)

    let mspec_String_Concat2 (g: TcGlobals) =
        mkILNonGenericStaticMethSpecInTy (g.ilg.typ_String, "Concat", [ g.ilg.typ_String; g.ilg.typ_String ], g.ilg.typ_String)

    let mspec_String_Concat3 (g: TcGlobals) =
        mkILNonGenericStaticMethSpecInTy (
            g.ilg.typ_String,
            "Concat",
            [ g.ilg.typ_String; g.ilg.typ_String; g.ilg.typ_String ],
            g.ilg.typ_String
        )

    let mspec_String_Concat4 (g: TcGlobals) =
        mkILNonGenericStaticMethSpecInTy (
            g.ilg.typ_String,
            "Concat",
            [ g.ilg.typ_String; g.ilg.typ_String; g.ilg.typ_String; g.ilg.typ_String ],
            g.ilg.typ_String
        )

    let mspec_String_Concat_Array (g: TcGlobals) =
        mkILNonGenericStaticMethSpecInTy (g.ilg.typ_String, "Concat", [ mkILArr1DTy g.ilg.typ_String ], g.ilg.typ_String)

    let fspec_Missing_Value (g: TcGlobals) =
        mkILFieldSpecInTy (g.iltyp_Missing, "Value", g.iltyp_Missing)

    let mkInitializeArrayMethSpec (g: TcGlobals) =
        let tref = g.FindSysILTypeRef "System.Runtime.CompilerServices.RuntimeHelpers"

        mkILNonGenericStaticMethSpecInTy (
            mkILNonGenericBoxedTy tref,
            "InitializeArray",
            [ g.ilg.typ_Array; g.iltyp_RuntimeFieldHandle ],
            ILType.Void
        )

    let mkInvalidCastExnNewobj (g: TcGlobals) =
        mkNormalNewobj (mkILCtorMethSpecForTy (mkILNonGenericBoxedTy (g.FindSysILTypeRef "System.InvalidCastException"), []))

    let typedExprForIntrinsic _g m (IntrinsicValRef(_, _, _, ty, _) as i) =
        let vref = ValRefForIntrinsic i
        exprForValRef m vref, ty

    //--------------------------------------------------------------------------
    // Make applications
    //---------------------------------------------------------------------------

    let primMkApp (f, fty) tyargs argsl m = Expr.App(f, fty, tyargs, argsl, m)

    // Check for the funky where a generic type instantiation at function type causes a generic function
    // to appear to accept more arguments than it really does, e.g. "id id 1", where the first "id" is
    // instantiated with "int -> int".
    //
    // In this case, apply the arguments one at a time.
    let isExpansiveUnderInstantiation g fty0 tyargs pargs argsl =
        isForallTy g fty0
        && let fty1 = formalApplyTys g fty0 (tyargs, pargs) in

           (not (isFunTy g fty1)
            || let rec loop fty xs =
                match xs with
                | [] -> false
                | _ :: t -> not (isFunTy g fty) || loop (rangeOfFunTy g fty) t in

               loop fty1 argsl)

    let mkExprAppAux g f fty argsl m =
        match argsl with
        | [] -> f
        | _ ->
            // Always combine the term application with a type application
            //
            // Combine the term application with a term application, but only when f' is an under-applied value of known arity
            match f with
            | Expr.App(f0, fty0, tyargs, pargs, m2) when
                (isNil pargs
                 || (match stripExpr f0 with
                     | Expr.Val(v, _, _) ->
                         match v.ValReprInfo with
                         | Some info -> info.NumCurriedArgs > pargs.Length
                         | None -> false
                     | _ -> false))
                && not (isExpansiveUnderInstantiation g fty0 tyargs pargs argsl)
                ->
                primMkApp (f0, fty0) tyargs (pargs @ argsl) (unionRanges m2 m)

            | _ ->
                // Don't combine. 'f' is not an application
                if not (isFunTy g fty) then
                    error (InternalError("expected a function type", m))

                primMkApp (f, fty) [] argsl m

    let rec mkAppsAux g f fty tyargsl argsl m =
        match tyargsl with
        | tyargs :: rest ->
            match tyargs with
            | [] -> mkAppsAux g f fty rest argsl m
            | _ ->
                let arfty = applyForallTy g fty tyargs
                mkAppsAux g (primMkApp (f, fty) tyargs [] m) arfty rest argsl m
        | [] -> mkExprAppAux g f fty argsl m

    let mkApps g ((f, fty), tyargsl, argl, m) = mkAppsAux g f fty tyargsl argl m

    let mkTyAppExpr m (f, fty) tyargs =
        match tyargs with
        | [] -> f
        | _ -> primMkApp (f, fty) tyargs [] m

    let mkCallGetGenericComparer (g: TcGlobals) m =
        typedExprForIntrinsic g m g.get_generic_comparer_info |> fst

    let mkCallGetGenericEREqualityComparer (g: TcGlobals) m =
        typedExprForIntrinsic g m g.get_generic_er_equality_comparer_info |> fst

    let mkCallGetGenericPEREqualityComparer (g: TcGlobals) m =
        typedExprForIntrinsic g m g.get_generic_per_equality_comparer_info |> fst

    let mkCallUnbox (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.unbox_info, [ [ ty ] ], [ e1 ], m)

    let mkCallUnboxFast (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.unbox_fast_info, [ [ ty ] ], [ e1 ], m)

    let mkCallTypeTest (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.istype_info, [ [ ty ] ], [ e1 ], m)

    let mkCallTypeOf (g: TcGlobals) m ty =
        mkApps g (typedExprForIntrinsic g m g.typeof_info, [ [ ty ] ], [], m)

    let mkCallTypeDefOf (g: TcGlobals) m ty =
        mkApps g (typedExprForIntrinsic g m g.typedefof_info, [ [ ty ] ], [], m)

    let mkCallDispose (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.dispose_info, [ [ ty ] ], [ e1 ], m)

    let mkCallSeq (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.seq_info, [ [ ty ] ], [ e1 ], m)

    let mkCallCreateInstance (g: TcGlobals) m ty =
        mkApps g (typedExprForIntrinsic g m g.create_instance_info, [ [ ty ] ], [ mkUnit g m ], m)

    let mkCallGetQuerySourceAsEnumerable (g: TcGlobals) m ty1 ty2 e1 =
        mkApps g (typedExprForIntrinsic g m g.query_source_as_enum_info, [ [ ty1; ty2 ] ], [ e1; mkUnit g m ], m)

    let mkCallNewQuerySource (g: TcGlobals) m ty1 ty2 e1 =
        mkApps g (typedExprForIntrinsic g m g.new_query_source_info, [ [ ty1; ty2 ] ], [ e1 ], m)

    let mkCallCreateEvent (g: TcGlobals) m ty1 ty2 e1 e2 e3 =
        mkApps g (typedExprForIntrinsic g m g.create_event_info, [ [ ty1; ty2 ] ], [ e1; e2; e3 ], m)

    let mkCallGenericComparisonWithComparerOuter (g: TcGlobals) m ty comp e1 e2 =
        mkApps g (typedExprForIntrinsic g m g.generic_comparison_withc_outer_info, [ [ ty ] ], [ comp; e1; e2 ], m)

    let mkCallGenericEqualityEROuter (g: TcGlobals) m ty e1 e2 =
        mkApps g (typedExprForIntrinsic g m g.generic_equality_er_outer_info, [ [ ty ] ], [ e1; e2 ], m)

    let mkCallGenericEqualityWithComparerOuter (g: TcGlobals) m ty comp e1 e2 =
        mkApps g (typedExprForIntrinsic g m g.generic_equality_withc_outer_info, [ [ ty ] ], [ comp; e1; e2 ], m)

    let mkCallGenericHashWithComparerOuter (g: TcGlobals) m ty comp e1 =
        mkApps g (typedExprForIntrinsic g m g.generic_hash_withc_outer_info, [ [ ty ] ], [ comp; e1 ], m)

    let mkCallEqualsOperator (g: TcGlobals) m ty e1 e2 =
        mkApps g (typedExprForIntrinsic g m g.equals_operator_info, [ [ ty ] ], [ e1; e2 ], m)

    let mkCallNotEqualsOperator (g: TcGlobals) m ty e1 e2 =
        mkApps g (typedExprForIntrinsic g m g.not_equals_operator, [ [ ty ] ], [ e1; e2 ], m)

    let mkCallLessThanOperator (g: TcGlobals) m ty e1 e2 =
        mkApps g (typedExprForIntrinsic g m g.less_than_operator, [ [ ty ] ], [ e1; e2 ], m)

    let mkCallLessThanOrEqualsOperator (g: TcGlobals) m ty e1 e2 =
        mkApps g (typedExprForIntrinsic g m g.less_than_or_equals_operator, [ [ ty ] ], [ e1; e2 ], m)

    let mkCallGreaterThanOperator (g: TcGlobals) m ty e1 e2 =
        mkApps g (typedExprForIntrinsic g m g.greater_than_operator, [ [ ty ] ], [ e1; e2 ], m)

    let mkCallGreaterThanOrEqualsOperator (g: TcGlobals) m ty e1 e2 =
        mkApps g (typedExprForIntrinsic g m g.greater_than_or_equals_operator, [ [ ty ] ], [ e1; e2 ], m)

    let mkCallAdditionOperator (g: TcGlobals) m ty e1 e2 =
        mkApps g (typedExprForIntrinsic g m g.unchecked_addition_info, [ [ ty; ty; ty ] ], [ e1; e2 ], m)

    let mkCallSubtractionOperator (g: TcGlobals) m ty e1 e2 =
        mkApps g (typedExprForIntrinsic g m g.unchecked_subtraction_info, [ [ ty; ty; ty ] ], [ e1; e2 ], m)

    let mkCallMultiplyOperator (g: TcGlobals) m ty1 ty2 retTy e1 e2 =
        mkApps g (typedExprForIntrinsic g m g.unchecked_multiply_info, [ [ ty1; ty2; retTy ] ], [ e1; e2 ], m)

    let mkCallDivisionOperator (g: TcGlobals) m ty1 ty2 retTy e1 e2 =
        mkApps g (typedExprForIntrinsic g m g.unchecked_division_info, [ [ ty1; ty2; retTy ] ], [ e1; e2 ], m)

    let mkCallModulusOperator (g: TcGlobals) m ty e1 e2 =
        mkApps g (typedExprForIntrinsic g m g.unchecked_modulus_info, [ [ ty; ty; ty ] ], [ e1; e2 ], m)

    let mkCallDefaultOf (g: TcGlobals) m ty =
        mkApps g (typedExprForIntrinsic g m g.unchecked_defaultof_info, [ [ ty ] ], [], m)

    let mkCallBitwiseAndOperator (g: TcGlobals) m ty e1 e2 =
        mkApps g (typedExprForIntrinsic g m g.bitwise_and_info, [ [ ty ] ], [ e1; e2 ], m)

    let mkCallBitwiseOrOperator (g: TcGlobals) m ty e1 e2 =
        mkApps g (typedExprForIntrinsic g m g.bitwise_or_info, [ [ ty ] ], [ e1; e2 ], m)

    let mkCallBitwiseXorOperator (g: TcGlobals) m ty e1 e2 =
        mkApps g (typedExprForIntrinsic g m g.bitwise_xor_info, [ [ ty ] ], [ e1; e2 ], m)

    let mkCallShiftLeftOperator (g: TcGlobals) m ty e1 e2 =
        mkApps g (typedExprForIntrinsic g m g.bitwise_shift_left_info, [ [ ty ] ], [ e1; e2 ], m)

    let mkCallShiftRightOperator (g: TcGlobals) m ty e1 e2 =
        mkApps g (typedExprForIntrinsic g m g.bitwise_shift_right_info, [ [ ty ] ], [ e1; e2 ], m)

    let mkCallUnaryNegOperator (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.unchecked_unary_minus_info, [ [ ty ] ], [ e1 ], m)

    let mkCallUnaryNotOperator (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.bitwise_unary_not_info, [ [ ty ] ], [ e1 ], m)

    let mkCallAdditionChecked (g: TcGlobals) m ty e1 e2 =
        mkApps g (typedExprForIntrinsic g m g.checked_addition_info, [ [ ty; ty; ty ] ], [ e1; e2 ], m)

    let mkCallSubtractionChecked (g: TcGlobals) m ty e1 e2 =
        mkApps g (typedExprForIntrinsic g m g.checked_subtraction_info, [ [ ty; ty; ty ] ], [ e1; e2 ], m)

    let mkCallMultiplyChecked (g: TcGlobals) m ty1 ty2 retTy e1 e2 =
        mkApps g (typedExprForIntrinsic g m g.checked_multiply_info, [ [ ty1; ty2; retTy ] ], [ e1; e2 ], m)

    let mkCallUnaryNegChecked (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.checked_unary_minus_info, [ [ ty ] ], [ e1 ], m)

    let mkCallToByteChecked (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.byte_checked_info, [ [ ty ] ], [ e1 ], m)

    let mkCallToSByteChecked (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.sbyte_checked_info, [ [ ty ] ], [ e1 ], m)

    let mkCallToInt16Checked (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.int16_checked_info, [ [ ty ] ], [ e1 ], m)

    let mkCallToUInt16Checked (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.uint16_checked_info, [ [ ty ] ], [ e1 ], m)

    let mkCallToIntChecked (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.int_checked_info, [ [ ty ] ], [ e1 ], m)

    let mkCallToInt32Checked (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.int32_checked_info, [ [ ty ] ], [ e1 ], m)

    let mkCallToUInt32Checked (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.uint32_checked_info, [ [ ty ] ], [ e1 ], m)

    let mkCallToInt64Checked (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.int64_checked_info, [ [ ty ] ], [ e1 ], m)

    let mkCallToUInt64Checked (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.uint64_checked_info, [ [ ty ] ], [ e1 ], m)

    let mkCallToIntPtrChecked (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.nativeint_checked_info, [ [ ty ] ], [ e1 ], m)

    let mkCallToUIntPtrChecked (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.unativeint_checked_info, [ [ ty ] ], [ e1 ], m)

    let mkCallToByteOperator (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.byte_operator_info, [ [ ty ] ], [ e1 ], m)

    let mkCallToSByteOperator (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.sbyte_operator_info, [ [ ty ] ], [ e1 ], m)

    let mkCallToInt16Operator (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.int16_operator_info, [ [ ty ] ], [ e1 ], m)

    let mkCallToUInt16Operator (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.uint16_operator_info, [ [ ty ] ], [ e1 ], m)

    let mkCallToInt32Operator (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.int32_operator_info, [ [ ty ] ], [ e1 ], m)

    let mkCallToUInt32Operator (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.uint32_operator_info, [ [ ty ] ], [ e1 ], m)

    let mkCallToInt64Operator (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.int64_operator_info, [ [ ty ] ], [ e1 ], m)

    let mkCallToUInt64Operator (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.uint64_operator_info, [ [ ty ] ], [ e1 ], m)

    let mkCallToSingleOperator (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.float32_operator_info, [ [ ty ] ], [ e1 ], m)

    let mkCallToDoubleOperator (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.float_operator_info, [ [ ty ] ], [ e1 ], m)

    let mkCallToIntPtrOperator (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.nativeint_operator_info, [ [ ty ] ], [ e1 ], m)

    let mkCallToUIntPtrOperator (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.unativeint_operator_info, [ [ ty ] ], [ e1 ], m)

    let mkCallToCharOperator (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.char_operator_info, [ [ ty ] ], [ e1 ], m)

    let mkCallToEnumOperator (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.enum_operator_info, [ [ ty ] ], [ e1 ], m)

    let mkCallArrayLength (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.array_length_info, [ [ ty ] ], [ e1 ], m)

    let mkCallArrayGet (g: TcGlobals) m ty e1 idx1 =
        mkApps g (typedExprForIntrinsic g m g.array_get_info, [ [ ty ] ], [ e1; idx1 ], m)

    let mkCallArray2DGet (g: TcGlobals) m ty e1 idx1 idx2 =
        mkApps g (typedExprForIntrinsic g m g.array2D_get_info, [ [ ty ] ], [ e1; idx1; idx2 ], m)

    let mkCallArray3DGet (g: TcGlobals) m ty e1 idx1 idx2 idx3 =
        mkApps g (typedExprForIntrinsic g m g.array3D_get_info, [ [ ty ] ], [ e1; idx1; idx2; idx3 ], m)

    let mkCallArray4DGet (g: TcGlobals) m ty e1 idx1 idx2 idx3 idx4 =
        mkApps g (typedExprForIntrinsic g m g.array4D_get_info, [ [ ty ] ], [ e1; idx1; idx2; idx3; idx4 ], m)

    let mkCallArraySet (g: TcGlobals) m ty e1 idx1 v =
        mkApps g (typedExprForIntrinsic g m g.array_set_info, [ [ ty ] ], [ e1; idx1; v ], m)

    let mkCallArray2DSet (g: TcGlobals) m ty e1 idx1 idx2 v =
        mkApps g (typedExprForIntrinsic g m g.array2D_set_info, [ [ ty ] ], [ e1; idx1; idx2; v ], m)

    let mkCallArray3DSet (g: TcGlobals) m ty e1 idx1 idx2 idx3 v =
        mkApps g (typedExprForIntrinsic g m g.array3D_set_info, [ [ ty ] ], [ e1; idx1; idx2; idx3; v ], m)

    let mkCallArray4DSet (g: TcGlobals) m ty e1 idx1 idx2 idx3 idx4 v =
        mkApps g (typedExprForIntrinsic g m g.array4D_set_info, [ [ ty ] ], [ e1; idx1; idx2; idx3; idx4; v ], m)

    let mkCallHash (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.hash_info, [ [ ty ] ], [ e1 ], m)

    let mkCallBox (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.box_info, [ [ ty ] ], [ e1 ], m)

    let mkCallIsNull (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.isnull_info, [ [ ty ] ], [ e1 ], m)

    let mkCallRaise (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.raise_info, [ [ ty ] ], [ e1 ], m)

    let mkCallNewDecimal (g: TcGlobals) m (e1, e2, e3, e4, e5) =
        mkApps g (typedExprForIntrinsic g m g.new_decimal_info, [], [ e1; e2; e3; e4; e5 ], m)

    let mkCallNewFormat (g: TcGlobals) m aty bty cty dty ety formatStringExpr =
        mkApps g (typedExprForIntrinsic g m g.new_format_info, [ [ aty; bty; cty; dty; ety ] ], [ formatStringExpr ], m)

    let tryMkCallBuiltInWitness (g: TcGlobals) traitInfo argExprs m =
        let info, tinst = g.MakeBuiltInWitnessInfo traitInfo
        let vref = ValRefForIntrinsic info

        match vref.TryDeref with
        | ValueSome v ->
            let f = exprForValRef m vref
            mkApps g ((f, v.Type), [ tinst ], argExprs, m) |> Some
        | ValueNone -> None

    let tryMkCallCoreFunctionAsBuiltInWitness (g: TcGlobals) info tyargs argExprs m =
        let vref = ValRefForIntrinsic info

        match vref.TryDeref with
        | ValueSome v ->
            let f = exprForValRef m vref
            mkApps g ((f, v.Type), [ tyargs ], argExprs, m) |> Some
        | ValueNone -> None

    let TryEliminateDesugaredConstants g m c =
        match c with
        | Const.Decimal d ->
            match Decimal.GetBits d with
            | [| lo; med; hi; signExp |] ->
                let scale = (min (((signExp &&& 0xFF0000) >>> 16) &&& 0xFF) 28) |> byte
                let isNegative = (signExp &&& 0x80000000) <> 0
                Some(mkCallNewDecimal g m (mkInt g m lo, mkInt g m med, mkInt g m hi, mkBool g m isNegative, mkByte g m scale))
            | _ -> failwith "unreachable"
        | _ -> None

    let mkCallSeqCollect g m alphaTy betaTy arg1 arg2 =
        let enumty2 =
            try
                rangeOfFunTy g (tyOfExpr g arg1)
            with _ -> (* defensive programming *)
                (mkSeqTy g betaTy)

        mkApps g (typedExprForIntrinsic g m g.seq_collect_info, [ [ alphaTy; enumty2; betaTy ] ], [ arg1; arg2 ], m)

    let mkCallSeqUsing g m resourceTy elemTy arg1 arg2 =
        // We're instantiating val using : 'a -> ('a -> 'sb) -> seq<'b> when 'sb :> seq<'b> and 'a :> IDisposable
        // We set 'sb -> range(typeof(arg2))
        let enumty =
            try
                rangeOfFunTy g (tyOfExpr g arg2)
            with _ -> (* defensive programming *)
                (mkSeqTy g elemTy)

        mkApps g (typedExprForIntrinsic g m g.seq_using_info, [ [ resourceTy; enumty; elemTy ] ], [ arg1; arg2 ], m)

    let mkCallSeqDelay g m elemTy arg1 =
        mkApps g (typedExprForIntrinsic g m g.seq_delay_info, [ [ elemTy ] ], [ arg1 ], m)

    let mkCallSeqAppend g m elemTy arg1 arg2 =
        mkApps g (typedExprForIntrinsic g m g.seq_append_info, [ [ elemTy ] ], [ arg1; arg2 ], m)

    let mkCallSeqGenerated g m elemTy arg1 arg2 =
        mkApps g (typedExprForIntrinsic g m g.seq_generated_info, [ [ elemTy ] ], [ arg1; arg2 ], m)

    let mkCallSeqFinally g m elemTy arg1 arg2 =
        mkApps g (typedExprForIntrinsic g m g.seq_finally_info, [ [ elemTy ] ], [ arg1; arg2 ], m)

    let mkCallSeqTryWith g m elemTy origSeq exnFilter exnHandler =
        mkApps g (typedExprForIntrinsic g m g.seq_trywith_info, [ [ elemTy ] ], [ origSeq; exnFilter; exnHandler ], m)

    let mkCallSeqOfFunctions g m ty1 ty2 arg1 arg2 arg3 =
        mkApps g (typedExprForIntrinsic g m g.seq_of_functions_info, [ [ ty1; ty2 ] ], [ arg1; arg2; arg3 ], m)

    let mkCallSeqToArray g m elemTy arg1 =
        mkApps g (typedExprForIntrinsic g m g.seq_to_array_info, [ [ elemTy ] ], [ arg1 ], m)

    let mkCallSeqToList g m elemTy arg1 =
        mkApps g (typedExprForIntrinsic g m g.seq_to_list_info, [ [ elemTy ] ], [ arg1 ], m)

    let mkCallSeqMap g m inpElemTy genElemTy arg1 arg2 =
        mkApps g (typedExprForIntrinsic g m g.seq_map_info, [ [ inpElemTy; genElemTy ] ], [ arg1; arg2 ], m)

    let mkCallSeqSingleton g m ty1 arg1 =
        mkApps g (typedExprForIntrinsic g m g.seq_singleton_info, [ [ ty1 ] ], [ arg1 ], m)

    let mkCallSeqEmpty g m ty1 =
        mkApps g (typedExprForIntrinsic g m g.seq_empty_info, [ [ ty1 ] ], [], m)

    let mkCall_sprintf (g: TcGlobals) m funcTy fmtExpr fillExprs =
        mkApps g (typedExprForIntrinsic g m g.sprintf_info, [ [ funcTy ] ], fmtExpr :: fillExprs, m)

    let mkCallDeserializeQuotationFSharp20Plus g m e1 e2 e3 e4 =
        let args = [ e1; e2; e3; e4 ]
        mkApps g (typedExprForIntrinsic g m g.deserialize_quoted_FSharp_20_plus_info, [], [ mkRefTupledNoTypes g m args ], m)

    let mkCallDeserializeQuotationFSharp40Plus g m e1 e2 e3 e4 e5 =
        let args = [ e1; e2; e3; e4; e5 ]
        mkApps g (typedExprForIntrinsic g m g.deserialize_quoted_FSharp_40_plus_info, [], [ mkRefTupledNoTypes g m args ], m)

    let mkCallCastQuotation g m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.cast_quotation_info, [ [ ty ] ], [ e1 ], m)

    let mkCallLiftValue (g: TcGlobals) m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.lift_value_info, [ [ ty ] ], [ e1 ], m)

    let mkCallLiftValueWithName (g: TcGlobals) m ty nm e1 =
        let vref = ValRefForIntrinsic g.lift_value_with_name_info
        // Use "Expr.ValueWithName" if it exists in FSharp.Core
        match vref.TryDeref with
        | ValueSome _ ->
            mkApps
                g
                (typedExprForIntrinsic g m g.lift_value_with_name_info, [ [ ty ] ], [ mkRefTupledNoTypes g m [ e1; mkString g m nm ] ], m)
        | ValueNone -> mkCallLiftValue g m ty e1

    let mkCallLiftValueWithDefn g m qty e1 =
        assert isQuotedExprTy g qty
        let ty = destQuotedExprTy g qty
        let vref = ValRefForIntrinsic g.lift_value_with_defn_info
        // Use "Expr.WithValue" if it exists in FSharp.Core
        match vref.TryDeref with
        | ValueSome _ ->
            let copyOfExpr = copyExpr g ValCopyFlag.CloneAll e1
            let quoteOfCopyOfExpr = Expr.Quote(copyOfExpr, ref None, false, m, qty)

            mkApps
                g
                (typedExprForIntrinsic g m g.lift_value_with_defn_info, [ [ ty ] ], [ mkRefTupledNoTypes g m [ e1; quoteOfCopyOfExpr ] ], m)
        | ValueNone -> Expr.Quote(e1, ref None, false, m, qty)

    let mkCallCheckThis g m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.check_this_info, [ [ ty ] ], [ e1 ], m)

    let mkCallFailInit g m =
        mkApps g (typedExprForIntrinsic g m g.fail_init_info, [], [ mkUnit g m ], m)

    let mkCallFailStaticInit g m =
        mkApps g (typedExprForIntrinsic g m g.fail_static_init_info, [], [ mkUnit g m ], m)

    let mkCallQuoteToLinqLambdaExpression g m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.quote_to_linq_lambda_info, [ [ ty ] ], [ e1 ], m)

    let mkOptionToNullable g m ty e1 =
        mkApps g (typedExprForIntrinsic g m g.option_toNullable_info, [ [ ty ] ], [ e1 ], m)

    let mkOptionDefaultValue g m ty e1 e2 =
        mkApps g (typedExprForIntrinsic g m g.option_defaultValue_info, [ [ ty ] ], [ e1; e2 ], m)

    let mkLazyDelayed g m ty f =
        mkApps g (typedExprForIntrinsic g m g.lazy_create_info, [ [ ty ] ], [ f ], m)

    let mkLazyForce g m ty e =
        mkApps g (typedExprForIntrinsic g m g.lazy_force_info, [ [ ty ] ], [ e; mkUnit g m ], m)

    let mkGetString g m e1 e2 =
        mkApps g (typedExprForIntrinsic g m g.getstring_info, [], [ e1; e2 ], m)

    let mkGetStringChar = mkGetString

    let mkGetStringLength g m e =
        let mspec = mspec_String_Length g

        Expr.Op(
            TOp.ILCall(false, false, false, false, ValUseFlag.NormalValUse, true, false, mspec.MethodRef, [], [], [ g.int32_ty ]),
            [],
            [ e ],
            m
        )

    let mkStaticCall_String_Concat2 g m arg1 arg2 =
        let mspec = mspec_String_Concat2 g

        Expr.Op(
            TOp.ILCall(false, false, false, false, ValUseFlag.NormalValUse, false, false, mspec.MethodRef, [], [], [ g.string_ty ]),
            [],
            [ arg1; arg2 ],
            m
        )

    let mkStaticCall_String_Concat3 g m arg1 arg2 arg3 =
        let mspec = mspec_String_Concat3 g

        Expr.Op(
            TOp.ILCall(false, false, false, false, ValUseFlag.NormalValUse, false, false, mspec.MethodRef, [], [], [ g.string_ty ]),
            [],
            [ arg1; arg2; arg3 ],
            m
        )

    let mkStaticCall_String_Concat4 g m arg1 arg2 arg3 arg4 =
        let mspec = mspec_String_Concat4 g

        Expr.Op(
            TOp.ILCall(false, false, false, false, ValUseFlag.NormalValUse, false, false, mspec.MethodRef, [], [], [ g.string_ty ]),
            [],
            [ arg1; arg2; arg3; arg4 ],
            m
        )

    let mkStaticCall_String_Concat_Array g m arg =
        let mspec = mspec_String_Concat_Array g

        Expr.Op(
            TOp.ILCall(false, false, false, false, ValUseFlag.NormalValUse, false, false, mspec.MethodRef, [], [], [ g.string_ty ]),
            [],
            [ arg ],
            m
        )

    // Quotations can't contain any IL.
    // As a result, we aim to get rid of all IL generation in the typechecker and pattern match
    // compiler, or else train the quotation generator to understand the generated IL.
    // Hence each of the following are marked with places where they are generated.

    // Generated by the optimizer and the encoding of 'for' loops
    let mkDecr (g: TcGlobals) m e =
        mkAsmExpr ([ AI_sub ], [], [ e; mkOne g m ], [ g.int_ty ], m)

    let mkIncr (g: TcGlobals) m e =
        mkAsmExpr ([ AI_add ], [], [ mkOne g m; e ], [ g.int_ty ], m)

    // Generated by the pattern match compiler and the optimizer for
    //    1. array patterns
    //    2. optimizations associated with getting 'for' loops into the shape expected by the JIT.
    //
    // NOTE: The conv.i4 assumes that int_ty is int32. Note: ldlen returns native UNSIGNED int
    let mkLdlen (g: TcGlobals) m arre =
        mkAsmExpr ([ I_ldlen; (AI_conv DT_I4) ], [], [ arre ], [ g.int_ty ], m)

    let mkLdelem (_g: TcGlobals) m ty arre idxe =
        mkAsmExpr ([ I_ldelem_any(ILArrayShape.SingleDimensional, mkILTyvarTy 0us) ], [ ty ], [ arre; idxe ], [ ty ], m)

    // This is generated in equality/compare/hash augmentations and in the pattern match compiler.
    // It is understood by the quotation processor and turned into "Equality" nodes.
    //
    // Note: this is IL assembly code, don't go inserting this in expressions which will be exposed via quotations
    let mkILAsmCeq (g: TcGlobals) m e1 e2 =
        mkAsmExpr ([ AI_ceq ], [], [ e1; e2 ], [ g.bool_ty ], m)

    let mkILAsmClt (g: TcGlobals) m e1 e2 =
        mkAsmExpr ([ AI_clt ], [], [ e1; e2 ], [ g.bool_ty ], m)

    // This is generated in the initialization of the "ctorv" field in the typechecker's compilation of
    // an implicit class construction.
    let mkNull m ty = Expr.Const(Const.Zero, m, ty)

    let mkThrow m ty e =
        mkAsmExpr ([ I_throw ], [], [ e ], [ ty ], m)

    // reraise - parsed as library call - internally represented as op form.
    let mkReraiseLibCall (g: TcGlobals) ty m =
        let ve, vt = typedExprForIntrinsic g m g.reraise_info
        Expr.App(ve, vt, [ ty ], [ mkUnit g m ], m)

    let mkReraise m returnTy =
        Expr.Op(TOp.Reraise, [ returnTy ], [], m) (* could suppress unitArg *)

    //--------------------------------------------------------------------------
    // Nullness tests and pokes
    //--------------------------------------------------------------------------

    (* match inp with DU(_) -> true | _ -> false *)
    let mkUnionCaseTest (g: TcGlobals) (e1, cref: UnionCaseRef, tinst, m) =
        let mbuilder = MatchBuilder(DebugPointAtBinding.NoneAtInvisible, m)
        let tg2 = mbuilder.AddResultTarget(Expr.Const(Const.Bool true, m, g.bool_ty))
        let tg3 = mbuilder.AddResultTarget(Expr.Const(Const.Bool false, m, g.bool_ty))

        let dtree =
            TDSwitch(e1, [ TCase(DecisionTreeTest.UnionCase(cref, tinst), tg2) ], Some tg3, m)

        let expr = mbuilder.Close(dtree, m, g.bool_ty)
        expr

    // Null tests are generated by
    //    1. The compilation of array patterns in the pattern match compiler
    //    2. The compilation of string patterns in the pattern match compiler
    let mkLabelled m l e =
        mkCompGenSequential m (Expr.Op(TOp.Label l, [], [], m)) e

    // Called for when creating compiled form of 'let fixed ...'.
    //
    // No sequence point is generated for this expression form as this function is only
    // used for compiler-generated code.
    let mkNullTest g m e1 e2 e3 =
        let mbuilder = MatchBuilder(DebugPointAtBinding.NoneAtInvisible, m)
        let tg2 = mbuilder.AddResultTarget(e2)
        let tg3 = mbuilder.AddResultTarget(e3)
        let dtree = TDSwitch(e1, [ TCase(DecisionTreeTest.IsNull, tg3) ], Some tg2, m)
        let expr = mbuilder.Close(dtree, m, tyOfExpr g e2)
        expr

    let mkNonNullTest (g: TcGlobals) m e =
        mkAsmExpr ([ AI_ldnull; AI_cgt_un ], [], [ e ], [ g.bool_ty ], m)

    // No sequence point is generated for this expression form as this function is only
    // used for compiler-generated code.
    let mkNonNullCond g m ty e1 e2 e3 =
        mkCond DebugPointAtBinding.NoneAtInvisible m ty (mkNonNullTest g m e1) e2 e3

    // No sequence point is generated for this expression form as this function is only
    // used for compiler-generated code.
    let mkIfThen (g: TcGlobals) m e1 e2 =
        mkCond DebugPointAtBinding.NoneAtInvisible m g.unit_ty e1 e2 (mkUnit g m)

[<AutoOpen>]
module internal ExprTransforms =

    //--------------------------------------------------------------------------
    // tupled lambda --> method/function with a given valReprInfo specification.
    //
    // AdjustArityOfLambdaBody: "(vs, body)" represents a lambda "fun (vs) -> body". The
    // aim is to produce a "static method" represented by a pair
    // "(mvs, body)" where mvs has the List.length "arity".
    //--------------------------------------------------------------------------

    let untupledToRefTupled g vs =
        let untupledTys = typesOfVals vs
        let m = (List.head vs).Range
        let tupledv, tuplede = mkCompGenLocal m "tupledArg" (mkRefTupledTy g untupledTys)

        let untupling_es =
            List.mapi (fun i _ -> mkTupleFieldGet g (tupInfoRef, tuplede, untupledTys, i, m)) untupledTys
        // These are non-sticky - at the caller,any sequence point for 'body' goes on 'body' _after_ the binding has been made
        tupledv, mkInvisibleLets m vs untupling_es

    // The required tupled-arity (arity) can either be 1
    // or N, and likewise for the tuple-arity of the input lambda, i.e. either 1 or N
    // where the N's will be identical.
    let AdjustArityOfLambdaBody g arity (vs: Val list) body =
        let nvs = vs.Length

        if not (nvs = arity || nvs = 1 || arity = 1) then
            failwith "lengths don't add up"

        if arity = 0 then
            vs, body
        elif nvs = arity then
            vs, body
        elif nvs = 1 then
            let v = vs.Head
            let untupledTys = destRefTupleTy g v.Type

            if (untupledTys.Length <> arity) then
                failwith "length untupledTys <> arity"

            let dummyvs, dummyes =
                untupledTys
                |> List.mapi (fun i ty -> mkCompGenLocal v.Range (v.LogicalName + "_" + string i) ty)
                |> List.unzip

            let body = mkInvisibleLet v.Range v (mkRefTupled g v.Range dummyes untupledTys) body
            dummyvs, body
        else
            let tupledv, untupler = untupledToRefTupled g vs
            [ tupledv ], untupler body

    let MultiLambdaToTupledLambda g vs body =
        match vs with
        | [] -> failwith "MultiLambdaToTupledLambda: expected some arguments"
        | [ v ] -> v, body
        | vs ->
            let tupledv, untupler = untupledToRefTupled g vs
            tupledv, untupler body

    [<return: Struct>]
    let (|RefTuple|_|) expr =
        match expr with
        | Expr.Op(TOp.Tuple(TupInfo.Const false), _, args, _) -> ValueSome args
        | _ -> ValueNone

    let MultiLambdaToTupledLambdaIfNeeded g (vs, arg) body =
        match vs, arg with
        | [], _ -> failwith "MultiLambdaToTupledLambda: expected some arguments"
        | [ v ], _ -> [ (v, arg) ], body
        | vs, RefTuple args when args.Length = vs.Length -> List.zip vs args, body
        | vs, _ ->
            let tupledv, untupler = untupledToRefTupled g vs
            [ (tupledv, arg) ], untupler body

    //--------------------------------------------------------------------------
    // Beta reduction via let-bindings. Reduce immediate apps. of lambdas to let bindings.
    // Includes binding the immediate application of generic
    // functions. Input type is the type of the function. Makes use of the invariant
    // that any two expressions have distinct local variables (because we explicitly copy
    // expressions).
    //------------------------------------------------------------------------

    let rec MakeApplicationAndBetaReduceAux g (f, fty, tyargsl: TType list list, argsl: Expr list, m) =
        match f with
        | Expr.Let(bind, body, mLet, _) ->
            // Lift bindings out, i.e. (let x = e in f) y --> let x = e in f y
            // This increases the scope of 'x', which I don't like as it mucks with debugging
            // scopes of variables, but this is an important optimization, especially when the '|>'
            // notation is used a lot.
            mkLetBind mLet bind (MakeApplicationAndBetaReduceAux g (body, fty, tyargsl, argsl, m))
        | _ ->
            match tyargsl with
            | [] :: rest -> MakeApplicationAndBetaReduceAux g (f, fty, rest, argsl, m)

            | tyargs :: rest ->
                // Bind type parameters by immediate substitution
                match f with
                | Expr.TyLambda(_, tyvs, body, _, bodyTy) when tyvs.Length = List.length tyargs ->
                    let tpenv = bindTypars tyvs tyargs emptyTyparInst
                    let body = instExpr g tpenv body
                    let bodyTyR = instType tpenv bodyTy
                    MakeApplicationAndBetaReduceAux g (body, bodyTyR, rest, argsl, m)

                | _ ->
                    let f = mkAppsAux g f fty [ tyargs ] [] m
                    let fty = applyTyArgs g fty tyargs
                    MakeApplicationAndBetaReduceAux g (f, fty, rest, argsl, m)
            | [] ->
                match argsl with
                | _ :: _ ->
                    // Bind term parameters by "let" explicit substitutions
                    //
                    // Only do this if there are enough lambdas for the number of arguments supplied. This is because
                    // all arguments get evaluated before application.
                    //
                    // VALID:
                    //      (fun a b -> E[a, b]) t1 t2 ---> let a = t1 in let b = t2 in E[t1, t2]
                    // INVALID:
                    //      (fun a -> E[a]) t1 t2 ---> let a = t1 in E[a] t2 UNLESS: E[a] has no effects OR t2 has no effects

                    match tryStripLambdaN argsl.Length f with
                    | Some(argvsl, body) ->
                        assert (argvsl.Length = argsl.Length)

                        let pairs, body =
                            List.mapFoldBack (MultiLambdaToTupledLambdaIfNeeded g) (List.zip argvsl argsl) body

                        let argvs2, args2 = List.unzip (List.concat pairs)
                        mkLetsBind m (mkCompGenBinds argvs2 args2) body
                    | _ -> mkExprAppAux g f fty argsl m

                | [] -> f

    let MakeApplicationAndBetaReduce g (f, fty, tyargsl, argl, m) =
        MakeApplicationAndBetaReduceAux g (f, fty, tyargsl, argl, m)

    [<return: Struct>]
    let (|NewDelegateExpr|_|) g expr =
        match expr with
        | Expr.Obj(lambdaId, ty, a, b, [ TObjExprMethod(c, d, e, tmvs, body, f) ], [], m) when isDelegateTy g ty ->
            ValueSome(
                lambdaId,
                List.concat tmvs,
                body,
                m,
                (fun bodyR -> Expr.Obj(lambdaId, ty, a, b, [ TObjExprMethod(c, d, e, tmvs, bodyR, f) ], [], m))
            )
        | _ -> ValueNone

    [<return: Struct>]
    let (|DelegateInvokeExpr|_|) g expr =
        match expr with
        | Expr.App(Expr.Val(invokeRef, _, _) as delInvokeRef, delInvokeTy, tyargs, [ delExpr; delInvokeArg ], m) when
            invokeRef.LogicalName = "Invoke" && isFSharpDelegateTy g (tyOfExpr g delExpr)
            ->
            ValueSome(delInvokeRef, delInvokeTy, tyargs, delExpr, delInvokeArg, m)
        | _ -> ValueNone

    [<return: Struct>]
    let (|OpPipeRight|_|) g expr =
        match expr with
        | Expr.App(Expr.Val(vref, _, _), _, [ _; resType ], [ xExpr; fExpr ], m) when valRefEq g vref g.piperight_vref ->
            ValueSome(resType, xExpr, fExpr, m)
        | _ -> ValueNone

    [<return: Struct>]
    let (|OpPipeRight2|_|) g expr =
        match expr with
        | Expr.App(Expr.Val(vref, _, _), _, [ _; _; resType ], [ Expr.Op(TOp.Tuple _, _, [ arg1; arg2 ], _); fExpr ], m) when
            valRefEq g vref g.piperight2_vref
            ->
            ValueSome(resType, arg1, arg2, fExpr, m)
        | _ -> ValueNone

    [<return: Struct>]
    let (|OpPipeRight3|_|) g expr =
        match expr with
        | Expr.App(Expr.Val(vref, _, _), _, [ _; _; _; resType ], [ Expr.Op(TOp.Tuple _, _, [ arg1; arg2; arg3 ], _); fExpr ], m) when
            valRefEq g vref g.piperight3_vref
            ->
            ValueSome(resType, arg1, arg2, arg3, fExpr, m)
        | _ -> ValueNone

    let rec MakeFSharpDelegateInvokeAndTryBetaReduce g (delInvokeRef, delExpr, delInvokeTy, tyargs, delInvokeArg, m) =
        match delExpr with
        | Expr.Let(bind, body, mLet, _) ->
            mkLetBind mLet bind (MakeFSharpDelegateInvokeAndTryBetaReduce g (delInvokeRef, body, delInvokeTy, tyargs, delInvokeArg, m))
        | NewDelegateExpr g (_, argvs & _ :: _, body, m, _) ->
            let pairs, body = MultiLambdaToTupledLambdaIfNeeded g (argvs, delInvokeArg) body
            let argvs2, args2 = List.unzip pairs
            mkLetsBind m (mkCompGenBinds argvs2 args2) body
        | _ ->
            // Remake the delegate invoke
            Expr.App(delInvokeRef, delInvokeTy, tyargs, [ delExpr; delInvokeArg ], m)

    //---------------------------------------------------------------------------
    // Adjust for expected usage
    // Convert a use of a value to saturate to the given arity.
    //---------------------------------------------------------------------------

    let MakeArgsForTopArgs (_g: TcGlobals) m argTysl tpenv =
        argTysl
        |> List.mapi (fun i argTys ->
            argTys
            |> List.mapi (fun j (argTy, argInfo: ArgReprInfo) ->
                let ty = instType tpenv argTy

                let nm =
                    match argInfo.Name with
                    | None -> CompilerGeneratedName("arg" + string i + string j)
                    | Some id -> id.idText

                fst (mkCompGenLocal m nm ty)))

    let AdjustValForExpectedValReprInfo g m (vref: ValRef) flags valReprInfo =

        let tps, argTysl, retTy, _ = GetValReprTypeInFSharpForm g valReprInfo vref.Type m
        let tpsR = copyTypars false tps
        let tyargsR = List.map mkTyparTy tpsR
        let tpenv = bindTypars tps tyargsR emptyTyparInst
        let rtyR = instType tpenv retTy
        let vsl = MakeArgsForTopArgs g m argTysl tpenv

        let call =
            MakeApplicationAndBetaReduce g (Expr.Val(vref, flags, m), vref.Type, [ tyargsR ], (List.map (mkRefTupledVars g m) vsl), m)

        let tauexpr, tauty =
            List.foldBack (fun vs (e, ty) -> mkMultiLambda m vs (e, ty), (mkFunTy g (mkRefTupledVarsTy g vs) ty)) vsl (call, rtyR)
        // Build a type-lambda expression for the toplevel value if needed...
        mkTypeLambda m tpsR (tauexpr, tauty), tpsR +-> tauty

    let stripTupledFunTy g ty =
        let argTys, retTy = stripFunTy g ty
        let curriedArgTys = argTys |> List.map (tryDestRefTupleTy g)
        curriedArgTys, retTy

    [<return: Struct>]
    let (|ExprValWithPossibleTypeInst|_|) expr =
        match expr with
        | Expr.App(Expr.Val(vref, flags, m), _fty, tyargs, [], _) -> ValueSome(vref, flags, tyargs, m)
        | Expr.Val(vref, flags, m) -> ValueSome(vref, flags, [], m)
        | _ -> ValueNone

    let mkCoerceIfNeeded g tgtTy srcTy expr =
        if typeEquiv g tgtTy srcTy then
            expr
        else
            mkCoerceExpr (expr, tgtTy, expr.Range, srcTy)

    let mkCompGenLetIn m nm ty e f =
        let v, ve = mkCompGenLocal m nm ty
        mkCompGenLet m v e (f (v, ve))

    let mkCompGenLetMutableIn m nm ty e f =
        let v, ve = mkMutableCompGenLocal m nm ty
        mkCompGenLet m v e (f (v, ve))

    /// Take a node representing a coercion from one function type to another, e.g.
    ///    A -> A * A -> int
    /// to
    ///    B -> B * A -> int
    /// and return an expression of the correct type that doesn't use a coercion type. For example
    /// return
    ///    (fun b1 b2 -> E (b1 :> A) (b2 :> A))
    ///
    ///    - Use good names for the closure arguments if available
    ///    - Create lambda variables if needed, or use the supplied arguments if available.
    ///
    /// Return the new expression and any unused suffix of supplied arguments
    ///
    /// If E is a value with TopInfo then use the arity to help create a better closure.
    /// In particular we can create a closure like this:
    ///    (fun b1 b2 -> E (b1 :> A) (b2 :> A))
    /// rather than
    ///    (fun b1 -> let clo = E (b1 :> A) in (fun b2 -> clo (b2 :> A)))
    /// The latter closures are needed to carefully preserve side effect order
    ///
    /// Note that the results of this translation are visible to quotations

    let AdjustPossibleSubsumptionExpr g (expr: Expr) (suppliedArgs: Expr list) : (Expr * Expr list) option =

        match expr with
        | Expr.Op(TOp.Coerce, [ inputTy; actualTy ], [ exprWithActualTy ], m) when isFunTy g actualTy && isFunTy g inputTy ->

            if typeEquiv g actualTy inputTy then
                Some(exprWithActualTy, suppliedArgs)
            else

                let curriedActualArgTys, retTy = stripTupledFunTy g actualTy

                let curriedInputTys, _ = stripFunTy g inputTy

                assert (curriedActualArgTys.Length = curriedInputTys.Length)

                let argTys =
                    (curriedInputTys, curriedActualArgTys) ||> List.mapi2 (fun i x y -> (i, x, y))

                // Use the nice names for a function of known arity and name. Note that 'nice' here also
                // carries a semantic meaning. For a function with top-info,
                //   let f (x: A) (y: A) (z: A) = ...
                // we know there are no side effects on the application of 'f' to 1, 2 args. This greatly simplifies
                // the closure built for
                //   f b1 b2
                // and indeed for
                //   f b1 b2 b3
                // we don't build any closure at all, and just return
                //   f (b1 :> A) (b2 :> A) (b3 :> A)

                let curriedNiceNames =
                    match stripExpr exprWithActualTy with
                    | ExprValWithPossibleTypeInst(vref, _, _, _) when vref.ValReprInfo.IsSome ->

                        let _, argTysl, _, _ =
                            GetValReprTypeInFSharpForm g vref.ValReprInfo.Value vref.Type expr.Range

                        argTysl
                        |> List.mapi (fun i argTys ->
                            argTys
                            |> List.mapi (fun j (_, argInfo) ->
                                match argInfo.Name with
                                | None -> CompilerGeneratedName("arg" + string i + string j)
                                | Some id -> id.idText))
                    | _ -> []

                let nCurriedNiceNames = curriedNiceNames.Length
                assert (curriedActualArgTys.Length >= nCurriedNiceNames)

                let argTysWithNiceNames, argTysWithoutNiceNames =
                    List.splitAt nCurriedNiceNames argTys

                /// Only consume 'suppliedArgs' up to at most the number of nice arguments
                let nSuppliedArgs = min suppliedArgs.Length nCurriedNiceNames
                let suppliedArgs, droppedSuppliedArgs = List.splitAt nSuppliedArgs suppliedArgs

                /// The relevant range for any expressions and applications includes the arguments
                let appm = (m, suppliedArgs) ||> List.fold (fun m e -> unionRanges m e.Range)

                // See if we have 'enough' suppliedArgs. If not, we have to build some lambdas, and,
                // we have to 'let' bind all arguments that we consume, e.g.
                //   Seq.take (effect;4) : int list -> int list
                // is a classic case. Here we generate
                //   let tmp = (effect;4) in
                //   (fun v -> Seq.take tmp (v :> seq<_>))
                let buildingLambdas = nSuppliedArgs <> nCurriedNiceNames

                /// Given a tuple of argument variables that has a tuple type that satisfies the input argument types,
                /// coerce it to a tuple that satisfies the matching coerced argument type(s).
                let CoerceDetupled (argTys: TType list) (detupledArgs: Expr list) (actualTys: TType list) =
                    assert (actualTys.Length = argTys.Length)
                    assert (actualTys.Length = detupledArgs.Length)
                    // Inject the coercions into the user-supplied explicit tuple
                    let argm = List.reduce unionRanges (detupledArgs |> List.map (fun e -> e.Range))
                    mkRefTupled g argm (List.map3 (mkCoerceIfNeeded g) actualTys argTys detupledArgs) actualTys

                /// Given an argument variable of tuple type that has been evaluated and stored in the
                /// given variable, where the tuple type that satisfies the input argument types,
                /// coerce it to a tuple that satisfies the matching coerced argument type(s).
                let CoerceBoundTuple tupleVar argTys (actualTys: TType list) =
                    assert (actualTys.Length > 1)

                    mkRefTupled
                        g
                        appm
                        ((actualTys, argTys)
                         ||> List.mapi2 (fun i actualTy dummyTy ->
                             let argExprElement = mkTupleFieldGet g (tupInfoRef, tupleVar, argTys, i, appm)
                             mkCoerceIfNeeded g actualTy dummyTy argExprElement))
                        actualTys

                /// Given an argument that has a tuple type that satisfies the input argument types,
                /// coerce it to a tuple that satisfies the matching coerced argument type. Try to detuple the argument if possible.
                let CoerceTupled niceNames (argExpr: Expr) (actualTys: TType list) =
                    let argExprTy = (tyOfExpr g argExpr)

                    let argTys =
                        match actualTys with
                        | [ _ ] -> [ tyOfExpr g argExpr ]
                        | _ -> tryDestRefTupleTy g argExprTy

                    assert (actualTys.Length = argTys.Length)

                    let nm =
                        match niceNames with
                        | [ nm ] -> nm
                        | _ -> "arg"

                    if buildingLambdas then
                        // Evaluate the user-supplied tuple-valued argument expression, inject the coercions and build an explicit tuple
                        // Assign the argument to make sure it is only run once
                        //     f ~~>: B -> int
                        //     f ~~> : (B * B) -> int
                        //
                        //  for
                        //     let f a = 1
                        //     let f (a, a) = 1
                        let v, ve = mkCompGenLocal appm nm argExprTy
                        let binderBuilder = (fun tm -> mkCompGenLet appm v argExpr tm)

                        let expr =
                            match actualTys, argTys with
                            | [ actualTy ], [ argTy ] -> mkCoerceIfNeeded g actualTy argTy ve
                            | _ -> CoerceBoundTuple ve argTys actualTys

                        binderBuilder, expr
                    else if typeEquiv g (mkRefTupledTy g actualTys) argExprTy then
                        id, argExpr
                    else

                        let detupledArgs, argTys =
                            match actualTys with
                            | [ _actualType ] -> [ argExpr ], [ tyOfExpr g argExpr ]
                            | _ -> tryDestRefTupleExpr argExpr, tryDestRefTupleTy g argExprTy

                        // OK, the tuples match, or there is no de-tupling,
                        //     f x
                        //     f (x, y)
                        //
                        //  for
                        //     let f (x, y) = 1
                        // and we're not building lambdas, just coerce the arguments in place
                        if detupledArgs.Length = actualTys.Length then
                            id, CoerceDetupled argTys detupledArgs actualTys
                        else
                            // In this case there is a tuple mismatch.
                            //     f p
                            //
                            //
                            //  for
                            //     let f (x, y) = 1
                            // Assign the argument to make sure it is only run once
                            let v, ve = mkCompGenLocal appm nm argExprTy
                            let binderBuilder = (fun tm -> mkCompGenLet appm v argExpr tm)
                            let expr = CoerceBoundTuple ve argTys actualTys
                            binderBuilder, expr

                // This variable is really a dummy to make the code below more regular.
                // In the i = N - 1 cases we skip the introduction of the 'let' for
                // this variable.
                let resVar, resVarAsExpr = mkCompGenLocal appm "result" retTy
                let N = argTys.Length

                let cloVar, exprForOtherArgs, _ =
                    List.foldBack
                        (fun (i, inpArgTy, actualArgTys) (cloVar: Val, res, resTy) ->

                            let inpArgTys =
                                match actualArgTys with
                                | [ _ ] -> [ inpArgTy ]
                                | _ -> destRefTupleTy g inpArgTy

                            assert (inpArgTys.Length = actualArgTys.Length)

                            let inpsAsVars, inpsAsExprs =
                                inpArgTys
                                |> List.mapi (fun j ty -> mkCompGenLocal appm ("arg" + string i + string j) ty)
                                |> List.unzip

                            let inpsAsActualArg = CoerceDetupled inpArgTys inpsAsExprs actualArgTys
                            let inpCloVarType = mkFunTy g (mkRefTupledTy g actualArgTys) cloVar.Type
                            let newResTy = mkFunTy g inpArgTy resTy

                            let inpCloVar, inpCloVarAsExpr =
                                mkCompGenLocal appm ("clo" + string i) inpCloVarType

                            let newRes =
                                // For the final arg we can skip introducing the dummy variable
                                if i = N - 1 then
                                    mkMultiLambda
                                        appm
                                        inpsAsVars
                                        (mkApps g ((inpCloVarAsExpr, inpCloVarType), [], [ inpsAsActualArg ], appm), resTy)
                                else
                                    mkMultiLambda
                                        appm
                                        inpsAsVars
                                        (mkCompGenLet
                                            appm
                                            cloVar
                                            (mkApps g ((inpCloVarAsExpr, inpCloVarType), [], [ inpsAsActualArg ], appm))
                                            res,
                                         resTy)

                            inpCloVar, newRes, newResTy)
                        argTysWithoutNiceNames
                        (resVar, resVarAsExpr, retTy)

                let exprForAllArgs =
                    if isNil argTysWithNiceNames then
                        mkCompGenLet appm cloVar exprWithActualTy exprForOtherArgs
                    else
                        // Mark the up as Some/None
                        let suppliedArgs =
                            List.map Some suppliedArgs
                            @ List.replicate (nCurriedNiceNames - nSuppliedArgs) None

                        assert (suppliedArgs.Length = nCurriedNiceNames)

                        let lambdaBuilders, binderBuilders, inpsAsArgs =

                            (argTysWithNiceNames, curriedNiceNames, suppliedArgs)
                            |||> List.map3 (fun (_, inpArgTy, actualArgTys) niceNames suppliedArg ->

                                let inpArgTys =
                                    match actualArgTys with
                                    | [ _ ] -> [ inpArgTy ]
                                    | _ -> destRefTupleTy g inpArgTy

                                /// Note: there might not be enough nice names, and they might not match in arity
                                let niceNames =
                                    match niceNames with
                                    | nms when nms.Length = inpArgTys.Length -> nms
                                    | [ nm ] -> inpArgTys |> List.mapi (fun i _ -> (nm + string i))
                                    | nms -> nms

                                match suppliedArg with
                                | Some arg ->
                                    let binderBuilder, inpsAsActualArg = CoerceTupled niceNames arg actualArgTys
                                    let lambdaBuilder = id
                                    lambdaBuilder, binderBuilder, inpsAsActualArg
                                | None ->
                                    let inpsAsVars, inpsAsExprs =
                                        (niceNames, inpArgTys)
                                        ||> List.map2 (fun nm ty -> mkCompGenLocal appm nm ty)
                                        |> List.unzip

                                    let inpsAsActualArg = CoerceDetupled inpArgTys inpsAsExprs actualArgTys
                                    let lambdaBuilder = (fun tm -> mkMultiLambda appm inpsAsVars (tm, tyOfExpr g tm))
                                    let binderBuilder = id
                                    lambdaBuilder, binderBuilder, inpsAsActualArg)
                            |> List.unzip3

                        // If no trailing args then we can skip introducing the dummy variable
                        // This corresponds to
                        //    let f (x: A) = 1
                        //
                        //   f ~~> type B -> int
                        //
                        // giving
                        //   (fun b -> f (b :> A))
                        // rather than
                        //   (fun b -> let clo = f (b :> A) in clo)
                        let exprApp =
                            if isNil argTysWithoutNiceNames then
                                mkApps g ((exprWithActualTy, actualTy), [], inpsAsArgs, appm)
                            else
                                mkCompGenLet appm cloVar (mkApps g ((exprWithActualTy, actualTy), [], inpsAsArgs, appm)) exprForOtherArgs

                        List.foldBack (fun f acc -> f acc) binderBuilders (List.foldBack (fun f acc -> f acc) lambdaBuilders exprApp)

                Some(exprForAllArgs, droppedSuppliedArgs)
        | _ -> None

    /// Find and make all subsumption eliminations
    let NormalizeAndAdjustPossibleSubsumptionExprs g inputExpr =
        let expr, args =
            // AdjustPossibleSubsumptionExpr can take into account an application
            match stripExpr inputExpr with
            | Expr.App(f, _fty, [], args, _) -> f, args

            | _ -> inputExpr, []

        match AdjustPossibleSubsumptionExpr g expr args with
        | None -> inputExpr
        | Some(exprR, []) -> exprR
        | Some(exprR, argsR) ->
            //printfn "adjusted...."
            Expr.App(exprR, tyOfExpr g exprR, [], argsR, inputExpr.Range)

    //---------------------------------------------------------------------------
    // LinearizeTopMatch - when only one non-failing target, make linear. The full
    // complexity of this is only used for spectacularly rare bindings such as
    //    type ('a, 'b) either = This of 'a | That of 'b
    //    let this_f1 = This (fun x -> x)
    //    let This fA | That fA = this_f1
    //
    // Here a polymorphic top level binding "fA" is _computed_ by a pattern match!!!
    // The TAST coming out of type checking must, however, define fA as a type function,
    // since it is marked with an arity that indicates it's r.h.s. is a type function]
    // without side effects and so can be compiled as a generic method (for example).

    // polymorphic things bound in complex matches at top level require eta expansion of the
    // type function to ensure the r.h.s. of the binding is indeed a type function
    let etaExpandTypeLambda g m tps (tm, ty) =
        if isNil tps then
            tm
        else
            mkTypeLambda m tps (mkApps g ((tm, ty), [ (List.map mkTyparTy tps) ], [], m), ty)

    let AdjustValToHaveValReprInfo (tmp: Val) parent valData =
        tmp.SetValReprInfo(Some valData)
        tmp.SetDeclaringEntity parent
        tmp.SetIsMemberOrModuleBinding()

    let destInt32 =
        function
        | Expr.Const(Const.Int32 n, _, _) -> Some n
        | _ -> None

    let destThrow =
        function
        | Expr.Op(TOp.ILAsm([ I_throw ], [ ty2 ]), [], [ e ], m) -> Some(m, ty2, e)
        | _ -> None

    let isThrow x = Option.isSome (destThrow x)

    let isIDelegateEventType g ty =
        match tryTcrefOfAppTy g ty with
        | ValueSome tcref -> tyconRefEq g g.fslib_IDelegateEvent_tcr tcref
        | _ -> false

    let destIDelegateEventType g ty =
        if isIDelegateEventType g ty then
            match argsOfAppTy g ty with
            | [ ty1 ] -> ty1
            | _ -> failwith "destIDelegateEventType: internal error"
        else
            failwith "destIDelegateEventType: not an IDelegateEvent type"

    /// For match with only one non-failing target T0, the other targets, T1... failing (say, raise exception).
    ///   tree, T0(v0, .., vN) => rhs ; T1() => fail ; ...
    /// Convert it to bind T0's variables, then continue with T0's rhs:
    ///   let tmp = switch tree, TO(fv0, ..., fvN) => Tup (fv0, ..., fvN) ; T1() => fail; ...
    ///   let v1 = #1 tmp in ...
    ///   and vN = #N tmp
    ///   rhs
    /// Motivation:
    /// - For top-level let bindings with possibly failing matches,
    ///   this makes clear that subsequent bindings (if reached) are top-level ones.
    let LinearizeTopMatchAux g parent (spBind, m, tree, targets, m2, ty) =
        let targetsL = Array.toList targets
        (* items* package up 0, 1, more items *)
        let itemsProj tys i x =
            match tys with
            | [] -> failwith "itemsProj: no items?"
            | [ _ ] -> x (* no projection needed *)
            | tys -> Expr.Op(TOp.TupleFieldGet(tupInfoRef, i), tys, [ x ], m)

        let isThrowingTarget =
            function
            | TTarget(_, x, _) -> isThrow x

        if 1 + List.count isThrowingTarget targetsL = targetsL.Length then
            // Have failing targets and ONE successful one, so linearize
            let (TTarget(vs, rhs, _)) = List.find (isThrowingTarget >> not) targetsL

            let fvs =
                vs
                |> List.map (fun v -> fst (mkLocal v.Range v.LogicalName v.Type)) (* fresh *)

            let vtys = vs |> List.map (fun v -> v.Type)
            let tmpTy = mkRefTupledVarsTy g vs
            let tmp, tmpe = mkCompGenLocal m "matchResultHolder" tmpTy

            AdjustValToHaveValReprInfo tmp parent ValReprInfo.emptyValData

            let newTg = TTarget(fvs, mkRefTupledVars g m fvs, None)

            let fixup (TTarget(tvs, tx, flags)) =
                match destThrow tx with
                | Some(m, _, e) ->
                    let tx = mkThrow m tmpTy e
                    TTarget(tvs, tx, flags) (* Throwing targets, recast it's "return type" *)
                | None -> newTg (* Non-throwing target, replaced [new/old] *)

            let targets = Array.map fixup targets

            let binds =
                vs
                |> List.mapi (fun i v ->
                    let ty = v.Type
                    let rhs = etaExpandTypeLambda g m v.Typars (itemsProj vtys i tmpe, ty)
                    // update the arity of the value
                    v.SetValReprInfo(Some(InferValReprInfoOfExpr g AllowTypeDirectedDetupling.Yes ty [] [] rhs))
                    // This binding is deliberately non-sticky - any sequence point for 'rhs' goes on 'rhs' _after_ the binding has been evaluated
                    mkInvisibleBind v rhs) in (* vi = proj tmp *)

            mkCompGenLet
                m
                tmp
                (primMkMatch (spBind, m, tree, targets, m2, tmpTy)) (* note, probably retyped match, but note, result still has same type *)
                (mkLetsFromBindings m binds rhs)
        else
            (* no change *)
            primMkMatch (spBind, m, tree, targets, m2, ty)

    let LinearizeTopMatch g parent =
        function
        | Expr.Match(spBind, m, tree, targets, m2, ty) -> LinearizeTopMatchAux g parent (spBind, m, tree, targets, m2, ty)
        | x -> x

    // CLEANUP NOTE: Get rid of this mutation.
    let ClearValReprInfo (f: Val) =
        f.SetValReprInfo None
        f
