namespace Language

open Xunit
open FSharp.Test.Compiler
open FSharp.Test

module ExperimentalAttributeCheckingTests =
    

    [<FactForNETCOREAPP>]
    let ``C# Experimental(diagnosticId) attribute warning is taken into account`` () =
        let CSLib =
            CSharp """
using System.Diagnostics.CodeAnalysis;

namespace MyLib;

[Experimental("MY001")]
public static class Class1
{
    public static string Test()
    {
        return "Hello";
    }
}
        """
            |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp12
            |> withName "CSLib"

        let app =
            Fsx """
open MyLib

let text = Class1.Test()
        """ |> withReferences [CSLib]

        app
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 57, Line 4, Col 12, Line 4, Col 18, """This construct is experimental. This warning can be disabled using '--nowarn:57' or '#nowarn "57"'.""")
            (Warning 57, Line 4, Col 12, Line 4, Col 23, """This construct is experimental. This warning can be disabled using '--nowarn:57' or '#nowarn "57"'.""")
        ]

    [<FactForNETCOREAPP>]
    let ``C# Experimental(UrlFormat) attribute warning is taken into account`` () =
        let CSLib =
            CSharp """
using System.Diagnostics.CodeAnalysis;

namespace MyLib;

[Experimental("MY001", UrlFormat = "https://contoso.com/obsoletion-warnings")]
public static class Class1
{
    public static string Test()
    {
        return "Hello";
    }
}
        """
            |> withCSharpLanguageVersion CSharpLanguageVersion.CSharp12
            |> withName "CSLib"

        let app =
            Fsx """
open MyLib

let text = Class1.Test()
        """ |> withReferences [CSLib]

        app
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 57, Line 4, Col 12, Line 4, Col 18, """This construct is experimental. This warning can be disabled using '--nowarn:57' or '#nowarn "57"'.""")
            (Warning 57, Line 4, Col 12, Line 4, Col 23, """This construct is experimental. This warning can be disabled using '--nowarn:57' or '#nowarn "57"'.""")
        ]

    [<FactForNETCOREAPP>]
    let ``F# Experimental attribute warning is taken into account`` () =
        Fsx """
[<Experimental("Use with caution")>]
module Class1 =
    let Test() = "Hello"

let text = Class1.Test()
        """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 57, Line 6, Col 12, Line 6, Col 18, """This construct is experimental. Use with caution. This warning can be disabled using '--nowarn:57' or '#nowarn "57"'.""")
        ]

    [<Fact>]
    let ``ExperimentalAttribute nowarn when preview specified``() =
        Fsx """
module TestModule =

    [<ExperimentalAttribute("Preview library feature, requires '--langversion:preview'")>]
    let getString = "A string"

    if getString = "A string" then ()
    """
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``ExperimentalAttribute warn when preview not specified``() =
        Fsx """
module TestModule =

    [<ExperimentalAttribute("Preview library feature, requires '--langversion:preview'")>]
    let getString = "A string"

    if getString = "A string" then ()
    """
        |> compile
        |> shouldFail
        |> withDiagnostics [
            (Warning 57, Line 7, Col 8, Line 7, Col 17, """This construct is experimental. Preview library feature, requires '--langversion:preview'. This warning can be disabled using '--nowarn:57' or '#nowarn "57"'.""")
        ]
    