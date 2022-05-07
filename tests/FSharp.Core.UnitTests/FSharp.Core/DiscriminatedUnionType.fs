// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
module FSharp.Core.UnitTests.DiscriminatedUnionTypes

open System
open System.Numerics
open System.Reflection
open System.Runtime.InteropServices
open FSharp.Core.UnitTests.LibraryTestFx

open Xunit
open FsCheck
open FsCheck.PropOperators

type EnumUnion = 
    | A
    | B

type UseUnionsAsEnums() = 
    [<Fact>]
    member this.CanCompare() = 
        Assert.AreEqual(EnumUnion.B, EnumUnion.B)
        Assert.AreNotEqual(EnumUnion.A, EnumUnion.B)

[<Flags>]
type FlagsUnion = 
    | One = 1
    | Two = 2
    | Four = 4

type UseUnionsAsFlags() = 
    
    [<Fact>]
    member this.CanCompareWithInts() = 
        Assert.AreEqual(int FlagsUnion.One, 1)
        Assert.AreEqual(int FlagsUnion.Two, 2)
        Assert.AreEqual(int FlagsUnion.Four, 4)
    
    [<Fact>]
    member this.CanCastFromInts() = 
        let four : FlagsUnion = enum 4
        Assert.AreEqual(four, FlagsUnion.Four)
    
    [<Fact>]
    member this.CanCreateValuesWithoutName() = 
        let unknown : FlagsUnion = enum 99 // strange, but valid
        Assert.AreEqual(int unknown, 99)
    
    [<Fact>]
    member this.CanParseViaBCL() = 
        let values = System.Enum.GetValues(typeof<FlagsUnion>)
        let fourFromString = System.Enum.Parse(typeof<FlagsUnion>, "Four", false) :?> FlagsUnion // downcast needed
        Assert.AreEqual(fourFromString, FlagsUnion.Four)
    
    [<Fact>]
    member this.CanUseBinaryOr() = 
        Assert.AreEqual(int (FlagsUnion.One ||| FlagsUnion.Two), 3)
        Assert.AreEqual(int (FlagsUnion.One ||| FlagsUnion.One), 1)
    
    [<Fact>]
    member this.CanCompareWithFlags() = 
        Assert.AreEqual(FlagsUnion.Two, FlagsUnion.Two)
        Assert.AreNotEqual(FlagsUnion.Two, FlagsUnion.One)

type UnionsWithData = 
    | Alpha of int
    | Beta of string * float

type UseUnionsWithData() = 
    let a1 = Alpha 1
    let a2 = Alpha 2
    let b1 = Beta("win", 8.1)
    
    [<Fact>]
    member this.CanAccessTheData() = 
        match a1 with
        | Alpha 1 -> ()
        | _ -> Assert.Fail()
        match a2 with
        | Alpha 2 -> ()
        | _ -> Assert.Fail()
        match a2 with
        | Alpha x -> Assert.AreEqual(x, 2)
        | _ -> Assert.Fail()
        match b1 with
        | Beta("win", 8.1) -> ()
        | _ -> Assert.Fail()
        match b1 with
        | Beta(x, y) -> 
            Assert.AreEqual(x, "win")
            Assert.AreEqual(y, 8.1)
        | _ -> Assert.Fail()
    
    [<Fact>]
    member this.CanAccessTheDataInGuards() = 
        match a1 with
        | Alpha x when x = 1 -> ()
        | _ -> Assert.Fail()
        match a2 with
        | Alpha x when x = 2 -> ()
        | _ -> Assert.Fail()

[<Struct>]
type StructUnion = SU of C : int * D : int

[<Struct>]
[<CustomComparison; CustomEquality>]
type ComparisonStructUnion =
    | SU2 of int * int 
    member x.C1 = (match x with SU2(a,b) -> a)
    member x.C2 = (match x with SU2(a,b) -> b)
    override self.Equals other =
        match other with
        | :? ComparisonStructUnion as o ->  (self.C1 + self.C2) = (o.C1 + o.C2)
        | _ -> false

    override self.GetHashCode() = hash self
    interface IComparable with
        member self.CompareTo other =
            match other with
            | :? ComparisonStructUnion as o -> compare (self.C1 + self.C2) (o.C1 + o.C2)
            | _ -> invalidArg "other" "cannot compare values of different types"

[<Struct>]
[<NoComparison; NoEquality>]
type NoComparisonStructUnion =
    | SU3 of int * int

let private hasAttribute<'T,'Attr>() =
    typeof<'T>.GetTypeInfo().GetCustomAttributes() |> Seq.exists  (fun x -> x.GetType() = typeof<'Attr>)


type UnionsFSCheckTests () =
    inherit TestClassWithSimpleNameAppDomainResolver ()

    [<Fact>]
    member _.``struct unions hold [<Struct>] metadata`` () =
        Assert.True (hasAttribute<StructUnion,StructAttribute>())

    [<Fact>]
    member _.``struct unions are comparable`` () =
        Check.QuickThrowOnFailure <|
        fun (i1:int) (i2:int) ->
            i1 <> i2 ==>
            let sr1 = SU (i1, i2)
            let sr2 = SU (i1, i2)
            let sr3 = SU (i2, i1)
            (sr1 = sr2)                    |@ "sr1 = sr2" .&.
            (sr1 <> sr3)                    |@ "sr1 <> sr3" .&.
            (sr1.Equals sr2)               |@ "sr1.Equals sr2"


    [<Fact>]
    member _.``struct unions support pattern matching`` () =
        Check.QuickThrowOnFailure <|
        fun (i1:int) (i2:int) ->
            let sr1 = SU(i1, i2)
            (match sr1 with
            | SU(c,d) when c = i1 && d = i2 -> true
            | _ -> false) 
            |@ "with pattern match on struct union" .&.
            (sr1 |> function 
            | SU(c,d) when c = i1 && d = i2 -> true
            | _ -> false)
            |@ "function pattern match on struct union"

    [<Fact>]
    member _.``struct unions support let binds using `` () =
        Check.QuickThrowOnFailure <|
        fun (i1:int) (i2:int) ->
            let sr1 = SU(i1,i2)
            let (SU (c1,d2))  as sr2 = sr1
            (sr1 = sr2)          |@ "sr1 = sr2" .&.
            (c1 = i1 && d2 = i2) |@ "c1 = i1 && d2 = i2"


    [<Fact>]
    member _.``struct unions support function argument bindings`` () =
        Check.QuickThrowOnFailure <|
        fun (i1:int) (i2:int) ->
            let sr1 = SU(i1,i2)
            let test sr1 (SU (c1,d2) as sr2) =
                sr1 = sr2 && c1 = i1 && d2 = i2
            test sr1 sr1

    [<Fact>]
    member _.``struct unions support [<CustomEquality>]`` () =
        Check.QuickThrowOnFailure <|
        fun (i1:int) (i2:int) ->
            let sr1 = SU2(i1,i2)
            let sr2 = SU2(i1,i2)
            (sr1.Equals sr2)      

    [<Fact>]
    member _.``struct unions support [<CustomComparison>]`` () =
        Check.QuickThrowOnFailure <|
        fun (i1:int) (i2:int) (k1:int) (k2:int) ->        
            let sr1 = SU2(i1,i2)
            let sr2 = SU2(k1,k2)
            if   sr1 > sr2 then compare sr1 sr2 = 1
            elif sr1 < sr2 then compare sr1 sr2 = -1
            elif sr1 = sr2 then compare sr1 sr2 = 0
            else false

    [<Fact>]
    member _.``struct unions hold [<CustomComparison>] [<CustomEquality>] metadata`` () =
        Assert.True (hasAttribute<ComparisonStructUnion,CustomComparisonAttribute>())
        Assert.True (hasAttribute<ComparisonStructUnion,CustomEqualityAttribute>())

    [<Fact>]
    member _.``struct unions hold [<NoComparison>] [<NoEquality>] metadata`` () =
        Assert.True (hasAttribute<NoComparisonStructUnion,NoComparisonAttribute>())
        Assert.True (hasAttribute<NoComparisonStructUnion,NoEqualityAttribute>())


    [<Fact>]
    member _.``can properly construct a struct union using FSharpValue.MakeUnionCase, and we get the fields`` () =
        let cases = Microsoft.FSharp.Reflection.FSharpType.GetUnionCases(typeof<StructUnion>)

        Assert.AreEqual (1, cases.Length)
        let case = cases.[0]

        Assert.AreEqual ("SU", case.Name)
    
        let structUnion = Microsoft.FSharp.Reflection.FSharpValue.MakeUnion (case, [|box 1234; box 3456|])

        Assert.True (structUnion.GetType().IsValueType)

        let _uc, fieldVals = Microsoft.FSharp.Reflection.FSharpValue.GetUnionFields(structUnion, typeof<StructUnion>)

        Assert.AreEqual (2, fieldVals.Length)

        let c = (fieldVals.[0] :?> int)
        Assert.AreEqual (1234, c)

        let c2 = (fieldVals.[1] :?> int)
        Assert.AreEqual (3456, c2)

    [<Fact>]
    member _.``struct unions does optimization correctly on pattern matching`` () =
        let arr = ResizeArray()
        match arr.Add(1); ValueSome () with
        | ValueSome () -> ()
        | ValueNone -> ()

        Assert.AreEqual(1, arr.Count)
