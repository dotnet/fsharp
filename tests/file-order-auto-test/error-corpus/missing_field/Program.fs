module ErrorCorpus.MissingField.Program

open ErrorCorpus.MissingField.Types

[<EntryPoint>]
let main _argv =
    // ERROR: accessing nonexistent field
    let p = { Name = "Alice"; Age = 30 }
    printfn "%s" p.Email   // Email doesn't exist
    0
