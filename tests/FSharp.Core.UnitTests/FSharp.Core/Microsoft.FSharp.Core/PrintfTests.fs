// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Various tests for:
// Microsoft.FSharp.Core.ExtraTopLevelOperators.printf

namespace FSharp.Core.UnitTests

open Xunit

type MyUnionType =
    | CaseOne
    | CaseTwo of myString : string
    | CaseTwoOpt of myString : string option
    | CaseThree of someNumber : int * myString : string

[<RequireQualifiedAccess>]
type SecondUnionType =
    | Case1
    | Case2 of myString : string
    | Case2Opt of myString : string option
    | Case3 of someNumber : int * myString : string

[<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]
type MyNullAsTrueUnionType =
    | NullCase
    | NonNull of int

[<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>]
[<RequireQualifiedAccess>]
type RQANullAsTrueUnionType =
    | NonNull of int
    | NullCase

type PrintfTests() =
    let test fmt arg (expected:string) =
        let actual = sprintf fmt arg
        Assert.AreEqual(expected, actual)

    [<Fact>]
    member this.FormatAndPrecisionSpecifiers() =
        test "%10s"  "abc" "       abc"
        test "%-10s" "abc" "abc       "
        test "%10d"  123   "       123"
        test "%-10d" 123   "123       "
        test "%10c"  'a'   "         a"
        test "%-10c" 'a'   "a         "
    
    [<Fact>]
    member __.``union case formatting`` () =
        Assert.AreEqual("CaseOne", sprintf "%A" CaseOne)
        Assert.AreEqual("CaseTwo \"hello\"", sprintf "%A" (CaseTwo "hello"))
        Assert.AreEqual("CaseTwoOpt None", sprintf "%A" (CaseTwoOpt None))
        Assert.AreEqual("CaseTwoOpt (Some \"hi\")", sprintf "%A" (CaseTwoOpt (Some "hi")))
        Assert.AreEqual("CaseThree (5, \"hello\")", sprintf "%A" (CaseThree (5, "hello")))

    [<Fact>]
    member __.``union case formatting with RequireQualifiedAccess`` () =
        Assert.AreEqual("Case1", sprintf "%A" SecondUnionType.Case1)
        Assert.AreEqual("Case2 \"hello\"", sprintf "%A" (SecondUnionType.Case2 "hello"))
        Assert.AreEqual("Case2Opt None", sprintf "%A" (SecondUnionType.Case2Opt None))
        Assert.AreEqual("Case2Opt (Some \"hi\")", sprintf "%A" (SecondUnionType.Case2Opt (Some "hi")))
        Assert.AreEqual("Case3 (5, \"hello\")", sprintf "%A" (SecondUnionType.Case3 (5, "hello")))

    [<Fact>]
    member __.``union case formatting with UseNullAsTrueValue`` () =
        Assert.AreEqual("NullCase", sprintf "%A" NullCase)
        Assert.AreEqual("NullCase", sprintf "%A" RQANullAsTrueUnionType.NullCase)

    [<Fact>]
    member __.``F# option formatting`` () =
        Assert.AreEqual("None", sprintf "%A" None)
        Assert.AreEqual("Some 15", sprintf "%A" (Some 15))

    [<Fact>]
    member __.``null formatting`` () =
        Assert.AreEqual("<null>", sprintf "%A" null)
        Assert.AreEqual("CaseTwo null", sprintf "%A" (CaseTwo null))

    [<Fact>]
    member __.``tuple formatting`` () =
        Assert.AreEqual("""(1, "two", 3.4)""", sprintf "%A" (1,"two",3.4))
        Assert.AreEqual("""(1, "two", 3.4, 5, 6, 7, 8, 9, "ten", 11.12)""", sprintf "%A" (1,"two",3.4,5,6,7,8,9,"ten",11.12))

    [<Fact>]
    member __.``value tuple formatting`` () =
        Assert.AreEqual("""struct (1, "two", 3.4)""", sprintf "%A" (struct (1,"two",3.4)))
        Assert.AreEqual("""struct (1, "two", 3.4, 5, 6, 7, 8, 9, "ten", 11.12)""", sprintf "%A" (struct (1,"two",3.4,5,6,7,8,9,"ten",11.12)))

    [<Fact>]
    member __.``list types`` () =
        Assert.AreEqual("""[CaseTwo "hello"; CaseTwo "hi there!"]""", [CaseTwo "hello"; CaseTwo "hi there!"] |> sprintf "%A")
        Assert.AreEqual("""[CaseTwoOpt (Some "hello"); CaseTwoOpt (Some "hi there!")]""", [CaseTwoOpt (Some "hello"); CaseTwoOpt (Some "hi there!")] |> sprintf "%A")
