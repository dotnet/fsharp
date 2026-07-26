module FSharp.Compiler.Service.Tests.CompletionTypeProvidersTests

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open Xunit

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.VisibilityChecksForGeneratedTypes`` () =
    let info =
        Checker.getCompletionInfoWithCompilerAndCompletionOptions
            [| "-r:" + PathRelativeToTestAssembly("DummyProviderForLanguageServiceTesting.dll") |]
            FSharpCodeCompletionOptions.Default
            """
type T = GeneratedType.SampleType
let t = T(5)
t.{caret}"""

    assertHasItemWithNames [ "PublicM"; "PublicProp" ] info
    assertHasNoItemsWithNames [ "f"; "ProtectedProp"; "PrivateProp"; "ProtectedM"; "PrivateM" ] info

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.EditorHideMethodsAttribute.InstanceMethod.CtrlSpaceCompletionContains`` () =
    let info =
        Checker.getCompletionInfoWithCompilerAndCompletionOptions
            [| "-r:" + PathRelativeToTestAssembly("DummyProviderForLanguageServiceTesting.dll") |]
            FSharpCodeCompletionOptions.Default
            """
let t = new N1.T1()
t.I{caret}"""

    assertHasItemWithNames [ "IM1" ] info

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.EditorHideMethodsAttribute.Event.CtrlSpaceCompletionContains`` () =
    let info =
        Checker.getCompletionInfoWithCompilerAndCompletionOptions
            [| "-r:" + PathRelativeToTestAssembly("EditorHideMethodsAttribute.dll") |]
            FSharpCodeCompletionOptions.Default
            """
let t = new N.T()
t.Eve{caret}"""

    assertHasItemWithNames [ "Event1" ] info

[<Fact(Skip = "non-FCS: needs a type provider not available to the FCS test harness.")>]
let ``TypeProvider.EditorHideMethodsAttribute.Type.CtrlSpaceCompletionContains`` () =
    let info =
        Checker.getCompletionInfoWithCompilerAndCompletionOptions
            [| "-r:" + PathRelativeToTestAssembly("DummyProviderForLanguageServiceTesting.dll") |]
            FSharpCodeCompletionOptions.Default
            """
type boo = N1.T<in{caret}"""

    assertHasItemWithNames [ "int" ] info

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.EditorHideMethodsAttribute.Type.DoesnotContain`` () =
    let info =
        Checker.getCompletionInfoWithCompilerAndCompletionOptions
            [| "-r:" + PathRelativeToTestAssembly("EditorHideMethodsAttribute.dll") |]
            FSharpCodeCompletionOptions.Default
            """
let t = new N.T()
t.{caret}"""

    assertHasNoItemsWithNames [ "Equals"; "GetHashCode" ] info

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.EditorHideMethodsAttribute.Type.Contains`` () =
    let info =
        Checker.getCompletionInfoWithCompilerAndCompletionOptions
            [| "-r:" + PathRelativeToTestAssembly("EditorHideMethodsAttribute.dll") |]
            FSharpCodeCompletionOptions.Default
            """
let t = new N.T()
t.{caret}"""

    assertHasItemWithNames [ "Event1" ] info

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.EditorHideMethodsAttribute.InstanceMethod.Contains`` () =
    let info =
        Checker.getCompletionInfoWithCompilerAndCompletionOptions
            [| "-r:" + PathRelativeToTestAssembly("DummyProviderForLanguageServiceTesting.dll") |]
            FSharpCodeCompletionOptions.Default
            """
let t = new N1.T1()
t.{caret}"""

    assertHasItemWithNames [ "IM1" ] info

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.TypeContainsNestedType`` () =
    let info =
        Checker.getCompletionInfoWithCompilerAndCompletionOptions
            [| "-r:" + PathRelativeToTestAssembly("DummyProviderForLanguageServiceTesting.dll") |]
            FSharpCodeCompletionOptions.Default
            """
type XXX = N1.T1.{caret}"""

    assertHasItemWithNames [ "SomeNestedType" ] info

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.EditorHideMethodsAttribute.Event.Contain`` () =
    let info =
        Checker.getCompletionInfoWithCompilerAndCompletionOptions
            [| "-r:" + PathRelativeToTestAssembly("EditorHideMethodsAttribute.dll") |]
            FSharpCodeCompletionOptions.Default
            """
let t = new N.T()
t.Event1.{caret}"""

    assertHasItemWithNames [ "AddHandler"; "RemoveHandler" ] info

[<Fact(Skip = "non-FCS: needs a type provider not available to the FCS test harness.")>]
let ``TypeProvider.EditorHideMethodsAttribute.Method.Contain`` () =
    let info =
        Checker.getCompletionInfoWithCompilerAndCompletionOptions
            [| "-r:" + PathRelativeToTestAssembly("EditorHideMethodsAttribute.dll") |]
            FSharpCodeCompletionOptions.Default
            """
let t = N.T.M.{caret}()"""

    Assert.Equal(0, info.Items.Length)

[<Fact(Skip = "Bug https://github.com/dotnet/fsharp/issues/17330")>]
let ``TypeProvider.EditorHideMethodsAttribute.Property.Contain`` () =
    let info =
        Checker.getCompletionInfoWithCompilerAndCompletionOptions
            [| "-r:" + PathRelativeToTestAssembly("EditorHideMethodsAttribute.dll") |]
            FSharpCodeCompletionOptions.Default
            """
let t = N.T.StaticProp.{caret}"""

    assertHasItemWithNames [ "GetType"; "Equals" ] info
