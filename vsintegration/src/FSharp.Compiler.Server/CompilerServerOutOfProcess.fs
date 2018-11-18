namespace Microsoft.FSharp.Compiler.Server

open System
open System.IO
open System.Reflection
open System.Diagnostics
open System.Collections.Generic

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices

[<Sealed>]
type internal CompilerServerOutOfProcess(uniqueName) =

    let mutable procOpt : Process option = None
    let mutable isStarted = false
    let mutable restartingHandler = Unchecked.defaultof<_>

    let getAssemblyDirectory (asm: Assembly) =
        Path.GetDirectoryName(asm.Location)

    let startProcess () =
        match procOpt with
        | Some(proc) -> 
            try proc.Kill() with | _ -> ()
            procOpt <- None
        | _ -> ()

        try
            let p = new Process()
            p.StartInfo.UseShellExecute <- true
            p.StartInfo.Arguments <- uniqueName
            p.StartInfo.FileName <- Path.Combine(getAssemblyDirectory (Assembly.GetExecutingAssembly()), "FSharp.Compiler.Server.Runner.exe")

            procOpt <- Some(p)

            p.Start() |> ignore

        with
        | ex ->
            printfn "failed: %s" ex.Message
            reraise()
        

    let ipcClient = new IpcMessageClient<CompilerCommand, CompilerResult>(uniqueName)

    do
        restartingHandler <- ipcClient.Restarting.Subscribe(fun () ->
            startProcess ()
        )

    member __.Start() =
        if not isStarted then
            startProcess ()
            ipcClient.Start ()
            isStarted <- true
        else
            failwith "FSharp Compiler Server Client already started."

    member __.SendCommand<'Result>(cmd: CompilerCommand) : Async<'Result> = async {
        let! result = ipcClient.Send(cmd)
        let result =
            match cmd, result with
            | CompilerCommand.GetSemanticClassification _, Ok(CompilerResult.GetSemanticClassification(result)) -> result :> obj
            | CompilerCommand.GetErrorInfos _, Ok(CompilerResult.GetErrorInfosResult(result)) -> result :> obj
            | _ -> failwith "Bad result"
        return result :?> 'Result
    }

    interface ICompilerServer with

        member this.GetSemanticClassificationAsync(checkerOptions, classifyRange) = async {
            return! this.SendCommand(CompilerCommand.GetSemanticClassification(checkerOptions, classifyRange))
        }

        member this.GetErrorInfosAsync(cmd) = async {
            return! this.SendCommand(CompilerCommand.GetErrorInfos(cmd))
        }

    interface IDisposable with

        member __.Dispose() =
            restartingHandler.Dispose()
            (ipcClient :> IDisposable).Dispose()
            match procOpt with
            | Some(proc) -> try proc.Dispose() with | _ -> ()
            | _ -> ()