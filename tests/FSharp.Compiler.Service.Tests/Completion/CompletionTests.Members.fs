module FSharp.Compiler.Service.Tests.CompletionMembersTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

let globalMemberCases: obj[] seq =
    [ [| "Basic"; "\nlet x = 1\nx.{caret}" |]
      [| "EndingWithTick"; "\nlet x' = 1\nx'.{caret}" |]
      [| "PartialMember2"; "\nlet x = 1\nx.{caret}CompareT" |]
      [| "ContainingTick"; "\nlet x'y = 1\nx'y.{caret}" |]
      [| "PartialMember1"; "\nlet x = 1\nx.CompareT{caret}" |] ]

[<Theory; MemberData(nameof globalMemberCases)>]
let ``GlobalMember completion lists CompareTo and GetHashCode`` (caseName: string) (source: string) =
    assertHasItemWithNames [ "CompareTo"; "GetHashCode" ] (Checker.getCompletionInfo source)

[<Theory>]
[<InlineData("System.Console.{caret}<")>]
[<InlineData("System.Console.{caret}>")>]
[<InlineData("System.Console.{caret}=")>]
[<InlineData("System.Console.{caret}!=")>]
[<InlineData("System.Console.{caret}$")>]
[<InlineData("System.Console.{caret}()")>]
let ``AdjacentToDot positive`` (source: string) =
    let info = Checker.getCompletionInfo source

    assertHasItemWithNames [ "BackgroundColor" ] info

[<Theory>]
[<InlineData("System.Console.<{caret}")>]
[<InlineData("System.Console.>{caret}")>]
[<InlineData("System.Console.={caret}")>]
[<InlineData("System.Console.!={caret}")>]
[<InlineData("System.Console.${caret}")>]
[<InlineData("System.Console.(){caret}")>]
[<InlineData("System.Console.+.{caret}")>]
let ``AdjacentToDot negative`` (source: string) =
    let info = Checker.getCompletionInfo source

    assertHasItemWithNames [ "abs" ] info
    assertHasNoItemsWithNames [ "BackgroundColor" ] info

[<Fact>]
let ``CtrlSpaceCompletion.Bug130670.Case1`` () =
    let info = Checker.getCompletionInfo "let i = async.Return(4){caret}"

    assertHasItemWithNames [ "AbstractClassAttribute" ] info
    assertHasNoItemsWithNames [ "GetType" ] info

[<Fact(Skip = "non-FCS: in-comment completion suppression is an editor-layer concern (CompletionUtils.shouldProvideCompletion), not reproducible via the FCS GetDeclarationListInfo API which resolves the qualified 'System.' island regardless of the '//' comment")>]
let ``InString`` () =
    let info = Checker.getCompletionInfo " // System.C{caret} "

    Assert.Equal(0, info.Items.Length)

[<Fact>]
let ``EmptyFile.Dot.Bug1115`` () =
    let info = Checker.getCompletionInfo ".{caret}"

    Assert.Equal(0, info.Items.Length)

[<Fact>]
let ``Project.FsFileWithBuildAction`` () =
    let info =
        Checker.getCompletionInfo
            """
let i = 4
let r = i.{caret}ToString()
let x = File1.bob"""

    assertHasItemWithNames [ "CompareTo" ] info

[<Fact>]
let ``DotOff.String`` () =
    let info =
        Checker.getCompletionInfo
            """
"x".{caret} (*marker*)
"""

    assertHasItemWithNames [ "Substring"; "GetHashCode" ] info

[<Fact>]
let ``Bug243082.DotAfterNewBreaksCompletion2`` () =
    let info =
        Checker.getCompletionInfo
            """
let s = 1
s.{caret}
new System."""

    assertHasItemWithNames [ "CompareTo"; "ToString" ] info

[<Fact>]
let ``NotShowInfo.LetBinding.Bug3602`` () =
    let info =
        Checker.getCompletionInfo
            """
let s.{caret} = "Hello world"
                            ()"""

    Assert.Equal(0, info.Items.Length)

[<Fact>]
let ``HandleInlineComments1`` () =
    let info =
        Checker.getCompletionInfo "let rrr = System  (* boo! *)  .{caret}  Int32  .  MaxValue"

    assertHasItemWithNames [ "Int32" ] info
    assertHasNoItemsWithNames [ "abs" ] info

[<Fact>]
let ``HandleInlineComments2`` () =
    let info =
        Checker.getCompletionInfo "let rrr = System  (* boo! *)  .  Int32  .{caret}  MaxValue"

    assertHasItemWithNames [ "MaxValue" ] info
    assertHasNoItemsWithNames [ "abs" ] info

[<Fact>]
let ``Expression.MultiLine.Bug66705`` () =
    let info =
        Checker.getCompletionInfo
            """
let x = 4
let y = x.GetType()
         .{caret}ToString()"""

    assertHasItemWithNames [ "ToString" ] info

[<Theory>]
[<InlineData("""
let x = "1"
let test2 = if (x).{caret}""")>]
[<InlineData("""
let x = "1"
let test2 = if (x).{caret}
let y = 2""")>]
[<InlineData("""
let x = "1"
try (x).{caret}""")>]
[<InlineData("""
let x = "1"
try (x).{caret}
let y = 2""")>]
let ``IncompleteStatement`` (source: string) =
    let info = Checker.getCompletionInfo source

    assertHasItemWithNames [ "Contains" ] info

[<Fact>]
let ``WithNonExistentDll`` () =
    let info =
        Checker.getCompletionInfoWithCompilerAndCompletionOptions
            [| @"-r:..\bar\nonexistent.dll" |]
            FSharpCodeCompletionOptions.Default
            "(*marker*) {caret} "

    assertHasItemWithNames [ "System"; "Array2D" ] info
    assertHasNoItemsWithNames [ "Int32" ] info

[<Fact>]
let ``FlagsAndSettings.Bug1969`` () =
    let info =
        Checker.getCompletionInfo
            """
let y = System.Deployment.Application.{caret}
()"""

    Assert.Equal(0, info.Items.Length)

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``OfSystemWindows`` () =
    let info =
        Checker.getCompletionInfoWithCompilerAndCompletionOptions
            [| "-r:System.Windows.Forms.dll" |]
            FSharpCodeCompletionOptions.Default
            "let y=new System.Windows.{caret}"

    Assert.Equal(3, info.Items.Length)

[<Fact>]
let ``Editor.WithoutContext.Bug986`` () =
    let info = Checker.getCompletionInfo "{caret}"

    assertHasNoItemsWithNames [ "IChapteredRowset"; "ICorRuntimeHost" ] info

[<Fact>]
let ``LetBind.TopLevel.Bug1650`` () =
    let info = Checker.getCompletionInfo "let x = {caret}"

    assertHasItemWithNames [ "System" ] info

[<Fact>]
let ``PrimTypeAndFunc`` () =
    let info1 =
        Checker.getCompletionInfo
            """
System.Int32.{caret} 
int. """

    assertHasItemWithNames [ "MinValue" ] info1

    let info2 =
        Checker.getCompletionInfo
            """
System.Int32. 
int.{caret} """

    assertHasNoItemsWithNames [ "MinValue" ] info2

[<Fact>]
let ``ThirdLevelOfDotting`` () =
    let info = Checker.getCompletionInfo "let x = System.Console.Wr{caret}"

    assertHasItemWithNames [ "BackgroundColor"; "CancelKeyPress" ] info

    for item in info.Items do
        match item.NameInCode with
        | "BackgroundColor" -> Assert.Equal(CompletionItemKind.Property, item.Kind)
        | "CancelKeyPress" -> Assert.Equal(CompletionItemKind.Event, item.Kind)
        | _ -> ()

[<Fact>]
let ``Expression.WithoutPreDefinedMethods`` () =
    let info =
        Checker.getCompletionInfo
            """
                let x = F{caret}"""

    assertHasNoItemsWithNames [ "FSharpDelegateEvent"; "PrivateMethod"; "PrivateType" ] info

[<Fact>]
let ``CaseInsensitive.MapMethod`` () =
    let info =
        Checker.getCompletionInfo
            """
                List.MaP{caret}
                """

    assertHasItemWithNames [ "map" ] info

[<Fact>]
let ``SimpleTypes.SystemTime`` () =
    let info =
        Checker.getCompletionInfo
            """
                let typestruct = System.DateTime.Now
                typestruct.{caret}"""

    assertHasItemWithNames [ "AddDays"; "Date" ] info

[<Theory>]
[<InlineData("#nowarn.{caret}")>]
[<InlineData("#define.{caret}")>]
[<InlineData("#define Foo.{caret}")>]
[<InlineData(" abcd.{caret}  ")>]
let ``MacroDirectives`` (source: string) =
    let info = Checker.getCompletionInfo source

    Assert.Equal(0, info.Items.Length)

[<Fact>]
let ``Identifier.This`` () =
    let info =
        Checker.getCompletionInfo
            """
                type Type1 =
                    member this.{caret}.Foo () = 3"""

    Assert.Equal(0, info.Items.Length)

[<Fact>]
let ``Regression4702.SystemWord`` () =
    let info = Checker.getCompletionInfo "System.{caret}"

    assertHasItemWithNames [ "Console"; "Byte"; "ArgumentException" ] info

[<Fact>]
let ``ExpressionDotting.Regression.Bug3709`` () =
    let info =
        Checker.getCompletionInfo
            """
                let foo = ""
                let foo = foo.E{caret}n "a" """

    assertHasItemWithNames [ "EndsWith" ] info

[<Fact>]
let ``ExpressionDotting.Regression.Bug187799.Test2`` () =
    let info =
        Checker.getCompletionInfo
            """
                type T() =
                    member _.M() = [|1..2|]
                type R = { P : T }
                // dotting through an F# record field
                let r = { P = T() }
                r.P.M().{caret}  """

    assertHasItemWithNames [ "Clone" ] info

[<Fact>]
let ``ExpressionDotting.Regression.Bug187799.Test3`` () =
    let info =
        Checker.getCompletionInfo
            """
                type R = { P : System.Reflection.InterfaceMapping  }
                // Dotting through an F# record field and an IL record field
                // Note that InterfaceMapping is a rare example of a public .NET instance field in mscorlib
                let r = { P = Unchecked.defaultof<System.Reflection.InterfaceMapping > }
                r.P.{caret}"""

    assertHasItemWithNames [ "InterfaceMethods" ] info

[<Fact>]
let ``ExpressionDotting.Regression.Bug187799.Test4`` () =
    let info =
        Checker.getCompletionInfo
            """
                type R = { P : System.Reflection.InterfaceMapping  }
                // Dotting through an F# record field and an IL record field
                // Note that InterfaceMapping is a rare example of a public .NET instance field in mscorlib
                let f() = { P = Unchecked.defaultof<System.Reflection.InterfaceMapping > }
                f().P.{caret}"""

    assertHasItemWithNames [ "InterfaceMethods" ] info

[<Fact>]
let ``ExpressionDotting.Regression.Bug187799.Test5`` () =
    let info =
        Checker.getCompletionInfo
            """
                type R = { P : System.Reflection.InterfaceMapping  }
                // Note that InterfaceMapping is a rare example of a public .NET instance field in mscorlib
                let f() = { P = Unchecked.defaultof<System.Reflection.InterfaceMapping > }
                f().P.InterfaceMethods.{caret}"""

    assertHasItemWithNames [ "GetEnumerator" ] info

[<Theory>]
[<InlineData("""
                type R = { P : System.AppDomain  }
                // Test dotting through an F# record field and a .NET event
                let f() = { P = null }
                f().P.UnhandledException.{caret}""", "AddHandler")>]
[<InlineData("""
                type R = { P : System.AppDomain  }
                // Test dotting through an F# record field and a .NET event
                let f() = { P = null }
                f().P.UnhandledException.GetType().{caret}""", "Assembly")>]
let ``ExpressionDotting.Regression.Bug187799.Test6`` (markedSource: string, expected: string) =
    let info = Checker.getCompletionInfo markedSource
    assertHasItemWithNames [ expected ] info

[<Fact(Skip = "non-FCS: the in-process FSharp.Compiler.Service.Tests host does not deploy FSharp.Compiler.Interactive.Settings.dll beside the loaded compiler, so the script closure cannot reference it and the `fsi` InteractiveSession object is not bound on either runtime; a real fsi/VS host (SDK layout) surfaces fsi.CommandLineArgs.")>]
let ``Fsx.Bug2530FsiObject`` () =
    let info = Checker.getCompletionInfo "fsi.{caret}"

    assertHasItemWithNames [ "CommandLineArgs" ] info
