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
        
    // SOURCE=SequenceExpressions00.fs     # SequenceExpressions00.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions00.fs"|])>]
    let ``Version 9: Mixed ranges and values without preview should fail (baseline comparison)`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 8, Col 15, Line 8, Col 20, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 8, Col 15, Line 8, Col 20, "Incomplete expression or invalid use of indexer syntax")
            (Error 3350, Line 9, Col 14, Line 9, Col 19, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 9, Col 14, Line 9, Col 19, "Incomplete expression or invalid use of indexer syntax")
            (Error 3350, Line 10, Col 15, Line 10, Col 20, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 10, Col 15, Line 10, Col 20, "Incomplete expression or invalid use of indexer syntax")
        ]
        
    // SOURCE=SequenceExpressions00.fs     # SequenceExpressions00.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions00.fs"|])>]
    let ``Preview: Mixed ranges and values compile in all collection forms (baseline comparison)`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompile
        |> shouldSucceed
        
    // SOURCE=SequenceExpressions02a.fs	# SequenceExpressions02a.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions02a.fs"|])>]
    let ``Version 9: List literal mixing a single ascending range with a leading value (02a)`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 3, Col 17, Line 3, Col 22, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 751, Line 3, Col 17, Line 3, Col 22, "Incomplete expression or invalid use of indexer syntax")
        ]

    // SOURCE=SequenceExpressions02a.fs	# SequenceExpressions02a.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions02a.fs"|])>]
    let ``Preview: List literal mixing a single ascending range with a leading value (02a)`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=SequenceExpressions02b.fs	# SequenceExpressions02b.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions02b.fs"|])>]
    let ``Version 9: List literal mixing a range in the middle of values (02b)`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 3, Col 17, Line 3, Col 22, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 3, Col 17, Line 3, Col 22, "Incomplete expression or invalid use of indexer syntax")
        ]

    // SOURCE=SequenceExpressions02b.fs	# SequenceExpressions02b.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions02b.fs"|])>]
    let ``Preview: List literal mixing a range in the middle of values (02b)`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=SequenceExpressions02c.fs	# SequenceExpressions02c.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions02c.fs"|])>]
    let ``Version 9: List literal with multiple ranges and trailing value (02c)`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 3, Col 13, Line 3, Col 17, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 3350, Line 3, Col 19, Line 3, Col 23, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 3, Col 13, Line 3, Col 17, "Incomplete expression or invalid use of indexer syntax")
            (Error 751, Line 3, Col 19, Line 3, Col 23, "Incomplete expression or invalid use of indexer syntax")
        ]

    // SOURCE=SequenceExpressions02c.fs	# SequenceExpressions02c.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions02c.fs"|])>]
    let ``Preview: List literal with multiple ranges and trailing value (02c)`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=SequenceExpressions02d.fs	# SequenceExpressions02d.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions02d.fs"|])>]
    let ``Version 9: List literal with stepped range between values (02d)`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 3, Col 16, Line 3, Col 24, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 751, Line 3, Col 16, Line 3, Col 24, "Incomplete expression or invalid use of indexer syntax")
        ]

    // SOURCE=SequenceExpressions02d.fs	# SequenceExpressions02d.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions02d.fs"|])>]
    let ``Preview: List literal with stepped range between values (02d)`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=SequenceExpressions03.fs 	# SequenceExpressions03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions03.fs"|])>]
    let ``Preview: seq builder: ranges as splices are accepted (03)`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed
        
    // SOURCE=SequenceExpressions03.fs 	# SequenceExpressions03.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions03.fs"|])>]
    let ``Version 9: seq builder: ranges without preview should fail (03)`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 3, Col 19, Line 3, Col 24, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 751, Line 3, Col 19, Line 3, Col 24, "Incomplete expression or invalid use of indexer syntax");
            (Error 3350, Line 7, Col 19, Line 7, Col 24, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 751, Line 7, Col 19, Line 7, Col 24, "Incomplete expression or invalid use of indexer syntax");
            (Error 3350, Line 11, Col 15, Line 11, Col 19, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 11, Col 21, Line 11, Col 25, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 751, Line 11, Col 15, Line 11, Col 19, "Incomplete expression or invalid use of indexer syntax");
            (Error 751, Line 11, Col 21, Line 11, Col 25, "Incomplete expression or invalid use of indexer syntax");
            (Error 3350, Line 15, Col 18, Line 15, Col 26, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 751, Line 15, Col 18, Line 15, Col 26, "Incomplete expression or invalid use of indexer syntax");
            (Error 3350, Line 19, Col 18, Line 19, Col 22, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 751, Line 19, Col 18, Line 19, Col 22, "Incomplete expression or invalid use of indexer syntax");
            (Error 3350, Line 23, Col 18, Line 23, Col 22, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 751, Line 23, Col 18, Line 23, Col 22, "Incomplete expression or invalid use of indexer syntax")
        ]

    // SOURCE=SequenceExpressions04.fs 	# SequenceExpressions04.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions04.fs"|])>]
    let ``Preview: seq builder: ranges in branches etc. accepted (04)`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed
        
    // SOURCE=SequenceExpressions04.fs 	# SequenceExpressions04.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions04.fs"|])>]
    let ``Version 9: seq builder: ranges without preview should fail (04)`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 3, Col 19, Line 3, Col 24, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 751, Line 3, Col 19, Line 3, Col 24, "Incomplete expression or invalid use of indexer syntax");
            (Error 3350, Line 7, Col 23, Line 7, Col 27, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 7, Col 29, Line 7, Col 35, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 751, Line 7, Col 23, Line 7, Col 27, "Incomplete expression or invalid use of indexer syntax");
            (Error 751, Line 7, Col 29, Line 7, Col 35, "Incomplete expression or invalid use of indexer syntax");
            (Error 3350, Line 11, Col 22, Line 11, Col 30, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 751, Line 11, Col 22, Line 11, Col 30, "Incomplete expression or invalid use of indexer syntax")
        ]

    // SOURCE=SequenceExpressions05.fs 	# SequenceExpressions05.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions05.fs"|])>]
    let ``Preview: seq builder: mixing ranges with values compiles and runs (05)`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed
        
    // SOURCE=SequenceExpressions05.fs 	# SequenceExpressions05.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions05.fs"|])>]
    let ``Version 9: seq builder: mixing ranges with values requires preview (05)`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 3, Col 19, Line 3, Col 24, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 3, Col 19, Line 3, Col 24, "Incomplete expression or invalid use of indexer syntax")
            (Error 3350, Line 7, Col 22, Line 7, Col 30, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 7, Col 22, Line 7, Col 30, "Incomplete expression or invalid use of indexer syntax")
        ]
        
    // SOURCE=SequenceExpressions06.fs 	# SequenceExpressions06.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions06.fs"|])>]
    let ``Preview: Type inference across list/array/seq with mixed ranges (06)`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed
        
    // SOURCE=SequenceExpressions06.fs 	# SequenceExpressions06.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions06.fs"|])>]
    let ``Version 9: Type inference with mixed ranges requires preview (06)`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 3, Col 24, Line 3, Col 28, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 3, Col 24, Line 3, Col 28, "Incomplete expression or invalid use of indexer syntax")
            (Error 3350, Line 4, Col 26, Line 4, Col 30, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 4, Col 26, Line 4, Col 30, "Incomplete expression or invalid use of indexer syntax")
            (Error 3350, Line 5, Col 29, Line 5, Col 33, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 5, Col 29, Line 5, Col 33, "Incomplete expression or invalid use of indexer syntax")
            (Error 3350, Line 8, Col 6, Line 8, Col 19, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 8, Col 6, Line 8, Col 19, "Incomplete expression or invalid use of indexer syntax")
        ]
        
    // SOURCE=SequenceExpressions07.fs 	# SequenceExpressions07.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions07.fs"|])>]
    let ``Preview: Custom CE builder splices built-in ranges via YieldFrom (07)`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed
        
        
    // SOURCE=SequenceExpressions07.fs 	# SequenceExpressions07.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions07.fs"|])>]
    let ``Version 9: Custom CE with mixed ranges requires preview (07)`` compilation =
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
    let ``Version 9: Equivalence examples require preview (08)`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 7, Col 15, Line 7, Col 20, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 7, Col 15, Line 7, Col 20, "Incomplete expression or invalid use of indexer syntax")
            (Error 3350, Line 8, Col 14, Line 8, Col 19, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 8, Col 14, Line 8, Col 19, "Incomplete expression or invalid use of indexer syntax")
            (Error 3350, Line 9, Col 15, Line 9, Col 20, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 9, Col 15, Line 9, Col 20, "Incomplete expression or invalid use of indexer syntax")
        ]
        
    // SOURCE=SequenceExpressions08.fs 	# SequenceExpressions08.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions08.fs"|])>]
    let ``Preview: Equivalence: direct ranges equal explicit yield! in seq/list/array (08)`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed
        
    // SOURCE=SequenceExpressions09.fs 	# SequenceExpressions09.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions09.fs"|])>]
    let ``Preview: Non-interference: custom (..) operator is not treated as range (09)`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=SequenceExpressions09.fs 	# SequenceExpressions09.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions09.fs"|])>]
    let ``Version 9: Non-interference: custom (..) operator is not treated as range (09)`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompileAndRun
        |> shouldSucceed
        
    // SOURCE=SequenceExpressions10.fs 	# SequenceExpressions10.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions10.fs"|])>]
    let ``Version 9: Mixed ranges and values across seq/list/array blocks require preview (10)`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 5, Col 5, Line 5, Col 9, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 7, Col 5, Line 7, Col 11, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 9, Col 5, Line 9, Col 11, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 751, Line 5, Col 5, Line 5, Col 9, "Incomplete expression or invalid use of indexer syntax");
            (Error 751, Line 7, Col 5, Line 7, Col 11, "Incomplete expression or invalid use of indexer syntax");
            (Error 751, Line 9, Col 5, Line 9, Col 11, "Incomplete expression or invalid use of indexer syntax");
            (Error 3350, Line 14, Col 5, Line 14, Col 9, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 16, Col 5, Line 16, Col 11, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 18, Col 5, Line 18, Col 11, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 751, Line 14, Col 5, Line 14, Col 9, "Incomplete expression or invalid use of indexer syntax");
            (Error 751, Line 16, Col 5, Line 16, Col 11, "Incomplete expression or invalid use of indexer syntax");
            (Error 751, Line 18, Col 5, Line 18, Col 11, "Incomplete expression or invalid use of indexer syntax");
            (Error 3350, Line 22, Col 5, Line 22, Col 9, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 24, Col 5, Line 24, Col 11, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 26, Col 5, Line 26, Col 11, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 751, Line 22, Col 5, Line 22, Col 9, "Incomplete expression or invalid use of indexer syntax");
            (Error 751, Line 24, Col 5, Line 24, Col 11, "Incomplete expression or invalid use of indexer syntax");
            (Error 751, Line 26, Col 5, Line 26, Col 11, "Incomplete expression or invalid use of indexer syntax")
        ]
        
    // SOURCE=SequenceExpressions10.fs 	# SequenceExpressions10.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions10.fs"|])>]
    let ``Preview: Mixed ranges and values across seq/list/array blocks compile and match explicit yield! (10)`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed
        
    // SOURCE=SequenceExpressions11.fs 	# SequenceExpressions11.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions11.fs"|])>]
    let ``Version 9: Stepped ranges mixed with values require preview (11)`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 5, Col 5, Line 5, Col 13, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 7, Col 5, Line 7, Col 11, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 9, Col 5, Line 9, Col 14, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 751, Line 5, Col 5, Line 5, Col 13, "Incomplete expression or invalid use of indexer syntax");
            (Error 751, Line 7, Col 5, Line 7, Col 11, "Incomplete expression or invalid use of indexer syntax");
            (Error 751, Line 9, Col 5, Line 9, Col 14, "Incomplete expression or invalid use of indexer syntax");
            (Error 3350, Line 14, Col 5, Line 14, Col 13, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 16, Col 5, Line 16, Col 11, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 18, Col 5, Line 18, Col 14, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 751, Line 14, Col 5, Line 14, Col 13, "Incomplete expression or invalid use of indexer syntax");
            (Error 751, Line 16, Col 5, Line 16, Col 11, "Incomplete expression or invalid use of indexer syntax");
            (Error 751, Line 18, Col 5, Line 18, Col 14, "Incomplete expression or invalid use of indexer syntax");
            (Error 3350, Line 22, Col 5, Line 22, Col 13, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 24, Col 5, Line 24, Col 11, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 26, Col 5, Line 26, Col 14, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 751, Line 22, Col 5, Line 22, Col 13, "Incomplete expression or invalid use of indexer syntax");
            (Error 751, Line 24, Col 5, Line 24, Col 11, "Incomplete expression or invalid use of indexer syntax");
            (Error 751, Line 26, Col 5, Line 26, Col 14, "Incomplete expression or invalid use of indexer syntax")
        ]
        
    // SOURCE=SequenceExpressions11.fs 	# SequenceExpressions11.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions11.fs"|])>]
    let ``Preview: Stepped ranges mixed with values compile and match explicit yield! (11)`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=SequenceExpressions12.fs 	# SequenceExpressions12.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions12.fs"|])>]
    let ``Version 9: Single direct range in list/seq/array requires preview (12)`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 3, Col 18, Line 3, Col 23, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")              
        ]
        
    // SOURCE=SequenceExpressions12.fs 	# SequenceExpressions12.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions12.fs"|])>]
    let ``Preview: Single direct range in list/seq/array equals explicit yield! (12)`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=SequenceExpressions13.fs 	# SequenceExpressions13.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions13.fs"|])>]
    let ``Version 9: Interleaving ranges and values requires preview (13)`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 3, Col 18, Line 3, Col 23, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 5, Col 38, Line 5, Col 44, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 9, Col 11, Line 9, Col 16, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 751, Line 9, Col 11, Line 9, Col 16, "Incomplete expression or invalid use of indexer syntax");
            (Error 3350, Line 9, Col 29, Line 9, Col 35, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 10, Col 22, Line 10, Col 27, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 10, Col 40, Line 10, Col 46, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 11, Col 19, Line 11, Col 24, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 11, Col 37, Line 11, Col 43, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
        ]
        
    // SOURCE=SequenceExpressions13.fs 	# SequenceExpressions13.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions13.fs"|])>]
    let ``Preview: Interleaving ranges and values equals explicit forms (13)`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=SequenceExpressions14.fs 	# SequenceExpressions14.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions14.fs"|])>]
    let ``Version 9: No ranges: existing yield/yield! behavior unchanged (14)`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=SequenceExpressions13.fs 	# SequenceExpressions13.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions14.fs"|])>]
    let ``Preview: No ranges: existing yield/yield! behavior unchanged (14)`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=E_SequenceExpressions02.fs 	# E_SequenceExpressions02.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"E_SequenceExpressions02.fs"|])>]
    let ``Version 9: Invalid 'yield range' vs 'yield! range' typing diagnostics (E_02)`` compilation =
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
    let ``Preview: Invalid 'yield range' vs 'yield! range' typing diagnostics (E_02)`` compilation =
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
    let ``Preview: yield vs yield! ranges baseline (15)`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed
    
    // SOURCE=SequenceExpressions15.fs 	# SequenceExpressions15.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions15.fs"|])>]
    let ``Version 9: yield vs yield! baseline compilation behavior (15)`` compilation =
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
            (Error 3350, Line 15, Col 30, Line 15, Col 35, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 18, Col 34, Line 18, Col 39, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 41, Col 25, Line 41, Col 30, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 49, Col 31, Line 49, Col 36, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
        ]

    // SOURCE=SequenceExpressions18.fs  	# SequenceExpressions18.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions18.fs"|])>]
    let ``Preview: char ranges mixed with values compile and run`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed
        
    // SOURCE=SequenceExpressions18.fs  	# SequenceExpressions18.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions18.fs"|])>]
    let ``Version 9: char ranges mixed with values require preview`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 3, Col 18, Line 3, Col 26, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 3, Col 18, Line 3, Col 26, "Incomplete expression or invalid use of indexer syntax")
        ]

    // SOURCE=SequenceExpressions19.fs  	# SequenceExpressions19.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions19.fs"|])>]
    let ``Preview: bigint ranges mixed with values compile and run`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=SequenceExpressions19.fs  	# SequenceExpressions19.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions19.fs"|])>]
    let ``Version 9: bigint ranges mixed with values require preview`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 4, Col 14, Line 4, Col 24, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 751, Line 4, Col 14, Line 4, Col 24, "Incomplete expression or invalid use of indexer syntax")
        ]

    // SOURCE=SequenceExpressions20.fs  	# SequenceExpressions20.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions20.fs"|])>]
    let ``Preview: empty and singleton ranges preserve order`` compilation =
        compilation
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    // SOURCE=SequenceExpressions20.fs  	# SequenceExpressions20.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"SequenceExpressions20.fs"|])>]
    let ``Version 9: empty and singleton ranges inside list literals require preview`` compilation =
        compilation
        |> withLangVersion90
        |> verifyCompile
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 3, Col 17, Line 3, Col 21, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 3, Col 23, Line 3, Col 27, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 3, Col 29, Line 3, Col 33, "Feature 'Allow mixed ranges and values in sequence expressions, e.g. seq { 1..10; 20 }' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 751, Line 3, Col 17, Line 3, Col 21, "Incomplete expression or invalid use of indexer syntax");
            (Error 751, Line 3, Col 23, Line 3, Col 27, "Incomplete expression or invalid use of indexer syntax");
            (Error 751, Line 3, Col 29, Line 3, Col 33, "Incomplete expression or invalid use of indexer syntax")
        ]
