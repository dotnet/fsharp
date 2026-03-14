module Debugging.SequencePointIssues

open System
open System.Threading.Tasks

let issue12052_match () =
    let input = 1

    let matched =
        match input with
        | 1 ->
            let branchValue = input + 10 // BP_12052_MATCH_BRANCH
            branchValue
        | _ ->
            input - 1

    let afterMatch = matched + 1 // BP_12052_AFTER_MATCH
    afterMatch

let issue19248_asyncReturn () =
    async {
        return 1 + 1 // BP_19248_RETURN_EXPR
    }
    |> Async.RunSynchronously

let issue19255_taskUse () =
    let disposable =
        { new IDisposable with
            member _.Dispose() = () }

    task {
        use _resource = disposable // BP_19255_USE_BINDING
        return 1 + 2 // BP_19255_RETURN_EXPR
    }
    |> fun t -> t.Result

let issue13504_listComprehension () =
    let values = [ 3 ]

    [ for x in values ->
        let squared = x * x // BP_13504_BODY_LET
        squared + 1
    ]

[<EntryPoint>]
let main _ =
    let _matchResult = issue12052_match ()
    let _asyncResult = issue19248_asyncReturn ()
    let _taskResult = issue19255_taskUse ()
    let _listResult = issue13504_listComprehension ()
    printfn "done"
    0
