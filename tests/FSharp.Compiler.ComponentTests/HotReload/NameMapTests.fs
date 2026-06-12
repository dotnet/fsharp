namespace FSharp.Compiler.ComponentTests.HotReload

open System
open System.IO
open System.Reflection
open System.Reflection.Metadata
open System.Reflection.PortableExecutable
open Xunit

open FSharp.Test
open FSharp.Test.Compiler

[<Collection(nameof NotThreadSafeResourceCollection)>]
module NameMapTests =

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

    let private getTypeNames compilationResult =
        let assemblyPath = getOutputPath compilationResult

        use stream = File.OpenRead assemblyPath
        use peReader = new PEReader(stream)
        let reader = peReader.GetMetadataReader()

        let rec buildName (handle: TypeDefinitionHandle) : string =
            let typeDef = reader.GetTypeDefinition(handle)
            let name = reader.GetString(typeDef.Name)

            let visibility = typeDef.Attributes &&& TypeAttributes.VisibilityMask

            let isNested =
                match visibility with
                | TypeAttributes.NestedPublic
                | TypeAttributes.NestedPrivate
                | TypeAttributes.NestedFamily
                | TypeAttributes.NestedAssembly
                | TypeAttributes.NestedFamORAssem
                | TypeAttributes.NestedFamANDAssem -> true
                | _ -> false

            if isNested then
                let declaringTypeHandle = typeDef.GetDeclaringType()
                $"{buildName declaringTypeHandle}+{name}"
            else
                let namespaceName =
                    if typeDef.Namespace.IsNil then
                        ""
                    else
                        reader.GetString(typeDef.Namespace)

                if String.IsNullOrEmpty namespaceName then
                    name
                else
                    $"{namespaceName}.{name}"

        [ for handle in reader.TypeDefinitions do
              yield buildName handle ]

    let private getMethodNames compilationResult =
        let assemblyPath = getOutputPath compilationResult

        use stream = File.OpenRead assemblyPath
        use peReader = new PEReader(stream)
        let reader = peReader.GetMetadataReader()

        [ for typeHandle in reader.TypeDefinitions do
              let typeDef = reader.GetTypeDefinition(typeHandle)
              for methodHandle in typeDef.GetMethods() do
                  let methodDef = reader.GetMethodDefinition(methodHandle)
                  yield reader.GetString(methodDef.Name) ]

    let private assertNoLineNumberSuffix (names: string list) =
        let offenders =
            names
            |> List.filter (fun name ->
                let idx = name.IndexOf('@')
                idx >= 0
                && idx + 1 < name.Length
                && Char.IsDigit name[idx + 1])

        let message =
            "Expected no compiler-generated names with line-number suffixes, but found: "
            + String.Join(", ", (offenders |> List.toArray))

        Assert.True(offenders.IsEmpty, message)

    [<Fact>]
    let ``closure types reuse stable name map`` () =
        let source =
            """
module HotReloadClosureSample

let makeAdder x =
    let inner y = x + y
    inner

let result = makeAdder 3 4
"""

        let compilation = compileHotReloadLibrary source
        let names : string list = getTypeNames compilation

        // Occurrence-derived baseline naming keys closure classes by the
        // ENCLOSING MEMBER's compiled name + occurrence chain
        // (makeAdder@hotreload#g0_o0), matching the allocator's fresh-name base used
        // for delta compiles; members the derivation fails closed on keep the replay
        // names (inner@hotreload, lambda@hotreload, ...).
        let closureNames =
            names
            |> List.filter (fun name ->
                name.Contains("lambda@") || name.Contains("inner@") || name.Contains("makeAdder@"))

        Assert.True(not (List.isEmpty closureNames), "Expected at least one closure type to be generated.")
        Assert.True(closureNames |> List.exists (fun name -> name.Contains("@hotreload")), "Expected at least one closure using @hotreload naming.")
        assertNoLineNumberSuffix names

    [<Fact>]
    let ``async state machine types reuse stable name map`` () =
        let source =
            """
module HotReloadAsyncSample

let fetchAsync () =
    async {
        let! value = async { return 1 }
        return value + 1
    }
"""

        let compilation = compileHotReloadLibrary source
        let names = getTypeNames compilation

        let asyncNames : string list = names |> List.filter (fun name -> name.Contains("Async") && name.Contains("@"))
        Assert.True(not (List.isEmpty asyncNames), "Expected async workflow to synthesize helper types.")
        Assert.True(asyncNames |> List.exists (fun name -> name.Contains("@hotreload")), "Expected async-generated types to use @hotreload naming.")
        assertNoLineNumberSuffix names

    [<Fact>]
    let ``computation expression helpers reuse stable name map`` () =
        let source =
            """
module HotReloadComputationExpressionSample

type Builder() =
    member _.Bind(x, f) = f x
    member _.Return(x) = x

let computation = Builder()

let run value =
    computation {
        let! x = value
        return x + 1
    }
"""

        let compilation = compileHotReloadLibrary source
        let names = getTypeNames compilation

        let computationNames : string list = names |> List.filter (fun name -> name.Contains("@"))
        Assert.True(not (List.isEmpty computationNames), "Expected computation expression to synthesize helper types.")
        Assert.True(computationNames |> List.exists (fun name -> name.Contains("@hotreload")), "Expected computation expression helpers to use @hotreload naming.")
        assertNoLineNumberSuffix names

    [<Fact>]
    let ``record helpers reuse stable name map`` () =
        let source =
            """
module HotReloadRecordSample

type Record = { X: int; Y: int }

let update record =
    { record with X = record.X + 1 }
"""

        let compilation = compileHotReloadLibrary source
        let typeNames = getTypeNames compilation
        let methodNames = getMethodNames compilation
        let helperNames = (typeNames @ methodNames) |> List.filter (fun name -> name.Contains("@"))

        assertNoLineNumberSuffix helperNames
        if not (List.isEmpty helperNames) then
            Assert.True(helperNames |> List.exists (fun name -> name.Contains("@hotreload")), "Expected record helpers to use @hotreload naming when compiler-generated names are present.")

    [<Fact>]
    let ``union helpers reuse stable name map`` () =
        let source =
            """
module HotReloadUnionSample

type Union =
    | Case of int
    | Case2 of string

let transform value =
    match value with
    | Case x -> Case2 (string x)
    | Case2 s -> Case (int s)
"""

        let compilation = compileHotReloadLibrary source
        let typeNames = getTypeNames compilation
        let methodNames = getMethodNames compilation
        let helperNames = (typeNames @ methodNames) |> List.filter (fun name -> name.Contains("@"))

        Assert.True(not (List.isEmpty helperNames), "Expected union helpers to generate compiler-named members.")
        assertNoLineNumberSuffix helperNames
        // Since the EraseUnions refactor (dotnet/fsharp#19518) union debug proxies are named
        // '<Alt>@DebugTypeProxy' — deterministic by construction, so they no longer flow through
        // the @hotreload name map. Stability across edits is what matters: accept either form.
        Assert.True(
            helperNames |> List.forall (fun name -> name.Contains("@hotreload") || name.Contains("@DebugTypeProxy")),
            sprintf "Expected union helpers to use stable (@hotreload or @DebugTypeProxy) naming, got: %A" helperNames)

    [<Fact>]
    let ``task builder helpers reuse stable name map`` () =
        let source =
            """
module HotReloadTaskSample

open System.Threading.Tasks
open Microsoft.FSharp.Control

let runTask () =
    task {
        let! value = Task.FromResult 1
        return value + 1
    }
"""

        let compilation = compileHotReloadLibrary source
        let typeNames = getTypeNames compilation
        let methodNames = getMethodNames compilation
        let helperNames = (typeNames @ methodNames) |> List.filter (fun name -> name.Contains("@"))

        Assert.True(not (List.isEmpty helperNames), "Expected task builder helpers to generate compiler-named members.")
        assertNoLineNumberSuffix helperNames
        Assert.True(helperNames |> List.exists (fun name -> name.Contains("@hotreload")), "Expected task builder helpers to use @hotreload naming.")
