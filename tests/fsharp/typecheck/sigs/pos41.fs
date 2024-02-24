module Pos41

open System
open System.Threading.Tasks

let a () = async {
    while! async { return true } do
        ()
}

let b () = task {
    while! task { return true } do
        ()
}

let c () = task {
    while! Task.FromResult true do
        ()
}

let d' (t: DateTime) = Task.FromResult (t.Second = 1)

let d () = task {
    do! Async.Sleep 1000

    while! d' DateTime.Now do
        try
            while true do
                printfn "yup"
        with _ ->
            do! Async.Sleep 1000
}