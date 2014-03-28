namespace Microsoft.FSharp.Linq

open System
open System.Linq
open System.Collections.Generic
open System.Linq.Expressions
open System.Reflection
open System.Reflection.Emit
open Microsoft.FSharp
open Microsoft.FSharp.Linq.QuotationEvaluation
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open Microsoft.FSharp.Quotations.DerivedPatterns

#nowarn "57"
#nowarn "44" //deprecation 
  
    
module Query =

    let debug = false
        
    let contains key source = 
        System.Linq.Enumerable.Contains(source,key)

    let minBy<'T1,'T2> keySelector source = 
        System.Linq.Enumerable.Min(source,Func<'T1,'T2>(keySelector))

    let maxBy<'T1,'T2> keySelector source = 
        System.Linq.Enumerable.Max(source,Func<'T1,'T2>(keySelector))

    let groupBy keySelector source = 
        System.Linq.Enumerable.GroupBy(source,Func<_,_>(keySelector))

    let join  outerSource innerSource outerKeySelector innerKeySelector resultSelector = 
        System.Linq.Enumerable.Join(outerSource,innerSource,Func<_,_>(outerKeySelector),Func<_,_>(innerKeySelector),Func<_,_,_>(resultSelector))

    let groupJoin  outerSource innerSource outerKeySelector innerKeySelector resultSelector = 
        System.Linq.Enumerable.GroupJoin(outerSource,innerSource,Func<_,_>(outerKeySelector),Func<_,_>(innerKeySelector),Func<_,_,_>(resultSelector))

    let ConvVar (v: Var) = 
        Expression.Parameter(v.Type, v.Name)

    let asExpr x = (x :> Expression)
            
    let (|Getter|_|) (prop: #PropertyInfo) =
        match prop.GetGetMethod(true) with 
        | null -> None
        | v -> Some v

    let (|CallPipe|_|) = (|SpecificCall|_|) <@ (|>) @>
    // Match 'f x' or 'x |> f' or 'x |> (fun x -> f (x :> ty))'
    let (|SpecificPipedCall0|_|) q = 
       let (|CallQ|_|) = (|SpecificCall|_|) q
       function
       | CallPipe (None, [_;_],[arg1;Lambda(arg1v,CallQ (None, tyargs,[arg1E])) ]) -> 
           let arg1 = arg1E.Substitute (Map.ofSeq [ (arg1v,arg1) ]).TryFind 
           Some(tyargs,arg1)

       | CallQ (None, tyargs,[arg1]) -> 
           Some(tyargs,arg1)

       | _ -> None           

    // Match 
    //     'f x y' or 
    //     'y |> f x' or
    //     'y |> (fun y -> f (x :> ty) (y :> ty))'
    //     'y |> let x = e in (fun y -> f (x :> ty) (y :> ty))'
    let (|SpecificPipedCall1|_|) q = 
       let (|CallQ|_|) = (|SpecificCall|_|) q
       function
       // Encoded form of some uses of 'T1rg2 |> f arg1'
       | CallPipe (None, [_;_],[arg2;Let(arg1v,arg1,Lambda(arg2v,CallQ (None, tyargs,[arg1E;arg2E]))) ]) -> 
              let arg1 = arg1E.Substitute (Map.ofSeq [ (arg1v,arg1) ]).TryFind 
              let arg2 = arg2E.Substitute (Map.ofSeq [ (arg2v,arg2) ]).TryFind 
              Some(tyargs,arg1,arg2)

       | CallPipe (None, [_;_],[arg2;Lambda(arg2v,CallQ (None, tyargs,[arg1;arg2E])) ]) -> 
              let arg2 = arg2E.Substitute (Map.ofSeq [ (arg2v,arg2) ]).TryFind 
              Some(tyargs,arg1,arg2)

       | CallQ (None, tyargs,[arg1;arg2]) -> 
              Some(tyargs,arg1,arg2)
       | _ -> None           

        
    let GetGenericMethodDefinition (m:MethodInfo) = 
        if m.IsGenericMethod then m.GetGenericMethodDefinition() else m

    let FindGenericStaticMethodInfo mexpr =
        match mexpr with 
        | Lambdas(_,Call(None,methInfo,_)) -> GetGenericMethodDefinition methInfo
        | _ -> failwithf "FindGenericStaticMethodInfo: %A is not a static method call lambda" mexpr

    let CallGenericStaticMethod mexpr  =
        let m = FindGenericStaticMethodInfo mexpr in
        //printf "m = %A\n" m;
        fun (tyargs,args) -> 
            //printf "invoking %A\n" m;
            
            let m = 
                if m.IsGenericMethod then 
                    m.MakeGenericMethod(Array.ofList tyargs) 
                else
                    m
            m.Invoke(null,Array.ofList args)

    let FT1 = typedefof<System.Func<_,_>>
    let FT2 = typedefof<System.Func<_,_,_>>
    let boolTy = typeof<bool>
    let MakeQueryFuncTy (dty,rty) = FT1.MakeGenericType([| dty; rty |])
    let MakeQueryFunc2Ty (dty1,dty2,rty) = FT2.MakeGenericType([| dty1; dty2; rty |])

    let IEnumerableTypeDef = typedefof<IEnumerable<_>>
    let IQueryableTypeDef = typedefof<IQueryable<_>>
    let MakeIEnumerableTy dty= IEnumerableTypeDef.MakeGenericType([| dty|])
    let MakeIQueryableTy dty= IQueryableTypeDef.MakeGenericType([| dty|])

    let isNamedType(typ:Type) = not (typ.IsArray || typ.IsByRef || typ.IsPointer)
    let equivHeadTypes (ty1:Type) (ty2:Type) = 
        isNamedType(ty1) &&
        if ty1.IsGenericType then 
          ty2.IsGenericType && (ty1.GetGenericTypeDefinition()).Equals(ty2.GetGenericTypeDefinition())
        else 
          ty1.Equals(ty2)

    let IsIQueryableTy ty = equivHeadTypes IQueryableTypeDef ty
   

    let CallSeqToList = 
        let F = CallGenericStaticMethod <@ Seq.toList @> 
        fun (srcTy,src) -> 
            F ([srcTy],[src])

    let CallSeqToArray = 
        let F = CallGenericStaticMethod <@ Seq.toArray @> 
        fun (srcTy,src) -> 
            F ([srcTy],[src])

    let CallQueryableContains = 
        let F = CallGenericStaticMethod <@ System.Linq.Queryable.Contains : _ * _ -> _ @> 
        fun (srcTy,src,key:Expression) -> 
            F ([srcTy],[src;box key])

    let CallQueryableMinBy = 
        let F = CallGenericStaticMethod <@ System.Linq.Queryable.Min : _ * Expression<Func<_,_>> -> _ @> 
        fun (srcTy,destTy,src,predicate:Expression) -> 
            F ([srcTy;destTy],[src;box predicate])

    let CallQueryableMin = 
        let F = CallGenericStaticMethod <@ System.Linq.Queryable.Min : _ -> _ @> 
        fun (srcTy,src) -> 
            F ([srcTy],[src])

    let CallQueryableMaxBy = 
        let F = CallGenericStaticMethod <@ System.Linq.Queryable.Max : _ * Expression<Func<_,_>> -> _ @> 
        fun (srcTy,destTy,src,predicate:Expression) -> 
            F ([srcTy;destTy],[src;box predicate])

    let CallQueryableMax = 
        let F = CallGenericStaticMethod <@ System.Linq.Queryable.Max : _ -> _ @> 
        fun (srcTy,src) -> 
            F ([srcTy],[src])

    let CallQueryableAverageBy = 
        let F_float = CallGenericStaticMethod <@ System.Linq.Queryable.Average : _ * Expression<Func<_,float>> -> _ @> 
        let F_float32 = CallGenericStaticMethod <@ System.Linq.Queryable.Average : _ * Expression<Func<_,float32>> -> _ @> 
        let F_decimal = CallGenericStaticMethod <@ System.Linq.Queryable.Average : _ * Expression<Func<_,decimal>> -> _ @> 
        // Note these don't satisfy the F# constraints anyway
        let F_int32 = CallGenericStaticMethod <@ System.Linq.Queryable.Average : _ * Expression<Func<_,int32>> -> _ @> 
        let F_int64 = CallGenericStaticMethod <@ System.Linq.Queryable.Average : _ * Expression<Func<_,int64>> -> _ @> 
        fun (srcTy,destTy,src,predicate:Expression) -> 
            match srcTy with 
            | ty when ty = typeof<float> -> F_float ([destTy],[src;box predicate])
            | ty when ty = typeof<float32> -> F_float32 ([destTy],[src;box predicate])
            | ty when ty = typeof<decimal> -> F_decimal ([destTy],[src;box predicate])
            | ty when ty = typeof<int32> -> F_int32 ([destTy],[src;box predicate])
            | ty when ty = typeof<int64> -> F_int64 ([destTy],[src;box predicate])
            | _ -> failwith "unrecognized use of 'Seq.averageBy'"


    let CallQueryableAverage = 
        let F_float = CallGenericStaticMethod <@ System.Linq.Queryable.Average : IQueryable<float> -> _ @> 
        let F_float32 = CallGenericStaticMethod <@ System.Linq.Queryable.Average : IQueryable<float32>  -> _ @> 
        let F_decimal = CallGenericStaticMethod <@ System.Linq.Queryable.Average : IQueryable<decimal>  -> _ @> 
        // Note these don't satisfy the F# constraints anyway
        let F_int32 = CallGenericStaticMethod <@ System.Linq.Queryable.Average : IQueryable<int32>  -> _ @> 
        let F_int64 = CallGenericStaticMethod <@ System.Linq.Queryable.Average : IQueryable<int64>  -> _ @> 
        fun (srcTy,src) -> 
            match srcTy with 
            | ty when ty = typeof<float> -> F_float ([],[src])
            | ty when ty = typeof<float32> -> F_float32 ([],[src])
            | ty when ty = typeof<decimal> -> F_decimal ([],[src])
            | ty when ty = typeof<int32> -> F_int32 ([],[src])
            | ty when ty = typeof<int64> -> F_int64 ([],[src])
            | _ -> failwith "unrecognized use of 'Seq.average'"

    let CallQueryableSumBy = 
        let F_float = CallGenericStaticMethod <@ System.Linq.Queryable.Sum : _ * Expression<Func<_,float>> -> _ @> 
        let F_float32 = CallGenericStaticMethod <@ System.Linq.Queryable.Sum : _ * Expression<Func<_,float32>> -> _ @> 
        let F_decimal = CallGenericStaticMethod <@ System.Linq.Queryable.Sum : _ * Expression<Func<_,decimal>> -> _ @> 
        let F_int32 = CallGenericStaticMethod <@ System.Linq.Queryable.Sum : _ * Expression<Func<_,int32>> -> _ @> 
        let F_int64 = CallGenericStaticMethod <@ System.Linq.Queryable.Sum : _ * Expression<Func<_,int64>> -> _ @> 
        fun (srcTy,destTy,src,predicate:Expression) -> 
            match srcTy with 
            | ty when ty = typeof<float> -> F_float ([destTy],[src;box predicate])
            | ty when ty = typeof<float32> -> F_float32 ([destTy],[src;box predicate])
            | ty when ty = typeof<decimal> -> F_decimal ([destTy],[src;box predicate])
            | ty when ty = typeof<int32> -> F_int32 ([destTy],[src;box predicate])
            | ty when ty = typeof<int64> -> F_int64 ([destTy],[src;box predicate])
            | _ -> failwith "unrecognized use of 'Seq.sumBy'"

    let CallQueryableSum = 
        let F_float = CallGenericStaticMethod <@ System.Linq.Queryable.Sum : IQueryable<float> -> _ @> 
        let F_float32 = CallGenericStaticMethod <@ System.Linq.Queryable.Sum : IQueryable<float32>  -> _ @> 
        let F_decimal = CallGenericStaticMethod <@ System.Linq.Queryable.Sum : IQueryable<decimal>  -> _ @> 
        // Note these don't satisfy the F# constraints anyway
        let F_int32 = CallGenericStaticMethod <@ System.Linq.Queryable.Sum : IQueryable<int32>  -> _ @> 
        let F_int64 = CallGenericStaticMethod <@ System.Linq.Queryable.Sum : IQueryable<int64>  -> _ @> 
        fun (srcTy,src) -> 
            match srcTy with 
            | ty when ty = typeof<float> -> F_float ([],[src])
            | ty when ty = typeof<float32> -> F_float32 ([],[src])
            | ty when ty = typeof<decimal> -> F_decimal ([],[src])
            | ty when ty = typeof<int32> -> F_int32 ([],[src])
            | ty when ty = typeof<int64> -> F_int64 ([],[src])
            | _ -> failwith "unrecognized use of 'Seq.sum"

    let CallQueryableFirst = 
        let F = CallGenericStaticMethod <@ System.Linq.Queryable.First : _ -> _ @> 
        fun (srcTy,src) -> 
            F ([srcTy],[src])

    let CallQueryableFirstFind = 
        let F = CallGenericStaticMethod <@ System.Linq.Queryable.First : _ * Expression<Func<_,_>> -> _ @> 
        fun (srcTy,src,predicate:Expression) -> 
            F ([srcTy],[src;box predicate])

    let CallQueryableCount = 
        let F = CallGenericStaticMethod <@ System.Linq.Queryable.Count : _ -> _ @> 
        fun (srcTy:Type,src:obj) -> 
            F ([srcTy],[src])

    let BindGenericMethod (methInfo:MethodInfo) tyargs = 
        if methInfo.IsGenericMethod then 
            methInfo.GetGenericMethodDefinition().MakeGenericMethod(Array.ofList tyargs)
        else
            methInfo

    let minfo = match <@@ LinqExpressionHelper @@> with Lambda(_,Call(_,minfo,_)) -> minfo | _ -> failwith "couldn't find method info"
    let MakeFakeExpression (x:Expr) = 
        Expr.Call(minfo.GetGenericMethodDefinition().MakeGenericMethod [| x.Type |], [ x ])

            
    let MakeGenericStaticMethod lam  =
        // Nb. the SelectMany/Where/Select methods theoretically expects an expression, but the
        // LINQ team decided to only pass it a delegate construction. The coercion from
        // the delegate construction to the expression is effectively implicit in LINQ, but
        // not in F# quotations, so we have to use 'Unchecked' version (see also FSBUGS #970)
        let methInfo = FindGenericStaticMethodInfo lam 
        (fun (tyargs,args) -> Expr.Call(BindGenericMethod methInfo tyargs,args))
           

    let MakeQueryableSelect = 
        let F = MakeGenericStaticMethod <@ (System.Linq.Queryable.Select : _ * Expression<Func<_,_>> -> _) @>
        fun (srcTy,targetTy,src,v,f)  -> 
            
            let selector = Expr.NewDelegate(MakeQueryFuncTy(srcTy,targetTy), v,f)

            let selector = MakeFakeExpression selector
            F ([srcTy;targetTy],[src;selector])

    let MakeQueryableAppend = 
        let F = MakeGenericStaticMethod <@ (System.Linq.Queryable.Concat ) @>
        fun (srcTy,src1,src2)  -> 
            F ([srcTy],[src1;src2])

    let MakeQueryableContains = 
        let F = MakeGenericStaticMethod <@ (System.Linq.Queryable.Contains : _ * _ -> _ ) @>
        fun (srcTy,src1,src2)  -> 
            F ([srcTy],[src1;src2])

    let MakeQueryableAsQueryable = 
        let F = MakeGenericStaticMethod <@ (System.Linq.Queryable.AsQueryable : seq<_>  -> IQueryable<_>) @>
        fun (ty,src)  -> 
            F ([ty],[src])

    let MakeEnumerableEmpty = 
        let F = MakeGenericStaticMethod <@ (System.Linq.Enumerable.Empty : unit -> seq<_>) @>
        fun (ty)  -> 
            F ([ty],[])

    let MakeQueryableEmpty = 
        fun (ty)  -> 
            MakeQueryableAsQueryable (ty,MakeEnumerableEmpty ty)

    let MakeQueryableSelectMany = 
        let F = MakeGenericStaticMethod <@ (System.Linq.Queryable.SelectMany : IQueryable<_> * Expression<Func<_,_>> -> IQueryable<_>) @>
        fun (srcTy,targetTy,src,v,f)  -> 
            // REVIEW: Previous notes in this file said that LINQ likes to see a coercion to an interface type
            // at this point. 
            //let src = Expr.Coerce(src,MakeIQueryableTy srcTy)
            let selector = Expr.NewDelegate(MakeQueryFuncTy(srcTy,MakeIEnumerableTy targetTy), [v],f)

            let selector = MakeFakeExpression selector
            F ([srcTy;targetTy],[src;selector])

    let MakeQueryableWhere = 
        let F = MakeGenericStaticMethod <@ (System.Linq.Queryable.Where : _ * Expression<Func<_,_>> -> _) @>
        fun (srcTy,src,v,f)  -> 
            let selector = Expr.NewDelegate(MakeQueryFuncTy(srcTy,typeof<bool>), [v],f)

            let selector = MakeFakeExpression selector
            F ([srcTy],[src;selector])



    let MakeQueryableOrderBy = 
        let F = MakeGenericStaticMethod <@ System.Linq.Queryable.OrderBy : _ * Expression<Func<_,_>> -> _ @> 
        fun (srcTy,keyTy,src,v,keySelector)  -> 
            
            let selector = Expr.NewDelegate(MakeQueryFuncTy(srcTy,keyTy), [v],keySelector)

            let selector = MakeFakeExpression selector
            F ([srcTy;keyTy],[src;selector])

    let MakeQueryableDistinct = 
        let F = MakeGenericStaticMethod <@ System.Linq.Queryable.Distinct : _ -> _ @> 
        fun (srcTy,src)  -> 
            let src = Expr.Coerce(src,MakeIQueryableTy srcTy)
            F ([srcTy],[src])

    let MakeQueryableTake = 
        let F = MakeGenericStaticMethod <@ System.Linq.Queryable.Take @> 
        fun (srcTy,src,count)  -> 
            let src = Expr.Coerce(src,MakeIQueryableTy srcTy)
            F ([srcTy],[src;count])

    let MakeQueryableGroupBy = 
        let F = MakeGenericStaticMethod <@ System.Linq.Queryable.GroupBy : _ * Expression<Func<_,_>> -> _ @> 
        fun (srcTy,keyTy,src,v,keySelector)  -> 
            let keySelector = Expr.NewDelegate(MakeQueryFuncTy(srcTy,keyTy), [v],keySelector) 

            let keySelector = MakeFakeExpression keySelector
            F ([srcTy;keyTy],[src;keySelector])

    let MakeQueryableMinBy = 
        let F = MakeGenericStaticMethod <@ System.Linq.Queryable.Min : _ * Expression<Func<_,_>> -> _ @> 
        fun (srcTy,keyTy,src,v,keySelector)  -> 
            let keySelector = Expr.NewDelegate(MakeQueryFuncTy(srcTy,keyTy), [v],keySelector) 

            let keySelector = MakeFakeExpression keySelector
            F ([srcTy;keyTy],[src;keySelector])

    let MakeQueryableMaxBy = 
        let F = MakeGenericStaticMethod <@ System.Linq.Queryable.Max : _ * Expression<Func<_,_>> -> _ @> 
        fun (srcTy,keyTy,src,v,keySelector)  -> 
            let keySelector = Expr.NewDelegate(MakeQueryFuncTy(srcTy,keyTy), [v],keySelector) 

            let keySelector = MakeFakeExpression keySelector
            F ([srcTy;keyTy],[src;keySelector])

    let MakeQueryableAny = 
        let F = MakeGenericStaticMethod <@ System.Linq.Queryable.Any : _ * Expression<Func<_,_>> -> _ @> 
        fun (srcTy,src,v,keySelector)  -> 
            let keySelector = Expr.NewDelegate(MakeQueryFuncTy(srcTy,boolTy), [v],keySelector) 

            let keySelector = MakeFakeExpression keySelector
            F ([srcTy],[src;keySelector])

    let MakeQueryableAll = 
        let F = MakeGenericStaticMethod <@ System.Linq.Queryable.All : _ * Expression<Func<_,_>> -> _ @> 
        fun (srcTy,src,v,keySelector)  -> 
            let keySelector = Expr.NewDelegate(MakeQueryFuncTy(srcTy,boolTy), [v],keySelector) 

            let keySelector = MakeFakeExpression keySelector
            F ([srcTy],[src;keySelector])

    let MakeQueryableJoin = 
        let F = MakeGenericStaticMethod <@ System.Linq.Queryable.Join : _ * _ * Expression<Func<_,_>> * Expression<Func<_,_>> * Expression<Func<_,_,_>> -> _ @> 
        fun (outerSourceTy,innerSourceTy,keyTy,resultTy,outerSource,innerSource,outerKeyVar,outerKeySelector,innerKeyVar,innerKeySelector,outerResultKeyVar,innerResultKeyVar,resultSelector)  -> 
            let outerKeySelector = Expr.NewDelegate(MakeQueryFuncTy(outerSourceTy,keyTy), [outerKeyVar],outerKeySelector) |> MakeFakeExpression 
            let innerKeySelector = Expr.NewDelegate(MakeQueryFuncTy(innerSourceTy,keyTy), [innerKeyVar],innerKeySelector) |> MakeFakeExpression 
            let resultSelector = Expr.NewDelegate(MakeQueryFunc2Ty(outerSourceTy,innerSourceTy,resultTy), [outerResultKeyVar;innerResultKeyVar],resultSelector) |> MakeFakeExpression 
            F ([outerSourceTy;innerSourceTy;keyTy;resultTy],[outerSource;innerSource;outerKeySelector;innerKeySelector;resultSelector])


    let MakeQueryableGroupJoin = 
        let F = MakeGenericStaticMethod <@ System.Linq.Queryable.GroupJoin : _ * _ * Expression<Func<_,_>> * Expression<Func<_,_>> * Expression<Func<_,_,_>> -> _ @> 
        fun (outerSourceTy,innerSourceTy,keyTy,resultTy,outerSource,innerSource,outerKeyVar,outerKeySelector,innerKeyVar,innerKeySelector,outerResultKeyVar,innerResultKeyVar,resultSelector)  -> 
            let outerKeySelector = Expr.NewDelegate(MakeQueryFuncTy(outerSourceTy,keyTy), [outerKeyVar],outerKeySelector) |> MakeFakeExpression 
            let innerKeySelector = Expr.NewDelegate(MakeQueryFuncTy(innerSourceTy,keyTy), [innerKeyVar],innerKeySelector) |> MakeFakeExpression 
            let resultSelector = Expr.NewDelegate(MakeQueryFunc2Ty(outerSourceTy,MakeIEnumerableTy(innerSourceTy),resultTy), [outerResultKeyVar;innerResultKeyVar],resultSelector) |> MakeFakeExpression 
            F ([outerSourceTy;innerSourceTy;keyTy;resultTy],[outerSource;innerSource;outerKeySelector;innerKeySelector;resultSelector])



    let MakeLambda = 
        let F = CallGenericStaticMethod <@ (fun (a:Expression,b:ParameterExpression[]) -> System.Linq.Expressions.Expression.Lambda<_>(a,b)) @> 
        fun (srcTy,targetTy,arg:Expression,p:ParameterExpression) -> 
            (F ([MakeQueryFuncTy(srcTy,targetTy)],[box arg;box [| p |] ]) :?> Expression)

    /// Project F# function expressions to Linq LambdaExpression nodes
    let FuncExprToLinqFunc2Expression (srcTy,targetTy,v,body) = 
        Expr.NewDelegate(Linq.Expressions.Expression.GetFuncType [| srcTy; targetTy |], [v],body).ToLinqExpression()


    let MakeMapConcat = 
        let F = MakeGenericStaticMethod <@ Seq.collect @>
        fun (srcTy,targetTy,f,src)  -> 
            F ([srcTy;MakeIEnumerableTy targetTy;targetTy],[f;src])

    let MakeFilter = 
        let F = MakeGenericStaticMethod <@ Seq.filter @>
        fun (srcTy,f,src)  -> 
            F ([srcTy],[f;src])

    let (|MacroReduction|_|) (p : Expr) = 
        match p with 
        | Applications(Lambdas(vs,body),args) when vs.Length = args.Length && List.forall2 (fun vs arg -> List.length vs = List.length args) vs args -> 
            let tab = Map.ofSeq (List.concat (List.map2 List.zip vs args))
            let body = body.Substitute tab.TryFind 
            Some body

        // Macro
        | PropertyGet(None,Getter(MethodWithReflectedDefinition(body)),[]) -> 
            Some body

        // Macro
        | Call(None,MethodWithReflectedDefinition(Lambdas(vs,body)),args) -> 
            let tab = Map.ofSeq (List.concat (List.map2 (fun (vs:Var list) arg -> match vs,arg with [v],arg -> [(v,arg)] | vs,NewTuple(args) -> List.zip vs args | _ -> List.zip vs [arg]) vs args))
            let body = body.Substitute tab.TryFind 
            Some body

        // Macro
        | Let(v,e,body) ->
            let tab = Map.ofSeq [ (v,e) ]
            let body = body.Substitute tab.TryFind 
            Some body
        | _ -> None

    let rec MacroExpand (p : Expr) = 
        match p with 
        // Macro reduction
        | MacroReduction(reduced) -> 
            MacroExpand reduced
        | ExprShape.ShapeCombination(comb,args) ->
            ExprShape.RebuildShapeCombination(comb, List.map MacroExpand args)
        | ExprShape.ShapeLambda(v,body) ->
            Expr.Lambda(v, MacroExpand body)
        | ExprShape.ShapeVar _ -> p

    let (|PipedCallMapConcat|_|) = (|SpecificPipedCall1|_|) <@ Seq.collect @>
    let (|CallSingleton|_|) = (|SpecificCall|_|) <@ Seq.singleton @>
    let (|CallEmpty|_|) = (|SpecificCall|_|) <@ Seq.empty @>
    let (|PipedCallSort|_|) = (|SpecificPipedCall0|_|) <@ Seq.sort @>
    let (|PipedCallSortBy|_|) = (|SpecificPipedCall1|_|) <@ Seq.sortBy @>
    let (|PipedCallSeqGroupBy|_|) = (|SpecificPipedCall1|_|) <@ Seq.groupBy @>
    let (|PipedCallSeqMinBy|_|) = (|SpecificPipedCall1|_|) <@ Seq.minBy @>
    let (|PipedCallSeqMaxBy|_|) = (|SpecificPipedCall1|_|) <@ Seq.maxBy @>
    let (|PipedCallQueryGroupBy|_|) = (|SpecificPipedCall1|_|) <@ groupBy @>
    let (|PipedCallQueryMinBy|_|) = (|SpecificPipedCall1|_|) <@ minBy @>
    let (|PipedCallQueryMaxBy|_|) = (|SpecificPipedCall1|_|) <@ maxBy @>
    let (|PipedCallSeqMap|_|) = (|SpecificPipedCall1|_|) <@ Seq.map @>
    let (|PipedCallSeqAppend|_|) = (|SpecificPipedCall1|_|) <@ Seq.append @>
    let (|PipedCallSeqFilter|_|) = (|SpecificPipedCall1|_|) <@ Seq.filter @>
    let (|PipedCallSeqExists|_|) = (|SpecificPipedCall1|_|) <@ Seq.exists @>
    let (|PipedCallSeqForAll|_|) = (|SpecificPipedCall1|_|) <@ Seq.forall @>
    let (|PipedCallSeqDelay|_|) = (|SpecificPipedCall0|_|) <@ Seq.delay @>
    let (|PipedCallSeqDistinct|_|) = (|SpecificPipedCall0|_|) <@ Seq.distinct @>
    let (|PipedCallSeqToList|_|) = (|SpecificPipedCall0|_|) <@ Seq.toList @>
    let (|PipedCallSeqTake|_|) = (|SpecificPipedCall1|_|) <@ Seq.take @>
    let (|PipedCallSeqTruncate|_|) = (|SpecificPipedCall1|_|) <@ Seq.truncate @>
    let (|PipedCallSeqToArray|_|) = (|SpecificPipedCall0|_|) <@ Seq.toArray @>
    let (|PipedCallSeqMin|_|) = (|SpecificPipedCall0|_|) <@ Seq.min @>
    let (|PipedCallSeqMax|_|) = (|SpecificPipedCall0|_|) <@ Seq.max @>
    let (|PipedCallQueryContains|_|) = (|SpecificPipedCall1|_|) <@ contains @>
    let (|CallSeq|_|) = (|SpecificCall|_|) <@ seq @>
    let (|CallQueryJoin|_|) = (|SpecificCall|_|) <@ join @>
    let (|CallQueryGroupJoin|_|) = (|SpecificCall|_|) <@ groupJoin @>
    let (|PipedCallAverageBy|_|) = (|SpecificPipedCall1|_|) <@ Seq.averageBy : (float -> float) -> seq<float> -> float @> 
    let (|PipedCallAverage|_|) = (|SpecificPipedCall0|_|) <@ Seq.average: seq<float> -> float @>
    let (|PipedCallSumBy|_|) = (|SpecificPipedCall1|_|) <@ Seq.sumBy : (float -> float) -> seq<float> -> float @>
    let (|PipedCallSum|_|) = (|SpecificPipedCall0|_|) <@ Seq.sum: seq<float> -> float @>
    let (|PipedCallSeqLength|_|) = (|SpecificPipedCall0|_|) <@ Seq.length @>
    let (|PipedCallSeqHead|_|) = (|SpecificPipedCall0|_|) <@ Seq.head @>
    let (|PipedCallSeqFind|_|) = (|SpecificPipedCall1|_|) <@ Seq.find @>

    let query (p : Expr<'T1>) : 'T1 = 

        let body = (p :> Expr)
        let rec TransInner (tm:Expr) = 
            match tm with 
            // Look through coercions, e.g. to IEnumerable 
            | Coerce (expr,ty) -> 
                TransInner expr
            
            // Seq.collect (fun x -> if P x then tgt else Seq.empty)  sq @> 
            //    ~~> TRANS(Seq.collect (x -> tgt) (sq.Where(x -> P x))
            | PipedCallMapConcat ([srcTy;_;targetTy],Lambda(selectorVar, selector),sq) ->
                let rec TransMapConcatSelector t = 
                    match t with 
                    | CallSingleton(None, _,[res]) -> 

                        MakeQueryableSelect(srcTy,targetTy, TransInner sq, [selectorVar],MacroExpand res)


                    | IfThenElse(g,tgt,CallEmpty (None, [_],[])) ->

                        let sq = MakeFilter(srcTy,Expr.Lambda(selectorVar,g),sq)
                        let sq = TransInner (MakeMapConcat(srcTy,targetTy,Expr.Lambda(selectorVar,tgt),sq))
                        sq

                    | MacroReduction(reduced) -> 
                        TransMapConcatSelector reduced

                    | selectorBody ->
                        let selectorBody = TransInner selectorBody
                        // For some reason IQueryable.SelectMany expects an IEnumerable return
                        // REVIEW: Previous notes in this file said that LINQ likes to see a coercion to an interface type at this point. 
                        let selectorBody = Expr.Coerce(TransInner selectorBody,MakeIEnumerableTy targetTy)
                        MakeQueryableSelectMany(srcTy,targetTy, TransInner sq, selectorVar,selectorBody)

                TransMapConcatSelector selector
            
            | PipedCallSeqMap ([srcTy;targetTy],Lambda(v,res),sq) ->
        
                MakeQueryableSelect(srcTy,targetTy, TransInner sq, [v],MacroExpand res)

            | PipedCallSeqAppend ([srcTy],sq1,sq2) ->
        
                MakeQueryableAppend(srcTy, TransInner sq1, TransInner sq2)

            // These occur in the F# quotation form of F# sequence expressions            
            | PipedCallSeqFilter ([srcTy],Lambda(v,res),sq) ->

                MakeQueryableWhere(srcTy, TransInner sq, v,MacroExpand res)

            // These occur in the F# quotation form of F# sequence expressions            
            | PipedCallSeqDelay (_,Lambda(_,body)) ->
        
                TransInner body
            
            // These occur in the F# quotation form of F# sequence expressions            
            | CallSeq (None, _,[body]) ->
        
                TransInner body

                
            | IfThenElse(g,t,e) ->

                Expr.IfThenElse(g,TransInner t, TransInner e)


            // These occur in the F# quotation form of F# sequence expressions            
            | CallEmpty (None, [ty],[]) ->
        
                MakeQueryableEmpty ty


            | PipedCallSortBy([ srcTy; keyTy ],Lambda(v,keySelector),source) ->

                MakeQueryableOrderBy(srcTy,keyTy, TransInner source, v,MacroExpand keySelector)

            | PipedCallSort([ srcTy ],source) ->

                let v = new Var("x",srcTy)
                MakeQueryableOrderBy(srcTy,srcTy, TransInner source, v,Expr.Var v)

            | PipedCallSeqGroupBy _ ->

                failwithf "The operator Seq.groupBy may not be used in queries. Use Microsoft.FSharp.Linq.Query.groupNy instead, which has a different return type to the standard F# operator" tm 

            | PipedCallQueryGroupBy([ srcTy; keyTy ],Lambda(v,keySelector),source) ->

                MakeQueryableGroupBy(srcTy,keyTy, TransInner source, v,MacroExpand keySelector)

            | PipedCallSeqMinBy _ ->

                failwithf "The operator Seq.minBy may not be used in queries. Use Microsoft.FSharp.Linq.Query.minBy instead, which has a different return type to the standard F# operator" tm 

            | PipedCallQueryMinBy([ srcTy; keyTy ],Lambda(v,keySelector),source) ->

                MakeQueryableMinBy(srcTy,keyTy, TransInner source, v,MacroExpand keySelector)

            | PipedCallSeqMaxBy _ ->

                failwithf "The operator Seq.maxBy may not be used in queries. Use Microsoft.FSharp.Linq.Query.maxBy instead, which has a different return type to the standard F# operator" tm 

            | PipedCallQueryMaxBy([ srcTy; keyTy ],Lambda(v,keySelector),source) ->

                MakeQueryableMaxBy(srcTy,keyTy, TransInner source, v,MacroExpand keySelector)

            | PipedCallSeqExists([ srcTy],Lambda(v,keySelector),source) ->

                MakeQueryableAny(srcTy,TransInner source, v,MacroExpand keySelector)

            | PipedCallSeqForAll([ srcTy],Lambda(v,keySelector),source) ->

                MakeQueryableAll(srcTy,TransInner source, v,MacroExpand keySelector)

            | CallQueryJoin(None, [ outerSourceTy;innerSourceTy;keyTy;resultTy ],[outerSource;innerSource;Lambda(outerKeyVar,outerKeySelector);Lambda(innerKeyVar,innerKeySelector);Lambdas([[outerResultKeyVar];[innerResultKeyVar]],resultSelector)])->

                MakeQueryableJoin(outerSourceTy,innerSourceTy,keyTy,resultTy,TransInner outerSource,TransInner innerSource,outerKeyVar,MacroExpand outerKeySelector,innerKeyVar,MacroExpand innerKeySelector,outerResultKeyVar,innerResultKeyVar,MacroExpand resultSelector)  

            | CallQueryGroupJoin(None, [ outerSourceTy;innerSourceTy;keyTy;resultTy ],[outerSource;innerSource;Lambda(outerKeyVar,outerKeySelector);Lambda(innerKeyVar,innerKeySelector);Lambdas([[outerResultKeyVar];[innerResultKeyVar]],resultSelector)])->

                MakeQueryableGroupJoin(outerSourceTy,innerSourceTy,keyTy,resultTy,TransInner outerSource,TransInner innerSource,outerKeyVar,MacroExpand outerKeySelector,innerKeyVar,MacroExpand innerKeySelector,outerResultKeyVar,innerResultKeyVar,MacroExpand resultSelector)  

            | PipedCallSeqDistinct([ srcTy ],source) ->
                MakeQueryableDistinct(srcTy, TransInner source)


            | PipedCallSeqTake([ srcTy ],count,sq) 
            | PipedCallSeqTruncate([ srcTy ],count,sq) ->
                MakeQueryableTake(srcTy, TransInner sq, MacroExpand count)

            | MacroReduction(reduced) -> 
                TransInner reduced

            // These occur in the F# quotation form of F# sequence expressions            
            //
            //       match i.Data with 
            //       | 8 -> ...
            //       | _ -> ()
            | Sequential(Value( _, unitType), sq) when unitType  = typeof<unit> -> 
        
                TransInner sq

            | expr when typeof<IQueryable>.IsAssignableFrom(expr.Type) ->
                expr


            // Error cases
            | _  -> 
                failwithf "The following construct was used in query but is not recognised by the F#-to-LINQ query translator:\n%A\nThis is not a valid query expression. Check the specification of permitted queries and consider moving some of the query out of the quotation" tm 

        let EvalInner (tm:Expr) = (TransInner tm).EvalUntyped()

        let rec EvalOuterWithPostProcess (tm:Expr) = 
            match tm with
            
            // Allow SQL <@ [ for x in ... ] @>
            | PipedCallSeqToList ([srcTy],sq) ->
        
                CallSeqToList (srcTy,EvalInner sq)

            | PipedCallSeqToArray ([srcTy],sq) ->
        
                CallSeqToArray (srcTy,EvalInner sq)


            | PipedCallSeqMin ([srcTy],sq) ->

                CallQueryableMin(srcTy,EvalInner sq)

            | PipedCallSeqMax ([srcTy],sq) ->

                CallQueryableMax(srcTy,EvalInner sq)

            | PipedCallQueryContains ([srcTy],v,sq) ->

                CallQueryableContains(srcTy,EvalInner sq,(MacroExpand v).ToLinqExpression())

            | PipedCallAverageBy([srcTy;destTy],Lambda(v,res),sq) ->

                CallQueryableAverageBy(srcTy, destTy, EvalInner sq, FuncExprToLinqFunc2Expression (srcTy,destTy,v,MacroExpand  res))

            | PipedCallAverage ([srcTy],sq) ->

                CallQueryableAverage(srcTy, EvalInner sq)

            | PipedCallSumBy ([srcTy;destTy],Lambda(v,res),sq) ->

                CallQueryableSumBy(srcTy, destTy, EvalInner sq, FuncExprToLinqFunc2Expression (srcTy,destTy,v,MacroExpand  res))

            | PipedCallSum ([srcTy],sq) ->

                CallQueryableSum(srcTy, EvalInner sq)

            | PipedCallSeqLength ([ srcTy ],sq) ->
                CallQueryableCount(srcTy, EvalInner sq)

            | PipedCallSeqHead ([ srcTy ],sq) ->
                CallQueryableFirst(srcTy, EvalInner sq)

            | PipedCallSeqFind([ srcTy ],sq, NewDelegate(_,[v],f)) ->
                CallQueryableFirstFind(srcTy, EvalInner sq, FuncExprToLinqFunc2Expression (srcTy,boolTy,v,MacroExpand f))

            | MacroReduction(reduced) -> 
                EvalOuterWithPostProcess reduced

            | _  -> 
                EvalInner tm
        
        let res = EvalOuterWithPostProcess body 
        unbox res


    let SQL x = query x


    
    

    //-------------------------------------------------------------------------
    // Nullable utilities for F#

(*

    /// This operator compares Nullable values with non-Nullable values using
    /// structural comparison
    [<ReflectedDefinition>]
    let (>=?!) (x : Nullable<'T1>) (y: 'T1) = 
        x.HasValue && x.Value >= y

    [<ReflectedDefinition>]
    let (>?!) (x : Nullable<'T1>) (y: 'T1) = 
        x.HasValue && x.Value > y

    [<ReflectedDefinition>]
    let (<=?!) (x : Nullable<'T1>) (y: 'T1) = 
        not x.HasValue || x.Value <= y

    [<ReflectedDefinition>]
    let (<?!) (x : Nullable<'T1>) (y: 'T1) = 
        not x.HasValue || x.Value < y

    [<ReflectedDefinition>]
    let (=?!) (x : Nullable<'T1>) (y: 'T1) = 
        x.HasValue && x.Value = y

    [<ReflectedDefinition>]
    let (<>?!) (x : Nullable<'T1>) (y: 'T1) = 
        not x.HasValue or x.Value <> y

    /// This overloaded operator divides Nullable values by non-Nullable values
    /// using the overloaded operator "/".  Inlined to allow use over any type,
    /// as this resolves the overloading on "/".
    [<ReflectedDefinition>]
    let inline (/?!) (x : Nullable<'T1>) (y: 'T1) = 
        if x.HasValue then new Nullable<'T1>(x.Value / y)
        else x

    /// This overloaded operator adds Nullable values by non-Nullable values
    /// using the overloaded operator "+".  Inlined to allow use over any type,
    /// as this resolves the overloading on "+".
    [<ReflectedDefinition>]
    let inline (+?!) (x : Nullable<'T1>) (y: 'T1) = 
        if x.HasValue then new Nullable<'T1>(x.Value + y)
        else x

    /// This overloaded operator adds Nullable values by non-Nullable values
    /// using the overloaded operator "-".  Inlined to allow use over any type,
    /// as this resolves the overloading on "-".
    [<ReflectedDefinition>]
    let inline (-?!) (x : Nullable<'T1>) (y: 'T1) = 
        if x.HasValue then new Nullable<'T1>(x.Value - y)
        else x

    /// This overloaded operator adds Nullable values by non-Nullable values
    /// using the overloaded operator "*".  Inlined to allow use over any type,
    /// as this resolves the overloading on "*".
    [<ReflectedDefinition>]
    let inline ( *?!) (x : Nullable<'T1>) (y: 'T1) = 
        if x.HasValue then new Nullable<'T1>(x.Value * y)
        else x

    /// This overloaded operator adds Nullable values by non-Nullable values
    /// using the overloaded operator "%".  Inlined to allow use over any type,
    /// as this resolves the overloading on "%".
    [<ReflectedDefinition>]
    let inline ( %?!) (x : Nullable<'T1>) (y: 'T1) = 
        if x.HasValue then new Nullable<'T1>(x.Value % y)
        else x

*)
    