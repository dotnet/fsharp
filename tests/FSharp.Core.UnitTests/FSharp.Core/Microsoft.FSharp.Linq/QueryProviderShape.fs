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

    /// Members read from elements that an earlier operator built without naming them:
    /// the provider has to take apart an object it was not told how to.
    let private unnamedMemberReads (expression: Expression) =
        let found = ResizeArray<Violation>()

        let rec producesUnnamed (expression: Expression) =
            match expression with
            | :? MethodCallExpression as call when isLinqOperator call ->
                let source = call.Arguments.[0]
                let sourceUnnamed = producesUnnamed source

                let lambdas =
                    [ for argument in Seq.skip 1 call.Arguments do
                          match argument with
                          | Lambda lambda -> lambda
                          | _ -> () ]

                if sourceUnnamed then
                    for lambda in lambdas do
                        let element = lambda.Parameters.[0]

                        if yields element.Type source && readsMemberOf element lambda.Body then
                            found.Add(Violation.ReadsUnnamedMember element.Type.Name)

                match call.Method.Name with
                | "Select"
                | "SelectMany"
                | "Join"
                | "GroupJoin" -> lambdas |> List.last |> _.Body |> buildsUnnamed
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
                | "DefaultIfEmpty" -> sourceUnnamed
                | _ -> false
            | _ -> false

        producesUnnamed expression |> ignore
        List.ofSeq found

    let violations (expression: Expression) =
        let visitor = ShapeVisitor()
        visitor.Visit expression |> ignore
        visitor.Violations @ unnamedMemberReads expression |> List.distinct

    /// The LINQ operators along the source chain, outermost last.
    let spine (expression: Expression) =
        let rec walk (expression: Expression) operators =
            match expression with
            | :? MethodCallExpression as call when isLinqOperator call -> walk call.Arguments.[0] (call.Method.Name :: operators)
            | _ -> operators

        walk expression []
