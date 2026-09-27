// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.FSharp.Linq.RuntimeHelpers

open System
open Microsoft.FSharp.Core
open Microsoft.FSharp.Core.LanguagePrimitives.IntrinsicOperators
open Microsoft.FSharp.Collections
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Reflection
open Microsoft.FSharp.Linq.RuntimeHelpers
open System.Collections
open System.Collections.Concurrent
open System.Collections.Generic
open System.Linq
open System.Reflection

// ----------------------------------------------------------------------------

/// A type used to reconstruct a grouping after applying a mutable->immutable mapping transformation
/// on a result of a query.
type Grouping<'K, 'T>(key: 'K, values: seq<'T>) =
    interface IGrouping<'K, 'T> with
        member _.Key = key

    interface IEnumerable with
        member _.GetEnumerator() =
            values.GetEnumerator() :> IEnumerator

    interface Generic.IEnumerable<'T> with
        member _.GetEnumerator() =
            values.GetEnumerator()

/// The groupings rebuilt so far, by the grouping each was rebuilt from. A query that mentions a group in several
/// places, or goes through a group of groups twice, gets one and the same object for a group every time.
type internal RebuiltGroupings<'K, 'T> private () =
    static let rebuilt =
        System.Runtime.CompilerServices.ConditionalWeakTable<obj, Grouping<'K, 'T>>()

    static member Of(source: obj, key: 'K, values: seq<'T>) : IGrouping<'K, 'T> =
        match rebuilt.TryGetValue source with
        | true, grouping -> grouping
        | _ -> rebuilt.GetValue(source, fun _ -> Grouping<'K, 'T>(key, values))

module internal Adapters =

    let memoize f =
        let d = new ConcurrentDictionary<Type, 'b>(HashIdentity.Structural)
        // Cache this as a delegate.
        let valueFactory = Func<Type, 'b> f

        fun x -> d.GetOrAdd(x, valueFactory)

    let isPartiallyImmutableRecord: Type -> bool =
        memoize (fun t ->
            FSharpType.IsRecord t
            && not (FSharpType.GetRecordFields t |> Array.forall (fun f -> f.CanWrite)))

    let MemberInitializationHelperMeth =
        methodhandleof LeafExpressionConverter.MemberInitializationHelper
        |> MethodInfo.GetMethodFromHandle
        :?> MethodInfo

    let NewAnonymousObjectHelperMeth =
        methodhandleof LeafExpressionConverter.NewAnonymousObjectHelper
        |> MethodInfo.GetMethodFromHandle
        :?> MethodInfo

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
    let (|PropSetList|_|) varArg (list: Expr list) =
        let rec propSetList acc x =
            match x with
            // detect " v.X <- y"
            | Patterns.PropertySet(Some(Patterns.Var var), _, _, _) as p :: xs when var = varArg ->
                propSetList (p :: acc) xs
            // skip unit values
            | Patterns.Value(v, _) :: xs when isNull v -> propSetList acc xs
            // detect "v"
            | [ Patterns.Var var ] when var = varArg -> Some acc
            | _ -> None

        propSetList [] list

    /// Recognize object construction written using 'new O(Prop1 = <e>, Prop2 = <e>, ...)'
    let (|ObjectConstruction|_|) e =
        match e with
        | Patterns.Let(var, (Patterns.NewObject(_, []) as init), LeftSequentialSeries propSets) ->
            match propSets with
            | PropSetList var propSets -> Some(var, init, propSets)
            | _ -> None
        | _ -> None

    // Get arrays of types & map of transformations
    let tupleTypes =
        [|
            typedefof<System.Tuple<_>>, typedefof<AnonymousObject<_>>
            typedefof<_ * _>, typedefof<AnonymousObject<_, _>>
            typedefof<_ * _ * _>, typedefof<AnonymousObject<_, _, _>>
            typedefof<_ * _ * _ * _>, typedefof<AnonymousObject<_, _, _, _>>
            typedefof<_ * _ * _ * _ * _>, typedefof<AnonymousObject<_, _, _, _, _>>
            typedefof<_ * _ * _ * _ * _ * _>, typedefof<AnonymousObject<_, _, _, _, _, _>>
            typedefof<_ * _ * _ * _ * _ * _ * _>, typedefof<AnonymousObject<_, _, _, _, _, _, _>>
            typedefof<_ * _ * _ * _ * _ * _ * _ * _>, typedefof<AnonymousObject<_, _, _, _, _, _, _, _>>
        |]

    let anonObjectTypes = tupleTypes |> Array.map snd

    let anonToTupleTypeMap =
        let t = Dictionary<Type, Type>()

        for k, v in tupleTypes do
            t.[v] <- k

        t

    /// Recognize anonymous type construction written using 'new AnonymousObject(<e1>, <e2>, ...)'
    let (|NewAnonymousObject|_|) e =
        match e with
        | Patterns.NewObject(ctor, args) when
            let dty = ctor.DeclaringType

            dty.IsGenericType
            && anonToTupleTypeMap.ContainsKey(dty.GetGenericTypeDefinition())
            ->
            Some(ctor, args)
        | _ -> None

    let OneNewAnonymousObject (args: Expr list) =
        // Will fit into a single tuple type
        let typ = anonObjectTypes.[args.Length - 1]
        let typ = typ.MakeGenericType [| for a in args -> a.Type |]
        let ctor = typ.GetConstructors().[0]
        let res = Expr.NewObject(ctor, args)

        assert
            (match res with
             | NewAnonymousObject _ -> true
             | _ -> false)

        res

    let rec NewAnonymousObject (args: Expr list) : Expr =
        match args with
        | x1 :: x2 :: x3 :: x4 :: x5 :: x6 :: x7 :: x8 :: tail ->
            // Too long to fit single tuple - nested tuple after first 7
            OneNewAnonymousObject [ x1; x2; x3; x4; x5; x6; x7; NewAnonymousObject(x8 :: tail) ]
        | args -> OneNewAnonymousObject args

    let AnonymousObjectGet (e: Expr, i: int) =
        // Recursively generate tuple get
        // (may be nested e.g. TupleGet(<e>, 9) ~> <e>.Item8.Item3)
        let rec walk i (inst: Expr) (newType: Type) =

            // Get property (at most the last one)
            let propInfo = newType.GetProperty("Item" + string (1 + min i 7))
            let res = Expr.PropertyGet(inst, propInfo)
            // Do we need to add another property get for the last property?
            if i < 7 then
                res
            else
                walk (i - 7) res (newType.GetGenericArguments().[7])

        walk i e e.Type

    /// Reference and struct tuples of the same arity share one anonymous object type.
    let RewriteTupleType (ty: Type) conv =
        assert (FSharpType.IsTuple ty)
        let elementTypes = ty.GetGenericArguments()
        let mutableTupleType = anonObjectTypes.[elementTypes.Length - 1]
        // Recursively transform type arguments
        mutableTupleType.MakeGenericType(elementTypes |> Array.toList |> conv |> Array.ofList)

    let (|RecordFieldGetSimplification|_|) (expr: Expr) =
        match expr with
        | Patterns.PropertyGet(Some(Patterns.NewRecord(typ, els)), propInfo, []) ->
            let fields =
                FSharpType.GetRecordFields(typ, BindingFlags.Public ||| BindingFlags.NonPublic)

            match fields |> Array.tryFindIndex (fun p -> p = propInfo) with
            | None -> None
            | Some i ->
                if i < els.Length then
                    Some els.[i]
                else
                    None
        | _ -> None

    /// The generic MethodInfo for Select function
    /// Describes how we got from productions of immutable objects to productions of anonymous objects, with enough information
    /// that we can invert the process in final query results.
    [<NoComparison; NoEquality>]
    type ConversionDescription =
        | TupleConv of isStruct: bool * ConversionDescription list
        | RecordConv of Type * ConversionDescription list
        | GroupingConv (* origKeyType: *) of Type (* origElemType: *) * Type * ConversionDescription
        | SeqConv of ConversionDescription
        | NoConv

    /// Given an type involving immutable tuples and records, logically corresponding to the type produced at a
    /// "yield" or "select", convert it to a type involving anonymous objects according to the conversion data.
    let rec ConvImmutableTypeToMutableType conv ty =
        match conv with
        | TupleConv(isStruct, convs) ->
            assert (FSharpType.IsTuple ty)

            match convs with
            | x1 :: x2 :: x3 :: x4 :: x5 :: x6 :: x7 :: x8 :: tail ->
                let els = [ x1; x2; x3; x4; x5; x6; x7; TupleConv(isStruct, x8 :: tail) ]
                RewriteTupleType ty (List.map2 ConvImmutableTypeToMutableType els)
            | _ -> RewriteTupleType ty (List.map2 ConvImmutableTypeToMutableType convs)
        | RecordConv(_, convs) ->
            assert (isPartiallyImmutableRecord ty)
            let types = [| for f in FSharpType.GetRecordFields ty -> f.PropertyType |]
            ConvImmutableTypeToMutableType (TupleConv(false, convs)) (FSharpType.MakeTupleType types)
        | GroupingConv(_keyTy, _elemTy, conv) ->
            assert ty.IsGenericType
            assert (ty.GetGenericTypeDefinition() = typedefof<IGrouping<_, _>>)
            let keyt1 = ty.GetGenericArguments().[0]
            let valt1 = ty.GetGenericArguments().[1]
            typedefof<IGrouping<_, _>>.MakeGenericType [| keyt1; ConvImmutableTypeToMutableType conv valt1 |]
        | SeqConv conv ->
            assert ty.IsGenericType
            let isIQ = ty.GetGenericTypeDefinition() = typedefof<IQueryable<_>>

            assert
                (ty.GetGenericTypeDefinition() = typedefof<seq<_>>
                 || ty.GetGenericTypeDefinition() = typedefof<IQueryable<_>>)

            let elemt1 = ty.GetGenericArguments().[0]
            let args = [| ConvImmutableTypeToMutableType conv elemt1 |]

            if isIQ then
                typedefof<IQueryable<_>>.MakeGenericType args
            else
                typedefof<seq<_>>.MakeGenericType args
        | NoConv -> ty

    let IsNewAnonymousObjectHelperQ =
        let mhandle = (methodhandleof LeafExpressionConverter.NewAnonymousObjectHelper)

        let minfo = (MethodInfo.GetMethodFromHandle mhandle) :?> MethodInfo

        let gmd = minfo.GetGenericMethodDefinition()

        (fun tm ->
            match tm with
            | Patterns.Call(_obj, minfo2, _args) ->
                minfo2.IsGenericMethod && (gmd = minfo2.GetGenericMethodDefinition())
            | _ -> false)

    /// Cleanup the use of property-set object constructions in leaf expressions that form parts of F# queries.
    let rec CleanupLeaf expr =
        if IsNewAnonymousObjectHelperQ expr then
            expr
        else // this has already been cleaned up, don't do it twice

            // rewrite bottom-up
            let expr =
                match expr with
                | ExprShape.ShapeCombination(comb, args) ->
                    match args with
                    | [] -> expr
                    | _ -> ExprShape.RebuildShapeCombination(comb, List.map CleanupLeaf args)
                | ExprShape.ShapeLambda(v, body) -> Expr.Lambda(v, CleanupLeaf body)
                | ExprShape.ShapeVar _ -> expr

            match expr with

            // Detect all object construction expressions - wrap them in 'MemberInitializationHelper'
            // so that it can be translated to Expression.MemberInit
            | ObjectConstruction(var, init, propSets) ->
                // Wrap object initialization into a value (
                let methInfo = MemberInitializationHelperMeth.MakeGenericMethod [| var.Type |]
                Expr.Call(methInfo, [ List.reduceBack (fun a b -> Expr.Sequential(a, b)) (propSets @ [ init ]) ])

            // Detect all anonymous type constructions - wrap them in 'NewAnonymousObjectHelper'
            // so that it can be translated to Expression.New with member arguments.
            | NewAnonymousObject(ctor, args) ->
                let methInfo =
                    NewAnonymousObjectHelperMeth.MakeGenericMethod [| ctor.DeclaringType |]

                Expr.Call(methInfo, [ Expr.NewObject(ctor, args) ])
            | expr -> expr

    /// A grouping rebuilt from the provider's grouping of converted elements, where a query mentions the group.
    [<return: Struct>]
    let rec (|RebuiltGrouping|_|) (expr: Expr) =
        match expr with
        | Patterns.Coerce(RebuiltGrouping grouping, _) -> ValueSome grouping
        | Patterns.Call(None, rebuild, [ _; key; elements ]) ->
            match rebuild.DeclaringType with
            | null -> ValueNone
            | owner when
                owner.IsGenericType
                && owner.GetGenericTypeDefinition() = typedefof<RebuiltGroupings<_, _>>
                ->
                ValueSome struct (key, elements)
            | _ -> ValueNone
        | _ -> ValueNone

    let isEnumerableMethod (methodInfo: MethodInfo) =
        Type.op_Equality (methodInfo.DeclaringType, typeof<Enumerable>)

    let isSequenceType (ty: Type) =
        ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<seq<_>>

    [<return: Struct>]
    let (|EnumerableSelect|_|) (expr: Expr) =
        match expr with
        | Patterns.Call(None, select, [ source; Patterns.NewDelegate(_, [ element ], projection) ]) when
            isEnumerableMethod select && select.Name = "Select"
            ->
            ValueSome struct (source, element, projection)
        | _ -> ValueNone

    /// A member of what the translation itself puts into a query: an item of an anonymous object, and under a
    /// grouping it rebuilds also the key of the grouping it comes from. Reading one has no effect and cannot fail.
    let isTranslationMember (ofGrouping: bool) (property: PropertyInfo) =
        let owner = property.DeclaringType

        owner.IsGenericType
        && (let definition = owner.GetGenericTypeDefinition()

            Array.contains definition anonObjectTypes
            || (ofGrouping && definition = typedefof<IGrouping<_, _>>))

    /// Whether the expression only takes apart what the translation put together and puts it together again:
    /// reads of the items of an anonymous object, tuples and records of those, and the same for a sequence or a
    /// grouping it holds. A projection the query itself wrote over its own types is never that.
    let rec rebuildsElementIn (ofGrouping: bool) (element: Var) (expr: Expr) =
        let rec readsElement (expr: Expr) =
            match expr with
            | Patterns.Var v -> v = element
            | Patterns.PropertyGet(Some source, property, []) ->
                isTranslationMember ofGrouping property && readsElement source
            | Patterns.Coerce(source, _) -> readsElement source
            | _ -> false

        match expr with
        | Patterns.NewTuple parts
        | Patterns.NewRecord(_, parts) -> List.forall (rebuildsElementIn ofGrouping element) parts
        | Patterns.Coerce(inner, _) -> rebuildsElementIn ofGrouping element inner
        | EnumerableSelect(source, inner, projection) -> readsElement source && rebuildsElementIn false inner projection
        | RebuiltGrouping(key, elements) ->
            rebuildsElementIn true element key && rebuildsElementIn true element elements
        | expr -> readsElement expr

    let rebuildsElement (element: Var) (expr: Expr) =
        rebuildsElementIn false element expr

    /// A group is rebuilt into the same object each time
    let rebuildsOneObject (projection: Expr) =
        match projection with
        | RebuiltGrouping _ -> true
        | _ -> false

    /// Whether the expression uses the variable as a value of its own, and not only for its members
    let rec usesWhole (v: Var) (expr: Expr) =
        match expr with
        | Patterns.TupleGet(Patterns.Var _, _)
        | Patterns.PropertyGet(Some(Patterns.Var _), _, []) -> false
        | ExprShape.ShapeVar used -> used = v
        | ExprShape.ShapeLambda(_, body) -> usesWhole v body
        | ExprShape.ShapeCombination(_, args) -> List.exists (usesWhole v) args

    /// 'Enumerable.M(source.Select(element => rebuilt), f)' as 'Enumerable.M(source, element => f(rebuilt))', and
    /// 'source.Select(element => rebuilt).Count()' as 'source.Count()'. Only for a projection that rebuilds the
    /// element, and an M whose result holds none of the elements: for anything else the projection has to run
    /// where it is written.
    [<return: Struct>]
    let (|SelectFusedIntoConsumer|_|) (expr: Expr) =
        match expr with
        | Patterns.Call(None, consumer, EnumerableSelect(source, element, projection) :: rest) when
            isEnumerableMethod consumer
            && consumer.IsGenericMethod
            && rebuildsElement element projection
            ->
            let definition = consumer.GetGenericMethodDefinition()
            let sourceParameter = definition.GetParameters().[0].ParameterType

            let fuse () =
                let elementParameter = sourceParameter.GetGenericArguments().[0]

                let typeArguments =
                    Array.map2
                        (fun (parameter: Type) argument ->
                            if Type.op_Equality (parameter, elementParameter) then
                                element.Type
                            else
                                argument)
                        (definition.GetGenericArguments())
                        (consumer.GetGenericArguments())

                definition.MakeGenericMethod typeArguments

            if not (isSequenceType sourceParameter) then
                ValueNone
            else
                match consumer.Name, rest with
                | ("Count" | "LongCount" | "Any"), [] ->
                    let fused = fuse ()

                    if Type.op_Equality (fused.ReturnType, consumer.ReturnType) then
                        ValueSome(Expr.Call(fused, [ source ]))
                    else
                        ValueNone
                // A lambda that uses the element as a value of its own gets it from the Select: there it is one
                // object, however often the lambda mentions it.
                | ("Count" | "LongCount" | "Any" | "All" | "Sum" | "Average" | "Min" | "Max" | "Select" | "SelectMany"),
                  [ Patterns.NewDelegate(_, [ consumed ], body) ] when
                    Type.op_Equality (consumed.Type, projection.Type)
                    && (rebuildsOneObject projection || not (usesWhole consumed body))
                    ->
                    let fused = fuse ()

                    if Type.op_Equality (fused.ReturnType, consumer.ReturnType) then
                        // A variable of its own: the same rebuilding function can already be in 'body'
                        let fresh = Var(consumed.Name, element.Type)

                        let rebuilt =
                            projection.Substitute(fun v ->
                                if v = element then
                                    Some(Expr.Var fresh)
                                else
                                    None)

                        let body =
                            body.Substitute(fun v ->
                                if v = consumed then
                                    Some rebuilt
                                else
                                    None)

                        let delegateTy = fused.GetParameters().[1].ParameterType
                        ValueSome(Expr.Call(fused, [ source; Expr.NewDelegate(delegateTy, [ fresh ], body) ]))
                    else
                        ValueNone
                | _ -> ValueNone
        | _ -> ValueNone

    /// Reads of a rebuilt grouping, taken from what it was rebuilt from.
    [<return: Struct>]
    let (|GroupingSimplification|_|) (expr: Expr) =
        match expr with
        | Patterns.PropertyGet(Some(RebuiltGrouping(key, _)), property, []) when property.Name = "Key" -> ValueSome key
        // 'AsEnumerable' and 'Cast' give back the grouping they are handed
        | Patterns.Call(None, consumer, RebuiltGrouping(_, elements) :: rest) when
            isEnumerableMethod consumer
            && consumer.Name <> "AsEnumerable"
            && consumer.Name <> "Cast"
            && isSequenceType (consumer.GetParameters().[0].ParameterType)
            && consumer.GetParameters().[0].ParameterType.IsAssignableFrom elements.Type
            ->
            ValueSome(Expr.Call(consumer, elements :: rest))
        | SelectFusedIntoConsumer fused -> ValueSome fused
        | _ -> ValueNone

    /// Simplify gets of tuples and gets of record fields.
    let rec SimplifyConsumingExpr e =
        // rewrite bottom-up
        let e =
            match e with
            | ExprShape.ShapeCombination(comb, args) ->
                ExprShape.RebuildShapeCombination(comb, List.map SimplifyConsumingExpr args)
            | ExprShape.ShapeLambda(v, body) -> Expr.Lambda(v, SimplifyConsumingExpr body)
            | ExprShape.ShapeVar _ -> e

        match e with
        | Patterns.TupleGet(Patterns.NewTuple els, i) -> els.[i]
        | RecordFieldGetSimplification newExpr -> newExpr
        | GroupingSimplification simpler -> SimplifyConsumingExpr simpler
        | _ -> e

    /// Given the expression part of a "yield" or "select" which produces a result in terms of immutable tuples or immutable records,
    /// generate an equivalent expression yielding anonymous objects. Also return the conversion for the immutable-to-mutable correspondence
    /// so we can reverse this later.
    let rec ProduceMoreMutables tipf expr =

        match expr with
        // Replace immutable tuples by anonymous objects
        | Patterns.NewTuple exprs ->
            let argExprsNow, argScripts =
                exprs |> List.map (ProduceMoreMutables tipf) |> List.unzip

            NewAnonymousObject argExprsNow, TupleConv(expr.Type.IsValueType, argScripts)

        // Replace immutable records by anonymous objects
        | Patterns.NewRecord(typ, args) when isPartiallyImmutableRecord typ ->
            let argExprsNow, argScripts =
                args |> List.map (ProduceMoreMutables tipf) |> List.unzip

            NewAnonymousObject argExprsNow, RecordConv(typ, argScripts)

        | expr -> tipf expr

    let MakeSeqConv conv =
        match conv with
        | NoConv -> NoConv
        | _ -> SeqConv conv
