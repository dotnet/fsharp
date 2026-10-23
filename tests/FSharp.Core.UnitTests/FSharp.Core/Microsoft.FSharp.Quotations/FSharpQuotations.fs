// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Various tests for Microsoft.FSharp.Quotations

namespace FSharp.Core.UnitTests.Quotations

open System
open FSharp.Core.UnitTests
open FSharp.Core.UnitTests.Collections
open FSharp.Core.UnitTests.LibraryTestFx
open Xunit
open FSharp.Quotations
open FSharp.Quotations.Patterns
open FSharp.Linq.RuntimeHelpers

type E = Microsoft.FSharp.Quotations.Expr;;

type StaticIndexedPropertyTest() =
    static member IdxProp with get (n : int) = n + 1

type QuotationEnum =
    | A = 1
    | B = 2

module Check =
    let argumentException f =
        let mutable ex = false
        try
            f () |> ignore
        with
        |   :? System.ArgumentException-> ex <- true
        Assert.True(ex, "InvalidOperationException expected")


type FSharpQuotationsTests() =

    [<Fact>]
    member x.MethodInfoNRE() =
        let f() =
            E.Call(null, []) |> ignore
        CheckThrowsArgumentNullException f

    [<Fact>]
    member x.FieldInfoNRE() =
        let f() =
            E.FieldGet(null) |> ignore
        CheckThrowsArgumentNullException f

    [<Fact>]
    member x.ConstructorNRE() =
        let f() =
            E.NewObject(null,[]) |> ignore
        CheckThrowsArgumentNullException f

    [<Fact>]
    member x.PropertyInfoNRE() =
        let f() =
            E.PropertyGet(null,[]) |> ignore
        CheckThrowsArgumentNullException f

    [<Fact>]
    member x.UnionCaseInfoNRE() =
        let f() =
            E.NewUnionCase(Unchecked.defaultof<Microsoft.FSharp.Reflection.UnionCaseInfo>,[]) |> ignore
        CheckThrowsArgumentNullException f

    [<Fact>]
    member x.ReShapeTypechecking_Let() =
        let q0 = <@ let a = 1 in a @>
        match q0 with
        |   ExprShape.ShapeCombination(shape, [value;lambda]) ->
                let goodValue = <@ 2 @>
                ExprShape.RebuildShapeCombination(shape, [goodValue;lambda]) |> ignore
        |   _ -> Assert.Fail()
        let q1 = <@ let a = 1 in a @>
        match q1 with
        |   ExprShape.ShapeCombination(shape, [value;lambda]) ->
                let wrongValue = <@ "!" @>
                Check.argumentException(fun () -> ExprShape.RebuildShapeCombination(shape, [wrongValue;lambda]))
        |   _ -> Assert.Fail()

    [<Fact>]
    member x.ReShapeStaticIndexedProperties() =
        let q0 = <@ StaticIndexedPropertyTest.IdxProp 5 @>
        match q0 with
        |   ExprShape.ShapeCombination(shape, args) ->
                try
                    ExprShape.RebuildShapeCombination(shape, args) |> ignore
                with
                | _ -> Assert.Fail()
        |   _ -> Assert.Fail()

    [<Fact>]
    member x.GetConstructorFiltersOutStaticConstructor() =
        ignore <@ System.Exception() @>

    [<Fact>]
    member x.``Issue 20800 - struct tuple quotation printing`` () =
        Assert.Equal(
            """NewStructTuple (Value (1), Value ("a"))""",
            (<@ struct (1, "a") @>).ToString()
        )

    [<Fact>]
    member x.``Issue 20800 - reference tuple quotation printing`` () =
        Assert.Equal(
            """NewTuple (Value (1), Value ("a"))""",
            (<@ (1, "a") @>).ToString()
        )

    static member private TupleLambdas<'T>() =
        [
            (<@ fun (x: 'T) -> struct (x, 1) @> :> Expr),
            typeof<'T -> struct ('T * int)>, typeof<struct ('T * int)>, "NewStructTuple"
            (<@ fun (x: 'T) -> (x, 1) @> :> Expr),
            typeof<'T -> ('T * int)>, typeof<'T * int>, "NewTuple"
        ]

    [<Fact>]
    member x.``Issue 20800 - tuple rendering preserves types patterns and shapes`` () =
        let assertPrinted expected (expr: Expr) =
            Assert.Equal(expected, expr.ToString())
            Assert.Equal(expected, expr.ToString(false))
            Assert.Equal(expected, expr.ToString(true))
            Assert.Equal(expected, sprintf "%A" expr)

        let assertTuple (expectedType: Type) expectedArgs expected (expr: Expr) =
            match expr with
            | ExprShape.ShapeCombination(shape, args) ->
                Assert.Equal<Expr list>(expectedArgs, args)
                let rebuilt = ExprShape.RebuildShapeCombination(shape, args)
                for tuple in [expr; rebuilt] do
                    Assert.Equal(expectedType, tuple.Type)
                    Assert.Equal(expectedType.IsValueType, tuple.Type.IsValueType)
                    match tuple with
                    | NewTuple args -> Assert.Equal<Expr list>(expectedArgs, args)
                    | _ -> Assert.Fail("Expected NewTuple to match both tuple kinds")
                    match tuple with
                    | NewStructTuple args ->
                        Assert.True(expectedType.IsValueType)
                        Assert.Equal<Expr list>(expectedArgs, args)
                    | _ -> Assert.False(expectedType.IsValueType)
                    assertPrinted expected tuple
            | _ -> Assert.Fail("Expected a tuple combination")

        let pairArgs = [ <@@ 1 @@>; <@@ "a" @@> ]
        for expectedType, label, expressions in [
            typeof<int * string>, "NewTuple",
            [ (<@ (1, "a") @> :> Expr); <@@ (1, "a") @@>; Expr.NewTuple pairArgs ]
            typeof<struct (int * string)>, "NewStructTuple",
            [ (<@ struct (1, "a") @> :> Expr); <@@ struct (1, "a") @@>
              Expr.NewStructTuple pairArgs
              Expr.NewStructTuple(typeof<struct (int * string)>.Assembly, pairArgs) ]
        ] do
            for expr in expressions do
                assertTuple expectedType pairArgs ($"""{label} (Value (1), Value ("a"))""") expr

        for expr, expectedType, args, expected in [
            (<@ struct ((1, "a"), 2) @> :> Expr), typeof<struct ((int * string) * int)>,
            [ <@@ (1, "a") @@>; <@@ 2 @@> ],
            """NewStructTuple (NewTuple (Value (1), Value ("a")), Value (2))"""
            (<@ (struct (1, "a"), 2) @> :> Expr), typeof<struct (int * string) * int>,
            [ <@@ struct (1, "a") @@>; <@@ 2 @@> ],
            """NewTuple (NewStructTuple (Value (1), Value ("a")), Value (2))"""
            (<@ struct (1, "a", true) @> :> Expr), typeof<struct (int * string * bool)>,
            [ yield! pairArgs; <@@ true @@> ],
            """NewStructTuple (Value (1), Value ("a"), Value (true))"""
            (<@ (1, "a", true) @> :> Expr), typeof<int * string * bool>,
            [ yield! pairArgs; <@@ true @@> ],
            """NewTuple (Value (1), Value ("a"), Value (true))"""
        ] do
            assertTuple expectedType args expected expr

        for expr, functionType, tupleType, label in [
            yield! FSharpQuotationsTests.TupleLambdas<int>()
            yield! FSharpQuotationsTests.TupleLambdas<string>()
        ] do
            Assert.Equal(functionType, expr.Type)
            assertPrinted $"Lambda (x, {label} (x, Value (1)))" expr
            match expr with
            | Lambda(v, body) ->
                assertTuple tupleType [ Expr.Var v; <@@ 1 @@> ] $"{label} (x, Value (1))" body
            | _ -> Assert.Fail("Expected a generic tuple lambda")

        let firstSix = "Value (1), Value (2), Value (3), Value (4), Value (5), Value (6)"
        for arity, tail, referenceType, structType, referenceLiteral, structLiteral in [
            7, "Value (7)", typeof<int * int * int * int * int * int * int>,
            typeof<struct (int * int * int * int * int * int * int)>,
            (<@ (1,2,3,4,5,6,7) @> :> Expr), (<@ struct (1,2,3,4,5,6,7) @> :> Expr)
            8, "Value (7), Value (8)", typeof<int * int * int * int * int * int * int * int>,
            typeof<struct (int * int * int * int * int * int * int * int)>,
            (<@ (1,2,3,4,5,6,7,8) @> :> Expr), (<@ struct (1,2,3,4,5,6,7,8) @> :> Expr)
            9, "Value (7), Value (8), Value (9)", typeof<int * int * int * int * int * int * int * int * int>,
            typeof<struct (int * int * int * int * int * int * int * int * int)>,
            (<@ (1,2,3,4,5,6,7,8,9) @> :> Expr), (<@ struct (1,2,3,4,5,6,7,8,9) @> :> Expr)
        ] do
            let args = [ for i in 1..arity -> Expr.Value i ]
            for expectedType, label, indent, literal, constructed in [
                referenceType, "NewTuple", "          ", referenceLiteral, Expr.NewTuple args
                structType, "NewStructTuple", "                ", structLiteral, Expr.NewStructTuple args
            ] do
                let expected = $"{label} ({firstSix},\n{indent}{tail})"
                for expr in [literal; constructed] do
                    assertTuple expectedType args expected expr

    [<Fact>]
    member x.``NewStructTuple literal should be recognized by NewStructTuple active pattern`` () =
        match <@ struct(1, "") @> with
        | NewStructTuple [ Value(:? int as i, _) ; Value(:? string as s, _) ] when i = 1 && s = "" -> ()
        | _ -> Assert.Fail()


    [<Fact>]
    member x.``NewStructTuple literal should be recognized by NewTuple active pattern`` () =
        match <@ struct(1, "") @> with
        | NewTuple [ Value(:? int as i, _) ; Value(:? string as s, _) ] when i = 1 && s = "" -> ()
        | _ -> Assert.Fail()

    [<Fact>]
    member x.``Quotation of an enum value preserves the enum type`` () =
        // Related to https://github.com/dotnet/fsharp/issues/995: an enum literal is quoted as a
        // Value node carrying the enum type, not the bare underlying integer.
        match <@ QuotationEnum.B @> with
        | Value(v, t) ->
            Assert.Equal(typeof<QuotationEnum>, t)
            Assert.Equal(box QuotationEnum.B, v)
        | _ -> Assert.Fail()

    [<Fact>]
    member x.``NewTuple literal should not be recognized by NewStructTuple active pattern`` () =
        match <@ (1, "") @> with
        | NewStructTuple _ -> Assert.Fail()
        | _ -> ()

    [<Fact>]
    member x.``NewStructTuple should be recognized by NewStructTuple active pattern`` () =
        let expr = Expr.NewStructTuple(typeof<struct(_ * _)>.Assembly, [ <@@ 1 @@>; <@@ "" @@> ])
        match expr with
        | NewStructTuple [ Value(:? int as i, _) ; Value(:? string as s, _) ] when i = 1 && s = "" -> ()
        | _ -> Assert.Fail()

    [<Fact>]
    member x.``NewStructTuple should be recognized by NewTuple active pattern`` () =
        let expr = Expr.NewStructTuple(typeof<struct(_ * _)>.Assembly, [ <@@ 1 @@>; <@@ "" @@> ])
        match expr with
        | NewTuple [ Value(:? int as i, _) ; Value(:? string as s, _) ] when i = 1 && s = "" -> ()
        | _ -> Assert.Fail()

    [<Fact>]
    member x.``NewStructTuple without assembly should be recognized by NewStructTuple active pattern`` () =
        let expr = Expr.NewStructTuple([ <@@ 1 @@>; <@@ "" @@> ])
        match expr with
        | NewStructTuple [ Value(:? int as i, _) ; Value(:? string as s, _) ] when i = 1 && s = "" -> ()
        | _ -> Assert.Fail()

    [<Fact>]
    member x.``NewStructTuple without assembly should be recognized by NewTuple active pattern`` () =
        let expr = Expr.NewStructTuple([ <@@ 1 @@>; <@@ "" @@> ])
        match expr with
        | NewTuple [ Value(:? int as i, _) ; Value(:? string as s, _) ] when i = 1 && s = "" -> ()
        | _ -> Assert.Fail()

    [<Fact>]
    member x.``NewTuple should not be recognized by NewStructTuple active pattern`` () =
        let expr = Expr.NewTuple [ <@@ 1 @@>; <@@ "" @@> ]
        match expr with
        | NewStructTuple _ -> Assert.Fail()
        | _ -> ()

/// This fixture is here to test handling of EqualityConditionalOn and ComparisonConditionalOn.
/// We don't generate witnesses for equality and comparison if they're conditional; the tests
/// assert that code gen doesn't fail in those cases.
[<RequireQualifiedAccess>]
module TestConditionalConstraints =
    open FSharp.Linq.RuntimeHelpers

    let eval q = LeafExpressionConverter.EvaluateQuotation q

    type DiscriminatedUnionWithGeneric<'a> =
        | Case of 'a

    [<NoComparison>]
    type ThingWithNoComparison =
        | NoComparison

    [<NoEquality ; NoComparison>]
    type ThingWithNoEquality =
        | NoEquality

        override this.ToString () =
            "NoEquality"

    let inline compare< ^T when ^T : comparison> (x : ^T) (y : ^T) : bool =
        x < y

    let inline equate< ^T when ^T : equality> (x : ^T) (y : ^T) : bool =
        x = y

    [<Fact>]
    let ``SRTP quotations can consume conditionally constrained types `` () =
        // Just normal calls, no quotation
        Assert.False (equate (DiscriminatedUnionWithGeneric.Case 3) (DiscriminatedUnionWithGeneric.Case 4))
        Assert.True (equate (DiscriminatedUnionWithGeneric.Case 3) (DiscriminatedUnionWithGeneric.Case 3))
        Assert.True (compare (DiscriminatedUnionWithGeneric.Case 3) (DiscriminatedUnionWithGeneric.Case 4))

        // Typed quotation, int
        <@ equate (DiscriminatedUnionWithGeneric.Case 3) (DiscriminatedUnionWithGeneric.Case 4) @>
        |> eval
        |> unbox<bool>
        |> Assert.False

        <@ equate (DiscriminatedUnionWithGeneric.Case 3) (DiscriminatedUnionWithGeneric.Case 3) @>
        |> eval
        |> unbox<bool>
        |> Assert.True

        <@ compare (DiscriminatedUnionWithGeneric.Case 3) (DiscriminatedUnionWithGeneric.Case 4) @>
        |> eval
        |> unbox<bool>
        |> Assert.True

        // Untyped quotation, int
        <@@ equate (DiscriminatedUnionWithGeneric.Case 3) (DiscriminatedUnionWithGeneric.Case 4) @@>
        |> eval
        |> unbox<bool>
        |> Assert.False

        <@@ equate (DiscriminatedUnionWithGeneric.Case 3) (DiscriminatedUnionWithGeneric.Case 3) @@>
        |> eval
        |> unbox<bool>
        |> Assert.True

        <@@ compare (DiscriminatedUnionWithGeneric.Case 3) (DiscriminatedUnionWithGeneric.Case 4) @@>
        |> eval
        |> unbox<bool>
        |> Assert.True

        // Typed and untyped quotation, ThingWithNoComparison
        <@ equate ThingWithNoComparison.NoComparison ThingWithNoComparison.NoComparison @>
        |> eval
        |> unbox<bool>
        |> Assert.True

        <@@ equate ThingWithNoComparison.NoComparison ThingWithNoComparison.NoComparison @@>
        |> eval
        |> unbox<bool>
        |> Assert.True

        // Typed and untyped quotation, ThingWithNoEquality
        <@ (fun x -> x.ToString ()) ThingWithNoEquality.NoEquality @>
        |> eval
        |> unbox<string>
        |> fun s -> Assert.AreEqual (s, "NoEquality")

        <@@ (fun x -> x.ToString ()) ThingWithNoEquality.NoEquality @@>
        |> eval
        |> unbox<string>
        |> fun s -> Assert.AreEqual (s, "NoEquality")

    // This test isn't quotation-related, but it *is* closely related to the quotation test: both are checking
    // we can cope without witnesses.
    [<Fact>]
    let ``Reflective invocations of conditionally constrained types throw with a reasonable error`` () =
        let compare = typeof<ThingWithNoComparison>.DeclaringType.GetMethod "compare"
        let compare = compare.MakeGenericMethod([| typeof<ThingWithNoComparison> |])
        let exc =
            try
                compare.Invoke (null, [|ThingWithNoComparison.NoComparison ; ThingWithNoComparison.NoComparison|])
                |> ignore<obj>
                None
            with
            | exc ->
                Some exc

        Assert.Contains ("does not implement the System.IComparable interface", exc.Value.InnerException.Message, StringComparison.Ordinal)

    // This test isn't quotation-related, but it *is* closely related to the quotation test: both are checking
    // we can cope without witnesses.
    [<Fact>]
    let ``We still use Object.ReferenceEquals for non-equatable methods when reflectively invoked`` () =
        let equate = typeof<ThingWithNoComparison>.DeclaringType.GetMethod "equate"
        let equate = equate.MakeGenericMethod([| typeof<ThingWithNoEquality> |])
        let anotherOne = Activator.CreateInstance (typeof<ThingWithNoEquality>, nonPublic=true)
        equate.Invoke (null, [| ThingWithNoEquality.NoEquality ; anotherOne |])
        |> unbox<bool>
        |> Assert.False

    // Tests for issues #11131 and #15648 - anonymous record field ordering
    // When anonymous records have fields in non-alphabetical order, the LINQ expression
    // should not contain Invoke patterns that LINQ providers can't translate.
    [<Fact>]
    let ``Anonymous record with non-alphabetical field order produces clean LINQ expression - issues 11131 and 15648`` () =
        // Non-alphabetical order - B before A
        let q = <@ fun (x: int) -> {| B = x; A = x + 1 |} @>

        let linqExpr = LeafExpressionConverter.QuotationToExpression q
        let exprStr = linqExpr.ToString()

        Assert.DoesNotContain(".Invoke(", exprStr)

    [<Fact>]
    let ``Nested anonymous record produces clean LINQ expression`` () =
        // Nested anonymous record with non-alphabetical field order
        let q = <@ fun (x: int) -> {| Outer = {| B = x; A = x + 1 |} |} @>

        let linqExpr = LeafExpressionConverter.QuotationToExpression q
        let exprStr = linqExpr.ToString()

        Assert.DoesNotContain(".Invoke(", exprStr)

    [<Fact>]
    let ``Both anonymous record field orders produce equivalent results`` () =
        // Alphabetical order
        let qAlpha = <@ fun (x: int) -> {| A = x + 1; B = x |} @>
        // Non-alphabetical order
        let qNonAlpha = <@ fun (x: int) -> {| B = x; A = x + 1 |} @>

        let linqAlpha = LeafExpressionConverter.QuotationToExpression qAlpha
        let linqNonAlpha = LeafExpressionConverter.QuotationToExpression qNonAlpha

        let exprAlpha = linqAlpha.ToString()
        let exprNonAlpha = linqNonAlpha.ToString()

        // Neither should contain Invoke
        Assert.DoesNotContain(".Invoke(", exprAlpha)
        Assert.DoesNotContain(".Invoke(", exprNonAlpha)

    // Tests for issue #16918 - Array indexing generates GetArray instead of proper array index expression
    // When array indexing is used in LINQ expressions, it should generate proper array index
    // expressions that LINQ providers can translate, not GetArray method calls.
    type ArrayTestUnfold = { c: string }
    type ArrayTestDoc = { p: string; u: ArrayTestUnfold[] }

    [<Fact>]
    let ``Array indexing produces ArrayIndex expression not GetArray - issue 16918`` () =
        // Array access like x.u.[0] should NOT produce GetArray call
        let q = <@ fun (x: ArrayTestDoc) -> x.u.[0] @>

        let linqExpr = LeafExpressionConverter.QuotationToExpression q
        let exprStr = linqExpr.ToString()

        // Should NOT contain GetArray
        Assert.DoesNotContain("GetArray", exprStr)
        // Should produce x.u[0] style array index expression
        Assert.Contains("[0]", exprStr)

    [<Fact>]
    let ``Nested array member access produces clean LINQ expression - issue 16918`` () =
        // x.u[0].c should generate proper expression tree without GetArray
        let q = <@ fun (x: ArrayTestDoc) -> x.u.[0].c @>

        let linqExpr = LeafExpressionConverter.QuotationToExpression q
        let exprStr = linqExpr.ToString()

        // Should NOT contain GetArray
        Assert.DoesNotContain("GetArray", exprStr)
        // Should contain array index
        Assert.Contains("[0]", exprStr)
        // Should contain property access
        Assert.Contains(".c", exprStr)

    [<Fact>]
    let ``Array indexing with variable index produces clean expression`` () =
        // Array access with variable index
        let q = <@ fun (x: int[]) (i: int) -> x.[i] @>

        let linqExpr = LeafExpressionConverter.QuotationToExpression q
        let exprStr = linqExpr.ToString()

        // Should NOT contain GetArray
        Assert.DoesNotContain("GetArray", exprStr)
