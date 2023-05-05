// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Editor.Tests

open System
open System.Threading
open Xunit
open FSharp.Compiler.EditorServices
open FSharp.Compiler.CodeAnalysis
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.QuickInfo
open FSharp.Editor.Tests.Helpers
open FSharp.Test
open Internal.Utilities.CancellableTasks

type public AssemblyResolverTestFixture() =

    member public __.Init() = AssemblyResolver.addResolver ()

module QuickInfoProviderTests =
    type Expected =
        | QuickInfo of description: string * docs: string
        | Desc of string
        | Empty
        | Error

    type TestCase = TestCase of prompt: string * Expected

    let private normalizeLineEnds (s: string) =
        s.Replace("\r\n", "\n").Replace("\n\n", "\n")

    let mkFull prompt desc docs =
        TestCase(prompt, QuickInfo(normalizeLineEnds desc, normalizeLineEnds docs))

    let mkDesc prompt desc =
        TestCase(prompt, Desc(normalizeLineEnds desc))

    let mkNone prompt = TestCase(prompt, Empty)

    let filePath = "C:\\test.fs"

    let private tooltipElementToExpected expected =
        function
        | ToolTipElement.None -> Empty
        | ToolTipElement.Group (xs) ->
            let descriptions = xs |> List.map (fun item -> item.MainDescription)

            let descriptionTexts =
                descriptions
                |> List.map (fun taggedTexts -> taggedTexts |> Array.map (fun taggedText -> taggedText.Text))

            let descriptionText = descriptionTexts |> Array.concat |> String.concat ""

            let remarks = xs |> List.choose (fun item -> item.Remarks)

            let remarkTexts =
                remarks |> Array.concat |> Array.map (fun taggedText -> taggedText.Text)

            let remarkText =
                (match remarks with
                 | [] -> ""
                 | _ -> "\n" + String.concat "" remarkTexts)

            let tps = xs |> List.collect (fun item -> item.TypeMapping)

            let tpTexts =
                tps |> List.map (fun x -> x |> Array.map (fun y -> y.Text) |> String.concat "")

            let tpText =
                (match tps with
                 | [] -> ""
                 | _ -> "\n" + String.concat "\n" tpTexts)

            let collectDocs (element: ToolTipElementData) =
                match element.XmlDoc with
                | FSharp.Compiler.Symbols.FSharpXmlDoc.FromXmlText xmlDoc -> xmlDoc.UnprocessedLines |> String.concat "\n"
                | _ -> ""

            let desc =
                [ descriptionText; remarkText; tpText ] |> String.concat "" |> normalizeLineEnds

            let docs = xs |> List.map collectDocs |> String.concat "" |> normalizeLineEnds

            match expected with
            | QuickInfo _ -> QuickInfo(desc, docs)
            | _ -> Desc desc

        | ToolTipElement.CompositionError (error) -> Error

    let executeQuickInfoTest (programText: string) testCases =
        let document =
            RoslynTestHelpers.CreateSolution(programText)
            |> RoslynTestHelpers.GetSingleDocument

        for TestCase (symbol, expected) in testCases do
            let caretPosition = programText.IndexOf(symbol) + symbol.Length - 1

            let quickInfo =
                let task =
                    FSharpAsyncQuickInfoSource.TryGetToolTip(document, caretPosition)
                    |> CancellableTask.start CancellationToken.None

                task.Result

            let actual =
                quickInfo
                |> Option.map (fun (_, _, _, ToolTipText elements) -> elements |> List.map (tooltipElementToExpected expected))
                |> Option.defaultValue [ Empty ]

            actual.Head |> Assert.shouldBeEqualWith expected $"Symbol: {symbol}"

    [<Fact>]
    let ShouldShowQuickInfoAtCorrectPositions () =
        let fileContents =
            """
let x = 1
let y = 2
System.Console.WriteLine(x + y)
    """

        let testCases =
            [
                mkFull "let" "let" "Used to associate, or bind, a name to a value or function."
                mkDesc "x" "val x: int\nFull name: Test.x"
                mkDesc "y" "val y: int\nFull name: Test.y"
                mkNone "1"
                mkNone "2"
                mkDesc
                    "x +"
                    """val (+) : x: 'T1 -> y: 'T2 -> 'T3 (requires member (+))
Full name: Microsoft.FSharp.Core.Operators.(+)
'T1 is int
'T2 is int
'T3 is int"""
                mkDesc "System" "namespace System"
                mkDesc "WriteLine" "System.Console.WriteLine(value: int) : unit"
            ]

        executeQuickInfoTest fileContents testCases

    [<Fact>]
    let ShouldShowQuickKeywordInfoAtCorrectPositionsForSignatureFiles () =
        let fileContents =
            """
namespace TestNs
module internal MyModule =
    val MyVal: isDecl:bool -> string
        """

        let testCases =
            [
                mkFull
                    "namespace"
                    "namespace"
                    "Used to associate a name with a group of related types and modules, to logically separate it from other code."
                mkFull
                    "module"
                    "module"
                    "Used to associate a name with a group of related types, values, and functions, to logically separate it from other code."
                mkFull "internal" "internal" "Used to specify that a member is visible inside an assembly but not outside it."
                mkFull "val" "val" "Used in a signature to indicate a value, or in a type to declare a member, in limited situations."
                mkFull
                    "->"
                    "->"
                    "In function types, delimits arguments and return values. Yields an expression (in sequence expressions); equivalent to the yield keyword. Used in match expressions"
            ]

        executeQuickInfoTest fileContents testCases

    [<Fact>]
    let ShouldShowQuickKeywordInfoAtCorrectPositionsWithinComputationExpressions () =
        let fileContents =
            """
type MyOptionBuilder() = 
        member __.Zero() = None
        member __.Return(x: 'T) = Some x
        member __.Bind(m: 'T option, f) = Option.bind f m

let myOpt = MyOptionBuilder()
let x = 
    myOpt{
        let! x = Some 5
        let! y = Some 11
        return  x + y
    }
    """

        let testCases =
            [
                mkFull "let!" "let!" "Used in computation expressions to bind a name to the result of another computation expression."
                mkFull "return" "return" "Used to provide a value for the result of the containing computation expression."
            ]

        executeQuickInfoTest fileContents testCases

    [<Fact>]
    let ShouldShowQuickInfoForGenericParameters () =
        let fileContents =
            """
    
type C() = 
    member x.FSharpGenericMethodExplitTypeParams<'T>(a:'T, y:'T) = (a,y)

    member x.FSharpGenericMethodInferredTypeParams(a, y) = (a,y)

open System.Linq
let coll = [ for i in 1 .. 100 -> (i, string i) ]
let res1 = coll.GroupBy (fun (a, b) -> a)
let res2 = System.Array.Sort [| 1 |]
let test4 x = C().FSharpGenericMethodExplitTypeParams([x], [x])
let test5<'U> (x: 'U) = C().FSharpGenericMethodExplitTypeParams([x], [x])
let test6 = C().FSharpGenericMethodExplitTypeParams(1, 1)
let test7 x = C().FSharpGenericMethodInferredTypeParams([x], [x])
let test8 = C().FSharpGenericMethodInferredTypeParams(1, 1)
let test9<'U> (x: 'U) = C().FSharpGenericMethodInferredTypeParams([x], [x])
let res3 = [1] |> List.map id
let res4 = (1.0,[1]) ||> List.fold (fun s x -> string s + string x) // note there is a type error here, still cehck quickinfo any way
let res5 = 1 + 2
let res6 = System.DateTime.Now + System.TimeSpan.Zero
let res7 = sin 5.0
let res8 = abs 5.0<kg>
    """

        let testCases =

            [
                mkDesc
                    "GroupBy"
                    "(extension) System.Collections.Generic.IEnumerable.GroupBy<'TSource,'TKey>(keySelector: System.Func<'TSource,'TKey>) : System.Collections.Generic.IEnumerable<IGrouping<'TKey,'TSource>>
'TSource is int * string
'TKey is int"
                mkDesc
                    "Sort"
                    "System.Array.Sort<'T>(array: 'T array) : unit
'T is int"
                mkDesc
                    "let test4 x = C().FSharpGenericMethodExplitTypeParams"
                    "member C.FSharpGenericMethodExplitTypeParams: a: 'T0 * y: 'T0 -> 'T0 * 'T0
'T is 'a list"
                mkDesc
                    "let test5<'U> (x: 'U) = C().FSharpGenericMethodExplitTypeParams"
                    "member C.FSharpGenericMethodExplitTypeParams: a: 'T0 * y: 'T0 -> 'T0 * 'T0
'T is 'U list"
                mkDesc
                    "let test6 = C().FSharpGenericMethodExplitTypeParams"
                    "member C.FSharpGenericMethodExplitTypeParams: a: 'T0 * y: 'T0 -> 'T0 * 'T0
'T is int"
                mkDesc
                    "let test7 x = C().FSharpGenericMethodInferredTypeParams"
                    "member C.FSharpGenericMethodInferredTypeParams: a: 'a1 * y: 'b2 -> 'a1 * 'b2
'a is 'a0 list
'b is 'a0 list"
                mkDesc
                    "let test8 = C().FSharpGenericMethodInferredTypeParams"
                    "member C.FSharpGenericMethodInferredTypeParams: a: 'a0 * y: 'b1 -> 'a0 * 'b1
'a is int
'b is int"
                mkDesc
                    "let test9<'U> (x: 'U) = C().FSharpGenericMethodInferredTypeParams"
                    "member C.FSharpGenericMethodInferredTypeParams: a: 'a0 * y: 'b1 -> 'a0 * 'b1
'a is 'U list
'b is 'U list"
                mkDesc
                    "let res3 = [1] |>"
                    "val (|>) : arg: 'T1 -> func: ('T1 -> 'U) -> 'U
Full name: Microsoft.FSharp.Core.Operators.(|>)
'T1 is int list
'U is int list"
                mkDesc
                    "let res3 = [1] |> List.map id"
                    "val id: x: 'T -> 'T
Full name: Microsoft.FSharp.Core.Operators.id
'T is int"
                mkDesc
                    "let res4 = (1.0,[1]) ||>"
                    "val (||>) : arg1: 'T1 * arg2: 'T2 -> func: ('T1 -> 'T2 -> 'U) -> 'U
Full name: Microsoft.FSharp.Core.Operators.(||>)
'T1 is float
'T2 is int list
'U is float"
                mkDesc
                    "let res4 = (1.0,[1]) ||> List.fold"
                    "val fold: folder: ('State -> 'T -> 'State) -> state: 'State -> list: 'T list -> 'State
Full name: Microsoft.FSharp.Collections.List.fold
'T is int
'State is float"
                mkDesc
                    "let res4 = (1.0,[1]) ||> List.fold (fun s x -> string s +"
                    "val (+) : x: 'T1 -> y: 'T2 -> 'T3 (requires member (+))
Full name: Microsoft.FSharp.Core.Operators.(+)
'T1 is string
'T2 is string
'T3 is float"
                mkDesc
                    "let res5 = 1 +"
                    "val (+) : x: 'T1 -> y: 'T2 -> 'T3 (requires member (+))
Full name: Microsoft.FSharp.Core.Operators.(+)
'T1 is int
'T2 is int
'T3 is int"
                mkDesc
                    "let res6 = System.DateTime.Now +"
                    "val (+) : x: 'T1 -> y: 'T2 -> 'T3 (requires member (+))
Full name: Microsoft.FSharp.Core.Operators.(+)
'T1 is System.DateTime
'T2 is System.TimeSpan
'T3 is System.DateTime"
                mkDesc
                    "let res7 = sin"
                    "val sin: value: 'T -> 'T (requires member Sin)
Full name: Microsoft.FSharp.Core.Operators.sin
'T is float"
                mkDesc
                    "let res8 = abs"
                    "val abs: value: 'T -> 'T (requires member Abs)
Full name: Microsoft.FSharp.Core.Operators.abs
'T is int"
            ]

        executeQuickInfoTest fileContents testCases
