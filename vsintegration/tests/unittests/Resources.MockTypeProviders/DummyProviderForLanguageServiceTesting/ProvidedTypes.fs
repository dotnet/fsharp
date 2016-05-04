// Copyright (c) Microsoft Corporation 2005-2012.
// This sample code is provided "as is" without warranty of any kind. 
// We disclaim all warranties, either express or implied, including the 
// warranties of merchantability and fitness for a particular purpose. 

// This file contains a set of helper types and methods for providing types in an implementation 
// of ITypeProvider.

// This code has been modified and is appropriate for use in conjunction with the F# 3.0-4.0 releases

namespace ProviderImplementation.ProvidedTypes

open System
open System.Text
open System.IO
open System.Reflection
open System.Reflection.Emit
open System.Linq.Expressions
open System.Collections.Generic
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open Microsoft.FSharp.Quotations.DerivedPatterns
open Microsoft.FSharp.Core.CompilerServices

//--------------------------------------------------------------------------------
// UncheckedQuotations

// The FSharp.Core 2.0 - 4.0 (4.0.0.0 - 4.4.0.0) quotations implementation is overly strict in that it doesn't allow 
// generation of quotations for cross-targeted FSharp.Core.  Below we define a series of Unchecked methods
// implemented via reflection hacks to allow creation of various nodes when using a cross-targets FSharp.Core and
// mscorlib.dll.  
//
//   - Most importantly, these cross-targeted quotations can be provided to the F# compiler by a type provider.  
//     They are generally produced via the AssemblyReplacer.fs component through a process of rewriting design-time quotations that
//     are not cross-targeted.
//
//   - However, these quotation values are a bit fragile. Using existing FSharp.Core.Quotations.Patterns 
//     active patterns on these quotation nodes will generally work correctly. But using ExprShape.RebuildShapeCombination 
//     on these new nodes will not succed, nor will operations that build new quotations such as Expr.Call. 
//     Instead, use the replacement provided in this module.
//
//   - Likewise, some operations in these quotation values like "expr.Type" may be a bit fragile, possibly returning non cross-targeted types in 
//     the result. However those operations are not used by the F# compiler.
[<AutoOpen>]
module internal UncheckedQuotations =

    let qTy = typeof<Microsoft.FSharp.Quotations.Var>.Assembly.GetType("Microsoft.FSharp.Quotations.ExprConstInfo") 
    assert (qTy <> null)
    let pTy = typeof<Microsoft.FSharp.Quotations.Var>.Assembly.GetType("Microsoft.FSharp.Quotations.PatternsModule")
    assert (pTy<> null)

    // These are handles to the internal functions that create quotation nodes of different sizes. Although internal, 
    // these function names have been stable since F# 2.0.
    let mkFE0 = pTy.GetMethod("mkFE0", BindingFlags.Static ||| BindingFlags.Public ||| BindingFlags.NonPublic)
    assert (mkFE0 <> null)
    let mkFE1 = pTy.GetMethod("mkFE1", BindingFlags.Static ||| BindingFlags.Public ||| BindingFlags.NonPublic)
    assert (mkFE1 <> null)
    let mkFE2 = pTy.GetMethod("mkFE2", BindingFlags.Static ||| BindingFlags.Public ||| BindingFlags.NonPublic)
    assert (mkFE2 <> null)
    let mkFEN = pTy.GetMethod("mkFEN", BindingFlags.Static ||| BindingFlags.Public ||| BindingFlags.NonPublic)
    assert (mkFEN <> null)

    // These are handles to the internal tags attached to quotation nodes of different sizes. Although internal, 
    // these function names have been stable since F# 2.0.
    let newDelegateOp = qTy.GetMethod("NewNewDelegateOp", BindingFlags.Static ||| BindingFlags.Public ||| BindingFlags.NonPublic)
    assert (newDelegateOp <> null)
    let instanceCallOp = qTy.GetMethod("NewInstanceMethodCallOp", BindingFlags.Static ||| BindingFlags.Public ||| BindingFlags.NonPublic)
    assert (instanceCallOp <> null)
    let staticCallOp = qTy.GetMethod("NewStaticMethodCallOp", BindingFlags.Static ||| BindingFlags.Public ||| BindingFlags.NonPublic)
    assert (staticCallOp <> null)
    let newObjectOp = qTy.GetMethod("NewNewObjectOp", BindingFlags.Static ||| BindingFlags.Public ||| BindingFlags.NonPublic)
    assert (newObjectOp <> null)
    let newArrayOp = qTy.GetMethod("NewNewArrayOp", BindingFlags.Static ||| BindingFlags.Public ||| BindingFlags.NonPublic)
    assert (newArrayOp <> null)
    let appOp = qTy.GetMethod("get_AppOp", BindingFlags.Static ||| BindingFlags.Public ||| BindingFlags.NonPublic)
    assert (appOp <> null)
    let instancePropGetOp = qTy.GetMethod("NewInstancePropGetOp", BindingFlags.Static ||| BindingFlags.Public ||| BindingFlags.NonPublic)
    assert (instancePropGetOp <> null)
    let staticPropGetOp = qTy.GetMethod("NewStaticPropGetOp", BindingFlags.Static ||| BindingFlags.Public ||| BindingFlags.NonPublic)
    assert (staticPropGetOp <> null)
    let instancePropSetOp = qTy.GetMethod("NewInstancePropSetOp", BindingFlags.Static ||| BindingFlags.Public ||| BindingFlags.NonPublic)
    assert (instancePropSetOp <> null)
    let staticPropSetOp = qTy.GetMethod("NewStaticPropSetOp", BindingFlags.Static ||| BindingFlags.Public ||| BindingFlags.NonPublic)
    assert (staticPropSetOp <> null)
    let instanceFieldGetOp = qTy.GetMethod("NewInstanceFieldGetOp", BindingFlags.Static ||| BindingFlags.Public ||| BindingFlags.NonPublic)
    assert (instanceFieldGetOp <> null)
    let staticFieldGetOp = qTy.GetMethod("NewStaticFieldGetOp", BindingFlags.Static ||| BindingFlags.Public ||| BindingFlags.NonPublic)
    assert (staticFieldGetOp <> null)
    let instanceFieldSetOp = qTy.GetMethod("NewInstanceFieldSetOp", BindingFlags.Static ||| BindingFlags.Public ||| BindingFlags.NonPublic)
    assert (instanceFieldSetOp <> null)
    let staticFieldSetOp = qTy.GetMethod("NewStaticFieldSetOp", BindingFlags.Static ||| BindingFlags.Public ||| BindingFlags.NonPublic)
    assert (staticFieldSetOp <> null)
    let tupleGetOp = qTy.GetMethod("NewTupleGetOp", BindingFlags.Static ||| BindingFlags.Public ||| BindingFlags.NonPublic)
    assert (tupleGetOp <> null)
    let letOp = qTy.GetMethod("get_LetOp", BindingFlags.Static ||| BindingFlags.Public ||| BindingFlags.NonPublic)
    assert (letOp <> null)
      
    type Microsoft.FSharp.Quotations.Expr with 

        static member NewDelegateUnchecked (ty: Type, vs: Var list, body: Expr) =
            let e =  List.foldBack (fun v acc -> Expr.Lambda(v,acc)) vs body 
            let op = newDelegateOp.Invoke(null, [| box ty |])
            mkFE1.Invoke(null, [| box op; box e |]) :?> Expr

        static member NewObjectUnchecked (cinfo: ConstructorInfo, args : Expr list) =
            let op = newObjectOp.Invoke(null, [| box cinfo |])
            mkFEN.Invoke(null, [| box op; box args |]) :?> Expr

        static member NewArrayUnchecked (elementType: Type, elements : Expr list) =
            let op = newArrayOp.Invoke(null, [| box elementType |])
            mkFEN.Invoke(null, [| box op; box elements |]) :?> Expr

        static member CallUnchecked (minfo: MethodInfo, args : Expr list) =
            let op = staticCallOp.Invoke(null, [| box minfo |])
            mkFEN.Invoke(null, [| box op; box args |]) :?> Expr

        static member CallUnchecked (obj: Expr, minfo: MethodInfo, args : Expr list) =
            let op = instanceCallOp.Invoke(null, [| box minfo |])
            mkFEN.Invoke(null, [| box op; box (obj::args) |]) :?> Expr

        static member ApplicationUnchecked (f: Expr, x: Expr) =
            let op = appOp.Invoke(null, [| |])
            mkFE2.Invoke(null, [| box op; box f; box x |]) :?> Expr

        static member PropertyGetUnchecked (pinfo: PropertyInfo, args : Expr list) =
            let op = staticPropGetOp.Invoke(null, [| box pinfo |])
            mkFEN.Invoke(null, [| box op; box args |]) :?> Expr

        static member PropertyGetUnchecked (obj: Expr, pinfo: PropertyInfo, ?args : Expr list) =
            let args = defaultArg args []
            let op = instancePropGetOp.Invoke(null, [| box pinfo |])
            mkFEN.Invoke(null, [| box op; box (obj::args) |]) :?> Expr

        static member PropertySetUnchecked (pinfo: PropertyInfo, value: Expr, ?args : Expr list) =
            let args = defaultArg args []
            let op = staticPropSetOp.Invoke(null, [| box pinfo |])
            mkFEN.Invoke(null, [| box op; box (args@[value]) |]) :?> Expr

        static member PropertySetUnchecked (obj: Expr, pinfo: PropertyInfo, value: Expr, args : Expr list) =
            let op = instancePropSetOp.Invoke(null, [| box pinfo |])
            mkFEN.Invoke(null, [| box op; box (obj::(args@[value])) |]) :?> Expr

        static member FieldGetUnchecked (pinfo: FieldInfo) =
            let op = staticFieldGetOp.Invoke(null, [| box pinfo |])
            mkFE0.Invoke(null, [| box op; |]) :?> Expr

        static member FieldGetUnchecked (obj: Expr, pinfo: FieldInfo) =
            let op = instanceFieldGetOp.Invoke(null, [| box pinfo |])
            mkFE1.Invoke(null, [| box op; box obj |]) :?> Expr

        static member FieldSetUnchecked (pinfo: FieldInfo, value: Expr) =
            let op = staticFieldSetOp.Invoke(null, [| box pinfo |])
            mkFE1.Invoke(null, [| box op; box value |]) :?> Expr

        static member FieldSetUnchecked (obj: Expr, pinfo: FieldInfo, value: Expr) =
            let op = instanceFieldSetOp.Invoke(null, [| box pinfo |])
            mkFE2.Invoke(null, [| box op; box obj; box value |]) :?> Expr

        static member TupleGetUnchecked (e: Expr, n:int) =
            let op = tupleGetOp.Invoke(null, [| box e.Type; box n |])
            mkFE1.Invoke(null, [| box op; box e |]) :?> Expr

        static member LetUnchecked (v:Var, e: Expr, body:Expr) =
            let lam = Expr.Lambda(v,body)
            let op = letOp.Invoke(null, [| |])
            mkFE2.Invoke(null, [| box op; box e; box lam |]) :?> Expr

    type Shape = Shape of (Expr list -> Expr)
    
    let (|ShapeCombinationUnchecked|ShapeVarUnchecked|ShapeLambdaUnchecked|) e =
        match e with 
        | NewObject (cinfo, args) ->
            ShapeCombinationUnchecked (Shape (function args -> Expr.NewObjectUnchecked (cinfo, args)), args)
        | NewArray (ty, args) ->
            ShapeCombinationUnchecked (Shape (function args -> Expr.NewArrayUnchecked (ty, args)), args)
        | NewDelegate (t, vars, expr) ->
            ShapeCombinationUnchecked (Shape (function [expr] -> Expr.NewDelegateUnchecked (t, vars, expr) | _ -> invalidArg "expr" "invalid shape"), [expr])
        | TupleGet (expr, n) ->
            ShapeCombinationUnchecked (Shape (function [expr] -> Expr.TupleGetUnchecked (expr, n) | _ -> invalidArg "expr" "invalid shape"), [expr])
        | Application (f, x) ->
            ShapeCombinationUnchecked (Shape (function [f; x] -> Expr.ApplicationUnchecked (f, x) | _ -> invalidArg "expr" "invalid shape"), [f; x])
        | Call (objOpt, minfo, args) ->
            match objOpt with 
            | None -> ShapeCombinationUnchecked (Shape (function args -> Expr.CallUnchecked (minfo, args)), args)
            | Some obj -> ShapeCombinationUnchecked (Shape (function (obj::args) -> Expr.CallUnchecked (obj, minfo, args) | _ -> invalidArg "expr" "invalid shape"), obj::args)
        | PropertyGet (objOpt, pinfo, args) ->
            match objOpt with 
            | None -> ShapeCombinationUnchecked (Shape (function args -> Expr.PropertyGetUnchecked (pinfo, args)), args)
            | Some obj -> ShapeCombinationUnchecked (Shape (function (obj::args) -> Expr.PropertyGetUnchecked (obj, pinfo, args) | _ -> invalidArg "expr" "invalid shape"), obj::args)
        | PropertySet (objOpt, pinfo, args, value) ->
            match objOpt with 
            | None -> ShapeCombinationUnchecked (Shape (function (value::args) -> Expr.PropertySetUnchecked (pinfo, value, args) | _ -> invalidArg "expr" "invalid shape"), value::args)
            | Some obj -> ShapeCombinationUnchecked (Shape (function (obj::value::args) -> Expr.PropertySetUnchecked (obj, pinfo, value, args) | _ -> invalidArg "expr" "invalid shape"), obj::value::args)
        | FieldGet (objOpt, pinfo) ->
            match objOpt with 
            | None -> ShapeCombinationUnchecked (Shape (function _ -> Expr.FieldGetUnchecked (pinfo)), [])
            | Some obj -> ShapeCombinationUnchecked (Shape (function [obj] -> Expr.FieldGetUnchecked (obj, pinfo) | _ -> invalidArg "expr" "invalid shape"), [obj])
        | FieldSet (objOpt, pinfo, value) ->
            match objOpt with 
            | None -> ShapeCombinationUnchecked (Shape (function [value] -> Expr.FieldSetUnchecked (pinfo, value) | _ -> invalidArg "expr" "invalid shape"), [value])
            | Some obj -> ShapeCombinationUnchecked (Shape (function [obj;value] -> Expr.FieldSetUnchecked (obj, pinfo, value) | _ -> invalidArg "expr" "invalid shape"), [obj; value])
        | Let (var, value, body) -> 
            ShapeCombinationUnchecked (Shape (function [value;Lambda(var, body)] -> Expr.LetUnchecked(var, value, body) | _ -> invalidArg "expr" "invalid shape"), [value; Expr.Lambda(var, body)])
        | TupleGet (expr, i) ->
            ShapeCombinationUnchecked (Shape (function [expr] -> Expr.TupleGetUnchecked (expr, i) | _ -> invalidArg "expr" "invalid shape"), [expr])
        | ExprShape.ShapeCombination (comb,args) -> 
            ShapeCombinationUnchecked (Shape (fun args -> ExprShape.RebuildShapeCombination(comb, args)), args)
        | ExprShape.ShapeVar v -> ShapeVarUnchecked v
        | ExprShape.ShapeLambda (v, e) -> ShapeLambdaUnchecked (v,e)

    let RebuildShapeCombinationUnchecked (Shape comb,args) = comb args

//--------------------------------------------------------------------------------

module QuotationSimplifier = 

    let transExpr isGenerated q =     
        let rec trans q = 
            match q with 
            // convert NewTuple to the call to the constructor of the Tuple type (only for generated types)
            | NewTuple(items) when isGenerated ->
                let rec mkCtor args ty = 
                    let ctor, restTyOpt = Reflection.FSharpValue.PreComputeTupleConstructorInfo ty
                    match restTyOpt with
                    | None -> Expr.NewObject(ctor, List.map trans args)
                    | Some restTy ->
                        let curr = [for a in Seq.take 7 args -> trans a]
                        let rest = List.ofSeq (Seq.skip 7 args) 
                        Expr.NewObject(ctor, curr @ [mkCtor rest restTy])
                let tys = [| for e in items -> e.Type |]
                let tupleTy = Reflection.FSharpType.MakeTupleType tys
                trans (mkCtor items tupleTy)
            // convert TupleGet to the chain of PropertyGet calls (only for generated types)
            | TupleGet(e, i) when isGenerated ->
                let rec mkGet ty i (e : Expr)  = 
                    let pi, restOpt = Reflection.FSharpValue.PreComputeTuplePropertyInfo(ty, i)
                    let propGet = Expr.PropertyGet(e, pi)
                    match restOpt with
                    | None -> propGet
                    | Some (restTy, restI) -> mkGet restTy restI propGet
                trans (mkGet e.Type i (trans e))
            | Value(value, ty) ->
                if value <> null then
                   let tyOfValue = value.GetType()
                   transValue(value, tyOfValue, ty)
                else q
            // Eliminate F# property gets to method calls
            | PropertyGet(obj,propInfo,args) -> 
                match obj with 
                | None -> trans (Expr.CallUnchecked(propInfo.GetGetMethod(),args))
                | Some o -> trans (Expr.CallUnchecked(trans o,propInfo.GetGetMethod(),args))
            // Eliminate F# property sets to method calls
            | PropertySet(obj,propInfo,args,v) -> 
                 match obj with 
                 | None -> trans (Expr.CallUnchecked(propInfo.GetSetMethod(),args@[v]))
                 | Some o -> trans (Expr.CallUnchecked(trans o,propInfo.GetSetMethod(),args@[v]))
            // Eliminate F# function applications to FSharpFunc<_,_>.Invoke calls
            | Application(f,e) -> 
                trans (Expr.CallUnchecked(trans f, f.Type.GetMethod "Invoke", [ e ]) )
            | NewUnionCase(ci, es) ->
                trans (Expr.CallUnchecked(Reflection.FSharpValue.PreComputeUnionConstructorInfo ci, es) )
            | NewRecord(ci, es) ->
                trans (Expr.NewObjectUnchecked(Reflection.FSharpValue.PreComputeRecordConstructorInfo ci, es) )
            | UnionCaseTest(e,uc) ->
                let tagInfo = Reflection.FSharpValue.PreComputeUnionTagMemberInfo uc.DeclaringType
                let tagExpr = 
                    match tagInfo with 
                    | :? PropertyInfo as tagProp ->
                         trans (Expr.PropertyGet(e,tagProp) )
                    | :? MethodInfo as tagMeth -> 
                         if tagMeth.IsStatic then trans (Expr.Call(tagMeth, [e]))
                         else trans (Expr.Call(e,tagMeth,[]))
                    | _ -> failwith "unreachable: unexpected result from PreComputeUnionTagMemberInfo"
                let tagNumber = uc.Tag
                trans <@@ (%%(tagExpr) : int) = tagNumber @@>

            // Explicitly handle weird byref variables in lets (used to populate out parameters), since the generic handlers can't deal with byrefs
            | Let(v,vexpr,bexpr) when v.Type.IsByRef ->

                // the binding must have leaves that are themselves variables (due to the limited support for byrefs in expressions)
                // therefore, we can perform inlining to translate this to a form that can be compiled
                inlineByref v vexpr bexpr

            // Eliminate recursive let bindings (which are unsupported by the type provider API) to regular let bindings
            | LetRecursive(bindings, expr) ->
                // This uses a "lets and sets" approach, converting something like
                //    let rec even = function
                //    | 0 -> true
                //    | n -> odd (n-1)
                //    and odd = function
                //    | 0 -> false
                //    | n -> even (n-1)
                //    X
                // to something like
                //    let even = ref Unchecked.defaultof<_>
                //    let odd  = ref Unchecked.defaultof<_>
                //    even := function
                //            | 0 -> true
                //            | n -> !odd (n-1)
                //    odd  := function
                //            | 0 -> false
                //            | n -> !even (n-1)
                //    X'
                // where X' is X but with occurrences of even/odd substituted by !even and !odd (since now even and odd are references)
                // Translation relies on typedefof<_ ref> - does this affect ability to target different runtime and design time environments?
                let vars = List.map fst bindings
                let vars' = vars |> List.map (fun v -> Quotations.Var(v.Name, typedefof<_ ref>.MakeGenericType(v.Type)))
                
                // init t generates the equivalent of <@ ref Unchecked.defaultof<t> @>
                let init (t:Type) =
                    let r = match <@ ref 1 @> with Call(None, r, [_]) -> r | _ -> failwith "Extracting MethodInfo from <@ 1 @> failed"
                    let d = match <@ Unchecked.defaultof<_> @> with Call(None, d, []) -> d | _ -> failwith "Extracting MethodInfo from <@ Unchecked.defaultof<_> @> failed"
                    Expr.Call(r.GetGenericMethodDefinition().MakeGenericMethod(t), [Expr.Call(d.GetGenericMethodDefinition().MakeGenericMethod(t),[])])

                // deref v generates the equivalent of <@ !v @>
                // (so v's type must be ref<something>)
                let deref (v:Quotations.Var) = 
                    let m = match <@ !(ref 1) @> with Call(None, m, [_]) -> m | _ -> failwith "Extracting MethodInfo from <@ !(ref 1) @> failed"
                    let tyArgs = v.Type.GetGenericArguments()
                    Expr.Call(m.GetGenericMethodDefinition().MakeGenericMethod(tyArgs), [Expr.Var v])

                // substitution mapping a variable v to the expression <@ !v' @> using the corresponding new variable v' of ref type
                let subst =
                    let map =
                        vars'
                        |> List.map deref
                        |> List.zip vars
                        |> Map.ofList
                    fun v -> Map.tryFind v map

                let expr' = expr.Substitute(subst)

                // maps variables to new variables
                let varDict = List.zip vars vars' |> dict

                // given an old variable v and an expression e, returns a quotation like <@ v' := e @> using the corresponding new variable v' of ref type
                let setRef (v:Quotations.Var) e = 
                    let m = match <@ (ref 1) := 2 @> with Call(None, m, [_;_]) -> m | _ -> failwith "Extracting MethodInfo from <@ (ref 1) := 2 @> failed"
                    Expr.Call(m.GetGenericMethodDefinition().MakeGenericMethod(v.Type), [Expr.Var varDict.[v]; e])

                // Something like 
                //  <@
                //      v1 := e1'
                //      v2 := e2'
                //      ...
                //      expr'
                //  @>
                // Note that we must substitute our new variable dereferences into the bound expressions
                let body = 
                    bindings
                    |> List.fold (fun b (v,e) -> Expr.Sequential(setRef v (e.Substitute subst), b)) expr'
                
                // Something like
                //   let v1 = ref Unchecked.defaultof<t1>
                //   let v2 = ref Unchecked.defaultof<t2>
                //   ...
                //   body
                vars
                |> List.fold (fun b v -> Expr.LetUnchecked(varDict.[v], init v.Type, b)) body                
                |> trans 

            // Handle the generic cases
            | ShapeLambdaUnchecked(v,body) -> 
                Expr.Lambda(v, trans body)
            | ShapeCombinationUnchecked(comb,args) -> 
                RebuildShapeCombinationUnchecked(comb,List.map trans args)
            | ShapeVarUnchecked _ -> q

        and inlineByref v vexpr bexpr =
            match vexpr with
            | Sequential(e',vexpr') ->
                (* let v = (e'; vexpr') in bexpr => e'; let v = vexpr' in bexpr *)
                Expr.Sequential(e', inlineByref v vexpr' bexpr)
                |> trans
            | IfThenElse(c,b1,b2) ->
                (* let v = if c then b1 else b2 in bexpr => if c then let v = b1 in bexpr else let v = b2 in bexpr *)
                Expr.IfThenElse(c, inlineByref v b1 bexpr, inlineByref v b2 bexpr)
                |> trans
            | Var _ -> 
                (* let v = v1 in bexpr => bexpr[v/v1] *)
                bexpr.Substitute(fun v' -> if v = v' then Some vexpr else None)
                |> trans
            | _ -> 
                failwith (sprintf "Unexpected byref binding: %A = %A" v vexpr)

        and transValue (v : obj, tyOfValue : Type, expectedTy : Type) = 
            let rec transArray (o : Array, ty : Type) = 
                let elemTy = ty.GetElementType()
                let converter = getConverterForType elemTy
                let elements = 
                    [
                        for el in o do
                            yield converter el
                    ]
                Expr.NewArrayUnchecked(elemTy, elements)
            and transList(o, ty : Type, nil, cons) =
                let converter = getConverterForType (ty.GetGenericArguments().[0])
                o
                |> Seq.cast
                |> List.ofSeq
                |> fun l -> List.foldBack(fun o s -> Expr.NewUnionCase(cons, [ converter(o); s ])) l (Expr.NewUnionCase(nil, []))
                |> trans

            and getConverterForType (ty : Type) = 
                if ty.IsArray then 
                    fun (v : obj) -> transArray(v :?> Array, ty)
                elif ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<_ list> then 
                    let nil, cons =
                        let cases = Reflection.FSharpType.GetUnionCases(ty)
                        let a = cases.[0]
                        let b = cases.[1]
                        if a.Name = "Empty" then a,b
                        else b,a
                     
                    fun v -> transList (v :?> System.Collections.IEnumerable, ty, nil, cons)
                else 
                    fun v -> Expr.Value(v, ty)
            let converter = getConverterForType tyOfValue
            let r = converter v
            if tyOfValue <> expectedTy then Expr.Coerce(r, expectedTy)
            else r
        trans q

    let getFastFuncType (args : list<Expr>) resultType =
        let types =
            [|
                for arg in args -> arg.Type
                yield resultType
            |]
        let fastFuncTy = 
            match List.length args with
            | 2 -> typedefof<OptimizedClosures.FSharpFunc<_, _, _>>.MakeGenericType(types)
            | 3 -> typedefof<OptimizedClosures.FSharpFunc<_, _, _, _>>.MakeGenericType(types)
            | 4 -> typedefof<OptimizedClosures.FSharpFunc<_, _, _, _, _>>.MakeGenericType(types)
            | 5 -> typedefof<OptimizedClosures.FSharpFunc<_, _, _, _, _, _>>.MakeGenericType(types)
            | _ -> invalidArg "args" "incorrect number of arguments"
        fastFuncTy.GetMethod("Adapt")
    
    let inline (===) a b = LanguagePrimitives.PhysicalEquality a b
    
    let traverse f = 
        let rec fallback e = 
            match e with
            | Let(v, value, body) ->
                let fixedValue = f fallback value
                let fixedBody = f fallback body
                if fixedValue === value && fixedBody === body then 
                    e
                else
                    Expr.Let(v, fixedValue, fixedBody) 
            | ShapeVarUnchecked _ -> e
            | ShapeLambdaUnchecked(v, body) -> 
                let fixedBody = f fallback body 
                if fixedBody === body then 
                    e
                else
                    Expr.Lambda(v, fixedBody)
            | ShapeCombinationUnchecked(shape, exprs) -> 
                let exprs1 = List.map (f fallback) exprs
                if List.forall2 (===) exprs exprs1 then 
                    e
                else
                    RebuildShapeCombinationUnchecked(shape, exprs1)
        fun e -> f fallback e

    let RightPipe = <@@ (|>) @@>
    let inlineRightPipe expr = 
        let rec loop expr = traverse loopCore expr
        and loopCore fallback orig = 
            match orig with
            | SpecificCall RightPipe (None, _, [operand; applicable]) ->
                let fixedOperand = loop operand
                match loop applicable with
                | Lambda(arg, body) ->
                    let v = Quotations.Var("__temp", operand.Type)
                    let ev = Expr.Var v

                    let fixedBody = loop body
                    Expr.Let(v, fixedOperand, fixedBody.Substitute(fun v1 -> if v1 = arg then Some ev else None))
                | fixedApplicable -> Expr.Application(fixedApplicable, fixedOperand)
            | x -> fallback x
        loop expr

    let inlineValueBindings e = 
        let map = Dictionary(HashIdentity.Reference)
        let rec loop expr = traverse loopCore expr
        and loopCore fallback orig = 
            match orig with
            | Let(id, (Value(_) as v), body) when not id.IsMutable ->
                map.[id] <- v
                let fixedBody = loop body
                map.Remove(id) |> ignore
                fixedBody
            | ShapeVarUnchecked v -> 
                match map.TryGetValue v with
                | true, e -> e
                | _ -> orig
            | x -> fallback x
        loop e


    let optimizeCurriedApplications expr = 
        let rec loop expr = traverse loopCore expr
        and loopCore fallback orig = 
            match orig with
            | Application(e, arg) -> 
                let e1 = tryPeelApplications e [loop arg]
                if e1 === e then 
                    orig 
                else 
                    e1
            | x -> fallback x
        and tryPeelApplications orig args = 
            let n = List.length args
            match orig with
            | Application(e, arg) -> 
                let e1 = tryPeelApplications e ((loop arg)::args)
                if e1 === e then 
                    orig 
                else 
                    e1
            | Let(id, applicable, (Lambda(_) as body)) when n > 0 -> 
                let numberOfApplication = countPeelableApplications body id 0
                if numberOfApplication = 0 then orig
                elif n = 1 then Expr.Application(applicable, List.head args)
                elif n <= 5 then
                    let resultType = 
                        applicable.Type 
                        |> Seq.unfold (fun t -> 
                            if not t.IsGenericType then None else
                            let args = t.GetGenericArguments()
                            if args.Length <> 2 then None else
                            Some (args.[1], args.[1])
                        )
                        |> Seq.toArray
                        |> (fun arr -> arr.[n - 1])

                    let adaptMethod = getFastFuncType args resultType
                    let adapted = Expr.Call(adaptMethod, [loop applicable])
                    let invoke = adapted.Type.GetMethod("Invoke", [| for arg in args -> arg.Type |])
                    Expr.Call(adapted, invoke, args)
                else
                    (applicable, args) ||> List.fold (fun e a -> Expr.Application(e, a))
            | _ -> 
                orig
        and countPeelableApplications expr v n =
            match expr with
            // v - applicable entity obtained on the prev step
            // \arg -> let v1 = (f arg) in rest ==> f 
            | Lambda(arg, Let(v1, Application(Var f, Var arg1), rest)) when v = f && arg = arg1 -> countPeelableApplications rest v1 (n + 1)
            // \arg -> (f arg) ==> f
            | Lambda(arg, Application(Var f, Var arg1)) when v = f && arg = arg1 -> n
            | _ -> n
        loop expr
    
    // Use the real variable names instead of indices, to improve output of Debug.fs
    let transQuotationToCode isGenerated qexprf (paramNames: string[]) (argExprs: Expr[]) = 
        // Add let bindings for arguments to ensure that arguments will be evaluated
        let vars = argExprs |> Array.mapi (fun i e -> Quotations.Var(paramNames.[i], e.Type))
        let expr = qexprf ([for v in vars -> Expr.Var v])

        let pairs = Array.zip argExprs vars
        let expr = Array.foldBack (fun (arg, var) e -> Expr.LetUnchecked(var, arg, e)) pairs expr
        let expr = 
            if isGenerated then
                let e1 = inlineRightPipe expr
                let e2 = optimizeCurriedApplications e1
                let e3 = inlineValueBindings e2
                e3
            else
                expr

        transExpr isGenerated expr


[<AutoOpen>]
module internal Misc =

    type internal ExpectedStackState = 
        | Empty = 1
        | Address = 2
        | Value = 3

    let TypeBuilderInstantiationType = 
        let runningOnMono = try System.Type.GetType("Mono.Runtime") <> null with e -> false 
        let typeName = if runningOnMono then "System.Reflection.MonoGenericClass" else "System.Reflection.Emit.TypeBuilderInstantiation"
        typeof<TypeBuilder>.Assembly.GetType(typeName)

    let GetTypeFromHandleMethod = typeof<Type>.GetMethod("GetTypeFromHandle")
    let LanguagePrimitivesType = typedefof<list<_>>.Assembly.GetType("Microsoft.FSharp.Core.LanguagePrimitives")
    let ParseInt32Method = LanguagePrimitivesType.GetMethod "ParseInt32"
    let DecimalConstructor = typeof<decimal>.GetConstructor([| typeof<int>; typeof<int>; typeof<int>; typeof<bool>; typeof<byte> |])
    let DateTimeConstructor = typeof<DateTime>.GetConstructor([| typeof<int64>; typeof<DateTimeKind> |])
    let DateTimeOffsetConstructor = typeof<DateTimeOffset>.GetConstructor([| typeof<int64>; typeof<TimeSpan> |])
    let TimeSpanConstructor = typeof<TimeSpan>.GetConstructor([|typeof<int64>|])
    let isEmpty s = s = ExpectedStackState.Empty
    let isAddress s = s = ExpectedStackState.Address

    let nonNull str x = if x=null then failwith ("Null in " + str) else x
    
    let notRequired opname item = 
        let msg = sprintf "The operation '%s' on item '%s' should not be called on provided type, member or parameter" opname item
        System.Diagnostics.Debug.Assert (false, msg)
        raise (System.NotSupportedException msg)

    let mkParamArrayCustomAttributeData() = 
#if FX_NO_CUSTOMATTRIBUTEDATA
        { new IProvidedCustomAttributeData with 
#else
        { new CustomAttributeData() with 
#endif 
            member __.Constructor =  typeof<ParamArrayAttribute>.GetConstructors().[0]
            member __.ConstructorArguments = upcast [| |]
            member __.NamedArguments = upcast [| |] }

#if FX_NO_CUSTOMATTRIBUTEDATA
    let CustomAttributeTypedArgument(ty,v) = 
        { new IProvidedCustomAttributeTypedArgument with 
              member x.ArgumentType = ty
              member x.Value = v }
    let CustomAttributeNamedArgument(memb,arg:IProvidedCustomAttributeTypedArgument) = 
        { new IProvidedCustomAttributeNamedArgument with 
              member x.MemberInfo = memb
              member x.ArgumentType = arg.ArgumentType
              member x.TypedValue = arg }
    type CustomAttributeData = Microsoft.FSharp.Core.CompilerServices.IProvidedCustomAttributeData
#endif

    let mkEditorHideMethodsCustomAttributeData() = 
#if FX_NO_CUSTOMATTRIBUTEDATA
        { new IProvidedCustomAttributeData with 
#else
        { new CustomAttributeData() with 
#endif 
            member __.Constructor =  typeof<TypeProviderEditorHideMethodsAttribute>.GetConstructors().[0]
            member __.ConstructorArguments = upcast [| |]
            member __.NamedArguments = upcast [| |] }

    let mkAllowNullLiteralCustomAttributeData value =
#if FX_NO_CUSTOMATTRIBUTEDATA
        { new IProvidedCustomAttributeData with 
#else
        { new CustomAttributeData() with 
#endif 
            member __.Constructor = typeof<AllowNullLiteralAttribute>.GetConstructors().[0]
            member __.ConstructorArguments = upcast [| CustomAttributeTypedArgument(typeof<bool>, value) |]
            member __.NamedArguments = upcast [| |] }

    /// This makes an xml doc attribute w.r.t. an amortized computation of an xml doc string.
    /// It is important that the text of the xml doc only get forced when poking on the ConstructorArguments
    /// for the CustomAttributeData object.
    let mkXmlDocCustomAttributeDataLazy(lazyText: Lazy<string>) = 
#if FX_NO_CUSTOMATTRIBUTEDATA
        { new IProvidedCustomAttributeData with 
#else
        { new CustomAttributeData() with 
#endif
            member __.Constructor =  typeof<TypeProviderXmlDocAttribute>.GetConstructors().[0]
            member __.ConstructorArguments = upcast [| CustomAttributeTypedArgument(typeof<string>, lazyText.Force())  |]
            member __.NamedArguments = upcast [| |] }

    let mkXmlDocCustomAttributeData(s:string) =  mkXmlDocCustomAttributeDataLazy (lazy s)

    let mkDefinitionLocationAttributeCustomAttributeData(line:int,column:int,filePath:string) = 
#if FX_NO_CUSTOMATTRIBUTEDATA
        { new IProvidedCustomAttributeData with 
#else
        { new CustomAttributeData() with 
#endif
            member __.Constructor =  typeof<TypeProviderDefinitionLocationAttribute>.GetConstructors().[0]
            member __.ConstructorArguments = upcast [| |]
            member __.NamedArguments = 
                upcast [| CustomAttributeNamedArgument(typeof<TypeProviderDefinitionLocationAttribute>.GetProperty("FilePath"), CustomAttributeTypedArgument(typeof<string>, filePath));
                            CustomAttributeNamedArgument(typeof<TypeProviderDefinitionLocationAttribute>.GetProperty("Line"), CustomAttributeTypedArgument(typeof<int>, line)) ;
                            CustomAttributeNamedArgument(typeof<TypeProviderDefinitionLocationAttribute>.GetProperty("Column"), CustomAttributeTypedArgument(typeof<int>, column)) 
                        |] }
    let mkObsoleteAttributeCustomAttributeData(message:string, isError: bool) = 
#if FX_NO_CUSTOMATTRIBUTEDATA
        { new IProvidedCustomAttributeData with 
#else
        { new CustomAttributeData() with 
#endif
                member __.Constructor =  typeof<System.ObsoleteAttribute>.GetConstructors() |> Array.find (fun x -> x.GetParameters().Length = 2)
                member __.ConstructorArguments = upcast [|CustomAttributeTypedArgument(typeof<string>, message) ; CustomAttributeTypedArgument(typeof<bool>, isError)  |]
                member __.NamedArguments = upcast [| |] }

    type CustomAttributesImpl() =
        let customAttributes = ResizeArray<CustomAttributeData>()
        let mutable hideObjectMethods = false
        let mutable nonNullable = false
        let mutable obsoleteMessage = None
        let mutable xmlDocDelayed = None
        let mutable xmlDocAlwaysRecomputed = None
        let mutable hasParamArray = false

        // XML doc text that we only compute once, if any. This must _not_ be forced until the ConstructorArguments
        // property of the custom attribute is foced.
        let xmlDocDelayedText = 
            lazy 
                (match xmlDocDelayed with None -> assert false; "" | Some f -> f())

        // Custom atttributes that we only compute once
        let customAttributesOnce = 
            lazy 
               [| if hideObjectMethods then yield mkEditorHideMethodsCustomAttributeData() 
                  if nonNullable then yield mkAllowNullLiteralCustomAttributeData false
                  match xmlDocDelayed with None -> () | Some _ -> customAttributes.Add(mkXmlDocCustomAttributeDataLazy xmlDocDelayedText) 
                  match obsoleteMessage with None -> () | Some s -> customAttributes.Add(mkObsoleteAttributeCustomAttributeData s) 
                  if hasParamArray then yield mkParamArrayCustomAttributeData()
                  yield! customAttributes |]

        member __.AddDefinitionLocation(line:int,column:int,filePath:string) = customAttributes.Add(mkDefinitionLocationAttributeCustomAttributeData(line, column, filePath))
        member __.AddObsolete(message : string, isError) = obsoleteMessage <- Some (message,isError)
        member __.HasParamArray with get() = hasParamArray and set(v) = hasParamArray <- v
        member __.AddXmlDocComputed xmlDocFunction = xmlDocAlwaysRecomputed <- Some xmlDocFunction
        member __.AddXmlDocDelayed xmlDocFunction = xmlDocDelayed <- Some xmlDocFunction
        member __.AddXmlDoc xmlDoc =  xmlDocDelayed <- Some (fun () -> xmlDoc)
        member __.HideObjectMethods with set v = hideObjectMethods <- v
        member __.NonNullable with set v = nonNullable <- v
        member __.AddCustomAttribute(attribute) = customAttributes.Add(attribute)
        member __.GetCustomAttributesData() = 
            [| yield! customAttributesOnce.Force()
               match xmlDocAlwaysRecomputed with None -> () | Some f -> customAttributes.Add(mkXmlDocCustomAttributeData (f()))  |]
            :> IList<_>


    let adjustTypeAttributes attributes isNested = 
        let visibilityAttributes = 
            match attributes &&& TypeAttributes.VisibilityMask with 
            | TypeAttributes.Public when isNested -> TypeAttributes.NestedPublic
            | TypeAttributes.NotPublic when isNested -> TypeAttributes.NestedAssembly
            | TypeAttributes.NestedPublic when not isNested -> TypeAttributes.Public
            | TypeAttributes.NestedAssembly 
            | TypeAttributes.NestedPrivate 
            | TypeAttributes.NestedFamORAssem
            | TypeAttributes.NestedFamily
            | TypeAttributes.NestedFamANDAssem when not isNested -> TypeAttributes.NotPublic
            | a -> a
        (attributes &&& ~~~TypeAttributes.VisibilityMask) ||| visibilityAttributes


        
type ProvidedStaticParameter(parameterName:string,parameterType:Type,?parameterDefaultValue:obj) = 
    inherit System.Reflection.ParameterInfo()

    let customAttributesImpl = CustomAttributesImpl()

    member __.AddXmlDocDelayed xmlDocFunction = customAttributesImpl.AddXmlDocDelayed xmlDocFunction
    member __.AddXmlDocComputed xmlDocFunction = customAttributesImpl.AddXmlDocComputed xmlDocFunction
    member __.AddXmlDoc xmlDoc = customAttributesImpl.AddXmlDoc xmlDoc

    override __.RawDefaultValue = defaultArg parameterDefaultValue null
    override __.Attributes = if parameterDefaultValue.IsNone then enum 0 else ParameterAttributes.Optional
    override __.Position = 0
    override __.ParameterType = parameterType
    override __.Name = parameterName 

    override __.GetCustomAttributes(_inherit) = ignore(_inherit); notRequired "GetCustomAttributes" parameterName
    override __.GetCustomAttributes(_attributeType, _inherit) = notRequired "GetCustomAttributes" parameterName

type ProvidedParameter(name:string,parameterType:Type,?isOut:bool,?optionalValue:obj) = 
    inherit System.Reflection.ParameterInfo()
    let customAttributesImpl = CustomAttributesImpl()
    let isOut = defaultArg isOut false
    member __.IsParamArray with get() = customAttributesImpl.HasParamArray and set(v) = customAttributesImpl.HasParamArray <- v
    override __.Name = name
    override __.ParameterType = parameterType
    override __.Attributes = (base.Attributes ||| (if isOut then ParameterAttributes.Out else enum 0)
                                              ||| (match optionalValue with None -> enum 0 | Some _ -> ParameterAttributes.Optional ||| ParameterAttributes.HasDefault))
    override __.RawDefaultValue = defaultArg optionalValue null
    member __.HasDefaultParameterValue = Option.isSome optionalValue
    member __.GetCustomAttributesDataImpl() = customAttributesImpl.GetCustomAttributesData()
#if FX_NO_CUSTOMATTRIBUTEDATA
#else
    override __.GetCustomAttributesData() = customAttributesImpl.GetCustomAttributesData()
#endif

type ProvidedConstructor(parameters : ProvidedParameter list) = 
    inherit ConstructorInfo()
    let parameters  = parameters |> List.map (fun p -> p :> ParameterInfo) 
    let mutable baseCall  = None

    let mutable declaringType = null : System.Type
    let mutable invokeCode    = None : option<Expr list -> Expr>
    let mutable isImplicitCtor  = false
    let mutable ctorAttributes = MethodAttributes.Public ||| MethodAttributes.RTSpecialName
    let nameText () = sprintf "constructor for %s" (if declaringType=null then "<not yet known type>" else declaringType.FullName)
    let isStatic() = ctorAttributes.HasFlag(MethodAttributes.Static)

    let customAttributesImpl = CustomAttributesImpl()
    member __.IsTypeInitializer 
        with get() = isStatic() && ctorAttributes.HasFlag(MethodAttributes.Private)
        and set(v) = 
            let typeInitializerAttributes = MethodAttributes.Static ||| MethodAttributes.Private
            ctorAttributes <- if v then ctorAttributes ||| typeInitializerAttributes else ctorAttributes &&& ~~~typeInitializerAttributes

    member __.AddXmlDocComputed xmlDocFunction            = customAttributesImpl.AddXmlDocComputed xmlDocFunction
    member __.AddXmlDocDelayed xmlDocFunction             = customAttributesImpl.AddXmlDocDelayed xmlDocFunction
    member __.AddXmlDoc xmlDoc                            = customAttributesImpl.AddXmlDoc xmlDoc
    member __.AddObsoleteAttribute (message,?isError)     = customAttributesImpl.AddObsolete (message,defaultArg isError false)
    member __.AddDefinitionLocation(line,column,filePath) = customAttributesImpl.AddDefinitionLocation(line, column, filePath)
    member __.GetCustomAttributesDataImpl() = customAttributesImpl.GetCustomAttributesData()
#if FX_NO_CUSTOMATTRIBUTEDATA
#else
    override __.GetCustomAttributesData()                 = customAttributesImpl.GetCustomAttributesData()
#endif

    member __.DeclaringTypeImpl 
        with set x = 
            if declaringType<>null then failwith (sprintf "ProvidedConstructor: declaringType already set on '%s'" (nameText())); 
            declaringType <- x

    member __.InvokeCode 
        with set (q:Expr list -> Expr) = 
            match invokeCode with
            | None -> invokeCode <- Some q
            | Some _ -> failwith (sprintf "ProvidedConstructor: code already given for '%s'" (nameText()))        

    member __.BaseConstructorCall
        with set (d:Expr list -> (ConstructorInfo * Expr list)) = 
            match baseCall with
            | None -> baseCall <- Some d
            | Some _ -> failwith (sprintf "ProvidedConstructor: base call already given for '%s'" (nameText()))        

    member __.GetInvokeCodeInternal isGenerated =
        match invokeCode with
        | Some f -> 
            // FSharp.Data change: use the real variable names instead of indices, to improve output of Debug.fs
            let paramNames = 
                parameters
                |> List.map (fun p -> p.Name) 
                |> List.append (if not isGenerated || isStatic() then [] else ["this"])
                |> Array.ofList
            QuotationSimplifier.transQuotationToCode isGenerated f paramNames
        | None -> failwith (sprintf "ProvidedConstructor: no invoker for '%s'" (nameText()))

    member __.GetBaseConstructorCallInternal isGenerated =
        match baseCall with
        | Some f -> Some(fun ctorArgs -> let c,baseCtorArgExprs = f ctorArgs in c, List.map (QuotationSimplifier.transExpr isGenerated) baseCtorArgExprs)
        | None -> None
    member __.IsImplicitCtor with get() = isImplicitCtor and set v = isImplicitCtor <- v

    // Implement overloads
    override __.GetParameters() = parameters |> List.toArray 
    override __.Attributes = ctorAttributes
    override __.Name = if isStatic() then ".cctor" else ".ctor"
    override __.DeclaringType = declaringType |> nonNull "ProvidedConstructor.DeclaringType"                                   
    override __.IsDefined(_attributeType, _inherit) = true 

    override __.Invoke(_invokeAttr, _binder, _parameters, _culture)      = notRequired "Invoke" (nameText())
    override __.Invoke(_obj, _invokeAttr, _binder, _parameters, _culture) = notRequired "Invoke" (nameText())
    override __.ReflectedType                                        = notRequired "ReflectedType" (nameText())
    override __.GetMethodImplementationFlags()                       = notRequired "GetMethodImplementationFlags" (nameText())
    override __.MethodHandle                                         = notRequired "MethodHandle" (nameText())
    override __.GetCustomAttributes(_inherit)                     = notRequired "GetCustomAttributes" (nameText())
    override __.GetCustomAttributes(_attributeType, _inherit)      = notRequired "GetCustomAttributes" (nameText())

type ProvidedMethod(methodName: string, parameters: ProvidedParameter list, returnType: Type) =
    inherit System.Reflection.MethodInfo()
    let argParams = parameters |> List.map (fun p -> p :> ParameterInfo) 

    // State
    let mutable declaringType : Type = null
    let mutable methodAttrs   = MethodAttributes.Public
    let mutable invokeCode    = None : option<Expr list -> Expr>
    let mutable staticParams = [ ] 
    let mutable staticParamsApply = None
    let isStatic() = methodAttrs.HasFlag(MethodAttributes.Static)
    let customAttributesImpl = CustomAttributesImpl()

    member __.AddXmlDocComputed xmlDocFunction            = customAttributesImpl.AddXmlDocComputed xmlDocFunction
    member __.AddXmlDocDelayed xmlDocFunction             = customAttributesImpl.AddXmlDocDelayed xmlDocFunction
    member __.AddXmlDoc xmlDoc                            = customAttributesImpl.AddXmlDoc xmlDoc
    member __.AddObsoleteAttribute (message,?isError)     = customAttributesImpl.AddObsolete (message,defaultArg isError false)
    member __.AddDefinitionLocation(line,column,filePath) = customAttributesImpl.AddDefinitionLocation(line, column, filePath)
    member __.AddCustomAttribute(attribute) = customAttributesImpl.AddCustomAttribute(attribute)
    member __.GetCustomAttributesDataImpl() = customAttributesImpl.GetCustomAttributesData()
#if FX_NO_CUSTOMATTRIBUTEDATA
#else
    override __.GetCustomAttributesData()                 = customAttributesImpl.GetCustomAttributesData()
#endif

    member __.SetMethodAttrs m = methodAttrs <- m 
    member __.AddMethodAttrs m = methodAttrs <- methodAttrs ||| m
    member __.DeclaringTypeImpl with set x = declaringType <- x // check: not set twice
    member __.IsStaticMethod 
        with get()  = isStatic()
        and set x = if x then methodAttrs <- methodAttrs ||| MethodAttributes.Static
                    else methodAttrs <- methodAttrs &&& (~~~ MethodAttributes.Static)

    member __.InvokeCode 
        with set  (q:Expr list -> Expr) = 
            match invokeCode with
            | None -> invokeCode <- Some q
            | Some _ -> failwith (sprintf "ProvidedConstructor: code already given for %s on type %s" methodName (if declaringType=null then "<not yet known type>" else declaringType.FullName))


    /// Abstract a type to a parametric-type. Requires "formal parameters" and "instantiation function".
    member __.DefineStaticParameters(staticParameters : list<ProvidedStaticParameter>, apply    : (string -> obj[] -> ProvidedMethod)) =
        staticParams      <- staticParameters 
        staticParamsApply <- Some apply

    /// Get ParameterInfo[] for the parametric type parameters (//s GetGenericParameters)
    member __.GetStaticParameters() = [| for p in staticParams -> p :> ParameterInfo |]

    /// Instantiate parametrics type
    member __.ApplyStaticArguments(mangledName:string, args:obj[]) =
        if staticParams.Length>0 then
            if staticParams.Length <> args.Length then
                failwith (sprintf "ProvidedTypeDefinition: expecting %d static parameters but given %d for method %s" staticParams.Length args.Length methodName)
            match staticParamsApply with
            | None -> failwith "ProvidedTypeDefinition: DefineStaticParameters was not called"
            | Some f -> f mangledName args
        else
            failwith (sprintf "ProvidedTypeDefinition: static parameters supplied but not expected for method %s" methodName)

    member __.GetInvokeCodeInternal isGenerated =
        match invokeCode with
        | Some f -> 
            // FSharp.Data change: use the real variable names instead of indices, to improve output of Debug.fs
            let paramNames = 
                parameters
                |> List.map (fun p -> p.Name) 
                |> List.append (if isStatic() then [] else ["this"])
                |> Array.ofList
            QuotationSimplifier.transQuotationToCode isGenerated f paramNames
        | None -> failwith (sprintf "ProvidedMethod: no invoker for %s on type %s" methodName (if declaringType=null then "<not yet known type>" else declaringType.FullName))

   // Implement overloads
    override __.GetParameters() = argParams |> Array.ofList
    override __.Attributes = methodAttrs
    override __.Name = methodName
    override __.DeclaringType = declaringType |> nonNull "ProvidedMethod.DeclaringType"                                   
    override __.IsDefined(_attributeType, _inherit) : bool = true
    override __.MemberType = MemberTypes.Method
    override __.CallingConvention = 
        let cc = CallingConventions.Standard
        let cc = if not (isStatic()) then cc ||| CallingConventions.HasThis else cc
        cc
    override __.ReturnType = returnType
    override __.ReturnParameter = null // REVIEW: Give it a name and type?
    override __.ToString() = "Method " + methodName
    
    // These don't have to return fully accurate results - they are used 
    // by the F# Quotations library function SpecificCall as a pre-optimization
    // when comparing methods
    override __.MetadataToken = hash declaringType + hash methodName
    override __.MethodHandle = RuntimeMethodHandle()

    override __.ReturnTypeCustomAttributes                           = notRequired "ReturnTypeCustomAttributes" methodName
    override __.GetBaseDefinition()                                  = notRequired "GetBaseDefinition" methodName
    override __.GetMethodImplementationFlags()                       = notRequired "GetMethodImplementationFlags" methodName
    override __.Invoke(_obj, _invokeAttr, _binder, _parameters, _culture) = notRequired "Invoke" methodName
    override __.ReflectedType                                        = notRequired "ReflectedType" methodName
    override __.GetCustomAttributes(_inherit)                        = notRequired "GetCustomAttributes" methodName
    override __.GetCustomAttributes(_attributeType, _inherit)        =  notRequired "GetCustomAttributes" methodName


type ProvidedProperty(propertyName: string, propertyType: Type, ?parameters: ProvidedParameter list) = 
    inherit System.Reflection.PropertyInfo()
    // State

    let parameters = defaultArg parameters []
    let mutable declaringType = null
    let mutable isStatic = false
    let mutable getterCode = None : option<Expr list -> Expr>
    let mutable setterCode = None : option<Expr list -> Expr>

    let hasGetter() = getterCode.IsSome
    let hasSetter() = setterCode.IsSome

    // Delay construction - to pick up the latest isStatic
    let markSpecialName (m:ProvidedMethod) = m.AddMethodAttrs(MethodAttributes.SpecialName); m
    let getter = lazy (ProvidedMethod("get_" + propertyName,parameters,propertyType,IsStaticMethod=isStatic,DeclaringTypeImpl=declaringType,InvokeCode=getterCode.Value) |> markSpecialName)  
    let setter = lazy (ProvidedMethod("set_" + propertyName,parameters @ [ProvidedParameter("value",propertyType)],typeof<System.Void>,IsStaticMethod=isStatic,DeclaringTypeImpl=declaringType,InvokeCode=setterCode.Value) |> markSpecialName) 
 
    let customAttributesImpl = CustomAttributesImpl()
    member __.AddXmlDocComputed xmlDocFunction            = customAttributesImpl.AddXmlDocComputed xmlDocFunction
    member __.AddXmlDocDelayed xmlDocFunction             = customAttributesImpl.AddXmlDocDelayed xmlDocFunction
    member __.AddXmlDoc xmlDoc                            = customAttributesImpl.AddXmlDoc xmlDoc
    member __.AddObsoleteAttribute (message,?isError)     = customAttributesImpl.AddObsolete (message,defaultArg isError false)
    member __.AddDefinitionLocation(line,column,filePath) = customAttributesImpl.AddDefinitionLocation(line, column, filePath)
    member __.GetCustomAttributesDataImpl() = customAttributesImpl.GetCustomAttributesData()
    member __.AddCustomAttribute attribute                = customAttributesImpl.AddCustomAttribute attribute
#if FX_NO_CUSTOMATTRIBUTEDATA
#else
    override __.GetCustomAttributesData()                 = customAttributesImpl.GetCustomAttributesData()
#endif

    member __.DeclaringTypeImpl with set x = declaringType <- x // check: not set twice

    member __.IsStatic 
        with get()  = isStatic
        and set x = isStatic <- x

    member __.GetterCode 
        with set  (q:Expr list -> Expr) = 
            if not getter.IsValueCreated then getterCode <- Some q else failwith "ProvidedProperty: getter MethodInfo has already been created"

    member __.SetterCode 
        with set (q:Expr list -> Expr) = 
            if not (setter.IsValueCreated) then setterCode <- Some q else failwith "ProvidedProperty: setter MethodInfo has already been created"

    // Implement overloads
    override __.PropertyType = propertyType
    override __.SetValue(_obj, _value, _invokeAttr, _binder, _index, _culture) = notRequired "SetValue" propertyName
    override __.GetAccessors _nonPublic  = notRequired "nonPublic" propertyName
    override __.GetGetMethod _nonPublic = if hasGetter() then getter.Force() :> MethodInfo else null
    override __.GetSetMethod _nonPublic = if hasSetter() then setter.Force() :> MethodInfo else null
    override __.GetIndexParameters() = [| for p in parameters -> upcast p |]
    override __.Attributes = PropertyAttributes.None
    override __.CanRead = hasGetter()
    override __.CanWrite = hasSetter()
    override __.GetValue(_obj, _invokeAttr, _binder, _index, _culture) : obj = notRequired "GetValue" propertyName
    override __.Name = propertyName
    override __.DeclaringType = declaringType |> nonNull "ProvidedProperty.DeclaringType"
    override __.MemberType : MemberTypes = MemberTypes.Property

    override __.ReflectedType                                     = notRequired "ReflectedType" propertyName
    override __.GetCustomAttributes(_inherit)                  = notRequired "GetCustomAttributes" propertyName
    override __.GetCustomAttributes(_attributeType, _inherit)   = notRequired "GetCustomAttributes" propertyName
    override __.IsDefined(_attributeType, _inherit)             = notRequired "IsDefined" propertyName

type ProvidedEvent(eventName:string,eventHandlerType:Type) = 
    inherit System.Reflection.EventInfo()
    // State

    let mutable declaringType = null
    let mutable isStatic = false
    let mutable adderCode = None : option<Expr list -> Expr>
    let mutable removerCode = None : option<Expr list -> Expr>

    // Delay construction - to pick up the latest isStatic
    let markSpecialName (m:ProvidedMethod) = m.AddMethodAttrs(MethodAttributes.SpecialName); m
    let adder = lazy (ProvidedMethod("add_" + eventName, [ProvidedParameter("handler", eventHandlerType)],typeof<System.Void>,IsStaticMethod=isStatic,DeclaringTypeImpl=declaringType,InvokeCode=adderCode.Value) |> markSpecialName)  
    let remover = lazy (ProvidedMethod("remove_" + eventName, [ProvidedParameter("handler", eventHandlerType)],typeof<System.Void>,IsStaticMethod=isStatic,DeclaringTypeImpl=declaringType,InvokeCode=removerCode.Value) |> markSpecialName) 
 
    let customAttributesImpl = CustomAttributesImpl()
    member __.AddXmlDocComputed xmlDocFunction            = customAttributesImpl.AddXmlDocComputed xmlDocFunction
    member __.AddXmlDocDelayed xmlDocFunction             = customAttributesImpl.AddXmlDocDelayed xmlDocFunction
    member __.AddXmlDoc xmlDoc                            = customAttributesImpl.AddXmlDoc xmlDoc
    member __.AddDefinitionLocation(line,column,filePath) = customAttributesImpl.AddDefinitionLocation(line, column, filePath)
    member __.GetCustomAttributesDataImpl() = customAttributesImpl.GetCustomAttributesData()
#if FX_NO_CUSTOMATTRIBUTEDATA
#else
    override __.GetCustomAttributesData()                 = customAttributesImpl.GetCustomAttributesData()
#endif

    member __.DeclaringTypeImpl with set x = declaringType <- x // check: not set twice
    member __.IsStatic 
        with get()  = isStatic
        and set x = isStatic <- x

    member __.AdderCode 
        with get() = adderCode.Value
        and  set f = 
            if not adder.IsValueCreated then adderCode <- Some f else failwith "ProvidedEvent: Add MethodInfo has already been created"                                         

    member __.RemoverCode
        with get() = removerCode.Value
        and  set f = 
            if not (remover.IsValueCreated) then removerCode <- Some f else failwith "ProvidedEvent: Remove MethodInfo has already been created"

    // Implement overloads
    override __.EventHandlerType = eventHandlerType
    override __.GetAddMethod _nonPublic = adder.Force() :> MethodInfo
    override __.GetRemoveMethod _nonPublic = remover.Force() :> MethodInfo
    override __.Attributes = EventAttributes.None
    override __.Name = eventName
    override __.DeclaringType = declaringType |> nonNull "ProvidedEvent.DeclaringType"
    override __.MemberType : MemberTypes = MemberTypes.Event

    override __.GetRaiseMethod _nonPublic                      = notRequired "GetRaiseMethod" eventName
    override __.ReflectedType                                  = notRequired "ReflectedType" eventName
    override __.GetCustomAttributes(_inherit)                  = notRequired "GetCustomAttributes" eventName
    override __.GetCustomAttributes(_attributeType, _inherit)  = notRequired "GetCustomAttributes" eventName
    override __.IsDefined(_attributeType, _inherit)            = notRequired "IsDefined" eventName

type ProvidedLiteralField(fieldName:string,fieldType:Type,literalValue:obj) = 
    inherit System.Reflection.FieldInfo()
    // State

    let mutable declaringType = null

    let customAttributesImpl = CustomAttributesImpl()
    member __.AddXmlDocComputed xmlDocFunction            = customAttributesImpl.AddXmlDocComputed xmlDocFunction
    member __.AddXmlDocDelayed xmlDocFunction             = customAttributesImpl.AddXmlDocDelayed xmlDocFunction
    member __.AddXmlDoc xmlDoc                            = customAttributesImpl.AddXmlDoc xmlDoc
    member __.AddObsoleteAttribute (message,?isError)     = customAttributesImpl.AddObsolete (message,defaultArg isError false)
    member __.AddDefinitionLocation(line,column,filePath) = customAttributesImpl.AddDefinitionLocation(line, column, filePath)
    member __.GetCustomAttributesDataImpl() = customAttributesImpl.GetCustomAttributesData()
#if FX_NO_CUSTOMATTRIBUTEDATA
#else
    override __.GetCustomAttributesData()                 = customAttributesImpl.GetCustomAttributesData()
#endif

    member __.DeclaringTypeImpl with set x = declaringType <- x // check: not set twice


    // Implement overloads
    override __.FieldType = fieldType
    override __.GetRawConstantValue()  = literalValue
    override __.Attributes = FieldAttributes.Static ||| FieldAttributes.Literal ||| FieldAttributes.Public
    override __.Name = fieldName
    override __.DeclaringType = declaringType |> nonNull "ProvidedLiteralField.DeclaringType"
    override __.MemberType : MemberTypes = MemberTypes.Field

    override __.ReflectedType                                     = notRequired "ReflectedType" fieldName
    override __.GetCustomAttributes(_inherit)                  = notRequired "GetCustomAttributes" fieldName
    override __.GetCustomAttributes(_attributeType, _inherit)   = notRequired "GetCustomAttributes" fieldName
    override __.IsDefined(_attributeType, _inherit)             = notRequired "IsDefined" fieldName

    override __.SetValue(_obj, _value, _invokeAttr, _binder, _culture) = notRequired "SetValue" fieldName
    override __.GetValue(_obj) : obj = notRequired "GetValue" fieldName
    override __.FieldHandle = notRequired "FieldHandle" fieldName

type ProvidedField(fieldName:string,fieldType:Type) = 
    inherit System.Reflection.FieldInfo()
    // State

    let mutable declaringType = null

    let customAttributesImpl = CustomAttributesImpl()
    let mutable fieldAttrs = FieldAttributes.Private
    member __.AddXmlDocComputed xmlDocFunction            = customAttributesImpl.AddXmlDocComputed xmlDocFunction
    member __.AddXmlDocDelayed xmlDocFunction             = customAttributesImpl.AddXmlDocDelayed xmlDocFunction
    member __.AddXmlDoc xmlDoc                            = customAttributesImpl.AddXmlDoc xmlDoc
    member __.AddObsoleteAttribute (message,?isError)     = customAttributesImpl.AddObsolete (message,defaultArg isError false)
    member __.AddDefinitionLocation(line,column,filePath) = customAttributesImpl.AddDefinitionLocation(line, column, filePath)
    member __.GetCustomAttributesDataImpl() = customAttributesImpl.GetCustomAttributesData()
#if FX_NO_CUSTOMATTRIBUTEDATA
#else
    override __.GetCustomAttributesData()                 = customAttributesImpl.GetCustomAttributesData()
#endif

    member __.DeclaringTypeImpl with set x = declaringType <- x // check: not set twice

    member __.SetFieldAttributes attrs = fieldAttrs <- attrs
    // Implement overloads
    override __.FieldType = fieldType
    override __.GetRawConstantValue()  = null
    override __.Attributes = fieldAttrs
    override __.Name = fieldName
    override __.DeclaringType = declaringType |> nonNull "ProvidedField.DeclaringType"
    override __.MemberType : MemberTypes = MemberTypes.Field

    override __.ReflectedType                                     = notRequired "ReflectedType" fieldName
    override __.GetCustomAttributes(_inherit)                  = notRequired "GetCustomAttributes" fieldName
    override __.GetCustomAttributes(_attributeType, _inherit)   = notRequired "GetCustomAttributes" fieldName
    override __.IsDefined(_attributeType, _inherit)             = notRequired "IsDefined" fieldName

    override __.SetValue(_obj, _value, _invokeAttr, _binder, _culture) = notRequired "SetValue" fieldName
    override __.GetValue(_obj) : obj = notRequired "GetValue" fieldName
    override __.FieldHandle = notRequired "FieldHandle" fieldName

/// Represents the type constructor in a provided symbol type.
[<NoComparison>]
type ProvidedSymbolKind = 
    | SDArray 
    | Array of int 
    | Pointer 
    | ByRef 
    | Generic of System.Type 
    | FSharpTypeAbbreviation of (System.Reflection.Assembly * string * string[])


/// Represents an array or other symbolic type involving a provided type as the argument.
/// See the type provider spec for the methods that must be implemented.
/// Note that the type provider specification does not require us to implement pointer-equality for provided types.
type ProvidedSymbolType(kind: ProvidedSymbolKind, args: Type list) =
    inherit Type()

    let rec isEquivalentTo (thisTy: Type) (otherTy: Type) =
        match thisTy, otherTy with
        | (:? ProvidedSymbolType as thisTy), (:? ProvidedSymbolType as thatTy) -> (thisTy.Kind,thisTy.Args) = (thatTy.Kind, thatTy.Args)
        | (:? ProvidedSymbolType as thisTy), otherTy | otherTy, (:? ProvidedSymbolType as thisTy) ->
            match thisTy.Kind, thisTy.Args with
            | ProvidedSymbolKind.SDArray, [ty] | ProvidedSymbolKind.Array _, [ty] when otherTy.IsArray-> ty.Equals(otherTy.GetElementType())
            | ProvidedSymbolKind.ByRef, [ty] when otherTy.IsByRef -> ty.Equals(otherTy.GetElementType())
            | ProvidedSymbolKind.Pointer, [ty] when otherTy.IsPointer -> ty.Equals(otherTy.GetElementType())
            | ProvidedSymbolKind.Generic baseTy, args -> otherTy.IsGenericType && isEquivalentTo baseTy (otherTy.GetGenericTypeDefinition()) && Seq.forall2 isEquivalentTo args (otherTy.GetGenericArguments())
            | _ -> false
        | a, b -> a.Equals b

    let nameText() = 
        match kind,args with 
        | ProvidedSymbolKind.SDArray,[arg] -> arg.Name + "[]" 
        | ProvidedSymbolKind.Array _,[arg] -> arg.Name + "[*]" 
        | ProvidedSymbolKind.Pointer,[arg] -> arg.Name + "*" 
        | ProvidedSymbolKind.ByRef,[arg] -> arg.Name + "&"
        | ProvidedSymbolKind.Generic gty, args -> gty.Name + (sprintf "%A" args)
        | ProvidedSymbolKind.FSharpTypeAbbreviation (_,_,path),_ -> path.[path.Length-1]
        | _ -> failwith "unreachable"

    static member convType (parameters: Type list) (ty:Type) = 
        if ty = null then null
        elif ty.IsGenericType then
            let args = Array.map (ProvidedSymbolType.convType parameters) (ty.GetGenericArguments())
            ProvidedSymbolType(Generic (ty.GetGenericTypeDefinition()), Array.toList args)  :> Type
        elif ty.HasElementType then 
            let ety = ProvidedSymbolType.convType parameters (ty.GetElementType()) 
            if ty.IsArray then 
                let rank = ty.GetArrayRank()
                if rank = 1 then ProvidedSymbolType(SDArray,[ety]) :> Type
                else ProvidedSymbolType(Array rank,[ety]) :> Type
            elif ty.IsPointer then ProvidedSymbolType(Pointer,[ety]) :> Type
            elif ty.IsByRef then ProvidedSymbolType(ByRef,[ety]) :> Type
            else ty
        elif ty.IsGenericParameter then 
            if ty.GenericParameterPosition <= parameters.Length - 1 then 
                parameters.[ty.GenericParameterPosition]
            else
                ty
        else ty

    override __.FullName =   
        match kind,args with 
        | ProvidedSymbolKind.SDArray,[arg] -> arg.FullName + "[]" 
        | ProvidedSymbolKind.Array _,[arg] -> arg.FullName + "[*]" 
        | ProvidedSymbolKind.Pointer,[arg] -> arg.FullName + "*" 
        | ProvidedSymbolKind.ByRef,[arg] -> arg.FullName + "&"
        | ProvidedSymbolKind.Generic gty, args -> gty.FullName + "[" + (args |> List.map (fun arg -> arg.ToString()) |> String.concat ",") + "]"
        | ProvidedSymbolKind.FSharpTypeAbbreviation (_,nsp,path),args -> String.concat "." (Array.append [| nsp |] path) + (match args with [] -> "" | _ -> args.ToString())
        | _ -> failwith "unreachable"
   
    /// Although not strictly required by the type provider specification, this is required when doing basic operations like FullName on
    /// .NET symbolic types made from this type, e.g. when building Nullable<SomeProvidedType[]>.FullName
    override __.DeclaringType =                                                                 
        match kind,args with 
        | ProvidedSymbolKind.SDArray,[arg] -> arg
        | ProvidedSymbolKind.Array _,[arg] -> arg
        | ProvidedSymbolKind.Pointer,[arg] -> arg
        | ProvidedSymbolKind.ByRef,[arg] -> arg
        | ProvidedSymbolKind.Generic gty,_ -> gty
        | ProvidedSymbolKind.FSharpTypeAbbreviation _,_ -> null
        | _ -> failwith "unreachable"

    override __.IsAssignableFrom(otherTy) = 
        match kind with
        | Generic gtd ->
            if otherTy.IsGenericType then
                let otherGtd = otherTy.GetGenericTypeDefinition()
                let otherArgs = otherTy.GetGenericArguments()
                let yes = gtd.Equals(otherGtd) && Seq.forall2 isEquivalentTo args otherArgs
                yes
                else
                    base.IsAssignableFrom(otherTy)
        | _ -> base.IsAssignableFrom(otherTy)

    override __.Name = nameText()

    override __.BaseType =
        match kind with 
        | ProvidedSymbolKind.SDArray -> typeof<System.Array>
        | ProvidedSymbolKind.Array _ -> typeof<System.Array>
        | ProvidedSymbolKind.Pointer -> typeof<System.ValueType>
        | ProvidedSymbolKind.ByRef -> typeof<System.ValueType>
        | ProvidedSymbolKind.Generic gty  ->
            if gty.BaseType = null then null else
            ProvidedSymbolType.convType args gty.BaseType
        | ProvidedSymbolKind.FSharpTypeAbbreviation _ -> typeof<obj>

    override __.GetArrayRank() = (match kind with ProvidedSymbolKind.Array n -> n | ProvidedSymbolKind.SDArray -> 1 | _ -> invalidOp "non-array type")
    override __.IsValueTypeImpl() = (match kind with ProvidedSymbolKind.Generic gtd -> gtd.IsValueType | _ -> false)
    override __.IsArrayImpl() = (match kind with ProvidedSymbolKind.Array _ | ProvidedSymbolKind.SDArray -> true | _ -> false)
    override __.IsByRefImpl() = (match kind with ProvidedSymbolKind.ByRef _ -> true | _ -> false)
    override __.IsPointerImpl() = (match kind with ProvidedSymbolKind.Pointer _ -> true | _ -> false)
    override __.IsPrimitiveImpl() = false
    override __.IsGenericType = (match kind with ProvidedSymbolKind.Generic _ -> true | _ -> false)
    override __.GetGenericArguments() = (match kind with ProvidedSymbolKind.Generic _ -> args |> List.toArray | _ -> invalidOp "non-generic type")
    override __.GetGenericTypeDefinition() = (match kind with ProvidedSymbolKind.Generic e -> e | _ -> invalidOp "non-generic type")
    override __.IsCOMObjectImpl() = false
    override __.HasElementTypeImpl() = (match kind with ProvidedSymbolKind.Generic _ -> false | _ -> true)
    override __.GetElementType() = (match kind,args with (ProvidedSymbolKind.Array _  | ProvidedSymbolKind.SDArray | ProvidedSymbolKind.ByRef | ProvidedSymbolKind.Pointer),[e] -> e | _ -> invalidOp "not an array, pointer or byref type")
    override this.ToString() = this.FullName

    override __.Assembly = 
        match kind with 
        | ProvidedSymbolKind.FSharpTypeAbbreviation (assembly,_nsp,_path) -> assembly
        | ProvidedSymbolKind.Generic gty -> gty.Assembly
        | _ -> notRequired "Assembly" (nameText())

    override __.Namespace = 
        match kind with 
        | ProvidedSymbolKind.FSharpTypeAbbreviation (_assembly,nsp,_path) -> nsp
        | _ -> notRequired "Namespace" (nameText())

    override __.GetHashCode()                                                                    = 
        match kind,args with 
        | ProvidedSymbolKind.SDArray,[arg] -> 10 + hash arg
        | ProvidedSymbolKind.Array _,[arg] -> 163 + hash arg
        | ProvidedSymbolKind.Pointer,[arg] -> 283 + hash arg
        | ProvidedSymbolKind.ByRef,[arg] -> 43904 + hash arg
        | ProvidedSymbolKind.Generic gty,_ -> 9797 + hash gty + List.sumBy hash args
        | ProvidedSymbolKind.FSharpTypeAbbreviation _,_ -> 3092
        | _ -> failwith "unreachable"
    
    override __.Equals(other: obj) =
        match other with
        | :? ProvidedSymbolType as otherTy -> (kind, args) = (otherTy.Kind, otherTy.Args)
        | _ -> false

    member __.Kind = kind
    member __.Args = args
    
    member __.IsFSharpTypeAbbreviation  = match kind with FSharpTypeAbbreviation _ -> true | _ -> false
    // For example, int<kg>
    member __.IsFSharpUnitAnnotated = match kind with ProvidedSymbolKind.Generic gtd -> not gtd.IsGenericTypeDefinition | _ -> false

    override __.Module : Module                                                                   = notRequired "Module" (nameText())
    override __.GetConstructors _bindingAttr                                                      = notRequired "GetConstructors" (nameText())
    override __.GetMethodImpl(_name, _bindingAttr, _binderBinder, _callConvention, _types, _modifiers) = 
        match kind with
        | Generic gtd -> 
            let ty = gtd.GetGenericTypeDefinition().MakeGenericType(Array.ofList args)
            ty.GetMethod(_name, _bindingAttr)
        | _ -> notRequired "GetMethodImpl" (nameText())
    override __.GetMembers _bindingAttr                                                           = notRequired "GetMembers" (nameText())
    override __.GetMethods _bindingAttr                                                           = notRequired "GetMethods" (nameText())
    override __.GetField(_name, _bindingAttr)                                                     = notRequired "GetField" (nameText())
    override __.GetFields _bindingAttr                                                            = notRequired "GetFields" (nameText())
    override __.GetInterface(_name, _ignoreCase)                                                  = notRequired "GetInterface" (nameText())
    override __.GetInterfaces()                                                                   = notRequired "GetInterfaces" (nameText())
    override __.GetEvent(_name, _bindingAttr)                                                     = notRequired "GetEvent" (nameText())
    override __.GetEvents _bindingAttr                                                            = notRequired "GetEvents" (nameText())
    override __.GetProperties _bindingAttr                                                        = notRequired "GetProperties" (nameText())
    override __.GetPropertyImpl(_name, _bindingAttr, _binder, _returnType, _types, _modifiers)    = notRequired "GetPropertyImpl" (nameText())
    override __.GetNestedTypes _bindingAttr                                                       = notRequired "GetNestedTypes" (nameText())
    override __.GetNestedType(_name, _bindingAttr)                                                = notRequired "GetNestedType" (nameText())
    override __.GetAttributeFlagsImpl()                                                           = notRequired "GetAttributeFlagsImpl" (nameText())
    override this.UnderlyingSystemType = 
        match kind with 
        | ProvidedSymbolKind.SDArray
        | ProvidedSymbolKind.Array _
        | ProvidedSymbolKind.Pointer
        | ProvidedSymbolKind.FSharpTypeAbbreviation _
        | ProvidedSymbolKind.ByRef -> upcast this
        | ProvidedSymbolKind.Generic gty -> gty.UnderlyingSystemType  
#if FX_NO_CUSTOMATTRIBUTEDATA
#else
    override __.GetCustomAttributesData()                                                        =  ([| |] :> IList<_>)
#endif
    override __.MemberType                                                                       = notRequired "MemberType" (nameText())
    override __.GetMember(_name,_mt,_bindingAttr)                                                = notRequired "GetMember" (nameText())
    override __.GUID                                                                             = notRequired "GUID" (nameText())
    override __.InvokeMember(_name, _invokeAttr, _binder, _target, _args, _modifiers, _culture, _namedParameters) = notRequired "InvokeMember" (nameText())
    override __.AssemblyQualifiedName                                                            = notRequired "AssemblyQualifiedName" (nameText())
    override __.GetConstructorImpl(_bindingAttr, _binder, _callConvention, _types, _modifiers)   = notRequired "GetConstructorImpl" (nameText())
    override __.GetCustomAttributes(_inherit)                                                    = [| |]
    override __.GetCustomAttributes(_attributeType, _inherit)                                    = [| |]
    override __.IsDefined(_attributeType, _inherit)                                              = false
    // FSharp.Data addition: this was added to support arrays of arrays
    override this.MakeArrayType() = ProvidedSymbolType(ProvidedSymbolKind.SDArray, [this]) :> Type
    override this.MakeArrayType arg = ProvidedSymbolType(ProvidedSymbolKind.Array arg, [this]) :> Type

type ProvidedSymbolMethod(genericMethodDefinition: MethodInfo, parameters: Type list) =
    inherit System.Reflection.MethodInfo()

    let convParam (p:ParameterInfo) = 
        { new System.Reflection.ParameterInfo() with
              override __.Name = p.Name
              override __.ParameterType = ProvidedSymbolType.convType parameters p.ParameterType
              override __.Attributes = p.Attributes
              override __.RawDefaultValue = p.RawDefaultValue
#if FX_NO_CUSTOMATTRIBUTEDATA
#else
              override __.GetCustomAttributesData() = p.GetCustomAttributesData()
#endif
        } 

    override this.IsGenericMethod = 
        (if this.DeclaringType.IsGenericType then this.DeclaringType.GetGenericArguments().Length else 0) < parameters.Length

    override this.GetGenericArguments() = 
        Seq.skip (if this.DeclaringType.IsGenericType then this.DeclaringType.GetGenericArguments().Length else 0) parameters |> Seq.toArray 

    override __.GetGenericMethodDefinition() = genericMethodDefinition

    override __.DeclaringType = ProvidedSymbolType.convType parameters genericMethodDefinition.DeclaringType
    override __.ToString() = "Method " + genericMethodDefinition.Name
    override __.Name = genericMethodDefinition.Name
    override __.MetadataToken = genericMethodDefinition.MetadataToken
    override __.Attributes = genericMethodDefinition.Attributes
    override __.CallingConvention = genericMethodDefinition.CallingConvention
    override __.MemberType = genericMethodDefinition.MemberType

    override __.IsDefined(_attributeType, _inherit) : bool = notRequired "IsDefined" genericMethodDefinition.Name
    override __.ReturnType = ProvidedSymbolType.convType parameters genericMethodDefinition.ReturnType
    override __.GetParameters() = genericMethodDefinition.GetParameters() |> Array.map convParam
    override __.ReturnParameter = genericMethodDefinition.ReturnParameter |> convParam
    override __.ReturnTypeCustomAttributes                           = notRequired "ReturnTypeCustomAttributes" genericMethodDefinition.Name
    override __.GetBaseDefinition()                                  = notRequired "GetBaseDefinition" genericMethodDefinition.Name
    override __.GetMethodImplementationFlags()                       = notRequired "GetMethodImplementationFlags" genericMethodDefinition.Name
    override __.MethodHandle                                         = notRequired "MethodHandle" genericMethodDefinition.Name
    override __.Invoke(_obj, _invokeAttr, _binder, _parameters, _culture) = notRequired "Invoke" genericMethodDefinition.Name
    override __.ReflectedType                                        = notRequired "ReflectedType" genericMethodDefinition.Name
    override __.GetCustomAttributes(_inherit)                     = notRequired "GetCustomAttributes" genericMethodDefinition.Name
    override __.GetCustomAttributes(_attributeType, _inherit)      =  notRequired "GetCustomAttributes" genericMethodDefinition.Name 



type ProvidedTypeBuilder() =
    static member MakeGenericType(genericTypeDefinition, genericArguments) = ProvidedSymbolType(Generic genericTypeDefinition, genericArguments) :> Type
    static member MakeGenericMethod(genericMethodDefinition, genericArguments) = ProvidedSymbolMethod(genericMethodDefinition, genericArguments) :> MethodInfo

[<Class>]
type ProvidedMeasureBuilder() =

    // TODO: this shouldn't be hardcoded, but without creating a dependency on FSharp.Compiler.Service
    // there seems to be no way to check if a type abbreviation exists
    let unitNamesTypeAbbreviations = 
        [ "meter"; "hertz"; "newton"; "pascal"; "joule"; "watt"; "coulomb"; 
          "volt"; "farad"; "ohm"; "siemens"; "weber"; "tesla"; "henry"
          "lumen"; "lux"; "becquerel"; "gray"; "sievert"; "katal" ]
        |> Set.ofList

    let unitSymbolsTypeAbbreviations = 
        [ "m"; "kg"; "s"; "A"; "K"; "mol"; "cd"; "Hz"; "N"; "Pa"; "J"; "W"; "C"
          "V"; "F"; "S"; "Wb"; "T"; "lm"; "lx"; "Bq"; "Gy"; "Sv"; "kat"; "H" ]
        |> Set.ofList

    static let theBuilder = ProvidedMeasureBuilder()
    static member Default = theBuilder
    member __.One = typeof<Core.CompilerServices.MeasureOne> 
    member __.Product (m1,m2) = typedefof<Core.CompilerServices.MeasureProduct<_,_>>.MakeGenericType [| m1;m2 |] 
    member __.Inverse m = typedefof<Core.CompilerServices.MeasureInverse<_>>.MakeGenericType [| m |] 
    member b.Ratio (m1, m2) = b.Product(m1, b.Inverse m2)
    member b.Square m = b.Product(m, m)

    // FSharp.Data change: if the unit is not a valid type, instead 
    // of assuming it's a type abbreviation, which may not be the case and cause a
    // problem later on, check the list of valid abbreviations
    member __.SI (m:string) = 
        let mLowerCase = m.ToLowerInvariant()
        let abbreviation =            
            if unitNamesTypeAbbreviations.Contains mLowerCase then
                Some ("Microsoft.FSharp.Data.UnitSystems.SI.UnitNames", mLowerCase)
            elif unitSymbolsTypeAbbreviations.Contains m then
                Some ("Microsoft.FSharp.Data.UnitSystems.SI.UnitSymbols", m)
            else
                None
        match abbreviation with
        | Some (ns, unitName) ->
            ProvidedSymbolType
               (ProvidedSymbolKind.FSharpTypeAbbreviation
                   (typeof<Core.CompilerServices.MeasureOne>.Assembly,
                    ns,
                    [| unitName |]), 
                []) :> Type
        | None ->
            typedefof<list<int>>.Assembly.GetType("Microsoft.FSharp.Data.UnitSystems.SI.UnitNames." + mLowerCase)

    member __.AnnotateType (basicType, annotation) = ProvidedSymbolType(Generic basicType, annotation) :> Type



[<RequireQualifiedAccess; NoComparison>]
type TypeContainer =
  | Namespace of Assembly * string // namespace
  | Type of System.Type
  | TypeToBeDecided

module GlobalProvidedAssemblyElementsTable = 
    let theTable = Dictionary<Assembly, Lazy<byte[]>>()

type ProvidedTypeDefinition(container:TypeContainer,className : string, baseType  : Type option) as this =
    inherit Type()

    do match container, !ProvidedTypeDefinition.Logger with
       | TypeContainer.Namespace _, Some logger -> logger (sprintf "Creating ProvidedTypeDefinition %s [%d]" className (System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode this))
       | _ -> ()

    // state
    let mutable attributes   = 
        TypeAttributes.Public ||| 
        TypeAttributes.Class ||| 
        TypeAttributes.Sealed |||
        enum (int32 TypeProviderTypeAttributes.IsErased)


    let mutable enumUnderlyingType = typeof<int>
    let mutable baseType   =  lazy baseType
    let mutable membersKnown   = ResizeArray<MemberInfo>()
    let mutable membersQueue   = ResizeArray<(unit -> list<MemberInfo>)>()       
    let mutable staticParams = [ ] 
    let mutable staticParamsApply = None
    let mutable container = container
    let mutable interfaceImpls = ResizeArray<Type>()
    let mutable interfaceImplsDelayed = ResizeArray<unit -> list<Type>>()
    let mutable methodOverrides = ResizeArray<ProvidedMethod * MethodInfo>()

    // members API
    let getMembers() = 
        if membersQueue.Count > 0 then 
            let elems = membersQueue |> Seq.toArray // take a copy in case more elements get added
            membersQueue.Clear()
            for  f in elems do
                for i in f() do 
                    membersKnown.Add i       
                    match i with
                    | :? ProvidedProperty    as p -> 
                        if p.CanRead then membersKnown.Add (p.GetGetMethod true)
                        if p.CanWrite then membersKnown.Add (p.GetSetMethod true)
                    | :? ProvidedEvent       as e -> 
                        membersKnown.Add (e.GetAddMethod true)
                        membersKnown.Add (e.GetRemoveMethod true)
                    | _ -> ()
        
        membersKnown.ToArray()

            // members API
    let getInterfaces() = 
        if interfaceImplsDelayed.Count > 0 then 
            let elems = interfaceImplsDelayed |> Seq.toArray // take a copy in case more elements get added
            interfaceImplsDelayed.Clear()
            for  f in elems do
                for i in f() do 
                    interfaceImpls.Add i       
        
        interfaceImpls.ToArray()

    let mutable theAssembly = 
      lazy
        match container with
        | TypeContainer.Namespace (theAssembly, rootNamespace) ->
            if theAssembly = null then failwith "Null assemblies not allowed"
            if rootNamespace<>null && rootNamespace.Length=0 then failwith "Use 'null' for global namespace"
            theAssembly
        | TypeContainer.Type superTy -> superTy.Assembly
        | TypeContainer.TypeToBeDecided -> failwith (sprintf "type '%s' was not added as a member to a declaring type" className)
    
    let rootNamespace =
      lazy 
        match container with
        | TypeContainer.Namespace (_,rootNamespace) -> rootNamespace
        | TypeContainer.Type enclosingTyp           -> enclosingTyp.Namespace
        | TypeContainer.TypeToBeDecided -> failwith (sprintf "type '%s' was not added as a member to a declaring type" className)

    let declaringType =
      lazy
        match container with
        | TypeContainer.Namespace _ -> null
        | TypeContainer.Type enclosingTyp           -> enclosingTyp
        | TypeContainer.TypeToBeDecided -> failwith (sprintf "type '%s' was not added as a member to a declaring type" className)

    let fullName = 
      lazy
        match container with
        | TypeContainer.Type declaringType -> declaringType.FullName + "+" + className
        | TypeContainer.Namespace (_,namespaceName) ->  
            if namespaceName="" then failwith "use null for global namespace"
            match namespaceName with
            | null -> className
            | _    -> namespaceName + "." + className
        | TypeContainer.TypeToBeDecided -> failwith (sprintf "type '%s' was not added as a member to a declaring type" className)
                                                            
    let patchUpAddedMemberInfo (this:Type) (m:MemberInfo) = 
        match m with
        | :? ProvidedConstructor as c -> c.DeclaringTypeImpl <- this // patch up "declaring type" on provided MethodInfo
        | :? ProvidedMethod      as m -> m.DeclaringTypeImpl <- this // patch up "declaring type" on provided MethodInfo
        | :? ProvidedProperty    as p -> p.DeclaringTypeImpl <- this // patch up "declaring type" on provided MethodInfo
        | :? ProvidedEvent       as e -> e.DeclaringTypeImpl <- this // patch up "declaring type" on provided MethodInfo
        | :? ProvidedTypeDefinition  as t -> t.DeclaringTypeImpl <- this 
        | :? ProvidedLiteralField as l -> l.DeclaringTypeImpl <- this
        | :? ProvidedField as l -> l.DeclaringTypeImpl <- this
        | _ -> ()

    let customAttributesImpl = CustomAttributesImpl()

    member __.AddXmlDocComputed xmlDocFunction            = customAttributesImpl.AddXmlDocComputed xmlDocFunction
    member __.AddXmlDocDelayed xmlDocFunction             = customAttributesImpl.AddXmlDocDelayed xmlDocFunction
    member __.AddXmlDoc xmlDoc                            = customAttributesImpl.AddXmlDoc xmlDoc
    member __.AddObsoleteAttribute (message,?isError)     = customAttributesImpl.AddObsolete (message,defaultArg isError false)
    member __.AddDefinitionLocation(line,column,filePath) = customAttributesImpl.AddDefinitionLocation(line, column, filePath)
    member __.HideObjectMethods with set v                = customAttributesImpl.HideObjectMethods <- v
    member __.NonNullable with set v                      = customAttributesImpl.NonNullable <- v
    member __.GetCustomAttributesDataImpl() = customAttributesImpl.GetCustomAttributesData()
    member __.AddCustomAttribute attribute                = customAttributesImpl.AddCustomAttribute attribute
#if FX_NO_CUSTOMATTRIBUTEDATA
#else
    override __.GetCustomAttributesData()                 = customAttributesImpl.GetCustomAttributesData()
#endif

    member __.ResetEnclosingType (ty) = 
        container <- TypeContainer.Type ty
    new (assembly:Assembly,namespaceName,className,baseType) = new ProvidedTypeDefinition(TypeContainer.Namespace (assembly,namespaceName), className, baseType)
    new (className,baseType) = new ProvidedTypeDefinition(TypeContainer.TypeToBeDecided, className, baseType)
    // state ops

    override __.UnderlyingSystemType = typeof<Type>

    member __.SetEnumUnderlyingType(ty) = enumUnderlyingType <- ty

    override __.GetEnumUnderlyingType() = if this.IsEnum then enumUnderlyingType else invalidOp "not enum type"

    member __.SetBaseType t = baseType <- lazy Some t

    member __.SetBaseTypeDelayed baseTypeFunction = baseType <- lazy (Some (baseTypeFunction()))

    member __.SetAttributes x = attributes <- x

    // Add MemberInfos
    member __.AddMembersDelayed(membersFunction : unit -> list<#MemberInfo>) =
        membersQueue.Add (fun () -> membersFunction() |> List.map (fun x -> patchUpAddedMemberInfo this x; x :> MemberInfo ))

    member __.AddMembers(memberInfos:list<#MemberInfo>) = (* strict *)
        memberInfos |> List.iter (patchUpAddedMemberInfo this) // strict: patch up now
        membersQueue.Add (fun () -> memberInfos |> List.map (fun x -> x :> MemberInfo))

    member __.AddMember(memberInfo:MemberInfo) = 
        this.AddMembers [memberInfo]    

    member __.AddMemberDelayed(memberFunction : unit -> #MemberInfo) = 
        this.AddMembersDelayed(fun () -> [memberFunction()])

    member __.AddAssemblyTypesAsNestedTypesDelayed (assemblyf : unit -> System.Reflection.Assembly)  = 
        let bucketByPath nodef tipf (items: (string list * 'Value) list) = 
            // Find all the items with an empty key list and call 'tipf' 
            let tips = 
                [ for (keylist,v) in items do 
                        match keylist with 
                        | [] -> yield tipf v
                        | _ -> () ]

            // Find all the items with a non-empty key list. Bucket them together by
            // the first key. For each bucket, call 'nodef' on that head key and the bucket.
            let nodes = 
                let buckets = new Dictionary<_,_>(10)
                for (keylist,v) in items do
                    match keylist with 
                    | [] -> ()
                    | key::rest -> 
                        buckets.[key] <- (rest,v) :: (if buckets.ContainsKey key then buckets.[key] else []);

                [ for (KeyValue(key,items)) in buckets -> nodef key items ]

            tips @ nodes
        this.AddMembersDelayed (fun _ -> 
            let topTypes = [ for ty in assemblyf().GetTypes() do 
                                    if not ty.IsNested then
                                            let namespaceParts = match ty.Namespace with null -> [] | s -> s.Split '.' |> Array.toList
                                            yield namespaceParts,  ty ]
            let rec loop types = 
                types 
                |> bucketByPath
                    (fun namespaceComponent typesUnderNamespaceComponent -> 
                        let t = ProvidedTypeDefinition(namespaceComponent, baseType = Some typeof<obj>)
                        t.AddMembers (loop typesUnderNamespaceComponent)
                        (t :> Type))
                    (fun ty -> ty)
            loop topTypes)

    /// Abstract a type to a parametric-type. Requires "formal parameters" and "instantiation function".
    member __.DefineStaticParameters(staticParameters : list<ProvidedStaticParameter>, apply    : (string -> obj[] -> ProvidedTypeDefinition)) =
        staticParams      <- staticParameters 
        staticParamsApply <- Some apply

    /// Get ParameterInfo[] for the parametric type parameters (//s GetGenericParameters)
    member __.GetStaticParameters() = [| for p in staticParams -> p :> ParameterInfo |]

    /// Instantiate parametrics type
    member __.MakeParametricType(name:string,args:obj[]) =
        if staticParams.Length>0 then
            if staticParams.Length <> args.Length then
                failwith (sprintf "ProvidedTypeDefinition: expecting %d static parameters but given %d for type %s" staticParams.Length args.Length (fullName.Force()))
            match staticParamsApply with
            | None -> failwith "ProvidedTypeDefinition: DefineStaticParameters was not called"
            | Some f -> f name args

        else
            failwith (sprintf "ProvidedTypeDefinition: static parameters supplied but not expected for %s" (fullName.Force()))

    member __.DeclaringTypeImpl
        with set x = 
            match container with TypeContainer.TypeToBeDecided -> () | _ -> failwith (sprintf "container type for '%s' was already set to '%s'" this.FullName x.FullName); 
            container <- TypeContainer.Type  x

    // Implement overloads
    override __.Assembly = theAssembly.Force()

    member __.SetAssembly assembly = theAssembly <- lazy assembly

    member __.SetAssemblyLazy assembly = theAssembly <- assembly

    override __.FullName = fullName.Force()

    override __.Namespace = rootNamespace.Force()

    override __.BaseType = match baseType.Value with Some ty -> ty | None -> null
    
    // Constructors
    override __.GetConstructors bindingAttr = 
        [| for m in this.GetMembers bindingAttr do                
                if m.MemberType = MemberTypes.Constructor then
                    yield (m :?> ConstructorInfo) |]
    // Methods
    override __.GetMethodImpl(name, bindingAttr, _binderBinder, _callConvention, _types, _modifiers) : MethodInfo = 
        let membersWithName = 
            [ for m in this.GetMembers(bindingAttr) do                
                if m.MemberType.HasFlag(MemberTypes.Method) && m.Name = name then
                    yield  m ]
        match membersWithName with 
        | []        -> null
        | [meth]    -> meth :?> MethodInfo
        | _several   -> failwith "GetMethodImpl. not support overloads"

    override __.GetMethods bindingAttr = 
        this.GetMembers bindingAttr 
        |> Array.filter (fun m -> m.MemberType.HasFlag(MemberTypes.Method)) 
        |> Array.map (fun m -> m :?> MethodInfo)

    // Fields
    override __.GetField(name, bindingAttr) = 
        let fields = [| for m in this.GetMembers bindingAttr do
                            if m.MemberType.HasFlag(MemberTypes.Field) && (name = null || m.Name = name) then // REVIEW: name = null. Is that a valid query?!
                                yield m |] 
        if fields.Length > 0 then fields.[0] :?> FieldInfo else null

    override __.GetFields bindingAttr = 
        [| for m in this.GetMembers bindingAttr do if m.MemberType.HasFlag(MemberTypes.Field) then yield m :?> FieldInfo |]

    override __.GetInterface(_name, _ignoreCase) = notRequired "GetInterface" this.Name

    override __.GetInterfaces() = 
        [| yield! getInterfaces()  |]

    member __.GetInterfaceImplementations() = 
        [| yield! getInterfaces() |]

    member __.AddInterfaceImplementation ityp = interfaceImpls.Add ityp

    member __.AddInterfaceImplementationsDelayed itypf = interfaceImplsDelayed.Add itypf

    member __.GetMethodOverrides() = 
        [| yield! methodOverrides |]

    member __.DefineMethodOverride (bodyMethInfo,declMethInfo) = methodOverrides.Add (bodyMethInfo, declMethInfo)

    // Events
    override __.GetEvent(name, bindingAttr) = 
        let events = this.GetMembers bindingAttr 
                     |> Array.filter(fun m -> m.MemberType.HasFlag(MemberTypes.Event) && (name = null || m.Name = name)) 
        if events.Length > 0 then events.[0] :?> EventInfo else null

    override __.GetEvents bindingAttr = 
        [| for m in this.GetMembers bindingAttr do if m.MemberType.HasFlag(MemberTypes.Event) then yield downcast m |]    

    // Properties
    override __.GetProperties bindingAttr = 
        [| for m in this.GetMembers bindingAttr do if m.MemberType.HasFlag(MemberTypes.Property) then yield downcast m |]

    override __.GetPropertyImpl(name, bindingAttr, binder, returnType, types, modifiers) = 
        if returnType <> null then failwith "Need to handle specified return type in GetPropertyImpl"
        if types      <> null then failwith "Need to handle specified parameter types in GetPropertyImpl"
        if modifiers  <> null then failwith "Need to handle specified modifiers in GetPropertyImpl"
        if binder  <> null then failwith "Need to handle binder in GetPropertyImpl"
        let props = this.GetMembers bindingAttr |> Array.filter(fun m -> m.MemberType.HasFlag(MemberTypes.Property) && (name = null || m.Name = name))  // Review: nam = null, valid query!?
        if props.Length > 0 then
            props.[0] :?> PropertyInfo
        else
            null
    // Nested Types
    override __.MakeArrayType() = ProvidedSymbolType(ProvidedSymbolKind.SDArray, [this]) :> Type
    override __.MakeArrayType arg = ProvidedSymbolType(ProvidedSymbolKind.Array arg, [this]) :> Type
    override __.MakePointerType() = ProvidedSymbolType(ProvidedSymbolKind.Pointer, [this]) :> Type
    override __.MakeByRefType() = ProvidedSymbolType(ProvidedSymbolKind.ByRef, [this]) :> Type

    // FSharp.Data addition: this method is used by Debug.fs and QuotationBuilder.fs
    // Emulate the F# type provider type erasure mechanism to get the 
    // actual (erased) type. We erase ProvidedTypes to their base type
    // and we erase array of provided type to array of base type. In the
    // case of generics all the generic type arguments are also recursively
    // replaced with the erased-to types
    static member EraseType(t:Type) : Type =
        match t with
        | :? ProvidedTypeDefinition as ptd when ptd.IsErased -> ProvidedTypeDefinition.EraseType t.BaseType 
        | t when t.IsArray -> 
            let rank = t.GetArrayRank()
            let et = ProvidedTypeDefinition.EraseType (t.GetElementType())
            if rank = 0 then et.MakeArrayType() else et.MakeArrayType(rank)
        | :? ProvidedSymbolType as sym when sym.IsFSharpUnitAnnotated -> 
            t.UnderlyingSystemType
        | t when t.IsGenericType && not t.IsGenericTypeDefinition ->
            let genericTypeDefinition = t.GetGenericTypeDefinition()
            let genericArguments = t.GetGenericArguments() |> Array.map ProvidedTypeDefinition.EraseType
            genericTypeDefinition.MakeGenericType(genericArguments)
        | t -> t

    static member Logger : (string -> unit) option ref = ref None

    // The binding attributes are always set to DeclaredOnly ||| Static ||| Instance ||| Public when GetMembers is called directly by the F# compiler
    // However, it's possible for the framework to generate other sets of flags in some corner cases (e.g. via use of `enum` with a provided type as the target)
    override __.GetMembers bindingAttr = 
        let mems = 
            getMembers() 
            |> Array.filter (fun mem -> 
                                let isStatic, isPublic = 
                                    match mem with
                                    | :? FieldInfo as f -> f.IsStatic, f.IsPublic
                                    | :? MethodInfo as m -> m.IsStatic, m.IsPublic
                                    | :? ConstructorInfo as c -> c.IsStatic, c.IsPublic
                                    | :? PropertyInfo as p -> 
                                        let m = if p.CanRead then p.GetGetMethod() else p.GetSetMethod()
                                        m.IsStatic, m.IsPublic
                                    | :? EventInfo as e -> 
                                        let m = e.GetAddMethod()
                                        m.IsStatic, m.IsPublic
                                    | :? Type as ty -> 
                                        true, ty.IsNestedPublic
                                    | _ -> failwith (sprintf "Member %O is of unexpected type" mem)
                                bindingAttr.HasFlag(if isStatic then BindingFlags.Static else BindingFlags.Instance) &&
                                (
                                    (bindingAttr.HasFlag(BindingFlags.Public) && isPublic) || (bindingAttr.HasFlag(BindingFlags.NonPublic) && not isPublic)
                                ))

        if bindingAttr.HasFlag(BindingFlags.DeclaredOnly) || this.BaseType = null then mems
        else 
            // FSharp.Data change: just using this.BaseType is not enough in the case of CsvProvider,
            // because the base type is CsvRow<RowType>, so we have to erase recursively to CsvRow<TupleType>
            let baseMems = (ProvidedTypeDefinition.EraseType this.BaseType).GetMembers bindingAttr
            Array.append mems baseMems

    override __.GetNestedTypes bindingAttr = 
        this.GetMembers bindingAttr 
        |> Array.filter(fun m -> 
            m.MemberType.HasFlag(MemberTypes.NestedType) || 
            // Allow 'fake' nested types that are actually real .NET types
            m.MemberType.HasFlag(MemberTypes.TypeInfo)) |> Array.map(fun m -> m :?> Type)

    override __.GetMember(name,mt,_bindingAttr) = 
        let mt = 
            if mt &&& MemberTypes.NestedType = MemberTypes.NestedType then 
                mt ||| MemberTypes.TypeInfo 
            else
                mt
        getMembers() |> Array.filter(fun m->0<>(int(m.MemberType &&& mt)) && m.Name = name)
        
    override __.GetNestedType(name, bindingAttr) = 
        let nt = this.GetMember(name, MemberTypes.NestedType ||| MemberTypes.TypeInfo, bindingAttr)
        match nt.Length with
        | 0 -> null
        | 1 -> downcast nt.[0]
        | _ -> failwith (sprintf "There is more than one nested type called '%s' in type '%s'" name this.FullName)

    // Attributes, etc..
    override __.GetAttributeFlagsImpl() = adjustTypeAttributes attributes this.IsNested 
    override this.IsValueTypeImpl() = match baseType.Value with Some ty -> ty.IsValueType | None -> false
    override __.IsArrayImpl() = false
    override __.IsByRefImpl() = false
    override __.IsPointerImpl() = false
    override __.IsPrimitiveImpl() = false
    override __.IsCOMObjectImpl() = false
    override __.HasElementTypeImpl() = false
    override __.Name = className
    override __.DeclaringType = declaringType.Force()
    override __.MemberType = if this.IsNested then MemberTypes.NestedType else MemberTypes.TypeInfo      
    override __.GetHashCode() = rootNamespace.GetHashCode() ^^^ className.GetHashCode()
    override __.Equals(that:obj) = 
        match that with
        | null              -> false
        | :? ProvidedTypeDefinition as ti -> System.Object.ReferenceEquals(this,ti)
        | _                 -> false

    override __.GetGenericArguments() = [||] 
    override __.ToString() = this.Name
    

    override __.Module : Module = notRequired "Module" this.Name
    override __.GUID                                                                                   = Guid.Empty
    override __.GetConstructorImpl(_bindingAttr, _binder, _callConvention, _types, _modifiers)         = null
    override __.GetCustomAttributes(_inherit)                                                          = [| |]
    override __.GetCustomAttributes(_attributeType, _inherit)                                          = [| |]
    override __.IsDefined(_attributeType: Type, _inherit)                                              = false

    override __.GetElementType()                                                                                  = notRequired "Module" this.Name
    override __.InvokeMember(_name, _invokeAttr, _binder, _target, _args, _modifiers, _culture, _namedParameters) = notRequired "Module" this.Name
    override __.AssemblyQualifiedName                                                                             = notRequired "Module" this.Name
    member __.IsErased 
        with get() = (attributes &&& enum (int32 TypeProviderTypeAttributes.IsErased)) <> enum 0
        and set v = 
           if v then attributes <- attributes ||| enum (int32 TypeProviderTypeAttributes.IsErased)
           else attributes <- attributes &&& ~~~(enum (int32 TypeProviderTypeAttributes.IsErased))

    member __.SuppressRelocation 
        with get() = (attributes &&& enum (int32 TypeProviderTypeAttributes.SuppressRelocate)) <> enum 0
        and set v = 
           if v then attributes <- attributes ||| enum (int32 TypeProviderTypeAttributes.SuppressRelocate)
           else attributes <- attributes &&& ~~~(enum (int32 TypeProviderTypeAttributes.SuppressRelocate))

type AssemblyGenerator(assemblyFileName) = 
    let assemblyShortName = Path.GetFileNameWithoutExtension assemblyFileName
    let assemblyName = AssemblyName assemblyShortName
#if FX_NO_LOCAL_FILESYSTEM
    let assembly = 
        System.AppDomain.CurrentDomain.DefineDynamicAssembly(name=assemblyName,access=AssemblyBuilderAccess.Run)
    let assemblyMainModule = 
        assembly.DefineDynamicModule("MainModule")
#else
    let assembly = 
        System.AppDomain.CurrentDomain.DefineDynamicAssembly(name=assemblyName,access=(AssemblyBuilderAccess.Save ||| AssemblyBuilderAccess.Run),dir=Path.GetDirectoryName assemblyFileName)
    let assemblyMainModule = 
        assembly.DefineDynamicModule("MainModule", Path.GetFileName assemblyFileName)
#endif
    let typeMap = Dictionary<ProvidedTypeDefinition,TypeBuilder>(HashIdentity.Reference)
    let typeMapExtra = Dictionary<string,TypeBuilder>(HashIdentity.Structural)
    let uniqueLambdaTypeName() = 
        // lambda name should be unique across all types that all type provider might contribute in result assembly
        sprintf "Lambda%O" (Guid.NewGuid()) 

    member __.Assembly = assembly :> Assembly

    /// Emit the given provided type definitions into an assembly and adjust 'Assembly' property of all type definitions to return that
    /// assembly.
    member __.Generate(providedTypeDefinitions:(ProvidedTypeDefinition * string list option) list) = 
        let ALL = BindingFlags.Public ||| BindingFlags.NonPublic ||| BindingFlags.Static ||| BindingFlags.Instance
        // phase 1 - set assembly fields and emit type definitions
        begin 
            let rec typeMembers (tb:TypeBuilder)  (td : ProvidedTypeDefinition) = 
                for ntd in td.GetNestedTypes(ALL) do
                    nestedType tb ntd

            and nestedType (tb:TypeBuilder)  (ntd : Type) = 
                match ntd with 
                | :? ProvidedTypeDefinition as pntd -> 
                    if pntd.IsErased then invalidOp ("The nested provided type "+pntd.Name+" is marked as erased and cannot be converted to a generated type. Set 'IsErased' to false on the ProvidedTypeDefinition")
                    // Adjust the attributes - we're codegen'ing this type as nested
                    let attributes = adjustTypeAttributes ntd.Attributes true
                    let ntb = tb.DefineNestedType(pntd.Name,attr=attributes)
                    pntd.SetAssembly null
                    typeMap.[pntd] <- ntb
                    typeMembers ntb pntd
                | _ -> ()
                     
            for (pt,enclosingGeneratedTypeNames) in providedTypeDefinitions do 
              match enclosingGeneratedTypeNames with 
              | None -> 
                // Filter out the additional TypeProviderTypeAttributes flags
                let attributes = pt.Attributes &&& ~~~(enum (int32 TypeProviderTypeAttributes.SuppressRelocate))
                                                &&& ~~~(enum (int32 TypeProviderTypeAttributes.IsErased))
                // Adjust the attributes - we're codegen'ing as non-nested
                let attributes = adjustTypeAttributes attributes false 
                let tb = assemblyMainModule.DefineType(name=pt.FullName,attr=attributes) 
                pt.SetAssembly null
                typeMap.[pt] <- tb
                typeMembers tb pt 
              | Some ns -> 
                let otb,_ = 
                    ((None,""),ns) ||> List.fold (fun (otb:TypeBuilder option,fullName) n -> 
                        let fullName = if fullName = "" then n else fullName + "." + n
                        let priorType = if typeMapExtra.ContainsKey(fullName) then Some typeMapExtra.[fullName]  else None
                        let tb = 
                            match priorType with 
                            | Some tbb -> tbb 
                            | None ->
                            // OK, the implied nested type is not defined, define it now
                            let attributes = 
                                  TypeAttributes.Public ||| 
                                  TypeAttributes.Class ||| 
                                  TypeAttributes.Sealed 
                            // Filter out the additional TypeProviderTypeAttributes flags
                            let attributes = adjustTypeAttributes attributes otb.IsSome
                            let tb = 
                                match otb with 
                                | None -> assemblyMainModule.DefineType(name=n,attr=attributes) 
                                | Some (otb:TypeBuilder) -> otb.DefineNestedType(name=n,attr=attributes)
                            typeMapExtra.[fullName] <- tb
                            tb
                        (Some tb, fullName))
                nestedType otb.Value pt
        end
        let rec convType (ty:Type) = 
            match ty with 
            | :? ProvidedTypeDefinition as ptd ->   
                if typeMap.ContainsKey ptd then typeMap.[ptd] :> Type else ty
            | _ -> 
                if ty.IsGenericType then ty.GetGenericTypeDefinition().MakeGenericType (Array.map convType (ty.GetGenericArguments()))
                elif ty.HasElementType then 
                    let ety = convType (ty.GetElementType()) 
                    if ty.IsArray then 
                        let rank = ty.GetArrayRank()
                        if rank = 1 then ety.MakeArrayType() 
                        else ety.MakeArrayType rank 
                    elif ty.IsPointer then ety.MakePointerType() 
                    elif ty.IsByRef then ety.MakeByRefType()
                    else ty
                else ty

        let ctorMap = Dictionary<ProvidedConstructor, ConstructorBuilder>(HashIdentity.Reference)
        let methMap = Dictionary<ProvidedMethod, MethodBuilder>(HashIdentity.Reference)
        let fieldMap = Dictionary<FieldInfo, FieldBuilder>(HashIdentity.Reference)

        let iterateTypes f = 
            let rec typeMembers (ptd : ProvidedTypeDefinition) = 
                let tb = typeMap.[ptd] 
                f tb (Some ptd)
                for ntd in ptd.GetNestedTypes(ALL) do
                    nestedType ntd

            and nestedType (ntd : Type) = 
                match ntd with 
                | :? ProvidedTypeDefinition as pntd -> typeMembers pntd
                | _ -> ()
                     
            for (pt,enclosingGeneratedTypeNames) in providedTypeDefinitions do 
              match enclosingGeneratedTypeNames with 
              | None -> 
                typeMembers pt 
              | Some ns -> 
                let _fullName  = 
                    ("",ns) ||> List.fold (fun fullName n -> 
                        let fullName = if fullName = "" then n else fullName + "." + n
                        f typeMapExtra.[fullName] None
                        fullName)
                nestedType pt
        
        
        // phase 1b - emit base types
        iterateTypes (fun tb ptd -> 
            match ptd with 
            | None -> ()
            | Some ptd -> 
            match ptd.BaseType with null -> () | bt -> tb.SetParent(convType bt))

        let defineCustomAttrs f (cattrs: IList<CustomAttributeData>) = 
            for attr in cattrs do
                let constructorArgs = [ for x in attr.ConstructorArguments -> x.Value ]
                let namedProps,namedPropVals = [ for x in attr.NamedArguments do match x.MemberInfo with :? PropertyInfo as pi -> yield (pi, x.TypedValue.Value) | _ -> () ] |> List.unzip
                let namedFields,namedFieldVals = [ for x in attr.NamedArguments do match x.MemberInfo with :? FieldInfo as pi -> yield (pi, x.TypedValue.Value) | _ -> () ] |> List.unzip
                let cab = CustomAttributeBuilder(attr.Constructor, Array.ofList constructorArgs, Array.ofList namedProps, Array.ofList namedPropVals, Array.ofList namedFields, Array.ofList namedFieldVals)
                f cab

        // phase 2 - emit member definitions
        iterateTypes (fun tb ptd -> 
            match ptd with 
            | None -> ()
            | Some ptd -> 
            for cinfo in ptd.GetConstructors(ALL) do
                match cinfo with 
                | :? ProvidedConstructor as pcinfo when not (ctorMap.ContainsKey pcinfo)  ->
                    let cb =
                        if pcinfo.IsTypeInitializer then
                            if (cinfo.GetParameters()).Length <> 0 then failwith "Type initializer should not have parameters"
                            tb.DefineTypeInitializer()
                        else 
                            let cb = tb.DefineConstructor(cinfo.Attributes, CallingConventions.Standard, [| for p in cinfo.GetParameters() -> convType p.ParameterType |])
                            for (i,p) in cinfo.GetParameters() |> Seq.mapi (fun i x -> (i,x)) do
                                cb.DefineParameter(i+1, ParameterAttributes.None, p.Name) |> ignore
                            cb
                    ctorMap.[pcinfo] <- cb
                | _ -> () 
                    
            if ptd.IsEnum then
                tb.DefineField("value__", ptd.GetEnumUnderlyingType(), FieldAttributes.Public ||| FieldAttributes.SpecialName ||| FieldAttributes.RTSpecialName)
                |> ignore

            for finfo in ptd.GetFields(ALL) do
                let fieldInfo = 
                    match finfo with 
                        | :? ProvidedField as pinfo -> 
                            Some (pinfo.Name, convType finfo.FieldType, finfo.Attributes, pinfo.GetCustomAttributesDataImpl(), None)
                        | :? ProvidedLiteralField as pinfo ->
                            Some (pinfo.Name, convType finfo.FieldType, finfo.Attributes, pinfo.GetCustomAttributesDataImpl(), Some (pinfo.GetRawConstantValue()))
                        | _ -> None
                match fieldInfo with
                | Some (name, ty, attr, cattr, constantVal) when not (fieldMap.ContainsKey finfo) ->
                    let fb = tb.DefineField(name, ty, attr)
                    if constantVal.IsSome then
                        fb.SetConstant constantVal.Value
                    defineCustomAttrs fb.SetCustomAttribute cattr
                    fieldMap.[finfo] <- fb
                | _ -> () 
            for minfo in ptd.GetMethods(ALL) do
                match minfo with 
                | :? ProvidedMethod as pminfo when not (methMap.ContainsKey pminfo)  -> 
                    let mb = tb.DefineMethod(minfo.Name, minfo.Attributes, convType minfo.ReturnType, [| for p in minfo.GetParameters() -> convType p.ParameterType |])
                    for (i, p) in minfo.GetParameters() |> Seq.mapi (fun i x -> (i,x :?> ProvidedParameter)) do
                        // TODO: check why F# compiler doesn't emit default value when just p.Attributes is used (thus bad metadata is emitted)
//                        let mutable attrs = ParameterAttributes.None
//                        
//                        if p.IsOut then attrs <- attrs ||| ParameterAttributes.Out
//                        if p.HasDefaultParameterValue then attrs <- attrs ||| ParameterAttributes.Optional

                        let pb = mb.DefineParameter(i+1, p.Attributes, p.Name)
                        if p.HasDefaultParameterValue then 
                            do
                                let ctor = typeof<System.Runtime.InteropServices.DefaultParameterValueAttribute>.GetConstructor([|typeof<obj>|])
                                let builder = new CustomAttributeBuilder(ctor, [|p.RawDefaultValue|])
                                pb.SetCustomAttribute builder
                            do
                                let ctor = typeof<System.Runtime.InteropServices.OptionalAttribute>.GetConstructor([||])
                                let builder = new CustomAttributeBuilder(ctor, [||])
                                pb.SetCustomAttribute builder
                            pb.SetConstant p.RawDefaultValue
                    methMap.[pminfo] <- mb
                | _ -> () 

            for ityp in ptd.GetInterfaceImplementations() do
                tb.AddInterfaceImplementation ityp)

        // phase 3 - emit member code
        iterateTypes (fun  tb ptd -> 
            match ptd with 
            | None -> ()
            | Some ptd -> 
            let cattr = ptd.GetCustomAttributesDataImpl() 
            defineCustomAttrs tb.SetCustomAttribute cattr
            // Allow at most one constructor, and use its arguments as the fields of the type
            let ctors =
                ptd.GetConstructors(ALL) // exclude type initializer
                |> Seq.choose (function :? ProvidedConstructor as pcinfo when not pcinfo.IsTypeInitializer -> Some pcinfo | _ -> None) 
                |> Seq.toList
            let implictCtorArgs =
                match ctors  |> List.filter (fun x -> x.IsImplicitCtor)  with
                | [] -> []
                | [ pcinfo ] -> [ for p in pcinfo.GetParameters() -> p ]
                | _ -> failwith "at most one implicit constructor allowed"

            let implicitCtorArgsAsFields = 
                [ for ctorArg in implictCtorArgs -> 
                      tb.DefineField(ctorArg.Name, convType ctorArg.ParameterType, FieldAttributes.Private) ]
            
            let rec emitLambda(callSiteIlg : ILGenerator, v : Quotations.Var, body : Expr, freeVars : seq<Quotations.Var>, locals : Dictionary<_, LocalBuilder>, parameters) =
                let lambda = assemblyMainModule.DefineType(uniqueLambdaTypeName(), TypeAttributes.Class)
                let baseType = typedefof<FSharpFunc<_, _>>.MakeGenericType(v.Type, body.Type)
                lambda.SetParent(baseType)
                let ctor = lambda.DefineDefaultConstructor(MethodAttributes.Public)
                let decl = baseType.GetMethod "Invoke"
                let paramTypes = [| for p in decl.GetParameters() -> p.ParameterType |]
                let invoke = lambda.DefineMethod("Invoke", MethodAttributes.Virtual ||| MethodAttributes.Final ||| MethodAttributes.Public, decl.ReturnType, paramTypes)
                lambda.DefineMethodOverride(invoke, decl)

                // promote free vars to fields
                let fields = ResizeArray()
                for v in freeVars do
                    let f = lambda.DefineField(v.Name, v.Type, FieldAttributes.Assembly)
                    fields.Add(v, f)

                let copyOfLocals = Dictionary()
                
                let ilg = invoke.GetILGenerator()
                for (v, f) in fields do
                    let l = ilg.DeclareLocal(v.Type)
                    ilg.Emit(OpCodes.Ldarg_0)
                    ilg.Emit(OpCodes.Ldfld, f)
                    ilg.Emit(OpCodes.Stloc, l)
                    copyOfLocals.[v] <- l

                let expectedState = if (invoke.ReturnType = typeof<System.Void>) then ExpectedStackState.Empty else ExpectedStackState.Value
                emitExpr (ilg, copyOfLocals, [| Quotations.Var("this", lambda); v|]) expectedState body
                ilg.Emit(OpCodes.Ret) 

                lambda.CreateType() |> ignore

                callSiteIlg.Emit(OpCodes.Newobj, ctor)
                for (v, f) in fields do
                    callSiteIlg.Emit(OpCodes.Dup)
                    match locals.TryGetValue v with
                    | true, loc -> 
                        callSiteIlg.Emit(OpCodes.Ldloc, loc)
                    | false, _ -> 
                        let index = parameters |> Array.findIndex ((=) v)
                        callSiteIlg.Emit(OpCodes.Ldarg, index)
                    callSiteIlg.Emit(OpCodes.Stfld, f)

            and emitExpr (ilg: ILGenerator, locals:Dictionary<Quotations.Var,LocalBuilder>, parameterVars) expectedState expr = 
                let pop () = ilg.Emit(OpCodes.Pop)
                let popIfEmptyExpected s = if isEmpty s then pop()
                let emitConvIfNecessary t1 = 
                    if t1 = typeof<int16> then
                        ilg.Emit(OpCodes.Conv_I2)
                    elif t1 = typeof<uint16> then
                        ilg.Emit(OpCodes.Conv_U2)
                    elif t1 = typeof<sbyte> then
                        ilg.Emit(OpCodes.Conv_I1)
                    elif t1 = typeof<byte> then
                        ilg.Emit(OpCodes.Conv_U1)
                /// emits given expression to corresponding IL
                let rec emit (expectedState : ExpectedStackState) (expr: Expr) = 
                    match expr with 
                    | ForIntegerRangeLoop(loopVar, first, last, body) ->
                      // for(loopVar = first..last) body
                      let lb = 
                          match locals.TryGetValue loopVar with
                          | true, lb -> lb
                          | false, _ ->
                              let lb = ilg.DeclareLocal(convType loopVar.Type)
                              locals.Add(loopVar, lb)
                              lb

                      // loopVar = first
                      emit ExpectedStackState.Value first
                      ilg.Emit(OpCodes.Stloc, lb)

                      let before = ilg.DefineLabel()
                      let after = ilg.DefineLabel()

                      ilg.MarkLabel before
                      ilg.Emit(OpCodes.Ldloc, lb)
                            
                      emit ExpectedStackState.Value last
                      ilg.Emit(OpCodes.Bgt, after)

                      emit ExpectedStackState.Empty body

                      // loopVar++
                      ilg.Emit(OpCodes.Ldloc, lb)
                      ilg.Emit(OpCodes.Ldc_I4_1)
                      ilg.Emit(OpCodes.Add)
                      ilg.Emit(OpCodes.Stloc, lb)

                      ilg.Emit(OpCodes.Br, before)
                      ilg.MarkLabel(after)

                    | NewArray(elementTy, elements) ->
                      ilg.Emit(OpCodes.Ldc_I4, List.length elements)
                      ilg.Emit(OpCodes.Newarr, convType elementTy)

                      elements 
                      |> List.iteri (fun i el ->
                          ilg.Emit(OpCodes.Dup)
                          ilg.Emit(OpCodes.Ldc_I4, i)
                          emit ExpectedStackState.Value el
                          ilg.Emit(OpCodes.Stelem, convType elementTy)
                          )

                      popIfEmptyExpected expectedState

                    | WhileLoop(cond, body) ->
                      let before = ilg.DefineLabel()
                      let after = ilg.DefineLabel()

                      ilg.MarkLabel before
                      emit ExpectedStackState.Value cond
                      ilg.Emit(OpCodes.Brfalse, after)
                      emit ExpectedStackState.Empty body
                      ilg.Emit(OpCodes.Br, before)

                      ilg.MarkLabel after

                    | Var v -> 
                        if isEmpty expectedState then () else
                        let methIdx = parameterVars |> Array.tryFindIndex (fun p -> p = v) 
                        match methIdx with 
                        | Some idx -> 
                            ilg.Emit((if isAddress expectedState then OpCodes.Ldarga else OpCodes.Ldarg), idx)
                        | None -> 
                        let implicitCtorArgFieldOpt = implicitCtorArgsAsFields |> List.tryFind (fun f -> f.Name = v.Name) 
                        match implicitCtorArgFieldOpt with 
                        | Some ctorArgField -> 
                            ilg.Emit(OpCodes.Ldarg_0)
                            ilg.Emit(OpCodes.Ldfld, ctorArgField)
                        | None -> 
                        match locals.TryGetValue v with 
                        | true, localBuilder -> 
                            ilg.Emit((if isAddress expectedState  then OpCodes.Ldloca else OpCodes.Ldloc), localBuilder.LocalIndex)
                        | false, _ -> 
                            failwith "unknown parameter/field"

                    | Coerce (arg,ty) -> 
                        // castClass may lead to observable side-effects - InvalidCastException
                        emit ExpectedStackState.Value arg
                        let argTy = convType arg.Type
                        let targetTy = convType ty
                        if argTy.IsValueType && not targetTy.IsValueType then
                          ilg.Emit(OpCodes.Box, argTy)
                        elif not argTy.IsValueType && targetTy.IsValueType then
                          ilg.Emit(OpCodes.Unbox_Any, targetTy)
                        // emit castclass if 
                        // - targettype is not obj (assume this is always possible for ref types)
                        // AND 
                        // - HACK: targettype is TypeBuilderInstantiationType 
                        //   (its implementation of IsAssignableFrom raises NotSupportedException so it will be safer to always emit castclass)
                        // OR
                        // - not (argTy :> targetTy)
                        elif targetTy <> typeof<obj> && (Misc.TypeBuilderInstantiationType.Equals(targetTy.GetType()) || not (targetTy.IsAssignableFrom(argTy))) then
                          ilg.Emit(OpCodes.Castclass, targetTy)
                              
                        popIfEmptyExpected expectedState
                    | SpecificCall <@ (-) @>(None, [t1; t2; _], [a1; a2]) ->
                        assert(t1 = t2)
                        emit ExpectedStackState.Value a1
                        emit ExpectedStackState.Value a2
                        if t1 = typeof<decimal> then
                            ilg.Emit(OpCodes.Call, typeof<decimal>.GetMethod "op_Subtraction")
                        else
                            ilg.Emit(OpCodes.Sub)
                            emitConvIfNecessary t1

                        popIfEmptyExpected expectedState

                    | SpecificCall <@ (/) @> (None, [t1; t2; _], [a1; a2]) ->
                        assert (t1 = t2)
                        emit ExpectedStackState.Value a1
                        emit ExpectedStackState.Value a2
                        if t1 = typeof<decimal> then
                            ilg.Emit(OpCodes.Call, typeof<decimal>.GetMethod "op_Division")
                        else
                            match Type.GetTypeCode t1 with
                            | TypeCode.UInt32
                            | TypeCode.UInt64
                            | TypeCode.UInt16
                            | TypeCode.Byte
                            | _ when t1 = typeof<unativeint> -> ilg.Emit (OpCodes.Div_Un)
                            | _ -> ilg.Emit(OpCodes.Div)

                            emitConvIfNecessary t1

                        popIfEmptyExpected expectedState

                    | SpecificCall <@ int @>(None, [sourceTy], [v]) ->
                        emit ExpectedStackState.Value v
                        match Type.GetTypeCode(sourceTy) with
                        | TypeCode.String -> 
                            ilg.Emit(OpCodes.Call, Misc.ParseInt32Method)
                        | TypeCode.Single
                        | TypeCode.Double
                        | TypeCode.Int64 
                        | TypeCode.UInt64
                        | TypeCode.UInt16
                        | TypeCode.Char
                        | TypeCode.Byte
                        | _ when sourceTy = typeof<nativeint> || sourceTy = typeof<unativeint> ->
                            ilg.Emit(OpCodes.Conv_I4)
                        | TypeCode.Int32
                        | TypeCode.UInt32
                        | TypeCode.Int16
                        | TypeCode.SByte -> () // no op
                        | _ -> failwith "TODO: search for op_Explicit on sourceTy"

                    | SpecificCall <@ LanguagePrimitives.IntrinsicFunctions.GetArray @> (None, [ty], [arr; index]) ->
                        // observable side-effect - IndexOutOfRangeException
                        emit ExpectedStackState.Value arr
                        emit ExpectedStackState.Value index
                        if isAddress expectedState then
                            ilg.Emit(OpCodes.Readonly)
                            ilg.Emit(OpCodes.Ldelema, convType ty)
                        else
                            ilg.Emit(OpCodes.Ldelem, convType ty)

                        popIfEmptyExpected expectedState

                    | SpecificCall <@ LanguagePrimitives.IntrinsicFunctions.GetArray2D @> (None, _ty, arr::indices)
                    | SpecificCall <@ LanguagePrimitives.IntrinsicFunctions.GetArray3D @> (None, _ty, arr::indices)
                    | SpecificCall <@ LanguagePrimitives.IntrinsicFunctions.GetArray4D @> (None, _ty, arr::indices) ->
                              
                        let meth = 
                          let name = if isAddress expectedState then "Address" else "Get"
                          arr.Type.GetMethod(name)

                        // observable side-effect - IndexOutOfRangeException
                        emit ExpectedStackState.Value arr
                        for index in indices do
                          emit ExpectedStackState.Value index
                              
                        if isAddress expectedState then
                          ilg.Emit(OpCodes.Readonly)

                        ilg.Emit(OpCodes.Call, meth)

                        popIfEmptyExpected expectedState

                    | FieldGet (objOpt,field) -> 
                        match field with
                        | :? ProvidedLiteralField as plf when plf.DeclaringType.IsEnum ->
                            if expectedState <> ExpectedStackState.Empty then
                                emit expectedState (Expr.Value(field.GetRawConstantValue(), field.FieldType.GetEnumUnderlyingType()))
                        | _ ->
                        match objOpt with 
                        | None -> () 
                        | Some e -> 
                          let s = if e.Type.IsValueType then ExpectedStackState.Address else ExpectedStackState.Value
                          emit s e
                        let field = 
                            match field with 
                            | :? ProvidedField as pf when fieldMap.ContainsKey pf -> fieldMap.[pf] :> FieldInfo 
                            | m -> m
                        if field.IsStatic then 
                            ilg.Emit(OpCodes.Ldsfld, field)
                        else
                            ilg.Emit(OpCodes.Ldfld, field)

                    | FieldSet (objOpt,field,v) -> 
                        match objOpt with 
                        | None -> () 
                        | Some e -> 
                          let s = if e.Type.IsValueType then ExpectedStackState.Address else ExpectedStackState.Value
                          emit s e
                        emit ExpectedStackState.Value v
                        let field = match field with :? ProvidedField as pf when fieldMap.ContainsKey pf -> fieldMap.[pf] :> FieldInfo | m -> m
                        if field.IsStatic then 
                            ilg.Emit(OpCodes.Stsfld, field)
                        else
                            ilg.Emit(OpCodes.Stfld, field)
                    | Call (objOpt,meth,args) -> 
                        match objOpt with 
                        | None -> () 
                        | Some e -> 
                          let s = if e.Type.IsValueType then ExpectedStackState.Address else ExpectedStackState.Value
                          emit s e
                        for pe in args do 
                            emit ExpectedStackState.Value pe
                        let getMeth (m:MethodInfo) = match m with :? ProvidedMethod as pm when methMap.ContainsKey pm -> methMap.[pm] :> MethodInfo | m -> m
                        // Handle the case where this is a generic method instantiated at a type being compiled
                        let mappedMeth = 
                            if meth.IsGenericMethod then 
                                let args = meth.GetGenericArguments() |> Array.map convType
                                let gmd = meth.GetGenericMethodDefinition() |> getMeth
                                gmd.GetGenericMethodDefinition().MakeGenericMethod args
                            elif meth.DeclaringType.IsGenericType then 
                                let gdty = convType (meth.DeclaringType.GetGenericTypeDefinition())
                                let gdtym = gdty.GetMethods() |> Seq.find (fun x -> x.Name = meth.Name)
                                assert (gdtym <> null) // ?? will never happen - if method is not found - KeyNotFoundException will be raised
                                let dtym =
                                    match convType meth.DeclaringType with
                                    | :? TypeBuilder as dty -> TypeBuilder.GetMethod(dty, gdtym)
                                    | dty -> MethodBase.GetMethodFromHandle(meth.MethodHandle, dty.TypeHandle) :?> _
                                
                                assert (dtym <> null)
                                dtym
                            else
                                getMeth meth
                        match objOpt with 
                        | Some obj when mappedMeth.IsAbstract || mappedMeth.IsVirtual  ->
                            if obj.Type.IsValueType then ilg.Emit(OpCodes.Constrained, convType obj.Type)
                            ilg.Emit(OpCodes.Callvirt, mappedMeth)
                        | _ ->
                            ilg.Emit(OpCodes.Call, mappedMeth)

                        let returnTypeIsVoid = mappedMeth.ReturnType = typeof<System.Void>
                        match returnTypeIsVoid, (isEmpty expectedState) with
                        | false, true -> 
                              // method produced something, but we don't need it
                              pop()
                        | true, false when expr.Type = typeof<unit> -> 
                              // if we need result and method produce void and result should be unit - push null as unit value on stack
                              ilg.Emit(OpCodes.Ldnull)
                        | _ -> ()

                    | NewObject (ctor,args) -> 
                        for pe in args do 
                            emit ExpectedStackState.Value pe
                        let meth = match ctor with :? ProvidedConstructor as pc when ctorMap.ContainsKey pc -> ctorMap.[pc] :> ConstructorInfo | c -> c
                        ilg.Emit(OpCodes.Newobj, meth)
                              
                        popIfEmptyExpected expectedState                              

                    | Value (obj, _ty) -> 
                        let rec emitC (v:obj) = 
                            match v with 
                            | :? string as x -> ilg.Emit(OpCodes.Ldstr, x)
                            | :? int8 as x -> ilg.Emit(OpCodes.Ldc_I4, int32 x)
                            | :? uint8 as x -> ilg.Emit(OpCodes.Ldc_I4, int32 (int8 x))
                            | :? int16 as x -> ilg.Emit(OpCodes.Ldc_I4, int32 x)
                            | :? uint16 as x -> ilg.Emit(OpCodes.Ldc_I4, int32 (int16 x))
                            | :? int32 as x -> ilg.Emit(OpCodes.Ldc_I4, x)
                            | :? uint32 as x -> ilg.Emit(OpCodes.Ldc_I4, int32 x)
                            | :? int64 as x -> ilg.Emit(OpCodes.Ldc_I8, x)
                            | :? uint64 as x -> ilg.Emit(OpCodes.Ldc_I8, int64 x)
                            | :? char as x -> ilg.Emit(OpCodes.Ldc_I4, int32 x)
                            | :? bool as x -> ilg.Emit(OpCodes.Ldc_I4, if x then 1 else 0)
                            | :? float32 as x -> ilg.Emit(OpCodes.Ldc_R4, x)
                            | :? float as x -> ilg.Emit(OpCodes.Ldc_R8, x)
#if FX_NO_GET_ENUM_UNDERLYING_TYPE
#else
                            | :? System.Enum as x when x.GetType().GetEnumUnderlyingType() = typeof<int32> -> ilg.Emit(OpCodes.Ldc_I4, unbox<int32> v)
#endif
                            | :? Type as ty ->
                                ilg.Emit(OpCodes.Ldtoken, convType ty)
                                ilg.Emit(OpCodes.Call, Misc.GetTypeFromHandleMethod)
                            | :? decimal as x ->
                                let bits = System.Decimal.GetBits x
                                ilg.Emit(OpCodes.Ldc_I4, bits.[0])
                                ilg.Emit(OpCodes.Ldc_I4, bits.[1])
                                ilg.Emit(OpCodes.Ldc_I4, bits.[2])
                                do
                                    let sign = (bits.[3] &&& 0x80000000) <> 0
                                    ilg.Emit(if sign then OpCodes.Ldc_I4_1 else OpCodes.Ldc_I4_0)
                                do
                                    let scale = byte ((bits.[3] >>> 16) &&& 0x7F)
                                    ilg.Emit(OpCodes.Ldc_I4_S, scale)
                                ilg.Emit(OpCodes.Newobj, Misc.DecimalConstructor)
                            | :? DateTime as x ->
                                ilg.Emit(OpCodes.Ldc_I8, x.Ticks)
                                ilg.Emit(OpCodes.Ldc_I4, int x.Kind)
                                ilg.Emit(OpCodes.Newobj, Misc.DateTimeConstructor)
                            | :? DateTimeOffset as x ->
                                ilg.Emit(OpCodes.Ldc_I8, x.Ticks)
                                ilg.Emit(OpCodes.Ldc_I8, x.Offset.Ticks)
                                ilg.Emit(OpCodes.Newobj, Misc.TimeSpanConstructor)
                                ilg.Emit(OpCodes.Newobj, Misc.DateTimeOffsetConstructor)
                            | null -> ilg.Emit(OpCodes.Ldnull)
                            | _ -> failwithf "unknown constant '%A' in generated method" v
                        if isEmpty expectedState then ()
                        else emitC obj

                    | Let(v,e,b) -> 
                        let lb = ilg.DeclareLocal (convType v.Type)
                        locals.Add (v, lb) 
                        emit ExpectedStackState.Value e
                        ilg.Emit(OpCodes.Stloc, lb.LocalIndex)
                        emit expectedState b
                              
                    | Sequential(e1, e2) ->
                        emit ExpectedStackState.Empty e1
                        emit expectedState e2                          

                    | IfThenElse(cond, ifTrue, ifFalse) ->
                        let ifFalseLabel = ilg.DefineLabel()
                        let endLabel = ilg.DefineLabel()

                        emit ExpectedStackState.Value cond

                        ilg.Emit(OpCodes.Brfalse, ifFalseLabel)

                        emit expectedState ifTrue
                        ilg.Emit(OpCodes.Br, endLabel)

                        ilg.MarkLabel(ifFalseLabel)
                        emit expectedState ifFalse

                        ilg.Emit(OpCodes.Nop)
                        ilg.MarkLabel(endLabel)

                    | TryWith(body, _filterVar, _filterBody, catchVar, catchBody) ->                                                                                      
                              
                        let stres, ldres = 
                            if isEmpty expectedState then ignore, ignore
                            else
                              let local = ilg.DeclareLocal (convType body.Type)
                              let stres = fun () -> ilg.Emit(OpCodes.Stloc, local)
                              let ldres = fun () -> ilg.Emit(OpCodes.Ldloc, local)
                              stres, ldres

                        let exceptionVar = ilg.DeclareLocal(convType catchVar.Type)
                        locals.Add(catchVar, exceptionVar)

                        let _exnBlock = ilg.BeginExceptionBlock()
                              
                        emit expectedState body
                        stres()

                        ilg.BeginCatchBlock(convType  catchVar.Type)
                        ilg.Emit(OpCodes.Stloc, exceptionVar)
                        emit expectedState catchBody
                        stres()
                        ilg.EndExceptionBlock()

                        ldres()

                    | VarSet(v,e) -> 
                        emit ExpectedStackState.Value e
                        match locals.TryGetValue v with 
                        | true, localBuilder -> 
                            ilg.Emit(OpCodes.Stloc, localBuilder.LocalIndex)
                        | false, _ -> 
                            failwith "unknown parameter/field in assignment. Only assignments to locals are currently supported by TypeProviderEmit"
                    | Lambda(v, body) ->
                        emitLambda(ilg, v, body, expr.GetFreeVars(), locals, parameterVars)
                        popIfEmptyExpected expectedState
                    | n -> 
                        failwith (sprintf "unknown expression '%A' in generated method" n)
                emit expectedState expr


            // Emit the constructor (if any)
            for pcinfo in ctors do 
                assert ctorMap.ContainsKey pcinfo
                let cb = ctorMap.[pcinfo]
                let cattr = pcinfo.GetCustomAttributesDataImpl() 
                defineCustomAttrs cb.SetCustomAttribute cattr
                let ilg = cb.GetILGenerator()
                let locals = Dictionary<Var,LocalBuilder>()
                let parameterVars = 
                    [| yield Var("this", pcinfo.DeclaringType)
                       for p in pcinfo.GetParameters() do 
                            yield Var(p.Name, p.ParameterType) |]
                let parameters = 
                    [| for v in parameterVars -> Expr.Var v |]
                match pcinfo.GetBaseConstructorCallInternal true with
                | None ->  
                    ilg.Emit(OpCodes.Ldarg_0)
                    let cinfo = ptd.BaseType.GetConstructor(BindingFlags.Public ||| BindingFlags.NonPublic ||| BindingFlags.Instance, null, [| |], null)
                    ilg.Emit(OpCodes.Call,cinfo)
                | Some f -> 
                    // argExprs should always include 'this'
                    let (cinfo,argExprs) = f (Array.toList parameters)
                    for argExpr in argExprs do 
                        emitExpr (ilg, locals, parameterVars) ExpectedStackState.Value argExpr
                    ilg.Emit(OpCodes.Call,cinfo)

                if pcinfo.IsImplicitCtor then
                    for ctorArgsAsFieldIdx,ctorArgsAsField in List.mapi (fun i x -> (i,x)) implicitCtorArgsAsFields do 
                        ilg.Emit(OpCodes.Ldarg_0)
                        ilg.Emit(OpCodes.Ldarg, ctorArgsAsFieldIdx+1)
                        ilg.Emit(OpCodes.Stfld, ctorArgsAsField)
                else
                    let code  = pcinfo.GetInvokeCodeInternal true
                    let code = code parameters
                    emitExpr (ilg, locals, parameterVars) ExpectedStackState.Empty code
                ilg.Emit(OpCodes.Ret)
            
            match ptd.GetConstructors(ALL) |> Seq.tryPick (function :? ProvidedConstructor as pc when pc.IsTypeInitializer -> Some pc | _ -> None) with
            | None -> ()
            | Some pc ->
                let cb = ctorMap.[pc]
                let ilg = cb.GetILGenerator()
                let cattr = pc.GetCustomAttributesDataImpl() 
                defineCustomAttrs cb.SetCustomAttribute cattr
                let expr = pc.GetInvokeCodeInternal true [||]
                emitExpr(ilg, new Dictionary<_, _>(), [||]) ExpectedStackState.Empty expr
                ilg.Emit OpCodes.Ret

            // Emit the methods
            for minfo in ptd.GetMethods(ALL) do
              match minfo with 
              | :? ProvidedMethod as pminfo   -> 
                let mb = methMap.[pminfo]
                let ilg = mb.GetILGenerator()
                let cattr = pminfo.GetCustomAttributesDataImpl() 
                defineCustomAttrs mb.SetCustomAttribute cattr

                let parameterVars = 
                    [| if not pminfo.IsStatic then 
                            yield Var("this", pminfo.DeclaringType)
                       for p in pminfo.GetParameters() do 
                            yield Var(p.Name, p.ParameterType) |]
                let parameters = 
                    [| for v in parameterVars -> Expr.Var v |]

                let expr = pminfo.GetInvokeCodeInternal true parameters 

                let locals = Dictionary<Var,LocalBuilder>()
                //printfn "Emitting linqCode for %s::%s, code = %s" pminfo.DeclaringType.FullName pminfo.Name (try linqCode.ToString() with _ -> "<error>")


                let expectedState = if (minfo.ReturnType = typeof<System.Void>) then ExpectedStackState.Empty else ExpectedStackState.Value
                emitExpr (ilg, locals, parameterVars) expectedState expr
                ilg.Emit OpCodes.Ret
              | _ -> ()
  
            for (bodyMethInfo,declMethInfo) in ptd.GetMethodOverrides() do 
                let bodyMethBuilder = methMap.[bodyMethInfo]
                tb.DefineMethodOverride(bodyMethBuilder,declMethInfo)

            for evt in ptd.GetEvents(ALL) |> Seq.choose (function :? ProvidedEvent as pe -> Some pe | _ -> None) do
                let eb = tb.DefineEvent(evt.Name, evt.Attributes, evt.EventHandlerType)
                defineCustomAttrs eb.SetCustomAttribute (evt.GetCustomAttributesDataImpl())
                eb.SetAddOnMethod(methMap.[evt.GetAddMethod(true) :?> _])
                eb.SetRemoveOnMethod(methMap.[evt.GetRemoveMethod(true) :?> _])
                // TODO: add raiser
            
            for pinfo in ptd.GetProperties(ALL) |> Seq.choose (function :? ProvidedProperty as pe -> Some pe | _ -> None) do
                let pb = tb.DefineProperty(pinfo.Name, pinfo.Attributes, convType pinfo.PropertyType, [| for p in pinfo.GetIndexParameters() -> convType p.ParameterType |])
                let cattr = pinfo.GetCustomAttributesDataImpl() 
                defineCustomAttrs pb.SetCustomAttribute cattr
                if  pinfo.CanRead then 
                    let minfo = pinfo.GetGetMethod(true)
                    pb.SetGetMethod (methMap.[minfo :?> ProvidedMethod ])
                if  pinfo.CanWrite then 
                    let minfo = pinfo.GetSetMethod(true)
                    pb.SetSetMethod (methMap.[minfo :?> ProvidedMethod ]))


        // phase 4 - complete types
        iterateTypes (fun tb _ptd -> tb.CreateType() |> ignore)

#if FX_NO_LOCAL_FILESYSTEM
#else
        assembly.Save (Path.GetFileName assemblyFileName)
#endif

        let assemblyLoadedInMemory = assemblyMainModule.Assembly 

        iterateTypes (fun _tb ptd -> 
            match ptd with 
            | None -> ()
            | Some ptd -> ptd.SetAssembly assemblyLoadedInMemory)

#if FX_NO_LOCAL_FILESYSTEM
#else
    member __.GetFinalBytes() = 
        let assemblyBytes = File.ReadAllBytes assemblyFileName
        let _assemblyLoadedInMemory = System.Reflection.Assembly.Load(assemblyBytes,null,System.Security.SecurityContextSource.CurrentAppDomain)
        //printfn "final bytes in '%s'" assemblyFileName
        //File.Delete assemblyFileName
        assemblyBytes
#endif

type ProvidedAssembly(assemblyFileName: string) = 
    let theTypes = ResizeArray<_>()
    let assemblyGenerator = AssemblyGenerator(assemblyFileName)
    let assemblyLazy = 
        lazy 
            assemblyGenerator.Generate(theTypes |> Seq.toList)
            assemblyGenerator.Assembly
#if FX_NO_LOCAL_FILESYSTEM
#else
    let theAssemblyBytesLazy = 
      lazy
        assemblyGenerator.GetFinalBytes()

    do
        GlobalProvidedAssemblyElementsTable.theTable.Add(assemblyGenerator.Assembly, theAssemblyBytesLazy) 

#endif

    let add (providedTypeDefinitions:ProvidedTypeDefinition list, enclosingTypeNames: string list option) = 
        for pt in providedTypeDefinitions do 
            if pt.IsErased then invalidOp ("The provided type "+pt.Name+"is marked as erased and cannot be converted to a generated type. Set 'IsErased' to false on the ProvidedTypeDefinition")
            theTypes.Add(pt,enclosingTypeNames)
            pt.SetAssemblyLazy assemblyLazy

    member x.AddNestedTypes (providedTypeDefinitions, enclosingTypeNames) = add (providedTypeDefinitions, Some enclosingTypeNames)
    member x.AddTypes (providedTypeDefinitions) = add (providedTypeDefinitions, None)
#if FX_NO_LOCAL_FILESYSTEM
#else
    static member RegisterGenerated (fileName:string) = 
        //printfn "registered assembly in '%s'" fileName
        let assemblyBytes = System.IO.File.ReadAllBytes fileName
        let assembly = Assembly.Load(assemblyBytes,null,System.Security.SecurityContextSource.CurrentAppDomain)
        GlobalProvidedAssemblyElementsTable.theTable.Add(assembly, Lazy<_>.CreateFromValue assemblyBytes)
        assembly
#endif


module Local = 

    let makeProvidedNamespace (namespaceName:string) (types:ProvidedTypeDefinition list) =
        let types = [| for ty in types -> ty :> Type |]
        {new IProvidedNamespace with
            member __.GetNestedNamespaces() = [| |]
            member __.NamespaceName = namespaceName
            member __.GetTypes() = types |> Array.copy
            member __.ResolveTypeName typeName : System.Type = 
                match types |> Array.tryFind (fun ty -> ty.Name = typeName) with
                | Some ty -> ty
                | None    -> null
        }

// Used by unit testing to check that invalidation handlers are being disconnected
module GlobalCountersForInvalidation = 
    let mutable invalidationHandlersAdded = 0
    let mutable invalidationHandlersRemoved = 0
    let GetInvalidationHandlersAdded() = invalidationHandlersAdded
    let GetInvalidationHandlersRemoved() = invalidationHandlersRemoved

#if FX_NO_LOCAL_FILESYSTEM
type TypeProviderForNamespaces(namespacesAndTypes : list<(string * list<ProvidedTypeDefinition>)>) =
#else
type TypeProviderForNamespaces(namespacesAndTypes : list<(string * list<ProvidedTypeDefinition>)>) as this =
#endif
    let otherNamespaces = ResizeArray<string * list<ProvidedTypeDefinition>>()

    let providedNamespaces = 
        lazy [| for (namespaceName,types) in namespacesAndTypes do 
                     yield Local.makeProvidedNamespace namespaceName types 
                for (namespaceName,types) in otherNamespaces do 
                     yield Local.makeProvidedNamespace namespaceName types |]

    let invalidateE = new Event<EventHandler,EventArgs>()    

    let disposing = Event<EventHandler,EventArgs>()

#if FX_NO_LOCAL_FILESYSTEM
#else
    let probingFolders = ResizeArray()
    let handler = ResolveEventHandler(fun _ args -> this.ResolveAssembly(args))
    do AppDomain.CurrentDomain.add_AssemblyResolve handler
#endif

    new (namespaceName:string,types:list<ProvidedTypeDefinition>) = new TypeProviderForNamespaces([(namespaceName,types)])
    new () = new TypeProviderForNamespaces([])

    [<CLIEvent>]
    member __.Disposing = disposing.Publish

#if FX_NO_LOCAL_FILESYSTEM
    interface System.IDisposable with 
        member x.Dispose() = 
            disposing.Trigger(x, EventArgs.Empty)
#else
    abstract member ResolveAssembly : args : System.ResolveEventArgs -> Assembly

    default __.ResolveAssembly(args) = 
        let expectedName = (AssemblyName(args.Name)).Name + ".dll"
        let expectedLocationOpt = 
            probingFolders 
            |> Seq.map (fun f -> IO.Path.Combine(f, expectedName))
            |> Seq.tryFind IO.File.Exists
        match expectedLocationOpt with
        | Some f -> Assembly.LoadFrom f
        | None -> null

    member __.RegisterProbingFolder (folder) = 
        // use GetFullPath to ensure that folder is valid
        ignore(IO.Path.GetFullPath folder)
        probingFolders.Add folder

    member __.RegisterRuntimeAssemblyLocationAsProbingFolder (config : TypeProviderConfig) =  
        config.RuntimeAssembly
        |> IO.Path.GetDirectoryName
        |> this.RegisterProbingFolder

    interface System.IDisposable with 
        member x.Dispose() = 
            disposing.Trigger(x, EventArgs.Empty)
            AppDomain.CurrentDomain.remove_AssemblyResolve handler
#endif

    member __.AddNamespace (namespaceName,types:list<_>) = otherNamespaces.Add (namespaceName,types)

    // FSharp.Data addition: this method is used by Debug.fs
    member __.Namespaces = Seq.readonly otherNamespaces

    member this.Invalidate() = invalidateE.Trigger(this,EventArgs())

    member __.GetStaticParametersForMethod(mb: MethodBase) =
        match mb with
        | :? ProvidedMethod as t -> t.GetStaticParameters()
        | _ -> [| |]

    member __.ApplyStaticArgumentsForMethod(mb: MethodBase, mangledName, objs) = 
        match mb with
        | :? ProvidedMethod as t -> t.ApplyStaticArguments(mangledName, objs) :> MethodBase
        | _ -> failwith (sprintf "ApplyStaticArguments: static parameters for method %s are unexpected" mb.Name)

    interface ITypeProvider with

        [<CLIEvent>]
        override __.Invalidate = invalidateE.Publish

        override __.GetNamespaces() = Array.copy providedNamespaces.Value

        member __.GetInvokerExpression(methodBase, parameters) =
            let rec getInvokerExpression (methodBase : MethodBase) parameters =
                match methodBase with
                | :? ProvidedMethod as m when (match methodBase.DeclaringType with :? ProvidedTypeDefinition as pt -> pt.IsErased | _ -> true) ->
                    m.GetInvokeCodeInternal false parameters
                    |> expand
                | :? ProvidedConstructor as m when (match methodBase.DeclaringType with :? ProvidedTypeDefinition as pt -> pt.IsErased | _ -> true) -> 
                    m.GetInvokeCodeInternal false parameters
                    |> expand
                // Otherwise, assume this is a generative assembly and just emit a call to the constructor or method
                | :?  ConstructorInfo as cinfo ->  
                    Expr.NewObjectUnchecked(cinfo, Array.toList parameters) 
                | :? System.Reflection.MethodInfo as minfo ->  
                    if minfo.IsStatic then 
                        Expr.CallUnchecked(minfo, Array.toList parameters) 
                    else
                        Expr.CallUnchecked(parameters.[0], minfo, Array.toList parameters.[1..])
                | _ -> failwith ("TypeProviderForNamespaces.GetInvokerExpression: not a ProvidedMethod/ProvidedConstructor/ConstructorInfo/MethodInfo, name=" + methodBase.Name + " class=" + methodBase.GetType().FullName)
            and expand expr = 
                match expr with
                | NewObject(ctor, args) -> getInvokerExpression ctor [| for arg in args -> expand arg|]
                | Call(inst, mi, args) ->
                    let args = 
                        [|
                            match inst with
                            | Some inst -> yield expand inst
                            | _ -> ()
                            yield! List.map expand args
                        |]
                    getInvokerExpression mi args
                | ShapeCombinationUnchecked(shape, args) -> RebuildShapeCombinationUnchecked(shape, List.map expand args)
                | ShapeVarUnchecked v -> Expr.Var v
                | ShapeLambdaUnchecked(v, body) -> Expr.Lambda(v, expand body)
            getInvokerExpression methodBase parameters
#if FX_NO_CUSTOMATTRIBUTEDATA

        member __.GetMemberCustomAttributesData(methodBase) = 
            match methodBase with
            | :? ProvidedTypeDefinition as m  -> m.GetCustomAttributesDataImpl()
            | :? ProvidedMethod as m  -> m.GetCustomAttributesDataImpl()
            | :? ProvidedProperty as m  -> m.GetCustomAttributesDataImpl()
            | :? ProvidedConstructor as m -> m.GetCustomAttributesDataImpl()
            | :? ProvidedEvent as m -> m.GetCustomAttributesDataImpl()
            | :?  ProvidedLiteralField as m -> m.GetCustomAttributesDataImpl()
            | :?  ProvidedField as m -> m.GetCustomAttributesDataImpl()
            | _ -> [| |] :> IList<_>

        member __.GetParameterCustomAttributesData(methodBase) = 
            match methodBase with
            | :? ProvidedParameter as m  -> m.GetCustomAttributesDataImpl()
            | _ -> [| |] :> IList<_>


#endif
        override __.GetStaticParameters(ty) =
            match ty with
            | :? ProvidedTypeDefinition as t ->
                if ty.Name = t.Name (* REVIEW: use equality? *) then
                    t.GetStaticParameters()
                else
                    [| |]
            | _ -> [| |]

        override __.ApplyStaticArguments(ty,typePathAfterArguments:string[],objs) = 
            let typePathAfterArguments = typePathAfterArguments.[typePathAfterArguments.Length-1]
            match ty with
            | :? ProvidedTypeDefinition as t -> (t.MakeParametricType(typePathAfterArguments,objs) :> Type)
            | _ -> failwith (sprintf "ApplyStaticArguments: static params for type %s are unexpected" ty.FullName)

#if FX_NO_LOCAL_FILESYSTEM
        override __.GetGeneratedAssemblyContents(_assembly) = 
            // TODO: this is very fake, we rely on the fact it is never needed
            match System.Windows.Application.GetResourceStream(System.Uri("FSharp.Core.dll",System.UriKind.Relative)) with 
            | null -> failwith "FSharp.Core.dll not found as Manifest Resource, we're just trying to read some random .NET assembly, ok?"
            | resStream ->  
                use stream = resStream.Stream
                let len = stream.Length
                let buf = Array.zeroCreate<byte> (int len)
                let rec loop where rem = 
                    let n = stream.Read(buf, 0, int rem)
                    if n < rem then loop (where  + n) (rem - n)
                loop 0 (int len) 
                buf

            //failwith "no file system"
#else
        override __.GetGeneratedAssemblyContents(assembly:Assembly) = 
            //printfn "looking up assembly '%s'" assembly.FullName
            match GlobalProvidedAssemblyElementsTable.theTable.TryGetValue assembly with 
            | true,bytes -> bytes.Force()
            | _ -> 
                let bytes = System.IO.File.ReadAllBytes assembly.ManifestModule.FullyQualifiedName
                GlobalProvidedAssemblyElementsTable.theTable.[assembly] <- Lazy<_>.CreateFromValue bytes
                bytes
#endif
