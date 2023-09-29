// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module EmittedIL.NullnessMetadata

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

type Optimize = Optimize | DoNotOptimize

let verifyCompilation (o:Optimize) compilation =
    compilation
    |> withLangVersionPreview
    |> withOptions ["--checknulls"]
    |> (match o with | Optimize -> withOptimize | DoNotOptimize -> withNoOptimize)
    |> withNoDebug
    |> withNoInterfaceData
    |> withNoOptimizationData
    |> asLibrary        
    |> verifyILBaseline

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ModuleLevelBindings.fs"|])>]
let ``Nullable attr for module bindings`` compilation =  
    compilation
    |> verifyCompilation DoNotOptimize

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ModuleLevelFunctions.fs"|])>]
let ``Nullable attr for module functions`` compilation =  
    compilation
    |> verifyCompilation DoNotOptimize

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ModuleLevelFunctionsOpt.fs"|])>]
let ``Nullable attr for module functions optimize`` compilation =  
    compilation
    |> verifyCompilation Optimize

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"CurriedFunctions.fs"|])>]
let ``Nullable attr for curriedFunc optimize`` compilation =  
    compilation
    |> verifyCompilation Optimize

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AnonRecords.fs"|])>]
let ``Nullable attr for anon records`` compilation =  
    compilation
    |> verifyCompilation DoNotOptimize

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"Records.fs"|])>]
let ``Nullable attr for records`` compilation =  
    compilation
    |> verifyCompilation DoNotOptimize

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"ReferenceDU.fs"|])>]
let ``Nullable attr for ref DUs`` compilation =  
    compilation
    |> verifyCompilation DoNotOptimize

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"StructDU.fs"|])>]
let ``Nullable attr for struct DUs`` compilation =  
    compilation
    |> verifyCompilation DoNotOptimize

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"CustomType.fs"|])>]
let ``Nullable attr for custom type`` compilation =  
    compilation
    |> verifyCompilation DoNotOptimize

[<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NullAsTrueValue.fs"|])>]
let ``Nullable attr for Option clones`` compilation =  
    compilation
    |> verifyCompilation DoNotOptimize


module Interop  =
    open System.IO

    let FsharpFunctins = 
        Path.Combine(__SOURCE_DIRECTORY__,"ModuleLevelFunctions.fs")
        |> File.ReadAllText
        |> FSharp
        |> asLibrary
        |> withLangVersionPreview
        |> withName "MyTestModule"
        |> withOptions ["--checknulls"]

    [<Fact>]
    let ``Csharp code can work with annotated FSharp module`` () =
        Path.Combine(__SOURCE_DIRECTORY__,"CsharpConsumer.cs")
        |> File.ReadAllText
        |> CSharp
        |> withReferences [FsharpFunctins]
        |> withCSharpLanguageVersion CSharpLanguageVersion.Preview
        |> asLibrary
        |> withName "CsharpAppConsumingNullness"
        |> compile
        |> shouldFail
        |> withDiagnostics [
            Error 29, Line 31, Col 20, Line 31, Col 61, "Cannot implicitly convert type 'int' to 'string'"
            Warning 8625, Line 12, Col 74, Line 12, Col 78, "Cannot convert null literal to non-nullable reference type."
            Warning 8604, Line 14, Col 88, Line 14, Col 113, "Possible null reference argument for parameter 'x' in 'string MyTestModule.nonNullableInputOutputFunc(string x)'."
            Warning 8620, Line 19, Col 88, Line 19, Col 101, "Argument of type '(string?, string?, int, int, int, int)' cannot be used for parameter 'x' of type '(string, string, int, int, int, int)' in '(string, string, int, int, int, int) MyTestModule.genericValueTypeTest((string, string, int, int, int, int) x)' due to differences in the nullability of reference types."
            Warning 8620, Line 21, Col 78, Line 21, Col 109, "Argument of type '(string, string?, int, int, int, int)' cannot be used for parameter 'x' of type '(string, string, int, int, int, int)' in '(string, string, int, int, int, int) MyTestModule.genericValueTypeTest((string, string, int, int, int, int) x)' due to differences in the nullability of reference types."
            Warning 8604, Line 26, Col 60, Line 26, Col 70, "Possible null reference argument for parameter 'x_0' in 'Tuple<string, string, int, int, int, int> MyTestModule.genericRefTypeTest(string x_0, string? x_1, int x_2, int x_3, int x_4, int x_5)'."
            Warning 8625, Line 31, Col 51, Line 31, Col 55, "Cannot convert null literal to non-nullable reference type."]
