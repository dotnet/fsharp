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
    /// name comes from a closure capture, which is codegen-sensitive: the previous method-group form
    /// (`tcImports.SystemRuntimeContainsType`) captured its receiver under a synthesized, optimization-dependent
    /// name (`objectArg` under `--optimize-`), so the contract held only in some configurations. The explicit
    /// lambda now used in CompilerImports.fs captures the local under its own name `tcImports` in every
    /// configuration.
    ///
    /// This test reflects over whichever build of FSharp.Compiler.Service it is run against. To stay meaningful
    /// for the optimized FCS that the SDK actually consumes as well as for the Debug build, it asserts both
    /// halves of the contract: a 'tcImports' field is present, and the regression symptom (a synthesized
    /// 'objectArg' receiver capture on this closure) is absent. The fix has been verified to satisfy both in
    /// Debug and Release builds.
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

        // The method-group regression symptom: the receiver captured under a synthesized 'objectArg' name
        // instead of 'tcImports'. Asserting its absence makes the guard catch a regression in the configuration
        // where the name is codegen-sensitive, not only where the SDK happens to still find 'tcImports'.
        let capturesSynthesizedReceiver =
            fieldsByClosure |> List.exists (fun (_, fields) -> List.contains "objectArg" fields)

        Assert.False(
            capturesSynthesizedReceiver,
            $"A 'systemRuntimeContainsType' closure captures its receiver under the synthesized name 'objectArg', which breaks the TypeProvider SDK reflection contract. Found: %A{fieldsByClosure}")

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

    /// Interning a provided-type entity clears the memoized lookup tables on the shared ModuleOrNamespaceType.
    /// Under graph-based parallel checking other threads read those tables (e.g. during name resolution) while
    /// interning is in progress. Without serialization a reader can recompute a table from a stale 'entities'
    /// snapshot and store it after a concurrent intern cleared the cache, dropping just-linked entities and
    /// resurfacing the spurious type errors this PR targets. Stress the read/compute/store against concurrent
    /// interns and assert the derived table stays consistent with the entity set.
    [<Fact>]
    let ``Interning provided entities keeps the lookup caches coherent under concurrent reads`` () =
        let cpath = CompPath(ILScopeRef.Local, SyntaxAccess.Unknown, [])

        let makeEntity (name: string) =
            Construct.NewModuleOrNamespace
                (Some cpath)
                taccessPublic
                (ident (name, Range.range0))
                XmlDoc.Empty
                []
                (MaybeLazy.Strict(Construct.NewEmptyModuleOrNamespaceType(Namespace true)))

        // Kept deliberately modest: enough writers to bump the version many times with readers interleaved,
        // but bounded thread counts and cooperative (yielding) readers so the guard does not peg CPU or pile
        // up allocations on memory-constrained CI agents.
        for _ in 1..5 do
            let mtyp = Construct.NewEmptyModuleOrNamespaceType(Namespace true)
            let names = [ for i in 1..40 -> $"Provided{i}" ]

            let writerCount = names.Length
            let readerCount = 6
            use barrier = new System.Threading.Barrier(writerCount + readerCount)
            let stop = ref 0

            let writers =
                [ for name in names ->
                      System.Threading.Thread(fun () ->
                          barrier.SignalAndWait()
                          mtyp.GetOrInternProvidedEntity(name, fun () -> makeEntity name) |> ignore) ]

            // Readers poll the version-stamped lookup tables while interning is in flight, yielding between
            // passes so they interleave with the writers without spinning hot. This still forces the
            // recompute/store races that a flag-gated lock would lose during the first concurrent append.
            let readers =
                [ for _ in 1..readerCount ->
                      System.Threading.Thread(fun () ->
                          barrier.SignalAndWait()
                          while System.Threading.Volatile.Read(&stop.contents) = 0 do
                              mtyp.AllEntitiesByCompiledAndLogicalMangledNames |> ignore
                              mtyp.TypesByMangledName |> ignore
                              mtyp.TypesByAccessNames |> ignore
                              mtyp.TypesByDemangledNameAndArity |> ignore
                              System.Threading.Thread.Yield() |> ignore) ]

            readers |> List.iter (fun t -> t.Start())
            writers |> List.iter (fun t -> t.Start())
            writers |> List.iter (fun t -> t.Join())
            System.Threading.Volatile.Write(&stop.contents, 1)
            readers |> List.iter (fun t -> t.Join())

            Assert.Equal(names.Length, mtyp.AllEntities |> Seq.length)
            // Once interning has quiesced, the version-stamped view must agree with the entity set: every
            // interned name resolves. A poisoned memo from a racing read cannot survive, because the final
            // entities version no longer matches any table a reader cached mid-append.
            let table = mtyp.AllEntitiesByCompiledAndLogicalMangledNames
            for name in names do
                Assert.True(table.ContainsKey name, $"Lookup cache dropped interned entity '{name}'.")

    let private newNamedEntity (name: string) =
        Construct.NewModuleOrNamespace
            (Some(CompPath(ILScopeRef.Local, SyntaxAccess.Unknown, [])))
            taccessPublic
            (ident (name, Range.range0))
            XmlDoc.Empty
            []
            (MaybeLazy.Strict(Construct.NewEmptyModuleOrNamespaceType(Namespace true)))

    // #20020: interned provided namespaces must be unique per name, and types interned under a namespace built by racing threads stay reachable.
    [<Fact>]
    let ``GetOrInternNamespaceEntity yields one namespace per name and keeps interned types reachable`` () =
        let root = Construct.NewEmptyModuleOrNamespaceType(Namespace true)
        let namespaceNames = [| "NsA"; "NsB"; "NsC" |]
        let workerCount = 60
        use barrier = new System.Threading.Barrier(workerCount)

        let workers =
            [ for w in 0 .. workerCount - 1 ->
                  let ns = namespaceNames[w % namespaceNames.Length]
                  let typeName = $"Type{w}"

                  System.Threading.Thread(fun () ->
                      barrier.SignalAndWait()
                      let nsEntity = root.GetOrInternNamespaceEntity(ns, fun () -> newNamedEntity ns)

                      nsEntity.ModuleOrNamespaceType.GetOrInternProvidedEntity(typeName, fun () -> newNamedEntity typeName)
                      |> ignore) ]

        workers |> List.iter (fun t -> t.Start())
        workers |> List.iter (fun t -> t.Join())

        let namespaceEntities = root.AllEntities |> Seq.toList
        Assert.Equal(namespaceNames.Length, namespaceEntities.Length)

        Assert.Equal(
            namespaceNames.Length,
            namespaceEntities |> List.map (fun e -> e.LogicalName) |> List.distinct |> List.length)

        for i in 0 .. namespaceNames.Length - 1 do
            let ns = namespaceNames[i]
            let expectedTypes = [ for w in 0 .. workerCount - 1 do if w % namespaceNames.Length = i then $"Type{w}" ]
            let nsEntity = root.GetOrInternNamespaceEntity(ns, fun () -> failwith "namespace should already be interned")
            let table = nsEntity.ModuleOrNamespaceType.AllEntitiesByCompiledAndLogicalMangledNames

            for typeName in expectedTypes do
                Assert.True(
                    table.ContainsKey typeName,
                    $"Provided type '{typeName}' under concurrently-created namespace '{ns}' was stranded.")

            Assert.Equal(expectedTypes.Length, nsEntity.ModuleOrNamespaceType.AllEntities |> Seq.length)

    // #20020: AddModuleOrNamespaceByMutation and AddProvidedTypeEntity share the 'entities' field; concurrent appends must not drop any entity.
    [<Fact>]
    let ``Concurrent AddModuleOrNamespaceByMutation and AddProvidedTypeEntity never lose an append`` () =
        for _ in 1..5 do
            let mtyp = Construct.NewEmptyModuleOrNamespaceType(Namespace true)
            let appendCount = 80
            use barrier = new System.Threading.Barrier(appendCount)

            let threads =
                [ for i in 0 .. appendCount - 1 ->
                      let entity = newNamedEntity $"E{i}"

                      System.Threading.Thread(fun () ->
                          barrier.SignalAndWait()

                          if i % 2 = 0 then
                              mtyp.AddModuleOrNamespaceByMutation entity
                          else
                              mtyp.AddProvidedTypeEntity entity) ]

            threads |> List.iter (fun t -> t.Start())
            threads |> List.iter (fun t -> t.Join())

            Assert.Equal(appendCount, mtyp.AllEntities |> Seq.length)
