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

