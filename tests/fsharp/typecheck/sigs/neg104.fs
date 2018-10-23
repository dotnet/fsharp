/// match! - parsing errors
module M

type AT = Async<int>

module A =
    let a (x: AT) =
        match! x with _ -> ()
    let b (x: int) =
        match! x with _ -> ()    

module B =
    let a (x: AT) = async {
        match! }

    let b (x: AT) = async {
        match! x }

    let c (x: AT) = async {
        match! x with }

    let d (x: AT) = async {
        match! x with | }

    let e (x: AT) = async {
        match! x with | x' }

    let f (x: AT) = async {
        match! x with | x' -> }

module C =
    let a (x: AT) = async {
        let! match! = x
        let! x = match!
        return () }