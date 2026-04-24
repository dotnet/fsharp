module ErrorCorpus.WrongArity.Program

open ErrorCorpus.WrongArity.Lib

[<EntryPoint>]
let main _argv =
    // ERROR: wrong number of arguments
    let r = add 1 2 3   // add takes 2 args
    printfn "%d" r
    0
