// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.PatternMatching

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module NamedPatPairs =
    
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

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NamedPatPairs01.fs"|])>]
    let ``Version9 NamedPatPairs - NamedPatPairs01_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> withLangVersion90
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 9, Col 18, Line 9, Col 19, "Feature 'Allow comma as union case name field separator.' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 10, Col 25, Line 10, Col 26, "Feature 'Allow comma as union case name field separator.' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 10, Col 18, Line 10, Col 19, "Feature 'Allow comma as union case name field separator.' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
        ]

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NamedPatPairs01.fs"|])>]
    let ``Preview: NamedPatPairs - NamedPatPairs01_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NamedPatPairs02.fs"|])>]
    let ``Version9 NamedPatPairs - NamedPatPairs02_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> withLangVersion90
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NamedPatPairs02.fs"|])>]
    let ``Preview: NamedPatPairs - NamedPatPairs02_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NamedPatPairs03.fs"|])>]
    let ``Version9 NamedPatPairs - NamedPatPairs03_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> withLangVersion90
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 10, Col 18, Line 10, Col 19, "Feature 'Allow comma as union case name field separator.' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 3350, Line 11, Col 25, Line 11, Col 26, "Feature 'Allow comma as union case name field separator.' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 3350, Line 11, Col 18, Line 11, Col 19, "Feature 'Allow comma as union case name field separator.' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
        ]

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NamedPatPairs03.fs"|])>]
    let ``Preview: NamedPatPairs - NamedPatPairs03_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NamedPatPairs04.fs"|])>]
    let ``Version9 NamedPatPairs - NamedPatPairs04_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> withLangVersion90
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NamedPatPairs04.fs"|])>]
    let ``Preview: NamedPatPairs - NamedPatPairs04_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed
        
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NamedPatPairs05.fs"|])>]
    let ``Version9 NamedPatPairs - NamedPatPairs05_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> withLangVersion90
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NamedPatPairs05.fs"|])>]
    let ``Preview: NamedPatPairs - NamedPatPairs05_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NamedPatPairs06.fs"|])>]
    let ``Version9 NamedPatPairs - NamedPatPairs06_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> withLangVersion90
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NamedPatPairs06.fs"|])>]
    let ``Preview: NamedPatPairs - NamedPatPairs06_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NamedPatPairs07.fs"|])>]
    let ``Version9 NamedPatPairs - NamedPatPairs07_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> withLangVersion90
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NamedPatPairs07.fs"|])>]
    let ``Preview: NamedPatPairs - NamedPatPairs07_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed
        
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NamedPatPairs08.fs"|])>]
    let ``Version9 NamedPatPairs - NamedPatPairs08_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> withLangVersion90
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NamedPatPairs08.fs"|])>]
    let ``Preview: NamedPatPairs - NamedPatPairs08_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NamedPatPairs09.fs"|])>]
    let ``Version9 NamedPatPairs - NamedPatPairs09_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> withLangVersion90
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NamedPatPairs09.fs"|])>]
    let ``Preview: NamedPatPairs - NamedPatPairs09_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NamedPatPairs10.fs"|])>]
    let ``Version9 NamedPatPairs - NamedPatPairs10_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> withLangVersion90
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 8, Col 21, Line 8, Col 22, "Feature 'Allow comma as union case name field separator.' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 3350, Line 12, Col 23, Line 12, Col 24, "Feature 'Allow comma as union case name field separator.' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
            (Error 3350, Line 16, Col 21, Line 16, Col 22, "Feature 'Allow comma as union case name field separator.' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
        ]

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NamedPatPairs10.fs"|])>]
    let ``Preview: NamedPatPairs - NamedPatPairs10_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed
        
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NamedPatPairs11.fs"|])>]
    let ``Version9 NamedPatPairs - NamedPatPairs11_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> withLangVersion90
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 4, Col 17, Line 4, Col 18, "Feature 'Allow comma as union case name field separator.' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 14, Col 24, Line 14, Col 25, "Feature 'Allow comma as union case name field separator.' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.");
            (Error 3350, Line 14, Col 17, Line 14, Col 18, "Feature 'Allow comma as union case name field separator.' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
        ]

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NamedPatPairs11.fs"|])>]
    let ``Preview: NamedPatPairs - NamedPatPairs11_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed
        
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NamedPatPairs12.fs"|])>]
    let ``Version9 NamedPatPairs - NamedPatPairs12_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> withLangVersion90
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NamedPatPairs12.fs"|])>]
    let ``Preview: NamedPatPairs - NamedPatPairs12_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NamedPatPairs13.fs"|])>]
    let ``Version9 NamedPatPairs - NamedPatPairs13_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> withLangVersion90
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3350, Line 10, Col 18, Line 10, Col 19, "Feature 'Allow comma as union case name field separator.' is not available in F# 9.0. Please use language version 'PREVIEW' or greater.")
        ]

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NamedPatPairs13.fs"|])>]
    let ``Preview: NamedPatPairs - NamedPatPairs13_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 1, Line 9, Col 17, Line 9, Col 21, "This expression was expected to have type
'int' 
but here has type
''a * 'b' ");
            (Error 1, Line 10, Col 24, Line 10, Col 28, "This expression was expected to have type
'bool' 
but here has type
''a * 'b' ");
            (Warning 25, Line 7, Col 11, Line 7, Col 16, "Incomplete pattern matches on this expression.")
        ]
        
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NamedPatPairs14.fs"|])>]
    let ``Version9 NamedPatPairs - NamedPatPairs14_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> withLangVersion90
        |> verifyCompileAndRun
        |> shouldSucceed
    
    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"NamedPatPairs14.fs"|])>]
    let ``Preview: NamedPatPairs - NamedPatPairs14_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> withLangVersionPreview
        |> verifyCompileAndRun
        |> shouldSucceed