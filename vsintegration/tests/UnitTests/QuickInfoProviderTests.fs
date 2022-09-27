// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// ------------------------------------------------------------------------------------------------------------------------
//
// To run the tests in this file: Compile VisualFSharp.UnitTests.dll and run it as a set of unit tests
// ------------------------------------------------------------------------------------------------------------------------


namespace VisualFSharp.UnitTests.Editor

open System
open NUnit.Framework
open Microsoft.VisualStudio.FSharp
open FSharp.Compiler.EditorServices
open FSharp.Compiler.CodeAnalysis
open Microsoft.CodeAnalysis
open Microsoft.VisualStudio.FSharp.Editor

[<SetUpFixture>]
type public AssemblyResolverTestFixture () =

    [<OneTimeSetUp>]
    member public __.Init () = AssemblyResolver.addResolver ()

[<NUnit.Framework.Category "Roslyn Services">]
module QuickInfoProviderTests =

let filePath = "C:\\test.fs"

let internal projectOptions = { 
    ProjectFileName = "C:\\test.fsproj"
    ProjectId = None
    SourceFiles =  [| filePath |]
    ReferencedProjects = [| |]
    OtherOptions = [| |]
    IsIncompleteTypeCheckEnvironment = true
    UseScriptResolutionRules = false
    LoadTime = DateTime.MaxValue
    OriginalLoadReferences = []
    UnresolvedReferences = None
    Stamp = None
}

let private normalizeLineEnds (s: string) = s.Replace("\r\n", "\n").Replace("\n\n", "\n")

let private tooltipTextToRawString (ToolTipText elements) : string =
    let rec parseElement = function
        | ToolTipElement.None -> ""
        | ToolTipElement.Group(xs) -> 
            let descriptions = xs |> List.map (fun item -> item.MainDescription)
            let descriptionTexts = descriptions |> List.map (fun taggedTexts -> taggedTexts |> Array.map (fun taggedText -> taggedText.Text))
            let descriptionText = descriptionTexts |> Array.concat |> String.concat ""
            
            let remarks = xs |> List.choose (fun item -> item.Remarks)
            let remarkTexts = remarks |> Array.concat |> Array.map (fun taggedText -> taggedText.Text)
            let remarkText = (match remarks with [] -> "" | _ -> "\n" + String.concat "" remarkTexts)
            
            let tps = xs |> List.collect (fun item -> item.TypeMapping)
            let tpTexts = tps |> List.map (fun x -> x |> Array.map (fun y -> y.Text) |> String.concat "")
            let tpText = (match tps with [] -> "" | _ -> "\n" + String.concat "\n" tpTexts)

            descriptionText + remarkText + tpText
        | ToolTipElement.CompositionError(error) -> error
    elements |> List.map parseElement |> String.concat "\n" |> normalizeLineEnds

let executeQuickInfoTest (programText:string) testCases = 
    let document, _ = RoslynTestHelpers.CreateSingleDocumentSolution(filePath, programText)
    Assert.Multiple(fun _ -> 
        for (symbol: string, expected: string option) in testCases do
            let expected = expected |> Option.map normalizeLineEnds |> Option.map (fun s -> s.Replace("___",""))      
            let caretPosition = programText.IndexOf(symbol) + symbol.Length - 1

            let quickInfo =
                FSharpAsyncQuickInfoSource.ProvideQuickInfo(document, caretPosition)
                |> Async.RunSynchronously
        
            let actual = quickInfo |> Option.map (fun qi -> tooltipTextToRawString qi.StructuredText)
            Assert.AreEqual(expected, actual,"Symbol: " + symbol)
    )

[<Test>]
let ShouldShowQuickInfoAtCorrectPositions() =
    let fileContents = """
let x = 1
let y = 2
System.Console.WriteLine(x + y)
    """

    let testCases = 
       [ "let", Some "let___Used to associate, or bind, a name to a value or function."
         "x", Some "val x: int\nFull name: Test.x"
         "y", Some "val y: int\nFull name: Test.y"
         "1", None
         "2", None
         "x +", Some """val (+) : x: 'T1 -> y: 'T2 -> 'T3 (requires member (+))
Full name: Microsoft.FSharp.Core.Operators.(+)
'T1 is int
'T2 is int
'T3 is int"""
         "System", Some "namespace System"
         "WriteLine", Some "System.Console.WriteLine(value: int) : unit" 
       ]

    executeQuickInfoTest fileContents testCases
   

[<Test>]
let ShouldShowQuickKeywordInfoAtCorrectPositionsForSignatureFiles() =
    let fileContents = """
namespace TestNs
module internal MyModule =
    val MyVal: isDecl:bool -> string
        """
    let testCases = 
       [ "namespace", Some "namespace___Used to associate a name with a group of related types and modules, to logically separate it from other code."
         "module", Some "module___Used to associate a name with a group of related types, values, and functions, to logically separate it from other code."
         "internal", Some "internal___Used to specify that a member is visible inside an assembly but not outside it."
         "val", Some "val___Used in a signature to indicate a value, or in a type to declare a member, in limited situations."
         "->", Some "->___In function types, delimits arguments and return values. Yields an expression (in sequence expressions); equivalent to the yield keyword. Used in match expressions"        
       ]
    executeQuickInfoTest fileContents testCases

[<Test>]
let ShouldShowQuickKeywordInfoAtCorrectPositionsWithinComputationExpressions() =
    let fileContents = """
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
       [ "let!", Some "let!___Used in computation expressions to bind a name to the result of another computation expression."
         "return", Some "return___Used to provide a value for the result of the containing computation expression."           
       ]

    executeQuickInfoTest fileContents testCases

[<Test>]
let ShouldShowQuickInfoForGenericParameters() =
    let fileContents = """

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

        [("GroupBy",
          Some
            "(extension) System.Collections.Generic.IEnumerable.GroupBy<'TSource,'TKey>(keySelector: System.Func<'TSource,'TKey>) : System.Collections.Generic.IEnumerable<IGrouping<'TKey,'TSource>>
'TSource is int * string
'TKey is int");
         ("Sort", Some "System.Array.Sort<'T>(array: 'T array) : unit
'T is int");
         ("let test4 x = C().FSharpGenericMethodExplitTypeParams",
          Some
            "member C.FSharpGenericMethodExplitTypeParams: a: 'T0 * y: 'T0 -> 'T0 * 'T0
'T is 'a list");
         ("let test5<'U> (x: 'U) = C().FSharpGenericMethodExplitTypeParams",
          Some
            "member C.FSharpGenericMethodExplitTypeParams: a: 'T0 * y: 'T0 -> 'T0 * 'T0
'T is 'U list");
         ("let test6 = C().FSharpGenericMethodExplitTypeParams",
          Some
            "member C.FSharpGenericMethodExplitTypeParams: a: 'T0 * y: 'T0 -> 'T0 * 'T0
'T is int");
         ("let test7 x = C().FSharpGenericMethodInferredTypeParams",
          Some
            "member C.FSharpGenericMethodInferredTypeParams: a: 'a1 * y: 'b2 -> 'a1 * 'b2
'a is 'a0 list
'b is 'a0 list");
         ("let test8 = C().FSharpGenericMethodInferredTypeParams",
          Some
            "member C.FSharpGenericMethodInferredTypeParams: a: 'a0 * y: 'b1 -> 'a0 * 'b1
'a is int
'b is int");
         ("let test9<'U> (x: 'U) = C().FSharpGenericMethodInferredTypeParams",
          Some
            "member C.FSharpGenericMethodInferredTypeParams: a: 'a0 * y: 'b1 -> 'a0 * 'b1
'a is 'U list
'b is 'U list");
         ("let res3 = [1] |>",
          Some
            "val (|>) : arg: 'T1 -> func: ('T1 -> 'U) -> 'U
Full name: Microsoft.FSharp.Core.Operators.(|>)
'T1 is int list
'U is int list");
         ("let res3 = [1] |> List.map id",
          Some
            "val id: x: 'T -> 'T
Full name: Microsoft.FSharp.Core.Operators.id
'T is int");
         ("let res4 = (1.0,[1]) ||>",
          Some
            "val (||>) : arg1: 'T1 * arg2: 'T2 -> func: ('T1 -> 'T2 -> 'U) -> 'U
Full name: Microsoft.FSharp.Core.Operators.(||>)
'T1 is float
'T2 is int list
'U is float");
         ("let res4 = (1.0,[1]) ||> List.fold",
          Some
            "val fold: folder: ('State -> 'T -> 'State) -> state: 'State -> list: 'T list -> 'State
Full name: Microsoft.FSharp.Collections.List.fold
'T is int
'State is float");
         ("let res4 = (1.0,[1]) ||> List.fold (fun s x -> string s +",
          Some
            "val (+) : x: 'T1 -> y: 'T2 -> 'T3 (requires member (+))
Full name: Microsoft.FSharp.Core.Operators.(+)
'T1 is string
'T2 is string
'T3 is float");
         ("let res5 = 1 +",
          Some
            "val (+) : x: 'T1 -> y: 'T2 -> 'T3 (requires member (+))
Full name: Microsoft.FSharp.Core.Operators.(+)
'T1 is int
'T2 is int
'T3 is int");
         ("let res6 = System.DateTime.Now +",
          Some
            "val (+) : x: 'T1 -> y: 'T2 -> 'T3 (requires member (+))
Full name: Microsoft.FSharp.Core.Operators.(+)
'T1 is System.DateTime
'T2 is System.TimeSpan
'T3 is System.DateTime");
         ("let res7 = sin",
          Some
            "val sin: value: 'T -> 'T (requires member Sin)
Full name: Microsoft.FSharp.Core.Operators.sin
'T is float");
         ("let res8 = abs",
          Some
            "val abs: value: 'T -> 'T (requires member Abs)
Full name: Microsoft.FSharp.Core.Operators.abs
'T is int")]    

    executeQuickInfoTest fileContents testCases