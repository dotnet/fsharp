namespace FSharp.Compiler.ComponentTests.HotReload

open System
open System.IO
open System.Reflection.Metadata
open System.Reflection.PortableExecutable
open Xunit

open FSharp.Test
open FSharp.Test.Compiler

/// Closure identity must be preserved across delta generations. These tests
/// pin the metadata-level invariant the delta path relies on today (and that the
/// added-lambda emitter extends): for pure body edits, every flag-on recompile of
/// the same lambda set produces closure classes with EXACTLY the same names, so a
/// delta can update the existing closure method bodies in place. Added/removed
/// lambdas are handled by the occurrence-keyed allocator (ClosureNameAllocatorTests
/// drives it over real occurrence extraction); the allocator is wired into IlxGen
/// lowering as documented in docs/hot-reload-closure-mapping.md.
[<Collection(nameof NotThreadSafeResourceCollection)>]
module ClosureIdentityTests =

    let private compileHotReloadLibrary source =
        FSharp source
        |> withOptions [ "--langversion:preview"; "--debug+"; "--enable:hotreloaddeltas"; "--optimize-" ]
        |> asLibrary
        |> compile
        |> shouldSucceed

    let private getOutputPath = function
        | CompilationResult.Success s ->
            match s.OutputPath with
            | Some path -> path
            | None -> failwith "Compilation did not produce an output path."
        | CompilationResult.Failure f ->
            failwithf "Compilation was expected to succeed, but failed with: %A" f.Diagnostics

    /// All closure-class type names in the compiled assembly (the @hotreload-managed
    /// compiler-generated nested types), qualified by their declaring type chain.
    let private getHotReloadTypeNames compilationResult =
        let assemblyPath = getOutputPath compilationResult

        use stream = File.OpenRead assemblyPath
        use peReader = new PEReader(stream)
        let reader = peReader.GetMetadataReader()

        let rec buildName (handle: TypeDefinitionHandle) : string =
            let typeDef = reader.GetTypeDefinition(handle)
            let name = reader.GetString(typeDef.Name)

            if typeDef.IsNested then
                $"{buildName (typeDef.GetDeclaringType())}+{name}"
            else
                name

        [ for handle in reader.TypeDefinitions do
              let fullName = buildName handle

              if fullName.Contains("@hotreload", StringComparison.Ordinal) then
                  yield fullName ]
        |> List.sort

    let private genSource filterBody mapBody =
        $"""
module ClosureIdentitySample

let transform (input: int list) =
    input
    |> List.filter (fun x -> {filterBody})
    |> List.map (fun x -> {mapBody})
"""

    [<Fact>]
    let ``baseline capture records occurrence-chain to closure-name tables on the session`` () =
        try
            // Two top-level lambdas in one member: the baseline capture must join the
            // IlxGen stamp -> name recording with the occurrence extraction and store
            // chain -> name tables, keyed by MethodDef token, on the session baseline.
            let compilation =
                genSource "x > 0" "x * 2 + List.length input"
                |> compileHotReloadLibrary

            let emittedNames =
                compilation
                |> getHotReloadTypeNames
                |> List.map (fun fullName ->
                    match fullName.LastIndexOf('+') with
                    | -1 -> fullName
                    | i -> fullName.Substring(i + 1))
                |> Set.ofList

            let session =
                match global.FSharp.Compiler.HotReload.FSharpEditAndContinueLanguageService.Instance.TryGetSession() with
                | ValueSome session -> session
                | ValueNone -> failwith "Expected the flag-on compile to start a hot reload session."

            let closureNameTables = session.Baseline.EncClosureNames
            Assert.False(Map.isEmpty closureNameTables, "Baseline EncClosureNames must be populated by the capture compile.")

            // 'transform' contains exactly two top-level occurrences with chains [0] and [1].
            let transformTable =
                closureNameTables
                |> Map.toList
                |> List.map snd
                |> List.tryFind (fun table -> table |> Map.exists (fun _ name -> name.StartsWith("transform@", StringComparison.Ordinal)))
                |> Option.defaultWith (fun () -> failwith $"No closure-name table found for 'transform' in %A{closureNameTables}.")

            Assert.Equal<Set<int list>>(Set.ofList [ [ 0 ]; [ 1 ] ], transformTable |> Map.toSeq |> Seq.map fst |> Set.ofSeq)

            // Every recorded name is a closure class the compile actually emitted.
            for KeyValue(_, name) in transformTable do
                Assert.Contains(name, emittedNames)
        finally
            global.FSharp.Compiler.HotReload.FSharpEditAndContinueLanguageService.Instance.ResetSessionState()

    [<Fact>]
    let ``closure class names are identical across body-edit delta generations`` () =
        try
            // Generation 1 (baseline): a member with two lambdas — a capture-free
            // filter and a map closing over 'input'.
            let gen1Names =
                genSource "x > 0" "x * 2 + List.length input"
                |> compileHotReloadLibrary
                |> getHotReloadTypeNames

            Assert.True(
                gen1Names.Length >= 2,
                $"Expected at least two @hotreload closure classes in the baseline, got: %A{gen1Names}"
            )

            // Generation 2: both lambda bodies edited, lambda set unchanged.
            let gen2Names =
                genSource "x > 1" "x * 3 + List.length input"
                |> compileHotReloadLibrary
                |> getHotReloadTypeNames

            Assert.Equal<string list>(gen1Names, gen2Names)

            // Generation 3: edited again — identity must hold across the chain,
            // not just for a single regeneration.
            let gen3Names =
                genSource "x >= 2" "x * 5 + List.length input"
                |> compileHotReloadLibrary
                |> getHotReloadTypeNames

            Assert.Equal<string list>(gen1Names, gen3Names)
        finally
            // The flag-on compiles above replace the process-global hot reload
            // session; clear it so later tests in the collection start clean.
            global.FSharp.Compiler.HotReload.FSharpEditAndContinueLanguageService.Instance.ResetSessionState()

    let private sessionClosureNameTables () =
        match global.FSharp.Compiler.HotReload.FSharpEditAndContinueLanguageService.Instance.TryGetSession() with
        | ValueSome session -> session.Baseline.EncClosureNames
        | ValueNone -> failwith "Expected an active hot reload session."

    let private allTableNames tables =
        tables
        |> Map.toList
        |> List.collect (fun (_, table: Map<int list, string>) -> table |> Map.toList |> List.map snd)
        |> Set.ofList

    [<Fact>]
    let ``occurrence-keyed naming keeps closure names byte-identical across three body-edit generations`` () =
        // Allocator wiring: generations 2 and 3 compile with an active session whose baseline
        // carries occurrence-chain -> name tables, so the IlxGen closure call site takes
        // the allocator path (stamp-keyed Reused names), not bare sequence replay. For a
        // pure body-edit chain the emitted names AND the chained tables must stay
        // byte-identical in every generation.
        try
            let gen1Names =
                genSource "x > 0" "x * 2 + List.length input"
                |> compileHotReloadLibrary
                |> getHotReloadTypeNames

            let gen1Tables = sessionClosureNameTables ()
            Assert.False(Map.isEmpty gen1Tables, "Generation 1 must capture closure-name tables.")

            let gen2Names =
                genSource "x > 1" "x * 3 + List.length input"
                |> compileHotReloadLibrary
                |> getHotReloadTypeNames

            Assert.Equal<string list>(gen1Names, gen2Names)
            // The recaptured tables carry the same names forward (Reused verbatim).
            Assert.Equal<Set<string>>(allTableNames gen1Tables, allTableNames (sessionClosureNameTables ()))

            let gen3Names =
                genSource "x >= 2" "x * 5 + List.length input"
                |> compileHotReloadLibrary
                |> getHotReloadTypeNames

            Assert.Equal<string list>(gen1Names, gen3Names)
            Assert.Equal<Set<string>>(allTableNames gen1Tables, allTableNames (sessionClosureNameTables ()))
        finally
            global.FSharp.Compiler.HotReload.FSharpEditAndContinueLanguageService.Instance.ResetSessionState()

    [<Fact>]
    let ``occurrence-keyed naming survives an added lambda: survivors keep baseline names, the added one is generation-suffixed`` () =
        // The added lambda is inserted BEFORE the surviving ones, which is exactly the
        // case pure sequence replay cannot handle (every later same-basename allocation
        // would shift by one). With the allocator path the surviving closures keep their
        // baseline class names verbatim and the added occurrence gets the
        // {base}@hotreload#g{N}_o{i} generation-suffixed name. Emission of the added
        // member in a DELTA is still rejected by classification (LambdaShapeChange) —
        // emission is the delta emitter's job; this pins the naming contract it builds on.
        try
            let gen1Names =
                genSource "x > 0" "x * 2 + List.length input"
                |> compileHotReloadLibrary
                |> getHotReloadTypeNames

            Assert.True(gen1Names.Length >= 2, $"Expected at least two baseline closures, got: %A{gen1Names}")

            // Generation 2 source: a capture-free map lambda is ADDED in front of the
            // surviving filter and map lambdas; the survivors' bodies are unchanged.
            let gen2Names =
                $"""
module ClosureIdentitySample

let transform (input: int list) =
    input
    |> List.map (fun x -> x + 1)
    |> List.filter (fun x -> x > 0)
    |> List.map (fun x -> x * 2 + List.length input)
"""
                |> compileHotReloadLibrary
                |> getHotReloadTypeNames

            // Every baseline closure class name survives byte-identically.
            for name in gen1Names do
                Assert.Contains(name, gen2Names)

            // Exactly one new closure class, carrying the generation-suffixed name
            // (session generation 1 — the first compile after the captured baseline).
            let addedNames = gen2Names |> List.filter (fun name -> not (List.contains name gen1Names))

            let addedName = Assert.Single addedNames
            Assert.Contains("transform@hotreload#g1_o", addedName)

            // The recaptured session tables chain all three names forward, so a further
            // generation would reuse the added closure's name too.
            let chainedNames = allTableNames (sessionClosureNameTables ())

            for name in gen2Names do
                let simpleName =
                    match name.LastIndexOf('+') with
                    | -1 -> name
                    | i -> name.Substring(i + 1)

                Assert.Contains(simpleName, chainedNames)
        finally
            global.FSharp.Compiler.HotReload.FSharpEditAndContinueLanguageService.Instance.ResetSessionState()
