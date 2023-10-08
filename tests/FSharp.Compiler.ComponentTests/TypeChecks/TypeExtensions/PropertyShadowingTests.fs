module FSharp.Compiler.ComponentTests.TypeChecks.TypeExtensions.PropertyShadowingTests
open Xunit
open FSharp.Test
open FSharp.Test.Compiler

let [<Literal>] folder = __SOURCE_DIRECTORY__ + "/PropertyShadowing"

[<Theory;
  Directory(
    folder
    , Includes=[|
        "ShadowWithExtensionMethod.fsx"
        "ShadowWithTypeExtension.fsx"
        "ShadowingAndStillOkWithChainedCalls.fsx"
        "LinqCount.fsx"
    |]
)>]
let ``can hide property`` compilation =
    compilation
    |> asFsx
    |> withOptions ["--langversion:preview"]
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
      |]
      , BaselineSuffix = ".support.added.later"
)>]
let ``cannot hide property v7.0 support added later`` compilation =
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
    |]
)>]
let ``cannot hide property`` compilation =
    compilation
    |> asFsx
    |> withOptions ["--langversion:preview"]
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
    |]
)>]
let ``cannot hide property v7.0`` compilation =
    compilation
    |> asFsx
    |> withOptions ["--langversion:7.0"]
    |> verifyBaselines
    |> compile
    |> shouldFail
