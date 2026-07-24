namespace EmittedIL

open System
open System.IO
open System.Reflection
open System.Reflection.Metadata
open System.Reflection.PortableExecutable
open Xunit

open FSharp.Test.Compiler

module CompilerGeneratedNameDeterminismTests =

    let private source =
        """
module GeneratedNameDeterminismSample

open System.Threading.Tasks

let makeAdder x =
    let inner y = x + y
    inner

let asyncValue () =
    async {
        let! value = async { return 1 }
        return value + 1
    }

let taskValue () =
    task {
        let! value = Task.FromResult 1
        return value + 1
    }

type Builder() =
    member _.Bind(x, f) = f x
    member _.Return(x) = x

let builder = Builder()

let computed value =
    builder {
        let! x = value
        return x + 1
    }
"""

    let private getOutputPath = function
        | CompilationResult.Success success ->
            match success.OutputPath with
            | Some path -> path
            | None -> failwith "Compilation did not produce an output path."
        | CompilationResult.Failure failure ->
            failwithf "Compilation was expected to succeed, but failed with: %A" failure.Diagnostics

    let private compileLibrary outputDirectory =
        FSharp source
        |> withOutputDirectory (Some(DirectoryInfo outputDirectory))
        |> withOptions [ "--debug:portable"; "--deterministic"; "--optimize-" ]
        |> asLibrary
        |> compile
        |> shouldSucceed
        |> getOutputPath

    let private typeName (reader: MetadataReader) (handle: TypeDefinitionHandle) =
        let rec buildName (handle: TypeDefinitionHandle) =
            let typeDef = reader.GetTypeDefinition handle
            let name = reader.GetString typeDef.Name

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
                        reader.GetString typeDef.Namespace

                if String.IsNullOrEmpty namespaceName then
                    name
                else
                    $"{namespaceName}.{name}"

        buildName handle

    let private emittedGeneratedNames assemblyPath =
        use stream = File.OpenRead assemblyPath
        use peReader = new PEReader(stream)
        let reader = peReader.GetMetadataReader()

        let names =
            [ for typeHandle in reader.TypeDefinitions do
                  yield typeName reader typeHandle

                  let typeDef = reader.GetTypeDefinition typeHandle

                  for methodHandle in typeDef.GetMethods() do
                      let methodDef = reader.GetMethodDefinition methodHandle
                      yield reader.GetString methodDef.Name ]

        names
        |> List.filter (fun name -> name.IndexOf('@') >= 0)
        |> List.sort

    [<Fact>]
    let ``normal compilation emits identical generated names across two compiles`` () =
        let tempRoot =
            Path.Combine(Path.GetTempPath(), "fsharp-generated-name-determinism-" + Guid.NewGuid().ToString("N"))

        try
            let firstOutput = Path.Combine(tempRoot, "first")
            let secondOutput = Path.Combine(tempRoot, "second")

            let firstNames = compileLibrary firstOutput |> emittedGeneratedNames
            let secondNames = compileLibrary secondOutput |> emittedGeneratedNames

            Assert.True(not firstNames.IsEmpty, "Expected at least one compiler-generated name in emitted metadata.")
            Assert.DoesNotContain(firstNames, fun name -> name.Contains("@hotreload"))
            Assert.Equal<string list>(firstNames, secondNames)
        finally
            if Directory.Exists tempRoot then
                Directory.Delete(tempRoot, true)
