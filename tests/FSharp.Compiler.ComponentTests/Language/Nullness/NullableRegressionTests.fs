module Language.NullableRegressions

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

let withVersionAndCheckNulls (version,checknulls) cu =
    cu
    |> withLangVersion version
    |> withWarnOn 3261
    |> withOptions ["--warnaserror+"]
    |> if checknulls then withCheckNulls else id

    
[<Theory>]
[<InlineData("preview",true)>]
[<InlineData("preview",false)>]
[<InlineData("8.0",false)>]
let ``Micro compilation`` langVersion checknulls =

    FsFromPath (__SOURCE_DIRECTORY__ ++ "micro.fsi")
    |> withAdditionalSourceFile (SourceFromPath (__SOURCE_DIRECTORY__ ++ "micro.fs"))
    |> withLangVersion langVersion
    |> fun x -> 
        if checknulls then 
            x |> withCheckNulls |> withDefines ["CHECKNULLS"]
        else x
    |> compile
    |> shouldSucceed

[<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".nullness_disabled", Includes=[|"existing-positive.fs"|])>]
let ``Existing positive v8 disabled`` compilation =
    compilation
    |> withVersionAndCheckNulls ("8.0",false)   
    |> verifyBaseline

[<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".nullness_disabled", Includes=[|"existing-positive.fs"|])>]
let ``Existing positive vPreview disabled`` compilation =
    compilation
    |> withVersionAndCheckNulls ("preview",false)   
    |> verifyBaseline

[<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".checknulls_on", Includes=[|"existing-positive.fs"|])>]
let ``Existing positive vPreview enabled`` compilation =
    compilation
    |> withVersionAndCheckNulls ("preview",true)   
    |> verifyBaseline

[<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".nullness_disabled", Includes=[|"existing-negative.fs"|])>]
let ``Existing negative v8 disabled`` compilation =
    compilation
    |> withVersionAndCheckNulls ("8.0",false)   
    |> verifyBaseline

[<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".nullness_disabled", Includes=[|"existing-negative.fs"|])>]
let ``Existing negative vPreview disabled`` compilation =
    compilation
    |> withVersionAndCheckNulls ("preview",false)   
    |> verifyBaseline

[<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".checknulls_on", Includes=[|"existing-negative.fs"|])>]
let ``Existing negative vPreview enabled`` compilation =
    compilation
    |> withVersionAndCheckNulls ("preview",true)   
    |> verifyBaseline

[<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".nullness_disabled", Includes=[|"library-functions.fs"|])>]
let ``Library functions nullness disabled`` compilation =
    compilation
    |> withVersionAndCheckNulls ("preview",false)   
    |> verifyBaseline

[<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".checknulls_on", Includes=[|"library-functions.fs"|])>]
let ``Library functions nullness enabled`` compilation =
    compilation
    |> withVersionAndCheckNulls ("preview",true)   
    |> verifyBaseline

[<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".nullness_disabled", Includes=[|"using-nullness-syntax-positive.fs"|])>]
let ``With new nullness syntax nullness disabled`` compilation =
    compilation
    |> withVersionAndCheckNulls ("preview",false)   
    |> verifyBaseline

[<Theory; Directory(__SOURCE_DIRECTORY__, BaselineSuffix=".checknulls_on", Includes=[|"using-nullness-syntax-positive.fs"|])>]
let ``With new nullness syntax nullness enabled`` compilation =
    compilation
    |> withVersionAndCheckNulls ("preview",true)   
    |> verifyBaseline