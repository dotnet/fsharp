namespace FSharp.Compiler.Compilation.Tests

open System
open System.IO
open System.Collections.Immutable
open System.Collections.Generic
open System.Threading
open FSharp.Compiler.Compilation
open FSharp.Compiler.Compilation.Utilities
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open NUnit.Framework
open Microsoft.CodeAnalysis.MSBuild

[<AutoOpen>]
module Helpers =

    let createTestModules name amount =
        [
            for i = 1 to amount do
                yield
                    sprintf
                    """
module TestModule%i =

    type %s () =

                    member val X = 1

                    member val Y = 2

                    member val Z = 3
                    
    let testFunction (x: %s) =
                    x.X + x.Y + x.Z
                    """ i name name
        ]
        |> List.reduce (+)

    let createSource name amount =
        (sprintf """namespace Test.%s""" name) + createTestModules name amount

    let getSemanticModel (text: SourceText) =
        let sources =
            [
                ("test1.fs", text)
            ]
        let workspace = new AdhocWorkspace ()
        let temporaryStorage = workspace.Services.TemporaryStorage

        let sourceSnapshots =
            sources
            |> List.map (fun (filePath, sourceText) -> temporaryStorage.CreateFSharpSourceSnapshot (filePath, sourceText, CancellationToken.None))
            |> ImmutableArray.CreateRange

        let currentReferencedAssemblies =
            let asmLocations =
                AppDomain.CurrentDomain.GetAssemblies()
                |> Array.choose (fun asm -> 
                    if not asm.IsDynamic then
                        Some asm.Location
                    else
                        None
                )
            HashSet(asmLocations, StringComparer.OrdinalIgnoreCase)

        let metadataReferences =
            Directory.EnumerateFiles(Path.GetDirectoryName typeof<System.Object>.Assembly.Location)
            |> Seq.choose (fun filePath ->
                if String.Equals (Path.GetExtension filePath, ".dll", StringComparison.OrdinalIgnoreCase) && currentReferencedAssemblies.Contains filePath then
                    Some (PortableExecutableReference.CreateFromFile filePath)
                else
                    None
            )
            |> Seq.map (fun peReference -> FSharpMetadataReference.PortableExecutable peReference)
            |> ImmutableArray.CreateRange

        let fsharpCoreMetadataReference =
            PortableExecutableReference.CreateFromFile typeof<int list>.Assembly.Location
            |> FSharpMetadataReference.PortableExecutable

        let c = FSharpCompilation.Create ("""C:\test.dll""", """C:\""", sourceSnapshots, metadataReferences.Add fsharpCoreMetadataReference)
        c.GetSemanticModel "test1.fs", temporaryStorage

[<TestFixture>]
type CompilationTests () =

    [<Test>]
    member __.``Basic Check``() =
        let sources =
            [
                for i = 1 to 3 do
                    yield ("test" + i.ToString() + ".fs", SourceText.From (createSource "CompilationTest" 1))
            ]
        let workspace = new AdhocWorkspace ()
        let temporaryStorage = workspace.Services.TemporaryStorage

        let sourceSnapshots =
            sources
            |> List.map (fun (filePath, sourceText) -> temporaryStorage.CreateFSharpSourceSnapshot (filePath, sourceText, CancellationToken.None))
            |> ImmutableArray.CreateRange

        let c = FSharpCompilation.Create ("""C:\test.dll""", """C:\""", sourceSnapshots, ImmutableArray.Empty)

        c.GetSemanticModel "test3.fs" |> ignore
        Assert.Throws<Exception> (fun () -> c.GetSemanticModel "badfile.fs" |> ignore) |> ignore

    [<Test>]
    member __.``Find Symbol - Basic`` () =
        let semanticModel, _ = 
            getSemanticModel (SourceText.From """
module TestModuleCompilationTest =

    type CompiltationTest () =

                    member val X = 1

                    member val Y = 2

                    member val Z = 3
                    
    let testFunction (x: CompilationTest) =
        x.X + x.Y + x.Z""")

        let symbol = semanticModel.TryFindSymbolAsync (4, 9) |> Async.RunSynchronously
        Assert.True (symbol.IsSome)

    [<Test>]
    member __.``Get Completion Symbols - Open Declaration`` () =
        let semanticModel, _ = 
            getSemanticModel (SourceText.From """
module CompilationTest.Test
open System.Collections
open System. 

let beef = obj ()
let x = beef.
let yopac = 1""")

        let symbols = semanticModel.GetCompletionSymbolsAsync (4, 13) |> Async.RunSynchronously
        Assert.False (symbols.IsEmpty)
        let symbols = semanticModel.GetCompletionSymbolsAsync (7, 14) |> Async.RunSynchronously
        Assert.False (symbols.IsEmpty)

    [<Test>]
    member __.``Internal Token Changes - Additions`` () =
        let text =
            SourceText.From """
module TestModuleCompilationTest =

    type CompiltationTest () =

            member val X = 1

            member val Y = 2

            member val Z = 3"""

        let semanticModel, storageService = getSemanticModel text

        let textChange =
            TextChange(TextSpan(text.Length - 1, 0), """
    let testFunction (x: CompilationTest) =
        x.X + x.Y + x.Z
            """)

        let text2 = text.WithChanges ([textChange])

        let newTextSnapshot = storageService.CreateFSharpSourceSnapshot (semanticModel.SyntaxTree.FilePath, text2, CancellationToken.None)
        let newCompilation = semanticModel.Compilation.ReplaceSourceSnapshot newTextSnapshot
        let newSemanticModel = newCompilation.GetSemanticModel "test1.fs"
        let tokenChanges = newSemanticModel.SyntaxTree.GetIncrementalTokenChangesAsync () |> Async.RunSynchronously
        Assert.True (tokenChanges.Length > 0)

    [<Test>]
    member __.``Parse With Tokens`` () =
        let text =
            SourceText.From """
namespace Test

// This is a comment.

/// This is a doc comment.
type TestClass () =

        member val X = 1

        member val Y = 2

        member val Z = 3"""

        let semanticModel, storageService = getSemanticModel text

        let realInput, realErrors = semanticModel.SyntaxTree.GetParseResultAsync () |> Async.RunSynchronously
        let input, errors = semanticModel.SyntaxTree.TestParseWithTokens () |> Async.RunSynchronously
        Assert.True input.IsSome


[<TestFixture>]
type UtilitiesTest () =

    [<Test>]
    member __.``Lru Cache Validation`` () =
        let lru = LruCache<int, obj> (5, Collections.Generic.EqualityComparer.Default)

        Assert.Throws<ArgumentNullException> (fun () -> lru.Set (1, null)) |> ignore

        let o1 = obj ()
        lru.Set (1, o1)

        Assert.AreEqual (1, lru.Count)

        match lru.TryGetValue 1 with
        | ValueSome o -> Assert.AreSame (o1, o)
        | _ -> failwith "couldn't find object in Lru"

        lru.Set (1, obj ())

        match lru.TryGetValue 1 with
        | ValueSome o -> Assert.AreNotSame (o1, o)
        | _ -> failwith "couldn't find object in Lru"

        lru.Set (2, obj ())
        lru.Set (3, obj ())
        lru.Set (4, obj ())
        lru.Set (5, obj ())

        Assert.AreEqual (5, lru.Count)

        lru.Set (6, obj ())

        Assert.AreEqual (5, lru.Count)

        Assert.True ((lru.TryGetValue 1).IsNone)

        lru.TryGetValue 2 |> ignore

        lru.Set (7, obj ())

        // Because we tried to acess 2 before setting 7, it put 3 at the back.
        Assert.True ((lru.TryGetValue 3).IsNone)

        Assert.True ((lru.TryGetValue 2).IsSome)

        Assert.AreEqual (5, lru.Count)

        Assert.True ((lru.TryGetValue 1).IsNone)
        Assert.True ((lru.TryGetValue 2).IsSome)
        Assert.True ((lru.TryGetValue 3).IsNone)
        Assert.True ((lru.TryGetValue 4).IsSome)
        Assert.True ((lru.TryGetValue 5).IsSome)
        Assert.True ((lru.TryGetValue 6).IsSome)

    [<Test>]
    member __.``Mru Weak Cache Validation`` () =
        let mru = MruWeakCache<int, obj> (5, 10, Collections.Generic.EqualityComparer.Default)

        Assert.Throws<ArgumentNullException> (fun () -> mru.Set (1, null)) |> ignore

        let stackF () =
            let o1 = obj ()
            mru.Set (1, o1)

            Assert.AreEqual (1, mru.Count)
            Assert.AreEqual (0, mru.WeakReferenceCount)

            match mru.TryGetValue 1 with
            | ValueSome o -> Assert.AreSame (o1, o)
            | _ -> failwith "couldn't find object in mru"

            mru.Set (2, obj ())
            mru.Set (3, obj ())
            mru.Set (4, obj ())

            let o5 = obj ()
            mru.Set (5, o5)

            Assert.AreEqual (5, mru.Count)
            Assert.AreEqual (0, mru.WeakReferenceCount)

            let o6 = obj ()
            mru.Set (6, o6)

            Assert.AreEqual (5, mru.Count)
            Assert.AreEqual (1, mru.WeakReferenceCount)

            Assert.True ((mru.TryGetValue 5) = ValueSome o5)

            // trying to get 6, evicts 5 out of the cache and into the weak reference cache.
            Assert.True ((mru.TryGetValue 6) = ValueSome o6)

            Assert.AreEqual (5, mru.Count)
            Assert.AreEqual (1, mru.WeakReferenceCount)

        stackF ()
        GC.Collect ()

        Assert.AreEqual (5, mru.Count)
        Assert.AreEqual (1, mru.WeakReferenceCount)

        Assert.True ((mru.TryGetValue 5).IsNone)
        Assert.True ((mru.TryGetValue 6).IsSome)
        Assert.True ((mru.TryGetValue 4).IsSome)
        Assert.True ((mru.TryGetValue 3).IsSome)
        Assert.True ((mru.TryGetValue 2).IsSome)
        Assert.True ((mru.TryGetValue 1).IsSome)

        Assert.AreEqual (5, mru.Count)
        Assert.AreEqual (0, mru.WeakReferenceCount)
        ()
