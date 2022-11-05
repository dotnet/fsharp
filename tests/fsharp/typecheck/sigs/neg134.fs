module Neg134
open System.Threading.Tasks

let a () = task {
    while! Task.FromResult 1 do
        ()
}

let b () = async {
    while! true do
        ()
}

let c () = async {
    do! 1

    while! async { return true } do
        ()
}

let d () = async {
    while! async { return true }
        ()
}