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
    member this.NumbersInDifferentBases() =
        test "%10u"  123   "       123"
        test "%-10u" 123   "123       "
        test "%10B"  123   "   1111011"
        test "%-10B" 123   "1111011   "
        test "%10o"  123   "       173"
        test "%-10o" 123   "173       "
        test "%10x"  123   "        7b"
        test "%-10x" 123   "7b        "
        test "%10X"  123   "        7B"
        test "%-10X" 123   "7B        "
        
    [<Fact>]
    member this.VariableWidth() =
        Assert.AreEqual("       a", sprintf "%*c"  8 'a'  )
        Assert.AreEqual("a       ", sprintf "%-*c" 8 'a'  )
        Assert.AreEqual("     abc", sprintf "%*s"  8 "abc")
        Assert.AreEqual("abc     ", sprintf "%-*s" 8 "abc")
        Assert.AreEqual("     123", sprintf "%*u"  8 123  )
        Assert.AreEqual("123     ", sprintf "%-*u" 8 123  )
        Assert.AreEqual(" 1111011", sprintf "%*B"  8 123  )
        Assert.AreEqual("1111011 ", sprintf "%-*B" 8 123  )
        Assert.AreEqual("     173", sprintf "%*o"  8 123  )
        Assert.AreEqual("173     ", sprintf "%-*o" 8 123  )
        Assert.AreEqual("      7b", sprintf "%*x"  8 123  )
        Assert.AreEqual("7b      ", sprintf "%-*x" 8 123  )
        Assert.AreEqual("      7B", sprintf "%*X"  8 123  )
        Assert.AreEqual("7B      ", sprintf "%-*X" 8 123  ) 
        
    // test cases for https://github.com/dotnet/fsharp/issues/15557
    [<Fact>]
    member this.``sign flag - positive and negative one``() =
        test "%f"  +1.0         "1.000000"
        test "%f"  -1.0         "-1.000000"
        test "%+f" +1.0         "+1.000000"
        test "%+f" -1.0         "-1.000000"
        
        test "%f"  +1.0f        "1.000000"
        test "%f"  -1.0f        "-1.000000"
        test "%+f" +1.0f        "+1.000000"
        test "%+f" -1.0f        "-1.000000"
        
        test "%f"  +1.0M        "1.000000"
        test "%f"  -1.0M        "-1.000000"
        test "%+f" +1.0M        "+1.000000"
        test "%+f" -1.0M        "-1.000000"
            
    [<Fact>]
    member this.``sign flag - positive and negative zero``() =
        test "%f"  +0.0         "0.000000"
        test "%f"  -0.0         "0.000000"
        test "%+f" +0.0         "+0.000000"
        test "%+f" -0.0         "+0.000000"
        
        test "%f"  +0.0f        "0.000000"
        test "%f"  -0.0f        "0.000000"
        test "%+f" +0.0f        "+0.000000"
        test "%+f" -0.0f        "+0.000000"
        
        test "%f"  +0.0M        "0.000000"
        test "%f"  -0.0M        "0.000000"
        test "%+f" +0.0M        "+0.000000"
        test "%+f" -0.0M        "+0.000000"
    
    [<Fact>]
    member this.``sign flag - very small positive and negative numbers``() =
        test "%f"  -0.0000001   "0.000000"
        // TODO: should this output -0.000000 or +0.000000? See https://github.com/dotnet/fsharp/pull/18147#issuecomment-2546220183
        // test "%+f" -0.0000001   "+0.000000"
        
        test "%f"  -0.0000001f  "0.000000"
        // see previous comment
        // test "%+f" -0.0000001f  "+0.000000"
        
        test "%f"  -0.0000001M  "0.000000"
        // see previous comment
        // test "%+f" -0.0000001M  "+0.000000"
    
    [<Fact>]
    member this.``sign flag - infinity``() =
        test "%f"  +infinity    "Infinity"
        test "%f"  -infinity    "-Infinity"
        test "%+f" +infinity    "Infinity"
        test "%+f" -infinity    "-Infinity"
        
        test "%f"  +infinityf   "Infinity"
        test "%f"  -infinityf   "-Infinity"
        test "%+f" +infinityf   "Infinity"
        test "%+f" -infinityf   "-Infinity"

    [<Fact>]
    member this.``sign flag - NaN``() =
        test "%f"  +nan         "NaN"
        test "%f"  -nan         "NaN"
        test "%+f" +nan         "NaN"
        test "%+f" -nan         "NaN"
        
        test "%f"  +nanf        "NaN"
        test "%f"  -nanf        "NaN"
        test "%+f" +nanf        "NaN"
        test "%+f" -nanf        "NaN"
    
    // test cases for https://github.com/dotnet/fsharp/issues/15558 (same root cause as #15557; listing for completeness)
    [<Fact>]
    member this.``zero padding - positive and negative one`` () =
        test "%010.3f" +1.0  "000001.000"
        test "%010.3f" -1.0  "-00001.000"
        
        test "%010.3f" +1.0f "000001.000"
        test "%010.3f" -1.0f "-00001.000"
        
        test "%010.3f" +1.0M "000001.000"
        test "%010.3f" -1.0M "-00001.000"
    
    [<Fact>]
    member this.``zero padding - positive and negative zero`` () =
        test "%010.3f" +0.0         "000000.000"
        test "%010.3f" -0.0         "000000.000"
        
        test "%010.3f" +0.0f        "000000.000"
        test "%010.3f" -0.0f        "000000.000"
        
        test "%010.3f" +0.0M        "000000.000"
        test "%010.3f" -0.0M        "000000.000"
    
    [<Fact>]
    member _.``union case formatting`` () =
        Assert.AreEqual("CaseOne", sprintf "%A" CaseOne)
        Assert.AreEqual("CaseTwo \"hello\"", sprintf "%A" (CaseTwo "hello"))
        Assert.AreEqual("CaseTwoOpt None", sprintf "%A" (CaseTwoOpt None))
        Assert.AreEqual("CaseTwoOpt (Some \"hi\")", sprintf "%A" (CaseTwoOpt (Some "hi")))
        Assert.AreEqual("CaseThree (5, \"hello\")", sprintf "%A" (CaseThree (5, "hello")))

    [<Fact>]
    member _.``union case formatting with RequireQualifiedAccess`` () =
        Assert.AreEqual("Case1", sprintf "%A" SecondUnionType.Case1)
        Assert.AreEqual("Case2 \"hello\"", sprintf "%A" (SecondUnionType.Case2 "hello"))
        Assert.AreEqual("Case2Opt None", sprintf "%A" (SecondUnionType.Case2Opt None))
        Assert.AreEqual("Case2Opt (Some \"hi\")", sprintf "%A" (SecondUnionType.Case2Opt (Some "hi")))
        Assert.AreEqual("Case3 (5, \"hello\")", sprintf "%A" (SecondUnionType.Case3 (5, "hello")))

    [<Fact>]
    member _.``union case formatting with UseNullAsTrueValue`` () =
        Assert.AreEqual("NullCase", sprintf "%A" NullCase)
        Assert.AreEqual("NullCase", sprintf "%A" RQANullAsTrueUnionType.NullCase)

    [<Fact>]
    member _.``F# option formatting`` () =
        Assert.AreEqual("None", sprintf "%A" None)
        Assert.AreEqual("Some 15", sprintf "%A" (Some 15))

    [<Fact>]
    member _.``null formatting`` () =
        Assert.AreEqual("<null>", sprintf "%A" null)
        Assert.AreEqual("CaseTwo null", sprintf "%A" (CaseTwo null))

    [<Fact>]
    member _.``tuple formatting`` () =
        Assert.AreEqual("""(1, "two", 3.4)""", sprintf "%A" (1,"two",3.4))
        Assert.AreEqual("""(1, "two", 3.4, 5, 6, 7, 8, 9, "ten", 11.12)""", sprintf "%A" (1,"two",3.4,5,6,7,8,9,"ten",11.12))

    [<Fact>]
    member _.``value tuple formatting`` () =
        Assert.AreEqual("""struct (1, "two", 3.4)""", sprintf "%A" (struct (1,"two",3.4)))
        Assert.AreEqual("""struct (1, "two", 3.4, 5, 6, 7, 8, 9, "ten", 11.12)""", sprintf "%A" (struct (1,"two",3.4,5,6,7,8,9,"ten",11.12)))

    [<Fact>]
    member _.``list types`` () =
        Assert.AreEqual("""[CaseTwo "hello"; CaseTwo "hi there!"]""", [CaseTwo "hello"; CaseTwo "hi there!"] |> sprintf "%A")
        Assert.AreEqual("""[CaseTwoOpt (Some "hello"); CaseTwoOpt (Some "hi there!")]""", [CaseTwoOpt (Some "hello"); CaseTwoOpt (Some "hi there!")] |> sprintf "%A")
