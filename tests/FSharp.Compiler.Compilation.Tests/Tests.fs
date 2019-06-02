namespace FSharp.Compiler.Compilation.Tests

open System
open System.IO
open System.Collections.Immutable
open System.Collections.Generic
open FSharp.Compiler.Compilation
open FSharp.Compiler.Compilation.Utilities
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open NUnit.Framework
open Microsoft.CodeAnalysis.MSBuild

//open Microsoft.CodeAnalysis.MS

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
        let compilationService = CompilationService (CompilationServiceOptions.Create workspace)

        let sourceSnapshots =
            sources
            |> List.map (fun (filePath, sourceText) -> compilationService.CreateSourceSnapshot (filePath, sourceText))
            |> ImmutableArray.CreateRange

        let options = CompilationOptions.Create ("""C:\test.dll""", """C:\""", sourceSnapshots, ImmutableArray.Empty)
        let c = compilationService.CreateCompilation options

        c.GetSemanticModel "test3.fs" |> ignore
        Assert.Throws<Exception> (fun () -> c.GetSemanticModel "badfile.fs" |> ignore) |> ignore

    [<Test>]
    member __.``Find Symbol - Basic`` () =
        let sources =
            [
                for i = 1 to 3 do
                    yield ("test" + i.ToString() + ".fs", SourceText.From (createSource "CompilationTest" 1))
            ]
        let workspace = new AdhocWorkspace ()
        let compilationService = CompilationService (CompilationServiceOptions.Create workspace)

        let sourceSnapshots =
            sources
            |> List.map (fun (filePath, sourceText) -> compilationService.CreateSourceSnapshot (filePath, sourceText))
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

        let compilationReferences =
            Directory.EnumerateFiles(Path.GetDirectoryName typeof<System.Object>.Assembly.Location)
            |> Seq.choose (fun filePath ->
                if String.Equals (Path.GetExtension filePath, ".dll", StringComparison.OrdinalIgnoreCase) && currentReferencedAssemblies.Contains filePath then
                    Some (PortableExecutableReference.CreateFromFile filePath)
                else
                    None
            )
            |> Seq.map (fun peReference -> CompilationReference.PortableExecutable peReference)
            |> ImmutableArray.CreateRange

        let fsharpCoreCompilationReference =
            PortableExecutableReference.CreateFromFile typeof<int list>.Assembly.Location
            |> CompilationReference.PortableExecutable

        let options = CompilationOptions.Create ("""C:\test.dll""", """C:\""", sourceSnapshots, compilationReferences.Add fsharpCoreCompilationReference)
        let c = compilationService.CreateCompilation options
        let semanticModel = c.GetSemanticModel "test1.fs"
        let symbol = semanticModel.TryFindSymbolAsync (4, 9) |> Async.RunSynchronously
        Assert.True (symbol.IsSome)

    [<Test>]
    member __.``Get Completion Symbols - Open Declaration`` () =
        let sources =
            [
                ("test1.fs",
                    """
module CompilationTest.Test
open System.Collections
open System

let beef = 1
                    """ |> SourceText.From
                )
            ]
        let workspace = new AdhocWorkspace ()
        let compilationService = CompilationService (CompilationServiceOptions.Create workspace)

        let sourceSnapshots =
            sources
            |> List.map (fun (filePath, sourceText) -> compilationService.CreateSourceSnapshot (filePath, sourceText))
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

        let compilationReferences =
            Directory.EnumerateFiles(Path.GetDirectoryName typeof<System.Object>.Assembly.Location)
            |> Seq.choose (fun filePath ->
                if String.Equals (Path.GetExtension filePath, ".dll", StringComparison.OrdinalIgnoreCase) && currentReferencedAssemblies.Contains filePath then
                    Some (PortableExecutableReference.CreateFromFile filePath)
                else
                    None
            )
            |> Seq.map (fun peReference -> CompilationReference.PortableExecutable peReference)
            |> ImmutableArray.CreateRange

        let fsharpCoreCompilationReference =
            PortableExecutableReference.CreateFromFile typeof<int list>.Assembly.Location
            |> CompilationReference.PortableExecutable

        let options = CompilationOptions.Create ("""C:\test.dll""", """C:\""", sourceSnapshots, compilationReferences.Add fsharpCoreCompilationReference)
        let c = compilationService.CreateCompilation options
        let semanticModel = c.GetSemanticModel "test1.fs"
        let symbols = semanticModel.GetCompletionSymbolsAsync (4, 12) |> Async.RunSynchronously
        Assert.False (symbols.IsEmpty)

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
