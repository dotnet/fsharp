module Miscellaneous.FileIndex

open FSharp.Compiler.Text
open System.Threading.Tasks
open Xunit

// This is a regression test for a bug that existed in FileIndex.fileIndexOfFile
[<Fact>]
let NoRaceCondition() =
    let parallelOptions = ParallelOptions()
    let mutable count = 10000
    while count > 0 do
        let file = $"test{count}.fs"
        let files = Array.create 2 file
        let indices = Array.create 2 -1
        let getFileIndex i = indices[i] <- FileIndex.fileIndexOfFile files[i]
        Parallel.For(0, files.Length, parallelOptions, getFileIndex) |> ignore
        if indices[0] <> indices[1] then
            Assert.Fail $"Found different indices: {indices[0]} and {indices[1]}"
        count <- count - 1
            