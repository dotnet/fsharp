module Neg135

let a () = async {
    while! async { return true }
        ()
}