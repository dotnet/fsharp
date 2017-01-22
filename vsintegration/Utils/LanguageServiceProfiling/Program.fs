open Microsoft.FSharp.Compiler.SourceCodeServices

[<EntryPoint>]
let main argv = 
    let checker = FSharpChecker.Create()
    
    0