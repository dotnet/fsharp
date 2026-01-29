module FSharpChecker.FindReferences

open System.Threading.Tasks
open Xunit
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.CodeAnalysis.ProjectSnapshot
open FSharp.Compiler.Text
open FSharp.Test.ProjectGeneration
open FSharp.Test.ProjectGeneration.Helpers

#nowarn "57"

type Occurrence = Definition | InType | Use

let deriveOccurrence (su:FSharpSymbolUse) =
    if su.IsFromDefinition 
    then Definition
    elif su.IsFromType
    then InType
    elif su.IsFromUse
    then Use
    else failwith $"Unexpected type of occurrence (for this test), symbolUse = {su}" 

// =============================================================================
// Test Helpers - Reduce boilerplate in single-file find-references tests
// =============================================================================

/// Finds all references to a symbol in source code using singleFileChecker.
/// Returns a list of (fileName, line, startCol, endCol) tuples.
let findRefsInSource source symbolName =
    let fileName, options, checker = singleFileChecker source
    let symbolUse = getSymbolUse fileName source symbolName options checker |> Async.RunSynchronously
    checker.FindBackgroundReferencesInFile(fileName, options, symbolUse.Symbol)
    |> Async.RunSynchronously

/// Runs a complete find-references test: finds symbol and asserts expected ranges.
let testFindRefsInSource source symbolName expectedRanges =
    findRefsInSource source symbolName |> expectToFind expectedRanges

/// Asserts that the given ranges contain exactly the expected line numbers.
let expectLines expectedLines (ranges: range seq) =
    let actualLines = ranges |> Seq.map (fun r -> r.StartLine) |> Seq.sort |> Seq.distinct |> Seq.toList
    Assert.Equal<int list>(expectedLines, actualLines)

/// Asserts that the given ranges match the expected (line, startCol, endCol) tuples.
let expectRanges expected (ranges: range seq) =
    let actual =
        ranges
        |> Seq.sortBy (fun r -> r.StartLine, r.StartColumn)
        |> Seq.map (fun r -> r.StartLine, r.StartColumn, r.EndColumn)
        |> Seq.toArray
    Assert.Equal<(int * int * int) array>(expected, actual)

/// Asserts that ranges include references at all specified line numbers.
let expectLinesInclude expectedLines (ranges: range list) =
    let actualLines = ranges |> List.map (fun r -> r.StartLine) |> Set.ofList
    for line in expectedLines do
        Assert.True(actualLines.Contains(line), $"Expected reference on line {line}. Ranges: {ranges}")

/// Asserts a minimum number of references.
let expectMinRefs minCount (ranges: range list) =
    Assert.True(ranges.Length >= minCount, $"Expected at least {minCount} references, got {ranges.Length}")

/// Shorthand for simple find-all-references tests with SyntheticProject.
let testFindAllRefs source symbolName assertion =
    SyntheticProject.Create({ sourceFile "Source" [] with Source = source })
        .Workflow {
            placeCursor "Source" symbolName
            findAllReferences assertion
        }

/// Shorthand for find-all-references tests expecting minimum count.
let testFindAllRefsMin source symbolName minCount =
    testFindAllRefs source symbolName (expectMinRefs minCount)

/// https://github.com/dotnet/fsharp/issues/13199
let reproSourceCode = """
type MyType() = 
    member x.DoNothing(d:MyType) = ()

let a = MyType()
let b = new MyType()
a.DoNothing(b)
"""
let impFile() = { sourceFile "First" [] with ExtraSource = reproSourceCode }
let createProject() = SyntheticProject.Create(impFile())

[<Fact>]
let ``Finding usage of type via GetUsesOfSymbolInFile should also find it's constructors`` () =
    createProject().Workflow
        {        
            checkFile "First" (fun (typeCheckResult: FSharpCheckFileResults) ->
             
                let symbolUse = typeCheckResult.GetSymbolUseAtLocation(7, 11, "type MyType() =", ["MyType"]).Value
                let references = 
                    typeCheckResult.GetUsesOfSymbolInFile(symbolUse.Symbol) 
                    |> Array.sortBy (fun su -> su.Range.StartLine)
                    |> Array.map (fun su -> su.Range.StartLine, su.Range.StartColumn, su.Range.EndColumn, deriveOccurrence su)

                Assert.Equal<(int*int*int*Occurrence)>(
                    [| 7,5,11,Definition
                       8,25,31,InType
                       10,8,14,Use
                       11,12,18,Use
                    |],references)  )           
        }


[<Fact>]
let ``Finding usage of type via FindReference should also find it's constructors`` () =
    createProject().Workflow {
        placeCursor "First" 7 11 "type MyType() =" ["MyType"]
        findAllReferencesInFile "First" (fun ranges ->
            expectRanges [| 7,5,11; 8,25,31; 10,8,14; 11,12,18 |] ranges)
    }

[<Fact>]
let ``Finding usage of type via FindReference works across files`` () =
    let secondFile = { sourceFile "Second" ["First"] with ExtraSource = """
open ModuleFirst
let secondA = MyType()
let secondB = new MyType()
secondA.DoNothing(secondB)
 """}
    let original = createProject()
    let project = {original with SourceFiles = original.SourceFiles @ [secondFile]}
    project.Workflow {
        placeCursor "First" 7 11 "type MyType() =" ["MyType"]
        findAllReferencesInFile "Second" (fun ranges ->
            expectRanges [| 9,14,20; 10,18,24 |] ranges)
    }

[<Theory>]
[<InlineData(true, true)>]
[<InlineData(true, false)>]
[<InlineData(false, true)>]
[<InlineData(false, false)>]
let ``Finding references in project`` (fastCheck, captureIdentifiersWhenParsing) =
    let size = 20

    let project =
        { SyntheticProject.Create() with
            SourceFiles = [
                sourceFile $"File%03d{0}" [] |> addSignatureFile
                for i in 1..size do
                    sourceFile $"File%03d{i}" [$"File%03d{i-1}"]
            ]
        }
        |> updateFile "File005" (addDependency "File000")
        |> updateFile "File010" (addDependency "File000")

    let checker = FSharpChecker.Create(
        enableBackgroundItemKeyStoreAndSemanticClassification = true,
        captureIdentifiersWhenParsing = captureIdentifiersWhenParsing)

    project.WorkflowWith checker {
        findAllReferencesToModuleFromFile "File000" fastCheck (expectNumberOfResults 5)
    }

[<Fact>]
let ``Find references to internal symbols in other projects`` () =
    let library = {
        SyntheticProject.Create("Library",
            { sourceFile "Library" [] with Source = """
namespace Lib

module internal Library =
    let foo x = x + 5

[<assembly: System.Runtime.CompilerServices.InternalsVisibleTo("FileFirst")>]
do ()    """ })
            with AutoAddModules = false }

    let project =
        { SyntheticProject.Create("App",
            { sourceFile "First" [] with Source = """
open Lib
let bar x = Library.foo x""" })
                with DependsOn = [library] }

    project.Workflow {
        placeCursor "Library" "foo"
        findAllReferences (expectToFind [
            "FileFirst.fs", 4, 12, 23
            "FileLibrary.fs", 5, 8, 11
        ])
    }


[<Fact>]
let ``We find back-ticked identifiers`` () =
    SyntheticProject.Create(
        { sourceFile "First" [] with ExtraSource = "let ``foo bar`` x = x + 5" },
        { sourceFile "Second" [] with ExtraSource = "let foo x = ModuleFirst.``foo bar`` x" })
        .Workflow {
            placeCursor "Second" 6 35 "let foo x = ModuleFirst.``foo bar`` x" ["``foo bar``"]
            findAllReferences (expectToFind [
                "FileFirst.fs", 6, 4, 15
                "FileSecond.fs", 6, 12, 35
            ])
        }

[<Fact>]
let ``We find operators`` () =
    SyntheticProject.Create(
        { sourceFile "First" [] with ExtraSource = "let (++) x y = x - y" },
        { sourceFile "Second" [] with ExtraSource = """
open ModuleFirst
let foo x = x ++ 4""" })
        .Workflow {
            placeCursor "Second" 8 16 "let foo x = x ++ 4" ["++"]
            findAllReferences (expectToFind [
                "FileFirst.fs", 6, 5, 7
                "FileSecond.fs", 8, 14, 16
            ])
        }

/// https://github.com/dotnet/fsharp/issues/14057
/// Operators with '.' should be found correctly (not split on '.')
[<Fact>]
let ``We find operators with dot character`` () =
    SyntheticProject.Create(
        { sourceFile "First" [] with ExtraSource = "let (-.-) x y = x + y" },
        { sourceFile "Second" [] with ExtraSource = """
open ModuleFirst
let foo x = x -.- 4""" })
        .Workflow {
            placeCursor "Second" 8 17 "let foo x = x -.- 4" ["-.-"]
            findAllReferences (expectToFind [
                "FileFirst.fs", 6, 5, 8
                "FileSecond.fs", 8, 14, 17
            ])
        }

[<Theory>]
[<InlineData("First")>]
[<InlineData("Second")>]
let ``We find disposable constructors`` searchIn =
    let source1 = "type MyReader = System.IO.StreamReader"
    let source2 = """open ModuleFirst
let reader = MyReader "test.txt"
"""
    { SyntheticProject.Create(
        { sourceFile "First" [] with Source = source1 },
        { sourceFile "Second" [] with Source = source2 })
        with SkipInitialCheck = true }

        .Workflow {
            placeCursor searchIn "MyReader"
            findAllReferences (expectToFind [
                "FileFirst.fs", 2, 5, 13
                "FileSecond.fs", 3, 13, 21
            ])
        }


module Parameters =

    [<Fact>]
    let ``We find function parameter in signature file`` () =
        let source = """let f param = param + 1"""
        let signature = """val f: param:int -> int"""
        SyntheticProject.Create(
            { sourceFile "Source" [] with Source = source; SignatureFile = Custom signature })
            .Workflow {
                placeCursor "Source" "param"
                findAllReferences (expectToFind [
                    "FileSource.fsi", 2, 7, 12
                    "FileSource.fs", 2, 6, 11
                    "FileSource.fs", 2, 14, 19
                ])
            }

    [<Fact>]
    let ``We find method parameter in signature file`` () =
        SyntheticProject.Create(
            { sourceFile "Source" [] with
                Source = "type MyClass() = member this.Method(methodParam) = methodParam + 1"
                SignatureFile = AutoGenerated })
            .Workflow {
                // Some race condition probably triggered by auto-generating signatures makes this
                // flaky in CI compressed metadata builds. Clearing the cache before we start fixes it ¯\_(ツ)_/¯
                clearCache
                placeCursor "Source" "methodParam"
                findAllReferences (expectToFind [
                    "FileSource.fsi", 8, 17, 28
                    "FileSource.fs", 2, 36, 47
                    "FileSource.fs", 2, 51, 62
                ])
            }

    [<Fact>]
    let ``We only find the correct parameter`` () =
        let source = """
let myFunc1 param = param + 1
let myFunc2 param = param + 2
"""
        let signature = """
val myFunc1: param: int -> int
val myFunc2: param: int -> int
"""
        SyntheticProject.Create("TupleParameterTest",
            { sourceFile "Source" [] with
                ExtraSource = source
                SignatureFile = Custom signature })
            .Workflow {
                checkFile "Source" expectOk
                placeCursor "Source" 7 17 "let myFunc1 param = param + 1" ["param"]
                findAllReferences (expectToFind [
                    "FileSource.fsi", 3, 13, 18
                    "FileSource.fs", 7, 12, 17
                    "FileSource.fs", 7, 20, 25
                ])
            }

module Exceptions =
    let source1 = "exception MyException of string"
    let signature1 = "exception MyException of string"

    let source2 = """
open ModuleFirst
let foo x = raise (MyException "foo")
"""
    let project() = SyntheticProject.Create(
        { sourceFile "First" [] with ExtraSource = source1 },
        { sourceFile "Second" [] with ExtraSource = source2 })

    let projectWithSignature() = SyntheticProject.Create(
        { sourceFile "First" [] with
            ExtraSource = source1
            SignatureFile = Custom signature1 },
        { sourceFile "Second" [] with ExtraSource = source2 })

    [<Fact>]
    let ``We find exception from definition`` () =
        project().Workflow {
            placeCursor "First" 6 21 "exception MyException of string" ["MyException"]
            findAllReferences (expectToFind [
                "FileFirst.fs", 6, 10, 21
                "FileSecond.fs", 8, 19, 30
            ])
        }

    [<Fact>]
    let ``We find exception from usage`` () =
        project().Workflow {
            placeCursor "Second" 8 30 "raise (MyException \"foo\")" ["MyException"]
            findAllReferences (expectToFind [
                "FileFirst.fs", 6, 10, 21
                "FileSecond.fs", 8, 19, 30
            ])
        }

    [<Fact>]
    let ``We find exception from definition and signature`` () =
        projectWithSignature().Workflow {
            placeCursor "First" 6 21 "exception MyException of string" ["MyException"]
            findAllReferences (expectToFind [
                "FileFirst.fs", 6, 10, 21
                "FileFirst.fsi", 2, 10, 21
                "FileSecond.fs", 8, 19, 30
            ])
        }

    [<Fact>]
    let ``We find exception from usage and signature`` () =
        projectWithSignature().Workflow {
            placeCursor "Second" 8 30 "raise (MyException \"foo\")" ["MyException"]
            findAllReferences (expectToFind [
                "FileFirst.fs", 6, 10, 21
                "FileFirst.fsi", 2, 10, 21
                "FileSecond.fs", 8, 19, 30
            ])
        }

module Attributes =

    let project() = SyntheticProject.Create(
        { sourceFile "First" [] with ExtraSource = "type MyAttribute() = inherit System.Attribute()" },
        { sourceFile "Second" [] with ExtraSource = """
open ModuleFirst
[<My>]
let foo x = 4""" },
        { sourceFile "Third" [] with ExtraSource = """
open ModuleFirst
[<MyAttribute>]
let foo x = 5""" })

    [<Fact>]
    let ``We find attributes from definition`` () =
        project().Workflow {
            placeCursor "First" 6 16 "type MyAttribute() = inherit System.Attribute()" ["MyAttribute"]
            findAllReferences (expectToFind [
                "FileFirst.fs", 6, 5, 16
                "FileSecond.fs", 8, 2, 4
                "FileThird.fs", 8, 2, 13
            ])
        }

    [<Fact>]
    let ``We find attributes from usage`` () =
        project().Workflow {
            placeCursor "Second" 8 4 "[<My>]" ["My"]
            findAllReferences (expectToFind [
                "FileFirst.fs", 6, 5, 16
                "FileSecond.fs", 8, 2, 4
                "FileThird.fs", 8, 2, 13
            ])
        }

    [<Fact>]
    let ``We find attributes from usage with Attribute suffix`` () =
        project().Workflow {
            placeCursor "Third" 8 13 "[<MyAttribute>]" ["MyAttribute"]
            findAllReferences (expectToFind [
                "FileFirst.fs", 6, 5, 16
                "FileSecond.fs", 8, 2, 4
                "FileThird.fs", 8, 2, 13
            ])
        }

[<Fact>]
let ``We find values of a type that has been aliased`` () =

    let project = SyntheticProject.Create(
        { sourceFile "First" [] with
            ExtraSource = "type MyInt = int32\n" +
                          "let myNum = 7"
            SignatureFile = Custom ("type MyInt = int32\n" +
                                    "val myNum: MyInt") },
        { sourceFile "Second" [] with
            ExtraSource = "let goo x = ModuleFirst.myNum + x"})

    project.Workflow {
        placeCursor "First" "myNum"
        findAllReferences (expectToFind [
            "FileFirst.fs", 7, 4, 9
            "FileFirst.fsi", 3, 4, 9
            "FileSecond.fs", 6, 12, 29
        ])
    }

[<Fact>]
let ``We don't find type aliases for a type`` () =
    let source = """
type MyType =
    member _.foo = "boo"
    member x.this : mytype = x
and mytype = MyType
"""
    testFindRefsInSource source "MyType" ["test.fs", 2, 5, 11; "test.fs", 5, 13, 19]

/// https://github.com/dotnet/fsharp/issues/14396
[<Fact>]
let ``itemKeyStore disappearance`` () =

    let source = """
type MyType() = class end

let x = MyType()
"""
    SyntheticProject.Create(
        { sourceFile "Program" [] with
            SignatureFile = Custom ""
            Source = source } ).Workflow {

        placeCursor "Program" "MyType"

        findAllReferences (expectToFind [
            "FileProgram.fs", 3, 5, 11
            "FileProgram.fs", 5, 8, 14
        ])

        updateFile "Program" (fun f -> { f with Source = "\n" + f.Source })
        saveFile "Program"

        findAllReferences (expectToFind [
            "FileProgram.fs", 4, 5, 11
            "FileProgram.fs", 6, 8, 14
        ])
    }

[<Fact>]
let ``itemKeyStore disappearance with live buffers`` () =

    let source = """
type MyType() = class end

let x = MyType()
"""
    let project = SyntheticProject.Create(
        { sourceFile "Program" [] with
            SignatureFile = Custom ""
            Source = source } )

    ProjectWorkflowBuilder(project, useGetSource = true, useChangeNotifications = true) {

        placeCursor "Program" "MyType"

        findAllReferences (expectToFind [
            "FileProgram.fs", 3, 5, 11
            "FileProgram.fs", 5, 8, 14
        ])

        updateFile "Program" (fun f -> { f with Source = "\n" + f.Source })

        placeCursor "Program" "MyType"

        findAllReferences (expectToFind [
            "FileProgram.fs", 4, 5, 11
            "FileProgram.fs", 6, 8, 14
        ])
    }


module ActivePatterns =

    /// https://github.com/dotnet/fsharp/issues/14206
    [<Fact>]
    let ``Finding references to an active pattern case shouldn't find other cases`` () =
        let source = """
let (|Even|Odd|) v =
    if v % 2 = 0 then Even else Odd
match 2 with
| Even -> ()
| Odd -> ()
"""
        testFindRefsInSource source "Even" ["test.fs", 2, 6, 10; "test.fs", 3, 22, 26; "test.fs", 5, 2, 6]

    [<Fact>]
    let ``We don't find references to cases from other active patterns with the same name`` () =
        let source = """
module One =

    let (|Even|Odd|) v =
        if v % 2 = 0 then Even else Odd
    match 2 with
    | Even -> ()
    | Odd -> ()

module Two =

    let (|Even|Steven|) v =
        if v % 3 = 0 then Steven else Even
    match 2 with
    | Even -> ()
    | Steven -> ()
"""
        testFindRefsInSource source "Even" ["test.fs", 4, 10, 14; "test.fs", 5, 26, 30; "test.fs", 7, 6, 10]

    [<Fact>]
    let ``We don't find references to cases the same active pattern defined in a different file`` () =

        let source = """
let (|Even|Odd|) v =
    if v % 2 = 0 then Even else Odd
match 2 with
| Even -> ()
| Odd -> ()
"""
        SyntheticProject.Create(
            { sourceFile "First" [] with Source = source },
            { sourceFile "Second" [] with Source = source }
        ).Workflow {
            placeCursor "First" "Even"
            findAllReferences (expectToFind [
                "FileFirst.fs", 3, 6, 10
                "FileFirst.fs", 4, 22, 26
                "FileFirst.fs", 6, 2, 6
            ])
        }

    [<Fact>]
    let ``We find active patterns in other files when there are signature files`` () =

        SyntheticProject.Create(
            { sourceFile "First" [] with
                Source = "let (|Even|Odd|) v = if v % 2 = 0 then Even else Odd"
                SignatureFile = AutoGenerated },
            { sourceFile "Second" [] with
                Source = """
open ModuleFirst
match 2 with | Even -> () | Odd -> ()
""" }
        ).Workflow {
            placeCursor "Second" "Even"
            findAllReferences (expectToFind [
                "FileFirst.fs", 2, 6, 10
                "FileFirst.fs", 2, 39, 43
                "FileFirst.fsi", 4, 6, 10
                "FileSecond.fs", 4, 15, 19
            ])
        }

    /// Fix for bug: https://github.com/dotnet/fsharp/issues/14969
    [<Fact>]
    let ``We find active patterns in signature files`` () =
        SyntheticProject.Create(
            { sourceFile "First" [] with
                Source = "let (|Even|Odd|) v = if v % 2 = 0 then Even else Odd"
                SignatureFile = AutoGenerated }
        ).Workflow {
            placeCursor "First" "Even"
            findAllReferences (expectToFind [
                "FileFirst.fs", 2, 6, 10
                "FileFirst.fs", 2, 39, 43
                "FileFirst.fsi", 4, 6, 10
            ])
        }

    /// Fix for bug: https://github.com/dotnet/fsharp/issues/19173
    /// Ensures active pattern cases are correctly distinguished in signature files
    [<Fact>]
    let ``Active pattern cases are correctly distinguished in signature files`` () =
        SyntheticProject.Create(
            { sourceFile "First" [] with
                Source = "let (|Even|Odd|) v = if v % 2 = 0 then Even else Odd"
                SignatureFile = AutoGenerated }
        ).Workflow {
            // When looking for Odd, should not find Even
            placeCursor "First" "Odd"
            findAllReferences (expectToFind [
                "FileFirst.fs", 2, 11, 14   // Odd in definition
                "FileFirst.fs", 2, 49, 52   // Odd in body
                "FileFirst.fsi", 4, 11, 14  // Odd in signature
            ])
        }


module Interfaces =

    let project() =
        let source1 = """
type IInterface1 =
    abstract member Property1 : int
    abstract member Method1: unit -> int
    abstract member Method1: string -> int

type IInterface2 =
    abstract member Property2 : int
        """
        let source2 = """
open ModuleFirst
type internal SomeType() =

    interface IInterface1 with
        member _.Property1 = 42
        member _.Method1() = 43
        member _.Method1(foo) = 43

    interface IInterface2 with
        member this.Property2 =
            (this :> IInterface1).Property1
        """

        SyntheticProject.Create( 
            { sourceFile "First" [] with Source = source1 }, 
            { sourceFile "Second" [] with Source = source2 } )

    let property1Locations() = [
        "FileFirst.fs", 4, 20, 29
        "FileSecond.fs", 7, 17, 26
        "FileSecond.fs", 13, 12, 43 // Not sure why we get the whole range here, but it seems to work fine.
    ]

    let method1Locations() = [
        "FileFirst.fs", 5, 20, 27
        "FileSecond.fs", 8, 17, 24
    ]

    [<Fact>]
    let ``We find all references to interface properties`` () =
        project().Workflow {
            placeCursor "First" "Property1"
            findAllReferences (expectToFind <| property1Locations())
        }

    [<Fact>]
    let ``We find all references to interface properties starting from implementation`` () =
        project().Workflow {
            placeCursor "Second" "Property1"
            findAllReferences (expectToFind <| property1Locations())
        }

    [<Fact>]
    let ``We find all references to interface methods`` () =
        project().Workflow {
            placeCursor "First" "Method1"
            findAllReferences (expectToFind <| method1Locations())
        }

    [<Fact>]
    let ``We find all references to interface methods starting from implementation`` () =
        project().Workflow {
            placeCursor "Second" "Method1"
            findAllReferences (expectToFind <| method1Locations())
        }

[<Fact>]
let ``Module with the same name as type`` () =
    let source = """
module Foo

type MyType =
    static member Two = 1

let x = MyType.Two

module MyType = do () // <-- Extra module with the same name as the type

let y = MyType.Two
"""
    testFindRefsInSource source "MyType" ["test.fs", 4, 5, 11; "test.fs", 7, 8, 14; "test.fs", 11, 8, 14]

[<Fact>]
let ``Module with the same name as type part 2`` () =
    let source = """
module Foo

module MyType =

    let Three = 7

type MyType =
    static member Two = 1

let x = MyType.Two

let y = MyType.Three
"""
    testFindRefsInSource source "MyType" ["test.fs", 4, 7, 13; "test.fs", 13, 8, 14]

module Properties =

    /// Related to bug: https://github.com/dotnet/fsharp/issues/18270
    /// Documents compiler service behavior: returns property def, getter, setter, and usage references.
    /// VS layer filters out 'get'/'set' keywords using Tokenizer.tryFixupSpan.
    [<Fact>]
    let ``We find all references for property with get and set accessors`` () =
        let source = """
module Foo

type IterationState<'T> = {
    BackingField : bool ref
} with
    member this.MyProperty
        with get () = this.BackingField.Value
        and set v = this.BackingField.Value <- v

let test () =
    let state = { BackingField = ref false }
    state.MyProperty <- true
    state.MyProperty
"""
        // Compiler returns all refs including get/set; VS layer filters appropriately
        testFindRefsInSource source "MyProperty" [
            "test.fs", 7, 16, 26   // Definition
            "test.fs", 8, 13, 16   // Getter at 'get' keyword
            "test.fs", 9, 12, 15   // Setter at 'set' keyword
            "test.fs", 13, 4, 20   // Usage with qualifier
            "test.fs", 14, 4, 20   // Usage with qualifier
        ]

/// Test for single-line interface syntax (related to #15399)
module SingleLineInterfaceSyntax =

    /// Issue: https://github.com/dotnet/fsharp/issues/15399
    [<Fact>]
    let ``We find interface members with single-line interface syntax`` () =
        let source = """
module Foo

type IFoo = abstract member Bar : unit -> unit

type Foo() = interface IFoo with member __.Bar () = ()

let foo = Foo() :> IFoo
foo.Bar()
"""
        testFindRefsInSource source "Bar" [
            "test.fs", 4, 28, 31  // Abstract member definition
            "test.fs", 6, 43, 46  // Implementation
            "test.fs", 9, 0, 7    // Usage via foo.Bar()
        ]

    [<Fact>]
    let ``We find interface type references with single-line interface syntax`` () =
        let source = """
module Foo

type IFoo = abstract member Bar : unit -> unit

type Foo() = interface IFoo with member __.Bar () = ()

let foo = Foo() :> IFoo
"""
        testFindRefsInSource source "IFoo" [
            "test.fs", 4, 5, 9    // Type definition
            "test.fs", 6, 23, 27  // In implementation
            "test.fs", 8, 19, 23  // In cast
        ]

module LineDirectives =

    open System

    /// A variant of singleFileChecker that allows a custom filename
    /// to avoid test isolation issues with LineDirectives.store
    let singleFileCheckerWithName (fileName: string) source =
        let getSource _ fn =
            FSharpFileSnapshot(
              FileName = fn,
              Version = "1",
              GetSource = fun () -> source |> SourceTextNew.ofString |> Task.FromResult )
            |> async.Return

        let checker = FSharpChecker.Create(
            keepAllBackgroundSymbolUses = false,
            enableBackgroundItemKeyStoreAndSemanticClassification = true,
            enablePartialTypeChecking = true,
            captureIdentifiersWhenParsing = true,
            useTransparentCompiler = true)

        let options =
            let baseOptions, _ =
                checker.GetProjectOptionsFromScript(
                    fileName,
                    SourceText.ofString "",
                    assumeDotNetFramework = false
                )
                |> Async.RunSynchronously

            { baseOptions with
                ProjectFileName = "project"
                ProjectId = None
                SourceFiles = [|fileName|]
                IsIncompleteTypeCheckEnvironment = false
                UseScriptResolutionRules = false
                LoadTime = DateTime()
                UnresolvedReferences = None
                OriginalLoadReferences = []
                Stamp = None }

        let snapshot = FSharpProjectSnapshot.FromOptions(options, getSource) |> Async.RunSynchronously

        fileName, snapshot, checker

    /// https://github.com/dotnet/fsharp/issues/9928
    /// Find All References should work correctly with #line directives.
    /// When #line is used, the returned ranges should be the remapped ranges
    /// (the "fake" file name and line numbers from the directive).
    [<Fact>]
    let ``Find references works with #line directives`` () =
        let source = """
module Foo
#line 100 "generated.fs"
let Thing = 42

let use1 = Thing + 1
"""
        // Use a unique filename to avoid test isolation issues with LineDirectives.store
        let fileName, options, checker = singleFileCheckerWithName "lineDirectivesTest.fs" source

        let symbolUse = getSymbolUse fileName source "Thing" options checker |> Async.RunSynchronously

        checker.FindBackgroundReferencesInFile(fileName, options, symbolUse.Symbol)
        |> Async.RunSynchronously
        |> expectToFind [
            // Definition at #line 100 (original line 4)
            "generated.fs", 100, 4, 9
            // Use at #line 102 (original line 6)
            "generated.fs", 102, 11, 16
        ]

module OrPatternSymbolResolution =
    
    /// https://github.com/dotnet/fsharp/issues/5546
    /// In SynPat.Or patterns (e.g., | x | x), both bindings were incorrectly marked
    /// as Binding occurrences. The second (and subsequent) occurrences should be Use.
    [<Fact>]
    let ``Or pattern second binding is classified as Use not Binding`` () =
        SyntheticProject.Create(
            { sourceFile "OrPattern" [] with 
                ExtraSource = "let test input = match input with | x | x -> x" })
            .Workflow {        
                checkFile "OrPattern" (fun (typeCheckResult: FSharpCheckFileResults) ->
                    // Get all symbol uses for the variable 'x'
                    let allSymbols = typeCheckResult.GetAllUsesOfAllSymbolsInFile()
                    
                    // Find the uses of 'x' in the pattern
                    let xUses = 
                        allSymbols 
                        |> Seq.filter (fun su -> su.Symbol.DisplayName = "x")
                        |> Seq.sortBy (fun su -> su.Range.StartLine, su.Range.StartColumn)
                        |> Seq.toArray
                    
                    // Should have 3 occurrences: first binding (Def), second binding (Use), and usage in body (Use)
                    Assert.True(xUses.Length >= 2, $"Expected at least 2 uses of 'x', got {xUses.Length}")
                    
                    // First occurrence should be definition
                    Assert.True(xUses.[0].IsFromDefinition, "First 'x' in Or pattern should be a definition")
                    
                    // Second occurrence should be use, not definition (#5546)
                    Assert.True(xUses.[1].IsFromUse, "Second 'x' in Or pattern should be a use, not a definition"))
            }

module EventHandlerSyntheticSymbols =
    
    /// https://github.com/dotnet/fsharp/issues/4136
    /// Events with [<CLIEvent>] generate synthetic 'handler' values that should not
    /// appear in GetAllUsesOfAllSymbolsInFile results.
    [<Fact>]
    let ``Event handler synthetic symbols are filtered from references`` () =
        SyntheticProject.Create(
            { sourceFile "EventTest" [] with 
                ExtraSource = "open System\ntype MyClass() =\n    let event = new Event<EventHandler, EventArgs>()\n    [<CLIEvent>]\n    member this.SelectionChanged = event.Publish" })
            .Workflow {        
                checkFile "EventTest" (fun (typeCheckResult: FSharpCheckFileResults) ->
                    let allSymbols = typeCheckResult.GetAllUsesOfAllSymbolsInFile()
                    
                    // Check that no synthetic 'handler' values are exposed
                    let handlerUses = 
                        allSymbols 
                        |> Seq.filter (fun su -> su.Symbol.DisplayName = "handler")
                        |> Seq.toArray
                    
                    // The synthetic 'handler' argument should be filtered out
                    Assert.True(handlerUses.Length = 0, 
                        $"Expected no 'handler' symbols (synthetic event handler values should be filtered), got {handlerUses.Length}"))
            }

/// https://github.com/dotnet/fsharp/issues/15290
module RecordCopyAndUpdate =
    
    [<Fact>]
    let ``Find references of record type includes copy-and-update`` () =
        let source = """
type Model = { V: string; I: int }
let m = { V = ""; I = 0 }
let m1 = { m with V = "m" }

type R = { M: Model }
"""
        testFindAllRefs source "Model" (fun ranges ->
            expectLinesInclude [3; 5; 7] ranges
            expectMinRefs 3 ranges)
    
    [<Fact>]
    let ``Find references of record type includes copy-and-update with nested fields`` () =
        let source = """
type Inner = { X: int }
type Outer = { I: Inner }
let o = { I = { X = 1 } }
let o2 = { o with I.X = 2 }
"""
        testFindAllRefs source "Outer" (expectLinesInclude [4; 6])

/// https://github.com/dotnet/fsharp/issues/16621
module UnionCaseTesters =
    
    [<Fact>]
    let ``Find references of union case B includes IsB usage`` () =
        let source = """
type X = A | B

let c = A
let result = c.IsB
"""
        testFindAllRefsMin source "B" 2  // Definition + IsB usage
    
    [<Fact>]
    let ``Find references of union case A includes IsA usage`` () =
        let source = """
type MyUnion = CaseA | CaseB of int

let x = CaseA
let useA = x.IsCaseA
let useB = x.IsCaseB
"""
        testFindAllRefsMin source "CaseA" 3  // Definition, construction, IsCaseA

/// https://github.com/dotnet/fsharp/issues/14902
module AdditionalConstructors =
    
    [<Fact>]
    let ``Find references of type includes all constructor usages`` () =
        let source = """
type MyClass(x: int) =
    new() = MyClass(0)

let a = MyClass()
let b = MyClass(5)
"""
        SyntheticProject.Create({ sourceFile "Source" [] with Source = source })
            .Workflow {
                placeCursor "Source" 3 12 "type MyClass(x: int) =" ["MyClass"]
                findAllReferences (fun ranges ->
                    expectLinesInclude [3; 4; 6; 7] ranges)  // Type def + all constructor usages
            }

    [<Fact>]
    let ``Additional constructor definition has correct symbol information`` () =
        let source = """
type MyClass(x: int) =
    new() = MyClass(0)

let a = MyClass()
let b = MyClass(5)
"""
        SyntheticProject.Create({ sourceFile "Source" [] with Source = source })
            .Workflow {
                checkFile "Source" (fun (result: FSharpCheckFileResults) ->
                    let allUses = result.GetAllUsesOfAllSymbolsInFile()
                    let additionalCtorDef = 
                        allUses 
                        |> Seq.tryFind (fun su -> su.IsFromDefinition && su.Range.StartLine = 4 && su.Range.StartColumn = 4)
                    Assert.True(additionalCtorDef.IsSome, "Should find additional constructor at (4,4)")
                    match additionalCtorDef.Value.Symbol with
                    | :? FSharp.Compiler.Symbols.FSharpMemberOrFunctionOrValue as mfv ->
                        Assert.True(mfv.IsConstructor, "Symbol should be a constructor")
                    | _ -> Assert.Fail("Expected FSharpMemberOrFunctionOrValue"))
            }

module ExternalDllOptimization =
    
    /// Issue #10227: Optimize Find All References for external DLL symbols
    [<Fact>]
    let ``Find references to external DLL symbol works correctly`` () =
        let source = """
let myString = System.String.Empty
let len = myString.Length
let copied = System.String.Copy myString
"""
        SyntheticProject.Create({ sourceFile "Source" [] with Source = source })
            .Workflow {
                checkFile "Source" (fun (result: FSharpCheckFileResults) ->
                    let symbolUse = result.GetSymbolUseAtLocation(3, 28, "let myString = System.String.Empty", ["String"])
                    Assert.True(symbolUse.IsSome, "Should find System.String symbol")
                    let symbol = symbolUse.Value.Symbol
                    Assert.False(System.String.IsNullOrEmpty(symbol.Assembly.SimpleName), "Assembly should have a name")
                    let usesInFile = result.GetUsesOfSymbolInFile(symbol)
                    Assert.True(usesInFile.Length >= 2, $"Expected at least 2 uses, found {usesInFile.Length}"))
            }
    
    [<Fact>]
    let ``External symbol has assembly information`` () =
        let source = """
let list = System.Collections.Generic.List<int>()
list.Add(42)
let count = list.Count
"""
        SyntheticProject.Create({ sourceFile "Source" [] with Source = source })
            .Workflow {
                checkFile "Source" (fun (result: FSharpCheckFileResults) ->
                    let symbolUse = result.GetSymbolUseAtLocation(3, 42, "let list = System.Collections.Generic.List<int>()", ["List"])
                    Assert.True(symbolUse.IsSome, "Should find List<T> symbol")
                    let assembly = symbolUse.Value.Symbol.Assembly
                    Assert.True(assembly.SimpleName.StartsWith("System") || assembly.SimpleName = "mscorlib" || assembly.SimpleName = "netstandard",
                        $"Assembly should be a system assembly, got: {assembly.SimpleName}"))
            }

/// https://github.com/dotnet/fsharp/issues/16993
module CSharpExtensionMethods =

    [<Fact>]
    let ``Find references for C# extension method finds all usages`` () =
        let source = """
open System
open System.Linq

let numbers = [| 1; 2; 3; 4; 5 |]
let firstEven = numbers.FirstOrDefault(fun x -> x % 2 = 0)
let firstOdd = numbers.FirstOrDefault(fun x -> x % 2 = 1)
let count = numbers.Count()
"""
        SyntheticProject.Create({ sourceFile "Source" [] with Source = source })
            .Workflow {
                checkFile "Source" (fun (result: FSharpCheckFileResults) ->
                    let allSymbols = result.GetAllUsesOfAllSymbolsInFile()
                    let firstOrDefaultUses = allSymbols |> Seq.filter (fun su -> su.Symbol.DisplayName = "FirstOrDefault") |> Seq.toArray
                    Assert.True(firstOrDefaultUses.Length >= 2, $"Expected at least 2 uses of FirstOrDefault, found {firstOrDefaultUses.Length}")
                    for su in firstOrDefaultUses do
                        match su.Symbol with
                        | :? FSharp.Compiler.Symbols.FSharpMemberOrFunctionOrValue as mfv ->
                            Assert.True(mfv.IsExtensionMember, "FirstOrDefault should be an extension member")
                        | _ -> ()
                    if firstOrDefaultUses.Length > 0 then
                        let usesInFile = result.GetUsesOfSymbolInFile(firstOrDefaultUses.[0].Symbol)
                        Assert.True(usesInFile.Length >= 2, $"Expected at least 2 uses in file, found {usesInFile.Length}"))
            }

    [<Fact>]
    let ``Extension method has correct symbol information`` () =
        let source = """
open System
open System.Linq

let arr = [| "a"; "b"; "c" |]
let first = arr.First()
"""
        SyntheticProject.Create({ sourceFile "Source" [] with Source = source })
            .Workflow {
                checkFile "Source" (fun (result: FSharpCheckFileResults) ->
                    let firstUses = 
                        result.GetAllUsesOfAllSymbolsInFile()
                        |> Seq.filter (fun su -> su.Symbol.DisplayName = "First")
                        |> Seq.toArray
                    Assert.True(firstUses.Length >= 1, $"Expected at least 1 use of First, found {firstUses.Length}")
                    for su in firstUses do
                        match su.Symbol with
                        | :? FSharp.Compiler.Symbols.FSharpMemberOrFunctionOrValue as mfv ->
                            Assert.True(mfv.IsExtensionMember, "First should be an extension member")
                        | _ -> ())
            }

/// https://github.com/dotnet/fsharp/issues/5545
module SAFEBookstoreSymbols =
    
    [<Fact>]
    let ``Find references of DU type inside module finds all usages in same file`` () =
        let source = """
type WishlistMsg = 
    | AddItem of string
    | RemoveItem of int

let update (msg: WishlistMsg) state =
    match msg with
    | AddItem item -> item :: state
    | RemoveItem idx -> state

let handleMsg (m: WishlistMsg) = 
    match m with
    | WishlistMsg.AddItem _ -> "adding"
    | WishlistMsg.RemoveItem _ -> "removing"
"""
        SyntheticProject.Create({ sourceFile "Source" [] with ExtraSource = source })
            .Workflow {
                placeCursor "Source" 7 5 "type WishlistMsg = " ["WishlistMsg"]
                findAllReferences (fun ranges ->
                    expectLinesInclude [7; 11] ranges  // Type def + at least one usage
                    expectMinRefs 3 ranges)
            }
    
    [<Fact>]
    let ``Find references of DU type in database pattern`` () =
        let source = """
type DatabaseType = 
    | SQLite
    | PostgreSQL
    | MSSQL

let getConnection (dbType: DatabaseType) =
    match dbType with
    | SQLite -> "sqlite://..."
    | PostgreSQL -> "postgresql://..."
    | MSSQL -> "mssql://..."

let defaultDb = SQLite
let altDb : DatabaseType = PostgreSQL
"""
        SyntheticProject.Create({ sourceFile "Source" [] with ExtraSource = source })
            .Workflow {
                placeCursor "Source" 7 5 "type DatabaseType = " ["DatabaseType"]
                findAllReferences (fun ranges -> expectLinesInclude [7; 12; 19] ranges)
            }