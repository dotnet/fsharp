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
        let textString = """
module TestModuleCompilationTest =

    type CompiltationTest<'T> () =

                    member val X = 1

                    member val Y = 2

                    member val Z = 3
                    
    let testFunction (x: CompilationTest) =
        x.X + x.Y + x.Z"""

        let semanticModel, _ = getSemanticModel (SourceText.From textString)

        let position = textString.IndexOf("""CompiltationTest<'T> ()""")
        let symbol = semanticModel.TryGetEnclosingSymbol (position, CancellationToken.None)
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
    member __.``Syntax Tree - Find Token`` () =
        let textString = """
namespace Test
        
/// Doc comment
type Class1 (* inside comment *) () =
            
    // normal comment
    member val X = 1
        
    member val Y = 1
        
    member val Z = 1
        
"""         
        let semanticModel, _ = getSemanticModel (SourceText.From textString)

        let position = textString.IndexOf("Test")
        let syntaxTree = semanticModel.SyntaxTree

        let rootNode = syntaxTree.GetRootNode CancellationToken.None
        let token = (rootNode.TryFindToken position).Value
        Assert.True token.IsIdentifier
        Assert.AreEqual ("Test", token.TryGetText().Value)

        let token2 = (rootNode.TryFindToken (position + 4)).Value

        Assert.True token2.IsIdentifier
        Assert.AreEqual ("Test", token2.TryGetText().Value)

        //

        let position = textString.IndexOf("Class1")
        let token3 = (rootNode.TryFindToken position).Value

        Assert.True token3.IsIdentifier
        Assert.AreEqual ("Class1", token3.TryGetText().Value)

        let token4 = token3.TryGetNextToken().Value
        Assert.True token4.IsWhitespace

    [<Test>]
    member __.``Syntax Tree - Function App`` () =
        let textString = """
namespace Test
        
module App =

    let test (x: int) (y: int) = x + y

    let useTheTest () =
        test 
"""         
        let semanticModel, _ = getSemanticModel (SourceText.From textString)

        let text = "        test "
        let position = textString.IndexOf(text) + text.Length - 1
        let syntaxTree = semanticModel.SyntaxTree

        let rootNode = syntaxTree.GetRootNode CancellationToken.None
        let token = (rootNode.TryFindToken position).Value

        Assert.IsTrue token.IsWhitespace

    [<Test>]
    member __.``Syntax Tree - String Token`` () =
        let textString = """
namespace Test
        
module App =

    let x = "hello
there
    "

    let y = 1
"""         
        let semanticModel, _ = getSemanticModel (SourceText.From textString)

        let text = "hello"
        let position = textString.IndexOf(text)
        let syntaxTree = semanticModel.SyntaxTree

        let rootNode = syntaxTree.GetRootNode CancellationToken.None
        let token = (rootNode.TryFindToken position).Value

        Assert.IsTrue token.IsString

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
