module FSharp.Compiler.ComponentTests.TypeChecks.TyparNameTests

open FSharp.Compiler.Symbols
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

let private getGenericParametersNamesFor
    (cUnit: CompilationUnit)
    (entityDisplayName: string)
    (valueDisplayName: string)
    (additionalFile: SourceCodeFileKind)
    : string array =
    let typeCheckResult =
        cUnit |> withAdditionalSourceFile additionalFile |> typecheckProject false

    assert (Array.isEmpty typeCheckResult.Diagnostics)

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

    let namesForB =
        getGenericParametersNamesFor definitionFile "A" "someGenericFunction" usageFile

    let alternativeUsageFile =
        ("""
module C

let alternateGenericFunction _ =
    A.someGenericFunction 1
"""
         |> FsSource)
            .WithFileName("C.fs")

    let namesForC =
        getGenericParametersNamesFor definitionFile "A" "someGenericFunction" alternativeUsageFile

    Assert.Equal<string array>(namesForB, namesForC)

[<Fact>]
let ``Fixed typar name in signature file is still respected`` () =
    let signatureFile =
        Fsi
            """
module A

val someGenericFunction: 'x -> unit
"""
        |> withFileName "A.fsi"

    let implementationFile =
        ("""
module A

let someGenericFunction _ = ()
"""
         |> FsSource)
            .WithFileName("A.fs")

    let names =
        getGenericParametersNamesFor signatureFile "A" "someGenericFunction" implementationFile

    Assert.Equal<string array>([| "x" |], names)

[<Fact>]
let ``Hash constraint typar in signature file gets pretty name`` () =
    let signatureFile =
        Fsi
            """
module A

val someGenericFunction: #exn list -> unit
"""
        |> withFileName "A.fsi"

    let implementationFile =
        ("""
module A

let someGenericFunction (_ : #exn list) = ()
"""
         |> FsSource)
            .WithFileName("A.fs")

    let names =
        getGenericParametersNamesFor signatureFile "A" "someGenericFunction" implementationFile

    Assert.Equal<string array>([| "a" |], names)

[<Fact>]
let ``Hash constraint with generic type parameter should have pretty name`` () =
    let signatureFile =
        Fsi
            """
module A

val array2D: rows: seq<#seq<'T>> -> 'T[,]
"""
        |> withFileName "A.fsi"

    let implementationFile =
        ("""
module A

let array2D (rows: seq<#seq<'T>>) : 'T[,] = failwith "todo"
"""
         |> FsSource)
            .WithFileName("A.fs")

    let names =
        getGenericParametersNamesFor signatureFile "A" "array2D" implementationFile

    Assert.Equal<string array>([| "a"; "T" |], names)
