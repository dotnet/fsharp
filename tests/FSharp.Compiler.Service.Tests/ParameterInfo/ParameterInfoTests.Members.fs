module FSharp.Compiler.Service.Tests.ParameterInfoMembersTests

open Xunit

[<Fact>]
let ``Regression.MethodInfo.Bug808310`` () =
    assertHasParameterInfo "System.Console.WriteLine({caret}"

[<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
let ``Single.DotNet.StaticMethod`` () =
    assertParameterInfoOverloads [["objA"; "objB"]] "System.Object.ReferenceEquals({caret}"

[<Fact>]
let ``Regression.NoParameterInfo.100I.Bug5038`` () =
    assertNoParameterInfo "100I({caret}"

[<Fact>]
let ``Single.DotNet.InstanceMethod`` () =
    assertParameterInfoOverloads [["startIndex: int"]; ["startIndex: int"; "length: int"]] """
let s = "Hello"
s.Substring({caret}"""

[<Fact>]
let ``Single.DotNet.NoParameters`` () =
    assertParameterInfoOverloads [[]] """
let x = "a"
x.ToUpperInvariant({caret}"""

[<Fact(Skip = "FSharp1.0:2394")>]
let ``Single.DotNet.OnSecondParameter`` () =
    assertHasParameterInfo "System.String.Format(\"x\",{caret}"

[<Fact(Skip = "FSharp1.0:5160")>]
let ``Single.Locations.PointOfDefinition`` () =
    assertNoParameterInfo """
type FunkyType =
    private new({caret}) = {}"""

[<Fact(Skip = "FSharp1.0:5244")>]
let ``Single.Locations.AfterTypeAnnotation`` () =
    assertNoParameterInfo """
type Emp =
    val mutable private m_DoB : System.DateTime
    {caret}"""

[<Fact>]
let ``Single.Locations.AfterValues`` () =
    assertNoParameterInfo "let _ = <@@ let x = 1 in x{caret} @@>"

[<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
let ``Single.Locations.EndOfFile`` () =
    assertParameterInfoOverloads [[]] "System.Console.ReadLine({caret}"

[<Fact>]
let ``Single.QuotedIdentifier`` () =
    assertParameterInfoOverloads [[]; ["maxValue: int"]; ["minValue: int"; "maxValue: int"]] """
let ``Random Number Generator`` = System.Random()
let ``?Max!Value?`` = 100
let _ = ``Random Number Generator``.Next({caret}``?Max!Value?``)"""

[<Fact>]
let ``Single.Locations.LineWithSpaces`` () =
    assertHasParameterInfo """
let r =
   System.Math.Abs({caret}0)"""

[<Fact>]
let ``Single.Locations.FullCall`` () =
    assertHasParameterInfo "System.Math.Abs({caret}0)"

[<Fact>]
let ``Single.Locations.SpacesAfterParen`` () =
    assertHasParameterInfo """
open System
let a = Math.Sign({caret}-10  )"""

[<Fact(Skip = "non-FCS: paraminfo popup LOCATION is editor-layer; GetMethods returns no group at this caret (curried `Math.Sin 10.0`, no parens)")>]
let ``Single.Locations.MethodCallWithoutParens`` () =
    assertHasParameterInfo """
open System
let n = Math.Sin 1{caret}0.0"""

[<Fact(Skip = "non-FCS: paraminfo popup LOCATION is editor-layer; call ident on a previous line, GetMethods returns no group at the lower-line `(`")>]
let ``Single.Locations.Multiline.IdentOnPrevPrevLine`` () =
    assertHasParameterInfo """
open System
do Console.WriteLine
        ({caret}
           "Multiline")"""

[<Fact(Skip = "non-FCS: paraminfo popup LOCATION is editor-layer; long-id split across lines, GetMethods returns no group at the lower-line `(`")>]
let ``Single.Locations.Multiline.LongIdentSplit`` () =
    assertHasParameterInfo """
let ll = new System.Collections.
                    Generic.List< _ > ({caret})"""

[<Fact(Skip = "non-FCS: GetMethods has no string/comment lexical context; suppression inside comments is editor-layer")>]
let ``Single.InComment`` () =
    assertNoParameterInfo "// System.Console.WriteLine({caret})"

[<Fact>]
let ``LocationOfParams.Case1`` () =
    assertHasParameterInfo "System.Console.WriteLine({caret}\"hello\")"

[<Fact(Skip = "non-FCS: paraminfo popup LOCATION is editor-layer; GetMethods returns no group at the lower-line `(`")>]
let ``LocationOfParams.Case3`` () =
    assertHasParameterInfo """System.Console.WriteLine
                 ({caret}
                        "hello {0}"  ,
                        "Brian" )  """

[<Fact>]
let ``LocationOfParams.InsideObjectExpression`` () =
    assertHasParameterInfo "let _ = { new System.Object({caret}) with member _.GetHashCode() = 2}"

[<Fact(Skip = "non-FCS: paraminfo popup LOCATION is editor-layer; GetMethods returns no group for the curried `sin (42.0)` call")>]
let ``LocationOfParams.Nested1`` () =
    assertHasParameterInfo "System.Console.WriteLine(\"hello {0}\"  , sin  ({caret}42.0 ) )"

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``LocationOfParams.EvenWhenOverloadResolutionFails.Case1`` () =
    assertHasParameterInfo "let a = new System.IO.FileStream({caret})"

[<Fact>]
let ``Multi.DotNet.InstanceMethod`` () =
    assertParameterInfoContains ["startIndex: int"; "length: int"] """
let s = "Hello"
s.Substring({caret}0,1)"""

[<Fact>]
let ``Multi.OverloadMethod.OrderedParameters`` () =
    assertParameterInfoContains ["year: int"; "month: int"; "day: int"] "new System.DateTime({caret}2000,12,1)"

[<Fact(Skip = "non-FCS: GetMethods has no string/comment lexical context; suppression inside comments is editor-layer")>]
let ``ParameterInfo.Multi.NoParameterInfo.InComments`` () =
    assertNoParameterInfo "//let _ = System.Object({caret})"

[<Fact>]
let ``Multi.NoParameterInfo.InComments2`` () =
    assertNoParameterInfo "(*System.Console.WriteLine({caret}\"Test on Fsharp style comments.\")*)"

[<Fact>]
let ``BasicBehavior.DotNet.Static`` () =
    assertParameterInfoContains ["string"; "obj array"] "System.String.Format({caret}"
