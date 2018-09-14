module Test.Test

open System

let ss (x: int) =
    "_" + x.ToString() + "_"

let test1 () =
    ss 1 + ss 2 + ss 3

let test2 () =
    ss 1 + ss 2 + ss 3 + ss 4

let test3 () =
    ss 1 + ss 2 + ss 3 + ss 4 + ss 5

let test4 () =
    ss 5 + ss 6 + ss 7 + String.Concat(ss 8, ss 9) + ss 10 + "_50_" + "_60_" + String.Concat(ss 100, String.Concat(ss 101, ss 102), ss 103) + String.Concat([|"_104_";"_105_"|]) + ss 106

let test5 () =
    ss 5 + ss 6 + ss 7 + String.Concat(ss 8, ss 9) + ss 10 + "_50_" + "_60_" + String.Concat(ss 100, let x = String.Concat(ss 101, ss 102) in Console.WriteLine(x);x, ss 103) + String.Concat([|"_104_";"_105_"|]) + ss 106

[<EntryPoint>]
let main argv =
    0
