/// match! - type check errors
module M

type MyUnion = | CaseA of int | CaseB | CaseC of string

let a (notAnAsync: string) = async {
    match! notAnAsync with
    | x -> () }

let b (myAsync: Async<int>) = async {
    match! myAsync with
    | CaseA(_) | CaseB | CaseC(_) -> () }

let c (myAsync: Async<int>) = async {
    match! myAsync with
    | 42 -> ()
    | CaseA(_) -> () }

let d (myAsyncChild: Async<Async<int>>) = async {
    match! myAsyncChild with
    | 42 -> ()
    match! myAsyncChild with
    | x ->
        match! x with
        | CaseA(_) | CaseB | CaseC(_) -> () }
