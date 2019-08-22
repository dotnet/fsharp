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

    // TODO: Unfortunately, we have to do this. F# requires a primary assembly. We should probably do the work to not have a primary assembly so we never have to do this.
    let defaultArgs = ["--targetprofile:netcore_private"]

    let createSource name amount =
        (sprintf """namespace Test.%s""" name) + createTestModules name amount

    let workspace = new AdhocWorkspace ()
    let temporaryStorage = workspace.Services.TemporaryStorage

    let getMetadataReferences () =
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
                // This is indeed, quite hacky, but it works for now.
                if String.Equals (Path.GetExtension filePath, ".dll", StringComparison.OrdinalIgnoreCase) && 
                   ((Path.GetFileNameWithoutExtension filePath).StartsWith("System.Runtime.Numerics") || 
                    (Path.GetFileNameWithoutExtension filePath).StartsWith("System.Net.Requests") || 
                    (Path.GetFileNameWithoutExtension filePath).StartsWith("System.Net.WebClient") ||
                    currentReferencedAssemblies.Contains filePath) then
                    Some (PortableExecutableReference.CreateFromFile filePath)
                else
                    None
            )
            |> Seq.map (fun peReference -> FSharpMetadataReference.PortableExecutable peReference)
            |> ImmutableArray.CreateRange

        let fsharpCoreMetadataReference =
            PortableExecutableReference.CreateFromFile typeof<int list>.Assembly.Location
            |> FSharpMetadataReference.PortableExecutable

        metadataReferences.Add fsharpCoreMetadataReference

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

        let c = FSharpCompilation.Create ("""C:\test.dll""", """C:\""", sourceSnapshots, getMetadataReferences(), defaultArgs)
        c.GetSemanticModel "test1.fs"

    let createScriptAux (text: string) =
        FSharpCompilation.CreateScript ("""C:\test.dll""", """C:\""", FSharpSourceSnapshot.FromText ("C:\\test1.fsx", SourceText.From text), getMetadataReferences(), defaultArgs)

    let semanticModelScript text =
        let c = createScriptAux text
        c.GetSemanticModel("C:\\test1.fsx")

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
module TestModuleCompilationTest

type CompiltationTest<'T> () =

                member val X = 1

                member val Y = 2

                member val Z = 3
                    
let testFunction (x: CompilationTest) =
    x.X + x.Y + x.Z"""

        let semanticModel = getSemanticModel (SourceText.From textString)

        let position = textString.IndexOf("""CompiltationTest<'T> ()""")
        let symbol = semanticModel.TryGetEnclosingSymbol (position, CancellationToken.None)
        Assert.True (symbol.IsSome)

        let diagnostics = semanticModel.SyntaxTree.GetDiagnostics ()
        Assert.True (diagnostics.IsEmpty)

    [<Test>]
    member __.``Find Symbol - Basic - Speculative`` () =
        let textString = """
module TestModuleCompilationTest =

    type CompiltationTest<'T> () =

                    member val X = 1

                    member val Y = 2

                    member val Z = 3
                    
    let testFunction (x: CompilationTest<'T>) =
        x.X + x.Y + x.Z"""

        let semanticModel = getSemanticModel (SourceText.From textString)

        let position = textString.IndexOf("""x.X + x.Y + x.Z""")
        let token = (semanticModel.SyntaxTree.GetRootNode ()).FindToken position

        Assert.False (token.IsNone)
        Assert.True (token.IsIdentifier)

        let node = token.GetParentNode ()
        let symbol = semanticModel.TryGetEnclosingSymbol (position, CancellationToken.None)
        let speculativeSymbolInfo = semanticModel.GetSpeculativeSymbolInfo (position, node, CancellationToken.None)

        Assert.True (symbol.IsSome)
        Assert.True (speculativeSymbolInfo.Symbol.IsSome)

    [<Test>]
    member __.``Get Completion Symbols - Open Declaration`` () =
        let semanticModel = 
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
        let semanticModel = getSemanticModel (SourceText.From textString)
        let syntaxTree = semanticModel.SyntaxTree
        let rootNode = semanticModel.SyntaxTree.GetRootNode ()

        let position = textString.IndexOf("Test")
        let token = rootNode.FindToken position

        Assert.False (token.IsNone)
        Assert.True (token.IsIdentifier)
        Assert.AreEqual ("Test", token.Value.Value)

        let token2 = rootNode.FindToken (position + 4)

        Assert.True token2.IsIdentifier
        Assert.AreEqual ("Test", token2.Value.Value)

        //

        let position = textString.IndexOf("Class1")
        let token3 = rootNode.FindToken position

        Assert.True token3.IsIdentifier
        Assert.AreEqual ("Class1", token3.Value.Value)

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
        let semanticModel = getSemanticModel (SourceText.From textString)

        let text = "hello"
        let position = textString.IndexOf(text)
        let syntaxTree = semanticModel.SyntaxTree

        let rootNode = syntaxTree.GetRootNode ()
        let token = rootNode.FindToken position

        Assert.IsTrue token.IsString

    [<Test>]
    member __.``Script Test - Simple`` () =
        let text = """
let x = 1 + 1
        """

        let c = semanticModelScript text
        let diags = c.GetDiagnostics()
        Assert.True (diags.IsEmpty, sprintf "%A" diags)

    [<Test>]
    member __.``Script Test - Reference Script`` () =

        let tmpPath = Path.GetTempFileName()
        let tmpFsx = Path.ChangeExtension(tmpPath, ".fsx")
        try
            let refText = """
module RefScript

type FromAnotherScript () = class end
"""
            File.WriteAllText(tmpFsx, refText)
            let text = sprintf """
#load @"%s"

open RefScript

let x = FromAnotherScript ()
                               """ tmpFsx

            let c = semanticModelScript text
            let diags = c.Compilation.GetDiagnostics()
            Assert.True (diags.IsEmpty, sprintf "%A" diags)
        finally
            try File.Delete tmpPath with | _ -> ()
            try File.Delete tmpFsx with | _ -> ()

    [<Test>]
    member __.``Script Test - Simple Emit`` () =
        let text = """
let x = 1 + 1
        """

        let c = semanticModelScript text
        
        use peStream = new MemoryStream()
        match c.Compilation.Emit (peStream) with
        | Ok _ -> ()
        | Error diags -> Assert.Fail (sprintf "%A" diags)

    [<Test>]
    member __.``Script Test - Simple Evaluation`` () =

        use peStream = new MemoryStream()
        let sm = semanticModelScript "[<EntryPoint>] let main _ = 1 + 1"
        let c = sm.Compilation

        let res =
            match c.Emit (peStream) with
            | Result.Ok _ ->
                let asm = System.Reflection.Assembly.Load(peStream.ToArray())
                let res = asm.EntryPoint.Invoke(null, [|Array.zeroCreate<string> 0|])
                Result.Ok (res)
            | Result.Error diags ->
                Result.Error diags

        match res with
        | Ok (value) -> Assert.AreEqual (2, value)
        | Error diags -> Assert.Fail (sprintf "%A" diags)

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
