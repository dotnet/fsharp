#if INTERACTIVE
#r "../../artifacts/bin/fcs/net461/FSharp.Compiler.Service.dll" // note, build FSharp.Compiler.Service.Tests.fsproj to generate this, this DLL has a public API so can be used from F# Interactive
#r "../../artifacts/bin/fcs/net461/nunit.framework.dll"
#load "FsUnit.fs"
#load "Common.fs"
#else
module Tests.Service.XmlDocTests.UnitsOfMeasures
#endif

open Tests.Service.XmlDocTests.XmlDoc
open FSharp.Compiler.Service.Tests.Common
open FSharp.Test.Compiler
open NUnit.Framework

// TODO: 12517: https://github.com/dotnet/fsharp/issues/12517
// https://github.com/dotnet/fsharp/blob/6c6588730c4d650a354e5ea3d46fb4630d7bba01/tests/fsharpqa/Source/XmlDoc/UnitOfMeasure/UnitOfMeasure01.fs
[<Test; Ignore("https://github.com/dotnet/fsharp/issues/12517")>]
let ``Regression test for Dev11:390683, 388264 - UnitOfMeasure01.fs``(): unit =
    let parseResults, checkResults = getParseAndCheckResultsOfSignatureFile """
namespace Test

open Microsoft.FSharp.Data.UnitSystems.SI.UnitNames

/// <summary>This is A</summary>
/// <param name="p1">This is pA</param>
let A (pA : float<ampere>) = pA / 1.<ampere>

/// <summary>This is B</summary>
/// <param name="p1">This is pB</param>
let B (pB : single) = 1.f<ampere> * pB

"""
    parseResults |> checkParsingErrors [||]
    checkResults |> checkXmlSymbols [ Entity  "Test.A", [|"<summary>This is A</summary>"|] ]
    checkResults |> checkXmlSymbols [ Entity  "Test.B", [|"<summary>This is B</summary>"|] ]
    // Include checkresults for <param>


// https://github.com/dotnet/fsharp/blob/6c6588730c4d650a354e5ea3d46fb4630d7bba01/tests/fsharpqa/Source/XmlDoc/UnitOfMeasure/UnitOfMeasure02.fs
[<Test; Ignore("https://github.com/dotnet/fsharp/issues/12517")>]
let ``Regression test for Dev11:390683, 388264 - UnitOfMeasure02.fs``(): unit =
    let parseResults, checkResults = getParseAndCheckResultsOfSignatureFile """

// Similar to UnitOfMeasure01, but with 2 params

open Microsoft.FSharp.Data.UnitSystems.SI.UnitNames

/// <summary>This is A</summary>
/// <param name="pA">This is pA</param>
let A (pA : float<ampere/ohm>) = pA / 1.<ampere>

/// <summary>This is B</summary>
/// <param name="pB1">This is pB1</param>
/// <param name="pB2">This is pB2</param>
let B (pB1 : int<meter>, pB2 : int<meter^2>) = pB1 * pB2

"""
    parseResults |> checkParsingErrors [||]
    checkResults |> checkXmlSymbols [ Field  "Test.DU.B", [|"<summary>This is B</summary>"|] ]
    checkResults |> checkXmlSymbols [ Field  "Test.DU.A", [|"<summary>This is A</summary>"|] ]
// Include checkresults for <param>


// https://github.com/dotnet/fsharp/blob/6c6588730c4d650a354e5ea3d46fb4630d7bba01/tests/fsharpqa/Source/XmlDoc/UnitOfMeasure/UnitOfMeasure03.fs
[<Test; Ignore("https://github.com/dotnet/fsharp/issues/12517")>]
let ``Regression test for Dev11:390683, 388264 -- UnitOfMeasure03.fs``(): unit =
    let parseResults, checkResults = getParseAndCheckResultsOfSignatureFile """
type T =
/// <summary>This is A</summary>
/// <param name="pA">This is pA</param>
member __.A (pA : float<ampere/ohm>) = pA / 1.<ampere>

/// <summary>This is B</summary>
/// <param name="pB1">This is pB1</param>
/// <param name="pB2">This is pB2</param>
static member B (pB1 : int<meter>, pB2 : int<meter^2>) = pB1 * pB2
"""
    parseResults |> checkParsingErrors [||]
    checkResults |> checkXmlSymbols [ Field  "Test.DU.B", [|"<summary>This is B</summary>"|] ]
    checkResults |> checkXmlSymbols [ Field  "Test.DU.A", [|"<summary>This is A</summary>"|] ]
/// Need to verify xmldoc for parameters parameters


// https://github.com/dotnet/fsharp/blob/6c6588730c4d650a354e5ea3d46fb4630d7bba01/tests/fsharpqa/Source/XmlDoc/UnitOfMeasure/UnitOfMeasure04.fs
[<Test>]
let ``Regression test for Dev11:390683, 388264 -- UnitOfMeasure04.fs``(): unit =
    let parseResults, checkResults = getParseAndCheckResultsOfSignatureFile """
namespace UnitOfMeasure04

// Similar to UnitOfMeasure03, but with the UoM in a DU

open Microsoft.FSharp.Data.UnitSystems.SI.UnitNames

type DU =
    ///<summary>This is A</summary>
    | A of float<ampere/ohm>

    ///<summary>This is B</summary>
    | B of (int<meter> * int64<meter^2>)
"""
    parseResults |> checkParsingErrors [||]
    checkResults |> checkXmlSymbols [ Field  "UnitOfMeasure04.DU.B", [|"<summary>This is B</summary>"|] ]
    checkResults |> checkXmlSymbols [ Field  "UnitOfMeasure04.DU.A", [|"<summary>This is A</summary>"|] ]
