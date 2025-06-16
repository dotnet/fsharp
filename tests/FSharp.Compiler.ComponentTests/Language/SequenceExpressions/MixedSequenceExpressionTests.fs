// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

open FSharp.Test
open Xunit
open FSharp.Test.Compiler

module  MixedSequenceExpressionTests =
    
    let verifyCompile compilation =
        compilation
        |> asExe
        |> withOptions ["--nowarn:988"]
        |> compile

    let verifyCompileAndRun compilation =
        compilation
        |> asExe
        |> withOptions ["--nowarn:988"]
        |> compileAndRun
        
    [<Fact>]
    let ``Version 90: Mixed ranges and values require preview language version``() =
        FSharp """
module MixedRangeVersionTest
        
let a = seq { yield! seq { 1..10 }; 19 }
let b = [-3; yield! [1..10]]
let c = [|-3; yield! [|1..10|]; 19|]

let d = seq { 1..10; 19 }
let e = [-3; 1..10]
let f = [|-3; 1..10; 19|]
        """
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 8, Col 13, Line 8, Col 26, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 8, Col 15, Line 8, Col 20, "Incomplete expression or invalid use of indexer syntax")
            (Error 3350, Line 9, Col 9, Line 9, Col 20, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 9, Col 14, Line 9, Col 19, "Incomplete expression or invalid use of indexer syntax")
            (Error 3350, Line 10, Col 9, Line 10, Col 26, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 10, Col 15, Line 10, Col 20, "Incomplete expression or invalid use of indexer syntax")
        ]
        
    [<Fact>]
    let ``Preview: Mixed ranges and values require preview language version``() =
        FSharp """
module MixedRangeVersionTest

let a = seq { yield! seq { 1..10 }; 19 }
let b = [-3; yield! [1..10]]
let c = [|-3; yield! [|1..10|]; 19|]

let d = seq { 1..10; 19 }
let e = [-3; 1..10]
let f = [|-3; 1..10; 19|]
        """
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldSucceed
        
    // SOURCE=SequenceExpressions02.fs 	# SequenceExpressions02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions02.fs"|])>]
    let ``Version 9: SequenceExpressions02 fs`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 3, Col 13, Line 3, Col 24, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 3, Col 18, Line 3, Col 23, "Incomplete expression or invalid use of indexer syntax")
            (Error 3350, Line 7, Col 13, Line 7, Col 28, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 7, Col 18, Line 7, Col 23, "Incomplete expression or invalid use of indexer syntax")
            (Error 3350, Line 11, Col 13, Line 11, Col 29, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 11, Col 14, Line 11, Col 18, "Incomplete expression or invalid use of indexer syntax")
            (Error 751, Line 11, Col 20, Line 11, Col 24, "Incomplete expression or invalid use of indexer syntax")
            (Error 3350, Line 15, Col 13, Line 15, Col 30, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 15, Col 17, Line 15, Col 25, "Incomplete expression or invalid use of indexer syntax")
        ]

    // SOURCE=SequenceExpressions02.fs 	# SequenceExpressions02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions02.fs"|])>]
    let ``Preview: SequenceExpressions02 fs`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=SequenceExpressions03.fs 	# SequenceExpressions03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions03.fs"|])>]
    let ``Preview: SequenceExpressions03 fs`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed
        
    // SOURCE=SequenceExpressions03.fs 	# SequenceExpressions03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions03.fs"|])>]
    let ``Version 9: SequenceExpressions03 fs`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 3, Col 13, Line 3, Col 26, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 3, Col 19, Line 3, Col 24, "Incomplete expression or invalid use of indexer syntax")
            (Error 3350, Line 7, Col 13, Line 7, Col 30, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 7, Col 19, Line 7, Col 24, "Incomplete expression or invalid use of indexer syntax")
            (Error 3350, Line 11, Col 13, Line 11, Col 31, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 11, Col 15, Line 11, Col 19, "Incomplete expression or invalid use of indexer syntax")
            (Error 751, Line 11, Col 21, Line 11, Col 25, "Incomplete expression or invalid use of indexer syntax")
            (Error 3350, Line 15, Col 13, Line 15, Col 32, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 15, Col 18, Line 15, Col 26, "Incomplete expression or invalid use of indexer syntax")
            (Error 3350, Line 19, Col 13, Line 19, Col 28, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 19, Col 18, Line 19, Col 22, "Incomplete expression or invalid use of indexer syntax")
            (Error 3350, Line 23, Col 13, Line 23, Col 28, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 23, Col 18, Line 23, Col 22, "Incomplete expression or invalid use of indexer syntax")
        ]

    // SOURCE=SequenceExpressions04.fs 	# SequenceExpressions04.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions04.fs"|])>]
    let ``Preview: SequenceExpressions04 fs`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed
        
    // SOURCE=SequenceExpressions04.fs 	# SequenceExpressions04.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions04.fs"|])>]
    let ``Version 9: SequenceExpressions04 fs`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 3, Col 17, Line 3, Col 30, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 3, Col 19, Line 3, Col 24, "Incomplete expression or invalid use of indexer syntax")
            (Error 3350, Line 7, Col 17, Line 7, Col 37, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 7, Col 23, Line 7, Col 27, "Incomplete expression or invalid use of indexer syntax")
            (Error 751, Line 7, Col 29, Line 7, Col 35, "Incomplete expression or invalid use of indexer syntax")
            (Error 3350, Line 11, Col 17, Line 11, Col 36, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 11, Col 22, Line 11, Col 30, "Incomplete expression or invalid use of indexer syntax")
        ]

    // SOURCE=SequenceExpressions05.fs 	# SequenceExpressions05.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions05.fs"|])>]
    let ``Preview: SequenceExpressions05 fs`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed
        
    // SOURCE=SequenceExpressions05.fs 	# SequenceExpressions05.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions05.fs"|])>]
    let ``Version 9: SequenceExpressions05 fs`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 3, Col 17, Line 3, Col 30, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 3, Col 19, Line 3, Col 24, "Incomplete expression or invalid use of indexer syntax")
            (Error 3350, Line 7, Col 17, Line 7, Col 36, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 7, Col 22, Line 7, Col 30, "Incomplete expression or invalid use of indexer syntax")
        ]
        
    // SOURCE=SequenceExpressions06.fs 	# SequenceExpressions06.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions06.fs"|])>]
    let ``Preview: SequenceExpressions06 fs`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed
        
    // SOURCE=SequenceExpressions06.fs 	# SequenceExpressions06.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions06.fs"|])>]
    let ``Version 9: SequenceExpressions06 fs`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 3, Col 23, Line 3, Col 33, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 3, Col 24, Line 3, Col 28, "Incomplete expression or invalid use of indexer syntax")
            (Error 3350, Line 4, Col 24, Line 4, Col 36, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 4, Col 26, Line 4, Col 30, "Incomplete expression or invalid use of indexer syntax")
            (Error 3350, Line 5, Col 27, Line 5, Col 39, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 5, Col 29, Line 5, Col 33, "Incomplete expression or invalid use of indexer syntax")
            (Error 3350, Line 8, Col 5, Line 8, Col 27, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 8, Col 6, Line 8, Col 19, "Incomplete expression or invalid use of indexer syntax")
        ]
        
    // SOURCE=SequenceExpressions07.fs 	# SequenceExpressions07.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions07.fs"|])>]
    let ``Preview: SequenceExpressions07 fs`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed
        
        
    // SOURCE=SequenceExpressions07.fs 	# SequenceExpressions07.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions07.fs"|])>]
    let ``Version 9: SequenceExpressions07 fs`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 13, Col 25, Line 13, Col 30, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 13, Col 25, Line 13, Col 30, "Incomplete expression or invalid use of indexer syntax");
            (Error 3350, Line 17, Col 21, Line 17, Col 25, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 3350, Line 17, Col 27, Line 17, Col 31, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 17, Col 21, Line 17, Col 25, "Incomplete expression or invalid use of indexer syntax")
            (Error 751, Line 17, Col 27, Line 17, Col 31, "Incomplete expression or invalid use of indexer syntax")
            (Error 3350, Line 21, Col 24, Line 21, Col 32, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 21, Col 24, Line 21, Col 32, "Incomplete expression or invalid use of indexer syntax")
        ]
        
    // SOURCE=SequenceExpressions08.fs 	# SequenceExpressions08.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions08.fs"|])>]
    let ``Version 9: SequenceExpressions08 fs`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 7, Col 13, Line 7, Col 26, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 751, Line 7, Col 15, Line 7, Col 20, "Incomplete expression or invalid use of indexer syntax");
            (Error 3350, Line 8, Col 9, Line 8, Col 20, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 751, Line 8, Col 14, Line 8, Col 19, "Incomplete expression or invalid use of indexer syntax");
            (Error 3350, Line 9, Col 9, Line 9, Col 26, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 751, Line 9, Col 15, Line 9, Col 20, "Incomplete expression or invalid use of indexer syntax")
        ]
        
    // SOURCE=SequenceExpressions08.fs 	# SequenceExpressions08.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions08.fs"|])>]
    let ``Preview: SequenceExpressions08 fs`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed
        
    // SOURCE=SequenceExpressions09.fs 	# SequenceExpressions09.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions09.fs"|])>]
    let ``Preview: SequenceExpressions09 fs`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=SequenceExpressions09.fs 	# SequenceExpressions09.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions09.fs"|])>]
    let ``Version 9: SequenceExpressions09 fs`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompileAndRun
        |> shouldSucceed
        
    // SOURCE=SequenceExpressions10.fs 	# SequenceExpressions10.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions10.fs"|])>]
    let ``Version 9: SequenceExpressions10 fs`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 3, Col 13, Line 10, Col 2, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 5, Col 5, Line 5, Col 9, "Incomplete expression or invalid use of indexer syntax")
            (Error 751, Line 7, Col 5, Line 7, Col 11, "Incomplete expression or invalid use of indexer syntax")
            (Error 751, Line 9, Col 5, Line 9, Col 11, "Incomplete expression or invalid use of indexer syntax")
            (Error 3350, Line 12, Col 9, Line 19, Col 2, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 14, Col 5, Line 14, Col 9, "Incomplete expression or invalid use of indexer syntax")
            (Error 751, Line 16, Col 5, Line 16, Col 11, "Incomplete expression or invalid use of indexer syntax")
            (Error 751, Line 18, Col 5, Line 18, Col 11, "Incomplete expression or invalid use of indexer syntax")
            (Error 3350, Line 20, Col 9, Line 27, Col 3, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 22, Col 5, Line 22, Col 9, "Incomplete expression or invalid use of indexer syntax")
            (Error 751, Line 24, Col 5, Line 24, Col 11, "Incomplete expression or invalid use of indexer syntax")
            (Error 751, Line 26, Col 5, Line 26, Col 11, "Incomplete expression or invalid use of indexer syntax")
        ]
        
    // SOURCE=SequenceExpressions10.fs 	# SequenceExpressions10.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions10.fs"|])>]
    let ``Preview: SequenceExpressions10 fs`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed
        
    // SOURCE=SequenceExpressions11.fs 	# SequenceExpressions11.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions11.fs"|])>]
    let ``Version 9: SequenceExpressions11 fs`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 3, Col 13, Line 10, Col 2, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 751, Line 5, Col 5, Line 5, Col 13, "Incomplete expression or invalid use of indexer syntax");
            (Error 751, Line 7, Col 5, Line 7, Col 11, "Incomplete expression or invalid use of indexer syntax");
            (Error 751, Line 9, Col 5, Line 9, Col 14, "Incomplete expression or invalid use of indexer syntax");
            (Error 3350, Line 12, Col 9, Line 19, Col 2, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 751, Line 14, Col 5, Line 14, Col 13, "Incomplete expression or invalid use of indexer syntax");
            (Error 751, Line 16, Col 5, Line 16, Col 11, "Incomplete expression or invalid use of indexer syntax");
            (Error 751, Line 18, Col 5, Line 18, Col 14, "Incomplete expression or invalid use of indexer syntax");
            (Error 3350, Line 20, Col 9, Line 27, Col 3, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 751, Line 22, Col 5, Line 22, Col 13, "Incomplete expression or invalid use of indexer syntax");
            (Error 751, Line 24, Col 5, Line 24, Col 11, "Incomplete expression or invalid use of indexer syntax");
            (Error 751, Line 26, Col 5, Line 26, Col 14, "Incomplete expression or invalid use of indexer syntax") 
        ]
        
    // SOURCE=SequenceExpressions11.fs 	# SequenceExpressions11.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions11.fs"|])>]
    let ``Preview: SequenceExpressions11 fs`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=SequenceExpressions12.fs 	# SequenceExpressions12.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions12.fs"|])>]
    let ``Version 9: SequenceExpressions12 fs`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 3, Col 18, Line 3, Col 23, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")              
        ]
        
    // SOURCE=SequenceExpressions12.fs 	# SequenceExpressions12.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions12.fs"|])>]
    let ``Preview: SequenceExpressions12 fs`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=SequenceExpressions13.fs 	# SequenceExpressions13.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions13.fs"|])>]
    let ``Version 9: SequenceExpressions13 fs`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 3, Col 18, Line 3, Col 23, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 5, Col 38, Line 5, Col 44, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 9, Col 9, Line 9, Col 37, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 751, Line 9, Col 11, Line 9, Col 16, "Incomplete expression or invalid use of indexer syntax");
            (Error 3350, Line 9, Col 29, Line 9, Col 35, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 10, Col 22, Line 10, Col 27, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 10, Col 40, Line 10, Col 46, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 11, Col 19, Line 11, Col 24, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 11, Col 37, Line 11, Col 43, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
        ]
        
    // SOURCE=SequenceExpressions13.fs 	# SequenceExpressions13.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions13.fs"|])>]
    let ``Preview: SequenceExpressions13 fs`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=SequenceExpressions14.fs 	# SequenceExpressions14.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions14.fs"|])>]
    let ``Version 9: SequenceExpressions14 fs`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=SequenceExpressions13.fs 	# SequenceExpressions13.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions14.fs"|])>]
    let ``Preview: SequenceExpressions14 fs`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=E_SequenceExpressions02.fs 	# E_SequenceExpressions02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_SequenceExpressions02.fs"|])>]
    let ``Version 9: E_SequenceExpressions01 fs`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 3, Col 27, Line 3, Col 32, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 1, Line 3, Col 27, Line 3, Col 32, "This expression was expected to have type
    'int' 
but here has type
    ''a seq' ");
            (Error 1, Line 3, Col 27, Line 3, Col 32, "This expression was expected to have type
    'int' 
but here has type
    'int seq' ");
            (Error 3350, Line 4, Col 29, Line 4, Col 34, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 1, Line 6, Col 27, Line 6, Col 34, "This expression was expected to have type
    'int' 
but here has type
    ''a list' ");
            (Error 1, Line 9, Col 30, Line 9, Col 43, "This expression was expected to have type
    'int' 
but here has type
    ''a seq' ");
            (Error 1, Line 9, Col 30, Line 9, Col 43, "This expression was expected to have type
    'int' 
but here has type
    'int seq' ");
            (Error 3350, Line 12, Col 53, Line 12, Col 58, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 1, Line 12, Col 53, Line 12, Col 58, "This expression was expected to have type
    'int' 
but here has type
    ''a seq' ");
            (Error 1, Line 12, Col 53, Line 12, Col 58, "This expression was expected to have type
    'int' 
but here has type
    'int seq' ");
            (Error 3350, Line 13, Col 55, Line 13, Col 60, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 14, Col 41, Line 14, Col 46, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 1, Line 14, Col 41, Line 14, Col 46, "This expression was expected to have type
'int' 
but here has type
''a seq' ");
            (Error 1, Line 14, Col 41, Line 14, Col 46, "This expression was expected to have type
    'int' 
but here has type
    'int seq' ");
            (Error 3350, Line 15, Col 42, Line 15, Col 47, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 1, Line 17, Col 53, Line 17, Col 60, "This expression was expected to have type
    'int' 
but here has type
    ''a list' ");
            (Error 1, Line 19, Col 42, Line 19, Col 49, "This expression was expected to have type
    'int' 
but here has type
    ''a list' ");
            (Error 1, Line 22, Col 56, Line 22, Col 69, "This expression was expected to have type
    'int' 
but here has type
    ''a seq' ");
            (Error 1, Line 22, Col 56, Line 22, Col 69, "This expression was expected to have type
    'int' 
but here has type
    'int seq' ");
            (Error 1, Line 24, Col 44, Line 24, Col 57, "This expression was expected to have type
    'int' 
but here has type
    ''a seq' ");
            (Error 1, Line 24, Col 44, Line 24, Col 57, "This expression was expected to have type
    'int' 
but here has type
    'int seq' ");
            (Error 3350, Line 27, Col 72, Line 27, Col 77, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 1, Line 27, Col 72, Line 27, Col 77, "This expression was expected to have type
    'int' 
but here has type
    ''a seq' ");
            (Error 1, Line 27, Col 72, Line 27, Col 77, "This expression was expected to have type
    'int' 
but here has type
    'int seq' ");
            (Error 3350, Line 28, Col 74, Line 28, Col 79, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 1, Line 30, Col 72, Line 30, Col 79, "This expression was expected to have type
    'int' 
but here has type
    ''a list' ");
            (Error 1, Line 33, Col 75, Line 33, Col 88, "This expression was expected to have type
    'int' 
but here has type
    ''a seq' ");
            (Error 1, Line 33, Col 75, Line 33, Col 88, "This expression was expected to have type
    'int' 
but here has type
    'int seq' ")
        ]

    // SOURCE=E_SequenceExpressions02.fs 	# E_SequenceExpressions02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_SequenceExpressions02.fs"|])>]
    let ``Preview: E_SequenceExpressions02 fs`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 3, Col 27, Line 3, Col 32, "This expression was expected to have type
    'int' 
but here has type
    ''a seq' ");
            (Error 1, Line 3, Col 27, Line 3, Col 32, "This expression was expected to have type
    'int' 
but here has type
    'int seq' ");
            (Error 1, Line 6, Col 27, Line 6, Col 34, "This expression was expected to have type
    'int' 
but here has type
    ''a list' ");
            (Error 1, Line 9, Col 30, Line 9, Col 43, "This expression was expected to have type
    'int' 
but here has type
    ''a seq' ");
            (Error 1, Line 9, Col 30, Line 9, Col 43, "This expression was expected to have type
    'int' 
but here has type
    'int seq' ");
            (Error 1, Line 12, Col 53, Line 12, Col 58, "This expression was expected to have type
    'int' 
but here has type
    ''a seq' ");
            (Error 1, Line 12, Col 53, Line 12, Col 58, "This expression was expected to have type
    'int' 
but here has type
    'int seq' ");
            (Error 1, Line 14, Col 41, Line 14, Col 46, "This expression was expected to have type
    'int' 
but here has type
    ''a seq' ");
            (Error 1, Line 14, Col 41, Line 14, Col 46, "This expression was expected to have type
    'int' 
but here has type
    'int seq' ");
            (Error 1, Line 17, Col 53, Line 17, Col 60, "This expression was expected to have type
    'int' 
but here has type
    ''a list' ")
            (Error 1, Line 19, Col 42, Line 19, Col 49, "This expression was expected to have type
    'int' 
but here has type
    ''a list' ");
            (Error 1, Line 22, Col 56, Line 22, Col 69, "This expression was expected to have type
    'int' 
but here has type
    ''a seq' ");
            (Error 1, Line 22, Col 56, Line 22, Col 69, "This expression was expected to have type
    'int' 
but here has type
    'int seq' ");
            (Error 1, Line 24, Col 44, Line 24, Col 57, "This expression was expected to have type
    'int' 
but here has type
    ''a seq' ");
            (Error 1, Line 24, Col 44, Line 24, Col 57, "This expression was expected to have type
    'int' 
but here has type
    'int seq' ");
            (Error 1, Line 27, Col 72, Line 27, Col 77, "This expression was expected to have type
    'int' 
but here has type
    ''a seq' ");
            (Error 1, Line 27, Col 72, Line 27, Col 77, "This expression was expected to have type
    'int' 
but here has type
    'int seq' ");
            (Error 1, Line 30, Col 72, Line 30, Col 79, "This expression was expected to have type
    'int' 
but here has type
    ''a list' ");
            (Error 1, Line 33, Col 75, Line 33, Col 88, "This expression was expected to have type
    'int' 
but here has type
    ''a seq' ");
            (Error 1, Line 33, Col 75, Line 33, Col 88, "This expression was expected to have type
    'int' 
but here has type
    'int seq' ")
        ]

    // SOURCE=SequenceExpressions15.fs 	# SequenceExpressions15.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions15.fs"|])>]
    let ``Preview: SequenceExpressions15 fs`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed
    
    // SOURCE=SequenceExpressions15.fs 	# SequenceExpressions15.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions15.fs"|])>]
    let ``Version 9: SequenceExpressions15 fs`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 3, Col 19, Line 3, Col 23, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 3350, Line 4, Col 23, Line 4, Col 27, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 3350, Line 5, Col 20, Line 5, Col 24, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 3350, Line 9, Col 29, Line 9, Col 33, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 3350, Line 10, Col 32, Line 10, Col 36, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 3350, Line 11, Col 31, Line 11, Col 35, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
        ]
    
    // SOURCE=SequenceExpressions16.fs 	# SequenceExpressions16.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions16.fs"|])>]
    let ``Preview: SequenceExpressions16 fs`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed
    
    // SOURCE=SequenceExpressions16.fs 	# SequenceExpressions16.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions16.fs"|])>]
    let ``Version 9: SequenceExpressions16 fs`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 3, Col 28, Line 3, Col 33, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 8, Col 54, Line 8, Col 59, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 9, Col 42, Line 9, Col 47, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 17, Col 73, Line 17, Col 78, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
        ]

    // SOURCE=SequenceExpressions17.fs 	# SequenceExpressions17.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions17.fs"|])>]
    let ``Preview: SequenceExpressions17 fs`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed
    
    // SOURCE=SequenceExpressions17.fs 	# SequenceExpressions17.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions17.fs"|])>]
    let ``Version 9: SequenceExpressions17 fs`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 751, Line 15, Col 23, Line 15, Col 28, "Incomplete expression or invalid use of indexer syntax");
            (Error 751, Line 18, Col 27, Line 18, Col 32, "Incomplete expression or invalid use of indexer syntax");
            (Error 751, Line 41, Col 18, Line 41, Col 23, "Incomplete expression or invalid use of indexer syntax");
            (Error 751, Line 49, Col 24, Line 49, Col 29, "Incomplete expression or invalid use of indexer syntax")
        ]