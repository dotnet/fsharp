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
        "ShadowStaticProperty.fsx"
        "ShadowWithLastOpenedTypeExtensions.fsx"
    |]
    , BaselineSuffix = ".realInternalSignatureOff"
)>]
let ``can hide property - realInternalSignatureOff`` compilation =
    compilation
    |> asFsx
    |> withOptions ["--langversion:preview"]
    |> withRealInternalSignatureOff
    |> verifyBaselines
    |> compileAndRun
    |> shouldSucceed

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
    , BaselineSuffix = ".realInternalSignatureOn"
)>]
let ``can hide property - realInternalSignatureOn`` compilation =
    compilation
    |> asFsx
    |> withOptions ["--langversion:preview"]
    |> withRealInternalSignatureOn
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
        "E_NoChangeForEvent.fsx"
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
        "E_NoChangeForEvent.fsx"
    |]
)>]
let ``cannot hide property v7.0`` compilation =
    compilation
    |> asFsx
    |> withOptions ["--langversion:7.0"]
    |> verifyBaselines
    |> compile
    |> shouldFail
