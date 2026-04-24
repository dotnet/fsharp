module ErrorCorpus.UndefinedName

[<EntryPoint>]
let main _argv =
    // ERROR: typo on a value name
    let x = nonexistentValue 42
    printfn "%d" x
    0
