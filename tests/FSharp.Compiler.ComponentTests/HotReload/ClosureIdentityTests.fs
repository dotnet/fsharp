namespace FSharp.Compiler.ComponentTests.HotReload

open System
open System.IO
open System.Reflection.Metadata
open System.Reflection.PortableExecutable
open Xunit

open FSharp.Test
open FSharp.Test.Compiler

/// Phase C3: closure identity must be preserved across delta generations. These tests
/// pin the metadata-level invariant the delta path relies on today (and that the C4
/// added-lambda emitter will extend): for pure body edits, every flag-on recompile of
/// the same lambda set produces closure classes with EXACTLY the same names, so a
/// delta can update the existing closure method bodies in place. Added/removed
/// lambdas are handled by the occurrence-keyed allocator (ClosureNameAllocatorTests
/// drives it over real C1 extraction); wiring the allocator into IlxGen lowering is
/// the pending C3 step documented in docs/hot-reload-closure-mapping.md.
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
