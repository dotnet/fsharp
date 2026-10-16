module FSharp.Compiler.Service.Tests.ParameterInfoLambdasTests

open Xunit

[<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
let ``Regression.LocationOfParams.Bug91479`` () =
    assertHasParameterInfo "let z = fun x -> x + System.Int16.Parse({caret} "

[<Fact(Skip = "https://github.com/dotnet/fsharp/issues/6166")>]
let ``Multi.DotNet.StaticMethod.WithinLambda`` () =
    assertParameterInfoContains ["string"; "System.Globalization.NumberStyles"] """let z = fun x -> x + System.Int16.Parse("",{caret}"""

[<Fact>]
let ``Multi.DotNet.StaticMethod.WithinLambda2`` () =
    assertParameterInfoOverloads [ ["fileName: string"] ] "let _ = fun file -> new System.IO.FileInfo({caret}"
