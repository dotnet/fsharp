// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.Compiler.Service.Tests

open Xunit
open FSharp.Test
open FSharp.Compiler.Syntax
open System.Reflection
open Internal.Utilities.Library.Extras
open FSharp.Compiler.AbstractIL.IL
open FSharp.Compiler.SyntaxTreeOps
open FSharp.Compiler.Text
open FSharp.Compiler.Xml
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics

type ManglingNamesOfProvidedTypesWithSingleParameter() = 
    
    [<Fact>]
    member this.MangleWithNonDefaultValue() = 
        let mangled = 
            PrettyNaming.ComputeMangledNameWithoutDefaultArgValues("MyNamespace.Test", [| "xyz" |], [| "Foo", Some "abc" |])
        Assert.shouldBe "MyNamespace.Test,Foo=\"xyz\"" mangled
    
    [<Fact>]
    member this.MangleWithDefaultValue() = 
        let mangled = 
            PrettyNaming.ComputeMangledNameWithoutDefaultArgValues("MyNamespace.Test", [| "xyz" |], [| "Foo", Some "xyz" |])
        Assert.shouldBe "MyNamespace.Test" mangled
    
    [<Fact>]
    member this.DemangleNonDefaultValue() = 
        let name, parameters = PrettyNaming.DemangleProvidedTypeName "MyNamespace.Test,Foo=\"xyz\""
        Assert.shouldBe "MyNamespace.Test" name
        Assert.shouldBeEquivalentTo [| "Foo", "xyz" |] parameters
    
    [<Fact>]
    member this.DemangleDefaultValue() = 
        let name, parameters = PrettyNaming.DemangleProvidedTypeName "MyNamespace.Test,"
        Assert.shouldBe "MyNamespace.Test" name
        Assert.shouldBeEmpty parameters

    [<Fact>]
    member this.DemangleNewDefaultValue() = 
        let name, parameters = PrettyNaming.DemangleProvidedTypeName "MyNamespace.Test"
        Assert.shouldBe "MyNamespace.Test" name
        Assert.shouldBeEmpty parameters


type ManglingNamesOfProvidedTypesWithMultipleParameter() = 
    
    [<Fact>]
    member this.MangleWithNonDefaultValue() = 
        let mangled = 
            PrettyNaming.ComputeMangledNameWithoutDefaultArgValues 
                ("MyNamespace.Test", [| "xyz"; "abc" |], 
                    [| "Foo", Some "foo"
                       "Foo2", Some "foo2" |])
        Assert.shouldBe "MyNamespace.Test,Foo=\"xyz\",Foo2=\"abc\"" mangled
    
    [<Fact>]
    member this.MangleWithDefaultValue() = 
        let mangled = 
            PrettyNaming.ComputeMangledNameWithoutDefaultArgValues 
                ("MyNamespace.Test", [| "xyz"; "abc" |], 
                    [| "Foo", Some "xyz"
                       "Foo2", Some "abc" |])
        Assert.shouldBe "MyNamespace.Test" mangled
    
    [<Fact>]
    member this.DemangleMultiParameter() = 
        let smashtogether arr = arr |> Seq.fold(fun acc (f,s) -> acc + $"-{f}-{s}") ""
        let name, parameters = PrettyNaming.DemangleProvidedTypeName "TestType,Foo=\"xyz\",Foo2=\"abc\""
        Assert.shouldBe "TestType" name
        let parameters = smashtogether parameters
        let expected = smashtogether [| "Foo", "xyz"; "Foo2", "abc" |]
        Assert.shouldBe expected parameters

/// Regression tests for the loose reflection contract between the compiler and the TypeProvider SDK,
/// and for concurrent linking of provided types under graph-based parallel checking.
module ProvidedTypeHostingTests =

    /// The TypeProvider SDK reflects over the value the compiler stores in
    /// TypeProviderConfig.systemRuntimeContainsType and requires a captured field literally named
    /// 'tcImports' (https://github.com/fsprojects/FSharp.TypeProviders.SDK ProvidedTypes.fs). That field
    /// name comes from a closure capture, which is codegen-sensitive: an unoptimized build once emitted it
    /// as 'objectArg'. Guard the contract here so it cannot silently break per configuration again.
    [<Fact>]
    let ``systemRuntimeContainsType closure exposes a field named tcImports`` () =
        let asm = typeof<FSharp.Compiler.CodeAnalysis.FSharpChecker>.Assembly

        let types =
            try
                asm.GetTypes()
            with :? ReflectionTypeLoadException as ex ->
                ex.Types |> Array.filter (isNull >> not)

        let flags = BindingFlags.Instance ||| BindingFlags.Public ||| BindingFlags.NonPublic

        let closures =
            types |> Array.filter (fun t -> not (isNull t) && t.Name.Contains "systemRuntimeContainsType")

        let fieldsByClosure =
            [ for t in closures -> t.Name, [ for f in t.GetFields flags -> f.Name ] ]

        let exposesTcImports =
            fieldsByClosure |> List.exists (fun (_, fields) -> List.contains "tcImports" fields)

        Assert.True(
            exposesTcImports,
            $"No 'systemRuntimeContainsType' closure exposes a 'tcImports' field. Found: %A{fieldsByClosure}")

    /// Under graph-based parallel checking the same provided type can be linked from several files at once.
    /// GetOrInternProvidedEntity must yield one canonical Entity (compared by object identity elsewhere) and
    /// run the side-effecting 'create' exactly once, regardless of how many threads race on the same name.
    [<Fact>]
    let ``GetOrInternProvidedEntity yields one entity and creates once under concurrency`` () =
        let mtyp = Construct.NewEmptyModuleOrNamespaceType(Namespace true)
        let cpath = CompPath(ILScopeRef.Local, SyntaxAccess.Unknown, [])
        let createCount = ref 0

        let create () =
            System.Threading.Interlocked.Increment createCount |> ignore

            Construct.NewModuleOrNamespace
                (Some cpath)
                taccessPublic
                (ident ("MyProvidedType", Range.range0))
                XmlDoc.Empty
                []
                (MaybeLazy.Strict(Construct.NewEmptyModuleOrNamespaceType(Namespace true)))

        let threadCount = 64
        use barrier = new System.Threading.Barrier(threadCount)
        let results = System.Collections.Concurrent.ConcurrentBag<Entity>()

        let threads =
            [ for _ in 1..threadCount ->
                  System.Threading.Thread(fun () ->
                      barrier.SignalAndWait()
                      results.Add(mtyp.GetOrInternProvidedEntity("MyProvidedType", create))) ]

        threads |> List.iter (fun t -> t.Start())
        threads |> List.iter (fun t -> t.Join())

        let entities = results.ToArray()
        let canonical = entities[0]
        Assert.All(entities, (fun e -> Assert.Same(canonical, e)))
        Assert.Equal(1, createCount.Value)
        Assert.Equal(1, mtyp.AllEntities |> Seq.length)
