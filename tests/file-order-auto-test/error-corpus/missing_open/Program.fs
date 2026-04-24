module ErrorCorpus.MissingOpen.Program

// ERROR: forgot to `open ErrorCorpus.MissingOpen.Lib`
[<EntryPoint>]
let main _argv =
    let s = myFunction ()
    printfn "%s" s
    0
