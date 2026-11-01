// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Core.UnitTests.Linq

open System
open System.Collections
open System.Collections.Generic
open System.Linq
open System.Linq.Expressions
open System.Reflection
open Microsoft.FSharp.Reflection

type ICapturedQuery =
    abstract InMemory: IQueryable

// EnumerableQuery turns Queryable calls into Enumerable ones only over its own constants;
// over any other IQueryable it calls Queryable again, which here would never end.
type private InMemoryRewriter() =
    inherit ExpressionVisitor()

    override _.VisitConstant node =
        match node.Value with
        | :? ICapturedQuery as captured -> Expression.Constant captured.InMemory
        | _ -> node

/// An in-memory query that stays on the provider that created it and prints its root by name.
[<Sealed>]
type CapturedQuery<'T>(provider: IQueryProvider, name: string, items: seq<'T>, derived: Expression voption) =

    let enumerate () =
        match derived with
        | ValueSome expression -> provider.Execute<IEnumerable<'T>>(expression).GetEnumerator()
        | ValueNone -> items.GetEnumerator()

    override _.ToString() =
        match derived with
        | ValueSome expression -> expression.ToString()
        | ValueNone -> name

    interface ICapturedQuery with
        member _.InMemory =
            match derived with
            | ValueSome expression -> EnumerableQuery<'T>(InMemoryRewriter().Visit expression)
            | ValueNone -> EnumerableQuery<'T> items

    interface IOrderedQueryable<'T> with
        member this.Expression =
            match derived with
            | ValueSome expression -> expression
            | ValueNone -> Expression.Constant this

        member _.ElementType = typeof<'T>
        member _.Provider = provider
        member _.GetEnumerator() : IEnumerator<'T> = enumerate ()
        member _.GetEnumerator() : IEnumerator = enumerate ()

/// Executes in memory, and records what it was asked to execute.
[<Sealed>]
type CapturingQueryProvider() =
    let executed = ResizeArray<Expression>()
    let mutable depth = 0

    let inMemory () =
        EnumerableQuery<int> Seq.empty :> IQueryProvider

    let track (expression: Expression) (run: Expression -> 'TResult) =
        if depth = 0 then
            executed.Add expression

        depth <- depth + 1

        try
            run (InMemoryRewriter().Visit expression)
        finally
            depth <- depth - 1

    /// The expressions executed directly, not from inside another execution.
    member _.Executed = executed :> IReadOnlyList<Expression>

    member this.Source(name: string, items: seq<'T>) : IQueryable<'T> =
        CapturedQuery<'T>(this, name, items, ValueNone)

    member this.Derived<'T>(expression: Expression) : IQueryable<'T> =
        CapturedQuery<'T>(this, "", Seq.empty, ValueSome expression)

    interface IQueryProvider with
        member this.CreateQuery<'T>(expression: Expression) : IQueryable<'T> = this.Derived<'T> expression

        member this.CreateQuery(expression: Expression) : IQueryable =
            let elementType =
                expression.Type.GetInterfaces()
                |> Array.append [| expression.Type |]
                |> Array.find (fun ty -> ty.IsGenericType && ty.GetGenericTypeDefinition() = typedefof<IEnumerable<_>>)
                |> fun ty -> ty.GetGenericArguments().[0]

            typeof<CapturingQueryProvider>
                .GetMethod(nameof Unchecked.defaultof<CapturingQueryProvider>.Derived)
                .MakeGenericMethod(elementType)
                .Invoke(this, [| expression |])
            :?> IQueryable

        member _.Execute<'TResult>(expression: Expression) : 'TResult =
            track expression (inMemory ()).Execute<'TResult>

        member _.Execute(expression: Expression) : obj =
            track expression (inMemory ()).Execute

/// Something in an expression tree that a C# expression lambda never contains, so no LINQ provider translates it.
[<RequireQualifiedAccess>]
type Violation =
    | Statement of ExpressionType
    | LambdaInvocation
    | UnboundParameter of name: string
    | RuntimeHelperCall of methodName: string
    | NestedInCollectionSelector of methodName: string
    | ReadsUnnamedMember of typeName: string
    | LeftProvider
    | Throws of exceptionType: string

module ProviderShape =

    let private fsharpCore = typeof<unit>.Assembly

    let private allMembers = BindingFlags.Public ||| BindingFlags.NonPublic

    let private isLinqOperator (call: MethodCallExpression) =
        call.Arguments.Count > 0
        && (call.Method.DeclaringType = typeof<Queryable>
            || call.Method.DeclaringType = typeof<Enumerable>)

    let private isStatement nodeType =
        match nodeType with
        | ExpressionType.Block
        | ExpressionType.Assign
        | ExpressionType.Loop
        | ExpressionType.Goto
        | ExpressionType.Label
        | ExpressionType.Try
        | ExpressionType.Switch
        | ExpressionType.RuntimeVariables
        | ExpressionType.DebugInfo -> true
        | _ -> false

    let private flattensOrJoins (call: MethodCallExpression) =
        isLinqOperator call
        && (match call.Method.Name with
            | "SelectMany"
            | "Join"
            | "GroupJoin" -> true
            | _ -> false)

    type private ShapeVisitor() =
        inherit ExpressionVisitor()

        let found = ResizeArray<Violation>()
        let bound = HashSet<ParameterExpression>(HashIdentity.Reference)
        let mutable collectionSelectorDepth = 0

        let bind (parameters: seq<ParameterExpression>) =
            parameters |> Seq.filter bound.Add |> Seq.toArray

        let unbind (parameters: ParameterExpression array) =
            for parameter in parameters do
                bound.Remove parameter |> ignore

        [<return: Struct>]
        let (|AssignsBound|_|) (expression: Expression) =
            match expression with
            | :? BinaryExpression as assign when assign.NodeType = ExpressionType.Assign ->
                match assign.Left with
                | :? ParameterExpression as variable when bound.Contains variable -> ValueSome assign.Right
                | _ -> ValueNone
            | _ -> ValueNone

        member _.Violations = found |> Seq.distinct |> List.ofSeq

        override _.VisitLambda<'T>(node: Expression<'T>) =
            let added = bind node.Parameters

            try
                base.VisitLambda node
            finally
                unbind added

        // A block is reported once: its variables and their initial assignments are part of it.
        override this.VisitBlock node =
            found.Add(Violation.Statement ExpressionType.Block)
            let added = bind node.Variables

            try
                for expression in node.Expressions do
                    match expression with
                    | AssignsBound value -> this.Visit value |> ignore
                    | expression -> this.Visit expression |> ignore

                node
            finally
                unbind added

        override _.VisitParameter node =
            if not (bound.Contains node) then
                found.Add(Violation.UnboundParameter node.Name)

            node

        override _.VisitInvocation node =
            found.Add Violation.LambdaInvocation
            base.VisitInvocation node

        // A query built earlier and used in this one is part of what the provider translates.
        override this.VisitConstant node =
            match node.Value with
            | :? IQueryable as query ->
                match query.Expression with
                | :? ConstantExpression as root when obj.ReferenceEquals(root.Value, query) -> ()
                | built -> this.Visit built |> ignore
            | _ -> ()

            node

        override _.VisitNew node =
            if node.Type.Assembly = fsharpCore && not (node.Type.Name.StartsWith("AnonymousObject", StringComparison.Ordinal)) then
                found.Add(Violation.RuntimeHelperCall $"new {node.Type.Name}")

            base.VisitNew node

        override this.VisitMethodCall node =
            if node.Method.DeclaringType.Assembly = fsharpCore then
                found.Add(Violation.RuntimeHelperCall $"{node.Method.DeclaringType.Name}.{node.Method.Name}")

            if collectionSelectorDepth > 0 && flattensOrJoins node then
                found.Add(Violation.NestedInCollectionSelector node.Method.Name)

            if isLinqOperator node && node.Method.Name = "SelectMany" then
                this.Visit node.Object |> ignore
                this.Visit node.Arguments.[0] |> ignore
                collectionSelectorDepth <- collectionSelectorDepth + 1

                try
                    this.Visit node.Arguments.[1] |> ignore
                finally
                    collectionSelectorDepth <- collectionSelectorDepth - 1

                for argument in Seq.skip 2 node.Arguments do
                    this.Visit argument |> ignore

                node
            else
                base.VisitMethodCall node

        override this.Visit node =
            match node with
            | null -> null
            | node ->
                if node.NodeType <> ExpressionType.Block && isStatement node.NodeType then
                    found.Add(Violation.Statement node.NodeType)

                base.Visit node

    [<return: Struct>]
    let private (|Lambda|_|) (argument: Expression) =
        match argument with
        | :? UnaryExpression as quote when quote.NodeType = ExpressionType.Quote -> ValueSome(quote.Operand :?> LambdaExpression)
        | :? LambdaExpression as lambda -> ValueSome lambda
        | _ -> ValueNone

    /// A tuple or record built without saying which member each argument becomes.
    let private buildsUnnamed (body: Expression) =
        match body with
        | :? NewExpression as construction when construction.Arguments.Count > 0 ->
            match construction.Members with
            | null ->
                FSharpType.IsTuple construction.Type
                || FSharpType.IsRecord(construction.Type, allMembers)
            | _ -> false
        | _ -> false

    type private MemberReadFinder(owner: ParameterExpression) =
        inherit ExpressionVisitor()

        member val Found = false with get, set

        override this.VisitMember node =
            if obj.ReferenceEquals(node.Expression, owner) then
                this.Found <- true

            base.VisitMember node

    let private readsMemberOf (owner: ParameterExpression) (body: Expression) =
        let finder = MemberReadFinder owner
        finder.Visit body |> ignore
        finder.Found

    let private yields (elementType: Type) (source: Expression) =
        typedefof<IEnumerable<_>>.MakeGenericType(elementType).IsAssignableFrom source.Type

    /// Whether the lambda gives its first parameter back as it is.
    let private returnsParameter (lambda: LambdaExpression) =
        lambda.Parameters.Count > 0 && obj.ReferenceEquals(lambda.Body, lambda.Parameters.[0])

    /// A query that is its own root, as opposed to one built from another by operators.
    let private isRoot (query: IQueryable) =
        match query.Expression with
        | :? ConstantExpression as root -> obj.ReferenceEquals(root.Value, query)
        | _ -> false

    /// Finds members read from elements that an earlier operator built without naming them: the provider has to
    /// take apart an object it was not told how to. Every chain of operators is followed, the ones inside lambdas
    /// and the ones of a query captured from earlier too, and each parameter of a lambda is tied to the sequence
    /// it stands for, which is not always the first source of the operator.
    type private UnnamedMemberFinder() =
        inherit ExpressionVisitor()

        let found = ResizeArray<Violation>()

        // Whether the elements are unnamed, for the parameters that stand for a sequence: the group of a GroupJoin
        let sequences = Dictionary<ParameterExpression, bool>(HashIdentity.Reference)

        let lambdaAt (call: MethodCallExpression) index =
            if index < call.Arguments.Count then
                match call.Arguments.[index] with
                | Lambda lambda -> ValueSome lambda
                | _ -> ValueNone
            else
                ValueNone

        // The parameter of the lambda stands for an element of the sequence, which was built without names
        let report (lambda: LambdaExpression) index (sequence: Expression) (unnamed: bool) =
            if unnamed && index < lambda.Parameters.Count then
                let parameter = lambda.Parameters.[index]

                if yields parameter.Type sequence && readsMemberOf parameter lambda.Body then
                    found.Add(Violation.ReadsUnnamedMember parameter.Type.Name)

        member _.Found = found |> Seq.distinct |> List.ofSeq

        /// Whether the elements of the sequence were built without naming their members.
        member this.Unnamed(sequence: Expression) : bool =
            match sequence with
            | :? MethodCallExpression as call when isLinqOperator call ->
                let source = call.Arguments.[0]

                let lambdas =
                    [ for index in 1 .. call.Arguments.Count - 1 do
                          match lambdaAt call index with
                          | ValueSome lambda -> lambda
                          | ValueNone -> () ]

                match call.Method.Name with
                | "Select" ->
                    lambdas
                    |> List.tryLast
                    |> Option.exists (fun selector -> buildsUnnamed selector.Body || (returnsParameter selector && this.Unnamed source))
                | "SelectMany" ->
                    match lambdas with
                    | [ collection ] -> this.Unnamed collection.Body
                    | [ _; result ] -> buildsUnnamed result.Body
                    | _ -> false
                | "Join"
                | "GroupJoin" -> lambdas |> List.tryLast |> Option.exists (fun result -> buildsUnnamed result.Body)
                | "Where"
                | "OrderBy"
                | "OrderByDescending"
                | "ThenBy"
                | "ThenByDescending"
                | "Skip"
                | "SkipWhile"
                | "Take"
                | "TakeWhile"
                | "Distinct"
                | "DefaultIfEmpty"
                | "Reverse"
                | "AsQueryable"
                | "AsEnumerable" -> this.Unnamed source
                | "Concat"
                | "Union"
                | "Intersect"
                | "Except" -> this.Unnamed source || (call.Arguments.Count > 1 && this.Unnamed call.Arguments.[1])
                | _ -> false
            | :? ConstantExpression as constant ->
                match constant.Value with
                | :? IQueryable as query when not (isRoot query) -> this.Unnamed query.Expression
                | _ -> false
            | :? ParameterExpression as parameter ->
                match sequences.TryGetValue parameter with
                | true, unnamed -> unnamed
                | _ -> false
            | :? UnaryExpression as conversion when conversion.NodeType = ExpressionType.Convert -> this.Unnamed conversion.Operand
            | _ -> false

        /// Reports the members that the lambdas of the operator read from elements built without names.
        member this.Check(call: MethodCallExpression) =
            let source = call.Arguments.[0]

            match call.Method.Name with
            | "Join"
            | "GroupJoin" when call.Arguments.Count >= 5 ->
                let outer, inner = call.Arguments.[0], call.Arguments.[1]
                let outerUnnamed, innerUnnamed = this.Unnamed outer, this.Unnamed inner

                lambdaAt call 2 |> ValueOption.iter (fun key -> report key 0 outer outerUnnamed)
                lambdaAt call 3 |> ValueOption.iter (fun key -> report key 0 inner innerUnnamed)

                lambdaAt call 4
                |> ValueOption.iter (fun result ->
                    report result 0 outer outerUnnamed

                    if call.Method.Name = "Join" then
                        report result 1 inner innerUnnamed
                    elif result.Parameters.Count > 1 then
                        // The second parameter is the group of the matching inner elements, not one of them
                        sequences.[result.Parameters.[1]] <- innerUnnamed)
            | "SelectMany" when call.Arguments.Count >= 3 ->
                // A collection selector, then a result selector over the element and an element of the collection
                let sourceUnnamed = this.Unnamed source
                let collection = lambdaAt call 1
                collection |> ValueOption.iter (fun selector -> report selector 0 source sourceUnnamed)

                lambdaAt call 2
                |> ValueOption.iter (fun result ->
                    report result 0 source sourceUnnamed

                    collection
                    |> ValueOption.iter (fun selector -> report result 1 selector.Body (this.Unnamed selector.Body)))
            | "Zip" when call.Arguments.Count = 3 ->
                let second = call.Arguments.[1]

                lambdaAt call 2
                |> ValueOption.iter (fun result ->
                    report result 0 source (this.Unnamed source)
                    report result 1 second (this.Unnamed second))
            // The first parameter of the function is the accumulated value
            | "Aggregate" -> ()
            | _ ->
                let sourceUnnamed = this.Unnamed source

                for index in 1 .. call.Arguments.Count - 1 do
                    lambdaAt call index |> ValueOption.iter (fun lambda -> report lambda 0 source sourceUnnamed)

        override this.VisitMethodCall node =
            if isLinqOperator node then
                this.Check node

            base.VisitMethodCall node

        // A query built earlier and used in this one is part of what the provider translates.
        override this.VisitConstant node =
            match node.Value with
            | :? IQueryable as query when not (isRoot query) -> this.Visit query.Expression |> ignore
            | _ -> ()

            node

    let private unnamedMemberReads (expression: Expression) =
        let finder = UnnamedMemberFinder()
        finder.Visit expression |> ignore
        finder.Found

    let violations (expression: Expression) =
        let visitor = ShapeVisitor()
        visitor.Visit expression |> ignore
        visitor.Violations @ unnamedMemberReads expression |> List.distinct

    /// Whether the expression calls a Queryable operator that gives a value and not a query, as a provider is asked
    /// to execute when a query is counted, summed or searched.
    let isTerminalOperator (expression: Expression) =
        match expression with
        | :? MethodCallExpression as call ->
            call.Method.DeclaringType = typeof<Queryable>
            && not (typeof<IQueryable>.IsAssignableFrom call.Type)
        | _ -> false

    /// The LINQ operators along the source chain, outermost last.
    let spine (expression: Expression) =
        let rec walk (expression: Expression) operators =
            match expression with
            | :? MethodCallExpression as call when isLinqOperator call -> walk call.Arguments.[0] (call.Method.Name :: operators)
            | _ -> operators

        walk expression []
