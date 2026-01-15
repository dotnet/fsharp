// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.
namespace ErrorMessages

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
printfn "Finished"
    """

    [<Fact>]
    let ``Warn If Discarded In List -- 01 - preview``() =
        // This no longer gives a warning because implicit yields are allowed in preview
        FSharp source
        |> asExe
        |> withLangVersionPreview
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics []

    [<Fact>]
    let ``Warn If Discarded In List - 01 - preview2``() =
        // This no longer gives a warning because implicit yields are allowed in preview
        FSharp source
        |> asExe
        |> withLangVersionPreview
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics []

    [<Fact>]
    let ``Warn If Discarded In List - 02 - preview``() =
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
        
printfn "Finished"
        """
        // This no longer gives a warning because implicit yields are allowed in preview
        FSharp source
        |> asExe
        |> withLangVersionPreview
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics []

    [<Fact>]
    let ``Warn If Discarded In List - 02 - preview2``() =
        let source = """
// This no longer gives a warning because implicit yields are now allowed
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

printfn "Finished"
"""
        // This no longer gives a warning because implicit yields are allowed in preview
        FSharp source
        |> asExe
        |> withLangVersionPreview
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics []

    [<Fact>]
    let ``Warn If Discarded In List - 02 - preview3``() =
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
        
printfn "Finished"
        """
        // This no longer gives a warning because implicit yields are allowed in preview
        FSharp source
        |> asExe
        |> withLangVersionPreview
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics []

    [<Fact>]
    let ``Warn If Discarded In List - 03 - preview``() =
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

printfn "Finished"
        """
        // Warning 20 no longer fires in preview because implicit yields are allowed
        FSharp source
        |> asExe
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
        |> withDiagnostics []

    [<Fact>]
    let ``Warn If Discarded In List - 03 - preview2``() =
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
printfn "Finished"
        """
        // This no longer gives a warning because implicit yields are allowed in preview
        FSharp source
        |> asExe
        |> withLangVersionPreview
        |> ignoreWarnings
        |> compile
        |> shouldSucceed
        |> withDiagnostics []
