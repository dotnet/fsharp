// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Linq.RuntimeHelpers

open System
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.Operators
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
open Microsoft.FSharp.Collections
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.DerivedPatterns
open Microsoft.FSharp.Reflection
open Microsoft.FSharp.Linq.RuntimeHelpers
open System.Collections.Generic
open System.Linq
open System.Linq.Expressions

#if FX_RESHAPED_REFLECTION
open PrimReflectionAdapters
open ReflectionAdapters
#endif

// ----------------------------------------------------------------------------

/// A type used to reconstruct a grouping after applying a mutable->immutable mapping transformation 
/// on a result of a query.
type Grouping<'K, 'T>(key:'K, values:seq<'T>) =
    interface System.Linq.IGrouping<'K, 'T> with
        member x.Key = key

    interface System.Collections.IEnumerable with
        member x.GetEnumerator() = values.GetEnumerator() :> System.Collections.IEnumerator

    interface System.Collections.Generic.IEnumerable<'T> with
        member x.GetEnumerator() = values.GetEnumerator()


module internal Adapters = 

    let memoize f = 
         let d = new System.Collections.Concurrent.ConcurrentDictionary<Type,'b>(HashIdentity.Structural)        
         fun x -> d.GetOrAdd(x, fun r -> f r)

    let isPartiallyImmutableRecord : Type -> bool = 
        memoize (fun t -> 
             FSharpType.IsRecord t && 
             not (FSharpType.GetRecordFields t |> Array.forall (fun f -> f.CanWrite)) )

    let MemberInitializationHelperMeth = 
        methodhandleof (fun x -> LeafExpressionConverter.MemberInitializationHelper x)
        |> System.Reflection.MethodInfo.GetMethodFromHandle 
        :?> System.Reflection.MethodInfo

    let NewAnonymousObjectHelperMeth = 
        methodhandleof (fun x -> LeafExpressionConverter.NewAnonymousObjectHelper x)
        |> System.Reflection.MethodInfo.GetMethodFromHandle 
        :?> System.Reflection.MethodInfo

    // The following patterns are used to recognize object construction 
    // using the 'new O(Prop1 = <e>, Prop2 = <e>)' syntax

    /// Recognize sequential series written as (... ((<e>; <e>); <e>); ...)
    let (|LeftSequentialSeries|) e =
        let rec leftSequentialSeries acc e =
            match e with 
            | Patterns.Sequential(e1, e2) -> leftSequentialSeries (e2 :: acc) e1
            | _ -> e :: acc
        leftSequentialSeries [] e

    /// Tests whether a list consists only of assignments of properties of the 
    /// given variable, null values (ignored) and ends by returning the given variable
    /// (pattern returns only property assignments)
    let (|PropSetList|_|) varArg (list:Expr list) =
        let rec propSetList acc x = 
            match x with 
            // detect " v.X <- y"
            | ((Patterns.PropertySet(Some(Patterns.Var var), _, _, _)) as p) :: xs when var = varArg ->
                propSetList (p :: acc) xs
            // skip unit values
            | (Patterns.Value (v, _)) :: xs when v = null -> propSetList acc xs
            // detect "v"
            | [Patterns.Var var] when var = varArg -> Some acc
            | _ -> None
        propSetList [] list

    /// Recognize object construction written using 'new O(Prop1 = <e>, Prop2 = <e>, ...)'
    let (|ObjectConstruction|_|) e = 
        match e with
        | Patterns.Let ( var, (Patterns.NewObject(_, []) as init), LeftSequentialSeries propSets ) ->
            match propSets with 
            | PropSetList var propSets -> Some(var, init, propSets)
            | _ -> None
        | _ -> None



    // Get arrays of types & map of transformations
    let tupleTypes = 
      [|  typedefof<System.Tuple<_>>,               typedefof<AnonymousObject<_>>
          typedefof<_ * _>,                         typedefof<AnonymousObject<_, _>>
          typedefof<_ * _ * _>,                     typedefof<AnonymousObject<_, _, _>>
          typedefof<_ * _ * _ * _>,                 typedefof<AnonymousObject<_, _, _, _>>
          typedefof<_ * _ * _ * _ * _>,             typedefof<AnonymousObject<_, _, _, _, _>>
          typedefof<_ * _ * _ * _ * _ * _>,         typedefof<AnonymousObject<_, _, _, _, _, _>>
          typedefof<_ * _ * _ * _ * _ * _ * _>,     typedefof<AnonymousObject<_, _, _, _, _, _, _>>
          typedefof<_ * _ * _ * _ * _ * _ * _ * _>, typedefof<AnonymousObject<_, _, _, _, _, _, _, _>> |]
    let anonObjectTypes = tupleTypes |> Array.map snd
    let tupleToAnonTypeMap = 
        let t = new Dictionary<Type,Type>()
        for (k,v) in tupleTypes do t.[k] <- v
        t

    let anonToTupleTypeMap = 
        let t = new Dictionary<Type,Type>()
        for (k,v) in tupleTypes do t.[v] <- k
        t


    /// Recognize anonymous type construction written using 'new AnonymousObject(<e1>, <e2>, ...)'
    let (|NewAnonymousObject|_|) e = 
        match e with
        | Patterns.NewObject(ctor,args) when 
                 let dty = ctor.DeclaringType 
                 dty.IsGenericType && anonToTupleTypeMap.ContainsKey (dty.GetGenericTypeDefinition()) -> 
             Some (ctor, args)
        | _ -> None

    let OneNewAnonymousObject (args:Expr list) =
        // Will fit into a single tuple type
        let typ = anonObjectTypes.[args.Length - 1]
        let typ = typ.MakeGenericType [| for a in args -> a.Type |]
        let ctor = typ.GetConstructors().[0]
        let res = Expr.NewObject (ctor, args)
        assert (match res with NewAnonymousObject _ -> true | _ -> false)
        res

    let rec NewAnonymousObject (args:Expr list) : Expr = 
        match args with 
        | x1 :: x2 :: x3 :: x4 :: x5 :: x6 :: x7 :: x8 :: tail ->
            // Too long to fit single tuple - nested tuple after first 7
            OneNewAnonymousObject [ x1; x2; x3; x4; x5; x6; x7; NewAnonymousObject (x8 :: tail) ]
        | args -> 
            OneNewAnonymousObject args

    let AnonymousObjectGet (e:Expr,i:int) = 
        // Recursively generate tuple get 
        // (may be nested e.g. TupleGet(<e>, 9) ~> <e>.Item8.Item3)
        let rec walk i (inst:Expr) (newType:Type) = 

            // Get property (at most the last one)
            let propInfo = newType.GetProperty ("Item"  + string (1 + min i 7))
            let res = Expr.PropertyGet (inst, propInfo)
            // Do we need to add another property get for the last property?
            if i < 7 then res 
            else walk (i - 7) res (newType.GetGenericArguments().[7]) 
            
        walk i e e.Type

    let RewriteTupleType (ty:Type) conv = 
        // Tuples are generic, so lookup only for generic types 
        assert ty.IsGenericType 
        let generic = ty.GetGenericTypeDefinition()
        match tupleToAnonTypeMap.TryGetValue generic with
        | true, mutableTupleType ->
            // Recursively transform type arguments
            mutableTupleType.MakeGenericType (ty.GetGenericArguments() |> Array.toList |> conv |> Array.ofList)
        | _ -> 
            assert false
            Printf.failwithf "unreachable, ty = %A" ty

    let (|RecordFieldGetSimplification|_|) (expr:Expr) = 
        match expr with 
        | Patterns.PropertyGet(Some (Patterns.NewRecord(typ,els)),propInfo,[]) ->
#if FX_RESHAPED_REFLECTION
            let fields = Microsoft.FSharp.Reflection.FSharpType.GetRecordFields(typ, true) 
#else
            let fields = Microsoft.FSharp.Reflection.FSharpType.GetRecordFields(typ,System.Reflection.BindingFlags.Public|||System.Reflection.BindingFlags.NonPublic) 
#endif
            match fields |> Array.tryFindIndex (fun p -> p = propInfo) with 
            | None -> None
            | Some i -> if i < els.Length then Some els.[i] else None
        | _ -> None


    /// The generic MethodInfo for Select function
    /// Describes how we got from productions of immutable objects to productions of anonymous objects, with enough information
    /// that we can invert the process in final query results.
    [<NoComparison; NoEquality>]
    type ConversionDescription = 
        | TupleConv of ConversionDescription list
        | RecordConv of Type * ConversionDescription list
        | GroupingConv of (* origKeyType: *) Type * (* origElemType: *) Type * ConversionDescription
        | SeqConv of ConversionDescription
        | NoConv

    /// Given an type involving immutable tuples and records, logically corresponding to the type produced at a
    /// "yield" or "select", convert it to a type involving anonymous objects according to the conversion data.
    let rec ConvImmutableTypeToMutableType conv ty = 
        match conv with 
        | TupleConv convs -> 
            assert (FSharpType.IsTuple ty)
            match convs with 
            | x1 :: x2 :: x3 :: x4 :: x5 :: x6 :: x7 :: x8 :: tail ->
                RewriteTupleType ty (List.map2 ConvImmutableTypeToMutableType [x1;x2;x3;x4;x5;x6;x7;TupleConv (x8 :: tail)])
            | _ -> 
                RewriteTupleType ty (List.map2 ConvImmutableTypeToMutableType convs)
        | RecordConv (_,convs) -> 
            assert (isPartiallyImmutableRecord ty)
            let types = [| for f in FSharpType.GetRecordFields ty -> f.PropertyType |]
            ConvImmutableTypeToMutableType (TupleConv convs) (FSharpType.MakeTupleType types) 
        | GroupingConv (_keyTy,_elemTy,conv) -> 
            assert ty.IsGenericType 
            assert (ty.GetGenericTypeDefinition() = typedefof<System.Linq.IGrouping<_, _>>)
            let keyt1 = ty.GetGenericArguments().[0]
            let valt1 = ty.GetGenericArguments().[1]
            typedefof<System.Linq.IGrouping<_, _>>.MakeGenericType [| keyt1; ConvImmutableTypeToMutableType conv valt1 |]
        | SeqConv conv -> 
            assert ty.IsGenericType
            let isIQ = ty.GetGenericTypeDefinition() = typedefof<IQueryable<_>>
            assert (ty.GetGenericTypeDefinition() = typedefof<seq<_>> || ty.GetGenericTypeDefinition() = typedefof<IQueryable<_>>)
            let elemt1 = ty.GetGenericArguments().[0]
            let args = [| ConvImmutableTypeToMutableType conv elemt1 |]
            if isIQ then typedefof<IQueryable<_>>.MakeGenericType args else typedefof<seq<_>>.MakeGenericType args
        | NoConv -> ty

    let IsNewAnonymousObjectHelperQ = 
        let mhandle = (methodhandleof (fun x -> LeafExpressionConverter.NewAnonymousObjectHelper x))
        let minfo = (System.Reflection.MethodInfo.GetMethodFromHandle mhandle) :?> System.Reflection.MethodInfo
        let gmd = minfo.GetGenericMethodDefinition() 
        (fun tm -> 
            match tm with
            | Patterns.Call(_obj,minfo2,_args) -> minfo2.IsGenericMethod && (gmd = minfo2.GetGenericMethodDefinition()) 
            | _ -> false)

    /// Cleanup the use of property-set object constructions in leaf expressions that form parts of F# queries.
    let rec CleanupLeaf expr = 
        if IsNewAnonymousObjectHelperQ expr then expr else // this has already been cleaned up, don't do it twice

        // rewrite bottom-up
        let expr = 
            match expr with 
            | ExprShape.ShapeCombination(comb,args) -> match args with [] -> expr | _ -> ExprShape.RebuildShapeCombination(comb,List.map CleanupLeaf args)
            | ExprShape.ShapeLambda(v,body) -> Expr.Lambda (v, CleanupLeaf body)
            | ExprShape.ShapeVar _ -> expr
        match expr with 

        // Detect all object construction expressions - wrap them in 'MemberInitializationHelper'
        // so that it can be translated to Expression.MemberInit 
        | ObjectConstruction(var, init, propSets) ->
            // Wrap object initialization into a value (
            let methInfo = MemberInitializationHelperMeth.MakeGenericMethod [|  var.Type |]
            Expr.Call (methInfo, [ List.reduceBack (fun a b -> Expr.Sequential (a,b)) (propSets @ [init]) ])

        // Detect all anonymous type constructions - wrap them in 'NewAnonymousObjectHelper'
        // so that it can be translated to Expression.New with member arguments.
        | NewAnonymousObject(ctor, args) ->
            let methInfo = NewAnonymousObjectHelperMeth.MakeGenericMethod [|  ctor.DeclaringType |]
            Expr.Call (methInfo, [ Expr.NewObject (ctor,args) ])
        | expr -> 
            expr

    /// Simplify gets of tuples and gets of record fields.
    let rec SimplifyConsumingExpr e = 
        // rewrite bottom-up
        let e = 
            match e with 
            | ExprShape.ShapeCombination(comb,args) -> ExprShape.RebuildShapeCombination(comb,List.map SimplifyConsumingExpr args)
            | ExprShape.ShapeLambda(v,body) -> Expr.Lambda (v, SimplifyConsumingExpr body)
            | ExprShape.ShapeVar _ -> e
        match e with
        | Patterns.TupleGet(Patterns.NewTuple els,i) -> els.[i]
        | RecordFieldGetSimplification newExpr -> newExpr 
        | _ -> e

    /// Given the expression part of a "yield" or "select" which produces a result in terms of immutable tuples or immutable records,
    /// generate an equivalent expression yielding anonymous objects. Also return the conversion for the immutable-to-mutable correspondence
    /// so we can reverse this later.
    let rec ProduceMoreMutables tipf expr = 

        match expr with 
        // Replace immutable tuples by anonymous objects 
        | Patterns.NewTuple exprs -> 
            let argExprsNow, argScripts = exprs |> List.map (ProduceMoreMutables tipf) |> List.unzip
            NewAnonymousObject argExprsNow, TupleConv  argScripts

        // Replace immutable records by anonymous objects 
        | Patterns.NewRecord(typ, args) when isPartiallyImmutableRecord typ ->
            let argExprsNow, argScripts = args |> List.map (ProduceMoreMutables tipf) |> List.unzip
            NewAnonymousObject argExprsNow, RecordConv(typ, argScripts)

        | expr -> 
            tipf expr

    let MakeSeqConv conv = match conv with NoConv -> NoConv | _ -> SeqConv conv


