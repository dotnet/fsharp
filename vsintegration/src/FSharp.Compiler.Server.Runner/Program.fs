open Microsoft.FSharp.Compiler.Server

[<EntryPoint>]
let main argv =
    match argv with
    | [||] -> failwith "Failed to provide unique server name."
    | [|serverName|] -> CompilerServer.Run serverName
    | _ -> failwith "Too many arguments. Specify one argument that is a unique server name."
    0
