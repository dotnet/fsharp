namespace Language

open Xunit
open FSharp.Test.Compiler
open FSharp.Test

module ExperimentalAttributeCheckingTests =
    
    [<Fact>]
    let ``Il Experimental attribute warning is taken into account(diagnosticId)`` () =
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

    [<Fact>]
    let ``Il Experimental attribute warning is taken into account`` () =
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

    [<Fact>]
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
            (Warning 57, Line 6, Col 12, Line 6, Col 18, """Use with caution. This warning can be disabled using '--nowarn:57' or '#nowarn "57"'.""")
        ]