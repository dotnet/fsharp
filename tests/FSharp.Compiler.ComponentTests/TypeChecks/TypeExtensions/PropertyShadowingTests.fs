module FSharp.Compiler.ComponentTests.TypeChecks.TypeExtensions.Shadowing
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

let [<Literal>] folder = __SOURCE_DIRECTORY__ + "/Shadowing"

[<Theory;
  Directory(
    folder
    , Includes=[|
        "ShadowWithExtensionMethod.fsx"
        "ShadowWithTypeExtension.fsx"
        "ShadowingAndStillOkWithChainedCalls.fsx"
        "LinqCount.fsx"
        "ShadowStaticProperty.fsx"
        "ShadowWithLastOpenedTypeExtensions.fsx"
    |]
)>]
let PropertyHiding compilation =
    compilation
    |> asFsx
    |> verifyBaselines
    |> compileAndRun
    |> shouldSucceed
    
[<Theory;
  Directory(
      folder
      , Includes = [|
        "ShadowWithExtensionMethod.fsx"
        "ShadowWithTypeExtension.fsx"
        "ShadowingAndStillOkWithChainedCalls.fsx"
        "LinqCount.fsx"
        "ShadowStaticProperty.fsx"
        "ShadowWithLastOpenedTypeExtensions.fsx"
      |]
      , BaselineSuffix = ".support.added.later"
)>]
let ``PropertyHiding v7.0`` compilation =
    compilation
    |> asFsx
    |> withOptions ["--langversion:7.0"]
    |> verifyBaselines
    |> compile
    |> shouldFail

[<Theory;
  Directory(
    folder
    , Includes=[|
        "E_CannotShadowIndexedPropertyWithExtensionMethod.fsx"
        "E_CannotShadowIndexedPropertyWithTypeExtension.fsx"
        "E_CannotShadowFunctionPropertyWithExtensionMethod.fsx"
        "E_CannotShadowFunctionPropertyWithTypeExtension.fsx"
        "E_NoChangeForEvent.fsx"
    |]
)>]
let ``PropertyHiding fails`` compilation =
    compilation
    |> asFsx
    |> verifyBaselines
    |> compile
    |> shouldFail
    

[<Theory;
  Directory(
    folder
    , Includes=[|
        "E_CannotShadowIndexedPropertyWithExtensionMethod.fsx"
        "E_CannotShadowIndexedPropertyWithTypeExtension.fsx"
        "E_CannotShadowFunctionPropertyWithExtensionMethod.fsx"
        "E_CannotShadowFunctionPropertyWithTypeExtension.fsx"
        "E_NoChangeForEvent.fsx"
    |]
)>]
let ``PropertyHidingFails v7.0`` compilation =
    compilation
    |> asFsx
    |> withOptions ["--langversion:7.0"]
    |> verifyBaselines
    |> compile
    |> shouldFail
