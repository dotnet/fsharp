module FSharp.Compiler.ComponentTests.TypeChecks.TyparNameTests

open FSharp.Compiler.Symbols
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

[<Fact>]
let ``The call site of a generic function should have no influence on the name of the type parameters`` () =
    let definitionFile =
        Fs
            """
module A

let someGenericFunction _ = ()
"""
        |> withFileName "A.fs"

    let usageFile =
        ("""
module B

let otherGenericFunction _ _ _ =
    A.someGenericFunction 1
"""
         |> FsSource)
            .WithFileName("B.fs")

    let getGenericParametersNamesFor
        (entityDisplayName: string)
        (valueDisplayName: string)
        (additionalFile: SourceCodeFileKind)
        : string array =
        let typeCheckResult =
            definitionFile |> withAdditionalSourceFile additionalFile |> typecheckProject

        typeCheckResult.AssemblySignature.Entities
        |> Seq.tryPick (fun (entity: FSharpEntity) ->
            if entity.DisplayName <> entityDisplayName then
                None
            else
                entity.MembersFunctionsAndValues
                |> Seq.tryFind (fun mfv -> mfv.DisplayName = valueDisplayName)
                |> Option.map (fun (mfv: FSharpMemberOrFunctionOrValue) ->
                    mfv.GenericParameters |> Seq.map (fun gp -> gp.DisplayName) |> Seq.toArray))
        |> Option.defaultValue Array.empty

    let namesForB = getGenericParametersNamesFor "A" "someGenericFunction" usageFile

    let alternativeUsageFile =
        ("""
module C

let alternateGenericFunction _ =
    A.someGenericFunction 1
"""
         |> FsSource)
            .WithFileName("C.fs")

    let namesForC =
        getGenericParametersNamesFor "A" "someGenericFunction" alternativeUsageFile

    Assert.Equal<string array>(namesForB, namesForC)
