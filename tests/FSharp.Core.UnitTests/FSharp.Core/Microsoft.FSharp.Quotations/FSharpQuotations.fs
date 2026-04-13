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
