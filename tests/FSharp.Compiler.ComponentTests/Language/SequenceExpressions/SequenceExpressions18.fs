module CharRangesTest

let test = seq { 'a'..'c'; 'Z' } |> Seq.toArray
let expected = [| 'a'; 'b'; 'c'; 'Z' |]

if test <> expected then failwith $"test failed: got {test}"

printfn "All list tests passed!"
