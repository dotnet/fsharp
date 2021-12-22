// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace FSharp.Compiler.ComponentTests.ErrorMessages

open Xunit
open FSharp.Test.Compiler

module ``Warn If Discarded In List`` =

    let source = """
let div _ _ = 1  
let subView _ _ = [1; 2]

// elmish view
let view model dispatch =
   [
       yield! subView model dispatch
       div [] []
   ]

exit 1
    """

    [<Fact>]
    let ``Warn If Discarded In List -- 01 - version46``() =
        // warning because implicit yields are not allowed in F# 4.6 and earlier
        FSharp source
        |> asExe
        |> withLangVersion46
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics [ Warning 3221, Line 9, Col 8,  Line 9, Col 17, "This expression returns a value of type 'int' but is implicitly discarded. Consider using 'let' to bind the result to a name, e.g. 'let result = expression'. If you intended to use the expression as a value in the sequence then use an explicit 'yield'." ]

    [<Fact>]
    let ``Warn If Discarded In List - 01 - version47``() =
        // This no longer gives a warning because implicit yields are allowed in F# 4.7 and above
        FSharp source
        |> asExe
        |> withLangVersion47
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics []

    [<Fact>]
    let ``Warn If Discarded In List - 02 - version46``() =
        let source = """
// #Warnings
//<Expects status="Warning" span="(15,19)" id="FS3222"></Expects>

// stupid things to make the sample compile
let div _ _ = 1
let subView _ _ = [1; 2]
let y = 1

// elmish view
let view model dispatch =
   [
    div [] [
       match y with
       | 1 -> yield! subView model dispatch
       | _ -> subView model dispatch
    ]
   ]

exit 1
"""
        // warning because implicit yields are not allowed in F# 4.6 and earlier
        FSharp source
        |> asExe
        |> withLangVersion46
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics [ Warning 3222, Line 16, Col 15,  Line 16, Col 37, "This expression returns a value of type 'int list' but is implicitly discarded. Consider using 'let' to bind the result to a name, e.g. 'let result = expression'. If you intended to use the expression as a value in the sequence then use an explicit 'yield!'." ]

    [<Fact>]
    let ``Warn If Discarded In List - 02 - version47``() =
        let source = """
// This no longer gives a warning because implicit yields are now allowed
        
        
// stupid things to make the sample compile
let div _ _ = 1  
let subView _ _ = [1; 2]
let subView2 _ _ = 1
let y = 1
        
// elmish view
let view model dispatch =
    [   
        div [] [
            match y with
            | 1 -> yield! subView model dispatch
            | _ -> subView2 model dispatch
        ]
    ]
        
exit 0
        """
        // This no longer gives a warning because implicit yields are allowed in F# 4.7 and above
        FSharp source
        |> asExe
        |> withLangVersion47
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics []

    [<Fact>]
    let ``Warn If Discarded In List - 03 - version46``() =
        let source = """
// stupid things to make the sample compile
let div _ _ = 1  
let subView _ _ = true
let y = 1

// elmish view
let view model dispatch =
   [   
        div [] [
           match y with
           | 1 -> ()
           | _ -> subView model dispatch
        ]
   ]

exit 0
        """
        // This no longer gives a warning because implicit yields are allowed in F# 4.7 and above
        FSharp source
        |> asExe
        |> withLangVersion46
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics [ Warning 20, Line 13, Col 19, Line 13, Col 41, "The result of this expression has type 'bool' and is implicitly ignored. Consider using 'ignore' to discard this value explicitly, e.g. 'expr |> ignore', or 'let' to bind the result to a name, e.g. 'let result = expr'." ]

    [<Fact>]
    let ``Warn If Discarded In List - 03 - version47``() =
        let source = """
// This no longer gives a warning because implicit yields are now allowed


// stupid things to make the sample compile
let div _ _ = 1  
let subView _ _ = true
let y = 1

// elmish view
let view model dispatch =
   [   
        div [] [
           match y with
           | 1 -> ()
           | _ -> subView model dispatch
        ]
   ]

exit 0
        """
        // This no longer gives a warning because implicit yields are allowed in F# 4.7 and above
        FSharp source
        |> asExe
        |> withLangVersion47
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics []
