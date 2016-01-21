namespace FSharp.Compiler.Unittests

[<AutoOpen>]
module CompilerTestHelpers =

    let (|Warning|_|) (exn: System.Exception) =
        match exn with
        | :? Microsoft.FSharp.Compiler.ErrorLogger.Error as e -> let n,d = e.Data0 in Some (n,d)
        | :? Microsoft.FSharp.Compiler.ErrorLogger.NumberedError as e -> let n,d = e.Data0 in Some (n,d)
        | _ -> None
