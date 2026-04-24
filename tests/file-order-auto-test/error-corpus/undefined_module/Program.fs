module ErrorCorpus.UndefinedModule

[<EntryPoint>]
let main _argv =
    // ERROR: referencing nonexistent module
    let result = NonexistentModule.someFunction 42
    printfn "%A" result
    0
